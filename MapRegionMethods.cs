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
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal class MapRegionMethods
    {
        public static int POINT_CIRCLE_RADIUS = 5;

        public static bool EDITING_REGION;
        public static MapRegionPoint? NEW_REGION_POINT;
        public static int PREVIOUS_REGION_POINT_INDEX = -1;
        public static int NEXT_REGION_POINT_INDEX = -1;

        public static SKPath GetLinePathFromRegionPoints(List<MapRegionPoint> points)
        {
            SKPath path = new();

            path.MoveTo(points.First().RegionPoint);

            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i].RegionPoint);
            }

            path.LineTo(points.First().RegionPoint);

            path.Close();

            return path;
        }

        public static SKPathEffect? ConstructRegionBorderEffect(MapRegion region)
        {
            SKPathEffect? pathEffect = null;
            switch (region.RegionBorderType)
            {
                case PathTypeEnum.DottedLinePath:
                    float[] intervals = [0, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.DashedLinePath:
                    intervals = [region.RegionBorderWidth, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.DashDotLinePath:
                    intervals = [region.RegionBorderWidth, region.RegionBorderWidth * 2, 0, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.DashDotDotLinePath:
                    intervals = [region.RegionBorderWidth, region.RegionBorderWidth * 2, 0, region.RegionBorderWidth * 2, 0, region.RegionBorderWidth * 2];
                    pathEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.LineAndDashesPath:
                    float ldWidth = Math.Max(1, region.RegionBorderWidth / 2.0F);

                    string svgPath = "M 0 0"
                        + " h" + (region.RegionBorderWidth).ToString()
                        + " v" + Math.Max(1, ldWidth / 2.0F).ToString()
                        + " h" + (-region.RegionBorderWidth).ToString()
                        + " M0" + "," + (region.RegionBorderWidth - 1.0F).ToString()
                        + " h" + (ldWidth).ToString()
                        + " v2"
                        + " h" + (-ldWidth).ToString();

                    pathEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(svgPath),
                        region.RegionBorderWidth, 0, SKPath1DPathEffectStyle.Morph);
                    break;
            }

            return pathEffect;
        }

        public static void ConstructRegionPaintObjects(MapRegion region, SKPathEffect? regionBorderEffect)
        {
            region.RegionBorderPaint = new SKPaint()
            {
                StrokeWidth = region.RegionBorderWidth,
                Color = region.RegionBorderColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateCorner(region.RegionBorderSmoothing)
            };

            Color innerColor = Color.FromArgb(region.RegionInnerOpacity, region.RegionBorderColor.R, region.RegionBorderColor.G, region.RegionBorderColor.B);

            region.RegionInnerPaint = new SKPaint()
            {
                Color = Extensions.ToSKColor(innerColor),
                Style = SKPaintStyle.Fill,
                PathEffect = SKPathEffect.CreateCorner(region.RegionBorderSmoothing)
            };

            if (regionBorderEffect != null)
            {
                region.RegionBorderPaint.PathEffect = SKPathEffect.CreateCompose(regionBorderEffect, SKPathEffect.CreateCorner(region.RegionBorderSmoothing));
            }
        }

        internal static void MoveSelectedRegionInRenderOrder(RealmStudioMap map, MapRegion? currentRegion, ComponentMoveDirectionEnum direction)
        {
            if (currentRegion != null)
            {
                // find the selected region in the Region Layer MapComponents
                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER);

                List<MapComponent> regionComponents = regionLayer.MapLayerComponents;
                MapRegion? selectedRegion = null;

                int selectedRegionIndex = 0;

                for (int i = 0; i < regionComponents.Count; i++)
                {
                    MapComponent regionComponent = regionComponents[i];
                    if (regionComponent is MapRegion region && region.RegionGuid.ToString() == currentRegion.RegionGuid.ToString())
                    {
                        selectedRegionIndex = i;
                        selectedRegion = region;
                        break;
                    }
                }

                if (direction == ComponentMoveDirectionEnum.Up)
                {
                    // moving a region up in render order means increasing its index
                    if (selectedRegion != null && selectedRegionIndex < regionComponents.Count - 1)
                    {
                        regionComponents[selectedRegionIndex] = regionComponents[selectedRegionIndex + 1];
                        regionComponents[selectedRegionIndex + 1] = selectedRegion;
                    }
                }
                else if (direction == ComponentMoveDirectionEnum.Down)
                {
                    // moving a region down in render order means decreasing its index
                    if (selectedRegion != null && selectedRegionIndex > 0)
                    {
                        regionComponents[selectedRegionIndex] = regionComponents[selectedRegionIndex - 1];
                        regionComponents[selectedRegionIndex - 1] = selectedRegion;
                    }
                }
            }
        }
    }
}
