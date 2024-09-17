using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_ClearBackgroundTexture(RealmStudioMap map, MapImage textureBitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        MapImage LayerTexture { get; set; } = textureBitmap;

        public void DoOperation()
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (LayerTexture != null)
            {
                baseLayer.MapLayerComponents.Remove(LayerTexture);
            }

            // base layer is cleared to WHITE, not transparent or empty;
            baseLayer.LayerSurface?.Canvas.Clear(SKColors.White);
        }

        public void UndoOperation()
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.BASELAYER);

            if (baseLayer.MapLayerComponents.Count() <= 1)
            {
                baseLayer.MapLayerComponents.Add(LayerTexture);
            }
        }
    }
}
