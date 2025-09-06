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
    internal sealed class DrawingManager : IMapComponentManager
    {
        private static DrawingUIMediator? _drawingUIMediator;
        private static PaintedLine? _currentPaintedLine;
        private static DrawnRectangle? _currentDrawnRectangle;
        private static DrawnEllipse? _currentDrawnEllipse;
        private static DrawingErase? _currentDrawingErase;
        private static DrawnPolygon? _currentDrawnPolygon;
        private static DrawnTriangle? _currentDrawnTriangle;
        private static DrawnRegularPolygon? _currentDrawnRegularPolygon;
        private static DrawnDiamond? _currentDrawnDiamond;
        private static DrawnArrow? _currentDrawnArrow;
        private static DrawnFivePointStar? _currentDrawnFivePointStar;
        private static DrawnSixPointStar? _currentDrawnSixPointStar;

        private static MapLayer? _drawingLayer;

        internal static DrawingUIMediator? DrawingMediator
        {
            get { return _drawingUIMediator; }
            set { _drawingUIMediator = value; }
        }

        internal static PaintedLine? CurrentPaintedLine
        {
            get { return _currentPaintedLine; }
            set { _currentPaintedLine = value; }
        }

        internal static DrawnRectangle? CurrentDrawnRectangle
        {
            get { return _currentDrawnRectangle; }
            set { _currentDrawnRectangle = value; }
        }

        internal static DrawnEllipse? CurrentDrawnEllipse
        {
            get { return _currentDrawnEllipse; }
            set { _currentDrawnEllipse = value; }
        }

        internal static DrawingErase? CurrentDrawingErase
        {
            get { return _currentDrawingErase; }
            set { _currentDrawingErase = value; }
        }

        internal static DrawnPolygon? CurrentDrawnPolygon
        {
            get { return _currentDrawnPolygon; }
            set { _currentDrawnPolygon = value; }
        }

        internal static DrawnTriangle? CurrentDrawnTriangle
        {
            get { return _currentDrawnTriangle; }
            set { _currentDrawnTriangle = value; }
        }

        internal static DrawnRegularPolygon? CurrentDrawnRegularPolygon
        {
            get { return _currentDrawnRegularPolygon; }
            set { _currentDrawnRegularPolygon = value; }
        }

        internal static DrawnDiamond? CurrentDrawnDiamond
        {
            get { return _currentDrawnDiamond; }
            set { _currentDrawnDiamond = value; }
        }

        internal static DrawnArrow? CurrentDrawnArrow
        {
            get { return _currentDrawnArrow; }
            set { _currentDrawnArrow = value; }
        }

        internal static DrawnFivePointStar? CurrentDrawnFivePointStar
        {
            get { return _currentDrawnFivePointStar; }
            set { _currentDrawnFivePointStar = value; }
        }

        internal static DrawnSixPointStar? CurrentDrawnSixPointStar
        {
            get { return _currentDrawnSixPointStar; }
            set { _currentDrawnSixPointStar = value; }
        }

        internal static MapLayer? DrawingLayer
        {
            get { return _drawingLayer; }
            set { _drawingLayer = value; }
        }

        public static IMapComponent? Create()
        {
            throw new NotImplementedException();
        }

        public static bool Delete()
        {
            DrawnMapComponent? dmc = FindSelectedDrawnMapComponent(MapStateMediator.CurrentMap);

            if (dmc != null)
            {
                MapLayer drawLayer = DrawingLayer ?? MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER);
                Cmd_DeleteDrawnMapComponent cmd = new(drawLayer, dmc);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }

            return true;
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(DrawingMediator);
            DrawnMapComponent? dmc = FindSelectedDrawnMapComponent(MapStateMediator.CurrentMap);

            if (dmc != null)
            {
                if (dmc is DrawnArrow arrow)
                {
                    arrow.Rotation = (int)DrawingMediator.DrawingShapeRotation;

                }
                else if (dmc is DrawnDiamond diamond)
                {
                    diamond.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnEllipse ellipse)
                {
                    ellipse.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnFivePointStar fivePointStar)
                {
                    fivePointStar.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnRegularPolygon poly)
                {
                    poly.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnPolygon polygon)
                {
                    polygon.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnRectangle rectangle)
                {
                    rectangle.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnSixPointStar sixPointStar)
                {
                    sixPointStar.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
                else if (dmc is DrawnTriangle triangle)
                {
                    triangle.Rotation = (int)DrawingMediator.DrawingShapeRotation;
                }
            }

            return true;
        }

        internal static void PlaceStampAtCursor(SKPoint currentCursorPoint)
        {
            ArgumentNullException.ThrowIfNull(DrawingMediator);

            if (DrawingMediator.DrawingStampBitmap != null &&
                DrawingMediator.DrawingStampBitmap.Width > 0 &&
                DrawingMediator.DrawingStampBitmap.Height > 0)
            {
                DrawnStamp drawnStamp = new()
                {
                    TopLeft = currentCursorPoint,
                    Opacity = DrawingMediator.DrawingStampOpacity,
                    Rotation = (int)DrawingMediator.DrawingStampRotation,
                    Scale = DrawingMediator.DrawingStampScale,
                    StampBitmap = DrawingMediator.DrawingStampBitmap.ToSKBitmap(),
                    IsSelected = false,
                };

                // use the selected map layer for the painted line
                MapLayer drawLayer = DrawingManager.DrawingLayer ?? MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER);

                Cmd_AddDrawnStamp cmd = new(drawLayer, drawnStamp);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static DrawnMapComponent? SelectDrawnMapComponentAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            DrawnMapComponent? selectedDrawnMapComponent = null;

            List<MapComponent>? drawnMapComponents = DrawingLayer?.MapLayerComponents;

            if (drawnMapComponents != null)
            {
                for (int i = 0; i < drawnMapComponents.Count; i++)
                {
                    if (drawnMapComponents[i] is DrawnMapComponent dmc)
                    {
                        SKRect bounds = dmc.Bounds;

                        if (bounds.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            selectedDrawnMapComponent = dmc;
                            selectedDrawnMapComponent.IsSelected = true;
                            break;
                        }
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedDrawnMapComponent);
            return selectedDrawnMapComponent;
        }

        internal static DrawnMapComponent? FindSelectedDrawnMapComponent(RealmStudioMap map)
        {
            DrawnMapComponent? selectedDrawnMapComponent = null;

            MapLayer drawLayer = DrawingLayer ?? MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER);

            List<MapComponent>? drawnMapComponents = drawLayer.MapLayerComponents;
            if (drawnMapComponents != null)
            {
                for (int i = 0; i < drawnMapComponents.Count; i++)
                {
                    if (drawnMapComponents[i] is DrawnMapComponent dmc && dmc.IsSelected)
                    {
                        selectedDrawnMapComponent = dmc;
                        break;
                    }
                }
            }
            return selectedDrawnMapComponent;
        }

        internal static void MoveDrawnComponent(DrawnMapComponent? dmc, float deltaX, float deltaY)
        {
            if (dmc != null)
            {
                if (dmc is DrawnArrow arrow)
                {
                    arrow.TopLeft = new SKPoint(Math.Max(0, Math.Min(arrow.TopLeft.X + deltaX, MapStateMediator.CurrentMap.MapWidth)), Math.Max(0, Math.Min(arrow.TopLeft.Y + deltaY, MapStateMediator.CurrentMap.MapHeight)));
                    arrow.BottomRight = new SKPoint(arrow.BottomRight.X + deltaX, arrow.BottomRight.Y + deltaY);
                }
                else if (dmc is DrawnDiamond diamond)
                {
                    diamond.TopLeft = new SKPoint(diamond.TopLeft.X + deltaX, diamond.TopLeft.Y + deltaY);
                    diamond.BottomRight = new SKPoint(diamond.BottomRight.X + deltaX, diamond.BottomRight.Y + deltaY);
                }
                else if (dmc is DrawnEllipse ellipse)
                {
                    ellipse.TopLeft = new SKPoint(ellipse.TopLeft.X + deltaX, ellipse.TopLeft.Y + deltaY);
                    ellipse.BottomRight = new SKPoint(ellipse.BottomRight.X + deltaX, ellipse.BottomRight.Y + deltaY);
                }
                else if (dmc is DrawnFivePointStar fivePointStar)
                {
                    fivePointStar.Center = new SKPoint(fivePointStar.Center.X + deltaX, fivePointStar.Center.Y + deltaY);
                }
                else if (dmc is DrawnRegularPolygon poly)
                {
                    poly.TopLeft = new SKPoint(poly.TopLeft.X + deltaX, poly.TopLeft.Y + deltaY);
                    poly.BottomRight = new SKPoint(poly.BottomRight.X + deltaX, poly.BottomRight.Y + deltaY);
                }
                else if (dmc is DrawnPolygon polygon)
                {
                    if (polygon.Points.Count > 0)
                    {
                        for (int i = 0; i < polygon.Points.Count; i++)
                        {
                            polygon.Points[i] = new SKPoint(polygon.Points[i].X + deltaX, polygon.Points[i].Y + deltaY);
                        }
                    }
                }
                else if (dmc is DrawnRectangle rectangle)
                {
                    rectangle.TopLeft = new SKPoint(rectangle.TopLeft.X + deltaX, rectangle.TopLeft.Y + deltaY);
                    rectangle.BottomRight = new SKPoint(rectangle.BottomRight.X + deltaX, rectangle.BottomRight.Y + deltaY);
                }
                else if (dmc is DrawnSixPointStar sixPointStar)
                {
                    sixPointStar.Center = new SKPoint(sixPointStar.Center.X + deltaX, sixPointStar.Center.Y + deltaY);
                }
                else if (dmc is DrawnStamp stamp)
                {
                    stamp.TopLeft = new SKPoint(stamp.TopLeft.X + deltaX, stamp.TopLeft.Y + deltaY);
                }
                else if (dmc is DrawnTriangle triangle)
                {
                    triangle.TopLeft = new SKPoint(triangle.TopLeft.X + deltaX, triangle.TopLeft.Y + deltaY);
                    triangle.BottomRight = new SKPoint(triangle.BottomRight.X + deltaX, triangle.BottomRight.Y + deltaY);
                }
            }
        }

        internal static void MoveDrawnComponent(DrawnMapComponent? dmc, ComponentMoveDirection direction)
        {
            if (dmc != null)
            {
                switch (direction)
                {
                    case ComponentMoveDirection.Up:
                        {
                            if (dmc is DrawnArrow arrow)
                            {
                                arrow.TopLeft = new SKPoint(arrow.TopLeft.X, Math.Max(arrow.TopLeft.Y - 1, 0));
                                arrow.BottomRight = new SKPoint(arrow.BottomRight.X, Math.Max(arrow.BottomRight.Y - 1, 0));
                            }
                            else if (dmc is DrawnDiamond diamond)
                            {
                                diamond.TopLeft = new SKPoint(diamond.TopLeft.X, Math.Max(diamond.TopLeft.Y - 1, 0));
                                diamond.BottomRight = new SKPoint(diamond.BottomRight.X, Math.Max(diamond.BottomRight.Y - 1, 0));
                            }
                            else if (dmc is DrawnEllipse ellipse)
                            {
                                ellipse.TopLeft = new SKPoint(ellipse.TopLeft.X, Math.Max(ellipse.TopLeft.Y - 1, 0));
                                ellipse.BottomRight = new SKPoint(ellipse.BottomRight.X, Math.Max(ellipse.BottomRight.Y - 1, 0));
                            }
                            else if (dmc is DrawnFivePointStar fivePointStar)
                            {
                                fivePointStar.Center = new SKPoint(fivePointStar.Center.X, Math.Max(fivePointStar.Center.Y - 1, 0));
                            }
                            else if (dmc is DrawnRegularPolygon poly)
                            {
                                poly.TopLeft = new SKPoint(poly.TopLeft.X, Math.Max(poly.TopLeft.Y - 1, 0));
                                poly.BottomRight = new SKPoint(poly.BottomRight.X, Math.Max(poly.BottomRight.Y - 1, 0));
                            }
                            else if (dmc is DrawnPolygon polygon)
                            {
                                for (int i = 0; i < polygon.Points.Count; i++)
                                {
                                    polygon.Points[i] = new SKPoint(polygon.Points[i].X, Math.Max(polygon.Points[i].Y - 1, 0));
                                }
                            }
                            else if (dmc is DrawnRectangle rectangle)
                            {
                                rectangle.TopLeft = new SKPoint(rectangle.TopLeft.X, Math.Max(rectangle.TopLeft.Y - 1, 0));
                                rectangle.BottomRight = new SKPoint(rectangle.BottomRight.X, Math.Max(rectangle.BottomRight.Y - 1, 0));
                            }
                            else if (dmc is DrawnSixPointStar sixPointStar)
                            {
                                sixPointStar.Center = new SKPoint(sixPointStar.Center.X, Math.Max(sixPointStar.Center.Y - 1, 0));
                            }
                            else if (dmc is DrawnStamp stamp)
                            {
                                stamp.TopLeft = new SKPoint(stamp.TopLeft.X, Math.Max(stamp.TopLeft.Y - 1, 0));
                            }
                            else if (dmc is DrawnTriangle triangle)
                            {
                                triangle.TopLeft = new SKPoint(triangle.TopLeft.X, Math.Max(triangle.TopLeft.Y - 1, 0));
                                triangle.BottomRight = new SKPoint(triangle.BottomRight.X, Math.Max(triangle.BottomRight.Y - 1, 0));
                            }
                        }
                        break;
                    case ComponentMoveDirection.Down:
                        {
                            if (dmc is DrawnArrow arrow)
                            {
                                arrow.TopLeft = new SKPoint(arrow.TopLeft.X, Math.Min(arrow.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                arrow.BottomRight = new SKPoint(arrow.BottomRight.X, Math.Min(arrow.BottomRight.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnDiamond diamond)
                            {
                                diamond.TopLeft = new SKPoint(diamond.TopLeft.X, Math.Min(diamond.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                diamond.BottomRight = new SKPoint(diamond.BottomRight.X, Math.Min(diamond.BottomRight.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnEllipse ellipse)
                            {
                                ellipse.TopLeft = new SKPoint(ellipse.TopLeft.X, Math.Min(ellipse.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                ellipse.BottomRight = new SKPoint(ellipse.BottomRight.X, Math.Min(ellipse.BottomRight.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnFivePointStar fivePointStar)
                            {
                                fivePointStar.Center = new SKPoint(fivePointStar.Center.X, Math.Min(fivePointStar.Center.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnRegularPolygon poly)
                            {
                                poly.TopLeft = new SKPoint(poly.TopLeft.X, Math.Min(poly.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                poly.BottomRight = new SKPoint(poly.BottomRight.X, Math.Min(poly.BottomRight.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnPolygon polygon)
                            {
                                for (int i = 0; i < polygon.Points.Count; i++)
                                {
                                    polygon.Points[i] = new SKPoint(polygon.Points[i].X, Math.Min(polygon.Points[i].Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                }
                            }
                            else if (dmc is DrawnRectangle rectangle)
                            {
                                rectangle.TopLeft = new SKPoint(rectangle.TopLeft.X, Math.Min(rectangle.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                rectangle.BottomRight = new SKPoint(rectangle.BottomRight.X, Math.Min(rectangle.BottomRight.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnSixPointStar sixPointStar)
                            {
                                sixPointStar.Center = new SKPoint(sixPointStar.Center.X, Math.Min(sixPointStar.Center.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnStamp stamp)
                            {
                                stamp.TopLeft = new SKPoint(stamp.TopLeft.X, Math.Min(stamp.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                            else if (dmc is DrawnTriangle triangle)
                            {
                                triangle.TopLeft = new SKPoint(triangle.TopLeft.X, Math.Min(triangle.TopLeft.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                                triangle.BottomRight = new SKPoint(triangle.BottomRight.X, Math.Min(triangle.BottomRight.Y + 1, MapStateMediator.CurrentMap.MapHeight));
                            }
                        }
                        break;
                    case ComponentMoveDirection.Right:
                        {
                            if (dmc is DrawnArrow arrow)
                            {
                                arrow.TopLeft = new SKPoint(Math.Min(arrow.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), arrow.TopLeft.Y);
                                arrow.BottomRight = new SKPoint(Math.Min(arrow.BottomRight.X + 1, MapStateMediator.CurrentMap.MapWidth), arrow.BottomRight.Y);
                            }
                            else if (dmc is DrawnDiamond diamond)
                            {
                                diamond.TopLeft = new SKPoint(Math.Min(diamond.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), diamond.TopLeft.Y);
                                diamond.BottomRight = new SKPoint(Math.Min(diamond.BottomRight.X + 1, MapStateMediator.CurrentMap.MapWidth), diamond.BottomRight.Y);
                            }
                            else if (dmc is DrawnEllipse ellipse)
                            {
                                ellipse.TopLeft = new SKPoint(Math.Min(ellipse.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), ellipse.TopLeft.Y);
                                ellipse.BottomRight = new SKPoint(Math.Min(ellipse.BottomRight.X + 1, MapStateMediator.CurrentMap.MapWidth), ellipse.BottomRight.Y);
                            }
                            else if (dmc is DrawnFivePointStar fivePointStar)
                            {
                                fivePointStar.Center = new SKPoint(Math.Min(fivePointStar.Center.X + 1, MapStateMediator.CurrentMap.MapWidth), fivePointStar.Center.Y);
                            }
                            else if (dmc is DrawnRegularPolygon poly)
                            {
                                poly.TopLeft = new SKPoint(Math.Min(poly.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), poly.TopLeft.Y);
                                poly.BottomRight = new SKPoint(Math.Min(poly.BottomRight.X + 1, MapStateMediator.CurrentMap.MapWidth), poly.BottomRight.Y);
                            }
                            else if (dmc is DrawnPolygon polygon)
                            {
                                for (int i = 0; i < polygon.Points.Count; i++)
                                {
                                    polygon.Points[i] = new SKPoint(Math.Min(polygon.Points[i].X + 1, MapStateMediator.CurrentMap.MapWidth), polygon.Points[i].Y);
                                }
                            }
                            else if (dmc is DrawnRectangle rectangle)
                            {
                                rectangle.TopLeft = new SKPoint(Math.Min(rectangle.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), rectangle.TopLeft.Y);
                                rectangle.BottomRight = new SKPoint(Math.Min(rectangle.BottomRight.X + 1, MapStateMediator.CurrentMap.MapWidth), rectangle.BottomRight.Y);
                            }
                            else if (dmc is DrawnSixPointStar sixPointStar)
                            {
                                sixPointStar.Center = new SKPoint(Math.Min(sixPointStar.Center.X + 1, MapStateMediator.CurrentMap.MapWidth), sixPointStar.Center.Y);
                            }
                            else if (dmc is DrawnStamp stamp)
                            {
                                stamp.TopLeft = new SKPoint(Math.Min(stamp.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), stamp.TopLeft.Y);
                            }
                            else if (dmc is DrawnTriangle triangle)
                            {
                                triangle.TopLeft = new SKPoint(Math.Min(triangle.TopLeft.X + 1, MapStateMediator.CurrentMap.MapWidth), triangle.TopLeft.Y);
                                triangle.BottomRight = new SKPoint(Math.Min(triangle.BottomRight.X + 1, MapStateMediator.CurrentMap.MapWidth), triangle.BottomRight.Y);
                            }
                        }
                        break;
                    case ComponentMoveDirection.Left:
                        {
                            if (dmc is DrawnArrow arrow)
                            {
                                arrow.TopLeft = new SKPoint(Math.Max(arrow.TopLeft.X - 1, 0), arrow.TopLeft.Y);
                                arrow.BottomRight = new SKPoint(Math.Max(arrow.BottomRight.X - 1, 0), arrow.BottomRight.Y);
                            }
                            else if (dmc is DrawnDiamond diamond)
                            {
                                diamond.TopLeft = new SKPoint(Math.Max(diamond.TopLeft.X - 1, 0), diamond.TopLeft.Y);
                                diamond.BottomRight = new SKPoint(Math.Max(diamond.BottomRight.X - 1, 0), diamond.BottomRight.Y);
                            }
                            else if (dmc is DrawnEllipse ellipse)
                            {
                                ellipse.TopLeft = new SKPoint(Math.Max(ellipse.TopLeft.X - 1, 0), ellipse.TopLeft.Y);
                                ellipse.BottomRight = new SKPoint(Math.Max(ellipse.BottomRight.X - 1, 0), ellipse.BottomRight.Y);
                            }
                            else if (dmc is DrawnFivePointStar fivePointStar)
                            {
                                fivePointStar.Center = new SKPoint(Math.Max(fivePointStar.Center.X - 1, 0), fivePointStar.Center.Y);
                            }
                            else if (dmc is DrawnRegularPolygon poly)
                            {
                                poly.TopLeft = new SKPoint(Math.Max(poly.TopLeft.X - 1, 0), poly.TopLeft.Y);
                                poly.BottomRight = new SKPoint(Math.Max(poly.BottomRight.X - 1, 0), poly.BottomRight.Y);
                            }
                            else if (dmc is DrawnPolygon polygon)
                            {
                                for (int i = 0; i < polygon.Points.Count; i++)
                                {
                                    polygon.Points[i] = new SKPoint(Math.Max(polygon.Points[i].X - 1, 0), polygon.Points[i].Y);
                                }
                            }
                            else if (dmc is DrawnRectangle rectangle)
                            {
                                rectangle.TopLeft = new SKPoint(Math.Max(rectangle.TopLeft.X - 1, 0), rectangle.TopLeft.Y);
                                rectangle.BottomRight = new SKPoint(Math.Max(rectangle.BottomRight.X - 1, 0), rectangle.BottomRight.Y);
                            }
                            else if (dmc is DrawnSixPointStar sixPointStar)
                            {
                                sixPointStar.Center = new SKPoint(Math.Max(sixPointStar.Center.X - 1, 0), sixPointStar.Center.Y);
                            }
                            else if (dmc is DrawnStamp stamp)
                            {
                                stamp.TopLeft = new SKPoint(Math.Max(stamp.TopLeft.X - 1, 0), stamp.TopLeft.Y);
                            }
                            else if (dmc is DrawnTriangle triangle)
                            {
                                triangle.TopLeft = new SKPoint(Math.Max(triangle.TopLeft.X - 1, 0), triangle.TopLeft.Y);
                                triangle.BottomRight = new SKPoint(Math.Max(triangle.BottomRight.X - 1, 0), triangle.BottomRight.Y);
                            }
                        }
                        break;
                }


            }
        }
    }
}
