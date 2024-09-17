using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_ClearOceanColor(RealmStudioMap map, MapImage colorBitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        MapImage LayerColor { get; set; } = colorBitmap;

        public void DoOperation()
        {
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (LayerColor != null)
            {
                oceanTextureOverlayLayer.MapLayerComponents.Remove(LayerColor);
            }

            oceanTextureOverlayLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        public void UndoOperation()
        {
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (oceanTextureOverlayLayer.MapLayerComponents.Count() < 1)
            {
                oceanTextureOverlayLayer.MapLayerComponents.Add(LayerColor);
            }
        }
    }
}
