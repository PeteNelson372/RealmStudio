using RealmStudioShapeRenderingLib;
using SkiaSharp;

namespace RealmStudioX
{
    public sealed class MapScene
    {
        public RealmStudioMap Map { get; }
        public Camera2D Camera { get; }

        public IReadOnlyList<MapLayer> Layers => Map.MapLayers;

        public MapScene(RealmStudioMap map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Map.Validate();
            Camera = new Camera2D();
        }

        public SKRect WorldBounds =>
            new(
                0, 0,
                Map.MapWidth,
                Map.MapHeight
            );

        public void Render(SKCanvas canvas)
        {
            ArgumentNullException.ThrowIfNull(canvas);

            // Clear background (outside camera transform)
            canvas.Clear(SKColors.White);

            // Apply camera transform
            Camera.Apply(canvas);

            // Render layers in order
            foreach (var layer in Layers.OrderBy(l => l.MapLayerOrder))
            {
                layer.Render(canvas);
            }

            // No Restore needed; camera uses Translate + Scale
        }
    }



}
