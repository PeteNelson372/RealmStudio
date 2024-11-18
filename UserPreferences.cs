using RealmStudio.Properties;

namespace RealmStudio
{
    public partial class UserPreferences : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        public UserPreferences()
        {
            InitializeComponent();

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            string measurementUnitsSetting = Settings.Default.MeasurementUnits;

            if (!string.IsNullOrEmpty(measurementUnitsSetting))
            {
                measurementUnitsSetting = measurementUnitsSetting.Trim();
                MeasurementUnitsCombo.SelectedIndex = MeasurementUnitsCombo.FindString(measurementUnitsSetting);
            }

            AutosaveRealmSwitch.Checked = Settings.Default.RealmAutosave;

            // realm assets
            string assetDirectory = Settings.Default.MapAssetDirectory;

            if (string.IsNullOrEmpty(assetDirectory))
            {
                Settings.Default.MapAssetDirectory = UtilityMethods.DEFAULT_ASSETS_FOLDER;
            }

            RealmAssetDirectoryTextBox.Text = Settings.Default.MapAssetDirectory;

            // realm folder (default location where maps are saved)
            string realmDirectory = Settings.Default.DefaultRealmDirectory;

            if (string.IsNullOrEmpty(realmDirectory))
            {
                Settings.Default.DefaultRealmDirectory = UtilityMethods.DEFAULT_REALM_FOLDER;
            }

            DefaultRealmDirectoryTextbox.Text = Settings.Default.DefaultRealmDirectory;

            // realm autosave folder (location where map backups are saved during autosave)
            string autosaveDirectory = Settings.Default.AutosaveDirectory;

            if (string.IsNullOrEmpty(autosaveDirectory))
            {
                Settings.Default.AutosaveDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
            }

            AutosaveDirectoryTextBox.Text = Settings.Default.AutosaveDirectory;

            int autosaveInterval = Settings.Default.AutosaveInterval;
            AutosaveIntervalTrack.Value = autosaveInterval;

            PlaySoundOnSaveSwitch.Checked = Settings.Default.PlaySoundOnSave;

            string defaultExportFormat = Settings.Default.DefaultExportFormat;

            if (!string.IsNullOrEmpty(defaultExportFormat))
            {
                defaultExportFormat = defaultExportFormat.Trim();
                DefaultExportFormatCombo.SelectedIndex = DefaultExportFormatCombo.FindString(defaultExportFormat);
            }
            else
            {
                DefaultExportFormatCombo.SelectedIndex = DefaultExportFormatCombo.FindString("PNG");
            }

            DrawContoursWhilePaintingSwitch.Checked = Settings.Default.CalculateContoursWhilePainting;

            ClipColoringToLandformSwitch.Checked = Settings.Default.ClipLandformColoring;

            Size defaultMapSize = Settings.Default.DefaultMapSize;

            SelectDefaultMapSizeRadioFromSize(defaultMapSize);

            Settings.Default.Save();
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

        private void ClosePreferencesButton_Click(object sender, EventArgs e)
        {
            // the user may have typed directly into the text boxes, so
            // get the text and save the settings
            Settings.Default.DefaultRealmDirectory = DefaultRealmDirectoryTextbox.Text;
            Settings.Default.MapAssetDirectory = RealmAssetDirectoryTextBox.Text;
            Settings.Default.AutosaveDirectory = AutosaveDirectoryTextBox.Text;

            Settings.Default.Save();
            Close();
        }

        private void RealmDirectoryButton_Click(object sender, EventArgs e)
        {
            string realmDirectory = Settings.Default.DefaultRealmDirectory;

            if (string.IsNullOrEmpty(realmDirectory))
            {
                realmDirectory = UtilityMethods.DEFAULT_REALM_FOLDER;
            }

            FolderBrowserDialog fbd = new()
            {
                InitialDirectory = realmDirectory
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                realmDirectory = fbd.SelectedPath;
                DefaultRealmDirectoryTextbox.Text = realmDirectory;
                Settings.Default.DefaultRealmDirectory = DefaultRealmDirectoryTextbox.Text;
                Settings.Default.Save();
            }
        }

        private void AutosaveIntervalTrack_Scroll(object sender, EventArgs e)
        {
            double value = AutosaveIntervalTrack.Value;
            double indexDbl = value / AutosaveIntervalTrack.TickFrequency;
            int index = (int)(Math.Round(indexDbl));

            AutosaveIntervalTrack.Value = AutosaveIntervalTrack.TickFrequency * index;

            TOOLTIP.Show(AutosaveIntervalTrack.Value.ToString(), this, new Point(AutosaveIntervalTrack.Right - 30, AutosaveIntervalTrack.Top - 20), 2000);

            Settings.Default.AutosaveInterval = AutosaveIntervalTrack.Value;
        }

        private void MeasurementUnitsCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string measurementUnits = MeasurementUnitsCombo.Text;

            if (!string.IsNullOrEmpty(measurementUnits))
            {
                Settings.Default.MeasurementUnits = measurementUnits;
            }
        }

        private void RealmAssetDirButton_Click(object sender, EventArgs e)
        {
            string assetDirectory = Settings.Default.MapAssetDirectory;

            if (string.IsNullOrEmpty(assetDirectory))
            {
                assetDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
            }

            FolderBrowserDialog fbd = new()
            {
                InitialDirectory = assetDirectory
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                assetDirectory = fbd.SelectedPath;
                RealmAssetDirectoryTextBox.Text = assetDirectory;
                Settings.Default.MapAssetDirectory = RealmAssetDirectoryTextBox.Text;

                Settings.Default.Save();
            }
        }

        private void AutosaveRealmSwitch_CheckedChanged()
        {
            Settings.Default.RealmAutosave = AutosaveRealmSwitch.Checked;
            Settings.Default.Save();
        }

        private void AutosaveDirButton_Click(object sender, EventArgs e)
        {
            string autosaveDirectory = Settings.Default.AutosaveDirectory;

            if (string.IsNullOrEmpty(autosaveDirectory))
            {
                // realm autosave folder (location where map backups are saved during autosave)
                autosaveDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
            }

            FolderBrowserDialog fbd = new()
            {
                InitialDirectory = autosaveDirectory
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                autosaveDirectory = fbd.SelectedPath;
                AutosaveDirectoryTextBox.Text = autosaveDirectory;
                Settings.Default.MapAssetDirectory = AutosaveDirectoryTextBox.Text;
                Settings.Default.Save();
            }
        }

        private void PlaySoundOnSaveSwitch_CheckedChanged()
        {
            Settings.Default.PlaySoundOnSave = PlaySoundOnSaveSwitch.Checked;
            Settings.Default.Save();
        }

        private void DefaultExportFormatCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string defaultExportFormat = DefaultExportFormatCombo.Text;

            if (!string.IsNullOrEmpty(defaultExportFormat))
            {
                Settings.Default.DefaultExportFormat = defaultExportFormat;
            }
        }

        private void DrawContoursWhilePaintingSwitch_CheckedChanged()
        {
            Settings.Default.CalculateContoursWhilePainting = DrawContoursWhilePaintingSwitch.Checked;
            Settings.Default.Save();
        }

        private void ClipColoringToLandformSwitch_CheckedChanged()
        {
            Settings.Default.ClipLandformColoring = ClipColoringToLandformSwitch.Checked;
            Settings.Default.Save();
        }

        private void WH1024x768Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(1024, 768);
            Settings.Default.Save();
        }

        private void WH1280x720Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(1280, 720);
            Settings.Default.Save();
        }

        private void WH1280x1024Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(1280, 1024);
            Settings.Default.Save();
        }

        private void WH1600x1200Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(1600, 1200);
            Settings.Default.Save();
        }

        private void WH1920x1080Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(1920, 1080);
            Settings.Default.Save();
        }

        private void WH2560x1080Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(2560, 1080);
            Settings.Default.Save();
        }

        private void WH2048x1024Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(2048, 1024);
            Settings.Default.Save();
        }

        private void WH3840x2160Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(3840, 2160);
            Settings.Default.Save();
        }

        private void WH4096x2048Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(4096, 2048);
            Settings.Default.Save();
        }

        private void WH3300x2250Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(3300, 2250);
            Settings.Default.Save();
        }

        private void WH1754x1240Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(1754, 1240);
            Settings.Default.Save();
        }

        private void WH2480x1754Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(2480, 1754);
            Settings.Default.Save();
        }

        private void WH3508x2480Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(3508, 2480);
            Settings.Default.Save();
        }

        private void WH4960x3508Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(4960, 3508);
            Settings.Default.Save();
        }

        private void WH7016x4960Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(7016, 4960);
            Settings.Default.Save();
        }

        private void WH7680x4320Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Settings.Default.DefaultMapSize = new Size(7680, 4320);
            Settings.Default.Save();
        }

        private void DrawContourTooltipButton_MouseHover(object sender, EventArgs e)
        {
            string tt = "Turning this preference off will improve performance while painting landforms, especially on large maps or for computers with less memory or slower CPUs.";
            TOOLTIP.Show(tt, this, new Point(DrawContourTooltipButton.Right - 30, DrawContourTooltipButton.Top - 20), 4000);
        }

        private void AutosaveIntervalTooltipButton_MouseHover(object sender, EventArgs e)
        {
            string tt = "The time in minutes between automatic backups of the current realm.";
            TOOLTIP.Show(tt, this, new Point(AutosaveIntervalTooltipButton.Right - 30, AutosaveIntervalTooltipButton.Top - 20), 4000);
        }

        private void AutosaveRealmSwitch_Click(object sender, EventArgs e)
        {
            Settings.Default.RealmAutosave = AutosaveRealmSwitch.Checked;
            Settings.Default.Save();
        }


    }
}
