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
    internal sealed class WindroseManager : IMapComponentManager
    {
        private static WindroseUIMediator? _windroseMediator;

        internal static WindroseUIMediator? WindroseMediator
        {
            get { return _windroseMediator; }
            set { _windroseMediator = value; }
        }

        public static IMapComponent? Create()
        {
            //ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            //ArgumentNullException.ThrowIfNull(WindroseMediator);
            //ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            //if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.PlaceWindrose)
            {
                MapWindrose windrose = new()
                {
                    //ParentMap = MapStateMediator.CurrentMap,
                    InnerCircles = WindroseMediator.InnerCircleCount,
                    InnerRadius = WindroseMediator.InnerCircleRadius,
                    FadeOut = WindroseMediator.FadeOut,
                    LineWidth = WindroseMediator.LineWidth,
                    OuterRadius = WindroseMediator.OuterRadius,
                    WindroseColor = WindroseMediator.WindroseColor,
                    DirectionCount = WindroseMediator.DirectionCount,
                };

                windrose.WindrosePaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = windrose.LineWidth,
                    Color = windrose.WindroseColor.ToSKColor(),
                    IsAntialias = true,
                };

                //MapStateMediator.CurrentWindrose = windrose;
                return windrose;
            }

            return null;
        }

        public static bool Delete()
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(WindroseMediator);

            /*
            if (MapStateMediator.CurrentWindrose != null)
            {
                MapStateMediator.CurrentWindrose.InnerCircles = WindroseMediator.InnerCircleCount;
                MapStateMediator.CurrentWindrose.InnerRadius = WindroseMediator.InnerCircleRadius;
                MapStateMediator.CurrentWindrose.FadeOut = WindroseMediator.FadeOut;
                MapStateMediator.CurrentWindrose.LineWidth = WindroseMediator.LineWidth;
                MapStateMediator.CurrentWindrose.OuterRadius = WindroseMediator.OuterRadius;
                MapStateMediator.CurrentWindrose.WindroseColor = WindroseMediator.WindroseColor;
                MapStateMediator.CurrentWindrose.DirectionCount = WindroseMediator.DirectionCount;

                MapStateMediator.CurrentWindrose.WindrosePaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = MapStateMediator.CurrentWindrose.LineWidth,
                    Color = MapStateMediator.CurrentWindrose.WindroseColor.ToSKColor(),
                    IsAntialias = true,
                };

                return true;
            }
            */

            return false;
        }

        internal static void AddWindrowseAtCurrentPoint()
        {
            /*
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.CurrentWindrose != null)
            {
                MapStateMediator.CurrentWindrose.X = (int)MapStateMediator.CurrentCursorPoint.X;
                MapStateMediator.CurrentWindrose.Y = (int)MapStateMediator.CurrentCursorPoint.Y;

                //Cmd_AddWindrose cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentWindrose);
                //CommandManager.AddCommand(cmd);
                //cmd.DoOperation();

                Create();
            }
            */
        }

        internal static void RemoveAllWindroses()
        {
            //ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            /*
            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WINDROSELAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WINDROSELAYER).MapLayerComponents[i] is MapWindrose)
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WINDROSELAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            */

            //MapStateMediator.CurrentWindrose = null;
        }

        internal static void MoveWindrose(MapWindrose? windrose, SKPoint zoomedScrolledPoint)
        {
            if (windrose == null) return;

            windrose.X = (int)zoomedScrolledPoint.X;
            windrose.Y = (int)zoomedScrolledPoint.Y;
        }
    }
}
