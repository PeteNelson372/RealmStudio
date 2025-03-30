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

namespace RealmStudio
{
    internal sealed class Cmd_ChangeLabelAttributes : IMapOperation
    {
        private readonly RealmStudioMap Map;
        private readonly MapLabel Label;
        private readonly Color LabelColor;
        private readonly Color OutlineColor;
        private readonly float OutlineWidth;
        private readonly Color GlowColor;
        private readonly int GlowStrength;
        private readonly Font SelectedFont;

        private readonly Color StoredLabelColor;
        private readonly Color StoredOutlineColor;
        private readonly float StoredOutlineWidth;
        private readonly Color StoredGlowColor;
        private readonly int StoredGlowStrength;
        private readonly Font StoredSelectedFont;


        public Cmd_ChangeLabelAttributes(RealmStudioMap map, MapLabel label, Color labelColor, Color outlineColor, float outlineWidth, Color glowColor, int glowStrength, Font selectedFont)
        {
            Map = map;
            Label = label;
            LabelColor = labelColor;
            OutlineColor = outlineColor;
            OutlineWidth = outlineWidth;
            GlowColor = glowColor;
            GlowStrength = glowStrength;
            SelectedFont = selectedFont;

            StoredLabelColor = Label.LabelColor;
            StoredOutlineColor = Label.LabelOutlineColor;
            StoredOutlineWidth = Label.LabelOutlineWidth;
            StoredGlowColor = Label.LabelGlowColor;
            StoredGlowStrength = Label.LabelGlowStrength;
            StoredSelectedFont = Label.LabelFont;
        }

        public void DoOperation()
        {
            Label.LabelColor = LabelColor;
            Label.LabelOutlineColor = OutlineColor;
            Label.LabelOutlineWidth = OutlineWidth;
            Label.LabelGlowColor = GlowColor;
            Label.LabelGlowStrength = GlowStrength;
            Label.LabelFont = SelectedFont;

            SKFont skLabelFont = LabelManager.GetSkLabelFont(Label.LabelFont);
            SKPaint paint = LabelManager.CreateLabelPaint(Label.LabelColor);

            Label.LabelSKFont.Dispose();
            Label.LabelSKFont = skLabelFont;
            Label.LabelPaint = paint;

            Label.Width = (int)skLabelFont.MeasureText(Label.LabelText.AsSpan(), out SKRect bounds, paint);
            Label.Height = (int)bounds.Height;
        }

        public void UndoOperation()
        {
            Label.LabelColor = StoredLabelColor;
            Label.LabelOutlineColor = StoredOutlineColor;
            Label.LabelOutlineWidth = StoredOutlineWidth;
            Label.LabelGlowColor = StoredGlowColor;
            Label.LabelGlowStrength = StoredGlowStrength;
            Label.LabelFont = StoredSelectedFont;

            SKFont skLabelFont = LabelManager.GetSkLabelFont(Label.LabelFont);
            SKPaint paint = LabelManager.CreateLabelPaint(Label.LabelColor);

            Label.LabelSKFont.Dispose();
            Label.LabelSKFont = skLabelFont;
            Label.LabelPaint = paint;

            Label.Width = (int)skLabelFont.MeasureText(Label.LabelText.AsSpan(), out SKRect bounds, paint);
            Label.Height = (int)bounds.Height;
        }
    }
}
