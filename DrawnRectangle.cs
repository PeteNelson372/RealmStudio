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
    internal class DrawnRectangle : DrawnMapComponent
    {
        private SKPoint _topLeft;
        private SKPoint _bottomRight;
        private SKColor _color = SKColors.Black;
        private int _brushSize = 2;
        private DrawingFillType _fillType = DrawingFillType.None;
        private SKShader? _shader;

        public SKPoint TopLeft
        {
            get => _topLeft;
            set => _topLeft = value;
        }
        public SKPoint BottomRight
        {
            get => _bottomRight;
            set => _bottomRight = value;
        }

        public SKColor Color
        {
            get => _color;
            set => _color = value;
        }
        public int BrushSize
        {
            get => _brushSize;
            set => _brushSize = value;
        }
        public DrawingFillType FillType
        {
            get => _fillType;
            set => _fillType = value;
        }

        public SKShader? Shader
        {
            get => _shader;
            set => _shader = value;
        }

        public override void Render(SKCanvas canvas)
        {
            using SKPaint paint = new()
            {
                Style = SKPaintStyle.Stroke,
                Color = Color,
                StrokeWidth = BrushSize,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Butt
            };

            using SKPaint fillPaint = new()
            {
                Style = SKPaintStyle.Fill,
                Color = Color,
                IsAntialias = true
            };

            if (FillType == DrawingFillType.Texture && Shader != null)
            {
                fillPaint.Shader = Shader;
                fillPaint.Style = SKPaintStyle.StrokeAndFill;
            }
            else if (FillType == DrawingFillType.Color)
            {
                fillPaint.Color = Color;
                fillPaint.Style = SKPaintStyle.StrokeAndFill;
            }
            else
            {
                fillPaint.Style = SKPaintStyle.Stroke;
            }

            SKRect rect = new(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);

            if (FillType != DrawingFillType.None)
            {
                // draw the filled rectangle first if the fill is enabled
                canvas.DrawRect(rect, fillPaint);
            }

            canvas.DrawRect(rect, paint);
        }
    }
}
