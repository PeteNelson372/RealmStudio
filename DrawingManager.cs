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
    internal class DrawingManager : IMapComponentManager
    {
        private static DrawingUIMediator? _drawingUIMediator;

        internal static DrawingUIMediator? DrawingMediator
        {
            get { return _drawingUIMediator; }
            set { _drawingUIMediator = value; }
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

        internal static void Paint(SKCanvas canvas, SKPoint currentCursorPoint, int brushSize)
        {
            ArgumentNullException.ThrowIfNull(DrawingMediator);

            MapBrush? brush = null;
            if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush1)
            {
                brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush1");
            }
            else if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush2)
            {
                brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush2");
            }
            else if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush3)
            {
                brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush3");
            }
            else if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush4)
            {
                brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush4"); ;
            }

            if (brush != null && brush.BrushBitmap == null)
            {
                brush.BrushBitmap = (Bitmap)Bitmap.FromFile(brush.BrushPath);
            }

            SKPaint ShaderPaint = new()
            {
                IsAntialias = true,
                Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                StrokeWidth = brushSize,
                Style = SKPaintStyle.Fill
            };

            SKShader StrokeShader = SKShader.CreateColor(DrawingMediator.DrawingLineColor.ToSKColor());
            SKBitmap? strokeBitmap = null;
            Bitmap resizedBitmap;

            if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush1
                        || DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush2
                        || DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush3
                        || DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush4)
            {
                if (brush == null || brush.BrushBitmap == null)
                {
                    return;
                }

                // scale and set opacity of the texture
                // resize the bitmap, but maintain aspect ratio
                resizedBitmap = DrawingMethods.ScaleBitmap(brush.BrushBitmap, brushSize, brushSize);
                strokeBitmap = resizedBitmap.ToSKBitmap();

                // combine the stroke color with the bitmap color
                ShaderPaint.ColorFilter = SKColorFilter.CreateBlendMode(DrawingMediator.DrawingLineColor.ToSKColor(), SKBlendMode.Modulate);

                ShaderPaint.Style = SKPaintStyle.Fill;
            }

            ShaderPaint.Shader = StrokeShader;

            if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.SoftBrush)
            {
                StrokeShader.Dispose();
                SKPoint gradientCenter = new(currentCursorPoint.X, currentCursorPoint.Y);
                StrokeShader = SKShader.CreateRadialGradient(gradientCenter, brushSize / 2, [DrawingMediator.DrawingLineColor.ToSKColor(), DrawingMediator.DrawingLineColor.ToSKColor().WithAlpha(0)], SKShaderTileMode.Clamp);
            }

            if (DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush1
                || DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush2
                || DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush3
                || DrawingMediator.DrawingPaintBrush == ColorPaintBrush.PatternBrush4)
            {
                if (strokeBitmap == null)
                {
                    return;
                }

                canvas.DrawBitmap(strokeBitmap, new SKRect(0, 0, strokeBitmap.Width, strokeBitmap.Height), new SKRect(currentCursorPoint.X - strokeBitmap.Width / 2, currentCursorPoint.Y - strokeBitmap.Height / 2, currentCursorPoint.X + strokeBitmap.Width / 2, currentCursorPoint.Y + strokeBitmap.Height / 2), ShaderPaint);
            }
            else
            {
                canvas.DrawCircle(currentCursorPoint.X, currentCursorPoint.Y, brushSize / 2, ShaderPaint);
            }
        }
    }
}
