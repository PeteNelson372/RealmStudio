namespace RealmStudio
{
    internal class Cmd_DeleteLabel(RealmStudioMap map, MapLabel label) : IMapOperation
    {
        private readonly RealmStudioMap Map = map;
        private readonly MapLabel Label = label;

        public void DoOperation()
        {
            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LABELLAYER);

            for (int i = labelLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (labelLayer.MapLayerComponents[i] is MapLabel l && l.LabelGuid.ToString() == Label.LabelGuid.ToString())
                {
                    labelLayer.MapLayerComponents.RemoveAt(i);
                }
            }

            labelLayer.IsModified = true;
        }

        public void UndoOperation()
        {
            Label.IsSelected = false;

            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.LABELLAYER);
            labelLayer.MapLayerComponents.Add(Label);
            labelLayer.IsModified = true;

        }
    }
}
