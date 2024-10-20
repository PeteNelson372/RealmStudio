using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_RemoveSymbolsFromArea(RealmStudioMap map, float eraserRadius, SKPoint eraserPoint) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly float EraserCircleRadius = eraserRadius;
        private readonly SKPoint CenterPoint = eraserPoint;

        private List<MapSymbol> RemovedSymbolList { get; set; } = [];

        public void DoOperation()
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);

            for (int i = symbolLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol symbol)
                {
                    SKPoint symbolPoint = new(symbol.X, symbol.Y);

                    if (DrawingMethods.PointInCircle(EraserCircleRadius, CenterPoint, symbolPoint))
                    {
                        RemovedSymbolList.Add(symbol);

                        symbolLayer.MapLayerComponents.Remove(symbol);
                    }
                }
            }

            symbolLayer.IsModified = true;
        }

        public void UndoOperation()
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.SYMBOLLAYER);

            foreach (MapSymbol symbol in RemovedSymbolList)
            {
                symbolLayer.MapLayerComponents.Add(symbol);
            }

            RemovedSymbolList.Clear();
            symbolLayer.IsModified = true;
        }
    }
}
