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
    internal sealed class MapScaleUIMediator : IUIMediatorObserver, INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private bool _scalePanelVisible;
        private int _scaleWidth = 256;
        private int _scaleHeight = 16;
        private int _segmentCount = 5;
        private int _scaleLineWidth = 3;
        private Color _scaleColor1 = Color.Black;
        private Color _scaleColor2 = Color.White;
        private Color _scaleColor3 = Color.Black;
        private float _segmentDistance = 100.0F;
        private string? _scaleUnitsText = string.Empty;
        private ScaleNumbersDisplayLocation _scaleNumbersDisplayType = ScaleNumbersDisplayLocation.All;
        private Font _scaleFont = new("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private Color _scaleFontColor = Color.Black;
        private Color _scaleNumberOutlineColor = Color.White;
        private int _scaleOutlineWidth = 2;

        internal MapScaleUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += MapScaleUIMediator_PropertyChanged;
        }

        #region Property Getters/Setters

        internal MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        internal bool ScalePanelVisible
        {
            get { return _scalePanelVisible; }
            set { SetPropertyField(nameof(ScalePanelVisible), ref _scalePanelVisible, value); }
        }
        internal int ScaleWidth
        {
            get { return _scaleWidth; }
            set { SetPropertyField(nameof(ScaleWidth), ref _scaleWidth, value); }
        }

        internal int SegmentCount
        {
            get { return _segmentCount; }
            set { SetPropertyField(nameof(SegmentCount), ref _segmentCount, value); }
        }

        internal int ScaleHeight
        {
            get { return _scaleHeight; }
            set { SetPropertyField(nameof(ScaleHeight), ref _scaleHeight, value); }
        }

        internal int ScaleLineWidth
        {
            get { return _scaleLineWidth; }
            set { SetPropertyField(nameof(ScaleLineWidth), ref _scaleLineWidth, value); }
        }

        internal Color ScaleColor1
        {
            get { return _scaleColor1; }
            set { SetPropertyField(nameof(ScaleColor1), ref _scaleColor1, value); }
        }

        internal Color ScaleColor2
        {
            get { return _scaleColor2; }
            set { SetPropertyField(nameof(ScaleColor2), ref _scaleColor2, value); }
        }

        internal Color ScaleColor3
        {
            get { return _scaleColor3; }
            set { SetPropertyField(nameof(ScaleColor3), ref _scaleColor3, value); }
        }

        internal float SegmentDistance
        {
            get { return _segmentDistance; }
            set { SetPropertyField(nameof(SegmentDistance), ref _segmentDistance, value); }
        }

        internal string? ScaleUnitsText
        {
            get { return _scaleUnitsText; }
            set { SetPropertyField(nameof(ScaleUnitsText), ref _scaleUnitsText, value); }
        }

        internal ScaleNumbersDisplayLocation ScaleNumbersDisplayType
        {
            get { return _scaleNumbersDisplayType; }
            set { SetPropertyField(nameof(ScaleNumbersDisplayType), ref _scaleNumbersDisplayType, value); }
        }

        internal Font ScaleFont
        {
            get { return _scaleFont; }
            set { SetPropertyField(nameof(ScaleFont), ref _scaleFont, value); }
        }

        internal Color ScaleFontColor
        {
            get { return _scaleFontColor; }
            set { SetPropertyField(nameof(ScaleFontColor), ref _scaleFontColor, value); }
        }

        internal Color ScaleNumberOutlineColor
        {
            get { return _scaleNumberOutlineColor; }
            set { SetPropertyField(nameof(ScaleNumberOutlineColor), ref _scaleNumberOutlineColor, value); }
        }

        internal int ScaleOutlineWidth
        {
            get => _scaleOutlineWidth;
            set { SetPropertyField(nameof(ScaleOutlineWidth), ref _scaleOutlineWidth, value); }
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
            UpdateMapScaleUI();
            MapScaleManager.Update();

            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateMapScaleUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.MapScaleCreatorPanel.Visible = ScalePanelVisible;
                MainForm.ScaleColor1Button.BackColor = ScaleColor1;
                MainForm.ScaleColor2Button.BackColor = ScaleColor2;
                MainForm.ScaleColor3Button.BackColor = ScaleColor3;
                MainForm.SelectScaleFontColorButton.BackColor = ScaleFontColor;
                MainForm.SelectScaleOutlineColorButton.BackColor = ScaleNumberOutlineColor;

                if (ScalePanelVisible)
                {
                    MainForm.ScaleUnitsTextBox.Text = ScaleUnitsText;
                }
            }));
        }

        #endregion

        #region Event Handlers
        private void MapScaleUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region IDisposable Implementation
        internal void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _scaleFont.Dispose();
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

        #region MapScale UI methods

        internal void Reset()
        {
            ScalePanelVisible = false;
            ScaleWidth = 256;
            ScaleHeight = 16;
            SegmentCount = 5;
            ScaleLineWidth = 3;
            SegmentDistance = 100.0F;
            ScaleUnitsText = string.Empty;
            ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.All;
            ScaleFont = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ScaleFontColor = Color.Black;
            ScaleNumberOutlineColor = Color.White;
            ScaleOutlineWidth = 2;
            ResetScaleColors();
        }

        internal void ResetScaleColors()
        {
            ScaleColor1 = Color.Black;
            ScaleColor2 = Color.White;
            ScaleColor3 = Color.Black;
        }

        #endregion
    }
}

