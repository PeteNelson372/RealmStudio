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
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal class WaterFeatureMethods
    {
        public static SKPath WaterFeaturErasePath { get; set; } = new();

        public static int WaterFeatureBrushSize { get; set; } = 20;
        public static int WaterFeatureEraserSize { get; set; } = 20;

        public static int WaterColorBrushSize { get; set; } = 20;
        public static int WaterColorEraserSize { get; set; } = 20;

        public static Color DEFAULT_WATER_OUTLINE_COLOR { get; } = ColorTranslator.FromHtml("#A19076");
        public static Color DEFAULT_WATER_COLOR { get; } = ColorTranslator.FromHtml("#658CBFC5");

        internal static void MergeWaterFeatures(RealmStudioMap map)
        {
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            List<Guid> mergedWaterFeatureGuids = [];

            // merge overlapping water features
            for (int i = 0; i < waterLayer.MapLayerComponents.Count; i++)
            {
                for (int j = 0; j < waterLayer.MapLayerComponents.Count; j++)
                {
                    if (i != j
                        && waterLayer.MapLayerComponents[i] is WaterFeature waterFeature_i
                        && waterLayer.MapLayerComponents[j] is WaterFeature waterFeature_j
                        && !mergedWaterFeatureGuids.Contains(((WaterFeature)waterLayer.MapLayerComponents[i]).WaterFeatureGuid)
                        && !mergedWaterFeatureGuids.Contains(((WaterFeature)waterLayer.MapLayerComponents[j]).WaterFeatureGuid))
                    {
                        SKPath waterFeaturePath1 = waterFeature_i.WaterFeaturePath;
                        SKPath waterFeaturePath2 = waterFeature_j.WaterFeaturePath;

                        bool pathsMerged = MergeWaterFeaturePaths(waterFeaturePath2, ref waterFeaturePath1);

                        if (pathsMerged)
                        {
                            waterFeature_i.WaterFeaturePath = new(waterFeaturePath1);
                            CreateInnerAndOuterPaths(map, waterFeature_i);

                            waterFeature_i.WaterFeatureColor = waterFeature_j.WaterFeatureColor;
                            waterFeature_i.WaterFeatureShorelineColor = waterFeature_j.WaterFeatureShorelineColor;

                            if (waterFeature_i.WaterFeatureType == WaterFeatureTypeEnum.Lake || waterFeature_j.WaterFeatureType == WaterFeatureTypeEnum.Lake)
                            {
                                waterFeature_i.WaterFeatureType = WaterFeatureTypeEnum.Lake;
                            }

                            ConstructWaterFeaturePaintObjects(waterFeature_i);

                            waterLayer.MapLayerComponents[i] = waterFeature_i;

                            mergedWaterFeatureGuids.Add(waterFeature_j.WaterFeatureGuid);
                        }
                    }
                }
            }

            for (int k = waterLayer.MapLayerComponents.Count - 1; k >= 0; k--)
            {
                if (waterLayer.MapLayerComponents[k] is WaterFeature feature
                    && mergedWaterFeatureGuids.Contains(feature.WaterFeatureGuid))
                {
                    waterLayer.MapLayerComponents.RemoveAt(k);
                }
            }
        }

        internal static bool MergeWaterFeaturePaths(SKPath waterFeaturePath2, ref SKPath waterFeaturePath1)
        {
            // merge paths from two water features; if the paths overlap, then waterFeaturePath1
            // is modified to include waterFeaturePath2 (the first path becomes the union
            // of the two original paths)
            bool pathsMerged = false;

            if (waterFeaturePath2.PointCount > 0 && waterFeaturePath1.PointCount > 0)
            {
                // get the intersection between the paths
                SKPath intersectionPath = waterFeaturePath1.Op(waterFeaturePath2, SKPathOp.Intersect);

                // if the intersection path isn't null or empty, then merge the paths
                if (intersectionPath != null && intersectionPath.PointCount > 0)
                {
                    // calculate the union between the water feature paths
                    SKPath unionPath = waterFeaturePath1.Op(waterFeaturePath2, SKPathOp.Union);

                    if (unionPath != null && unionPath.PointCount > 0)
                    {
                        pathsMerged = true;
                        waterFeaturePath1.Dispose();
                        waterFeaturePath1 = new SKPath(unionPath)
                        {
                            FillType = SKPathFillType.Winding
                        };

                        unionPath.Dispose();
                    }
                }
            }

            return pathsMerged;
        }

        internal static void CreateInnerAndOuterPaths(RealmStudioMap map, WaterFeature waterFeature)
        {
            if (map == null || waterFeature == null) return;

            waterFeature.ContourPath = DrawingMethods.GetContourPathFromPath(waterFeature.WaterFeaturePath,
                map.MapWidth, map.MapHeight, out List<SKPoint> contourPoints);

            waterFeature.ContourPoints = contourPoints;

            int pathDistance = waterFeature.ShorelineEffectDistance / 3;

            waterFeature.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Below);
            waterFeature.InnerPath1.Close();

            waterFeature.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Below);
            waterFeature.InnerPath2.Close();

            waterFeature.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Below);
            waterFeature.InnerPath3.Close();

            waterFeature.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Above);
            waterFeature.OuterPath1.Close();

            waterFeature.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Above);
            waterFeature.OuterPath2.Close();

            waterFeature.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Above);
            waterFeature.OuterPath3.Close();

        }

        internal static void ConstructWaterFeaturePaintObjects(WaterFeature waterFeature)
        {
            SKShader colorShader = SKShader.CreateColor(Extensions.ToSKColor(waterFeature.WaterFeatureColor));

            // TODO: should water have a texture applied?
            //SKBitmap shaderBitmap = Extensions.ToSKBitmap(MapDrawingMethods.GetNoisyBitmap(MapPaintMethods.WATER_BRUSH_SIZE, MapPaintMethods.WATER_BRUSH_SIZE));
            //SKShader bitmapShader = SKShader.CreateBitmap(shaderBitmap);
            //SKShader combinedShader = SKShader.CreateCompose(colorShader, bitmapShader, SKBlendMode.SrcOver);

            int pathDistance = waterFeature.ShorelineEffectDistance / 3;

            waterFeature.WaterFeatureBackgroundPaint?.Dispose();
            waterFeature.WaterFeatureBackgroundPaint = new()
            {
                Style = SKPaintStyle.Fill,
                Shader = colorShader,
                BlendMode = SKBlendMode.Src,
                Color = Extensions.ToSKColor(waterFeature.WaterFeatureColor),
                IsAntialias = true,
            };

            waterFeature.WaterFeatureShorelinePaint?.Dispose();
            waterFeature.WaterFeatureShorelinePaint = new()
            {
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcATop,
                Color = Extensions.ToSKColor(waterFeature.WaterFeatureShorelineColor),
                StrokeWidth = pathDistance,
                IsAntialias = true,
            };

            waterFeature.ShallowWaterPaint?.Dispose();
            waterFeature.ShallowWaterPaint = new()
            {
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcATop,
                StrokeWidth = pathDistance,
                IsAntialias = true
            };

            if (waterFeature.WaterFeatureType == WaterFeatureTypeEnum.Lake)
            {
                waterFeature.WaterFeatureShorelinePaint.PathEffect = SKPathEffect.CreateCorner(100);
                waterFeature.ShallowWaterPaint.PathEffect = SKPathEffect.CreateCorner(100);
                waterFeature.WaterFeatureBackgroundPaint.PathEffect = SKPathEffect.CreateCorner(100);
            }
        }

        public static SKPath? GenerateRandomLakePath(SKPoint location, float lakeSize)
        {
            SKPath lakePath = new();

            // generate a bitmap using Simplex noise; the bitmap returned is 500x500 pixels
            // so it will need to be scaled to lakeSize before generating the path
            Bitmap noiseGeneratedlakeBitmap = ShapeGenerator.GetNoiseGeneratedLakeShape();

            // fill any holes in the bitmap
            Bitmap filledBitmap = DrawingMethods.FillHoles(noiseGeneratedlakeBitmap);

            // extract the largest blob from the bitmap; this will be the lake shape
            Bitmap? unscaledLakeBitmap = DrawingMethods.ExtractLargestBlob(filledBitmap);

            if (unscaledLakeBitmap != null)
            {
                // scale the bitmap
                Bitmap scaledLakeBitmap = new(unscaledLakeBitmap, new Size((int)lakeSize, (int)lakeSize));

                DrawingMethods.FlattenBitmapColors(ref scaledLakeBitmap);

                using Graphics g = Graphics.FromImage(scaledLakeBitmap);
                using Pen p = new(Color.White, 3);

                g.DrawLine(p, new Point(2, 2), new Point(scaledLakeBitmap.Width - 2, 2));
                g.DrawLine(p, new Point(2, scaledLakeBitmap.Height - 2), new Point(scaledLakeBitmap.Width - 2, scaledLakeBitmap.Height - 2));
                g.DrawLine(p, new Point(2, 2), new Point(2, scaledLakeBitmap.Height - 2));
                g.DrawLine(p, new Point(scaledLakeBitmap.Width - 2, 2), new Point(scaledLakeBitmap.Width - 2, scaledLakeBitmap.Height - 2));

                // run Moore meighborhood algorithm to get the perimeter path
                List<SKPoint> contourPoints = DrawingMethods.GetBitmapContourPoints(scaledLakeBitmap);

                if (contourPoints.Count > 2)
                {
                    // the Moore-Neighbor algorithm sets the first (0th) pixel in the list of contour points to
                    // an empty pixel, so remove it before constructing the path from the contour points
                    contourPoints.RemoveAt(0);

                    lakePath.MoveTo(contourPoints[0]);

                    for (int i = 1; i < contourPoints.Count; i++)
                    {
                        lakePath.LineTo(contourPoints[i]);
                    }

                    lakePath.Close();
                }

                lakePath.Transform(SKMatrix.CreateTranslation(location.X - (lakeSize / 2.0F), location.Y - (lakeSize / 2.0F)));
            }

            return lakePath;
        }

        internal static List<MapRiverPoint> GetParallelRiverPoints(List<MapRiverPoint> points, float distance, ParallelEnum location, bool fromStartingPoint)
        {
            List<MapRiverPoint> parallelPoints = [];

            float offsetAngle = (location == ParallelEnum.Above) ? 90 : -90;

            SKPoint maxXPoint = new(-1.0F, 0);
            SKPoint maxYPoint = new(0, -1.0F);

            SKPoint minXPoint = new(65535.0F, 0);
            SKPoint minYPoint = new(0, 65535.0F);

            for (int i = 0; i < points.Count - 1; i++)
            {
                SKPoint newPoint;

                float circleRadius = (float)Math.Sqrt(distance * distance + distance * distance);

                if (fromStartingPoint)
                {
                    circleRadius = distance * (i / (float)points.Count);
                }

                if (i == 0)
                {
                    float lineAngle = DrawingMethods.CalculateLineAngle(points[i].RiverPoint, points[i + 1].RiverPoint);
                    newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle + offsetAngle, points[i].RiverPoint);
                    parallelPoints.Add(new MapRiverPoint(newPoint));

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
                    float lineAngle1 = DrawingMethods.CalculateLineAngle(points[i - 1].RiverPoint, points[i].RiverPoint);
                    float lineAngle2 = DrawingMethods.CalculateLineAngle(points[i].RiverPoint, points[i + 1].RiverPoint);

                    float angleDifference = (float)((lineAngle2 - lineAngle1 >= 0) ? Math.Round(lineAngle2 - lineAngle1) : Math.Round((lineAngle2 - lineAngle1) + 360.0F));
                    if (angleDifference == 0.0F)
                    {
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));

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
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));

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
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));

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
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));
                    }
                    else if (angleDifference > 180.0F && angleDifference < 270.0F)
                    {
                        //3
                        float circleAngle = lineAngle1 - offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));

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
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));

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
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].RiverPoint);
                        parallelPoints.Add(new MapRiverPoint(newPoint));

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


            return parallelPoints;
        }

        internal static void ConstructRiverPaintObjects(River mapRiver)
        {
            float strokeWidth = mapRiver.RiverWidth / 2;

            SKShader colorShader = SKShader.CreateColor(Extensions.ToSKColor(mapRiver.RiverColor));

            MapTexture? riverTexture = AssetManager.WATER_TEXTURE_LIST.Find(x => x.TextureName == "Gray Texture");

            SKShader combinedShader;

            if (riverTexture != null && mapRiver.RenderRiverTexture)
            {
                riverTexture.TextureBitmap ??= Image.FromFile(riverTexture.TexturePath) as Bitmap;

                SKBitmap bitmap = Extensions.ToSKBitmap(riverTexture.TextureBitmap);
                SKBitmap resizedSKBitmap = new((int)mapRiver.RiverWidth, (int)mapRiver.RiverWidth);

                bitmap.ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                SKShader bitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                combinedShader = SKShader.CreateCompose(colorShader, bitmapShader, SKBlendMode.Modulate);
            }
            else
            {
                combinedShader = colorShader;
            }

            int pathDistance = (int)mapRiver.RiverWidth;

            SKColor riverColor = Extensions.ToSKColor(Color.FromArgb(mapRiver.RiverColor.A, mapRiver.RiverColor));

            mapRiver.RiverFillPaint = new()
            {
                Color = riverColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                BlendMode = SKBlendMode.Src,
                Shader = combinedShader
            };

            mapRiver.RiverShorelinePaint = new()
            {
                StrokeWidth = mapRiver.RiverWidth / 4,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.Src,
                StrokeJoin = SKStrokeJoin.Round,
                StrokeCap = SKStrokeCap.Butt,
                Color = Extensions.ToSKColor(mapRiver.RiverShorelineColor),
                IsAntialias = true,
            };

            // shallow water is a lighter shade of river color
            SKColor shallowWaterColor = Extensions.ToSKColor(Color.FromArgb(mapRiver.RiverShorelineColor.A / 4, mapRiver.RiverColor));

            mapRiver.RiverShallowWaterPaint = new()
            {
                Color = shallowWaterColor,
                StrokeWidth = mapRiver.RiverWidth / 8,
                BlendMode = SKBlendMode.Src,
                Style = SKPaintStyle.Stroke,
                StrokeJoin = SKStrokeJoin.Round,
                StrokeCap = SKStrokeCap.Butt,
                IsAntialias = true,
            };

        }

        internal static void EraseWaterFeature(RealmStudioMap map)
        {
            if (WaterFeaturErasePath.PointCount > 0)
            {
                MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

                foreach (IWaterFeature iwf in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (iwf is WaterFeature wf)
                    {
                        using SKPath diffPath = wf.WaterFeaturePath.Op(WaterFeaturErasePath, SKPathOp.Difference);

                        if (diffPath != null)
                        {
                            wf.WaterFeaturePath = new(diffPath);

                            Task.Run(() => CreateInnerAndOuterPaths(map, wf));
                        }
                    }

                }

                WaterFeaturErasePath.Reset();
            }
        }

        internal static void ConstructRiverPaths(River river)
        {
            List<MapRiverPoint> distinctRiverPoints = river.RiverPoints.Distinct(new RiverPointComparer()).ToList();

            if (distinctRiverPoints.Count < 3)
            {
                return;
            }

            using SKPath riverPath = new();

            if (distinctRiverPoints.Count > 2)
            {
                riverPath.MoveTo(distinctRiverPoints[0].RiverPoint);

                for (int j = 0; j < distinctRiverPoints.Count; j += 3)
                {
                    if (j < distinctRiverPoints.Count - 2)
                    {
                        riverPath.CubicTo(distinctRiverPoints[j].RiverPoint, distinctRiverPoints[j + 1].RiverPoint, distinctRiverPoints[j + 2].RiverPoint);
                    }
                }
            }

            river.RiverPath?.Dispose();
            river.RiverPath = new(riverPath);

            List<MapRiverPoint> abovePoints = GetParallelRiverPoints(distinctRiverPoints, river.RiverWidth / 2.0F, ParallelEnum.Above, river.RiverSourceFadeIn);
            List<MapRiverPoint> belowPoints = GetParallelRiverPoints(distinctRiverPoints, river.RiverWidth / 2.0F, ParallelEnum.Below, river.RiverSourceFadeIn);

            using SKPath riverBoundaryPath = new();

            if (abovePoints.Count > 2)
            {
                riverBoundaryPath.MoveTo(abovePoints[0].RiverPoint);

                for (int j = 0; j < abovePoints.Count; j += 3)
                {
                    if (j < abovePoints.Count - 2)
                    {
                        riverBoundaryPath.CubicTo(abovePoints[j].RiverPoint, abovePoints[j + 1].RiverPoint, abovePoints[j + 2].RiverPoint);
                    }
                }
            }

            if (belowPoints.Count > 2)
            {
                riverBoundaryPath.LineTo(belowPoints.Last().RiverPoint);

                for (int j = belowPoints.Count - 1; j >= 2; j -= 3)
                {
                    if (j < belowPoints.Count - 2)
                    {
                        riverBoundaryPath.CubicTo(belowPoints[j].RiverPoint, belowPoints[j - 1].RiverPoint, belowPoints[j - 2].RiverPoint);
                    }
                }
            }

            if (abovePoints.Count > 2 && belowPoints.Count > 2)
            {
                riverBoundaryPath.LineTo(abovePoints[0].RiverPoint);

                river.RiverBoundaryPath?.Dispose();
                river.RiverBoundaryPath = new(riverBoundaryPath);
            }

            int pathDistance = (int)(river.RiverWidth / 2);

            List<MapRiverPoint> shorelineAbovePoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance / 2, ParallelEnum.Above, river.RiverSourceFadeIn);
            List<MapRiverPoint> shorelineBelowPoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance / 2, ParallelEnum.Below, river.RiverSourceFadeIn);
            using SKPath shorelinePath = new();

            if (shorelineAbovePoints.Count > 2)
            {
                shorelinePath.MoveTo(shorelineAbovePoints[0].RiverPoint);

                for (int j = 0; j < shorelineAbovePoints.Count; j += 3)
                {
                    if (j < shorelineAbovePoints.Count - 2)
                    {
                        shorelinePath.CubicTo(shorelineAbovePoints[j].RiverPoint, shorelineAbovePoints[j + 1].RiverPoint, shorelineAbovePoints[j + 2].RiverPoint);
                    }
                }
            }

            if (shorelineBelowPoints.Count > 2)
            {
                shorelinePath.MoveTo(shorelineBelowPoints[0].RiverPoint);

                for (int j = 0; j < shorelineBelowPoints.Count; j += 3)
                {
                    if (j < shorelineBelowPoints.Count - 2)
                    {
                        shorelinePath.CubicTo(shorelineBelowPoints[j].RiverPoint, shorelineBelowPoints[j + 1].RiverPoint, shorelineBelowPoints[j + 2].RiverPoint);
                    }
                }

                river.ShorelinePath?.Dispose();
                river.ShorelinePath = new(shorelinePath);
            }

            List<MapRiverPoint> gradient1AbovePoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance, ParallelEnum.Above, river.RiverSourceFadeIn);
            List<MapRiverPoint> gradient1BelowPoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance, ParallelEnum.Below, river.RiverSourceFadeIn);
            using SKPath gradient1Path = new();

            if (gradient1AbovePoints.Count > 2)
            {
                gradient1Path.MoveTo(gradient1AbovePoints[0].RiverPoint);

                for (int j = 0; j < gradient1AbovePoints.Count; j += 3)
                {
                    if (j < gradient1AbovePoints.Count - 2)
                    {
                        gradient1Path.CubicTo(gradient1AbovePoints[j].RiverPoint, gradient1AbovePoints[j + 1].RiverPoint, gradient1AbovePoints[j + 2].RiverPoint);
                    }
                }
            }

            if (gradient1BelowPoints.Count > 2)
            {
                gradient1Path.MoveTo(gradient1BelowPoints[0].RiverPoint);

                for (int j = 0; j < gradient1BelowPoints.Count; j += 3)
                {
                    if (j < gradient1BelowPoints.Count - 2)
                    {
                        gradient1Path.CubicTo(gradient1BelowPoints[j].RiverPoint, gradient1BelowPoints[j + 1].RiverPoint, gradient1BelowPoints[j + 2].RiverPoint);
                    }
                }

                river.Gradient1Path?.Dispose();
                river.Gradient1Path = new(gradient1Path);
            }

            /*
             * gradient2 and gradient3 are not being used now
             * 
            List<MapRiverPoint> gradient2AbovePoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance * 2, ParallelEnum.Above, river.RiverSourceFadeIn);
            List<MapRiverPoint> gradient2BelowPoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance * 2, ParallelEnum.Below, river.RiverSourceFadeIn);
            using SKPath gradient2Path = new();

            if (gradient2AbovePoints.Count > 2)
            {
                gradient2Path.MoveTo(gradient2AbovePoints[0].RiverPoint);

                for (int j = 0; j < gradient2AbovePoints.Count; j += 3)
                {
                    if (j < gradient2AbovePoints.Count - 2)
                    {
                        gradient2Path.CubicTo(gradient2AbovePoints[j].RiverPoint, gradient2AbovePoints[j + 1].RiverPoint, gradient2AbovePoints[j + 2].RiverPoint);
                    }
                }
            }

            if (gradient2BelowPoints.Count > 2)
            {
                gradient2Path.MoveTo(gradient2BelowPoints[0].RiverPoint);

                for (int j = 0; j < gradient2BelowPoints.Count; j += 3)
                {
                    if (j < gradient2BelowPoints.Count - 2)
                    {
                        gradient2Path.CubicTo(gradient2BelowPoints[j].RiverPoint, gradient2BelowPoints[j + 1].RiverPoint, gradient2BelowPoints[j + 2].RiverPoint);
                    }
                }

                river.Gradient2Path?.Dispose();
                river.Gradient2Path = new(gradient2Path);
            }

            List<MapRiverPoint> gradient3AbovePoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance * 3, ParallelEnum.Above, river.RiverSourceFadeIn);
            List<MapRiverPoint> gradient3BelowPoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance * 3, ParallelEnum.Below, river.RiverSourceFadeIn);
            using SKPath gradient3Path = new();

            if (gradient3AbovePoints.Count > 2)
            {
                gradient3Path.MoveTo(gradient3AbovePoints[0].RiverPoint);

                for (int j = 0; j < gradient3AbovePoints.Count; j += 3)
                {
                    if (j < gradient3AbovePoints.Count - 2)
                    {
                        gradient3Path.CubicTo(gradient3AbovePoints[j].RiverPoint, gradient3AbovePoints[j + 1].RiverPoint, gradient3AbovePoints[j + 2].RiverPoint);
                    }
                }
            }

            if (gradient3BelowPoints.Count > 2)
            {
                gradient3Path.MoveTo(gradient3BelowPoints[0].RiverPoint);

                for (int j = 0; j < gradient3BelowPoints.Count; j += 3)
                {
                    if (j < gradient3BelowPoints.Count - 2)
                    {
                        gradient3Path.CubicTo(gradient3BelowPoints[j].RiverPoint, gradient3BelowPoints[j + 1].RiverPoint, gradient3BelowPoints[j + 2].RiverPoint);
                    }
                }

                river.Gradient3Path?.Dispose();
                river.Gradient3Path = new(gradient3Path);
            }
            */

            List<MapRiverPoint> shallowWaterAbovePoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance * 0.8F, ParallelEnum.Above, river.RiverSourceFadeIn);
            List<MapRiverPoint> shallowWaterBelowPoints = GetParallelRiverPoints(distinctRiverPoints, pathDistance * 0.8F, ParallelEnum.Below, river.RiverSourceFadeIn);
            using SKPath shallowWaterPath = new();

            if (shallowWaterAbovePoints.Count > 2)
            {
                shallowWaterPath.MoveTo(shallowWaterAbovePoints[0].RiverPoint);

                for (int j = 0; j < shallowWaterAbovePoints.Count; j += 3)
                {
                    if (j < shallowWaterAbovePoints.Count - 2)
                    {
                        shallowWaterPath.CubicTo(shallowWaterAbovePoints[j].RiverPoint, shallowWaterAbovePoints[j + 1].RiverPoint, shallowWaterAbovePoints[j + 2].RiverPoint);
                    }
                }
            }

            if (shallowWaterBelowPoints.Count > 2)
            {
                shallowWaterPath.MoveTo(shallowWaterBelowPoints[0].RiverPoint);

                for (int j = 0; j < shallowWaterBelowPoints.Count; j += 3)
                {
                    if (j < shallowWaterBelowPoints.Count - 2)
                    {
                        shallowWaterPath.CubicTo(shallowWaterBelowPoints[j].RiverPoint, shallowWaterBelowPoints[j + 1].RiverPoint, shallowWaterBelowPoints[j + 2].RiverPoint);
                    }
                }

                river.ShallowWaterPath?.Dispose();
                river.ShallowWaterPath = new(shallowWaterPath);
            }
        }

        public static MapRiverPoint? SelectRiverPointAtPoint(River river, SKPoint mapClickPoint, bool selectOnlyControlPoints = true)
        {
            MapRiverPoint? selectedPoint = null;

            for (int i = 0; i < river.RiverPoints.Count; i++)
            {
                using SKPath controlPointPath = new();
                controlPointPath.AddCircle(river.RiverPoints[i].RiverPoint.X, river.RiverPoints[i].RiverPoint.Y, river.RiverWidth);

                if (selectOnlyControlPoints)
                {
                    if (controlPointPath.Contains(mapClickPoint.X, mapClickPoint.Y) && river.RiverPoints[i].IsControlPoint)
                    {
                        selectedPoint = river.RiverPoints[i];
                        break;
                    }
                }
                else if (controlPointPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                {
                    selectedPoint = river.RiverPoints[i];
                    break;
                }
            }

            if (selectedPoint != null)
            {
                selectedPoint.IsSelected = true;
            }

            return selectedPoint;
        }

        public static int GetRiverPointIndexById(River river, Guid guid)
        {
            for (int i = 0; i < river.RiverPoints.Count; i++)
            {
                if (river.RiverPoints[i].PointGuid == guid)
                {
                    return i;
                }
            }

            return -1;
        }

        internal static void MoveSelectedRiverPoint(River? selectedRiver, MapRiverPoint mapRiverPoint, SKPoint movePoint)
        {
            if (selectedRiver != null && selectedRiver != null)
            {
                int selectedIndex = GetRiverPointIndexById(selectedRiver, mapRiverPoint.PointGuid);

                if (selectedIndex > -1)
                {
                    // move the selected river point and the 10 points before and after it
                    if (selectedIndex - 10 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.1F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.1F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 10].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 9 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.2F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.2F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 9].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 8 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.3F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.3F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 8].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 7 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.4F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.4F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 7].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 6 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.5F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.5F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 6].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 5 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.6F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.6F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 5].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 4 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.7F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.7F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 4].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 3 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.8F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.8F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 3].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 2 > 0)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.9F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.9F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedRiver.RiverPoints[selectedIndex - 2].RiverPoint = newPoint;
                    }

                    if (selectedIndex - 1 > 0)
                    {
                        selectedRiver.RiverPoints[selectedIndex - 1].RiverPoint = movePoint;
                    }

                    selectedRiver.RiverPoints[selectedIndex].RiverPoint = movePoint;

                    if (selectedIndex + 1 < selectedRiver.RiverPoints.Count - 1)
                    {
                        selectedRiver.RiverPoints[selectedIndex + 1].RiverPoint = movePoint;
                    }

                    if (selectedIndex + 2 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.9F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.9F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 2].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 3 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.8F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.8F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 3].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 4 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.7F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.7F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 4].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 5 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.6F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.6F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 5].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 6 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.5F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.5F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 6].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 7 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.4F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.4F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 7].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 8 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.3F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.3F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 8].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 9 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.2F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.2F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 9].RiverPoint = newPoint;
                    }

                    if (selectedIndex + 10 < selectedRiver.RiverPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedRiver.RiverPoints[selectedIndex].RiverPoint.X) * 0.1F;
                        float yDelta = (movePoint.Y - selectedRiver.RiverPoints[selectedIndex].RiverPoint.Y) * 0.1F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedRiver.RiverPoints[selectedIndex + 10].RiverPoint = newPoint;
                    }

                }

                ConstructRiverPaths(selectedRiver);
                ConstructRiverPaintObjects(selectedRiver);
            }
        }
    }
}
