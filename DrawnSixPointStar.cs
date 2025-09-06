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
    internal sealed class DrawnSixPointStar : DrawnMapComponent
    {
        private SKPoint _center;
        private float _radius;
        private SKColor _color = SKColors.Black;
        private int _brushSize = 2;
        private int _rotation;
        private DrawingFillType _fillType = DrawingFillType.None;
        private SKShader? _shader;

        public SKPoint Center
        {
            get => _center;
            set => _center = value;
        }

        public float Radius
        {
            get => _radius;
            set => _radius = value;
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

            List<SKPoint> points = [];

            for (int degrees = 0; degrees < 360; degrees += 30)
            {
                float radians = (float)(degrees * Math.PI / 180.0);

                if (degrees % 60 != 0)
                {
                    // every 60 degrees, we draw a point at the outer radius
                    float x = Center.X + Radius * (float)Math.Cos(radians);
                    float y = Center.Y + Radius * (float)Math.Sin(radians);
                    points.Add(new SKPoint(x, y));
                }
                else
                {
                    // every 30 degrees, we draw a point at the inner radius
                    float innerRadius = Radius / 2.0F;
                    float x = Center.X + innerRadius * (float)Math.Cos(radians);
                    float y = Center.Y + innerRadius * (float)Math.Sin(radians);
                    points.Add(new SKPoint(x, y));
                }
            }

            using SKPath path = new();

            path.MoveTo(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i]);
            }

            path.Close();
            Bounds = path.Bounds;

            using SKAutoCanvasRestore autoRestore = new(canvas, true);
            if (Rotation != 0)
            {
                canvas.RotateDegrees(Rotation, Bounds.MidX, Bounds.MidY);
            }

            base.Render(canvas);

            if (FillType != DrawingFillType.None)
            {
                // draw the filled star first if the fill is enabled
                canvas.DrawPath(path, fillPaint);
            }

            canvas.DrawPath(path, paint);
        }
    }
}
