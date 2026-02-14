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
using SkiaSharp.Views.Desktop;
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    internal sealed class BoxManager : IMapComponentManager
    {
        private static BoxUIMediator? _boxMediator;

        internal static BoxUIMediator? BoxMediator
        {
            get { return _boxMediator; }
            set { _boxMediator = value; }
        }

        public static IMapComponent? Create()
        {
            PlacedMapBox? newPlacedMapBox = new();
            /*
            ArgumentNullException.ThrowIfNull(BoxMediator);
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (BoxMediator.Box == null || BoxMediator.Box.BoxBitmap == null) return null;

            PlacedMapBox? newPlacedMapBox = new()
            {
                X = (int)MapStateMediator.PreviousCursorPoint.X,
                Y = (int)MapStateMediator.PreviousCursorPoint.Y,
                BoxCenterLeft = BoxMediator.Box.BoxCenterLeft,
                BoxCenterTop = BoxMediator.Box.BoxCenterTop,
                BoxCenterRight = BoxMediator.Box.BoxCenterRight,
                BoxCenterBottom = BoxMediator.Box.BoxCenterBottom,
                BoxTint = BoxMediator.BoxTint
            };

            SKBitmap b = BoxMediator.Box.BoxBitmap.ToSKBitmap();
            SKBitmap resizedBitmap = b.Resize(new SKSizeI((int)BoxMediator.BoxRect.Width, (int)BoxMediator.BoxRect.Height), SKSamplingOptions.Default);

            newPlacedMapBox.BoxBitmap = resizedBitmap.Copy();
            newPlacedMapBox.Width = resizedBitmap.Width;
            newPlacedMapBox.Height = resizedBitmap.Height;

            resizedBitmap.Dispose();
            PaintObjects.BoxPaint.Dispose();

            PaintObjects.BoxPaint = new()
            {
                Style = SKPaintStyle.Fill,
                ColorFilter = SKColorFilter.CreateBlendMode(
                    Extensions.ToSKColor(BoxMediator.BoxTint),
                    SKBlendMode.Modulate) // combine the tint with the bitmap color
            };

            newPlacedMapBox.BoxPaint = PaintObjects.BoxPaint.Clone();

            //Cmd_AddLabelBox cmd = new(MapStateMediator.CurrentMap, newPlacedMapBox);
            //CommandManager.AddCommand(cmd);
            //cmd.DoOperation();
            */
            return newPlacedMapBox;
        }

        public static bool Delete()
        {
            /*
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.SelectedPlacedMapBox == null) return false;

            //Cmd_DeleteLabelBox cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedPlacedMapBox);
            //CommandManager.AddCommand(cmd);
            //cmd.DoOperation();

            MapStateMediator.SelectedPlacedMapBox = null;
            */
            return true;
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            /*
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);


            List<MapComponent> mapLabelComponents = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap,
                MapBuilder.BOXLAYER).MapLayerComponents;

            for (int i = 0; i < mapLabelComponents.Count; i++)
            {
                if (mapLabelComponents[i] is PlacedMapBox mapBox && mapBox.BoxGuid.ToString() == componentGuid.ToString())
                {
                    return mapBox;
                }
            }
            */

            return null;
        }

        public static bool Update()
        {
            /*
            ArgumentNullException.ThrowIfNull(BoxMediator);
            if (MapStateMediator.SelectedPlacedMapBox == null) return false;

            Cmd_ChangeBoxColor cmd = new(MapStateMediator.SelectedPlacedMapBox, BoxMediator.BoxTint);
            //CommandManager.AddCommand(cmd);
            cmd.DoOperation();
            */

            return true;
        }

        internal static void MoveBox(PlacedMapBox placedMapBox, SKPoint zoomedScrolledPoint)
        {
            placedMapBox.X = (int)zoomedScrolledPoint.X - (placedMapBox.Width / 2);
            placedMapBox.Y = (int)zoomedScrolledPoint.Y - (placedMapBox.Height / 2);
        }

        internal static PlacedMapBox? SelectMapBoxAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            PlacedMapBox? selectedBox = null;
            /*
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);





            List<MapComponent> mapLabelComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER).MapLayerComponents;

            for (int i = 0; i < mapLabelComponents.Count; i++)
            {
                if (mapLabelComponents[i] is PlacedMapBox mapBox)
                {
                    SKRect boxRect = new(mapBox.X, mapBox.Y, mapBox.X + mapBox.Width, mapBox.Y + mapBox.Height);

                    if (boxRect.Contains(zoomedScrolledPoint))
                    {
                        selectedBox = mapBox;
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedBox);

            if (selectedBox != null)
            {
                bool isSelected = !selectedBox.IsSelected;
                selectedBox.IsSelected = isSelected;

                if (selectedBox.IsSelected)
                {
                    MapStateMediator.SelectedPlacedMapBox = selectedBox;
                }
                else
                {
                    if (MapStateMediator.SelectedPlacedMapBox != null)
                    {
                        MapStateMediator.SelectedPlacedMapBox.IsSelected = false;
                    }

                    MapStateMediator.SelectedPlacedMapBox = null;
                }
            }
            else
            {
                if (MapStateMediator.SelectedPlacedMapBox != null)
                {
                    MapStateMediator.SelectedPlacedMapBox.IsSelected = false;
                }

                MapStateMediator.SelectedPlacedMapBox = null;
            }

            */

            return selectedBox;
        }

        internal static PlacedMapBox? CreatePlacedMapBox(MapBox mapBox, SKPoint zoomedScrolledPoint, Color boxTintColor)
        {
            PlacedMapBox? newPlacedMapBox = new()
            {
                X = (int)zoomedScrolledPoint.X,
                Y = (int)zoomedScrolledPoint.Y,
                Width = 0,
                Height = 0,
                BoxCenterLeft = mapBox.BoxCenterLeft,
                BoxCenterTop = mapBox.BoxCenterTop,
                BoxCenterRight = mapBox.BoxCenterRight,
                BoxCenterBottom = mapBox.BoxCenterBottom,
                BoxTint = boxTintColor
            };


            PaintObjects.BoxPaint.Dispose();

            PaintObjects.BoxPaint = new()
            {
                Style = SKPaintStyle.Fill,
                ColorFilter = SKColorFilter.CreateBlendMode(
                    Extensions.ToSKColor(boxTintColor),
                    SKBlendMode.Modulate) // combine the tint with the bitmap color
            };

            newPlacedMapBox.BoxPaint = PaintObjects.BoxPaint.Clone();

            return newPlacedMapBox;
        }

        internal static void FinalizeMapBoxes()
        {
            //ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            /*
            // finalize loading of placed map boxes
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BOXLAYER);

            for (int i = 0; i < boxLayer.MapLayerComponents.Count; i++)
            {
                if (boxLayer.MapLayerComponents[i] is PlacedMapBox box && box.BoxBitmap != null)
                {
                    if (box.BoxTint != Color.White)
                    {
                        PaintObjects.BoxPaint.Dispose();
                        PaintObjects.BoxPaint = new()
                        {
                            Style = SKPaintStyle.Fill,
                            ColorFilter = SKColorFilter.CreateBlendMode(
                                Extensions.ToSKColor(box.BoxTint),
                                SKBlendMode.Modulate) // combine the tint with the bitmap color
                        };
                        box.BoxPaint = PaintObjects.BoxPaint.Clone();
                    }
                }
            }

            */
        }
    }
}
