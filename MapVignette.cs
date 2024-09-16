using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapVignette : MapComponent
    {
        [XmlIgnore]
        public RealmStudioMap? Map { get; set; }

        [XmlElement]
        public int VignetteStrength { get; set; } = 64;

        [XmlElement]
        public XmlColor VignetteColor { get; set; } = ColorTranslator.FromHtml("#C9977B");

        public MapVignette(RealmStudioMap parentMap)
        {
            Map = parentMap;
        }

        public MapVignette() { }

        public override void Render(SKCanvas canvas)
        {
            if (Map != null)
            {
                SKRect bounds = new(0, 0, Map.MapWidth, Map.MapHeight);

                SKColor gradientColor = ((Color)VignetteColor).ToSKColor();

                int tenthLeftRight = (int)(bounds.Width / 5);
                int tenthTopBottom = (int)(bounds.Height / 5);

                using SKShader linGradLR = SKShader.CreateLinearGradient(new SKPoint(0, bounds.Height / 2), new SKPoint(tenthLeftRight / 2, bounds.Height / 2), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);
                using SKShader linGradTB = SKShader.CreateLinearGradient(new SKPoint(bounds.Width / 2, 0), new SKPoint(bounds.Width / 2, tenthTopBottom), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);
                using SKShader linGradRL = SKShader.CreateLinearGradient(new SKPoint(bounds.Width, bounds.Height / 2), new SKPoint(bounds.Width - tenthLeftRight, bounds.Height / 2), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);
                using SKShader linGradBT = SKShader.CreateLinearGradient(new SKPoint(bounds.Width / 2, bounds.Height), new SKPoint(bounds.Width / 2, bounds.Height - tenthTopBottom), [gradientColor.WithAlpha((byte)VignetteStrength), SKColors.Transparent], SKShaderTileMode.Clamp);

                using SKPaint paint = new()
                {
                    Shader = linGradLR,
                    IsAntialias = true,
                    Color = gradientColor,
                };

                SKRect rect = new(0, 0, tenthLeftRight, bounds.Height);
                canvas.DrawRect(rect, paint);

                paint.Shader = linGradTB;
                rect = new(0, 0, bounds.Width, tenthTopBottom);
                canvas.DrawRect(rect, paint);

                paint.Shader = linGradRL;
                rect = new(bounds.Width, 0, bounds.Width - tenthLeftRight, bounds.Height);
                canvas.DrawRect(rect, paint);

                paint.Shader = linGradBT;
                rect = new(0, bounds.Height - tenthTopBottom, bounds.Width, bounds.Height);
                canvas.DrawRect(rect, paint);
            }
        }
    }
}
