using RealmStudio.Properties;

namespace RealmStudio
{
    public partial class UserPreferences : Form
    {
        public UserPreferences()
        {
            InitializeComponent();

            string measurementUnitsSetting = Settings.Default.MeasurementUnits;

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

            string defaultAssetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + Path.DirectorySeparatorChar
                + "RealmStudio"
                + Path.DirectorySeparatorChar
                + "Assets";

            string assetDirectory = Settings.Default.MapAssetDirectory;

            if (string.IsNullOrEmpty(assetDirectory))
            {
                Settings.Default.MapAssetDirectory = defaultAssetFolder;
            }

            RealmAssetDirectoryTextBox.Text = Settings.Default.MapAssetDirectory;
        }

        private void ClosePreferencesButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AutosaveIntervalTrack_Scroll(object sender, EventArgs e)
        {
            double value = AutosaveIntervalTrack.Value;
            double indexDbl = value / AutosaveIntervalTrack.TickFrequency;
            int index = (int)(Math.Round(indexDbl));

            AutosaveIntervalTrack.Value = AutosaveIntervalTrack.TickFrequency * index;
        }

        private void MeasurementUnitsCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string measurementUnits = MeasurementUnitsCombo.Text;

            if (!string.IsNullOrEmpty(measurementUnits))
            {
                Settings.Default.MeasurementUnits = measurementUnits;
            }
        }
    }
}
