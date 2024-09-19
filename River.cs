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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    internal class River : MapComponent, IWaterFeature, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; } = null;

        public Guid MapRiverGuid { get; set; } = Guid.NewGuid();

        public string MapRiverName { get; set; } = "";

        public List<MapRiverPoint> RiverPoints { get; set; } = [];

        public Color RiverColor { get; set; } = ColorTranslator.FromHtml("#839690");

        public float RiverWidth { get; set; } = 4;

        public bool RiverSourceFadeIn { get; set; } = false;

        public Color RiverShorelineColor { get; set; } = ColorTranslator.FromHtml("#A19076");

        public bool ShowRiverPoints { get; set; } = false;

        public bool IsSelected { get; set; } = false;

        public SKPaint? RiverPaint { get; set; } = null;

        public SKPaint? RiverFillPaint { get; set; } = null;

        public SKPaint? RiverShorelinePaint { get; set; } = null;

        public SKPaint? RiverShallowWaterPaint { get; set; } = null;

        public SKPath? RiverBoundaryPath { get; set; } = null;

        public List<SKPath> RiverColorPaths = [];

        public override void Render(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            using SKRegion waterPathRegion = new();

            List<MapRiverPoint> distinctRiverPoints = RiverPoints.Distinct(new RiverPointComparer()).ToList();

            // clip the river drawing to the outer path of landforms
            List<MapComponent> landformList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformList.Count; i++)
            {
                SKPath landformOutlinePath = ((Landform)landformList[i]).ContourPath;

                if (landformOutlinePath != null && landformOutlinePath.PointCount > 0 && distinctRiverPoints.Count > 0 && RiverPaint != null)
                {
                    waterPathRegion.SetPath(landformOutlinePath);

                    canvas.Save();
                    canvas.ClipRegion(waterPathRegion);

                    // use multiple paths and multiple sets of parallel points and paint objects
                    // to draw lines as gradients to shade rivers

                    // river
                    using SKPath riverPath = new();

                    List<MapRiverPoint> parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth / 2.0F, ParallelEnum.Above, RiverSourceFadeIn);

                    if (parallelPoints.Count > 2)
                    {
                        riverPath.MoveTo(parallelPoints[0].RiverPoint);

                        for (int j = 0; j < parallelPoints.Count; j += 3)
                        {
                            if (j < parallelPoints.Count - 2)
                            {
                                riverPath.CubicTo(parallelPoints[j].RiverPoint, parallelPoints[j + 1].RiverPoint, parallelPoints[j + 2].RiverPoint);
                            }
                        }
                    }


                    List<MapRiverPoint> parallelPoints2 = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth / 2.0F, ParallelEnum.Below, RiverSourceFadeIn);

                    if (parallelPoints2.Count > 2)
                    {
                        riverPath.MoveTo(parallelPoints2[0].RiverPoint);

                        for (int j = 0; j < parallelPoints2.Count; j += 3)
                        {
                            if (j < parallelPoints2.Count - 2)
                            {
                                riverPath.CubicTo(parallelPoints2[j].RiverPoint, parallelPoints2[j + 1].RiverPoint, parallelPoints2[j + 2].RiverPoint);
                            }
                        }
                    }

                    canvas.DrawPath(riverPath, RiverPaint);

                    // fill path
                    using SKPath riverFillPath = new();

                    if (parallelPoints.Count > 2)
                    {
                        riverFillPath.MoveTo(parallelPoints[0].RiverPoint);

                        for (int j = 0; j < parallelPoints.Count; j += 3)
                        {
                            if (j < parallelPoints.Count - 2)
                            {
                                riverFillPath.CubicTo(parallelPoints[j].RiverPoint, parallelPoints[j + 1].RiverPoint, parallelPoints[j + 2].RiverPoint);
                            }
                        }
                    }

                    if (parallelPoints2.Count > 2)
                    {
                        riverFillPath.MoveTo(parallelPoints.Last().RiverPoint);
                        riverFillPath.LineTo(parallelPoints2.Last().RiverPoint);

                        for (int j = parallelPoints2.Count - 1; j >= 2; j -= 3)
                        {
                            if (j < parallelPoints2.Count - 2)
                            {
                                riverFillPath.CubicTo(parallelPoints2[j].RiverPoint, parallelPoints2[j - 1].RiverPoint, parallelPoints2[j - 2].RiverPoint);
                            }
                        }
                    }

                    if (parallelPoints.Count > 2 && parallelPoints2.Count > 2)
                    {
                        riverFillPath.MoveTo(parallelPoints2[0].RiverPoint);
                        riverFillPath.LineTo(parallelPoints[0].RiverPoint);

                        canvas.DrawPath(riverFillPath, RiverFillPaint);
                    }

                    // shoreline
                    using SKPath shorelinePath = new();

                    parallelPoints.Clear();
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth * 1.1F, ParallelEnum.Below, RiverSourceFadeIn);

                    if (parallelPoints.Count > 2)
                    {
                        shorelinePath.MoveTo(parallelPoints[0].RiverPoint);

                        for (int j = 0; j < parallelPoints.Count; j += 3)
                        {
                            if (j < parallelPoints.Count - 2)
                            {
                                shorelinePath.CubicTo(parallelPoints[j].RiverPoint, parallelPoints[j + 1].RiverPoint, parallelPoints[j + 2].RiverPoint);
                            }
                        }
                    }

                    parallelPoints.Clear();
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth * 1.1F, ParallelEnum.Above, RiverSourceFadeIn);

                    if (parallelPoints.Count > 2)
                    {
                        shorelinePath.MoveTo(parallelPoints[0].RiverPoint);

                        for (int j = 0; j < parallelPoints.Count; j += 3)
                        {
                            if (j < parallelPoints.Count - 2)
                            {
                                shorelinePath.CubicTo(parallelPoints[j].RiverPoint, parallelPoints[j + 1].RiverPoint, parallelPoints[j + 2].RiverPoint);
                            }
                        }
                    }

                    canvas.DrawPath(shorelinePath, RiverShorelinePaint);
                    RiverBoundaryPath?.Dispose();
                    RiverBoundaryPath = new(shorelinePath)
                    {
                        FillType = SKPathFillType.Winding,
                    };

                    parallelPoints.Clear();
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth * 0.9F, ParallelEnum.Above, RiverSourceFadeIn);

                    // shallow water
                    using SKPath shallowWaterPath = new();

                    if (parallelPoints.Count > 2)
                    {
                        shallowWaterPath.MoveTo(parallelPoints[0].RiverPoint);

                        for (int j = 0; j < parallelPoints.Count; j += 3)
                        {
                            if (j < parallelPoints.Count - 2)
                            {
                                shallowWaterPath.CubicTo(parallelPoints[j].RiverPoint, parallelPoints[j + 1].RiverPoint, parallelPoints[j + 2].RiverPoint);
                            }
                        }
                    }

                    parallelPoints.Clear();
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth * 0.9F, ParallelEnum.Below, RiverSourceFadeIn);

                    if (parallelPoints.Count > 2)
                    {
                        shallowWaterPath.MoveTo(parallelPoints[0].RiverPoint);

                        for (int j = 0; j < parallelPoints.Count; j += 3)
                        {
                            if (j < parallelPoints.Count - 2)
                            {
                                shallowWaterPath.CubicTo(parallelPoints[j].RiverPoint, parallelPoints[j + 1].RiverPoint, parallelPoints[j + 2].RiverPoint);
                            }
                        }
                    }

                    canvas.DrawPath(shallowWaterPath, RiverShallowWaterPaint);

                    canvas.Restore();
                }
            }

        }

        public List<MapRiverPoint> GetRiverControlPoints()
        {
            List<MapRiverPoint> mapRiverPoints = [];

            for (int i = 0; i < RiverPoints.Count - 10; i += 10)
            {
                mapRiverPoints.Add(RiverPoints[i]);
            }

            mapRiverPoints.Add(RiverPoints.Last());

            return mapRiverPoints;
        }

        public XmlSchema? GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
