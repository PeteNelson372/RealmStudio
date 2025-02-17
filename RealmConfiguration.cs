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
using RealmStudio.Properties;

namespace RealmStudio
{
    public partial class RealmConfiguration : Form
    {
        public RealmStudioMap map = new();
        public float MapAspectRatio { get; set; } = 1.0F;
        public bool AspectRatioLocked = true;
        private bool WidthChanging;
        private bool HeightChanging;

        public RealmConfiguration()
        {
            InitializeComponent();

            Size defaultMapSize = Settings.Default.DefaultMapSize;

            SelectDefaultMapSizeRadioFromSize(defaultMapSize);

            HeightChanging = false;
            WidthUpDown.Value = defaultMapSize.Width;
            WidthUpDown.Refresh();

            WidthChanging = true;
            HeightUpDown.Value = defaultMapSize.Height;
            HeightUpDown.Refresh();

            map.MapWidth = (int)WidthUpDown.Value;
            map.MapHeight = (int)HeightUpDown.Value;

            CalculateAspectRatio();

            map.RealmType = RealmTypeEnum.World;


            string measurementUnits = Settings.Default.MeasurementUnits.Trim();

            if (!string.IsNullOrEmpty(measurementUnits))
            {
                if (measurementUnits == "US Customary")
                {
                    MapAreaUnitCombo.SelectedIndex = 6;  // miles
                    map.MapAreaUnits = "Miles";
                }
                else if (measurementUnits == "Metric")
                {
                    MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                    map.MapAreaUnits = "Kilometers";
                }
            }

            AssetManager.LoadThemes();

            MapThemeList.Items.Clear();

            List<MapTheme> themes = AssetManager.THEME_LIST;

            for (int i = 0; i < themes.Count; i++)
            {
                if (themes[i] != null && !string.IsNullOrEmpty(themes[i].ThemeName))
                {
                    MapThemeList.Items.Add(themes[i].ThemeName);

                    if (themes[i].IsDefaultTheme)
                    {
                        MapThemeList.SelectedIndex = i;
                        AssetManager.CURRENT_THEME = AssetManager.THEME_LIST[i];
                    }
                }
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

        private void WH1024x768Radio_Click(object sender, EventArgs e)
        {
            if (WH1024x768Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1024;
                HeightUpDown.Value = 768;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

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

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void CalculateAspectRatio()
        {
            MapAspectRatio = (float)(WidthUpDown.Value / HeightUpDown.Value);
            AspectRatioLabel.Text = MapAspectRatio.ToString("F2");

            MapAreaHeightLabel.Text = ((float)MapAreaWidthUpDown.Value / MapAspectRatio).ToString("F2");

            // MapAreaWidth and MapAreaHeight are the size of the map in MapAreaUnits (e.g. 1000 miles x 500 miles)
            map.MapAreaWidth = (float)MapAreaWidthUpDown.Value;
            map.MapAreaHeight = float.Parse(MapAreaHeightLabel.Text);

            // MapPixelWidth and MapPixelHeight are the size of one pixel in MapAreaUnits
            map.MapPixelWidth = map.MapAreaWidth / map.MapWidth;
            map.MapPixelHeight = map.MapAreaHeight / map.MapHeight;
        }

        private void MapAreaWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            CalculateAspectRatio();
        }

        private void WorldRadioButton_Click(object sender, EventArgs e)
        {
            if (WorldRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.World;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void RegionRadioButton_Click(object sender, EventArgs e)
        {
            if (RegionRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Region;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void CityRadioButton_Click(object sender, EventArgs e)
        {
            if (CityRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.City;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void InteriorRadioButton_Click(object sender, EventArgs e)
        {
            if (InteriorRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Interior;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        map.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        map.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void DungeonRadioButton_Click(object sender, EventArgs e)
        {
            if (DungeonRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Dungeon;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        map.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        map.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void SolarSystemRadioButton_Click(object sender, EventArgs e)
        {
            if (SolarSystemRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.SolarSystem;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void ShipRadioButton_Click(object sender, EventArgs e)
        {
            if (ShipRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Ship;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        map.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        map.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void OtherRadioButton_Click(object sender, EventArgs e)
        {
            if (OtherRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Other;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        map.MapAreaUnits = "Kilometers";
                    }
                }
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
                map.MapWidth = (int)WidthUpDown.Value;

                if (AspectRatioLocked)
                {
                    HeightUpDown.Value = (decimal)(map.MapWidth / MapAspectRatio);
                }

                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
                WidthChanging = false;
            }
        }

        private void HeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!WidthChanging)
            {
                HeightChanging = true;
                map.MapHeight = (int)HeightUpDown.Value;

                if (AspectRatioLocked)
                {
                    WidthUpDown.Value = (decimal)(map.MapHeight * MapAspectRatio);
                }

                map.MapWidth = (int)WidthUpDown.Value;

                CalculateAspectRatio();
                HeightChanging = false;
            }
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            map.MapName = RealmNameTextBox.Text;
            Close();
        }

        private void MapThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MapThemeList.SelectedIndex >= 0)
            {
                AssetManager.CURRENT_THEME = AssetManager.THEME_LIST[MapThemeList.SelectedIndex];
            }
        }
    }
}
