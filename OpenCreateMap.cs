/**************************************************************************************************************************
* Copyright 2026, Peter R. Nelson
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
using System.ComponentModel;
using System.IO;

namespace RealmStudio
{
    internal partial class OpenCreateMap : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float MapAspectRatio { get; set; } = 1.0F;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AspectRatioLocked { get; set; } = true;

        private bool WidthChanging;
        private bool HeightChanging;

        private readonly List<IRealmStudioMapGroup> CurrentMapGroups = [];

        private Task? _loadTask;

        private RealmStudioMapRoot? _mapRoot;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RealmStudioMapRoot? MapRoot
        {
            get { return _mapRoot; }
            set { _mapRoot = value; }
        }

        public OpenCreateMap()
        {
            InitializeComponent();

            Size defaultMapSize = Settings.Default.DefaultMapSize;
            WorldRadioButton.Checked = true;

            _mapRoot = new RealmStudioMapRoot();

            SelectDefaultMapSizeRadioFromSize(defaultMapSize);

            HeightChanging = false;
            WidthUpDown.Value = defaultMapSize.Width;
            WidthUpDown.Refresh();

            WidthChanging = true;
            HeightUpDown.Value = defaultMapSize.Height;
            HeightUpDown.Refresh();

            MapRoot?.MapWidth = (int)WidthUpDown.Value;
            MapRoot?.MapHeight = (int)HeightUpDown.Value;

            CalculateAspectRatio();

            MapRoot?.RealmType = RealmMapType.World;


            string measurementUnits = Settings.Default.MeasurementUnits.Trim();

            if (!string.IsNullOrEmpty(measurementUnits))
            {
                if (measurementUnits == "US Customary")
                {
                    MapAreaUnitCombo.SelectedIndex = 6;  // miles
                    MapRoot?.MapAreaUnits = "Miles";
                }
                else if (measurementUnits == "Metric")
                {
                    MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                    MapRoot?.MapAreaUnits = "Kilometers";
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

        private async Task LoadCreatedMapsFromDefaultDirectory()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (string.IsNullOrEmpty(Settings.Default.DefaultRealmDirectory))
            {
                Settings.Default.DefaultRealmDirectory = UtilityMethods.DEFAULT_REALM_FOLDER;
            }

            string defaultMapDirectory = Settings.Default.DefaultRealmDirectory;

            CurrentMapGroups.Clear();

            var files = from file in Directory.EnumerateFiles(defaultMapDirectory, "*.*", SearchOption.TopDirectoryOnly).Order()
                        where file.EndsWith(".rsmapx", StringComparison.InvariantCultureIgnoreCase)
                            || file.EndsWith(".rssetx", StringComparison.InvariantCultureIgnoreCase)
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                try
                {
                    if (f.File.EndsWith(".rsmapx", StringComparison.OrdinalIgnoreCase))
                    {
                        RealmStudioMapRoot? map = LoadRealmStudioMapRootFromFile(f.File);
                        if (map != null)
                        {
                            CurrentMapGroups.Add(map);
                        }
                    }
                    else if (f.File.EndsWith(".rssetx", StringComparison.OrdinalIgnoreCase))
                    {
                        // TODO: Load map sets
                        //List<RealmStudioMapRoot>? maps = MapFileMethods.OpenMapSet(f.File);
                    }

                }
                catch (Exception ex)
                {
                    // Ignore errors loading maps and map sets
                    Program.LOGGER.Error($"Error loading map or map set file: {f.File}", ex);
                }
            }



            Cursor.Current = Cursors.Default;
        }

        private static RealmStudioMapRoot? LoadRealmStudioMapRootFromFile(string file, bool showErrors = false)
        {
            try
            {
                RealmStudioMapRoot? map = MapFileMethods.OpenMapRoot(file);
                return map!;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error($"Error loading map file: {file}", ex);
                if (showErrors)
                {
                    MessageBox.Show($"Error loading map file: {file}. The map file may be corrupted.", "Error Loading Map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return null;
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

        private void CalculateAspectRatio()
        {
            MapAspectRatio = (float)(WidthUpDown.Value / HeightUpDown.Value);
            AspectRatioLabel.Text = MapAspectRatio.ToString("F2");

            MapAreaHeightLabel.Text = ((float)MapAreaWidthUpDown.Value / MapAspectRatio).ToString("F2");

            // MapAreaWidth and MapAreaHeight are the size of the map in MapAreaUnits (e.g. 1000 miles x 500 miles)
            MapRoot?.MapAreaWidth = (float)MapAreaWidthUpDown.Value;
            MapRoot?.MapAreaHeight = float.Parse(MapAreaHeightLabel.Text);

            // MapPixelWidth and MapPixelHeight are the size of one pixel in MapAreaUnits
            MapRoot?.MapPixelWidth = MapRoot.MapAreaWidth / MapRoot.MapWidth;
            MapRoot?.MapPixelHeight = MapRoot.MapAreaHeight / MapRoot.MapHeight;
        }

        private void MapAreaWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            CalculateAspectRatio();
        }

        private void WH1024x768Radio_Click(object sender, EventArgs e)
        {
            if (WH1024x768Radio.Checked)
            {
                bool aspectLocked = AspectRatioLocked;
                AspectRatioLocked = false;

                WidthUpDown.Value = 1024;
                HeightUpDown.Value = 768;

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

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

                MapRoot?.MapWidth = (int)WidthUpDown.Value;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();

                AspectRatioLocked = aspectLocked;
            }
        }

        private void WorldRadioButton_Click(object sender, EventArgs e)
        {
            if (WorldRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.World;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        MapRoot?.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        MapRoot?.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void RegionRadioButton_Click(object sender, EventArgs e)
        {
            if (RegionRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.Region;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        MapRoot?.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        MapRoot?.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void CityRadioButton_Click(object sender, EventArgs e)
        {
            if (CityRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.City;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        MapRoot?.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        MapRoot?.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void InteriorRadioButton_Click(object sender, EventArgs e)
        {
            if (InteriorRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.Interior;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        MapRoot?.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        MapRoot?.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void DungeonRadioButton_Click(object sender, EventArgs e)
        {
            if (DungeonRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.Dungeon;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        MapRoot?.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        MapRoot?.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void SolarSystemRadioButton_Click(object sender, EventArgs e)
        {
            if (SolarSystemRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.SolarSystem;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        MapRoot?.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        MapRoot?.MapAreaUnits = "Kilometers";
                    }
                }
            }
        }

        private void ShipRadioButton_Click(object sender, EventArgs e)
        {
            if (ShipRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.Ship;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        MapRoot?.MapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        MapRoot?.MapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void OtherRadioButton_Click(object sender, EventArgs e)
        {
            if (OtherRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.Other;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 6;  // miles
                        MapRoot?.MapAreaUnits = "Miles";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 5;  // Kilometers
                        MapRoot?.MapAreaUnits = "Kilometers";
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
                MapRoot?.MapWidth = (int)WidthUpDown.Value;

                if (AspectRatioLocked && MapRoot != null)
                {
                    HeightUpDown.Value = (decimal)(MapRoot.MapWidth / MapAspectRatio);
                }

                MapRoot?.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
                WidthChanging = false;
            }
        }

        private void HeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!WidthChanging)
            {
                HeightChanging = true;
                MapRoot?.MapHeight = (int)HeightUpDown.Value;

                if (AspectRatioLocked && MapRoot != null)
                {
                    WidthUpDown.Value = (decimal)(MapRoot.MapHeight * MapAspectRatio);
                }

                MapRoot?.MapWidth = (int)WidthUpDown.Value;

                CalculateAspectRatio();
                HeightChanging = false;
            }
        }

        private void MapThemeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MapThemeList.SelectedIndex >= 0)
            {
                AssetManager.CURRENT_THEME = AssetManager.THEME_LIST[MapThemeList.SelectedIndex];
            }
        }

        private void MapFileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentMapGroups[MapFileListBox.SelectedIndex] is RealmStudioMapRoot)
            {
                RealmStudioMapRoot selectedMap = (RealmStudioMapRoot)CurrentMapGroups[MapFileListBox.SelectedIndex];

                if (selectedMap != null)
                {
                    MapRoot = selectedMap;

                    List<string> mapData =
                    [
                        $"Name: {selectedMap.MapName}",
                        $"Type: {selectedMap.RealmType}",
                        $"Map Size: {selectedMap.MapWidth} x {selectedMap.MapHeight} pixels",
                        $"Path: {selectedMap.MapPath}",
                        $"File Size: {new FileInfo(selectedMap.MapPath).Length} bytes",
                        $"Created: {File.GetCreationTime(selectedMap.MapPath).ToString()}"
                    ];

                    MapInfoTextBox.Lines = [.. mapData];
                }
                else
                {
                    MapRoot = new RealmStudioMapRoot();
                    MapInfoTextBox.Lines = [];
                }
            }
            else if (CurrentMapGroups[MapFileListBox.SelectedIndex] is RealmStudioMapSet)
            {
                // TODO: Handle map sets
            }
        }

        public void LoadCreatedMaps()
        {
            _loadTask = Task.Run(() => LoadCreatedMapsFromDefaultDirectory());
        }   

        private void OpenCreateMap_Shown(object sender, EventArgs e)
        {
            _loadTask?.GetAwaiter().GetResult();

            MapFileListBox.Items.Clear();
            for (int i = 0; i < CurrentMapGroups.Count; i++)
            {
                if (CurrentMapGroups[i] is RealmStudioMapRoot)
                {
                    MapFileListBox.Items.Add(((RealmStudioMapRoot)CurrentMapGroups[i]).MapName);
                }
                else if (CurrentMapGroups[i] is RealmStudioMapSet)
                {
                    //MapFileListBox.Items.Add($"{((RealmStudioMapSet)CurrentMapGroups[i]).SetName} (Map Set)");
                }
            }
        }

        private void OpenMapIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Select Realm Studio File",
                    DefaultExt = "rsmapx",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false,
                    Filter =
                        "Realm Studio Map (*.rsmapx)|*.rsmapx|" +
                        "Realm Studio Set (*.rssetx)|*.rssetx|" +
                        "All Files (*.*)|*.*",

                    FilterIndex = 1
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(ofd.FileName))
                    {
                        if (ofd.FileName.EndsWith(".rsmapx", StringComparison.InvariantCultureIgnoreCase))
                        {
                            RealmStudioMapRoot? loadedMap = LoadRealmStudioMapRootFromFile(ofd.FileName, true);

                            if (loadedMap != null)
                            {
                                MapInfoTextBox.Lines = [];
                                List<string> mapData = new()
                            {
                                $"Name: {loadedMap.MapName}",
                                $"Type: {loadedMap.RealmType}",
                                $"Size: {loadedMap.MapWidth} x {loadedMap.MapHeight} pixels",
                                $"Path: {loadedMap.MapPath}",
                                $"Created: {File.GetCreationTime(loadedMap.MapPath)}"
                            };

                                MapInfoTextBox.Lines = [.. mapData];
                            }
                            else
                            {
                                MapInfoTextBox.Lines = [];
                            }
                        }
                        else if (ofd.FileName.EndsWith(".rssetx", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // TODO: Load map sets
                        }
                    }
                }
            }
            catch { }
        }

        private void ClearSelectedMapButton_Click(object sender, EventArgs e)
        {
            MapRoot = null;
            MapInfoTextBox.Lines = [];
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Open selected Realm Studio map?", "Open Map?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Close();
            }
        }

        private void CancelConfigButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to exit RealmStudio?", "Exit RealmStudio", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Close();
                Application.Exit();
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Create new Realm Studio map with selected parameters?", "Create Map?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                MapRoot = new()
                {
                    MapWidth = (int)WidthUpDown.Value,
                    MapHeight = (int)HeightUpDown.Value,
                    MapAreaWidth = (float)MapAreaWidthUpDown.Value,
                    MapAreaHeight = float.Parse(MapAreaHeightLabel.Text),
                    MapName = RealmNameTextBox.Text.Trim(),
                    MapPath = "",
                    MapTheme = MapThemeList.SelectedItem != null ? MapThemeList.SelectedItem.ToString() ?? "" : "",
                    RealmType = WorldRadioButton.Checked ? RealmMapType.World :
                                 RegionRadioButton.Checked ? RealmMapType.Region :
                                 CityRadioButton.Checked ? RealmMapType.City :
                                 InteriorRadioButton.Checked ? RealmMapType.Interior :
                                 DungeonRadioButton.Checked ? RealmMapType.Dungeon :
                                 SolarSystemRadioButton.Checked ? RealmMapType.SolarSystem :
                                 ShipRadioButton.Checked ? RealmMapType.Ship :
                                 OtherRadioButton.Checked ? RealmMapType.Other :
                                 RealmMapType.World
                };

                MapRoot.MapPixelWidth = MapRoot.MapAreaWidth / MapRoot.MapWidth;
                MapRoot.MapPixelHeight = MapRoot.MapAreaHeight / MapRoot.MapHeight;

                if (MapAreaUnitCombo.SelectedItem != null)
                {
                    MapRoot.MapAreaUnits = MapAreaUnitCombo.SelectedItem.ToString() ?? "Miles";
                }

                Close();
            }
        }
    }
}
