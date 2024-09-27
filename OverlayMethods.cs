﻿/**************************************************************************************************************************
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
    internal class OverlayMethods
    {
        internal static void CompletePlacedFrame(PlacedMapFrame mapFrame)
        {
            if (mapFrame.FrameBitmap != null)
            {
                float mapWidthScale = (float)mapFrame.MapWidth / mapFrame.FrameBitmap.Width;
                float mapHeightScale = (float)mapFrame.MapHeight / mapFrame.FrameBitmap.Height;

                SKRectI center = new((int)mapFrame.FrameCenterLeft,
                    (int)mapFrame.FrameCenterTop,
                    (int)(mapFrame.FrameBitmap.Width - mapFrame.FrameCenterRight),
                    (int)(mapFrame.FrameBitmap.Height - mapFrame.FrameCenterBottom));

                if (center.IsEmpty || center.Left < 0 || center.Right <= 0 || center.Top < 0 || center.Bottom <= 0)
                {
                    return;
                }
                else if (center.Width <= 0 || center.Height <= 0)
                {
                    // swap 
                    if (center.Right < center.Left)
                    {
                        (center.Left, center.Right) = (center.Right, center.Left);
                    }

                    if (center.Bottom < center.Top)
                    {
                        (center.Top, center.Bottom) = (center.Bottom, center.Top);
                    }
                }

                SKBitmap[] slicedBitmaps = DrawingMethods.SliceNinePatchBitmap(mapFrame.FrameBitmap, center);

                mapFrame.Patch_A = slicedBitmaps[0].Copy();
                mapFrame.Patch_B = slicedBitmaps[1].Copy();
                mapFrame.Patch_C = slicedBitmaps[2].Copy();
                mapFrame.Patch_D = slicedBitmaps[3].Copy();
                mapFrame.Patch_E = slicedBitmaps[4].Copy();
                mapFrame.Patch_F = slicedBitmaps[5].Copy();
                mapFrame.Patch_G = slicedBitmaps[6].Copy();
                mapFrame.Patch_H = slicedBitmaps[7].Copy();
                mapFrame.Patch_I = slicedBitmaps[8].Copy();

                slicedBitmaps[0].Dispose();
                slicedBitmaps[1].Dispose();
                slicedBitmaps[2].Dispose();
                slicedBitmaps[3].Dispose();
                slicedBitmaps[4].Dispose();
                slicedBitmaps[5].Dispose();
                slicedBitmaps[6].Dispose();
                slicedBitmaps[7].Dispose();
                slicedBitmaps[8].Dispose();

                if (mapFrame.FrameBitmap != null
                    && mapFrame.Patch_A != null
                    && mapFrame.Patch_B != null
                    && mapFrame.Patch_C != null
                    && mapFrame.Patch_D != null
                    && mapFrame.Patch_E != null
                    && mapFrame.Patch_F != null
                    && mapFrame.Patch_G != null
                    && mapFrame.Patch_H != null
                    && mapFrame.Patch_I != null)
                {
                    // have to account for the total width and height of the map
                    while ((mapFrame.Patch_A.Width * mapFrame.FrameScale * mapWidthScale)
                        + (mapFrame.Patch_B.Width * mapFrame.FrameScale * mapWidthScale)
                        + (mapFrame.Patch_C.Width * mapFrame.FrameScale * mapWidthScale) > mapFrame.MapWidth)
                    {
                        mapFrame.FrameScale = mapFrame.FrameScale - 0.1F;
                    }

                    while ((mapFrame.Patch_A.Height * mapFrame.FrameScale * mapHeightScale)
                        + (mapFrame.Patch_D.Height * mapFrame.FrameScale * mapHeightScale)
                        + (mapFrame.Patch_G.Height * mapFrame.FrameScale * mapHeightScale) > mapFrame.MapHeight)
                    {
                        mapFrame.FrameScale = mapFrame.FrameScale - 0.1F;
                    }

                    // scale the patches
                    using SKBitmap scaledA = new((int)Math.Round(mapFrame.Patch_A.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_A.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledB = new((int)Math.Round(mapFrame.Patch_B.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_B.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledC = new((int)Math.Round(mapFrame.Patch_C.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_C.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledD = new((int)Math.Round(mapFrame.Patch_D.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_D.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledE = new((int)Math.Round(mapFrame.Patch_E.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_E.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledF = new((int)Math.Round(mapFrame.Patch_F.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_F.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledG = new((int)Math.Round(mapFrame.Patch_G.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_G.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledH = new((int)Math.Round(mapFrame.Patch_H.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_H.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledI = new((int)Math.Round(mapFrame.Patch_I.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.Patch_I.Height * mapFrame.FrameScale * mapHeightScale));

                    mapFrame.Patch_A.ScalePixels(scaledA, SKFilterQuality.High);
                    mapFrame.Patch_B.ScalePixels(scaledB, SKFilterQuality.High);
                    mapFrame.Patch_C.ScalePixels(scaledC, SKFilterQuality.High);
                    mapFrame.Patch_D.ScalePixels(scaledD, SKFilterQuality.High);
                    mapFrame.Patch_E.ScalePixels(scaledE, SKFilterQuality.High);
                    mapFrame.Patch_F.ScalePixels(scaledF, SKFilterQuality.High);
                    mapFrame.Patch_G.ScalePixels(scaledG, SKFilterQuality.High);
                    mapFrame.Patch_H.ScalePixels(scaledH, SKFilterQuality.High);
                    mapFrame.Patch_I.ScalePixels(scaledI, SKFilterQuality.High);

                    mapFrame.Patch_A.Dispose();
                    mapFrame.Patch_B.Dispose();
                    mapFrame.Patch_C.Dispose();
                    mapFrame.Patch_D.Dispose();
                    mapFrame.Patch_E.Dispose();
                    mapFrame.Patch_F.Dispose();
                    mapFrame.Patch_G.Dispose();
                    mapFrame.Patch_H.Dispose();
                    mapFrame.Patch_I.Dispose();

                    mapFrame.Patch_A = scaledA.Copy();
                    mapFrame.Patch_B = scaledB.Copy();
                    mapFrame.Patch_C = scaledC.Copy();
                    mapFrame.Patch_D = scaledD.Copy();
                    mapFrame.Patch_E = scaledE.Copy();
                    mapFrame.Patch_F = scaledF.Copy();
                    mapFrame.Patch_G = scaledG.Copy();
                    mapFrame.Patch_H = scaledH.Copy();
                    mapFrame.Patch_I = scaledI.Copy();

                }

                SKPaint framePaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    ColorFilter = SKColorFilter.CreateBlendMode(
                        Extensions.ToSKColor(mapFrame.FrameTint),
                        SKBlendMode.Modulate), // combine the tint with the bitmap color
                };

                mapFrame.FramePaint = framePaint;
            }
        }

        internal static void CreateFrame(RealmStudioMap map, MapFrame? frame, Color frameTint, float frameScale)
        {
            if (frame == null || frame.FrameBitmap == null) return;

            PlacedMapFrame mapFrame = new()
            {
                X = 0,
                Y = 0,
                Width = map.MapWidth,
                Height = map.MapHeight,
                MapWidth = map.MapWidth,
                MapHeight = map.MapHeight,
                FrameBitmap = frame.FrameBitmap.Copy(),
                FrameScale = frameScale,
                FrameCenterLeft = frame.FrameCenterLeft,
                FrameCenterTop = frame.FrameCenterTop,
                FrameCenterRight = frame.FrameCenterRight,
                FrameCenterBottom = frame.FrameCenterBottom,
                FrameTint = frameTint,
            };

            using SKPaint framePaint = new()
            {
                Style = SKPaintStyle.Fill,
                ColorFilter = SKColorFilter.CreateBlendMode(
                Extensions.ToSKColor(mapFrame.FrameTint),
                SKBlendMode.Modulate) // combine the tint with the bitmap color
            };

            mapFrame.FramePaint = framePaint.Clone();

            CompletePlacedFrame(mapFrame);

            // there can only be one frame on the map, so remove any existing frame
            RemoveAllFrames(map);

            MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents.Add(mapFrame);
        }

        internal static void RemoveAllFrames(RealmStudioMap map)
        {
            MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents.Clear();
        }
    }
}
