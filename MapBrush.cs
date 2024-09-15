namespace RealmStudio
{
    internal class MapBrush
    {
        public required string BrushName { get; set; }

        public Bitmap? BrushBitmap { get; set; }

        public required string BrushPath { get; set; }

        public Color BrushColor { get; set; } = Color.Black;

        public Size BrushSize { get; set; } = Size.Empty;
    }
}
