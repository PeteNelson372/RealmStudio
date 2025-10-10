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
    internal sealed class DrawingUIMediator : IUIMediatorObserver, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;

        private int _drawingLineBrushSize = 4;
        private Color _drawingLineColor = Color.Black;
        private Color _drawingFillColor = Color.White;
        private bool _fillDrawnShape;
        private List<SKPoint> _linePoints = [];
        private List<SKPoint> _paintPoints = [];

        private readonly List<MapTexture> _drawingTextureList = [];
        private int _drawingTextureIndex;
        private float _drawingTextureScale = 1.0F;
        private float _drawingTextureOpacity = 1.0f;

        private DrawingFillType _fillType = DrawingFillType.Color;

        private bool _showDrawingLayer;

        private SKPaint _fillPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Butt
        };

        private Bitmap? _drawingStampBitmap;

        private ColorPaintBrush _drawingPaintBrush = ColorPaintBrush.SoftBrush;
        private MapBrush? _drawingMapBrush;

        private float _drawingStampOpacity = 1.0f;
        private float _drawingStampScale = 1.0f;
        private float _drawingStampRotation;

        private float _drawingShapeRotation;

        private bool disposedValue;

        public DrawingUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += DrawingUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        internal int DrawingLineBrushSize
        {
            get { return _drawingLineBrushSize; }
            set { SetPropertyField(nameof(DrawingLineBrushSize), ref _drawingLineBrushSize, value); }
        }

        internal Color DrawingLineColor
        {
            get { return _drawingLineColor; }
            set { SetPropertyField(nameof(DrawingLineColor), ref _drawingLineColor, value); }
        }

        internal Color DrawingFillColor
        {
            get { return _drawingFillColor; }
            set { SetPropertyField(nameof(DrawingFillColor), ref _drawingFillColor, value); }
        }

        internal bool FillDrawnShape
        {
            get { return _fillDrawnShape; }
            set { SetPropertyField(nameof(FillDrawnShape), ref _fillDrawnShape, value); }
        }

        internal List<SKPoint> LinePoints
        {
            get { return _linePoints; }
            set { SetPropertyField(nameof(LinePoints), ref _linePoints, value); }
        }

        internal List<SKPoint> PaintPoints
        {
            get { return _paintPoints; }
            set { SetPropertyField(nameof(PaintPoints), ref _paintPoints, value); }
        }

        internal ColorPaintBrush DrawingPaintBrush
        {
            get { return _drawingPaintBrush; }
            set { SetPropertyField(nameof(DrawingPaintBrush), ref _drawingPaintBrush, value); }
        }

        internal MapBrush? DrawingMapBrush
        {
            get { return _drawingMapBrush; }
            set { SetPropertyField(nameof(DrawingMapBrush), ref _drawingMapBrush, value); }
        }

        internal List<MapTexture> DrawingTextureList
        {
            get { return _drawingTextureList; }
        }

        internal int DrawingTextureIndex
        {
            get { return _drawingTextureIndex; }
            set { SetPropertyField(nameof(DrawingTextureIndex), ref _drawingTextureIndex, value); }
        }

        internal float DrawingTextureScale
        {
            get { return _drawingTextureScale; }
            set { SetPropertyField(nameof(DrawingTextureScale), ref _drawingTextureScale, value); }
        }

        internal float DrawingTextureOpacity
        {
            get { return _drawingTextureOpacity; }
            set { SetPropertyField(nameof(DrawingTextureOpacity), ref _drawingTextureOpacity, value); }
        }

        internal DrawingFillType FillType
        {
            get { return _fillType; }
            set { SetPropertyField(nameof(FillType), ref _fillType, value); }
        }

        internal SKPaint FillPaint
        {
            get { return _fillPaint; }
            set { SetPropertyField(nameof(FillPaint), ref _fillPaint, value); }
        }

        internal Bitmap? DrawingStampBitmap
        {
            get { return _drawingStampBitmap; }
            set { SetPropertyField(nameof(DrawingStampBitmap), ref _drawingStampBitmap, value); }
        }

        internal float DrawingStampOpacity
        {
            get { return _drawingStampOpacity; }
            set { SetPropertyField(nameof(DrawingStampOpacity), ref _drawingStampOpacity, value); }
        }

        internal float DrawingStampScale
        {
            get { return _drawingStampScale; }
            set { SetPropertyField(nameof(DrawingStampScale), ref _drawingStampScale, value); }
        }

        internal float DrawingStampRotation
        {
            get { return _drawingStampRotation; }
            set { SetPropertyField(nameof(DrawingStampRotation), ref _drawingStampRotation, value); }
        }

        internal float DrawingShapeRotation
        {
            get { return _drawingShapeRotation; }
            set { SetPropertyField(nameof(DrawingShapeRotation), ref _drawingShapeRotation, value); }
        }

        internal bool ShowDrawingLayer
        {
            get { return _showDrawingLayer; }
            set { SetPropertyField(nameof(ShowDrawingLayer), ref _showDrawingLayer, value); }
        }

        #endregion

        #region Event Handlers
        private void DrawingUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
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
            UpdateDrawingUI(changedPropertyName);
            DrawingManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateDrawingUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MainForm.SelectPaintColorButton.BackColor = DrawingLineColor;
            MainForm.SelectFillColorButton.BackColor = DrawingFillColor;

            if (DrawingPaintBrush == ColorPaintBrush.PatternBrush1)
            {
                DrawingMapBrush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush1");
            }
            else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush2)
            {
                DrawingMapBrush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush2");
            }
            else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush3)
            {
                DrawingMapBrush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush3");
            }
            else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush4)
            {
                DrawingMapBrush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush4"); ;
            }

            if (DrawingPaintBrush == ColorPaintBrush.SoftBrush)
            {
                _drawingMapBrush = new()
                {
                    BrushName = "Soft Brush",
                    BrushBitmap = DrawingTextureList[DrawingTextureIndex].TextureBitmap,
                    BrushPath = DrawingTextureList[DrawingTextureIndex].TexturePath,
                };
            }
            else if (DrawingPaintBrush == ColorPaintBrush.HardBrush)
            {
                _drawingMapBrush = new()
                {
                    BrushName = "Hard Brush",
                    BrushBitmap = DrawingTextureList[DrawingTextureIndex].TextureBitmap,
                    BrushPath = DrawingTextureList[DrawingTextureIndex].TexturePath,
                };
            }

            if (DrawingMapBrush != null && DrawingMapBrush.BrushBitmap == null)
            {
                DrawingMapBrush.BrushBitmap = (Bitmap)Bitmap.FromFile(DrawingMapBrush.BrushPath);
            }

            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                if (DrawingPaintBrush == ColorPaintBrush.SoftBrush)
                {
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (DrawingPaintBrush == ColorPaintBrush.HardBrush)
                {
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush1)
                {
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush2)
                {
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush3)
                {
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderSize = 3;
                }
                else if (DrawingPaintBrush == ColorPaintBrush.PatternBrush4)
                {
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderColor = Color.DarkSeaGreen;
                    MainForm.DrawingPatternBrush4Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingSoftBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingHardBrushButton.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush1Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush2Button.FlatAppearance.BorderSize = 3;

                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderColor = Color.LightGray;
                    MainForm.DrawingPatternBrush3Button.FlatAppearance.BorderSize = 3;
                }

                MapStateMediator.MainUIMediator.SetDrawingModeLabel();

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    if (changedPropertyName == "DrawingTextureIndex")
                    {
                        UpdateDrawingTexturePictureBox();
                    }
                    else if (changedPropertyName == "DrawingStampBitmap")
                    {
                        if (DrawingStampBitmap != null)
                        {
                            Bitmap b = DrawingMethods.SetBitmapOpacity(DrawingStampBitmap, DrawingStampOpacity);
                            MainForm.StampPictureBox.Image = b;
                        }
                        else
                        {
                            MainForm.StampPictureBox.Image = null;
                        }
                    }
                    else if (changedPropertyName == "DrawingStampOpacity")
                    {
                        if (DrawingStampBitmap != null)
                        {
                            Bitmap b = DrawingMethods.SetBitmapOpacity(DrawingStampBitmap, DrawingStampOpacity);
                            MainForm.StampPictureBox.Image = b;
                        }
                    }
                    else if (changedPropertyName == "DrawingStampScale")
                    {
                        MainForm.DrawingStampScaleTrack.Value = (int)(DrawingStampScale * 100);
                    }
                    else if (changedPropertyName == "DrawingTextureOpacity")
                    {
                        if (DrawingTextureList.Count > 0)
                        {
                            UpdateDrawingTexturePictureBox();
                        }
                    }
                    else if (changedPropertyName == "DrawingTextureScale")
                    {
                        if (DrawingTextureList.Count > 0)
                        {
                            UpdateDrawingTexturePictureBox();
                        }
                    }
                    else if (changedPropertyName == "DrawingShapeRotation")
                    {
                        MainForm.DrawingShapeRotationTrack.Value = (int)DrawingShapeRotation;
                    }
                    else if (changedPropertyName == "ShowDrawingLayer")
                    {
                        MapLayer drawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER);
                        drawingLayer.ShowLayer = ShowDrawingLayer;
                    }
                }

            }));
        }

        #endregion

        #region Drawing UI Methods

        private void UpdateDrawingTexturePictureBox()
        {
            if (DrawingTextureIndex < 0)
            {
                DrawingTextureIndex = 0;
            }

            if (DrawingTextureIndex > DrawingTextureList.Count - 1)
            {
                DrawingTextureIndex = DrawingTextureList.Count - 1;
            }

            if (DrawingTextureList[DrawingTextureIndex].TextureBitmap == null)
            {
                DrawingTextureList[DrawingTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(DrawingTextureList[DrawingTextureIndex].TexturePath);
            }

            if (DrawingTextureList[DrawingTextureIndex].TextureBitmap != null)
            {
                Bitmap? textureBitmap = DrawingTextureList[DrawingTextureIndex].TextureBitmap;
                if (textureBitmap != null)
                {
                    using Bitmap resizedBitmap = (DrawingTextureScale != 1.0F)
                        ? DrawingMethods.ScaleBitmap(textureBitmap, (int)(textureBitmap.Width * DrawingTextureScale), (int)(textureBitmap.Height * DrawingTextureScale))
                        : new(textureBitmap, MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                    using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, DrawingTextureOpacity);

                    MainForm.DrawingFillTextureBox.Image = new Bitmap(b);
                    MainForm.DrawingFillTextureBox.Refresh();

                    MainForm.DrawingFillTextureNameLabel.Text = DrawingTextureList[DrawingTextureIndex].TextureName;
                    MainForm.DrawingFillTextureNameLabel.Refresh();
                }
            }
        }

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FillPaint.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
