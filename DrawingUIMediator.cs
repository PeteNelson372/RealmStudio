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
        private List<SKPoint> _polygonPoints = [];

        private readonly List<MapTexture> _drawingTextureList = [];
        private int _drawingTextureIndex;

        private DrawingFillType _fillType = DrawingFillType.Color;

        private SKPaint _fillPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Butt
        };

        private Bitmap? _drawingStampBitmap;

        private ColorPaintBrush _drawingPaintBrush = ColorPaintBrush.SoftBrush;

        private float _drawingStampOpacity = 1.0f;
        private float _drawingStampScale = 1.0f;
        private float _drawingStampRotation;

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

        internal List<SKPoint> PolygonPoints
        {
            get { return _polygonPoints; }
            set { SetPropertyField(nameof(PolygonPoints), ref _polygonPoints, value); }
        }

        internal ColorPaintBrush DrawingPaintBrush
        {
            get { return _drawingPaintBrush; }
            set { SetPropertyField(nameof(DrawingPaintBrush), ref _drawingPaintBrush, value); }
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

                MapStateMediator.SelectedColorPaintBrush = DrawingPaintBrush;

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

            MainForm.DrawingFillTextureBox.Image = DrawingTextureList[DrawingTextureIndex].TextureBitmap;
            MainForm.DrawingFillTextureNameLabel.Text = DrawingTextureList[DrawingTextureIndex].TextureName;
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
