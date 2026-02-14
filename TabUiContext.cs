namespace RealmStudioX
{
    internal class TabUiContext
    {
        public EditorTab ActiveTab { get; init; }

        public bool ShowBackgroundPanels => ActiveTab == EditorTab.Background;
        public bool ShowOceanPanels => ActiveTab == EditorTab.Ocean;
        public bool ShowLandformPanels => ActiveTab == EditorTab.Land;

        public bool ShowWaterPanels => ActiveTab == EditorTab.Water;

        public bool ShowPathPanels => ActiveTab == EditorTab.Paths;

        public bool ShowSymbolPanels => ActiveTab == EditorTab.Symbols;

        public bool ShowLabelPanels => ActiveTab == EditorTab.Labels;

        public bool ShowOverlayPanels => ActiveTab == EditorTab.Overlays;

        public bool ShowRegionPanels => ActiveTab == EditorTab.Regions;

        public bool ShowDrawingPanels => ActiveTab == EditorTab.Drawing;

        public bool ShowInteriorPanels => ActiveTab == EditorTab.Interior;

        public bool ShowDungeonPanels => ActiveTab == EditorTab.Dungeon;

        public bool ShowShipPanels => ActiveTab == EditorTab.Ship;

        public bool ShowPlanetPanels => ActiveTab == EditorTab.Planet;
    }
}
