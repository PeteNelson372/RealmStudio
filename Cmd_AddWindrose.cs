using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_AddWindrose(RealmStudioMap map, MapWindrose windrose) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly MapWindrose Windrose = windrose;

        public void DoOperation()
        {
            MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WINDROSELAYER).MapLayerComponents.Add(Windrose);
        }

        public void UndoOperation()
        {
            MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WINDROSELAYER).MapLayerComponents.Remove(Windrose);
            MapBuilder.GetMapLayerByIndex(Map, MapBuilder.WINDROSELAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }
    }
}
