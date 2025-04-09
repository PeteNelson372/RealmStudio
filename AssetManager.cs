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
using RealmStudio.Properties;
using SkiaSharp;
using System.IO;
using System.Xml.Linq;

namespace RealmStudio
{
    internal sealed class AssetManager
    {
        // TODO: refactor AssetManager so map object managers are responsible
        // for loading assets for the map objects they manage


        public static Cursor? EYEDROPPER_CURSOR { get; set; }

        public static List<MapTexture> WATER_TEXTURE_LIST { get; set; } = [];

        public static List<MapTexture> HATCH_TEXTURE_LIST { get; set; } = [];

        public static List<MapBox> MAP_BOX_LIST = [];

        public static List<MapFrame> MAP_FRAME_TEXTURES { get; set; } = [];

        public static readonly List<RealmStudioApplicationIcon> APPLICATION_ICON_LIST = [];

        public static readonly List<MapBrush> BRUSH_LIST = [];
        public static readonly List<MapTheme> THEME_LIST = [];

        public static readonly string DEFAULT_THEME_NAME = "Medieval Quest";

        public static List<MapSymbolCollection> MAP_SYMBOL_COLLECTIONS = [];  // TODO: move to SymbolManager

        // the symbols read from symbol collections
        public static List<MapSymbol> MAP_SYMBOL_LIST { get; set; } = [];   // TODO: move to SymbolManager

        // the tags that can be selected in the UI to filter the tags in the tag list box on the UI
        public static readonly List<string> ORIGINAL_SYMBOL_TAGS = [];
        public static readonly List<string> SYMBOL_TAGS = [];
        public static readonly List<string> STRUCTURE_SYNONYMS = [];
        public static readonly List<string> TERRAIN_SYNONYMS = [];
        public static readonly List<string> VEGETATION_SYNONYMS = [];

        public static MapTheme? CURRENT_THEME { get; set; }

        public static List<LandformShapingFunction> LANDFORM_SHAPING_FUNCTIONS = [];

        public static string ASSET_DIRECTORY = string.Empty;

        private static string SymbolTagsFilePath = Path.DirectorySeparatorChar + "SymbolTags.txt";

        private static string StructureSynonymsFilePath = Path.DirectorySeparatorChar + "StructureSynonyms.txt";
        private static string TerrainSynonymsFilePath = Path.DirectorySeparatorChar + "TerrainSynonyms.txt";
        private static string VegetationSynonymsFilePath = Path.DirectorySeparatorChar + "VegetationSynonyms.txt";

        public static string DefaultModelsDirectory = Settings.Default.DefaultRealmDirectory + Path.DirectorySeparatorChar + "Models";

        public static readonly string CollectionFileName = "collection.xml";

        public static readonly string WonderdraftSymbolsFileName = ".wonderdraft_symbols";

        public static LoadingStatusForm LOADING_STATUS_FORM = new();

        internal static void InitializeAssetDirectories()
        {
            ASSET_DIRECTORY = Settings.Default.MapAssetDirectory;

            if (string.IsNullOrEmpty(ASSET_DIRECTORY))
            {
                ASSET_DIRECTORY = UtilityMethods.DEFAULT_ASSETS_FOLDER;
            }

            DefaultModelsDirectory = Settings.Default.DefaultRealmDirectory + Path.DirectorySeparatorChar + "Models";

            SymbolTagsFilePath = SymbolManager.DefaultSymbolDirectory + Path.DirectorySeparatorChar + "SymbolTags.txt";

            StructureSynonymsFilePath = SymbolManager.DefaultSymbolDirectory + Path.DirectorySeparatorChar + "StructureSynonyms.txt";
            TerrainSynonymsFilePath = SymbolManager.DefaultSymbolDirectory + Path.DirectorySeparatorChar + "TerrainSynonyms.txt";
            VegetationSynonymsFilePath = SymbolManager.DefaultSymbolDirectory + Path.DirectorySeparatorChar + "VegetationSynonyms.txt";

            // set map object manager directories
            SymbolManager.DefaultSymbolDirectory = ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Symbols";
            LabelPresetManager.LabelPresetsDirectory = ASSET_DIRECTORY + Path.DirectorySeparatorChar + "LabelPresets";
        }

        internal static int LoadAllAssets()
        {
            int LoadPercentage = 0;

            ResetAssets();

            LOADING_STATUS_FORM.SetStatusText("Loading Symbol Tags");

            // load symbol tags
            LoadSymbolTags();

            LoadPercentage += 2;
            LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

            LOADING_STATUS_FORM.SetStatusText("Loading Symbol Type Synonyms");

            // load symbol type synonyms
            LoadSymbolTypeSynonyms();

            LoadPercentage += 2;
            LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

            LOADING_STATUS_FORM.SetStatusText("Loading Name Generators");

            // load name generator files
            MapToolMethods.LoadNameGeneratorFiles();

            LoadPercentage += 6;
            LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

            // load shaping function files
            LOADING_STATUS_FORM.SetStatusText("Loading Shaping Functions");

            MapToolMethods.LoadShapingFunctions();

            LoadPercentage += 2;
            LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

            LOADING_STATUS_FORM.SetStatusText("Loading Assets");

            // load assets
            int numAssets = 0;

            var files = from file in Directory.EnumerateFiles(ASSET_DIRECTORY, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".png")
                            || file.Contains(".jpg")
                            || file.Contains(".ico")
                            || file.Contains(".rstheme")
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

                    if (t.TexturePath != null && t.TextureBitmap == null)
                    {
                        t.TextureBitmap = new Bitmap(t.TexturePath);
                    }

                    BackgroundManager.BackgroundMediator.BackgroundTextureList.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Water"))
                {
                    MapTexture t = new(assetName, path);

                    if (t.TexturePath != null && t.TextureBitmap == null)
                    {
                        t.TextureBitmap = new Bitmap(t.TexturePath);
                    }

                    OceanManager.OceanMediator.OceanTextureList.Add(t);
                    WaterFeatureManager.WaterFeatureMediator.WaterTextureList.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Land"))
                {
                    MapTexture t = new(assetName, path);

                    if (t.TexturePath != null && t.TextureBitmap == null)
                    {
                        t.TextureBitmap = new Bitmap(t.TexturePath);
                    }

                    MapStateMediator.LandformUIMediator.LandTextureList.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Hatch"))
                {
                    MapTexture t = new(assetName, path);

                    if (t.TexturePath != null && t.TextureBitmap == null)
                    {
                        t.TextureBitmap = new Bitmap(t.TexturePath);
                    }

                    HATCH_TEXTURE_LIST.Add(t);
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("Textures\\Path"))
                {
                    MapTexture t = new(assetName, path);

                    if (t.TexturePath != null && t.TextureBitmap == null)
                    {
                        t.TextureBitmap = new Bitmap(t.TexturePath);
                    }

                    MapStateMediator.PathUIMediator.PathTextureList.Add(t);
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

                                v.ViewBoxLeft = float.Parse(viewBoxElements[0]);
                                v.ViewBoxTop = float.Parse(viewBoxElements[1]);
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

                            MapStateMediator.PathUIMediator.PathVectorList.Add(v);
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
                    RealmStudioApplicationIcon icon = new()
                    {
                        IconName = assetName,
                        IconPath = path,
                        IconCursor = new Cursor(path)
                    };

                    APPLICATION_ICON_LIST.Add(icon);

                    if (icon.IconName == "Eye Dropper")
                    {
                        // create a cursor from the icon bitmap with the hotspot at the top-left corner
                        Bitmap b = new(icon.IconPath);
                        EYEDROPPER_CURSOR = CustomCursor.CreateCursor(b, 0, 0);
                    }
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Themes"))
                {
                    MapTheme? t = MapFileMethods.ReadThemeFromXml(path);

                    if (t != null)
                    {
                        THEME_LIST.Add(t);

                        // current theme may have already been set in configuration dialog
                        if (t.IsDefaultTheme && CURRENT_THEME == null)
                        {
                            CURRENT_THEME = t;
                        }
                    }
                }
                else if (Path.GetDirectoryName(f.File).EndsWith("\\Stamps"))
                {
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            numAssets += files.Count();

            int numPresets = LabelPresetManager.LoadLabelPresets();
            numAssets += numPresets;

            int numBoxes = LoadBoxAssets();
            numAssets += numBoxes;

            int numFrames = LoadFrameAssets();
            numAssets += numFrames;

            LoadPercentage += 10;
            LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

            int numSymbols = LoadSymbolCollections();

            LoadPercentage = 100;
            LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

            numAssets += numSymbols;

            Thread.Sleep(1000);

            return numAssets;
        }

        private static void ResetAssets()
        {

            ArgumentNullException.ThrowIfNull(BackgroundManager.BackgroundMediator);
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);
            ArgumentNullException.ThrowIfNull(OceanManager.OceanMediator);
            ArgumentNullException.ThrowIfNull(PathManager.PathMediator);

            HATCH_TEXTURE_LIST.Clear();
            BRUSH_LIST.Clear();
            APPLICATION_ICON_LIST.Clear();
            THEME_LIST.Clear();

            BackgroundManager.BackgroundMediator.BackgroundTextureList.Clear();
            LandformManager.LandformMediator.LandTextureList.Clear();
            OceanManager.OceanMediator.OceanTextureList.Clear();

            PathManager.PathMediator.PathTextureList.Clear();
            PathManager.PathMediator.PathVectorList.Clear();

            LabelPresetManager.ClearLabelPresets();

            MAP_SYMBOL_COLLECTIONS.Clear();
        }

        internal static void LoadThemes()
        {
            THEME_LIST.Clear();

            string assetDirectory = Settings.Default.MapAssetDirectory;

            if (string.IsNullOrEmpty(assetDirectory))
            {
                assetDirectory = UtilityMethods.DEFAULT_ASSETS_FOLDER;
            }

            if (!Directory.Exists(assetDirectory) || (Directory.Exists(assetDirectory) && Directory.GetDirectories(assetDirectory).Length == 0))
            {
                MessageBox.Show("No map assets could be found. Realm Studio will close.", "Realm Studio Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Application.Exit();
                return;
            }

            string themeDirectory = assetDirectory + Path.DirectorySeparatorChar + "Themes";

            var files = from file in Directory.EnumerateFiles(assetDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".rstheme")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                bool rewriteTheme = false;

                string path = Path.GetFullPath(f.File);

                MapTheme? t = MapFileMethods.ReadThemeFromXml(path);

                if (t != null)
                {
                    if (!File.Exists(t.ThemePath))
                    {
                        rewriteTheme = true;
                        t.ThemePath = path;
                    }

                    if (t.BackgroundTexture != null)
                    {
                        if (!File.Exists(t.BackgroundTexture.TexturePath))
                        {
                            string fileName = System.IO.Path.GetFileName(t.BackgroundTexture.TexturePath);

                            string textureDirectory = assetDirectory + Path.DirectorySeparatorChar + "Textures"
                                + Path.DirectorySeparatorChar + "Background" + Path.DirectorySeparatorChar;

                            string bitmapPath = textureDirectory + fileName;

                            if (File.Exists(bitmapPath))
                            {
                                t.BackgroundTexture.TexturePath = bitmapPath;
                                rewriteTheme = true;
                            }
                        }
                    }

                    if (t.OceanTexture != null)
                    {
                        if (!File.Exists(t.OceanTexture.TexturePath))
                        {
                            string fileName = Path.GetFileName(t.OceanTexture.TexturePath);

                            string textureDirectory = assetDirectory + Path.DirectorySeparatorChar + "Textures"
                                + Path.DirectorySeparatorChar + "Water" + Path.DirectorySeparatorChar;

                            string bitmapPath = textureDirectory + fileName;

                            if (File.Exists(bitmapPath))
                            {
                                t.OceanTexture.TexturePath = bitmapPath;
                                rewriteTheme = true;
                            }
                        }
                    }

                    if (t.LandformTexture != null)
                    {
                        if (!File.Exists(t.LandformTexture.TexturePath))
                        {
                            string fileName = Path.GetFileName(t.LandformTexture.TexturePath);

                            string textureDirectory = assetDirectory + Path.DirectorySeparatorChar + "Textures"
                                + Path.DirectorySeparatorChar + "Land" + Path.DirectorySeparatorChar;

                            string bitmapPath = textureDirectory + fileName;

                            if (File.Exists(bitmapPath))
                            {
                                t.LandformTexture.TexturePath = bitmapPath;
                                rewriteTheme = true;
                            }
                        }
                    }

                    THEME_LIST.Add(t);

                    if (t.IsDefaultTheme)
                    {
                        CURRENT_THEME = t;
                    }

                    if (rewriteTheme)
                    {
                        MapFileMethods.SerializeTheme(t);
                    }
                }
            }
        }



        internal static int LoadSymbolCollections()
        {
            MAP_SYMBOL_COLLECTIONS.Clear();
            MAP_SYMBOL_LIST.Clear();

            int StartingLoadPercentage = LOADING_STATUS_FORM.GetStatusPercentage();
            int LoadPercentage = LOADING_STATUS_FORM.GetStatusPercentage();

            int numSymbols = 0;
            try
            {
                var files = from file in Directory.EnumerateFiles(SymbolManager.DefaultSymbolDirectory, "*.*", SearchOption.AllDirectories).Order()
                            where file.EndsWith(CollectionFileName)
                            select new
                            {
                                File = file
                            };

                int collectionCount = files.Count();
                int statusIncrement = (int)Math.Round((100.0 - StartingLoadPercentage) / collectionCount);

                foreach (var f in files)
                {
                    MapSymbolCollection? collection = MapFileMethods.ReadCollectionFromXml(f.File);

                    if (collection != null)
                    {
                        LOADING_STATUS_FORM.SetStatusText("Loading Collection " + collection.CollectionName);

                        MAP_SYMBOL_COLLECTIONS.Add(collection);

                        collection.CollectionPath = f.File;

                        bool rewriteCollectionFile = false;

                        // load symbol file into object
                        foreach (MapSymbol symbol in collection.GetCollectionMapSymbols())
                        {
                            numSymbols++;

                            if (!string.IsNullOrEmpty(symbol.SymbolFilePath))
                            {
                                if (symbol.SymbolFormat == SymbolFileFormat.PNG
                                    || symbol.SymbolFormat == SymbolFileFormat.JPG
                                    || symbol.SymbolFormat == SymbolFileFormat.BMP)
                                {
                                    if (!string.IsNullOrEmpty(symbol.SymbolFilePath))
                                    {
                                        if (!File.Exists(symbol.SymbolFilePath))
                                        {
                                            rewriteCollectionFile = true;

                                            string fileName = Path.GetFileName(symbol.SymbolFilePath);
                                            string bitmapPath = Path.GetDirectoryName(collection.CollectionPath) + Path.DirectorySeparatorChar + fileName;

                                            symbol.SymbolFilePath = bitmapPath;
                                        }

                                        if (File.Exists(symbol.SymbolFilePath))
                                        {
                                            symbol.SetSymbolBitmapFromPath(symbol.SymbolFilePath);
                                        }
                                    }
                                }
                                else if (symbol.SymbolFormat == SymbolFileFormat.Vector)
                                {
                                    if (!File.Exists(symbol.SymbolFilePath))
                                    {
                                        rewriteCollectionFile = true;

                                        string fileName = Path.GetFileName(symbol.SymbolFilePath);
                                        string bitmapPath = Path.GetDirectoryName(collection.CollectionPath) + Path.DirectorySeparatorChar + fileName;

                                        symbol.SymbolFilePath = bitmapPath;
                                    }

                                    if (File.Exists(symbol.SymbolFilePath))
                                    {
                                        symbol.SetSymbolVectorFromPath(symbol.SymbolFilePath);
                                    }
                                }

                                MAP_SYMBOL_LIST.Add(symbol);

                                foreach (string tag in symbol.SymbolTags)
                                {
                                    SymbolManager.AddTagSymbolAssocation(tag, symbol);
                                }
                            }

                            if (string.IsNullOrEmpty(symbol.CollectionPath))
                            {
                                symbol.CollectionPath = collection.GetCollectionPath();
                            }
                        }

                        if (rewriteCollectionFile)
                        {
                            MapFileMethods.SerializeSymbolCollection(collection);
                        }

                        LoadPercentage += statusIncrement;
                        LOADING_STATUS_FORM.SetStatusPercentage(LoadPercentage);

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

            string frameAssetDirectory = ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Frames" + Path.DirectorySeparatorChar;

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
                    bool rewriteFrameFile = false;

                    if (!File.Exists(mapFrame.FrameXmlFilePath))
                    {
                        rewriteFrameFile = true;
                        mapFrame.FrameXmlFilePath = f.File;
                    }

                    if (!string.IsNullOrEmpty(mapFrame.FrameBitmapPath))
                    {
                        if (!File.Exists(mapFrame.FrameBitmapPath))
                        {
                            rewriteFrameFile = true;

                            string fileName = Path.GetFileName(mapFrame.FrameBitmapPath);
                            string bitmapPath = frameAssetDirectory + fileName;

                            mapFrame.FrameBitmapPath = bitmapPath;
                        }

                        if (File.Exists(mapFrame.FrameBitmapPath))
                        {
                            using (Stream stream = new FileStream(mapFrame.FrameBitmapPath, FileMode.Open))
                            {
                                mapFrame.FrameBitmap = SKBitmap.Decode(stream);
                            }

                            if (rewriteFrameFile)
                            {
                                MapFileMethods.SerializeFrameAsset(mapFrame);
                            }

                            numFrames++;
                            MAP_FRAME_TEXTURES.Add(mapFrame);
                        }
                    }
                }

            }

            return numFrames;
        }

        internal static int LoadBoxAssets()
        {
            MAP_BOX_LIST.Clear();  // TODO: move to BoxManager

            string boxAssetDirectory = ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Boxes" + Path.DirectorySeparatorChar;

            int numBoxes = 0;
            var files = from file in Directory.EnumerateFiles(boxAssetDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".xml")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                MapBox? mapBox = MapFileMethods.ReadBoxAssetFromXml(f.File);

                if (mapBox != null)
                {
                    bool rewriteBoxFile = false;

                    if (!File.Exists(mapBox.BoxXmlFilePath))
                    {
                        rewriteBoxFile = true;
                        mapBox.BoxXmlFilePath = f.File;
                    }

                    if (!string.IsNullOrEmpty(mapBox.BoxBitmapPath))
                    {
                        if (!File.Exists(mapBox.BoxBitmapPath))
                        {
                            rewriteBoxFile = true;
                            string fileName = Path.GetFileName(mapBox.BoxBitmapPath);
                            string bitmapPath = boxAssetDirectory + fileName;

                            mapBox.BoxBitmapPath = bitmapPath;
                        }

                        if (File.Exists(mapBox.BoxBitmapPath))
                        {
                            mapBox.BoxBitmap = new Bitmap(mapBox.BoxBitmapPath);
                            mapBox.BoxBitmap.SetResolution(96.0F, 96.0F);

                            float xScale = (float)(96.0 / mapBox.BoxBitmap.HorizontalResolution);
                            float yScale = (float)(96.0 / mapBox.BoxBitmap.VerticalResolution);

                            SKRect center = new(mapBox.BoxCenterLeft,
                                mapBox.BoxCenterTop,
                                mapBox.BoxCenterRight,
                                mapBox.BoxCenterBottom);

                            SKMatrix tm = SKMatrix.CreateScale(xScale, yScale);
                            SKRect newCenter = tm.MapRect(center);

                            mapBox.BoxCenterLeft = newCenter.Left;
                            mapBox.BoxCenterTop = newCenter.Top;
                            mapBox.BoxCenterRight = newCenter.Right;
                            mapBox.BoxCenterBottom = newCenter.Bottom;

                            if (rewriteBoxFile)
                            {
                                MapFileMethods.SerializeBoxAsset(mapBox);
                            }

                            numBoxes++;
                            MAP_BOX_LIST.Add(mapBox);
                        }
                    }
                }

            }

            return numBoxes;
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

        private static void LoadSymbolTypeSynonyms()
        {
            STRUCTURE_SYNONYMS.Clear();
            TERRAIN_SYNONYMS.Clear();
            VEGETATION_SYNONYMS.Clear();

            IEnumerable<string> structureSynonyms = File.ReadLines(StructureSynonymsFilePath);
            foreach (string synonym in structureSynonyms)
            {
                if (!string.IsNullOrEmpty(synonym))
                {
                    string trimmedSynonym = synonym.Trim([' ', ',']).ToLowerInvariant();

                    if (!STRUCTURE_SYNONYMS.Contains(trimmedSynonym))
                    {
                        STRUCTURE_SYNONYMS.Add(trimmedSynonym);
                    }
                }
            }

            IEnumerable<string> terrainSynonyms = File.ReadLines(TerrainSynonymsFilePath);
            foreach (string synonym in terrainSynonyms)
            {
                if (!string.IsNullOrEmpty(synonym))
                {
                    string trimmedSynonym = synonym.Trim([' ', ',']).ToLowerInvariant();

                    if (!TERRAIN_SYNONYMS.Contains(trimmedSynonym))
                    {
                        TERRAIN_SYNONYMS.Add(trimmedSynonym);
                    }
                }
            }

            IEnumerable<string> vegetationSynonyms = File.ReadLines(VegetationSynonymsFilePath);
            foreach (string synonym in vegetationSynonyms)
            {
                if (!string.IsNullOrEmpty(synonym))
                {
                    string trimmedSynonym = synonym.Trim([' ', ',']).ToLowerInvariant();

                    if (!VEGETATION_SYNONYMS.Contains(trimmedSynonym))
                    {
                        VEGETATION_SYNONYMS.Add(trimmedSynonym);
                    }
                }
            }
        }

        public static void AddSymbolTag(string tag)
        {
            tag = tag.Trim([' ', ',']).ToLowerInvariant();
            if (SYMBOL_TAGS.Contains(tag)) return;
            SYMBOL_TAGS.Add(tag);
        }
    }
}
