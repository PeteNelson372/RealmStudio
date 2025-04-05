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
    internal sealed class RegionManager : IMapComponentManager
    {
        private const int _pointCircleRadius = 5;
        private const int _maxPointToCoastlineDistance = 5;

        private static bool _editingRegion;
        private static MapRegionPoint? _newRegionPoint;
        private static int _previousRegionPointIndex = -1;
        private static int _nextRegionPointIndex = -1;

        private static RegionUIMediator? _regionUIMediator;

        internal static RegionUIMediator? RegionUIMediator
        {
            get { return _regionUIMediator; }
            set { _regionUIMediator = value; }
        }

        public static bool EditingRegion
        {
            get { return _editingRegion; }
            set { _editingRegion = value; }
        }

        public static MapRegionPoint? NewRegionPoint
        {
            get { return _newRegionPoint; }
            set { _newRegionPoint = value; }
        }

        public static int PreviousRegionPointIndex
        {
            get { return _previousRegionPointIndex; }
            set { _previousRegionPointIndex = value; }
        }

        public static int NextRegionPointIndex
        {
            get { return _nextRegionPointIndex; }
            set { _nextRegionPointIndex = value; }
        }

        public static int PointCircleRadius
        {
            get { return _pointCircleRadius; }
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? Create()
        {
            ArgumentNullException.ThrowIfNull(RegionUIMediator);

            MapStateMediator.CurrentMapRegion = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
            };

            Update();

            return MapStateMediator.CurrentMapRegion;
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(RegionUIMediator);

            MapRegion? mapRegion = MapStateMediator.CurrentMapRegion;
            if (mapRegion == null) return false;

            mapRegion.RegionBorderColor = RegionUIMediator.RegionColor;
            mapRegion.RegionBorderWidth = RegionUIMediator.RegionBorderWidth;

            mapRegion.RegionInnerOpacity = RegionUIMediator.RegionInnerOpacity;
            mapRegion.RegionBorderSmoothing = RegionUIMediator.RegionBorderSmoothing;
            mapRegion.RegionBorderType = RegionUIMediator.RegionBorderType;

            SKPathEffect? regionBorderEffect = ConstructRegionBorderEffect(mapRegion);
            ConstructRegionPaintObjects(mapRegion, regionBorderEffect);

            return true;
        }

        public static bool Delete()
        {
            if (MapStateMediator.CurrentMapRegion != null)
            {
                Cmd_DeleteMapRegion cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                MapStateMediator.CurrentMapRegion = null;

                return true;
            }

            return false;
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

        internal static void MoveSelectedRegionInRenderOrder(ComponentMoveDirection direction)
        {
            if (MapStateMediator.CurrentMapRegion != null)
            {
                // find the selected region in the Region Layer MapComponents
                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.REGIONLAYER);

                List<MapComponent> regionComponents = regionLayer.MapLayerComponents;

                int selectedRegionIndex = 0;

                for (int i = 0; i < regionComponents.Count; i++)
                {
                    MapComponent regionComponent = regionComponents[i];
                    if (regionComponent is MapRegion region && region.RegionGuid.ToString() == MapStateMediator.CurrentMapRegion.RegionGuid.ToString())
                    {
                        selectedRegionIndex = i;
                        break;
                    }
                }

                if (direction == ComponentMoveDirection.Up)
                {
                    // moving a region up in render order means increasing its index
                    if (MapStateMediator.CurrentMapRegion != null && selectedRegionIndex < regionComponents.Count - 1)
                    {
                        regionComponents[selectedRegionIndex] = regionComponents[selectedRegionIndex + 1];
                        regionComponents[selectedRegionIndex + 1] = MapStateMediator.CurrentMapRegion;
                    }
                }
                else if (direction == ComponentMoveDirection.Down)
                {
                    // moving a region down in render order means decreasing its index
                    if (selectedRegionIndex > 0)
                    {
                        regionComponents[selectedRegionIndex] = regionComponents[selectedRegionIndex - 1];
                        regionComponents[selectedRegionIndex - 1] = MapStateMediator.CurrentMapRegion;
                    }
                }
            }
        }

        internal static void FinalizeMapRegions()
        {
            // finalize loading of regions
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.REGIONLAYER);
            for (int i = 0; i < regionLayer.MapLayerComponents.Count; i++)
            {
                if (regionLayer.MapLayerComponents[i] is MapRegion region)
                {
                    region.ParentMap = MapStateMediator.CurrentMap;
                    SKPathEffect? regionBorderEffect = ConstructRegionBorderEffect(region);
                    ConstructRegionPaintObjects(region, regionBorderEffect);
                }
            }
        }

        internal static void SnapRegionToLandformCoastline()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMapRegion);

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

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);

            // get the distance from the point the cursor was clicked to the contour points of all landforms
            foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                for (int i = 0; i < lf.ContourPoints.Count; i++)
                {
                    SKPoint p = lf.ContourPoints[i];
                    float distance = SKPoint.Distance(MapStateMediator.CurrentCursorPoint, p);

                    if (distance < currentDistance && distance < _maxPointToCoastlineDistance)
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
                MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);

                if (MapStateMediator.CurrentMapRegion.MapRegionPoints.Count > 1 && coastlinePointIndex > 1)
                {
                    SKPoint previousPoint = MapStateMediator.CurrentMapRegion.MapRegionPoints[^2].RegionPoint;

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
                MapStateMediator.CurrentMapRegion.MapRegionPoints.Clear();

                if (MapStateMediator.CurrentCursorPoint.Y < MapStateMediator.PreviousCursorPoint.Y)
                {
                    // drag mouse up to snap to west coast of landform
                    for (int i = previousCoastlinePointIndex; i < landform1.ContourPoints.Count - 1; i++)
                    {
                        MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                        MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);
                    }

                    for (int i = 0; i <= coastlinePointIndex; i++)
                    {
                        MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                        MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);
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
                            MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);
                        }
                    }
                    else
                    {
                        for (int i = coastlinePointIndex; i <= previousCoastlinePointIndex; i++)
                        {
                            MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                            MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);
                        }
                    }
                }
            }
        }

        internal static void EndMapRegion()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMapRegion);

            MapRegionPoint mrp = new(MapStateMediator.CurrentCursorPoint);
            MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);

            Cmd_AddMapRegion cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            MapStateMediator.CurrentMapRegion.IsSelected = false;
        }

        internal static void DrawRegionOnWorkLayer()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMapRegion);

            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER);
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            if (MapStateMediator.CurrentMapRegion.MapRegionPoints.Count > 1)
            {
                // temporarily add the layer click point for rendering
                MapRegionPoint mrp = new(MapStateMediator.CurrentCursorPoint);
                MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);

                // render
                MapStateMediator.CurrentMapRegion.Render(workLayer.LayerSurface?.Canvas);

                // remove it
                MapStateMediator.CurrentMapRegion.MapRegionPoints.RemoveAt(MapStateMediator.CurrentMapRegion.MapRegionPoints.Count - 1);
            }
            else
            {
                workLayer.LayerSurface?.Canvas.DrawLine(MapStateMediator.PreviousCursorPoint, MapStateMediator.CurrentCursorPoint, MapStateMediator.CurrentMapRegion.RegionBorderPaint);
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
                    _editingRegion = true;
                }
                else
                {
                    p.IsSelected = false;
                }
            }

            return selectedMapRegionPoint;
        }

        internal static void DrawCoastlinePointOnWorkLayer2()
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

            float currentDistance = float.MaxValue;

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);
            MapLayer workLayer2 = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER2);
            workLayer2.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            // get the distance from the cursor point to the contour points of all landforms
            foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                for (int i = 0; i < lf.ContourPoints.Count; i++)
                {
                    SKPoint p = lf.ContourPoints[i];
                    float distance = SKPoint.Distance(MapStateMediator.CurrentCursorPoint, p);

                    if (distance < currentDistance && distance < _maxPointToCoastlineDistance)
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
                workLayer2.LayerSurface?.Canvas.DrawCircle(coastlinePoint, _pointCircleRadius, PaintObjects.MapPathSelectedControlPointPaint);
            }
        }

        internal static bool IsRegionPointSelected(MapRegion mapRegion, SKPoint zoomedScrolledPoint)
        {
            bool regionPointSelected = false;

            foreach (MapRegionPoint p in mapRegion.MapRegionPoints)
            {
                using SKPath path = new();
                path.AddCircle(p.RegionPoint.X, p.RegionPoint.Y, _pointCircleRadius);

                if (path.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                {
                    regionPointSelected = true;
                    p.IsSelected = true;
                }
                else
                {
                    if (!_editingRegion)
                    {
                        p.IsSelected = false;
                    }
                }
            }

            return regionPointSelected;
        }

        internal static void DrawRegionPointOnWorkLayer()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMapRegion);

            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

            _previousRegionPointIndex = -1;
            _nextRegionPointIndex = -1;
            _newRegionPoint = null;

            // is the cursor on a line segment between 2 region vertices? if so, draw a circle at the cursor location
            for (int i = 0; i < MapStateMediator.CurrentMapRegion.MapRegionPoints.Count - 1; i++)
            {
                if (DrawingMethods.LineContainsPoint(MapStateMediator.CurrentCursorPoint,
                    MapStateMediator.CurrentMapRegion.MapRegionPoints[i].RegionPoint, MapStateMediator.CurrentMapRegion.MapRegionPoints[i + 1].RegionPoint))
                {
                    _editingRegion = true;

                    _previousRegionPointIndex = i;
                    _nextRegionPointIndex = i + 1;

                    _newRegionPoint = new MapRegionPoint(MapStateMediator.CurrentCursorPoint);

                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(MapStateMediator.CurrentCursorPoint, _pointCircleRadius, PaintObjects.RegionNewPointFillPaint);
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(MapStateMediator.CurrentCursorPoint, _pointCircleRadius, PaintObjects.RegionPointOutlinePaint);

                    break;
                }
            }

            // is the cursor on the segment between the first and last region vertices?
            if (DrawingMethods.LineContainsPoint(MapStateMediator.CurrentCursorPoint, MapStateMediator.CurrentMapRegion.MapRegionPoints[0].RegionPoint,
                MapStateMediator.CurrentMapRegion.MapRegionPoints[^1].RegionPoint))
            {
                _editingRegion = true;

                _previousRegionPointIndex = 0;
                _nextRegionPointIndex = MapStateMediator.CurrentMapRegion.MapRegionPoints.Count;

                _newRegionPoint = new MapRegionPoint(MapStateMediator.CurrentCursorPoint);

                MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(MapStateMediator.CurrentCursorPoint, _pointCircleRadius, PaintObjects.RegionNewPointFillPaint);
                MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(MapStateMediator.CurrentCursorPoint, _pointCircleRadius, PaintObjects.RegionPointOutlinePaint);
            }
        }

        internal static bool DeleteSelectedRegionPoint(MapRegion currentMapRegion)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMapRegion);

            foreach (MapRegionPoint mrp in MapStateMediator.CurrentMapRegion.MapRegionPoints)
            {
                if (mrp.IsSelected)
                {
                    Cmd_DeleteMapRegionPoint cmd = new(MapStateMediator.CurrentMap, currentMapRegion, mrp);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    return true;
                }
            }

            return false;
        }
    }
}
