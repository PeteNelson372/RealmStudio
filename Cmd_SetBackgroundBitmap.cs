using SkiaSharp.Components;
using SkiaSharp;
using System.Drawing;

namespace RealmStudio
{
    internal class Cmd_SetBackgroundBitmap(RealmStudioMap map, SKBitmap bitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        private SKBitmap LayerBitmap { get; set; } = bitmap;
        private MapImage? BackgroundTexture { get; set; }

        public void DoOperation()
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (baseLayer.MapLayerComponents.Count() < 1)
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
            MapLayer backgroundLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (BackgroundTexture != null)
            {
                backgroundLayer.MapLayerComponents.Remove(BackgroundTexture);
            }

            // base layer is cleared to WHITE, not transparent or empty
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);
            baseLayer.LayerSurface?.Canvas.Clear(SKColors.White);
        }
    }
}
