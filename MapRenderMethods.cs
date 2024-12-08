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
            if (baseLayer.LayerSurface == null) return;

            baseLayer.LayerSurface.Canvas.Clear(SKColors.White);

            baseLayer.Render(baseLayer.LayerSurface.Canvas);
            renderCanvas.DrawSurface(baseLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderBackgroundForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render base layer
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);
            canvas.Clear(SKColors.White);

            baseLayer.Render(canvas);
            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderBoxes(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);
            if (boxLayer.LayerSurface == null) return;

            if (boxLayer.ShowLayer && boxLayer.MapLayerComponents.Count > 0)
            {
                boxLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                boxLayer.Render(boxLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(boxLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderBoxesForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);

            if (boxLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                boxLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderDrawing(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // TODO
        }

        internal static void RenderDrawingForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // TODO
        }

        internal static void RenderLowerGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);
            if (belowSymbolGridLayer.LayerSurface == null) return;

            if (belowSymbolGridLayer.ShowLayer && belowSymbolGridLayer.MapLayerComponents.Count > 0)
            {
                belowSymbolGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                belowSymbolGridLayer.Render(belowSymbolGridLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(belowSymbolGridLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderLowerGridForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);

            if (belowSymbolGridLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                belowSymbolGridLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderUpperGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);
            if (aboveOceanGridLayer.LayerSurface == null) return;

            if (aboveOceanGridLayer.ShowLayer && aboveOceanGridLayer.LayerSurface != null && aboveOceanGridLayer.MapLayerComponents.Count > 0)
            {
                aboveOceanGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                aboveOceanGridLayer.Render(aboveOceanGridLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(aboveOceanGridLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderUpperGridForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);

            if (aboveOceanGridLayer.ShowLayer && aboveOceanGridLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                aboveOceanGridLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderDefaultGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);
            if (defaultGridLayer.LayerSurface == null) return;

            if (defaultGridLayer.ShowLayer && defaultGridLayer.MapLayerComponents.Count > 0)
            {
                defaultGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                defaultGridLayer.Render(defaultGridLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(defaultGridLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderDefaultGridForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);

            if (defaultGridLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                defaultGridLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderLabels(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);
            if (labelLayer.LayerSurface == null) return;

            if (labelLayer.ShowLayer && labelLayer.MapLayerComponents.Count > 0)
            {
                labelLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                labelLayer.Render(labelLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(labelLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderLabelsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            if (labelLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                labelLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderLandforms(RealmStudioMap map, Landform? currentLandform, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            if (landformLayer.LayerSurface == null) return;

            if (landformLayer.ShowLayer && landformLayer.MapLayerComponents.Count > 0)
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

        internal static void RenderLandformsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            if (landformLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);

                for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
                {
                    if (landformLayer.MapLayerComponents[i] is Landform l)
                    {
                        l.RenderCoastline(canvas);
                        l.RenderLandform(canvas);
                    }
                    else if (landformLayer.MapLayerComponents[i] is LayerPaintStroke lps)
                    {
                        // eraser strokes
                        lps.Render(canvas);
                    }
                }

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }


            // landform drawing (color painting over landform)
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);

            if (landDrawingLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                landDrawingLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderLowerMapPaths(RealmStudioMap map, MapPath? currentPath, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);
            if (pathLowerLayer.LayerSurface == null) return;

            if (pathLowerLayer.ShowLayer && pathLowerLayer.MapLayerComponents.Count > 0)
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
                    selectionLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

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

        internal static void RenderLowerMapPathsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);

            if (pathLowerLayer.ShowLayer && pathLowerLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));

                using SKCanvas canvas = new(b);

                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(canvas);
                }

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderUpperMapPaths(RealmStudioMap map, MapPath? currentPath, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // path upper layer
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);

            if (pathUpperLayer.ShowLayer && pathUpperLayer.LayerSurface != null && pathUpperLayer.MapLayerComponents.Count > 0)
            {
                if (currentPath != null && currentPath.DrawOverSymbols)
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
                    selectionLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

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

        internal static void RenderUpperMapPathsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // path upper layer
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);

            if (pathUpperLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(canvas);
                }

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderMeasures(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);
            if (measureLayer.LayerSurface == null) return;

            if (measureLayer.ShowLayer && measureLayer.MapLayerComponents.Count > 0)
            {
                measureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                measureLayer.Render(measureLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(measureLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderMeasuresForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);

            if (measureLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                measureLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderOcean(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render ocean layers
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);

            if (oceanTextureLayer.LayerSurface == null || oceanTextureOverlayLayer.LayerSurface == null || oceanDrawingLayer.LayerSurface == null) return;

            if (oceanTextureLayer.ShowLayer && oceanTextureLayer.MapLayerComponents.Count > 0)
            {
                oceanTextureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureLayer.Render(oceanTextureLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(oceanTextureLayer.LayerSurface, scrollPoint);
            }

            if (oceanTextureOverlayLayer.ShowLayer && oceanTextureOverlayLayer.MapLayerComponents.Count > 0)
            {
                oceanTextureOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureOverlayLayer.Render(oceanTextureOverlayLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(oceanTextureOverlayLayer.LayerSurface, scrollPoint);
            }

            if (oceanDrawingLayer.ShowLayer && oceanDrawingLayer.MapLayerComponents.Count > 0)
            {
                oceanDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanDrawingLayer.Render(oceanDrawingLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(oceanDrawingLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderOceanForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render ocean layers
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);

            if (oceanTextureLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                oceanTextureLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }

            if (oceanTextureOverlayLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                oceanTextureOverlayLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }

            if (oceanDrawingLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                oceanDrawingLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderFrame(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);
            if (frameLayer.LayerSurface == null) return;

            if (frameLayer.ShowLayer && frameLayer.MapLayerComponents.Count > 0)
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

        internal static void RenderFrameForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);

            if (frameLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents[0];

                if (placedFrame.FrameEnabled)
                {
                    canvas.Clear(SKColors.Transparent);
                    frameLayer.Render(canvas);

                    renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
                }
            }
        }

        internal static void RenderOverlays(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER);
            if (overlayLayer.LayerSurface == null) return;

            if (overlayLayer.ShowLayer && overlayLayer.MapLayerComponents.Count > 0)
            {
                overlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                overlayLayer.Render(overlayLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(overlayLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderOverlaysForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER);

            if (overlayLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                overlayLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderRegions(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);
            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);

            if (regionLayer.LayerSurface == null || regionOverlayLayer.LayerSurface == null) return;

            if (regionLayer.ShowLayer && regionLayer.MapLayerComponents.Count > 0)
            {
                regionLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                regionLayer.Render(regionLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(regionLayer.LayerSurface, scrollPoint);
            }

            if (regionOverlayLayer.ShowLayer && regionOverlayLayer.MapLayerComponents.Count > 0)
            {
                regionOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                regionOverlayLayer.Render(regionOverlayLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(regionOverlayLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderRegionsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

            if (regionLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                regionLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);

            if (regionOverlayLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                regionOverlayLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }


        internal static void RenderSymbols(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);
            if (symbolLayer.LayerSurface == null) return;

            if (symbolLayer.ShowLayer && symbolLayer.MapLayerComponents.Count > 0)
            {
                symbolLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                symbolLayer.Render(symbolLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(symbolLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderSymbolsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            if (symbolLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                symbolLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderVignette(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);
            if (vignetteLayer.LayerSurface == null) return;

            if (vignetteLayer.ShowLayer && vignetteLayer.MapLayerComponents.Count > 0)
            {
                vignetteLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                vignetteLayer.Render(vignetteLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(vignetteLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderVignetteForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);

            if (vignetteLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                vignetteLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderWaterFeatures(RealmStudioMap map, WaterFeature? currentWaterFeature, River? currentRiver, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);

            if (waterLayer.LayerSurface == null || waterDrawingLayer.LayerSurface == null) return;

            if (waterLayer.ShowLayer && waterLayer.MapLayerComponents.Count > 0)
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
            if (waterDrawingLayer.ShowLayer && waterDrawingLayer.MapLayerComponents.Count > 0)
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

        internal static void RenderWaterFeaturesForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            if (waterLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);

                // water features
                foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (w is WaterFeature wf)
                    {
                        wf.Render(canvas);
                    }
                    else if (w is River r)
                    {
                        r.Render(canvas);
                    }
                }

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }

            // water drawing layer (colors painted on top of water features)
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);

            if (waterDrawingLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                waterDrawingLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderWindroses(RealmStudioMap map, MapWindrose? currentWindrose, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);
            if (windroseLayer.LayerSurface == null) return;

            currentWindrose?.Render(windroseLayer.LayerSurface.Canvas);

            if (windroseLayer.ShowLayer && windroseLayer.MapLayerComponents.Count > 0)
            {
                windroseLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                windroseLayer.Render(windroseLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(windroseLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderWindrosesForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);

            if (windroseLayer.MapLayerComponents.Count > 0)
            {
                using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
                using SKCanvas canvas = new(b);

                canvas.Clear(SKColors.Transparent);
                windroseLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
            }
        }

        internal static void RenderWorkLayer(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint, bool toSnapShot = false)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);

            if (workLayer.ShowLayer && workLayer.LayerSurface != null)
            {
                if (!toSnapShot)
                {
                    // code that makes use of the work layer is responsible for clearing
                    // and drawing to the canvas properly; it is only rendered here
                    workLayer.Render(workLayer.LayerSurface.Canvas);
                    renderCanvas.DrawSurface(workLayer.LayerSurface, scrollPoint);
                }
            }
        }
    }
}
