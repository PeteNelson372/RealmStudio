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
using RealmStudio.Properties;
using SkiaSharp;

namespace RealmStudio
{
    public partial class ResizeRealm : Form
    {
        internal RealmStudioMap? resizedMap;
        private readonly RealmStudioMainForm mainForm;
        private readonly RealmStudioMap currentMap;
        private SKRect selectedArea;
        private ResizeMapAnchorPoint anchorPoint = ResizeMapAnchorPoint.CenterZoomed;
        public bool AspectRatioLocked { get; set; } = true;
        public float MapAspectRatio { get; set; } = 1.0F;

        private bool WidthChanging;
        private bool HeightChanging;


        public ResizeRealm(RealmStudioMainForm mainForm, RealmStudioMap currentMap)
        {
            InitializeComponent();

            this.mainForm = mainForm;
            this.currentMap = currentMap;

            Size defaultMapSize = Settings.Default.DefaultMapSize;

            SelectDefaultMapSizeRadioFromSize(defaultMapSize);

            HeightChanging = false;
            WidthUpDown.Value = defaultMapSize.Width;
            WidthUpDown.Refresh();

            WidthChanging = true;
            HeightUpDown.Value = defaultMapSize.Height;
            HeightUpDown.Refresh();

            CalculateAspectRatio();

            ResizeAnchorLabel.Text = "Center Zoomed";
        }

        internal RealmStudioMap? GetResizedMap()
        {
            return resizedMap;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Resize the current map?", "Confirm Detail Map", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                int resizedMapWidth = Math.Min((int)(WidthUpDown.Value), 10000);
                int resizedMapHeight = Math.Min((int)(HeightUpDown.Value), 10000);

                CheckAspectRatioChange();

                // the selected area is the entire current map
                selectedArea.Top = 0;
                selectedArea.Left = 0;
                selectedArea.Right = currentMap.MapWidth;
                selectedArea.Bottom = currentMap.MapHeight;

                resizedMap = RealmMapMethods.CreateNewMapFromMap(currentMap,
                    currentMap.MapName,
                    selectedArea,
                    resizedMapWidth,
                    resizedMapHeight,
                    anchorPoint,
                    ScaleMapCheck.Checked,
                    mainForm.SKGLRenderControl.GRContext,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true);
            }
        }

        private void CancelCreateDetailButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void WH1024x768Radio_Click(object sender, EventArgs e)
        {
            if (WH1024x768Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1024;
                HeightUpDown.Value = 768;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH1280x720Radio_Click(object sender, EventArgs e)
        {
            if (WH1280x720Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1280;
                HeightUpDown.Value = 720;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH1280x1024Radio_Click(object sender, EventArgs e)
        {
            if (WH1280x1024Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1280;
                HeightUpDown.Value = 1024;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH1600x1200Radio_Click(object sender, EventArgs e)
        {
            if (WH1600x1200Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1600;
                HeightUpDown.Value = 1200;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH1920x1080Radio_Click(object sender, EventArgs e)
        {
            if (WH1920x1080Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1920;
                HeightUpDown.Value = 1080;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH2560x1080Radio_Click(object sender, EventArgs e)
        {
            if (WH2560x1080Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 2560;
                HeightUpDown.Value = 1080;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH2048x1024Radio_Click(object sender, EventArgs e)
        {
            if (WH2048x1024Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 2048;
                HeightUpDown.Value = 1024;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH3840x2160Radio_Click(object sender, EventArgs e)
        {
            if (WH3840x2160Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 3840;
                HeightUpDown.Value = 2160;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH4096x2048Radio_Click(object sender, EventArgs e)
        {
            if (WH4096x2048Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 4096;
                HeightUpDown.Value = 2048;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH3300x2250Radio_Click(object sender, EventArgs e)
        {
            if (WH3300x2250Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 3300;
                HeightUpDown.Value = 2250;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH1754x1240Radio_Click(object sender, EventArgs e)
        {
            if (WH1754x1240Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1754;
                HeightUpDown.Value = 1240;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH2480x1754Radio_Click(object sender, EventArgs e)
        {
            if (WH2480x1754Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 2840;
                HeightUpDown.Value = 1754;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH3508x2480Radio_Click(object sender, EventArgs e)
        {
            if (WH3508x2480Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 3508;
                HeightUpDown.Value = 2480;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH4960x3508Radio_Click(object sender, EventArgs e)
        {
            if (WH4960x3508Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 4960;
                HeightUpDown.Value = 3508;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH7016x4960Radio_Click(object sender, EventArgs e)
        {
            if (WH7016x4960Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 7016;
                HeightUpDown.Value = 4960;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WH7680x4320Radio_Click(object sender, EventArgs e)
        {
            if (WH7680x4320Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 7680;
                HeightUpDown.Value = 4320;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void CalculateAspectRatio()
        {
            MapAspectRatio = (float)(WidthUpDown.Value / HeightUpDown.Value);
            AspectRatioLabel.Text = MapAspectRatio.ToString("F2");
        }

        private void CheckAspectRatioChange()
        {
            string currentAspectRatioText = ((float)currentMap.MapWidth / (float)currentMap.MapHeight).ToString("F2");

            if (AspectRatioLabel.Text != currentAspectRatioText && ScaleMapCheck.Checked)
            {
                // aspect ratio is different from the current map
                MessageBox.Show("The chosen size for the resized map has a different\naspect ratio (width / height) than the current map.\nThis will cause objects to be stretched to fit the new map.", "Map Aspect Ratio Changed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void LockAspectRatioButton_Click(object sender, EventArgs e)
        {
            AspectRatioLocked = !AspectRatioLocked;

            if (AspectRatioLocked)
            {
                LockAspectRatioButton.IconChar = FontAwesome.Sharp.IconChar.Lock;
            }
            else
            {
                LockAspectRatioButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
            }
        }

        private void SwapResolutionButton_Click(object sender, EventArgs e)
        {
            (HeightUpDown.Value, WidthUpDown.Value) = (WidthUpDown.Value, HeightUpDown.Value);
        }

        private void WidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!HeightChanging)
            {
                WidthChanging = true;

                if (AspectRatioLocked)
                {
                    HeightUpDown.Value = (decimal)((float)WidthUpDown.Value / MapAspectRatio);
                }

                CalculateAspectRatio();

                WidthChanging = false;
            }
        }

        private void HeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!WidthChanging)
            {
                HeightChanging = true;

                if (AspectRatioLocked)
                {
                    WidthUpDown.Value = (decimal)((float)HeightUpDown.Value * MapAspectRatio);
                }

                CalculateAspectRatio();
                HeightChanging = false;
            }
        }

        private void SelectDefaultMapSizeRadioFromSize(Size defaultMapSize)
        {
            if (defaultMapSize.Width == 1024 && defaultMapSize.Height == 768)
            {
                WH1024x768Radio.Checked = true;
            }

            if (defaultMapSize.Width == 1280 && defaultMapSize.Height == 720)
            {
                WH1280x720Radio.Checked = true;
            }

            if (defaultMapSize.Width == 1280 && defaultMapSize.Height == 1024)
            {
                WH1280x1024Radio.Checked = true;
            }

            if (defaultMapSize.Width == 1600 && defaultMapSize.Height == 1200)
            {
                WH1600x1200Radio.Checked = true;
            }

            if (defaultMapSize.Width == 1920 && defaultMapSize.Height == 1080)
            {
                WH1920x1080Radio.Checked = true;
            }

            if (defaultMapSize.Width == 2560 && defaultMapSize.Height == 1080)
            {
                WH2560x1080Radio.Checked = true;
            }

            if (defaultMapSize.Width == 2048 && defaultMapSize.Height == 1024)
            {
                WH2048x1024Radio.Checked = true;
            }

            if (defaultMapSize.Width == 3840 && defaultMapSize.Height == 2160)
            {
                WH3840x2160Radio.Checked = true;
            }

            if (defaultMapSize.Width == 4096 && defaultMapSize.Height == 2048)
            {
                WH4096x2048Radio.Checked = true;
            }

            if (defaultMapSize.Width == 3300 && defaultMapSize.Height == 2250)
            {
                WH3300x2250Radio.Checked = true;
            }

            if (defaultMapSize.Width == 1754 && defaultMapSize.Height == 1240)
            {
                WH1754x1240Radio.Checked = true;
            }

            if (defaultMapSize.Width == 2480 && defaultMapSize.Height == 1754)
            {
                WH2480x1754Radio.Checked = true;
            }

            if (defaultMapSize.Width == 3508 && defaultMapSize.Height == 2480)
            {
                WH3508x2480Radio.Checked = true;
            }

            if (defaultMapSize.Width == 4960 && defaultMapSize.Height == 3508)
            {
                WH4960x3508Radio.Checked = true;
            }

            if (defaultMapSize.Width == 7016 && defaultMapSize.Height == 4960)
            {
                WH7016x4960Radio.Checked = true;
            }

            if (defaultMapSize.Width == 7680 && defaultMapSize.Height == 4320)
            {
                WH7680x4320Radio.Checked = true;
            }
        }

        private void TopLeftAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.TopLeft;
            ResizeAnchorLabel.Text = "Top Left";
        }

        private void TopCenterAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.TopCenter;
            ResizeAnchorLabel.Text = "Top Center";
        }

        private void TopRightAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.TopRight;
            ResizeAnchorLabel.Text = "Top Right";
        }

        private void CenterLeftAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.CenterLeft;
            ResizeAnchorLabel.Text = "Center Left";
        }

        private void CenterAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.Center;
            ResizeAnchorLabel.Text = "Center";

            if (ScaleMapCheck.Checked)
            {
                anchorPoint = ResizeMapAnchorPoint.CenterZoomed;
                ResizeAnchorLabel.Text = "Center Zoomed";
            }
        }

        private void CenterRightAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.CenterRight;
            ResizeAnchorLabel.Text = "Center Right";
        }

        private void BottomLeftAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.BottomLeft;
            ResizeAnchorLabel.Text = "Bottom Left";
        }

        private void BottomCenterAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.BottomCenter;
            ResizeAnchorLabel.Text = "Bottom Center";
        }

        private void BottomRightAnchorButton_Click(object sender, EventArgs e)
        {
            anchorPoint = ResizeMapAnchorPoint.BottomRight;
            ResizeAnchorLabel.Text = "Bottom Right";
        }
    }
}
