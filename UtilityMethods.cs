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

namespace RealmStudio
{
    internal class UtilityMethods
    {
        public static Color SelectColorFromDialog(Form owner, Color initialColor)
        {
            // color selector
            using ColorSelector cs = new()
            {
                Owner = owner,
                SelectedColor = initialColor
            };

            if (cs.ShowDialog(owner) == DialogResult.OK)
            {
                return cs.SelectedColor;
            }
            else
            {
                return Color.Empty;
            }
        }

        public static SKFont CreateSkFontFromFont(Font font)
        {
            string familyName = font.FontFamily.Name;

            SKFontStyleWeight weight = SKFontStyleWeight.Normal;

            if (font.Bold)
            {
                weight = SKFontStyleWeight.Bold;
            }

            SKFontStyleWidth width = SKFontStyleWidth.Normal;

            if (font.Name.Contains("condensed", StringComparison.CurrentCultureIgnoreCase))
            {
                width = SKFontStyleWidth.Condensed;
            }
            else if (font.Name.ToLower().Contains("expanded"))
            {
                width = SKFontStyleWidth.Expanded;
            }

            SKFontStyleSlant slant = SKFontStyleSlant.Upright;

            if (font.Italic)
            {
                slant = SKFontStyleSlant.Italic;
            }

            SKFontStyle fs = new(weight, width, slant);


            SKTypeface tf = SKTypeface.FromFamilyName(familyName, fs);


            SKFont sKFont = new(tf, font.SizeInPoints * 1.33F);

            return sKFont;            
        }
    }
}
