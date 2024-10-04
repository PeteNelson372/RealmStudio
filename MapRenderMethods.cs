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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal class MapRenderMethods
    {
        internal static void ClearSelectionLayer(RealmStudioMap map)
        {
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);
            selectionLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        internal static void CreateMapLayers(RealmStudioMap map, GRContext grContext)
        {
            foreach (MapLayer layer in map.MapLayers)
            {
                // if the surfaces haven't been created, create them
                layer.LayerSurface ??= SKSurface.Create(grContext, false, new SKImageInfo(map.MapWidth, map.MapHeight));
            }
        }

        internal static void RenderBackground(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render base layer
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER);

            baseLayer.RenderPicture?.Dispose();

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            recorder.RecordingCanvas.Clear(SKColors.White);

            baseLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            baseLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(baseLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderBoxes(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            boxLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            boxLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(boxLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderCursor(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // paint cursor layer
            MapLayer cursorLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.CURSORLAYER);
            e.Surface.Canvas.DrawSurface(cursorLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderDrawing(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // TODO
        }

        internal static void RenderLowerGrid(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            belowSymbolGridLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            belowSymbolGridLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(belowSymbolGridLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderUpperGrid(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            aboveOceanGridLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            aboveOceanGridLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(aboveOceanGridLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderDefaultGrid(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            defaultGridLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            defaultGridLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(defaultGridLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderLabels(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            labelLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            labelLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(labelLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderLandforms(RealmStudioMap map, Landform? currentLandform, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render landforms
            MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDCOASTLINELAYER);
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            selectionLayer.RenderPicture?.Dispose();
            selectionLayer.RenderPicture = null;

            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            using var coastlineRecorder = new SKPictureRecorder();
            using var landFormRecorder = new SKPictureRecorder();
            using var drawingRecorder = new SKPictureRecorder();
            using var selectionRecorder = new SKPictureRecorder();

            // Start recording 
            coastlineRecorder.BeginRecording(clippingBounds);
            currentLandform?.RenderCoastline(coastlineRecorder.RecordingCanvas);

            landFormRecorder.BeginRecording(clippingBounds);
            currentLandform?.RenderLandform(landFormRecorder.RecordingCanvas);

            foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                l.RenderCoastline(coastlineRecorder.RecordingCanvas);
                l.RenderLandform(landFormRecorder.RecordingCanvas);

                if (l.IsSelected)
                {
                    // draw an outline around the landform to show that it is selected
                    l.ContourPath.GetBounds(out SKRect boundRect);
                    using SKPath boundsPath = new();
                    boundsPath.AddRect(boundRect);

                    // only one landform can be selected
                    selectionRecorder.BeginRecording(clippingBounds);
                    selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                    selectionLayer.RenderPicture = selectionRecorder.EndRecording();
                }
            }

            landCoastlineLayer.RenderPicture = coastlineRecorder?.EndRecording();
            landformLayer.RenderPicture = landFormRecorder?.EndRecording();

            drawingRecorder.BeginRecording(clippingBounds);
            landDrawingLayer.Render(drawingRecorder.RecordingCanvas);
            landDrawingLayer.RenderPicture = drawingRecorder.EndRecording();

            // paint landform coastline layer
            e.Surface.Canvas.DrawPicture(landCoastlineLayer.RenderPicture, scrollPoint);

            // paint landform layer
            e.Surface.Canvas.DrawPicture(landformLayer.RenderPicture, scrollPoint);

            // paint land drawing layer
            e.Surface.Canvas.DrawPicture(landDrawingLayer.RenderPicture, scrollPoint);

            if (selectionLayer.RenderPicture != null)
            {
                // paint selection layer
                e.Surface.Canvas.DrawPicture(selectionLayer.RenderPicture, scrollPoint);
            }
        }

        internal static void RenderLowerMapPaths(RealmStudioMap map, MapPath? currentPath, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            // TODO: change to use picture recorder for rendering
            // path lower layer
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);

            if (pathLowerLayer.LayerSurface != null)
            {
                pathLowerLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                if (currentPath != null && !currentPath.DrawOverSymbols)
                {
                    currentPath.Render(pathLowerLayer.LayerSurface.Canvas);
                }

                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(pathLowerLayer.LayerSurface.Canvas);

                    if (mp.IsSelected)
                    {
                        if (mp.BoundaryPath != null)
                        {
                            // draw an outline around the path to show that it is selected
                            mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                        }
                    }

                    if (mp.ShowPathPoints)
                    {
                        List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                        foreach (MapPathPoint p in controlPoints)
                        {
                            if (p.IsSelected)
                            {
                                pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathSelectedControlPointPaint);
                            }
                            else
                            {
                                pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointPaint);
                            }

                            pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointOutlinePaint);
                        }
                    }
                }

                e.Surface.Canvas.DrawSurface(pathLowerLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderUpperMapPaths(RealmStudioMap map, MapPath? currentPath, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            // TODO: change to use picture recorder for rendering
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);

            // path upper layer

            if (pathUpperLayer.LayerSurface != null)
            {
                pathUpperLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                if (currentPath != null && currentPath.DrawOverSymbols)
                {
                    currentPath.Render(pathUpperLayer.LayerSurface.Canvas);
                }

                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(pathUpperLayer.LayerSurface.Canvas);

                    if (mp.IsSelected)
                    {
                        if (mp.BoundaryPath != null)
                        {
                            // draw an outline around the path to show that it is selected
                            mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                        }
                    }

                    if (mp.ShowPathPoints)
                    {
                        List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                        foreach (MapPathPoint p in controlPoints)
                        {
                            if (p.IsSelected)
                            {
                                pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathSelectedControlPointPaint);
                            }
                            else
                            {
                                pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointPaint);
                            }

                            pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointOutlinePaint);
                        }
                    }
                }

                e.Surface.Canvas.DrawSurface(pathUpperLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderMeasures(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            measureLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            measureLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(measureLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderOcean(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render ocean texture layer
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            if (oceanTextureLayer.LayerSurface != null)
            {
                oceanTextureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureLayer.Render(oceanTextureLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(oceanTextureLayer.LayerSurface, scrollPoint);
            }

            // render ocean texture overlay layer (ocean color fill)
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            if (oceanTextureOverlayLayer.LayerSurface != null)
            {
                oceanTextureOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureOverlayLayer.Render(oceanTextureOverlayLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(oceanTextureOverlayLayer.LayerSurface, scrollPoint);
            }

            // render ocean drawing layer (user-painted color)
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);
            if (oceanDrawingLayer.LayerSurface != null)
            {
                oceanDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanDrawingLayer.Render(oceanDrawingLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(oceanDrawingLayer.LayerSurface, scrollPoint);
            }
        }

        internal static void RenderFrame(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);

            if (frameLayer.ShowLayer && frameLayer.MapLayerComponents.Count > 0)
            {
                // there can only be one frame on the map, so get it and check to see if it is enabled
                PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents[0];

                if (placedFrame.FrameEnabled)
                {
                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    frameLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    frameLayer.RenderPicture = recorder.EndRecording();

                    e.Surface.Canvas.DrawPicture(frameLayer.RenderPicture, scrollPoint);
                }
            }
        }

        internal static void RenderRegions(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            regionLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            regionLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(regionLayer.RenderPicture, scrollPoint);

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);
            e.Surface.Canvas.DrawSurface(regionOverlayLayer.LayerSurface, scrollPoint);

            regionOverlayLayer.LayerSurface?.Canvas.Clear();
        }

        internal static void RenderSelections(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // paint selection layer
            //MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);
            //e.Surface.Canvas.DrawSurface(selectionLayer.LayerSurface, scrollPoint);
        }

        internal static void RenderSymbols(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            // Dispose of any previous Pictures
            symbolLayer.RenderPicture?.Dispose();

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                MapSymbol symbol = (MapSymbol)symbolLayer.MapLayerComponents[i];
                if (symbol.PlacedBitmap != null)
                {
                    symbol.Render(recorder.RecordingCanvas);

                    if (symbol.IsSelected)
                    {
                        // draw line around the symbol bitmap to show it is selected
                        SKRect selectRect = new(symbol.X, symbol.Y, symbol.X + symbol.Width, symbol.Y + symbol.Height);

                        recorder.RecordingCanvas.DrawRect(selectRect, PaintObjects.MapSymbolSelectPaint);
                    }
                }
            }

            // Create a new SKPicture with recorded Draw commands 
            symbolLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(symbolLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderVignette(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);

            vignetteLayer.RenderPicture?.Dispose();

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            vignetteLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            vignetteLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(vignetteLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderWaterFeatures(RealmStudioMap map, WaterFeature? currentWaterFeature, River? currentRiver, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // use picture recorder for rendering
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);


            selectionLayer.RenderPicture?.Dispose();
            selectionLayer.RenderPicture = null;

            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            using var waterFeatureRecorder = new SKPictureRecorder();
            using var drawingRecorder = new SKPictureRecorder();
            using var selectionRecorder = new SKPictureRecorder();

            waterFeatureRecorder.BeginRecording(clippingBounds);
            drawingRecorder.BeginRecording(clippingBounds);

            // water features
            currentWaterFeature?.Render(waterFeatureRecorder.RecordingCanvas);
            currentRiver?.Render(waterFeatureRecorder.RecordingCanvas);

            // render layer paint strokes
            waterDrawingLayer.Render(drawingRecorder.RecordingCanvas);

            foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
            {
                if (w is WaterFeature wf)
                {
                    wf.Render(waterFeatureRecorder.RecordingCanvas);

                    if (wf.IsSelected)
                    {
                        // draw an outline around the water feature to show that it is selected
                        wf.WaterFeaturePath.GetBounds(out SKRect boundRect);
                        using SKPath boundsPath = new();
                        boundsPath.AddRect(boundRect);

                        selectionRecorder.BeginRecording(clippingBounds);
                        selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                        selectionLayer.RenderPicture = selectionRecorder.EndRecording();
                    }
                }
                else if (w is River r)
                {
                    r.Render(waterFeatureRecorder.RecordingCanvas);

                    if (r.IsSelected)
                    {
                        if (r.RiverBoundaryPath != null)
                        {
                            // draw an outline around the path to show that it is selected
                            r.RiverBoundaryPath.GetTightBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionRecorder.BeginRecording(clippingBounds);
                            selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                            selectionLayer.RenderPicture = selectionRecorder.EndRecording();
                        }
                    }

                    if (r.ShowRiverPoints)
                    {
                        List<MapRiverPoint> controlPoints = r.GetRiverControlPoints();

                        foreach (MapRiverPoint p in controlPoints)
                        {
                            waterFeatureRecorder.RecordingCanvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, 2.0F, PaintObjects.RiverControlPointPaint);
                            waterFeatureRecorder.RecordingCanvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, 2.0F, PaintObjects.RiverControlPointOutlinePaint);
                        }
                    }
                }
            }

            waterLayer.RenderPicture = waterFeatureRecorder.EndRecording();
            waterDrawingLayer.RenderPicture = drawingRecorder.EndRecording();

            // paint water layer
            e.Surface.Canvas.DrawPicture(waterLayer.RenderPicture, scrollPoint);

            // paint water drawing layer
            e.Surface.Canvas.DrawPicture(waterDrawingLayer.RenderPicture, scrollPoint);

            if (selectionLayer.RenderPicture != null)
            {
                // paint selection layer
                e.Surface.Canvas.DrawPicture(selectionLayer.RenderPicture, scrollPoint);
            }
        }

        internal static void RenderWindroses(RealmStudioMap map, MapWindrose? currentWindrose, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);

            // Dispose of any previous Pictures
            windroseLayer.RenderPicture?.Dispose();

            // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
            using var recorder = new SKPictureRecorder();
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // Start recording 
            recorder.BeginRecording(clippingBounds);

            currentWindrose?.Render(recorder.RecordingCanvas);
            windroseLayer.Render(recorder.RecordingCanvas);

            // Create a new SKPicture with recorded Draw commands 
            windroseLayer.RenderPicture = recorder.EndRecording();

            e.Surface.Canvas.DrawPicture(windroseLayer.RenderPicture, scrollPoint);
        }

        internal static void RenderWorkLayer(RealmStudioMap map, SKPaintGLSurfaceEventArgs e, SKPoint scrollPoint)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
            e.Surface.Canvas.DrawSurface(workLayer.LayerSurface, scrollPoint);
        }
    }
}
