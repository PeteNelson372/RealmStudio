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
* The text of the GNU General Public License (GPL) is found in the LICENSE file.
* If the LICENSE file is not present or the text of the GNU GPL is not present in the LICENSE file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* contact@brookmonte.com
*
***************************************************************************************************************************/
using Clipper2Lib;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal class DrawingMethods
    {
        const float PI_OVER_180 = (float)Math.PI / 180F;
        const float D180_OVER_PI = (float)((float)180.0F / Math.PI);


        public static SKPoint[] GetPoints(uint quantity, SKPoint p1, SKPoint p2)
        {
            var points = new SKPoint[quantity];

            double distance = DistanceBetween(p1, p2);

            points[0] = p1;
            points[quantity - 1] = p2;

            double tdelta = distance / quantity;

            for (int i = 1; i < quantity; i++)
            {
                //t = dt / d
                //(xt, yt) = (((1 - t) * x0 + t * x1), ((1 - t) * y0 + t * y1))
                double t = (i * tdelta) / distance;
                points[i].X = (int)Math.Ceiling((1 - t) * p1.X + t * p2.X);
                points[i].Y = (int)Math.Ceiling((1 - t) * p1.Y + t * p2.Y);
            }

            return points;
        }

        public static SKPoint PointOnCircle(float radius, float angleInDegrees, SKPoint origin)
        {
            float angleInRadians = angleInDegrees * PI_OVER_180;

            // Convert from degrees to radians via multiplication by PI/180        
            float x = (float)(radius * Math.Cos(angleInRadians)) + origin.X;
            float y = (float)(radius * Math.Sin(angleInRadians)) + origin.Y;

            return new SKPoint(x, y);
        }

        public static float DistanceBetween(SKPoint from, SKPoint to)
        {
            return SKPoint.Distance(from, to);
        }

        public static bool IsPaintableImage(Bitmap bitmap)
        {
            bool isPaintableImage = true;

            if (bitmap != null)
            {
                var lockedBitmap = new LockBitmap(bitmap);
                lockedBitmap.LockBits();

                for (int y = 0; y < lockedBitmap.Height; y++)
                {
                    for (int x = 0; x < lockedBitmap.Width; x++)
                    {
                        Color pixelColor = lockedBitmap.GetPixel(x, y);

                        byte rValue = pixelColor.R;
                        byte gValue = pixelColor.G;
                        byte bValue = pixelColor.B;

                        if (rValue > 64 && (gValue != 0 || bValue != 0))
                        {
                            continue;
                        }
                        else if (gValue > 64 && rValue == 0 && bValue == 0)
                        {
                            continue;
                        }
                        else if (bValue > 64 && rValue == 3 && gValue == 3)
                        {
                            continue;
                        }
                        else
                        {
                            isPaintableImage = false;
                            break;
                        }
                    }
                }

                lockedBitmap.UnlockBits();
            }

            return isPaintableImage;
        }

        public static bool IsGrayScaleImage(Bitmap bitmap)
        {
            bool IsGrayScaleImage = true;

            if (bitmap != null)
            {
                var lockedBitmap = new LockBitmap(bitmap);
                lockedBitmap.LockBits();

                for (int y = 0; y < lockedBitmap.Height; y++)
                {
                    for (int x = 0; x < lockedBitmap.Width; x++)
                    {
                        Color pixelColor = lockedBitmap.GetPixel(x, y);

                        if (pixelColor.R != pixelColor.G || pixelColor.G != pixelColor.B || pixelColor.R != pixelColor.B)
                        {
                            IsGrayScaleImage = false;
                            break;
                        }
                    }
                }

                lockedBitmap.UnlockBits();
            }

            return IsGrayScaleImage;
        }

        internal static SKPath GetInnerOrOuterPath(List<SKPoint> pathPoints, float distance, ParallelEnum location)
        {
            List<SKPoint> newPoints = GetParallelPoints(pathPoints, distance, location);
            SKPath newPath = new();

            if (newPoints.Count > 0)
            {
                newPath.MoveTo(newPoints[0]);

                for (int i = 1; i < newPoints.Count; i++)
                {
                    if (newPoints[i] != SKPoint.Empty)
                    {
                        newPath.LineTo(newPoints[i]);
                    }
                }

                newPath.Close();
            }

            return newPath;
        }

        internal static List<SKPoint> GetParallelPoints(List<SKPoint> points, double distance, ParallelEnum location)
        {
            if (points.Count == 0) return points;

            PathD clipperPath = [];

            foreach (SKPoint point in points)
            {
                clipperPath.Add(new PointD(point.X, point.Y));
            }

            SKPoint firstPoint = points.First();
            clipperPath.Add(new PointD(firstPoint.X, firstPoint.Y));

            PathsD clipperPaths = [];
            PathsD inflatedPaths = [];

            clipperPaths.Add(clipperPath);

            double d = distance;

            if (location == ParallelEnum.Below)
            {
                d = distance * -1.0;
            }

            // offset polyline
            inflatedPaths = Clipper.InflatePaths(clipperPaths, d, JoinType.Round, EndType.Polygon);

            if (inflatedPaths.Count > 0)
            {
                PathD inflatedPathD = inflatedPaths.First();

                List<SKPoint> inflatedPath = [];

                foreach (PointD p in inflatedPathD)
                {
                    inflatedPath.Add(new SKPoint((float)p.x, (float)p.y));
                }

                return inflatedPath;
            }
            else
            {
                return points;
            }
        }

        internal static SKPath GetContourPathFromPath(SKPath path, int width, int height, out List<SKPoint> contourPoints)
        {
            // create a black bitmap from the path with background pixels set to Color.White
            using SKBitmap contourBitmap = new(width, height);

            using SKCanvas canvas = new(contourBitmap);

            canvas.Clear();
            canvas.DrawPath(path, PaintObjects.ContourPathPaint);

            // make sure the bitmap has a 2-pixel wide margin of empty pixels
            // so that the contour points can be found
            using SKPath marginPath = new();
            marginPath.MoveTo(1, 1);
            marginPath.LineTo(width - 1, 1);
            marginPath.LineTo(width - 1, height - 1);
            marginPath.LineTo(1, height - 1);
            marginPath.Close();

            canvas.DrawPath(marginPath, PaintObjects.ContourMarginPaint);

            contourPoints = GetBitmapContourPoints(Extensions.ToBitmap(contourBitmap));

            SKPath contourPath = new();

            if (contourPoints.Count > 2)
            {
                // the Moore-Neighbor algorithm sets the first (0th) pixel in the list of contour points to
                // an empty pixel, so remove it before constructing the path from the contour points
                contourPoints.RemoveAt(0);

                contourPath.MoveTo(contourPoints[0]);

                for (int i = 1; i < contourPoints.Count; i++)
                {
                    contourPath.LineTo(contourPoints[i]);
                }

                contourPath.Close();
            }

            return contourPath;
        }

        internal static List<SKPoint> GetBitmapContourPoints(Bitmap bitmap)
        {
            List<SKPoint> contourPoints = [];

            if (bitmap != null)
            {
                try
                {
                    var lockedBitmap = new LockBitmap(bitmap);
                    lockedBitmap.LockBits();

                    contourPoints = MooreNeighborTraceContour(lockedBitmap);

                    lockedBitmap.UnlockBits();
                }
                catch (Exception e)
                {
                    Program.LOGGER.Error(e);
                }
            }

            return contourPoints;
        }

        private static SKPoint? FindStartingPoint(LockBitmap lockedBitmap, int width, int height)
        {
            // Iterate over the pixels in the image to find a starting point on the boundary of the object.
            // this implementation works from left-to-right, bottom-to-top
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = height - 2; j > 0; j--)
                {
                    // If the current pixel is on the boundary of the object, return it.
                    if (lockedBitmap.GetPixel(i, j).ToArgb() == Color.Black.ToArgb()
                        && (lockedBitmap.GetPixel(i - 1, j).ToArgb() == Color.Empty.ToArgb()
                        || lockedBitmap.GetPixel(i + 1, j).ToArgb() == Color.Empty.ToArgb()
                        || lockedBitmap.GetPixel(i, j - 1).ToArgb() == Color.Empty.ToArgb()
                        || lockedBitmap.GetPixel(i, j + 1).ToArgb() == Color.Empty.ToArgb()
                        || lockedBitmap.GetPixel(i - 1, j).ToArgb() == Color.White.ToArgb()
                        || lockedBitmap.GetPixel(i + 1, j).ToArgb() == Color.White.ToArgb()
                        || lockedBitmap.GetPixel(i, j - 1).ToArgb() == Color.White.ToArgb()
                        || lockedBitmap.GetPixel(i, j + 1).ToArgb() == Color.White.ToArgb()))
                    {
                        return new SKPoint(i, j);
                    }
                }
            }
            // If no starting point is found, return null.
            return null;
        }

        private static List<SKPoint> MooreNeighborTraceContour(LockBitmap image)
        {
            // see https://www.imageprocessingplace.com/downloads_V3/root_downloads/tutorials/contour_tracing_Abeer_George_Ghuneim/moore.html
            // and https://en.wikipedia.org/wiki/Moore_neighborhood
            // for a description of how this algorithm works

            List<SKPoint> contour = [];  // B
            contour.Add(SKPoint.Empty);

            SKPoint? sp = FindStartingPoint(image, image.Width, image.Height);

            if (sp == null)
            {
                return contour;
            }

            SKPoint startPoint = (SKPoint)sp;  // s

            contour.Add(startPoint);

            SKPoint contourPoint = new(startPoint.X, startPoint.Y);  // p

            SKPoint backPoint = new(startPoint.X, startPoint.Y + 1);  // b

            SKPoint checkPoint = new(backPoint.X - 1, backPoint.Y);  // c

            // the indexes of the Moore neighborhood cells around the point being checked
            // 1 2 3
            // 0 X 4
            // 7 6 5

            // since the first point in the Moore neighborhood around the startPoint to be checked
            // is below and to the left of the starting point, the checkStartIndex begins at cell 7

            int checkStartIndex = 7;

            // foundIndex is updated by the CheckMooreNeighborhood method;
            // when the CheckMooreNeighborhood method finds a black contour (boundary) pixel
            // the foundIndex is updated to point to the cell *previously* checked (the empty pixel checked before the black pixel);
            // that cell becomes the first one checked when looking for the next black contour pixel
            int foundIndex;

            while (!CheckExit(contour, contourPoint))
            {
                if (image.GetPixel((int)checkPoint.X, (int)checkPoint.Y).ToArgb() == Color.Black.ToArgb())
                {
                    contour.Add(checkPoint);

                    backPoint.X = contourPoint.X;
                    backPoint.Y = contourPoint.Y;

                    contourPoint.X = checkPoint.X;
                    contourPoint.Y = checkPoint.Y;

                    SKPoint newCheckPoint = CheckMooreNeighborhood(image, checkPoint, checkStartIndex, out foundIndex);

                    if (newCheckPoint != SKPoint.Empty)
                    {
                        checkPoint.X = newCheckPoint.X;
                        checkPoint.Y = newCheckPoint.Y;

                        // update the checkStartIndex to the value of the foundIndex
                        checkStartIndex = foundIndex;
                    }
                    else
                    {
                        // error condition
                        throw new Exception("An error occurred while tracing contour: E1.");
                    }
                }
                else
                {
                    backPoint.X = contourPoint.X;
                    backPoint.Y = contourPoint.Y;

                    SKPoint newCheckPoint = CheckMooreNeighborhood(image, checkPoint, checkStartIndex, out foundIndex);

                    if (newCheckPoint != SKPoint.Empty)
                    {
                        checkPoint.X = newCheckPoint.X;
                        checkPoint.Y = newCheckPoint.Y;

                        // update the checkStartIndex to the value of the foundIndex
                        checkStartIndex = foundIndex;
                    }
                    else
                    {
                        // error condition
                        throw new Exception("An error occurred while tracing contour: E2.");
                    }
                }
            }

            return contour;
        }

        private static bool CheckExit(List<SKPoint> contour, SKPoint contourPoint)
        {
            // the CheckExit method looks at the list of contour points
            // if the startPoint appears in the list of contour points at least twice and
            // if the point before the startPoint in the list is the same, then the entire contour
            // has been found
            // this stopping method is described at
            // https://www.imageprocessingplace.com/downloads_V3/root_downloads/tutorials/contour_tracing_Abeer_George_Ghuneim/ray.html
            // as a method for stopping the Radial Sweep contour tracing algorithm; it has been adapted here
            // for use with the Moore-Neighbor algorithm

            bool boundaryComplete = false;
            List<int> foundIndexes = [];

            if (contour.Contains(contourPoint) && contour.Count > 2)
            {
                for (int i = 0; i < contour.Count; i++)
                {
                    if (contour[i].X == contourPoint.X && contour[i].Y == contourPoint.Y)
                    {
                        foundIndexes.Add(i);
                    }
                }

                if (foundIndexes.Count == 2)
                {
                    if (contour[foundIndexes[0]].X == contour[foundIndexes[1]].X && contour[foundIndexes[0]].Y == contour[foundIndexes[1]].Y)
                    {
                        if (contour[foundIndexes[0] - 1].X == contour[foundIndexes[1] - 1].X && contour[foundIndexes[0] - 1].Y == contour[foundIndexes[1] - 1].Y)
                        {
                            boundaryComplete = true;
                        }
                    }
                }
            }

            return boundaryComplete;
        }

        private static readonly Point[] mooreNeighborhoodCheckTable =
        [
            new Point(-1, 0),
            new Point(-1, -1),
            new Point(0, -1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1,1),
            new Point(0, 1),
            new Point(-1, 1)
        ];

        private static SKPoint CheckMooreNeighborhood(LockBitmap image, SKPoint p, int startIndex, out int foundIndex)
        {
            foundIndex = 0;
            // move around the given point clockwise from the starting index, looking for a black pixel
            for (int i = 0; i < mooreNeighborhoodCheckTable.Length; i++)
            {
                // make sure that as the Moore neighborhood is checked, the index into the mooreNeighborhoodCheckTable is in range (0 - 7)
                int tableIndex = (i + startIndex + mooreNeighborhoodCheckTable.Length) % mooreNeighborhoodCheckTable.Length;

                int px = (int)(p.X + mooreNeighborhoodCheckTable[tableIndex].X);
                int py = (int)(p.Y + mooreNeighborhoodCheckTable[tableIndex].Y);

                if (px < 0 || px >= image.Width || py < 0 || py >= image.Height)
                {
                    break;
                }

                if (image.GetPixel(px, py).ToArgb() == Color.Black.ToArgb())
                {
                    foundIndex = tableIndex - 1;
                    return new SKPoint(px, py);
                }
            }

            foundIndex = -1;
            return SKPoint.Empty;
        }
    }
}
