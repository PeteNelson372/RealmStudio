namespace RealmStudioX
{
    public abstract class UiMediatorBase
    {
        private int _updateDepth;
        private bool _pendingChange;

        public event Action? Changed;

        public void BeginUpdate()
        {
            _updateDepth++;
        }

        public void EndUpdate()
        {
            if (_updateDepth == 0)
                return;

            _updateDepth--;

            if (_updateDepth == 0 && _pendingChange)
            {
                _pendingChange = false;
                Changed?.Invoke();
            }
        }

        protected void RaiseChanged()
        {
            if (_updateDepth > 0)
            {
                _pendingChange = true;
                return;
            }

            Changed?.Invoke();
        }
    }

}
