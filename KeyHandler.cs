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
    internal sealed class KeyHandler
    {
        internal static void HandleKey(Keys keyCode)
        {
            //ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            //ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            switch (keyCode)
            {
                case Keys.Escape:
                    {
                        //MapStateMediator.ResetUI();
                    }
                    break;
                case Keys.Delete:
                    {
                        //MapStateMediator.DeleteSelectedMapObjects();
                    }
                    break;
                case Keys.PageUp:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    if (RealmStudioMainForm.ModifierKeys == Keys.Control)
                                    {
                                        SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 5);
                                    }
                                    else if (RealmStudioMainForm.ModifierKeys == Keys.None)
                                    {
                                        SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 1);
                                    }
                                }
                                break;
                            case MapDrawingMode.RegionSelect:
                                {
                                    RegionManager.MoveSelectedRegionInRenderOrder(ComponentMoveDirection.Up);
                                }
                                break;
                        }
                        */
                    }
                    break;
                case Keys.PageDown:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    if (RealmStudioMainForm.ModifierKeys == Keys.Control)
                                    {
                                        SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 5);
                                    }
                                    else if (RealmStudioMainForm.ModifierKeys == Keys.None)
                                    {
                                        SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 1);
                                    }
                                }
                                break;
                            case MapDrawingMode.RegionSelect:
                                {
                                    RegionManager.MoveSelectedRegionInRenderOrder(ComponentMoveDirection.Down);
                                }
                                break;
                        }
                        */
                    }
                    break;
                case Keys.End:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    // move symbol to bottom of render order
                                    SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 1, true);
                                }
                                break;
                        }
                        */
                    }
                    break;
                case Keys.Home:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    // move symbol to top of render order
                                    SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 1, true);
                                }
                                break;
                        }
                        */
                    }
                    break;
                case Keys.Down:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    if (MapStateMediator.SelectedMapSymbol != null)
                                    {
                                        MapStateMediator.SelectedMapSymbol.Y = Math.Min(MapStateMediator.SelectedMapSymbol.Y + 1, MapStateMediator.CurrentMap.MapHeight);
                                    }
                                }
                                break;
                            case MapDrawingMode.DrawingSelect:
                                {
                                    DrawnMapComponent? dmc = DrawingManager.FindSelectedDrawnMapComponent(MapStateMediator.CurrentMap);
                                    DrawingManager.MoveDrawnComponent(dmc, ComponentMoveDirection.Down);
                                    break;
                                }
                        }
                        */
                    }
                    break;
                case Keys.Up:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    if (MapStateMediator.SelectedMapSymbol != null)
                                    {
                                        MapStateMediator.SelectedMapSymbol.Y = Math.Max(0, MapStateMediator.SelectedMapSymbol.Y - 1);
                                    }
                                }
                                break;
                            case MapDrawingMode.DrawingSelect:
                                {
                                    DrawnMapComponent? dmc = DrawingManager.FindSelectedDrawnMapComponent(MapStateMediator.CurrentMap);
                                    DrawingManager.MoveDrawnComponent(dmc, ComponentMoveDirection.Up);
                                    break;
                                }
                        }
                        */
                    }
                    break;
                case Keys.Left:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    if (MapStateMediator.SelectedMapSymbol != null)
                                    {
                                        if (MapStateMediator.SelectedMapSymbol != null)
                                        {
                                            MapStateMediator.SelectedMapSymbol.X = Math.Max(0, MapStateMediator.SelectedMapSymbol.X - 1);
                                        }
                                    }
                                }
                                break;
                            case MapDrawingMode.DrawingSelect:
                                {
                                    DrawnMapComponent? dmc = DrawingManager.FindSelectedDrawnMapComponent(MapStateMediator.CurrentMap);
                                    DrawingManager.MoveDrawnComponent(dmc, ComponentMoveDirection.Left);
                                    break;
                                }
                        }
                        */
                    }
                    break;
                case Keys.Right:
                    {
                        /*
                        switch (MapStateMediator.MainUIMediator.CurrentDrawingMode)
                        {
                            case MapDrawingMode.SymbolSelect:
                                {
                                    if (MapStateMediator.SelectedMapSymbol != null)
                                    {
                                        MapStateMediator.SelectedMapSymbol.X = Math.Min(MapStateMediator.SelectedMapSymbol.X + 1, MapStateMediator.CurrentMap.MapWidth);
                                    }
                                }
                                break;
                            case MapDrawingMode.DrawingSelect:
                                {
                                    DrawnMapComponent? dmc = DrawingManager.FindSelectedDrawnMapComponent(MapStateMediator.CurrentMap);
                                    DrawingManager.MoveDrawnComponent(dmc, ComponentMoveDirection.Right);
                                    break;
                                }
                        }
                        */
                    }
                    break;
            }
        }
    }
}
