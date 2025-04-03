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
    internal sealed class WaterFeatureUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private readonly List<MapTexture> _waterTextureList = [];

        private bool _showWaterFeatureLayers = true;

        // water feature UI values
        private Color _waterColor = Color.FromArgb(168, 140, 191, 197);
        private Color _shorelineColor = Color.FromArgb(161, 144, 118);
        private int _waterFeatureBrushSize = 20;
        private int _waterFeatureEraserSize = 20;
        private Color _waterPaintColor = Color.FromArgb(128, 145, 203, 184);
        private int _waterColorBrushSize = 20;
        private int _waterColorEraserSize = 20;

        // river UI values
        private bool _editRiverPoints;
        private bool _renderRiverTexture;
        private int _riverWidth = 4;
        private bool _riverSourceFadeIn = true;


        internal WaterFeatureUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += WaterFeatureUIMediator_PropertyChanged;
        }

        #region Property Setter/Getters

        internal MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        // Water Feature UI properties
        internal List<MapTexture> WaterTextureList
        {
            get { return _waterTextureList; }
        }

        internal bool ShowWaterFeatureLayers
        {
            get { return _showWaterFeatureLayers; }
            set { SetPropertyField(nameof(ShowWaterFeatureLayers), ref _showWaterFeatureLayers, value); }
        }

        internal Color WaterColor
        {
            get { return _waterColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(WaterColor), ref _waterColor, value);
                }
            }
        }

        internal Color ShorelineColor
        {
            get { return _shorelineColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(ShorelineColor), ref _shorelineColor, value);
                }
            }
        }

        internal int WaterFeatureBrushSize
        {
            get { return _waterFeatureBrushSize; }
            set { SetPropertyField(nameof(WaterFeatureBrushSize), ref _waterFeatureBrushSize, value); }
        }

        internal int WaterFeatureEraserSize
        {
            get { return _waterFeatureEraserSize; }
            set { SetPropertyField(nameof(WaterFeatureEraserSize), ref _waterFeatureEraserSize, value); }
        }

        internal Color WaterPaintColor
        {
            get { return _waterPaintColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(WaterPaintColor), ref _waterPaintColor, value);
                }
            }
        }

        internal int WaterColorBrushSize
        {
            get { return _waterColorBrushSize; }
            set { SetPropertyField(nameof(WaterColorBrushSize), ref _waterColorBrushSize, value); }
        }

        internal int WaterColorEraserSize
        {
            get { return _waterColorEraserSize; }
            set { SetPropertyField(nameof(WaterColorEraserSize), ref _waterColorEraserSize, value); }
        }

        // River property setters/getters
        internal bool EditRiverPoints
        {
            get { return _editRiverPoints; }
            set { SetPropertyField(nameof(EditRiverPoints), ref _editRiverPoints, value); }
        }

        internal bool RenderRiverTexture
        {
            get { return _renderRiverTexture; }
            set { SetPropertyField(nameof(RenderRiverTexture), ref _renderRiverTexture, value); }
        }

        internal int RiverWidth
        {
            get { return _riverWidth; }
            set { SetPropertyField(nameof(RiverWidth), ref _riverWidth, value); }
        }

        internal bool RiverSourceFadeIn
        {
            get { return _riverSourceFadeIn; }
            set { SetPropertyField(nameof(RiverSourceFadeIn), ref _riverSourceFadeIn, value); }
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
            if (MapStateMediator.CurrentMap != null)
            {
                UpdateWatureFeatureUI();
                UpdateRiverUI();
                WaterFeatureManager.Update();

                MainForm.SKGLRenderControl.Invalidate();
            }
        }

        internal void UpdateWatureFeatureUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WATERLAYER);
                MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WATERDRAWINGLAYER);

                waterLayer.ShowLayer = ShowWaterFeatureLayers;
                waterDrawingLayer.ShowLayer = ShowWaterFeatureLayers;

                MainForm.WaterColorSelectionButton.BackColor = WaterColor;
                MainForm.ShorelineColorSelectionButton.BackColor = ShorelineColor;
                MainForm.WaterPaintColorSelectButton.BackColor = WaterPaintColor;
            }));
        }

        internal void UpdateRiverUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                SetRiverPointsVisible();
                SetRenderRiverTexture();
            }));
        }

        #endregion

        #region EventHandlers

        private void WaterFeatureUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Water Feature UI methods

        internal MapComponent? SelectWaterFeatureAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            MapComponent? selectedWaterFeature = null;

            List<MapComponent> waterFeatureComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER).MapLayerComponents;

            for (int i = 0; i < waterFeatureComponents.Count; i++)
            {
                if (waterFeatureComponents[i] is WaterFeature waterFeature)
                {
                    SKPath boundaryPath = waterFeature.WaterFeaturePath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            waterFeature.IsSelected = !waterFeature.IsSelected;

                            if (waterFeature.IsSelected)
                            {
                                selectedWaterFeature = waterFeature;
                            }
                            break;
                        }
                    }
                }
                else if (waterFeatureComponents[i] is River river)
                {
                    SKPath? boundaryPath = river.RiverBoundaryPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            river.IsSelected = !river.IsSelected;

                            if (river.IsSelected)
                            {
                                selectedWaterFeature = river;
                            }
                            else
                            {
                                EditRiverPoints = false;
                                river.ShowRiverPoints = false;
                            }
                            break;
                        }
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedWaterFeature);
            return selectedWaterFeature;
        }
        private void SetRenderRiverTexture()
        {
            if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river)
            {
                river.RenderRiverTexture = RenderRiverTexture;
                WaterFeatureManager.ConstructRiverPaintObjects(river);
            }
        }

        private void SetRiverPointsVisible()
        {
            if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river)
            {
                river.ShowRiverPoints = EditRiverPoints;
            }
        }


        internal void SetWaterColorFromPreset(string htmlColor)
        {
            Color waterColor = ColorTranslator.FromHtml(htmlColor);

            MainForm.WaterPaintColorSelectButton.BackColor = waterColor;
        }

        internal void SetWaterPaintColorFromCustomPresetButton(Button b)
        {
            if (b.Text != "")
            {
                Color waterColor = b.BackColor;

                MainForm.WaterPaintColorSelectButton.BackColor = waterColor;
            }
        }

        #endregion

        #region Static Water Feature UI methods


        #endregion
    }
}
