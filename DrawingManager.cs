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

        public static IMapComponent? Create()
        {
            throw new NotImplementedException();
        }

        public static bool Delete()
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            return true;
        }

        internal static void PlaceStampAtCursor(SKCanvas canvas, SKPoint currentCursorPoint)
        {
            ArgumentNullException.ThrowIfNull(DrawingMediator);

            if (DrawingMediator.DrawingStampBitmap != null &&
                DrawingMediator.DrawingStampBitmap.Width > 0 &&
                DrawingMediator.DrawingStampBitmap.Height > 0)
            {
                Bitmap stampBitmap = DrawingMethods.SetBitmapOpacity(DrawingMediator.DrawingStampBitmap, DrawingMediator.DrawingStampOpacity);

                SKBitmap scaledStamp = DrawingMethods.ScaleSKBitmap(stampBitmap.ToSKBitmap(), DrawingMediator.DrawingStampScale);

                SKBitmap rotatedAndScaledStamp = DrawingMethods.RotateSKBitmap(scaledStamp, DrawingMediator.DrawingStampRotation, false);

                canvas.DrawBitmap(rotatedAndScaledStamp,
                    new SKPoint(currentCursorPoint.X - (rotatedAndScaledStamp.Width / 2), currentCursorPoint.Y - (rotatedAndScaledStamp.Height / 2)), null);

                stampBitmap.Dispose();
                scaledStamp.Dispose();
                rotatedAndScaledStamp.Dispose();
            }
        }
    }
}
