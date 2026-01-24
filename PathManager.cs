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
    internal sealed class PathManager : IMapComponentManager
    {
        private static PathUIMediator? _pathMediator;

        internal static PathUIMediator? PathMediator
        {
            get { return _pathMediator; }
            set { _pathMediator = value; }
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? Create()
        {
            ArgumentNullException.ThrowIfNull(PathMediator);

            // initialize map path
            MapPath? newPath = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
                PathType = PathMediator.PathType,
                PathColor = PathMediator.PathColor,
                PathWidth = PathMediator.PathWidth,
                DrawOverSymbols = PathMediator.DrawOverSymbols,
                PathTexture = PathMediator.PathTextureList[PathMediator.PathTextureIndex],
                PathTextureOpacity = PathMediator.PathTextureOpacity,
                PathTextureScale = PathMediator.PathTextureScale,
                PathTowerDistance = PathMediator.PathTowerDistance,
                PathTowerSize = PathMediator.PathTowerSize,
            };

            ConstructPathPaint(newPath);
            newPath.PathPoints.Add(new MapPathPoint(MapStateMediator.CurrentCursorPoint));

            return newPath;
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(PathMediator);

            if (MapStateMediator.SelectedMapPath != null)
            {
                MapStateMediator.SelectedMapPath.PathType = PathMediator.PathType;
                MapStateMediator.SelectedMapPath.PathColor = PathMediator.PathColor;
                MapStateMediator.SelectedMapPath.PathWidth = PathMediator.PathWidth;
                MapStateMediator.SelectedMapPath.DrawOverSymbols = PathMediator.DrawOverSymbols;
                MapStateMediator.SelectedMapPath.PathTexture = PathMediator.PathTextureList[PathMediator.PathTextureIndex];
                MapStateMediator.SelectedMapPath.PathTextureOpacity = PathMediator.PathTextureOpacity;
                MapStateMediator.SelectedMapPath.PathTextureScale = PathMediator.PathTextureScale;
                MapStateMediator.SelectedMapPath.PathTowerDistance = PathMediator.PathTowerDistance;
                MapStateMediator.SelectedMapPath.PathTowerSize = PathMediator.PathTowerSize;

                ConstructPathPaint(MapStateMediator.SelectedMapPath);
            }

            return true;
        }

        public static bool Delete()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.SelectedMapPath != null)
            {
                Cmd_RemoveMapPath cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapPath);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                MapStateMediator.SelectedMapPath = null;
                MapStateMediator.SelectedMapPathPoint = null;

                return true;
            }

            return false;
        }

        public static void ConstructPathPaint(MapPath mapPath)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (mapPath.PathPaint != null) return;

            float strokeWidth = mapPath.PathWidth;

            switch (mapPath.PathType)
            {
                case PathType.ThickSolidLinePath:
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

            // load the texture bitmap if needed
            if (mapPath.PathTexture != null && !string.IsNullOrEmpty(mapPath.PathTexture.TexturePath) && mapPath.PathTexture.TextureBitmap == null)
            {
                mapPath.PathTexture.TextureBitmap = (Bitmap?)Bitmap.FromFile(mapPath.PathTexture.TexturePath);
            }

            SKPathEffect? pathLineEffect = ConstructPathLineEffect(mapPath.PathType, mapPath.PathWidth * 2);

            if (pathLineEffect != null)
            {
                pathPaint.PathEffect = pathLineEffect;
            }

            switch (mapPath.PathType)
            {
                case PathType.LineAndDashesPath:
                case PathType.BorderedGradientPath:
                case PathType.BorderedLightSolidPath:
                    pathPaint.StrokeCap = SKStrokeCap.Butt;
                    break;
                case PathType.TexturedPath:
                    if (mapPath.PathTexture != null && mapPath.PathTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(mapPath.PathTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * mapPath.PathTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * mapPath.PathTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, mapPath.PathTextureOpacity / 255.0F);

                        // construct a shader from the selected path texture
                        pathPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathType.BorderAndTexturePath:
                    pathPaint.StrokeCap = SKStrokeCap.Round;

                    if (mapPath.PathTexture != null && mapPath.PathTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(mapPath.PathTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * mapPath.PathTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * mapPath.PathTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, mapPath.PathTextureOpacity / 255.0F);

                        // construct a shader from the selected path texture
                        pathPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathType.RoundTowerWall:
                    pathPaint.StrokeCap = SKStrokeCap.Round;

                    if (mapPath.PathTexture != null && mapPath.PathTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(mapPath.PathTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * mapPath.PathTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * mapPath.PathTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, mapPath.PathTextureOpacity / 255.0F);

                        // construct a shader from the selected path texture
                        pathPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
                case PathType.SquareTowerWall:
                    pathPaint.StrokeCap = SKStrokeCap.Round;

                    if (mapPath.PathTexture != null && mapPath.PathTexture.TextureBitmap != null)
                    {
                        // scale and set opacity of the texture
                        // resize the bitmap, but maintain aspect ratio
                        using Bitmap resizedBitmap = DrawingMethods.ScaleBitmap(mapPath.PathTexture.TextureBitmap,
                            (int)(MapStateMediator.CurrentMap.MapWidth * mapPath.PathTextureScale), (int)(MapStateMediator.CurrentMap.MapHeight * mapPath.PathTextureScale));

                        using Bitmap b = DrawingMethods.SetBitmapOpacity(resizedBitmap, mapPath.PathTextureOpacity / 255.0F);

                        // construct a shader from the selected path texture
                        pathPaint.Shader = SKShader.CreateBitmap(b.ToSKBitmap(), SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    }

                    pathPaint.Style = SKPaintStyle.Stroke;
                    break;
            }

            mapPath.PathPaint = pathPaint;
        }

        private static SKPathEffect? ConstructPathLineEffect(PathType pathType, float pathWidth)
        {
            SKPathEffect? pathLineEffect = null;

            switch (pathType)
            {
                case PathType.DottedLinePath:
                    float[] intervals = [0, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.DashedLinePath:
                    intervals = [pathWidth, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.DashDotLinePath:
                    intervals = [pathWidth, pathWidth, 0, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.DashDotDotLinePath:
                    intervals = [pathWidth, pathWidth, 0, pathWidth, 0, pathWidth];
                    pathLineEffect = SKPathEffect.CreateDash(intervals, 0);
                    break;
                case PathType.ChevronLinePath:
                    string svgPath = "M 0 0"
                        + " L" + pathWidth.ToString() + " 0"
                        + " L" + (pathWidth * 1.5F).ToString() + " " + (pathWidth / 2.0F).ToString()
                        + " L" + pathWidth.ToString() + " " + pathWidth.ToString()
                        + " L0 " + pathWidth.ToString()
                        + " L" + (pathWidth / 2.0F).ToString() + " " + (pathWidth / 2.0F).ToString()
                        + " L0 0"
                        + " Z";

                    SKPath chevronPath = SKPath.ParseSvgPathData(svgPath);
                    chevronPath.Transform(SKMatrix.CreateScale(0.5F, 0.5F));

                    pathLineEffect = SKPathEffect.Create1DPath(chevronPath, pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    break;
                case PathType.BearTracksPath:
                    SKPath? bearTrackPath = GetPathFromSvg("Bear Tracks", pathWidth);

                    if (bearTrackPath != null)
                    {
                        pathLineEffect = SKPathEffect.Create1DPath(bearTrackPath,
                            pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    }

                    break;
                case PathType.BirdTracksPath:
                    SKPath? birdTrackPath = GetPathFromSvg("Bird Tracks", pathWidth);

                    if (birdTrackPath != null)
                    {
                        pathLineEffect = SKPathEffect.Create1DPath(birdTrackPath,
                            pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    }
                    break;
                case PathType.FootprintsPath:
                    SKPath? footprintsPath = GetPathFromSvg("Foot Prints", pathWidth);

                    if (footprintsPath != null)
                    {
                        pathLineEffect = SKPathEffect.Create1DPath(footprintsPath,
                            pathWidth, 0, SKPath1DPathEffectStyle.Rotate);
                    }
                    break;
            }

            return pathLineEffect;
        }

        private static SKPath? GetPathFromSvg(string vectorName, float pathWidth)
        {
            ArgumentNullException.ThrowIfNull(PathMediator);

            MapVector? pathVector = null;

            for (int i = 0; i < PathMediator.PathVectorList.Count; i++)
            {
                if (PathMediator.PathVectorList[i].VectorName == vectorName)
                {
                    pathVector = PathMediator.PathVectorList[i];
                    break;
                }
            }

            if (pathVector != null && pathVector.VectorSkPath != null)
            {
                pathVector.ScaledPath = new(pathVector.VectorSkPath);

                float xSize = pathWidth;
                float ySize = pathWidth;

                float xDelta = 0.0F;
                float yDelta = 0.0F;

                if (pathVector.ViewBoxSizeWidth != 0 && pathVector.ViewBoxSizeHeight != 0)
                {
                    xSize = pathVector.ViewBoxSizeWidth;
                    ySize = pathVector.ViewBoxSizeHeight;
                }

                float xScale = pathWidth / xSize;
                float yScale = pathWidth / ySize;


                if (pathVector.ViewBoxLeft != 0)
                {
                    xDelta = -pathVector.ViewBoxLeft * xScale;
                }

                if (pathVector.ViewBoxTop != 0)
                {
                    yDelta = -pathVector.ViewBoxTop * yScale;
                }

                pathVector.ScaledPath.Transform(SKMatrix.CreateScaleTranslation(xScale, yScale, xDelta, yDelta));
                return pathVector.ScaledPath;
            }

            return null;
        }

        internal static List<MapPathPoint> GetParallelPathPoints(List<MapPathPoint> points, float distance, ParallelDirection location)
        {
            List<MapPathPoint> parallelPoints = [];

            //float d = (location == ParallelDirection.Below) ? distance : -distance;
            float offsetAngle = (location == ParallelDirection.Above) ? 90 : -90;

            SKPoint maxXPoint = new(-1.0F, 0);
            SKPoint maxYPoint = new(0, -1.0F);

            SKPoint minXPoint = new(65535.0F, 0);
            SKPoint minYPoint = new(0, 65535.0F);

            for (int i = 0; i < points.Count - 1; i++)
            {
                SKPoint newPoint;

                if (i == 0)
                {
                    float lineAngle = DrawingMethods.CalculateAngleBetweenPoints(points[i].MapPoint, points[i + 1].MapPoint);
                    float circleRadius = (float)Math.Sqrt(distance * distance + distance * distance);
                    newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle + offsetAngle, points[i].MapPoint);
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
                    float lineAngle1 = DrawingMethods.CalculateAngleBetweenPoints(points[i - 1].MapPoint, points[i].MapPoint);
                    float lineAngle2 = DrawingMethods.CalculateAngleBetweenPoints(points[i].MapPoint, points[i + 1].MapPoint);

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
                        newPoint = DrawingMethods.PointOnCircle(circleRadius, lineAngle1 + offsetAngle, points[i + 1].MapPoint);
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
            // the path has a sharp turn in it; this is done by finding the
            // point where the lines in the path intersect, then removing
            // the points that are outside the lines (X value is greater
            // than the intersection point X value and Y value is less than
            // the intersection point Y value)

            // this algorithm doesn't work consistently, so it is
            // commented out; need to figure out a reliable algorithm
            // for removing the loops, maybe by taking into account the
            // "direction" of the line?
            //
            // it may be that looking at whether the original path line (set
            // of points) is above/below or left/right of the parallel line
            // (set of points) will give the information needed to know
            // if loops need to be culled or not; however, how to know which
            // points should be culled still has to be determined

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
            if (curvePoints.Count > 2)
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

        internal static void DrawLineFromPoints(SKCanvas canvas, List<MapPathPoint> linePoints, SKPaint paint)
        {
            if (linePoints.Count > 0)
            {
                using SKPath path = new();
                path.MoveTo(linePoints[0].MapPoint);

                for (int j = 1; j < linePoints.Count - 1; j++)
                {
                    path.LineTo(linePoints[j].MapPoint);
                }

                canvas.DrawPath(path, paint);
            }
        }


        public static SKPath GenerateMapPathBoundaryPath(List<MapPathPoint> points)
        {
            SKPath path = new();

            if (points.Count < 3) return path;

            path.MoveTo(points[0].MapPoint);

            for (int j = 0; j < points.Count - 2; j += 3)
            {
                path.CubicTo(points[j].MapPoint, points[j + 1].MapPoint, points[j + 2].MapPoint);
            }

            path.GetBounds(out SKRect pathBounds);

            if (pathBounds.Width < 5.0F)
            {
                pathBounds.Inflate(5.0F, 0);
            }

            if (pathBounds.Height < 5.0F)
            {
                pathBounds.Inflate(0, 5.0F);
            }

            path.Reset();
            path.AddRect(pathBounds, SKPathDirection.Clockwise);

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

        public static void FinalizeMapPaths()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);

            for (int i = 0; i < pathLowerLayer.MapLayerComponents.Count; i++)
            {
                if (pathLowerLayer.MapLayerComponents[i] is MapPath mapPath)
                {
                    mapPath.ParentMap = MapStateMediator.CurrentMap;
                    ConstructPathPaint(mapPath);

                    if (mapPath.PathPoints.Count > 1)
                    {
                        SKPath path = GenerateMapPathBoundaryPath(mapPath.PathPoints);

                        if (path.PointCount > 0)
                        {
                            mapPath.BoundaryPath?.Dispose();
                            mapPath.BoundaryPath = new(path);
                            path.Dispose();
                        }
                        else
                        {
                            pathLowerLayer.MapLayerComponents.Remove(mapPath);
                        }
                    }
                    else
                    {
                        pathLowerLayer.MapLayerComponents.Remove(mapPath);
                    }
                }
            }

            for (int i = 0; i < pathUpperLayer.MapLayerComponents.Count; i++)
            {
                if (pathUpperLayer.MapLayerComponents[i] is MapPath mapPath)
                {
                    mapPath.ParentMap = MapStateMediator.CurrentMap;
                    ConstructPathPaint(mapPath);

                    if (mapPath.PathPoints.Count > 1)
                    {
                        SKPath path = GenerateMapPathBoundaryPath(mapPath.PathPoints);

                        if (path.PointCount > 0)
                        {
                            mapPath.BoundaryPath?.Dispose();
                            mapPath.BoundaryPath = new(path);
                            path.Dispose();
                        }
                        else
                        {
                            pathUpperLayer.MapLayerComponents.Remove(mapPath);
                        }
                    }
                    else
                    {
                        pathUpperLayer.MapLayerComponents.Remove(mapPath);
                    }
                }
            }
        }

        internal static MapPath? CreatePath(SKPoint zoomedScrolledPoint, PathType pathType, Color pathColor,
            int pathWidth, bool drawOverSymbols, MapTexture mapTexture)
        {

            // initialize map path
            MapPath? newPath = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
                PathType = pathType,
                PathColor = pathColor,
                PathWidth = pathWidth,
                DrawOverSymbols = drawOverSymbols,
            };

            mapTexture.TextureBitmap ??= (Bitmap?)Bitmap.FromFile(mapTexture.TexturePath);

            newPath.PathTexture = mapTexture;

            ConstructPathPaint(newPath);
            newPath.PathPoints.Add(new MapPathPoint(zoomedScrolledPoint));

            return newPath;
        }

        internal static void MovePath(MapPath selectedPath, SKPoint zoomedScrolledPoint, SKPoint previousCursorPoint)
        {
            selectedPath.BoundaryPath = GenerateMapPathBoundaryPath(selectedPath.PathPoints);
            selectedPath.BoundaryPath.GetTightBounds(out SKRect boundsRect);

            // move the entire selected path with the mouse
            SizeF delta = new()
            {
                Width = zoomedScrolledPoint.X - previousCursorPoint.X,
                Height = zoomedScrolledPoint.Y - previousCursorPoint.Y,
            };

            foreach (MapPathPoint point in selectedPath.PathPoints)
            {
                SKPoint p = point.MapPoint;
                p.X = p.X + delta.Width - (int)(boundsRect.MidX - previousCursorPoint.X);
                p.Y = p.Y + delta.Height - (int)(boundsRect.MidY - previousCursorPoint.Y);
                point.MapPoint = p;
            }

            selectedPath.BoundaryPath = GenerateMapPathBoundaryPath(selectedPath.PathPoints);
        }

        internal static float Get5DegreePathAngle(SKPoint mapPoint, SKPoint zoomedScrolledPoint)
        {
            float lineAngle = DrawingMethods.CalculateAngleBetweenPoints(mapPoint, zoomedScrolledPoint, true);

            lineAngle = (float)(Math.Round(lineAngle / 5, MidpointRounding.AwayFromZero) * 5);

            return lineAngle;
        }

        internal static SKPoint ForceHorizontalVerticalLine(SKPoint pathPoint, SKPoint firstPoint, float pathAngle)
        {
            // clamp the line to straight horizontal or straight vertical
            // by forcing the new point X or Y coordinate to be the
            // same as the first point of the path
            if (pathAngle >= 0 && pathAngle < 45)
            {
                pathPoint.Y = firstPoint.Y;
            }
            else if (pathAngle >= 45 && pathAngle < 135)
            {
                pathPoint.X = firstPoint.X;
            }
            else if (pathAngle >= 135 && pathAngle < 225)
            {
                pathPoint.Y = firstPoint.Y;
            }
            else if (pathAngle >= 225 && pathAngle < 315)
            {
                pathPoint.X = firstPoint.X;
            }
            else if (pathAngle >= 315 && pathAngle < 360)
            {
                pathPoint.Y = firstPoint.Y;
            }

            return pathPoint;
        }

        internal static void AddNewPathPoint(MapPath? mapPath, SKPoint newPathPoint)
        {
            // make the spacing between points consistent
            float spacing = 4.0F;

            if (mapPath != null)
            {
                if (mapPath.PathType == PathType.RailroadTracksPath)
                {
                    // railroad track points are further apart to make them look better
                    spacing = 2.0F;
                }

                if (SKPoint.Distance(mapPath.PathPoints.Last().MapPoint, newPathPoint) >= mapPath.PathWidth / spacing)
                {
                    mapPath.PathPoints.Add(new MapPathPoint(newPathPoint));
                }
            }
        }

        internal static SKPoint GetNewPathPoint(MapPath? mapPath, Keys modifierKeys, float selectedPathAngle, SKPoint zoomedScrolledPoint, int minimumPathPointCount)
        {
            SKPoint newPathPoint = zoomedScrolledPoint;
            MapPathPoint? firstPoint = mapPath?.PathPoints.First();

            if (modifierKeys == Keys.Shift && mapPath?.PathPoints.Count > minimumPathPointCount)
            {
                // draw straight path, clamped to 5 degree angles
                if (firstPoint != null)
                {
                    if (selectedPathAngle == -1)
                    {
                        selectedPathAngle = Get5DegreePathAngle(firstPoint.MapPoint, zoomedScrolledPoint);
                    }

                    float distance = SKPoint.Distance(firstPoint.MapPoint, zoomedScrolledPoint);
                    newPathPoint = DrawingMethods.PointOnCircle(distance, selectedPathAngle, firstPoint.MapPoint);
                }
            }
            else if (modifierKeys == Keys.Control)
            {
                // draw straight horizontal or vertical path
                if (firstPoint != null)
                {
                    if (selectedPathAngle == -1)
                    {
                        selectedPathAngle = DrawingMethods.CalculateAngleBetweenPoints(firstPoint.MapPoint, zoomedScrolledPoint, true); ;
                    }

                    // clamp the line to straight horizontal or straight vertical
                    // by forcing the new point X or Y coordinate to be the
                    // same as the first point of the path
                    newPathPoint = ForceHorizontalVerticalLine(newPathPoint, firstPoint.MapPoint, selectedPathAngle);
                }
            }

            return newPathPoint;
        }

        internal static MapPathPoint? GetSelectedPathPoint(MapPath? mapPath, SKPoint zoomedScrolledPoint)
        {
            if (mapPath == null) return null;

            foreach (MapPathPoint mp in mapPath.PathPoints)
            {
                mp.IsSelected = false;
            }

            return SelectMapPathPointAtPoint(mapPath, zoomedScrolledPoint);
        }

        internal static void RemovePathPoint()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.SelectedMapPath != null && MapStateMediator.SelectedMapPathPoint != null)
            {
                Cmd_RemovePathPoint cmd = new(MapStateMediator.SelectedMapPath, MapStateMediator.SelectedMapPathPoint);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                MapStateMediator.SelectedMapPathPoint = null;

                MapStateMediator.CurrentMap.IsSaved = false;
            }
        }
    }
}
