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
            selectionLayer.RenderPicture?.Dispose();
            selectionLayer.RenderPicture = null;
        }

        internal static void RenderBackground(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render base layer
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER);

            if (baseLayer.ShowLayer)
            {
                if (!baseLayer.IsModified && baseLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(baseLayer.RenderPicture, scrollPoint);
                }
                else
                {
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

                    renderCanvas.DrawPicture(baseLayer.RenderPicture, scrollPoint);

                    MapBuilder.SetLayerModified(map, MapBuilder.BASELAYER, false);
                }
            }
            else
            {
                baseLayer.RenderPicture?.Dispose();
            }
        }

        internal static void RenderBoxes(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);

            if (boxLayer.ShowLayer)
            {
                if (!boxLayer.IsModified && boxLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(boxLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    boxLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    boxLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    boxLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(boxLayer.RenderPicture, scrollPoint);
                    MapBuilder.SetLayerModified(map, MapBuilder.BOXLAYER, false);
                }
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

            if (belowSymbolGridLayer.ShowLayer)
            {
                if (!belowSymbolGridLayer.IsModified && belowSymbolGridLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(belowSymbolGridLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    belowSymbolGridLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    belowSymbolGridLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    belowSymbolGridLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(belowSymbolGridLayer.RenderPicture, scrollPoint);
                    MapBuilder.SetLayerModified(map, MapBuilder.BELOWSYMBOLSGRIDLAYER, false);
                }
            }
        }

        internal static void RenderUpperGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);

            if (aboveOceanGridLayer.ShowLayer)
            {
                if (!aboveOceanGridLayer.IsModified && aboveOceanGridLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(aboveOceanGridLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    aboveOceanGridLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    aboveOceanGridLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    aboveOceanGridLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(aboveOceanGridLayer.RenderPicture, scrollPoint);
                    MapBuilder.SetLayerModified(map, MapBuilder.ABOVEOCEANGRIDLAYER, false);
                }
            }
        }

        internal static void RenderDefaultGrid(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render default grid layer
            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);

            if (defaultGridLayer.ShowLayer)
            {
                if (!defaultGridLayer.IsModified && defaultGridLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(defaultGridLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    defaultGridLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    defaultGridLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    defaultGridLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(defaultGridLayer.RenderPicture, scrollPoint);
                    MapBuilder.SetLayerModified(map, MapBuilder.DEFAULTGRIDLAYER, false);
                }
            }
        }

        internal static void RenderLabels(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            if (labelLayer.ShowLayer)
            {
                if (!labelLayer.IsModified && labelLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(labelLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    labelLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    labelLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    labelLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(labelLayer.RenderPicture, scrollPoint);
                    MapBuilder.SetLayerModified(map, MapBuilder.LABELLAYER, false);
                }
            }
        }

        internal static void RenderLandforms(RealmStudioMap map, Landform? currentLandform, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // render landforms
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDCOASTLINELAYER);
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            if (landCoastlineLayer.ShowLayer)
            {
                // coastline
                if (!landCoastlineLayer.IsModified && landCoastlineLayer.RenderPicture != null)
                {
                    // paint landform coastline layer
                    renderCanvas.DrawPicture(landCoastlineLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    landCoastlineLayer.RenderPicture?.Dispose();

                    using var coastlineRecorder = new SKPictureRecorder();

                    // Start recording 
                    coastlineRecorder.BeginRecording(clippingBounds);
                    currentLandform?.RenderCoastline(coastlineRecorder.RecordingCanvas);

                    foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
                    {
                        l.RenderCoastline(coastlineRecorder.RecordingCanvas);
                    }

                    landCoastlineLayer.RenderPicture = coastlineRecorder?.EndRecording();
                    renderCanvas.DrawPicture(landCoastlineLayer.RenderPicture, scrollPoint);
                    landCoastlineLayer.IsModified = false;
                }
            }

            if (landformLayer.ShowLayer)
            {
                //landform
                if (!landformLayer.IsModified && landformLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(landformLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    landformLayer.RenderPicture?.Dispose();

                    using var landFormRecorder = new SKPictureRecorder();

                    landFormRecorder.BeginRecording(clippingBounds);
                    currentLandform?.RenderLandform(landFormRecorder.RecordingCanvas);

                    foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
                    {
                        l.RenderLandform(landFormRecorder.RecordingCanvas);
                    }

                    landformLayer.RenderPicture = landFormRecorder?.EndRecording();
                    renderCanvas.DrawPicture(landformLayer.RenderPicture, scrollPoint);
                    landformLayer.IsModified = false;
                }
            }

            // landform drawing (color painting over landform)
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDDRAWINGLAYER);

            if (landDrawingLayer.ShowLayer)
            {
                // using a picture recorder to provide a canvas to draw on
                // causes transparent (or empty) color to render as black;
                // using a bitmap canvas correctly renders transparent and empty colors

                SKBitmap b = new SKBitmap(map.MapWidth, map.MapHeight);
                SKCanvas canvas = new(b);
                landDrawingLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, scrollPoint);
            }

            if (landformLayer.ShowLayer)
            {
                // landform selection
                selectionLayer.RenderPicture?.Dispose();
                selectionLayer.RenderPicture = null;

                using var selectionRecorder = new SKPictureRecorder();

                foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
                {
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

                        break;
                    }
                }

                if (selectionLayer.RenderPicture != null)
                {
                    // paint selection layer
                    renderCanvas.DrawPicture(selectionLayer.RenderPicture, scrollPoint);
                }
            }
        }

        internal static void RenderLowerMapPaths(RealmStudioMap map, MapPath? currentPath, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);

            if (pathLowerLayer.ShowLayer)
            {

                if (!pathLowerLayer.IsModified && pathLowerLayer.RenderPicture != null)
                {
                    // paint the path lower layer
                    renderCanvas.DrawPicture(pathLowerLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    pathLowerLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    if (currentPath != null && !currentPath.DrawOverSymbols)
                    {
                        currentPath.Render(recorder.RecordingCanvas);
                    }

                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.Render(recorder.RecordingCanvas);

                        if (mp.ShowPathPoints)
                        {
                            List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                            foreach (MapPathPoint p in controlPoints)
                            {
                                if (p.IsSelected)
                                {
                                    recorder.RecordingCanvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathSelectedControlPointPaint);
                                }
                                else
                                {
                                    recorder.RecordingCanvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointPaint);
                                }

                                recorder.RecordingCanvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointOutlinePaint);
                            }
                        }
                    }

                    pathLowerLayer.RenderPicture = recorder.EndRecording();

                    // paint the path lower layer
                    renderCanvas.DrawPicture(pathLowerLayer.RenderPicture, scrollPoint);
                    pathLowerLayer.IsModified = false;
                }
            }

            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);
            selectionLayer.RenderPicture?.Dispose();
            selectionLayer.RenderPicture = null;

            if (pathLowerLayer.ShowLayer)
            {
                using var selectionRecorder = new SKPictureRecorder();

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

                            selectionRecorder.BeginRecording(clippingBounds);
                            selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                            selectionLayer.RenderPicture = selectionRecorder.EndRecording();
                            break;
                        }
                    }
                }

                if (selectionLayer.RenderPicture != null)
                {
                    // paint selection layer
                    renderCanvas.DrawPicture(selectionLayer.RenderPicture, scrollPoint);
                }
            }
        }

        internal static void RenderUpperMapPaths(RealmStudioMap map, MapPath? currentPath, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // path upper layer
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);

            if (pathUpperLayer.ShowLayer)
            {
                if (!pathUpperLayer.IsModified && pathUpperLayer.RenderPicture != null)
                {
                    // paint the path upper layer
                    renderCanvas.DrawPicture(pathUpperLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    pathUpperLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    if (currentPath != null && currentPath.DrawOverSymbols)
                    {
                        currentPath.Render(recorder.RecordingCanvas);
                    }

                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.Render(recorder.RecordingCanvas);

                        if (mp.ShowPathPoints)
                        {
                            List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                            foreach (MapPathPoint p in controlPoints)
                            {
                                if (p.IsSelected)
                                {
                                    recorder.RecordingCanvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathSelectedControlPointPaint);
                                }
                                else
                                {
                                    recorder.RecordingCanvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointPaint);
                                }

                                recorder.RecordingCanvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 4.0F, PaintObjects.MapPathControlPointOutlinePaint);
                            }
                        }
                    }

                    pathUpperLayer.RenderPicture = recorder.EndRecording();

                    // paint the path upper layer
                    renderCanvas.DrawPicture(pathUpperLayer.RenderPicture, scrollPoint);
                    pathUpperLayer.IsModified = false;
                }
            }

            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            selectionLayer.RenderPicture?.Dispose();
            selectionLayer.RenderPicture = null;

            if (pathUpperLayer.ShowLayer)
            {
                using var selectionRecorder = new SKPictureRecorder();

                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    if (mp.IsSelected)
                    {
                        if (mp.BoundaryPath != null)
                        {
                            // draw an outline around the path to show that it is selected
                            mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionRecorder.BeginRecording(clippingBounds);
                            selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                            selectionLayer.RenderPicture = selectionRecorder.EndRecording();
                            break;
                        }
                    }
                }

                if (selectionLayer.RenderPicture != null)
                {
                    // paint selection layer
                    renderCanvas.DrawPicture(selectionLayer.RenderPicture, scrollPoint);
                }
            }
        }

        internal static void RenderMeasures(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER);

            if (measureLayer.ShowLayer)
            {
                if (!measureLayer.IsModified && measureLayer.RenderPicture != null)
                {
                    // paint the measure layer
                    renderCanvas.DrawPicture(measureLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    measureLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    measureLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    measureLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(measureLayer.RenderPicture, scrollPoint);
                    measureLayer.IsModified = false;
                }
            }
        }

        internal static void RenderOcean(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // render ocean layers
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OCEANDRAWINGLAYER);

            if (oceanTextureLayer.ShowLayer)
            {
                // ocean texture layer            
                if (!oceanTextureLayer.IsModified && oceanTextureLayer.RenderPicture != null)
                {
                    // paint the ocean texture layer
                    renderCanvas.DrawPicture(oceanTextureLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    oceanTextureLayer.RenderPicture?.Dispose();

                    using var oceanTextureRecorder = new SKPictureRecorder();
                    oceanTextureRecorder.BeginRecording(clippingBounds);

                    oceanTextureLayer.Render(oceanTextureRecorder.RecordingCanvas);
                    oceanTextureLayer.RenderPicture = oceanTextureRecorder.EndRecording();

                    renderCanvas.DrawPicture(oceanTextureLayer.RenderPicture, scrollPoint);
                    oceanTextureLayer.IsModified = false;
                }
            }

            if (oceanTextureOverlayLayer.ShowLayer)
            {
                // render ocean texture overlay layer (ocean color fill)
                if (!oceanTextureOverlayLayer.IsModified && oceanTextureOverlayLayer.RenderPicture != null)
                {
                    // paint the ocean texture layer
                    renderCanvas.DrawPicture(oceanTextureOverlayLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    oceanTextureOverlayLayer.RenderPicture?.Dispose();

                    using var oceanTextureOverlayRecorder = new SKPictureRecorder();
                    oceanTextureOverlayRecorder.BeginRecording(clippingBounds);

                    oceanTextureOverlayLayer.Render(oceanTextureOverlayRecorder.RecordingCanvas);
                    oceanTextureOverlayLayer.RenderPicture = oceanTextureOverlayRecorder.EndRecording();

                    renderCanvas.DrawPicture(oceanTextureOverlayLayer.RenderPicture, scrollPoint);
                    oceanTextureOverlayLayer.IsModified = false;
                }
            }


            if (oceanDrawingLayer.ShowLayer)
            {
                // using a picture recorder to provide a canvas to draw on
                // causes transparent (or empty) color to render as black;
                // using a bitmap canvas correctly renders transparent and empty colors

                SKBitmap b = new SKBitmap(map.MapWidth, map.MapHeight);
                SKCanvas canvas = new(b);
                oceanDrawingLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, scrollPoint);
            }
        }

        internal static void RenderFrame(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER);

            if (frameLayer.ShowLayer && frameLayer.MapLayerComponents.Count > 0)
            {
                // there can only be one frame on the map, so get it and check to see if it is enabled
                PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents[0];

                if (!frameLayer.IsModified && frameLayer.RenderPicture != null && placedFrame.FrameEnabled)
                {
                    // paint the framelayer
                    renderCanvas.DrawPicture(frameLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    // there can only be one frame on the map, so check to see if it is enabled
                    if (placedFrame.FrameEnabled)
                    {
                        frameLayer.RenderPicture?.Dispose();

                        // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                        using var recorder = new SKPictureRecorder();
                        SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                        // Start recording 
                        recorder.BeginRecording(clippingBounds);

                        frameLayer.Render(recorder.RecordingCanvas);

                        // Create a new SKPicture with recorded Draw commands 
                        frameLayer.RenderPicture = recorder.EndRecording();

                        renderCanvas.DrawPicture(frameLayer.RenderPicture, scrollPoint);
                        frameLayer.IsModified = false;
                    }
                }
            }
        }

        internal static void RenderOverlays(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER);

            if (overlayLayer.ShowLayer && overlayLayer.MapLayerComponents.Count > 0)
            {
                if (!overlayLayer.IsModified && overlayLayer.RenderPicture != null)
                {
                    // paint the overlay layer
                    renderCanvas.DrawPicture(overlayLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    overlayLayer.RenderPicture?.Dispose();
                    overlayLayer.RenderPicture = null;

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    overlayLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    overlayLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(overlayLayer.RenderPicture, scrollPoint);
                    overlayLayer.IsModified = false;
                }
            }
        }

        internal static void RenderRegions(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

            if (regionLayer.ShowLayer)
            {
                if (!regionLayer.IsModified && regionLayer.RenderPicture != null)
                {
                    // paint the region layer
                    renderCanvas.DrawPicture(regionLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    regionLayer.RenderPicture?.Dispose();

                    // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                    using var recorder = new SKPictureRecorder();
                    SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                    // Start recording 
                    recorder.BeginRecording(clippingBounds);

                    regionLayer.Render(recorder.RecordingCanvas);

                    // Create a new SKPicture with recorded Draw commands 
                    regionLayer.RenderPicture = recorder.EndRecording();

                    renderCanvas.DrawPicture(regionLayer.RenderPicture, scrollPoint);
                    regionLayer.IsModified = false;
                }
            }

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONOVERLAYLAYER);

            if (regionOverlayLayer.ShowLayer)
            {
                if (regionOverlayLayer.RenderPicture != null)
                {
                    renderCanvas.DrawPicture(regionOverlayLayer.RenderPicture, scrollPoint);
                    regionOverlayLayer.RenderPicture.Dispose();
                    regionOverlayLayer.RenderPicture = null;
                }
            }
        }

        internal static void RenderSymbols(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            if (symbolLayer.ShowLayer)
            {
                if (!symbolLayer.IsModified && symbolLayer.RenderPicture != null)
                {
                    // paint the symbol layer
                    renderCanvas.DrawPicture(symbolLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    // Dispose of any previous picture
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

                    renderCanvas.DrawPicture(symbolLayer.RenderPicture, scrollPoint);
                    symbolLayer.IsModified = false;
                }
            }
        }

        internal static void RenderVignette(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER);

            if (!vignetteLayer.IsModified && vignetteLayer.RenderPicture != null)
            {
                // paint the symbol layer
                renderCanvas.DrawPicture(vignetteLayer.RenderPicture, scrollPoint);
            }
            else
            {
                vignetteLayer.RenderPicture?.Dispose();

                // Create an SKPictureRecorder to record the Canvas Draw commands to an SKPicture
                using var recorder = new SKPictureRecorder();
                SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

                // Start recording 
                recorder.BeginRecording(clippingBounds);
                vignetteLayer.Render(recorder.RecordingCanvas);

                // Create a new SKPicture with recorded Draw commands 
                vignetteLayer.RenderPicture = recorder.EndRecording();

                renderCanvas.DrawPicture(vignetteLayer.RenderPicture, scrollPoint);
                vignetteLayer.IsModified = false;
            }
        }

        internal static void RenderWaterFeatures(RealmStudioMap map, WaterFeature? currentWaterFeature, River? currentRiver, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            SKRect clippingBounds = new(0, 0, map.MapWidth, map.MapHeight);

            // use picture recorder for rendering
            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            if (waterLayer.ShowLayer)
            {
                if (!waterLayer.IsModified && waterLayer.RenderPicture != null)
                {
                    // paint the symbol layer
                    renderCanvas.DrawPicture(waterLayer.RenderPicture, scrollPoint);
                }
                else
                {
                    waterLayer.RenderPicture?.Dispose();

                    using var waterFeatureRecorder = new SKPictureRecorder();
                    waterFeatureRecorder.BeginRecording(clippingBounds);

                    // water features
                    currentWaterFeature?.Render(waterFeatureRecorder.RecordingCanvas);
                    currentRiver?.Render(waterFeatureRecorder.RecordingCanvas);

                    foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                    {
                        if (w is WaterFeature wf)
                        {
                            wf.Render(waterFeatureRecorder.RecordingCanvas);
                        }
                        else if (w is River r)
                        {
                            r.Render(waterFeatureRecorder.RecordingCanvas);

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

                    // paint water layer
                    renderCanvas.DrawPicture(waterLayer.RenderPicture, scrollPoint);
                    waterLayer.IsModified = false;
                }
            }

            // water drawing layer (colors painted on top ofwater features)
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERDRAWINGLAYER);

            if (waterDrawingLayer.ShowLayer)
            {
                // using a picture recorder to provide a canvas to draw on
                // causes transparent (or empty) color to render as black;
                // using a bitmap canvas correctly renders transparent and empty colors

                SKBitmap b = new SKBitmap(map.MapWidth, map.MapHeight);
                SKCanvas canvas = new(b);
                waterDrawingLayer.Render(canvas);

                renderCanvas.DrawBitmap(b, scrollPoint);
            }

            // selection layer
            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SELECTIONLAYER);

            selectionLayer.RenderPicture?.Dispose();
            selectionLayer.RenderPicture = null;

            if (waterLayer.ShowLayer)
            {
                using var selectionRecorder = new SKPictureRecorder();

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

                            selectionRecorder.BeginRecording(clippingBounds);
                            selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                            selectionLayer.RenderPicture = selectionRecorder.EndRecording();
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

                                selectionRecorder.BeginRecording(clippingBounds);
                                selectionRecorder.RecordingCanvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                                selectionLayer.RenderPicture = selectionRecorder.EndRecording();
                                break;
                            }
                        }
                    }
                }

                if (selectionLayer.RenderPicture != null)
                {
                    // paint selection layer
                    renderCanvas.DrawPicture(selectionLayer.RenderPicture, scrollPoint);
                }
            }
        }

        internal static void RenderWindroses(RealmStudioMap map, MapWindrose? currentWindrose, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WINDROSELAYER);

            if (!windroseLayer.IsModified && windroseLayer.RenderPicture != null)
            {
                // paint the symbol layer
                renderCanvas.DrawPicture(windroseLayer.RenderPicture, scrollPoint);
            }
            else
            {
                // Dispose of any previous picture
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

                renderCanvas.DrawPicture(windroseLayer.RenderPicture, scrollPoint);
                windroseLayer.IsModified = false;
            }
        }

        internal static void RenderWorkLayer(RealmStudioMap map, SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);

            if (workLayer.RenderPicture != null)
            {
                renderCanvas.DrawPicture(workLayer.RenderPicture, scrollPoint);
            }
        }
    }
}
