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
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;

namespace RealmStudio
{
    internal sealed class MapBuilder
    {
        public static readonly Color DEFAULT_BACKGROUND_COLOR = Color.Transparent;
        public const ushort MAP_DEFAULT_WIDTH = 1920;
        public const ushort MAP_DEFAULT_HEIGHT = 1080;

        public const int BASELAYER = 0;
        public const int OCEANTEXTURELAYER = 1;
        public const int OCEANTEXTUREOVERLAYLAYER = 2;
        public const int OCEANDRAWINGLAYER = 3;
        public const int WINDROSELAYER = 4;
        public const int ABOVEOCEANGRIDLAYER = 5;
        public const int LANDCOASTLINELAYER = 6;
        public const int LANDFORMLAYER = 7;
        public const int LANDDRAWINGLAYER = 8;
        public const int WATERLAYER = 9;
        public const int WATERDRAWINGLAYER = 10;
        public const int BELOWSYMBOLSGRIDLAYER = 11;
        public const int PATHLOWERLAYER = 12;
        public const int SYMBOLLAYER = 13;
        public const int PATHUPPERLAYER = 14;
        public const int REGIONLAYER = 15;
        public const int REGIONOVERLAYLAYER = 16;
        public const int DEFAULTGRIDLAYER = 17;
        public const int BOXLAYER = 18;
        public const int LABELLAYER = 19;
        public const int OVERLAYLAYER = 20;
        public const int FRAMELAYER = 21;
        public const int MEASURELAYER = 22;
        public const int DRAWINGLAYER = 23;
        public const int VIGNETTELAYER = 24;
        public const int SELECTIONLAYER = 25;
        public const int HEIGHTMAPLAYER = 26;
        public const int WORKLAYER = 27;
        public const int WORKLAYER2 = 28;

        public static readonly int MAP_LAYER_COUNT = WORKLAYER2 + 1;

        // layer static methods
        public static MapLayer GetMapLayerByIndex(RealmStudioMap map, int layerIndex)
        {
            return map.MapLayers[layerIndex];
        }

        public static MapLayer? GetMapLayerByName(RealmStudioMap map, string layerName)
        {
            return map.MapLayers.FirstOrDefault(l => l.MapLayerName.Equals(layerName, StringComparison.OrdinalIgnoreCase));
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

        private static MapLayer ConstructMapLayer(string layerName, int layerOrder, int width, int height, bool drawable, GRContext grContext)
        {
            SKImageInfo imageInfo = new(width, height);

            MapLayer ml = new()
            {
                MapLayerName = layerName,
                MapLayerOrder = layerOrder,
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                ShowLayer = true,
                Drawable = drawable,
                LayerSurface = SKSurface.Create(grContext, false, imageInfo)
            };

            return ml;
        }

        public static RealmStudioMap CreateMap(RealmStudioMap currentMap, GRContext grContext)
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

            CreateMapLayers(ref map, grContext);

            if (MAP_LAYER_COUNT != map.MapLayers.Count)
            {
                throw new Exception("Error constructing map. Map layer count error");
            }

            return map;
        }

        internal static RealmStudioMap CreateMap(string mapPath, string mapName, int width, int height, GRContext grContext)
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

            CreateMapLayers(ref map, grContext);

            if (MAP_LAYER_COUNT != map.MapLayers.Count)
            {
                throw new Exception("Error constructing map. Map layer count error");
            }

            return map;
        }

        private static void CreateMapLayers(ref RealmStudioMap map, GRContext grContext)
        {
            // create the map layers and add them to the map
            MapLayer layer = ConstructMapLayer("base", BASELAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("oceantexture", OCEANTEXTURELAYER, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("oceantextureoverlay", OCEANTEXTUREOVERLAYLAYER, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("oceandrawing", OCEANDRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("windrose", WINDROSELAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("aboveoceangrid", ABOVEOCEANGRIDLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("coastline", LANDCOASTLINELAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("landform", LANDFORMLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("landdrawing", LANDDRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("water", WATERLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("waterdrawing", WATERDRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("belowsymbolsgrid", BELOWSYMBOLSGRIDLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("pathlower", PATHLOWERLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("symbols", SYMBOLLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("pathupper", PATHUPPERLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("region", REGIONLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("regionoverlay", REGIONOVERLAYLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("grid", DEFAULTGRIDLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("boxes", BOXLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("labels", LABELLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("overlay", OVERLAYLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("frame", FRAMELAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("measures", MEASURELAYER, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("userdrawing", DRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("vignette", VIGNETTELAYER, map.MapWidth, map.MapHeight, true, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("selection", SELECTIONLAYER, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("heightmap", HEIGHTMAPLAYER, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("work", WORKLAYER, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);

            layer = ConstructMapLayer("work2", WORKLAYER2, map.MapWidth, map.MapHeight, false, grContext);
            map.MapLayers.Add(layer);
        }

        public static void ConstructMissingLayersForMap(RealmStudioMap map, GRContext grContext)
        {
            List<string> allLayerNames = ["base", "oceantexture", "oceantextureoverlay", "oceandrawing", "windrose", "aboveoceangridlayer",
            "coastline","landform","landdrawing","water","waterdrawing","belowsymbolsgrid","pathlower","symbols","pathupper","region",
            "regionoverlay", "grid","boxes","labels","overlay","frame","measures","userdrawing","vignette","selection","heightmap","work", "work2"];

            List<string> mapLayerNames = [];
            for (int i = 0; i < map.MapLayers.Count; i++)
            {
                mapLayerNames.Add(map.MapLayers[i].MapLayerName);
            }

            foreach (string layerName in mapLayerNames)
            {
                allLayerNames.Remove(layerName);
            }

            for (int i = 0; i < allLayerNames.Count; i++)
            {
                string layerName = allLayerNames[i];

                switch (layerName)
                {
                    case "base":
                        {
                            MapLayer layer = ConstructMapLayer("base", BASELAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "oceantexture":
                        {
                            MapLayer layer = ConstructMapLayer("oceantexture", OCEANTEXTURELAYER, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "oceantextureoverlay":
                        {
                            MapLayer layer = ConstructMapLayer("oceantextureoverlay", OCEANTEXTUREOVERLAYLAYER, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "oceandrawing":
                        {
                            MapLayer layer = ConstructMapLayer("oceandrawing", OCEANDRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "windrose":
                        {
                            MapLayer layer = ConstructMapLayer("windrose", WINDROSELAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "aboveoceangridlayer":
                        {
                            MapLayer layer = ConstructMapLayer("aboveoceangridlayer", ABOVEOCEANGRIDLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "coastline":
                        {
                            MapLayer layer = ConstructMapLayer("coastline", LANDCOASTLINELAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "landform":
                        {
                            MapLayer layer = ConstructMapLayer("landform", LANDFORMLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "landdrawing":
                        {
                            MapLayer layer = ConstructMapLayer("landdrawing", LANDDRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "water":
                        {
                            MapLayer layer = ConstructMapLayer("water", WATERLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "waterdrawing":
                        {
                            MapLayer layer = ConstructMapLayer("waterdrawing", WATERDRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "belowsymbolsgrid":
                        {
                            MapLayer layer = ConstructMapLayer("belowsymbolsgrid", BELOWSYMBOLSGRIDLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "pathlower":
                        {
                            MapLayer layer = ConstructMapLayer("pathlower", PATHLOWERLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "symbols":
                        {
                            MapLayer layer = ConstructMapLayer("symbols", SYMBOLLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "pathupper":
                        {
                            MapLayer layer = ConstructMapLayer("pathupper", PATHUPPERLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "region":
                        {
                            MapLayer layer = ConstructMapLayer("region", REGIONLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "regionoverlay":
                        {
                            MapLayer layer = ConstructMapLayer("regionoverlay", REGIONOVERLAYLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "grid":
                        {
                            MapLayer layer = ConstructMapLayer("grid", DEFAULTGRIDLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "boxes":
                        {
                            MapLayer layer = ConstructMapLayer("boxes", BOXLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "labels":
                        {
                            MapLayer layer = ConstructMapLayer("labels", LABELLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "overlay":
                        {
                            MapLayer layer = ConstructMapLayer("overlay", OVERLAYLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "frame":
                        {
                            MapLayer layer = ConstructMapLayer("frame", FRAMELAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "measures":
                        {
                            MapLayer layer = ConstructMapLayer("measures", MEASURELAYER, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "userdrawing":
                        {
                            MapLayer layer = ConstructMapLayer("userdrawing", DRAWINGLAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "vignette":
                        {
                            MapLayer layer = ConstructMapLayer("vignette", VIGNETTELAYER, map.MapWidth, map.MapHeight, true, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "selection":
                        {
                            MapLayer layer = ConstructMapLayer("selection", SELECTIONLAYER, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "heightmap":
                        {
                            MapLayer layer = ConstructMapLayer("heightmap", HEIGHTMAPLAYER, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "work":
                        {
                            MapLayer layer = ConstructMapLayer("work", WORKLAYER, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    case "work2":
                        {
                            MapLayer layer = ConstructMapLayer("work2", WORKLAYER2, map.MapWidth, map.MapHeight, false, grContext);
                            map.MapLayers.Add(layer);
                        }
                        break;
                    default:
                        {
                            throw new Exception("Unknown map layer identifer");
                        }
                }

                map.MapLayers = [.. map.MapLayers.OrderBy(o => o.MapLayerOrder)];
            }
        }

        public static void DisposeMap(RealmStudioMap map)
        {
            foreach (var layer in map.MapLayers)
            {
                if (layer.MapLayerName == "landform")
                {
                    foreach (MapComponent mc in layer.MapLayerComponents)
                    {
                        if (mc is Landform lf)
                        {
                            lf.LandformRenderSurface?.Dispose();
                            lf.LandformRenderSurface = null;
                            lf.CoastlineRenderSurface?.Dispose();
                            lf.CoastlineRenderSurface = null;
                        }
                        else if (mc is LayerPaintStroke lps)
                        {
                            lps.RenderComponent = false;
                            lps.RenderSurface?.Dispose();
                            lps.RenderSurface = null;
                        }
                    }
                }
                else if (layer.MapLayerName == "landdrawing")
                {
                    foreach (MapComponent mc in layer.MapLayerComponents)
                    {
                        if (mc is LayerPaintStroke lps)
                        {
                            lps.RenderComponent = false;
                            lps.RenderSurface?.Dispose();
                            lps.RenderSurface = null;
                        }
                    }
                }
                else if (layer.MapLayerName == "oceandrawing")
                {
                    foreach (MapComponent mc in layer.MapLayerComponents)
                    {
                        if (mc is LayerPaintStroke lps)
                        {
                            lps.RenderComponent = false;
                            lps.RenderSurface?.Dispose();
                            lps.RenderSurface = null;
                        }
                    }
                }
                else if (layer.MapLayerName == "waterdrawing")
                {
                    foreach (MapComponent mc in layer.MapLayerComponents)
                    {
                        if (mc is LayerPaintStroke lps)
                        {
                            lps.RenderComponent = false;
                            lps.RenderSurface?.Dispose();
                            lps.RenderSurface = null;
                        }
                    }
                }
                else if (layer.MapLayerName == "vignette")
                {
                    foreach (MapComponent mc in layer.MapLayerComponents)
                    {
                        if (mc is MapVignette mv)
                        {
                            mv.VignetteRenderSurface?.Dispose();
                            mv.VignetteRenderSurface = null;
                        }
                    }
                }

                layer.RenderComponent = false;
                layer.LayerSurface?.Dispose();
                layer.LayerSurface = null;
            }
        }
    }
}
