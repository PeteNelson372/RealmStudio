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

namespace RealmStudio
{
    internal sealed class FrameUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private float _frameScale = 100.0F;
        private Color _frameTint = Color.White;
        private bool _frameEnabled = true;

        private MapFrame? _frame;

        public FrameUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += FrameUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        public bool FrameEnabled
        {
            get { return _frameEnabled; }
            set { SetPropertyField(nameof(FrameEnabled), ref _frameEnabled, value); }
        }

        public MapFrame? Frame
        {
            get { return _frame; }
            set { SetPropertyField(nameof(Frame), ref _frame, value); }
        }

        public float FrameScale
        {
            get { return _frameScale; }
            set { SetPropertyField(nameof(FrameScale), ref _frameScale, value); }
        }

        public Color FrameTint
        {
            get { return _frameTint; }
            set
            {
                if (_frameTint.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(FrameTint), ref _frameTint, value);
                }
            }
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
            UpdateFrameUI();
            FrameManager.Update(MapStateMediator.CurrentMap, MapState, this);
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateFrameUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.FrameTintColorSelectButton.BackColor = FrameTint;

                PlacedMapFrame? pmf = (PlacedMapFrame?)FrameManager.GetComponentById(MapStateMediator.CurrentMap, Guid.Empty);
                if (pmf != null)
                {
                    pmf.FrameEnabled = FrameEnabled;
                }
            }));
        }

        #endregion

        #region Event Handlers 
        private void FrameUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }


        private void FramePictureBox_MouseClick(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Left)
            {
                if (RealmStudioMainForm.ModifierKeys == Keys.None)
                {
                    PictureBox pb = (PictureBox)sender;

                    if (pb.Tag is MapFrame frame)
                    {
                        foreach (Control control in MainForm.FrameStyleTable.Controls)
                        {
                            if (control != pb)
                            {
                                control.BackColor = SystemColors.Control;
                            }
                        }

                        Color pbBackColor = pb.BackColor;

                        if (pbBackColor.ToArgb() == SystemColors.Control.ToArgb())
                        {
                            Frame = frame;

                            // clicked picture box is not selected, so select it
                            pb.BackColor = Color.LightSkyBlue;

                            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.FRAMELAYER).MapLayerComponents.Clear();

                            Cmd_CreateMapFrame cmd = new(MapStateMediator.CurrentMap, frame);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MapStateMediator.CurrentMap.IsSaved = false;
                        }
                        else
                        {
                            // clicked symbol is already selected, so deselect it
                            pb.BackColor = SystemColors.Control;
                        }
                    }
                }
            }
        }
        #endregion

        #region Frame UI Methods

        internal void AddMapFramesToFrameTable(List<MapFrame> mapFrames)
        {
            MainForm.FrameStyleTable.Hide();
            MainForm.FrameStyleTable.Controls.Clear();

            foreach (MapFrame frame in mapFrames)
            {
                if (frame.FrameBitmapPath != null)
                {
                    if (frame.FrameBitmap == null)
                    {
                        SKImage image = SKImage.FromEncodedData(frame.FrameBitmapPath);
                        frame.FrameBitmap = SKBitmap.FromImage(image);
                    }

                    PictureBox pb = new()
                    {
                        Tag = frame,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = frame.FrameBitmap.ToBitmap(),
                    };

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                    pb.MouseClick += FramePictureBox_MouseClick;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                    MainForm.FrameStyleTable.Controls.Add(pb);
                }
            }
            MainForm.FrameStyleTable.Show();
        }

        #endregion
    }
}
