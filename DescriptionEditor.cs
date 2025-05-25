namespace RealmStudio
{
    public partial class DescriptionEditor : Form
    {
        private string _descriptionText = string.Empty;

        public string DescriptionText
        {
            get => _descriptionText;
            set
            {
                _descriptionText = value;
                DescriptionTextbox.Text = value;
            }
        }

        public DescriptionEditor()
        {
            InitializeComponent();
        }

        private void CloseDescriptionButton_Click(object sender, EventArgs e)
        {
            DescriptionText = DescriptionTextbox.Text;
            Close();
        }
    }
}
