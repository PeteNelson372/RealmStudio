﻿/**************************************************************************************************************************
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
    public partial class WaterFeatureInfo : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private readonly RealmStudioMap Map;
        private readonly WaterFeature WaterFeature;
        private readonly SKGLControl RenderControl;

        public WaterFeatureInfo(RealmStudioMap map, WaterFeature waterFeature, SKGLControl renderControl)
        {
            InitializeComponent();

            Map = map;
            WaterFeature = waterFeature;
            RenderControl = renderControl;

            GuidLabel.Text = WaterFeature.WaterFeatureGuid.ToString();
            NameTextbox.Text = WaterFeature.WaterFeatureName;

            WaterColorSelectionButton.BackColor = WaterFeature.WaterFeatureColor;
            ShorelineColorSelectionButton.BackColor = WaterFeature.WaterFeatureShorelineColor;
        }

        private void CloseWaterFeatureDataButton_Click(object sender, EventArgs e)
        {
            Close();
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

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            WaterFeature.WaterFeatureName = NameTextbox.Text;

            WaterFeature.WaterFeatureColor = WaterColorSelectionButton.BackColor;
            WaterFeature.WaterFeatureShorelineColor = ShorelineColorSelectionButton.BackColor;

            TOOLTIP.Show("Water feature data changes applied", this, new Point(StatusMessageLabel.Left, StatusMessageLabel.Top), 3000);

            WaterFeatureMethods.ConstructWaterFeaturePaintObjects(WaterFeature);

            RenderControl.Invalidate();
        }
    }
}