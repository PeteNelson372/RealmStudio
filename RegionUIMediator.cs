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
    internal sealed class RegionUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        private Color _regionColor = Color.FromArgb(0, 86, 179);
        private int _regionBorderWidth = 8;
        private int _regionBorderSmoothing = 20;
        private int _regionInnerOpacity = 64;
        private PathType _regionBorderType;
        private bool _showRegions = true;

        public RegionUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += RegionUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        public Color RegionColor
        {
            get { return _regionColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(RegionColor), ref _regionColor, value);
                }
            }
        }

        public int RegionBorderWidth
        {
            get { return _regionBorderWidth; }
            set { SetPropertyField(nameof(RegionBorderWidth), ref _regionBorderWidth, value); }
        }

        public int RegionBorderSmoothing
        {
            get { return _regionBorderSmoothing; }
            set { SetPropertyField(nameof(RegionBorderSmoothing), ref _regionBorderSmoothing, value); }
        }

        public int RegionInnerOpacity
        {
            get { return _regionInnerOpacity; }
            set { SetPropertyField(nameof(RegionInnerOpacity), ref _regionInnerOpacity, value); }
        }

        public PathType RegionBorderType
        {
            get { return _regionBorderType; }
            set { SetPropertyField(nameof(RegionBorderType), ref _regionBorderType, value); }
        }

        public bool ShowRegions
        {
            get { return _showRegions; }
            set { SetPropertyField(nameof(ShowRegions), ref _showRegions, value); }
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
            UpdateRegionUI();
            RegionManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateRegionUI()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.RegionColorSelectButton.BackColor = RegionColor;

                switch (RegionBorderType)
                {
                    case PathType.SolidLinePath:
                        {
                            MainForm.RegionSolidBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.DottedLinePath:
                        {
                            MainForm.RegionDottedBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.DashedLinePath:
                        {
                            MainForm.RegionDashBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.DashDotLinePath:
                        {
                            MainForm.RegionDashDotBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.DashDotDotLinePath:
                        {
                            MainForm.RegionDashDotDotBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.DoubleSolidBorderPath:
                        {
                            MainForm.RegionDoubleSolidBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.LineAndDashesPath:
                        {
                            MainForm.RegionSolidAndDashesBorderRadio.Checked = true;
                        }
                        break;
                    case PathType.BorderedGradientPath:
                        {
                            MainForm.RegionBorderedGradientRadio.Checked = true;
                        }
                        break;
                    case PathType.BorderedLightSolidPath:
                        {
                            MainForm.RegionBorderedLightSolidRadio.Checked = true;
                        }
                        break;
                    default:
                        {
                            MainForm.RegionSolidBorderRadio.Checked = true;
                        }
                        break;
                }

                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.REGIONLAYER);
                MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.REGIONOVERLAYLAYER);

                regionLayer.ShowLayer = ShowRegions;
                regionOverlayLayer.ShowLayer = ShowRegions;
            }));
        }

        #endregion

        #region Event Handlers
        private void RegionUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion

        #region Map Region UI methods

        internal void Reset()
        {
            RegionColor = Color.FromArgb(0, 86, 179);
            RegionBorderWidth = 8;
            RegionBorderSmoothing = 20;
            RegionInnerOpacity = 64;
            RegionBorderType = PathType.SolidLinePath;
        }

        internal static MapRegion? SelectRegionAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            MapRegion? selectedRegion = null;

            List<MapComponent> mapRegionComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER).MapLayerComponents;

            for (int i = 0; i < mapRegionComponents.Count; i++)
            {
                if (mapRegionComponents[i] is MapRegion mapRegion)
                {
                    SKPath? boundaryPath = mapRegion.BoundaryPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                        {
                            mapRegion.IsSelected = !mapRegion.IsSelected;

                            if (mapRegion.IsSelected)
                            {
                                selectedRegion = mapRegion;
                            }
                            break;
                        }
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(map, selectedRegion);

            MapStateMediator.CurrentMapRegion = selectedRegion;

            return selectedRegion;
        }

        #endregion
    }
}
