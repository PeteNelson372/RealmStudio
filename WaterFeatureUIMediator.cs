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

        private ColorPaintBrush _waterPaintBrush = ColorPaintBrush.SoftBrush;

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

        private Color _customColor1 = Color.White;
        private Color _customColor2 = Color.White;
        private Color _customColor3 = Color.White;
        private Color _customColor4 = Color.White;
        private Color _customColor5 = Color.White;
        private Color _customColor6 = Color.White;
        private Color _customColor7 = Color.White;
        private Color _customColor8 = Color.White;

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

        internal ColorPaintBrush WaterPaintBrush
        {
            get { return _waterPaintBrush; }
            set { SetPropertyField(nameof(WaterPaintBrush), ref _waterPaintBrush, value); }
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

        public Color CustomColor1
        {
            get { return _customColor1; }
            set { SetPropertyField(nameof(CustomColor1), ref _customColor1, value); }
        }

        public Color CustomColor2
        {
            get { return _customColor2; }
            set { SetPropertyField(nameof(CustomColor2), ref _customColor2, value); }
        }

        public Color CustomColor3
        {
            get { return _customColor3; }
            set { SetPropertyField(nameof(CustomColor3), ref _customColor3, value); }
        }

        public Color CustomColor4
        {
            get { return _customColor4; }
            set { SetPropertyField(nameof(CustomColor4), ref _customColor4, value); }
        }

        public Color CustomColor5
        {
            get { return _customColor5; }
            set { SetPropertyField(nameof(CustomColor5), ref _customColor5, value); }
        }

        public Color CustomColor6
        {
            get { return _customColor6; }
            set { SetPropertyField(nameof(CustomColor6), ref _customColor6, value); }
        }

        public Color CustomColor7
        {
            get { return _customColor7; }
            set { SetPropertyField(nameof(CustomColor7), ref _customColor7, value); }
        }

        public Color CustomColor8
        {
            get { return _customColor8; }
            set { SetPropertyField(nameof(CustomColor8), ref _customColor8, value); }
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
                UpdateWatureFeatureUI(changedPropertyName);
                UpdateRiverUI();
                WaterFeatureManager.Update();

                MainForm.SKGLRenderControl.Invalidate();
            }
        }

        internal void UpdateWatureFeatureUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WATERLAYER);
                MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WATERDRAWINGLAYER);

                waterLayer.ShowLayer = ShowWaterFeatureLayers;
                waterDrawingLayer.ShowLayer = ShowWaterFeatureLayers;

                MainForm.WaterColorSelectionButton.BackColor = WaterColor;
                MainForm.ShorelineColorSelectionButton.BackColor = ShorelineColor;
                MainForm.WaterPaintColorSelectButton.BackColor = WaterPaintColor;

                if (WaterPaintBrush == ColorPaintBrush.SoftBrush)
                {
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (WaterPaintBrush == ColorPaintBrush.HardBrush)
                {
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (WaterPaintBrush == ColorPaintBrush.PatternBrush1)
                {
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (WaterPaintBrush == ColorPaintBrush.PatternBrush2)
                {
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (WaterPaintBrush == ColorPaintBrush.PatternBrush3)
                {
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (WaterPaintBrush == ColorPaintBrush.PatternBrush4)
                {
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.WaterPatternBrush4Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.WaterPatternBrush3Button.FlatAppearance.BorderSize = 3;
                }

                MapStateMediator.SelectedColorPaintBrush = WaterPaintBrush;
                MapStateMediator.MainUIMediator.SetDrawingModeLabel();

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    switch (changedPropertyName)
                    {
                        case "CustomColor1":
                            {
                                MainForm.WaterCustomColor1.BackColor = CustomColor1;
                                MainForm.WaterCustomColor1.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor1.Text = (CustomColor1.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor1);
                            }
                            break;
                        case "CustomColor2":
                            {
                                MainForm.WaterCustomColor2.BackColor = CustomColor2;
                                MainForm.WaterCustomColor2.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor2.Text = (CustomColor2.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor2);
                            }
                            break;
                        case "CustomColor3":
                            {
                                MainForm.WaterCustomColor3.BackColor = CustomColor3;
                                MainForm.WaterCustomColor3.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor3.Text = (CustomColor3.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor3);
                            }
                            break;
                        case "CustomColor4":
                            {
                                MainForm.WaterCustomColor4.BackColor = CustomColor4;
                                MainForm.WaterCustomColor4.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor4.Text = (CustomColor4.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor4);
                            }
                            break;
                        case "CustomColor5":
                            {
                                MainForm.WaterCustomColor5.BackColor = CustomColor5;
                                MainForm.WaterCustomColor5.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor5.Text = (CustomColor5.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor5);
                            }
                            break;
                        case "CustomColor6":
                            {
                                MainForm.WaterCustomColor6.BackColor = CustomColor6;
                                MainForm.WaterCustomColor6.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor6.Text = (CustomColor6.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor6);
                            }
                            break;
                        case "CustomColor7":
                            {
                                MainForm.WaterCustomColor7.BackColor = CustomColor7;
                                MainForm.WaterCustomColor7.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor7.Text = (CustomColor7.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor7);
                            }
                            break;
                        case "CustomColor8":
                            {
                                MainForm.WaterCustomColor8.BackColor = CustomColor8;
                                MainForm.WaterCustomColor8.ForeColor = SystemColors.ControlDark;
                                MainForm.WaterCustomColor8.Text = (CustomColor8.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor8);
                            }
                            break;
                    }

                }
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

        internal void Reset()
        {
            WaterTextureList.Clear();

            ShowWaterFeatureLayers = true;

            WaterPaintBrush = ColorPaintBrush.SoftBrush;

            // water feature UI values
            WaterColor = Color.FromArgb(168, 140, 191, 197);
            ShorelineColor = Color.FromArgb(161, 144, 118);
            WaterFeatureBrushSize = 20;
            WaterFeatureEraserSize = 20;
            WaterPaintColor = Color.FromArgb(128, 145, 203, 184);
            WaterColorBrushSize = 20;
            WaterColorEraserSize = 20;

            // river UI values
            EditRiverPoints = false;
            RenderRiverTexture = false;
            RiverWidth = 4;
            RiverSourceFadeIn = true;

            CustomColor1 = Color.White;
            CustomColor2 = Color.White;
            CustomColor3 = Color.White;
            CustomColor4 = Color.White;
            CustomColor5 = Color.White;
            CustomColor6 = Color.White;
            CustomColor7 = Color.White;
            CustomColor8 = Color.White;
        }

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
            WaterPaintColor = waterColor;
        }

        #endregion

        #region Static Water Feature UI methods


        #endregion
    }
}
