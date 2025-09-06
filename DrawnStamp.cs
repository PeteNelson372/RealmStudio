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
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    internal sealed class DrawnStamp : DrawnMapComponent
    {
        private SKPoint _topLeft;
        private int _rotation;
        private float _opacity = 1.0f;
        private float _scale = 1.0f;
        private SKBitmap? _stampBitmap;


        public SKPoint TopLeft
        {
            get => _topLeft;
            set => _topLeft = value;
        }

        public int Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public float Opacity
        {
            get => _opacity;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Opacity must be between 0 and 1.");
                }
                _opacity = value;
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Scale must be greater than 0.");
                }
                _scale = value;
            }
        }

        public SKBitmap? StampBitmap
        {
            get => _stampBitmap;
            set
            {
                _stampBitmap = value ?? throw new ArgumentNullException(nameof(value), "Stamp bitmap cannot be null.");
            }
        }

        public override void Render(SKCanvas canvas)
        {
            if (StampBitmap != null)
            {
                using Bitmap stampBitmap = DrawingMethods.SetBitmapOpacity(StampBitmap.ToBitmap(), Opacity);

                using Bitmap scaledStamp = DrawingMethods.ScaleBitmap(stampBitmap, (int)(stampBitmap.Width * Scale), (int)(stampBitmap.Height * Scale));

                using SKBitmap rotatedAndScaledStamp = DrawingMethods.RotateSKBitmap(scaledStamp.ToSKBitmap(), Rotation, false);

                canvas.DrawBitmap(rotatedAndScaledStamp,
                    new SKPoint(TopLeft.X - (rotatedAndScaledStamp.Width / 2), TopLeft.Y - (rotatedAndScaledStamp.Height / 2)), null);

                Bounds = new SKRect(TopLeft.X - (rotatedAndScaledStamp.Width / 2), TopLeft.Y - (rotatedAndScaledStamp.Height / 2),
                                    TopLeft.X + (rotatedAndScaledStamp.Width / 2), TopLeft.Y + (rotatedAndScaledStamp.Height / 2));

                base.Render(canvas);
            }
        }
    }
}
