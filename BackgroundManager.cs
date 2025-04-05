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
    internal sealed class BackgroundManager : IMapComponentManager
    {
        private static BackgroundUIMediator? _backgroundMediator;

        internal static BackgroundUIMediator? BackgroundMediator
        {
            get { return _backgroundMediator; }
            set { _backgroundMediator = value; }
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
            ArgumentNullException.ThrowIfNull(BackgroundMediator);

            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);
            Bitmap? textureBitmap = BackgroundMediator.BackgroundTextureList[BackgroundMediator.BackgroundTextureIndex].TextureBitmap;
            float scale = BackgroundMediator.BackgroundTextureScale;
            bool mirrorBackground = BackgroundMediator.MirrorBackgroundTexture;

            if (scale == 0.0F)
            {
                baseLayer.ShowLayer = false;
                return false;
            }
            else
            {
                baseLayer.ShowLayer = true;

                if (baseLayer.MapLayerComponents.Count == 1 && textureBitmap != null && scale > 0.0F && scale <= 1.0F)
                {
                    Bitmap resizedBitmap;

                    if (scale != 1.0F)
                    {
                        // resize the bitmap, but maintain aspect ratio
                        resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * scale), (int)(MapStateMediator.CurrentMap.MapHeight * scale));
                    }
                    else
                    {
                        resizedBitmap = new(textureBitmap,
                            MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);
                    }

                    MapImage BackgroundTexture = new()
                    {
                        X = 0,
                        Y = 0,
                        Width = MapStateMediator.CurrentMap.MapWidth,
                        Height = MapStateMediator.CurrentMap.MapHeight,
                        MirrorImage = mirrorBackground,
                        MapImageBitmap = resizedBitmap.ToSKBitmap(),
                    };

                    baseLayer.MapLayerComponents[0] = BackgroundTexture;

                    return true;
                }
            }

            return false;
        }

        public static bool Delete()
        {
            throw new NotImplementedException();
        }

        internal static void ApplyBackgroundTexture(Bitmap? textureBitmap, float scale, bool mirrorBackground)
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);

            if (baseLayer.MapLayerComponents.Count < 1 && textureBitmap != null && scale > 0.0F && scale <= 1.0F)
            {
                if (scale != 1.0F)
                {
                    // resize the bitmap, but maintain aspect ratio
                    Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                        (int)(MapStateMediator.CurrentMap.MapWidth * scale), (int)(MapStateMediator.CurrentMap.MapHeight * scale));

                    Cmd_SetBackgroundTexture cmd = new(MapStateMediator.CurrentMap, Extensions.ToSKBitmap(resizedBitmap), mirrorBackground);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
                else
                {
                    Bitmap resizedBitmap = new(textureBitmap,
                        MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                    Cmd_SetBackgroundTexture cmd = new(MapStateMediator.CurrentMap, Extensions.ToSKBitmap(resizedBitmap), mirrorBackground);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
                }
            }
        }

        internal static void ClearBackgroundTexture()
        {
            MapLayer backgroundLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);

            if (backgroundLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerTexture = (MapImage)MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER).MapLayerComponents.First();

                Cmd_ClearBackgroundTexture cmd = new(MapStateMediator.CurrentMap, layerTexture);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static void FillBackgroundTexture()
        {
            ArgumentNullException.ThrowIfNull(BackgroundMediator);

            ClearBackgroundTexture();

            ApplyBackgroundTexture(BackgroundMediator.BackgroundTextureList[BackgroundMediator.BackgroundTextureIndex].TextureBitmap,
                BackgroundMediator.BackgroundTextureScale,
                BackgroundMediator.MirrorBackgroundTexture);
        }
    }
}
