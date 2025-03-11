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
    internal sealed class Cmd_PaintSymbol(MapSymbol? symbol, SKColor paintColor, SKColor color1, SKColor color2, SKColor color3) : IMapOperation
    {
        private readonly MapSymbol? Symbol = symbol;
        private readonly SKColor PaintColor = paintColor;
        private readonly SKColor SymbolColor1 = color1;
        private readonly SKColor SymbolColor2 = color2;
        private readonly SKColor SymbolColor3 = color3;
        private SKBitmap? SavedPlacedBitmap;

        public void DoOperation()
        {
            if (Symbol == null) { return; }

            Symbol.CustomSymbolColors[0] = SymbolColor1;
            Symbol.CustomSymbolColors[1] = SymbolColor2;
            Symbol.CustomSymbolColors[2] = SymbolColor3;

            if (Symbol.IsGrayscale)
            {
                SKPaint paint = new()
                {
                    ColorFilter = SKColorFilter.CreateBlendMode(PaintColor,
                        SKBlendMode.Modulate) // combine the selected color with the bitmap colors
                };

                Symbol.SymbolPaint = paint;
            }
            else if (Symbol.UseCustomColors)
            {
                Symbol.ColorMappedBitmap = Symbol.SymbolBitmap?.Copy();

                Bitmap colorMappedBitmap = Extensions.ToBitmap(Symbol.SymbolBitmap?.Copy());

                SymbolMethods.MapCustomColorsToColorableBitmap(ref colorMappedBitmap,
                    Symbol.CustomSymbolColors[0].ToDrawingColor(),
                    Symbol.CustomSymbolColors[1].ToDrawingColor(),
                    Symbol.CustomSymbolColors[2].ToDrawingColor());

                Symbol.ColorMappedBitmap = Extensions.ToSKBitmap(colorMappedBitmap).Copy();

                SavedPlacedBitmap = Symbol.PlacedBitmap?.Copy();
                Symbol.PlacedBitmap = Extensions.ToSKBitmap(colorMappedBitmap).Copy();
            }
        }

        public void UndoOperation()
        {
            if (Symbol != null)
            {
                // undo coloring
                Symbol.SymbolPaint = null;

                if (SavedPlacedBitmap != null)
                {
                    Symbol.PlacedBitmap = SavedPlacedBitmap.Copy();
                }
            }
        }
    }
}
