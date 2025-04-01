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
using System.ComponentModel;

namespace RealmStudio
{
    internal class LandformUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private readonly List<MapTexture> _landTextureList = [];

        private bool _showLandformLayers = true;

        private Color _landPaintColor = Color.FromArgb(128, 230, 208, 171);
        private int _landformBrushSize = 64;
        private int _landformEraserSize = 64;
        private int _landPaintBrushSize = 20;
        private int _landPaintEraserSize = 20;
        private Color _landOutlineColor = Color.FromArgb(62, 55, 40);
        private Color _landBackgroundColor = Color.White;
        private Color _coastlineColor = Color.FromArgb(187, 156, 195, 183);
        private int _coastlineEffectDistance;
        private int _landOutlineWidth = 2;
        private bool _useTextureBackground = true;

        private int _landformTextureIndex;

        private GeneratedLandformType _generatedLandformType = GeneratedLandformType.Region;

        public LandformUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += LandformUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        internal bool ShowLandformLayers
        {
            get { return _showLandformLayers; }
            set { SetPropertyField(nameof(ShowLandformLayers), ref _showLandformLayers, value); }
        }

        internal Color LandPaintColor
        {
            get { return _landPaintColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(LandPaintColor), ref _landPaintColor, value);
                }
            }
        }

        internal Color LandOutlineColor
        {
            get { return _landOutlineColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(LandOutlineColor), ref _landOutlineColor, value);
                }
            }
        }

        internal Color LandBackgroundColor
        {
            get { return _landBackgroundColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(LandBackgroundColor), ref _landBackgroundColor, value);
                }
            }
        }

        internal Color CoastlineColor
        {
            get { return _coastlineColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(CoastlineColor), ref _coastlineColor, value);
                }
            }
        }

        internal int LandFormBrushSize
        {
            get { return _landformBrushSize; }
            set { SetPropertyField(nameof(LandFormBrushSize), ref _landformBrushSize, value); }
        }

        internal int LandformEraserSize
        {
            get { return _landformEraserSize; }
            set { SetPropertyField(nameof(LandformEraserSize), ref _landformEraserSize, value); }
        }

        internal int LandPaintBrushSize
        {
            get { return _landPaintBrushSize; }
            set { SetPropertyField(nameof(LandPaintBrushSize), ref _landPaintBrushSize, value); }
        }

        internal int LandPaintEraserSize
        {
            get { return _landPaintEraserSize; }
            set { SetPropertyField(nameof(LandPaintEraserSize), ref _landPaintEraserSize, value); }
        }

        internal int LandOutlineWidth
        {
            get { return _landOutlineWidth; }
            set { SetPropertyField(nameof(LandOutlineWidth), ref _landOutlineWidth, value); }
        }
        internal int CoastlineEffectDistance
        {
            get { return _coastlineEffectDistance; }
            set { SetPropertyField(nameof(CoastlineEffectDistance), ref _coastlineEffectDistance, value); }
        }

        internal bool UseTextureBackground
        {
            get { return _useTextureBackground; }
            set { SetPropertyField(nameof(UseTextureBackground), ref _useTextureBackground, value); }
        }

        internal int LandformTextureIndex
        {
            get { return _landformTextureIndex; }
            set { SetPropertyField(nameof(LandformTextureIndex), ref _landformTextureIndex, value); }
        }

        internal List<MapTexture> LandTextureList
        {
            get { return _landTextureList; }
        }

        public GeneratedLandformType LandformGenerationType
        {
            get { return _generatedLandformType; }
            set { SetPropertyField(nameof(LandformGenerationType), ref _generatedLandformType, value); }
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
            UpdateLandformUI(changedPropertyName);
            LandformManager.Update(MapStateMediator.CurrentMap, MapState, this);
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateLandformUI(string? changedPropertyName)
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.LandColorSelectionButton.BackColor = LandPaintColor;
                MainForm.CoastlineColorSelectionButton.BackColor = CoastlineColor;
                MainForm.LandformBackgroundColorSelectButton.BackColor = LandBackgroundColor;
                MainForm.LandformOutlineColorSelectButton.BackColor = LandOutlineColor;

                if (MapStateMediator.CurrentLandform != null)
                {
                    MapStateMediator.CurrentLandform.FillWithTexture = UseTextureBackground;
                }

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    if (changedPropertyName == "ShowLandformLayers")
                    {
                        MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDCOASTLINELAYER);
                        MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);
                        MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDDRAWINGLAYER);

                        landCoastlineLayer.ShowLayer = ShowLandformLayers;
                        landformLayer.ShowLayer = ShowLandformLayers;
                        landDrawingLayer.ShowLayer = ShowLandformLayers;
                    }
                    else if (changedPropertyName == "LandformTextureIndex")
                    {
                        UpdateLandTextureComboBox();
                    }
                    else if (changedPropertyName == "LandformGenerationType")
                    {
                        UncheckAllLandformTypeMenuItems();

                        switch (LandformGenerationType)
                        {
                            case GeneratedLandformType.Region:
                                {
                                    MainForm.RegionMenuItem.Checked = true;
                                }
                                break;
                            case GeneratedLandformType.Continent:
                                {
                                    MainForm.ContinentMenuItem.Checked = true;
                                }
                                break;
                            case GeneratedLandformType.Archipelago:
                                {
                                    MainForm.ArchipelagoMenuItem.Checked = true;
                                }
                                break;
                            case GeneratedLandformType.Island:
                                {
                                    MainForm.IslandMenuItem.Checked = true;
                                }
                                break;
                            case GeneratedLandformType.Atoll:
                                {
                                    MainForm.AtollMenuItem.Checked = true;
                                }
                                break;
                            case GeneratedLandformType.World:
                                {
                                    MainForm.WorldMenuItem.Checked = true;
                                }
                                break;
                        }
                    }
                }

            }));
        }

        #endregion

        #region Event Handlers
        private void LandformUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Landform UI Methods

        private void UpdateLandTextureComboBox()
        {
            if (LandformTextureIndex < 0)
            {
                LandformTextureIndex = 0;
            }

            if (LandformTextureIndex > LandTextureList.Count - 1)
            {
                LandformTextureIndex = LandTextureList.Count - 1;
            }

            if (LandTextureList[LandformTextureIndex].TextureBitmap == null)
            {
                LandTextureList[LandformTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(LandTextureList[LandformTextureIndex].TexturePath);
            }

            MainForm.LandformTexturePreviewPicture.Image = LandTextureList[LandformTextureIndex].TextureBitmap;
            MainForm.LandTextureNameLabel.Text = LandTextureList[LandformTextureIndex].TextureName;
        }

        internal void SetLandPaintColorFromCustomPresetButton(Button b)
        {
            if (b.Text != "")
            {
                Color landColor = b.BackColor;
                MainForm.LandColorSelectionButton.BackColor = landColor;
            }
        }

        internal void SetLandColorFromPreset(string htmlColor)
        {
            Color landColor = ColorTranslator.FromHtml(htmlColor);

            MainForm.LandColorSelectionButton.BackColor = landColor;
        }

        private void UncheckAllLandformTypeMenuItems()
        {
            MainForm.RegionMenuItem.Checked = false;
            MainForm.ContinentMenuItem.Checked = false;
            MainForm.IslandMenuItem.Checked = false;
            MainForm.ArchipelagoMenuItem.Checked = false;
            MainForm.AtollMenuItem.Checked = false;
            MainForm.WorldMenuItem.Checked = false;
        }

        #endregion
    }
}
