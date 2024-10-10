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
namespace RealmStudio
{
    public partial class ColorSelector : Form
    {
        private Color _color;

        public Color SelectedColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;

                ColorSelectorManager.Color = _color;

                ColorSelectorGrid.Color = _color;
                ColorSelectorWheel.Color = _color;
                ColorSelectorEditor.Color = _color;

                ColorPreviewLabel.BackColor = _color;
            }
        }

        public ColorSelector()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ColorSelector_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, SystemColors.Control, ButtonBorderStyle.Solid);
        }

        private void ColorSelectorManager_ColorChanged(object sender, EventArgs e)
        {
            SelectedColor = ColorSelectorManager.Color;
            ColorPreviewLabel.BackColor = SelectedColor;
        }

        private void ColorSelectorWheel_ColorChanged(object sender, EventArgs e)
        {
            ColorSelectorManager.Color = ColorSelectorWheel.Color;
        }

        private void ColorSelectorEditor_ColorChanged(object sender, EventArgs e)
        {
            ColorSelectorManager.Color = ColorSelectorEditor.Color;
        }

        private void ColorSelectorPicker_Selected(object sender, EventArgs e)
        {
            ColorSelectorManager.Color = ColorSelectorPicker.Color;
        }
    }
}
