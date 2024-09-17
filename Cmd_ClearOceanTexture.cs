using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_ClearOceanTexture(RealmStudioMap map, MapImage textureBitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        MapImage LayerTexture { get; set; } = textureBitmap;

        public void DoOperation()
        {
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTURELAYER);

            if (LayerTexture != null)
            {
                oceanTextureLayer.MapLayerComponents.Remove(LayerTexture);
            }

            oceanTextureLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        public void UndoOperation()
        {
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count() < 1)
            {
                oceanTextureLayer.MapLayerComponents.Add(LayerTexture);
            }
        }
    }
}
