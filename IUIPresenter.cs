namespace RealmStudioX
{
    public interface IUIPresenter<in TUiState>
    {
        void Apply(TUiState state);
    }
}
