using System.ComponentModel;

namespace RealmStudio
{
    internal class OceanUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private readonly List<MapTexture> _oceanTextureList = [];
        private int _oceanTextureIndex;

        private int _oceanPaintBrushSize = 64;
        private int _oceanPaintEraserSize = 64;

        private float _oceanBrushVelocity = 1.0F;
        private Color _oceanPaintColor = Color.FromArgb(128, 145, 203, 184);
        private Color _oceanFillColor = Color.White;
        private float _oceanTextureScale = 1.0F;
        private float _oceanTextureOpacity = 1.0f;
        private bool _mirrorOceanTexture;

        private bool _showOceanLayers = true;

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
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.OceanPaintColorSelectButton.BackColor = OceanPaintColor;
                MainForm.OceanColorSelectButton.BackColor = OceanFillColor;
                MainForm.MirrorOceanTextureSwitch.Checked = MirrorOceanTexture;

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    if (changedPropertyName == "ShowOceanLayers")
                    {
                        MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);
                        MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
                        MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANDRAWINGLAYER);

                        oceanTextureLayer.ShowLayer = ShowOceanLayers;
                        oceanTextureOverlayLayer.ShowLayer = ShowOceanLayers;
                        oceanDrawingLayer.ShowLayer = ShowOceanLayers;
                    }
                    else if (changedPropertyName == "OceanTextureIndex")
                    {
                        UpdateOceanTextureComboBox();
                    }
                    else if (changedPropertyName == "OceanTextureOpacity")
                    {
                        MainForm.OceanTextureOpacityTrack.Value = (int)OceanTextureOpacity;
                        UpdateOceanTextureComboBox();
                    }
                    else if (changedPropertyName == "OceanTextureScale")
                    {
                        MainForm.OceanScaleTextureTrack.Value = (int)OceanTextureScale;
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

                    MainForm.OceanTextureBox.Image = b;
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
