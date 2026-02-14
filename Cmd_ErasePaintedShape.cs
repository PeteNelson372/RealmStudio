using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealmStudioX
{
    using SkiaSharp;
    using RealmStudioShapeRenderingLib;

    public sealed class Cmd_ErasePaintedShape : ICommand
    {
        private readonly PaintedShape _shape;
        private readonly SKPoint _point;
        private readonly float _radius;

        private SKPath? _before;
        private bool disposedValue;

        public Cmd_ErasePaintedShape(
            PaintedShape shape,
            SKPoint point,
            float radius)
        {
            _shape = shape;
            _point = point;
            _radius = radius;
        }

        public string? Description => "Erase from PaintedShape";

        public void Execute()
        {
            _before = new SKPath(_shape.HitPath);
            _shape.EraseCircle(_point, _radius);
        }

        public void Undo()
        {
            if (_before != null)
            {
                _shape.RestoreGeometry(_before);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _before?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_ErasePaintedShape()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
