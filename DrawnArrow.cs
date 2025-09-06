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
    internal sealed class DrawnArrow : DrawnMapComponent
    {
        private SKPoint _topLeft;
        private SKPoint _bottomRight;
        private SKColor _color = SKColors.Black;
        private int _brushSize = 2;
        private int _rotation;
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

        public int Rotation
        {
            get => _rotation;
            set => _rotation = value;
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
            SKRect rect = new(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
            Bounds = rect;

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

            // draw the arrow
            SKPoint p1 = new(TopLeft.X, TopLeft.Y + (rect.Height * 0.2F));

            SKPoint p2 = new(TopLeft.X + (rect.Width * 0.7F), TopLeft.Y + (rect.Height * 0.2F));

            SKPoint p3 = new(TopLeft.X + (rect.Width * 0.7F), TopLeft.Y);

            SKPoint p4 = new(BottomRight.X, (TopLeft.Y + BottomRight.Y) / 2);

            SKPoint p5 = new(TopLeft.X + (rect.Width * 0.7F), BottomRight.Y);

            SKPoint p6 = new(TopLeft.X + (rect.Width * 0.7F), TopLeft.Y + (rect.Height * 0.8F));

            SKPoint p7 = new(TopLeft.X, TopLeft.Y + (rect.Height * 0.8F));

            using SKPath path = new();
            path.MoveTo(p1);
            path.LineTo(p2);
            path.LineTo(p3);
            path.LineTo(p4);
            path.LineTo(p5);
            path.LineTo(p6);
            path.LineTo(p7);
            path.Close();

            using SKAutoCanvasRestore autoRestore = new(canvas, true);
            if (Rotation != 0)
            {
                canvas.RotateDegrees(Rotation, Bounds.MidX, Bounds.MidY);
            }

            if (FillType != DrawingFillType.None)
            {
                // draw the filled diamond first if the fill is enabled
                canvas.DrawPath(path, fillPaint);
            }

            canvas.DrawPath(path, paint);
            base.Render(canvas);
        }
    }
}
