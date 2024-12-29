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
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class MapRegionInfo : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private readonly RealmStudioMap Map;
        private readonly MapRegion MapRegion;
        private readonly SKGLControl RenderControl;

        public MapRegionInfo(RealmStudioMap map, MapRegion mapRegion, SKGLControl renderControl)
        {
            Map = map;
            MapRegion = mapRegion;
            RenderControl = renderControl;

            InitializeComponent();

            RegionColorSelectButton.BackColor = MapRegion.RegionBorderColor;
            RegionBorderWidthTrack.Value = MapRegion.RegionBorderWidth;
            RegionBorderSmoothingTrack.Value = MapRegion.RegionBorderSmoothing;
            RegionOpacityTrack.Value = MapRegion.RegionInnerOpacity;

            SetSelectedRegionBorderType(MapRegion);
        }

        private void RegionColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, RegionColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                RegionColorSelectButton.BackColor = selectedColor;
            }
        }

        private void RegionBorderWidthTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(RegionBorderWidthTrack.Value.ToString(), this, new Point(RegionBorderWidthTrack.Right - 30, RegionBorderWidthTrack.Top - 20), 2000);
        }

        private void RegionBorderSmoothingTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(RegionBorderSmoothingTrack.Value.ToString(), this, new Point(RegionBorderSmoothingTrack.Right - 30, RegionBorderSmoothingTrack.Top - 20), 2000);
        }

        private void RegionOpacityTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(RegionOpacityTrack.Value.ToString(), this, new Point(RegionOpacityTrack.Right - 30, RegionOpacityTrack.Top - 20), 2000);
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            MapRegion.RegionBorderColor = RegionColorSelectButton.BackColor;
            MapRegion.RegionBorderWidth = RegionBorderWidthTrack.Value;
            MapRegion.RegionBorderSmoothing = RegionBorderSmoothingTrack.Value;
            MapRegion.RegionInnerOpacity = RegionOpacityTrack.Value;

            MapRegion.RegionBorderType = GetSelectedRegionBorderType();

            SKPathEffect? regionBorderEffect = MapRegionMethods.ConstructRegionBorderEffect(MapRegion);
            MapRegionMethods.ConstructRegionPaintObjects(MapRegion, regionBorderEffect);

            TOOLTIP.Show("Region data changes applied", this, new Point(StatusMessageLabel.Left, StatusMessageLabel.Top), 3000);

            RenderControl.Invalidate();
        }

        private void CloseLandformDataButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetSelectedRegionBorderType(MapRegion region)
        {
            if (region.RegionBorderType == PathTypeEnum.SolidLinePath) { RegionSolidBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.DottedLinePath) { RegionDottedBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.DashedLinePath) { RegionDashBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.DashDotLinePath) { RegionDashDotBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.DashDotDotLinePath) { RegionDashDotDotBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.DoubleSolidBorderPath) { RegionDoubleSolidBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.LineAndDashesPath) { RegionSolidAndDashesBorderRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.BorderedGradientPath) { RegionBorderedGradientRadio.Checked = true; return; }
            if (region.RegionBorderType == PathTypeEnum.BorderedLightSolidPath) { RegionBorderedLightSolidRadio.Checked = true; return; }
        }

        private PathTypeEnum GetSelectedRegionBorderType()
        {
            if (RegionSolidBorderRadio.Checked) return PathTypeEnum.SolidLinePath;
            if (RegionDottedBorderRadio.Checked) return PathTypeEnum.DottedLinePath;
            if (RegionDashBorderRadio.Checked) return PathTypeEnum.DashedLinePath;
            if (RegionDashDotBorderRadio.Checked) return PathTypeEnum.DashDotLinePath;
            if (RegionDashDotDotBorderRadio.Checked) return PathTypeEnum.DashDotDotLinePath;
            if (RegionSolidAndDashesBorderRadio.Checked) return PathTypeEnum.LineAndDashesPath;
            if (RegionBorderedGradientRadio.Checked) return PathTypeEnum.BorderedGradientPath;
            if (RegionBorderedLightSolidRadio.Checked) return PathTypeEnum.BorderedLightSolidPath;
            if (RegionDoubleSolidBorderRadio.Checked) return PathTypeEnum.DoubleSolidBorderPath;

            return PathTypeEnum.SolidLinePath;
        }

        private void SolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionSolidBorderRadio.Checked = !RegionSolidBorderRadio.Checked;
        }

        private void DottedRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDottedBorderRadio.Checked = !RegionDottedBorderRadio.Checked;
        }

        private void DashedRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashBorderRadio.Checked = !RegionDashBorderRadio.Checked;
        }

        private void DashDotRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashDotBorderRadio.Checked = !RegionDashDotBorderRadio.Checked;
        }

        private void DashDotDotRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashDotDotBorderRadio.Checked = !RegionDashDotDotBorderRadio.Checked;
        }

        private void DoubleSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDoubleSolidBorderRadio.Checked = !RegionDoubleSolidBorderRadio.Checked;
        }

        private void SolidAndDashRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionSolidAndDashesBorderRadio.Checked = !RegionSolidAndDashesBorderRadio.Checked;
        }

        private void GradientRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedGradientRadio.Checked = !RegionBorderedGradientRadio.Checked;
        }

        private void LightSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedLightSolidRadio.Checked = !RegionBorderedLightSolidRadio.Checked;
        }
    }
}