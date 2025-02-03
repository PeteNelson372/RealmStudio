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
                StrokeMiter = 1.0F,
                IsAntialias = true,
            };

            SKPathEffect? pathLineEffect = ConstructPathLineEffect(mapPath.PathType, mapPath.PathWidth * 2);

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
                    if (mapPath.PathTexture != null)
                    {
                        // construct a shader from the selected path texture
                        pathPaint.Shader = SKShader.CreateBitmap(mapPath.PathTexture.TextureBitmap.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathTypeEnum.BorderAndTexturePath:
                    pathPaint.StrokeCap = SKStrokeCap.Round;

                    if (mapPath.PathTexture != null)
                    {
                        // construct a shader from the selected path texture
                        pathPaint.Shader = SKShader.CreateBitmap(mapPath.PathTexture.TextureBitmap.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }
                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
            }

            mapPath.PathPaint = pathPaint;
        }

        private static SKPathEffect? ConstructPathLineEffect(PathTypeEnum pathType, float pathWidth)
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

            return pathLineEffect;
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

            float d = (location == ParallelEnum.Below) ? distance : -distance;
            float offsetAngle = (location == ParallelEnum.Above) ? 90 : -90;

            SKPoint maxXPoint = new SKPoint(-1.0F, 0);
            SKPoint maxYPoint = new SKPoint(0, -1.0F);

            SKPoint minXPoint = new SKPoint(65535.0F, 0);
            SKPoint minYPoint = new SKPoint(0, 65535.0F);

            for (int i = 0; i < points.Count - 1; i++)
            {
                SKPoint newPoint;

                if (i == 0)
                {
                    float lineAngle = DrawingMethods.CalculateLineAngle(points[i].MapPoint, points[i + 1].MapPoint);
                    newPoint = DrawingMethods.PointOnCircle(distance, lineAngle + offsetAngle, points[i].MapPoint);
                    parallelPoints.Add(new MapPathPoint(newPoint));

                    if (newPoint.X > maxXPoint.X)
                    {
                        maxXPoint = newPoint;
                    }

                    if (newPoint.Y < minYPoint.Y)
                    {
                        minYPoint = newPoint;
                    }

                    if (newPoint.X < minXPoint.X)
                    {
                        minXPoint = newPoint;
                    }

                    if (newPoint.Y > maxYPoint.Y)
                    {
                        maxYPoint = newPoint;
                    }
                }
                else
                {
                    float lineAngle1 = DrawingMethods.CalculateLineAngle(points[i - 1].MapPoint, points[i].MapPoint);
                    float lineAngle2 = DrawingMethods.CalculateLineAngle(points[i].MapPoint, points[i + 1].MapPoint);

                    float angleDifference = (float)((lineAngle2 - lineAngle1 >= 0) ? Math.Round(lineAngle2 - lineAngle1) : Math.Round((lineAngle2 - lineAngle1) + 360.0F));
                    float circleRadius = (float)Math.Sqrt(distance * distance + distance * distance);

                    if (angleDifference == 0.0F)
                    {
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference > 0.0F && angleDifference < 90.0F)
                    {
                        //1
                        float circleAngle = lineAngle1 + offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 90.0F)
                    {
                        // edge case - do not add a point if the lines are 90 degrees from each other
                    }
                    else if (angleDifference > 90.0F && angleDifference < 180.0F)
                    {
                        //2
                        float circleAngle = lineAngle1 + offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 180.0F) //====================================
                    {
                        newPoint = DrawingMethods.PointOnCircle(distance, lineAngle1 + offsetAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));
                    }
                    else if (angleDifference > 180.0F && angleDifference < 270.0F)
                    {
                        //3
                        float circleAngle = lineAngle1 - offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 270.0F)
                    {
                        // edge case - do not add a point if the lines are 270 degrees from each other
                    }
                    else if (angleDifference > 270.0F && angleDifference < 360.0F)
                    {
                        //4
                        float circleAngle = lineAngle1 - offsetAngle + (angleDifference / 2.0F);
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, circleAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                    else if (angleDifference == 360.0F)
                    {
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].MapPoint);
                        parallelPoints.Add(new MapPathPoint(newPoint));

                        if (newPoint.X > maxXPoint.X)
                        {
                            maxXPoint = newPoint;
                        }

                        if (newPoint.Y < minYPoint.Y)
                        {
                            minYPoint = newPoint;
                        }
                    }
                }
            }

            // remove points that cause loops in the parallel path when
            // the path has a sharp turn in it; his is done by finding the
            // point where the lines in the path intersect, then removing
            // the points that are outside the lines (X value is greater
            // than the intersection point X value and Y value is less than
            // the intersection point Y value)

            // this algorithm doesn't work consistently, so it is
            // commented out; need to figure out a reliable algorithm
            // for removing the loops, maybe by taking into account the
            // "direction" of the line?

            /*
            bool pointsRemoved = false;

            if (parallelPoints.Count > 3)
            {
                // get the intersection between the lines
                // Line AB represented as a1x + b1y = c1
                // Line CD represented as a2x + b2y = c2 
                // A is parallelPoints[0].MapPoint
                // B is maxXPoint
                // C is minYPoint
                // D is parallelPoints.Last().MapPoint

                double a1 = maxXPoint.Y - parallelPoints[0].MapPoint.Y;
                double b1 = parallelPoints[0].MapPoint.X - maxXPoint.X;
                double c1 = a1 * (parallelPoints[0].MapPoint.X) + b1 * (parallelPoints[0].MapPoint.Y);

                // Line CD represented as a2x + b2y = c2 
                double a2 = parallelPoints.Last().MapPoint.Y - minYPoint.Y;
                double b2 = minYPoint.X - parallelPoints.Last().MapPoint.X;
                double c2 = a2 * (minYPoint.X) + b2 * (minYPoint.Y);

                double determinant = a1 * b2 - a2 * b1;

                SKPoint intersectionPoint = SKPoint.Empty;

                if (determinant != 0)
                {
                    double x = (b2 * c1 - b1 * c2) / determinant;
                    double y = (a1 * c2 - a2 * c1) / determinant;

                    intersectionPoint = new SKPoint((float)x, (float)y);
                }

                if (!intersectionPoint.IsEmpty)
                {
                    for (int i = parallelPoints.Count - 1; i >= 0; i--)
                    {
                        if (parallelPoints[i].MapPoint.X > intersectionPoint.X
                            && parallelPoints[i].MapPoint.Y <= intersectionPoint.Y)
                        {
                            parallelPoints.RemoveAt(i);
                            pointsRemoved = true;
                        }
                    }
                }
            }

            if (parallelPoints.Count > 3 && !pointsRemoved)
            {
                // A is parallelPoints[0].MapPoint
                // B is minYPoint
                // C is minXPoint
                // D is parallelPoints.Last().MapPoint

                double a1 = minYPoint.Y - parallelPoints[0].MapPoint.Y;
                double b1 = parallelPoints[0].MapPoint.X - minYPoint.X;
                double c1 = a1 * (parallelPoints[0].MapPoint.X) + b1 * (parallelPoints[0].MapPoint.Y);

                // Line CD represented as a2x + b2y = c2 
                double a2 = parallelPoints.Last().MapPoint.Y - minXPoint.Y;
                double b2 = minXPoint.X - parallelPoints.Last().MapPoint.X;
                double c2 = a2 * (minXPoint.X) + b2 * (minXPoint.Y);

                double determinant = a1 * b2 - a2 * b1;

                SKPoint intersectionPoint = SKPoint.Empty;

                if (determinant != 0)
                {
                    double x = (b2 * c1 - b1 * c2) / determinant;
                    double y = (a1 * c2 - a2 * c1) / determinant;

                    intersectionPoint = new SKPoint((float)x, (float)y);
                }

                if (!intersectionPoint.IsEmpty)
                {
                    for (int i = parallelPoints.Count - 1; i >= 0; i--)
                    {
                        if (parallelPoints[i].MapPoint.X < intersectionPoint.X
                            && parallelPoints[i].MapPoint.Y <= intersectionPoint.Y)
                        {
                            parallelPoints.RemoveAt(i);
                        }
                    }
                }
            }
            */

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

                for (int j = 0; j < curvePoints.Count - 2; j += 3)
                {
                    path.CubicTo(curvePoints[j].MapPoint, curvePoints[j + 1].MapPoint, curvePoints[j + 2].MapPoint);
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
