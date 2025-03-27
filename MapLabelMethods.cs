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
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RealmStudio
{
    internal sealed class MapLabelMethods
    {
        public static PrivateFontCollection EMBEDDED_FONTS { get; } = new();

        internal static SKPaint CreateLabelPaint(Color labelColor)
        {
            SKPaint paint = new()
            {
                Color = Extensions.ToSKColor(labelColor),
                IsAntialias = true
            };

            return paint;
        }

        internal static SKFont GetSkLabelFont(Font labelFont)
        {
            List<string> resourceNames = [.. Assembly.GetExecutingAssembly().GetManifestResourceNames()];

            SKFontStyle fs = SKFontStyle.Normal;

            if (labelFont.Bold && labelFont.Italic)
            {
                fs = SKFontStyle.BoldItalic;
            }
            else if (labelFont.Bold)
            {
                fs = SKFontStyle.Bold;
            }
            else if (labelFont.Italic)
            {
                fs = SKFontStyle.Italic;
            }

            string fontName = StringExtensions.FindBestMatch(labelFont.FontFamily.Name, resourceNames);

            SKTypeface? fontTypeface;

            if (!string.IsNullOrEmpty(fontName))
            {
                fontTypeface = SKTypeface.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(fontName));
            }
            else
            {
                fontTypeface = SKTypeface.FromFamilyName(labelFont.FontFamily.Name, fs);

                if (fontTypeface == SKTypeface.Default)
                {
                    fontTypeface = SKTypeface.FromFamilyName(labelFont.FontFamily.Name);
                }
            }

            SKFont skLabelFont = new(fontTypeface)
            {
                Size = labelFont.SizeInPoints * 1.333F
            };

            return skLabelFont;
        }

#pragma warning disable SYSLIB1054
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, ref uint pcFonts);

        [DllImport("gdi32", CharSet = CharSet.Ansi)]
        private static extern int GetOutlineTextMetrics(
            IntPtr hdc,            // handle to DC
            int cbData,            // size in bytes for text metrics
            IntPtr lpOtm         // pointer to buffer to receive outline text metrics structure
        );
#pragma warning restore SYSLIB1054

        /// <summary>
        /// Gets the <see cref="PanoseFontFamilyTypes"/> for the specified font.
        /// </summary>
        /// <param name="graphics">A graphics object to use when detecting the Panose
        /// family.</param>
        /// <param name="font">The font to check.</param>
        /// <returns>The Panose font family type.</returns>
        public static PanoseFontFamilyTypes PanoseFontFamilyType(Graphics graphics, Font font)
        {
            byte bFamilyType = 0;

            IntPtr hdc = graphics.GetHdc();
            IntPtr hFontOld = SelectObject(hdc, font.ToHfont());

            int bufSize = GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
            IntPtr lpOtm = Marshal.AllocCoTaskMem(bufSize);
            Marshal.WriteInt32(lpOtm, bufSize);
            int success = GetOutlineTextMetrics(hdc, bufSize, lpOtm);
            if (success != 0)
            {
                int offset = 61;
                bFamilyType = Marshal.ReadByte(lpOtm, offset);
            }

            Marshal.FreeCoTaskMem(lpOtm);

            SelectObject(hdc, hFontOld);
            graphics.ReleaseHdc(hdc);

            return (PanoseFontFamilyTypes)bFamilyType;
        }

        internal static void AddEmbeddedResourceFont(byte[] fontData)
        {
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;

            EMBEDDED_FONTS.AddMemoryFont(fontPtr, fontData.Length);
            AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
            Marshal.FreeCoTaskMem(fontPtr);
        }

        internal static void FinalizeMapBoxes(RealmStudioMap map)
        {
            // finalize loading of placed map boxes
            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER);
            for (int i = 0; i < boxLayer.MapLayerComponents.Count; i++)
            {
                if (boxLayer.MapLayerComponents[i] is PlacedMapBox box && box.BoxBitmap != null)
                {
                    SKRectI center = new((int)box.BoxCenterLeft, (int)box.BoxCenterTop,
                        (int)(box.Width - box.BoxCenterRight), (int)(box.Height - box.BoxCenterBottom));

                    if (center.IsEmpty || center.Left < 0 || center.Right <= 0 || center.Top < 0 || center.Bottom <= 0)
                    {
                    }
                    else if (center.Width <= 0 || center.Height <= 0)
                    {
                        // swap 
                        if (center.Right < center.Left)
                        {
                            (center.Left, center.Right) = (center.Right, center.Left);
                        }

                        if (center.Bottom < center.Top)
                        {
                            (center.Top, center.Bottom) = (center.Bottom, center.Top);
                        }
                    }
                }
            }
        }

        internal static PlacedMapBox? CreatePlacedMapBox(MapBox mapBox, SKPoint zoomedScrolledPoint, Color boxTintColor)
        {
            PlacedMapBox? newPlacedMapBox = new()
            {
                X = (int)zoomedScrolledPoint.X,
                Y = (int)zoomedScrolledPoint.Y,

                BoxCenterLeft = mapBox.BoxCenterLeft,
                BoxCenterTop = mapBox.BoxCenterTop,
                BoxCenterRight = mapBox.BoxCenterRight,
                BoxCenterBottom = mapBox.BoxCenterBottom,
                BoxTint = boxTintColor
            };

            PaintObjects.BoxPaint.Dispose();

            PaintObjects.BoxPaint = new()
            {
                Style = SKPaintStyle.Fill,
                ColorFilter = SKColorFilter.CreateBlendMode(
                    Extensions.ToSKColor(boxTintColor),
                    SKBlendMode.Modulate) // combine the tint with the bitmap color
            };

            newPlacedMapBox.BoxPaint = PaintObjects.BoxPaint.Clone();

            return newPlacedMapBox;
        }

        internal static TextBox? CreateLabelEditTextBox(SKGLControl glControl, MapLabel mapLabel, Rectangle tbRect)
        {
            TextBox? labelEditTextBox = new()
            {
                Parent = glControl,
                Name = Guid.NewGuid().ToString(),
                Left = tbRect.Left,
                Top = tbRect.Top,
                Width = tbRect.Width,
                Height = tbRect.Height,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = true,
                Font = mapLabel.LabelFont,
                Visible = true,
                BackColor = Color.AliceBlue,
                ForeColor = mapLabel.LabelColor,
                BorderStyle = BorderStyle.None,
                Multiline = false,
                TextAlign = HorizontalAlignment.Left,
                Text = mapLabel.LabelText,
            };

            return labelEditTextBox;
        }

        internal static void DeleteLabel(RealmStudioMap map, MapLabel label)
        {
            // delete the currently selected label
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            for (int i = labelLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (labelLayer.MapLayerComponents[i] is MapLabel l && l.LabelGuid.ToString() == label.LabelGuid.ToString())
                {
                    labelLayer.MapLayerComponents.RemoveAt(i);
                }
            }
        }

        internal static void MoveLabel(MapLabel label, SKPoint zoomedScrolledPoint)
        {
            if (label.LabelPath != null)
            {
                float dx = zoomedScrolledPoint.X - (label.X + (label.Width / 2));
                float dy = zoomedScrolledPoint.Y - (label.Y + (label.Height / 2));
                label.LabelPath.Transform(SKMatrix.CreateTranslation(dx, dy));
            }
            else
            {
                label.X = (int)zoomedScrolledPoint.X - label.Width / 2;
                label.Y = (int)zoomedScrolledPoint.Y;
            }
        }

        internal static void MoveBox(PlacedMapBox placedMapBox, SKPoint zoomedScrolledPoint)
        {
            placedMapBox.X = (int)zoomedScrolledPoint.X - (placedMapBox.Width / 2);
            placedMapBox.Y = (int)zoomedScrolledPoint.Y - (placedMapBox.Height / 2);
        }

        internal static SKPath CreateNewArcPath(SKPoint zoomedScrolledPoint, SKPoint previousCursorPoint)
        {
            SKPath newArcPath = new();

            if (zoomedScrolledPoint.Y > previousCursorPoint.Y)
            {
                // start on the left and drag right and down to draw an arc downward (open part of the arc facing down)
                SKRect r = new(previousCursorPoint.X, previousCursorPoint.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);
                newArcPath.AddArc(r, 180, 180);
            }
            else
            {
                // start on the right and drag left and up to draw an arc upward (open part of the arc facing up)
                SKRect r = new(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, previousCursorPoint.X, previousCursorPoint.Y);
                newArcPath.AddArc(r, 180, -180);
            }

            return newArcPath;
        }

        internal static void DrawLabelPathOnWorkLayer(RealmStudioMap map, SKPath mapLabelPath)
        {
            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
            MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

            MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawPath(mapLabelPath, PaintObjects.LabelPathPaint);
        }

        internal static void DrawBoxOnWorkLayer(RealmStudioMap map, MapBox mapBox, PlacedMapBox placedMapBox, SKPoint zoomedScrolledPoint, SKPoint previousPoint)
        {
            SKRect boxRect = new(previousPoint.X, previousPoint.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

            if (boxRect.Width > 0 && boxRect.Height > 0)
            {
                SKBitmap? b = mapBox.BoxBitmap.ToSKBitmap();

                if (b != null)
                {
                    SKBitmap resizedBitmap = b.Resize(new SKSizeI((int)boxRect.Width, (int)boxRect.Height), SKSamplingOptions.Default);

                    placedMapBox.SetBoxBitmap(resizedBitmap);
                    placedMapBox.Width = resizedBitmap.Width;
                    placedMapBox.Height = resizedBitmap.Height;

                    MapLayer workLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER);
                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawBitmap(resizedBitmap, previousPoint, PaintObjects.BoxPaint);
                }
            }
        }

        internal static Rectangle GetLabelTextBoxRect(MapLabel mapLabel, SKPoint drawingPoint, float drawingZoom, Size labelSize)
        {
            int tbX = (int)((mapLabel.X - drawingPoint.X) * drawingZoom);
            int tbY = (int)((mapLabel.Y - drawingPoint.Y) * drawingZoom) - (labelSize.Height / 2);
            int tbW = (int)(mapLabel.Width * drawingZoom);
            int tbH = (int)(mapLabel.Height * 1.5F * drawingZoom);

            Rectangle rect = new(tbX, tbY, tbW, tbH);

            return rect;
        }

        internal static TextBox? CreateNewLabelTextbox(SKGLControl glControl, Font tbFont, Rectangle labelRect, Color tbColor)
        {
            TextBox labelTextBox = new()
            {
                Parent = glControl,
                Name = Guid.NewGuid().ToString(),
                Left = labelRect.Left,
                Top = labelRect.Top,
                Width = labelRect.Width,
                Height = labelRect.Height,
                Margin = System.Windows.Forms.Padding.Empty,
                AutoSize = false,
                Font = tbFont,
                Visible = true,
                PlaceholderText = "...Label...",
                BackColor = Color.Peru,
                ForeColor = tbColor,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = false,
                TextAlign = HorizontalAlignment.Center,
                Text = "...Label...",
            };

            return labelTextBox;
        }

        internal static void AddNewBox(RealmStudioMap map, PlacedMapBox? placedMapBox)
        {
            if (placedMapBox != null && placedMapBox.BoxBitmap != null)
            {
                SKRectI center = new((int)placedMapBox.BoxCenterLeft,
                    (int)placedMapBox.BoxCenterTop,
                    (int)(placedMapBox.Width - placedMapBox.BoxCenterRight),
                    (int)(placedMapBox.Height - placedMapBox.BoxCenterBottom));

                if (center.IsEmpty || center.Left < 0 || center.Right <= 0 || center.Top < 0 || center.Bottom <= 0)
                {
                    return;
                }
                else if (center.Width <= 0 || center.Height <= 0)
                {
                    // swap 
                    if (center.Right < center.Left)
                    {
                        (center.Left, center.Right) = (center.Right, center.Left);
                    }

                    if (center.Bottom < center.Top)
                    {
                        (center.Top, center.Bottom) = (center.Bottom, center.Top);
                    }
                }

                Cmd_AddLabelBox cmd = new(map, placedMapBox);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }
    }
}
