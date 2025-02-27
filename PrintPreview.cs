/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
using AForge.Imaging.Filters;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing.Printing;

namespace RealmStudio
{
    public partial class PrintPreview : Form
    {
        private readonly RealmStudioMap CurrentMap;
        private readonly List<Printer> Printers = [];
        private Printer? SelectedPrinter;
        private readonly Bitmap RealmBitmap;
        private Bitmap? PreviewBitmap;
        private Bitmap PrintBitmap;
        private readonly List<Rectangle> PrintRectangles = [];
        private Bitmap? SliceBitmap;
        private int PrintingPage;
        private Rectangle PageBounds;
        private float ScalePercentage = 1.0F;

        float ScreenDpiX;
        float ScreenDpiY;

        float VerticalDpi = 600;
        float HorizontalDpi = 600;

        private readonly List<string> recognizedPaperSizes = [
            "Letter",
            "Legal",
            "Tabloid",
            "A4",
            "A3",
            "A5",
            "B4",
            "B5",
            "C5",
            "Executive",
            "Statement",
            "Folio",
            "Ledger",
            "8x10",
            "10x14",
            "11x17",
            "C",
            "D",
            "E"
        ];

        public PrintPreview(RealmStudioMap currentMap)
        {
            Cursor.Current = Cursors.WaitCursor;

            InitializeComponent();
            CurrentMap = currentMap;

            SKSurface s = SKSurface.Create(new SKImageInfo(currentMap.MapWidth, currentMap.MapHeight));
            s.Canvas.Clear();

            MapRenderMethods.RenderMapForExport(currentMap, s.Canvas);

            RealmBitmap = s.Snapshot().ToBitmap();
            PrintBitmap = new(RealmBitmap);

            PrinterListCombo.Items.Clear();

            string installedPrinterName;
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                installedPrinterName = PrinterSettings.InstalledPrinters[i];
                Printers.Add(new Printer(installedPrinterName));
            }

            PrinterListCombo.DataSource = Printers;
            PrinterListCombo.DisplayMember = "Name";

            for (int i = 0; i < Printers.Count; i++)
            {
                if (Printers[i].IsDefault)
                {
                    PrinterListCombo.SelectedIndex = i;
                    break;
                }
            }

            PrintStatusLabel.Text = "";
            PrintStatusLabel.Refresh();

            SetPrinterValues();
            RecalculatePrint();
        }

        private void SetPrinterValues()
        {
            if (SelectedPrinter != null && SelectedPrinter.PrintSettings != null)
            {
                if (SelectedPrinter.PrintSettings.SupportsColor)
                {
                    ColorRadio.Enabled = true;
                    GrayscaleRadio.Enabled = true;
                }
                else
                {
                    ColorRadio.Enabled = false;
                    GrayscaleRadio.Enabled = true;
                    GrayscaleRadio.Checked = true;
                }

                if (SelectedPrinter.PrintSettings.PrinterResolutions != null)
                {
                    PrintQualityCombo.Items.Clear();
                    foreach (PrinterResolution resolution in SelectedPrinter.PrintSettings.PrinterResolutions)
                    {
                        if (resolution.X <= 0 || resolution.Y <= 0)
                        {
                            PrintQualityCombo.Items.Add(resolution.Kind.ToString());
                        }
                        else
                        {
                            PrintQualityCombo.Items.Add(resolution.X + " x " + resolution.Y + " DPI");
                        }
                    }

                    PrintQualityCombo.SelectedIndex = 0;
                }

                if (SelectedPrinter.PageSettings != null)
                {
                    Margins margins = SelectedPrinter.PageSettings.Margins;
                    TopMarginUpDown.Value = (decimal)margins.Top / 100.0M;
                    LeftMarginUpDown.Value = (decimal)margins.Left / 100.0M;
                    RightMarginUpDown.Value = (decimal)margins.Right / 100.0M;
                    BottomMarginUpDown.Value = (decimal)margins.Bottom / 100.0M;
                }

                if (SelectedPrinter.PaperSizes.Count > 0)
                {
                    PaperSizeCombo.Items.Clear();
                    foreach (PaperSize ps in SelectedPrinter.PaperSizes)
                    {
                        if (recognizedPaperSizes.Contains(ps.PaperName))
                        {
                            PaperSizeCombo.Items.Add(ps.PaperName);
                        }
                    }

                    PaperSizeCombo.SelectedItem = PaperSizeCombo.Items[0];
                }

                ScalePercentageCombo.SelectedIndex = 2;
            }

            // dialog starts in landscape
            // width > height (usual case), aspect ratio > 1
            float aspectRatio = CurrentMap.MapWidth / CurrentMap.MapHeight;

            if (aspectRatio < 1)
            {
                // aspect ratio < 1, so map height > width = portrait
                PrintBitmap.Dispose();
                PrintBitmap = new(RealmBitmap);
            }
            else
            {
                // aspect >= 1, so map width > height = landscape
                PrintBitmap.Dispose();
                PrintBitmap = new(RealmBitmap);
                PrintBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

        }

        private void RecalculatePrint()
        {
            PrintStatusLabel.Text = "";
            PrintStatusLabel.Refresh();

            // Get the screen resolution.
            using (Graphics gr = this.CreateGraphics())
            {
                ScreenDpiX = gr.DpiX;
                ScreenDpiY = gr.DpiY;
            }

            float aspectRatio = CurrentMap.MapWidth / CurrentMap.MapHeight;

            if (PortraitRadio.Checked)
            {
                if (aspectRatio >= 1)
                {
                    // map is landscape
                    PrintBitmap.Dispose();
                    PrintBitmap = new(RealmBitmap);

                    if (GrayscaleRadio.Checked)
                    {
                        PrintBitmap = Grayscale.CommonAlgorithms.BT709.Apply(PrintBitmap);
                    }

                    PrintBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else
                {
                    // map is portrait
                    PrintBitmap.Dispose();
                    PrintBitmap = new(RealmBitmap);

                    if (GrayscaleRadio.Checked)
                    {
                        PrintBitmap = Grayscale.CommonAlgorithms.BT709.Apply(PrintBitmap);
                    }
                }
            }
            else if (LandscapeRadio.Checked)
            {
                if (aspectRatio >= 1)
                {
                    // map is landscape
                    PrintBitmap.Dispose();
                    PrintBitmap = new(RealmBitmap);

                    if (GrayscaleRadio.Checked)
                    {
                        PrintBitmap = Grayscale.CommonAlgorithms.BT709.Apply(PrintBitmap);
                    }
                }
                else
                {
                    // map is portrait
                    PrintBitmap.Dispose();
                    PrintBitmap = new(RealmBitmap);

                    if (GrayscaleRadio.Checked)
                    {
                        PrintBitmap = Grayscale.CommonAlgorithms.BT709.Apply(PrintBitmap);
                    }

                    PrintBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
            }

            DrawPrintPreview();
        }

        private void DrawPrintPreview()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (SelectedPrinter == null) return;

            // figure out how many pages the map will require to print

            int pagesAcross = 1;
            int pagesDown = 1;

            if (FitToRadio.Checked && !MaintainAspectRatioCheck.Checked)
            {
                // if fit to radio is checked, the map will be fit to that many pages
                pagesAcross = (int)SheetsAcrossUpDown.Value;
                pagesDown = (int)SheetsDownUpDown.Value;
            }
            else if (ScaleRadio.Checked)
            {
                // if scale radio is checked, the map will be scaled to that percentage
                // and then the number of pages will be calculated based on the scaled map
                if (ScalePercentageCombo.SelectedItem != null)
                {
                    string scalePercentageString = (string)ScalePercentageCombo.SelectedItem;
                    scalePercentageString = scalePercentageString[..scalePercentageString.IndexOf('%')].Trim();
                    ScalePercentage = float.Parse(scalePercentageString) / 100.0F;
                }

                PrintBitmap = new(PrintBitmap, (int)(PrintBitmap.Width * ScalePercentage), (int)(PrintBitmap.Height * ScalePercentage));
            }

            // get the print quality and figure out the page DPI; that will
            // be used to calculate the number of pages required to print the map;
            // the margin values are in hundredths of an inch, so they need to be converted to pixels
            // and then the number of pages is calculated based on the paper size and the margins

            string? selectedPrintQuality = (string?)PrintQualityCombo.SelectedItem;

            if (selectedPrintQuality == null) return;

            switch (selectedPrintQuality)
            {
                case "High":
                    VerticalDpi = 1200;
                    HorizontalDpi = 1200;
                    break;
                case "Normal":
                    VerticalDpi = 600;
                    HorizontalDpi = 600;
                    break;
                case "Draft":
                    VerticalDpi = 300;
                    HorizontalDpi = 300;
                    break;
                case "Default":
                    VerticalDpi = 600;
                    HorizontalDpi = 600;
                    break;
                default:
                    string[] resolutionParts = selectedPrintQuality.Split('x');
                    if (resolutionParts.Length == 2)
                    {
                        HorizontalDpi = int.Parse(resolutionParts[0]);
                        string resolutionParts1 = resolutionParts[1][..resolutionParts[1].IndexOf("DPI")].Trim();
                        VerticalDpi = int.Parse(resolutionParts1);
                    }
                    break;
            }

            // default page size in inches
            float pageVerticalSize = 11;
            float pageHorizontalSize = 8.5F;

            string? selectedPaperSize = (string?)PaperSizeCombo.SelectedItem;

            if (selectedPaperSize == null) return;

            float printBitmapWidthInches = PrintBitmap.Width / ScreenDpiX;
            float printBitmapHeightInches = PrintBitmap.Height / ScreenDpiY;

            // all page sizes are in inches
            switch (selectedPaperSize)
            {
                case "Letter":
                    pageVerticalSize = 11;
                    pageHorizontalSize = 8.5F;
                    break;
                case "Legal":
                    pageVerticalSize = 14;
                    pageHorizontalSize = 8.5F;
                    break;
                case "Tabloid":
                    pageVerticalSize = 17;
                    pageHorizontalSize = 11;
                    break;
                case "A4":
                    pageVerticalSize = 11.67F;
                    pageHorizontalSize = 8.25F;
                    break;
                case "A3":
                    pageVerticalSize = 16.25F;
                    pageHorizontalSize = 11.67F;
                    break;
                case "A5":
                    pageVerticalSize = 8.25F;
                    pageHorizontalSize = 5.875F;
                    break;
                case "B4":
                    pageVerticalSize = 13.9F;
                    pageHorizontalSize = 9.8F;
                    break;
                case "B5":
                    pageVerticalSize = 9.8F;
                    pageHorizontalSize = 6.9F;
                    break;
                case "C5":
                    pageVerticalSize = 9.0F;
                    pageHorizontalSize = 6.4F;
                    break;
                case "Executive":
                    pageVerticalSize = 10.5F;
                    pageHorizontalSize = 7.25F;
                    break;
                case "Statement":
                    pageVerticalSize = 8.5F;
                    pageHorizontalSize = 5.5F;
                    break;
                case "Folio":
                    pageVerticalSize = 13;
                    pageHorizontalSize = 8.5F;
                    break;
                case "Ledger":
                    pageVerticalSize = 17;
                    pageHorizontalSize = 11;
                    break;
                case "8x10":
                    pageVerticalSize = 10;
                    pageHorizontalSize = 8;
                    break;
                case "10x14":
                    pageVerticalSize = 14;
                    pageHorizontalSize = 10;
                    break;
                case "11x17":
                    pageVerticalSize = 17;
                    pageHorizontalSize = 11;
                    break;
                case "C":
                    pageVerticalSize = 22;
                    pageHorizontalSize = 17;
                    break;
                case "D":
                    pageVerticalSize = 34;
                    pageHorizontalSize = 22;
                    break;
                case "E":
                    pageVerticalSize = 44;
                    pageHorizontalSize = 34;
                    break;
            }

            int printBitmapWidth = PrintBitmap.Width;
            int printBitmapHeight = PrintBitmap.Height;

            float marginPixelTopHeight = (float)TopMarginUpDown.Value * ScreenDpiY;
            float marginPixelLeftWidth = (float)LeftMarginUpDown.Value * ScreenDpiX;
            float marginPixelRightWidth = (float)RightMarginUpDown.Value * ScreenDpiX;
            float marginPixelBottomHeight = (float)BottomMarginUpDown.Value * ScreenDpiY;

            float marginInchesTopHeight = (float)TopMarginUpDown.Value;
            float marginInchesLeftWidth = (float)LeftMarginUpDown.Value;
            float marginInchesRightWidth = (float)RightMarginUpDown.Value;
            float marginInchesBottomHeight = (float)BottomMarginUpDown.Value;

            if (marginInchesTopHeight + marginInchesBottomHeight >= pageVerticalSize - 1
                || marginInchesLeftWidth + marginInchesRightWidth >= pageHorizontalSize - 1)
            {
                TopMarginUpDown.Value = 1;
                LeftMarginUpDown.Value = 1;
                RightMarginUpDown.Value = 1;
                BottomMarginUpDown.Value = 1;

                MessageBox.Show("The margin values entered overlap or are off the paper.\nPlease enter different margin sizes.", "Invalid Margins", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            // number of vertical inches available for printing on the page
            float printableHeight = pageVerticalSize - marginInchesTopHeight - marginInchesBottomHeight;

            // number of horizontal inches available for printing on the page
            float printableWidth = pageHorizontalSize - marginInchesLeftWidth - marginInchesRightWidth;

            // number of calculated horizontal pages required to print the map
            float horizontalPages = (int)Math.Ceiling((double)printBitmapWidthInches / printableWidth);

            // number of calculated vertical pages required to print the map
            float verticalPages = (int)Math.Ceiling((double)printBitmapHeightInches / printableHeight);

            // total number of horizontal inches on the total number of horizontal pages
            float totalHorizontalSizeInches = horizontalPages * printableWidth;

            // total number of vertical inches on the total number of vertical pages
            float totalVerticalSizeInches = verticalPages * printableHeight;

            // if the map is fit to the number of pages, then the number of pages is already calculated
            // but the map may need to be scaled to fit the number of pages

            if (FitToRadio.Checked && !MaintainAspectRatioCheck.Checked)
            {
                // if the selected number of sheets across or sheets down
                // exceeds the number needed to fit the map, then the map
                // will be scaled up to fit the number of sheets selected

                float horizontalScalePercentage = 1.0F;
                float verticalScalePercentage = 1.0F;
                float scalePercentage = 1.0F;

                if (horizontalPages < pagesAcross)
                {
                    horizontalScalePercentage = totalHorizontalSizeInches / printBitmapWidthInches;
                }

                if (verticalPages < pagesDown)
                {
                    verticalScalePercentage = totalVerticalSizeInches / printBitmapHeightInches;
                }

                float newBitmapWidthInches = printBitmapWidth * horizontalScalePercentage / ScreenDpiX;
                float newBitmapHeightInches = printBitmapHeight * verticalScalePercentage / ScreenDpiY;

                if (newBitmapWidthInches > totalHorizontalSizeInches)
                {
                    scalePercentage = verticalScalePercentage;
                }
                else if (newBitmapHeightInches > totalVerticalSizeInches)
                {
                    scalePercentage = horizontalScalePercentage;
                }

                using Bitmap fitPrintBitmap = new(PrintBitmap, (int)(PrintBitmap.Width * scalePercentage), (int)(PrintBitmap.Height * scalePercentage));
                PrintBitmap.Dispose();
                PrintBitmap = (Bitmap)fitPrintBitmap.Clone();

                printBitmapWidth = PrintBitmap.Width;
                printBitmapHeight = PrintBitmap.Height;

                horizontalPages = (int)Math.Ceiling((double)printBitmapWidth / printableWidth);
                verticalPages = (int)Math.Ceiling((double)printBitmapHeight / printableHeight);

                string acrossLabel = pagesAcross == 1 ? " page across" : " pages across";
                string downLabel = pagesDown == 1 ? " page down" : " pages down";

                PrintRealmSheetsLabel.Text = "Printing the realm will require " + pagesAcross + acrossLabel + " and " + pagesDown + downLabel + ".";
            }
            else
            {
                pagesAcross = (int)horizontalPages;
                pagesDown = (int)verticalPages;

                string acrossLabel = pagesAcross == 1 ? " page across" : " pages across";
                string downLabel = pagesDown == 1 ? " page down" : " pages down";

                PrintRealmSheetsLabel.Text = "Printing the realm will require " + pagesAcross + acrossLabel + " and " + pagesDown + downLabel + ".";
            }

            int previewBaseWidth = RealmBitmap.Width;
            int previewBaseHeight = RealmBitmap.Height;

            if (PortraitRadio.Checked)
            {
                (previewBaseHeight, previewBaseWidth) = (previewBaseWidth, previewBaseHeight);
            }

            // print parameters are calculated, so now draw the print preview
            PreviewBitmap = new(previewBaseWidth + (int)marginPixelLeftWidth + (int)marginPixelRightWidth,
                previewBaseHeight + (int)marginPixelTopHeight + (int)marginPixelBottomHeight);

            using Graphics previewGraphics = Graphics.FromImage(PreviewBitmap);
            previewGraphics.Clear(Color.White);

            if (ScaleRadio.Checked)
            {
                Bitmap scaledBitmap = new(PrintBitmap, (int)(PrintBitmap.Width / ScalePercentage), (int)(PrintBitmap.Height / ScalePercentage));
                previewGraphics.DrawImage(scaledBitmap, marginPixelLeftWidth, marginPixelTopHeight);
            }
            else
            {
                previewGraphics.DrawImage(PrintBitmap, marginPixelLeftWidth, marginPixelTopHeight);
            }

            PrintBitmap.Dispose();
            PrintBitmap = (Bitmap)PreviewBitmap.Clone();

            // show page divisions
            using Pen pen = new(Color.Black, 2);

            int xDivisionSize = PreviewBitmap.Width / pagesAcross;
            int yDivisionSize = PreviewBitmap.Height / pagesDown;

            for (int x = 1; x < pagesAcross; x++)
            {
                previewGraphics.DrawLine(pen, x * xDivisionSize, 0, x * xDivisionSize, PreviewBitmap.Height);
            }

            for (int y = 1; y < pagesDown; y++)
            {
                previewGraphics.DrawLine(pen, 0, y * yDivisionSize, PreviewBitmap.Width, y * yDivisionSize);
            }

            RealmPreviewPicture.Image = PreviewBitmap;

            // slice the preview bitmap and add the slices to the printlist so the
            // realm can be printed

            PrintRectangles.Clear();

            for (int i = 0; i < pagesAcross; i++)
            {
                for (int j = 0; j < pagesDown; j++)
                {
                    Rectangle sliceRect = new(i * xDivisionSize, j * yDivisionSize, xDivisionSize - 1, yDivisionSize - 1);
                    PrintRectangles.Add(sliceRect);
                }
            }

            if (PrintRectangles.Count != pagesAcross * pagesDown)
            {
                PrintStatusLabel.Text = "An error has occurred while calculating print parameters.";
                return;
            }

            Cursor.Current = Cursors.Default;
        }


        private void PrinterListCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PrinterListCombo.Items[PrinterListCombo.SelectedIndex] is Printer p)
            {
                SelectedPrinter = p;
            }

            SetPrinterValues();
        }

        private void PortraitRadio_CheckedChanged_1(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void LandscapeRadio_CheckedChanged_1(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void ColorRadio_CheckedChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void GrayscaleRadio_CheckedChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void PaperSizeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void MarginsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void PrintQualityCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void ScaleRadio_CheckedChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void ScalePercentageCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void FitToRadio_CheckedChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void SheetsAcrossUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (FitToRadio.Checked)
            {
                RecalculatePrint();
            }
        }

        private void SheetsDownUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (FitToRadio.Checked)
            {
                RecalculatePrint();
            }
        }

        private void TopMarginUpDown_ValueChanged(object sender, EventArgs e)
        {
            // TODO: restrict minimum and maximum margins according to printer minimum margins and paper size
            RecalculatePrint();
        }

        private void LeftMarginUpDown_ValueChanged(object sender, EventArgs e)
        {
            // TODO: restrict minimum and maximum margins according to printer minimum margins and paper size
            RecalculatePrint();
        }

        private void BottomMarginUpDown_ValueChanged(object sender, EventArgs e)
        {
            // TODO: restrict minimum and maximum margins according to printer minimum margins and paper size
            RecalculatePrint();
        }

        private void RightMarginUpDown_ValueChanged(object sender, EventArgs e)
        {
            // TODO: restrict minimum and maximum margins according to printer minimum margins and paper size
            RecalculatePrint();
        }

        private void MaintainAspectRatioCheck_CheckedChanged(object sender, EventArgs e)
        {
            RecalculatePrint();
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            if (SelectedPrinter == null)
            {
                return;
            }

            // print using selected parameters
            using PrintDocument pd = new();
            pd.PrinterSettings.PrinterName = SelectedPrinter.Name;
            pd.PrintPage += PrintPage;

            foreach (PaperSize ps in SelectedPrinter.PaperSizes)
            {
                if (((string?)PaperSizeCombo.SelectedItem) == ps.PaperName)
                {
                    pd.DefaultPageSettings.PaperSize = ps;
                }
            }

            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            pd.OriginAtMargins = true;

            if (LandscapeRadio.Checked)
            {
                pd.DefaultPageSettings.Landscape = true;
            }
            else
            {
                pd.DefaultPageSettings.Landscape = false;
            }

            PageBounds = pd.PrinterSettings.DefaultPageSettings.Bounds;

            int numCopies = (int)NumberCopiesUpDown.Value;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                for (int i = 0; i < numCopies; i++)
                {
                    PrintingPage = 0;
                    pd.Print();
                }

                PrintStatusLabel.Text = "Printing job queued or complete.";
                PrintStatusLabel.Refresh();
            }
            catch (Exception ex)
            {

                PrintStatusLabel.Text = "An error has occurred while printing: " + ex.Message;
                PrintStatusLabel.Refresh();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void PrintPage(object o, PrintPageEventArgs e)
        {
            if (PrintingPage >= PrintRectangles.Count || e.Graphics == null)
            {
                e.HasMorePages = false;
                return;
            }

            PrintStatusLabel.Text = "Printing page " + (PrintingPage + 1).ToString();
            PrintStatusLabel.Refresh();

            Rectangle sliceRect = PrintRectangles[PrintingPage];

            SliceBitmap = new(sliceRect.Width, sliceRect.Height, PrintBitmap.PixelFormat);

            if (SliceBitmap != null && SliceBitmap.Width > 0 && SliceBitmap.Height > 0)
            {
                using (var g = Graphics.FromImage(SliceBitmap))
                {
                    g.DrawImage(PrintBitmap, 0, 0, sliceRect, GraphicsUnit.Pixel);
                }

                PageBounds = e.PageBounds;

                if (MaintainAspectRatioCheck.Checked)
                {
                    e.Graphics.DrawImageUnscaledAndClipped(SliceBitmap, PageBounds);
                }
                else
                {
                    e.Graphics.DrawImage(SliceBitmap, PageBounds);
                }

                SliceBitmap.Dispose();
                SliceBitmap = null;

                PrintingPage++;

                if (PrintingPage < PrintRectangles.Count)
                {
                    e.HasMorePages = true;
                }
                else
                {
                    e.HasMorePages = false;
                }
            }
        }

        private void ClosePrintPreviewButton_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}
