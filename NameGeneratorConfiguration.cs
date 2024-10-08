namespace RealmStudio
{
    public partial class NameGeneratorConfiguration : Form
    {
        public event EventHandler? NameGenerated;

        public string SelectedName { get; set; } = string.Empty;

        public NameGeneratorConfiguration()
        {
            InitializeComponent();
        }

        private void CloseNameConfigurationButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        protected virtual void OnNameGenerated(EventArgs e)
        {
            NameGenerated?.Invoke(this, e);
        }
    }
}
