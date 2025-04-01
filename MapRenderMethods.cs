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

namespace RealmStudio
{
    internal sealed class MapRenderMethods
    {
        public static void RenderMapToCanvas(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // background
            RenderBackground(map, renderCanvas, scrollPoint);

            // ocean layers
            RenderOcean(map, renderCanvas, scrollPoint);

            // wind roses
            RenderWindroses(map, renderCanvas, scrollPoint);

            // lower grid layer (above ocean)
            RenderLowerGrid(map, renderCanvas, scrollPoint);

            // landforms
            RenderLandforms(map, renderCanvas, scrollPoint);

            // water features
            RenderWaterFeatures(map, renderCanvas, scrollPoint);

            // upper grid layer (above water features)
            RenderUpperGrid(map, renderCanvas, scrollPoint);

            // lower path layer
            RenderLowerMapPaths(map, renderCanvas, scrollPoint);

            // symbol layer
            RenderSymbols(map, renderCanvas, scrollPoint);

            // upper path layer
            RenderUpperMapPaths(map, renderCanvas, scrollPoint);

            // region and region overlay layers
            RenderRegions(map, renderCanvas, scrollPoint);

            // default grid layer
            RenderDefaultGrid(map, renderCanvas, scrollPoint);

            // box layer
            RenderBoxes(map, renderCanvas, scrollPoint);

            // label layer
            RenderLabels(map, renderCanvas, scrollPoint);

            // overlay layer (map scale)
            RenderOverlays(map, renderCanvas, scrollPoint);

            // render frame
            RenderFrame(map, renderCanvas, scrollPoint);

            // measure layer
            RenderMeasures(map, renderCanvas, scrollPoint);

            // TODO: drawing layer
            //RenderDrawing(map, renderCanvas, scrollPoint);

            // vignette layer
            RenderVignette(map, renderCanvas, scrollPoint);
        }

        public static void RenderMapForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // background
            RenderBackgroundForExport(map, renderCanvas);

            // ocean layers
            RenderOceanForExport(map, renderCanvas);

            // wind roses
            RenderWindrosesForExport(map, renderCanvas);

            // lower grid layer (above ocean)
            RenderLowerGridForExport(map, renderCanvas);

            // landforms
            RenderLandformsForExport(map, renderCanvas);

            // water features
            RenderWaterFeaturesForExport(map, renderCanvas);

            // upper grid layer (above water features)
            RenderUpperGridForExport(map, renderCanvas);

            // lower path layer
            RenderLowerMapPathsForExport(map, renderCanvas);

            // symbol layer
            RenderSymbolsForExport(map, renderCanvas);

            // upper path layer
            RenderUpperMapPathsForExport(map, renderCanvas);

            // region and region overlay layers
            RenderRegionsForExport(map, renderCanvas);

            // default grid layer
            RenderDefaultGridForExport(map, renderCanvas);

            // box layer
            RenderBoxesForExport(map, renderCanvas);

            // label layer
            RenderLabelsForExport(map, renderCanvas);

            // overlay layer (map scale)
            RenderOverlaysForExport(map, renderCanvas);

            // render frame
            RenderFrameForExport(map, renderCanvas);

            // measure layer
            RenderMeasuresForExport(map, renderCanvas);

            // TODO: drawing layer
            //MapRenderMethods.RenderDrawingForExport(map, renderCanvas);

            // vignette layer
            RenderVignetteForExport(map, renderCanvas);
        }

        internal static void DrawCursor(SKCanvas canvas, MapDrawingMode drawingMode, SKPoint point, int brushSize, float symbolScale, float symbolRotation,
            bool mirrorSymbol, bool useAreaBrush)
        {
            switch (drawingMode)
            {
                case MapDrawingMode.SymbolPlace:
                    {
                        if (SymbolManager.SelectedSymbolTableMapSymbol != null && !useAreaBrush)
                        {
                            if (SymbolManager.SelectedSymbolTableMapSymbol.SymbolFormat != SymbolFileFormat.Vector)
                            {
                                SKBitmap? symbolBitmap = SymbolManager.SelectedSymbolTableMapSymbol.ColorMappedBitmap;
                                if (symbolBitmap != null)
                                {
                                    // TODO: use matrix for scaling and rotation
                                    SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleSKBitmap(symbolBitmap, symbolScale);
                                    SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateSKBitmap(scaledSymbolBitmap, symbolRotation, mirrorSymbol);

                                    canvas.DrawBitmap(rotatedAndScaledBitmap,
                                        new SKPoint(point.X - (rotatedAndScaledBitmap.Width / 2), point.Y - (rotatedAndScaledBitmap.Height / 2)), null);

                                    scaledSymbolBitmap.Dispose();
                                    rotatedAndScaledBitmap.Dispose();
                                }
                            }
                            else
                            {
                                // display the SVG as the cursor
                                SKBitmap? symbolBitmap = SymbolManager.GetBitmapForVectorSymbol(SymbolManager.SelectedSymbolTableMapSymbol,
                                    0, 0, symbolRotation, symbolScale);

                                if (symbolBitmap != null)
                                {
                                    SKPoint pt = new(point.X - (symbolBitmap.Width / 2), point.Y - (symbolBitmap.Height / 2));
                                    canvas.DrawBitmap(symbolBitmap, pt);
                                }
                            }
                        }
                        else if (useAreaBrush)
                        {
                            canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                        else
                        {
                            canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                    }
                    break;
                default:
                    {
                        if (brushSize > 0)
                        {
                            if (drawingMode == MapDrawingMode.MapHeightIncrease || drawingMode == MapDrawingMode.MapHeightDecrease)
                            {
                                canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCircleGreenPaint);
                            }
                            else
                            {
                                canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                            }
                        }
                    }
                    break;
            }
        }

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

            boxLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            boxLayer.Render(boxLayer.LayerSurface.Canvas);
            renderCanvas.DrawSurface(boxLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderBoxesForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            boxLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        /*
        internal static void RenderDrawing(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // TODO
        }

        internal static void RenderDrawingForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // TODO
        }
        */

        internal static void RenderLowerGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);
            if (belowSymbolGridLayer.LayerSurface == null) return;

            belowSymbolGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            belowSymbolGridLayer.Render(belowSymbolGridLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(belowSymbolGridLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderLowerGridForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            belowSymbolGridLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderUpperGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);
            if (aboveOceanGridLayer.LayerSurface == null) return;

            aboveOceanGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            aboveOceanGridLayer.Render(aboveOceanGridLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(aboveOceanGridLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderUpperGridForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            aboveOceanGridLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderDefaultGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);
            if (defaultGridLayer.LayerSurface == null) return;

            defaultGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            defaultGridLayer.Render(defaultGridLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(defaultGridLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderDefaultGridForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            defaultGridLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderLabels(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);
            if (labelLayer.LayerSurface == null) return;

            labelLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            labelLayer.Render(labelLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(labelLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderLabelsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            labelLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderLandforms(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDCOASTLINELAYER);

            if (landformLayer.LayerSurface == null || landCoastlineLayer.LayerSurface == null) return;

            landCoastlineLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            landformLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

            MapStateMediator.CurrentLandform?.RenderCoastline(landCoastlineLayer.LayerSurface.Canvas);
            MapStateMediator.CurrentLandform?.RenderLandform(landformLayer.LayerSurface.Canvas);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform l)
                {
                    if (!l.IsDeleted)
                    {
                        l.RenderCoastline(landCoastlineLayer.LayerSurface.Canvas);
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
                }
            }

            renderCanvas.DrawSurface(landCoastlineLayer.LayerSurface, scrollPoint);
            renderCanvas.DrawSurface(landformLayer.LayerSurface, scrollPoint);

            // landform drawing (color painting over landform)
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);

            if (landDrawingLayer.LayerSurface != null)
            {
                landDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                landDrawingLayer.Render(landDrawingLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(landDrawingLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderLandformsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

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

            // landform drawing (color painting over landform)
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);

            using SKBitmap b2 = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas2 = new(b2);

            landDrawingLayer.Render(canvas2);

            renderCanvas.DrawBitmap(b2, new SKPoint(0, 0));
        }

        internal static void RenderHeightMap(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint, SKRect? selectedArea)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.HEIGHTMAPLAYER);

            if (landformLayer.LayerSurface == null || heightMapLayer.LayerSurface == null) return;

            SKPath clipPath = new();
            landformLayer.LayerSurface.Canvas.Clear(SKColors.Black);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform l)
                {
                    l.RenderLandformForHeightMap(landformLayer.LayerSurface.Canvas);

                    SKPath landformOutlinePath = l.ContourPath;
                    clipPath.AddPath(landformOutlinePath);
                }
            }

            renderCanvas.DrawSurface(landformLayer.LayerSurface, scrollPoint);

            // render height map layer
            heightMapLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

            if (clipPath.PointCount > 2)
            {
                heightMapLayer.LayerSurface.Canvas.ClipPath(clipPath);
            }

            heightMapLayer.Render(heightMapLayer.LayerSurface.Canvas);
            renderCanvas.DrawSurface(heightMapLayer.LayerSurface, scrollPoint);

            if (selectedArea != null)
            {
                MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
                workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
                workLayer.LayerSurface?.Canvas.DrawRect((SKRect)selectedArea, PaintObjects.LandformAreaSelectPaint);
            }
        }

        internal static void RenderLowerMapPaths(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);
            if (pathLowerLayer.LayerSurface == null) return;

            pathLowerLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

            if (pathLowerLayer.LayerSurface != null && pathLowerLayer.ShowLayer)
            {
                if (MapStateMediator.CurrentMapPath != null && !MapStateMediator.CurrentMapPath.DrawOverSymbols)
                {
                    MapStateMediator.CurrentMapPath.Render(pathLowerLayer.LayerSurface.Canvas);
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
                                mp.BoundaryPath.GetBounds(out SKRect boundRect);
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

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));

            using SKCanvas canvas = new(b);

            foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.Render(canvas);
            }

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderUpperMapPaths(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // path upper layer
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);
            if (pathUpperLayer.LayerSurface == null) return;

            pathUpperLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

            if (pathUpperLayer.LayerSurface != null && pathUpperLayer.ShowLayer)
            {
                if (MapStateMediator.CurrentMapPath != null && MapStateMediator.CurrentMapPath.DrawOverSymbols)
                {
                    MapStateMediator.CurrentMapPath.Render(pathUpperLayer.LayerSurface.Canvas);
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
                                mp.BoundaryPath.GetBounds(out SKRect boundRect);
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

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.Render(canvas);
            }

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderMeasures(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);
            if (measureLayer.LayerSurface == null) return;

            measureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            measureLayer.Render(measureLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(measureLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderMeasuresForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            measureLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderOcean(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render ocean layers
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);

            if (oceanTextureLayer.LayerSurface == null || oceanTextureOverlayLayer.LayerSurface == null || oceanDrawingLayer.LayerSurface == null) return;

            oceanTextureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            oceanTextureLayer.Render(oceanTextureLayer.LayerSurface.Canvas);
            renderCanvas.DrawSurface(oceanTextureLayer.LayerSurface, scrollPoint);

            oceanTextureOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            oceanTextureOverlayLayer.Render(oceanTextureOverlayLayer.LayerSurface.Canvas);
            renderCanvas.DrawSurface(oceanTextureOverlayLayer.LayerSurface, scrollPoint);

            oceanDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            oceanDrawingLayer.Render(oceanDrawingLayer.LayerSurface.Canvas);
            renderCanvas.DrawSurface(oceanDrawingLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderOceanForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render ocean layers
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);
            canvas.Clear(SKColors.Transparent);
            oceanTextureLayer.Render(canvas);
            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));

            using SKBitmap b2 = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas2 = new(b2);
            canvas2.Clear(SKColors.Transparent);
            oceanTextureOverlayLayer.Render(canvas2);
            renderCanvas.DrawBitmap(b2, new SKPoint(0, 0));

            using SKBitmap b3 = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas3 = new(b3);
            canvas3.Clear(SKColors.Transparent);
            oceanDrawingLayer.Render(canvas3);
            renderCanvas.DrawBitmap(b3, new SKPoint(0, 0));
        }

        internal static void RenderFrame(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);
            if (frameLayer.LayerSurface == null || frameLayer.MapLayerComponents.Count == 0) return;

            PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents[0];

            if (placedFrame.FrameEnabled)
            {
                frameLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                frameLayer.Render(frameLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(frameLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderFrameForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);
            if (frameLayer.MapLayerComponents.Count == 0) return;

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

        internal static void RenderOverlays(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER);
            if (overlayLayer.LayerSurface == null) return;

            overlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            overlayLayer.Render(overlayLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(overlayLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderOverlaysForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            overlayLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderRegions(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);
            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);

            if (regionLayer.LayerSurface == null || regionOverlayLayer.LayerSurface == null) return;

            regionLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            regionLayer.Render(regionLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(regionLayer.LayerSurface, scrollPoint);

            regionOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            regionOverlayLayer.Render(regionOverlayLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(regionOverlayLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderRegionsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            regionLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);

            using SKBitmap b2 = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas2 = new(b2);

            canvas2.Clear(SKColors.Transparent);
            regionOverlayLayer.Render(canvas2);

            renderCanvas.DrawBitmap(b2, new SKPoint(0, 0));
        }


        internal static void RenderSymbols(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);
            if (symbolLayer.LayerSurface == null) return;

            symbolLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            symbolLayer.Render(symbolLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(symbolLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderSymbolsForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            symbolLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderVignette(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);
            if (vignetteLayer.LayerSurface == null) return;

            vignetteLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
            vignetteLayer.Render(vignetteLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(vignetteLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderVignetteForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            vignetteLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderWaterFeatures(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);

            if (waterLayer.LayerSurface == null || waterDrawingLayer.LayerSurface == null) return;

            waterLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

            if (waterLayer.ShowLayer)
            {
                // water features
                MapStateMediator.CurrentRiver?.Render(waterLayer.LayerSurface.Canvas);
                MapStateMediator.CurrentWaterFeature?.Render(waterLayer.LayerSurface.Canvas);

                // render rivers first, then water features (lakes, painted water features)
                // so that water features are painted on top of rivers to make it appear that
                // rivers flow into/out of water features
                foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (w is River r)
                    {
                        r.Render(waterLayer.LayerSurface.Canvas);

                        if (r.ShowRiverPoints)
                        {
                            List<MapRiverPoint> controlPoints = r.GetRiverControlPoints();

                            foreach (MapRiverPoint p in controlPoints)
                            {
                                if (p.IsSelected)
                                {
                                    waterLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, r.RiverWidth / 2.0F, PaintObjects.RiverSelectedControlPointPaint);
                                    waterLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, r.RiverWidth / 2.0F, PaintObjects.RiverControlPointOutlinePaint);
                                }
                                else
                                {
                                    waterLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, r.RiverWidth / 2.0F, PaintObjects.RiverControlPointPaint);
                                    waterLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, r.RiverWidth / 2.0F, PaintObjects.RiverControlPointOutlinePaint);
                                }
                            }
                        }
                    }
                }

                foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (w is WaterFeature wf)
                    {
                        wf.Render(waterLayer.LayerSurface.Canvas);
                    }
                }

                renderCanvas.DrawSurface(waterLayer.LayerSurface, scrollPoint);

                // water drawing layer (colors painted on top of water features)
                waterDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                waterDrawingLayer.Render(waterDrawingLayer.LayerSurface.Canvas);

                renderCanvas.DrawSurface(waterDrawingLayer.LayerSurface, scrollPoint);

                // selection layer
                MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

                if (selectionLayer.LayerSurface != null)
                {
                    selectionLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

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
        }

        internal static void RenderWaterFeaturesForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

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

            // water drawing layer (colors painted on top of water features)
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);

            using SKBitmap b2 = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas2 = new(b2);

            canvas2.Clear(SKColors.Transparent);
            waterDrawingLayer.Render(canvas2);

            renderCanvas.DrawBitmap(b2, new SKPoint(0, 0));
        }


        internal static void RenderWindroses(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);
            if (windroseLayer.LayerSurface == null) return;

            windroseLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

            MapStateMediator.CurrentWindrose?.Render(windroseLayer.LayerSurface.Canvas);

            windroseLayer.Render(windroseLayer.LayerSurface.Canvas);

            renderCanvas.DrawSurface(windroseLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderWindrosesForExport(RealmStudioMap map, SKCanvas renderCanvas)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);

            using SKBitmap b = new(new SKImageInfo(map.MapWidth, map.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Transparent);
            windroseLayer.Render(canvas);

            renderCanvas.DrawBitmap(b, new SKPoint(0, 0));
        }

        internal static void RenderWorkLayer(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);

            if (workLayer.LayerSurface != null)
            {
                // code that makes use of the work layer is responsible for clearing
                // and drawing to the canvas properly; it is only rendered here
                //workLayer.Render(workLayer.LayerSurface.Canvas);
                renderCanvas.DrawSurface(workLayer.LayerSurface, scrollPoint);
            }
        }
    }
}
