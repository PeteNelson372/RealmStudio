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
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    internal sealed class InteriorUIMediator : UiMediatorBase, IUIMediatorObserver
    {
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
        private Color _wallOutlineColor = ColorTranslator.FromHtml("#4B311A");
        private Color _wallBackgroundColor = ColorTranslator.FromHtml("#4B311A");
        private int _wallThickness = 4;
        private int _wallOutlineWidth = 2;

        private float _wallTextureScale = 1.0f;

        private bool _useTextureBackground = true;

        private bool _showAlignmentGrid;
        private int _alignmentGridSize = 12;
        private bool _alignToGrid;

        private Color _customColor1 = Color.White;
        private Color _customColor2 = Color.White;
        private Color _customColor3 = Color.White;
        private Color _customColor4 = Color.White;
        private Color _customColor5 = Color.White;
        private Color _customColor6 = Color.White;


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

        internal float WallTextureScale
        {
            get { return _wallTextureScale; }
            set { SetPropertyField(nameof(WallTextureScale), ref _wallTextureScale, value); }
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

        internal bool ShowAlignmentGrid
        {
            get { return _showAlignmentGrid; }
            set { SetPropertyField(nameof(ShowAlignmentGrid), ref _showAlignmentGrid, value); }
        }

        internal int AlignmentGridSize
        {
            get { return _alignmentGridSize; }
            set { SetPropertyField(nameof(AlignmentGridSize), ref _alignmentGridSize, value); }
        }

        internal bool AlignToGrid
        {
            get { return _alignToGrid; }
            set { SetPropertyField(nameof(AlignToGrid), ref _alignToGrid, value); }
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

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                RaiseChanged();
            }
        }


        /*
        private void UpdateInteriorUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.GridUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MainForm.Invoke(new MethodInvoker(delegate ()
            {

                MapStateMediator.MainUIMediator.SetDrawingModeLabel();

                if (MapStateMediator.CurrentLandform != null)
                {
                    MapStateMediator.CurrentLandform.Shading.FillWithTexture = UseTextureBackground;
                }

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    switch (changedPropertyName)
                    {
                        case "ShowInteriorLayers":
                            {
                                if (MapStateMediator.CurrentMap != null)
                                {
                                    //MapLayer interiorOutlineLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIOROUTLINELAYER);
                                    //MapLayer interiorLayerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORLAYER);
                                    //MapLayer interiorDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.INTERIORDRAWINGLAYER);

                                    //interiorOutlineLayer.ShowLayer = ShowInteriorLayers;
                                    //interiorLayerLayer.ShowLayer = ShowInteriorLayers;
                                    //interiorDrawingLayer.ShowLayer = ShowInteriorLayers;
                                }
                            }
                            break;
                        case "InteriorFloorTextureIndex":
                            {
                                UpdateInteriorFloorTexture();
                            }
                            break;
                        case "InteriorWallTextureIndex":
                            {
                                UpdateInteriorWallTexture();
                            }
                            break;
                        case "ShowAlignmentGrid":
                            {
                                MapStateMediator.MainUIMediator.SelectedBrushSize = AlignmentGridSize;

                                
                                MapLayer gridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER);

                                if (ShowAlignmentGrid)
                                {
                                    gridLayer.ShowLayer = true;
                                    AlignmentGrid alignmentGrid = new()
                                    {
                                        GridSize = AlignmentGridSize,
                                        GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER,
                                        GridEnabled = true,
                                        ParentMap = MapStateMediator.CurrentMap,
                                        Height = MapStateMediator.CurrentMap.MapHeight,
                                        Width = MapStateMediator.CurrentMap.MapWidth
                                    };

                                    gridLayer.MapLayerComponents.Add(alignmentGrid);
                                }
                                else
                                {
                                    for (int i = gridLayer.MapLayerComponents.Count - 1; i >= 0; i--)
                                    {
                                        if (gridLayer.MapLayerComponents[i] is AlignmentGrid)
                                        {
                                            gridLayer.MapLayerComponents.RemoveAt(i);
                                        }
                                    }

                                    gridLayer.ShowLayer = MapStateMediator.GridUIMediator.GridEnabled;
                                }

                                MainForm.SKGLRenderControl.Invalidate();

                                
                            }
                            break;
                        case "AlignToGrid":
                            {
                                MapStateMediator.MainUIMediator.SelectedBrushSize = AlignmentGridSize;
                            }
                            break;
                        case "AlignmentGridSize":
                            {
                                
                                MapLayer gridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER);

                                MapStateMediator.MainUIMediator.SelectedBrushSize = AlignmentGridSize;

                                foreach (IMapComponent component in gridLayer.MapLayerComponents)
                                {
                                    if (component is AlignmentGrid alignmentGrid)
                                    {
                                        alignmentGrid.GridSize = AlignmentGridSize;
                                    }
                                }
                                MainForm.SKGLRenderControl.Invalidate();
                                
                            }
                            break;
                    }
                }

            }));
        }

        */

        #endregion

        #region Interior UI Methods
        /*
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

        private void UpdateInteriorWallTexture()
        {
            if (InteriorWallTextureIndex < 0)
            {
                InteriorWallTextureIndex = 0;
            }
            if (InteriorWallTextureIndex > InteriorWallTextureList.Count - 1)
            {
                InteriorWallTextureIndex = InteriorWallTextureList.Count - 1;
            }
            if (InteriorWallTextureList[InteriorWallTextureIndex].TextureBitmap == null)
            {
                InteriorWallTextureList[InteriorWallTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(InteriorWallTextureList[InteriorWallTextureIndex].TexturePath);
            }
            MainForm.WallTexturePreviewPicture.Image = InteriorWallTextureList[InteriorWallTextureIndex].TextureBitmap;
            MainForm.WallTextureNameLabel.Text = InteriorWallTextureList[InteriorWallTextureIndex].TextureName;
        }

        */

        #endregion
    }
}
