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
    internal sealed class PathUIMediator : IUIMediatorObserver
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private readonly List<MapTexture> _pathTextureList = [];
        private readonly List<MapVector> _pathVectorList = [];

        private bool _showPathLayers = true;
        private PathType _pathType = PathType.SolidLinePath;
        private int _pathWidth;
        private Color _pathColor = Color.FromArgb(75, 49, 26);
        private bool _editPathPoints;
        private bool _drawOverSymbols;

        private int _pathTextureIndex;

        internal PathUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += PathUIMediator_PropertyChanged;
        }

        #region Property Setter/Getters

        internal MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        // path UI properties

        internal bool ShowPathLayers
        {
            get { return _showPathLayers; }
            set { SetPropertyField(nameof(ShowPathLayers), ref _showPathLayers, value); }
        }

        internal PathType PathType
        {
            get { return _pathType; }
            set { SetPropertyField(nameof(PathType), ref _pathType, value); }
        }

        internal int PathWidth
        {
            get { return _pathWidth; }
            set { SetPropertyField(nameof(PathWidth), ref _pathWidth, value); }
        }

        internal Color PathColor
        {
            get { return _pathColor; }
            set { SetPropertyField(nameof(PathColor), ref _pathColor, value); }
        }

        internal bool EditPathPoints
        {
            get { return _editPathPoints; }
            set { SetPropertyField(nameof(EditPathPoints), ref _editPathPoints, value); }
        }

        internal bool DrawOverSymbols
        {
            get { return _drawOverSymbols; }
            set { SetPropertyField(nameof(DrawOverSymbols), ref _drawOverSymbols, value); }
        }

        internal List<MapTexture> PathTextureList
        {
            get { return _pathTextureList; }
        }

        internal List<MapVector> PathVectorList
        {
            get { return _pathVectorList; }
        }

        internal int PathTextureIndex
        {
            get { return _pathTextureIndex; }
            set { SetPropertyField(nameof(PathTextureIndex), ref _pathTextureIndex, value); }
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
            if (MapStateMediator.CurrentMap != null)
            {
                UpdatePathUI(changedPropertyName);
                SymbolManager.Update();

                MainForm.SKGLRenderControl.Invalidate();
            }
        }

        internal void UpdatePathUI(string? changedPropertyName)
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);

                pathLowerLayer.ShowLayer = ShowPathLayers;
                pathUpperLayer.ShowLayer = ShowPathLayers;

                SetSelectedPathType(PathType);

                MainForm.PathColorSelectButton.BackColor = PathColor;

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    if (changedPropertyName == "PathTextureIndex")
                    {
                        UpdatePathTextureComboBox();
                    }
                    else if (changedPropertyName == "PathWidth")
                    {
                        MainForm.PathWidthTrack.Value = PathWidth;
                    }
                }
            }));
        }

        private void UpdatePathTextureComboBox()
        {
            if (PathTextureIndex < 0)
            {
                PathTextureIndex = 0;
            }

            if (PathTextureIndex > PathTextureList.Count - 1)
            {
                PathTextureIndex = PathTextureList.Count - 1;
            }

            if (PathTextureList[PathTextureIndex].TextureBitmap == null)
            {
                PathTextureList[PathTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(PathTextureList[PathTextureIndex].TexturePath);
            }

            MainForm.PathTexturePreviewPicture.Image = PathTextureList[PathTextureIndex].TextureBitmap;
            MainForm.PathTextureNameLabel.Text = PathTextureList[PathTextureIndex].TextureName;
        }

        #endregion

        #region EventHandlers

        private void PathUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Path UI methods

        internal void Reset()
        {
            PathType = PathType.SolidLinePath;
            PathWidth = 0;
            PathColor = Color.FromArgb(75, 49, 26);
            EditPathPoints = false;
            DrawOverSymbols = false;
            SetSelectedPathType(PathType);
        }

        private void SetSelectedPathType(PathType pathType)
        {
            if (pathType == PathType.SolidLinePath) { MainForm.SolidLineRadio.Checked = true; return; }
            if (pathType == PathType.DottedLinePath) { MainForm.DottedLineRadio.Checked = true; return; }
            if (pathType == PathType.DashedLinePath) { MainForm.DashedLineRadio.Checked = true; return; }
            if (pathType == PathType.DashDotLinePath) { MainForm.DashDotLineRadio.Checked = true; return; }
            if (pathType == PathType.DashDotDotLinePath) { MainForm.DashDotDotLineRadio.Checked = true; return; }
            if (pathType == PathType.ChevronLinePath) { MainForm.ChevronLineRadio.Checked = true; return; }
            if (pathType == PathType.LineAndDashesPath) { MainForm.LineAndDashesRadio.Checked = true; return; }
            if (pathType == PathType.ShortIrregularDashPath) { MainForm.SmallDashesRadio.Checked = true; return; }
            if (pathType == PathType.ThickSolidLinePath) { MainForm.ThickLineRadio.Checked = true; return; }
            if (pathType == PathType.SolidBlackBorderPath) { MainForm.BlackBorderPathRadio.Checked = true; return; }
            if (pathType == PathType.BorderedGradientPath) { MainForm.BorderedGradientRadio.Checked = true; return; }
            if (pathType == PathType.BorderedLightSolidPath) { MainForm.BorderedLightSolidRadio.Checked = true; return; }
            if (pathType == PathType.DoubleSolidBorderPath) { MainForm.DoubleSolidBorderRadio.Checked = true; return; }
            if (pathType == PathType.BearTracksPath) { MainForm.BearTracksRadio.Checked = true; return; }
            if (pathType == PathType.BirdTracksPath) { MainForm.BirdTracksRadio.Checked = true; return; }
            if (pathType == PathType.FootprintsPath) { MainForm.FootPrintsRadio.Checked = true; return; }
            if (pathType == PathType.RailroadTracksPath) { MainForm.RailroadTracksRadio.Checked = true; return; }
            if (pathType == PathType.TexturedPath) { MainForm.TexturePathRadio.Checked = true; return; }
            if (pathType == PathType.BorderAndTexturePath) { MainForm.BorderTexturePathRadio.Checked = true; return; }
        }

        internal void SetPathTypeFromButtonName(string buttonname)
        {
            PathType = buttonname switch
            {
                "SolidLineRadio" => PathType.SolidLinePath,
                "DottedLineRadio" => PathType.DottedLinePath,
                "DashedLineRadio" => PathType.DashedLinePath,
                "DashDotLineRadio" => PathType.DashDotLinePath,
                "DashDotDotLineRadio" => PathType.DashDotDotLinePath,
                "DoubleSolidBorderRadio" => PathType.DoubleSolidBorderPath,
                "ChevronLineRadio" => PathType.ChevronLinePath,
                "LineAndDashesRadio" => PathType.LineAndDashesPath,
                "SmallDashesRadio" => PathType.ShortIrregularDashPath,
                "ThickLineRadio" => PathType.ThickSolidLinePath,
                "BlackBorderPathRadio" => PathType.SolidBlackBorderPath,
                "BorderedGradientRadio" => PathType.BorderedGradientPath,
                "BorderedLightSolidRadio" => PathType.BorderedLightSolidPath,
                "BearTracksRadio" => PathType.BearTracksPath,
                "BirdTracksRadio" => PathType.BirdTracksPath,
                "FootPrintsRadio" => PathType.FootprintsPath,
                "RailroadTracksRadio" => PathType.RailroadTracksPath,
                "TexturePathRadio" => PathType.TexturedPath,
                "BorderTexturePathRadio" => PathType.BorderAndTexturePath,
                _ => PathType.SolidLinePath,
            };
        }

        internal void SelectOrEditPaths()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MapStateMediator.MainUIMediator.SelectedBrushSize = 0;

            if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathSelect)
            {
                if (EditPathPoints)
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }
                }
                else
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathEdit)
            {
                if (EditPathPoints)
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }
                }
                else
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else
            {
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }
            }

            if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.None)
            {
                MapStateMediator.SelectedMapPath = null;
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }
            }

            MapStateMediator.MainUIMediator.SetDrawingMode(MapStateMediator.MainUIMediator.CurrentDrawingMode, MapStateMediator.MainUIMediator.SelectedBrushSize);
        }

        #endregion

        #region Static Map Path UI methods

        internal static MapPath? SelectMapPathAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            MapPath? selectedMapPath = null;

            List<MapComponent> mapPathUpperComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER).MapLayerComponents;

            for (int i = 0; i < mapPathUpperComponents.Count; i++)
            {
                if (mapPathUpperComponents[i] is MapPath mapPath)
                {
                    SKPath? boundaryPath = mapPath.BoundaryPath;

                    if (boundaryPath.PointCount > 0)
                    {
                        boundaryPath.GetBounds(out SKRect boundRect);

                        if (boundRect.Contains(mapClickPoint))
                        {
                            mapPath.IsSelected = !mapPath.IsSelected;
                            if (mapPath.IsSelected)
                            {
                                selectedMapPath = mapPath;
                            }
                            break;
                        }
                    }
                    else
                    {
                        mapPathUpperComponents.Remove(mapPath);
                    }
                }
            }

            List<MapComponent> mapPathLowerComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER).MapLayerComponents;

            for (int i = 0; i < mapPathLowerComponents.Count; i++)
            {
                if (mapPathLowerComponents[i] is MapPath mapPath)
                {
                    SKPath? boundaryPath = mapPath.BoundaryPath;

                    if (boundaryPath.PointCount > 0)
                    {
                        boundaryPath.GetBounds(out SKRect boundRect);

                        if (boundRect.Contains(mapClickPoint))
                        {
                            mapPath.IsSelected = !mapPath.IsSelected;
                            if (mapPath.IsSelected)
                            {
                                selectedMapPath = mapPath;
                            }
                            break;
                        }
                    }
                    else
                    {
                        mapPathLowerComponents.Remove(mapPath);
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedMapPath);
            return selectedMapPath;
        }

        internal static void ClearPathPointSelection()
        {
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
            foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }

            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
            foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }
        }

        internal static void SetShowPathPoints()
        {
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
            foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
            {
                if (mp.IsSelected)
                {
                    mp.ShowPathPoints = true;
                }
            }

            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
            foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
            {
                if (mp.IsSelected)
                {
                    mp.ShowPathPoints = true;
                }
            }
        }

        internal void EnableDisablePathSelection()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            if (MapStateMediator.MainUIMediator.CurrentDrawingMode != MapDrawingMode.PathSelect && MapStateMediator.MainUIMediator.CurrentDrawingMode != MapDrawingMode.PathEdit)
            {
                MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.PathSelect, 0);
            }
            else
            {
                MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);
            }

            if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathSelect)
            {
                if (EditPathPoints)
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;
                    SetShowPathPoints();
                }
                else
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;
                    ClearPathPointSelection();
                }
            }
            else if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathEdit)
            {
                if (EditPathPoints)
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;
                    SetShowPathPoints();


                }
                else
                {
                    MapStateMediator.MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;
                    ClearPathPointSelection();
                }
            }
            else
            {
                ClearPathPointSelection();
            }

            if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.None)
            {
                MapStateMediator.SelectedMapPath = null;
                ClearPathPointSelection();
            }

            MapStateMediator.MainUIMediator.SetDrawingMode(MapStateMediator.MainUIMediator.CurrentDrawingMode, MapStateMediator.MainUIMediator.SelectedBrushSize);
        }

        #endregion
    }
}
