
using SkiaSharp;

namespace RealmStudio
{
    internal sealed class MapScaleManager : IMapComponentManager
    {
        private static MapScaleUIMediator? _scaleUIMediator;

        internal static MapScaleUIMediator? ScaleUIMediator
        {
            get { return _scaleUIMediator; }
            set { _scaleUIMediator = value; }
        }

        public static IMapComponent? Create(RealmStudioMap? map, IUIMediatorObserver? mediator)
        {
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(ScaleUIMediator);

            MapScale mapScale = new()
            {
                X = 100,
                Y = map.MapHeight - 100,
                Width = ScaleUIMediator.ScaleWidth,
                Height = ScaleUIMediator.ScaleHeight,
                ScaleSegmentCount = ScaleUIMediator.SegmentCount,
                ScaleLineWidth = ScaleUIMediator.ScaleLineWidth,
                ScaleColor1 = ScaleUIMediator.ScaleColor1,
                ScaleColor2 = ScaleUIMediator.ScaleColor2,
                ScaleColor3 = ScaleUIMediator.ScaleColor3,
                ScaleDistance = ScaleUIMediator.SegmentDistance,
                ScaleDistanceUnit = string.IsNullOrEmpty(ScaleUIMediator.ScaleUnitsText) ? "" : ScaleUIMediator.ScaleUnitsText,
                ScaleFontColor = ScaleUIMediator.ScaleFontColor,
                ScaleOutlineWidth = ScaleUIMediator.ScaleOutlineWidth,
                ScaleOutlineColor = ScaleUIMediator.ScaleNumberOutlineColor,
                ScaleFont = ScaleUIMediator.ScaleFont,
                ScaleNumbersDisplayType = ScaleUIMediator.ScaleNumbersDisplayType,
            };

            Cmd_AddMapScale cmd = new(MapStateMediator.CurrentMap, mapScale);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            return mapScale;
        }

        public static bool Delete(RealmStudioMap? map, IMapComponent? component)
        {
            // TODO: create and use IMapOperation command class for deleting the scale
            ArgumentNullException.ThrowIfNull(map);

            for (int i = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            return true;
        }

        public static IMapComponent? GetComponentById(RealmStudioMap? map, Guid componentGuid)
        {
            ArgumentNullException.ThrowIfNull(map);
            MapScale? mapScale = null;

            for (int i = MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale ms
                    && ms.ScaleGuid.ToString() == componentGuid.ToString())
                {
                    mapScale = ms;
                    break;
                }
            }

            return mapScale;
        }

        public static bool Update(RealmStudioMap? map, MapStateMediator? MapStateMediator, IUIMediatorObserver? mediator)
        {
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(ScaleUIMediator);

            if (MapStateMediator.CurrentMapScale != null)
            {
                MapScale? scale = (MapScale?)GetComponentById(map, MapStateMediator.CurrentMapScale.ScaleGuid);

                if (scale != null)
                {
                    scale.Width = ScaleUIMediator.ScaleWidth;
                    scale.Height = ScaleUIMediator.ScaleHeight;
                    scale.ScaleSegmentCount = ScaleUIMediator.SegmentCount;
                    scale.ScaleLineWidth = ScaleUIMediator.ScaleLineWidth;
                    scale.ScaleColor1 = ScaleUIMediator.ScaleColor1;
                    scale.ScaleColor2 = ScaleUIMediator.ScaleColor2;
                    scale.ScaleColor3 = ScaleUIMediator.ScaleColor3;
                    scale.ScaleDistance = ScaleUIMediator.SegmentDistance;
                    scale.ScaleDistanceUnit = string.IsNullOrEmpty(ScaleUIMediator.ScaleUnitsText) ? "" : ScaleUIMediator.ScaleUnitsText;
                    scale.ScaleFontColor = ScaleUIMediator.ScaleFontColor;
                    scale.ScaleOutlineWidth = ScaleUIMediator.ScaleOutlineWidth;
                    scale.ScaleOutlineColor = ScaleUIMediator.ScaleNumberOutlineColor;
                    scale.ScaleFont = ScaleUIMediator.ScaleFont;
                    scale.ScaleNumbersDisplayType = ScaleUIMediator.ScaleNumbersDisplayType;

                    MapStateMediator.CurrentMapScale = scale;
                }
            }

            return true;
        }

        internal static void MoveMapScale(MapScale mapScale, SKPoint zoomedScrolledPoint)
        {
            mapScale.X = (int)zoomedScrolledPoint.X - mapScale.Width / 2;
            mapScale.Y = (int)zoomedScrolledPoint.Y - mapScale.Height / 2;
        }

        internal static MapScale? SelectMapScale(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            MapScale? mapScale = null;

            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    mapScale = (MapScale?)MapBuilder.GetMapLayerByIndex(map, MapBuilder.OVERLAYLAYER).MapLayerComponents[i];
                }
            }

            if (mapScale != null)
            {
                SKRect scaleRect = new(mapScale.X, mapScale.Y, mapScale.X + mapScale.Width, mapScale.Y + mapScale.Height);

                if (scaleRect.Contains(zoomedScrolledPoint))
                {
                    mapScale.IsSelected = true;
                }
            }

            return mapScale;
        }
    }
}
