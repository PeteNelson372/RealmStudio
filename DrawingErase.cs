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
    internal class DrawingErase : DrawnMapComponent
    {
        private int _brushSize = 2;
        private List<SKPoint> _points = [];

        public List<SKPoint> Points
        {
            get => _points;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Points cannot be null.");
                }
                _points = value;
            }
        }

        public int BrushSize
        {
            get => _brushSize;
            set => _brushSize = value;
        }

        public override void Render(SKCanvas canvas)
        {
            using SKPaint paint = new()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Transparent,
                StrokeWidth = BrushSize / 2,
                IsAntialias = true,
                BlendMode = SKBlendMode.Clear
            };

            foreach (SKPoint erasePoint in Points)
            {
                canvas.DrawCircle(erasePoint, BrushSize / 2, paint);
            }
        }
    }
}
