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
    internal class CutCopyPasteManager
    {
        // map components that are currently selected in cut/copy/paste operations
        private static List<MapComponent> _selectedMapComponents = [];

        public static List<MapComponent> SelectedMapComponents
        {
            get { return _selectedMapComponents; }
            set { _selectedMapComponents = value; }
        }

        internal static void ClearComponentSelection()
        {
            SelectedMapComponents.Clear();
            RealmMapState.PreviousSelectedRealmArea = SKRect.Empty;
            RealmMapState.SelectedRealmArea = SKRect.Empty;
            MapBuilder.GetMapLayerByIndex(RealmMapState.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        internal static void CopySelectedComponents()
        {
            if (RealmMapState.SelectedRealmArea != SKRect.Empty)
            {
                // get all objects within the selected area and copy them
                SelectedMapComponents = RealmMapMethods.SelectMapComponentsInArea(RealmMapState.CurrentMap, RealmMapState.SelectedRealmArea);

                // do not cut the selected objects from their layer
                Cmd_CutOrCopyFromArea cmd = new(RealmMapState.CurrentMap, SelectedMapComponents, RealmMapState.SelectedRealmArea, false);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                RealmMapState.PreviousSelectedRealmArea = RealmMapState.SelectedRealmArea;
                RealmMapState.SelectedRealmArea = SKRect.Empty;
                MapBuilder.GetMapLayerByIndex(RealmMapState.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
            }
        }

        internal static void CutSelectedComponents()
        {
            Cmd_CutOrCopyFromArea cmd = new(RealmMapState.CurrentMap, SelectedMapComponents, RealmMapState.SelectedRealmArea, true);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            RealmMapState.PreviousSelectedRealmArea = RealmMapState.SelectedRealmArea;
            RealmMapState.SelectedRealmArea = SKRect.Empty;
            MapBuilder.GetMapLayerByIndex(RealmMapState.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        internal static void PasteSelectedComponentsAtPoint(SKPoint zoomedScrolledPoint)
        {
            if (SelectedMapComponents.Count > 0)
            {
                Cmd_PasteSelectedComponents cmd = new(RealmMapState.CurrentMap, SelectedMapComponents, RealmMapState.PreviousSelectedRealmArea, zoomedScrolledPoint);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }
    }
}
