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
    }
}
