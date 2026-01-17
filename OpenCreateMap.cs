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

        // a map group can be either a map or a map set
        private readonly List<IRealmStudioMapGroup> CurrentMapGroups = [];

        private Task? _loadTask;

        private RealmStudioMapSet? _mapSet;

        private RealmStudioMapRoot? _mapRoot;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RealmStudioMapRoot? MapRoot
        {
            get { return _mapRoot; }
            set { _mapRoot = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RealmStudioMapSet? MapSet
        {
            get { return _mapSet; }
            set { _mapSet = value; }
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

            ImageList imageList = new()
            {
                ImageSize = new Size(16, 16) // Small icons
            };

            imageList.Images.Add("map", Properties.Resources.maps);
            imageList.Images.Add("mapset", Properties.Resources.atlas);

            MapFileListView.SmallImageList = imageList;
        }

        private async Task LoadCreatedMapsFromDefaultDirectory()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (string.IsNullOrEmpty(Settings.Default.DefaultRealmDirectory))
            {
                Settings.Default.DefaultRealmDirectory = UtilityMethods.DEFAULT_REALM_FOLDER;
                Settings.Default.Save();
            }

            string defaultMapDirectory = Settings.Default.DefaultRealmDirectory;

            CurrentMapGroups.Clear();

            var files = from file in Directory.EnumerateFiles(defaultMapDirectory, "*.*", SearchOption.TopDirectoryOnly).Order()
                        where file.EndsWith(UtilityMethods.REALM_STUDIO_MAP_FILE_EXTENSION, StringComparison.InvariantCultureIgnoreCase)
                            || file.EndsWith(UtilityMethods.REALM_STUDIO_MAPSET_FILE_EXTENSION, StringComparison.InvariantCultureIgnoreCase)
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                try
                {
                    if (f.File.EndsWith(UtilityMethods.REALM_STUDIO_MAP_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    {
                        RealmStudioMapRoot? map = LoadRealmStudioMapRootFromFile(f.File);
                        if (map != null)
                        {
                            CurrentMapGroups.Add(map);
                        }
                    }
                    else if (f.File.EndsWith(UtilityMethods.REALM_STUDIO_MAPSET_FILE_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Load map sets
                        RealmStudioMapSet? loadedMapSet = LoadRealmStudioMapSetFromFile(f.File, true);
                        if (loadedMapSet != null)
                        {
                            CurrentMapGroups.Add(loadedMapSet);
                        }
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

        private static RealmStudioMapSet? LoadRealmStudioMapSetFromFile(string file, bool showErrors = false)
        {
            try
            {
                RealmStudioMapSet? mapSet = MapFileMethods.OpenMapSet(file);
                return mapSet;
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error($"Error loading map set: {file}", ex);
                if (showErrors)
                {
                    MessageBox.Show($"Error loading map set: {file}. The map set file may be corrupted.", "Error Loading Map", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void LoadCreatedMaps()
        {
            _loadTask = Task.Run(() => LoadCreatedMapsFromDefaultDirectory());
        }

        private void OpenCreateMap_Shown(object sender, EventArgs e)
        {
            _loadTask?.GetAwaiter().GetResult();

            MapFileListView.Items.Clear();


            for (int i = 0; i < CurrentMapGroups.Count; i++)
            {
                if (CurrentMapGroups[i] is RealmStudioMapRoot maproot)
                {
                    ListViewItem lvItem = new($"{maproot.MapName}")
                    {
                        ImageKey = "map"
                    };

                    MapFileListView.Items.Add(lvItem);
                }
                else if (CurrentMapGroups[i] is RealmStudioMapSet mapset)
                {
                    ListViewItem lvItem = new($"{mapset.MapSetName}")
                    {
                        ImageKey = "mapset"
                    };

                    MapFileListView.Items.Add(lvItem);
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
                    DefaultExt = UtilityMethods.REALM_STUDIO_MAP_FILE_EXTENSION,
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
                        if (ofd.FileName.EndsWith(UtilityMethods.REALM_STUDIO_MAP_FILE_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
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
                        else if (ofd.FileName.EndsWith(UtilityMethods.REALM_STUDIO_MAPSET_FILE_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Load map sets
                            RealmStudioMapSet? loadedMapSet = LoadRealmStudioMapSetFromFile(ofd.FileName, true);

                            if (loadedMapSet != null)
                            {
                                MapInfoTextBox.Lines = [];
                                List<string> mapSetData = new()
                                {
                                    $"Name: {loadedMapSet.MapSetName}",
                                    $"Path: {loadedMapSet.MapSetPath}",
                                    $"Number of Maps: {loadedMapSet.SetMaps.Count}",
                                    $"Created: {File.GetCreationTime(loadedMapSet.MapSetPath)}"
                                };

                                string mapHeader = "Maps in Set:";
                                string mapData = "";
                                foreach (var mapRef in loadedMapSet.SetMaps)
                                {
                                    mapData += $"\n - Map Name: {mapRef.MapName}";
                                    mapData += $"\n   Type: {mapRef.RealmType}";
                                    mapData += $"\n   Path: {mapRef.MapPath}";
                                }

                                mapSetData.Add(mapHeader + mapData);

                                MapInfoTextBox.Lines = [.. mapSetData];
                            }
                            else
                            {
                                MapInfoTextBox.Lines = [];
                            }
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
            RealmMapType selectedRealmType = WorldRadioButton.Checked ? RealmMapType.World :
                                 RegionRadioButton.Checked ? RealmMapType.Region :
                                 CityRadioButton.Checked ? RealmMapType.City :
                                 InteriorRadioButton.Checked ? RealmMapType.Interior :
                                 InteriorFloorRadioButton.Checked ? RealmMapType.InteriorFloor :
                                 DungeonRadioButton.Checked ? RealmMapType.Dungeon :
                                 DungeonLevelRadioButton.Checked ? RealmMapType.DungeonLevel :
                                 SolarSystemRadioButton.Checked ? RealmMapType.SolarSystem :
                                 SolarSystemBodyRadioButton.Checked ? RealmMapType.SolarSystemBody :
                                 ShipRadioButton.Checked ? RealmMapType.Ship :
                                 ShipDeckRadioButton.Checked ? RealmMapType.ShipDeck :
                                 OtherRadioButton.Checked ? RealmMapType.Other :
                                 RealmMapType.Other;

            if (selectedRealmType == RealmMapType.World
                || selectedRealmType == RealmMapType.Region
                || selectedRealmType == RealmMapType.City
                || selectedRealmType == RealmMapType.InteriorFloor
                || selectedRealmType == RealmMapType.DungeonLevel
                || selectedRealmType == RealmMapType.ShipDeck
                || selectedRealmType == RealmMapType.SolarSystemBody
                || selectedRealmType == RealmMapType.Other)
            {
                if (DialogResult.Yes == MessageBox.Show("Create new " + selectedRealmType.GetDescription() + " map with selected parameters?", "Create " + selectedRealmType.GetDescription() + " Map?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MapSet = null;

                    MapRoot = new()
                    {
                        MapWidth = (int)WidthUpDown.Value,
                        MapHeight = (int)HeightUpDown.Value,
                        MapAreaWidth = (float)MapAreaWidthUpDown.Value,
                        MapAreaHeight = float.Parse(MapAreaHeightLabel.Text),
                        MapName = RealmNameTextBox.Text.Trim(),
                        MapPath = "",
                        MapTheme = MapThemeList.SelectedItem != null ? MapThemeList.SelectedItem.ToString() ?? "" : "",
                        RealmType = selectedRealmType
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
            else if (selectedRealmType == RealmMapType.Interior
                || selectedRealmType == RealmMapType.Dungeon
                || selectedRealmType == RealmMapType.SolarSystem
                || selectedRealmType == RealmMapType.Ship)
            {
                if (DialogResult.Yes == MessageBox.Show("Create new " + selectedRealmType.GetDescription() + " map set with selected parameters?", "Create " + selectedRealmType.GetDescription() + " Map Set?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MapRoot = null;

                    MapSet = new()
                    {
                        MapSetName = RealmNameTextBox.Text.Trim(),
                        MapSetType = selectedRealmType,
                        MapSetGuid = Guid.NewGuid(),
                        DefaultMapWidth = (int)WidthUpDown.Value,
                        DefaultMapHeight = (int)HeightUpDown.Value,
                        DefaultMapAreaWidth = (float)MapAreaWidthUpDown.Value,
                        DefaultMapAreaHeight = float.Parse(MapAreaHeightLabel.Text),
                        DefaultMapAreaUnits = MapAreaUnitCombo.SelectedItem != null ? MapAreaUnitCombo.SelectedItem.ToString() ?? "Miles" : "Miles",
                        DefaultThemeName = MapThemeList.SelectedItem as string ?? AssetManager.CURRENT_THEME?.ThemeName ?? string.Empty,
                        MapSetPath = "",
                        SetMaps = []
                    };

                    Close();
                }
            }
        }

        private void MapFileListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (MapFileListView.SelectedItems.Count == 0)
                {
                    return;
                }

                int selectedIndex = MapFileListView.SelectedIndices[0];

                if (CurrentMapGroups[selectedIndex] is RealmStudioMapRoot selectedMap)
                {
                    if (selectedMap != null)
                    {
                        MapRoot = selectedMap;
                        MapSet = null;

                        List<string> mapData =
                        [
                            $"Name: {selectedMap.MapName}",
                            $"Type: {selectedMap.RealmType}",
                            $"Map Size: {selectedMap.MapWidth} x {selectedMap.MapHeight} pixels",
                            $"Path: {selectedMap.MapPath}",
                            $"File Size: {new FileInfo(selectedMap.MapPath).Length} bytes",
                            $"Created: {File.GetCreationTime(selectedMap.MapPath)}"
                        ];

                        MapInfoTextBox.Lines = [.. mapData];
                    }
                    else
                    {
                        MapRoot = new RealmStudioMapRoot();
                        MapInfoTextBox.Lines = [];
                    }
                }
                else if (CurrentMapGroups[selectedIndex] is RealmStudioMapSet selectedMapSet)
                {
                    if (selectedMapSet != null)
                    {
                        MapRoot = null;
                        MapSet = selectedMapSet;

                        MapInfoTextBox.Lines = [];
                        List<string> mapSetData =
                                    [
                                        $"Name: {selectedMapSet.MapSetName}",
                                        $"Path: {selectedMapSet.MapSetPath}",
                                        $"Number of Maps: {selectedMapSet.SetMaps.Count}",
                                        $"Created: {File.GetCreationTime(selectedMapSet.MapSetPath)}",
                                        "Maps in Set:",
                                ];

                        foreach (var mapRef in selectedMapSet.SetMaps)
                        {
                            mapSetData.Add($" - Map Name: {mapRef.MapName}");
                            mapSetData.Add($"   Type: {mapRef.RealmType}");
                            mapSetData.Add($"   Path: {mapRef.MapPath}");
                        }

                        MapInfoTextBox.Lines = [.. mapSetData];
                    }
                    else
                    {
                        MapInfoTextBox.Lines = [];
                    }
                }
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error("Error selecting map from list", ex);
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

        private void WorldPictureBox_Click(object sender, EventArgs e)
        {
            WorldRadioButton.Checked = !WorldRadioButton.Checked;
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

        private void RegionPictureBox_Click(object sender, EventArgs e)
        {
            RegionRadioButton.Checked = !RegionRadioButton.Checked;
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

        private void CityPictureBox_Click(object sender, EventArgs e)
        {
            CityRadioButton.Checked = !CityRadioButton.Checked;
        }

        private void InteriorRadioButton_Click(object sender, EventArgs e)
        {
            if (InteriorRadioButton.Checked)
            {
                MapSet?.MapSetType = RealmMapType.Interior;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        MapSet?.DefaultMapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        MapSet?.DefaultMapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void InteriorPictureBox_Click(object sender, EventArgs e)
        {
            InteriorRadioButton.Checked = !InteriorRadioButton.Checked;
        }

        private void InteriorFloorRadioButton_Click(object sender, EventArgs e)
        {
            if (InteriorFloorRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.InteriorFloor;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        MapSet?.DefaultMapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        MapSet?.DefaultMapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void InteriorFloorPictureBox_Click(object sender, EventArgs e)
        {
            InteriorFloorRadioButton.Checked = !InteriorFloorRadioButton.Checked;
        }

        private void DungeonRadioButton_Click(object sender, EventArgs e)
        {
            if (DungeonRadioButton.Checked)
            {
                MapSet?.MapSetType = RealmMapType.Dungeon;

                string measurementUnits = Settings.Default.MeasurementUnits.Trim();
                if (!string.IsNullOrEmpty(measurementUnits))
                {
                    if (measurementUnits == "US Customary")
                    {
                        MapAreaUnitCombo.SelectedIndex = 2;  // feet
                        MapSet?.DefaultMapAreaUnits = "Feet";
                    }
                    else if (measurementUnits == "Metric")
                    {
                        MapAreaUnitCombo.SelectedIndex = 4;  // Meters
                        MapSet?.DefaultMapAreaUnits = "Meters";
                    }
                }
            }
        }

        private void DungeonPictureBox_Click(object sender, EventArgs e)
        {
            DungeonRadioButton.Checked = !DungeonRadioButton.Checked;
        }

        private void DungeonLevelRadioButton_Click(object sender, EventArgs e)
        {
            if (DungeonLevelRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.DungeonLevel;

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

        private void DungeonLevelPictureBox_Click(object sender, EventArgs e)
        {
            DungeonLevelRadioButton.Checked = !DungeonLevelRadioButton.Checked;
        }

        private void SolarSystemRadioButton_Click(object sender, EventArgs e)
        {
            if (SolarSystemRadioButton.Checked)
            {
                MapSet?.MapSetType = RealmMapType.SolarSystem;

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

        private void SolarSystemPictureBox_Click(object sender, EventArgs e)
        {
            SolarSystemRadioButton.Checked = !SolarSystemRadioButton.Checked;
        }

        private void SolarSystemBodyRadioButton_Click(object sender, EventArgs e)
        {
            if (SolarSystemBodyRadioButton.Checked)
            {
                MapRoot?.RealmType = RealmMapType.SolarSystemBody;

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

        private void SolarSystemBodyPictureBox_Click(object sender, EventArgs e)
        {
            SolarSystemBodyRadioButton.Checked = !SolarSystemBodyRadioButton.Checked;
        }

        private void ShipRadioButton_Click(object sender, EventArgs e)
        {
            if (ShipRadioButton.Checked)
            {
                MapSet?.MapSetType = RealmMapType.Ship;

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

        private void ShipPictureBox_Click(object sender, EventArgs e)
        {
            ShipRadioButton.Checked = !ShipRadioButton.Checked;
        }

        private void ShipDeckRadioButton_Click(object sender, EventArgs e)
        {
            if (ShipDeckRadioButton.Checked)
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

        private void ShipDeckPictureBox_Click(object sender, EventArgs e)
        {
            ShipDeckRadioButton.Checked = !ShipDeckRadioButton.Checked;
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

        private void OtherPictureBox_Click(object sender, EventArgs e)
        {
            OtherRadioButton.Checked = !OtherRadioButton.Checked;
        }
    }
}
