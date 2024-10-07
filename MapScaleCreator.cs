namespace RealmStudio
{
    public partial class MapScaleCreator : Form
    {
        private readonly RealmStudioMainForm realmStudioMainForm;

        public MapScaleCreator(RealmStudioMainForm realmStudioMainForm)
        {
            InitializeComponent();
            this.realmStudioMainForm = realmStudioMainForm;

            Location = new Point(realmStudioMainForm.Width - 174, realmStudioMainForm.Location.Y + 114);
        }
    }
}
