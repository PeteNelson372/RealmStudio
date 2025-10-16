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
using System.Globalization;

namespace RealmStudio
{
    public partial class DetailMapForm : Form
    {
        private readonly RealmStudioMainForm mainForm;
        private readonly RealmStudioMap currentMap;
        private SKRect selectedArea;

        private RealmStudioMap? detailMap;


        public DetailMapForm(RealmStudioMainForm mainForm, RealmStudioMap currentMap, SKRect selectedArea)
        {
            InitializeComponent();

            this.mainForm = mainForm;
            this.currentMap = currentMap;
            this.selectedArea = selectedArea;

            MapTopUpDown.Value = (decimal)selectedArea.Top;
            MapLeftUpDown.Value = (decimal)selectedArea.Left;
            MapWidthUpDown.Value = (decimal)selectedArea.Width;
            MapHeightUpDown.Value = (decimal)selectedArea.Height;

            DetailMapWidthLabel.Text = (MapWidthUpDown.Value * MapZoomUpDown.Value).ToString(CultureInfo.InvariantCulture);
            DetailMapHeightLabel.Text = (MapHeightUpDown.Value * MapZoomUpDown.Value).ToString(CultureInfo.InvariantCulture);
        }

        private void CancelCreateDetailButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MapZoomTrack_Scroll(object sender, EventArgs e)
        {
            MapZoomUpDown.Value = (decimal)(MapZoomTrack.Value / 10.0);
        }

        private void MapZoomUpDown_ValueChanged(object sender, EventArgs e)
        {
            MapZoomTrack.Value = (int)(MapZoomUpDown.Value * 10);

            int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
            int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

            DetailMapWidthLabel.Text = detailMapWidth.ToString(CultureInfo.InvariantCulture);
            DetailMapHeightLabel.Text = detailMapHeight.ToString(CultureInfo.InvariantCulture);
        }

        private void MapTopUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void MapLeftUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void MapWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void MapHeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void ChangeDetailMapLocationAndSize()
        {
            selectedArea.Top = (float)MapTopUpDown.Value;
            selectedArea.Left = (float)MapLeftUpDown.Value;
            selectedArea.Right = (float)MapLeftUpDown.Value + (float)MapWidthUpDown.Value;
            selectedArea.Bottom = (float)MapTopUpDown.Value + (float)MapHeightUpDown.Value;

            int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
            int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

            DetailMapWidthLabel.Text = detailMapWidth.ToString();
            DetailMapHeightLabel.Text = detailMapHeight.ToString();

            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.WORKLAYER);
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            workLayer.LayerSurface?.Canvas.DrawRect(selectedArea, PaintObjects.LandformAreaSelectPaint);
            mainForm.SKGLRenderControl.Invalidate();
        }

        public RealmStudioMap? GetDetailMap()
        {
            return detailMap;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Create the detail map?", "Confirm Detail Map", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
                int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

                detailMap = RealmMapMethods.CreateNewMapFromMap(currentMap,
                    MapNameTextBox.Text,
                    selectedArea,
                    detailMapWidth,
                    detailMapHeight,
                    ResizeMapAnchorPoint.DetailMap,
                    true,
                    mainForm.SKGLRenderControl.GRContext,
                    IncludeTerrainCheck.Checked,
                    IncludeVegetationCheck.Checked,
                    IncludeStructuresCheck.Checked,
                    IncludeLabelsCheck.Checked,
                    false,
                    IncludePathsCheck.Checked,
                    false,
                    false,
                    false,
                    true,
                    true);
            }
        }
    }
}
