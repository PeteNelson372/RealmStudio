namespace RealmStudioX
{
    internal class CameraUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        private float _zoom;

        public float Zoom
        {
            get { return _zoom; } 
            set { SetPropertyField(nameof(Zoom), ref _zoom, value); }
        }

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                RaiseChanged();
            }
        }
    }
}
