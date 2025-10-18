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

using SkiaSharp;

namespace RealmStudio
{
    public partial class DrawnPixelEditForm : Form
    {
        private Bitmap? _mapBitmap;
        private SKPoint _topLeft = new(0, 0);
        private List<Tuple<SKPoint, Color, Color>> _bitmapEdits = new();

        public Bitmap? MapBitmap
        {
            get => _mapBitmap;
            set => _mapBitmap = value;
        }

        public SKPoint TopLeft
        {
            get => _topLeft;
            set => _topLeft = value;
        }

        public List<Tuple<SKPoint, Color, Color>> BitmapEdits
        {
            get => _bitmapEdits;
            set => _bitmapEdits = value;
        }

        public DrawnPixelEditForm()
        {
            InitializeComponent();

            if (MapBitmap != null)
            {
                PixelEditor.Image = MapBitmap;
            }
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            if (MapBitmap != null)
            {
                PixelEditor.Zoom = 800;
                PixelEditor.Image = MapBitmap;
            }
            return base.ShowDialog(owner);
        }

        private void PixelEditor_MouseClick(object sender, MouseEventArgs e)
        {
            int bitmapX = e.X / PixelEditor.GridCellSize;
            int bitmapY = e.Y / PixelEditor.GridCellSize;

            if (bitmapX >= 0 && bitmapX < PixelEditor.Image.Width && bitmapY >= 0 && bitmapY < PixelEditor.Image.Height)
            {
                Bitmap bitmap = (Bitmap)PixelEditor.Image;
                Color currentColor = bitmap.GetPixel(bitmapX, bitmapY);
                Color selectedColor = SelectFillColorButton.BackColor; // Default color

                SKPoint editPoint = new(bitmapX + TopLeft.X, bitmapY + TopLeft.Y);

                if (e.Button == MouseButtons.Left)
                {
                    BitmapEdits.Add(new Tuple<SKPoint, Color, Color>(editPoint, currentColor, selectedColor));
                    bitmap.SetPixel(bitmapX, bitmapY, selectedColor);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    foreach (var edit in BitmapEdits)
                    {
                        if (edit.Item1 == editPoint)
                        {
                            bitmap.SetPixel(bitmapX, bitmapY, edit.Item2); // Revert to original color
                            BitmapEdits.Remove(edit);
                            break;
                        }
                    }
                }

                PixelEditor.Invalidate();
            }
        }

        private void PixelEditor_MouseMove(object sender, MouseEventArgs e)
        {
            int bitmapX = e.X / PixelEditor.GridCellSize;
            int bitmapY = e.Y / PixelEditor.GridCellSize;

            if (bitmapX >= 0 && bitmapX < PixelEditor.Image.Width && bitmapY >= 0 && bitmapY < PixelEditor.Image.Height)
            {
                Bitmap bitmap = (Bitmap)PixelEditor.Image;
                Color currentColor = bitmap.GetPixel(bitmapX, bitmapY);
                Color selectedColor = SelectFillColorButton.BackColor; // Default color

                SKPoint editPoint = new(bitmapX + TopLeft.X, bitmapY + TopLeft.Y);

                if (e.Button == MouseButtons.Left)
                {
                    BitmapEdits.Add(new Tuple<SKPoint, Color, Color>(editPoint, currentColor, selectedColor));
                    bitmap.SetPixel(bitmapX, bitmapY, selectedColor);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    for (int i = BitmapEdits.Count - 1; i >= 0; i--)
                    {
                        if (BitmapEdits[i].Item1 == editPoint)
                        {
                            bitmap.SetPixel(bitmapX, bitmapY, BitmapEdits[i].Item2); // Revert to original color
                            BitmapEdits.RemoveAt(i);
                        }
                    }
                }

                PixelEditor.Invalidate();
            }
        }

        private void SelectFillColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            Color c = UtilityMethods.SelectColor(this, e, SelectFillColorButton.BackColor);
            SelectFillColorButton.BackColor = c;
        }

        private void ClearPixelEditsButton_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show("Are you sure you want to clear all edits? This operation cannot be undone.", "Clear Pixel Edits", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                Bitmap bitmap = (Bitmap)PixelEditor.Image;

                for (int i = BitmapEdits.Count - 1; i >= 0; i--)
                {
                    bitmap.SetPixel((int)(BitmapEdits[i].Item1.X - TopLeft.X), (int)(BitmapEdits[i].Item1.Y - TopLeft.Y), BitmapEdits[i].Item2); // Revert to original color
                    BitmapEdits.RemoveAt(i);
                }

                PixelEditor.Invalidate();
            }
        }
    }
}
