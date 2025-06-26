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
    internal class Cmd_DrawOnCanvas(MapLayer layer, GRContext context) : IMapOperation
    {
        // this command is unusual in that it is constructed, added to the Undo stack
        // and DoOperation is called *before* the draing operation is performed, so that
        // the state of the canvas is saved before the drawing operation occurs.
        // The UndoOperation method then can restore the canvas to the state it was in before the drawing operation.

        private readonly MapLayer Layer = layer;
        private readonly GRContext Context = context;

        private SKData? SavedImageData;
        private SKData? SavedPreviousImageData;

        public void DoOperation()
        {
            if (SavedPreviousImageData != null && Layer.LayerSurface?.Canvas != null)
            {
                // Clear the canvas
                Layer.LayerSurface.Canvas.Surface?.Canvas.Clear(SKColors.Transparent);

                // Draw the saved previous image back onto the canvas
                Layer.LayerSurface.Canvas.Surface?.Canvas.DrawImage(SKImage.FromEncodedData(SavedPreviousImageData), 0, 0);

            }
            else if (Layer.LayerSurface?.Canvas != null)
            {
                using SKImage? currentImage = Layer.LayerSurface.Canvas.Surface?.Snapshot();

                if (currentImage != null && currentImage.IsValid(Context))
                {
                    SavedImageData = currentImage.Encode(SKEncodedImageFormat.Png, 100);
                }
            }
        }

        public void UndoOperation()
        {
            if (Layer.LayerSurface?.Canvas != null)
            {
                using SKImage? previousImage = Layer.LayerSurface.Canvas.Surface?.Snapshot();
                if (previousImage != null && previousImage.IsValid(Context))
                {
                    SavedPreviousImageData = previousImage.Encode(SKEncodedImageFormat.Png, 100);
                }

                if (SavedImageData != null)
                {
                    // Clear the canvas
                    Layer.LayerSurface.Canvas.Surface?.Canvas.Clear(SKColors.Transparent);

                    // Draw the saved image back onto the canvas
                    Layer.LayerSurface.Canvas.Surface?.Canvas.DrawImage(SKImage.FromEncodedData(SavedImageData), 0, 0);
                }

            }
        }
    }
}
