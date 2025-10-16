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
using RealmStudio.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.IO;
using System.IO.Compression;

namespace RealmStudio
{
    internal sealed class RealmMapMethods
    {
        internal static int GetNewBrushSize(TrackBar brushSizeTrack, int sizeDelta)
        {
            int newValue = brushSizeTrack.Value + sizeDelta;
            newValue = Math.Max(brushSizeTrack.Minimum, Math.Min(newValue, brushSizeTrack.Maximum));

            brushSizeTrack.Value = newValue;

            return newValue;
        }

        internal static List<MapComponent> SelectMapComponentsInArea(RealmStudioMap map, SKRect selectedArea)
        {
            List<MapComponent> selectedMapComponents = [];

            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            for (int i = symbolLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol ms)
                {
                    if (selectedArea.Contains(ms.X, ms.Y))
                    {
                        selectedMapComponents.Add(ms);
                    }
                }
            }

            for (int i = pathLowerLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (pathLowerLayer.MapLayerComponents[i] is MapPath mp)
                {
                    mp.BoundaryPath = PathManager.GenerateMapPathBoundaryPath(mp.PathPoints);
                    SKRect pathBounds = mp.BoundaryPath.ComputeTightBounds();

                    if (selectedArea.Contains(pathBounds))
                    {
                        selectedMapComponents.Add(mp);
                    }
                }
            }

            for (int i = pathUpperLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (pathUpperLayer.MapLayerComponents[i] is MapPath mp)
                {
                    mp.BoundaryPath = PathManager.GenerateMapPathBoundaryPath(mp.PathPoints);
                    SKRect pathBounds = mp.BoundaryPath.ComputeTightBounds();

                    if (selectedArea.Contains(pathBounds))
                    {
                        selectedMapComponents.Add(mp);
                    }
                }
            }

            for (int i = waterLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (waterLayer.MapLayerComponents[i] is IWaterFeature iwf)
                {
                    if (iwf is WaterFeature wf)
                    {
                        SKRect pathBounds = wf.ContourPath.ComputeTightBounds();

                        if (selectedArea.Contains(pathBounds))
                        {
                            selectedMapComponents.Add(wf);
                        }
                    }
                    else if (iwf is River mr)
                    {
                        if (mr.RiverBoundaryPath != null && mr.RiverBoundaryPath.PointCount > 0)
                        {
                            SKRect pathBounds = mr.RiverBoundaryPath.ComputeTightBounds();

                            if (selectedArea.Contains(pathBounds))
                            {
                                selectedMapComponents.Add(mr);
                            }
                        }
                    }
                }
            }

            return selectedMapComponents;
        }

        internal static void SaveRealmFileBackup(string filepath)
        {
            string autosaveDirectory = Settings.Default.AutosaveDirectory;

            if (string.IsNullOrEmpty(autosaveDirectory))
            {
                autosaveDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
            }

            string fileNameNoExtension = Path.GetFileNameWithoutExtension(filepath);
            string saveTime = DateTime.Now.ToFileTimeUtc().ToString();

            string saveFilename = fileNameNoExtension + "_" + saveTime + ".rsmapx";
            string autosaveFullPath = autosaveDirectory + Path.DirectorySeparatorChar + saveFilename;

            File.Copy(filepath, autosaveFullPath, true);
        }

        internal static bool SaveRealmBackup(RealmStudioMap map, bool useMapNameForBackup = false)
        {
            string currentmapFileName = map.MapPath;

            try
            {
                // realm autosave folder (location where map backups are saved during autosave)
                string autosaveDirectory = Settings.Default.AutosaveDirectory;

                if (string.IsNullOrEmpty(autosaveDirectory))
                {
                    autosaveDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
                }

                string autosaveFilename = map.MapGuid.ToString();

                if (useMapNameForBackup)
                {
                    autosaveFilename = map.MapName;
                }

                string saveTime = DateTime.Now.ToFileTimeUtc().ToString();

                autosaveFilename += "_" + saveTime + ".rsmapx";

                string autosaveFullPath = autosaveDirectory + Path.DirectorySeparatorChar + autosaveFilename;

                map.MapPath = autosaveFullPath;

                MapFileMethods.SaveMap(map);

                return true;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
                MessageBox.Show("An error has occurred while saving a backup copy of the map.", "Map Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            finally
            {
                map.MapPath = currentmapFileName;
            }

            return false;
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
            bool includeRegions,
            bool includeDrawnShapes,
            bool includeHeightMap)
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


            // get the landforms and drawn shapes within or intersecting the selected area, then translate and scale them
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

                                LandformManager.CreateAllPathsFromDrawnPath(newRealmMap, newLandform);
                                newRealmLandformLayer.MapLayerComponents.Add(newLandform);

                                break;
                            }
                        }
                    }
                }
                else if (landformLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);

                        if (newDmc != null)
                        {
                            newRealmLandformLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
                }
            }

            // go through the current map to get textures, painted colors, etc. and assign them to the detail map

            // texture needs to be resized to new map size
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.BASELAYER);
            MapLayer newRealmBaseLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.BASELAYER);

            for (int i = 0; i < baseLayer.MapLayerComponents.Count; i++)
            {
                if (baseLayer.MapLayerComponents[i] is MapImage mi)
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
                else if (baseLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmBaseLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
                }
            }

            // texture needs to be resized to new map size
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANTEXTURELAYER);
            MapLayer newRealmOceanTextureLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OCEANTEXTURELAYER);

            for (int i = 0; i < oceanTextureLayer.MapLayerComponents.Count; i++)
            {
                if (oceanTextureLayer.MapLayerComponents[i] is MapImage mi)
                {
                    Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), newRealmMap.MapWidth, newRealmMap.MapHeight);

                    MapImage BackgroundTexture = new()
                    {
                        Width = newRealmMap.MapWidth,
                        Height = newRealmMap.MapHeight,
                        MapImageBitmap = resizedBitmap.ToSKBitmap(),
                    };

                    newRealmOceanTextureLayer.MapLayerComponents.Add(BackgroundTexture);
                }
                else if (oceanTextureLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmOceanTextureLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
                }
            }


            // texture needs to be resized to new map size
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer newRealmOceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            for (int i = 0; i < oceanTextureOverlayLayer.MapLayerComponents.Count; i++)
            {
                if (oceanTextureOverlayLayer.MapLayerComponents[i] is MapImage mi)
                {
                    Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), newRealmMap.MapWidth, newRealmMap.MapHeight);

                    MapImage BackgroundTexture = new()
                    {
                        Width = newRealmMap.MapWidth,
                        Height = newRealmMap.MapHeight,
                        MapImageBitmap = resizedBitmap.ToSKBitmap(),
                    };

                    newRealmOceanTextureOverlayLayer.MapLayerComponents.Add(BackgroundTexture);
                }
                else if (oceanTextureOverlayLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmOceanTextureOverlayLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
                }
            }


            // ocean drawing layer
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANDRAWINGLAYER);
            MapLayer newRealmOceanDrawingLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OCEANDRAWINGLAYER);

            for (int i = 0; i < oceanDrawingLayer.MapLayerComponents.Count; i++)
            {
                if (oceanDrawingLayer.MapLayerComponents[i] is LayerPaintStroke lps)
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
                else if (oceanDrawingLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmOceanDrawingLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
                }
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
                else if (landDrawingLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmLandDrawingLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
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

                                WaterFeatureManager.CreateInnerAndOuterPaths(newRealmMap, newRealmWf);
                                WaterFeatureManager.ConstructWaterFeaturePaintObjects(newRealmWf);

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

                            WaterFeatureManager.ConstructRiverPaintObjects(newRealmRiver);
                            WaterFeatureManager.ConstructRiverPaths(newRealmRiver);

                            newRealmWaterLayer.MapLayerComponents.Add(newRealmRiver);

                            break;
                        }
                    }
                }
                else if (waterLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmWaterLayer.MapLayerComponents.Add(newDmc);
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
                else if (waterDrawingLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmWaterDrawingLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
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
                else if (symbolLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds) && includeDrawnShapes)
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmSymbolLayer.MapLayerComponents.Add(newDmc);
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
                    else if (pathLowerLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmPathLowerLayer.MapLayerComponents.Add(newDmc);
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
                    else if (pathUpperLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmPathUpperLayer.MapLayerComponents.Add(newDmc);
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
                        PathManager.ConstructPathPaint(newPath);

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
                    else if (labelLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmLabelLayer.MapLayerComponents.Add(newDmc);
                            }
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

                    SKFont skLabelFont = LabelManager.GetSkLabelFont(labelFont);
                    SKPaint paint = LabelManager.CreateLabelPaint(newLabel.LabelColor);

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
                    else if (boxLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmBoxLayer.MapLayerComponents.Add(newDmc);
                            }
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

                        newRealmBoxLayer.MapLayerComponents.Add(newBox);
                    }
                }
            }

            if (includeScale)
            {
                // get scale
                MapLayer scaleLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OVERLAYLAYER);
                MapLayer newRealmScaleLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.OVERLAYLAYER);

                for (int i = 0; i < scaleLayer.MapLayerComponents.Count; i++)
                {
                    if (scaleLayer.MapLayerComponents[i] is MapScale ms)
                    {
                        MapScale newScale = new(ms)
                        {
                            // initial position of the scale is near the bottom-left corner of the map
                            X = 100,
                            Y = newRealmMap.MapHeight - 100,
                            Width = (int)(ms.Width * scaleX),
                            Height = (int)(ms.Height * scaleY),
                        };

                        newRealmScaleLayer.MapLayerComponents.Add(newScale);
                    }
                    else if (scaleLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmScaleLayer.MapLayerComponents.Add(newDmc);
                            }
                        }
                    }
                }
            }

            if (includeGrid)
            {
                // get grid
                MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.DEFAULTGRIDLAYER);
                MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.ABOVEOCEANGRIDLAYER);
                MapLayer belowSymbolsGridLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER);

                for (int i = 0; i < defaultGridLayer.MapLayerComponents.Count; i++)
                {
                    if (defaultGridLayer.MapLayerComponents[i] is MapGrid mapGrid)
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
                    else if (defaultGridLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.Add(newDmc);
                            }
                        }
                    }
                }

                for (int i = 0; i < aboveOceanGridLayer.MapLayerComponents.Count; i++)
                {
                    if (aboveOceanGridLayer.MapLayerComponents[i] is MapGrid mapGrid)
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
                    else if (aboveOceanGridLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.Add(newDmc);
                            }
                        }
                    }
                }

                for (int i = 0; i < belowSymbolsGridLayer.MapLayerComponents.Count; i++)
                {
                    if (belowSymbolsGridLayer.MapLayerComponents[i] is MapGrid mapGrid)
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
                    else if (belowSymbolsGridLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.Add(newDmc);
                            }
                        }
                    }
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

                                SKPathEffect? regionBorderEffect = RegionManager.ConstructRegionBorderEffect(newRegion);
                                RegionManager.ConstructRegionPaintObjects(newRegion, regionBorderEffect);

                                newRealmRegionLayer.MapLayerComponents.Add(newRegion);
                                break;
                            }
                        }
                    }
                    else if (regionLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmRegionLayer.MapLayerComponents.Add(newDmc);
                            }
                        }
                    }
                }
            }

            if (includeHeightMap)
            {
                MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.HEIGHTMAPLAYER);

                if (heightMapLayer.MapLayerComponents.Count == 2)
                {
                    MapLayer newHeightMapLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.HEIGHTMAPLAYER);

                    using SKBitmap b = new(new SKImageInfo(newRealmMap.MapWidth, newRealmMap.MapHeight));
                    using SKCanvas canvas = new(b);

                    canvas.Clear(SKColors.Black);

                    for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
                    {
                        if (landformLayer.MapLayerComponents[i] is Landform l)
                        {
                            l.RenderLandformForHeightMap(canvas);
                        }
                    }

                    MapImage landformImage = new()
                    {
                        MapImageBitmap = b.Copy()
                    };

                    newHeightMapLayer.MapLayerComponents.Add(landformImage);

                    if (heightMapLayer.MapLayerComponents[1] is MapHeightMap mhm)
                    {
                        Bitmap resizedBitmap = new(mhm.MapImageBitmap.ToBitmap(), newRealmMap.MapWidth, newRealmMap.MapHeight);

                        MapHeightMap heightMap = new()
                        {
                            Width = newRealmMap.MapWidth,
                            Height = newRealmMap.MapHeight,
                            MapImageBitmap = resizedBitmap.ToSKBitmap(),
                        };

                        newHeightMapLayer.MapLayerComponents.Add(heightMap);
                    }
                }
            }

            if (includeDrawnShapes)
            {
                // get drawings
                MapLayer drawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.DRAWINGLAYER);
                MapLayer newRealmDrawingLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.DRAWINGLAYER);
                for (int i = 0; i < drawingLayer.MapLayerComponents.Count; i++)
                {
                    if (drawingLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                    {
                        if (selectedMapArea.IntersectsWith(dmc.Bounds))
                        {
                            DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                            if (newDmc != null)
                            {
                                newRealmDrawingLayer.MapLayerComponents.Add(newDmc);
                            }
                        }
                    }
                }
            }


            // vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.VIGNETTELAYER);
            MapLayer newRealmVignetteLayer = MapBuilder.GetMapLayerByIndex(newRealmMap, MapBuilder.VIGNETTELAYER);

            for (int i = 0; i < vignetteLayer.MapLayerComponents.Count; i++)
            {
                if (vignetteLayer.MapLayerComponents[i] is MapVignette mv)
                {
                    RealmStudioMainForm? mainForm = UtilityMethods.GetMainForm();

                    if (mainForm != null)
                    {
                        MapVignette newVignette = new()
                        {
                            ParentMap = newRealmMap,
                            VignetteColor = mv.VignetteColor,
                            VignetteShape = mv.VignetteShape,
                            VignetteStrength = mv.VignetteStrength,
                            X = 0,
                            Y = 0,
                            Width = newRealmMap.MapWidth,
                            Height = newRealmMap.MapHeight,
                            VignetteRenderSurface = SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false,
                                        new SKImageInfo(newRealmMap.MapWidth, newRealmMap.MapHeight)),
                        };

                        newRealmVignetteLayer.MapLayerComponents.Add(newVignette);
                    }
                }
                else if (vignetteLayer.MapLayerComponents[i] is DrawnMapComponent dmc)
                {
                    if (selectedMapArea.IntersectsWith(dmc.Bounds))
                    {
                        DrawnMapComponent? newDmc = DrawingManager.CreateScaledTransformedDrawnComponent(dmc, scaleX, scaleY, deltaX, deltaY);
                        if (newDmc != null)
                        {
                            newRealmVignetteLayer.MapLayerComponents.Add(newDmc);
                        }
                    }
                }
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

        internal static void AddMapImagesToHeightMapLayer(RealmStudioMap map)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.HEIGHTMAPLAYER);

            if (heightMapLayer.MapLayerComponents.Count == 2)
            {
                // heightmap images have already been created
                return;
            }

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Black);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform l)
                {
                    l.RenderLandformForHeightMap(canvas);
                }
            }

            heightMapLayer.MapLayerComponents.Clear();

            MapImage landformImage = new()
            {
                MapImageBitmap = b.Copy()
            };

            heightMapLayer.MapLayerComponents.Add(landformImage);

            using SKBitmap b2 = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            b2.Erase(SKColors.Transparent);

            MapHeightMap heightMap = new()
            {
                HeightMap = new float[map.MapWidth, map.MapHeight],
                MapImageBitmap = b2.Copy(),
            };

            heightMapLayer.MapLayerComponents.Add(heightMap);
        }

        internal static void PruneOldBackupsOfMap(RealmStudioMap map, int backupCount)
        {
            // realm autosave folder (location where map backups are saved during autosave)
            string defaultAutosaveFolder = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;

            string autosaveDirectory = Settings.Default.AutosaveDirectory;

            if (string.IsNullOrEmpty(autosaveDirectory))
            {
                autosaveDirectory = defaultAutosaveFolder;
            }

            string autosaveFilename = map.MapGuid.ToString();

            string oldestFilePath = string.Empty;
            DateTime? oldestCreationDateTime = null;

            var files = from file in Directory.EnumerateFiles(autosaveDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(autosaveFilename)
                        select new
                        {
                            File = file
                        };

            // keep 5 backups of the map
            if (files.Count() >= backupCount)
            {
                foreach (var f in files)
                {
                    DateTime creationDateTime = File.GetCreationTimeUtc(f.File);
                    string path = Path.GetFullPath(f.File);

                    if (string.IsNullOrEmpty(oldestFilePath) || creationDateTime < oldestCreationDateTime)
                    {
                        oldestFilePath = path;
                        oldestCreationDateTime = creationDateTime;
                    }
                }

                if (!string.IsNullOrEmpty(oldestFilePath)
                    && File.Exists(oldestFilePath)
                    && oldestFilePath.Contains("autosave")
                    && oldestFilePath.StartsWith(autosaveDirectory)
                    && oldestFilePath.EndsWith(".rsmapx"))
                {
                    try
                    {
                        File.Delete(oldestFilePath);
                    }
                    catch (Exception ex)
                    {
                        Program.LOGGER.Error(ex);
                        throw new Exception("Prune backup failed");
                    }
                }
            }
        }

        public static void DeselectAllMapComponents(RealmStudioMap map, MapComponent? selectedComponent)
        {
            // when components are deselected, the MapStateMediator selected objexct has to be updated

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform landform)
                {
                    if (selectedComponent != null && selectedComponent is Landform lf && lf.LandformGuid.ToString() == landform.LandformGuid.ToString())
                    {
                        continue;
                    }
                    else if (landform.IsSelected)
                    {
                        landform.IsSelected = false;
                        MapStateMediator.SelectedLandform = null;
                    }
                }
                else
                {
                    landformLayer.MapLayerComponents[i].IsSelected = false;
                }
            }

            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            for (int i = 0; i < waterLayer.MapLayerComponents.Count; i++)
            {
                if (waterLayer.MapLayerComponents[i] is WaterFeature waterfeature)
                {
                    if (selectedComponent != null && selectedComponent is WaterFeature wf && wf.WaterFeatureGuid.ToString() == waterfeature.WaterFeatureGuid.ToString())
                    {
                        continue;
                    }
                    else if (waterfeature.IsSelected)
                    {
                        waterfeature.IsSelected = false;
                        MapStateMediator.SelectedWaterFeature = null;
                    }
                }
                else if (waterLayer.MapLayerComponents[i] is River river)
                {
                    if (selectedComponent != null && selectedComponent is River r && r.MapRiverGuid.ToString() == river.MapRiverGuid.ToString())
                    {
                        continue;
                    }
                    else if (river.IsSelected)
                    {
                        river.IsSelected = false;
                        MapStateMediator.SelectedWaterFeature = null;
                    }
                }
                else
                {
                    waterLayer.MapLayerComponents[i].IsSelected = false;
                }
            }


            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);

            for (int i = 0; i < pathUpperLayer.MapLayerComponents.Count; i++)
            {
                if (pathUpperLayer.MapLayerComponents[i] is MapPath mapPath)
                {
                    if (selectedComponent != null && selectedComponent is MapPath mp && mp == mapPath)
                    {
                        continue;
                    }
                    else if (mapPath.IsSelected)
                    {
                        mapPath.IsSelected = false;
                        mapPath.ShowPathPoints = false;
                        MapStateMediator.SelectedMapPath = null;
                    }
                }
                else
                {
                    pathUpperLayer.MapLayerComponents[i].IsSelected = false;
                }
            }


            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);

            for (int i = 0; i < pathLowerLayer.MapLayerComponents.Count; i++)
            {
                if (pathLowerLayer.MapLayerComponents[i] is MapPath mapPath)
                {
                    if (selectedComponent != null && selectedComponent is MapPath mp && mp == mapPath)
                    {
                        continue;
                    }
                    else if (mapPath.IsSelected)
                    {
                        mapPath.IsSelected = false;
                        mapPath.ShowPathPoints = false;
                        MapStateMediator.SelectedMapPath = null;
                    }
                }
                else
                {
                    pathLowerLayer.MapLayerComponents[i].IsSelected = false;
                }
            }


            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol symbol)
                {
                    if (selectedComponent != null && selectedComponent is MapSymbol s && s == symbol)
                    {
                        continue;
                    }
                    else if (symbol.IsSelected)
                    {
                        symbol.IsSelected = false;
                        MapStateMediator.SelectedMapSymbol = null;
                    }
                }
                else
                {
                    symbolLayer.MapLayerComponents[i].IsSelected = false;
                }
            }


            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            for (int i = 0; i < labelLayer.MapLayerComponents.Count; i++)
            {
                if (labelLayer.MapLayerComponents[i] is MapLabel label)
                {
                    if (selectedComponent != null && selectedComponent is MapLabel l && l == label)
                    {
                        continue;
                    }
                    else if (label.IsSelected)
                    {
                        label.IsSelected = false;
                        MapStateMediator.SelectedMapLabel = null;
                    }
                }
                else
                {
                    labelLayer.MapLayerComponents[i].IsSelected = false;
                }
            }

            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);

            for (int i = 0; i < boxLayer.MapLayerComponents.Count; i++)
            {
                if (boxLayer.MapLayerComponents[i] is PlacedMapBox box)
                {
                    if (selectedComponent != null && selectedComponent is PlacedMapBox b && b.BoxGuid.ToString() == box.BoxGuid.ToString())
                    {
                        continue;
                    }
                    else if (box.IsSelected)
                    {
                        box.IsSelected = false;
                    }
                }
                else
                {
                    boxLayer.MapLayerComponents[i].IsSelected = false;
                }
            }


            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

            for (int i = 0; i < regionLayer.MapLayerComponents.Count; i++)
            {
                if (regionLayer.MapLayerComponents[i] is MapRegion region)
                {
                    if (selectedComponent != null && selectedComponent is MapRegion r && r.RegionGuid.ToString() == region.RegionGuid.ToString())
                    {
                        continue;
                    }
                    else if (region.IsSelected)
                    {
                        region.IsSelected = false;
                    }
                }
                else
                {
                    regionLayer.MapLayerComponents[i].IsSelected = false;
                }
            }

            MapLayer drawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DRAWINGLAYER);

            for (int i = 0; i < drawingLayer.MapLayerComponents.Count; i++)
            {
                if (drawingLayer.MapLayerComponents[i] is DrawnMapComponent drawnMapComponent)
                {
                    if (selectedComponent != null && selectedComponent is DrawnMapComponent dmc && dmc == drawnMapComponent)
                    {
                        continue;
                    }
                    else if (drawnMapComponent.IsSelected)
                    {
                        drawnMapComponent.IsSelected = false;
                        MapStateMediator.SelectedDrawnMapComponent = null;
                    }
                }
                else
                {
                    drawingLayer.MapLayerComponents[i].IsSelected = false;
                }
            }


            // drawn components can be drawn on many layers, so they have to be able to be selected/deselected on any layer

            for (int i = MapBuilder.BASELAYER; i < MapBuilder.WORKLAYER; i++)
            {
                MapLayer layer = MapBuilder.GetMapLayerByIndex(map, i);
                for (int j = 0; j < layer.MapLayerComponents.Count; j++)
                {
                    if (layer.MapLayerComponents[j] is DrawnMapComponent drawnMapComponent)
                    {
                        if (selectedComponent != null && selectedComponent is DrawnMapComponent dmc && dmc == drawnMapComponent)
                        {
                            continue;
                        }
                        else if (drawnMapComponent.IsSelected)
                        {
                            drawnMapComponent.IsSelected = false;
                            MapStateMediator.SelectedDrawnMapComponent = null;
                        }
                    }
                }
            }
        }

        internal static SKRect DrawSelectedRealmAreaOnWorkLayer(RealmStudioMap map, SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            SKRect selectedArea = new(previousPoint.X, previousPoint.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            workLayer.LayerSurface?.Canvas.DrawRect(selectedArea, PaintObjects.LandformAreaSelectPaint);

            return selectedArea;
        }
    }
}
