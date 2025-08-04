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
    internal sealed class PaintedLine : DrawnMapComponent, IDisposable
    {
        private bool disposedValue;

        private List<SKPoint> _points = [];
        private ColorPaintBrush? _colorbrush;
        private MapBrush? _brush;
        private SKColor _color = SKColors.Black;
        private int _brushSize = 2;
        private Bitmap? _resizedBitmap;
        private SKBitmap? _strokeBitmap;
        private DrawingFillType _fillType = DrawingFillType.None;

        private readonly SKPaint ShaderPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        public List<SKPoint> Points
        {
            get => _points;
            set
            {
                _points = value ?? throw new ArgumentNullException(nameof(value), "Points cannot be null.");
            }
        }

        public MapBrush? Brush
        {
            get => _brush;
            set
            {
                _brush = value;
            }
        }

        public ColorPaintBrush? ColorBrush
        {
            get => _colorbrush;
            set
            {
                _colorbrush = value;

                if (_colorbrush == ColorPaintBrush.PatternBrush1)
                {
                    Brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush1");
                }
                else if (_colorbrush == ColorPaintBrush.PatternBrush2)
                {
                    Brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush2");
                }
                else if (_colorbrush == ColorPaintBrush.PatternBrush3)
                {
                    Brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush3");
                }
                else if (_colorbrush == ColorPaintBrush.PatternBrush4)
                {
                    Brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush4"); ;
                }

                if (Brush != null && Brush.BrushBitmap == null)
                {
                    Brush.BrushBitmap = (Bitmap)Bitmap.FromFile(Brush.BrushPath);
                }

                // scale and set opacity of the texture
                // resize the bitmap, but maintain aspect ratio
                if (Brush != null && Brush.BrushBitmap != null)
                {
                    _resizedBitmap = DrawingMethods.ScaleBitmap(Brush.BrushBitmap, _brushSize, _brushSize);
                    _strokeBitmap = _resizedBitmap.ToSKBitmap();
                }
            }
        }

        public SKColor Color
        {
            get => _color;
            set
            {
                if (value == SKColors.Empty)
                {
                    throw new ArgumentException("Color cannot be empty.", nameof(value));
                }
                _color = value;
                ShaderPaint.Color = _color;
            }
        }

        public int BrushSize
        {
            get => _brushSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Brush size must be greater than zero.");
                }

                _brushSize = value;

                // Resize the bitmap when brush size changes
                if (Brush != null && _resizedBitmap != null && Brush.BrushBitmap != null)
                {
                    _resizedBitmap = DrawingMethods.ScaleBitmap(Brush.BrushBitmap, _brushSize, _brushSize);
                    _strokeBitmap = _resizedBitmap.ToSKBitmap();
                }

                ShaderPaint.StrokeWidth = _brushSize;
            }
        }

        public DrawingFillType FillType
        {
            get => _fillType;
            set
            {
                _fillType = value;
                if (_fillType == DrawingFillType.Texture && Brush != null && _strokeBitmap != null)
                {
                    ShaderPaint.Shader = SKShader.CreateBitmap(_strokeBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                }
                else
                {
                    ShaderPaint.Shader = SKShader.CreateColor(Color);
                }
            }
        }

        public SKBitmap? StrokeBitmap
        {
            get => _strokeBitmap;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "StrokeBitmap cannot be null.");
                }
                _strokeBitmap = value;
            }
        }

        public override void Render(SKCanvas canvas)
        {
            ShaderPaint.Color = Color;
            ShaderPaint.StrokeWidth = BrushSize;

            SKShader StrokeShader = SKShader.CreateColor(Color);

            if (FillType == DrawingFillType.Texture)
            {
                if (Brush != null && Brush.BrushBitmap != null)
                {
                    // combine the stroke color with the bitmap color
                    ShaderPaint.ColorFilter = SKColorFilter.CreateBlendMode(Color, SKBlendMode.Modulate);
                }

                // if the fill type is texture, we need to create a shader from the bitmap
                StrokeShader = SKShader.CreateBitmap(_strokeBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
            }
            else if (FillType == DrawingFillType.Color)
            {
                if (ColorBrush == ColorPaintBrush.PatternBrush1
                    || ColorBrush == ColorPaintBrush.PatternBrush2
                    || ColorBrush == ColorPaintBrush.PatternBrush3
                    || ColorBrush == ColorPaintBrush.PatternBrush4)
                {
                    ShaderPaint.ColorFilter = SKColorFilter.CreateBlendMode(Color, SKBlendMode.Modulate);
                }
            }

            foreach (SKPoint point in Points)
            {
                if (FillType != DrawingFillType.Texture && ColorBrush == ColorPaintBrush.SoftBrush)
                {
                    StrokeShader.Dispose();
                    SKPoint gradientCenter = new(point.X, point.Y);
                    StrokeShader = SKShader.CreateRadialGradient(gradientCenter, BrushSize / 2, [Color, Color.WithAlpha(0)], SKShaderTileMode.Clamp);
                }

                ShaderPaint.Shader = StrokeShader;

                if (ColorBrush == ColorPaintBrush.PatternBrush1
                    || ColorBrush == ColorPaintBrush.PatternBrush2
                    || ColorBrush == ColorPaintBrush.PatternBrush3
                    || ColorBrush == ColorPaintBrush.PatternBrush4)
                {
                    if (_resizedBitmap == null)
                    {
                        return;
                    }

                    canvas.DrawBitmap(_resizedBitmap.ToSKBitmap(), new SKRect(0, 0,
                        _resizedBitmap.Width, _resizedBitmap.Height),
                        new SKRect(point.X - _resizedBitmap.Width / 2,
                            point.Y - _resizedBitmap.Height / 2,
                            point.X + _resizedBitmap.Width / 2,
                            point.Y + _resizedBitmap.Height / 2), ShaderPaint);
                }
                else
                {
                    canvas.DrawCircle(point.X, point.Y, BrushSize / 2, ShaderPaint);
                }
            }
        }

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ShaderPaint.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}