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
using System.Reflection;

namespace RealmStudio
{
    internal class MapLabelMethods
    {
        internal static SKPaint CreateLabelPaint(SKFont skLabelFont, Font labelFont, Color labelColor, LabelTextAlignEnum labelAlignment)
        {
            SKPaint paint = new()
            {
                Color = Extensions.ToSKColor(labelColor),
            };

            switch (labelAlignment)
            {
                case LabelTextAlignEnum.AlignLeft:
                    paint.TextAlign = SKTextAlign.Left;
                    break;
                case LabelTextAlignEnum.AlignCenter:
                    paint.TextAlign = SKTextAlign.Center;
                    break;
                case LabelTextAlignEnum.AlignRight:
                    paint.TextAlign = SKTextAlign.Right;
                    break;
            }

            paint.Typeface = skLabelFont.Typeface;

            paint.IsAntialias = true;

            return paint;
        }

        internal static SKFont GetSkLabelFont(Font labelFont)
        {
            List<string> resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();

            SKFontStyle fs = SKFontStyle.Normal;

            if (labelFont.Bold && labelFont.Italic)
            {
                fs = SKFontStyle.BoldItalic;
            }
            else if (labelFont.Bold)
            {
                fs = SKFontStyle.Bold;
            }
            else if (labelFont.Italic)
            {
                fs = SKFontStyle.Italic;
            }

            SKTypeface? fontTypeface = null;

            string fontName = StringExtensions.FindBestMatch(labelFont.FontFamily.Name, resourceNames);

            if (!string.IsNullOrEmpty(fontName))
            {
                fontTypeface = SKTypeface.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(fontName));
            }
            else
            {
                fontTypeface = SKTypeface.FromFamilyName(labelFont.FontFamily.Name, fs);

                if (fontTypeface == SKTypeface.Default)
                {
                    fontTypeface = SKTypeface.FromFamilyName(labelFont.FontFamily.Name);
                }
            }

            SKFont skLabelFont = new(fontTypeface)
            {
                Size = labelFont.Size * 1.33F
            };

            return skLabelFont;
        }
    }
}
