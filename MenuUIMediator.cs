namespace RealmStudioX
{
    internal class MenuUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        private bool _renderAsHeightMap;
        internal bool RenderAsHeightMap
        {
            get { return _renderAsHeightMap; }
            set { SetPropertyField(nameof(RenderAsHeightMap), ref _renderAsHeightMap, value); }
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
