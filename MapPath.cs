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
        public string MapPathDescription { get; set; } = string.Empty;
        public List<MapPathPoint> PathPoints { get; set; } = [];
        public PathType PathType { get; set; } = PathType.SolidLinePath;
        public Color PathColor { get; set; } = ColorTranslator.FromHtml("#4B311A");
        public float PathWidth { get; set; } = 4;
        public float PathTowerDistance { get; set; } = 10.0F;
        public float PathTowerSize { get; set; } = 1.2F;
        public MapTexture? PathTexture { get; set; }
        public int PathTextureOpacity { get; set; } = 255;
        public float PathTextureScale { get; set; } = 1.0F;
        public bool DrawOverSymbols { get; set; }
        public bool ShowPathPoints { get; set; }
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
            PathTextureOpacity = original.PathTextureOpacity;
            PathTextureScale = original.PathTextureScale;
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

                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, blackBorderPaint);
                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, PathPaint);
                            }
                            break;
                        case PathType.BorderedGradientPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                borderPaint.Color = SKColors.Black;

                                List<MapPathPoint> parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);

                                using SKPaint linePaint1 = PathPaint.Clone();
                                linePaint1.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                Color clr = Color.FromArgb(154, PathColor);
                                linePaint1.Color = Extensions.ToSKColor(clr);

                                parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.4F, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint1);

                                using SKPaint linePaint2 = PathPaint.Clone();
                                linePaint2.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                clr = Color.FromArgb(102, PathColor);
                                linePaint2.Color = Extensions.ToSKColor(clr);

                                parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.6F, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint2);

                                using SKPaint linePaint3 = PathPaint.Clone();
                                linePaint3.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                clr = Color.FromArgb(51, PathColor);
                                linePaint3.Color = Extensions.ToSKColor(clr);

                                parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.8F, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint3);

                                using SKPaint linePaint4 = PathPaint.Clone();
                                linePaint4.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                clr = Color.FromArgb(25, PathColor);
                                linePaint4.Color = Extensions.ToSKColor(clr);

                                parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint4);
                            }
                            break;
                        case PathType.BorderedLightSolidPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;
                                borderPaint.Color = SKColors.Black;

                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);

                                using SKPaint linePaint1 = PathPaint.Clone();
                                linePaint1.StrokeWidth = PathPaint.StrokeWidth * 0.8F;
                                Color clr = Color.FromArgb(102, PathColor);
                                linePaint1.Color = Extensions.ToSKColor(clr);

                                List<MapPathPoint> parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint1);
                            }
                            break;
                        case PathType.DoubleSolidBorderPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);

                                List<MapPathPoint> parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth, ParallelDirection.Above);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                            }
                            break;
                        case PathType.LineAndDashesPath:
                            {
                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);

                                float[] intervals = [PathWidth, PathWidth];

                                SKPathEffect? pathLineEffect = null;
                                pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                                borderPaint.PathEffect = pathLineEffect;

                                List<MapPathPoint> parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth, ParallelDirection.Above);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
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

                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, borderPaint);
                            }
                            break;
                        case PathType.BorderAndTexturePath:
                            {
                                PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, PathPaint);

                                using SKPaint borderPaint = PathPaint.Clone();
                                borderPaint.Shader = null;

                                borderPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                List<MapPathPoint> parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);

                                parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Below);
                                PathManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                            }

                            break;
                        case PathType.RailroadTracksPath:
                            {
                                using SKPaint trackPaint = PathPaint.Clone();
                                trackPaint.StrokeWidth = PathPaint.StrokeWidth * 0.2F;

                                PathManager.DrawLineFromPoints(canvas, distinctPathPoints, trackPaint);

                                List<MapPathPoint> parallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.8F, ParallelDirection.Above);
                                PathManager.DrawLineFromPoints(canvas, parallelPoints, trackPaint);

                                float[] intervals = [PathWidth / 4.0F, PathWidth, PathWidth / 4.0F, PathWidth];

                                SKPathEffect pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                                trackPaint.PathEffect = pathLineEffect;
                                trackPaint.StrokeWidth = PathWidth * 1.5F;
                                trackPaint.StrokeCap = SKStrokeCap.Butt;

                                List<MapPathPoint> crossTiePoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.4F, ParallelDirection.Above);
                                PathManager.DrawLineFromPoints(canvas, crossTiePoints, trackPaint);
                            }
                            break;
                        case PathType.RoundTowerWall:
                            {
                                if (distinctPathPoints.Count < 2) return;

                                using SKPaint borderPaint = new()
                                {
                                    Color = Extensions.ToSKColor(PathColor),
                                    StrokeWidth = PathPaint.StrokeWidth * 0.25F,
                                    Style = SKPaintStyle.Stroke,
                                    StrokeCap = SKStrokeCap.Round,
                                    StrokeJoin = SKStrokeJoin.Round,
                                    StrokeMiter = 1.0F,
                                    IsAntialias = true,
                                };

                                List<MapPathPoint> aboveParallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                                List<MapPathPoint> belowParallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Below);

                                canvas.DrawCircle(distinctPathPoints[0].MapPoint, PathWidth * PathTowerSize, PathPaint);
                                canvas.DrawCircle(distinctPathPoints[0].MapPoint, PathWidth * PathTowerSize * 1.01F, borderPaint);

                                PathPaint.Style = SKPaintStyle.Fill;
                                SKPoint towerPoint = distinctPathPoints[0].MapPoint;

                                float distance = 0.0f;

                                for (int i = 1; i < distinctPathPoints.Count - 1; i++)
                                {
                                    distance += SKPoint.Distance(distinctPathPoints[i - 1].MapPoint, distinctPathPoints[i].MapPoint);

                                    canvas.DrawLine(distinctPathPoints[i - 1].MapPoint, distinctPathPoints[i].MapPoint, PathPaint);

                                    if (i > 0 && i < aboveParallelPoints.Count - 1
                                        && i < belowParallelPoints.Count - 1
                                        && aboveParallelPoints.Count > 2
                                        && belowParallelPoints.Count > 2)
                                    {
                                        canvas.DrawLine(aboveParallelPoints[i - 1].MapPoint, aboveParallelPoints[i].MapPoint, borderPaint);
                                        canvas.DrawLine(belowParallelPoints[i - 1].MapPoint, belowParallelPoints[i].MapPoint, borderPaint);
                                    }

                                    if (towerPoint != SKPoint.Empty)
                                    {
                                        canvas.DrawCircle(towerPoint, PathWidth * PathTowerSize, PathPaint);
                                        canvas.DrawCircle(towerPoint, PathWidth * PathTowerSize * 1.01F, borderPaint);
                                    }

                                    if (distance > PathWidth * PathTowerDistance)
                                    {
                                        towerPoint = distinctPathPoints[i].MapPoint;

                                        canvas.DrawCircle(distinctPathPoints[i].MapPoint, PathWidth * PathTowerSize, PathPaint);
                                        canvas.DrawCircle(distinctPathPoints[i].MapPoint, PathWidth * PathTowerSize * 1.01F, borderPaint);

                                        distance = 0.0f;
                                    }
                                }
                            }
                            break;
                        case PathType.SquareTowerWall:
                            {
                                if (distinctPathPoints.Count < 2) return;

                                using SKPaint borderPaint = new()
                                {
                                    Color = Extensions.ToSKColor(PathColor),
                                    StrokeWidth = PathPaint.StrokeWidth * 0.25F,
                                    Style = SKPaintStyle.Stroke,
                                    StrokeCap = SKStrokeCap.Round,
                                    StrokeJoin = SKStrokeJoin.Round,
                                    StrokeMiter = 1.0F,
                                    IsAntialias = true,
                                };

                                List<MapPathPoint> aboveParallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                                List<MapPathPoint> belowParallelPoints = PathManager.GetParallelPathPoints(distinctPathPoints, PathPaint.StrokeWidth * 0.5F, ParallelDirection.Below);

                                float lineAngle = DrawingMethods.CalculateAngleBetweenPoints(distinctPathPoints[0].MapPoint, distinctPathPoints[1].MapPoint, true);

                                SKMatrix translateMatrix = SKMatrix.CreateTranslation(-PathWidth * PathTowerSize, -PathWidth * PathTowerSize);
                                SKMatrix m = SKMatrix.CreateRotationDegrees(lineAngle, distinctPathPoints[0].MapPoint.X, distinctPathPoints[0].MapPoint.Y);

                                m = m.PreConcat(translateMatrix);

                                canvas.SetMatrix(m);

                                canvas.DrawRect(distinctPathPoints[0].MapPoint.X, distinctPathPoints[0].MapPoint.Y, PathWidth * PathTowerSize * 2, PathWidth * PathTowerSize * 2, PathPaint);
                                canvas.DrawRect(distinctPathPoints[0].MapPoint.X, distinctPathPoints[0].MapPoint.Y, PathWidth * PathTowerSize * 2.02F, PathWidth * PathTowerSize * 2.02F, borderPaint);

                                canvas.ResetMatrix();

                                PathPaint.Style = SKPaintStyle.Fill;

                                float distance = 0.0f;
                                SKPoint towerPoint = distinctPathPoints[0].MapPoint;
                                float towerLineAngle = lineAngle;

                                for (int i = 1; i < distinctPathPoints.Count - 1; i++)
                                {
                                    distance += SKPoint.Distance(distinctPathPoints[i - 1].MapPoint, distinctPathPoints[i].MapPoint);

                                    canvas.DrawLine(distinctPathPoints[i - 1].MapPoint, distinctPathPoints[i].MapPoint, PathPaint);

                                    if (i > 0 && i < aboveParallelPoints.Count - 1
                                        && i < belowParallelPoints.Count - 1
                                        && aboveParallelPoints.Count > 2
                                        && belowParallelPoints.Count > 2)
                                    {
                                        canvas.DrawLine(aboveParallelPoints[i - 1].MapPoint, aboveParallelPoints[i].MapPoint, borderPaint);
                                        canvas.DrawLine(belowParallelPoints[i - 1].MapPoint, belowParallelPoints[i].MapPoint, borderPaint);
                                    }

                                    if (towerPoint != SKPoint.Empty)
                                    {
                                        m = SKMatrix.CreateRotationDegrees(lineAngle, towerPoint.X, towerPoint.Y);
                                        m = m.PreConcat(translateMatrix);

                                        canvas.SetMatrix(m);

                                        canvas.DrawRect(towerPoint.X, towerPoint.Y, PathWidth * PathTowerSize * 2, PathWidth * PathTowerSize * 2, PathPaint);
                                        canvas.DrawRect(towerPoint.X, towerPoint.Y, PathWidth * PathTowerSize * 2.02F, PathWidth * PathTowerSize * 2.02F, borderPaint);

                                        canvas.ResetMatrix();
                                    }

                                    if (distance > PathWidth * PathTowerDistance)
                                    {
                                        lineAngle = DrawingMethods.CalculateAngleBetweenPoints(distinctPathPoints[i - 1].MapPoint, distinctPathPoints[i].MapPoint, true);
                                        towerLineAngle = lineAngle;
                                        towerPoint = distinctPathPoints[i].MapPoint;

                                        m = SKMatrix.CreateRotationDegrees(lineAngle, distinctPathPoints[i].MapPoint.X, distinctPathPoints[i].MapPoint.Y);
                                        m = m.PreConcat(translateMatrix);

                                        canvas.SetMatrix(m);

                                        canvas.DrawRect(distinctPathPoints[i].MapPoint.X, distinctPathPoints[i].MapPoint.Y, PathWidth * PathTowerSize * 2, PathWidth * PathTowerSize * 2, PathPaint);
                                        canvas.DrawRect(distinctPathPoints[i].MapPoint.X, distinctPathPoints[i].MapPoint.Y, PathWidth * PathTowerSize * 2.02F, PathWidth * PathTowerSize * 2.02F, borderPaint);

                                        canvas.ResetMatrix();

                                        distance = 0.0f;
                                    }
                                }
                            }
                            break;
                        default:
                            PathManager.DrawBezierCurvesFromPoints(canvas, distinctPathPoints, PathPaint);
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
            if (nameElemEnum.First() != null && nameElemEnum.Any() && nameElemEnum.First() != null)
            {
                string? mapPathName = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathName").Value).FirstOrDefault();
                MapPathName = mapPathName;
            }
            else
            {
                MapPathName = string.Empty;
            }

            IEnumerable<XElement?> guidElemEnum = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathGuid"));
            if (guidElemEnum.First() != null)
            {
                string? mapGuid = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathGuid").Value).FirstOrDefault();
                MapPathGuid = Guid.Parse(mapGuid);
            }

            IEnumerable<XElement?> descrElemEnum = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathDescription"));
            if (descrElemEnum != null && descrElemEnum.Any() && descrElemEnum.First() != null)
            {
                string? description = mapPathDoc.Descendants().Select(x => x.Element(ns + "MapPathDescription").Value).FirstOrDefault();
                MapPathDescription = description;
            }
            else
            {
                MapPathDescription = string.Empty;
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

            IEnumerable<XElement?> pathTextureElem = mapPathDoc.Descendants().Select(x => x.Element(ns + "PathTexture"));
            if (pathTextureElem != null && pathTextureElem.Any() && pathTextureElem.First() != null)
            {
                string? pathTextureName = null;
                string? pathTexturePath = null;

                List<XNode> textureNodes = [.. pathTextureElem.Nodes()];
                if (textureNodes.Count == 2)
                {
                    pathTextureName = ((XElement)textureNodes[0]).Value;
                    pathTexturePath = ((XElement)textureNodes[1]).Value;
                }

                if (!string.IsNullOrEmpty(pathTextureName) && !string.IsNullOrEmpty(pathTexturePath))
                {
                    PathTexture = new MapTexture(pathTextureName, pathTexturePath);
                }
                else
                {
                    PathTexture = null;
                }
            }
            else
            {
                PathTexture = null;
            }

            IEnumerable<XElement> pathTextureOpacityElem = mapPathDoc.Descendants(ns + "PathTextureOpacity");
            if (pathTextureOpacityElem != null && pathTextureOpacityElem.Any() && pathTextureOpacityElem.First() != null)
            {
                string? pathTextureOpacity = mapPathDoc.Descendants().Select(x => x.Element(ns + "PathTextureOpacity").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(pathTextureOpacity))
                {
                    int pathTextureOpacityValue = 255;

                    if (int.TryParse(pathTextureOpacity, out int n))
                    {
                        if (n > 0)
                        {
                            pathTextureOpacityValue = n;
                        }
                        else
                        {
                            pathTextureOpacityValue = 255;
                        }
                    }

                    PathTextureOpacity = pathTextureOpacityValue;
                }
                else
                {
                    PathTextureOpacity = 255;
                }
            }
            else
            {
                PathTextureOpacity = 255;
            }

            IEnumerable<XElement> pathTextureScaleElem = mapPathDoc.Descendants(ns + "PathTextureScale");
            if (pathTextureScaleElem != null && pathTextureScaleElem.Any() && pathTextureScaleElem.First() != null)
            {
                string? pathTextureScale = mapPathDoc.Descendants().Select(x => x.Element(ns + "PathTextureScale").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(pathTextureScale))
                {
                    float pathTextureScaleValue = 1.0F;

                    if (float.TryParse(pathTextureScale, out float n))
                    {
                        if (n > 0.0F && n < 2.0F)
                        {
                            pathTextureScaleValue = n;
                        }
                        else
                        {
                            pathTextureScaleValue = 1.0F;
                        }
                    }

                    PathTextureScale = pathTextureScaleValue;
                }
                else
                {
                    PathTextureScale = 1.0F;
                }
            }
            else
            {
                PathTextureScale = 1.0F;
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

            // map path description
            writer.WriteStartElement("MapPathDescription");
            writer.WriteString(MapPathDescription);
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

            writer.WriteStartElement("PathTextureOpacity");
            writer.WriteValue(PathTextureOpacity);
            writer.WriteEndElement();

            writer.WriteStartElement("PathTextureScale");
            writer.WriteValue(PathTextureScale);
            writer.WriteEndElement();

            if (PathTexture != null)
            {
                writer.WriteStartElement("PathTexture");
                writer.WriteStartElement("TextureName");
                writer.WriteString(PathTexture.TextureName);
                writer.WriteEndElement();
                writer.WriteStartElement("TexturePath");
                writer.WriteString(PathTexture.TexturePath);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
}
