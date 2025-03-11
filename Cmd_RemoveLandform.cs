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
namespace RealmStudio
{
    internal sealed class Cmd_RemoveLandform(RealmStudioMap map, Landform selectedLandform) : IMapOperation
    {
        private readonly RealmStudioMap _map = map;
        private Landform? storedLandform;

        public void DoOperation()
        {
            for (int i = MapBuilder.GetMapLayerByIndex(_map, MapBuilder.LANDFORMLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(_map, MapBuilder.LANDFORMLAYER).MapLayerComponents[i] is Landform lf)
                {
                    if (lf.LandformGuid.ToString() == selectedLandform.LandformGuid.ToString())
                    {
                        storedLandform = lf;
                        MapBuilder.GetMapLayerByIndex(_map, MapBuilder.LANDFORMLAYER).MapLayerComponents.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void UndoOperation()
        {
            if (storedLandform != null)
            {
                MapBuilder.GetMapLayerByIndex(_map, MapBuilder.LANDFORMLAYER).MapLayerComponents.Add(storedLandform);
                LandformMethods.MergeLandforms(_map);
            }
        }
    }
}