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

        public static SKPaint WaterFeatureSelectPaint { get; } = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.CadetBlue,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([3F, 3F], 6F)
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
    }
}
