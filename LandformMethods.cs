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
using System.Drawing.Imaging;


namespace RealmStudio
{
    internal class LandformMethods
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

            Task cpt1 = Task.Run(() => landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Below));
            Task cpt2 = Task.Run(() => landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Below));
            Task cpt3 = Task.Run(() => landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Below));
            Task cpt4 = Task.Run(() => landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelEnum.Below));
            Task cpt5 = Task.Run(() => landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelEnum.Below));
            Task cpt6 = Task.Run(() => landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelEnum.Below));
            Task cpt7 = Task.Run(() => landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelEnum.Below));
            Task cpt8 = Task.Run(() => landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelEnum.Below));

            Task cpt9 = Task.Run(() => landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Above));
            Task cpt10 = Task.Run(() => landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Above));
            Task cpt11 = Task.Run(() => landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Above));
            Task cpt12 = Task.Run(() => landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelEnum.Above));
            Task cpt13 = Task.Run(() => landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelEnum.Above));
            Task cpt14 = Task.Run(() => landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelEnum.Above));
            Task cpt15 = Task.Run(() => landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelEnum.Above));
            Task cpt16 = Task.Run(() => landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelEnum.Above));

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

            landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, pathDistance, ParallelEnum.Below);
            landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 2 * pathDistance, ParallelEnum.Below);
            landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 3 * pathDistance, ParallelEnum.Below);
            landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 4 * pathDistance, ParallelEnum.Below);
            landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 5 * pathDistance, ParallelEnum.Below);
            landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 6 * pathDistance, ParallelEnum.Below);
            landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 7 * pathDistance, ParallelEnum.Below);
            landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 8 * pathDistance, ParallelEnum.Below);

            landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, pathDistance, ParallelEnum.Above);
            landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 2 * pathDistance, ParallelEnum.Above);
            landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 3 * pathDistance, ParallelEnum.Above);
            landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 4 * pathDistance, ParallelEnum.Above);
            landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 5 * pathDistance, ParallelEnum.Above);
            landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 6 * pathDistance, ParallelEnum.Above);
            landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 7 * pathDistance, ParallelEnum.Above);
            landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 8 * pathDistance, ParallelEnum.Above);
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

        internal static List<SKPath> GenerateRandomLandformPaths(SKPoint location, SKSize size, GeneratedLandformTypeEnum selectedLandformType, bool flipVertical = false)
        {
            List<SKPath> generatedLandformPaths = [];

            Bitmap? landformBitmap = ShapeGenerator.GetNoiseGeneratedLandformShape((int)size.Width, (int)size.Height, selectedLandformType, flipVertical);

            if (landformBitmap == null) { return generatedLandformPaths; }

            // fill any holes in the bitmap
            Bitmap filledBitmap = DrawingMethods.FillHoles(landformBitmap);

            // extract all blobs over set size - arbitrarily set to 1/16 the input size
            List<Bitmap> generatedBitmaps = DrawingMethods.ExtractBlobs(filledBitmap, new Size((int)(size.Width / 16), (int)(size.Height / 16)));

            for (int i = 0; i < generatedBitmaps.Count; i++)
            {
                Bitmap landBitmap = generatedBitmaps[i];

                // the mean filter smooths out pixels that differ a lot in color from their neighbors;
                // when applied to a monochrome image like the generated landform bitmaps, the filter
                // will average out black or white pixels that are surrounded (or mostly surrounded)
                // by pixels of the opposite color. When the bitmap colors are then flattened back to
                // monochrome (black and white), this has the effect of removing "spikes" from around the
                // edges of the landform (and other anomalies) that cause the Moore neighborhood algorithm
                // to fail, causing the landform boundaries not to be computed correctly
                Mean meanFilter = new();
                meanFilter.ApplyInPlace(landBitmap);
                meanFilter.ApplyInPlace(landBitmap);

                DrawingMethods.FlattenBitmapColors(ref landBitmap);

                // fill any holes in the bitmap
                Bitmap filledLandBitmap = DrawingMethods.FillHoles(landBitmap);

                using Graphics g = Graphics.FromImage(filledLandBitmap);
                using Pen p = new(Color.White, 3);

                g.DrawLine(p, new Point(2, 2), new Point(filledLandBitmap.Width - 2, 2));
                g.DrawLine(p, new Point(2, filledLandBitmap.Height - 2), new Point(filledLandBitmap.Width - 2, filledLandBitmap.Height - 2));
                g.DrawLine(p, new Point(2, 2), new Point(2, filledLandBitmap.Height - 2));
                g.DrawLine(p, new Point(filledLandBitmap.Width - 2, 2), new Point(filledLandBitmap.Width - 2, filledLandBitmap.Height - 2));

                // run Moore meighborhood algorithm to get the perimeter path
                List<SKPoint> contourPoints = DrawingMethods.GetBitmapContourPoints(filledLandBitmap);

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

                    if (selectedLandformType != GeneratedLandformTypeEnum.Icecap)
                    {
                        landformPath.Transform(SKMatrix.CreateTranslation(location.X - (size.Width / 2.0F), location.Y - (size.Height / 2.0F)));
                    }
                    else
                    {
                        landformPath.Transform(SKMatrix.CreateTranslation(location.X - (size.Width / 2.0F), location.Y));
                    }

                    generatedLandformPaths.Add(new(landformPath));
                }
            }

            return generatedLandformPaths;
        }

        internal static SKPath TraceImage(string fileName)
        {
            Bitmap b = (Bitmap)Bitmap.FromFile(fileName);

            int bmpWidth = b.Width;
            int bmpHeight = b.Height;

            // find color surrounding image
            Color topLeftColor = b.GetPixel(0, 0);
            Color topRightColor = b.GetPixel(bmpWidth - 1, 0);
            Color bottomLeftColor = b.GetPixel(0, bmpHeight - 1);
            Color bottomRightColor = b.GetPixel(bmpWidth - 1, bmpHeight - 1);

            Color surroundColor = Color.White;
            bool surroundColorFound = false;

            if (topLeftColor == topRightColor && bottomLeftColor == bottomRightColor && topLeftColor == bottomRightColor)
            {
                surroundColorFound = true;
                surroundColor = topLeftColor;
            }

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

            //DrawingMethods.FlattenBitmapColors(ref gsb);

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
            //DrawingMethods.FlattenBitmapColors(ref invertedBitmap);

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

            // make sure the bitmap has a 3-pixel white border so that the Moore-neighborhood algorithm doesn't fail
            // due to image pixels being right at the edges of the bitmap
            //using Graphics g1 = Graphics.FromImage(invertedBitmap);
            //using Pen p1 = new(Color.White, 3);

            //g1.DrawLine(p1, new Point(2, 2), new Point(invertedBitmap.Width - 2, 2));
            //g1.DrawLine(p1, new Point(2, invertedBitmap.Height - 2), new Point(invertedBitmap.Width - 2, invertedBitmap.Height - 2));
            //g1.DrawLine(p1, new Point(2, 2), new Point(2, invertedBitmap.Height - 2));
            //g1.DrawLine(p1, new Point(invertedBitmap.Width - 2, 2), new Point(invertedBitmap.Width - 2, invertedBitmap.Height - 2));

            DrawingMethods.FlattenBitmapColors(ref invertedBitmap);

            invertedBitmap.Save("C:\\Users\\Pete Nelson\\OneDrive\\Desktop\\inverted.bmp");

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
    }
}
