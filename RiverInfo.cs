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
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class RiverInfo : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private readonly RealmStudioMap Map;
        private readonly River River;
        private readonly SKGLControl RenderControl;

        public RiverInfo(RealmStudioMap map, River river, SKGLControl renderControl)
        {
            InitializeComponent();

            Map = map;
            River = river;
            RenderControl = renderControl;

            GuidLabel.Text = River.MapRiverGuid.ToString();
            NameTextbox.Text = River.MapRiverName;

            WaterColorSelectionButton.BackColor = River.RiverColor;
            ShorelineColorSelectionButton.BackColor = River.RiverShorelineColor;

            RiverWidthTrack.Value = (int)River.RiverWidth;
            RiverSourceFadeInSwitch.Checked = River.RiverSourceFadeIn;
        }

        private void WaterColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, WaterColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                WaterColorSelectionButton.BackColor = selectedColor;
                WaterColorSelectionButton.Refresh();
            }
        }

        private void ShorelineColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, ShorelineColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                ShorelineColorSelectionButton.BackColor = selectedColor;
                ShorelineColorSelectionButton.Refresh();
            }
        }

        private void RiverWidthTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(RiverWidthTrack.Value.ToString(), this, new Point(RiverWidthTrack.Right, RiverWidthTrack.Top), 2000);
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            River.MapRiverName = NameTextbox.Text;

            River.RiverColor = WaterColorSelectionButton.BackColor;
            River.RiverShorelineColor = ShorelineColorSelectionButton.BackColor;

            River.RiverWidth = RiverWidthTrack.Value;
            River.RiverSourceFadeIn = RiverSourceFadeInSwitch.Checked;

            TOOLTIP.Show("River data changes applied", this, new Point(StatusMessageLabel.Left, StatusMessageLabel.Top), 3000);

            WaterFeatureManager.ConstructRiverPaintObjects(River);

            RenderControl.Invalidate();
        }

        private void CloseRiverFeatureDataButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RiverDescriptionButton_Click(object sender, EventArgs e)
        {
            DescriptionEditor descriptionEditor = new();
            descriptionEditor.DescrptionEditorOverlay.Text = "River Description Editor";
            descriptionEditor.DescriptionText = River.MapRiverDescription ?? string.Empty;
            DialogResult r = descriptionEditor.ShowDialog(this);

            if (r == DialogResult.OK)
            {
                River.MapRiverDescription = descriptionEditor.DescriptionText;
            }
        }

        private void RiverDescriptionButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Edit River Description", this, new Point(RiverDescriptionButton.Left, RiverDescriptionButton.Top - 20), 3000);
        }

        private void GenerateRiverNameButton_Click(object sender, EventArgs e)
        {
            string generatedName = MapToolMethods.GenerateRandomWaterFeatureName();
            NameTextbox.Text = generatedName;
        }

        private void GenerateRiverNameButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Generate River Name", this, new Point(GenerateRiverNameButton.Left, GenerateRiverNameButton.Top - 20), 3000);
        }
    }
}
