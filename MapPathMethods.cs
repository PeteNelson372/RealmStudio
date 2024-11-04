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
    internal class MapPathMethods
    {
        private static SKShader? PathTextureShader { get; set; } = null;

        public static void ConstructPathPaint(MapPath mapPath)
        {
            if (mapPath.PathPaint != null) return;

            float strokeWidth = mapPath.PathWidth;

            switch (mapPath.PathType)
            {
                case PathTypeEnum.ThickSolidLinePath:
                    strokeWidth = mapPath.PathWidth * 1.5F;
                    break;
            }

            SKPaint pathPaint = new()
            {
                Color = Extensions.ToSKColor(mapPath.PathColor),
                StrokeWidth = strokeWidth,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
            };

            SKPathEffect pathLineEffect = ConstructPathLineEffect(mapPath.PathType, mapPath.PathWidth * 2);

            if (pathLineEffect != null)
            {
                pathPaint.PathEffect = pathLineEffect;
            }

            switch (mapPath.PathType)
            {
                case PathTypeEnum.LineAndDashesPath:
                case PathTypeEnum.BorderedGradientPath:
                case PathTypeEnum.BorderedLightSolidPath:
                    pathPaint.StrokeCap = SKStrokeCap.Butt;
                    break;
                case PathTypeEnum.TexturedPath:
                    // construct a shader from the selected path texture
                    pathPaint.Shader = SKShader.CreateBitmap(mapPath.PathTexture, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathTypeEnum.BorderAndTexturePath:
                    pathPaint.StrokeCap = SKStrokeCap.Butt;

                    // construct a shader from the selected path texture
                    pathPaint.Shader = SKShader.CreateBitmap(mapPath.PathTexture, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
            }

            mapPath.PathPaint = pathPaint;
        }

        private static SKPathEffect ConstructPathLineEffect(PathTypeEnum pathType, float pathWidth)
        {
            SKPathEffect? pathLineEffect = null;

            switch (pathType)
            {
                case PathTypeEnum.DottedLinePath:
                    float[] intervals = [0, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.DashedLinePath:
                    intervals = [pathWidth, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.DashDotLinePath:
                    intervals = [pathWidth, pathWidth, 0, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.DashDotDotLinePath:
                    intervals = [pathWidth, pathWidth, 0, pathWidth, 0, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathTypeEnum.ChevronLinePath:
                    string svgPath = "M 0 0"
                        + " L" + pathWidth.ToString() + " 0"
                        + " L" + (pathWidth * 1.5F).ToString() + " " + (pathWidth / 2.0F).ToString()
                        + " L" + pathWidth.ToString() + " " + pathWidth.ToString()
                        + " L0 " + pathWidth.ToString()
                        + " L" + (pathWidth / 2.0F).ToString() + " " + (pathWidth / 2.0F).ToString()
                        + " L0 0"
                        + " Z";

                    pathLineEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(svgPath), pathWidth * 2, 0, SKPath1DPathEffectStyle.Rotate);
                    break;
                case PathTypeEnum.LineAndDashesPath:

                    float ldWidth = Math.Max(1, pathWidth / 2.0F);

                    svgPath = "M 0 0"
                        + " h" + (pathWidth).ToString()
                        + " v" + Math.Max(1, ldWidth / 2.0F).ToString()
                        + " h" + (-pathWidth).ToString()
                        + " M0" + "," + (pathWidth - 1.0F).ToString()
                        + " h" + (ldWidth).ToString()
                        + " v2"
                        + " h" + (-ldWidth).ToString();

                    pathLineEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(svgPath),
                        pathWidth, 0, SKPath1DPathEffectStyle.Morph);
                    break;
                case PathTypeEnum.ShortIrregularDashPath:
                    svgPath = "m0 0"
                        + " v " + pathWidth.ToString()
                        + " h " + (pathWidth / 4.0F).ToString()
                        + " v " + (-pathWidth).ToString()
                        + " z"
                        + " m" + pathWidth.ToString() + " 0"
                        + " v " + pathWidth.ToString()
                        + " h " + (pathWidth / 4.0F).ToString()
                        + " v " + (-pathWidth).ToString()
                        + " z";

                    pathLineEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(svgPath),
                        pathWidth * 2, 0, SKPath1DPathEffectStyle.Rotate);
                    break;
                case PathTypeEnum.BearTracksPath:
                    SKPath? bearTrackPath = GetPathFromSvg("Bear Tracks", pathWidth);

                    if (bearTrackPath != null)
                    {
                        pathLineEffect = SKPathEffect.Create1DPath(bearTrackPath,
                            pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    }

                    break;
                case PathTypeEnum.BirdTracksPath:
                    SKPath? birdTrackPath = GetPathFromSvg("Bird Tracks", pathWidth);

                    if (birdTrackPath != null)
                    {
                        pathLineEffect = SKPathEffect.Create1DPath(birdTrackPath,
                            pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    }
                    break;
                case PathTypeEnum.FootprintsPath:
                    SKPath? footprintsPath = GetPathFromSvg("Foot Prints", pathWidth);

                    if (footprintsPath != null)
                    {
                        pathLineEffect = SKPathEffect.Create1DPath(footprintsPath,
                            pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    }
                    break;
                case PathTypeEnum.RailroadTracksPath:
                    // TODO: the railroad tracks path doesn't look great; improve it
                    svgPath = "M0,0"
                        + " h " + pathWidth.ToString()
                        + " v" + (pathWidth * 0.2F).ToString()
                        + " h " + (-pathWidth).ToString()
                        + " M" + (pathWidth / 3.33F).ToString() + ", " + (pathWidth * 0.2F).ToString()
                        + " v " + pathWidth.ToString()
                        + " h" + (pathWidth * 0.2F).ToString()
                        + " v " + (-pathWidth).ToString()
                        + " M0," + (pathWidth * 1.2F).ToString()
                        + " h " + pathWidth.ToString()
                        + " v" + (-pathWidth * 0.2F).ToString()
                        + " h " + (-pathWidth).ToString();

                    pathLineEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(svgPath), pathWidth, 0, SKPath1DPathEffectStyle.Morph);
                    break;

            }

#pragma warning disable CS8603 // Possible null reference return.
            return pathLineEffect;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private static SKPath? GetPathFromSvg(string vectorName, float pathWidth)
        {
            MapVector? pathVector = null;

            for (int i = 0; i < AssetManager.PATH_VECTOR_LIST.Count; i++)
            {
                if (AssetManager.PATH_VECTOR_LIST[i].VectorName == vectorName)
                {
                    pathVector = AssetManager.PATH_VECTOR_LIST[i];
                    break;
                }
            }

            if (pathVector != null && pathVector.VectorSkPath != null)
            {
                pathVector.ScaledPath = new(pathVector.VectorSkPath);

                float xSize = pathWidth;
                float ySize = pathWidth;

                if (pathVector.ViewBoxSizeWidth != 0 && pathVector.ViewBoxSizeHeight != 0)
                {
                    xSize = pathVector.ViewBoxSizeWidth;
                    ySize = pathVector.ViewBoxSizeHeight;
                }

                float xScale = pathWidth / xSize;
                float yScale = pathWidth / ySize;

                pathVector.ScaledPath.Transform(SKMatrix.CreateScale(xScale, yScale));
                return pathVector.ScaledPath;
            }

            return null;
        }

        internal static List<MapPathPoint> GetParallelPathPoints(List<MapPathPoint> points, float distance, ParallelEnum location)
        {
            List<MapPathPoint> parallelPoints = [];

            for (int i = 0; i < points.Count - 1; i += 2)
            {
                float lineAngle = DrawingMethods.CalculateLineAngle(points[i].MapPoint, points[i + 1].MapPoint);

                float angle = (location == ParallelEnum.Below) ? 90 : -90;

                SKPoint p1 = DrawingMethods.PointOnCircle(distance, lineAngle + angle, points[i].MapPoint);
                SKPoint p2 = DrawingMethods.PointOnCircle(distance, lineAngle + angle, points[i + 1].MapPoint);

                parallelPoints.Add(new MapPathPoint(p1));
                parallelPoints.Add(new MapPathPoint(p2));
            }

            return parallelPoints;
        }

        internal static void DrawBezierCurvesFromPoints(SKCanvas canvas, List<MapPathPoint> curvePoints, SKPaint paint)
        {
            if (curvePoints.Count == 2)
            {
                canvas.DrawLine(curvePoints[0].MapPoint, curvePoints[1].MapPoint, paint);
            }
            else if (curvePoints.Count > 2)
            {
                using SKPath path = new();
                path.MoveTo(curvePoints[0].MapPoint);

                for (int j = 0; j < curvePoints.Count; j += 3)
                {
                    if (j < curvePoints.Count - 2)
                    {
                        path.CubicTo(curvePoints[j].MapPoint, curvePoints[j + 1].MapPoint, curvePoints[j + 2].MapPoint);
                    }
                }

                canvas.DrawPath(path, paint);
            }
        }

        public static SKPath GenerateMapPathBoundaryPath(List<MapPathPoint> points)
        {
            SKPath path = new();

            if (points.Count < 3) return path;

            path.MoveTo(points[0].MapPoint);

            for (int j = 0; j < points.Count; j += 3)
            {
                if (j < points.Count - 2)
                {
                    path.CubicTo(points[j].MapPoint, points[j + 1].MapPoint, points[j + 2].MapPoint);
                }
            }

            return path;
        }

        public static MapPathPoint? SelectMapPathPointAtPoint(MapPath mapPath, SKPoint mapClickPoint, bool selectOnlyControlPoints = true)
        {
            MapPathPoint? selectedPoint = null;

            for (int i = 0; i < mapPath.PathPoints.Count; i++)
            {
                using SKPath controlPointPath = new();
                controlPointPath.AddCircle(mapPath.PathPoints[i].MapPoint.X, mapPath.PathPoints[i].MapPoint.Y, mapPath.PathWidth);

                if (selectOnlyControlPoints)
                {
                    if (controlPointPath.Contains(mapClickPoint.X, mapClickPoint.Y) && mapPath.PathPoints[i].IsControlPoint)
                    {
                        selectedPoint = mapPath.PathPoints[i];
                        break;
                    }
                }
                else if (controlPointPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                {
                    selectedPoint = mapPath.PathPoints[i];
                    break;
                }
            }

            if (selectedPoint != null)
            {
                selectedPoint.IsSelected = true;
            }

            return selectedPoint;
        }

        internal static void MoveSelectedMapPathPoint(MapPath? selectedMapPath, MapPathPoint mapPathPoint, SKPoint movePoint)
        {
            if (selectedMapPath != null && mapPathPoint != null)
            {
                int selectedIndex = GetMapPathPointIndexById(selectedMapPath, mapPathPoint.PointGuid);

                if (selectedIndex > -1)
                {
                    // move the selected mappath point and the 4 points before and after it
                    if (selectedIndex - 4 > 0)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.2F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.2F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedMapPath.PathPoints[selectedIndex - 4].MapPoint = newPoint;
                    }

                    if (selectedIndex - 3 > 0)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.4F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.4F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedMapPath.PathPoints[selectedIndex - 3].MapPoint = newPoint;
                    }

                    if (selectedIndex - 2 > 0)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.6F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.6F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedMapPath.PathPoints[selectedIndex - 2].MapPoint = newPoint;
                    }

                    if (selectedIndex - 1 > 0)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.8F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.8F;
                        SKPoint newPoint = new(movePoint.X - xDelta, movePoint.Y - yDelta);
                        selectedMapPath.PathPoints[selectedIndex - 1].MapPoint = newPoint;
                    }

                    selectedMapPath.PathPoints[selectedIndex].MapPoint = movePoint;

                    if (selectedIndex + 1 < selectedMapPath.PathPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.2F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.2F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedMapPath.PathPoints[selectedIndex + 1].MapPoint = newPoint;
                    }

                    if (selectedIndex + 2 < selectedMapPath.PathPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.4F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.4F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedMapPath.PathPoints[selectedIndex + 2].MapPoint = newPoint;
                    }

                    if (selectedIndex + 3 < selectedMapPath.PathPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.6F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.6F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedMapPath.PathPoints[selectedIndex + 3].MapPoint = newPoint;
                    }

                    if (selectedIndex + 4 < selectedMapPath.PathPoints.Count - 1)
                    {
                        float xDelta = (movePoint.X - selectedMapPath.PathPoints[selectedIndex].MapPoint.X) * 0.8F;
                        float yDelta = (movePoint.Y - selectedMapPath.PathPoints[selectedIndex].MapPoint.Y) * 0.8F;
                        SKPoint newPoint = new(movePoint.X + xDelta, movePoint.Y + yDelta);
                        selectedMapPath.PathPoints[selectedIndex + 4].MapPoint = newPoint;
                    }

                }

                SKPath path = GenerateMapPathBoundaryPath(selectedMapPath.PathPoints);
                selectedMapPath.BoundaryPath?.Dispose();
                selectedMapPath.BoundaryPath = new(path);
                path.Dispose();
            }
        }

        public static int GetMapPathPointIndexById(MapPath mapPath, Guid guid)
        {
            for (int i = 0; i < mapPath.PathPoints.Count; i++)
            {
                if (mapPath.PathPoints[i].PointGuid == guid)
                {
                    return i;
                }
            }

            return -1;
        }

        public static MapPathPoint? GetMapPathPointById(MapPath mapPath, Guid guid)
        {
            foreach (MapPathPoint point in mapPath.PathPoints)
            {
                if (point.PointGuid == guid)
                {
                    return point;
                }
            }

            return null;
        }
    }
}
