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

namespace RealmStudio
{
    internal sealed class Cmd_FillMapWithInteriorFloor(RealmStudioMap map, InteriorFloor floor) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly InteriorFloor FilledFloor = floor;

        public void DoOperation()
        {
            SKRect r = new(2, 2, Map.MapWidth - 2, Map.MapHeight - 2);

            FilledFloor.X = 2;
            FilledFloor.Y = 2;
            FilledFloor.Width = Map.MapWidth - 2;
            FilledFloor.Height = Map.MapHeight - 2;

            FilledFloor.DrawPath.AddRect(r);

            InteriorManager.CreateAllPathsFromDrawnPath(Map, FilledFloor);

            MapLayer interiorLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.INTERIORLAYER);
            interiorLayer.MapLayerComponents.Add(FilledFloor);
        }

        public void UndoOperation()
        {
            MapLayer interiorLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.INTERIORLAYER);
            interiorLayer.MapLayerComponents.Remove(FilledFloor);
        }
    }
}