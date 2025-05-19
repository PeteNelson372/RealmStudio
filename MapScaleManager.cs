
using SkiaSharp;

namespace RealmStudio
{
    internal sealed class MapScaleManager : IMapComponentManager
    {
        private static MapScaleUIMediator? _scaleMediator;

        internal static MapScaleUIMediator? ScaleMediator
        {
            get { return _scaleMediator; }
            set { _scaleMediator = value; }
        }

        public static IMapComponent? Create()
        {
            ArgumentNullException.ThrowIfNull(ScaleMediator);

            MapScale mapScale = new()
            {
                X = 100,
                Y = MapStateMediator.CurrentMap.MapHeight - 100,
                Width = ScaleMediator.ScaleWidth,
                Height = ScaleMediator.ScaleHeight,
                ScaleSegmentCount = ScaleMediator.SegmentCount,
                ScaleLineWidth = ScaleMediator.ScaleLineWidth,
                ScaleColor1 = ScaleMediator.ScaleColor1,
                ScaleColor2 = ScaleMediator.ScaleColor2,
                ScaleColor3 = ScaleMediator.ScaleColor3,
                ScaleDistance = ScaleMediator.SegmentDistance,
                ScaleDistanceUnit = string.IsNullOrEmpty(ScaleMediator.ScaleUnitsText) ? "" : ScaleMediator.ScaleUnitsText,
                ScaleFontColor = ScaleMediator.ScaleFontColor,
                ScaleOutlineWidth = ScaleMediator.ScaleOutlineWidth,
                ScaleOutlineColor = ScaleMediator.ScaleNumberOutlineColor,
                ScaleFont = ScaleMediator.ScaleFont,
                ScaleNumbersDisplayType = ScaleMediator.ScaleNumbersDisplayType,
            };

            Cmd_AddMapScale cmd = new(MapStateMediator.CurrentMap, mapScale);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            return mapScale;
        }

        public static bool Delete()
        {
            // TODO: create and use IMapOperation command class for deleting the scale

            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            return true;
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            MapScale? mapScale = null;

            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale ms
                    && ms.ScaleGuid.ToString() == componentGuid.ToString())
                {
                    mapScale = ms;
                    break;
                }
            }

            return mapScale;
        }

        public static bool Update()
        {
            ArgumentNullException.ThrowIfNull(ScaleMediator);

            if (MapStateMediator.CurrentMapScale != null)
            {
                MapScale? scale = (MapScale?)GetComponentById(MapStateMediator.CurrentMapScale.ScaleGuid);

                if (scale != null)
                {
                    scale.Width = ScaleMediator.ScaleWidth;
                    scale.Height = ScaleMediator.ScaleHeight;
                    scale.ScaleSegmentCount = ScaleMediator.SegmentCount;
                    scale.ScaleLineWidth = ScaleMediator.ScaleLineWidth;
                    scale.ScaleColor1 = ScaleMediator.ScaleColor1;
                    scale.ScaleColor2 = ScaleMediator.ScaleColor2;
                    scale.ScaleColor3 = ScaleMediator.ScaleColor3;
                    scale.ScaleDistance = ScaleMediator.SegmentDistance;
                    scale.ScaleDistanceUnit = string.IsNullOrEmpty(ScaleMediator.ScaleUnitsText) ? "" : ScaleMediator.ScaleUnitsText;
                    scale.ScaleFontColor = ScaleMediator.ScaleFontColor;
                    scale.ScaleOutlineWidth = ScaleMediator.ScaleOutlineWidth;
                    scale.ScaleOutlineColor = ScaleMediator.ScaleNumberOutlineColor;
                    scale.ScaleFont = ScaleMediator.ScaleFont;
                    scale.ScaleNumbersDisplayType = ScaleMediator.ScaleNumbersDisplayType;

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

        internal static MapScale? SelectMapScale(SKPoint zoomedScrolledPoint)
        {
            MapScale? mapScale = null;

            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    mapScale = (MapScale?)MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OVERLAYLAYER).MapLayerComponents[i];
                }
            }

            if (mapScale != null)
            {
                SKRect scaleRect = new(mapScale.X, mapScale.Y, mapScale.X + mapScale.Width, mapScale.Y + mapScale.Height);

                if (scaleRect.Contains(zoomedScrolledPoint))
                {
                    mapScale.IsSelected = true;
                }
                else
                {
                    mapScale.IsSelected = false;
                }
            }

            return mapScale;
        }
    }
}
