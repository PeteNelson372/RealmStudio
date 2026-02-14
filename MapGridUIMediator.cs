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

namespace RealmStudioX
{
    internal sealed class MapGridUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        private MapStateMediator? _mapState;

        private bool _gridEnabled = true;
        private MapGridType _gridType = MapGridType.Square;
        private int _gridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
        private string? _gridLayerName = "Default";
        private int _gridSize = 64;
        private int _gridLineWidth = 2;
        private Color _gridColor = Color.FromArgb(126, 0, 0, 0);
        private bool _showGridSize = true;

        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }
        public bool GridEnabled
        {
            get => _gridEnabled;
            set { SetPropertyField(nameof(GridEnabled), ref _gridEnabled, value); }
        }
        public MapGridType GridType
        {
            get { return _gridType; }
            set { SetPropertyField(nameof(GridType), ref _gridType, value); }
        }
        public int GridLayerIndex
        {
            get { return _gridLayerIndex; }
            set
            {
                SetPropertyField(nameof(GridLayerIndex), ref _gridLayerIndex, value);

                if (value == MapBuilder.DEFAULTGRIDLAYER)
                {
                    _gridLayerName = "Default";
                }
                else if (value == MapBuilder.ABOVEOCEANGRIDLAYER)
                {
                    _gridLayerName = "Above Ocean";
                }
                else if (value == MapBuilder.BELOWSYMBOLSGRIDLAYER)
                {
                    _gridLayerName = "Below Symbols";
                }
                else
                {
                    _gridLayerName = "Default";
                }
            }
        }
        public string? GridLayerName
        {
            get { return _gridLayerName; }
            set
            {
                SetPropertyField(nameof(GridLayerName), ref _gridLayerName, value);

                switch (value)
                {
                    case "Default":
                        {
                            _gridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
                        }
                        break;
                    case "Above Ocean":
                        {
                            _gridLayerIndex = MapBuilder.ABOVEOCEANGRIDLAYER;
                        }
                        break;
                    case "Below Symbols":
                        {
                            _gridLayerIndex = MapBuilder.BELOWSYMBOLSGRIDLAYER;
                        }
                        break;
                    default:
                        {
                            _gridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
                        }
                        break;
                }
            }
        }
        public int GridSize
        {
            get { return _gridSize; }
            set { SetPropertyField(nameof(GridSize), ref _gridSize, value); }
        }
        public int GridLineWidth
        {
            get { return _gridLineWidth; }
            set { SetPropertyField(nameof(GridLineWidth), ref _gridLineWidth, value); }
        }
        public Color GridColor
        {
            get => _gridColor;
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(GridColor), ref _gridColor, value);
                }
            }
        }
        public bool ShowGridSize
        {
            get => _showGridSize;
            set { SetPropertyField(nameof(ShowGridSize), ref _showGridSize, value); }
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
        private void UpdateGridUI(string? changedPropertyName)
        {
            // this methods updates the Main Form Symbol Tab UI
            // based on the property that has changed
            MainForm.EnableGridSwitch.Checked = GridEnabled;

            MainForm.GridLayerUpDown.SelectedItem = GridLayerName;
            MainForm.GridColorSelectButton.BackColor = GridColor;
            MainForm.ShowGridSizeSwitch.Checked = ShowGridSize;

            if (string.IsNullOrEmpty(changedPropertyName))
            {
                MainForm.GridSizeTrack.Value = GridSize;
                MainForm.GridLineWidthTrack.Value = GridLineWidth;
            }
            else
            {
                switch (changedPropertyName)
                {
                    case "GridSize":
                        {
                            MainForm.GridSizeTrack.Value = GridSize;
                        }
                        break;
                    case "GridLineWidth":
                        {
                            MainForm.GridLineWidthTrack.Value = GridLineWidth;
                            break;
                        }
                    default:
                        {
                            MainForm.GridSizeTrack.Value = GridSize;
                            MainForm.GridLineWidthTrack.Value = GridLineWidth;
                            break;
                        }
                }
            }
        }
        */
        #endregion

        #region Grid UI methods

        /*
        internal void Reset()
        {
            GridEnabled = true;
            GridType = MapGridType.Square;
            GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
            GridLayerName = "Default";
            GridSize = 64;
            GridLineWidth = 2;
            GridColor = Color.FromArgb(126, 0, 0, 0);
            ShowGridSize = true;
        }

        internal void Initialize(bool enabled, MapGridType gridType, int layerIndex, int gridSize, int lineWidth, Color gridColor, bool showSize)
        {
            // this method initializes the grid UI values without triggering a UI update in NotifyUpdate
            // this avoids a recursive loop when the UI is updated at the time a new map is loaded or a theme is applied

            _gridEnabled = enabled;
            _gridType = gridType;
            _gridLayerIndex = layerIndex;
            _gridSize = gridSize;
            _gridLineWidth = lineWidth;
            _gridColor = gridColor;
            _showGridSize = showSize;

            MainForm.EnableGridSwitch.Checked = _gridEnabled;
            MainForm.GridLayerUpDown.SelectedItem = _gridLayerName;
            MainForm.GridColorSelectButton.BackColor = _gridColor;
            MainForm.ShowGridSizeSwitch.Checked = _showGridSize;
            MainForm.GridSizeTrack.Value = _gridSize;
            MainForm.GridLineWidthTrack.Value = _gridLineWidth;

            switch (_gridType)
            {
                case MapGridType.Square:
                    {
                        MainForm.SquareGridRadio.Checked = true;
                    }
                    break;
                case MapGridType.FlatHex:
                    {
                        MainForm.FlatHexGridRadio.Checked = true;
                        _gridSize *= 2;
                    }
                    break;
                case MapGridType.PointedHex:
                    {
                        MainForm.PointedHexGridRadio.Checked = true;
                        _gridSize *= 2;
                    }
                    break;
                default:
                    {
                        MainForm.SquareGridRadio.Checked = true;
                    }
                    break;
            }

            if (_gridLayerIndex == MapBuilder.DEFAULTGRIDLAYER)
            {
                _gridLayerName = "Default";
            }
            else if (_gridLayerIndex == MapBuilder.ABOVEOCEANGRIDLAYER)
            {
                _gridLayerName = "Above Ocean";
            }
            else if (_gridLayerIndex == MapBuilder.BELOWSYMBOLSGRIDLAYER)
            {
                _gridLayerName = "Below Symbols";
            }
            else
            {
                _gridLayerName = "Default";
            }

            MainForm.GridLayerUpDown.SelectedItem = _gridLayerName;
        }
        */
        #endregion
    }
}
