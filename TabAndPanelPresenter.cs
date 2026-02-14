namespace RealmStudioX
{
    internal class TabAndPanelPresenter : IUIPresenter<MainTabControlStatusBarUiState>
    {
        private RealmStudioMainForm _mainForm;
        private TabControl _mainTab;
        private TabManager _tabManager;
        private MainTabControlStatusBarUiState? _previousState;

        public TabManager TabManager
        {
            get { return _tabManager; }
            set { _tabManager = value; }
        }

        public TabAndPanelPresenter(RealmStudioMainForm mainForm)
        {
            // Validate input
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _mainTab = _mainForm.MainTab;

            _tabManager = new TabManager(_mainTab);
        }

        public void Apply(MainTabControlStatusBarUiState state)
        {
            if (_previousState == state) return;

            // MainTab tab visibility
            TabManager.SetVisible("BackgroundTab", state.BackgroundTabVisible);
            TabManager.SetVisible("OceanTab", state.OceanTabVisible);
            TabManager.SetVisible("LandTab", state.LandTabVisible);
            TabManager.SetVisible("WaterTab", state.WaterTabVisible);
            TabManager.SetVisible("PathTab", state.PathTabVisible);
            TabManager.SetVisible("SymbolTab", state.SymbolTabVisible);
            TabManager.SetVisible("LabelTab", state.LabelTabVisible);
            TabManager.SetVisible("OverlayTab", state.OverlayTabVisible);
            TabManager.SetVisible("RegionTab", state.RegionTabVisible);
            TabManager.SetVisible("DrawingTab", state.DrawingTabVisible);
            TabManager.SetVisible("InteriorFloorTab", state.InteriorTabVisible);
            TabManager.SetVisible("DungeonLevelTab", state.DungeonTabVisible);
            TabManager.SetVisible("ShipDeckTab", state.ShipTabVisible);
            TabManager.SetVisible("PlanetTab", state.PlanetTabVisible);

            // panels on right of UI, tool strips in tabs, other controls
            _mainForm.BackgroundToolPanel.Visible = false;
            _mainForm.OceanToolPanel.Visible = false;
            _mainForm.LandToolPanel.Visible = false;
            _mainForm.WaterToolPanel.Visible = false;
            _mainForm.PathToolPanel.Visible = false;
            _mainForm.SymbolToolPanel.Visible = false;
            _mainForm.LabelToolPanel.Visible = false;
            _mainForm.RegionToolPanel.Visible = false;
            _mainForm.OverlayToolPanel.Visible = false;
            _mainForm.DrawingToolPanel.Visible = false;
            _mainForm.InteriorToolPanel.Visible = false;
            _mainForm.DungeonToolPanel.Visible = false;
            _mainForm.PlanetToolPanel.Visible = false;
            _mainForm.ShipToolPanel.Visible = false;

            _mainForm.HeightMapToolsPanel.Visible = false;
            _mainForm.MapScaleCreatorPanel.Visible = false;
            _mainForm.FontSelectionPanel.Visible = false;

            _mainForm.BackgroundToolPanel.Visible = state.BackgroundToolPanelVisible;
            _mainForm.OceanToolPanel.Visible = state.OceanToolPanelVisible;
            _mainForm.LandToolPanel.Visible = state.LandToolPanelVisible;
            _mainForm.WaterToolPanel.Visible = state.WaterToolPanelVisible;
            _mainForm.PathToolPanel.Visible = state.PathToolPanelVisible;
            _mainForm.SymbolToolPanel.Visible = state.SymbolToolPanelVisible;
            _mainForm.LabelToolPanel.Visible = state.LabelToolPanelVisible;
            _mainForm.RegionToolPanel.Visible = state.RegionToolPanelVisible;
            _mainForm.OverlayToolPanel.Visible = state.OverlayToolPanelVisible;
            _mainForm.DrawingToolPanel.Visible = state.DrawingToolPanelVisible;
            _mainForm.InteriorToolPanel.Visible = state.InteriorToolPanelVisible;
            _mainForm.DungeonToolPanel.Visible = state.DungeonToolPanelVisible;
            _mainForm.PlanetToolPanel.Visible = state.PlanetToolPanelVisible;
            _mainForm.ShipToolPanel.Visible = state.ShipToolPanelVisible;

            // TODO:
            _mainForm.HeightMapToolsPanel.Visible = false;
            _mainForm.MapScaleCreatorPanel.Visible = false;
            _mainForm.FontSelectionPanel.Visible = false;

            _previousState = state;
        }
    }
}
