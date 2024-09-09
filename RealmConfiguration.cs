namespace RealmStudio
{
    public partial class RealmConfiguration : Form
    {
        public RealmStudioMap map = new();
        public float MapAspectRatio { get; set; } = 1.0F;
        public bool AspectRatioLocked = true;

        public RealmConfiguration()
        {
            InitializeComponent();
        }

        private void WH1024x768Radio_Click(object sender, EventArgs e)
        {
            if (WH1024x768Radio.Checked)
            {
                WidthUpDown.Value = 1024;
                HeightUpDown.Value = 768;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH1280x720Radio_Click(object sender, EventArgs e)
        {
            if (WH1280x720Radio.Checked)
            {
                WidthUpDown.Value = 1280;
                HeightUpDown.Value = 720;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH1280x1024Radio_Click(object sender, EventArgs e)
        {
            if (WH1280x1024Radio.Checked)
            {
                WidthUpDown.Value = 1280;
                HeightUpDown.Value = 1024;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH1600x1200Radio_Click(object sender, EventArgs e)
        {
            if (WH1600x1200Radio.Checked)
            {
                WidthUpDown.Value = 1600;
                HeightUpDown.Value = 1200;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH1920x1080Radio_Click(object sender, EventArgs e)
        {
            if (WH1920x1080Radio.Checked)
            {
                WidthUpDown.Value = 1920;
                HeightUpDown.Value = 1080;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH2560x1080Radio_Click(object sender, EventArgs e)
        {
            if (WH2560x1080Radio.Checked)
            {
                WidthUpDown.Value = 2560;
                HeightUpDown.Value = 1080;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH2048x1024Radio_Click(object sender, EventArgs e)
        {
            if (WH2048x1024Radio.Checked)
            {
                WidthUpDown.Value = 2048;
                HeightUpDown.Value = 1024;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH3840x2160Radio_Click(object sender, EventArgs e)
        {
            if (WH3840x2160Radio.Checked)
            {
                WidthUpDown.Value = 3840;
                HeightUpDown.Value = 2160;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH4096x2048Radio_Click(object sender, EventArgs e)
        {
            if (WH4096x2048Radio.Checked)
            {
                WidthUpDown.Value = 4096;
                HeightUpDown.Value = 2048;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH3300x2250Radio_Click(object sender, EventArgs e)
        {
            if (WH3300x2250Radio.Checked)
            {
                WidthUpDown.Value = 3300;
                HeightUpDown.Value = 2250;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH1754x1240Radio_Click(object sender, EventArgs e)
        {
            if (WH1754x1240Radio.Checked)
            {
                WidthUpDown.Value = 1754;
                HeightUpDown.Value = 1240;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH2840x1754Radio_Click(object sender, EventArgs e)
        {
            if (WH2840x1754Radio.Checked)
            {
                WidthUpDown.Value = 2840;
                HeightUpDown.Value = 1754;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH3508x2480Radio_Click(object sender, EventArgs e)
        {
            if (WH3508x2480Radio.Checked)
            {
                WidthUpDown.Value = 3508;
                HeightUpDown.Value = 2480;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH4960x3508Radio_Click(object sender, EventArgs e)
        {
            if (WH4960x3508Radio.Checked)
            {
                WidthUpDown.Value = 4960;
                HeightUpDown.Value = 3508;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH7016x4960Radio_Click(object sender, EventArgs e)
        {
            if (WH7016x4960Radio.Checked)
            {
                WidthUpDown.Value = 7016;
                HeightUpDown.Value = 4960;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void WH7680x4320Radio_Click(object sender, EventArgs e)
        {
            if (WH7680x4320Radio.Checked)
            {
                WidthUpDown.Value = 7680;
                HeightUpDown.Value = 4320;

                map.MapWidth = (int)WidthUpDown.Value;
                map.MapHeight = (int)HeightUpDown.Value;

                CalculateAspectRatio();
            }
        }

        private void CalculateAspectRatio()
        {
            MapAspectRatio = (float)(WidthUpDown.Value / HeightUpDown.Value);
            AspectRatioLabel.Text = MapAspectRatio.ToString("F2");
        }

        private void WorldRadioButton_Click(object sender, EventArgs e)
        {
            if (WorldRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.World;
            }
        }

        private void RegionRadioButton_Click(object sender, EventArgs e)
        {
            if (RegionRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Region;
            }
        }

        private void CityRadioButton_Click(object sender, EventArgs e)
        {
            if (CityRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.City;
            }
        }

        private void InteriorRadioButton_Click(object sender, EventArgs e)
        {
            if (InteriorRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Interior;
            }
        }

        private void DungeonRadioButton_Click(object sender, EventArgs e)
        {
            if (DungeonRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Dungeon;
            }
        }

        private void SolarSystemRadioButton_Click(object sender, EventArgs e)
        {
            if (SolarSystemRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.SolarSystem;
            }
        }

        private void ShipRadioButton_Click(object sender, EventArgs e)
        {
            if (ShipRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Ship;
            }
        }

        private void OtherRadioButton_Click(object sender, EventArgs e)
        {
            if (OtherRadioButton.Checked)
            {
                map.RealmType = RealmTypeEnum.Other;
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
            var temp = WidthUpDown.Value;
            WidthUpDown.Value = HeightUpDown.Value;
            HeightUpDown.Value = temp;
        }
    }
}
