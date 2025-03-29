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
    internal sealed class OverlayMethods
    {
        internal static void CompletePlacedFrame(PlacedMapFrame mapFrame)
        {
            if (mapFrame.FrameBitmap != null)
            {
                float mapWidthScale = (float)mapFrame.Width / mapFrame.FrameBitmap.Width;
                float mapHeightScale = (float)mapFrame.Height / mapFrame.FrameBitmap.Height;

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

                SKBitmap[] bitmapSlices = DrawingMethods.SliceNinePatchBitmap(mapFrame.FrameBitmap, center);

                mapFrame.PatchA = bitmapSlices[0].Copy();
                mapFrame.PatchB = bitmapSlices[1].Copy();
                mapFrame.PatchC = bitmapSlices[2].Copy();
                mapFrame.PatchD = bitmapSlices[3].Copy();
                mapFrame.PatchE = bitmapSlices[4].Copy();
                mapFrame.PatchF = bitmapSlices[5].Copy();
                mapFrame.PatchG = bitmapSlices[6].Copy();
                mapFrame.PatchH = bitmapSlices[7].Copy();
                mapFrame.PatchI = bitmapSlices[8].Copy();

                bitmapSlices[0].Dispose();
                bitmapSlices[1].Dispose();
                bitmapSlices[2].Dispose();
                bitmapSlices[3].Dispose();
                bitmapSlices[4].Dispose();
                bitmapSlices[5].Dispose();
                bitmapSlices[6].Dispose();
                bitmapSlices[7].Dispose();
                bitmapSlices[8].Dispose();

                if (mapFrame.FrameBitmap != null
                    && mapFrame.PatchA != null
                    && mapFrame.PatchB != null
                    && mapFrame.PatchC != null
                    && mapFrame.PatchD != null
                    && mapFrame.PatchE != null
                    && mapFrame.PatchF != null
                    && mapFrame.PatchG != null
                    && mapFrame.PatchH != null
                    && mapFrame.PatchI != null)
                {
                    // have to account for the total width and height of the map
                    while ((mapFrame.PatchA.Width * mapFrame.FrameScale * mapWidthScale)
                        + (mapFrame.PatchB.Width * mapFrame.FrameScale * mapWidthScale)
                        + (mapFrame.PatchC.Width * mapFrame.FrameScale * mapWidthScale) > mapFrame.Width)
                    {
                        mapFrame.FrameScale -= 0.1F;
                    }

                    while ((mapFrame.PatchA.Height * mapFrame.FrameScale * mapHeightScale)
                        + (mapFrame.PatchD.Height * mapFrame.FrameScale * mapHeightScale)
                        + (mapFrame.PatchG.Height * mapFrame.FrameScale * mapHeightScale) > mapFrame.Height)
                    {
                        mapFrame.FrameScale -= 0.1F;
                    }

                    // scale the patches
                    using SKBitmap scaledA = new((int)Math.Round(mapFrame.PatchA.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchA.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledB = new((int)Math.Round(mapFrame.PatchB.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchB.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledC = new((int)Math.Round(mapFrame.PatchC.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchC.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledD = new((int)Math.Round(mapFrame.PatchD.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchD.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledE = new((int)Math.Round(mapFrame.PatchE.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchE.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledF = new((int)Math.Round(mapFrame.PatchF.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchF.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledG = new((int)Math.Round(mapFrame.PatchG.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchG.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledH = new((int)Math.Round(mapFrame.PatchH.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchH.Height * mapFrame.FrameScale * mapHeightScale));
                    using SKBitmap scaledI = new((int)Math.Round(mapFrame.PatchI.Width * mapFrame.FrameScale * mapWidthScale), (int)Math.Round(mapFrame.PatchI.Height * mapFrame.FrameScale * mapHeightScale));

                    mapFrame.PatchA.ScalePixels(scaledA, SKSamplingOptions.Default);
                    mapFrame.PatchB.ScalePixels(scaledB, SKSamplingOptions.Default);
                    mapFrame.PatchC.ScalePixels(scaledC, SKSamplingOptions.Default);
                    mapFrame.PatchD.ScalePixels(scaledD, SKSamplingOptions.Default);
                    mapFrame.PatchE.ScalePixels(scaledE, SKSamplingOptions.Default);
                    mapFrame.PatchF.ScalePixels(scaledF, SKSamplingOptions.Default);
                    mapFrame.PatchG.ScalePixels(scaledG, SKSamplingOptions.Default);
                    mapFrame.PatchH.ScalePixels(scaledH, SKSamplingOptions.Default);
                    mapFrame.PatchI.ScalePixels(scaledI, SKSamplingOptions.Default);

                    mapFrame.PatchA.Dispose();
                    mapFrame.PatchB.Dispose();
                    mapFrame.PatchC.Dispose();
                    mapFrame.PatchD.Dispose();
                    mapFrame.PatchE.Dispose();
                    mapFrame.PatchF.Dispose();
                    mapFrame.PatchG.Dispose();
                    mapFrame.PatchH.Dispose();
                    mapFrame.PatchI.Dispose();

                    mapFrame.PatchA = scaledA.Copy();
                    mapFrame.PatchB = scaledB.Copy();
                    mapFrame.PatchC = scaledC.Copy();
                    mapFrame.PatchD = scaledD.Copy();
                    mapFrame.PatchE = scaledE.Copy();
                    mapFrame.PatchF = scaledF.Copy();
                    mapFrame.PatchG = scaledG.Copy();
                    mapFrame.PatchH = scaledH.Copy();
                    mapFrame.PatchI = scaledI.Copy();

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
            MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents.Clear();

            MapBuilder.GetMapLayerByIndex(map, MapBuilder.FRAMELAYER).MapLayerComponents.Add(mapFrame);
        }
    }
}
