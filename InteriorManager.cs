/**************************************************************************************************************************
* Copyright 2026, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using AForge.Imaging.Filters;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;
using Windows.Devices.Geolocation;


namespace RealmStudio
{
    internal sealed class InteriorManager : IMapComponentManager
    {
        public static SKPath InteriorFloorErasePath { get; set; } = new SKPath();

        private static InteriorUIMediator? _interiorUIMediator;
        private Color _interiorFloorPaintColor = Color.FromArgb(128, 230, 208, 171);

        internal static InteriorUIMediator? InteriorMediator
        {
            get { return _interiorUIMediator; }
            set { _interiorUIMediator = value; }
        }

        internal Color InteriorFloorPaintColor
        {
            get { return _interiorFloorPaintColor; }
            set { _interiorFloorPaintColor = value; }
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? Create()
        {
            ArgumentNullException.ThrowIfNull(InteriorMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            InteriorFloor newFloor = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
                Width = MapStateMediator.CurrentMap.MapWidth,
                Height = MapStateMediator.CurrentMap.MapHeight,
                InteriorFloorTexture = InteriorMediator.InteriorFloorTextureList[InteriorMediator.InteriorFloorTextureIndex],
                IsModified = true,

                InteriorFloorRenderSurface =
                    SKSurface.Create(MapStateMediator.MainUIMediator.MainForm.SKGLRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight)),

                InteriorOutlineRenderSurface =
                    SKSurface.Create(MapStateMediator.MainUIMediator.MainForm.SKGLRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
            };

            BuildFloorTextureAndShaders(newFloor);

            return newFloor;
        }

        public static IMapComponent? CreateWall()
        {
            ArgumentNullException.ThrowIfNull(InteriorMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            InteriorWall newWall = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
                WallBackgroundColor = InteriorMediator.WallBackgroundColor,
                WallOutlineColor = InteriorMediator.WallOutlineColor,
                WallWidth = InteriorMediator.WallThickness,
                WallTexture = InteriorMediator.InteriorWallTextureList[InteriorMediator.InteriorWallTextureIndex],
                WallTextureScale = InteriorMediator.WallTextureScale,
            };

            newWall.WallPoints.Add(new LinePoint(MapStateMediator.CurrentCursorPoint));

            ConstructWallPaint(newWall);

            return newWall;
        }

        public static bool Update()
        {
            return true;
        }

        public static bool Delete()
        {
            return true;
        }

        internal static void CreateAllPathsFromDrawnPath(RealmStudioMap map, InteriorFloor floor)
        {
            if (map == null || floor == null) return;

            floor.ContourPath = DrawingMethods.GetContourPathFromPath(floor.DrawPath,
                map.MapWidth, map.MapHeight, out List<SKPoint> contourPoints);

            floor.ContourPoints = contourPoints;

            int pathDistance = 2;

            Task cpt1 = Task.Run(() => floor.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelDirection.Below));

            Task cpt2 = Task.Run(() => floor.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelDirection.Above));


            Task.WaitAll(cpt1, cpt2);

            floor.IsModified = true;
        }

        internal static void CreateInnerAndOuterPathsFromContourPoints(InteriorFloor floor)
        {
            if (MapStateMediator.CurrentMap == null || floor == null) return;

            if (floor.ContourPoints.Count == 0)
            {
                floor.ContourPoints = [.. floor.ContourPath.Points];
            }

            int pathDistance = 2;

            floor.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(floor.ContourPoints, pathDistance, ParallelDirection.Below);
            floor.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(floor.ContourPoints, pathDistance, ParallelDirection.Above);

        }

        internal static void EraseInteriorFloor(InteriorFloor floor)
        {
            if (InteriorFloorErasePath.PointCount > 0)
            {
                using SKPath diffPath = floor.DrawPath.Op(InteriorFloorErasePath, SKPathOp.Difference);

                if (diffPath != null)
                {
                    floor.DrawPath = new(diffPath);
                    floor.IsModified = true;
                }

                InteriorFloorErasePath.Reset();
            }
        }

        internal static void MergeInteriorFloors(RealmStudioMap map)
        {
            MapLayer interiorFloorLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.INTERIORLAYER);

            List<Guid> mergedInteriorFloorGuids = [];

            // merge overlapping interior floors
            for (int i = 0; i < interiorFloorLayer.MapLayerComponents.Count; i++)
            {
                for (int j = 0; j < interiorFloorLayer.MapLayerComponents.Count; j++)
                {
                    if (i != j
                        && interiorFloorLayer.MapLayerComponents[i] is InteriorFloor floor_i
                        && interiorFloorLayer.MapLayerComponents[j] is InteriorFloor floor_j
                        && !mergedInteriorFloorGuids.Contains(((InteriorFloor)interiorFloorLayer.MapLayerComponents[i]).InteriorFloorGuid)
                        && !mergedInteriorFloorGuids.Contains(((InteriorFloor)interiorFloorLayer.MapLayerComponents[j]).InteriorFloorGuid))
                    {
                        floor_i.IsModified = true;
                        floor_j.IsModified = true;

                        SKPath floorPath1 = floor_i.ContourPath;
                        SKPath floorPath2 = floor_j.ContourPath;

                        floorPath1.GetTightBounds(out SKRect lpb1);
                        floorPath2.GetTightBounds(out SKRect lpb2);

                        if (lpb1.IntersectsWith(lpb2))
                        {
                            bool pathsMerged = MergeInteriorFloorPaths(floorPath2, ref floorPath1);

                            if (pathsMerged)
                            {
                                floor_i.DrawPath = new(floorPath1);
                                CreateAllPathsFromDrawnPath(map, floor_i);

                                interiorFloorLayer.MapLayerComponents[i] = floor_i;
                                mergedInteriorFloorGuids.Add(floor_j.InteriorFloorGuid);
                            }
                        }
                    }
                }
            }

            for (int k = interiorFloorLayer.MapLayerComponents.Count - 1; k >= 0; k--)
            {
                if (interiorFloorLayer.MapLayerComponents[k] is InteriorFloor f
                    && mergedInteriorFloorGuids.Contains(((InteriorFloor)interiorFloorLayer.MapLayerComponents[k]).InteriorFloorGuid))
                {
                    f.InteriorFloorRenderSurface?.Dispose();
                    f.InteriorOutlineRenderSurface?.Dispose();
                    interiorFloorLayer.MapLayerComponents.RemoveAt(k);
                }
            }
        }

        private static bool MergeInteriorFloorPaths(SKPath interiorFloorPath2, ref SKPath interiorFloorPath1)
        {
            // merge paths from two landforms; if the paths overlap, then landformPath1
            // is modified to include landformPath2 (landformPath1 becomes the union
            // of the two original paths)
            bool pathsMerged = false;

            if (interiorFloorPath2.PointCount > 0 && interiorFloorPath1.PointCount > 0)
            {
                // get the intersection between the paths
                SKPath intersectionPath = interiorFloorPath1.Op(interiorFloorPath2, SKPathOp.Intersect);

                // if the intersection path isn't null or empty, then merge the paths
                if (intersectionPath != null && intersectionPath.PointCount > 0)
                {
                    // calculate the union between the paths
                    SKPath unionPath = interiorFloorPath1.Op(interiorFloorPath2, SKPathOp.Union);

                    if (unionPath != null && unionPath.PointCount > 0)
                    {
                        pathsMerged = true;
                        interiorFloorPath1.Dispose();
                        interiorFloorPath1 = new SKPath(unionPath)
                        {
                            FillType = SKPathFillType.Winding
                        };

                        unionPath.Dispose();
                    }
                }
            }

            return pathsMerged;
        }

        public static void FillMapWithInteriorFloor(SKGLControl glControl)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            MapLayer interiorFloorLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER);

            if (interiorFloorLayer.MapLayerComponents.Count > 0)
            {
                MessageBox.Show("Interior floor areas have already been drawn. Please clear them before filling the map.", "Interior Floor Already Drawn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                InteriorFloor? floor = (InteriorFloor?)Create();

                if (floor != null)
                {
                    Cmd_FillMapWithInteriorFloor cmd = new(MapStateMediator.CurrentMap, floor);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    MapStateMediator.CurrentMap.IsSaved = false;
                    glControl.Invalidate();
                }
            }
        }

        internal static void BuildFloorTextureAndShaders(InteriorFloor floor)
        {
            ArgumentNullException.ThrowIfNull(InteriorMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (floor.InteriorFloorTexture != null)
            {
                if (floor.InteriorFloorTexture.TextureBitmap != null)
                {
                    Bitmap resizedBitmap = new(floor.InteriorFloorTexture.TextureBitmap, MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                    // create and set a shader from the texture
                    SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                        SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                    floor.InteriorFloorFillPaint.Shader = flpShader;
                }
                else
                {
                    Bitmap b = new(floor.InteriorFloorTexture.TexturePath);
                    Bitmap resizedBitmap = new(b, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                    floor.InteriorFloorTexture.TextureBitmap = new(resizedBitmap);

                    // create and set a shader from the texture
                    SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                        SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                    floor.InteriorFloorFillPaint.Shader = flpShader;
                }
            }
            else
            {
                // texture is not set in the interior floor object, so use default
                if (InteriorMediator.InteriorFloorTextureList[InteriorMediator.InteriorFloorTextureIndex].TextureBitmap == null)
                {
                    InteriorMediator.InteriorFloorTextureList[InteriorMediator.InteriorFloorTextureIndex].TextureBitmap =
                        (Bitmap?)Bitmap.FromFile(InteriorMediator.InteriorFloorTextureList[InteriorMediator.InteriorFloorTextureIndex].TexturePath);
                }

                floor.InteriorFloorTexture = InteriorMediator.InteriorFloorTextureList[InteriorMediator.InteriorFloorTextureIndex];

                if (floor.InteriorFloorTexture.TextureBitmap != null)
                {
                    Bitmap resizedBitmap = new(floor.InteriorFloorTexture.TextureBitmap, MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                    // create and set a shader from the texture
                    SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                        SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                    floor.InteriorFloorFillPaint.Shader = flpShader;
                }
            }

            if (!floor.FillWithTexture || floor.InteriorFloorTexture == null || floor.InteriorFloorTexture.TextureBitmap == null)
            {
                floor.InteriorFloorFillPaint.Shader?.Dispose();
                floor.InteriorFloorFillPaint.Shader = null;

                SKShader flpShader = SKShader.CreateColor(floor.InteriorFloorBackgroundColor.ToSKColor());
                floor.InteriorFloorFillPaint.Shader = flpShader;
            }

            MapTexture? dashTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Watercolor Dashes");

            if (dashTexture != null)
            {
                dashTexture.TextureBitmap ??= new Bitmap(dashTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(dashTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                floor.DashShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            MapTexture? lineHatchTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Line Hatch");

            if (lineHatchTexture != null)
            {
                lineHatchTexture.TextureBitmap ??= new Bitmap(lineHatchTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(lineHatchTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                floor.LineHatchBitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }
        }

        internal static void FinalizeInteriorFloors(SKGLControl glControl)
        {
            ArgumentNullException.ThrowIfNull(InteriorMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            // finalize loading of interior floors
            MapLayer interiorFloorLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER);
            SKImageInfo lfImageInfo = new(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

            for (int i = 0; i < interiorFloorLayer.MapLayerComponents.Count; i++)
            {
                if (interiorFloorLayer.MapLayerComponents[i] is InteriorFloor floor)
                {
                    floor.ParentMap = MapStateMediator.CurrentMap;
                    floor.IsModified = true;

                    floor.InteriorFloorRenderSurface ??= SKSurface.Create(glControl.GRContext, false, lfImageInfo);
                    floor.InteriorOutlineRenderSurface ??= SKSurface.Create(glControl.GRContext, false, lfImageInfo);

                    CreateInnerAndOuterPathsFromContourPoints(floor);

                    floor.ContourPath.GetBounds(out SKRect boundsRect);
                    floor.Width = (int)boundsRect.Width;
                    floor.Height = (int)boundsRect.Height;

                    BuildFloorTextureAndShaders(floor);
                }
            }

            // finalize loading of interior floor drawing layer
            MapLayer interiorFloorDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORDRAWINGLAYER);

            for (int i = 0; i < interiorFloorDrawingLayer.MapLayerComponents.Count; i++)
            {
                if (interiorFloorDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                {
                    paintStroke.ParentMap = MapStateMediator.CurrentMap;
                    paintStroke.RenderSurface = SKSurface.Create(glControl.GRContext, false, lfImageInfo);
                    paintStroke.Rendered = false;

                    if (paintStroke.Erase)
                    {
                        paintStroke.ShaderPaint = PaintObjects.InteriorFloorColorEraserPaint;
                    }
                    else
                    {
                        paintStroke.ShaderPaint = PaintObjects.InteriorFloorColorPaint;
                    }
                }
            }
        }

        internal static InteriorFloor? CreateNewInteriorFloor(RealmStudioMap map, SKPath? floorPath, SKRect realmArea)
        {
            InteriorFloor? floor = (InteriorFloor?)Create();

            if (floor != null)
            {
                if (floorPath != null)
                {
                    floor.DrawPath = new(floorPath);
                }

                if (realmArea != SKRect.Empty)
                {
                    floor.X = (int)realmArea.Left;
                    floor.Y = (int)realmArea.Top;
                }
                else
                {
                    floor.X = 0;
                    floor.Y = 0;
                }

                CreateAllPathsFromDrawnPath(map, floor);

                floor.ContourPath.GetBounds(out SKRect boundsRect);

                floor.Width = (int)boundsRect.Width;
                floor.Height = (int)boundsRect.Height;

                return floor;
            }

            return null;
        }

        internal static void RemoveLayerPaintStrokesFromLandformLayer()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);

            for (int i = landformLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (landformLayer.MapLayerComponents[i] is LayerPaintStroke)
                {
                    landformLayer.MapLayerComponents.RemoveAt(i);
                }
            }
        }

        internal static InteriorFloor? GetInteriorFloorIntersectingCircle(SKPoint mapPoint, int circleRadius)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            using SKPath circlePath = new();
            circlePath.AddCircle(mapPoint.X, mapPoint.Y, circleRadius);

            List<MapComponent> floorComponents = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER).MapLayerComponents;

            for (int i = 0; i < floorComponents.Count; i++)
            {
                if (floorComponents[i] is InteriorFloor mapInteriorFloor)
                {
                    if (mapInteriorFloor.DrawPath != null && mapInteriorFloor.DrawPath.PointCount > 0)
                    {
                        if (!mapInteriorFloor.DrawPath.Op(circlePath, SKPathOp.Intersect).IsEmpty)
                        {
                            return mapInteriorFloor;
                        }
                    }
                }
            }

            return null;
        }

        internal static void EraseFromLandform(RealmStudioMap map, SKPoint zoomedScrolledPoint, int brushRadius)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            InteriorFloor? erasedFloor = GetInteriorFloorIntersectingCircle(zoomedScrolledPoint, brushRadius);

            if (erasedFloor != null)
            {
                EraseInteriorFloor(erasedFloor);
                CreateAllPathsFromDrawnPath(map, erasedFloor);

                erasedFloor.IsModified = true;

                if (erasedFloor.ContourPath.PointCount == 0)
                {
                    // the interior floor has been totally erased, so mark it deleted
                    MarkInteriorFloorDeleted(erasedFloor);
                }

                MergeInteriorFloors(MapStateMediator.CurrentMap);
            }
        }

        private static void MarkInteriorFloorDeleted(InteriorFloor deletedFloor)
        {
            deletedFloor.IsDeleted = true;
        }

        internal static void RemoveDeletedInteriorFloors()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            // TODO: what should be done about water features and other objects drawn on top of the floor?
            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER).MapLayerComponents[i] is InteriorFloor f)
                {
                    if (f.IsDeleted)
                    {
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER).MapLayerComponents.RemoveAt(i);
                    }
                }
            }
        }

        internal static Landform? SelectInteriorFloorAtPoint(SKPoint mapClickPoint)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            Landform? selectedLandform = null;

            List<MapComponent> landformComponents = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformComponents.Count; i++)
            {
                if (landformComponents[i] is Landform mapLandform)
                {
                    SKPath boundaryPath = mapLandform.DrawPath;

                    if (boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            mapLandform.IsSelected = true;
                            selectedLandform = mapLandform;
                            break;
                        }
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedLandform);
            return selectedLandform;
        }

        internal static void MoveInteriorFloor(RealmStudioMap map, InteriorFloor floor, SKPoint zoomedScrolledPoint, SKPoint previousCursorPoint)
        {
            // move the entire selected interior floor with the mouse

            float deltaX = zoomedScrolledPoint.X - previousCursorPoint.X;
            float deltaY = zoomedScrolledPoint.Y - previousCursorPoint.Y;

            floor.ContourPath.GetTightBounds(out SKRect boundsRect);

            SKPoint p = new()
            {
                X = deltaX - (int)(boundsRect.MidX - previousCursorPoint.X),
                Y = deltaY - (int)(boundsRect.MidY - previousCursorPoint.Y)
            };

            SKMatrix tm = SKMatrix.CreateTranslation(p.X, p.Y);

            floor.DrawPath.Transform(tm);
            CreateAllPathsFromDrawnPath(map, floor);

            floor.X += (int)deltaX;
            floor.Y += (int)deltaY;
        }

        internal static void ClearAllInteriorFloors()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            DialogResult confirmResult = MessageBox.Show("This action will clear all interior floor drawing and any drawn floors.\nPlease confirm.", "Clear All?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (confirmResult != DialogResult.OK)
            {
                return;
            }

            MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            Cmd_ClearAllInteriorFloors cmd = new(MapStateMediator.CurrentMap);

            CommandManager.AddCommand(cmd);
            cmd.DoOperation();
        }

        internal static void StartColorPainting(TimerManager applicationTimerManager, SKGLControl glRenderControl)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(InteriorMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.CurrentLayerPaintStroke == null)
            {
                MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap,
                    InteriorMediator.InteriorPaintColor.ToSKColor(),
                    MapStateMediator.SelectedColorPaintBrush,
                    MapStateMediator.MainUIMediator.SelectedBrushSize / 2,
                    MapBuilder.LANDDRAWINGLAYER)
                {
                    RenderSurface = SKSurface.Create(glRenderControl.GRContext, false,
                    new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                };

                MapBrush? brush = null;
                if (InteriorMediator.InteriorPaintBrush == ColorPaintBrush.PatternBrush1)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush1");
                }
                else if (InteriorMediator.InteriorPaintBrush == ColorPaintBrush.PatternBrush2)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush2");
                }
                else if (InteriorMediator.InteriorPaintBrush == ColorPaintBrush.PatternBrush3)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush3");
                }
                else if (InteriorMediator.InteriorPaintBrush == ColorPaintBrush.PatternBrush4)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush4"); ;
                }

                if (brush != null)
                {
                    brush.BrushBitmap = (Bitmap)Bitmap.FromFile(brush.BrushPath);
                    MapStateMediator.CurrentLayerPaintStroke.StrokeBrush = brush;
                }

                Cmd_AddInteriorFloorPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                applicationTimerManager.BrushTimerEnabled = true;
            }
        }

        internal static void StartColorErasing(SKGLControl glRenderControl)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.CurrentLayerPaintStroke == null)
            {
                MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, SKColors.Empty,
                    ColorPaintBrush.HardBrush, MapStateMediator.MainUIMediator.SelectedBrushSize / 2, MapBuilder.LANDDRAWINGLAYER, true)
                {
                    RenderSurface = SKSurface.Create(glRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                };

                Cmd_AddLandPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static void StopColorPainting(TimerManager applicationTimerManager)
        {
            applicationTimerManager.BrushTimerEnabled = false;

            if (MapStateMediator.CurrentLayerPaintStroke != null)
            {
                MapStateMediator.CurrentLayerPaintStroke = null;
            }
        }

        internal static void StopColorErasing()
        {
            MapStateMediator.CurrentLayerPaintStroke = null;
        }

        internal static List<LinePoint> GetParallelPathPoints(List<LinePoint> points, float distance, ParallelDirection location)
        {
            List<LinePoint> parallelPoints = [];

            //float d = (location == ParallelDirection.Below) ? distance : -distance;
            float offsetAngle = (location == ParallelDirection.Above) ? 90 : -90;

            SKPoint maxXPoint = new(-1.0F, 0);
            SKPoint maxYPoint = new(0, -1.0F);

            SKPoint minXPoint = new(65535.0F, 0);
            SKPoint minYPoint = new(0, 65535.0F);

            for (int i = 0; i < points.Count - 1; i++)
            {
                SKPoint newPoint;

                if (i == 0)
                {
                    float lineAngle = DrawingMethods.CalculateAngleBetweenPoints(points[i].LineSegmentPoint, points[i + 1].LineSegmentPoint);
                    float circleRadius = (float)Math.Sqrt(distance * distance + distance * distance);
                    newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle + offsetAngle, points[i].LineSegmentPoint);
                    parallelPoints.Add(new LinePoint(newPoint));

                    if (newPoint.X > maxXPoint.X)
                    {
                        maxXPoint = newPoint;
                    }

                    if (newPoint.Y < minYPoint.Y)
                    {
                        minYPoint = newPoint;
                    }

                    if (newPoint.X < minXPoint.X)
                    {
                        minXPoint = newPoint;
                    }

                    if (newPoint.Y > maxYPoint.Y)
                    {
                        maxYPoint = newPoint;
                    }
                }
                else
                {
                    float lineAngle1 = DrawingMethods.CalculateAngleBetweenPoints(points[i - 1].LineSegmentPoint, points[i].LineSegmentPoint);
                    float lineAngle2 = DrawingMethods.CalculateAngleBetweenPoints(points[i].LineSegmentPoint, points[i + 1].LineSegmentPoint);

                    float angleDifference = (float)((lineAngle2 - lineAngle1 >= 0) ? Math.Round(lineAngle2 - lineAngle1) : Math.Round((lineAngle2 - lineAngle1) + 360.0F));
                    float circleRadius = (float)Math.Sqrt(distance * distance + distance * distance);

                    if (angleDifference == 0.0F)
                    {
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference > 0.0F && angleDifference < 90.0F)
                    {
                        //1
                        float circleAngle = lineAngle1 + offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 90.0F)
                    {
                        // edge case - do not add a point if the lines are 90 degrees from each other
                    }
                    else if (angleDifference > 90.0F && angleDifference < 180.0F)
                    {
                        //2
                        float circleAngle = lineAngle1 + offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 180.0F) //====================================
                    {
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));
                    }
                    else if (angleDifference > 180.0F && angleDifference < 270.0F)
                    {
                        //3
                        float circleAngle = lineAngle1 - offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 270.0F)
                    {
                        // edge case - do not add a point if the lines are 270 degrees from each other
                    }
                    else if (angleDifference > 270.0F && angleDifference < 360.0F)
                    {
                        //4
                        float circleAngle = lineAngle1 - offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 360.0F)
                    {
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].LineSegmentPoint);
                        parallelPoints.Add(new LinePoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                }
            }

            // remove points that cause loops in the parallel path when
            // the path has a sharp turn in it; this is done by finding the
            // point where the lines in the path intersect, then removing
            // the points that are outside the lines (X value is greater
            // than the intersection point X value and Y value is less than
            // the intersection point Y value)

            // this algorithm doesn't work consistently, so it is
            // commented out; need to figure out a reliable algorithm
            // for removing the loops, maybe by taking into account the
            // "direction" of the line?
            //
            // it may be that looking at whether the original path line (set
            // of points) is above/below or left/right of the parallel line
            // (set of points) will give the information needed to know
            // if loops need to be culled or not; however, how to know which
            // points should be culled still has to be determined

            /*
            bool pointsRemoved = false;

            if (parallelPoints.Count > 3)
            {
                // get the intersection between the lines
                // Line AB represented as a1x + b1y = c1
                // Line CD represented as a2x + b2y = c2 
                // A is parallelPoints[0].MapPoint
                // B is maxXPoint
                // C is minYPoint
                // D is parallelPoints.Last().MapPoint

                double a1 = maxXPoint.Y - parallelPoints[0].MapPoint.Y;
                double b1 = parallelPoints[0].MapPoint.X - maxXPoint.X;
                double c1 = a1 * (parallelPoints[0].MapPoint.X) + b1 * (parallelPoints[0].MapPoint.Y);

                // Line CD represented as a2x + b2y = c2 
                double a2 = parallelPoints.Last().MapPoint.Y - minYPoint.Y;
                double b2 = minYPoint.X - parallelPoints.Last().MapPoint.X;
                double c2 = a2 * (minYPoint.X) + b2 * (minYPoint.Y);

                double determinant = a1 * b2 - a2 * b1;

                SKPoint intersectionPoint = SKPoint.Empty;

                if (determinant != 0)
                {
                    double x = (b2 * c1 - b1 * c2) / determinant;
                    double y = (a1 * c2 - a2 * c1) / determinant;

                    intersectionPoint = new SKPoint((float)x, (float)y);
                }

                if (!intersectionPoint.IsEmpty)
                {
                    for (int i = parallelPoints.Count - 1; i >= 0; i--)
                    {
                        if (parallelPoints[i].MapPoint.X > intersectionPoint.X
                            && parallelPoints[i].MapPoint.Y <= intersectionPoint.Y)
                        {
                            parallelPoints.RemoveAt(i);
                            pointsRemoved = true;
                        }
                    }
                }
            }

            if (parallelPoints.Count > 3 && !pointsRemoved)
            {
                // A is parallelPoints[0].MapPoint
                // B is minYPoint
                // C is minXPoint
                // D is parallelPoints.Last().MapPoint

                double a1 = minYPoint.Y - parallelPoints[0].MapPoint.Y;
                double b1 = parallelPoints[0].MapPoint.X - minYPoint.X;
                double c1 = a1 * (parallelPoints[0].MapPoint.X) + b1 * (parallelPoints[0].MapPoint.Y);

                // Line CD represented as a2x + b2y = c2 
                double a2 = parallelPoints.Last().MapPoint.Y - minXPoint.Y;
                double b2 = minXPoint.X - parallelPoints.Last().MapPoint.X;
                double c2 = a2 * (minXPoint.X) + b2 * (minXPoint.Y);

                double determinant = a1 * b2 - a2 * b1;

                SKPoint intersectionPoint = SKPoint.Empty;

                if (determinant != 0)
                {
                    double x = (b2 * c1 - b1 * c2) / determinant;
                    double y = (a1 * c2 - a2 * c1) / determinant;

                    intersectionPoint = new SKPoint((float)x, (float)y);
                }

                if (!intersectionPoint.IsEmpty)
                {
                    for (int i = parallelPoints.Count - 1; i >= 0; i--)
                    {
                        if (parallelPoints[i].MapPoint.X < intersectionPoint.X
                            && parallelPoints[i].MapPoint.Y <= intersectionPoint.Y)
                        {
                            parallelPoints.RemoveAt(i);
                        }
                    }
                }
            }
            */

            return parallelPoints;
        }

        internal static void DrawBezierCurvesFromPoints(SKCanvas canvas, List<LinePoint> curvePoints, SKPaint paint)
        {
            if (curvePoints.Count > 2)
            {
                using SKPath path = new();
                path.MoveTo(curvePoints[0].LineSegmentPoint);

                for (int j = 0; j < curvePoints.Count - 2; j += 3)
                {
                    path.CubicTo(curvePoints[j].LineSegmentPoint, curvePoints[j + 1].LineSegmentPoint, curvePoints[j + 2].LineSegmentPoint);
                }

                canvas.DrawPath(path, paint);
            }
        }

        internal static void DrawLineFromPoints(SKCanvas canvas, List<LinePoint> linePoints, SKPaint paint)
        {
            if (linePoints.Count > 0)
            {
                using SKPath path = new();
                path.MoveTo(linePoints[0].LineSegmentPoint);

                for (int j = 1; j < linePoints.Count - 1; j++)
                {
                    path.LineTo(linePoints[j].LineSegmentPoint);
                }

                canvas.DrawPath(path, paint);
            }
        }

        internal static void AddNewWallPoint(InteriorWall? wall, SKPoint newWallPoint)
        {
            // make the spacing between points consistent
            float spacing = 4.0F;

            if (wall != null)
            {
                if (SKPoint.Distance(wall.WallPoints.Last().LineSegmentPoint, newWallPoint) >= wall.WallWidth / spacing)
                {
                    wall.WallPoints.Add(new LinePoint(newWallPoint));
                }
            }
        }

        internal static SKPoint GetNewWallPoint(InteriorWall? wall, Keys modifierKeys, float selectedWallAngle, SKPoint zoomedScrolledPoint, int minimumWallPointCount)
        {
            SKPoint newWallPoint = zoomedScrolledPoint;
            LinePoint? firstPoint = wall?.WallPoints.First();

            if (modifierKeys == Keys.Shift && wall?.WallPoints.Count > minimumWallPointCount)
            {
                // draw straight path, clamped to 5 degree angles
                if (firstPoint != null)
                {
                    if (selectedWallAngle == -1)
                    {
                        selectedWallAngle = DrawingMethods.Get5DegreePathAngle(firstPoint.LineSegmentPoint, zoomedScrolledPoint);
                    }

                    float distance = SKPoint.Distance(firstPoint.LineSegmentPoint, zoomedScrolledPoint);
                    newWallPoint = DrawingMethods.PointOnCircle(distance, selectedWallAngle, firstPoint.LineSegmentPoint);
                }
            }
            else if (modifierKeys == Keys.Control)
            {
                // draw straight horizontal or vertical path
                if (firstPoint != null)
                {
                    if (selectedWallAngle == -1)
                    {
                        selectedWallAngle = DrawingMethods.CalculateAngleBetweenPoints(firstPoint.LineSegmentPoint, zoomedScrolledPoint, true); ;
                    }

                    // clamp the line to straight horizontal or straight vertical
                    // by forcing the new point X or Y coordinate to be the
                    // same as the first point of the path
                    newWallPoint = DrawingMethods.ForceHorizontalVerticalLine(newWallPoint, firstPoint.LineSegmentPoint, selectedWallAngle);
                }
            }

            return newWallPoint;
        }

        public static SKPath GenerateWallBoundaryPath(List<LinePoint> points)
        {
            SKPath path = new();

            if (points.Count < 3) return path;

            path.MoveTo(points[0].LineSegmentPoint);

            for (int j = 0; j < points.Count - 2; j += 3)
            {
                path.CubicTo(points[j].LineSegmentPoint, points[j + 1].LineSegmentPoint , points[j + 2].LineSegmentPoint);
            }

            path.GetBounds(out SKRect pathBounds);

            if (pathBounds.Width < 5.0F)
            {
                pathBounds.Inflate(5.0F, 0);
            }

            if (pathBounds.Height < 5.0F)
            {
                pathBounds.Inflate(0, 5.0F);
            }

            path.Reset();
            path.AddRect(pathBounds, SKPathDirection.Clockwise);

            return path;
        }

        public static void ConstructWallPaint(InteriorWall wall)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (wall.WallPaint != null) return;

            float strokeWidth = wall.WallWidth;

            switch (wall.WallType)
            {
                case PathType.ThickSolidLinePath:
                    strokeWidth = wall.WallWidth * 1.5F;
                    break;
            }

            SKPaint wallPaint = new()
            {
                Color = Extensions.ToSKColor(wall.WallBackgroundColor),
                StrokeWidth = strokeWidth,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Butt,
                StrokeJoin = SKStrokeJoin.Bevel,
                StrokeMiter = 1.0F,
                IsAntialias = true,
            };

            // load the texture bitmap if needed
            if (wall.WallTexture != null && !string.IsNullOrEmpty(wall.WallTexture.TexturePath) && wall.WallTexture.TextureBitmap == null)
            {
                wall.WallTexture.TextureBitmap = (Bitmap?)Bitmap.FromFile(wall.WallTexture.TexturePath);
            }

            switch (wall.WallType)
            {
                case PathType.LineAndDashesPath:
                case PathType.BorderedGradientPath:
                case PathType.BorderedLightSolidPath:
                    wallPaint.StrokeCap = SKStrokeCap.Butt;
                    break;
                case PathType.TexturedPath:
                    if (wall.WallTexture != null && wall.WallTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(wall.WallTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * wall.WallTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * wall.WallTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, wall.WallTextureOpacity / 255.0F);

                        // construct a shader from the selected wall texture
                        wallPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    wallPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathType.BorderAndTexturePath:
                    wallPaint.StrokeCap = SKStrokeCap.Round;

                    if (wall.WallTexture != null && wall.WallTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(wall.WallTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * wall.WallTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * wall.WallTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, wall.WallTextureOpacity / 255.0F);

                        // construct a shader from the selected wall texture
                        wallPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    wallPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathType.RoundTowerWall:
                    wallPaint.StrokeCap = SKStrokeCap.Round;

                    if (wall.WallTexture != null && wall.WallTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(wall.WallTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * wall.WallTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * wall.WallTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, wall.WallTextureOpacity / 255.0F);

                        // construct a shader from the selected wall texture
                        wallPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    wallPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathType.SquareTowerWall:
                    wallPaint.StrokeCap = SKStrokeCap.Round;

                    if (wall.WallTexture != null && wall.WallTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(wall.WallTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * wall.WallTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * wall.WallTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, wall.WallTextureOpacity / 255.0F);

                        // construct a shader from the selected wall texture
                        wallPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    wallPaint.Style = SKPaintStyle.Stroke;
                    break;
            }

            wall.WallPaint = wallPaint;
        }
    }
}
