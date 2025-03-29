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
    internal class MapMeasureManager : IMapComponentManager
    {
        private static MapMeasureUIMediator? _measureUIMediator;

        internal static MapMeasureUIMediator? MeasureUIMediator
        {
            get { return _measureUIMediator; }
            set { _measureUIMediator = value; }
        }

        public static IMapComponent? Create(RealmStudioMap? map, IUIMediatorObserver? mediator)
        {
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(MeasureUIMediator);

            // make sure there is only one measure object
            Delete(map, null);

            MapMeasure? newMapMeasure = new(map)
            {
                UseMapUnits = MeasureUIMediator.UseScaleUnits,
                MeasureArea = MeasureUIMediator.MeasureArea,
                MeasureLineColor = MeasureUIMediator.MapMeasureColor,
            };

            MapStateMediator.CurrentMapMeasure = newMapMeasure;
            MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER).MapLayerComponents.Add(newMapMeasure);

            return newMapMeasure;
        }

        public static bool Delete(RealmStudioMap? map, IMapComponent? component)
        {
            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();
            MapStateMediator.CurrentMapMeasure = null;
            return true;
        }

        public static IMapComponent? GetComponentById(RealmStudioMap? map, Guid componentGuid)
        {
            ArgumentNullException.ThrowIfNull(map);

            MapScale? component = null;

            foreach (MapScale ms in MapBuilder.GetMapLayerByIndex(map, MapBuilder.MEASURELAYER).MapLayerComponents.Cast<MapScale>())
            {
                if (ms.ScaleGuid.ToString() == componentGuid.ToString())
                {
                    component = ms;
                    break;
                }
            }

            return component;
        }

        public static bool Update(RealmStudioMap? map, MapStateMediator? MapStateMediator, IUIMediatorObserver? mediator)
        {
            ArgumentNullException.ThrowIfNull(MeasureUIMediator);

            if (MapStateMediator.CurrentMapMeasure != null)
            {
                MapStateMediator.CurrentMapMeasure.MeasureLineColor = MeasureUIMediator.MapMeasureColor;

                MapStateMediator.CurrentMapMeasure.MeasureLinePaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1,
                    Color = MapStateMediator.CurrentMapMeasure.MeasureLineColor.ToSKColor()
                };

                MapStateMediator.CurrentMapMeasure.MeasureAreaPaint = new()
                {
                    Style = SKPaintStyle.StrokeAndFill,
                    StrokeWidth = 1,
                    Color = MapStateMediator.CurrentMapMeasure.MeasureLineColor.ToSKColor()
                };
            }

            return true;
        }

        internal static void AddMeasurePoint(SKPoint zoomedScrolledPoint)
        {
            if (MapStateMediator.CurrentMapMeasure == null) return;

            if (!MapStateMediator.CurrentMapMeasure.MeasurePoints.Contains(zoomedScrolledPoint))
            {
                MapStateMediator.CurrentMapMeasure.MeasurePoints.Add(zoomedScrolledPoint);
            }
        }

        internal static void DrawMapMeasureOnWorkLayer(RealmStudioMap map, MapMeasure mapMeasure, SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            if (mapMeasure.MeasureArea && mapMeasure.MeasurePoints.Count > 1)
            {
                SKPath path = new();

                path.MoveTo(mapMeasure.MeasurePoints.First());

                for (int i = 1; i < mapMeasure.MeasurePoints.Count; i++)
                {
                    path.LineTo(mapMeasure.MeasurePoints[i]);
                }

                path.LineTo(zoomedScrolledPoint);

                path.Close();

                workLayer.LayerSurface?.Canvas.DrawPath(path, mapMeasure.MeasureAreaPaint);
            }
            else
            {
                workLayer.LayerSurface?.Canvas.DrawLine(previousPoint, zoomedScrolledPoint, mapMeasure.MeasureLinePaint);
            }

            // render measure value and units
            SKPoint measureValuePoint = new(zoomedScrolledPoint.X + 30, zoomedScrolledPoint.Y + 20);

            float lineLength = SKPoint.Distance(previousPoint, zoomedScrolledPoint);
            float totalLength = mapMeasure.TotalMeasureLength + lineLength;

            mapMeasure.RenderDistanceLabel(workLayer.LayerSurface?.Canvas, measureValuePoint, totalLength);

            if (mapMeasure.MeasureArea && mapMeasure.MeasurePoints.Count > 1)
            {
                // temporarity add the point at the mouse position
                mapMeasure.MeasurePoints.Add(zoomedScrolledPoint);

                // calculate the polygon area
                float area = DrawingMethods.CalculatePolygonArea(mapMeasure.MeasurePoints);

                // remove the temporarily added point
                mapMeasure.MeasurePoints.RemoveAt(mapMeasure.MeasurePoints.Count - 1);

                // display the area label
                SKPoint measureAreaPoint = new(zoomedScrolledPoint.X + 30, zoomedScrolledPoint.Y + 40);

                mapMeasure.RenderAreaLabel(workLayer.LayerSurface?.Canvas, measureAreaPoint, area);
            }
        }

        internal static void EndMapMeasure(MapMeasure mapMeasure, SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            mapMeasure.MeasurePoints.Add(zoomedScrolledPoint);

            float lineLength = SKPoint.Distance(previousPoint, zoomedScrolledPoint);
            mapMeasure.TotalMeasureLength += lineLength;
            mapMeasure.RenderValue = true;

            // reset everything
            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
            MapStateMediator.PreviousCursorPoint = SKPoint.Empty;

            MapStateMediator.CurrentMapMeasure = null;
        }
    }
}
