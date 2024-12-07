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
    internal class MapRenderMethods
    {
        internal static void ClearSelectionLayer(RealmStudioMap map)
        {
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);
            selectionLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        internal static void RenderBackground(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render base layer
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER);

            if (baseLayer.LayerSurface != null)
            {
                baseLayer.LayerSurface.Canvas.Clear(SKColors.White);
                baseLayer.Render(baseLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(baseLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderBoxes(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);

            if (boxLayer.ShowLayer && boxLayer.LayerSurface != null && boxLayer.MapLayerComponents.Count > 0)
            {
                boxLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                boxLayer.Render(boxLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(boxLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderDrawing(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // TODO
        }

        internal static void RenderLowerGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);

            if (belowSymbolGridLayer.ShowLayer && belowSymbolGridLayer.LayerSurface != null && belowSymbolGridLayer.MapLayerComponents.Count > 0)
            {
                belowSymbolGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                belowSymbolGridLayer.Render(belowSymbolGridLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(belowSymbolGridLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderUpperGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);

            if (aboveOceanGridLayer.ShowLayer && aboveOceanGridLayer.LayerSurface != null && aboveOceanGridLayer.MapLayerComponents.Count > 0)
            {
                aboveOceanGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                aboveOceanGridLayer.Render(aboveOceanGridLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(aboveOceanGridLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderDefaultGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);

            if (defaultGridLayer.ShowLayer && defaultGridLayer.LayerSurface != null && defaultGridLayer.MapLayerComponents.Count > 0)
            {
                defaultGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                defaultGridLayer.Render(defaultGridLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(defaultGridLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderLabels(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            if (labelLayer.ShowLayer && labelLayer.LayerSurface != null && labelLayer.MapLayerComponents.Count > 0)
            {
                labelLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                labelLayer.Render(labelLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(labelLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderLandforms(RealmStudioMap map, Landform? currentLandform, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            if (landformLayer.ShowLayer && landformLayer.LayerSurface != null)
            {
                landformLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                currentLandform?.RenderCoastline(landformLayer.LayerSurface.Canvas);
                currentLandform?.RenderLandform(landformLayer.LayerSurface.Canvas);

                for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
                {
                    if (landformLayer.MapLayerComponents[i] is Landform l)
                    {
                        l.RenderCoastline(landformLayer.LayerSurface.Canvas);
                        l.RenderLandform(landformLayer.LayerSurface.Canvas);

                        if (l.IsSelected)
                        {
                            // draw an outline around the landform to show that it is selected
                            l.ContourPath.GetBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            landformLayer.LayerSurface.Canvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                        }
                    }
                    else if (landformLayer.MapLayerComponents[i] is LayerPaintStroke lps)
                    {
                        lps.Render(landformLayer.LayerSurface.Canvas);
                    }
                }

                landformLayer.Render(landformLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(landformLayer.LayerSurface, scrollPoint);
            }


            // landform drawing (color painting over landform)
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);

            if (landDrawingLayer.ShowLayer && landDrawingLayer.LayerSurface != null && landDrawingLayer.MapLayerComponents.Count > 0)
            {
                landDrawingLayer.Render(landDrawingLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(landDrawingLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderLowerMapPaths(RealmStudioMap map, MapPath? currentPath, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);

            if (pathLowerLayer.ShowLayer && pathLowerLayer.LayerSurface != null && pathLowerLayer.MapLayerComponents.Count > 0)
            {

                if (currentPath != null && !currentPath.DrawOverSymbols)
                {
                    currentPath.Render(pathLowerLayer.LayerSurface.Canvas);
                }

                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(pathLowerLayer.LayerSurface.Canvas);

                    if (mp.ShowPathPoints)
                    {
                        List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                        foreach (MapPathPoint p in controlPoints)
                        {
                            if (p.IsSelected)
                            {
                                pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathSelectedControlPointPaint);
                            }
                            else
                            {
                                pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointPaint);
                            }

                            pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointOutlinePaint);
                        }
                    }
                }

                renderCanvas.DrawSurface(pathLowerLayer.LayerSurface, scrollPoint);

                MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

                if (selectionLayer.LayerSurface != null)
                {
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            if (mp.BoundaryPath != null)
                            {
                                // only one path can be selected
                                // draw an outline around the path to show that it is selected
                                mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                                using SKPath boundsPath = new();
                                boundsPath.AddRect(boundRect);

                                selectionLayer.LayerSurface.Canvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                                break;
                            }
                        }
                    }

                    renderCanvas.DrawSurface(selectionLayer.LayerSurface, scrollPoint);
                }
            }
        }

        internal static void RenderUpperMapPaths(RealmStudioMap map, MapPath? currentPath, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // path upper layer
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);

            if (pathUpperLayer.ShowLayer && pathUpperLayer.LayerSurface != null && pathUpperLayer.MapLayerComponents.Count > 0)
            {

                if (currentPath != null && !currentPath.DrawOverSymbols)
                {
                    currentPath.Render(pathUpperLayer.LayerSurface.Canvas);
                }

                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(pathUpperLayer.LayerSurface.Canvas);

                    if (mp.ShowPathPoints)
                    {
                        List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                        foreach (MapPathPoint p in controlPoints)
                        {
                            if (p.IsSelected)
                            {
                                pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathSelectedControlPointPaint);
                            }
                            else
                            {
                                pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointPaint);
                            }

                            pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointOutlinePaint);
                        }
                    }
                }

                renderCanvas.DrawSurface(pathUpperLayer.LayerSurface, scrollPoint);

                MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

                if (selectionLayer.LayerSurface != null)
                {
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            if (mp.BoundaryPath != null)
                            {
                                // only one path can be selected
                                // draw an outline around the path to show that it is selected
                                mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                                using SKPath boundsPath = new();
                                boundsPath.AddRect(boundRect);

                                selectionLayer.LayerSurface.Canvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                                break;
                            }
                        }
                    }

                    renderCanvas.DrawSurface(selectionLayer.LayerSurface, scrollPoint);
                }
            }
        }

        internal static void RenderMeasures(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);

            if (measureLayer.ShowLayer && measureLayer.LayerSurface != null && measureLayer.MapLayerComponents.Count > 0)
            {
                measureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                measureLayer.Render(measureLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(measureLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderOcean(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // render ocean layers
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);

            if (oceanTextureLayer.ShowLayer && oceanTextureLayer.LayerSurface != null && oceanTextureLayer.MapLayerComponents.Count > 0)
            {
                oceanTextureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureLayer.Render(oceanTextureLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(oceanTextureLayer.LayerSurface, scrollPoint);
            }

            if (oceanTextureOverlayLayer.ShowLayer && oceanTextureOverlayLayer.LayerSurface != null
                && oceanTextureOverlayLayer.MapLayerComponents.Count > 0)
            {
                oceanTextureOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureOverlayLayer.Render(oceanTextureOverlayLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(oceanTextureOverlayLayer.LayerSurface, scrollPoint);
            }

            if (oceanDrawingLayer.ShowLayer && oceanDrawingLayer.LayerSurface != null && oceanDrawingLayer.MapLayerComponents.Count > 0)
            {
                oceanDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanDrawingLayer.Render(oceanDrawingLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(oceanDrawingLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderFrame(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);

            if (frameLayer.ShowLayer && frameLayer.LayerSurface != null && frameLayer.MapLayerComponents.Count > 0)
            {
                PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents[0];

                if (placedFrame.FrameEnabled)
                {
                    frameLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    frameLayer.Render(frameLayer.LayerSurface.Canvas);
                    renderCanvas.DrawSurface(frameLayer.LayerSurface, scrollPoint);
                }
            }
        }

        internal static void RenderOverlays(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER);

            if (overlayLayer.ShowLayer && overlayLayer.LayerSurface != null && overlayLayer.MapLayerComponents.Count > 0)
            {
                overlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                overlayLayer.Render(overlayLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(overlayLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderRegions(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

            if (regionLayer.ShowLayer && regionLayer.LayerSurface != null)
            {
                regionLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                regionLayer.Render(regionLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(regionLayer.LayerSurface, scrollPoint);
            }

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);

            if (regionOverlayLayer.ShowLayer && regionOverlayLayer.LayerSurface != null)
            {
                regionOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                regionOverlayLayer.Render(regionOverlayLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(regionOverlayLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderSymbols(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            if (symbolLayer.ShowLayer && symbolLayer.LayerSurface != null && symbolLayer.MapLayerComponents.Count > 0)
            {
                symbolLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                symbolLayer.Render(symbolLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(symbolLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderVignette(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);

            if (vignetteLayer.ShowLayer && vignetteLayer.LayerSurface != null)
            {
                vignetteLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                vignetteLayer.Render(vignetteLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(vignetteLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderWaterFeatures(RealmStudioMap map, WaterFeature? currentWaterFeature, River? currentRiver, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            if (waterLayer.ShowLayer && waterLayer.LayerSurface != null)
            {
                waterLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                // water features
                currentWaterFeature?.Render(waterLayer.LayerSurface.Canvas);
                currentRiver?.Render(waterLayer.LayerSurface.Canvas);

                foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (w is WaterFeature wf)
                    {
                        wf.Render(waterLayer.LayerSurface.Canvas);
                    }
                    else if (w is River r)
                    {
                        r.Render(waterLayer.LayerSurface.Canvas);

                        if (r.ShowRiverPoints)
                        {
                            List<MapRiverPoint> controlPoints = r.GetRiverControlPoints();

                            foreach (MapRiverPoint p in controlPoints)
                            {
                                waterLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, 2.0F, PaintObjects.RiverControlPointPaint);
                                waterLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, 2.0F, PaintObjects.RiverControlPointOutlinePaint);
                            }
                        }
                    }
                }

                renderCanvas.DrawSurface(waterLayer.LayerSurface, scrollPoint);
            }

            // water drawing layer (colors painted on top of water features)
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);

            if (waterDrawingLayer.ShowLayer && waterDrawingLayer.LayerSurface != null && waterDrawingLayer.MapLayerComponents.Count > 0)
            {
                waterDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                waterDrawingLayer.Render(waterDrawingLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(waterDrawingLayer.LayerSurface, scrollPoint);
            }

            // selection layer
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            if (waterLayer.ShowLayer && waterLayer.MapLayerComponents.Count > 0 && selectionLayer.LayerSurface != null)
            {
                foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (w is WaterFeature wf)
                    {
                        if (wf.IsSelected)
                        {
                            // draw an outline around the water feature to show that it is selected
                            wf.WaterFeaturePath.GetBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);
                            selectionLayer.LayerSurface.Canvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                            break;
                        }
                    }
                    else if (w is River r)
                    {
                        if (r.IsSelected)
                        {
                            if (r.RiverBoundaryPath != null)
                            {
                                // draw an outline around the path to show that it is selected
                                r.RiverBoundaryPath.GetTightBounds(out SKRect boundRect);
                                using SKPath boundsPath = new();
                                boundsPath.AddRect(boundRect);
                                selectionLayer.LayerSurface.Canvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                                break;
                            }
                        }
                    }
                }

                renderCanvas.DrawSurface(selectionLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderWindroses(RealmStudioMap map, MapWindrose? currentWindrose, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);

            if (windroseLayer.ShowLayer && windroseLayer.LayerSurface != null && windroseLayer.MapLayerComponents.Count > 0)
            {
                windroseLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                windroseLayer.Render(windroseLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(windroseLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderWorkLayer(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);

            if (workLayer.ShowLayer && workLayer.LayerSurface != null)
            {
                // code that makes use of the work layer is responsible for clearing
                // and drawing to the canvas properly; it is only rendered here
                workLayer.Render(workLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(workLayer.LayerSurface, scrollPoint);
            }
        }
    }
}
