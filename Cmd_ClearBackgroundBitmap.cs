using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_ClearBackgroundBitmap(RealmStudioMap map, MapImage bitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        MapImage LayerBitmap { get; set; } = bitmap;

        public void DoOperation()
        {
            MapLayer backgroundLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (LayerBitmap != null)
            {
                backgroundLayer.MapLayerComponents.Remove(LayerBitmap);
            }

            // base layer is cleared to WHITE, not transparent or empty
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);
            baseLayer.LayerSurface?.Canvas.Clear(SKColors.White);
        }

        public void UndoOperation()
        {
            MapLayer backgroundLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (backgroundLayer.MapLayerComponents.Count() <= 1)
            {
                backgroundLayer.MapLayerComponents.Add(LayerBitmap);
            }
        }
    }
}
