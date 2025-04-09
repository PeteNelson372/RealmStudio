using System.ComponentModel;

namespace RealmStudio
{
    internal sealed class OceanUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private readonly List<MapTexture> _oceanTextureList = [];
        private int _oceanTextureIndex;

        private bool _showOceanLayers = true;

        private ColorPaintBrush _oceanPaintBrush = ColorPaintBrush.SoftBrush;

        private int _oceanPaintBrushSize = 64;
        private int _oceanPaintEraserSize = 64;

        private float _oceanBrushVelocity = 1.0F;
        private Color _oceanPaintColor = Color.FromArgb(128, 145, 203, 184);
        private Color _oceanFillColor = Color.White;
        private float _oceanTextureScale = 100.0F;
        private float _oceanTextureOpacity = 100.0f;
        private bool _mirrorOceanTexture;

        private Color _customColor1 = Color.White;
        private Color _customColor2 = Color.White;
        private Color _customColor3 = Color.White;
        private Color _customColor4 = Color.White;
        private Color _customColor5 = Color.White;
        private Color _customColor6 = Color.White;
        private Color _customColor7 = Color.White;
        private Color _customColor8 = Color.White;

        public OceanUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += OceanUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        internal bool ShowOceanLayers
        {
            get { return _showOceanLayers; }
            set { SetPropertyField(nameof(ShowOceanLayers), ref _showOceanLayers, value); }
        }

        internal int OceanTextureIndex
        {
            get { return _oceanTextureIndex; }
            set { SetPropertyField(nameof(OceanTextureIndex), ref _oceanTextureIndex, value); }
        }

        internal List<MapTexture> OceanTextureList
        {
            get { return _oceanTextureList; }
        }

        internal ColorPaintBrush OceanPaintBrush
        {
            get { return _oceanPaintBrush; }
            set { SetPropertyField(nameof(OceanPaintBrush), ref _oceanPaintBrush, value); }
        }

        internal int OceanPaintBrushSize
        {
            get { return _oceanPaintBrushSize; }
            set { SetPropertyField(nameof(OceanPaintBrushSize), ref _oceanPaintBrushSize, value); }
        }

        internal int OceanPaintEraserSize
        {
            get { return _oceanPaintEraserSize; }
            set { SetPropertyField(nameof(OceanPaintEraserSize), ref _oceanPaintEraserSize, value); }
        }

        internal float OceanBrushVelocity
        {
            get { return _oceanBrushVelocity; }
            set { SetPropertyField(nameof(OceanBrushVelocity), ref _oceanBrushVelocity, value); }
        }

        internal Color OceanPaintColor
        {
            get { return _oceanPaintColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(OceanPaintColor), ref _oceanPaintColor, value);
                }
            }
        }

        internal Color OceanFillColor
        {
            get { return _oceanFillColor; }
            set
            {
                if (value != Color.Empty)
                {
                    SetPropertyField(nameof(OceanFillColor), ref _oceanFillColor, value);
                }
            }
        }

        internal float OceanTextureScale
        {
            get { return _oceanTextureScale; }
            set { SetPropertyField(nameof(OceanTextureScale), ref _oceanTextureScale, value); }
        }

        internal float OceanTextureOpacity
        {
            get { return _oceanTextureOpacity; }
            set { SetPropertyField(nameof(OceanTextureOpacity), ref _oceanTextureOpacity, value); }
        }

        internal bool MirrorOceanTexture
        {
            get { return _mirrorOceanTexture; }
            set { SetPropertyField(nameof(MirrorOceanTexture), ref _mirrorOceanTexture, value); }
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
            UpdateOceanUI(changedPropertyName);
            OceanManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateOceanUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.OceanPaintColorSelectButton.BackColor = OceanPaintColor;
                MainForm.OceanColorSelectButton.BackColor = OceanFillColor;
                MainForm.MirrorOceanTextureSwitch.Checked = MirrorOceanTexture;

                if (OceanPaintBrush == ColorPaintBrush.SoftBrush)
                {
                    MainForm.OceanSoftBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.OceanSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.OceanHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.OceanHardBrushButton.FlatAppearance.BorderSize = 3;
                }
                else
                {
                    MainForm.OceanHardBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.OceanHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.OceanSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.OceanSoftBrushButton.FlatAppearance.BorderSize = 3;
                }

                MapStateMediator.SelectedColorPaintBrush = OceanPaintBrush;

                MapStateMediator.MainUIMediator.SetDrawingModeLabel();

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    switch (changedPropertyName)
                    {
                        case "ShowOceanLayers":
                            {
                                MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);
                                MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
                                MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANDRAWINGLAYER);

                                oceanTextureLayer.ShowLayer = ShowOceanLayers;
                                oceanTextureOverlayLayer.ShowLayer = ShowOceanLayers;
                                oceanDrawingLayer.ShowLayer = ShowOceanLayers;
                            }
                            break;
                        case "OceanTextureIndex":
                            {
                                UpdateOceanTextureComboBox();
                            }
                            break;
                        case "App_OceanTextureIndex":
                            {
                                UpdateOceanTextureComboBox();
                            }
                            break;
                        case "CustomColor1":
                            {
                                MainForm.OceanCustomColorButton1.BackColor = CustomColor1;
                                MainForm.OceanCustomColorButton1.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton1.Text = (CustomColor1.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor1);
                            }
                            break;
                        case "CustomColor2":
                            {
                                MainForm.OceanCustomColorButton2.BackColor = CustomColor2;
                                MainForm.OceanCustomColorButton2.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton2.Text = (CustomColor2.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor2);
                            }
                            break;
                        case "CustomColor3":
                            {
                                MainForm.OceanCustomColorButton3.BackColor = CustomColor3;
                                MainForm.OceanCustomColorButton3.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton3.Text = (CustomColor3.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor3);
                            }
                            break;
                        case "CustomColor4":
                            {
                                MainForm.OceanCustomColorButton4.BackColor = CustomColor4;
                                MainForm.OceanCustomColorButton4.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton4.Text = (CustomColor4.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor4);
                            }
                            break;
                        case "CustomColor5":
                            {
                                MainForm.OceanCustomColorButton5.BackColor = CustomColor5;
                                MainForm.OceanCustomColorButton5.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton5.Text = (CustomColor5.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor5);
                            }
                            break;
                        case "CustomColor6":
                            {
                                MainForm.OceanCustomColorButton6.BackColor = CustomColor6;
                                MainForm.OceanCustomColorButton6.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton6.Text = (CustomColor6.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor6);
                            }
                            break;
                        case "CustomColor7":
                            {
                                MainForm.OceanCustomColorButton7.BackColor = CustomColor7;
                                MainForm.OceanCustomColorButton7.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton7.Text = (CustomColor7.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor7);
                            }
                            break;
                        case "CustomColor8":
                            {
                                MainForm.OceanCustomColorButton8.BackColor = CustomColor8;
                                MainForm.OceanCustomColorButton8.ForeColor = SystemColors.ControlDark;
                                MainForm.OceanCustomColorButton8.Text = (CustomColor8.ToArgb() == Color.White.ToArgb()) ? "" : ColorTranslator.ToHtml(CustomColor8);
                            }
                            break;
                    }
                }
            }));
        }

        #endregion

        #region Event Handlers
        private void OceanUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Ocean UI Methods

        internal void Initialize(int oceanTextureIndex, float textureScale, float textureOpacity, bool mirrorTexture)
        {
            _oceanTextureIndex = oceanTextureIndex;
            _oceanTextureScale = textureScale;
            _oceanTextureOpacity = textureOpacity;
            _mirrorOceanTexture = mirrorTexture;

            MainForm.OceanScaleTextureTrack.Value = (int)_oceanTextureScale;
            MainForm.OceanTextureOpacityTrack.Value = (int)_oceanTextureOpacity;
            MainForm.MirrorOceanTextureSwitch.Checked = _mirrorOceanTexture;

            UpdateOceanTextureComboBox();
        }

        internal void Reset()
        {
            OceanTextureIndex = 0;
            OceanPaintBrush = ColorPaintBrush.SoftBrush;
            OceanPaintBrushSize = 64;
            OceanPaintEraserSize = 64;
            OceanBrushVelocity = 1.0F;
            OceanPaintColor = Color.FromArgb(128, 145, 203, 184);
            OceanFillColor = Color.White;
            OceanTextureScale = 100.0F;
            OceanTextureOpacity = 100.0f;
            MirrorOceanTexture = false;
            CustomColor1 = Color.White;
            CustomColor2 = Color.White;
            CustomColor3 = Color.White;
            CustomColor4 = Color.White;
            CustomColor5 = Color.White;
            CustomColor6 = Color.White;
            CustomColor7 = Color.White;
            CustomColor8 = Color.White;
        }

        private void UpdateOceanTextureComboBox()
        {
            if (OceanTextureIndex < 0)
            {
                OceanTextureIndex = 0;
            }

            if (OceanTextureIndex > OceanTextureList.Count - 1)
            {
                OceanTextureIndex = OceanTextureList.Count - 1;
            }

            if (OceanTextureList[OceanTextureIndex].TextureBitmap == null)
            {
                OceanTextureList[OceanTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(OceanTextureList[OceanTextureIndex].TexturePath);
            }

            if (OceanTextureList[OceanTextureIndex].TextureBitmap != null)
            {
                Bitmap? textureBitmap = OceanTextureList[OceanTextureIndex].TextureBitmap;
                if (textureBitmap != null)
                {
                    Bitmap b = new(textureBitmap);

                    Bitmap newB = DrawingMethods.SetBitmapOpacity(b, OceanTextureOpacity / 100.0F);

                    MainForm.OceanTextureBox.Image = newB;
                    MainForm.OceanTextureBox.Refresh();

                    MainForm.OceanTextureNameLabel.Text = OceanTextureList[OceanTextureIndex].TextureName;
                    MainForm.OceanTextureNameLabel.Refresh();
                }
            }
        }

        internal void SetOceanColorFromPreset(string htmlColor)
        {
            Color oceanColor = ColorTranslator.FromHtml(htmlColor);
            OceanPaintColor = oceanColor;
        }

        #endregion
    }
}
