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
using System.IO;
using System.IO.Compression;

namespace RealmStudio
{
    internal sealed class RealmMapMethods
    {
        internal static void RenderHeightMapToCanvas(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            renderCanvas.Clear(SKColors.Black);
            MapRenderMethods.RenderHeightMap(map, renderCanvas, scrollPoint);
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
                    List<SKPoint> brushPoints = [];

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
                                    brushPoints.Add(new SKPoint(x, y));
                                }
                            }
                        }
                    }

                    foreach (SKPoint index in brushPoints)
                    {
                        SKColor pixelColor = heightMapBitmap.GetPixel((int)index.X, (int)index.Y);
                        float colorValue;

                        if (pixelColor == SKColors.Black || pixelColor == SKColors.Transparent || pixelColor == SKColors.Empty)
                        {
                            colorValue = 35;
                        }
                        else
                        {
                            colorValue = heightMap[(int)index.X, (int)index.Y];
                        }

                        colorValue += changeAmount;
                        heightMap[(int)index.X, (int)index.Y] = colorValue;

                        // color value is the average of the colorValue and the
                        // eight pixels surrounding the current pixel

                        float accumulatedHeight = 0;

                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                if (index.X + i >= 0 && index.X + i < heightMapBitmap.Width
                                    && index.Y + j >= 0 && index.Y + j < heightMapBitmap.Height)
                                {
                                    accumulatedHeight += heightMap[(int)(index.X + i), (int)(index.Y + j)];
                                }
                            }
                        }

                        colorValue = accumulatedHeight / 9;

                        colorValue = Math.Min(255.0F, colorValue);
                        colorValue = Math.Max(35.0F, colorValue);

                        heightMap[(int)index.X, (int)index.Y] = colorValue;

                        int r = (int)Math.Round(colorValue);
                        int g = (int)Math.Round(colorValue);
                        int b = (int)Math.Round(colorValue);

                        heightMapBitmap.SetPixel((int)index.X, (int)index.Y, new SKColor((byte)r, (byte)g, (byte)b));
                    }
                }
            }
        }

        internal static RealmStudioMap? CreateDetailMap(RealmStudioMainForm mainForm, RealmStudioMap currentMap, SKRect selectedArea)
        {
            if (selectedArea.IsEmpty)
            {
                MessageBox.Show("Please use the Area button on the Menu Bar to select an area for the detail map.", "Select a Map Area");
            }
            else
            {
                DetailMapForm detailMapForm = new(mainForm, currentMap, selectedArea);
                DialogResult result = detailMapForm.ShowDialog();

                return result == DialogResult.OK ? detailMapForm.GetDetailMap() : null;
            }

            return null;

        }

        internal static RealmStudioMap? CreateResizedMap(RealmStudioMainForm mainForm, RealmStudioMap currentMap)
        {

            ResizeRealm resizeRealmForm = new(mainForm, currentMap);
            DialogResult result = resizeRealmForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                return result == DialogResult.OK ? resizeRealmForm.GetResizedMap() : null;
            }

            return null;
        }


        internal static RealmStudioMap CreateNewMapFromMap(RealmStudioMap currentMap,
            string newMapName,
            SKRect selectedMapArea,
            int newMapWidth,
            int newMapHeight,
            ResizeMapAnchorPoint anchorPoint,
            bool scaleToFit,
            GRContext newMapGRContext,
            bool includeTerrainSymbols,
            bool includeVegetationSymbols,
            bool includeStructureSymbols,
            bool includeLabels,
            bool includeBoxes,
            bool includePaths,
            bool includeScale,
            bool includeGrid,
            bool includeRegions)
        {
            // if the current map is being resized, the selectedMapArea is the entire current map
            // if a detail map is being created, the selectedMapArea is the area of the current map that is selected by the user

            // create the new map at the given size
            RealmStudioMap newRealmMap = new()
            {
                MapWidth = newMapWidth,
                MapHeight = newMapHeight,
                MapName = newMapName,
                MapAreaUnits = currentMap.MapAreaUnits,
                RealmType = currentMap.RealmType,
            };

            newRealmMap = MapBuilder.CreateMap(newRealmMap, newMapGRContext);
            newRealmMap.IsSaved = false;

            // scale and delta are calculated from selected area, new map width and height,
            // whether or not current map should be scaled to fit the new map, and anchor point

            // if the new map is smaller than the current map, then the current map
            // will be scaled to fit the new map and the anchor point will be the top-left

            if (newMapWidth < currentMap.MapWidth && newMapHeight < currentMap.MapHeight)
            {
                scaleToFit = true;
                anchorPoint = ResizeMapAnchorPoint.TopLeft;
            }

            // the location and size of each symbol, landforms, painted colors, paths, and labels
            // has to be determined based on the location, size, and scale of the current map
            // versus the new map
            float scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
            float scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

            newRealmMap.MapAreaWidth = currentMap.MapAreaWidth * scaleX;
            newRealmMap.MapAreaHeight = currentMap.MapAreaHeight * scaleY;

            newRealmMap.MapPixelWidth = newRealmMap.MapAreaWidth / newRealmMap.MapWidth;
            newRealmMap.MapPixelHeight = newRealmMap.MapAreaHeight / newRealmMap.MapHeight;

            float deltaX = -selectedMapArea.Left * scaleX;
            float deltaY = -selectedMapArea.Top * scaleY;

            // determine the scale and translation based on the anchor point
            // CenterZoomed and DetailMap cases are the same and the
            // default calculations for scale and translation above are used

            switch (anchorPoint)
            {
                case ResizeMapAnchorPoint.Center:
                    if (scaleToFit)
                    {
                        scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                        scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

                        deltaX = 0;
                        deltaY = 0;
                    }
                    else
                    {
                        scaleX = 1.0F;
                        scaleY = 1.0F;

                        deltaX = -(currentMap.MapWidth - newMapWidth) / 2;
                        deltaY = -(currentMap.MapHeight - newMapHeight) / 2;
                    }
                    break;
                case ResizeMapAnchorPoint.TopLeft:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;

                            deltaX = 0;
                            deltaY = 0;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.TopCenter:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;

                            deltaX = -(currentMap.MapWidth - newMapWidth) / 2;
                            deltaY = 0;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.TopRight:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;

                            deltaX = newRealmMap.MapWidth - currentMap.MapWidth;
                            deltaY = 0;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.CenterLeft:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;
                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;
                            deltaX = 0;
                            deltaY = -(currentMap.MapHeight - newMapHeight) / 2;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.CenterRight:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;
                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;
                            deltaX = newRealmMap.MapWidth - currentMap.MapWidth;
                            deltaY = -(currentMap.MapHeight - newMapHeight) / 2;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.BottomLeft:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;

                            deltaX = 0;
                            deltaY = newRealmMap.MapHeight - currentMap.MapHeight;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.BottomCenter:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;
                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;
                            deltaX = -(currentMap.MapWidth - newMapWidth) / 2;
                            deltaY = newRealmMap.MapHeight - currentMap.MapHeight;
                        }
                    }
                    break;
                case ResizeMapAnchorPoint.BottomRight:
                    {
                        if (scaleToFit)
                        {
                            scaleX = newRealmMap.MapWidth / selectedMapArea.Width;
                            scaleY = newRealmMap.MapHeight / selectedMapArea.Height;

                            deltaX = 0;
                            deltaY = 0;
                        }
                        else
                        {
                            scaleX = 1.0F;
                            scaleY = 1.0F;

                            deltaX = newRealmMap.MapWidth - currentMap.MapWidth;
                            deltaY = newRealmMap.MapHeight - currentMap.MapHeight;
                        }
                    }
                    break;
            }


            // get the landforms within or intersecting the selected area, then translate and scale them
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.LANDFORMLAYER);
            MapLayer newRealmLandformLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.LANDFORMLAYER);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform lf)
                {
                    SKPath lfContour = lf.ContourPath;

                    if (lfContour != null)
                    {
                        foreach (SKPoint p in lfContour.Points)
                        {
                            if (selectedMapArea.Contains(p))
                            {
                                // landform path is in or intersects the selected area
                                SKPath transformedPath = new(lfContour);
                                transformedPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                                Landform newLandform = new(lf)
                                {
                                    ParentMap = newRealmMap,
                                    DrawPath = transformedPath,
                                };

                                SKImageInfo lfImageInfo = new(newRealmMap.MapWidth, newRealmMap.MapHeight);

                                newLandform.LandformRenderSurface ??= SKSurface.Create(newMapGRContext, false, lfImageInfo);
                                newLandform.CoastlineRenderSurface ??= SKSurface.Create(newMapGRContext, false, lfImageInfo);

                                LandformMethods.CreateAllPathsFromDrawnPath(newRealmMap, newLandform);
                                newRealmLandformLayer.MapLayerComponents.Add(newLandform);

                                break;
                            }
                        }
                    }
                }

                // TODO:layer paint strokes in land layer
            }

            // go through the current map to get textures, painted colors, etc. and assign them to the detail map

            // texture needs to be resized to new map size
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.BASELAYER);
            MapLayer newRealmBaseLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.BASELAYER);

            foreach (MapImage mi in baseLayer.MapLayerComponents.Cast<MapImage>())
            {
                Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), newRealmMap.MapWidth, newRealmMap.MapHeight);

                MapImage BackgroundTexture = new()
                {
                    Width = newRealmMap.MapWidth,
                    Height = newRealmMap.MapHeight,
                    MapImageBitmap = resizedBitmap.ToSKBitmap(),
                };

                newRealmBaseLayer.MapLayerComponents.Add(BackgroundTexture);
            }

            // texture needs to be resized to new map size
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANTEXTURELAYER);
            MapLayer newRealmOceanTextureLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OCEANTEXTURELAYER);

            foreach (MapImage mi in oceanTextureLayer.MapLayerComponents.Cast<MapImage>())
            {
                Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), newRealmMap.MapWidth, newRealmMap.MapHeight);

                MapImage OceanTexture = new()
                {
                    Width = newRealmMap.MapWidth,
                    Height = newRealmMap.MapHeight,
                    MapImageBitmap = resizedBitmap.ToSKBitmap(),
                };

                newRealmOceanTextureLayer.MapLayerComponents.Add(OceanTexture);
            }

            // texture needs to be resized to new map size
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer newRealmOceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            foreach (MapImage mi in oceanTextureOverlayLayer.MapLayerComponents.Cast<MapImage>())
            {
                Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), newRealmMap.MapWidth, newRealmMap.MapHeight);

                MapImage OceanColor = new()
                {
                    Width = newRealmMap.MapWidth,
                    Height = newRealmMap.MapHeight,
                    MapImageBitmap = resizedBitmap.ToSKBitmap(),
                };

                newRealmOceanTextureOverlayLayer.MapLayerComponents.Add(OceanColor);
            }

            // ocean drawing layer
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANDRAWINGLAYER);
            MapLayer newRealmOceanDrawingLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OCEANDRAWINGLAYER);

            foreach (LayerPaintStroke lps in oceanDrawingLayer.MapLayerComponents.Cast<LayerPaintStroke>())
            {
                LayerPaintStroke newPaintStroke = new()
                {
                    ParentMap = newRealmMap,
                    StrokeColor = lps.StrokeColor,
                    PaintBrush = lps.PaintBrush,
                    BrushRadius = (int)(lps.BrushRadius * scaleX),
                    MapLayerIdentifier = lps.MapLayerIdentifier,
                    Erase = lps.Erase,
                    Rendered = false,
                };

                foreach (LayerPaintStrokePoint point in lps.PaintStrokePoints)
                {
                    LayerPaintStrokePoint newStrokePoint = new()
                    {
                        StrokeLocation = new SKPoint((point.X * scaleX) + deltaX, (point.Y * scaleY) + deltaY),
                        StrokeRadius = (int)(point.StrokeRadius * scaleX)
                    };

                    newPaintStroke.PaintStrokePoints.Add(newStrokePoint);
                }

                SKImageInfo imageInfo = new(newRealmMap.MapWidth, newRealmMap.MapHeight);
                newPaintStroke.RenderSurface ??= SKSurface.Create(newMapGRContext, false, imageInfo);
                newRealmOceanDrawingLayer.MapLayerComponents.Add(newPaintStroke);
            }


            // land drawing layer
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.LANDDRAWINGLAYER);
            MapLayer newRealmLandDrawingLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.LANDDRAWINGLAYER);

            for (int i = 0; i < landDrawingLayer.MapLayerComponents.Count; i++)
            {
                // construct a new LayerPaintStroke from the existing one
                // and create a new paint surface for it

                if (landDrawingLayer.MapLayerComponents[i] is LayerPaintStroke lps)
                {
                    LayerPaintStroke newPaintStroke = new()
                    {
                        ParentMap = newRealmMap,
                        StrokeColor = lps.StrokeColor,
                        PaintBrush = lps.PaintBrush,
                        BrushRadius = (int)(lps.BrushRadius * scaleX),
                        MapLayerIdentifier = lps.MapLayerIdentifier,
                        Erase = lps.Erase,
                        Rendered = false,
                    };

                    foreach (LayerPaintStrokePoint point in lps.PaintStrokePoints)
                    {
                        LayerPaintStrokePoint newStrokePoint = new()
                        {
                            StrokeLocation = new SKPoint((point.X * scaleX) + deltaX, (point.Y * scaleY) + deltaY),
                            StrokeRadius = (int)(point.StrokeRadius * scaleX)
                        };

                        newPaintStroke.PaintStrokePoints.Add(newStrokePoint);
                    }

                    SKImageInfo imageInfo = new(newRealmMap.MapWidth, newRealmMap.MapHeight);
                    newPaintStroke.RenderSurface ??= SKSurface.Create(newMapGRContext, false, imageInfo);
                    newRealmLandDrawingLayer.MapLayerComponents.Add(newPaintStroke);
                }
            }

            // water layer objects - these need to be treated basically the same as landforms
            // get the water features within or intersecting the selected area, then translate and scale them
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.WATERLAYER);
            MapLayer newRealmWaterLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.WATERLAYER);

            for (int i = 0; i < waterLayer.MapLayerComponents.Count; i++)
            {
                if (waterLayer.MapLayerComponents[i] is WaterFeature wf)
                {
                    SKPath wfPath = wf.WaterFeaturePath;
                    SKPath wfContour = wf.ContourPath;

                    if (wfContour != null)
                    {
                        foreach (SKPoint p in wfContour.Points)
                        {
                            if (selectedMapArea.Contains(p))
                            {
                                // water feature path is in or intersects the selected area
                                SKPath transformedContourPath = new(wfContour);
                                transformedContourPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                                SKPath transformedWfPath = new(wfPath);
                                transformedWfPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                                WaterFeature newRealmWf = new(wf)
                                {
                                    WaterFeaturePath = transformedWfPath,
                                    ParentMap = newRealmMap,
                                };

                                SKImageInfo lfImageInfo = new(newRealmMap.MapWidth, newRealmMap.MapHeight);

                                WaterFeatureMethods.CreateInnerAndOuterPaths(newRealmMap, newRealmWf);
                                WaterFeatureMethods.ConstructWaterFeaturePaintObjects(newRealmWf);

                                newRealmWaterLayer.MapLayerComponents.Add(newRealmWf);

                                break;
                            }
                        }
                    }
                }
                else if (waterLayer.MapLayerComponents[i] is River r)
                {
                    foreach (MapRiverPoint mrp in r.RiverPoints)
                    {
                        if (selectedMapArea.Contains(mrp.RiverPoint) && r.RiverPath?.PointCount > 2)
                        {
                            River newRealmRiver = new(r)
                            {
                                ParentMap = newRealmMap,
                            };

                            SKPath transformedRiverPath = new(newRealmRiver.RiverPath);
                            transformedRiverPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                            newRealmRiver.RiverPath = transformedRiverPath;

                            foreach (SKPoint p in transformedRiverPath.Points)
                            {
                                MapRiverPoint newMrp = new()
                                {
                                    RiverPoint = p
                                };

                                newRealmRiver.RiverPoints.Add(newMrp);
                            }

                            WaterFeatureMethods.ConstructRiverPaintObjects(newRealmRiver);
                            WaterFeatureMethods.ConstructRiverPaths(newRealmRiver);

                            newRealmWaterLayer.MapLayerComponents.Add(newRealmRiver);

                            break;
                        }
                    }
                }
            }

            // water drawing layer
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.WATERDRAWINGLAYER);
            MapLayer newRealmWaterDrawingLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.WATERDRAWINGLAYER);

            for (int i = 0; i < waterDrawingLayer.MapLayerComponents.Count; i++)
            {
                // construct a new LayerPaintStroke from the existing one
                // and create a new paint surface for it

                if (waterDrawingLayer.MapLayerComponents[i] is LayerPaintStroke lps)
                {
                    LayerPaintStroke newPaintStroke = new()
                    {
                        ParentMap = newRealmMap,
                        StrokeColor = lps.StrokeColor,
                        PaintBrush = lps.PaintBrush,
                        BrushRadius = (int)(lps.BrushRadius * scaleX),
                        MapLayerIdentifier = lps.MapLayerIdentifier,
                        Erase = lps.Erase,
                        Rendered = false,
                    };

                    foreach (LayerPaintStrokePoint point in lps.PaintStrokePoints)
                    {
                        LayerPaintStrokePoint newStrokePoint = new()
                        {
                            StrokeLocation = new SKPoint((point.X * scaleX) + deltaX, (point.Y * scaleY) + deltaY),
                            StrokeRadius = (int)(point.StrokeRadius * scaleX)
                        };

                        newPaintStroke.PaintStrokePoints.Add(newStrokePoint);
                    }

                    SKImageInfo imageInfo = new(newRealmMap.MapWidth, newRealmMap.MapHeight);
                    newPaintStroke.RenderSurface ??= SKSurface.Create(newMapGRContext, false, imageInfo);
                    newRealmWaterDrawingLayer.MapLayerComponents.Add(newPaintStroke);
                }
            }

            // gather the symbols in the selected area
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.SYMBOLLAYER);
            MapLayer newRealmSymbolLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.SYMBOLLAYER);
            List<MapSymbol> gatheredSymbols = [];

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol ms)
                {
                    if (selectedMapArea.Contains(ms.X, ms.Y))
                    {
                        if (includeTerrainSymbols && ms.SymbolType == MapSymbolType.Terrain)
                        {
                            gatheredSymbols.Add(ms);
                        }

                        if (includeVegetationSymbols && ms.SymbolType == MapSymbolType.Vegetation)
                        {
                            gatheredSymbols.Add(ms);
                        }

                        if (includeStructureSymbols && ms.SymbolType == MapSymbolType.Structure)
                        {
                            gatheredSymbols.Add(ms);
                        }
                    }
                }
            }

            // scale the symbols and add them to the detail map
            foreach (MapSymbol ms in gatheredSymbols)
            {
                MapSymbol newSymbol = new(ms)
                {
                    X = (int)((ms.X * scaleX) + deltaX),
                    Y = (int)((ms.Y * scaleY) + deltaY),
                    SymbolWidth = (int)(ms.SymbolWidth * scaleX),
                    SymbolHeight = (int)(ms.SymbolHeight * scaleY),
                };

                Bitmap resizedPlacedBitmap = new(ms.PlacedBitmap.ToBitmap(), newSymbol.SymbolWidth, newSymbol.SymbolHeight);
                newSymbol.PlacedBitmap = resizedPlacedBitmap.ToSKBitmap();

                newRealmSymbolLayer.MapLayerComponents.Add(newSymbol);
            }


            // get map paths

            if (includePaths)
            {
                List<MapPath> gatheredPaths = [];

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.PATHLOWERLAYER);
                MapLayer newRealmPathLowerLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.PATHLOWERLAYER);

                for (int i = 0; i < pathLowerLayer.MapLayerComponents.Count; i++)
                {
                    if (pathLowerLayer.MapLayerComponents[i] is MapPath mp)
                    {
                        foreach (MapPathPoint point in mp.PathPoints)
                        {
                            if (selectedMapArea.Contains(point.MapPoint.X, point.MapPoint.Y))
                            {
                                gatheredPaths.Add(mp);
                                break;
                            }
                        }
                    }
                }

                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.PATHUPPERLAYER);
                MapLayer newRealmPathUpperLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.PATHUPPERLAYER);

                for (int i = 0; i < pathUpperLayer.MapLayerComponents.Count; i++)
                {
                    if (pathUpperLayer.MapLayerComponents[i] is MapPath mp)
                    {
                        foreach (MapPathPoint point in mp.PathPoints)
                        {
                            if (selectedMapArea.Contains(point.MapPoint.X, point.MapPoint.Y))
                            {
                                gatheredPaths.Add(mp);
                                break;
                            }
                        }
                    }
                }

                foreach (MapPath mp in gatheredPaths)
                {
                    if (mp.PathPoints.Count > 0)
                    {
                        MapPath newPath = new(mp)
                        {
                            ParentMap = newRealmMap,
                            PathWidth = mp.PathWidth * scaleX,
                            X = (int)((mp.X * scaleX) + deltaX),
                            Y = (int)((mp.Y * scaleY) + deltaY),
                        };

                        foreach (MapPathPoint point in mp.PathPoints)
                        {
                            MapPathPoint newPathPoint = new()
                            {
                                MapPoint = new SKPoint((point.MapPoint.X * scaleX) + deltaX, (point.MapPoint.Y * scaleY) + deltaY),
                            };

                            newPath.PathPoints.Add(newPathPoint);
                        }

                        if (mp.PathTexture != null)
                        {
                            newPath.PathTexture = new MapTexture
                            {
                                TextureName = mp.PathTexture.TextureName,
                                TexturePath = mp.PathTexture.TexturePath,
                                TextureBitmap = (Bitmap?)Bitmap.FromFile(mp.PathTexture.TexturePath)
                            };
                        }

                        newPath.PathPaint = null;   // force to null so that PathPaint is constructed
                        MapPathMethods.ConstructPathPaint(newPath);

                        if (mp.DrawOverSymbols)
                        {
                            newRealmPathUpperLayer.MapLayerComponents.Add(newPath);
                        }
                        else
                        {
                            newRealmPathLowerLayer.MapLayerComponents.Add(newPath);
                        }
                    }
                }
            }

            if (includeLabels)
            {
                // get labels
                MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.LABELLAYER);
                MapLayer newRealmLabelLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.LABELLAYER);
                List<MapLabel> gatheredLabels = [];

                for (int i = 0; i < labelLayer.MapLayerComponents.Count; i++)
                {
                    if (labelLayer.MapLayerComponents[i] is MapLabel ml)
                    {
                        SKRect mlBoundingRect = new(ml.X, ml.Y, ml.X + ml.Width, ml.Y + ml.Height);
                        if (selectedMapArea.IntersectsWith(mlBoundingRect))
                        {
                            gatheredLabels.Add(ml);
                        }
                    }
                }

                foreach (MapLabel ml in gatheredLabels)
                {
                    MapLabel newLabel = new(ml)
                    {
                        X = (int)((ml.X * scaleX) + deltaX),
                        Y = (int)((ml.Y * scaleY) + deltaY),
                        Height = (int)(ml.Height * scaleX),
                        Width = (int)(ml.Width * scaleY),
                        LabelOutlineWidth = ml.LabelOutlineWidth * scaleX,  // scale
                    };

                    Font labelFont = new(newLabel.LabelFont.FontFamily, newLabel.LabelFont.Size * scaleY,
                        newLabel.LabelFont.Style, GraphicsUnit.Pixel);

                    newLabel.LabelFont = labelFont;

                    SKFont skLabelFont = MapLabelMethods.GetSkLabelFont(labelFont);
                    SKPaint paint = MapLabelMethods.CreateLabelPaint(newLabel.LabelColor);

                    newLabel.LabelPaint = paint;
                    newLabel.LabelSKFont.Dispose();
                    newLabel.LabelSKFont = skLabelFont;

                    if (ml.LabelPath != null)
                    {
                        SKPath transformedLabelPath = new(ml.LabelPath);
                        transformedLabelPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                        newLabel.LabelPath = transformedLabelPath;
                    }

                    newRealmLabelLayer.MapLayerComponents.Add(newLabel);
                }
            }

            if (includeBoxes)
            {
                // get boxes
                MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.BOXLAYER);
                MapLayer newRealmBoxLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.BOXLAYER);
                List<PlacedMapBox> gatheredBoxes = [];

                for (int i = 0; i < boxLayer.MapLayerComponents.Count; i++)
                {
                    if (boxLayer.MapLayerComponents[i] is PlacedMapBox box)
                    {
                        GraphicsUnit unit = GraphicsUnit.Pixel;
                        SKRect mlBoundingRect = box.BoxBitmap.ToBitmap().GetBounds(ref unit).ToSKRect();
                        if (selectedMapArea.IntersectsWith(mlBoundingRect))
                        {
                            gatheredBoxes.Add(box);
                        }
                    }
                }

                foreach (PlacedMapBox box in gatheredBoxes)
                {
                    if (box.BoxBitmap != null)
                    {
                        SKBitmap resizedBitmap = DrawingMethods.ResizeSKBitmap(box.BoxBitmap, new SKSizeI((int)(box.Width * scaleX), (int)(box.Height * scaleY)));

                        PlacedMapBox newBox = new(box)
                        {
                            X = (int)((box.X * scaleX) + deltaX),
                            Y = (int)((box.Y * scaleY) + deltaY),
                            Width = (int)(box.Width * scaleX),
                            Height = (int)(box.Height * scaleY),
                            BoxBitmap = resizedBitmap.Copy(),
                            BoxCenterLeft = box.BoxCenterLeft * scaleX,
                            BoxCenterTop = box.BoxCenterTop * scaleY,
                            BoxCenterRight = box.BoxCenterRight * scaleX,
                            BoxCenterBottom = box.BoxCenterBottom * scaleY,
                        };

                        SKRectI center = new((int)newBox.BoxCenterLeft,
                            (int)newBox.BoxCenterTop,
                            (int)(newBox.Width - newBox.BoxCenterRight),
                            (int)(newBox.Height - newBox.BoxCenterBottom));

                        if (center.IsEmpty || center.Left < 0 || center.Right <= 0 || center.Top < 0 || center.Bottom <= 0)
                        {
                            center.Left = 0;
                            center.Right = 0;
                            center.Top = 0;
                            center.Bottom = 0;
                        }
                        else if (center.Width <= 0 || center.Height <= 0)
                        {
                            // swap 
                            if (center.Right < center.Left)
                            {
                                (center.Left, center.Right) = (center.Right, center.Left);
                            }

                            if (center.Bottom < center.Top)
                            {
                                (center.Top, center.Bottom) = (center.Bottom, center.Top);
                            }
                        }

                        SKBitmap[] bitmapSlices = DrawingMethods.SliceNinePatchBitmap(newBox.BoxBitmap, center);

                        newBox.PatchA = bitmapSlices[0].Copy();   // top-left corner
                        newBox.PatchB = bitmapSlices[1].Copy();   // top
                        newBox.PatchC = bitmapSlices[2].Copy();   // top-right corner
                        newBox.PatchD = bitmapSlices[3].Copy();   // left size
                        newBox.PatchE = bitmapSlices[4].Copy();   // middle
                        newBox.PatchF = bitmapSlices[5].Copy();   // right side
                        newBox.PatchG = bitmapSlices[6].Copy();   // bottom-left corner
                        newBox.PatchH = bitmapSlices[7].Copy();   // bottom
                        newBox.PatchI = bitmapSlices[8].Copy();   // bottom-right corner

                        newRealmBoxLayer.MapLayerComponents.Add(newBox);
                    }
                }
            }

            if (includeScale)
            {
                // get scale
                MapLayer scaleLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OVERLAYLAYER);
                MapLayer newRealmScaleLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OVERLAYLAYER);

                // there is only one map scale
                MapScale? mapScale = scaleLayer.MapLayerComponents.Cast<MapScale>().FirstOrDefault();

                if (mapScale != null)
                {
                    MapScale newScale = new(mapScale)
                    {
                        // initial position of the scale is near the bottom-left corner of the map
                        X = 100,
                        Y = newRealmMap.MapHeight - 100,
                        Width = (int)(mapScale.Width * scaleX),
                        Height = (int)(mapScale.Height * scaleY),
                    };

                    newRealmScaleLayer.MapLayerComponents.Add(newScale);
                }
            }

            if (includeGrid)
            {
                // get grid
                MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.DEFAULTGRIDLAYER);
                MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.ABOVEOCEANGRIDLAYER);
                MapLayer belowSymbolsGridLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER);

                // there is only one map grid                                
                MapGrid? mapGrid = defaultGridLayer.MapLayerComponents.Cast<MapGrid>().FirstOrDefault();

                mapGrid ??= aboveOceanGridLayer.MapLayerComponents.Cast<MapGrid>().FirstOrDefault();

                mapGrid ??= belowSymbolsGridLayer.MapLayerComponents.Cast<MapGrid>().FirstOrDefault();

                if (mapGrid != null)
                {
                    MapGrid newGrid = new(mapGrid)
                    {
                        ParentMap = newRealmMap,
                        GridEnabled = true,
                    };

                    newGrid.GridPaint = new()
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = newGrid.GridColor.ToSKColor(),
                        StrokeWidth = newGrid.GridLineWidth,
                        StrokeJoin = SKStrokeJoin.Bevel
                    };

                    MapBuilder.GetMapLayerByIndex(newRealmMap, newGrid.GridLayerIndex).MapLayerComponents.Add(newGrid);
                }
            }

            if (includeRegions)
            {
                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.REGIONLAYER);
                MapLayer newRealmRegionLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.REGIONLAYER);

                for (int i = 0; i < regionLayer.MapLayerComponents.Count; i++)
                {
                    if (regionLayer.MapLayerComponents[i] is MapRegion mr)
                    {
                        foreach (MapRegionPoint mrp in mr.MapRegionPoints)
                        {
                            if (selectedMapArea.Contains(mrp.RegionPoint))
                            {
                                MapRegion newRegion = new(mr)
                                {
                                    ParentMap = newRealmMap,
                                };

                                foreach (MapRegionPoint point in mr.MapRegionPoints)
                                {
                                    MapRegionPoint newRegionPoint = new()
                                    {
                                        RegionPoint = new SKPoint((point.RegionPoint.X * scaleX) + deltaX, (point.RegionPoint.Y * scaleY) + deltaY),
                                    };

                                    newRegion.MapRegionPoints.Add(newRegionPoint);
                                }

                                SKPathEffect? regionBorderEffect = MapRegionMethods.ConstructRegionBorderEffect(newRegion);
                                MapRegionMethods.ConstructRegionPaintObjects(newRegion, regionBorderEffect);

                                newRealmRegionLayer.MapLayerComponents.Add(newRegion);
                                break;
                            }
                        }
                    }
                }
            }

            // vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.VIGNETTELAYER);
            MapLayer newRealmVignetteLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.VIGNETTELAYER);

            foreach (MapVignette mv in vignetteLayer.MapLayerComponents.Cast<MapVignette>())
            {
                mv.ParentMap = newRealmMap;
                newRealmVignetteLayer.MapLayerComponents.Add(mv);
            }

            return newRealmMap;
        }

        internal static void ExportMapLayersAsZipFile(FileStream fileStream, RealmStudioMap map, RealmMapExportFormat exportFormat)
        {
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Update, true);

            ImageConverter converter = new();

            SKSurface s = SKSurface.Create(new SKImageInfo(map.MapWidth, map.MapHeight));
            s.Canvas.Clear();

            // background
            MapRenderMethods.RenderBackgroundForExport(map, s.Canvas);
            Bitmap bitmap = s.Snapshot().ToBitmap();

            byte[]? bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "background" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // ocean layers
            MapRenderMethods.RenderOceanForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "ocean" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // wind roses
            MapRenderMethods.RenderWindrosesForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "windroses" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // lower grid layer (above ocean)
            MapRenderMethods.RenderLowerGridForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "gridlower" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // landforms
            MapRenderMethods.RenderLandformsForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "landforms" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // water features
            MapRenderMethods.RenderWaterFeaturesForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "waterfeatures" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // upper grid layer (above water features)
            MapRenderMethods.RenderUpperGridForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "gridupper" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // lower path layer
            MapRenderMethods.RenderLowerMapPathsForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "pathslower" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // symbol layer
            MapRenderMethods.RenderSymbolsForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "symbols" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // upper path layer
            MapRenderMethods.RenderUpperMapPathsForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "pathsupper" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // region and region overlay layers
            MapRenderMethods.RenderRegionsForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "regions" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // default grid layer
            MapRenderMethods.RenderDefaultGridForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "griddefault" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // box layer
            MapRenderMethods.RenderBoxesForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "boxes" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // label layer
            MapRenderMethods.RenderLabelsForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "labels" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // overlay layer (map scale)
            MapRenderMethods.RenderOverlaysForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "overlay" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // render frame
            MapRenderMethods.RenderFrameForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "frame" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // measure layer
            MapRenderMethods.RenderMeasuresForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "measures" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();

            // TODO: drawing layer
            //MapRenderMethods.RenderDrawingForExport(CURRENT_MAP, s.Canvas);

            // vignette layer
            MapRenderMethods.RenderVignetteForExport(map, s.Canvas);
            bitmap = s.Snapshot().ToBitmap();
            bitmapBytes = (byte[]?)converter.ConvertTo(bitmap, typeof(byte[]));

            if (bitmapBytes != null)
            {
                var fileName = "vignette" + "." + exportFormat.ToString().ToLowerInvariant();
                var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(bitmapBytes, 0, bitmapBytes.Length);
            }

            s.Canvas.Clear();
        }
    }
}
