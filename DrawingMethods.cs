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
using AForge.Imaging;
using AForge.Imaging.Filters;
using Clipper2Lib;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;
using Blob = AForge.Imaging.Blob;

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

        internal static List<SKPoint> GetPointsInCircle(SKPoint cursorPoint, int radius, int stepSize)
        {
            if (radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), "Argument must be positive.");
            }

            List<SKPoint> pointsInCircle = [];

            int minX = (int)Math.Max(0, cursorPoint.X - radius);
            int maxX = (int)(cursorPoint.X + radius);
            int minY = (int)Math.Max(0, cursorPoint.Y - radius);
            int maxY = (int)(cursorPoint.Y + radius);

            for (int i = minX; i <= maxX; i += stepSize)
            {
                for (int j = minY; j <= maxY; j += stepSize)
                {
                    SKPoint p = new(i, j);
                    if (PointInCircle(radius, cursorPoint, p))
                    {
                        pointsInCircle.Add(p);
                    }
                }
            }

            return pointsInCircle;
        }

        public static float DistanceBetween(SKPoint from, SKPoint to)
        {
            return SKPoint.Distance(from, to);
        }

        public static bool PointInCircle(float radius, SKPoint origin, SKPoint pointToTest)
        {
            float square_dist = SKPoint.DistanceSquared(origin, pointToTest);
            return square_dist < (radius * radius);
        }

        public static float CalculateLineAngle(SKPoint start, SKPoint end)
        {
            float xDiff = end.X - start.X;
            float yDiff = end.Y - start.Y;

            return (float)((float)Math.Atan2(yDiff, xDiff) * D180_OVER_PI);
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

        public static Bitmap MakeGrayscale(Bitmap original, float threshold, bool makeMonochrome)
        {
            //create a blank bitmap the same size as original
            Bitmap grayScaleBitmap = new(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(grayScaleBitmap))
            {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                    [
                        [0.299f, 0.299f, 0.299f, 0, 0],
                        [0.587f, 0.587f, 0.587f, 0, 0],
                        [0.114f, 0.114f, 0.114f, 0, 0],
                        [0,      0,      0,      1, 0],
                        [0,      0,      0,      0, 1]
                    ]);

                //create some image attributes
                using ImageAttributes attributes = new ImageAttributes();

                if (makeMonochrome)
                {
                    // set threshold to convert grayscale to monochrome
                    attributes.SetThreshold(threshold);
                }

                //set the color matrix attribute so colors from light gray to white are set to transparent
                attributes.SetColorMatrix(colorMatrix);

                Color lowerColor = Color.LightGray;
                Color upperColor = Color.White;

                attributes.SetColorKey(lowerColor, upperColor, ColorAdjustType.Default);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return grayScaleBitmap;
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

        internal static SKBitmap ScaleBitmap(SKBitmap bitmap, float scale)
        {
            int bitmapWidth = (int)Math.Round(bitmap.Width * scale);
            int bitmapHeight = (int)Math.Round(bitmap.Height * scale);

            SKBitmap resizedSKBitmap = new(bitmapWidth, bitmapHeight);
            bitmap.ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

            return resizedSKBitmap;
        }

        internal static SKBitmap RotateBitmap(SKBitmap bmp, float angle, bool flipX)
        {
            Bitmap bitmap = Extensions.ToBitmap(bmp);
            double radianAngle = angle / 180.0 * Math.PI;
            double cosA = Math.Abs(Math.Cos(radianAngle));
            double sinA = Math.Abs(Math.Sin(radianAngle));

            int newWidth = (int)Math.Ceiling(cosA * bitmap.Width + sinA * bitmap.Height);
            int newHeight = (int)Math.Ceiling(sinA * bitmap.Width + cosA * bitmap.Height);

            Bitmap rotatedBitmap = new(newWidth, newHeight);
            rotatedBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using Graphics g = Graphics.FromImage(rotatedBitmap);
            g.TranslateTransform((float)(newWidth - bitmap.Width) / 2, (float)(newHeight - bitmap.Height) / 2);
            g.TranslateTransform(bitmap.Width / 2, bitmap.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-bitmap.Width / 2, -bitmap.Height / 2);
            g.DrawImage(bitmap, new Point(0, 0));

            if (flipX)
            {
                rotatedBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }

            return Extensions.ToSKBitmap(rotatedBitmap);
        }

        internal static Bitmap? ExtractLargestBlob(Bitmap b)
        {
            if (b.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // convert the bitmap to an 8bpp grayscale image for processing
                b = Grayscale.CommonAlgorithms.BT709.Apply(b);
            }

            // invert the bitmap colors white -> black; black -> white
            Invert invert = new();
            using Bitmap invertedBitmap = invert.Apply(b);

            // extract the largest blob; this will be the landform to be created
            // create an instance of blob counter algorithm
            BlobCounterBase bc = new BlobCounter
            {
                // set filtering options
                FilterBlobs = true,
                MinWidth = 5,
                MinHeight = 5,

                // set ordering options
                ObjectsOrder = ObjectsOrder.Size
            };

            // process binary image
            bc.ProcessImage(invertedBitmap);

            //bc.ProcessImage(b);

            Blob[] blobs = bc.GetObjectsInformation();

            // extract the biggest blob
            if (blobs.Length > 0)
            {
                bc.ExtractBlobsImage(b, blobs[0], true);

                Blob biggestBlob = blobs[0];
                Bitmap managedImage = biggestBlob.Image.ToManagedImage();

                // re-invert the colors
                Bitmap invertedBlobBitmap = invert.Apply(managedImage);

                //return managedImage;
                return invertedBlobBitmap;
            }

            return null;
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
