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
using SkiaSharp;

namespace RealmStudioX
{
    public sealed class LandformUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        private readonly List<MapTexture> _landTextureList = [];
        private int _landformTextureIndex;
        private string _landformTextureId = string.Empty;

        private bool _showLandformLayers = true;

        private ColorPaintBrush _landPaintBrush = ColorPaintBrush.SoftBrush;

        private float _landShadingDepth = 200F;

        private SKColor _landPaintColor = new(128, 230, 208, 171);
        private int _landformBrushSize = 64;
        private int _landformEraserSize = 64;
        private int _landPaintBrushSize = 20;
        private int _landPaintEraserSize = 20;
        private SKColor _landOutlineColor = new(62, 55, 40);
        private SKColor _landBackgroundColor = new(140, 180, 120);
        private SKColor _coastlineColor = new(187, 156, 195, 183);
        private LandformCoastlineStyle _coastlineStyle = LandformCoastlineStyle.DashPattern;
        private int _coastlineEffectDistance;
        private int _landOutlineWidth = 2;
        private bool _useTextureBackground = true;

        private SKColor _customColor1 = SKColors.White;
        private SKColor _customColor2 = SKColors.White;
        private SKColor _customColor3 = SKColors.White;
        private SKColor _customColor4 = SKColors.White;
        private SKColor _customColor5 = SKColors.White;
        private SKColor _customColor6 = SKColors.White;

        private GeneratedLandformType _generatedLandformType = GeneratedLandformType.Region;

        #region Property Setters/Getters

        internal bool ShowLandformLayers
        {
            get { return _showLandformLayers; }
            set { SetPropertyField(nameof(ShowLandformLayers), ref _showLandformLayers, value); }
        }

        internal ColorPaintBrush LandPaintBrush
        {
            get { return _landPaintBrush; }
            set { SetPropertyField(nameof(LandPaintBrush), ref _landPaintBrush, value); }
        }

        internal float LandShadingDepth
        {
            get { return _landShadingDepth; }
            set { SetPropertyField(nameof(LandShadingDepth), ref _landShadingDepth, value); }
        }

        internal SKColor LandPaintColor
        {
            get { return _landPaintColor; }
            set
            {
                if (value != SKColor.Empty)
                {
                    SetPropertyField(nameof(LandPaintColor), ref _landPaintColor, value);
                }
            }
        }

        internal SKColor LandOutlineColor
        {
            get { return _landOutlineColor; }
            set
            {
                if (value != SKColor.Empty)
                {
                    SetPropertyField(nameof(LandOutlineColor), ref _landOutlineColor, value);
                }
            }
        }

        internal SKColor LandBackgroundColor
        {
            get { return _landBackgroundColor; }
            set
            {
                if (value != SKColor.Empty)
                {
                    SetPropertyField(nameof(LandBackgroundColor), ref _landBackgroundColor, value);
                }
            }
        }

        internal SKColor CoastlineColor
        {
            get { return _coastlineColor; }
            set
            {
                if (value != SKColor.Empty)
                {
                    SetPropertyField(nameof(CoastlineColor), ref _coastlineColor, value);
                }
            }
        }

        internal LandformCoastlineStyle CoastlineStyle
        {
            get { return _coastlineStyle; }
            set { SetPropertyField(nameof(CoastlineStyle), ref _coastlineStyle, value); }
        }

        internal int LandformBrushSize
        {
            get { return _landformBrushSize; }
            set { SetPropertyField(nameof(LandformBrushSize), ref _landformBrushSize, value); }
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

        internal string LandformTextureId
        {
            get { return _landformTextureId; }
            set { SetPropertyField(nameof(LandformTextureId), ref _landformTextureId, value); }
        }

        internal GeneratedLandformType LandformGenerationType
        {
            get { return _generatedLandformType; }
            set { SetPropertyField(nameof(LandformGenerationType), ref _generatedLandformType, value); }
        }

        public SKColor CustomColor1
        {
            get { return _customColor1; }
            set { SetPropertyField(nameof(CustomColor1), ref _customColor1, value); }
        }

        public SKColor CustomColor2
        {
            get { return _customColor2; }
            set { SetPropertyField(nameof(CustomColor2), ref _customColor2, value); }
        }

        public SKColor CustomColor3
        {
            get { return _customColor3; }
            set { SetPropertyField(nameof(CustomColor3), ref _customColor3, value); }
        }

        public SKColor CustomColor4
        {
            get { return _customColor4; }
            set { SetPropertyField(nameof(CustomColor4), ref _customColor4, value); }
        }

        public SKColor CustomColor5
        {
            get { return _customColor5; }
            set { SetPropertyField(nameof(CustomColor5), ref _customColor5, value); }
        }

        public SKColor CustomColor6
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

        #endregion

        /*
        private void UpdateLandformUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.LandColorSelectionButton.BackColor = LandPaintColor;
                MainForm.CoastlineColorSelectionButton.BackColor = CoastlineColor;
                MainForm.LandformBackgroundColorSelectButton.BackColor = LandBackgroundColor;
                MainForm.LandformOutlineColorSelectButton.BackColor = LandOutlineColor;

                if (LandPaintBrush == ColorPaintBrush.SoftBrush)
                {
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (LandPaintBrush == ColorPaintBrush.HardBrush)
                {
                    MainForm.LandHardBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.LandHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (LandPaintBrush == ColorPaintBrush.PatternBrush1)
                {
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (LandPaintBrush == ColorPaintBrush.PatternBrush2)
                {
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (LandPaintBrush == ColorPaintBrush.PatternBrush3)
                {
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (LandPaintBrush == ColorPaintBrush.PatternBrush4)
                {
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.LandPatternBrush4Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.LandPatternBrush3Button.FlatAppearance.BorderSize = 3;
                }


                MapStateMediator.SelectedColorPaintBrush = LandPaintBrush;

                MapStateMediator.MainUIMediator.SetDrawingModeLabel();

                if (MapStateMediator.CurrentLandform != null)
                {
                    MapStateMediator.CurrentLandform.Shading.FillWithTexture = UseTextureBackground;
                }

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    switch (changedPropertyName)
                    {
                        case "ShowLandformLayers":
                            {
                                //MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDCOASTLINELAYER);
                                //MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);
                                //MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDDRAWINGLAYER);

                                //landCoastlineLayer.ShowLayer = ShowLandformLayers;
                                //landformLayer.ShowLayer = ShowLandformLayers;
                                //landDrawingLayer.ShowLayer = ShowLandformLayers;
                            }
                            break;
                        case "LandformTextureIndex":
                            {
                                UpdateLandTexture();
                            }
                            break;
                        case "LandformGenerationType":
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
                            break;
                        case "CoastlineStyle":
                            {
                                for (int i = 0; i < MainForm.CoastlineStyleList.Items.Count; i++)
                                {
                                    if (MainForm.CoastlineStyleList.Items[i].ToString() == CoastlineStyle.GetDescription())
                                    {
                                        MainForm.CoastlineStyleList.SelectedIndex = i;
                                        break;
                                    }
                                }
                            }
                            break;
                        case "CustomColor1":
                            {
                                MainForm.LandCustomColorButton1.BackColor = CustomColor1;
                                MainForm.LandCustomColorButton1.ForeColor = SystemColors.ControlDark;
                                MainForm.LandCustomColorButton1.Text = (CustomColor1.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor1);
                            }
                            break;
                        case "CustomColor2":
                            {
                                MainForm.LandCustomColorButton2.BackColor = CustomColor2;
                                MainForm.LandCustomColorButton2.ForeColor = SystemColors.ControlDark;
                                MainForm.LandCustomColorButton2.Text = (CustomColor2.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor2);
                            }
                            break;
                        case "CustomColor3":
                            {
                                MainForm.LandCustomColorButton3.BackColor = CustomColor3;
                                MainForm.LandCustomColorButton3.ForeColor = SystemColors.ControlDark;
                                MainForm.LandCustomColorButton3.Text = (CustomColor3.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor3);
                            }
                            break;
                        case "CustomColor4":
                            {
                                MainForm.LandCustomColorButton4.BackColor = CustomColor4;
                                MainForm.LandCustomColorButton4.ForeColor = SystemColors.ControlDark;
                                MainForm.LandCustomColorButton4.Text = (CustomColor4.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor4);
                            }
                            break;
                        case "CustomColor5":
                            {
                                MainForm.LandCustomColorButton5.BackColor = CustomColor5;
                                MainForm.LandCustomColorButton5.ForeColor = SystemColors.ControlDark;
                                MainForm.LandCustomColorButton5.Text = (CustomColor5.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor5);
                            }
                            break;
                        case "CustomColor6":
                            {
                                MainForm.LandCustomColorButton6.BackColor = CustomColor6;
                                MainForm.LandCustomColorButton6.ForeColor = SystemColors.ControlDark;
                                MainForm.LandCustomColorButton6.Text = (CustomColor6.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor6);
                            }
                            break;
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

        internal void Reset()
        {
            LandformTextureIndex = 0;

            ShowLandformLayers = true;

            LandPaintBrush = ColorPaintBrush.SoftBrush;

            LandPaintColor = Color.FromArgb(128, 230, 208, 171);
            LandformBrushSize = 64;
            LandformEraserSize = 64;
            LandPaintBrushSize = 20;
            LandPaintEraserSize = 20;
            LandOutlineColor = Color.FromArgb(62, 55, 40);
            LandBackgroundColor = Color.White;
            CoastlineColor = Color.FromArgb(187, 156, 195, 183);
            CoastlineStyle = LandformCoastlineStyle.DashPattern;
            CoastlineEffectDistance = 16;
            LandOutlineWidth = 2;
            UseTextureBackground = true;

            CustomColor1 = Color.White;
            CustomColor2 = Color.White;
            CustomColor3 = Color.White;
            CustomColor4 = Color.White;
            CustomColor5 = Color.White;
            CustomColor6 = Color.White;
        }

        private void UpdateLandTexture()
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

        internal void SetLandColorFromPreset(string htmlColor)
        {
            Color landColor = ColorTranslator.FromHtml(htmlColor);
            LandPaintColor = landColor;
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

        */
    }
}
