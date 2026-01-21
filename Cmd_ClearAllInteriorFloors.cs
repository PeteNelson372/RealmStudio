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
    internal sealed class Cmd_ClearAllInteriorFloors(RealmStudioMap map) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly List<InteriorFloor> StoredFloors = [];
        private readonly List<LayerPaintStroke> StoredLayerPaintStrokes = [];

        public void DoOperation()
        {
            MapLayer interiorLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.INTERIORLAYER);

            for (int i = 0; i < interiorLayer.MapLayerComponents.Count; i++)
            {
                StoredFloors.Add((InteriorFloor)interiorLayer.MapLayerComponents[i]);
            }

            interiorLayer.MapLayerComponents.Clear();

            MapLayer interiorDrawingLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.INTERIORDRAWINGLAYER);

            for (int i = 0; i < interiorDrawingLayer.MapLayerComponents.Count; i++)
            {
                StoredLayerPaintStrokes.Add((LayerPaintStroke)interiorDrawingLayer.MapLayerComponents[i]);
            }
        }

        public void UndoOperation()
        {
            MapLayer interiorLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.INTERIORLAYER);

            interiorLayer.MapLayerComponents.Clear();

            foreach (InteriorFloor f in StoredFloors)
            {
                interiorLayer.MapLayerComponents.Add(f);
            }

            MapLayer interiorDrawingLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.INTERIORDRAWINGLAYER);

            foreach (LayerPaintStroke lps in StoredLayerPaintStrokes)
            {
                interiorDrawingLayer.MapLayerComponents.Add(lps);
            }
        }
    }
}