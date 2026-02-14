namespace RealmStudioX
{
    public class AppStatusBarUiState
    {
        public string MapSize { get; set; } = "";

        public string ZoomLevel { get; set; } = "";

        public string DrawingMode { get; set; } = "";

        public string DrawingLayer { get; set; } = "USERDRAWING";

        public string DrawingPoint { get; set; } = "";

        public string ApplicationStatusMessage { get; set; } = "";

        public string ApplicationHelpMessage { get; set; } = "Ctrl+Mouse Wheel Up/Down to Zoom. Mouse Wheel Up/Down to change brush and eraser sizes. Click and drag Middle Mouse button (Mouse Wheel) to pan.";
    }
}
