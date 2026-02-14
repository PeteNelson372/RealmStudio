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
    public sealed class MainFormUIMediator: UiMediatorBase, IUIMediatorObserver
    {
        private MapDrawingMode _currentDrawingMode = MapDrawingMode.None;
        private static int _selectedBrushSize;
        private static double _currentBrushVelocity = 2.0;

        private bool _overlayLayerEnabled = true;
        private bool _waIntegrationEnabled;

        private MapLayer? _activeMapLayer;

        private IToolEditor? _activeTool;

        // default to background tab at app start up
        private EditorTab _activeTab = EditorTab.Background;

        #region Property Setters/Getters

        public EditorTab ActiveTab
        {
            get { return _activeTab; }
            set { SetPropertyField(nameof(ActiveTab), ref _activeTab, value); }
        }

        public RealmStudioShapeRenderingLib.MapLayer? ActiveMapLayer
        {
            get { return _activeMapLayer; }
            set { SetPropertyField(nameof(ActiveMapLayer), ref _activeMapLayer, value); }
        }

        public IToolEditor? ActiveEditorTool
        {
            get { return _activeTool; }
            set { SetPropertyField(nameof(ActiveEditorTool), ref _activeTool, value); }
        }

        public MapDrawingMode CurrentDrawingMode
        {
            get { return _currentDrawingMode; }
            set { SetPropertyField(nameof(CurrentDrawingMode), ref _currentDrawingMode, value); }
        }

        // UI values

        public int SelectedBrushSize
        {
            get { return _selectedBrushSize; }
            set { SetPropertyField(nameof(SelectedBrushSize), ref _selectedBrushSize, value); }
        }

        public double CurrentBrushVelocity
        {
            get { return _currentBrushVelocity; }
            set { SetPropertyField(nameof(CurrentBrushVelocity), ref _currentBrushVelocity, value); }
        }

        public bool OverlayLayerEnabled
        {
            get { return _overlayLayerEnabled; }
            set { SetPropertyField(nameof(OverlayLayerEnabled), ref _overlayLayerEnabled, value); }
        }

        public bool WorldAnvilIntegrationEnabled
        {
            get { return _waIntegrationEnabled; }
            set { SetPropertyField(nameof(WorldAnvilIntegration), ref _waIntegrationEnabled, value); }
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

        #endregion


    }
}
