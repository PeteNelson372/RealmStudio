namespace RealmStudioX
{
    internal class UserInterfaceStateBuilder
    {
        public static UserInterfaceState Build(
            RealmTypeUiPolicy? policy,
            MapScene? scene,
            EditorController? editorController,
            CommandManager? commandManager,
            MainFormUIMediator? mainMediator,
            BackgroundUIMediator? backgroundMediator,
            BoxUIMediator? boxMediator,
            DrawingUIMediator? drawingMediator,
            FrameUIMediator? frameMediator,
            MapGridUIMediator? gridMediator,
            InteriorUIMediator? interiorMediator,
            LabelUIMediator? labelMediator,
            OceanUIMediator? oceanMediator,
            PathUIMediator? pathMediator,
            LabelPresetUIMediator? presetMediator,
            LandformUIMediator? landformMediator,
            MapMeasureUIMediator? measureMediator,
            MapScaleUIMediator? scaleMediator,
            SymbolUIMediator? symbolMediator,
            RegionUIMediator? regionMediator,
            VignetteUIMediator? vignetteMediator,
            WaterFeatureUIMediator? waterFeatureMediator,
            WindroseUIMediator? windroseMediator,
            MenuUIMediator? menuMediator)
        {
            return new UserInterfaceState
            {
                MainTabUiState = BuildMainTab(editorController, policy, mainMediator, menuMediator, scene),
            };
        }

        private static MainTabControlStatusBarUiState BuildMainTab(EditorController? editorController,
            RealmTypeUiPolicy? policy,
            MainFormUIMediator? mainMediator,
            MenuUIMediator? menuMediator,
            MapScene? scene)
        {
            ArgumentNullException.ThrowIfNull(editorController, nameof(editorController));
            ArgumentNullException.ThrowIfNull(mainMediator, nameof(mainMediator));
            ArgumentNullException.ThrowIfNull(menuMediator, nameof(menuMediator));
            ArgumentNullException.ThrowIfNull(policy, nameof(policy));

            MainTabControlStatusBarUiState tabState = new()
            {
                BackgroundTabVisible = policy.ShowBackgroundTab,
                OceanTabVisible = policy.ShowOceanTab,
                LandTabVisible = policy.ShowLandformTab,
                WaterTabVisible = policy.ShowWaterTab,
                PathTabVisible = policy.ShowPathsTab,
                SymbolTabVisible = policy.ShowSymbolsTab,
                LabelTabVisible = policy.ShowLabelsTab,
                OverlayTabVisible = policy.ShowOverlaysTab,
                RegionTabVisible = policy.ShowRegionsTab,
                DrawingTabVisible = policy.ShowDrawingTab,
                InteriorTabVisible = policy.ShowInteriorTab,
                DungeonTabVisible = policy.ShowDungeonTab,
                ShipTabVisible = policy.ShowShipTab,
                PlanetTabVisible = policy.ShowPlanetTab,

                // what panels and other controls should be shown/hidden when the user selects a tab?

                // start with all panels hidden except BackgroundToolPanel

                BackgroundToolPanelVisible = true,
                OceanToolPanelVisible = false,
                LandToolPanelVisible = false,
                WaterToolPanelVisible = false,
                PathToolPanelVisible = false,
                SymbolToolPanelVisible = false,
                LabelToolPanelVisible = false,
                OverlayToolPanelVisible = false,
                RegionToolPanelVisible = false,
                DrawingToolPanelVisible = false,
                HeightMapToolsPanelVisible = false,
                DungeonToolPanelVisible = false,
                InteriorToolPanelVisible = false,
                PlanetToolPanelVisible = false,
                ShipToolPanelVisible = false
            };

            tabState.BackgroundToolPanelVisible = (policy.ShowBackgroundToolPanel && mainMediator.ActiveTab == EditorTab.Background)
                || (mainMediator.ActiveTab == EditorTab.Land && menuMediator.RenderAsHeightMap);

            tabState.OceanToolPanelVisible = policy.ShowOceanTab && mainMediator.ActiveTab == EditorTab.Ocean;

            tabState.LandToolPanelVisible = policy.ShowLandformTab
                && !menuMediator.RenderAsHeightMap
                && mainMediator.ActiveTab == EditorTab.Land;

            tabState.LandToolStripVisible = !menuMediator.RenderAsHeightMap && policy.ShowLandformTab && mainMediator.ActiveTab == EditorTab.Land;

            tabState.HeightMapToolsPanelVisible = menuMediator.RenderAsHeightMap && policy.ShowLandformTab && mainMediator.ActiveTab == EditorTab.Land;

            tabState.WaterToolPanelVisible = policy.ShowWaterTab && mainMediator.ActiveTab == EditorTab.Water;

            tabState.PathToolPanelVisible = policy.ShowPathsTab && mainMediator.ActiveTab == EditorTab.Paths;

            tabState.SymbolToolPanelVisible = policy.ShowSymbolsTab && mainMediator.ActiveTab == EditorTab.Symbols;

            tabState.LabelToolPanelVisible = policy.ShowLabelsTab && mainMediator.ActiveTab == EditorTab.Labels;

            tabState.OverlayToolPanelVisible = policy.ShowOverlaysTab && mainMediator.ActiveTab == EditorTab.Overlays;

            tabState.RegionToolPanelVisible = policy.ShowRegionsTab && mainMediator.ActiveTab == EditorTab.Regions;

            tabState.DrawingToolPanelVisible = policy.ShowDrawingTab && mainMediator.ActiveTab == EditorTab.Drawing;

            tabState.DungeonToolPanelVisible = policy.ShowDungeonTab && mainMediator.ActiveTab == EditorTab.Dungeon;

            tabState.InteriorToolPanelVisible = policy.ShowInteriorTab && mainMediator.ActiveTab == EditorTab.Interior;

            tabState.PlanetToolPanelVisible = policy.ShowPlanetTab && mainMediator.ActiveTab == EditorTab.Planet;

            tabState.ShipToolPanelVisible = policy.ShowShipTab || mainMediator.ActiveTab == EditorTab.Ship;

            return tabState;
        }

        public static AppStatusBarUiState BuildStatusBar(EditorController? editorController, MapScene? scene)
        {
            AppStatusBarUiState barState = new();

            if (editorController != null && editorController.Scene != null && editorController.Scene.Map != null)
            {
                barState.MapSize = "Map Size: " + editorController.Scene.Map.MapWidth.ToString() + " x " + editorController.Scene.Map.MapHeight.ToString();

                barState.DrawingPoint = "Cursor Point: "
                    + ((int)editorController.Scene.Camera.CurrentMouseLocation.X).ToString()
                    + " , "
                    + ((int)editorController.Scene.Camera.CurrentMouseLocation.Y).ToString()
                    + "   Map Point: "
                    + ((int)editorController.Scene.Camera.CurrentCursorPoint.X).ToString()
                    + " , "
                    + ((int)editorController.Scene.Camera.CurrentCursorPoint.Y).ToString();

                barState.DrawingLayer = editorController.MainMediator?.ActiveMapLayer?.MapLayerName ?? string.Empty;
            }

            return barState;
        }


    }
}
