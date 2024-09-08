namespace RealmStudio
{
    public partial class RealmConfiguration : Form
    {
        public RealmStudioMap map = new();
        public float MapAspectRatio { get; set; } = 1.0F;

        public RealmConfiguration()
        {
            InitializeComponent();
        }

        private void WH1024x768Radio_Click(object sender, EventArgs e)
        {
            if ( WH1024x768Radio.Checked)
            {
                WidthUpDown.Value = 1024;
                HeightUpDown.Value = 768;

                map.MapWidth = 1024;
                map.MapHeight = 768;

                MapAspectRatio = (float)(WidthUpDown.Value / HeightUpDown.Value);
                AspectRatioLabel.Text = MapAspectRatio.ToString("F2");
            }


        }

        private void WH1280x720Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH1280x1024Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH1600x1200Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH1920x1080Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH2560x1080Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH2048x1024Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH3840x2160Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH4096x2048Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH3300x2250Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH1754x1240Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH2840x1754Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH3508x2480Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH4960x3508Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH7016x4960Radio_Click(object sender, EventArgs e)
        {

        }

        private void WH7680x4320Radio_Click(object sender, EventArgs e)
        {

        }
    }
}
