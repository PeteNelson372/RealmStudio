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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using RealmStudio.Properties;
using SkiaSharp;
using System.Xml.Linq;

namespace RealmStudio
{
    internal class AssetManager
    {
        public static Cursor? EYEDROPPER_CURSOR;

        public static readonly List<MapTexture> BACKGROUND_TEXTURE_LIST = [];
        public static List<MapTexture> WATER_TEXTURE_LIST { get; set; } = [];
        public static List<MapTexture> LAND_TEXTURE_LIST { get; set; } = [];
        public static List<MapTexture> HATCH_TEXTURE_LIST { get; set; } = [];

        public static List<MapBox> MAP_BOX_TEXTURE_LIST = [];

        public static List<LabelPreset> LABEL_PRESETS = [];

        public static List<MapFrame> MAP_FRAME_TEXTURES { get; set; } = [];

        public static List<MapTexture> PATH_TEXTURE_LIST { get; set; } = [];
        public static List<MapVector> PATH_VECTOR_LIST { get; set; } = [];

        public static readonly List<ApplicationIcon> APPLICATION_ICON_LIST = [];

        public static readonly List<MapBrush> BRUSH_LIST = [];
        public static readonly List<MapTheme> THEME_LIST = [];

        public static List<MapSymbolCollection> MAP_SYMBOL_COLLECTIONS = [];

        // the symbols read from symbol collections
        public static List<MapSymbol> MAP_SYMBOL_LIST { get; set; } = [];

        // the tags that can be selected in the UI to filter the tags in the tag list box on the UI
        public static readonly List<string> ORIGINAL_SYMBOL_TAGS = [];
        public static readonly List<string> SYMBOL_TAGS = [];

        public static MapTheme? CURRENT_THEME { get; set; } = null;

        public static int SELECTED_LAND_TEXTURE_INDEX { get; set; } = 0;
        public static int SELECTED_BACKGROUND_TEXTURE_INDEX { get; set; } = 0;
        public static int SELECTED_OCEAN_TEXTURE_INDEX { get; set; } = 0;
        public static int SELECTED_PATH_TEXTURE_INDEX { get; set; } = 0;

        public static readonly string DefaultSymbolDirectory = Resources.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Symbols";

        private static readonly string SymbolTagsFilePath = DefaultSymbolDirectory + Path.DirectorySeparatorChar + "SymbolTags.txt";

        private static readonly string CollectionFileName = "collection.xml";

        private static readonly string WonderdraftSymbolsFileName = ".wonderdraft_symbols";

        internal static int LoadAllAssets()
        {
            string assetDirectory = Resources.ASSET_DIRECTORY;

            ResetAssets();

            EYEDROPPER_CURSOR = new Cursor(Resources.Eye_Dropper.Handle);

            // load symbol tags
            LoadSymbolTags();

            // load name generator files
            MapToolMethods.LoadNameGeneratorFiles();

            // load assets
            int numAssets = 0;

            var files = from file in Directory.EnumerateFiles(assetDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".png")
                            || file.Contains(".jpg")
                            || file.Contains(".ico")
                            || file.Contains(".mctheme")
                            || file.Contains(".svg")
                            || file.Contains(".mclblprst")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                string assetName = Path.GetFileNameWithoutExtension(f.File);
                string path = Path.GetFullPath(f.File);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Background"))
                {
                    MapTexture t = new(assetName, path);
                    BACKGROUND_TEXTURE_LIST.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Water"))
                {
                    MapTexture t = new(assetName, path);
                    WATER_TEXTURE_LIST.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Land"))
                {
                    MapTexture t = new(assetName, path);
                    LAND_TEXTURE_LIST.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Hatch"))
                {
                    MapTexture t = new(assetName, path);
                    HATCH_TEXTURE_LIST.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Path"))
                {
                    MapTexture t = new(assetName, path);
                    PATH_TEXTURE_LIST.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Vectors\\Path"))
                {
                    MapVector v = new(assetName, path);

                    XDocument svgXmlRoot = XDocument.Load(v.VectorPath, LoadOptions.None);

                    if (svgXmlRoot != null)
                    {
                        XElement? rootElement = svgXmlRoot.Root;

                        if (rootElement != null)
                        {
                            string? viewBoxAttr = (string?)rootElement.Attribute("viewBox");

                            if (viewBoxAttr != null)
                            {
                                string[] viewBoxElements = viewBoxAttr.Split(' ');

                                v.ViewBoxSizeWidth = float.Parse(viewBoxElements[2]);
                                v.ViewBoxSizeHeight = float.Parse(viewBoxElements[3]);
                            }
                            else
                            {
                                v.ViewBoxSizeWidth = 0.0F;
                                v.ViewBoxSizeHeight = 0.0F;
                            }

                            var nodes = rootElement.Descendants();

                            foreach (var n in nodes)
                            {
                                if (n.Name.LocalName == "path")
                                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                                    v.VectorSvg += " " + (string)n.Attribute("d");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                                }
                            }

                            if (v.VectorSvg != null && v.VectorSvg.Length > 0)
                            {
                                v.VectorSkPath = SKPath.ParseSvgPathData(v.VectorSvg);
                            }

                            PATH_VECTOR_LIST.Add(v);
                        }

                    }
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Brushes"))
                {
                    MapBrush mb = new() { BrushPath = path, BrushName = assetName };
                    BRUSH_LIST.Add(mb);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Icons"))
                {
                    ApplicationIcon icon = new()
                    {
                        IconName = assetName,
                        IconPath = path,
                        IconCursor = new Cursor(path)
                    };

                    APPLICATION_ICON_LIST.Add(icon);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Themes"))
                {
                    MapTheme? t = MapFileMethods.ReadThemeFromXml(path);

                    if (t != null)
                    {
                        THEME_LIST.Add(t);

                        if (t.IsDefaultTheme)
                        {
                            CURRENT_THEME = t;
                        }
                    }
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Stamps"))
                {
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Boxes"))
                {
                    MapBox b = new()
                    {
                        BoxName = assetName,
                        BoxBitmapPath = path
                    };

                    MAP_BOX_TEXTURE_LIST.Add(b);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\LabelPresets"))
                {
                    LabelPreset? preset = MapFileMethods.ReadLabelPreset(path);

                    if (preset != null)
                    {
                        LABEL_PRESETS.Add(preset);
                    }
                }

#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            numAssets += files.Count();

            int numSymbols = LoadSymbolCollections();

            numAssets += numSymbols;

            int numFrames = LoadFrameAssets();
            numAssets += numFrames;

            return numAssets;
        }

        private static void ResetAssets()
        {
            BACKGROUND_TEXTURE_LIST.Clear();
            WATER_TEXTURE_LIST.Clear();
            LAND_TEXTURE_LIST.Clear();
            HATCH_TEXTURE_LIST.Clear();
            PATH_TEXTURE_LIST.Clear();
            PATH_VECTOR_LIST.Clear();
            BRUSH_LIST.Clear();
            APPLICATION_ICON_LIST.Clear();
            THEME_LIST.Clear();
            LABEL_PRESETS.Clear();

            MAP_SYMBOL_COLLECTIONS.Clear();
        }

        internal static int LoadSymbolCollections()
        {
            int numSymbols = 0;
            try
            {
                var files = from file in Directory.EnumerateFiles(DefaultSymbolDirectory, "*.*", SearchOption.AllDirectories).Order()
                            where file.EndsWith(CollectionFileName)
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    MapSymbolCollection? collection = MapFileMethods.ReadCollectionFromXml(f.File);

                    if (collection != null)
                    {
                        MAP_SYMBOL_COLLECTIONS.Add(collection);

                        // load symbol file into object
                        foreach (MapSymbol symbol in collection.GetCollectionMapSymbols())
                        {
                            numSymbols++;

                            if (!string.IsNullOrEmpty(symbol.SymbolFilePath))
                            {
                                if (symbol.SymbolFormat == SymbolFormatEnum.PNG
                                    || symbol.SymbolFormat == SymbolFormatEnum.JPG
                                    || symbol.SymbolFormat == SymbolFormatEnum.BMP)
                                {
                                    symbol.SetSymbolBitmapFromPath(symbol.SymbolFilePath);
                                }

                                MAP_SYMBOL_LIST.Add(symbol);

                                foreach (string tag in symbol.SymbolTags)
                                {
                                    SymbolMethods.AddTagSymbolAssocation(tag, symbol);
                                }
                            }

                            if (string.IsNullOrEmpty(symbol.CollectionPath))
                            {
                                symbol.CollectionPath = collection.GetCollectionPath();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading symbols from collection: " + ex.Message, "Error Loading Symbols", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            return numSymbols;
        }

        internal static int LoadFrameAssets()
        {
            MAP_FRAME_TEXTURES.Clear();

            string frameAssetDirectory = Resources.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Frames" + Path.DirectorySeparatorChar;

            int numFrames = 0;
            var files = from file in Directory.EnumerateFiles(frameAssetDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".xml")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                MapFrame? mapFrame = MapFileMethods.ReadFrameAssetFromXml(f.File);

                if (mapFrame != null)
                {
                    numFrames++;
                    MAP_FRAME_TEXTURES.Add(mapFrame);
                }

            }

            return numFrames;
        }

        public static void LoadSymbolTags()
        {
            SYMBOL_TAGS.Clear();
            ORIGINAL_SYMBOL_TAGS.Clear();

            IEnumerable<string> tags = File.ReadLines(SymbolTagsFilePath);
            foreach (string tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    AddSymbolTag(tag);
                }
            }

            SYMBOL_TAGS.Sort();

            foreach (string tag in SYMBOL_TAGS)
            {
                ORIGINAL_SYMBOL_TAGS.Add(tag);
            }
        }

        public static void AddSymbolTag(string tag)
        {
            tag = tag.Trim([' ', ',']).ToLower();
            if (SYMBOL_TAGS.Contains(tag)) return;
            SYMBOL_TAGS.Add(tag);
        }
    }
}
