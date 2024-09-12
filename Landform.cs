using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    class Landform : MapComponent, IXmlSerializable
    {
        public string LandformName { get; set; } = string.Empty;
        public Guid LandformGuid { get; } = Guid.NewGuid();

        public SKPath ContourPath { get; set; } = new SKPath();

        public SKPath DrawPath { get; set; } = new SKPath();

        // inner paths are used to paint the gradient shading around the inside of the landform
        public SKPath InnerPath1 { get; set; } = new SKPath();
        public SKPath InnerPath2 { get; set; } = new SKPath();
        public SKPath InnerPath3 { get; set; } = new SKPath();
        public SKPath InnerPath4 { get; set; } = new SKPath();
        public SKPath InnerPath5 { get; set; } = new SKPath();
        public SKPath InnerPath6 { get; set; } = new SKPath();
        public SKPath InnerPath7 { get; set; } = new SKPath();
        public SKPath InnerPath8 { get; set; } = new SKPath();

        // outer paths are used to paint the coastline effect around the outside of the landform
        public SKPath OuterPath1 { get; set; } = new SKPath();
        public SKPath OuterPath2 { get; set; } = new SKPath();
        public SKPath OuterPath3 { get; set; } = new SKPath();
        public SKPath OuterPath4 { get; set; } = new SKPath();
        public SKPath OuterPath5 { get; set; } = new SKPath();
        public SKPath OuterPath6 { get; set; } = new SKPath();
        public SKPath OuterPath7 { get; set; } = new SKPath();
        public SKPath OuterPath8 { get; set; } = new SKPath();

        private static SKPaint CoastlinePaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };

        private static SKPaint LandformOutlinePaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };

        //private MapTexture? landformTexture;
        private Color landformOutlineColor = ColorTranslator.FromHtml("#3D3728");
        private int landformOutlineWidth = 2;
        private GradientDirectionEnum shorelineStyle = GradientDirectionEnum.None;
        private Color coastlineColor = ColorTranslator.FromHtml("#BB9CC3B7");
        private int coastlineEffectDistance = 16;
        private string coastlineStyleName = "Dash Pattern";
        private string? coastlineHatchPattern = string.Empty;
        private int coastlineHatchOpacity = 0;
        private int coastlineHatchScale = 0;
        private string? coastlineHatchBlendMode = string.Empty;
        private bool paintCoastlineGradient = true;


        public Landform()
        {
        }

        public override void Render(SKCanvas canvas)
        {
            double colorAlphaStep = 1.0 / (256.0 / 8.0);

            Color renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (1 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath8, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (2 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath7, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (3 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath6, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (4 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath5, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (5 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath4, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (6 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath3, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (7 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath2, CoastlinePaint);

            renderCoastColor = Color.FromArgb((int)(coastlineColor.A * (8 * colorAlphaStep)), coastlineColor);
            CoastlinePaint.Color = renderCoastColor.ToSKColor();
            canvas.DrawPath(OuterPath1, CoastlinePaint);

            LandformOutlinePaint.Color = landformOutlineColor.ToSKColor();

            canvas.DrawPath(DrawPath, LandformOutlinePaint);
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
