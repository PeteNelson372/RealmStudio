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

namespace RealmStudio
{
    internal sealed class MapHeightMapMethods
    {
        public List<string> LandFormObjModelList { get; set; } = [];

        internal static void ExportHeightMap3DModel(RealmStudioMap map)
        {
            SKSurface s = SKSurface.Create(new SKImageInfo(map.MapWidth, map.MapHeight));
            s.Canvas.Clear(SKColors.Black);

            // render the map as a height map
            RenderHeightMapToCanvas(map, s.Canvas, new SKPoint(0, 0), null);

            SKBitmap heightMap = SKBitmap.FromImage(s.Snapshot());

            Cursor.Current = Cursors.WaitCursor;

            List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMap, Byte.MaxValue / 2.0F);

            Cursor.Current = Cursors.Default;

            HeightMapTo3DModel.WriteObjModelToFile(objModelStringList);
        }

        internal static void RenderHeightMapToCanvas(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint, SKRect? selectedArea)
        {
            renderCanvas.Clear(SKColors.Black);
            MapRenderMethods.RenderHeightMap(map, renderCanvas, scrollPoint, selectedArea);
        }

        internal static void ChangeHeightMapAreaHeight(RealmStudioMap? map, SKPoint mapPoint, int brushRadius, float changeAmount)
        {
            ArgumentNullException.ThrowIfNull(map);

            MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.HEIGHTMAPLAYER);

            if (heightMapLayer.MapLayerComponents.Count == 2)
            {
                MapHeightMap mapHeightMap = (MapHeightMap)heightMapLayer.MapLayerComponents[1];

                float[,]? heightMap = mapHeightMap.HeightMap;

                SKBitmap? heightMapBitmap = mapHeightMap.MapImageBitmap;

                if (heightMapBitmap != null && heightMap != null)
                {
                    // get all pixels within the brush area
                    // for each pixel in the brush area, get its color and then change its value by adding
                    // changeAmount

                    // radius of the circle squared
                    double radiusSquared = brushRadius * brushRadius;

                    for (int x = (int)mapPoint.X - brushRadius; x < (int)mapPoint.X + brushRadius; x++)
                    {
                        for (int y = (int)mapPoint.Y - brushRadius; y < (int)mapPoint.Y + brushRadius; y++)
                        {
                            if (x >= 0 && x < heightMapBitmap.Width && y >= 0 && y < heightMapBitmap.Height)
                            {
                                // delta x,y from the point at the center of the circle brush
                                double dx = x - mapPoint.X;
                                double dy = y - mapPoint.Y;

                                // distance squared of the point from the center of the circle at point
                                double distanceSquared = dx * dx + dy * dy;

                                // a random value ranging from 0.0 to the radius squared
                                double pointRandom = Random.Shared.NextDouble() * radiusSquared;

                                // if the point is inside the circle brush and the random value is greater than the
                                // distance squared, add the point to the list of points to be increased/decreased in grayscale color
                                // points closer to the center of the brush circle are more likely to be included,
                                // since distance squared increases as points are further from the center point of the brush
                                if (distanceSquared <= radiusSquared && pointRandom >= distanceSquared)
                                {
                                    SetHeightmapPixelHeight(x, y, heightMapBitmap, heightMap, changeAmount);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void SetHeightmapPixelHeight(int x, int y, SKBitmap heightMapBitmap, float[,] heightMap, float changeAmount)
        {
            SKColor pixelColor = heightMapBitmap.GetPixel(x, y);
            float colorValue;

            if (pixelColor == SKColors.Black || pixelColor == SKColors.Transparent || pixelColor == SKColors.Empty)
            {
                colorValue = 35;
            }
            else
            {
                colorValue = heightMap[x, y];
            }

            colorValue += changeAmount;
            heightMap[x, y] = colorValue;

            // color value is the average of the colorValue and the
            // eight pixels surrounding the current pixel

            float accumulatedHeight = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 && x + i < heightMapBitmap.Width
                        && y + j >= 0 && y + j < heightMapBitmap.Height)
                    {
                        accumulatedHeight += heightMap[x + i, y + j];
                    }
                }
            }

            colorValue = accumulatedHeight / 9;

            colorValue = Math.Min(255.0F, colorValue);
            colorValue = Math.Max(35.0F, colorValue);
            heightMap[x, y] = colorValue;

            int r = (int)Math.Round(colorValue);
            int g = (int)Math.Round(colorValue);
            int b = (int)Math.Round(colorValue);
            heightMapBitmap.SetPixel(x, y, new SKColor((byte)r, (byte)g, (byte)b));
        }

        internal static SKBitmap? ExtractRectFromHeightMap(RealmStudioMap map, SKRect? extractRect)
        {
            if (extractRect == null) return null;

            using SKSurface s = SKSurface.Create(new SKImageInfo(map.MapWidth, map.MapHeight));
            s.Canvas.Clear(SKColors.Black);

            RenderHeightMapToCanvas(map, s.Canvas, new SKPoint(0, 0), null);

            SKBitmap heightMap = SKBitmap.FromImage(s.Snapshot());

            // extract the section of the heightmap within the bounding box from the heightmap
            SKBitmap extractedBitmap = new((int)((SKRect)extractRect).Width, (int)((SKRect)extractRect).Height);
            using SKCanvas canvas = new(extractedBitmap);

            canvas.Clear(SKColors.Black);

            canvas.DrawBitmap(heightMap, (SKRect)extractRect, new SKRect(0, 0, ((SKRect)extractRect).Width, ((SKRect)extractRect).Height));

            return extractedBitmap;
        }

        internal static void ConvertMapImageToMapHeightMap(RealmStudioMap map)
        {
            // convert MapImage objects in HeightMap layer to MapHeightMap objects
            MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.HEIGHTMAPLAYER);
            if (heightMapLayer != null)
            {
                if (heightMapLayer.MapLayerComponents.Count == 2)
                {
                    if (heightMapLayer.MapLayerComponents[1] is MapImage mi && mi.MapImageBitmap != null)
                    {
                        MapHeightMap mhm = new()
                        {
                            MapImageBitmap = mi.MapImageBitmap.Copy(),
                            HeightMap = new float[map.MapWidth, map.MapHeight]
                        };

                        mhm.MapImageBitmap ??= new SKBitmap(map.MapWidth, map.MapHeight);

                        for (int i = 0; i < mhm.MapImageBitmap.Width; i++)
                        {
                            for (int j = 0; j < mhm.MapImageBitmap.Height; j++)
                            {
                                mhm.HeightMap[i, j] = mhm.MapImageBitmap.GetPixel(i, j).Red;
                            }
                        }

                        heightMapLayer.MapLayerComponents[1] = mhm;
                    }
                    else if (heightMapLayer.MapLayerComponents[1] is MapHeightMap mhm)
                    {
                        mhm.MapImageBitmap ??= new SKBitmap(map.MapWidth, map.MapHeight);
                        mhm.HeightMap ??= new float[map.MapWidth, map.MapHeight];

                        for (int i = 0; i < mhm.MapImageBitmap.Width; i++)
                        {
                            for (int j = 0; j < mhm.MapImageBitmap.Height; j++)
                            {
                                mhm.HeightMap[i, j] = mhm.MapImageBitmap.GetPixel(i, j).Red;
                            }
                        }
                    }
                }
                else
                {
                    RealmMapMethods.AddMapImagesToHeightMapLayer(map);
                }
            }
        }

        internal static SKBitmap? GetBitmapForThreeDView(RealmStudioMap map, Landform? landform, SKRect? realmArea)
        {
            SKBitmap? heightMapBitmap = null;

            try
            {
                if (realmArea != SKRect.Empty)
                {
                    SKBitmap? extractedBitmap = ExtractRectFromHeightMap(map, (SKRect?)realmArea);
                    heightMapBitmap = extractedBitmap?.Copy();
                }
                else if (landform != null)
                {
                    // extract a heightmap bitmap using the area of the selected landform bounds
                    // then create a 3D model from the bitmap and display it in the ThreeDView
                    Cursor.Current = Cursors.WaitCursor;

                    landform.ContourPath.GetTightBounds(out SKRect landformBounds);

                    // inflate the bounds by 20 pixels to give the height map some border
                    landformBounds.Inflate(20, 20);
                    landformBounds.Left = Math.Max(0, landformBounds.Left - 10);
                    landformBounds.Top = Math.Max(0, landformBounds.Top - 10);

                    SKBitmap? extractedBitmap = ExtractRectFromHeightMap(map, landformBounds);

                    heightMapBitmap = extractedBitmap?.Copy();
                }
                else
                {
                    // generate the 3D model from the entire map
                    using SKSurface s = SKSurface.Create(new SKImageInfo(map.MapWidth, map.MapHeight));
                    s.Canvas.Clear(SKColors.Black);

                    RenderHeightMapToCanvas(map, s.Canvas, new SKPoint(0, 0), null);

                    heightMapBitmap = SKBitmap.FromImage(s.Snapshot()).Copy();
                }
            }
            catch { }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return heightMapBitmap;
        }
    }
}
