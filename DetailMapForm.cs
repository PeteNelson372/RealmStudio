using SkiaSharp;

namespace RealmStudio
{
    public partial class DetailMapForm : Form
    {
        private RealmStudioMap currentMap;
        private SKRect selectedArea;


        public DetailMapForm(RealmStudioMap currentMap, SKRect selectedArea)
        {
            InitializeComponent();
            this.currentMap = currentMap;
            this.selectedArea = selectedArea;

            MapTopUpDown.Value = (decimal)selectedArea.Top;
            MapLeftUpDown.Value = (decimal)selectedArea.Left;
            MapWidthUpDown.Value = (decimal)selectedArea.Width;
            MapHeightUpDown.Value = (decimal)selectedArea.Height;

            DetailMapWidthLabel.Text = (MapWidthUpDown.Value * MapZoomUpDown.Value).ToString();
            DetailMapHeightLabel.Text = (MapHeightUpDown.Value * MapZoomUpDown.Value).ToString();
        }

        private void CancelCreateDetailButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MapZoomTrack_Scroll(object sender, EventArgs e)
        {
            MapZoomUpDown.Value = (decimal)(MapZoomTrack.Value / 10.0);
        }

        private void MapZoomUpDown_ValueChanged(object sender, EventArgs e)
        {
            MapZoomTrack.Value = (int)(MapZoomUpDown.Value * 10);

            int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
            int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

            DetailMapWidthLabel.Text = detailMapWidth.ToString();
            DetailMapHeightLabel.Text = detailMapHeight.ToString();
        }
    }
}
