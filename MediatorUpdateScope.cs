namespace RealmStudioX
{
    public sealed class MediatorUpdateScope : IDisposable
    {
        private readonly UiMediatorBase _mediator;

        public MediatorUpdateScope(UiMediatorBase mediator)
        {
            _mediator = mediator;
            _mediator.BeginUpdate();
        }

        public void Dispose()
        {
            _mediator.EndUpdate();
        }
    }
}
