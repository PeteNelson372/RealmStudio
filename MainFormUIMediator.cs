/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using System.ComponentModel;

namespace RealmStudio
{
    internal sealed class MainFormUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm _mainForm;
        private MapDrawingMode _currentDrawingMode = MapDrawingMode.None;
        private static int _selectedBrushSize;
        private static double _currentBrushVelocity = 2.0;
        private static float _drawingZoom = 1.0f;

        private bool _overlayLayerEnabled = true;
        private bool _waIntegrationEnabled;

        private TabPage? _backGroundTabPage;
        private TabPage? _oceanTabPage;
        private TabPage? _landformTabPage;
        private TabPage? _waterFeaturesTabPage;
        private TabPage? _pathsTabPage;
        private TabPage? _symbolsTabPage;
        private TabPage? _labelsTabPage;
        private TabPage? _overlaysTabPage;
        private TabPage? _regionsTabPage;
        private TabPage? _drawingTabPage;
        private TabPage? _interiorTabPage;
        private TabPage? _dungeonTabPage;
        private TabPage? _shipTabPage;
        private TabPage? _planetTabPage;

        public MainFormUIMediator(RealmStudioMainForm mainForm)
        {
            _mainForm = mainForm;
            PropertyChanged += MainFormUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        internal RealmStudioMainForm MainForm
        {
            get { return _mainForm; }
        }

        public MapDrawingMode CurrentDrawingMode
        {
            get { return _currentDrawingMode; }
            set { SetPropertyField(nameof(CurrentDrawingMode), ref _currentDrawingMode, value); }
        }

        // UI values

        internal int SelectedBrushSize
        {
            get { return _selectedBrushSize; }
            set { SetPropertyField(nameof(SelectedBrushSize), ref _selectedBrushSize, value); }
        }

        internal double CurrentBrushVelocity
        {
            get { return _currentBrushVelocity; }
            set { SetPropertyField(nameof(CurrentBrushVelocity), ref _currentBrushVelocity, value); }
        }

        internal float DrawingZoom
        {
            get { return _drawingZoom; }
            set { SetPropertyField(nameof(DrawingZoom), ref _drawingZoom, value); }
        }

        internal bool OverlayLayerEnabled
        {
            get { return _overlayLayerEnabled; }
            set { SetPropertyField(nameof(OverlayLayerEnabled), ref _overlayLayerEnabled, value); }
        }

        internal bool WorldAnvilIntegrationEnabled
        {
            get { return _waIntegrationEnabled; }
            set { SetPropertyField(nameof(WorldAnvilIntegration), ref _waIntegrationEnabled, value); }
        }

        internal TabPage? BackGroundTabPage
        {
            get { return _backGroundTabPage; }
            set { SetPropertyField(nameof(BackGroundTabPage), ref _backGroundTabPage, value); }
        }

        internal TabPage? OceanTabPage
        {
            get { return _oceanTabPage; }
            set { SetPropertyField(nameof(OceanTabPage), ref _oceanTabPage, value); }
        }

        internal TabPage? LandformTabPage
        {
            get { return _landformTabPage; }
            set { SetPropertyField(nameof(LandformTabPage), ref _landformTabPage, value); }
        }

        internal TabPage? WaterFeaturesTabPage
        {
            get { return _waterFeaturesTabPage; }
            set { SetPropertyField(nameof(WaterFeaturesTabPage), ref _waterFeaturesTabPage, value); }
        }

        internal TabPage? PathsTabPage
        {
            get { return _pathsTabPage; }
            set { SetPropertyField(nameof(PathsTabPage), ref _pathsTabPage, value); }
        }

        internal TabPage? SymbolsTabPage
        {
            get { return _symbolsTabPage; }
            set { SetPropertyField(nameof(SymbolsTabPage), ref _symbolsTabPage, value); }
        }

        internal TabPage? LabelsTabPage
        {
            get { return _labelsTabPage; }
            set { SetPropertyField(nameof(LabelsTabPage), ref _labelsTabPage, value); }
        }

        internal TabPage? OverlaysTabPage
        {
            get { return _overlaysTabPage; }
            set { SetPropertyField(nameof(OverlaysTabPage), ref _overlaysTabPage, value); }
        }

        internal TabPage? RegionsTabPage
        {
            get { return _regionsTabPage; }
            set { SetPropertyField(nameof(RegionsTabPage), ref _regionsTabPage, value); }
        }

        internal TabPage? DrawingTabPage
        {
            get { return _drawingTabPage; }
            set { SetPropertyField(nameof(DrawingTabPage), ref _drawingTabPage, value); }
        }

        internal TabPage? InteriorTabPage
        {
            get { return _interiorTabPage; }
            set { SetPropertyField(nameof(InteriorTabPage), ref _interiorTabPage, value); }
        }

        internal TabPage? DungeonTabPage
        {
            get { return _dungeonTabPage; }
            set { SetPropertyField(nameof(DungeonTabPage), ref _dungeonTabPage, value); }
        }

        internal TabPage? ShipTabPage
        {
            get { return _shipTabPage; }
            set { SetPropertyField(nameof(ShipTabPage), ref _shipTabPage, value); }
        }

        internal TabPage? PlanetTabPage
        {
            get { return _planetTabPage; }
            set { SetPropertyField(nameof(PlanetTabPage), ref _planetTabPage, value); }
        }

        #endregion

        #region Property Change Handler Methods

        internal void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyUpdate(string? changedPropertyName)
        {
            UpdateMainFormUI(changedPropertyName);
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateMainFormUI(string? changedPropertyName)
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                switch (changedPropertyName)
                {
                    case "CurrentDrawingMode":
                        {
                            SetDrawingModeLabel();
                        }
                        break;
                    case "OverlayLayerEnabled":
                        {
                            EnabledDisableOverlayLayer();
                        }
                        break;
                    case "WorldAnvilIntegration":
                        {
                            EnableDisableWorldAnvilIntegration();
                        }
                        break;
                }
            }));
        }



        #endregion

        #region Event Handlers

        private void MainFormUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region UI Update Methods

        internal static SKPoint CalculateCursorPoint(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            SKPoint cursorPoint = new((e.X / MapStateMediator.MainUIMediator.DrawingZoom) - MapStateMediator.ScrollPoint.X,
                (e.Y / MapStateMediator.MainUIMediator.DrawingZoom) - MapStateMediator.ScrollPoint.Y);

            return cursorPoint;
        }

        internal void ConfigureMainTab()
        {
            if (MapStateMediator.CurrentMap == null)
            {
                return;
            }

            for (int i = MainForm.MainTab.TabPages.Count - 1; i >= 1; i--)
            {
                if (MainForm.MainTab.TabPages[i] == OceanTabPage ||
                    MainForm.MainTab.TabPages[i] == LandformTabPage ||
                    MainForm.MainTab.TabPages[i] == InteriorTabPage ||
                    MainForm.MainTab.TabPages[i] == DungeonTabPage ||
                    MainForm.MainTab.TabPages[i] == PlanetTabPage ||
                    MainForm.MainTab.TabPages[i] == ShipTabPage)
                {
                    MainForm.MainTab.TabPages.RemoveAt(i);
                }
            }

            switch (MapStateMediator.CurrentMap.RealmType)
            {
                case RealmMapType.World:
                    {
                        if (OceanTabPage != null && LandformTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(1, OceanTabPage);
                            MainForm.MainTab.TabPages.Insert(2, LandformTabPage);
                        }
                    }
                    break;
                case RealmMapType.Region:
                    {
                        if (OceanTabPage != null && LandformTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(1, OceanTabPage);
                            MainForm.MainTab.TabPages.Insert(2, LandformTabPage);
                        }
                    }
                    break;
                case RealmMapType.City:
                    {
                        if (OceanTabPage != null && LandformTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(1, OceanTabPage);
                            MainForm.MainTab.TabPages.Insert(2, LandformTabPage);
                        }
                    }
                    break;
                case RealmMapType.Interior:
                    {
                        // shouldn't happen
                    }
                    break;
                case RealmMapType.Dungeon:
                    {
                        // shouldn't happen
                    }
                    break;
                case RealmMapType.SolarSystem:
                    {
                        // shouldn't happen
                    }
                    break;
                case RealmMapType.Ship:
                    {
                        // shouldn't happen
                    }
                    break;
                case RealmMapType.Other:
                    {
                        if (OceanTabPage != null && LandformTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(1, OceanTabPage);
                            MainForm.MainTab.TabPages.Insert(2, LandformTabPage);
                        }
                    }
                    break;
                case RealmMapType.InteriorFloor:
                    {
                        if (InteriorTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(2, InteriorTabPage);
                        }
                    }
                    break;
                case RealmMapType.DungeonLevel:
                    {
                        if (DungeonTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(2, DungeonTabPage);
                        }
                    }
                    break;
                case RealmMapType.ShipDeck:
                    {
                        if (ShipTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(2, ShipTabPage);
                        }
                    }
                    break;
                case RealmMapType.SolarSystemBody:
                    {
                        if (PlanetTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(2, PlanetTabPage);
                        }
                    }
                    break;
                default:
                    {
                        if (OceanTabPage != null && LandformTabPage != null)
                        {
                            MainForm.MainTab.TabPages.Insert(1, OceanTabPage);
                            MainForm.MainTab.TabPages.Insert(2, LandformTabPage);
                        }
                    }
                    break;
            }
        }

        internal void SetDrawingMode(MapDrawingMode newDrawingMode, int newBrushSize = -1)
        {
            CurrentDrawingMode = newDrawingMode;

            MainForm.RoundRectButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.TriangleButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.RightTriangleButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.DiamondButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.PentagonButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.HexagonButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.ArrowButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.FivePointStarButton.FlatAppearance.BorderColor = Color.LightGray;
            MainForm.SixPointStarButton.FlatAppearance.BorderColor = Color.LightGray;

            if (newDrawingMode == MapDrawingMode.DrawingRoundedRectangle)
            {
                MainForm.RoundRectButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingTriangle)
            {
                MainForm.TriangleButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingRightTriangle)
            {
                MainForm.RightTriangleButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingDiamond)
            {
                MainForm.DiamondButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingPentagon)
            {
                MainForm.PentagonButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingHexagon)
            {
                MainForm.HexagonButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingArrow)
            {
                MainForm.ArrowButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingFivePointStar)
            {
                MainForm.FivePointStarButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }
            else if (newDrawingMode == MapDrawingMode.DrawingSixPointStar)
            {
                MainForm.SixPointStarButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            }

            SetDrawingModeLabel();

            if (newBrushSize >= 0)
            {
                SelectedBrushSize = newBrushSize;
            }

            MainForm.SKGLRenderControl.Focus();
        }

        internal void SetDrawingModeLabel()
        {
            string modeText = "Drawing Mode: ";

            modeText += CurrentDrawingMode switch
            {
                MapDrawingMode.None => "None",
                MapDrawingMode.LandPaint => "Paint Landform",
                MapDrawingMode.LandErase => "Erase Landform",
                MapDrawingMode.LandColorErase => "Erase Landform Color",
                MapDrawingMode.LandColor => "Color Landform",
                MapDrawingMode.OceanErase => "Erase Ocean",
                MapDrawingMode.OceanPaint => "Paint Ocean",
                MapDrawingMode.ColorSelect => "Select Color",
                MapDrawingMode.LandformSelect => "Select Landform",
                MapDrawingMode.LandformHeightMapSelect => "Select Landform",
                MapDrawingMode.WaterPaint => "Paint Water Feature",
                MapDrawingMode.WaterErase => "Erase Water Feature",
                MapDrawingMode.WaterColor => "Color Water Feature",
                MapDrawingMode.WaterColorErase => "Erase Water Feature Color",
                MapDrawingMode.LakePaint => "Paint Lake",
                MapDrawingMode.RiverPaint => "Paint River",
                MapDrawingMode.RiverEdit => "Edit River",
                MapDrawingMode.WaterFeatureSelect => "Select Water Feature",
                MapDrawingMode.PathPaint => "Draw Path",
                MapDrawingMode.PathSelect => "Select Path",
                MapDrawingMode.PathEdit => "Edit Path",
                MapDrawingMode.SymbolErase => "Erase Symbol",
                MapDrawingMode.SymbolPlace => "Place Symbol",
                MapDrawingMode.SymbolSelect => "Select Symbol",
                MapDrawingMode.SymbolColor => "Color Symbol",
                MapDrawingMode.DrawBezierLabelPath => "Draw Curve Label Path",
                MapDrawingMode.DrawArcLabelPath => "Draw Arc Label Path",
                MapDrawingMode.DrawLabel => "Place Label",
                MapDrawingMode.LabelSelect => "Select Label",
                MapDrawingMode.DrawBox => "Draw Box",
                MapDrawingMode.PlaceWindrose => "Place Windrose",
                MapDrawingMode.SelectMapScale => "Move Map Scale",
                MapDrawingMode.DrawMapMeasure => "Draw Map Measure",
                MapDrawingMode.RegionPaint => "Draw Region",
                MapDrawingMode.RegionSelect => "Select Region",
                MapDrawingMode.RealmAreaSelect => "Select Area",
                MapDrawingMode.HeightMapPaint => "Paint Height Map",
                MapDrawingMode.MapHeightIncrease => "Increase Map Height",
                MapDrawingMode.MapHeightDecrease => "Decrease Map Height",
                MapDrawingMode.DrawingLine => "Draw Line",
                MapDrawingMode.DrawingErase => "Erase",
                MapDrawingMode.DrawingPaint => "Paint",
                MapDrawingMode.DrawingRectangle => "Draw Rectangle",
                MapDrawingMode.DrawingEllipse => "Draw Ellipse",
                MapDrawingMode.DrawingPolygon => "Draw Polygon",
                MapDrawingMode.DrawingStamp => "Stamp",
                MapDrawingMode.DrawingDiamond => "Draw Diamond",
                MapDrawingMode.DrawingRoundedRectangle => "Draw Rounded Rectangle",
                MapDrawingMode.DrawingTriangle => "Draw Triangle",
                MapDrawingMode.DrawingRightTriangle => "Draw Right Triangle",
                MapDrawingMode.DrawingHexagon => "Draw Hexagon",
                MapDrawingMode.DrawingPentagon => "Draw Pentagon",
                MapDrawingMode.DrawingArrow => "Draw Arrow",
                MapDrawingMode.DrawingFivePointStar => "Draw 5-Point Star",
                MapDrawingMode.DrawingSixPointStar => "Draw 6-Point Star",
                MapDrawingMode.DrawingSelect => "Select Drawn Object",
                MapDrawingMode.InteriorFloorPaint => "Paint Interior Floor",
                _ => "Undefined",
            };

            modeText += ". Selected Brush: ";

            switch (MapStateMediator.SelectedColorPaintBrush)
            {
                case ColorPaintBrush.SoftBrush:
                    modeText += "Soft Brush";
                    break;
                case ColorPaintBrush.HardBrush:
                    modeText += "Hard Brush";
                    break;
                case ColorPaintBrush.PatternBrush1:
                    modeText += "Pattern 1";
                    break;
                case ColorPaintBrush.PatternBrush2:
                    modeText += "Pattern 2";
                    break;
                case ColorPaintBrush.PatternBrush3:
                    modeText += "Pattern 3";
                    break;
                case ColorPaintBrush.PatternBrush4:
                    modeText += "Pattern 4";
                    break;
                case ColorPaintBrush.None:
                    break;
                default:
                    modeText += "None";
                    break;
            }

            MainForm.DrawingModeLabel.Text = modeText;
            MainForm.ApplicationStatusStrip.Refresh();
        }

        internal void SetDrawingLayerLabel(string labelText)
        {
            MainForm.DrawingLayerLabel.Text = labelText.ToUpperInvariant();
            MainForm.ApplicationStatusStrip.Refresh();
        }

        internal void ChangeDrawingZoom(int upDown)
        {
            // TODO: scrollbars need to be updated
            // increase/decrease zoom by 10%, limiting to no less than 10% and no greater than 800%
            DrawingZoom = (upDown < 0) ? Math.Max(0.1f, DrawingZoom - 0.1f) : Math.Min(8.0f, DrawingZoom + 0.1f);
        }

        internal void EnabledDisableOverlayLayer()
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER);
            overlayLayer.ShowLayer = OverlayLayerEnabled;

            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.FRAMELAYER);
            frameLayer.ShowLayer = OverlayLayerEnabled;

            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.ABOVEOCEANGRIDLAYER);
            aboveOceanGridLayer.ShowLayer = OverlayLayerEnabled;

            MapLayer belowSymbolsGridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER);
            belowSymbolsGridLayer.ShowLayer = OverlayLayerEnabled;

            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER);
            defaultGridLayer.ShowLayer = OverlayLayerEnabled;

            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.MEASURELAYER);
            measureLayer.ShowLayer = OverlayLayerEnabled;
        }

        internal void ShowHideFontSelectionPanel(bool visible)
        {
            MainForm.FontSelectionPanel.Visible = visible;
        }

        internal void EnableDisableWorldAnvilIntegration()
        {
            if (WorldAnvilIntegrationEnabled)
            {
                MainForm.WorldAnvilIntegrationButton.Visible = true;
                MainForm.WorldAnvilIntegrationButton.Enabled = true;
            }
            else
            {
                MainForm.WorldAnvilIntegrationButton.Visible = false;
                MainForm.WorldAnvilIntegrationButton.Enabled = false;

                MainForm.WorldAnvilMapButton.Visible = false;
                MainForm.WorldAnvilMapButton.Enabled = false;
            }
        }

        #endregion
    }
}
