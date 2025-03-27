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

namespace RealmStudio
{
    internal class RealmMapState
    {
        private static RealmStudioMap _currentMap = new();
        private static SKGLControl? _glControl;

        private static MapDrawingMode _currentDrawingMode = MapDrawingMode.None;

        // objects that are currently being created/updated/deleted/etc.
        private static Landform? _currentLandform;
        private static MapWindrose? _currentWindrose;
        private static WaterFeature? _currentWaterFeature;
        private static River? _currentRiver;
        private static MapPath? _currentMapPath;
        private static MapGrid? _currentMapGrid;
        private static MapMeasure? _currentMapMeasure;
        private static MapRegion? _currentMapRegion;
        private static LayerPaintStroke? _currentPaintStroke;

        // objects that are currently selected
        private static Landform? _selectedLandform;
        private static MapPath? _selectedMapPath;
        private static MapPathPoint? _selectedMapPathPoint;
        private static MapSymbol? _selectedMapSymbol;
        private static MapBox? _selectedMapBox;
        private static PlacedMapBox? _selectedPlacedMapBox;
        private static MapLabel? _selectedMapLabel;
        private static MapScale? _selectedMapScale;
        private static IWaterFeature? _selectedWaterFeature;
        private static MapRiverPoint? _selectedRiverPoint;
        private static ColorPaintBrush _selectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        private static GeneratedLandformType _selectedLandformType = GeneratedLandformType.NotSet;
        private static SKRect _selectedRealmArea = SKRect.Empty;
        private static SKRect _previousSelectedRealmArea = SKRect.Empty;
        private static Font _selectedLabelFont = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static Font _selectedMapScaleFont = new("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);

        // UI values (TODO: maybe move into UIMediatorObserver for main form?)
        private static int _selectedBrushSize;
        private static double _currentBrushVelocity = 2.0;

        private static float _drawingZoom = 1.0f;

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



        public static RealmStudioMap CurrentMap
        {
            get { return _currentMap; }
            set { _currentMap = value; }
        }

        public static SKGLControl? GLRenderControl
        {
            get { return _glControl; }
            set { _glControl = value; }
        }

        public static MapDrawingMode CurrentDrawingMode
        {
            get { return _currentDrawingMode; }
            set { _currentDrawingMode = value; }
        }

        public static Landform? CurrentLandform
        {
            get { return _currentLandform; }
            set { _currentLandform = value; }
        }
        public static MapWindrose? CurrentWindrose
        {
            get { return _currentWindrose; }
            set { _currentWindrose = value; }
        }
        public static WaterFeature? CurrentWaterFeature
        {
            get { return _currentWaterFeature; }
            set { _currentWaterFeature = value; }
        }

        public static River? CurrentRiver
        {
            get { return _currentRiver; }
            set { _currentRiver = value; }
        }

        public static MapPath? CurrentMapPath
        {
            get { return _currentMapPath; }
            set { _currentMapPath = value; }
        }

        public static MapGrid? CurrentMapGrid
        {
            get { return _currentMapGrid; }
            set { _currentMapGrid = value; }
        }

        public static MapMeasure? CurrentMapMeasure
        {
            get { return _currentMapMeasure; }
            set { _currentMapMeasure = value; }
        }

        public static MapRegion? CurrentMapRegion
        {
            get { return _currentMapRegion; }
            set { _currentMapRegion = value; }
        }

        public static LayerPaintStroke? CurrentLayerPaintStroke
        {
            get { return _currentPaintStroke; }
            set { _currentPaintStroke = value; }
        }

        public static Landform? SelectedLandform
        {
            get { return _selectedLandform; }
            set { _selectedLandform = value; }
        }

        public static MapPath? SelectedMapPath
        {
            get { return _selectedMapPath; }
            set { _selectedMapPath = value; }
        }

        public static MapPathPoint? SelectedMapPathPoint
        {
            get { return _selectedMapPathPoint; }
            set { _selectedMapPathPoint = value; }
        }

        public static MapSymbol? SelectedMapSymbol
        {
            get { return _selectedMapSymbol; }
            set { _selectedMapSymbol = value; }
        }

        public static MapBox? SelectedMapBox
        {
            get { return _selectedMapBox; }
            set { _selectedMapBox = value; }
        }

        public static PlacedMapBox? SelectedPlacedMapBox
        {
            get { return _selectedPlacedMapBox; }
            set { _selectedPlacedMapBox = value; }
        }

        public static MapLabel? SelectedMapLabel
        {
            get { return _selectedMapLabel; }
            set { _selectedMapLabel = value; }
        }

        public static MapScale? SelectedMapScale
        {
            get { return _selectedMapScale; }
            set { _selectedMapScale = value; }
        }

        public static IWaterFeature? SelectedWaterFeature
        {
            get { return _selectedWaterFeature; }
            set { _selectedWaterFeature = value; }
        }

        public static MapRiverPoint? SelectedRiverPoint
        {
            get { return _selectedRiverPoint; }
            set { _selectedRiverPoint = value; }
        }

        public static ColorPaintBrush SelectedColorPaintBrush
        {
            get { return _selectedColorPaintBrush; }
            set { _selectedColorPaintBrush = value; }
        }

        public static GeneratedLandformType GeneratedLandformType
        {
            get { return _selectedLandformType; }
            set { _selectedLandformType = value; }
        }

        public static SKRect SelectedRealmArea
        {
            get { return _selectedRealmArea; }
            set { _selectedRealmArea = value; }
        }

        public static SKRect PreviousSelectedRealmArea
        {
            get { return _previousSelectedRealmArea; }
            set { _previousSelectedRealmArea = value; }
        }

        public static Font SelectedLabelFont
        {
            get { return _selectedLabelFont; }
            set { _selectedLabelFont = value; }
        }

        public static Font SelectedMapScaleFont
        {
            get { return _selectedMapScaleFont; }
            set { _selectedMapScaleFont = value; }
        }

        // UI values

        public static int SelectedBrushSize
        {
            get { return _selectedBrushSize; }
            set { _selectedBrushSize = value; }
        }

        public static double CurrentBrushVelocity
        {
            get { return _currentBrushVelocity; }
            set { _currentBrushVelocity = value; }
        }

        public static float DrawingZoom
        {
            get { return _drawingZoom; }
            set { _drawingZoom = value; }
        }

        public static SKPoint ScrollPoint
        {
            get { return _scrollPoint; }
            set { _scrollPoint = value; }
        }

        public static SKPoint DrawingPoint
        {
            get { return _drawingPoint; }
            set { _drawingPoint = value; }
        }

        public static SKPoint CurrentMouseLocation
        {
            get { return _currentMouseLocation; }
            set { _currentMouseLocation = value; }
        }

        public static SKPoint PreviousMouseLocation
        {
            get { return _previousMouseLocation; }
            set { _previousMouseLocation = value; }
        }

        public static SKPoint CurrentCursorPoint
        {
            get { return _currentCursorPoint; }
            set { _currentCursorPoint = value; }
        }

        public static SKPoint PreviousCursorPoint
        {
            get { return _previousCursorPoint; }
            set { _previousCursorPoint = value; }
        }

        // default values

        public static Font DefaultLabelFont
        {
            get { return _defaultLabelFont; }
        }

        public static double BasePaintEventInterval
        {
            get { return _basePaintEventInterval; }
        }

        public static int BackupCount
        {
            get { return _backupCount; }
        }
    }
}
