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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class MapRegion : MapComponent, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; }

        public string RegionName { get; set; } = string.Empty;
        public Guid RegionGuid { get; set; } = Guid.NewGuid();
        public string RegionDescription { get; set; } = string.Empty;
        public List<MapRegionPoint> MapRegionPoints { get; set; } = [];
        public Color RegionBorderColor { get; set; } = ColorTranslator.FromHtml("#0056B3");
        public int RegionBorderWidth { get; set; } = 10;
        public int RegionInnerOpacity { get; set; } = 64;
        public int RegionBorderSmoothing { get; set; } = 20;
        public PathType RegionBorderType { get; set; } = PathType.SolidLinePath;
        public bool IsSelected { get; set; }

        public SKPaint RegionBorderPaint { get; set; } = new();

        public SKPaint RegionInnerPaint { get; set; } = new();

        public SKPath? BoundaryPath { get; set; }

        public MapRegion() { }

        public MapRegion(MapRegion original)
        {
            ParentMap = original.ParentMap;
            RegionBorderColor = original.RegionBorderColor;
            RegionBorderSmoothing = original.RegionBorderSmoothing;
            RegionBorderType = original.RegionBorderType;
            RegionBorderWidth = original.RegionBorderWidth;
            RegionInnerOpacity = original.RegionInnerOpacity;
            RegionName = original.RegionName;
        }

        public override void Render(SKCanvas? canvas)
        {
            if (ParentMap == null || canvas == null) return;

            SKPath path = new();

            path.MoveTo(MapRegionPoints.First().RegionPoint);

            for (int i = 1; i < MapRegionPoints.Count; i++)
            {
                path.LineTo(MapRegionPoints[i].RegionPoint);
            }

            path.LineTo(MapRegionPoints.First().RegionPoint);
            path.Close();

            BoundaryPath = new(path);

            // handle drawing of borders with gradients and other forms
            // that are not handled by a PathEffect in the RegionBorderPaint

            switch (RegionBorderType)
            {
                case PathType.BorderedGradientPath:
                    {
                        PaintBorderGradientBorder(BoundaryPath, canvas);
                    }
                    break;
                case PathType.BorderedLightSolidPath:
                    {
                        PaintLightSolidBorder(BoundaryPath, canvas);
                    }
                    break;
                case PathType.DoubleSolidBorderPath:
                    {
                        PaintDoubleSolidBorder(BoundaryPath, canvas);
                    }
                    break;
                default:
                    canvas.DrawPath(path, RegionInnerPaint);
                    canvas.DrawPath(path, RegionBorderPaint);
                    break;
            }


            if (IsSelected)
            {
                if (BoundaryPath != null)
                {
                    // draw an outline around the region to show that it is selected
                    BoundaryPath.GetTightBounds(out SKRect boundRect);
                    using SKPath boundsPath = new();
                    boundsPath.AddRect(boundRect);

                    canvas.DrawPath(boundsPath, PaintObjects.RegionSelectPaint);

                    // draw dots on region vertices

                    float minDistance = 20;
                    float totalDistance = 0;

                    for (int i = 0; i < MapRegionPoints.Count; i++)
                    {
                        MapRegionPoint p = MapRegionPoints[i];

                        bool renderPoint = false;

                        if (i < MapRegionPoints.Count - 1)
                        {
                            float distance = SKPoint.Distance(p.RegionPoint, MapRegionPoints[i + 1].RegionPoint);
                            totalDistance += distance;

                            if (totalDistance > minDistance)
                            {
                                renderPoint = true;
                                totalDistance = 0;
                            }
                        }
                        else
                        {
                            renderPoint = true;
                        }

                        if (renderPoint)
                        {
                            p.Render(canvas);
                        }
                    }
                }
            }
        }

        private void PaintBorderGradientBorder(SKPath path, SKCanvas canvas)
        {
            using SKPaint gradientPaint = new()
            {
                StrokeWidth = RegionBorderWidth,
                Color = RegionBorderColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateCorner(RegionBorderSmoothing)
            };

            canvas.DrawPath(path, RegionInnerPaint);

            gradientPaint.StrokeWidth = RegionBorderPaint.StrokeWidth * 0.4F;
            gradientPaint.Color = SKColors.Black;
            canvas.DrawPath(path, gradientPaint);

            Color clr = Color.FromArgb(154, RegionBorderPaint.Color.ToDrawingColor());
            gradientPaint.Color = Extensions.ToSKColor(clr);
            List<MapRegionPoint> parallelPoints = DrawingMethods.GetParallelRegionPoints(MapRegionPoints, RegionBorderPaint.StrokeWidth * 0.4F, ParallelDirection.Below);
            using SKPath p1 = RegionManager.GetLinePathFromRegionPoints(parallelPoints);
            canvas.DrawPath(p1, gradientPaint);

            clr = Color.FromArgb(102, RegionBorderPaint.Color.ToDrawingColor());
            gradientPaint.Color = Extensions.ToSKColor(clr);
            parallelPoints = DrawingMethods.GetParallelRegionPoints(MapRegionPoints, RegionBorderPaint.StrokeWidth * 0.6F, ParallelDirection.Below);
            using SKPath p2 = RegionManager.GetLinePathFromRegionPoints(parallelPoints);
            canvas.DrawPath(p2, gradientPaint);

            clr = Color.FromArgb(51, RegionBorderPaint.Color.ToDrawingColor());
            gradientPaint.Color = Extensions.ToSKColor(clr);
            parallelPoints = DrawingMethods.GetParallelRegionPoints(MapRegionPoints, RegionBorderPaint.StrokeWidth * 0.8F, ParallelDirection.Below);
            using SKPath p3 = RegionManager.GetLinePathFromRegionPoints(parallelPoints);
            canvas.DrawPath(p3, gradientPaint);

            clr = Color.FromArgb(25, RegionBorderPaint.Color.ToDrawingColor());
            gradientPaint.Color = Extensions.ToSKColor(clr);

            parallelPoints = DrawingMethods.GetParallelRegionPoints(MapRegionPoints, RegionBorderPaint.StrokeWidth, ParallelDirection.Below);
            using SKPath p4 = RegionManager.GetLinePathFromRegionPoints(parallelPoints);
            canvas.DrawPath(p4, gradientPaint);
        }

        private void PaintLightSolidBorder(SKPath path, SKCanvas canvas)
        {
            canvas.DrawPath(path, RegionInnerPaint);

            using SKPaint borderPaint = new()
            {
                Color = SKColors.Black,
                StrokeWidth = RegionBorderWidth * 0.2F,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateCorner(RegionBorderSmoothing)
            };

            canvas.DrawPath(path, borderPaint);

            using SKPaint linePaint1 = new()
            {
                StrokeWidth = RegionBorderWidth * 0.8F,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateCorner(RegionBorderSmoothing)
            };

            Color clr = Color.FromArgb(102, RegionBorderPaint.Color.ToDrawingColor());
            linePaint1.Color = Extensions.ToSKColor(clr);

            List<MapRegionPoint> parallelPoints = DrawingMethods.GetParallelRegionPoints(MapRegionPoints, RegionBorderPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
            using SKPath p1 = RegionManager.GetLinePathFromRegionPoints(parallelPoints);
            canvas.DrawPath(p1, linePaint1);
        }

        private void PaintDoubleSolidBorder(SKPath path, SKCanvas canvas)
        {
            canvas.DrawPath(path, RegionInnerPaint);

            using SKPaint borderPaint = new()
            {
                Color = SKColors.Black,
                StrokeWidth = RegionBorderWidth * 0.2F,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateCorner(RegionBorderSmoothing)
            };

            canvas.DrawPath(path, borderPaint);

            List<SKPoint> points = [.. path.Points];
            List<SKPoint> parallelPoints = DrawingMethods.GetParallelPoints(points, RegionBorderPaint.StrokeWidth, ParallelDirection.Below);
            using SKPath p1 = DrawingMethods.GetLinePathFromPoints(parallelPoints);
            canvas.DrawPath(p1, borderPaint);
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.

            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();
            XDocument mapRegionDoc = XDocument.Parse(content);

            IEnumerable<XElement?> nameElemEnum = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionName"));
            if (nameElemEnum != null && nameElemEnum.Any() && nameElemEnum.First() != null)
            {
                string? regionName = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionName").Value).FirstOrDefault();
                RegionName = regionName;
            }
            else
            {
                RegionName = string.Empty;
            }

            IEnumerable<XElement?> guidElemEnum = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionGuid"));
            if (guidElemEnum.First() != null)
            {
                string? regionGuid = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionGuid").Value).FirstOrDefault();
                RegionGuid = Guid.Parse(regionGuid);
            }

            IEnumerable<XElement?> descrElemEnum = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionDescription"));
            if (descrElemEnum != null && descrElemEnum.Any() && descrElemEnum.First() != null)
            {
                string? description = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionDescription").Value).FirstOrDefault();
                RegionDescription = description;
            }
            else
            {
                RegionDescription = string.Empty;
            }

            IEnumerable<XElement> regionPointElem = mapRegionDoc.Descendants(ns + "MapRegionPoint");
            if (regionPointElem.First() != null)
            {
                XmlReaderSettings settings = new()
                {
                    IgnoreWhitespace = true
                };

                foreach (XElement elem in regionPointElem)
                {
                    string regionPointString = elem.ToString();

                    using XmlReader pointReader = XmlReader.Create(new StringReader(regionPointString), settings);
                    pointReader.Read();

                    MapRegionPoint mrp = new();
                    mrp.ReadXml(pointReader);

                    MapRegionPoints.Add(mrp);
                }
            }

            IEnumerable<XElement?> regionBorderColorElem = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderColor"));
            if (regionBorderColorElem.First() != null)
            {
                string? regionBorderColor = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderColor").Value).FirstOrDefault();

                int argbValue = 0;

                if (!string.IsNullOrEmpty(regionBorderColor))
                {
                    if (regionBorderColor.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(regionBorderColor).ToArgb();
                    }
                    else if (int.TryParse(regionBorderColor, out int n))
                    {
                        argbValue = n;
                    }

                    RegionBorderColor = Color.FromArgb(argbValue);
                }
                else
                {
                    RegionBorderColor = Color.Blue;
                }
            }

            IEnumerable<XElement?> regionBorderWidthElem = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderWidth"));
            if (regionBorderWidthElem.First() != null)
            {
                string? regionBorderWidth = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderWidth").Value).FirstOrDefault();
                RegionBorderWidth = int.Parse(regionBorderWidth);
            }

            IEnumerable<XElement?> regionInnerOpacityElem = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionInnerOpacity"));
            if (regionInnerOpacityElem.First() != null)
            {
                string? regionInnerOpacity = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionInnerOpacity").Value).FirstOrDefault();
                RegionInnerOpacity = int.Parse(regionInnerOpacity);
            }

            IEnumerable<XElement?> regionBorderSmoothingElem = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderSmoothing"));
            if (regionBorderSmoothingElem.First() != null)
            {
                string? regionBorderSmoothing = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderSmoothing").Value).FirstOrDefault();
                RegionBorderSmoothing = int.Parse(regionBorderSmoothing);
            }

            IEnumerable<XElement?> regionBorderTypeElem = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderType"));
            if (regionBorderTypeElem.First() != null)
            {
                string? regionBorderType = mapRegionDoc.Descendants().Select(x => x.Element(ns + "RegionBorderType").Value).FirstOrDefault();
                RegionBorderType = Enum.Parse<PathType>(regionBorderType);
            }

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("RegionName");
            writer.WriteString(RegionName);
            writer.WriteEndElement();

            writer.WriteStartElement("RegionGuid");
            writer.WriteString(RegionGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RegionDescription");
            writer.WriteString(RegionDescription);
            writer.WriteEndElement();

            writer.WriteStartElement("MapRegionPoints");
            foreach (MapRegionPoint point in MapRegionPoints)
            {
                writer.WriteStartElement("MapRegionPoint");
                point.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("RegionBorderColor");
            writer.WriteValue(RegionBorderColor.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("RegionBorderWidth");
            writer.WriteValue(RegionBorderWidth);
            writer.WriteEndElement();

            writer.WriteStartElement("RegionInnerOpacity");
            writer.WriteValue(RegionInnerOpacity);
            writer.WriteEndElement();

            writer.WriteStartElement("RegionBorderSmoothing");
            writer.WriteValue(RegionBorderSmoothing);
            writer.WriteEndElement();

            writer.WriteStartElement("RegionBorderType");
            writer.WriteValue(RegionBorderType.ToString());
            writer.WriteEndElement();
        }
    }
}
