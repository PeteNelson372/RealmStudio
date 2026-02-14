namespace RealmStudioX
{
    public class UserInterfaceState
    {
        public static UserInterfaceState Empty { get; } = new();

        public MainTabControlStatusBarUiState MainTabUiState { get; set; } = new MainTabControlStatusBarUiState();

        public AppTitleBarUiState TitleBarUiState {  get; set; } = new AppTitleBarUiState();
        public MainMenuBarUiState MainMenuUiState { get; set; } = new MainMenuBarUiState();

    }
}
