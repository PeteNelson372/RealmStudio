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

        private readonly RealmStudioMainForm _mainForm;
        private RealmMapState? _mapState;

        private MapGridType _gridType = MapGridType.NotSet;
        private int _gridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
        private int _gridSize = 64;
        private int _gridLineWidth = 2;
        private Color _gridColor = Color.FromArgb(126, 0, 0, 0);
        private bool _showGridSize = true;

        public MapGridUIMediator(RealmStudioMainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public RealmMapState? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }
        public MapGridType GridType
        {
            get { return _gridType; }
            set { SetPropertyField(nameof(GridType), ref _gridType, value); }
        }
        public int GridLayer
        {
            get { return _gridLayerIndex; }
            set { SetPropertyField(nameof(GridLayer), ref _gridLayerIndex, value); }
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
            set { SetPropertyField(nameof(GridColor), ref _gridColor, value); }
        }
        public bool ShowGridSize
        {
            get => _showGridSize;
            set { SetPropertyField(nameof(ShowGridSize), ref _showGridSize, value); }
        }

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
            if (RealmMapState.CurrentMap != null)
            {
                MapGridManager.Update(RealmMapState.CurrentMap, MapState, this);
            }
        }
    }
}
