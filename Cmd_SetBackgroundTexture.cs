using SkiaSharp.Components;
using SkiaSharp;
using System.Drawing;

namespace RealmStudio
{
    internal class Cmd_SetBackgroundTexture(RealmStudioMap map, SKBitmap textureBitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        private SKBitmap LayerBitmap { get; set; } = textureBitmap;
        private MapImage? BackgroundTexture { get; set; }

        public void DoOperation()
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (baseLayer.MapLayerComponents.Count < 1)
            {
                BackgroundTexture = new()
                {
                    Width = LayerBitmap.Width,
                    Height = LayerBitmap.Height,
                    MapImageBitmap = LayerBitmap.Copy()
                };
                baseLayer.MapLayerComponents.Add(BackgroundTexture);
            }
        }

        public void UndoOperation()
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (BackgroundTexture != null)
            {
                baseLayer.MapLayerComponents.Remove(BackgroundTexture);
            }

            // base layer is cleared to WHITE, not transparent or empty
            baseLayer.LayerSurface?.Canvas.Clear(SKColors.White);
        }
    }
}
