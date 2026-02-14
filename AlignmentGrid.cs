/**************************************************************************************************************************
* Copyright 2026, Peter R. Nelson
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
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public class AlignmentGrid : MapComponent
    {
        public RealmStudioMap? ParentMap { get; set; }

        public Guid GridGuid { get; set; } = Guid.NewGuid();

        public bool GridEnabled { get; set; }

        public Color GridColor { get; set; } = Color.FromArgb(128, 128, 128, 128);

        public int GridLayerIndex { get; set; } = MapBuilder.DEFAULTGRIDLAYER;

        public int GridSize { get; set; } = 64;

        public int GridLineWidth { get; set; } = 1;

        public SKPaint? GridPaint { get; set; }

        public AlignmentGrid() { }

        public override void Render(SKCanvas canvas)
        {
            if (GridEnabled)
            {
                GridPaint ??= new()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = GridColor.ToSKColor(),
                    StrokeWidth = GridLineWidth,
                    StrokeJoin = SKStrokeJoin.Bevel,
                    PathEffect = SKPathEffect.CreateDash([3F, 3F], 6F)
                };

                RenderSquareGrid(canvas);
            }
        }

        private void RenderSquareGrid(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            int numHorizontalLines = (int)(Height / GridSize);
            int numVerticalLines = (int)(Width / GridSize);

            int xOffset = GridSize;
            int yOffset = GridSize;

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

                for (int i = 0; i < numHorizontalLines; i++)
                {
                    SKPoint startPoint = new(0, yOffset);
                    SKPoint endPoint = new(Width, yOffset);
                    canvas.DrawLine(startPoint, endPoint, GridPaint);

                    yOffset += GridSize;
                }

                for (int j = 0; j < numVerticalLines; j++)
                {
                    SKPoint startPoint = new(xOffset, 0);
                    SKPoint endPoint = new(xOffset, Height);
                    canvas.DrawLine(startPoint, endPoint, GridPaint);

                    xOffset += GridSize;
                }

            }
        }
    }
}
