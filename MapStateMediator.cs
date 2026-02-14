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
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    internal sealed class MapStateMediator
    {
        public event Action<MapStateMediator>? Changed;

        private static RealmStudioMap? _currentMap;
        private static RealmStudioMapSet? _currentMapSet;

        private static MainFormUIMediator? _mainFormUIMediator;

        private static BackgroundUIMediator? _backgroundMediator;
        private static BoxUIMediator? _boxUIMediator;
        private static FrameUIMediator? _frameUIMediator;
        private static LandformUIMediator? _landformUIMediator;
        private static MapGridUIMediator? _gridUIMediator;
        private static MapMeasureUIMediator? _measureUIMediator;
        private static MapScaleUIMediator? _scaleUIMediator;
        private static OceanUIMediator? _oceanUIMediator;
        private static PathUIMediator? _pathUIMediator;
        private static RegionUIMediator? _regionUIMediator;
        private static SymbolUIMediator? _symbolUIMediator;
        private static VignetteUIMediator? _vignetteUIMediator;
        private static WaterFeatureUIMediator? _waterFeatureUIMediator;
        private static WindroseUIMediator? _windroseUIMediator;
        private static InteriorUIMediator? _interiorUIMediator;

        private static TimerManager? _applicationTimerManager;

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
        private static PlacedMapFrame? _currentMapFrame;
        private static InteriorFloor? _currentInteriorFloor;
        private static InteriorWall? _currentInteriorWall;

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
        private static DrawnMapComponent? _selectedDrawnMapComponent;
        private static SKRect _selectedRealmArea = SKRect.Empty;
        private static SKRect _previousSelectedRealmArea = SKRect.Empty;
        private static InteriorFloor? _selectedInteriorFloor;

        // mouse cursor points
        private static SKPoint _scrollPoint = new(0, 0);
        private static SKPoint _drawingPoint = new(0, 0);

        private static SKPoint _currentMouseLocation = SKPoint.Empty;
        private static SKPoint _previousMouseLocation = SKPoint.Empty;

        private static SKPoint _currentCursorPoint = new(0, 0);
        private static SKPoint _previousCursorPoint = new(0, 0);

        // map default values
        private static readonly Font _defaultLabelFont = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly double _basePaintEventInterval = 10.0;
        private static readonly int _backupCount = 5;

        #region Property Setters/Getters

        internal RealmStudioMap? CurrentMap
        {
            get { return _currentMap; }
            set { SetPropertyField(nameof(CurrentMap), ref _currentMap, value); }
        }

        internal RealmStudioMapSet? CurrentMapSet
        {
            get { return _currentMapSet; }
            set { SetPropertyField(nameof(CurrentMapSet), ref _currentMapSet, value); }
        }

        internal static TimerManager? ApplicationTimerManager
        {
            get { return _applicationTimerManager; }
            set { _applicationTimerManager = value; }
        }

        // map state properties
 

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

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                Changed?.Invoke(this);
            }
        }

        #endregion

        #region UI Methods
        /*

        internal static void ResetUI()
        {
            ArgumentNullException.ThrowIfNull(ApplicationTimerManager);
            ArgumentNullException.ThrowIfNull(MainUIMediator);
            ArgumentNullException.ThrowIfNull(LabelManager.LabelMediator);

            // deselect selected symbols, map frames, boxes,
            // clear any label paths that have been drawn
            // deselect all selected objects
            // what else?

            // stop and dispose of the brush timer
            ApplicationTimerManager.BrushTimerEnabled = false;

            // stop placing symbols
            ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;

            // clear drawing mode and set brush size to 0
            MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            // dispose of any map label path
            LabelManager.CurrentMapLabelPath?.Dispose();
            LabelManager.CurrentMapLabelPath = new();

            // clear other selected and current objects?
            CurrentMapRegion = null;
            CurrentWindrose = null;

            if (CurrentMap != null)
            {
                // clear the work layers
                MapBuilder.GetMapLayerByIndex(CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                MapBuilder.GetMapLayerByIndex(CurrentMap, MapBuilder.WORKLAYER2).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                // unselect anything selected
                RealmMapMethods.DeselectAllMapComponents(CurrentMap, null);
                
            }


            // dispose of any label text box that is drawn
            LabelManager.LabelMediator.RemoveTextBox();

            // clear all selections
            PreviousSelectedRealmArea = SKRect.Empty;
            SelectedRealmArea = SKRect.Empty;

            MainUIMediator.ShowHideFontSelectionPanel(false);
        }

        internal static void DeleteSelectedMapObjects()
        {
            ArgumentNullException.ThrowIfNull(MainUIMediator);

            if (CurrentMap == null)
            {
                return;
            }

            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.LandformSelect:
                    if (SelectedLandform != null)
                    {
                        //Cmd_RemoveLandform cmd = new(CurrentMap, SelectedLandform);
                        //CommandManager.AddCommand(cmd);
                        //cmd.DoOperation();

                        SelectedLandform = null;
                        //CurrentMap.IsSaved = false;
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    {
                        if (SelectedWaterFeature is River r)
                        {
                            WaterFeatureManager.DeleteRiver(r);
                        }
                        else
                        {
                            WaterFeatureManager.DeleteWaterFeature(SelectedWaterFeature);
                        }
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        WaterFeatureManager.RemoveRiverPoint();
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        PathManager.Delete();
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        PathManager.RemovePathPoint();
                    }
                    break;
                case MapDrawingMode.SymbolSelect:
                    {
                        SymbolManager.Delete();
                    }
                    break;
                case MapDrawingMode.LabelSelect:
                    {
                        if (SelectedMapLabel != null)
                        {
                            LabelManager.Delete();
                        }

                        if (SelectedPlacedMapBox != null)
                        {
                            BoxManager.Delete();
                        }
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (CurrentMapRegion != null)
                        {
                            bool pointDeleted = RegionManager.DeleteSelectedRegionPoint(CurrentMapRegion);

                            if (!pointDeleted)
                            {
                                RegionManager.Delete();
                            }
                        }
                    }
                    break;
                case MapDrawingMode.DrawingSelect:
                    {
                        if (SelectedDrawnMapComponent != null)
                        {
                            DrawingManager.Delete();
                        }
                    }
                    break;
            }
        }
        */
        #endregion
    }
}
