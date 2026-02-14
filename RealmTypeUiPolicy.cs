using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public class RealmTypeUiPolicy
    {
        public RealmMapType RealmType { get; init; }

        // Tab visibility
        public bool ShowBackgroundTab { get; init; }
        public bool ShowOceanTab { get; init; }
        public bool ShowLandformTab { get; init; }
        public bool ShowWaterTab { get; init; }
        public bool ShowPathsTab { get; init; }
        public bool ShowSymbolsTab { get; init; }
        public bool ShowLabelsTab { get; init; }
        public bool ShowOverlaysTab { get; init; }
        public bool ShowRegionsTab { get; init; }
        public bool ShowDrawingTab { get; init; }
        public bool ShowInteriorTab { get; init; }
        public bool ShowDungeonTab { get; init; }
        public bool ShowShipTab { get; init; }
        public bool ShowPlanetTab { get; init; }

        // Other panels and menu options
        public bool ShowBackgroundToolPanel { get; init; }
        public bool ShowLandToolPanel { get; init; }
        public bool ShowLandToolStrip { get; init; }
        public bool ShowHeightMapToolsPanel { get; init; }
        public bool ShowHeightmapMenuEntry { get; init; }
        public bool ShowWorldGlobeMenuEntry { get; init; }
    }
}
