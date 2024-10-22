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
using AForge;
using AForge.Math.Geometry;
using DelaunatorSharp;
using SimplexNoise;
using SkiaSharp;

namespace RealmStudio
{
    internal class ShapeGenerator
    {
        public static List<SKPoint> GenerateRandomPointSet(int x, int y, int width, int height, int gridSize)
        {
            List<SKPoint> pointSet = GetRandomPoints(x, y, width, height, gridSize);
            return pointSet;
        }

        public static SKPath GetConvexHullPath(List<IPoint> points)
        {
            SKPath convexHullPath = new SKPath();
            List<IntPoint> intPoints = [];

            foreach (IPoint point in points)
            {
                intPoints.Add(new IntPoint((int)point.X, (int)point.Y));
            }

            IConvexHullAlgorithm hullFinder = new GrahamConvexHull();
            List<IntPoint> hull = hullFinder.FindHull(intPoints);

            convexHullPath.MoveTo(new SKPoint(hull[0].X, hull[0].Y));

            for (int i = 1; i < hull.Count; i++)
            {
                convexHullPath.LineTo(new SKPoint(hull[i].X, hull[i].Y));
            }

            convexHullPath.Close();

            return convexHullPath;
        }

        private static List<SKPoint> GetRandomPoints(int x, int y, int width, int height, int gridSize)
        {
            List<SKPoint> mapPoints = [];

            for (int pointx = x; pointx < x + width; pointx += gridSize)
            {
                for (int pointy = y; pointy < y + height; pointy += gridSize)
                {
                    SKPoint p = new((float)(pointx + gridSize * (Random.Shared.NextDouble() - Random.Shared.NextDouble())), (float)(pointy + gridSize * (Random.Shared.NextDouble() - Random.Shared.NextDouble())));
                    mapPoints.Add(p);
                }
            }

            return mapPoints;
        }


        public static List<SKPoint> GetRadialShape(float x, float y, float radius, float roughness)
        {
            List<SKPoint> shapePoints = [];

            float previousRadialDistance = 0.0F;

            double revolutions = 2.0;

            for (int i = 0; i < 360 * revolutions; i++)
            {
                float randomRadialDistance = Random.Shared.NextSingle();
                float scaledRadialDistance = (0.5F * radius) * randomRadialDistance;
                float variedScaleRadialDistance = scaledRadialDistance + Random.Shared.Next((int)(-roughness * scaledRadialDistance), (int)(roughness * scaledRadialDistance));

                float finalRadialDistance = variedScaleRadialDistance + previousRadialDistance;

                previousRadialDistance = variedScaleRadialDistance;

                // convert to cartesian coordinates
                float cx = (float)(finalRadialDistance * Math.Cos(i / revolutions));
                float cy = (float)(finalRadialDistance * Math.Sin(i / revolutions));

                shapePoints.Add(new SKPoint(x + cx, y + cy));
            }

            return shapePoints;
        }

        public static Bitmap GetNoiseGeneratedLakeShape()
        {
            // LAKE

            // see: https://www.redblobgames.com/maps/terrain-from-noise/#demo
            // and https://github.com/WardBenjamin/SimplexNoise

            float width = 500;
            float height = 500;

            Noise.Seed = Random.Shared.Next(int.MaxValue - 1);

            // scale parameter: larger scale value = denser noise, so scale = wavelength (higher wavelength = denser noise)
            // or scale is inverse of frequency

            float[,] noiseArray1 = Noise.Calc2D((int)width, (int)height, 0.008F);
            float[,] noiseArray2 = Noise.Calc2D((int)width, (int)height, 0.015F);

            float[,] elevation = new float[(int)width, (int)height];

            float waterLevel = 0.5F;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float e = (noiseArray1[x, y] / 255.0F);
                    //float e = ((noiseArray1[x, y] / 255.0F) + (noiseArray2[x, y] / 255.0F)) / 2.0F;
                    elevation[x, y] = e;
                }
            }


            float[,] distance = new float[(int)width, (int)height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float nx = 2 * x / width - 1;
                    float ny = 2 * y / height - 1;

                    distance[x, y] = 1 - (float)Math.Min(1, (nx * nx + ny * ny) / Math.Sqrt(2));
                }
            }

            float interpolationWeight = 0.6F;

            float[,] interpolatedElevationAndDistance = new float[(int)width, (int)height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    interpolatedElevationAndDistance[x, y] = float.Lerp(elevation[x, y], 1 - distance[x, y], interpolationWeight);
                }
            }

            // make sure the bitmap has a white border surrounding it so the lake image can be extracted
            Bitmap lakeBitmap = new((int)width, (int)height);
            using Graphics g = Graphics.FromImage(lakeBitmap);
            g.Clear(Color.White);

            var lockedBitmap = new LockBitmap(lakeBitmap);
            lockedBitmap.LockBits();

            for (int x = 2; x < width - 2; x++)
            {
                for (int y = 2; y < height - 2; y++)
                {
                    if (interpolatedElevationAndDistance[x, y] > waterLevel)
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

            return lakeBitmap;
        }

        public static Bitmap GetNoiseGeneratedIslandShape(int width, int height, float islandSize)
        {
            // ISLAND

            Noise.Seed = Random.Shared.Next(int.MaxValue - 1);

            // scale parameter: larger scale value = denser noise, so scale = wavelength (higher wavelength = denser noise)
            // or scale is inverse of frequency

            // generate height map from simplex noise
            float[,] elevation = new float[width, height];

            float waterLevel = 0.5F;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    elevation[x, y] = Noise.CalcPixel2D(x, y, 0.005F) / 255.0F;
                }
            }

            Bitmap elevationBitmap = ConvertValuesToBWBitmap(elevation, width, height, waterLevel);
            elevationBitmap.Save("C:\\Users\\Pete Nelson\\OneDrive\\Desktop\\elevationbitmap.bmp");

            // calculate distance of point from center of bitmap
            float[,] distance = new float[width, height];

            SKPoint mapCenter = new SKPoint(width / 2, height / 2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    distance[x, y] = 1 - (SKPoint.Distance(mapCenter, new SKPoint(x, y)) / (float)Math.Sqrt(width * width + height * height));
                }
            }

            Bitmap distanceBitmap = ConvertValuesToBWBitmap(distance, width, height, islandSize);
            distanceBitmap.Save("C:\\Users\\Pete Nelson\\OneDrive\\Desktop\\distancebitmap.bmp");

            // interpolate distance with elevation
            float interpolationWeight = 0.6F;

            float[,] interpolatedElevationAndDistance = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    interpolatedElevationAndDistance[x, y] = float.Lerp(elevation[x, y], 1 - distance[x, y], interpolationWeight);
                }
            }

            // convert interpolated values into black/white pixels on a bitmap based on water level

            Bitmap islandBitmap = ConvertValuesToBWBitmap(interpolatedElevationAndDistance, width, height, waterLevel);
            return islandBitmap;
        }


        internal static Bitmap ConvertValuesToBWBitmap(float[,] values, int width, int height, float threshold)
        {
            Bitmap b = new(width, height);
            using Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);

            var lockedBitmap = new LockBitmap(b);
            lockedBitmap.LockBits();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (values[x, y] > threshold)
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

            return b;
        }
    }
}
