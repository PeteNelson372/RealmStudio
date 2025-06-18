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

                    Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, OceanMediator.OceanTextureOpacity);

                    MapImage OceanTexture = new()
                    {
                        X = 0,
                        Y = 0,
                        Width = MapStateMediator.CurrentMap.MapWidth,
                        Height = MapStateMediator.CurrentMap.MapHeight,
                        MirrorImage = mirrorBackground,
                        ImageName = OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureName,
                        MapImageBitmap = b.ToSKBitmap(),
                        Opacity = OceanMediator.OceanTextureOpacity,
                        Scale = OceanMediator.OceanTextureScale,
                    };

                    oceanTextureLayer.MapLayerComponents[0] = OceanTexture;

                    MapStateMediator.CurrentMap.IsSaved = false;

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

        }

        internal static void ApplyOceanTexture()
        {
            ArgumentNullException.ThrowIfNull(OceanMediator);

            MapStateMediator.CurrentMap.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);
            oceanTextureLayer.MapLayerComponents.Clear();

            if (oceanTextureLayer.MapLayerComponents.Count < 1)
            {
                float scale = OceanMediator.OceanTextureScale;
                bool mirrorBackground = OceanMediator.MirrorOceanTexture;
                Bitmap? textureBitmap = (Bitmap?)OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureBitmap;

                if (textureBitmap != null && scale > 0.0F)
                {
                    if (scale != 1.0F)
                    {
                        // resize the bitmap, but maintain aspect ratio
                        Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(textureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * scale), (int)(MapStateMediator.CurrentMap.MapHeight * scale));

                        Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, OceanMediator.OceanTextureOpacity);

                        MapImage OceanTexture = new()
                        {
                            X = 0,
                            Y = 0,
                            Width = MapStateMediator.CurrentMap.MapWidth,
                            Height = MapStateMediator.CurrentMap.MapHeight,
                            UseShader = mirrorBackground,
                            MirrorImage = mirrorBackground,
                            ImageName = OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureName,
                            MapImageBitmap = b.ToSKBitmap(),
                            Opacity = OceanMediator.OceanTextureOpacity,
                            Scale = OceanMediator.OceanTextureScale,
                        };

                        Cmd_SetOceanTexture cmd = new(MapStateMediator.CurrentMap, OceanTexture);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();
                    }
                    else
                    {
                        Bitmap resizedBitmap = new(textureBitmap, MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                        Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, OceanMediator.OceanTextureOpacity);

                        MapImage OceanTexture = new()
                        {
                            X = 0,
                            Y = 0,
                            Width = MapStateMediator.CurrentMap.MapWidth,
                            Height = MapStateMediator.CurrentMap.MapHeight,
                            UseShader = mirrorBackground,
                            MirrorImage = mirrorBackground,
                            ImageName = OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureName,
                            MapImageBitmap = b.ToSKBitmap(),
                            Opacity = OceanMediator.OceanTextureOpacity,
                            Scale = OceanMediator.OceanTextureScale,
                        };

                        Cmd_SetOceanTexture cmd = new(MapStateMediator.CurrentMap, OceanTexture);
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

        internal static void StartColorPainting(TimerManager applicationTimerManager, SKGLControl glRenderControl)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(OceanMediator);

            if (MapStateMediator.CurrentLayerPaintStroke == null)
            {
                MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap,
                    OceanMediator.OceanPaintColor.ToSKColor(),
                    MapStateMediator.SelectedColorPaintBrush,
                    MapStateMediator.MainUIMediator.SelectedBrushSize / 2,
                    MapBuilder.OCEANDRAWINGLAYER)
                {
                    RenderSurface = SKSurface.Create(glRenderControl.GRContext, false,
                    new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                };

                MapBrush? brush = null;
                if (OceanMediator.OceanPaintBrush == ColorPaintBrush.PatternBrush1)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush1");
                }
                else if (OceanMediator.OceanPaintBrush == ColorPaintBrush.PatternBrush2)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush2");
                }
                else if (OceanMediator.OceanPaintBrush == ColorPaintBrush.PatternBrush3)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush3");
                }
                else if (OceanMediator.OceanPaintBrush == ColorPaintBrush.PatternBrush4)
                {
                    brush = AssetManager.BRUSH_LIST.Find(x => x.BrushName == "Pattern Brush4"); ;
                }

                if (brush != null)
                {
                    brush.BrushBitmap = (Bitmap)Bitmap.FromFile(brush.BrushPath);
                    MapStateMediator.CurrentLayerPaintStroke.StrokeBrush = brush;
                }

                Cmd_AddOceanPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                applicationTimerManager.BrushTimerEnabled = true;
            }
        }

        internal static void StartColorErasing(SKGLControl glRenderControl)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            if (MapStateMediator.CurrentLayerPaintStroke == null)
            {
                MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, SKColors.Empty,
                    ColorPaintBrush.HardBrush, MapStateMediator.MainUIMediator.SelectedBrushSize / 2, MapBuilder.OCEANDRAWINGLAYER, true)
                {
                    RenderSurface = SKSurface.Create(glRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                };

                Cmd_AddOceanPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        internal static void StopColorPainting(TimerManager applicationTimerManager)
        {
            applicationTimerManager.BrushTimerEnabled = false;

            if (MapStateMediator.CurrentLayerPaintStroke != null)
            {
                MapStateMediator.CurrentLayerPaintStroke = null;
            }
        }

        internal static void StopColorErasing()
        {
            MapStateMediator.CurrentLayerPaintStroke = null;
        }
    }
}
