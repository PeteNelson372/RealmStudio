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

namespace RealmStudio
{
    internal class SymbolMethods
    {
        private static readonly string SymbolTagsFilePath = AssetManager.DefaultSymbolDirectory + Path.DirectorySeparatorChar + "SymbolTags.txt";

        private static readonly List<Tuple<string, List<MapSymbol>>> TagSymbolAssociationList = [];

        // the symbol selected by the user from the SymbolTable control on the UI
        public static MapSymbol? SelectedSymbolTableMapSymbol = null;

        // additional symbols selected by the user from the SymbolTable control on the UI
        public static readonly List<MapSymbol> SecondarySelectedSymbols = [];

        internal static void PlaceSymbolOnMap(RealmStudioMap map, MapSymbol? mapSymbol, SKBitmap? bitmap, SKPoint cursorPoint)
        {
            if (mapSymbol != null && bitmap != null)
            {
                map.IsSaved = false;

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

                Cmd_PlaceSymbol cmd = new(map, placedSymbol);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
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
            Tuple<string, List<MapSymbol>>? tagSymbols = TagSymbolAssociationList.Find(x => x.Item1.Equals(tag));

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

        internal static List<MapSymbol> GetFilteredSymbolList(SymbolTypeEnum selectedSymbolType, List<string> selectedCollections, List<string> selectedTags, string filterText = "")
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

        internal static List<MapSymbol> GetMapSymbolsWithType(SymbolTypeEnum symbolType)
        {
            List<MapSymbol> typeSymbols = AssetManager.MAP_SYMBOL_LIST.FindAll(x => x.SymbolType == symbolType);
            return typeSymbols;
        }

        internal static bool CanPlaceSymbol(RealmStudioMap map, MapSymbol? mapSymbol, SKBitmap rotatedAndScaledBitmap, SKPoint cursorPoint, float placementDensity)
        {
            // if the symbol is within the excluded radius from any other symbols, then the symbol cannot be placed at the cursor point
            if (mapSymbol == null) return false;

            bool canPlace = true;

            float exclusionRadius = ((rotatedAndScaledBitmap.Width + rotatedAndScaledBitmap.Height) / 2.0F) / placementDensity;

            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                MapSymbol symbol = (MapSymbol)symbolLayer.MapLayerComponents[i];
                SKPoint symbolPoint = new(symbol.X, symbol.Y);
                bool placeAllowed = !DrawingMethods.PointInCircle(exclusionRadius, symbolPoint, cursorPoint);

                if (!placeAllowed)
                {
                    canPlace = false;
                    break;
                }
            }

            return canPlace;
        }

        internal static void ColorSymbolsInArea(RealmStudioMap map, SKPoint colorCursorPoint, int colorBrushRadius, Color[] symbolColors)
        {
            List<MapComponent> components = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER).MapLayerComponents;

            foreach (MapComponent component in components)
            {
                MapSymbol symbol = (MapSymbol)component;

                SKPoint symbolPoint = new(symbol.X, symbol.Y);

                if (DrawingMethods.PointInCircle(colorBrushRadius, colorCursorPoint, symbolPoint))
                {
                    if (symbol.IsGrayscale)
                    {
                        SKPaint paint = new()
                        {
                            ColorFilter = SKColorFilter.CreateBlendMode(
                                Extensions.ToSKColor(symbolColors[0]),
                                SKBlendMode.Modulate) // combine the selected color with the bitmap colors
                        };

                        symbol.SymbolPaint = paint;
                    }

                    symbol.CustomSymbolColors[0] = Extensions.ToSKColor(symbolColors[0]);
                    symbol.CustomSymbolColors[1] = Extensions.ToSKColor(symbolColors[1]);
                    symbol.CustomSymbolColors[2] = Extensions.ToSKColor(symbolColors[2]);
                }
            }
        }
    }
}
