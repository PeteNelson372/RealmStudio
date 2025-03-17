﻿/**************************************************************************************************************************
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class MapPath : MapComponent, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; }
        public Guid MapPathGuid { get; set; } = Guid.NewGuid();
        public string MapPathName { get; set; } = "";
        public List<MapPathPoint> PathPoints { get; set; } = [];
        public PathType PathType { get; set; } = PathType.SolidLinePath;
        public Color PathColor { get; set; } = ColorTranslator.FromHtml("#4B311A");
        public float PathWidth { get; set; } = 4;
        public MapTexture? PathTexture { get; set; }
        public bool DrawOverSymbols { get; set; }
        public bool ShowPathPoints { get; set; }
        public bool IsSelected { get; set; }
        public SKPaint? PathPaint { get; set; }
        public SKPath BoundaryPath { get; set; } = new();

        public MapPath() { }

        public MapPath(MapPath original)
        {
            X = original.X;
            Y = original.Y;
            DrawOverSymbols = original.DrawOverSymbols;
            Height = original.Height;
            MapPathName = original.MapPathName;
            ParentMap = original.ParentMap;
            PathColor = original.PathColor;

            if (original.PathTexture != null)
            {
                PathTexture = new MapTexture(original.PathTexture.TextureName, original.PathTexture.TexturePath);
            }

            PathType = original.PathType;
            PathWidth = original.PathWidth;
            ShowPathPoints = original.ShowPathPoints;
        }


        public override void Render(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            // clip the path drawing to the outer path of landforms
            List<MapComponent> landformList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            List<MapPathPoint> distinctPathPoints = [.. PathPoints.Distinct(new MapPathPointComparer())];
            using SKPath clipPath = new();

            for (int i = 0; i < landformList.Count; i++)
            {
                if (landformList[i] is Landform landform)
                {
                    if (landform.ContourPath != null && landform.ContourPath.PointCount > 0)
                    {
                        clipPath.AddPath(landform.ContourPath);
                    }
                }
            }
            if (distinctPathPoints.Count > 0 && PathPaint != null)
            {
                using (new SKAutoCanvasRestore(canvas))
                {
                    canvas.ClipPath(clipPath);

                    switch (PathType)
                    {
                        case PathType.SolidBlackBorderPath:
                            {
                                using SKPaint blackBorderPaint = PathPaint.Clone();
                                blackBorderPaint.StrokeWidth = PathPaint.StrokeWidth * 1.2F;
                                blackBorderPaint.Color = SKColors.Black;

                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, blackBorderPaint);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, PathPaint);
                            }
                            break;
                        case PathType.BorderedGradientPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                borderPaint.Color = SKColors.Black;

                                List<MapPathPoint> parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);

                                using SKPaint linePaint1 = PathPaint.Clone();
                                linePaint1.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                Color clr = Color.FromArgb(154, PathColor);
                                linePaint1.Color = Extensions.ToSKColor(clr);

                                parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.4F, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint1);

                                using SKPaint linePaint2 = PathPaint.Clone();
                                linePaint2.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                clr = Color.FromArgb(102, PathColor);
                                linePaint2.Color = Extensions.ToSKColor(clr);

                                parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.6F, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint2);

                                using SKPaint linePaint3 = PathPaint.Clone();
                                linePaint3.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                clr = Color.FromArgb(51, PathColor);
                                linePaint3.Color = Extensions.ToSKColor(clr);

                                parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.8F, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint3);

                                using SKPaint linePaint4 = PathPaint.Clone();
                                linePaint4.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                clr = Color.FromArgb(25, PathColor);
                                linePaint4.Color = Extensions.ToSKColor(clr);

                                parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint4);
                            }
                            break;
                        case PathType.BorderedLightSolidPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                borderPaint.Color = SKColors.Black;

                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);

                                using SKPaint linePaint1 = PathPaint.Clone();
                                linePaint1.StrokeWidth = PathPaint.StrokeWidth * 0.8F;
                                Color clr = Color.FromArgb(102, PathColor);
                                linePaint1.Color = Extensions.ToSKColor(clr);

                                List<MapPathPoint> parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint1);
                            }
                            break;
                        case PathType.DoubleSolidBorderPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);

                                List<MapPathPoint> parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth, ParallelDirection.Above);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                            }
                            break;
                        case PathType.LineAndDashesPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);

                                float[] intervals = [PathWidth, PathWidth];

                                SKPathEffect? pathLineEffect = null;
                                pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                                borderPaint.PathEffect = pathLineEffect;

                                List<MapPathPoint> parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth, ParallelDirection.Above);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                            }
                            break;
                        case PathType.ShortIrregularDashPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();

                                float[] intervals = [PathWidth / 4.0F, PathWidth, PathWidth / 4.0F, PathWidth];

                                SKPathEffect pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                                borderPaint.PathEffect = pathLineEffect;
                                borderPaint.StrokeWidth = PathWidth;
                                borderPaint.StrokeCap = SKStrokeCap.Butt;

                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);
                            }
                            break;
                        case PathType.BorderAndTexturePath:
                            {
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, PathPaint);

                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.Shader = null;

                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                List<MapPathPoint> parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);

                                parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Below);
                                MapPathMethods.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                            }

                            break;
                        case PathType.RailroadTracksPath:
                            {
                                using SKPaint trackPaint = PathPaint.Clone();
                                trackPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                MapPathMethods.DrawLineFromPoints(canvas, distinctPathPoints, trackPaint);

                                List<MapPathPoint> parallelPoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.8F, ParallelDirection.Above);
                                MapPathMethods.DrawLineFromPoints(canvas, parallelPoints, trackPaint);

                                float[] intervals = [PathWidth / 4.0F, PathWidth, PathWidth / 4.0F, PathWidth];

                                SKPathEffect pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                                trackPaint.PathEffect = pathLineEffect;
                                trackPaint.StrokeWidth = PathWidth * 1.5F;
                                trackPaint.StrokeCap = SKStrokeCap.Butt;

                                List<MapPathPoint> crossTiePoints = MapPathMethods.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.4F, ParallelDirection.Above);
                                MapPathMethods.DrawLineFromPoints(canvas, crossTiePoints, trackPaint);
                            }
                            break;
                        default:
                            MapPathMethods.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, PathPaint);
                            break;
                    }
                }

            }
        }

        public List<MapPathPoint> GetMapPathControlPoints()
        {
            List<MapPathPoint> mapPathPoints = [];

            for (int i = 0; i < PathPoints.Count - 10; i += 10)
            {
                mapPathPoints.Add(PathPoints[i]);
                PathPoints[i].IsControlPoint = true;
            }

            mapPathPoints.Add(PathPoints[^1]);
            PathPoints[^1].IsControlPoint = true;

            return mapPathPoints;
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
            XDocument mapPathDoc = XDocument.Parse(content);

            IEnumerable<XElement?> nameElemEnum = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathName"));
            if (nameElemEnum.First() != null)
            {
                string? mapPathName = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathName").Value).FirstOrDefault();
                MapPathName = mapPathName;
            }

            IEnumerable<XElement?> guidElemEnum = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathGuid"));
            if (guidElemEnum.First() != null)
            {
                string? mapGuid = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathGuid").Value).FirstOrDefault();
                MapPathGuid = Guid.Parse(mapGuid);
            }

            IEnumerable<XElement?> typeElemEnum = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathType"));
            if (typeElemEnum.First() != null)
            {
                string? pathType = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathType").Value).FirstOrDefault();
                PathType = Enum.Parse<PathType>(pathType);
            }

            IEnumerable<XElement> pathColorElem = mapPathDoc.Descendants(ns + "PathColor");
            if (pathColorElem != null && pathColorElem.Any() && pathColorElem.First() != null)
            {
                string? pathColor = mapPathDoc.Descendants().Select(x => x.Element(ns + "PathColor").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(pathColor))
                {
                    int argbValue = 0;
                    if (pathColor.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(pathColor).ToArgb();
                    }
                    else if (int.TryParse(pathColor, out int n))
                    {
                        if (n > 0)
                        {
                            argbValue = n;
                        }
                        else
                        {
                            argbValue = ColorTranslator.FromHtml("#4B311A").ToArgb();
                        }
                    }

                    PathColor = Color.FromArgb(argbValue);
                }
                else
                {
                    PathColor = ColorTranslator.FromHtml("#4B311A");
                }
            }
            else
            {
                PathColor = ColorTranslator.FromHtml("#4B311A");
            }

            IEnumerable<XElement?> pathWidthElem = mapPathDoc.Descendants().Select(x => x.Element(ns + "PathWidth"));
            if (pathWidthElem.First() != null)
            {
                string? pathWidth = mapPathDoc.Descendants().Select(x => x.Element(ns + "PathWidth").Value).FirstOrDefault();
                PathWidth = int.Parse(pathWidth);
            }

            IEnumerable<XElement?> drawOverSymbolsElem = mapPathDoc.Descendants().Select(x => x.Element(ns + "DrawOverSymbols"));
            if (drawOverSymbolsElem.First() != null)
            {
                string? drawOverSymbols = mapPathDoc.Descendants().Select(x => x.Element(ns + "DrawOverSymbols").Value).FirstOrDefault();
                DrawOverSymbols = bool.Parse(drawOverSymbols);
            }

            IEnumerable<XElement> pathPointElem = mapPathDoc.Descendants(ns + "PathPoint");
            if (pathPointElem.First() != null)
            {
                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true
                };

                foreach (XElement elem in pathPointElem)
                {
                    string pathPointString = elem.ToString();

                    using XmlReader pointReader = XmlReader.Create(new StringReader(pathPointString), settings);
                    pointReader.Read();
                    MapPathPoint mpp = new();
                    mpp.ReadXml(pointReader);

                    PathPoints.Add(mpp);
                }
            }
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);

            // map path name
            writer.WriteStartElement("MapPathName");
            writer.WriteString(MapPathName);
            writer.WriteEndElement();

            // map path GUID
            writer.WriteStartElement("MapPathGuid");
            writer.WriteString(MapPathGuid.ToString());
            writer.WriteEndElement();

            // map path points
            writer.WriteStartElement("MapPathPoints");
            foreach (MapPathPoint point in PathPoints)
            {
                writer.WriteStartElement("PathPoint");
                point.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("MapPathType");
            writer.WriteString(PathType.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("PathColor");
            writer.WriteValue(PathColor.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("PathWidth");
            writer.WriteValue(PathWidth);
            writer.WriteEndElement();

            writer.WriteStartElement("DrawOverSymbols");
            writer.WriteValue(DrawOverSymbols);
            writer.WriteEndElement();
        }
    }
}
