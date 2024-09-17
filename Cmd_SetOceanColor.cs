using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_SetOceanColor(RealmStudioMap map, SKBitmap colorBitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        private SKBitmap LayerColorBitmap { get; set; } = colorBitmap;
        private MapImage? OceanColor { get; set; }

        public void DoOperation()
        {
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (oceanTextureOverlayLayer.MapLayerComponents.Count < 1)
            {
                OceanColor = new()
                {
                    Width = LayerColorBitmap.Width,
                    Height = LayerColorBitmap.Height,
                    MapImageBitmap = LayerColorBitmap.Copy()
                };
                oceanTextureOverlayLayer.MapLayerComponents.Add(OceanColor);
            }
        }

        public void UndoOperation()
        {
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (OceanColor != null)
            {
                oceanTextureOverlayLayer.MapLayerComponents.Remove(OceanColor);
            }

            oceanTextureOverlayLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }
    }
}
