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
        public RealmStudioMap Map { get; set; } = new();
        public float MapAspectRatio { get; set; } = 1.0F;
        public bool AspectRatioLocked { get; set; } = true;

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

            Map.MapWidth = (int)WidthUpDown.Value;
            Map.MapHeight = (int)HeightUpDown.Value;

            CalculateAspectRatio();

            Map.RealmType = RealmMapType.World;


            string measurementUnits = Settings.Default.MeasurementUnits.Trim();

            if (!string.IsNullOrEmpty(measurementUnits))
            {
                if (measurementUnits == "US Customary")
                {
                    MapAreaUnitCombo.SelectedIndex = 6;  // miles
                    Map.MapAreaUnits = "Miles";
                }
                else if (measurementUnits == "Metric")
                {
                    MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                    Map.MapAreaUnits = "Kilometers";
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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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

                Map.MapWidth = (int)WidthUpDown.Value;
                Map.MapHeight = (int)HeightUpDown.Value;

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
            Map.MapAreaWidth = (float)MapAreaWidthUpDown.Value;
            Map.MapAreaHeight = float.Parse(MapAreaHeightLabel.Text);

            // MapPixelWidth and MapPixelHeight are the size of one pixel in MapAreaUnits
            Map.MapPixelWidth = Map.MapAreaWidth / Map.MapWidth;
            Map.MapPixelHeight = Map.MapAreaHeight / Map.MapHeight;
        }

        private void MapAreaWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            CalculateAspectRatio();
        }

        private void WorldRadioButton_Click(object sender, EventArgs e)
        {
            if (WorldRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.World;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        Map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        Map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void RegionRadioButton_Click(object sender, EventArgs e)
        {
            if (RegionRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.Region;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        Map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        Map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void CityRadioButton_Click(object sender, EventArgs e)
        {
            if (CityRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.City;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        Map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        Map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void InteriorRadioButton_Click(object sender, EventArgs e)
        {
            if (InteriorRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.Interior;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        Map.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        Map.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void DungeonRadioButton_Click(object sender, EventArgs e)
        {
            if (DungeonRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.Dungeon;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        Map.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        Map.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void SolarSystemRadioButton_Click(object sender, EventArgs e)
        {
            if (SolarSystemRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.SolarSystem;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        Map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        Map.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void ShipRadioButton_Click(object sender, EventArgs e)
        {
            if (ShipRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.Ship;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        Map.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        Map.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void OtherRadioButton_Click(object sender, EventArgs e)
        {
            if (OtherRadioButton.Checked)
            {
                Map.RealmType = RealmMapType.Other;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        Map.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        Map.MapAreaUnits = "Kilometers";
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
                Map.MapWidth = (int)WidthUpDown.Value;

                if (AspectRatioLocked)
                {
                    HeightUpDown.Value = (decimal)(Map.MapWidth / MapAspectRatio);
                }

                Map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
                WidthChanging = false;
            }
        }

        private void HeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!WidthChanging)
            {
                HeightChanging = true;
                Map.MapHeight = (int)HeightUpDown.Value;

                if (AspectRatioLocked)
                {
                    WidthUpDown.Value = (decimal)(Map.MapHeight * MapAspectRatio);
                }

                Map.MapWidth = (int)WidthUpDown.Value;

                CalculateAspectRatio();
                HeightChanging = false;
            }
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            Map.MapName = RealmNameTextBox.Text;

            if (MapAreaUnitCombo.SelectedIndex >= 0)
            {
                string? areaUnits = (string?)MapAreaUnitCombo.Items[MapAreaUnitCombo.SelectedIndex];
                if (!string.IsNullOrEmpty(areaUnits))
                {
                    Map.MapAreaUnits = areaUnits;
                }
            }

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
