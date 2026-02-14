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
using RealmStudioShapeRenderingLib;
namespace RealmStudioX
{
    internal sealed class Cmd_DeleteMapRegionPoint(RealmStudioMap map, MapRegion mapRegion, MapRegionPoint regionPoint) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly MapRegion SelectedMapRegion = mapRegion;
        private readonly MapRegionPoint DeletedMapRegionPoint = regionPoint;
        private int PointIndex = -1;

        public void DoOperation()
        {
            if (SelectedMapRegion.MapRegionPoints.Count > 2)
            {
                for (int i = 0; i < SelectedMapRegion.MapRegionPoints.Count - 1; i++)
                {
                    if (SelectedMapRegion.MapRegionPoints[i].PointGuid.ToString() == DeletedMapRegionPoint.PointGuid.ToString())
                    {
                        PointIndex = i;
                        break;
                    }
                }

                SelectedMapRegion.MapRegionPoints.Remove(DeletedMapRegionPoint);
            }
        }

        public void UndoOperation()
        {
            if (PointIndex >= 0 && PointIndex < SelectedMapRegion.MapRegionPoints.Count)
            {
                SelectedMapRegion.MapRegionPoints.Insert(PointIndex, DeletedMapRegionPoint);
            }
        }
    }
}
