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
    internal sealed class MapStateMediator
    {
        public static event PropertyChangedEventHandler? PropertyChanged;

        private static RealmStudioMap _currentMap = new();

        private static MainFormUIMediator? _mainFormUIMediator;

        private static BoxUIMediator? _boxUIMediator;
        private static FrameUIMediator? _frameUIMediator;
        private static MapGridUIMediator? _gridUIMediator;
        private static RegionUIMediator? _regionUIMediator;
        private static MapMeasureUIMediator? _measureUIMediator;
        private static MapScaleUIMediator? _scaleUIMediator;
        private static MapSymbolUIMediator? _symbolUIMediator;
        private static PathUIMediator? _pathUIMediator;

        // objects that are currently being created/updated/deleted/etc.
        private static Landform? _currentLandform;
        private static MapWindrose? _currentWindrose;
        private static WaterFeature? _currentWaterFeature;
        private static River? _currentRiver;
        private static MapPath? _currentMapPath;
        private static MapGrid? _currentMapGrid;
        private static MapMeasure? _currentMapMeasure;
        private static MapScale? _currentMapScale;
        private static MapRegion? _currentMapRegion;
        private static LayerPaintStroke? _currentPaintStroke;

        // objects that are currently selected
        private static Landform? _selectedLandform;
        private static MapPath? _selectedMapPath;
        private static MapPathPoint? _selectedMapPathPoint;
        private static MapSymbol? _selectedMapSymbol;
        private static PlacedMapBox? _selectedPlacedMapBox;
        private static MapLabel? _selectedMapLabel;
        private static MapScale? _selectedMapScale;
        private static IWaterFeature? _selectedWaterFeature;
        private static MapRiverPoint? _selectedRiverPoint;
        private static ColorPaintBrush _selectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        private static GeneratedLandformType _selectedLandformType = GeneratedLandformType.NotSet;
        private static SKRect _selectedRealmArea = SKRect.Empty;
        private static SKRect _previousSelectedRealmArea = SKRect.Empty;

        // TODO: how are these points related; can some be eliminated?
        private static SKPoint _scrollPoint = new(0, 0);
        private static SKPoint _drawingPoint = new(0, 0);

        private static SKPoint _currentMouseLocation = new(0, 0);
        private static SKPoint _previousMouseLocation = new(0, 0);

        private static SKPoint _currentCursorPoint = new(0, 0);
        private static SKPoint _previousCursorPoint = new(0, 0);

        // map default values
        private static readonly Font _defaultLabelFont = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly double _basePaintEventInterval = 10.0;
        private static readonly int _backupCount = 5;

        internal MapStateMediator()
        {
            PropertyChanged += MapStateMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        internal static RealmStudioMap CurrentMap
        {
            get { return _currentMap; }
            set { SetPropertyField(nameof(CurrentMap), ref _currentMap, value); }
        }

        // mediators
        internal static MainFormUIMediator? MainUIMediator
        {
            get { return _mainFormUIMediator; }
            set { _mainFormUIMediator = value; }
        }

        internal static BoxUIMediator? BoxMediator
        {
            get { return _boxUIMediator; }
            set { _boxUIMediator = value; }
        }

        internal static FrameUIMediator? FrameMediator
        {
            get { return _frameUIMediator; }
            set { _frameUIMediator = value; }
        }

        internal static MapGridUIMediator? GridUIMediator
        {
            get { return _gridUIMediator; }
            set { _gridUIMediator = value; }
        }

        internal static MapSymbolUIMediator? SymbolUIMediator
        {
            get { return _symbolUIMediator; }
            set { _symbolUIMediator = value; }
        }

        internal static RegionUIMediator? RegionUIMediator
        {
            get { return _regionUIMediator; }
            set { _regionUIMediator = value; }
        }

        internal static MapMeasureUIMediator? MeasureUIMediator
        {
            get { return _measureUIMediator; }
            set { _measureUIMediator = value; }
        }

        internal static MapScaleUIMediator? ScaleUIMediator
        {
            get { return _scaleUIMediator; }
            set { _scaleUIMediator = value; }
        }

        internal static PathUIMediator? PathUIMediator
        {
            get { return _pathUIMediator; }
            set { _pathUIMediator = value; }
        }

        // map state properties
        internal static Landform? CurrentLandform
        {
            get { return _currentLandform; }
            set { _currentLandform = value; }
        }
        internal static MapWindrose? CurrentWindrose
        {
            get { return _currentWindrose; }
            set { _currentWindrose = value; }
        }
        internal static WaterFeature? CurrentWaterFeature
        {
            get { return _currentWaterFeature; }
            set { _currentWaterFeature = value; }
        }

        internal static River? CurrentRiver
        {
            get { return _currentRiver; }
            set { _currentRiver = value; }
        }

        internal static MapPath? CurrentMapPath
        {
            get { return _currentMapPath; }
            set { _currentMapPath = value; }
        }

        internal static MapGrid? CurrentMapGrid
        {
            get { return _currentMapGrid; }
            set { _currentMapGrid = value; }
        }

        internal static MapMeasure? CurrentMapMeasure
        {
            get { return _currentMapMeasure; }
            set { _currentMapMeasure = value; }
        }

        internal static MapScale? CurrentMapScale
        {
            get { return _currentMapScale; }
            set { _currentMapScale = value; }
        }

        internal static MapRegion? CurrentMapRegion
        {
            get { return _currentMapRegion; }
            set { _currentMapRegion = value; }
        }

        internal static LayerPaintStroke? CurrentLayerPaintStroke
        {
            get { return _currentPaintStroke; }
            set { _currentPaintStroke = value; }
        }

        internal static Landform? SelectedLandform
        {
            get { return _selectedLandform; }
            set { _selectedLandform = value; }
        }

        internal static MapPath? SelectedMapPath
        {
            get { return _selectedMapPath; }
            set { _selectedMapPath = value; }
        }

        internal static MapPathPoint? SelectedMapPathPoint
        {
            get { return _selectedMapPathPoint; }
            set { _selectedMapPathPoint = value; }
        }

        internal static MapSymbol? SelectedMapSymbol
        {
            get { return _selectedMapSymbol; }
            set { _selectedMapSymbol = value; }
        }

        internal static PlacedMapBox? SelectedPlacedMapBox
        {
            get { return _selectedPlacedMapBox; }
            set { _selectedPlacedMapBox = value; }
        }

        internal static MapLabel? SelectedMapLabel
        {
            get { return _selectedMapLabel; }
            set { _selectedMapLabel = value; }
        }

        internal static MapScale? SelectedMapScale
        {
            get { return _selectedMapScale; }
            set { _selectedMapScale = value; }
        }

        internal static IWaterFeature? SelectedWaterFeature
        {
            get { return _selectedWaterFeature; }
            set { _selectedWaterFeature = value; }
        }

        internal static MapRiverPoint? SelectedRiverPoint
        {
            get { return _selectedRiverPoint; }
            set { _selectedRiverPoint = value; }
        }

        internal static ColorPaintBrush SelectedColorPaintBrush
        {
            get { return _selectedColorPaintBrush; }
            set { _selectedColorPaintBrush = value; }
        }

        internal static GeneratedLandformType GeneratedLandformType
        {
            get { return _selectedLandformType; }
            set { _selectedLandformType = value; }
        }

        internal static SKRect SelectedRealmArea
        {
            get { return _selectedRealmArea; }
            set { _selectedRealmArea = value; }
        }

        internal static SKRect PreviousSelectedRealmArea
        {
            get { return _previousSelectedRealmArea; }
            set { _previousSelectedRealmArea = value; }
        }

        internal static SKPoint ScrollPoint
        {
            get { return _scrollPoint; }
            set { _scrollPoint = value; }
        }

        internal static SKPoint DrawingPoint
        {
            get { return _drawingPoint; }
            set { _drawingPoint = value; }
        }

        internal static SKPoint CurrentMouseLocation
        {
            get { return _currentMouseLocation; }
            set { _currentMouseLocation = value; }
        }

        internal static SKPoint PreviousMouseLocation
        {
            get { return _previousMouseLocation; }
            set { _previousMouseLocation = value; }
        }

        internal static SKPoint CurrentCursorPoint
        {
            get { return _currentCursorPoint; }
            set { _currentCursorPoint = value; }
        }

        internal static SKPoint PreviousCursorPoint
        {
            get { return _previousCursorPoint; }
            set { _previousCursorPoint = value; }
        }

        // default values

        internal static Font DefaultLabelFont
        {
            get { return _defaultLabelFont; }
        }

        internal static double BasePaintEventInterval
        {
            get { return _basePaintEventInterval; }
        }

        internal static int BackupCount
        {
            get { return _backupCount; }
        }

        #endregion

        #region Property Change Handler Methods
        internal static void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(null, e);
        }

        internal static void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        public static void NotifyUpdate(string? changedPropertyName)
        {
            switch (changedPropertyName)
            {
                case "CurrentMap":
                    {
                        if (ScaleUIMediator != null)
                        {
                            ScaleUIMediator.ScaleUnitsText = CurrentMap.MapAreaUnits;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private static void MapStateMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion
    }
}
