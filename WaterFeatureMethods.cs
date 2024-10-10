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
                        && waterLayer.MapLayerComponents[i] is WaterFeature
                        && waterLayer.MapLayerComponents[j] is WaterFeature
                        && !mergedWaterFeatureGuids.Contains(((WaterFeature)waterLayer.MapLayerComponents[i]).WaterFeatureGuid)
                        && !mergedWaterFeatureGuids.Contains(((WaterFeature)waterLayer.MapLayerComponents[j]).WaterFeatureGuid))
                    {
                        WaterFeature waterFeature_i = (WaterFeature)waterLayer.MapLayerComponents[i];
                        WaterFeature waterFeature_j = (WaterFeature)waterLayer.MapLayerComponents[j];

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
                if (waterLayer.MapLayerComponents[k] is WaterFeature
                    && mergedWaterFeatureGuids.Contains(((WaterFeature)waterLayer.MapLayerComponents[k]).WaterFeatureGuid))
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

            float calcDistance = distance;

            for (int i = 0; i < points.Count - 1; i += 2)
            {
                float lineAngle = DrawingMethods.CalculateLineAngle(points[i].RiverPoint, points[i + 1].RiverPoint);

                float angle = (location == ParallelEnum.Below) ? 90 : -90;

                if (fromStartingPoint)
                {
                    calcDistance = distance * ((float)i / (float)points.Count);
                }

                SKPoint p1 = DrawingMethods.PointOnCircle(calcDistance, lineAngle + angle, points[i].RiverPoint);
                SKPoint p2 = DrawingMethods.PointOnCircle(calcDistance, lineAngle + angle, points[i + 1].RiverPoint);

                parallelPoints.Add(new MapRiverPoint(p1));
                parallelPoints.Add(new MapRiverPoint(p2));
            }

            return parallelPoints;
        }

        internal static void ConstructRiverPaintObjects(River mapRiver)
        {
            float strokeWidth = mapRiver.RiverWidth / 2;

            SKShader colorShader = SKShader.CreateColor(Extensions.ToSKColor(mapRiver.RiverColor));

            MapTexture? riverTexture = AssetManager.WATER_TEXTURE_LIST.Find(x => x.TextureName == "Gray Texture");

            SKShader combinedShader;

            if (riverTexture != null)
            {
                riverTexture.TextureBitmap ??= System.Drawing.Image.FromFile(riverTexture.TexturePath) as Bitmap;

                SKBitmap bitmap = Extensions.ToSKBitmap(riverTexture.TextureBitmap);
                SKBitmap resizedSKBitmap = new((int)mapRiver.RiverWidth, (int)mapRiver.RiverWidth);

                bitmap.ScalePixels(resizedSKBitmap, SKFilterQuality.High);

                SKShader bitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                combinedShader = SKShader.CreateCompose(colorShader, bitmapShader, SKBlendMode.Modulate);
            }
            else
            {
                combinedShader = colorShader;
            }

            int pathDistance = (int)(mapRiver.RiverWidth / 3);

            SKColor riverColor = Extensions.ToSKColor(Color.FromArgb(mapRiver.RiverColor.A, mapRiver.RiverColor));

            mapRiver.RiverPaint = new()
            {
                Color = riverColor,
                StrokeWidth = pathDistance,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Butt,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                BlendMode = SKBlendMode.Src,
                Shader = combinedShader
            };

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
                StrokeWidth = pathDistance,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcATop,
                Color = Extensions.ToSKColor(mapRiver.RiverShorelineColor),
                IsAntialias = true,
            };

            // shallow water is a lighter shade of river color
            SKColor shallowWaterColor = Extensions.ToSKColor(Color.FromArgb(mapRiver.RiverShorelineColor.A / 4, mapRiver.RiverColor));

            mapRiver.RiverShallowWaterPaint = new()
            {
                Color = shallowWaterColor,
                StrokeWidth = pathDistance,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcATop,
                IsAntialias = true
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
    }
}
