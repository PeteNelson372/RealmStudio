using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public class Cmd_AddShape(MapLayer layer, Shape2D shape) : ICommand
    {
        private readonly MapLayer _layer = layer;
        private readonly Shape2D _shape = shape;
        private bool disposedValue;

        public static string Description => "Add Shape";

        public void Execute() => _layer.Add(_shape);
        public void Undo() => _layer.Remove(_shape);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_AddShape()
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
