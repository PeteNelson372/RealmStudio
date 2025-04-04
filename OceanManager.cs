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
    internal sealed class OceanManager : IMapComponentManager
    {
        private static OceanUIMediator? _oceanUIMediator;

        internal static OceanUIMediator? OceanMediator
        {
            get { return _oceanUIMediator; }
            set { _oceanUIMediator = value; }
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
            // TODO: update ocean colors and drawing layer
            ArgumentNullException.ThrowIfNull(OceanMediator);

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);

            Bitmap? textureBitmap = OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureBitmap;
            float scale = OceanMediator.OceanTextureScale;
            bool mirrorBackground = OceanMediator.MirrorOceanTexture;

            if (scale == 0.0F)
            {
                oceanTextureLayer.ShowLayer = false;
                return false;
            }
            else
            {
                oceanTextureLayer.ShowLayer = true;

                if (oceanTextureLayer.MapLayerComponents.Count == 1 && textureBitmap != null && scale > 0.0F)
                {
                    Bitmap resizedBitmap;

                    if (scale != 100.0F)
                    {
                        // resize the bitmap, but maintain aspect ratio
                        resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * scale / 100.0F), (int)(MapStateMediator.CurrentMap.MapHeight * scale / 100.0F));
                    }
                    else
                    {
                        resizedBitmap = new(textureBitmap,
                            MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);
                    }

                    Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, OceanMediator.OceanTextureOpacity / 100.0F);

                    MapImage OceanTexture = new()
                    {
                        X = 0,
                        Y = 0,
                        Width = MapStateMediator.CurrentMap.MapWidth,
                        Height = MapStateMediator.CurrentMap.MapHeight,
                        MirrorImage = mirrorBackground,
                        MapImageBitmap = b.ToSKBitmap(),
                    };

                    oceanTextureLayer.MapLayerComponents[0] = OceanTexture;

                    return true;
                }
            }

            return false;
        }

        public static bool Delete()
        {
            throw new NotImplementedException();
        }

        internal static void FinalizeOceanLayer()
        {
            // finalize loading of ocean drawing layer
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANDRAWINGLAYER);

            for (int i = 0; i < oceanDrawingLayer.MapLayerComponents.Count; i++)
            {
                if (oceanDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                {
                    paintStroke.ParentMap = MapStateMediator.CurrentMap;

                    if (!paintStroke.Erase)
                    {
                        paintStroke.ShaderPaint = PaintObjects.OceanPaint;
                    }
                    else
                    {
                        paintStroke.ShaderPaint = PaintObjects.OceanEraserPaint;
                    }
                }
            }
        }

        internal static void ApplyOceanTexture()
        {
            ArgumentNullException.ThrowIfNull(OceanMediator);

            MapStateMediator.CurrentMap.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count < 1)
            {
                float scale = OceanMediator.OceanTextureScale;
                bool mirrorBackground = OceanMediator.MirrorOceanTexture;
                Bitmap? textureBitmap = (Bitmap?)OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureBitmap;

                if (textureBitmap != null && scale > 0.0F)
                {
                    if (scale != 100.0F)
                    {
                        // resize the bitmap, but maintain aspect ratio
                        Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * scale / 100.0F), (int)(MapStateMediator.CurrentMap.MapHeight * scale / 100.0F));

                        Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, OceanMediator.OceanTextureOpacity / 100.0F);

                        Cmd_SetOceanTexture cmd = new(MapStateMediator.CurrentMap, Extensions.ToSKBitmap(b), mirrorBackground);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();
                    }
                    else
                    {
                        Bitmap resizedBitmap = new(textureBitmap, MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                        Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, OceanMediator.OceanTextureOpacity / 100.0F);

                        Cmd_SetOceanTexture cmd = new(MapStateMediator.CurrentMap, Extensions.ToSKBitmap(b), mirrorBackground);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();
                    }
                }
            }
        }

        internal static void ClearOceanColor()
        {
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (oceanTextureOverlayLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerColor = (MapImage)MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER).MapLayerComponents.First();

                Cmd_ClearOceanColor cmd = new(MapStateMediator.CurrentMap, layerColor);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static void FillOceanColor()
        {
            ArgumentNullException.ThrowIfNull(OceanMediator);

            MapStateMediator.CurrentMap.IsSaved = false;

            // get the user-selected ocean color
            Color fillColor = OceanMediator.OceanFillColor;
            SKBitmap b = new(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

            using (SKCanvas canvas = new(b))
            {
                canvas.Clear(Extensions.ToSKColor(fillColor));
            }

            Cmd_SetOceanColor cmd = new(MapStateMediator.CurrentMap, b);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();
        }

        internal static void RemoveOceanTexture()
        {
            MapStateMediator.CurrentMap.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerTexture = (MapImage)MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER).MapLayerComponents.First();

                Cmd_ClearOceanTexture cmd = new(MapStateMediator.CurrentMap, layerTexture);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }
    }
}
