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
using SkiaSharp.Views.Desktop;
using Svg.Skia;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace RealmStudio
{
    internal sealed class SymbolManager : IMapComponentManager
    {
        private static SymbolUIMediator? _symbolMediator;

        private static MapSymbolType _selectedSymbolType = MapSymbolType.NotSet;
        private static string _defaultSymbolDirectory = AssetManager.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Symbols";
        private static string _symbolTagsFilePath = AssetManager.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Symbols" + Path.DirectorySeparatorChar + "SymbolTags.txt";
        private static readonly List<Tuple<string, List<MapSymbol>>> _tagSymbolAssociationList = [];
        private static MapSymbol? _selectedSymbolTableMapSymbol;
        private static readonly List<MapSymbol> _secondarySelectedSymbols = [];

        internal static SymbolUIMediator? SymbolMediator
        {
            get { return _symbolMediator; }
            set { _symbolMediator = value; }
        }

        internal static MapSymbolType SelectedSymbolType
        {
            get { return _selectedSymbolType; }
            set { _selectedSymbolType = value; }
        }

        internal static string DefaultSymbolDirectory
        {
            get { return _defaultSymbolDirectory; }
            set
            {
                _defaultSymbolDirectory = value;
                _symbolTagsFilePath = _defaultSymbolDirectory + Path.DirectorySeparatorChar + "SymbolTags.txt";
            }
        }

        internal static string SymbolTagsFilePath
        {
            get { return _symbolTagsFilePath; }
        }

        internal static List<Tuple<string, List<MapSymbol>>> TagSymbolAssociationList
        {
            get { return _tagSymbolAssociationList; }
        }

        // the symbol selected by the user from the SymbolTable control on the UI
        internal static MapSymbol? SelectedSymbolTableMapSymbol
        {
            get { return _selectedSymbolTableMapSymbol; }
            set { _selectedSymbolTableMapSymbol = value; }
        }

        // additional symbols selected by the user from the SymbolTable control on the UI
        internal static List<MapSymbol> SecondarySelectedSymbols
        {
            get { return _secondarySelectedSymbols; }
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? Create()
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            return true;
        }

        public static bool Delete()
        {
            if (MapStateMediator.SelectedMapSymbol != null)
            {
                Cmd_RemoveSymbol cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapSymbol);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                MapStateMediator.SelectedMapSymbol = null;

                return true;
            }

            return false;
        }

        internal static void PlaceSymbolOnMap(MapSymbol? mapSymbol, SKBitmap? bitmap, SKPoint cursorPoint)
        {
            if (mapSymbol != null && bitmap != null)
            {
                MapStateMediator.CurrentMap.IsSaved = false;

                MapSymbol placedSymbol = new(mapSymbol)
                {
                    X = (int)cursorPoint.X,
                    Y = (int)cursorPoint.Y,
                    Width = bitmap.Width,
                    SymbolWidth = bitmap.Width,
                    Height = bitmap.Height,
                    SymbolHeight = bitmap.Height,
                };

                placedSymbol.SetPlacedBitmap(bitmap);

                Cmd_PlaceSymbol cmd = new(MapStateMediator.CurrentMap, placedSymbol);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static void PlaceVectorSymbolOnMap(MapSymbol? mapSymbol, SKBitmap vectorBitmap, SKPoint cursorPoint)
        {
            if (mapSymbol != null)
            {
                MapStateMediator.CurrentMap.IsSaved = false;
                MapSymbol placedSymbol = new(mapSymbol)
                {
                    X = (int)cursorPoint.X,
                    Y = (int)cursorPoint.Y,
                    Width = vectorBitmap.Width,
                    SymbolWidth = vectorBitmap.Width,
                    Height = vectorBitmap.Height,
                    SymbolHeight = vectorBitmap.Height,
                };

                // TODO: color the vectorBitmap with the custom colors?

                placedSymbol.SetPlacedBitmap(vectorBitmap.Copy());

                SKPaint vectorPaint = new()
                {
                    IsAntialias = true,
                    ColorFilter = SKColorFilter.CreateBlendMode(
                        placedSymbol.CustomSymbolColors[0],
                        SKBlendMode.Modulate) // combine the selected color with the bitmap colors
                };

                placedSymbol.SymbolPaint = vectorPaint;

                Cmd_PlaceSymbol cmd = new(MapStateMediator.CurrentMap, placedSymbol);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static void RemovePlacedSymbolsFromArea(SKPoint centerPoint, float eraserCircleRadius)
        {
            Cmd_RemoveSymbolsFromArea cmd = new(MapStateMediator.CurrentMap, eraserCircleRadius, centerPoint);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();
        }

        internal static void AnalyzeSymbolBitmapColors(MapSymbol symbol)
        {
            // gather the colors from each pixel of the bitmap
            // if all colors are shades of gray, transparent, black, or white, then the bitmap is grayscale
            // if all of the colors are either red, green, blue, transparent, or black, the image should use custom colors when it is painted
            // the red, green, and blue values may not be exactly pure colors
            SKBitmap? bitmap = symbol.SymbolBitmap;

            if (bitmap == null) return;

            if (!symbol.IsGrayscale)
            {
                // check for grayscale image
                symbol.IsGrayscale = DrawingMethods.IsGrayScaleImage(Extensions.ToBitmap(bitmap)); ;
            }

            // if the symbol is paintable (probably determined from .wonderdraft_symbols file or user input), then no need to analyze the bitmap colors
            if (!symbol.UseCustomColors && !symbol.IsGrayscale)
            {
                // otherwise, check to see if the bitmap uses any non-paintable colors
                // (colors that are not near a pure shade of red, green, blue, or black)
                symbol.UseCustomColors = DrawingMethods.IsPaintableImage(Extensions.ToBitmap(bitmap));
            }
        }

        public static void AddTagSymbolAssocation(string tag, MapSymbol mapSymbol)
        {
            Tuple<string, List<MapSymbol>>? tagSymbols = TagSymbolAssociationList.Find(x => x.Item1.Equals(tag, StringComparison.Ordinal));

            if (tagSymbols != null)
            {
                if (!tagSymbols.Item2.Contains(mapSymbol))
                {
                    tagSymbols.Item2.Add(mapSymbol);
                }
            }
            else
            {
                Tuple<string, List<MapSymbol>> newTagAssociation = new(tag, []);
                newTagAssociation.Item2.Add(mapSymbol);

                TagSymbolAssociationList.Add(newTagAssociation);
            }
        }

        public static void MapCustomColorsToColorableBitmap(ref Bitmap bitmap, Color customColor1, Color customColor2, Color customColor3)
        {
            if (bitmap == null)
            {
                return;
            }

            var lockedBitmap = new LockBitmap(bitmap);
            lockedBitmap.LockBits();

            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    Color pixelColor = lockedBitmap.GetPixel(x, y);

                    byte rValue = pixelColor.R;
                    byte gValue = pixelColor.G;
                    byte bValue = pixelColor.B;

                    if (rValue > 64 && gValue == 0 && bValue == 0)
                    {
                        float colorScale = rValue / 255F;
                        SKColor newColor = customColor1.ToSKColor();

                        newColor.ToHsl(out float hue, out float saturation, out float brightness);
                        brightness *= colorScale;

                        Color scaledColor = SKColor.FromHsl(hue, saturation, brightness).ToDrawingColor();
                        lockedBitmap.SetPixel(x, y, scaledColor);
                    }
                    else if (gValue > 64 && rValue == 0 && bValue == 0)
                    {
                        float colorScale = gValue / 255F;
                        SKColor newColor = customColor2.ToSKColor();

                        newColor.ToHsl(out float hue, out float saturation, out float brightness);
                        brightness *= colorScale;

                        Color scaledColor = SKColor.FromHsl(hue, saturation, brightness).ToDrawingColor();
                        lockedBitmap.SetPixel(x, y, scaledColor);
                    }
                    else if (bValue > 64 && rValue == 3 && gValue == 3)
                    {
                        float colorScale = bValue / 255F;
                        SKColor newColor = customColor3.ToSKColor();

                        newColor.ToHsl(out float hue, out float saturation, out float brightness);
                        brightness *= colorScale;

                        Color scaledColor = SKColor.FromHsl(hue, saturation, brightness).ToDrawingColor();
                        lockedBitmap.SetPixel(x, y, scaledColor);
                    }
                }
            }

            lockedBitmap.UnlockBits();
        }

        internal static List<MapSymbol> GetFilteredSymbolList(MapSymbolType selectedSymbolType, List<string> selectedCollections, List<string> selectedTags, string filterText = "")
        {
            List<MapSymbol> filteredSymbols = GetMapSymbolsWithType(selectedSymbolType);

            if (selectedCollections.Count > 0)
            {
                for (int i = filteredSymbols.Count - 1; i >= 0; i--)
                {

                    if (!selectedCollections.Contains(filteredSymbols[i].CollectionName))
                    {
                        filteredSymbols.RemoveAt(i);
                    }
                }
            }

            if (selectedTags.Count > 0)
            {
                foreach (string tag in selectedTags)
                {
                    for (int i = filteredSymbols.Count - 1; i >= 0; i--)
                    {
                        if (!filteredSymbols[i].SymbolTags.Contains(tag))
                        {
                            filteredSymbols.RemoveAt(i);
                        }
                    }
                }
            }

            // filter the sybol list by text entered in the search box
            if (!string.IsNullOrEmpty(filterText))
            {
                for (int i = filteredSymbols.Count - 1; i >= 0; i--)
                {
                    bool removeSymbol = true;

                    // keep the symbol if the symbol name contains the filter text
                    if (filteredSymbols[i].SymbolName.Contains(filterText))
                    {
                        removeSymbol = false;
                    }

                    // keep the symbol if any of its tags contains the filter text
                    for (int j = 0; j < filteredSymbols[i].SymbolTags.Count; j++)
                    {
                        if (filteredSymbols[i].SymbolTags[j].Contains(filterText))
                        {
                            removeSymbol = false;
                        }
                    }

                    if (removeSymbol)
                    {
                        filteredSymbols.RemoveAt(i);
                    }
                }
            }

            return filteredSymbols;
        }

        internal static List<MapSymbol> GetMapSymbolsWithType(MapSymbolType symbolType)
        {
            List<MapSymbol> typeSymbols = AssetManager.MAP_SYMBOL_LIST.FindAll(x => x.SymbolType == symbolType);
            return typeSymbols;
        }

        internal static bool CanPlaceSymbol(SKPoint cursorPoint, float placementDensityRadius)
        {
            // if there are any symbols within the placementDensityRadius around the cursor point, then the symbol cannot be placed at the cursor point

            bool canPlace = true;

            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.SYMBOLLAYER);

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                MapSymbol symbol = (MapSymbol)symbolLayer.MapLayerComponents[i];
                SKPoint symbolPoint = new(symbol.X, symbol.Y);
                bool placeAllowed = !DrawingMethods.PointInCircle(placementDensityRadius, cursorPoint, symbolPoint);

                if (!placeAllowed)
                {
                    canPlace = false;
                    break;
                }
            }

            return canPlace;
        }

        internal static void ColorSymbolsInArea(SKPoint colorCursorPoint, int colorBrushRadius, Color[] symbolColors, bool randomizeColors)
        {
            List<MapComponent> components = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.SYMBOLLAYER).MapLayerComponents;

            foreach (MapComponent component in components)
            {
                MapSymbol symbol = (MapSymbol)component;

                SKPoint symbolPoint = new(symbol.X, symbol.Y);

                Color symbolColor = symbolColors[0];

                if (DrawingMethods.PointInCircle(colorBrushRadius, colorCursorPoint, symbolPoint))
                {
                    if (symbol.IsGrayscale && symbol.SymbolType == SelectedSymbolType)
                    {
                        if (randomizeColors)
                        {
                            // vary RGB by 10%
                            int rVariance = Random.Shared.Next(-25, 26);
                            int gVariance = Random.Shared.Next(-25, 26);
                            int bVariance = Random.Shared.Next(-25, 26);

                            rVariance = Math.Max(0, Math.Min(255, symbolColor.R + rVariance));
                            gVariance = Math.Max(0, Math.Min(255, symbolColor.G + gVariance));
                            bVariance = Math.Max(0, Math.Min(255, symbolColor.B + bVariance));

                            symbolColor = Color.FromArgb(rVariance, gVariance, bVariance);
                        }

                        SKPaint paint = new()
                        {
                            ColorFilter = SKColorFilter.CreateBlendMode(
                                Extensions.ToSKColor(symbolColor),
                                SKBlendMode.Modulate) // combine the selected color with the bitmap colors
                        };

                        symbol.SymbolPaint = paint;
                    }

                    symbol.CustomSymbolColors[0] = Extensions.ToSKColor(symbolColor);
                    symbol.CustomSymbolColors[1] = Extensions.ToSKColor(symbolColors[1]);
                    symbol.CustomSymbolColors[2] = Extensions.ToSKColor(symbolColors[2]);
                }
            }
        }

        public static void SaveSymbolTags()
        {
            if (AssetManager.SYMBOL_TAGS.Count > 0 && AssetManager.SYMBOL_TAGS.Count >= AssetManager.ORIGINAL_SYMBOL_TAGS.Count)
            {
                File.WriteAllLines(SymbolTagsFilePath, AssetManager.SYMBOL_TAGS);
            }
        }

        internal static void SaveCollections()
        {
            foreach (MapSymbolCollection collection in AssetManager.MAP_SYMBOL_COLLECTIONS)
            {
                if (collection != null && collection.IsModified)
                {
                    // make a backup of the current collection.xml file
                    File.Copy(collection.GetCollectionPath(), collection.GetCollectionPath() + ".backup", true);

                    MapFileMethods.SerializeSymbolCollection(collection);
                }
            }
        }

        internal static List<string> AutoTagSymbol(MapSymbol symbol)
        {
            const int minTagLength = 2;

            List<string> potentialTags = [];

            string symbolFileName = Path.GetFileNameWithoutExtension(symbol.SymbolFilePath);

            if (string.IsNullOrEmpty(symbol.SymbolName))
            {
                symbol.SymbolName = symbolFileName;
            }

            string[] symbolNameParts = symbol.SymbolName.Split([' ', '_', '-']);
            string[] collectionNameParts = symbol.CollectionName.Split([' ', '_', '-']);

            foreach (string symbolNamePart in symbolNameParts)
            {
                string potentialTag = new string([.. symbolNamePart.Where(char.IsLetter)]).ToLowerInvariant();

                if (!string.IsNullOrEmpty(potentialTag) && potentialTag.Length > minTagLength && !potentialTags.Contains(potentialTag))
                {
                    potentialTags.Add(potentialTag);
                }
            }

            foreach (string collectionNamePart in collectionNameParts)
            {
                string potentialTag = new string([.. collectionNamePart.Where(char.IsLetter)]).ToLowerInvariant();

                if (!string.IsNullOrEmpty(potentialTag) && potentialTag.Length > minTagLength && !potentialTags.Contains(potentialTag))
                {
                    potentialTags.Add(potentialTag);
                }
            }


            for (int i = potentialTags.Count - 1; i >= 0; i--)
            {
                string potentialTag = potentialTags[i];
                bool tagMatched = false;

                foreach (string tag in AssetManager.SYMBOL_TAGS)
                {
                    if (tag.Contains(potentialTag) || potentialTag.Contains(tag))
                    {
                        tagMatched = true;
                    }
                }

                if (!tagMatched)
                {
                    potentialTags.RemoveAt(i);
                }
            }

            return potentialTags;

        }

        internal static void GuessSymbolTypeFromTags(MapSymbol mapSymbol)
        {
            foreach (string tag in mapSymbol.SymbolTags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    if (AssetManager.STRUCTURE_SYNONYMS.Contains(tag))
                    {
                        mapSymbol.SymbolType = MapSymbolType.Structure;
                        return;
                    }

                    if (AssetManager.TERRAIN_SYNONYMS.Contains(tag))
                    {
                        mapSymbol.SymbolType = MapSymbolType.Terrain;
                        return;
                    }

                    if (AssetManager.VEGETATION_SYNONYMS.Contains(tag))
                    {
                        mapSymbol.SymbolType = MapSymbolType.Vegetation;
                        return;
                    }
                }
            }
        }

        internal static SKBitmap? GetBitmapForVectorSymbol(MapSymbol symbol, float width, float height, float rotation, float scale = 1.0F)
        {
            if (symbol.SymbolFormat == SymbolFileFormat.Vector && !string.IsNullOrEmpty(symbol.SymbolSVG))
            {
                try
                {
                    using MemoryStream ms = new(Encoding.ASCII.GetBytes(symbol.SymbolSVG));
                    using var skSvg = new SKSvg();
                    skSvg.Load(ms);

                    if (skSvg.Picture != null)
                    {
                        float symbolWidthScale = width / skSvg.Picture.CullRect.Width;
                        float symbolHeightScale = height / skSvg.Picture.CullRect.Height;

                        if (width == 0 || height == 0)
                        {
                            symbolWidthScale = scale;
                            symbolHeightScale = scale;
                        }

                        width = skSvg.Picture.CullRect.Width * symbolWidthScale;
                        height = skSvg.Picture.CullRect.Height * symbolHeightScale;

                        SKBitmap b = new(new SKImageInfo((int)width, (int)height));

                        using SKCanvas c = new(b);

                        SKMatrix matrix = SKMatrix.CreateScale(symbolWidthScale, symbolHeightScale);

                        c.RotateDegrees(rotation, b.Width / 2, b.Height / 2);
                        c.DrawPicture(skSvg.Picture, in matrix);

                        return b;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        internal static void FinalizeMapSymbols()
        {
            // finalize loading of symbols
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.SYMBOLLAYER);

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol symbol)
                {
                    SKColor? paintColor = symbol.CustomSymbolColors[0];

                    // reconstruct paint object for grayscale symbols
                    if (symbol.IsGrayscale && paintColor != null)
                    {
                        SKPaint paint = new()
                        {
                            ColorFilter = SKColorFilter.CreateBlendMode((SKColor)paintColor,
                                SKBlendMode.Modulate) // combine the selected color with the bitmap colors
                        };

                        symbol.SymbolPaint = paint;

                        if (symbol.PlacedBitmap != null)
                        {
                            if (symbol.Width != symbol.PlacedBitmap.Width || symbol.Height != symbol.PlacedBitmap.Height)
                            {
                                // resize the placed bitmap to match the size set in the symbol - this shouldn't be necessary
                                SKBitmap resizedPlacedBitmap = new(symbol.Width, symbol.Height);

                                symbol.PlacedBitmap.ScalePixels(resizedPlacedBitmap, SKSamplingOptions.Default);
                                symbol.SetPlacedBitmap(resizedPlacedBitmap);

                            }
                        }
                    }
                }
            }
        }

        internal static void MoveSelectedSymbolInRenderOrder(ComponentMoveDirection direction, int amount = 1, bool toTopBottom = false)
        {
            if (MapStateMediator.SelectedMapSymbol != null)
            {
                // find the selected symbol in the Symbol Layer MapComponents
                MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.SYMBOLLAYER);

                List<MapComponent> symbolComponents = symbolLayer.MapLayerComponents;
                MapSymbol? selectedSymbol = null;

                int selectedSymbolIndex = 0;

                for (int i = 0; i < symbolComponents.Count; i++)
                {
                    MapComponent symbolComponent = symbolComponents[i];
                    if (symbolComponent is MapSymbol symbol && symbol.SymbolGuid.ToString() == MapStateMediator.SelectedMapSymbol.SymbolGuid.ToString())
                    {
                        selectedSymbolIndex = i;
                        selectedSymbol = symbol;
                        break;
                    }
                }

                if (direction == ComponentMoveDirection.Up)
                {
                    // moving a symbol up in render order means increasing its index
                    if (selectedSymbol != null && selectedSymbolIndex < symbolComponents.Count - 1)
                    {
                        if (toTopBottom)
                        {
                            symbolComponents.RemoveAt(selectedSymbolIndex);
                            symbolComponents.Add(selectedSymbol);
                        }
                        else
                        {
                            int moveLocation;

                            if (selectedSymbolIndex + amount < symbolComponents.Count - 1)
                            {
                                moveLocation = selectedSymbolIndex + amount;
                            }
                            else
                            {
                                moveLocation = symbolComponents.Count - 1;
                            }

                            symbolComponents[selectedSymbolIndex] = symbolComponents[moveLocation];
                            symbolComponents[moveLocation] = selectedSymbol;
                        }
                    }
                }
                else if (direction == ComponentMoveDirection.Down)
                {
                    // moving a symbol down in render order means decreasing its index
                    if (selectedSymbol != null && selectedSymbolIndex > 0)
                    {
                        if (toTopBottom)
                        {
                            symbolComponents.RemoveAt(selectedSymbolIndex);
                            symbolComponents.Insert(0, selectedSymbol);
                        }
                        else
                        {
                            int moveLocation;

                            if (selectedSymbolIndex - amount >= 0)
                            {
                                moveLocation = selectedSymbolIndex - amount;
                            }
                            else
                            {
                                moveLocation = 0;
                            }

                            symbolComponents[selectedSymbolIndex] = symbolComponents[moveLocation];
                            symbolComponents[moveLocation] = selectedSymbol;
                        }
                    }
                }
            }
        }

        internal static void MoveSymbol(MapSymbol? mapSymbol, SKPoint zoomedScrolledPoint)
        {
            if (mapSymbol != null && mapSymbol.IsSelected)
            {
                mapSymbol.X = (int)zoomedScrolledPoint.X - mapSymbol.Width / 2;
                mapSymbol.Y = (int)zoomedScrolledPoint.Y - mapSymbol.Height / 2;
            }
        }

        private static void PlaceSelectedMapSymbolAtPoint(SKPoint cursorPoint, float symbolScale, float symbolRotation)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            MapSymbol? symbolToPlace = SelectedSymbolTableMapSymbol;

            if (symbolToPlace != null)
            {
                if (SecondarySelectedSymbols.Count > 0)
                {
                    int selectedIndex = Random.Shared.Next(0, SecondarySelectedSymbols.Count + 1);

                    if (selectedIndex > 0)
                    {
                        symbolToPlace = SecondarySelectedSymbols[selectedIndex - 1];
                    }
                }

                symbolToPlace.X = (int)cursorPoint.X;
                symbolToPlace.Y = (int)cursorPoint.Y;

                SKBitmap? symbolBitmap = symbolToPlace.ColorMappedBitmap;

                if (symbolBitmap != null)
                {
                    SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleSKBitmap(symbolBitmap, symbolScale);
                    SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateSKBitmap(scaledSymbolBitmap, symbolRotation, SymbolMediator.MirrorSymbol);

                    float bitmapRadius = rotatedAndScaledBitmap.Width / 2;

                    // PLACEMENT_RATE controls the timer used for placing symbols with the area brush
                    // PLACEMENT_DENSITY controls how close together symbols can be placed

                    // decreasing this value increases the density of symbol placement on the map
                    // so high values of placement density on the placement density updown increase placement density on the map
                    float placementDensityRadius = bitmapRadius / SymbolMediator.SymbolPlacementDensity;

                    bool canPlaceSymbol = CanPlaceSymbol(cursorPoint, placementDensityRadius);

                    if (canPlaceSymbol)
                    {
                        symbolToPlace.Width = rotatedAndScaledBitmap.Width;
                        symbolToPlace.Height = rotatedAndScaledBitmap.Height;

                        PlaceSymbolOnMap(symbolToPlace, rotatedAndScaledBitmap, cursorPoint);
                    }
                }
            }
        }

        private static SKBitmap RotateAndScaleSymbolBitmap(SKBitmap symbolBitmap, float symbolScale, float symbolRotation)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleSKBitmap(symbolBitmap, symbolScale);
            SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateSKBitmap(scaledSymbolBitmap, symbolRotation, SymbolMediator.MirrorSymbol);

            return rotatedAndScaledBitmap;
        }

        internal static void PlaceSelectedSymbolInArea(SKPoint mouseCursorPoint, float symbolScale, float symbolRotation, int areaBrushSize)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            if (SelectedSymbolTableMapSymbol != null)
            {
                lock (SelectedSymbolTableMapSymbol)
                {
                    SKBitmap? symbolBitmap = SelectedSymbolTableMapSymbol.SymbolBitmap;
                    if (symbolBitmap != null)
                    {
                        SKBitmap rotatedAndScaledBitmap = RotateAndScaleSymbolBitmap(symbolBitmap, symbolScale, symbolRotation);

                        SKPoint cursorPoint = new(mouseCursorPoint.X - (rotatedAndScaledBitmap.Width / 2), mouseCursorPoint.Y - (rotatedAndScaledBitmap.Height / 2));

                        float bitmapRadius = rotatedAndScaledBitmap.Width / 2;

                        // decreasing this value increases the density of symbol placement on the map
                        // so high values of placement density on the placement density updown increase placement density on the map
                        float placementDensityRadius = bitmapRadius / SymbolMediator.SymbolPlacementDensity;

                        List<SKPoint> areaPoints = DrawingMethods.GetPointsInCircle(cursorPoint, (int)Math.Ceiling((double)areaBrushSize), (int)placementDensityRadius);

                        foreach (SKPoint p in areaPoints)
                        {
                            SKPoint point = p;

                            // 1% randomization of point location
                            point.X = Random.Shared.Next((int)(p.X * 0.99F), (int)(p.X * 1.01F));
                            point.Y = Random.Shared.Next((int)(p.Y * 0.99F), (int)(p.Y * 1.01F));

                            PlaceSelectedMapSymbolAtPoint(point, symbolScale, symbolRotation);
                        }
                    }
                }
            }
        }

        internal static void PlaceSelectedSymbolAtCursor(SKPoint mouseCursorPoint)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            if (SelectedSymbolTableMapSymbol != null)
            {
                float symbolScale = SymbolMediator.SymbolScale / 100.0F;
                float symbolRotation = SymbolMediator.SymbolRotation;

                if (SelectedSymbolTableMapSymbol.SymbolFormat != SymbolFileFormat.Vector)
                {
                    SKBitmap? symbolBitmap = SelectedSymbolTableMapSymbol.SymbolBitmap;
                    if (symbolBitmap != null)
                    {
                        SKBitmap rotatedAndScaledBitmap = RotateAndScaleSymbolBitmap(symbolBitmap, symbolScale, symbolRotation);
                        SKPoint cursorPoint = new(mouseCursorPoint.X - (rotatedAndScaledBitmap.Width / 2), mouseCursorPoint.Y - (rotatedAndScaledBitmap.Height / 2));

                        PlaceSelectedMapSymbolAtPoint(cursorPoint, symbolScale, symbolRotation);
                    }
                }
                else
                {
                    PlaceVectorSymbolAtPoint(mouseCursorPoint, symbolScale, symbolRotation);
                }
            }
        }


        private static void PlaceVectorSymbolAtPoint(SKPoint mouseCursorPoint, float symbolScale, float symbolRotation)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            MapSymbol? symbolToPlace = SelectedSymbolTableMapSymbol;

            if (symbolToPlace != null)
            {
                SKBitmap? b = GetBitmapForVectorSymbol(symbolToPlace,
                    0, 0, symbolRotation, symbolScale);

                if (b != null)
                {
                    float bitmapRadius = b.Width / 2;
                    float placementDensityRadius = bitmapRadius / SymbolMediator.SymbolPlacementDensity;

                    SKPoint cursorPoint = new(mouseCursorPoint.X - (b.Width / 2), mouseCursorPoint.Y - (b.Height / 2));

                    bool canPlaceSymbol = CanPlaceSymbol(cursorPoint, placementDensityRadius);

                    if (canPlaceSymbol)
                    {
                        symbolToPlace.Width = b.Width;
                        symbolToPlace.Height = b.Height;

                        PlaceVectorSymbolOnMap(SelectedSymbolTableMapSymbol, b, cursorPoint);
                    }
                }
            }
        }

        internal static Bitmap? GetSymbolPictureBoxBitmap(MapSymbol symbol)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            Bitmap pbm = new(SymbolUIMediator.SymbolPictureBoxWidth - 2, SymbolUIMediator.SymbolPictureBoxHeight - 8);

            if (symbol.SymbolFormat != SymbolFileFormat.Vector)
            {
                symbol.ColorMappedBitmap = symbol.SymbolBitmap?.Copy();

                Bitmap colorMappedBitmap = (Bitmap)Extensions.ToBitmap(symbol.ColorMappedBitmap).Clone();

                if (colorMappedBitmap.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    Bitmap clone = new(colorMappedBitmap.Width, colorMappedBitmap.Height, PixelFormat.Format32bppPArgb);

                    using Graphics gr = Graphics.FromImage(clone);
                    gr.DrawImage(colorMappedBitmap, new Rectangle(0, 0, clone.Width, clone.Height));
                    colorMappedBitmap = new(clone);
                    clone.Dispose();
                }

                if (symbol.UseCustomColors)
                {
                    MapCustomColorsToColorableBitmap(ref colorMappedBitmap,
                        SymbolMediator.SymbolColor1, SymbolMediator.SymbolColor2, SymbolMediator.SymbolColor3);
                }
                else if (symbol.IsGrayscale)
                {
                    // TODO: color the grayscale with custom color (using SymbolColor1Button background color)?
                }

                symbol.ColorMappedBitmap = Extensions.ToSKBitmap((Bitmap)colorMappedBitmap.Clone());

                pbm = DrawingMethods.ScaleBitmap(colorMappedBitmap,
                    SymbolUIMediator.SymbolPictureBoxWidth - 2, SymbolUIMediator.SymbolPictureBoxHeight - 8);
                colorMappedBitmap.Dispose();
            }
            else
            {
                // display the SVG
                using MemoryStream ms = new(Encoding.ASCII.GetBytes(symbol.SymbolSVG));
                var skSvg = new SKSvg();
                skSvg.Load(ms);

                using SKBitmap b = new(new SKImageInfo(SymbolUIMediator.SymbolPictureBoxHeight - 8, SymbolUIMediator.SymbolPictureBoxHeight - 8));
                using SKCanvas c = new(b);

                /*
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;


                    //could also tint the svg:
                    //paint.ColorFilter = SKColorFilter.CreateBlendMode(TintColor.ToSKColor(), SKBlendMode.SrcIn);

                }
                */

                if (skSvg.Picture != null)
                {
                    SKMatrix matrix = SKMatrix.CreateScale((SymbolUIMediator.SymbolPictureBoxHeight - 8) / skSvg.Picture.CullRect.Width,
                        (SymbolUIMediator.SymbolPictureBoxHeight - 8) / skSvg.Picture.CullRect.Height);

                    c.DrawPicture(skSvg.Picture, in matrix);

                    pbm = Extensions.ToBitmap(b.Copy());
                }
            }

            return pbm;
        }

        internal static void ColorSelectedSymbol(MapSymbol? selectedMapSymbol)
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            if (selectedMapSymbol == null) return;

            // if a symbol has been selected and is grayscale or custom colored, then color it with the
            // selected custom colors

            if (selectedMapSymbol.IsGrayscale || selectedMapSymbol.UseCustomColors)
            {
                Cmd_PaintSymbol cmd = new(selectedMapSymbol,
                    SymbolMediator.SymbolColor1.ToSKColor(), SymbolMediator.SymbolColor1.ToSKColor(),
                    SymbolMediator.SymbolColor2.ToSKColor(), SymbolMediator.SymbolColor3.ToSKColor());

                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }

            selectedMapSymbol.IsSelected = false;
            MapStateMediator.SelectedMapSymbol = null;
        }

        internal static void ColorSymbolAtPoint()
        {
            ArgumentNullException.ThrowIfNull(SymbolMediator);

            MapSymbol? symbolAtPoint = SymbolUIMediator.SelectMapSymbolAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint.ToDrawingPoint());

            if (symbolAtPoint != null)
            {
                if (symbolAtPoint.IsGrayscale)
                {
                    Cmd_PaintSymbol cmd = new(symbolAtPoint,
                        SymbolMediator.SymbolColor1.ToSKColor(), SymbolMediator.SymbolColor1.ToSKColor(),
                        SymbolMediator.SymbolColor2.ToSKColor(), SymbolMediator.SymbolColor3.ToSKColor());

                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
            }
        }
    }
}
