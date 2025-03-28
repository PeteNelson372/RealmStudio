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
    internal class MapGridManager : IMapComponentManager
    {
        private static MapGridUIMediator? _mapGridUIMediator;

        internal static MapGridUIMediator? GridUIMediator
        {
            get { return _mapGridUIMediator; }
            set { _mapGridUIMediator = value; }
        }

        // CRUD, Add and Get operations on MapGrid objects

        public static IMapComponent Create(RealmStudioMap? map, IUIMediatorObserver? mediator)
        {
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(GridUIMediator);

            Delete(map, null);

            MapGrid newGrid = new()
            {
                ParentMap = map,
                GridType = GridUIMediator.GridType,
                GridEnabled = true,
                GridColor = GridUIMediator.GridColor,
                GridLineWidth = GridUIMediator.GridLineWidth,
                GridSize = GridUIMediator.GridSize,
                Width = map.MapWidth,
                Height = map.MapHeight,
                GridLayerIndex = GridUIMediator.GridLayerIndex
            };

            if (GridUIMediator.GridType == MapGridType.FlatHex || GridUIMediator.GridType == MapGridType.PointedHex)
            {
                newGrid.GridSize /= 2;
            }

            newGrid.GridPaint = new()
            {
                Style = SKPaintStyle.Stroke,
                Color = newGrid.GridColor.ToSKColor(),
                StrokeWidth = newGrid.GridLineWidth,
                StrokeJoin = SKStrokeJoin.Bevel
            };

            MapBuilder.GetMapLayerByIndex(map, newGrid.GridLayerIndex).MapLayerComponents.Add(newGrid);
            RealmMapState.CurrentMapGrid = newGrid;

            return RealmMapState.CurrentMapGrid;
        }

        public static bool Delete(RealmStudioMap? map, IMapComponent? component)
        {
            ArgumentNullException.ThrowIfNull(map);

            // there is only one grid, so find it in the layers it could be in and delete it

            for (int i = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            return true;
        }

        public static IMapComponent? GetComponentById(RealmStudioMap? map, Guid componentGuid)
        {
            ArgumentNullException.ThrowIfNull(map);

            MapGrid? component = null;

            foreach (MapGrid mg in MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents)
            {
                if (mg.GridGuid.ToString() == componentGuid.ToString())
                {
                    component = mg;
                    break;
                }
            }

            foreach (MapGrid mg in MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents)
            {
                if (mg.GridGuid.ToString() == componentGuid.ToString())
                {
                    component = mg;
                    break;
                }
            }

            foreach (MapGrid mg in MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents)
            {
                if (mg.GridGuid.ToString() == componentGuid.ToString())
                {
                    component = mg;
                    break;
                }
            }

            return component;
        }

        public static bool Update(RealmStudioMap? map, RealmMapState? mapState, IUIMediatorObserver? mediator)
        {
            ArgumentNullException.ThrowIfNull(GridUIMediator);

            if (RealmMapState.CurrentMapGrid != null)
            {
                RealmMapState.CurrentMapGrid.GridEnabled = GridUIMediator.GridEnabled;
                RealmMapState.CurrentMapGrid.GridType = GridUIMediator.GridType;
                RealmMapState.CurrentMapGrid.GridColor = GridUIMediator.GridColor;
                RealmMapState.CurrentMapGrid.GridLineWidth = GridUIMediator.GridLineWidth;
                RealmMapState.CurrentMapGrid.GridSize = GridUIMediator.GridSize;

                if (RealmMapState.CurrentMapGrid.GridType == MapGridType.FlatHex || RealmMapState.CurrentMapGrid.GridType == MapGridType.PointedHex)
                {
                    RealmMapState.CurrentMapGrid.GridSize /= 2;
                }

                RealmMapState.CurrentMapGrid.ShowGridSize = GridUIMediator.ShowGridSize;

                RealmMapState.CurrentMapGrid.GridPaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = RealmMapState.CurrentMapGrid.GridColor.ToSKColor(),
                    StrokeWidth = RealmMapState.CurrentMapGrid.GridLineWidth,
                    StrokeJoin = SKStrokeJoin.Bevel
                };

                RealmMapState.GLRenderControl?.Invalidate();
            }

            return true;
        }

        internal static MapGrid? FinalizeMapGrid(RealmStudioMap map)
        {
            // finalize loading of grid
            MapGrid? currentMapGrid = null;

            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.DEFAULTGRIDLAYER);
            for (int i = 0; i < defaultGridLayer.MapLayerComponents.Count; i++)
            {
                if (defaultGridLayer.MapLayerComponents[i] is MapGrid grid)
                {
                    currentMapGrid = grid;
                    currentMapGrid.GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
                    currentMapGrid.GridEnabled = true;
                    break;
                }
            }

            if (currentMapGrid == null)
            {
                MapLayer oceanGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.ABOVEOCEANGRIDLAYER);
                for (int i = 0; i < oceanGridLayer.MapLayerComponents.Count; i++)
                {
                    if (oceanGridLayer.MapLayerComponents[i] is MapGrid grid)
                    {
                        currentMapGrid = grid;
                        currentMapGrid.GridLayerIndex = MapBuilder.ABOVEOCEANGRIDLAYER;
                        currentMapGrid.GridEnabled = true;
                        break;
                    }
                }
            }

            if (currentMapGrid == null)
            {
                MapLayer symbolGridLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BELOWSYMBOLSGRIDLAYER);
                for (int i = 0; i < symbolGridLayer.MapLayerComponents.Count; i++)
                {
                    if (symbolGridLayer.MapLayerComponents[i] is MapGrid grid)
                    {
                        currentMapGrid = grid;
                        currentMapGrid.GridLayerIndex = MapBuilder.BELOWSYMBOLSGRIDLAYER;
                        currentMapGrid.GridEnabled = true;
                        break;
                    }
                }
            }

            return currentMapGrid;
        }
    }
}
