using System.ComponentModel;

namespace RealmStudio
{
    public partial class ColorQuickPick : Form
    {
        private Color _selectedColor = Color.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedColor
        {
            get => _selectedColor;
            set => _selectedColor = value;
        }

        public ColorQuickPick()
        {
            InitializeComponent();
        }

        private void SelectColorGrid_ColorChanged(object sender, EventArgs e)
        {
            if (SelectColorGrid.ColorIndex >= 0 && SelectColorGrid.ColorIndex < SelectColorGrid.Colors.Count)
            {
                SelectedColor = SelectColorGrid.Colors[SelectColorGrid.ColorIndex];
            }

            Close();
        }

        private void ColorSelectCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ColorQuickPick_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, SystemColors.Control, ButtonBorderStyle.Solid);
        }
    }
}
