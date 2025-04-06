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
    internal sealed class WindroseUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private int _innerCircleCount;
        private int _innerCircleRadius;
        private bool _fadeOut;
        private int _lineWidth = 2;
        private int _outerRadius = 1000;
        private Color _windroseColor = Color.FromArgb(127, 61, 55, 40);
        private int _directionCount = 16;

        public WindroseUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += WindroseUIMediator_PropertyChanged;
        }

        public int InnerCircleCount
        {
            get { return _innerCircleCount; }
            set { SetPropertyField(nameof(InnerCircleCount), ref _innerCircleCount, value); }
        }

        public int InnerCircleRadius
        {
            get { return _innerCircleRadius; }
            set { SetPropertyField(nameof(InnerCircleRadius), ref _innerCircleRadius, value); }
        }

        public bool FadeOut
        {
            get { return _fadeOut; }
            set { SetPropertyField(nameof(FadeOut), ref _fadeOut, value); }
        }

        public int LineWidth
        {
            get { return _lineWidth; }
            set { SetPropertyField(nameof(LineWidth), ref _lineWidth, value); }
        }

        public int OuterRadius
        {
            get { return _outerRadius; }
            set { SetPropertyField(nameof(OuterRadius), ref _outerRadius, value); }
        }

        public Color WindroseColor
        {
            get { return _windroseColor; }
            set { SetPropertyField(nameof(WindroseColor), ref _windroseColor, value); }
        }

        internal int DirectionCount
        {
            get { return _directionCount; }
            set { SetPropertyField(nameof(DirectionCount), ref _directionCount, value); }
        }


        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
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
            UpdateWindroseUI();
            WindroseManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        public void UpdateWindroseUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.WindroseColorSelectButton.BackColor = WindroseColor;
            }));
        }
        #endregion

        #region Event Handlers
        private void WindroseUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Windrose UI Methods

        internal void Reset()
        {
            // Reset the Windrose UI to its default state
            InnerCircleCount = 0;
            InnerCircleRadius = 0;
            FadeOut = false;
            LineWidth = 2;
            OuterRadius = 1000;
            WindroseColor = Color.FromArgb(127, 61, 55, 40);
            DirectionCount = 16;
        }

        #endregion
    }
}
