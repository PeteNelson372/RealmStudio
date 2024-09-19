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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapVignette : MapComponent
    {
        [XmlIgnore]
        public RealmStudioMap? Map { get; set; }

        [XmlElement]
        public int VignetteStrength { get; set; } = 64;

        [XmlElement]
        public XmlColor VignetteColor { get; set; } = ColorTranslator.FromHtml("#C9977B");

        public MapVignette(RealmStudioMap parentMap)
        {
            Map = parentMap;
        }

        public MapVignette() { }

        public override void Render(SKCanvas canvas)
        {
            if (Map != null)
            {
                SKRect bounds = new(0, 0, Map.MapWidth, Map.MapHeight);

                SKColor gradientColor = ((Color)VignetteColor).ToSKColor();

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
                canvas.DrawRect(rect, paint);

                paint.Shader = linGradTB;
                rect = new(0, 0, bounds.Width, tenthTopBottom);
                canvas.DrawRect(rect, paint);

                paint.Shader = linGradRL;
                rect = new(bounds.Width, 0, bounds.Width - tenthLeftRight, bounds.Height);
                canvas.DrawRect(rect, paint);

                paint.Shader = linGradBT;
                rect = new(0, bounds.Height - tenthTopBottom, bounds.Width, bounds.Height);
                canvas.DrawRect(rect, paint);
            }
        }
    }
}
