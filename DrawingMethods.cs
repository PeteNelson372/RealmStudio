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
using AForge.Imaging;
using AForge.Imaging.Filters;
using Clipper2Lib;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Blob = AForge.Imaging.Blob;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    internal sealed class DrawingMethods
    {
        private const float PI_OVER_180 = (float)Math.PI / 180F;
        private const float D180_OVER_PI = (float)((float)180.0F / Math.PI);
        private const double SELECTION_FUZZINESS = 4;

#pragma warning disable SYSLIB1054
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
#pragma warning restore SYSLIB1054

        private static readonly int[] dirmap = [1, 5, 4, 6, 2, 10, 8, 9];
        private static readonly int[] effmap = [0, 0x40000, 0x10, 0x80000];

        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr handle, int msec, int flags);

        public enum Effect { Roll, Slide, Center, Blend }

        public static List<SKPoint> PolyPoints(SKPoint location, float sides, float radius, float start)
        {
            List<SKPoint> points = [];

            float x_center = location.X;
            float y_center = location.Y;
            float angle = start;
            float angle_increment = (float)(2.0F * Math.PI / sides);

            for (int i = 0; i < sides; i++)
            {
                float x = (float)(x_center + radius * Math.Cos(angle));
                float y = (float)(y_center + radius * Math.Sin(angle));

                points.Add(new SKPoint(x, y));

                angle += angle_increment;
            }

            return points;
        }


        public static void Animate(Control ctl, Effect effect, int msec, int angle)
        {
            int flags = effmap[(int)effect];

            if (ctl.Visible)
            {
                flags |= 0x10000;
                angle += 180;
            }
            else
            {
                if (ctl.TopLevelControl == ctl)
                {
                    flags |= 0x20000;
                }
                else if (effect == Effect.Blend)
                {
                    throw new ArgumentException("Animation failed");
                }
            }

            flags |= dirmap[(angle % 360) / 45];
            bool ok = AnimateWindow(ctl.Handle, msec, flags);

            if (!ok)
            {
                throw new Exception("Animation failed");
            }

            ctl.Visible = !ctl.Visible;
        }



        public static byte[] ConvertBitmapSourceToByteArray(ImageSource imageSource)
        {
            var image = imageSource as BitmapSource;
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {

            BitmapData? bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        internal static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            Bitmap bitmap = new(outStream);

            return new Bitmap(bitmap);
        }

        internal static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        internal static Bitmap? BitmapSourceToBitmap(BitmapSource source)
        {
            if (source == null)
            {
                return null;
            }

            var pixelFormat = PixelFormat.Format32bppArgb;  //Bgr32 default
            Bitmap bmp = new(source.PixelWidth, source.PixelHeight, pixelFormat);

            BitmapData data = bmp.LockBits(
                new Rectangle(Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly,
                pixelFormat);

            source.CopyPixels(
                Int32Rect.Empty,
                data.Scan0,
                data.Height * data.Stride,
                data.Stride);

            bmp.UnlockBits(data);

            return bmp;
        }

        public static bool CompareBitmaps(Bitmap b1, Bitmap b2)
        {
            if (b1 == null || b2 == null) return false;

            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        public static Icon IconFromImage(System.Drawing.Image img)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            // Header
            bw.Write((short)0);   // 0 : reserved
            bw.Write((short)1);   // 2 : 1=ico, 2=cur
            bw.Write((short)1);   // 4 : number of images
                                  // Image directory
            var w = img.Width;
            if (w >= 256) w = 0;
            bw.Write((byte)w);    // 0 : width of image
            var h = img.Height;
            if (h >= 256) h = 0;
            bw.Write((byte)h);    // 1 : height of image
            bw.Write((byte)0);    // 2 : number of colors in palette
            bw.Write((byte)0);    // 3 : reserved
            bw.Write((short)0);   // 4 : number of color planes
            bw.Write((short)0);   // 6 : bits per pixel
            var sizeHere = ms.Position;
            bw.Write(0);     // 8 : image size
            var start = (int)ms.Position + 4;
            bw.Write(start);      // 12: offset of image data
                                  // Image data
            img.Save(ms, ImageFormat.Png);
            var imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, SeekOrigin.Begin);

            // And load it
            return new Icon(ms);
        }

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
            // Convert from degrees to radians via multiplication by PI/180
            float angleInRadians = angleInDegrees * PI_OVER_180;

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

        internal static bool LineContainsPoint(SKPoint pointToCheck, SKPoint lineStartPoint, SKPoint lineEndPoint)
        {
            SKPoint leftPoint;
            SKPoint rightPoint;

            // Normalize start/end to left right to make the offset calc simpler.
            if (lineStartPoint.X <= lineEndPoint.X)
            {
                leftPoint = lineStartPoint;
                rightPoint = lineEndPoint;
            }
            else
            {
                leftPoint = lineEndPoint;
                rightPoint = lineStartPoint;
            }

            // If point is out of bounds, no need to do further checks.                  
            if (pointToCheck.X + SELECTION_FUZZINESS < Math.Min(leftPoint.X, rightPoint.X) || Math.Max(leftPoint.X, rightPoint.X) < pointToCheck.X - SELECTION_FUZZINESS)
            {
                return false;
            }
            else if (pointToCheck.Y + SELECTION_FUZZINESS < Math.Min(leftPoint.Y, rightPoint.Y) || Math.Max(leftPoint.Y, rightPoint.Y) < pointToCheck.Y - SELECTION_FUZZINESS)
            {
                return false;
            }

            double deltaX = rightPoint.X - leftPoint.X;
            double deltaY = rightPoint.Y - leftPoint.Y;

            // If the line is straight, the earlier boundary check is enough to determine that the point is on the line.
            // Also prevents division by zero exceptions.
            if (deltaX == 0 || deltaY == 0)
            {
                return true;
            }

            double slope = deltaY / deltaX;
            double offset = leftPoint.Y - leftPoint.X * slope;
            double calculatedY = pointToCheck.X * slope + offset;

            // Check calculated Y matches the points Y coord with some easing.
            bool lineContains = pointToCheck.Y - SELECTION_FUZZINESS <= calculatedY && calculatedY <= pointToCheck.Y + SELECTION_FUZZINESS;

            return lineContains;
        }

        public static float CalculateAngleBetweenPoints(SKPoint start, SKPoint end, bool clampedTo360 = false)
        {
            float xDiff = end.X - start.X;
            float yDiff = end.Y - start.Y;

            float lineAngle = (float)((float)Math.Atan2(yDiff, xDiff) * D180_OVER_PI);

            if (clampedTo360)
            {
                lineAngle = (lineAngle < 0) ? lineAngle + 360 : lineAngle;
            }

            return lineAngle;
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

        internal static float CalculatePolygonArea(List<SKPoint> polygonPoints)
        {
            if (polygonPoints.Count < 3)
            {
                return 0;
            }

            // use shoelace algorithm to calculate area of the polygon
            float area = 0;

            int j = polygonPoints.Count - 1;
            for (int i = 0; i < polygonPoints.Count; i++)
            {
                area += (polygonPoints[j].X + polygonPoints[i].X) * (polygonPoints[j].Y - polygonPoints[i].Y);
                j = i;  // j is previous vertex to i
            }

            area = Math.Abs(area / 2.0F);

            return area;
        }

        internal static List<SKRectI> SubdivideRect2(SKRectI rect, int numDivisions, int minRectSize = 300)
        {
            List<SKRectI> rects = [];
            const int margin = 20;

            int tries = 0;

            if (rect.Left + margin >= rect.Right - minRectSize - margin || rect.Top + margin >= rect.Bottom - minRectSize - margin)
            {
                return rects;
            }

            while (rects.Count < numDivisions && tries < 100)
            {
                tries++;

                int left = Random.Shared.Next(rect.Left + margin, rect.Right - minRectSize - margin);
                int top = Random.Shared.Next(rect.Top + margin, rect.Bottom - minRectSize - margin);

                while (left + minRectSize > rect.Right - margin)
                {
                    left = Random.Shared.Next(rect.Left + margin, rect.Right - minRectSize - margin);
                }

                int right = Random.Shared.Next(left + minRectSize, rect.Right - margin);

                while (top + minRectSize > rect.Bottom - margin)
                {
                    top = Random.Shared.Next(rect.Top + margin, rect.Bottom - minRectSize - margin);
                }

                int bottom = Random.Shared.Next(top + minRectSize, rect.Bottom - margin);

                SKRectI newRect = new(left, top, right, bottom);

                if (newRect.Width > minRectSize && newRect.Height > minRectSize)
                {
                    bool overlapped = false;

                    foreach (SKRectI r in rects)
                    {
                        if (newRect.IntersectsWith(r))
                        {
                            overlapped = true; break;
                        }
                    }

                    if (!overlapped)
                    {
                        rects.Add(newRect);
                    }
                }
            }

            return rects;
        }

        internal static List<SKRectI> SubdivideRect(SKRectI rect, int numDivisions)
        {
            List<SKRectI> rects = [];

            SKRectI divideRect = rect;

            int splitCount = 0;

            while (splitCount < numDivisions)
            {
                List<SKRectI> splits = SplitRect(divideRect);

                if (splits.Count > 1)
                {
                    splitCount++;

                    int split0Size = splits[0].Width * splits[0].Height;
                    int split1Size = splits[1].Width * splits[1].Height;

                    if (split0Size > split1Size)
                    {
                        divideRect = splits[0];

                        if (splits[1].Width > 0 && splits[1].Height > 0)
                        {
                            rects.Add(splits[1]);
                        }
                    }
                    else
                    {
                        divideRect = splits[1];

                        if (splits[0].Width > 0 && splits[0].Height > 0)
                        {
                            rects.Add(splits[0]);
                        }
                    }
                }
                else if (splits.Count == 1)
                {
                    if (splits[0].Width > 0 && splits[0].Height > 0)
                    {
                        rects.Add(splits[0]);
                    }
                }
                else
                {
                    rects.Add(divideRect);
                }

            }

            return rects;
        }

        internal static List<SKRectI> SplitRect(SKRectI rect)
        {
            List<SKRectI> rects = [];

            SKRectI divideRect = rect;

            bool vertDivide = Random.Shared.Next(0, 2) == 1;

            int minSplitSize = 200;

            if (vertDivide)
            {
                if (rect.Width > minSplitSize * 2)
                {
                    int divideLoc = Random.Shared.Next(divideRect.Left + minSplitSize, divideRect.Right - minSplitSize);

                    divideLoc = Math.Max(divideLoc, minSplitSize);
                    divideLoc = Math.Min(divideLoc, rect.Right);

                    SKRectI rect1 = new(divideRect.Left, divideRect.Top, divideLoc, divideRect.Bottom);
                    SKRectI rect2 = new(divideLoc + 1, divideRect.Top, divideRect.Right, divideRect.Bottom);

                    rects.Add(rect1);
                    rects.Add(rect2);
                }
                else
                {
                    rects.Add(rect);
                }

            }
            else
            {
                if (rect.Height > minSplitSize * 2)
                {
                    int divideLoc = Random.Shared.Next(divideRect.Top + minSplitSize, divideRect.Bottom - minSplitSize);

                    divideLoc = Math.Max(divideLoc, minSplitSize);
                    divideLoc = Math.Min(divideLoc, rect.Bottom);

                    SKRectI rect1 = new(divideRect.Left, divideRect.Top, divideRect.Right, divideLoc);
                    SKRectI rect2 = new(divideRect.Left, divideLoc + 1, divideRect.Right, divideRect.Bottom);

                    rects.Add(rect1);
                    rects.Add(rect2);
                }
                else
                {
                    rects.Add(rect);
                }
            }

            return rects;
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
                ColorMatrix colorMatrix = new(
                    [
                        [0.299f, 0.299f, 0.299f, 0, 0],
                        [0.587f, 0.587f, 0.587f, 0, 0],
                        [0.114f, 0.114f, 0.114f, 0, 0],
                        [0,      0,      0,      1, 0],
                        [0,      0,      0,      0, 1]
                    ]);

                //create some image attributes
                using ImageAttributes attributes = new();

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

        public static void FlattenBitmapColors(ref Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }

            var lockedBitmap = new LockBitmap(bitmap);
            lockedBitmap.LockBits();

            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    if (lockedBitmap.GetPixel(x, y) == Color.Transparent
                        || ColorIsNear(lockedBitmap.GetPixel(x, y), Color.White, 64)
                        || lockedBitmap.GetPixel(x, y) == Color.White)
                    {
                        lockedBitmap.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        lockedBitmap.SetPixel(x, y, Color.Black);
                    }
                }
            }

            lockedBitmap.UnlockBits();
        }

        public static bool ColorIsNear(Color color, Color checkColor, int tolerance)
        {
            int rValue = color.R;
            int gValue = color.G;
            int bValue = color.B;

            int minR = checkColor.R - tolerance;
            int maxR = checkColor.R + tolerance;

            int minG = checkColor.G - tolerance;
            int maxG = checkColor.G + tolerance;

            int minB = checkColor.B - tolerance;
            int maxB = checkColor.B + tolerance;

            if (rValue < minR || rValue > maxR)
            {
                return false;
            }

            if (gValue < minG || gValue > maxG)
            {
                return false;
            }

            if (bValue < minB || bValue > maxB)
            {
                return false;
            }

            return true;

        }

        internal static Bitmap SetBitmapOpacity(Bitmap b, float opacity)
        {
            Bitmap newB = new(b.Width, b.Height, PixelFormat.Format32bppArgb);

            using Graphics g = Graphics.FromImage(newB);

            //create a color matrix object  
            ColorMatrix matrix = new()
            {
                //set the opacity  
                Matrix33 = opacity
            };

            //create image attributes  
            ImageAttributes attributes = new();

            //set the color(opacity) of the image  
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            //now draw the image
            g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, attributes);

            return newB;
        }

        internal static SKBitmap ResizeSKBitmap(SKBitmap bitmap, SKSizeI newsize)
        {
            SKBitmap resizedSKBitmap = new(newsize.Width, newsize.Height);
            bitmap.ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

            return resizedSKBitmap;
        }

        internal static Bitmap ScaleBitmap(Bitmap bmp, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / bmp.Width;
            var ratioY = (double)maxHeight / bmp.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(bmp.Width * ratio);
            var newHeight = (int)(bmp.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(bmp, 0, 0, newWidth, newHeight);

            return newImage;
        }

        internal static SKBitmap ScaleSKBitmap(SKBitmap bitmap, float scale)
        {
            int bitmapWidth = (int)Math.Round(bitmap.Width * scale);
            int bitmapHeight = (int)Math.Round(bitmap.Height * scale);

            SKBitmap resizedSKBitmap = new(bitmapWidth, bitmapHeight);
            bitmap.ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

            return resizedSKBitmap;
        }

        internal static SKBitmap RotateSKBitmap(SKBitmap bmp, float angle, bool flipX)
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

        internal static Bitmap FillHoles(Bitmap inputBitmap)
        {
            // convert the bitmap to an 8bpp grayscale image for processing
            if (inputBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // convert the bitmap to an 8bpp grayscale image for processing
                Bitmap newB = Grayscale.CommonAlgorithms.BT709.Apply(inputBitmap);
                inputBitmap = newB;
            }

            // invert the bitmap colors white -> black; black -> white
            // FillHoles filter looks for black holes in white background
            // inputBitmap has white holes in black background, so
            // invert is required
            Invert invert = new();
            Bitmap invertedBitmap = invert.Apply(inputBitmap);

            Mean meanFilter = new();
            meanFilter.ApplyInPlace(invertedBitmap);
            meanFilter.ApplyInPlace(invertedBitmap);

            FlattenBitmapColors(ref invertedBitmap);

            // fill holes in the input bitmap (black areas surrounded by white)
            FillHoles fillHolesFilter = new()
            {
                //MaxHoleWidth = inputBitmap.Width / 8,     // 1/8 size of bitmap is maximum size of hole filled
                //MaxHoleHeight = inputBitmap.Height / 8,
                CoupledSizeFiltering = false
            };

            Bitmap filledBitmap = fillHolesFilter.Apply(invertedBitmap);

            invertedBitmap.Dispose();

            // re-invert the colors to restore to original
            Bitmap filledInvertedBitmap = invert.Apply(filledBitmap);

            Bitmap newfilledInvertedBitmap = new(filledInvertedBitmap, filledInvertedBitmap.Width, filledInvertedBitmap.Height);

            using Graphics g = Graphics.FromImage(newfilledInvertedBitmap);
            using Pen p = new(Color.White, 3);

            g.DrawLine(p, new Point(0, 0), new Point(newfilledInvertedBitmap.Width, 0));
            g.DrawLine(p, new Point(0, newfilledInvertedBitmap.Height), new Point(newfilledInvertedBitmap.Width, newfilledInvertedBitmap.Height));
            g.DrawLine(p, new Point(0, 0), new Point(0, newfilledInvertedBitmap.Height));
            g.DrawLine(p, new Point(newfilledInvertedBitmap.Width, 0), new Point(newfilledInvertedBitmap.Width, newfilledInvertedBitmap.Height));

            return newfilledInvertedBitmap;
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
                MinWidth = 100,
                MinHeight = 100,

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
                bc.ExtractBlobsImage(invertedBitmap, blobs[0], true);

                Blob biggestBlob = blobs[0];
                Bitmap managedImage = biggestBlob.Image.ToManagedImage();

                // re-invert the colors
                Bitmap invertedBlobBitmap = invert.Apply(managedImage);

                //return managedImage;
                return invertedBlobBitmap;
            }

            return null;
        }

        internal static List<Bitmap> ExtractBlobs(Bitmap b, Size minSize)
        {
            List<Bitmap> extractedBlobs = [];

            if (b.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                // convert the bitmap to an 8bpp grayscale image for processing
                b = Grayscale.CommonAlgorithms.BT709.Apply(b);
            }

            // invert the bitmap colors white -> black; black -> white
            Invert invert = new();
            using Bitmap invertedBitmap = invert.Apply(b);

            // extract blobs; these will be the landforms to be created
            // create an instance of blob counter algorithm
            BlobCounterBase bc = new BlobCounter()
            {
                BackgroundThreshold = Color.FromArgb(10, 10, 10),
                FilterBlobs = true,
                MinWidth = minSize.Width,
                MinHeight = minSize.Height,
                MaxWidth = b.Width,
                MaxHeight = b.Height,
                BlobsFilter = null,

                // set ordering options
                ObjectsOrder = ObjectsOrder.Size
            };

            // process binary image
            bc.ProcessImage(invertedBitmap);

            Blob[] blobs = bc.GetObjects(invertedBitmap, true);

            // extract the blobs
            if (blobs.Length > 0)
            {
                for (int i = 0; i < blobs.Length; i++)
                {
                    bc.ExtractBlobsImage(invertedBitmap, blobs[i], true);

                    Blob aBlob = blobs[i];
                    Bitmap managedImage = aBlob.Image.ToManagedImage();

                    // re-invert the colors
                    Bitmap invertedBlobBitmap = invert.Apply(managedImage);

                    invertedBlobBitmap = new(invertedBlobBitmap, invertedBlobBitmap.Width, invertedBlobBitmap.Height);

                    extractedBlobs.Add((Bitmap)invertedBlobBitmap.Clone());
                }

                return extractedBlobs;
            }

            return extractedBlobs;
        }


        internal static SKBitmap[] SliceNinePatchBitmap(SKBitmap resizedBitmap, SKRectI center)
        {
            SKBitmap[] slicedBitmaps = new SKBitmap[9];

            slicedBitmaps[0] = new SKBitmap(center.Left, center.Top);
            using SKCanvas canvas_A = new(slicedBitmaps[0]);
            SKRect src_A = new(0, 0, center.Left, center.Top);
            SKRect dst_A = new(0, 0, slicedBitmaps[0].Width, slicedBitmaps[0].Height);
            canvas_A.DrawBitmap(resizedBitmap, src_A, dst_A);

            slicedBitmaps[1] = new SKBitmap(center.Right - center.Left, center.Top);
            using SKCanvas canvas_B = new(slicedBitmaps[1]);
            SKRect src_B = new(center.Left, 0, center.Right, center.Top);
            SKRect dst_B = new(0, 0, slicedBitmaps[1].Width, slicedBitmaps[1].Height);
            canvas_B.DrawBitmap(resizedBitmap, src_B, dst_B);

            slicedBitmaps[2] = new SKBitmap(resizedBitmap.Width - center.Right, center.Top);
            using SKCanvas canvas_C = new(slicedBitmaps[2]);
            SKRect src_C = new(center.Right, 0, resizedBitmap.Width, center.Top);
            SKRect dst_C = new(0, 0, slicedBitmaps[2].Width, slicedBitmaps[2].Height);
            canvas_C.DrawBitmap(resizedBitmap, src_C, dst_C);

            slicedBitmaps[3] = new SKBitmap(center.Left, center.Height);
            using SKCanvas canvas_D = new(slicedBitmaps[3]);
            SKRect src_D = new(0, center.Top, center.Left, center.Bottom);
            SKRect dst_D = new(0, 0, slicedBitmaps[3].Width, slicedBitmaps[3].Height);
            canvas_D.DrawBitmap(resizedBitmap, src_D, dst_D);

            slicedBitmaps[4] = new SKBitmap(center.Width, center.Height);
            using SKCanvas canvas_E = new(slicedBitmaps[4]);
            SKRect src_E = new(center.Left, center.Top, center.Right, center.Bottom);
            SKRect dst_E = new(0, 0, slicedBitmaps[4].Width, slicedBitmaps[4].Height);
            canvas_E.DrawBitmap(resizedBitmap, src_E, dst_E);

            slicedBitmaps[5] = new SKBitmap(resizedBitmap.Width - center.Right, center.Height);
            using SKCanvas canvas_F = new(slicedBitmaps[5]);
            SKRect src_F = new(center.Right, center.Top, resizedBitmap.Width, center.Bottom);
            SKRect dst_F = new(0, 0, slicedBitmaps[5].Width, slicedBitmaps[5].Height);
            canvas_F.DrawBitmap(resizedBitmap, src_F, dst_F);

            slicedBitmaps[6] = new SKBitmap(center.Left, resizedBitmap.Height - center.Bottom);
            using SKCanvas canvas_G = new(slicedBitmaps[6]);
            SKRect src_G = new(0, center.Bottom, center.Left, resizedBitmap.Height);
            SKRect dst_G = new(0, 0, slicedBitmaps[6].Width, slicedBitmaps[6].Height);
            canvas_G.DrawBitmap(resizedBitmap, src_G, dst_G);

            slicedBitmaps[7] = new SKBitmap(center.Width, resizedBitmap.Height - center.Bottom);
            using SKCanvas canvas_H = new(slicedBitmaps[7]);
            SKRect src_H = new(center.Left, center.Bottom, center.Right, resizedBitmap.Height);
            SKRect dst_H = new(0, 0, slicedBitmaps[7].Width, slicedBitmaps[7].Height);
            canvas_H.DrawBitmap(resizedBitmap, src_H, dst_H);

            slicedBitmaps[8] = new SKBitmap(resizedBitmap.Width - center.Right, resizedBitmap.Height - center.Bottom);
            using SKCanvas canvas_I = new(slicedBitmaps[8]);
            SKRect src_I = new(center.Right, center.Bottom, resizedBitmap.Width, resizedBitmap.Height);
            SKRect dst_I = new(0, 0, slicedBitmaps[8].Width, slicedBitmaps[8].Height);
            canvas_I.DrawBitmap(resizedBitmap, src_I, dst_I);

            return slicedBitmaps;
        }

        internal static SKPath GetInnerOrOuterPath(List<SKPoint> pathPoints, float distance, ParallelDirection location)
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

        public static SKPath GetLinePathFromPoints(List<SKPoint> points)
        {
            SKPath path = new();

            path.MoveTo(points.First());

            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i]);
            }

            //path.LineTo(points.First());

            path.Close();

            return path;
        }

        internal static List<SKPoint> GetParallelPoints(List<SKPoint> points, double distance, ParallelDirection location)
        {
            if (points.Count == 0) return points;

            //int pointCount = points.Count;

            PathD clipperPath = new(points.Count);

            /*
            for (int i = 0; i < pointCount; i++)
            {
                clipperPath.Add(new PointD(points[i].X, points[i].Y));
            }
            */

            foreach (SKPoint p in CollectionsMarshal.AsSpan(points))
            {
                clipperPath.Add(new PointD((float)p.X, (float)p.Y));
            }

            SKPoint firstPoint = points.First();
            clipperPath.Add(new PointD(firstPoint.X, firstPoint.Y));

            PathsD clipperPaths = [];

            clipperPaths.Add(clipperPath);

            //double d = distance;

            if (location == ParallelDirection.Below)
            {
                distance = -distance;
            }

            // offset polyline
            PathsD inflatedPaths = Clipper.InflatePaths(clipperPaths, distance, JoinType.Round, EndType.Polygon);

            if (inflatedPaths.Count > 0)
            {
                PathD inflatedPathD = inflatedPaths.First();

                List<SKPoint> inflatedPath = new(inflatedPathD.Count);

                //int inflatedPathCount = inflatedPathD.Count;

                foreach (PointD p in CollectionsMarshal.AsSpan(inflatedPathD))
                {
                    inflatedPath.Add(new SKPoint((float)p.x, (float)p.y));
                }

                /*
                for (int i = 0; i < inflatedPathCount; i++)
                {
                    inflatedPath.Add(new SKPoint((float)inflatedPathD[i].x, (float)inflatedPathD[i].y));

                }
                */

                return inflatedPath;
            }
            else
            {
                return points;
            }
        }

        internal static List<MapRegionPoint> GetParallelRegionPoints(List<MapRegionPoint> points, float distance, ParallelDirection location)
        {
            PathD clipperPath = [];

            foreach (MapRegionPoint point in points)
            {
                clipperPath.Add(new PointD(point.RegionPoint.X, point.RegionPoint.Y));
            }

            PathsD clipperPaths = [];

            clipperPaths.Add(clipperPath);

            float d = (location == ParallelDirection.Below) ? -distance : distance;

            // offset polyline
            PathsD inflatedPaths = Clipper.InflatePaths(clipperPaths, d, JoinType.Square, EndType.Polygon);

            if (inflatedPaths.Count > 0)
            {
                PathD inflatedPathD = inflatedPaths.First();

                List<MapRegionPoint> inflatedPath = [];

                foreach (PointD p in inflatedPathD)
                {
                    inflatedPath.Add(new MapRegionPoint(new SKPoint((float)p.x, (float)p.y)));
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
            SKPoint entryPoint = SKPoint.Empty;

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

            while (!CheckExit2(contour, contourPoint, entryPoint))
            {
                if (image.GetPixel((int)checkPoint.X, (int)checkPoint.Y).ToArgb() == Color.Black.ToArgb())
                {
                    contour.Add(checkPoint);

                    backPoint.X = contourPoint.X;
                    backPoint.Y = contourPoint.Y;

                    if (entryPoint == SKPoint.Empty)
                    {
                        entryPoint.X = backPoint.X;
                        entryPoint.Y = backPoint.Y;
                    }

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

        private static bool CheckExit2(List<SKPoint> contour, SKPoint startPoint, SKPoint entryPoint)
        {
            bool boundaryComplete = false;

            if (contour.Contains(startPoint) && contour.Count > 4)
            {
                if (contour.Last().X == startPoint.X && contour.Last().Y == startPoint.Y)
                {
                    if (contour[^2].X == entryPoint.X && contour[^2].Y == entryPoint.Y)
                    {
                        boundaryComplete = true;
                    }

                    if (!boundaryComplete)
                    {
                        int startCount = 0;

                        for (int i = 0; i < contour.Count; i++)
                        {
                            if (contour[i].X == startPoint.X && contour[i].Y == startPoint.Y)
                            {
                                startCount++;
                            }

                            if (startCount >= 7)
                            {
                                boundaryComplete = true;
                                break;
                            }
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
