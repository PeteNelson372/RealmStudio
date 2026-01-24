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
    internal sealed class VignetteManager : IMapComponentManager
    {
        private static VignetteUIMediator? _vignetteMediator;

        internal static VignetteUIMediator? VignetteMediator
        {
            get { return _vignetteMediator; }
            set { _vignetteMediator = value; }
        }


        public static IMapComponent? Create()
        {
            ArgumentNullException.ThrowIfNull(VignetteMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MapLayer? vignetteLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER);
            RealmStudioMainForm? mainForm = UtilityMethods.GetMainForm();

            if (mainForm != null && vignetteLayer.MapLayerComponents.Count == 0)
            {
                MapVignette vignette = new()
                {
                    ParentMap = MapStateMediator.CurrentMap,
                    VignetteColor = VignetteMediator.VignetteColor.ToArgb(),
                    VignetteStrength = VignetteMediator.VignetteStrength,
                    VignetteShape = VignetteMediator.VignetteShape,
                    VignetteRenderSurface = SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false,
                                        new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight)),
                };

                vignetteLayer.MapLayerComponents.Add(vignette);

                return vignette;
            }

            return null;
        }

        public static bool Delete()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER).MapLayerComponents.Clear();
            return true;
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(VignetteMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                {
                    v.VignetteColor = VignetteMediator.VignetteColor.ToArgb();
                    v.VignetteStrength = VignetteMediator.VignetteStrength;
                    v.VignetteShape = VignetteMediator.VignetteShape;
                    v.IsModified = true;
                }
            }

            return true;
        }

        internal static void FinalizeMapVignette()
        {
            ArgumentNullException.ThrowIfNull(VignetteMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);


            // finalize loading of vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER);
            if (vignetteLayer.MapLayerComponents.Count == 1)
            {
                MapVignette vignette = (MapVignette)vignetteLayer.MapLayerComponents[0];

                vignette.ParentMap = MapStateMediator.CurrentMap;
                vignette.Width = MapStateMediator.CurrentMap.MapWidth;
                vignette.Height = MapStateMediator.CurrentMap.MapHeight;
                vignette.VignetteRenderSurface ??= SKSurface.Create(MapStateMediator.MainUIMediator.MainForm.SKGLRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight));

                VignetteMediator.Initialize(vignette.VignetteStrength, Color.FromArgb(vignette.VignetteColor), vignette.VignetteShape);

            }
        }
    }
}
