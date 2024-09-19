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
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal class SymbolMethods
    {
        // the tags that can be selected in the UI to filter the tags in the tag list box on the UI
        private static readonly List<string> OriginalSymbolTags = [];
        private static readonly List<string> SymbolTags = [];

        private static readonly string SymbolTagsFilePath = AssetManager.DefaultSymbolDirectory + Path.DirectorySeparatorChar + "SymbolTags.txt";

        private static readonly List<Tuple<string, List<MapSymbol>>> TagSymbolAssociationList = [];

        public static void LoadSymbolTags()
        {
            SymbolTags.Clear();
            OriginalSymbolTags.Clear();

            IEnumerable<string> tags = File.ReadLines(SymbolTagsFilePath);
            foreach (string tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    AddSymbolTag(tag);
                }
            }

            SymbolTags.Sort();

            foreach (string tag in SymbolTags)
            {
                OriginalSymbolTags.Add(tag);
            }
        }

        public static void AddSymbolTag(string tag)
        {
            tag = tag.Trim([' ', ',']).ToLower();
            if (SymbolTags.Contains(tag)) return;
            SymbolTags.Add(tag);
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
    }
}
