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
    internal sealed class MapGridManager : IMapComponentManager
    {
        private static MapGridUIMediator? _mapGridUIMediator;

        internal static MapGridUIMediator? GridUIMediator
        {
            get { return _mapGridUIMediator; }
            set { _mapGridUIMediator = value; }
        }

        // CRUD, Add and Get operations on MapGrid objects

        public static IMapComponent Create()
        {
            ArgumentNullException.ThrowIfNull(GridUIMediator);

            Delete();

            MapGrid newGrid = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
                GridType = GridUIMediator.GridType,
                GridEnabled = true,
                GridColor = GridUIMediator.GridColor,
                GridLineWidth = GridUIMediator.GridLineWidth,
                GridSize = GridUIMediator.GridSize,
                Width = MapStateMediator.CurrentMap.MapWidth,
                Height = MapStateMediator.CurrentMap.MapHeight,
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

            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, newGrid.GridLayerIndex).MapLayerComponents.Add(newGrid);
            MapStateMediator.CurrentMapGrid = newGrid;

            return MapStateMediator.CurrentMapGrid;
        }

        public static bool Delete()
        {
            // there is only one grid, so find it in the layers it could be in and delete it

            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            return true;
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            MapGrid? component = null;

            foreach (MapGrid mg in MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.Cast<MapGrid>())
            {
                if (mg.GridGuid.ToString() == componentGuid.ToString())
                {
                    component = mg;
                    break;
                }
            }

            foreach (MapGrid mg in MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.Cast<MapGrid>())
            {
                if (mg.GridGuid.ToString() == componentGuid.ToString())
                {
                    component = mg;
                    break;
                }
            }

            foreach (MapGrid mg in MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.Cast<MapGrid>())
            {
                if (mg.GridGuid.ToString() == componentGuid.ToString())
                {
                    component = mg;
                    break;
                }
            }

            return component;
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(GridUIMediator);

            if (MapStateMediator.CurrentMapGrid != null)
            {
                MapStateMediator.CurrentMapGrid.GridEnabled = GridUIMediator.GridEnabled;
                MapStateMediator.CurrentMapGrid.GridType = GridUIMediator.GridType;
                MapStateMediator.CurrentMapGrid.GridColor = GridUIMediator.GridColor;
                MapStateMediator.CurrentMapGrid.GridLineWidth = GridUIMediator.GridLineWidth;
                MapStateMediator.CurrentMapGrid.GridSize = GridUIMediator.GridSize;

                if (MapStateMediator.CurrentMapGrid.GridType == MapGridType.FlatHex || MapStateMediator.CurrentMapGrid.GridType == MapGridType.PointedHex)
                {
                    MapStateMediator.CurrentMapGrid.GridSize /= 2;
                }

                MapStateMediator.CurrentMapGrid.ShowGridSize = GridUIMediator.ShowGridSize;

                MapStateMediator.CurrentMapGrid.GridPaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = MapStateMediator.CurrentMapGrid.GridColor.ToSKColor(),
                    StrokeWidth = MapStateMediator.CurrentMapGrid.GridLineWidth,
                    StrokeJoin = SKStrokeJoin.Bevel
                };
            }

            return true;
        }

        internal static void FinalizeMapGrid()
        {
            ArgumentNullException.ThrowIfNull(GridUIMediator);

            // finalize loading of grid
            MapGrid? currentMapGrid = null;

            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DEFAULTGRIDLAYER);
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
                MapLayer oceanGridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.ABOVEOCEANGRIDLAYER);
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
                MapLayer symbolGridLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BELOWSYMBOLSGRIDLAYER);
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

            if (currentMapGrid != null)
            {
                currentMapGrid.ParentMap = MapStateMediator.CurrentMap;

                GridUIMediator.Initialize(currentMapGrid.GridEnabled, currentMapGrid.GridType, currentMapGrid.GridLayerIndex, currentMapGrid.GridSize,
                    currentMapGrid.GridLineWidth, currentMapGrid.GridColor, currentMapGrid.ShowGridSize);

                MapStateMediator.CurrentMapGrid = currentMapGrid;
            }
        }
    }
}
