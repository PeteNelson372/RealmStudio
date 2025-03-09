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
    internal class MapMeasure : MapComponent
    {
        private readonly RealmStudioMap Map;

        public MapMeasure(RealmStudioMap map)
        {
            Map = map;
            MeasureLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                Color = MeasureLineColor.ToSKColor()
            };

            MeasureAreaPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 1,
                Color = MeasureLineColor.ToSKColor()
            };

            MeasureValuePaint = new()
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            MeasureValueOutlinePaint = new()
            {
                Color = SKColors.Black,
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDilate(1, 1)
            };
        }

        public Color MeasureLineColor { get; set; } = Color.FromArgb(191, 138, 26, 0);

        public Font MeasureValueFont { get; set; } = new Font("Tahoma", 8.0F, FontStyle.Regular, GraphicsUnit.Point, 0);

        public bool UseMapUnits { get; set; }

        public bool MeasureArea { get; set; }

        public List<SKPoint> MeasurePoints { get; set; } = [];

        public SKPaint MeasureLinePaint { get; set; }

        public SKPaint MeasureAreaPaint { get; set; }

        public SKPaint MeasureValuePaint { get; set; }

        public SKPaint MeasureValueOutlinePaint { get; set; }

        public float TotalMeasureLength { get; set; }

        public bool RenderValue;

        public override void Render(SKCanvas canvas)
        {
            if (MeasurePoints.Count >= 2)
            {
                if (MeasureArea && MeasurePoints.Count > 2)
                {
                    SKPath path = new();

                    path.MoveTo(MeasurePoints.First());

                    for (int i = 1; i < MeasurePoints.Count; i++)
                    {
                        path.LineTo(MeasurePoints[i]);
                    }

                    path.Close();

                    canvas.DrawPath(path, MeasureAreaPaint);
                }
                else
                {
                    for (int i = 0; i < MeasurePoints.Count - 1; i++)
                    {
                        canvas.DrawLine(MeasurePoints[i], MeasurePoints[i + 1], MeasureLinePaint);
                    }
                }

                if (RenderValue)
                {
                    // render measure value and units
                    SKPoint measureValuePoint = new(MeasurePoints.Last().X + 30, MeasurePoints.Last().Y + 20);
                    RenderDistanceLabel(canvas, measureValuePoint, TotalMeasureLength);

                    if (MeasureArea)
                    {
                        float area = DrawingMethods.CalculatePolygonArea(MeasurePoints);

                        SKPoint measureAreaPoint = new(MeasurePoints.Last().X + 30, MeasurePoints.Last().Y + 40);

                        RenderAreaLabel(canvas, measureAreaPoint, area);
                    }
                }
            }
        }

        public void RenderDistanceLabel(SKCanvas? canvas, SKPoint labelPoint, float distance)
        {
            SKFont measureValueSkFont = new(SKTypeface.FromFamilyName(MeasureValueFont.FontFamily.Name), (MeasureValueFont.Size * 4.0F) / 3.0F);

            if (UseMapUnits && !string.IsNullOrEmpty(Map.MapAreaUnits))
            {
                string lblText = string.Format("{0} {1}", (int)(distance * Map.MapPixelWidth), Map.MapAreaUnits);

                canvas?.DrawText(lblText, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValueOutlinePaint);
                canvas?.DrawText(lblText, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValuePaint);
            }
            else
            {
                string lblText = string.Format("{0} {1}", (int)distance, "pixels");

                canvas?.DrawText(lblText, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValueOutlinePaint);
                canvas?.DrawText(lblText, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValuePaint);
            }
        }

        public void RenderAreaLabel(SKCanvas? canvas, SKPoint labelPoint, float measuredArea)
        {
            SKFont measureValueSkFont = new(SKTypeface.FromFamilyName(MeasureValueFont.FontFamily.Name), (MeasureValueFont.Size * 4.0F) / 3.0F);

            if (UseMapUnits && !string.IsNullOrEmpty(Map.MapAreaUnits))
            {
                string labelString = string.Format("{0} {1}", (int)(measuredArea * (Map.MapPixelWidth * Map.MapPixelWidth)), Map.MapAreaUnits + "\xB2");

                canvas?.DrawText(labelString, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValueOutlinePaint);
                canvas?.DrawText(labelString, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValuePaint);
            }
            else
            {
                string labelString = string.Format("{0} {1}", (int)measuredArea, "pixels\xB2");

                canvas?.DrawText(labelString, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValueOutlinePaint);
                canvas?.DrawText(labelString, labelPoint, SKTextAlign.Center, measureValueSkFont, MeasureValuePaint);
            }
        }
    }
}
