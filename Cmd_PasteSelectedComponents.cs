/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
    internal sealed class Cmd_PasteSelectedComponents(RealmStudioMap map, List<MapComponent> selectedMapComponents, SKRect selectedArea, SKPoint pastePoint) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly List<MapComponent> StoredComponents = [];
        private readonly SKPoint PastePoint = pastePoint;
        private readonly SKRect SelectedArea = selectedArea;

        public void DoOperation()
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER);
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WATERLAYER);

            foreach (MapComponent mc in selectedMapComponents)
            {
                if (mc is MapSymbol ms)
                {
                    StoredComponents.Add(ms);

                    MapSymbol newSymbol = new(ms)
                    {
                        X = (int)(ms.X + PastePoint.X - (SelectedArea.Width / 2)),
                        Y = (int)(ms.Y + PastePoint.Y - (SelectedArea.Height / 2)),
                        SymbolWidth = ms.SymbolWidth,
                        SymbolHeight = ms.SymbolHeight,
                        SymbolPaint = ms.SymbolPaint,
                    };

                    Bitmap newPlacedBitmap = new(ms.PlacedBitmap.ToBitmap(), newSymbol.SymbolWidth, newSymbol.SymbolHeight);
                    newSymbol.PlacedBitmap = newPlacedBitmap.ToSKBitmap();

                    symbolLayer.MapLayerComponents.Add(newSymbol);
                }
                else if (mc is MapPath mp)
                {
                    List<MapPathPoint> points = [.. mp.PathPoints];

                    // have to create a new mappath instance
                    MapPath newMapPath = new()
                    {
                        X = (int)(mp.X + PastePoint.X - (SelectedArea.Width / 2)),
                        Y = (int)(mp.Y + PastePoint.Y - (SelectedArea.Height / 2)),
                        DrawOverSymbols = mp.DrawOverSymbols,
                        Height = mp.Height,
                        MapPathName = mp.MapPathName,
                        ParentMap = mp.ParentMap,
                        PathColor = mp.PathColor,
                        PathTexture = mp.PathTexture,
                        PathType = mp.PathType,
                        PathWidth = mp.PathWidth,
                        ShowPathPoints = false,
                    };

                    newMapPath.PathPoints.Clear();

                    foreach (MapPathPoint point in points)
                    {
                        SKPoint p = new(point.MapPoint.X + PastePoint.X - (SelectedArea.Width / 2), point.MapPoint.Y + PastePoint.Y - (SelectedArea.Height / 2));
                        MapPathPoint mpp = new()
                        {
                            MapPoint = p
                        };
                        newMapPath.PathPoints.Add(mpp);
                    }

                    SKPath boundaryPath = PathManager.GenerateMapPathBoundaryPath(newMapPath.PathPoints);
                    newMapPath.BoundaryPath = boundaryPath;

                    newMapPath.PathPaint = null;
                    PathManager.ConstructPathPaint(newMapPath);

                    if (mp.DrawOverSymbols)
                    {
                        pathUpperLayer.MapLayerComponents.Add(newMapPath);
                    }
                    else
                    {
                        pathLowerLayer.MapLayerComponents.Add(newMapPath);
                    }
                }
                else if (mc is IWaterFeature iwf)
                {
                    if (iwf is WaterFeature wf)
                    {
                        WaterFeature newWf = new()
                        {
                            X = (int)(wf.X + PastePoint.X - (SelectedArea.Width / 2)),
                            Y = (int)(wf.Y + PastePoint.Y - (SelectedArea.Height / 2)),
                            Width = wf.Width,
                            Height = wf.Height,
                            WaterFeatureGuid = wf.WaterFeatureGuid,
                            ShallowWaterPaint = wf.ShallowWaterPaint,
                            WaterFeatureBackgroundPaint = wf.WaterFeatureBackgroundPaint,
                            ShorelineEffectDistance = wf.ShorelineEffectDistance,
                            WaterFeatureColor = wf.WaterFeatureColor,
                            WaterFeatureName = wf.WaterFeatureName,
                            WaterFeatureType = wf.WaterFeatureType,
                            WaterFeatureShorelineColor = wf.WaterFeatureShorelineColor,
                            WaterFeatureShorelinePaint = wf.WaterFeatureShorelinePaint,
                            ParentMap = wf.ParentMap,
                        };

                        SKPath wfPath = new(wf.WaterFeaturePath);
                        wfPath.Transform(SKMatrix.CreateTranslation(PastePoint.X - (SelectedArea.Width / 2), PastePoint.Y - SelectedArea.Height / 2));

                        newWf.WaterFeaturePath.Dispose();
                        newWf.WaterFeaturePath = wfPath;

                        WaterFeatureManager.CreateInnerAndOuterPaths(Map, newWf);

                        waterLayer.MapLayerComponents.Add(newWf);
                    }
                    else if (iwf is River mr)
                    {
                        List<MapRiverPoint> points = [.. mr.RiverPoints];

                        River newRiver = new()
                        {
                            X = (int)(mr.X + PastePoint.X - (SelectedArea.Width / 2)),
                            Y = (int)(mr.Y + PastePoint.Y - (SelectedArea.Height / 2)),
                            Width = mr.Width,
                            Height = mr.Height,
                            MapRiverGuid = mr.MapRiverGuid,
                            MapRiverName = mr.MapRiverName,
                            ParentMap = mr.ParentMap,
                            RenderRiverTexture = mr.RenderRiverTexture,
                            RiverColor = mr.RiverColor,
                            RiverFillPaint = mr.RiverFillPaint,
                            RiverShallowWaterPaint = mr.RiverShallowWaterPaint,
                            RiverShorelineColor = mr.RiverShorelineColor,
                            RiverShorelinePaint = mr.RiverShorelinePaint,
                            RiverSourceFadeIn = mr.RiverSourceFadeIn,
                            RiverWidth = mr.RiverWidth,
                            ShorelineEffectDistance = mr.ShorelineEffectDistance,
                        };

                        newRiver.RiverPoints.Clear();

                        foreach (MapRiverPoint point in points)
                        {
                            SKPoint p = new(point.RiverPoint.X + PastePoint.X - (SelectedArea.Width / 2), point.RiverPoint.Y + PastePoint.Y - (SelectedArea.Height / 2));
                            MapRiverPoint mrp = new()
                            {
                                RiverPoint = p
                            };

                            newRiver.RiverPoints.Add(mrp);
                        }

                        WaterFeatureManager.ConstructRiverPaths(newRiver);
                        WaterFeatureManager.ConstructRiverPaintObjects(newRiver);

                        waterLayer.MapLayerComponents.Add(mr);
                    }
                }
            }

            //selectedMapComponents.Clear();
        }

        public void UndoOperation()
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER);
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WATERLAYER);

            foreach (MapComponent mc in StoredComponents)
            {
                selectedMapComponents.Add(mc);

                if (mc is MapSymbol ms)
                {
                    for (int i = symbolLayer.MapLayerComponents.Count - 1; i >= 0; i--)
                    {
                        if (symbolLayer.MapLayerComponents[i] is MapSymbol ms2 && ms2.SymbolGuid.ToString() == ms.SymbolGuid.ToString())
                        {
                            symbolLayer.MapLayerComponents.RemoveAt(i);
                            break;
                        }
                    }
                }
                else if (mc is MapPath mp)
                {
                    if (mp.DrawOverSymbols)
                    {
                        for (int i = pathUpperLayer.MapLayerComponents.Count - 1; i >= 0; i--)
                        {
                            if (pathUpperLayer.MapLayerComponents[i] is MapPath mp2 && mp2.MapPathGuid.ToString() == mp.MapPathGuid.ToString())
                            {
                                pathUpperLayer.MapLayerComponents.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = pathLowerLayer.MapLayerComponents.Count - 1; i >= 0; i--)
                        {
                            if (pathLowerLayer.MapLayerComponents[i] is MapPath mp2 && mp2.MapPathGuid.ToString() == mp.MapPathGuid.ToString())
                            {
                                pathLowerLayer.MapLayerComponents.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
}