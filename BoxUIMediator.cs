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
using SkiaSharp.Views.Desktop;
using System.ComponentModel;
using System.IO;

namespace RealmStudio
{
    internal class BoxUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private Color _boxTint = Color.White;
        private MapBox? _box;
        private SKRect _boxRect;

        public BoxUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += BoxUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        public MapBox? Box
        {
            get { return _box; }
            set { SetPropertyField(nameof(Box), ref _box, value); }
        }

        public Color BoxTint
        {
            get { return _boxTint; }
            set
            {
                if (_boxTint.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(BoxTint), ref _boxTint, value);
                }
            }
        }

        public SKRect BoxRect
        {
            get { return _boxRect; }
            set { _boxRect = value; }
        }

        #endregion

        #region Property Change Handler Methods

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyUpdate(string? changedPropertyName)
        {
            UpdateBoxUI();
            BoxManager.Update(MapStateMediator.CurrentMap, MapState, this);
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateBoxUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.SelectBoxTintButton.BackColor = BoxTint;
            }));
        }

        #endregion

        #region Event Handlers 
        private void BoxUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        internal void MapBoxPictureBox_MouseClick(object? sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Left)
            {
                if (RealmStudioMainForm.ModifierKeys == Keys.None)
                {
                    PictureBox? pb = (PictureBox?)sender;

                    if (pb != null && pb.Tag is MapBox b)
                    {
                        foreach (Control control in MainForm.LabelBoxStyleTable.Controls)
                        {
                            if (control != pb)
                            {
                                control.BackColor = SystemColors.Control;
                            }
                        }

                        Color pbBackColor = pb.BackColor;

                        if (pbBackColor.ToArgb() == SystemColors.Control.ToArgb())
                        {
                            // clicked symbol is not selected, so select it
                            pb.BackColor = Color.LightSkyBlue;
                            Box = b;
                        }
                        else
                        {
                            // clicked symbol is already selected, so deselect it
                            pb.BackColor = SystemColors.Control;
                            Box = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region Box UI Methods

        internal void DrawBoxOnWorkLayer(SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            SKRect boxRect = new(previousPoint.X, previousPoint.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

            if (Box != null && MapStateMediator.SelectedPlacedMapBox != null && boxRect.Width > 0 && boxRect.Height > 0)
            {
                BoxRect = boxRect;

                SKBitmap? b = Box.BoxBitmap.ToSKBitmap();

                if (b != null)
                {
                    float xScale = (float)(boxRect.Width / b.Width);
                    float yScale = (float)(boxRect.Height / b.Height);

                    SKRect center = new(MapStateMediator.SelectedPlacedMapBox.BoxCenterLeft,
                        MapStateMediator.SelectedPlacedMapBox.BoxCenterTop,
                        MapStateMediator.SelectedPlacedMapBox.BoxCenterRight,
                        MapStateMediator.SelectedPlacedMapBox.BoxCenterBottom);

                    SKMatrix tm = SKMatrix.CreateScale(xScale, yScale);
                    SKRect newCenter = tm.MapRect(center);

                    MapStateMediator.SelectedPlacedMapBox.BoxCenterLeft = newCenter.Left;
                    MapStateMediator.SelectedPlacedMapBox.BoxCenterTop = newCenter.Top;
                    MapStateMediator.SelectedPlacedMapBox.BoxCenterRight = newCenter.Right;
                    MapStateMediator.SelectedPlacedMapBox.BoxCenterBottom = newCenter.Bottom;

                    SKBitmap resizedBitmap = b.Resize(new SKImageInfo((int)boxRect.Width, (int)boxRect.Height), SKSamplingOptions.Default);

                    MapStateMediator.SelectedPlacedMapBox.BoxBitmap = resizedBitmap.Copy();
                    MapStateMediator.SelectedPlacedMapBox.Width = resizedBitmap.Width;
                    MapStateMediator.SelectedPlacedMapBox.Height = resizedBitmap.Height;



                    MapLayer workLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER);
                    workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);


                    workLayer.LayerSurface?.Canvas.DrawBitmap(resizedBitmap, previousPoint, PaintObjects.BoxPaint);
                }
            }
        }


        internal void AddMapBoxesToLabelBoxTable(List<MapBox> mapBoxes)
        {
            MainForm.LabelBoxStyleTable.Hide();
            MainForm.LabelBoxStyleTable.Controls.Clear();
            foreach (MapBox box in mapBoxes)
            {
                if (box.BoxBitmap == null && !string.IsNullOrEmpty(box.BoxBitmapPath))
                {
                    if (File.Exists(box.BoxBitmapPath))
                    {
                        box.BoxBitmap ??= new Bitmap(box.BoxBitmapPath);
                    }
                }

                if (box.BoxBitmap != null)
                {
                    PictureBox pb = new()
                    {
                        Tag = box,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = (Image)box.BoxBitmap.Clone(),
                    };

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                    pb.MouseClick += MapBoxPictureBox_MouseClick;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                    MainForm.LabelBoxStyleTable.Controls.Add(pb);
                }

            }
            MainForm.LabelBoxStyleTable.Show();
            MainForm.LabelBoxStyleTable.Refresh();
        }

        #endregion
    }
}
