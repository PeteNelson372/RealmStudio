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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class InteriorWall : MapComponent, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; }
        public Guid WallGuid { get; set; } = Guid.NewGuid();
        public string WallName { get; set; } = "";
        public string WallDescription { get; set; } = string.Empty;
        public List<LinePoint> WallPoints { get; set; } = [];
        public PathType WallType { get; set; } = PathType.SolidWall;
        public Color WallOutlineColor { get; set; } = ColorTranslator.FromHtml("#4B311A");
        public Color WallBackgroundColor { get; set; } = ColorTranslator.FromHtml("#4B311A");
        public float WallWidth { get; set; } = 8;
        public float WallTowerDistance { get; set; } = 10.0F;
        public float WallTowerSize { get; set; } = 1.2F;
        public MapTexture? WallTexture { get; set; }
        public int WallTextureOpacity { get; set; } = 255;      // not settable via UI at this time
        public float WallTextureScale { get; set; } = 1.0F;
        public bool DrawOverSymbols { get; set; }
        public bool ShowWallPoints { get; set; }
        public SKPaint? WallPaint { get; set; }
        public SKPath BoundaryPath { get; set; } = new();

        public InteriorWall()
        {
        }

        public InteriorWall(InteriorWall original)
        {
            X = original.X;
            Y = original.Y;
            DrawOverSymbols = original.DrawOverSymbols;
            Height = original.Height;
            WallName = original.WallName;
            ParentMap = original.ParentMap;
            WallOutlineColor = original.WallOutlineColor;
            WallBackgroundColor = original.WallBackgroundColor;

            if (original.WallTexture != null)
            {
                WallTexture = new MapTexture(original.WallTexture.TextureName, original.WallTexture.TexturePath);
            }

            WallType = original.WallType;
            WallWidth = original.WallWidth;
            ShowWallPoints = original.ShowWallPoints;
            WallTextureOpacity = original.WallTextureOpacity;
            WallTextureScale = original.WallTextureScale;
        }

        public override void Render(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            List<LinePoint> distinctWallPoints = [.. WallPoints.Distinct(new LinePointComparer())];

            switch (WallType)
            {
                case PathType.SolidBlackBorderPath:
                    {
                        using SKPaint blackBorderPaint = WallPaint.Clone();
                        blackBorderPaint.StrokeWidth = WallPaint.StrokeWidth * 1.2F;
                        blackBorderPaint.Color = SKColors.Black;

                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, blackBorderPaint);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, WallPaint);
                    }
                    break;
                case PathType.BorderedGradientPath:
                    {
                        using SKPaint borderPaint = WallPaint.Clone();
                        borderPaint.StrokeWidth = WallPaint.StrokeWidth * 0.2F;
                        borderPaint.Color = SKColors.Black;

                        List<LinePoint> parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);

                        using SKPaint linePaint1 = WallPaint.Clone();
                        linePaint1.StrokeWidth = WallPaint.StrokeWidth * 0.2F;
                        Color clr = Color.FromArgb(154, WallOutlineColor);
                        linePaint1.Color = Extensions.ToSKColor(clr);

                        parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.4F, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint1);

                        using SKPaint linePaint2 = WallPaint.Clone();
                        linePaint2.StrokeWidth = WallPaint.StrokeWidth * 0.2F;
                        clr = Color.FromArgb(102, WallOutlineColor);
                        linePaint2.Color = Extensions.ToSKColor(clr);

                        parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.6F, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint2);

                        using SKPaint linePaint3 = WallPaint.Clone();
                        linePaint3.StrokeWidth = WallPaint.StrokeWidth * 0.2F;
                        clr = Color.FromArgb(51, WallOutlineColor);
                        linePaint3.Color = Extensions.ToSKColor(clr);

                        parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.8F, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint3);

                        using SKPaint linePaint4 = WallPaint.Clone();
                        linePaint4.StrokeWidth = WallPaint.StrokeWidth * 0.2F;
                        clr = Color.FromArgb(25, WallOutlineColor);
                        linePaint4.Color = Extensions.ToSKColor(clr);

                        parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint4);
                    }
                    break;
                case PathType.BorderedLightSolidPath:
                    {
                        using SKPaint borderPaint = WallPaint.Clone();
                        borderPaint.StrokeWidth = WallPaint.StrokeWidth * 0.2F;
                        borderPaint.Color = SKColors.Black;

                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, borderPaint);

                        using SKPaint linePaint1 = WallPaint.Clone();
                        linePaint1.StrokeWidth = WallPaint.StrokeWidth * 0.8F;
                        Color clr = Color.FromArgb(102, WallOutlineColor);
                        linePaint1.Color = Extensions.ToSKColor(clr);

                        List<LinePoint> parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.2F, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, linePaint1);
                    }
                    break;
                case PathType.DoubleSolidBorderPath:
                    {
                        using SKPaint borderPaint = WallPaint.Clone();
                        borderPaint.StrokeWidth = WallPaint.StrokeWidth * 0.2F;

                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, borderPaint);

                        List<LinePoint> parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth, ParallelDirection.Above);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                    }
                    break;
                case PathType.LineAndDashesPath:
                    {
                        using SKPaint borderPaint = WallPaint.Clone();
                        borderPaint.StrokeWidth = WallPaint.StrokeWidth * 0.2F;

                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, borderPaint);

                        float[] intervals = [WallWidth, WallWidth];

                        SKPathEffect? pathLineEffect = null;
                        pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                        borderPaint.PathEffect = pathLineEffect;

                        List<LinePoint> parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth, ParallelDirection.Above);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                    }
                    break;
                case PathType.ShortIrregularDashPath:
                    {
                        using SKPaint borderPaint = WallPaint.Clone();

                        float[] intervals = [WallWidth / 4.0F, WallWidth, WallWidth / 4.0F, WallWidth];

                        SKPathEffect pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                        borderPaint.PathEffect = pathLineEffect;
                        borderPaint.StrokeWidth = WallPaint.StrokeWidth;
                        borderPaint.StrokeCap = SKStrokeCap.Butt;

                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, borderPaint);
                    }
                    break;
                case PathType.BorderAndTexturePath:
                    {
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, WallPaint);

                        using SKPaint borderPaint = WallPaint.Clone();
                        borderPaint.Shader = null;

                        borderPaint.StrokeWidth = WallPaint.StrokeWidth * 0.2F;

                        List<LinePoint> parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);

                        parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.5F, ParallelDirection.Below);
                        InteriorManager.DrawBezierCurvesFromPoints(canvas, parallelPoints, borderPaint);
                    }

                    break;
                case PathType.RailroadTracksPath:
                    {
                        using SKPaint trackPaint = WallPaint.Clone();
                        trackPaint.StrokeWidth = WallPaint.StrokeWidth * 0.2F;

                        InteriorManager.DrawLineFromPoints(canvas, distinctWallPoints, trackPaint);

                        List<LinePoint> parallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.8F, ParallelDirection.Above);
                        InteriorManager.DrawLineFromPoints(canvas, parallelPoints, trackPaint);

                        float[] intervals = [WallWidth / 4.0F, WallWidth, WallWidth / 4.0F, WallWidth];

                        SKPathEffect pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                        trackPaint.PathEffect = pathLineEffect;
                        trackPaint.StrokeWidth = WallWidth * 1.5F;
                        trackPaint.StrokeCap = SKStrokeCap.Butt;

                        List<LinePoint> crossTiePoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.4F, ParallelDirection.Above);
                        InteriorManager.DrawLineFromPoints(canvas, crossTiePoints, trackPaint);
                    }
                    break;
                case PathType.RoundTowerWall:
                    {
                        if (distinctWallPoints.Count < 2) return;

                        using SKPaint borderPaint = new()
                        {
                            Color = Extensions.ToSKColor(WallOutlineColor),
                            StrokeWidth = WallPaint.StrokeWidth * 0.25F,
                            Style = SKPaintStyle.Stroke,
                            StrokeCap = SKStrokeCap.Round,
                            StrokeJoin = SKStrokeJoin.Round,
                            StrokeMiter = 1.0F,
                            IsAntialias = true,
                        };

                        List<LinePoint> aboveParallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                        List<LinePoint> belowParallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.5F, ParallelDirection.Below);

                        canvas.DrawCircle(distinctWallPoints[0].LineSegmentPoint, WallWidth * WallTowerSize, WallPaint);
                        canvas.DrawCircle(distinctWallPoints[0].LineSegmentPoint, WallWidth * WallTowerSize * 1.01F, borderPaint);

                        WallPaint.Style = SKPaintStyle.Fill;
                        SKPoint towerPoint = distinctWallPoints[0].LineSegmentPoint;

                        float distance = 0.0f;

                        for (int i = 1; i < distinctWallPoints.Count - 1; i++)
                        {
                            distance += SKPoint.Distance(distinctWallPoints[i - 1].LineSegmentPoint, distinctWallPoints[i].LineSegmentPoint);

                            canvas.DrawLine(distinctWallPoints[i - 1].LineSegmentPoint, distinctWallPoints[i].LineSegmentPoint, WallPaint);

                            if (i > 0 && i < aboveParallelPoints.Count - 1
                                && i < belowParallelPoints.Count - 1
                                && aboveParallelPoints.Count > 2
                                && belowParallelPoints.Count > 2)
                            {
                                canvas.DrawLine(aboveParallelPoints[i - 1].LineSegmentPoint, aboveParallelPoints[i].LineSegmentPoint, borderPaint);
                                canvas.DrawLine(belowParallelPoints[i - 1].LineSegmentPoint, belowParallelPoints[i].LineSegmentPoint, borderPaint);
                            }

                            if (towerPoint != SKPoint.Empty)
                            {
                                canvas.DrawCircle(towerPoint, WallWidth * WallTowerSize, WallPaint);
                                canvas.DrawCircle(towerPoint, WallWidth * WallTowerSize * 1.01F, borderPaint);
                            }

                            if (distance > WallWidth * WallTowerDistance)
                            {
                                towerPoint = distinctWallPoints[i].LineSegmentPoint;

                                canvas.DrawCircle(distinctWallPoints[i].LineSegmentPoint, WallWidth * WallTowerSize, WallPaint);
                                canvas.DrawCircle(distinctWallPoints[i].LineSegmentPoint, WallWidth * WallTowerSize * 1.01F, borderPaint);

                                distance = 0.0f;
                            }
                        }
                    }
                    break;
                case PathType.SquareTowerWall:
                    {
                        if (distinctWallPoints.Count < 2) return;

                        using SKPaint borderPaint = new()
                        {
                            Color = Extensions.ToSKColor(WallOutlineColor),
                            StrokeWidth = WallPaint.StrokeWidth * 0.25F,
                            Style = SKPaintStyle.Stroke,
                            StrokeCap = SKStrokeCap.Round,
                            StrokeJoin = SKStrokeJoin.Round,
                            StrokeMiter = 1.0F,
                            IsAntialias = true,
                        };

                        List<LinePoint> aboveParallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.5F, ParallelDirection.Above);
                        List<LinePoint> belowParallelPoints = InteriorManager.GetParallelPathPoints(distinctWallPoints, WallPaint.StrokeWidth * 0.5F, ParallelDirection.Below);

                        float lineAngle = DrawingMethods.CalculateAngleBetweenPoints(distinctWallPoints[0].LineSegmentPoint, distinctWallPoints[1].LineSegmentPoint, true);

                        SKMatrix translateMatrix = SKMatrix.CreateTranslation(-WallWidth * WallTowerSize, -WallWidth * WallTowerSize);
                        SKMatrix m = SKMatrix.CreateRotationDegrees(lineAngle, distinctWallPoints[0].LineSegmentPoint.X, distinctWallPoints[0].LineSegmentPoint.Y);

                        m = m.PreConcat(translateMatrix);

                        canvas.SetMatrix(m);

                        canvas.DrawRect(distinctWallPoints[0].LineSegmentPoint.X, distinctWallPoints[0].LineSegmentPoint.Y, WallWidth * WallTowerSize * 2, WallWidth * WallTowerSize * 2, WallPaint);
                        canvas.DrawRect(distinctWallPoints[0].LineSegmentPoint.X, distinctWallPoints[0].LineSegmentPoint.Y, WallWidth * WallTowerSize * 2.02F, WallWidth * WallTowerSize * 2.02F, borderPaint);

                        canvas.ResetMatrix();

                        WallPaint.Style = SKPaintStyle.Fill;

                        float distance = 0.0f;
                        SKPoint towerPoint = distinctWallPoints[0].LineSegmentPoint;
                        float towerLineAngle = lineAngle;

                        for (int i = 1; i < distinctWallPoints.Count - 1; i++)
                        {
                            distance += SKPoint.Distance(distinctWallPoints[i - 1].LineSegmentPoint, distinctWallPoints[i].LineSegmentPoint);

                            canvas.DrawLine(distinctWallPoints[i - 1].LineSegmentPoint, distinctWallPoints[i].LineSegmentPoint, WallPaint);

                            if (i > 0 && i < aboveParallelPoints.Count - 1
                                && i < belowParallelPoints.Count - 1
                                && aboveParallelPoints.Count > 2
                                && belowParallelPoints.Count > 2)
                            {
                                canvas.DrawLine(aboveParallelPoints[i - 1].LineSegmentPoint, aboveParallelPoints[i].LineSegmentPoint, borderPaint);
                                canvas.DrawLine(belowParallelPoints[i - 1].LineSegmentPoint, belowParallelPoints[i].LineSegmentPoint, borderPaint);
                            }

                            if (towerPoint != SKPoint.Empty)
                            {
                                m = SKMatrix.CreateRotationDegrees(lineAngle, towerPoint.X, towerPoint.Y);
                                m = m.PreConcat(translateMatrix);

                                canvas.SetMatrix(m);

                                canvas.DrawRect(towerPoint.X, towerPoint.Y, WallWidth * WallTowerSize * 2, WallWidth * WallTowerSize * 2, WallPaint);
                                canvas.DrawRect(towerPoint.X, towerPoint.Y, WallWidth * WallTowerSize * 2.02F, WallWidth * WallTowerSize * 2.02F, borderPaint);

                                canvas.ResetMatrix();
                            }

                            if (distance > WallWidth * WallTowerDistance)
                            {
                                lineAngle = DrawingMethods.CalculateAngleBetweenPoints(distinctWallPoints[i - 1].LineSegmentPoint, distinctWallPoints[i].LineSegmentPoint, true);
                                towerLineAngle = lineAngle;
                                towerPoint = distinctWallPoints[i].LineSegmentPoint;

                                m = SKMatrix.CreateRotationDegrees(lineAngle, distinctWallPoints[i].LineSegmentPoint.X, distinctWallPoints[i].LineSegmentPoint.Y);
                                m = m.PreConcat(translateMatrix);

                                canvas.SetMatrix(m);

                                canvas.DrawRect(distinctWallPoints[i].LineSegmentPoint.X, distinctWallPoints[i].LineSegmentPoint.Y, WallWidth * WallTowerSize * 2, WallWidth * WallTowerSize * 2, WallPaint);
                                canvas.DrawRect(distinctWallPoints[i].LineSegmentPoint.X, distinctWallPoints[i].LineSegmentPoint.Y, WallWidth * WallTowerSize * 2.02F, WallWidth * WallTowerSize * 2.02F, borderPaint);

                                canvas.ResetMatrix();

                                distance = 0.0f;
                            }
                        }
                    }
                    break;
                default:
                    InteriorManager.DrawBezierCurvesFromPoints(canvas, distinctWallPoints, WallPaint);
                    break;
            }
        }

        public List<LinePoint> GetWallControlPoints()
        {
            List<LinePoint> wallPoints = [];

            for (int i = 0; i < WallPoints.Count - 10; i += 10)
            {
                wallPoints.Add(WallPoints[i]);
                WallPoints[i].IsControlPoint = true;
            }

            wallPoints.Add(WallPoints[^1]);
            WallPoints[^1].IsControlPoint = true;

            return wallPoints;
        }

        public XmlSchema? GetSchema()
        {
            return null;
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
