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
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapVignette : MapComponent
    {
        [XmlIgnore]
        public RealmStudioMap ParentMap { get; set; } = new();

        [XmlElement]
        public int VignetteStrength { get; set; } = 148;

        [XmlElement]
        public int VignetteColor { get; set; } = ColorTranslator.FromHtml("#C9977B").ToArgb();

        [XmlElement]
        public bool RectangleVignette { get; set; }

        [XmlIgnore]
        public bool IsModified { get; set; } = true;

        [XmlIgnore]
        public SKSurface? VignetteRenderSurface { get; set; }


        public MapVignette() { }

        public override void Render(SKCanvas canvas)
        {
            if (VignetteRenderSurface == null)
            {
                return;
            }

            if (!IsModified)
            {
                canvas.DrawSurface(VignetteRenderSurface, new SKPoint(0, 0));
            }
            else
            {
                if (RectangleVignette)
                {
                    RenderRectangleVignette(canvas);
                }
                else
                {
                    RenderOvalVignette(canvas);
                }
            }
        }

        public void RenderOvalVignette(SKCanvas canvas)
        {
            if (VignetteRenderSurface == null)
            {
                return;
            }

            SKCanvas vignetteCanvas = VignetteRenderSurface.Canvas;
            vignetteCanvas.Clear(SKColors.Transparent);

            // TODO: translate GDI code to Skia
            SKRect mapBounds = new(0, 0, ParentMap.MapWidth, ParentMap.MapHeight);
            SKRect ellipsebounds = new(0, 0, ParentMap.MapWidth, ParentMap.MapHeight);
            ellipsebounds.Offset(-ellipsebounds.Left, -ellipsebounds.Top);

            float x = ellipsebounds.Width - (int)Math.Round(.70712 * ellipsebounds.Width);
            float y = ellipsebounds.Height - (int)Math.Round(.70712 * ellipsebounds.Height);
            ellipsebounds.Inflate(x, y);

            Bitmap b = new(ParentMap.MapWidth, ParentMap.MapHeight);
            Graphics g = Graphics.FromImage(b);

            using GraphicsPath path = new();

            path.AddEllipse(ellipsebounds.ToDrawingRect());

            using PathGradientBrush brush = new(path);

            SKColor vColor = Color.FromArgb(VignetteColor).ToSKColor().WithAlpha((byte)VignetteStrength);
            brush.WrapMode = WrapMode.Tile;
            brush.CenterColor = Color.FromArgb(0, 0, 0, 0);
            brush.SurroundColors = [vColor.ToDrawingColor()];

            Blend blend = new()
            {
                Positions = [0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0F],
                Factors = [0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f]
            };

            brush.Blend = blend;

            Region oldClip = g.Clip;
            g.Clip = new Region(mapBounds.ToDrawingRect());

            g.FillRectangle(brush, ellipsebounds.ToDrawingRect());

            g.Clip = oldClip;
            g.Dispose();

            vignetteCanvas.DrawBitmap(b.ToSKBitmap(), new SKPoint(0, 0));

            canvas.DrawSurface(VignetteRenderSurface, new SKPoint(0, 0));

            IsModified = false;
        }

        public void RenderRectangleVignette(SKCanvas canvas)
        {
            if (VignetteRenderSurface == null)
            {
                return;
            }

            SKCanvas vignetteCanvas = VignetteRenderSurface.Canvas;
            vignetteCanvas.Clear(SKColors.Transparent);

            SKRect bounds = new(0, 0, ParentMap.MapWidth, ParentMap.MapHeight);

            SKColor gradientColor = Color.FromArgb(VignetteColor).ToSKColor();

            int tenthLeftRight = (int)(bounds.Width / 5);
            int tenthTopBottom = (int)(bounds.Height / 5);

            using SKShader linGradLR = SKShader.CreateLinearGradient(new SKPoint(0, bounds.Height / 2), new SKPoint(tenthLeftRight / 2, bounds.Height / 2), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);
            using SKShader linGradTB = SKShader.CreateLinearGradient(new SKPoint(bounds.Width / 2, 0), new SKPoint(bounds.Width / 2, tenthTopBottom), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);
            using SKShader linGradRL = SKShader.CreateLinearGradient(new SKPoint(bounds.Width, bounds.Height / 2), new SKPoint(bounds.Width - tenthLeftRight, bounds.Height / 2), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);
            using SKShader linGradBT = SKShader.CreateLinearGradient(new SKPoint(bounds.Width / 2, bounds.Height), new SKPoint(bounds.Width / 2, bounds.Height - tenthTopBottom), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);

            using SKPaint paint = new()
            {
                Shader = linGradLR,
                IsAntialias = true,
                Color = gradientColor,
            };

            SKRect rect = new(0, 0, tenthLeftRight, bounds.Height);
            vignetteCanvas.DrawRect(rect, paint);

            paint.Shader = linGradTB;
            rect = new(0, 0, bounds.Width, tenthTopBottom);
            vignetteCanvas.DrawRect(rect, paint);

            paint.Shader = linGradRL;
            rect = new(bounds.Width, 0, bounds.Width - tenthLeftRight, bounds.Height);
            vignetteCanvas.DrawRect(rect, paint);

            paint.Shader = linGradBT;
            rect = new(0, bounds.Height - tenthTopBottom, bounds.Width, bounds.Height);
            vignetteCanvas.DrawRect(rect, paint);

            canvas.DrawSurface(VignetteRenderSurface, new SKPoint(0, 0));

            IsModified = false;
        }
    }
}
