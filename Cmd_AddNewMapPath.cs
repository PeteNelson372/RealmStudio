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
namespace RealmStudio
{
    internal sealed class Cmd_AddNewMapPath(RealmStudioMap map, MapPath newMapPath) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly MapPath NewMapPath = newMapPath;

        public void DoOperation()
        {
            if (NewMapPath.DrawOverSymbols)
            {
                MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Add(NewMapPath);
            }
            else
            {
                MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Add(NewMapPath);
            }
        }

        public void UndoOperation()
        {
            if (NewMapPath.DrawOverSymbols)
            {
                for (int i = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
                {
                    if (MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents[i] is MapPath mp)
                    {
                        if (mp.MapPathGuid.ToString() == NewMapPath.MapPathGuid.ToString())
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
                        if (mp.MapPathGuid.ToString() == NewMapPath.MapPathGuid.ToString())
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
