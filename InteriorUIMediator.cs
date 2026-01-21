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
    internal sealed class InteriorUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;

        private readonly List<MapTexture> _interiorWallTextureList = [];
        private int _interiorWallTextureIndex;

        private readonly List<MapTexture> _interiorFloorTextureList = [];
        private int _interiorFloorTextureIndex;

        private bool _showInteriorLayers = true;

        private ColorPaintBrush _interiorPaintBrush = ColorPaintBrush.SoftBrush;

        private Color _interiorPaintColor = Color.FromArgb(128, 230, 208, 171);
        private int _interiorBrushSize = 64;
        private int _interiorEraserSize = 64;
        private int _interiorPaintBrushSize = 20;
        private int _interiorPaintEraserSize = 20;
        private Color _wallOutlineColor = Color.FromArgb(62, 55, 40);
        private Color _wallBackgroundColor = Color.White;
        private int _wallThickness = 4;
        private int _wallOutlineWidth = 2;
        private bool _useTextureBackground = true;

        private Color _customColor1 = Color.White;
        private Color _customColor2 = Color.White;
        private Color _customColor3 = Color.White;
        private Color _customColor4 = Color.White;
        private Color _customColor5 = Color.White;
        private Color _customColor6 = Color.White;

        public InteriorUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += InteriorUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        internal bool ShowInteriorLayers
        {
            get { return _showInteriorLayers; }
            set { SetPropertyField(nameof(ShowInteriorLayers), ref _showInteriorLayers, value); }
        }

        internal ColorPaintBrush InteriorPaintBrush
        {
            get { return _interiorPaintBrush; }
            set { SetPropertyField(nameof(InteriorPaintBrush), ref _interiorPaintBrush, value); }
        }

        internal Color InteriorPaintColor
        {
            get { return _interiorPaintColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(InteriorPaintColor), ref _interiorPaintColor, value);
                }
            }
        }

        internal int InteriorBrushSize
        {
            get { return _interiorBrushSize; }
            set { SetPropertyField(nameof(InteriorBrushSize), ref _interiorBrushSize, value); }
        }

        internal Color WallOutlineColor
        {
            get { return _wallOutlineColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(WallOutlineColor), ref _wallOutlineColor, value);
                }
            }
        }

        internal Color WallBackgroundColor
        {
            get { return _wallBackgroundColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(WallBackgroundColor), ref _wallBackgroundColor, value);
                }
            }
        }

        internal int WallThickness
        {
            get { return _wallThickness; }
            set { SetPropertyField(nameof(WallThickness), ref _wallThickness, value); }
        }

        internal int WallOutlineWidth
        {
            get { return _wallOutlineWidth; }
            set { SetPropertyField(nameof(WallOutlineWidth), ref _wallOutlineWidth, value); }
        }

        internal bool UseTextureBackground
        {
            get { return _useTextureBackground; }
            set { SetPropertyField(nameof(UseTextureBackground), ref _useTextureBackground, value); }
        }

        internal int InteriorFloorTextureIndex
        {
            get { return _interiorFloorTextureIndex; }
            set { SetPropertyField(nameof(InteriorFloorTextureIndex), ref _interiorFloorTextureIndex, value); }
        }

        internal List<MapTexture> InteriorFloorTextureList
        {
            get { return _interiorFloorTextureList; }
        }


        internal int InteriorWallTextureIndex
        {
            get { return _interiorWallTextureIndex; }
            set { SetPropertyField(nameof(InteriorWallTextureIndex), ref _interiorWallTextureIndex, value); }
        }

        internal List<MapTexture> InteriorWallTextureList
        {
            get { return _interiorWallTextureList; }
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
            LandformManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateLandformUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MainForm.Invoke(new MethodInvoker(delegate ()
            {

                MapStateMediator.MainUIMediator.SetDrawingModeLabel();

                if (MapStateMediator.CurrentLandform != null)
                {
                    MapStateMediator.CurrentLandform.FillWithTexture = UseTextureBackground;
                }

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    switch (changedPropertyName)
                    {
                        case "ShowInteriorLayers":
                            {
                                if (MapStateMediator.CurrentMap != null)
                                {
                                    MapLayer interiorOutlineLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIOROUTLINELAYER);
                                    MapLayer interiorLayerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER);
                                    MapLayer interiorDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORDRAWINGLAYER);

                                    interiorOutlineLayer.ShowLayer = ShowInteriorLayers;
                                    interiorLayerLayer.ShowLayer = ShowInteriorLayers;
                                    interiorDrawingLayer.ShowLayer = ShowInteriorLayers;
                                }
                            }
                            break;
                        case "InteriorFloorTextureIndex":
                            {
                                UpdateInteriorFloorTexture();
                            }
                            break;
                    }
                }

            }));
        }

        #endregion

        #region Event Handlers
        private void InteriorUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Interior UI Methods

        internal void Reset()
        {
            InteriorWallTextureIndex = 0;
            InteriorFloorTextureIndex = 0;

            ShowInteriorLayers = true;

            InteriorPaintBrush = ColorPaintBrush.SoftBrush;


            UseTextureBackground = true;

            CustomColor1 = Color.White;
            CustomColor2 = Color.White;
            CustomColor3 = Color.White;
            CustomColor4 = Color.White;
            CustomColor5 = Color.White;
            CustomColor6 = Color.White;
        }

        private void UpdateInteriorFloorTexture()
        {
            if (InteriorFloorTextureIndex < 0)
            {
                InteriorFloorTextureIndex = 0;
            }

            if (InteriorFloorTextureIndex > InteriorFloorTextureList.Count - 1)
            {
                InteriorFloorTextureIndex = InteriorFloorTextureList.Count - 1;
            }

            if (InteriorFloorTextureList[InteriorFloorTextureIndex].TextureBitmap == null)
            {
                InteriorFloorTextureList[InteriorFloorTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(InteriorFloorTextureList[InteriorFloorTextureIndex].TexturePath);
            }

            MainForm.InteriorFloorTexturePreviewPicture.Image = InteriorFloorTextureList[InteriorFloorTextureIndex].TextureBitmap;
            MainForm.InteriorFloorTextureNameLabel.Text = InteriorFloorTextureList[InteriorFloorTextureIndex].TextureName;
        }


        #endregion
    }
}
