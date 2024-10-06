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
* contact@brookmonte.com
*
***************************************************************************************************************************/
namespace RealmStudio
{
    class Cmd_RemoveWaterFeature(RealmStudioMap map, IWaterFeature selectedWaterFeature) : IMapOperation
    {
        private readonly RealmStudioMap _map = map;
        private IWaterFeature? storedWaterFeature = null;

        public void DoOperation()
        {
            for (int i = MapBuilder.GetMapLayerByIndex(_map, MapBuilder.WATERLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(_map, MapBuilder.WATERLAYER).MapLayerComponents[i] is IWaterFeature iwf)
                {
                    if (iwf is WaterFeature wf)
                    {
                        WaterFeature? swf = selectedWaterFeature as WaterFeature;
                        if (swf != null && wf.WaterFeatureGuid.ToString() == swf.WaterFeatureGuid.ToString())
                        {
                            storedWaterFeature = wf;
                            MapBuilder.GetMapLayerByIndex(_map, MapBuilder.WATERLAYER).MapLayerComponents.RemoveAt(i);

                        }
                    }
                    else if (iwf is River r)
                    {
                        River? sr = selectedWaterFeature as River;
                        if (sr != null && r.MapRiverGuid.ToString() == sr.MapRiverGuid.ToString())
                        {
                            storedWaterFeature = r;
                            MapBuilder.GetMapLayerByIndex(_map, MapBuilder.WATERLAYER).MapLayerComponents.RemoveAt(i);

                        }
                    }
                }
            }

        }

        public void UndoOperation()
        {
            if (storedWaterFeature != null)
            {
                if (storedWaterFeature is WaterFeature wf)
                {
                    MapBuilder.GetMapLayerByIndex(_map, MapBuilder.WATERLAYER).MapLayerComponents.Add(wf);
                }
                else if (storedWaterFeature is River r)
                {
                    MapBuilder.GetMapLayerByIndex(_map, MapBuilder.WATERLAYER).MapLayerComponents.Add(r);
                }
            }
        }
    }
}
