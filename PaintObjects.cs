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

namespace RealmStudio
{
    internal class PaintObjects
    {
        public static readonly SKPaint LandformSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.Firebrick,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F),
        };

        public static readonly SKPaint LandColorPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };

        public static readonly SKPaint LandColorEraserPaint = new()
        {
            Color = SKColor.Empty,
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Src,
            IsAntialias = true,
        };

        public static SKPaint WaterFeatureSelectPaint { get; } = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.CadetBlue,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([3F, 3F], 6F)
        };

        public static SKPaint WaterColorPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            BlendMode = SKBlendMode.SrcOver
        };

        public static SKPaint WaterColorEraserPaint { get; } = new()
        {
            Color = SKColor.Empty,
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Src,
            IsAntialias = true
        };

        public static readonly SKPaint CursorCirclePaint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F),
        };

        public static readonly SKPaint ContourPathPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = false,
            Color = SKColors.Black,
            StrokeWidth = 1,
        };

        public static readonly SKPaint ContourMarginPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = false,
            Color = SKColors.White,
            StrokeWidth = 2,
        };

        // the SKPaint object used to draw the box around the selected symbol
        public static readonly SKPaint MapSymbolSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.LawnGreen,
            StrokeWidth = 1,
            PathEffect = SKPathEffect.CreateDash([3F, 3F], 6F),
        };

        public static readonly SKPaint RiverSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.BlueViolet,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F)
        };

        public static SKPaint RiverControlPointPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 2,
            Color = SKColors.WhiteSmoke
        };

        public static SKPaint RiverControlPointOutlinePaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeWidth = 1,
            Color = SKColors.Black
        };

        public static SKPaint RiverSelectedControlPointPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 2,
            Color = SKColors.BlueViolet,
        };

        public static SKPaint MapPathSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.BlueViolet,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F)
        };

        public static SKPaint MapPathSelectErasePaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.Empty,
            StrokeWidth = 2,
        };

        public static SKPaint MapPathControlPointPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 2,
            Color = SKColors.WhiteSmoke,
        };

        public static SKPaint MapPathSelectedControlPointPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 2,
            Color = SKColors.BlueViolet,
        };

        public static SKPaint MapPathControlPointOutlinePaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeWidth = 1,
            Color = SKColors.Black,
        };

        public static SKPaint LabelSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.Coral,
            StrokeWidth = 1,
            PathEffect = SKPathEffect.CreateDash([4F, 2F], 6F),
        };

        public static SKPaint LabelPathPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.Gray,
            StrokeWidth = 1,
            PathEffect = SKPathEffect.CreateDash([2F, 2F], 4F)
        };

        public static readonly SKPaint RegionSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.BlueViolet,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F),
        };

        public static readonly SKPaint RegionPointFillPaint = new()
        {
            Style = SKPaintStyle.StrokeAndFill,
            IsAntialias = true,
            Color = SKColors.White,
            StrokeWidth = 1,
        };

        public static readonly SKPaint RegionPointSelectedFillPaint = new()
        {
            Style = SKPaintStyle.StrokeAndFill,
            IsAntialias = true,
            Color = SKColors.Blue,
            StrokeWidth = 1,
        };

        public static readonly SKPaint RegionNewPointFillPaint = new()
        {
            Style = SKPaintStyle.StrokeAndFill,
            IsAntialias = true,
            Color = SKColors.Yellow,
            StrokeWidth = 1,
        };

        public static readonly SKPaint RegionPointOutlinePaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.Black,
            StrokeWidth = 1,
        };

        public static SKPaint OceanPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            BlendMode = SKBlendMode.SrcOver,
        };

        public static readonly SKPaint OceanEraserPaint = new()
        {
            Color = SKColor.Empty,
            Style = SKPaintStyle.Fill,
            BlendMode = SKBlendMode.Src,
        };

    }
}
