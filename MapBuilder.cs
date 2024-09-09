/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
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
* The text of the GNU General Public License (GPL) is found in the LICENSE file.
* If the LICENSE file is not present or the text of the GNU GPL is not present in the LICENSE file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* contact@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;

namespace RealmStudio
{
    internal class MapBuilder
    {
        public static readonly Color DEFAULT_BACKGROUND_COLOR = Color.Transparent;
        public static readonly ushort MAP_DEFAULT_WIDTH = 1920;
        public static readonly ushort MAP_DEFAULT_HEIGHT = 1080;

        public static readonly int BASELAYER = 0;
        public static readonly int OCEANTEXTURELAYER = 1;
        public static readonly int OCEANTEXTUREOVERLAYLAYER = 2;
        public static readonly int OCEANDRAWINGLAYER = 3;
        public static readonly int WINDROSELAYER = 4;
        public static readonly int ABOVEOCEANGRIDLAYER = 5;
        public static readonly int LANDCOASTLINELAYER = 6;
        public static readonly int LANDFORMLAYER = 7;
        public static readonly int LANDDRAWINGLAYER = 8;
        public static readonly int WATERLAYER = 9;
        public static readonly int WATERDRAWINGLAYER = 10;
        public static readonly int BELOWSYMBOLSGRIDLAYER = 11;
        public static readonly int PATHLOWERLAYER = 12;
        public static readonly int SYMBOLLAYER = 13;
        public static readonly int PATHUPPERLAYER = 14;
        public static readonly int REGIONLAYER = 15;
        public static readonly int REGIONOVERLAYLAYER = 16;
        public static readonly int DEFAULTGRIDLAYER = 17;
        public static readonly int BOXLAYER = 18;
        public static readonly int LABELLAYER = 19;
        public static readonly int OVERLAYLAYER = 20;
        public static readonly int MEASURELAYER = 21;
        public static readonly int DRAWINGLAYER = 22;
        public static readonly int VIGNETTELAYER = 23;
        public static readonly int SELECTIONLAYER = 24;
        public static readonly int WORKLAYER = 25;

        public static readonly int MAP_LAYER_COUNT = WORKLAYER + 1;

        // layer static methods
        public static MapLayer GetMapLayerByIndex(RealmStudioMap map, int index)
        {
            return map.MapLayers[index];
        }

        public static MapLayer? GetMapLayerByName(RealmStudioMap map, string layerName)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map), "map is null");
            }

            if (string.IsNullOrEmpty(layerName))
            {
                throw new ArgumentNullException(nameof(layerName), "layerName is null or empty");
            }

            return map.MapLayers.Find(x => x.MapLayerName == layerName);
        }

        public static void ShowLayer(RealmStudioMap map, int layerIndex)
        {
            MapLayer l = GetMapLayerByIndex(map, layerIndex);
            l.ShowLayer = true;
        }

        public static void HideLayer(RealmStudioMap map, int layerIndex)
        {
            MapLayer l = GetMapLayerByIndex(map, layerIndex);
            l.ShowLayer = false;
        }

        private static MapLayer ConstructMapLayer(string layerName, ushort layerOrder, uint x, uint y, int width, int height, SKColor clearColor)
        {
            SKImageInfo imageInfo = new(width, height);

            MapLayer ml = new(imageInfo)
            {
                MapLayerName = layerName,
                MapLayerOrder = layerOrder,
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                ShowLayer = true,
            };

            return ml;
        }

        public static RealmStudioMap CreateMap(RealmStudioMap currentMap)
        {
            // create the map object
            RealmStudioMap map = new()
            {
                MapPath = currentMap.MapPath,
                MapName = currentMap.MapName,
                MapWidth = currentMap.MapWidth,
                MapHeight = currentMap.MapHeight,
                IsSaved = true,
                MapAreaWidth = currentMap.MapAreaWidth,
                MapAreaHeight = currentMap.MapAreaHeight,
                MapAreaUnits = currentMap.MapAreaUnits,
            };

            map.MapPixelWidth = map.MapAreaWidth / map.MapWidth;
            map.MapPixelHeight = map.MapAreaHeight / map.MapHeight;

            CreateMapLayers(ref map);

            if (MAP_LAYER_COUNT != map.MapLayers.Count)
            {
                throw new Exception("Error constructing map. Map layer count error");
            }

            return map;
        }

        internal static RealmStudioMap? CreateMap(string mapPath, string mapName, int width, int height)
        {
            RealmStudioMap map = new()
            {
                MapPath = mapPath,
                MapName = mapName,
                MapWidth = width,
                MapHeight = height,
                IsSaved = true,
                MapAreaWidth = width,
                MapAreaHeight = height,
                MapAreaUnits = string.Empty,
            };

            map.MapPixelWidth = map.MapAreaWidth / map.MapWidth;
            map.MapPixelHeight = map.MapAreaHeight / map.MapHeight;

            CreateMapLayers(ref map);

            if (MAP_LAYER_COUNT != map.MapLayers.Count)
            {
                throw new Exception("Error constructing map. Map layer count error");
            }
            return map;
        }

        private static void CreateMapLayers(ref RealmStudioMap map)
        {
            // create the map layers and add them to the map
            MapLayer layer = ConstructMapLayer("base", (ushort)BASELAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.White);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("oceantexture", (ushort)OCEANTEXTURELAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("oceantextureoverlay", (ushort)OCEANTEXTUREOVERLAYLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("oceandrawing", (ushort)OCEANDRAWINGLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("windrose", (ushort)WINDROSELAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("aboveoceangridlayer", (ushort)ABOVEOCEANGRIDLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("coastline", (ushort)LANDCOASTLINELAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("landform", (ushort)LANDFORMLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("landdrawing", (ushort)LANDDRAWINGLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("water", (ushort)WATERLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("waterdrawing", (ushort)WATERDRAWINGLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("belowsymbolsgrid", (ushort)BELOWSYMBOLSGRIDLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("pathlower", (ushort)PATHLOWERLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("symbols", (ushort)SYMBOLLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("pathupper", (ushort)PATHUPPERLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("region", (ushort)REGIONLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("regionoverlay", (ushort)REGIONOVERLAYLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("grid", (ushort)DEFAULTGRIDLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("boxes", (ushort)BOXLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("labels", (ushort)LABELLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("overlays", (ushort)OVERLAYLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("measures", (ushort)MEASURELAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("userdrawing", (ushort)DRAWINGLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("vignette", (ushort)VIGNETTELAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("selection", (ushort)SELECTIONLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("work", (ushort)WORKLAYER, 0, 0, map.MapWidth, map.MapHeight, SKColors.Empty);
            map.MapLayers.Add(layer);
        }

    }
}
