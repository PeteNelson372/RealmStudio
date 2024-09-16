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
