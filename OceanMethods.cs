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

using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal sealed class OceanMethods
    {
        public static int OceanPaintBrushSize { get; set; } = 64;
        public static int OceanPaintEraserSize { get; set; } = 64;

        internal static void ApplyOceanTexture(RealmStudioMap map, Bitmap? textureBitmap, float scale, bool mirrorBackground)
        {
            if (textureBitmap != null && scale > 0.0F)
            {
                if (scale != 1.0F)
                {
                    // resize the bitmap, but maintain aspect ratio
                    Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                        (int)(map.MapWidth * scale), (int)(map.MapHeight * scale));

                    Cmd_SetOceanTexture cmd = new(map, Extensions.ToSKBitmap(resizedBitmap), mirrorBackground);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
                else
                {

                    Bitmap resizedBitmap = new(textureBitmap, map.MapWidth, map.MapHeight);

                    Cmd_SetOceanTexture cmd = new(map, Extensions.ToSKBitmap(resizedBitmap), mirrorBackground);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
            }
        }
    }
}
