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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using DelaunatorSharp;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Point = DelaunatorSharp.Point;

namespace RealmStudio
{
    internal class WaterFeatureMethods
    {
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
            waterFeature.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Below);
            waterFeature.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Below);

            waterFeature.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Above);
            waterFeature.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Above);
            waterFeature.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Above);

        }

        internal static void ConstructWaterFeaturePaintObjects(WaterFeature waterFeature)
        {
            SKShader colorShader = SKShader.CreateColor(Extensions.ToSKColor((Color)waterFeature.WaterFeatureColor));

            // TODO: should water have a texture applied?
            //SKBitmap shaderBitmap = Extensions.ToSKBitmap(MapDrawingMethods.GetNoisyBitmap(MapPaintMethods.WATER_BRUSH_SIZE, MapPaintMethods.WATER_BRUSH_SIZE));
            //SKShader bitmapShader = SKShader.CreateBitmap(shaderBitmap);
            //SKShader combinedShader = SKShader.CreateCompose(colorShader, bitmapShader, SKBlendMode.SrcOver);

            waterFeature.WaterFeatureBackgroundPaint = new()
            {
                Style = SKPaintStyle.Fill,
                Shader = colorShader,
                BlendMode = SKBlendMode.Src,
                Color = Extensions.ToSKColor((Color)waterFeature.WaterFeatureColor),
                IsAntialias = true,
            };

            waterFeature.WaterFeatureShorelinePaint = new()
            {
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.Src,
                Color = Extensions.ToSKColor(waterFeature.WaterFeatureShorelineColor),
                StrokeWidth = 2,
                IsAntialias = true,
            };

            waterFeature.ShallowWaterPaint = new()
            {
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcATop,
                StrokeWidth = 2,
                IsAntialias = true
            };
        }

        public static SKPath? GenerateRandomLakePath(SKPoint location, float lakeSize)
        {
            SKPoint centerLocation = new SKPoint(location.X - (lakeSize / 4), location.Y - (lakeSize / 4));

            List<SKPoint> lakePoints = ShapeGenerator.GenerateRandomPointSet((int)centerLocation.X, (int)centerLocation.Y, (int)lakeSize - 2, (int)lakeSize - 2, (int)(lakeSize / 2.0));

            while (lakePoints.Count < lakeSize * 2)
            {
                lakePoints.AddRange(ShapeGenerator.GenerateRandomPointSet((int)centerLocation.X + Random.Shared.Next(-3, 3), (int)centerLocation.Y + Random.Shared.Next(-3, 3), (int)lakeSize - 2, (int)lakeSize - 2, (int)(lakeSize / 2.0)));
            }

            List<IPoint> iPoints = [];

            foreach (SKPoint point in lakePoints)
            {
                iPoints.Add(new Point(point.X, point.Y));
            }

            List<SKPoint> voronoiCellPoints = [];

            Delaunator delaunator = new([.. iPoints]);

            IEnumerable<IVoronoiCell> cells = delaunator.GetVoronoiCellsBasedOnCentroids();

            foreach (IVoronoiCell cell in cells)
            {
                IPoint[] points = cell.Points;

                foreach (IPoint point in points)
                {
                    voronoiCellPoints.Add(new SKPoint((float)point.X, (float)point.Y));
                }
            }

            // Compute the concave hull.
            List<SKPoint> hullPoints = ConcaveHull.ComputeConcaveHull(voronoiCellPoints, 3);

            SKPath concaveHullPath = new();
            concaveHullPath.MoveTo(new SKPoint(hullPoints[0].X, hullPoints[0].Y));

            for (int i = 1; i < hullPoints.Count; i++)
            {
                concaveHullPath.LineTo(new SKPoint(hullPoints[i].X, hullPoints[i].Y));
            }

            //concaveHullPath.Close();

            //SKPath lakePath = ShapeGenerator.GetConvexHullPath(lakePoints);

            return concaveHullPath;
        }

        public static Bitmap GetLakeBitmap(int width, int height)
        {
            //float fx = (float)Random.Shared.NextDouble();
            //float fy = (float)Random.Shared.NextDouble();

            float fx = 0.02F;
            float fy = 0.02F;

            float seed = (float)Random.Shared.NextDouble();

            SKBitmap b = new(width, height);
            SKCanvas skc = new(b);
            skc.Clear(SKColors.White);

            SKRect tileRect = new(0, 0, width, height);

            //int numOctaves = (int)Math.Round(Math.Log2(width));

            int numOctaves = 2;

            using SKPaint paint = new();

            paint.Style = SKPaintStyle.Fill;
            paint.Shader = SKShader.CreatePerlinNoiseTurbulence(fx, fy, numOctaves, seed, new SKSizeI((int)tileRect.Width, (int)tileRect.Height));

            skc.DrawRect(tileRect, paint);
            Bitmap gsb = DrawingMethods.MakeGrayscale(Extensions.ToBitmap(b), 0.19F, false);

            MakeLakeBitmap(ref gsb);

            return gsb;
        }

        private static void MakeLakeBitmap(ref Bitmap bitmap)
        {
            byte colorValue = 128;

            if (bitmap == null)
            {
                return;
            }

            var lockedBitmap = new LockBitmap(bitmap);
            lockedBitmap.LockBits();

            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    Color c = lockedBitmap.GetPixel(x, y);

                    if (c.R < colorValue && c.G < colorValue && c.B < colorValue)
                    {
                        lockedBitmap.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        lockedBitmap.SetPixel(x, y, Color.Black);
                    }
                }
            }

            lockedBitmap.UnlockBits();
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
            if (mapRiver.RiverPaint != null) return;

            float strokeWidth = mapRiver.RiverWidth / 2;

            SKShader colorShader = SKShader.CreateColor(Extensions.ToSKColor(mapRiver.RiverColor));

            MapTexture? riverTexture = AssetManager.WATER_TEXTURE_LIST.Find(x => x.TextureName == "Gray Texture");

            SKShader combinedShader;

            if (riverTexture != null)
            {
                riverTexture.TextureBitmap ??= Image.FromFile(riverTexture.TexturePath) as Bitmap;

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

            SKColor riverColor = Extensions.ToSKColor(Color.FromArgb(mapRiver.RiverColor.A, mapRiver.RiverColor));

            mapRiver.RiverPaint = new()
            {
                Color = riverColor,
                StrokeWidth = 4,
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
                StrokeWidth = strokeWidth - 2,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.Src,
                Color = Extensions.ToSKColor(mapRiver.RiverShorelineColor),
                IsAntialias = true,
            };

            // shallow water is a lighter shade of river color
            SKColor shallowWaterColor = Extensions.ToSKColor(Color.FromArgb(mapRiver.RiverShorelineColor.A / 4, mapRiver.RiverColor));

            shallowWaterColor.ToHsl(out float hue, out float saturation, out float luminance);
            luminance *= 1.1F;

            shallowWaterColor = SKColor.FromHsl(hue, saturation, luminance);

            mapRiver.RiverShallowWaterPaint = new()
            {
                Color = shallowWaterColor,
                StrokeWidth = strokeWidth / 2F,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.SrcATop,
                IsAntialias = true
            };
        }
    }
}
