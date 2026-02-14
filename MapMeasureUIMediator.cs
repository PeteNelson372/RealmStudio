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

namespace RealmStudioX
{
    internal sealed class MapMeasureUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        private MapStateMediator? _mapState;
        private Color _mapMeasureColor = Color.FromArgb(191, 138, 26, 0);
        private bool _useScaleUnits = true;
        private bool _measureArea;


        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        public Color MapMeasureColor
        {
            get { return _mapMeasureColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(MapMeasureColor), ref _mapMeasureColor, value);
                }
            }
        }

        public bool UseScaleUnits
        {
            get { return _useScaleUnits; }
            set { SetPropertyField(nameof(UseScaleUnits), ref _useScaleUnits, value); }
        }

        public bool MeasureArea
        {
            get { return _measureArea; }
            set { SetPropertyField(nameof(MeasureArea), ref _measureArea, value); }
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
        private void UpdateMapMeasureUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.SelectMeasureColorButton.BackColor = MapMeasureColor;
            }));
        }
        */
        #endregion

        #region Map Measure UI Methods

        /*
        internal void Reset()
        {
            MapMeasureColor = Color.FromArgb(191, 138, 26, 0);
            UseScaleUnits = true;
            MeasureArea = false;
        }
        */

        #endregion
    }
}
