using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealmStudio
{
    internal class Cmd_SetOceanTexture(RealmStudioMap map, SKBitmap textureBitmap) : IMapOperation
    {
        public RealmStudioMap Map { get; set; } = map;
        private SKBitmap LayerTexture { get; set; } = textureBitmap;
        private MapImage? OceanTexture { get; set; }

        public void DoOperation()
        {
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count < 1)
            {
                OceanTexture = new()
                {
                    Width = LayerTexture.Width,
                    Height = LayerTexture.Height,
                    MapImageBitmap = LayerTexture.Copy()
                };
                oceanTextureLayer.MapLayerComponents.Add(OceanTexture);
            }
        }

        public void UndoOperation()
        {
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.OCEANTEXTURELAYER);

            if (OceanTexture != null)
            {
                oceanTextureLayer.MapLayerComponents.Remove(OceanTexture);
            }

            oceanTextureLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }
    }
}
