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
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    /*
    internal sealed class LabelManager : IMapComponentManager
    {
        public static PrivateFontCollection EMBEDDED_FONTS { get; } = new();
        private static SKPath? _currentMapLabelPath = new();
        private static readonly List<SKPoint> _currentMapPathLabelPoints = [];
        private static TextBox? _labelTextBox;
        private static bool _creatingLabel;

        private static LabelUIMediator? _labelMediator;

        internal static LabelUIMediator? LabelMediator
        {
            get { return _labelMediator; }
            set { _labelMediator = value; }
        }

        internal static SKPath? CurrentMapLabelPath
        {
            get { return _currentMapLabelPath; }
            set { _currentMapLabelPath = value; }
        }

        internal static List<SKPoint> CurrentMapPathLabelPoints
        {
            get { return _currentMapPathLabelPoints; }
        }

        internal static TextBox? LabelTextBox
        {
            get { return _labelTextBox; }
            set { _labelTextBox = value; }
        }

        internal static bool CreatingLabel
        {
            get { return _creatingLabel; }
            set { _creatingLabel = value; }
        }


        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static IMapComponent? Create()
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(LabelMediator);

            if (MapStateMediator.SelectedMapLabel != null)
            {
                // update label rotation
                //Cmd_ChangeLabelRotation cmd = new(MapStateMediator.SelectedMapLabel, LabelMediator.LabelRotation);
                //CommandManager.AddCommand(cmd);
                //cmd.DoOperation();

                // update other attributes
                Color labelColor = LabelMediator.LabelColor;
                Color outlineColor = LabelMediator.OutlineColor;
                float outlineWidth = LabelMediator.OutlineWidth;
                Color glowColor = LabelMediator.GlowColor;
                int glowStrength = (int)LabelMediator.GlowStrength;

                Font tbFont = new(LabelMediator.SelectedLabelFont.FontFamily,
                    LabelMediator.SelectedLabelFont.Size * 0.75F, LabelMediator.SelectedLabelFont.Style, GraphicsUnit.Point);

                //Cmd_ChangeLabelAttributes cmd2 = new(MapStateMediator.SelectedMapLabel, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, tbFont);
                //CommandManager.AddCommand(cmd2);
                //cmd2.DoOperation();

                return true;
            }

            return false;
        }

        public static bool Delete()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            if (MapStateMediator.SelectedMapLabel != null)
            {
                //Cmd_DeleteLabel cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapLabel);
                //CommandManager.AddCommand(cmd);
                //cmd.DoOperation();

                return true;
            }

            return false;
        }

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
                Enabled = true,
            };

            return labelEditTextBox;
        }

        internal static void DeleteLabel(RealmStudioMap map, MapLabel label)
        {
            // delete the currently selected label
            //MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER);

            //for (int i = labelLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            //{
            //    if (labelLayer.MapLayerComponents[i] is MapLabel l && l.LabelGuid.ToString() == label.LabelGuid.ToString())
            //    {
            //        labelLayer.MapLayerComponents.RemoveAt(i);
            //    }
            //}
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

        internal static void ConstructBezierPathFromPoints()
        {
            CurrentMapLabelPath?.Dispose();
            CurrentMapLabelPath = new();

            if (CurrentMapPathLabelPoints.Count > 2)
            {
                CurrentMapLabelPath.MoveTo(CurrentMapPathLabelPoints[0]);

                for (int j = 0; j < CurrentMapPathLabelPoints.Count; j += 3)
                {
                    if (j < CurrentMapPathLabelPoints.Count - 2)
                    {
                        CurrentMapLabelPath.CubicTo(CurrentMapPathLabelPoints[j], CurrentMapPathLabelPoints[j + 1], CurrentMapPathLabelPoints[j + 2]);
                    }
                }
            }
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
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            workLayer.LayerSurface?.Canvas.DrawPath(mapLabelPath, PaintObjects.LabelPathPaint);
            
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
                Enabled = true,
            };

            return labelTextBox;
        }

        internal static void ResetLabelPath()
        {
            CurrentMapLabelPath?.Dispose();
            CurrentMapLabelPath = new();
            CurrentMapPathLabelPoints.Clear();
        }
    }
    */
}
