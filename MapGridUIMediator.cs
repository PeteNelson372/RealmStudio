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
    internal class MapGridUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        //https://stackoverflow.com/questions/2246777/raise-an-event-whenever-a-propertys-value-changed

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private bool _gridEnabled = true;
        private MapGridType _gridType = MapGridType.Square;
        private int _gridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
        private string? _gridLayerName = "Default";
        private int _gridSize = 64;
        private int _gridLineWidth = 2;
        private Color _gridColor = Color.FromArgb(126, 0, 0, 0);
        private bool _showGridSize = true;

        public MapGridUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += MapGridUIMediator_PropertyChanged;
        }

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
            if (MapStateMediator.CurrentMap != null)
            {
                UpdateGridUI(changedPropertyName);
                MapGridManager.Update(MapStateMediator.CurrentMap, MapState, this);
                MainForm.SKGLRenderControl.Invalidate();
            }
        }

        private void UpdateGridUI(string? changedPropertyName)
        {
            // this methods updates the Main Form Symbol Tab UI
            // based on the property that has changed
            MainForm.EnableGridSwitch.Checked = GridEnabled;

            switch (GridType)
            {
                case MapGridType.Square:
                    {
                        MainForm.SquareGridRadio.Checked = true;
                    }
                    break;
                case MapGridType.FlatHex:
                    {
                        MainForm.FlatHexGridRadio.Checked = true;
                    }
                    break;
                case MapGridType.PointedHex:
                    {
                        MainForm.PointedHexGridRadio.Checked = true;
                    }
                    break;
                default:
                    {
                        MainForm.SquareGridRadio.Checked = true;
                    }
                    break;
            }

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

        #endregion

        #region Event Handlers
        private void MapGridUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion
    }
}
