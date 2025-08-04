namespace RealmStudio
{
    public abstract class DrawnMapComponent : MapComponent
    {
        private readonly Guid _drawnComponentGuid = Guid.NewGuid();

        protected DrawnMapComponent()
        {
            _drawnComponentGuid = Guid.NewGuid();
        }

        public Guid DrawnComponentGuid
        {
            get => _drawnComponentGuid;
        }
    }
}
