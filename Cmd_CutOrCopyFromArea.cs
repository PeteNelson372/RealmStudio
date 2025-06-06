﻿/**************************************************************************************************************************
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
    internal sealed class Cmd_CutOrCopyFromArea(RealmStudioMap map, List<MapComponent> selectedMapComponents, SKRect selectedMapArea, bool cutFromArea = true) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly List<MapComponent> StoredComponents = [];
        private readonly SKRect SelectedMapArea = selectedMapArea;
        private readonly bool CutFromArea = cutFromArea;

        public void DoOperation()
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER);
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WATERLAYER);

            for (int i = selectedMapComponents.Count - 1; i >= 0; i--)
            {
                if (selectedMapComponents[i] is MapSymbol ms)
                {
                    for (int j = symbolLayer.MapLayerComponents.Count - 1; j >= 0; j--)
                    {
                        if (symbolLayer.MapLayerComponents[j] is MapSymbol ms2
                            && ms.SymbolGuid.ToString() == ms2.SymbolGuid.ToString())
                        {
                            // store the X,Y position of the symbol as an offset from the top,left corner of the selected area
                            // this is done so if/when the selected component (symbol) is pasted, it can be pasted
                            // relative to the cursor position

                            MapSymbol newSymbol = new(ms)
                            {
                                SymbolGuid = ms.SymbolGuid,
                                X = (int)(ms.X - SelectedMapArea.Left),
                                Y = (int)(ms.Y - SelectedMapArea.Top),
                            };

                            Bitmap newPlacedBitmap = new(ms.PlacedBitmap.ToBitmap(), newSymbol.SymbolWidth, newSymbol.SymbolHeight);
                            newSymbol.PlacedBitmap = newPlacedBitmap.ToSKBitmap();

                            StoredComponents.Add(newSymbol);
                            selectedMapComponents[i] = newSymbol;

                            if (CutFromArea)
                            {
                                symbolLayer.MapLayerComponents.RemoveAt(j);
                            }

                            break;
                        }
                    }
                }
                else if (selectedMapComponents[i] is MapPath mp)
                {
                    for (int j = pathLowerLayer.MapLayerComponents.Count - 1; j >= 0; j--)
                    {
                        if (pathLowerLayer.MapLayerComponents[j] is MapPath mp2
                            && mp.MapPathGuid.ToString() == mp2.MapPathGuid.ToString())
                        {
                            // store the X,Y position of the symbol as an offset from the top,left corner of the selected area
                            // this is done so if/when the selected component (symbol) is pasted, it can be pasted
                            // relative to the cursor position

                            MapPath newMapPath = new(mp)
                            {
                                MapPathGuid = mp.MapPathGuid,
                                X = (int)(mp.X - SelectedMapArea.Left),
                                Y = (int)(mp.Y - SelectedMapArea.Top),
                            };

                            newMapPath.PathPoints.Clear();

                            foreach (MapPathPoint point in mp.PathPoints)
                            {
                                SKPoint p = new(point.MapPoint.X - SelectedMapArea.Left, point.MapPoint.Y - SelectedMapArea.Top);
                                MapPathPoint mpp = new()
                                {
                                    MapPoint = p
                                };
                                newMapPath.PathPoints.Add(mpp);
                            }

                            StoredComponents.Add(newMapPath);
                            selectedMapComponents[i] = newMapPath;

                            if (CutFromArea)
                            {
                                pathLowerLayer.MapLayerComponents.RemoveAt(j);
                            }

                            break;
                        }
                    }

                    for (int j = pathUpperLayer.MapLayerComponents.Count - 1; j >= 0; j--)
                    {
                        if (pathUpperLayer.MapLayerComponents[j] is MapPath mp2
                            && mp.MapPathGuid.ToString() == mp2.MapPathGuid.ToString())
                        {
                            // store the X,Y position of the symbol as an offset from the top,left corner of the selected area
                            // this is done so if/when the selected component (symbol) is pasted, it can be pasted
                            // relative to the cursor position

                            MapPath newMapPath = new(mp)
                            {
                                MapPathGuid = mp.MapPathGuid,
                                X = (int)(mp.X - SelectedMapArea.Left),
                                Y = (int)(mp.Y - SelectedMapArea.Top),
                            };

                            newMapPath.PathPoints.Clear();

                            foreach (MapPathPoint point in mp.PathPoints)
                            {
                                SKPoint p = new(point.MapPoint.X - SelectedMapArea.Left, point.MapPoint.Y - SelectedMapArea.Top);
                                MapPathPoint mpp = new()
                                {
                                    MapPoint = p
                                };
                                newMapPath.PathPoints.Add(mpp);
                            }

                            PathManager.ConstructPathPaint(newMapPath);

                            StoredComponents.Add(newMapPath);
                            selectedMapComponents[i] = newMapPath;

                            if (CutFromArea)
                            {
                                pathUpperLayer.MapLayerComponents.RemoveAt(j);
                            }

                            break;
                        }
                    }
                }
                else if (selectedMapComponents[i] is IWaterFeature iwf)
                {
                    if (iwf is WaterFeature wf)
                    {
                        for (int j = waterLayer.MapLayerComponents.Count - 1; j >= 0; j--)
                        {
                            if (waterLayer.MapLayerComponents[j] is WaterFeature wf2
                                && wf.WaterFeatureGuid.ToString() == wf2.WaterFeatureGuid.ToString())
                            {
                                WaterFeature newWf = new(wf)
                                {
                                    X = (int)(wf.X - SelectedMapArea.Left),
                                    Y = (int)(wf.Y - SelectedMapArea.Top),
                                    WaterFeatureGuid = wf.WaterFeatureGuid,
                                };

                                SKPath wfPath = new(wf.WaterFeaturePath);
                                wfPath.Transform(SKMatrix.CreateTranslation(-SelectedMapArea.Left, -SelectedMapArea.Top));

                                newWf.WaterFeaturePath = wfPath;

                                WaterFeatureManager.CreateInnerAndOuterPaths(Map, newWf);

                                StoredComponents.Add(newWf);
                                selectedMapComponents[i] = newWf;

                                if (CutFromArea)
                                {
                                    waterLayer.MapLayerComponents.RemoveAt(j);
                                }

                                break;
                            }
                        }
                    }
                    else if (iwf is River mr)
                    {
                        for (int j = waterLayer.MapLayerComponents.Count - 1; j >= 0; j--)
                        {
                            River newRiver = new()
                            {
                                X = (int)(mr.X - SelectedMapArea.Left),
                                Y = (int)(mr.Y - SelectedMapArea.Top),
                                MapRiverGuid = mr.MapRiverGuid,
                            };

                            newRiver.RiverPoints.Clear();

                            foreach (MapRiverPoint point in mr.RiverPoints)
                            {
                                SKPoint p = new(point.RiverPoint.X - SelectedMapArea.Left, point.RiverPoint.Y - SelectedMapArea.Top);
                                MapRiverPoint mrp = new()
                                {
                                    RiverPoint = p
                                };
                                newRiver.RiverPoints.Add(mrp);
                            }

                            WaterFeatureManager.ConstructRiverPaths(newRiver);
                            WaterFeatureManager.ConstructRiverPaintObjects(newRiver);

                            StoredComponents.Add(newRiver);
                            selectedMapComponents[i] = newRiver;

                            if (CutFromArea)
                            {
                                waterLayer.MapLayerComponents.RemoveAt(j);
                            }
                        }
                    }
                }
            }
        }

        public void UndoOperation()
        {
            if (!CutFromArea)
            {
                return;
            }

            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER);
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WATERLAYER);

            foreach (MapComponent component in StoredComponents)
            {
                if (component is MapSymbol ms)
                {
                    ms.X = (int)(ms.X + SelectedMapArea.Left);
                    ms.Y = (int)(ms.Y + SelectedMapArea.Top);
                    symbolLayer.MapLayerComponents.Add(ms);
                }
                else if (component is MapPath mp)
                {
                    List<MapPathPoint> points = [.. mp.PathPoints];
                    mp.PathPoints.Clear();

                    foreach (MapPathPoint point in points)
                    {
                        SKPoint p = new(point.MapPoint.X + SelectedMapArea.Left, point.MapPoint.Y + SelectedMapArea.Top);
                        MapPathPoint mpp = new()
                        {
                            MapPoint = p
                        };
                        mp.PathPoints.Add(mpp);
                    }

                    SKPath boundaryPath = PathManager.GenerateMapPathBoundaryPath(mp.PathPoints);
                    mp.BoundaryPath = boundaryPath;

                    mp.PathPaint = null;
                    PathManager.ConstructPathPaint(mp);

                    if (mp.DrawOverSymbols)
                    {
                        pathUpperLayer.MapLayerComponents.Add(mp);
                    }
                    else
                    {
                        pathLowerLayer.MapLayerComponents.Add(mp);
                    }
                }
                else if (component is IWaterFeature iwf)
                {
                    if (iwf is WaterFeature wf)
                    {
                        SKPath wfPath = new(wf.WaterFeaturePath);
                        wfPath.Transform(SKMatrix.CreateTranslation(SelectedMapArea.Left, SelectedMapArea.Top));

                        wf.WaterFeaturePath.Dispose();
                        wf.WaterFeaturePath = wfPath;

                        WaterFeatureManager.CreateInnerAndOuterPaths(Map, wf);

                        waterLayer.MapLayerComponents.Add(wf);
                    }
                    else if (iwf is River mr)
                    {
                        List<MapRiverPoint> points = [.. mr.RiverPoints];
                        mr.RiverPoints.Clear();

                        foreach (MapRiverPoint point in points)
                        {
                            SKPoint p = new(point.RiverPoint.X + SelectedMapArea.Left, point.RiverPoint.Y + SelectedMapArea.Top);
                            MapRiverPoint mrp = new()
                            {
                                RiverPoint = p
                            };

                            mr.RiverPoints.Add(mrp);
                        }

                        WaterFeatureManager.ConstructRiverPaths(mr);
                        WaterFeatureManager.ConstructRiverPaintObjects(mr);

                        waterLayer.MapLayerComponents.Add(mr);
                    }
                }
            }
        }
    }
}
