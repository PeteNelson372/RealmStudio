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
    internal sealed class MapRegionMethods
    {
        public static int POINT_CIRCLE_RADIUS = 5;

        public static bool EDITING_REGION;
        public static MapRegionPoint? NEW_REGION_POINT;
        public static int PREVIOUS_REGION_POINT_INDEX = -1;
        public static int NEXT_REGION_POINT_INDEX = -1;

        internal static MapRegion? SelectRegionAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            MapRegion? selectedRegion = null;

            List<MapComponent> mapRegionComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER).MapLayerComponents;

            for (int i = 0; i < mapRegionComponents.Count; i++)
            {
                if (mapRegionComponents[i] is MapRegion mapRegion)
                {
                    SKPath? boundaryPath = mapRegion.BoundaryPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                        {
                            mapRegion.IsSelected = !mapRegion.IsSelected;

                            if (mapRegion.IsSelected)
                            {
                                selectedRegion = mapRegion;
                            }
                            break;
                        }
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(map, selectedRegion);

            return selectedRegion;
        }

        public static SKPath GetLinePathFromRegionPoints(List<MapRegionPoint> points)
        {
            SKPath path = new();

            path.MoveTo(points.First().RegionPoint);

            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i].RegionPoint);
            }

            path.LineTo(points.First().RegionPoint);

            path.Close();

            return path;
        }

        public static SKPathEffect? ConstructRegionBorderEffect(MapRegion region)
        {
            SKPathEffect? pathEffect = null;
            switch (region.RegionBorderType)
            {
                case PathType.DottedLinePath:
                    float[] intervals = [0, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.DashedLinePath:
                    intervals = [region.RegionBorderWidth, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.DashDotLinePath:
                    intervals = [region.RegionBorderWidth, region.RegionBorderWidth * 2, 0, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.DashDotDotLinePath:
                    intervals = [region.RegionBorderWidth, region.RegionBorderWidth * 2, 0, region.RegionBorderWidth * 2, 0, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.LineAndDashesPath:
                    float ldWidth = Math.Max(1, region.RegionBorderWidth / 2.0F);

                    string svgPath = "M 0 0"
                        + " h" + (region.RegionBorderWidth).ToString()
                        + " v" + Math.Max(1, ldWidth / 2.0F).ToString()
                        + " h" + (-region.RegionBorderWidth).ToString()
                        + " M0" + "," + (region.RegionBorderWidth - 1.0F).ToString()
                        + " h" + (ldWidth).ToString()
                        + " v2"
                        + " h" + (-ldWidth).ToString();

                    pathEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(svgPath),
                        region.RegionBorderWidth, 0, SKPath1DPathEffectStyle.Morph);
                    break;
            }

            return pathEffect;
        }

        public static void ConstructRegionPaintObjects(MapRegion region, SKPathEffect? regionBorderEffect)
        {
            region.RegionBorderPaint = new SKPaint()
            {
                StrokeWidth = region.RegionBorderWidth,
                Color = region.RegionBorderColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateCorner(region.RegionBorderSmoothing)
            };

            Color innerColor = Color.FromArgb(region.RegionInnerOpacity, region.RegionBorderColor.R, region.RegionBorderColor.G, region.RegionBorderColor.B);

            region.RegionInnerPaint = new SKPaint()
            {
                Color = Extensions.ToSKColor(innerColor),
                Style = SKPaintStyle.Fill,
                PathEffect = SKPathEffect.CreateCorner(region.RegionBorderSmoothing)
            };

            if (regionBorderEffect != null)
            {
                region.RegionBorderPaint.PathEffect = SKPathEffect.CreateCompose(regionBorderEffect, SKPathEffect.CreateCorner(region.RegionBorderSmoothing));
            }
        }

        internal static void MoveSelectedRegionInRenderOrder(RealmStudioMap map, MapRegion? currentRegion, ComponentMoveDirection direction)
        {
            if (currentRegion != null)
            {
                // find the selected region in the Region Layer MapComponents
                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

                List<MapComponent> regionComponents = regionLayer.MapLayerComponents;
                MapRegion? selectedRegion = null;

                int selectedRegionIndex = 0;

                for (int i = 0; i < regionComponents.Count; i++)
                {
                    MapComponent regionComponent = regionComponents[i];
                    if (regionComponent is MapRegion region && region.RegionGuid.ToString() == currentRegion.RegionGuid.ToString())
                    {
                        selectedRegionIndex = i;
                        selectedRegion = region;
                        break;
                    }
                }

                if (direction == ComponentMoveDirection.Up)
                {
                    // moving a region up in render order means increasing its index
                    if (selectedRegion != null && selectedRegionIndex < regionComponents.Count - 1)
                    {
                        regionComponents[selectedRegionIndex] = regionComponents[selectedRegionIndex + 1];
                        regionComponents[selectedRegionIndex + 1] = selectedRegion;
                    }
                }
                else if (direction == ComponentMoveDirection.Down)
                {
                    // moving a region down in render order means decreasing its index
                    if (selectedRegion != null && selectedRegionIndex > 0)
                    {
                        regionComponents[selectedRegionIndex] = regionComponents[selectedRegionIndex - 1];
                        regionComponents[selectedRegionIndex - 1] = selectedRegion;
                    }
                }
            }
        }

        internal static void FinalizeMapRegions(RealmStudioMap map)
        {
            // finalize loading of regions
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);
            for (int i = 0; i < regionLayer.MapLayerComponents.Count; i++)
            {
                if (regionLayer.MapLayerComponents[i] is MapRegion region)
                {
                    region.ParentMap = map;
                    SKPathEffect? regionBorderEffect = ConstructRegionBorderEffect(region);
                    ConstructRegionPaintObjects(region, regionBorderEffect);
                }
            }
        }

        internal static void SnapRegionToLandformCoastline(RealmStudioMap map, MapRegion currentMapRegion, SKPoint zoomedScrolledPoint, SKPoint previousCursorPoint)
        {
            // find the closest point to the current point
            // on the contour path of a coastline;
            // if the nearest point on the coastline
            // is within 5 pixels of the current point,
            // then set the region point to be the point
            // on the coastline
            // then check the *previous* region point; if the previous
            // region point is on the coastline of the same landform
            // then add all of the points on the coastline contour
            // to the region points

            int coastlinePointIndex = -1;
            SKPoint coastlinePoint = SKPoint.Empty;
            Landform? landform1 = null;
            Landform? landform2 = null;

            float currentDistance = float.MaxValue;

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            // get the distance from the point the cursor was clicked to the contour points of all landforms
            foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                for (int i = 0; i < lf.ContourPoints.Count; i++)
                {
                    SKPoint p = lf.ContourPoints[i];
                    float distance = SKPoint.Distance(zoomedScrolledPoint, p);

                    if (distance < currentDistance && distance < 5)
                    {
                        landform1 = lf;
                        coastlinePointIndex = i;
                        coastlinePoint = p;
                        currentDistance = distance;
                    }
                }

                if (coastlinePointIndex >= 0) break;
            }

            int previousCoastlinePointIndex = -1;
            currentDistance = float.MaxValue;

            if (landform1 != null && coastlinePointIndex >= 0)
            {
                MapRegionPoint mrp = new(landform1.ContourPoints[coastlinePointIndex]);
                currentMapRegion.MapRegionPoints.Add(mrp);

                if (currentMapRegion.MapRegionPoints.Count > 1 && coastlinePointIndex > 1)
                {
                    SKPoint previousPoint = currentMapRegion.MapRegionPoints[^2].RegionPoint;

                    foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
                    {
                        for (int i = 0; i < lf.ContourPoints.Count; i++)
                        {
                            SKPoint p = lf.ContourPoints[i];
                            float distance = SKPoint.Distance(previousPoint, p);

                            if (distance < currentDistance && !coastlinePoint.Equals(previousPoint))
                            {
                                landform2 = lf;
                                previousCoastlinePointIndex = i;
                                currentDistance = distance;
                            }
                        }

                        if (previousCoastlinePointIndex >= 0) break;
                    }
                }
            }

            if (landform1 != null && landform2 != null
                && landform1.LandformGuid.ToString() == landform2.LandformGuid.ToString()
                && coastlinePointIndex >= 0 && previousCoastlinePointIndex >= 0)
            {
                currentMapRegion.MapRegionPoints.Clear();

                if (zoomedScrolledPoint.Y < previousCursorPoint.Y)
                {
                    // drag mouse up to snap to west coast of landform
                    for (int i = previousCoastlinePointIndex; i < landform1.ContourPoints.Count - 1; i++)
                    {
                        MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                        currentMapRegion.MapRegionPoints.Add(mrp);
                    }

                    for (int i = 0; i <= coastlinePointIndex; i++)
                    {
                        MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                        currentMapRegion.MapRegionPoints.Add(mrp);
                    }
                }
                else
                {
                    // drag mouse down to snap region to east coast of landform
                    if (coastlinePointIndex > previousCoastlinePointIndex)
                    {
                        for (int i = previousCoastlinePointIndex; i <= coastlinePointIndex; i++)
                        {
                            MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                            currentMapRegion.MapRegionPoints.Add(mrp);
                        }
                    }
                    else
                    {
                        for (int i = coastlinePointIndex; i <= previousCoastlinePointIndex; i++)
                        {
                            MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                            currentMapRegion.MapRegionPoints.Add(mrp);
                        }
                    }
                }
            }
        }

        internal static void EndMapRegion(RealmStudioMap map, MapRegion mapRegion, SKPoint zoomedScrolledPoint)
        {
            MapRegionPoint mrp = new(zoomedScrolledPoint);
            mapRegion.MapRegionPoints.Add(mrp);

            Cmd_AddMapRegion cmd = new(map, mapRegion);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            mapRegion.IsSelected = false;
        }

        internal static void DrawRegionOnWorkLayer(RealmStudioMap map, MapRegion mapRegion, SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
            MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

            if (mapRegion.MapRegionPoints.Count > 1)
            {
                // temporarily add the layer click point for rendering
                MapRegionPoint mrp = new(zoomedScrolledPoint);
                mapRegion.MapRegionPoints.Add(mrp);

                // render
                mapRegion.Render(MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas);

                // remove it
                mapRegion.MapRegionPoints.RemoveAt(mapRegion.MapRegionPoints.Count - 1);
            }
            else
            {
                MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawLine(previousPoint, zoomedScrolledPoint, mapRegion.RegionBorderPaint);
            }
        }

        internal static void MoveRegion(MapRegion mapRegion, SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            // not moving a point, so drag the region
            float xDelta = zoomedScrolledPoint.X - previousPoint.X;
            float yDelta = zoomedScrolledPoint.Y - previousPoint.Y;

            foreach (MapRegionPoint p in mapRegion.MapRegionPoints)
            {
                SKPoint newPoint = p.RegionPoint;
                newPoint.X += xDelta;
                newPoint.Y += yDelta;
                p.RegionPoint = newPoint;
            }
        }

        internal static MapRegionPoint? GetSelectedMapRegionPoint(MapRegion mapRegion, SKPoint zoomedScrolledPoint)
        {
            MapRegionPoint? selectedMapRegionPoint = null;

            foreach (MapRegionPoint p in mapRegion.MapRegionPoints)
            {
                using SKPath path = new();
                path.AddCircle(p.RegionPoint.X, p.RegionPoint.Y, 5);

                if (path.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                {
                    // editing (moving) the selected point
                    p.IsSelected = true;
                    selectedMapRegionPoint = p;
                    EDITING_REGION = true;
                }
                else
                {
                    p.IsSelected = false;
                }
            }

            return selectedMapRegionPoint;
        }

        internal static void DrawCoastlinePointOnWorkLayer(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            const int maxPointToCoastlineDistance = 5;

            // find the closest point to the current point
            // on the contour path of a coastline;
            // if the nearest point on the coastline
            // is within 5 pixels of the current point,
            // then set the region point to be the point
            // on the coastline
            // then check the *previous* region point; if the previous
            // region point is on the coastline of the same landform
            // then add all of the points on the coastline contour
            // to the region points

            int coastlinePointIndex = -1;
            SKPoint coastlinePoint = SKPoint.Empty;

            float currentDistance = float.MaxValue;

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            // get the distance from the cursor point to the contour points of all landforms
            foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                for (int i = 0; i < lf.ContourPoints.Count; i++)
                {
                    SKPoint p = lf.ContourPoints[i];
                    float distance = SKPoint.Distance(zoomedScrolledPoint, p);

                    if (distance < currentDistance && distance < maxPointToCoastlineDistance)
                    {
                        coastlinePointIndex = i;
                        coastlinePoint = p;
                        currentDistance = distance;
                    }
                }

                if (coastlinePointIndex >= 0) break;
            }

            if (coastlinePoint != SKPoint.Empty)
            {
                workLayer.LayerSurface?.Canvas.DrawCircle(coastlinePoint, 5, PaintObjects.MapPathSelectedControlPointPaint);
            }
        }

        internal static bool IsRegionPointSelected(MapRegion mapRegion, SKPoint zoomedScrolledPoint)
        {
            const int regionEditPointRadius = 5;
            bool regionPointSelected = false;

            foreach (MapRegionPoint p in mapRegion.MapRegionPoints)
            {
                using SKPath path = new();
                path.AddCircle(p.RegionPoint.X, p.RegionPoint.Y, regionEditPointRadius);

                if (path.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                {
                    regionPointSelected = true;
                    p.IsSelected = true;
                }
                else
                {
                    if (!EDITING_REGION)
                    {
                        p.IsSelected = false;
                    }
                }
            }

            return regionPointSelected;
        }

        internal static void DrawRegionPointOnWorkLayer(RealmStudioMap map, MapRegion mapRegion, SKPoint zoomedScrolledPoint)
        {
            MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

            PREVIOUS_REGION_POINT_INDEX = -1;
            NEXT_REGION_POINT_INDEX = -1;
            NEW_REGION_POINT = null;

            // is the cursor on a line segment between 2 region vertices? if so, draw a circle at the cursor location
            for (int i = 0; i < mapRegion.MapRegionPoints.Count - 1; i++)
            {
                if (DrawingMethods.LineContainsPoint(zoomedScrolledPoint,
                    mapRegion.MapRegionPoints[i].RegionPoint, mapRegion.MapRegionPoints[i + 1].RegionPoint))
                {
                    EDITING_REGION = true;

                    PREVIOUS_REGION_POINT_INDEX = i;
                    NEXT_REGION_POINT_INDEX = i + 1;

                    NEW_REGION_POINT = new MapRegionPoint(zoomedScrolledPoint);

                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, POINT_CIRCLE_RADIUS, PaintObjects.RegionNewPointFillPaint);
                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, POINT_CIRCLE_RADIUS, PaintObjects.RegionPointOutlinePaint);

                    break;
                }
            }

            // is the cursor on the segment between the first and last region vertices?
            if (DrawingMethods.LineContainsPoint(zoomedScrolledPoint, mapRegion.MapRegionPoints[0].RegionPoint,
                mapRegion.MapRegionPoints[^1].RegionPoint))
            {
                EDITING_REGION = true;

                PREVIOUS_REGION_POINT_INDEX = 0;
                NEXT_REGION_POINT_INDEX = mapRegion.MapRegionPoints.Count;

                NEW_REGION_POINT = new MapRegionPoint(zoomedScrolledPoint);

                MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, POINT_CIRCLE_RADIUS, PaintObjects.RegionNewPointFillPaint);
                MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, POINT_CIRCLE_RADIUS, PaintObjects.RegionPointOutlinePaint);
            }
        }
    }
}
