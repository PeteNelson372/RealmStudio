/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
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
    internal sealed class LandformMethods
    {
        public static SKPath LandformErasePath { get; set; } = new SKPath();
        public static int LandformBrushSize { get; set; } = 64;
        public static int LandformEraserSize { get; set; } = 64;
        public static int LandformColorBrushSize { get; set; } = 20;
        public static int LandformColorEraserBrushSize { get; set; } = 20;

        internal static void CreateAllPathsFromDrawnPath(RealmStudioMap map, Landform landform)
        {
            if (map == null || landform == null) return;

            landform.ContourPath = DrawingMethods.GetContourPathFromPath(landform.DrawPath,
                map.MapWidth, map.MapHeight, out List<SKPoint> contourPoints);

            landform.ContourPoints = contourPoints;

            int pathDistance = landform.CoastlineEffectDistance / 8;

            Task cpt1 = Task.Run(() => landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelDirection.Below));
            Task cpt2 = Task.Run(() => landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelDirection.Below));
            Task cpt3 = Task.Run(() => landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelDirection.Below));
            Task cpt4 = Task.Run(() => landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelDirection.Below));
            Task cpt5 = Task.Run(() => landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelDirection.Below));
            Task cpt6 = Task.Run(() => landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelDirection.Below));
            Task cpt7 = Task.Run(() => landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelDirection.Below));
            Task cpt8 = Task.Run(() => landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelDirection.Below));

            Task cpt9 = Task.Run(() => landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelDirection.Above));
            Task cpt10 = Task.Run(() => landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelDirection.Above));
            Task cpt11 = Task.Run(() => landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelDirection.Above));
            Task cpt12 = Task.Run(() => landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelDirection.Above));
            Task cpt13 = Task.Run(() => landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelDirection.Above));
            Task cpt14 = Task.Run(() => landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelDirection.Above));
            Task cpt15 = Task.Run(() => landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelDirection.Above));
            Task cpt16 = Task.Run(() => landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelDirection.Above));

            Task.WaitAll(cpt1, cpt2, cpt3, cpt4, cpt4, cpt6, cpt7, cpt8, cpt9, cpt10, cpt11, cpt12, cpt13, cpt14, cpt15, cpt16);

            landform.IsModified = true;
        }

        internal static void CreateInnerAndOuterPathsFromContourPoints(RealmStudioMap map, Landform landform)
        {
            if (map == null || landform == null) return;

            if (landform.ContourPoints.Count == 0)
            {
                landform.ContourPoints = [.. landform.ContourPath.Points];
            }

            int pathDistance = landform.CoastlineEffectDistance / 8;

            landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, pathDistance, ParallelDirection.Below);
            landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 2 * pathDistance, ParallelDirection.Below);
            landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 3 * pathDistance, ParallelDirection.Below);
            landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 4 * pathDistance, ParallelDirection.Below);
            landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 5 * pathDistance, ParallelDirection.Below);
            landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 6 * pathDistance, ParallelDirection.Below);
            landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 7 * pathDistance, ParallelDirection.Below);
            landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 8 * pathDistance, ParallelDirection.Below);

            landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, pathDistance, ParallelDirection.Above);
            landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 2 * pathDistance, ParallelDirection.Above);
            landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 3 * pathDistance, ParallelDirection.Above);
            landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 4 * pathDistance, ParallelDirection.Above);
            landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 5 * pathDistance, ParallelDirection.Above);
            landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 6 * pathDistance, ParallelDirection.Above);
            landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 7 * pathDistance, ParallelDirection.Above);
            landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 8 * pathDistance, ParallelDirection.Above);
        }

        internal static void EraseLandForm(Landform lf)
        {
            if (LandformErasePath.PointCount > 0)
            {
                using SKPath diffPath = lf.DrawPath.Op(LandformErasePath, SKPathOp.Difference);

                if (diffPath != null)
                {
                    lf.DrawPath = new(diffPath);
                    lf.IsModified = true;
                }

                LandformErasePath.Reset();
            }
        }

        internal static void MergeLandforms(RealmStudioMap map)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            List<Guid> mergedLandformGuids = [];

            // merge overlapping landforms
            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                for (int j = 0; j < landformLayer.MapLayerComponents.Count; j++)
                {
                    if (i != j
                        && landformLayer.MapLayerComponents[i] is Landform landform_i
                        && landformLayer.MapLayerComponents[j] is Landform landform_j
                        && !mergedLandformGuids.Contains(((Landform)landformLayer.MapLayerComponents[i]).LandformGuid)
                        && !mergedLandformGuids.Contains(((Landform)landformLayer.MapLayerComponents[j]).LandformGuid))
                    {
                        landform_i.IsModified = true;
                        landform_j.IsModified = true;

                        SKPath landformPath1 = landform_i.ContourPath;
                        SKPath landformPath2 = landform_j.ContourPath;

                        landformPath1.GetTightBounds(out SKRect lpb1);
                        landformPath2.GetTightBounds(out SKRect lpb2);

                        if (lpb1.IntersectsWith(lpb2))
                        {
                            bool pathsMerged = MergeLandformPaths(landformPath2, ref landformPath1);

                            if (pathsMerged)
                            {
                                landform_i.DrawPath = new(landformPath1);
                                CreateAllPathsFromDrawnPath(map, landform_i);

                                landformLayer.MapLayerComponents[i] = landform_i;
                                mergedLandformGuids.Add(landform_j.LandformGuid);
                            }
                        }
                    }
                }
            }

            for (int k = landformLayer.MapLayerComponents.Count - 1; k >= 0; k--)
            {
                if (landformLayer.MapLayerComponents[k] is Landform lf
                    && mergedLandformGuids.Contains(((Landform)landformLayer.MapLayerComponents[k]).LandformGuid))
                {
                    lf.LandformRenderSurface?.Dispose();
                    lf.CoastlineRenderSurface?.Dispose();
                    landformLayer.MapLayerComponents.RemoveAt(k);
                }
            }
        }

        private static bool MergeLandformPaths(SKPath landformPath2, ref SKPath landformPath1)
        {
            // merge paths from two landforms; if the paths overlap, then landformPath1
            // is modified to include landformPath2 (landformPath1 becomes the union
            // of the two original paths)
            bool pathsMerged = false;

            if (landformPath2.PointCount > 0 && landformPath1.PointCount > 0)
            {
                // get the intersection between the paths
                SKPath intersectionPath = landformPath1.Op(landformPath2, SKPathOp.Intersect);

                // if the intersection path isn't null or empty, then merge the paths
                if (intersectionPath != null && intersectionPath.PointCount > 0)
                {
                    // calculate the union between the paths
                    SKPath unionPath = landformPath1.Op(landformPath2, SKPathOp.Union);

                    if (unionPath != null && unionPath.PointCount > 0)
                    {
                        pathsMerged = true;
                        landformPath1.Dispose();
                        landformPath1 = new SKPath(unionPath)
                        {
                            FillType = SKPathFillType.Winding
                        };

                        unionPath.Dispose();
                    }
                }
            }

            return pathsMerged;
        }

        public static void FillMapWithLandForm(RealmStudioMap map, Landform landform)
        {
            Cmd_FillMapWithLandform cmd = new(map, landform);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();
        }

        internal static SKPath TraceImage(string fileName)
        {
            Bitmap b = (Bitmap)Bitmap.FromFile(fileName);

            // make sure the bitmap has a 3-pixel white border so that the Moore-neighborhood algorithm doesn't fail
            // due to image pixels being right at the edges of the bitmap
            using Graphics g = Graphics.FromImage(b);
            using Pen p = new(Color.White, 3);

            g.DrawLine(p, new Point(2, 2), new Point(b.Width - 2, 2));
            g.DrawLine(p, new Point(2, b.Height - 2), new Point(b.Width - 2, b.Height - 2));
            g.DrawLine(p, new Point(2, 2), new Point(2, b.Height - 2));
            g.DrawLine(p, new Point(b.Width - 2, 2), new Point(b.Width - 2, b.Height - 2));

            // convert to monochrome
            Bitmap gsb = DrawingMethods.MakeGrayscale(b, 0.0F, true);

            if (gsb.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // convert the bitmap to an 8bpp grayscale image for processing
                Bitmap newB = Grayscale.CommonAlgorithms.BT709.Apply(gsb);
                gsb = newB;
            }

            Median medianfilter = new();
            // apply the filter
            medianfilter.ApplyInPlace(gsb);

            // find edges
            // create filter
            SobelEdgeDetector filter = new();

            // apply the filter
            filter.ApplyInPlace(gsb);

            // remove background
            gsb.MakeTransparent(Color.Black);

            if (gsb.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // convert the bitmap to an 8bpp grayscale image for processing
                Bitmap newB = Grayscale.CommonAlgorithms.BT709.Apply(gsb);
                gsb = newB;
            }

            Invert invert = new();
            Bitmap invertedBitmap = invert.Apply(gsb);

            // flatten colors again
            // convert to monochrome
            Bitmap gsbInverted = DrawingMethods.MakeGrayscale(invertedBitmap, 0.5F, true);

            Mean meanFilter = new();
            meanFilter.ApplyInPlace(invertedBitmap);
            meanFilter.ApplyInPlace(invertedBitmap);

            DrawingMethods.FlattenBitmapColors(ref invertedBitmap);

            /*
             * Tesseract OCR code - not needed for now
             * 
            string engtraineddatapath = AppDomain.CurrentDomain.BaseDirectory + "Resources";

            if (Directory.Exists(engtraineddatapath))
            {
                using (var engine = new TesseractEngine(engtraineddatapath, "eng", EngineMode.Default))
                {
                    using Pix pixBmp = PixConverter.ToPix(invertedBitmap);
                    //using Pix pixBmp = Pix.LoadFromFile(fileName);

                    // engine created; ocr the inverted grayscale bitmap

                    using (var page = engine.Process(pixBmp))
                    {
                        string text = page.GetTsvText(0);
                        MessageBox.Show(text);
                    }
                }
            }
            */

            DrawingMethods.FlattenBitmapColors(ref invertedBitmap);

            // run Moore meighborhood algorithm to get the perimeter path
            List<SKPoint> contourPoints = DrawingMethods.GetBitmapContourPoints(invertedBitmap);

            SKPath landformPath = new();
            if (contourPoints.Count > 4)   // require at least 4 points in the contour to be added as a landform
            {
                // the Moore-Neighbor algorithm sets the first (0th) pixel in the list of contour points to
                // an empty pixel, so remove it before constructing the path from the contour points
                contourPoints.RemoveAt(0);

                landformPath.MoveTo(contourPoints[0]);

                for (int j = 1; j < contourPoints.Count; j++)
                {
                    landformPath.LineTo(contourPoints[j]);
                }

                if (landformPath[0] != landformPath[contourPoints.Count - 1])
                {
                    landformPath.Close();
                }

            }

            return landformPath;
        }

        internal static void FinalizeLandforms(RealmStudioMap map, SKGLControl glControl)
        {
            // finalize loading of landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            SKImageInfo lfImageInfo = new(map.MapWidth, map.MapHeight);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform landform)
                {
                    landform.ParentMap = map;
                    landform.IsModified = true;

                    landform.LandformRenderSurface ??= SKSurface.Create(glControl.GRContext, false, lfImageInfo);
                    landform.CoastlineRenderSurface ??= SKSurface.Create(glControl.GRContext, false, lfImageInfo);

                    CreateInnerAndOuterPathsFromContourPoints(map, landform);

                    landform.ContourPath.GetBounds(out SKRect boundsRect);
                    landform.Width = (int)boundsRect.Width;
                    landform.Height = (int)boundsRect.Height;

                    if (landform.LandformTexture != null)
                    {
                        if (landform.LandformTexture.TextureBitmap != null)
                        {
                            Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, map.MapWidth, map.MapHeight);

                            // create and set a shader from the texture
                            SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                                SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                            landform.LandformFillPaint.Shader = flpShader;
                        }
                        else
                        {
                            Bitmap b = new(landform.LandformTexture.TexturePath);
                            Bitmap resizedBitmap = new(b, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                            landform.LandformTexture.TextureBitmap = new(resizedBitmap);

                            // create and set a shader from the texture
                            SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                                SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                            landform.LandformFillPaint.Shader = flpShader;
                        }
                    }
                    else
                    {
                        // texture is not set in the landform object, so use default
                        if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
                        {
                            AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
                        }

                        landform.LandformTexture = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX];

                        if (landform.LandformTexture.TextureBitmap != null)
                        {
                            Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, map.MapWidth, map.MapHeight);

                            // create and set a shader from the texture
                            SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                                SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                            landform.LandformFillPaint.Shader = flpShader;
                        }
                    }

                    if (!landform.FillWithTexture || landform.LandformTexture == null || landform.LandformTexture.TextureBitmap == null)
                    {
                        landform.LandformFillPaint.Shader?.Dispose();
                        landform.LandformFillPaint.Shader = null;

                        SKShader flpShader = SKShader.CreateColor(landform.LandformBackgroundColor.ToSKColor());
                        landform.LandformFillPaint.Shader = flpShader;
                    }

                    MapTexture? dashTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Watercolor Dashes");

                    if (dashTexture != null)
                    {
                        dashTexture.TextureBitmap ??= new Bitmap(dashTexture.TexturePath);

                        SKBitmap resizedSKBitmap = new(100, 100);

                        Extensions.ToSKBitmap(dashTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                        landform.DashShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    MapTexture? lineHatchTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Line Hatch");

                    if (lineHatchTexture != null)
                    {
                        lineHatchTexture.TextureBitmap ??= new Bitmap(lineHatchTexture.TexturePath);

                        SKBitmap resizedSKBitmap = new(100, 100);

                        Extensions.ToSKBitmap(lineHatchTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                        landform.LineHatchBitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }
                }
            }

            // finalize loading of land drawing layer
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);

            for (int i = 0; i < landDrawingLayer.MapLayerComponents.Count; i++)
            {
                if (landDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                {
                    paintStroke.ParentMap = map;
                    paintStroke.RenderSurface = SKSurface.Create(glControl.GRContext, false, lfImageInfo);
                    paintStroke.Rendered = false;

                    if (paintStroke.Erase)
                    {
                        paintStroke.ShaderPaint = PaintObjects.LandColorEraserPaint;
                    }
                    else
                    {
                        paintStroke.ShaderPaint = PaintObjects.LandColorPaint;
                    }
                }
            }
        }

        internal static Landform CreateNewLandform(RealmStudioMap map, SKPath? landformPath, SKRect realmArea, bool fillWithTexture, SKGLControl glControl)
        {
            Landform landform = new()
            {
                ParentMap = map,
                Width = map.MapWidth,
                Height = map.MapHeight,
                IsModified = true,
                FillWithTexture = fillWithTexture,
                LandformRenderSurface = SKSurface.Create(glControl.GRContext, false,
                    new SKImageInfo(map.MapWidth, map.MapHeight)),
                CoastlineRenderSurface = SKSurface.Create(glControl.GRContext, false,
                    new SKImageInfo(map.MapWidth, map.MapHeight))
            };

            if (landformPath != null)
            {
                landform.DrawPath = new(landformPath);
            }

            if (realmArea != SKRect.Empty)
            {
                landform.X = (int)realmArea.Left;
                landform.Y = (int)realmArea.Top;
            }
            else
            {
                landform.X = 0;
                landform.Y = 0;
            }

            CreateAllPathsFromDrawnPath(map, landform);

            landform.ContourPath.GetBounds(out SKRect boundsRect);

            landform.Width = (int)boundsRect.Width;
            landform.Height = (int)boundsRect.Height;

            return landform;
        }

        internal static void RemoveLayerPaintStrokesFromLandformLayer(RealmStudioMap map)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            for (int i = landformLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (landformLayer.MapLayerComponents[i] is LayerPaintStroke)
                {
                    landformLayer.MapLayerComponents.RemoveAt(i);
                }
            }
        }

        internal static Landform? GetLandformIntersectingCircle(RealmStudioMap map, SKPoint mapPoint, int circleRadius)
        {
            using SKPath circlePath = new();
            circlePath.AddCircle(mapPoint.X, mapPoint.Y, circleRadius);

            List<MapComponent> landformComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformComponents.Count; i++)
            {
                if (landformComponents[i] is Landform mapLandform)
                {
                    if (mapLandform.DrawPath != null && mapLandform.DrawPath.PointCount > 0)
                    {
                        if (!mapLandform.DrawPath.Op(circlePath, SKPathOp.Intersect).IsEmpty)
                        {
                            return mapLandform;
                        }
                    }
                }
            }

            return null;
        }

        internal static void EraseFromLandform(RealmStudioMap map, SKPoint zoomedScrolledPoint, int brushRadius)
        {
            Landform? erasedLandform = GetLandformIntersectingCircle(map, zoomedScrolledPoint, brushRadius);

            if (erasedLandform != null)
            {
                EraseLandForm(erasedLandform);
                CreateAllPathsFromDrawnPath(map, erasedLandform);

                erasedLandform.IsModified = true;

                if (erasedLandform.ContourPath.PointCount == 0)
                {
                    // the landform has been totally erased, so mark it deleted
                    MarkLandformDeleted(erasedLandform);
                }

                MergeLandforms(map);
            }
        }

        private static void MarkLandformDeleted(Landform deletedLandform)
        {
            deletedLandform.IsDeleted = true;
        }

        internal static void RemoveDeletedLandforms(RealmStudioMap map)
        {
            // TODO: what should be done about water features and other objects drawn on top of the landform?
            for (int i = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents[i] is Landform l)
                {
                    if (l.IsDeleted)
                    {
                        MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents.RemoveAt(i);
                    }
                }
            }
        }

        internal static Landform? SelectLandformAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            Landform? selectedLandform = null;

            List<MapComponent> landformComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents;

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

            RealmMapMethods.DeselectAllMapComponents(map, selectedLandform);
            return selectedLandform;
        }

        internal static void MoveLandform(Landform landform, SKPoint zoomedScrolledPoint, SKPoint previousCursorPoint)
        {
            // move the entire selected landform with the mouse

            float deltaX = zoomedScrolledPoint.X - previousCursorPoint.X;
            float deltaY = zoomedScrolledPoint.Y - previousCursorPoint.Y;

            landform.ContourPath.GetTightBounds(out SKRect boundsRect);

            SKPoint p = new()
            {
                X = deltaX - (int)(boundsRect.MidX - previousCursorPoint.X),
                Y = deltaY - (int)(boundsRect.MidY - previousCursorPoint.Y)
            };

            SKMatrix tm = SKMatrix.CreateTranslation(p.X, p.Y);

            landform.DrawPath.Transform(tm);
            CreateAllPathsFromDrawnPath(landform.ParentMap, landform);

            landform.X += (int)deltaX;
            landform.Y += (int)deltaY;
        }
    }
}
