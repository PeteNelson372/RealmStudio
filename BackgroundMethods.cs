/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
    internal sealed class BackgroundMethods
    {
        internal static void ApplyBackgroundTexture(RealmStudioMap map, Bitmap? textureBitmap, float scale, bool mirrorBackground)
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER);

            if (baseLayer.MapLayerComponents.Count < 1 && textureBitmap != null && scale > 0.0F)
            {
                if (scale != 1.0F)
                {
                    // resize the bitmap, but maintain aspect ratio
                    Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                        (int)(map.MapWidth * scale), (int)(map.MapHeight * scale));

                    Cmd_SetBackgroundTexture cmd = new(map, Extensions.ToSKBitmap(resizedBitmap), mirrorBackground);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
                else
                {
                    Bitmap resizedBitmap = new(textureBitmap,
                        map.MapWidth, map.MapHeight);

                    Cmd_SetBackgroundTexture cmd = new(map, Extensions.ToSKBitmap(resizedBitmap), mirrorBackground);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
            }
        }

        internal static void ClearBackgroundImage(RealmStudioMap map)
        {
            MapLayer backgroundLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER);

            if (backgroundLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerTexture = (MapImage)MapBuilder.GetMapLayerByIndex(map, MapBuilder.BASELAYER).MapLayerComponents.First();

                Cmd_ClearBackgroundTexture cmd = new(map, layerTexture);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static MapTexture GetSelectedMapTexture()
        {
            return AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX];
        }

        internal static Bitmap? GetSelectedBackgroundImage()
        {
            return AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap;
        }

        internal static string GetCurrentImageName()
        {
            return AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureName;
        }

        internal static Bitmap? NextBackgroundTexture()
        {
            if (AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX < AssetManager.BACKGROUND_TEXTURE_LIST.Count - 1)
            {
                AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX++;
            }

            if (AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TexturePath);
            }

            return AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap;
        }

        internal static Bitmap? PreviousBackgroundTexture()
        {
            if (AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX > 0)
            {
                AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX--;
            }

            if (AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TexturePath);
            }

            return AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap;
        }

        internal static void ChangeVignetteColor(RealmStudioMap map, Color selectedColor)
        {
            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                {
                    v.VignetteColor = selectedColor.ToArgb();
                    v.IsModified = true;
                }
            }
        }

        internal static void ChangeVignetteStrength(RealmStudioMap map, int value)
        {
            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                {
                    v.VignetteStrength = value;
                    v.IsModified = true;
                }
            }
        }

        internal static void ChangeVignetteShape(RealmStudioMap map, bool isRectangleVignette)
        {
            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                {
                    v.RectangleVignette = isRectangleVignette;
                    v.IsModified = true;
                }
            }
        }
    }
}
