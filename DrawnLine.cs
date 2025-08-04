using SkiaSharp;

namespace RealmStudio
{
    internal class DrawnLine : DrawnMapComponent, IDisposable
    {
        private bool disposedValue;

        private List<SKPoint> _points = [];
        private SKPaint _paint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 2,
            IsStroke = true
        };

        public DrawnLine()
        {
            // Default constructor
        }

        public List<SKPoint> Points
        {
            get => _points;
            set
            {
                _points = value ?? throw new ArgumentNullException(nameof(value), "Points cannot be null.");
            }
        }

        public SKPaint Paint
        {
            get => _paint;
            set
            {
                _paint = value ?? throw new ArgumentNullException(nameof(value), "Paint cannot be null.");
            }
        }


        public override void Render(SKCanvas canvas)
        {
            for (int i = 0; i < _points.Count - 1; i++)
            {
                SKPoint start = _points[i];
                SKPoint end = _points[i + 1];

                canvas.DrawLine(start, end, Paint);
            }
        }

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Paint.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
