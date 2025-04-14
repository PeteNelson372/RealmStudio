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
using SkiaSharp;

namespace RealmStudio
{
    internal class WorldGlobeManager
    {
        public static ThreeDView? CurrentWorldGlobeMapView { get; set; }

        internal static void ShowWorldGlobe()
        {
            SKBitmap worldTexture = new(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);
            using SKCanvas canvas = new(worldTexture);

            // render for world globe view
            // -- don't render labels
            // -- don't render frame
            // -- don't render map scale
            // -- don't render regions
            // -- don't render vignette
            // can/should the map be automatically changed (background, landform texture) temporarily
            // to look like a world viewed from space?
            MapRenderMethods.RenderMapFor3DGlobeView(MapStateMediator.CurrentMap, canvas);

            // fix map edges so that there are no seams or gaps on the globe
            // crop 3 pixels off the edges, then resize to original size
            using SKBitmap croppedWorldTexture = new(worldTexture.Width - 6, worldTexture.Height - 6);
            using SKCanvas croppedCanvas = new(croppedWorldTexture);
            SKRect sourceRect = new(3, 3, worldTexture.Width - 3, worldTexture.Height - 3);
            SKRect destRect = new(0, 0, croppedWorldTexture.Width, croppedWorldTexture.Height);
            croppedCanvas.DrawBitmap(worldTexture, sourceRect, destRect);

            // resize the cropped texture to the original size
            SKBitmap resizedBitmap = croppedWorldTexture.Resize(new SKImageInfo(worldTexture.Width, worldTexture.Height), SKSamplingOptions.Default);

            ThreeDView td = new("World Globe");
            CurrentWorldGlobeMapView = td;

            // show animation controls and button to load background texture (stars, etc.)
            td.AnimationGroup.Visible = true;


            td.Show();
            td.ShowWorldGlobe(resizedBitmap);
        }
    }
}
