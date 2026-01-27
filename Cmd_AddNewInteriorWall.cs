/**************************************************************************************************************************
* Copyright 2026, Peter R. Nelson
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
    internal sealed class Cmd_AddNewInteriorWall(RealmStudioMap map, InteriorWall newWall) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly InteriorWall NewInteriorWall = newWall;

        public void DoOperation()
        {
            // Add the new interior wall to the appropriate layer; walls are added to path layers
            if (NewInteriorWall.DrawOverSymbols)
            {
                MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Add(NewInteriorWall);
            }
            else
            {
                MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Add(NewInteriorWall);
            }
        }

        public void UndoOperation()
        {
            if (NewInteriorWall.DrawOverSymbols)
            {
                for (int i = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
                {
                    if (MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents[i] is MapPath mp)
                    {
                        if (mp.MapPathGuid.ToString() == NewInteriorWall.WallGuid.ToString())
                        {
                            MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
                {
                    if (MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents[i] is MapPath mp)
                    {
                        if (mp.MapPathGuid.ToString() == NewInteriorWall.WallGuid.ToString())
                        {
                            MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
    }
}
