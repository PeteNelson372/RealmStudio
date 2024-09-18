using SkiaSharp;
using SkiaSharp.Components;
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

        public SKPaint? RiverPaint { get; set; }

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

                    // shoreline
                    using SKPath shorelinePath = new();

                    List<MapRiverPoint> parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth * 1.1F, ParallelEnum.Below, RiverSourceFadeIn);

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
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth, ParallelEnum.Above, RiverSourceFadeIn);

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
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth, ParallelEnum.Below, RiverSourceFadeIn);

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

                    // river
                    using SKPath riverPath = new();

                    parallelPoints.Clear();
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth / 2.0F, ParallelEnum.Above, RiverSourceFadeIn);

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

                    parallelPoints.Clear();
                    parallelPoints = WaterFeatureMethods.GetParallelRiverPoints(distinctRiverPoints, RiverPaint.StrokeWidth / 2.0F, ParallelEnum.Below, RiverSourceFadeIn);

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

                    canvas.DrawPath(riverPath, RiverPaint);

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
