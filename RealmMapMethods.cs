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
    internal class RealmMapMethods
    {
        internal static RealmStudioMap? CreateDetailMap(RealmStudioMainForm mainForm, RealmStudioMap currentMap, SKRect selectedArea)
        {
            if (selectedArea.IsEmpty)
            {
                MessageBox.Show("Please use the Area button on the Land tab to select an area for the detail map.", "Select a Map Area");
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

        // TODO: add values indicating whether to include frames, boxes, grid, regions, windroses, map scale, etc.
        // regions that are attached to a landform contour might be tricky to figure out; hopefully, scale and translation of path will work
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

                                Landform newLandform = new()
                                {
                                    ParentMap = newRealmMap,
                                    DrawPath = transformedPath,
                                    CoastlineColor = lf.CoastlineColor,
                                    CoastlineEffectDistance = lf.CoastlineEffectDistance,
                                    CoastlineFillPaint = lf.CoastlineFillPaint,
                                    CoastlineHatchBlendMode = lf.CoastlineHatchBlendMode,
                                    CoastlineHatchOpacity = lf.CoastlineHatchOpacity,
                                    CoastlineHatchScale = lf.CoastlineHatchScale,
                                    CoastlinePaint = lf.CoastlinePaint,
                                    CoastlineStyleName = lf.CoastlineStyleName,
                                    DashShader = lf.DashShader,
                                    LandformFillColor = lf.LandformFillColor,
                                    LandformFillPaint = lf.LandformFillPaint,
                                    LandformGradientPaint = lf.LandformGradientPaint,
                                    LandformName = lf.LandformName,
                                    LandformOutlineColor = lf.LandformOutlineColor,
                                    LandformOutlineWidth = lf.LandformOutlineWidth,
                                    LandformTexture = lf.LandformTexture,
                                    LineHatchBitmapShader = lf.LineHatchBitmapShader,
                                    PaintCoastlineGradient = lf.PaintCoastlineGradient,
                                    ShorelineStyle = lf.ShorelineStyle,
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

                                WaterFeature newRealmWf = new()
                                {
                                    WaterFeaturePath = transformedWfPath,
                                    ShallowWaterPaint = wf.ShallowWaterPaint,
                                    ShorelineEffectDistance = wf.ShorelineEffectDistance,
                                    WaterFeatureName = wf.WaterFeatureName,
                                    ParentMap = newRealmMap,
                                    WaterFeatureColor = wf.WaterFeatureColor,
                                    WaterFeatureShorelineColor = wf.WaterFeatureShorelineColor,
                                    WaterFeatureType = wf.WaterFeatureType,
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
                            River newRealmRiver = new()
                            {
                                MapRiverName = r.MapRiverName,
                                ParentMap = newRealmMap,
                                RiverColor = r.RiverColor,
                                RiverShorelineColor = r.RiverShorelineColor,
                                RiverSourceFadeIn = r.RiverSourceFadeIn,
                                RiverWidth = r.RiverWidth * scaleX,      // scale up?
                                ShorelineEffectDistance = r.ShorelineEffectDistance,
                                RiverBoundaryPath = r.RiverBoundaryPath,
                                RiverPath = r.RiverPath,
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
                        if (includeTerrainSymbols && ms.SymbolType == SymbolTypeEnum.Terrain)
                        {
                            gatheredSymbols.Add(ms);
                        }

                        if (includeVegetationSymbols && ms.SymbolType == SymbolTypeEnum.Vegetation)
                        {
                            gatheredSymbols.Add(ms);
                        }

                        if (includeStructureSymbols && ms.SymbolType == SymbolTypeEnum.Structure)
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
                    SymbolPaint = ms.SymbolPaint,
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
                        MapPath newPath = new()
                        {
                            DrawOverSymbols = mp.DrawOverSymbols,
                            IsSelected = false,
                            MapPathName = mp.MapPathName,
                            ParentMap = newRealmMap,
                            PathColor = mp.PathColor,
                            PathWidth = mp.PathWidth * scaleX,
                            X = (int)((mp.X * scaleX) + deltaX),
                            Y = (int)((mp.Y * scaleY) + deltaY),
                            PathType = mp.PathType,
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
                    MapLabel newLabel = new()
                    {
                        X = (int)((ml.X * scaleX) + deltaX),
                        Y = (int)((ml.Y * scaleY) + deltaY),
                        Height = (int)(ml.Height * scaleX),
                        Width = (int)(ml.Width * scaleY),
                        LabelColor = ml.LabelColor,
                        LabelFont = ml.LabelFont,
                        LabelGlowColor = ml.LabelGlowColor,
                        LabelGlowStrength = ml.LabelGlowStrength,
                        LabelOutlineColor = ml.LabelOutlineColor,
                        LabelOutlineWidth = ml.LabelOutlineWidth * scaleX,  // scale
                        LabelPaint = ml.LabelPaint?.Clone(),
                        LabelPath = ml.LabelPath,
                        LabelRotationDegrees = ml.LabelRotationDegrees,
                        LabelSKFont = ml.LabelSKFont,
                        LabelText = ml.LabelText,
                        RenderComponent = ml.RenderComponent,
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
                        SKBitmap resizedBitmap = DrawingMethods.ResizeBitmap(box.BoxBitmap, new SKSizeI((int)(box.Width * scaleX), (int)(box.Height * scaleY)));

                        PlacedMapBox newBox = new()
                        {
                            X = (int)((box.X * scaleX) + deltaX),
                            Y = (int)((box.Y * scaleY) + deltaY),
                            Width = (int)(box.Width * scaleX),
                            Height = (int)(box.Height * scaleY),
                            BoxBitmap = resizedBitmap.Copy(),
                            BoxTint = box.BoxTint,
                            BoxPaint = box.BoxPaint?.Clone(),
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
                    MapScale newScale = new()
                    {
                        Width = (int)(mapScale.Width * scaleX),
                        Height = (int)(mapScale.Height * scaleY),
                        ScaleSegmentCount = mapScale.ScaleSegmentCount,
                        ScaleLineWidth = mapScale.ScaleLineWidth,
                        ScaleColor1 = mapScale.ScaleColor1,
                        ScaleColor2 = mapScale.ScaleColor2,
                        ScaleColor3 = mapScale.ScaleColor3,
                        ScaleDistance = mapScale.ScaleDistance,
                        ScaleDistanceUnit = mapScale.ScaleDistanceUnit,
                        ScaleFontColor = mapScale.ScaleFontColor,
                        ScaleOutlineWidth = mapScale.ScaleOutlineWidth,
                        ScaleOutlineColor = mapScale.ScaleOutlineColor,
                        ScaleFont = mapScale.ScaleFont,
                        ScaleNumbersDisplayType = mapScale.ScaleNumbersDisplayType,
                        // initial position of the scale is near the bottom-left corner of the map
                        X = 100,
                        Y = newRealmMap.MapHeight - 100
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
                    MapGrid newGrid = new()
                    {
                        ParentMap = newRealmMap,
                        GridEnabled = true,
                        GridColor = mapGrid.GridColor,
                        GridLineWidth = mapGrid.GridLineWidth,
                        GridSize = mapGrid.GridSize,
                        GridType = mapGrid.GridType,
                        Width = newRealmMap.MapWidth,
                        Height = newRealmMap.MapHeight,
                        GridLayerIndex = mapGrid.GridLayerIndex,
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
                                MapRegion newRegion = new()
                                {
                                    ParentMap = newRealmMap,
                                    RegionBorderColor = mr.RegionBorderColor,
                                    RegionBorderSmoothing = mr.RegionBorderSmoothing,
                                    RegionBorderType = mr.RegionBorderType,
                                    RegionBorderWidth = mr.RegionBorderWidth,
                                    RegionInnerOpacity = mr.RegionInnerOpacity,
                                    RegionName = mr.RegionName,
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

    }
}
