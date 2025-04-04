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

            SKPoint cursorPoint = new((e.X / MapStateMediator.MainUIMediator.DrawingZoom) - MapStateMediator.DrawingPoint.X,
                (e.Y / MapStateMediator.MainUIMediator.DrawingZoom) - MapStateMediator.DrawingPoint.Y);

            return cursorPoint;
        }

        internal void SetDrawingMode(MapDrawingMode newDrawingMode, int newBrushSize = -1)
        {
            CurrentDrawingMode = newDrawingMode;
            SetDrawingModeLabel();

            if (newBrushSize >= 0)
            {
                SelectedBrushSize = newBrushSize;
            }
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
                case ColorPaintBrush.None:
                    break;
                default:
                    modeText += "None";
                    break;
            }

            MainForm.DrawingModeLabel.Text = modeText;
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

        #endregion
    }
}
