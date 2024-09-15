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

        public SKPath DrawPath { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        // inner paths are used to paint the gradient shading around the inside of the landform
        public SKPath InnerPath1 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath2 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath3 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath4 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath5 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath6 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath7 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath8 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        // outer paths are used to paint the coastline effect around the outside of the landform
        public SKPath OuterPath1 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath2 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath3 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath4 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath5 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath6 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath7 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath8 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public List<SKPoint> ContourPoints { get; set; } = [];

        private static SKPaint CoastlinePaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };

        private static SKPaint LandformGradientPaint = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            BlendMode = SKBlendMode.SrcATop
        };

        private static SKPaint LandformOutlinePaint = new()
        {
            Style = SKPaintStyle.StrokeAndFill,
            StrokeWidth = 1,
            IsAntialias = true,
            BlendMode = SKBlendMode.Src,
        };

        //private MapTexture? landformTexture;
        public Color LandformOutlineColor { get; set; } = ColorTranslator.FromHtml("#3D3728");
        public Color LandformFillColor { get; set; } = ColorTranslator.FromHtml("#AC964F");
        public int LandformOutlineWidth { get; set; } = 2;
        public GradientDirectionEnum ShorelineStyle { get; set; } = GradientDirectionEnum.None;
        public Color CoastlineColor { get; set; } = ColorTranslator.FromHtml("#BB9CC3B7");
        public int CoastlineEffectDistance { get; set; } = 16;
        public string CoastlineStyleName { get; set; } = "Dash Pattern";
        public string? CoastlineHatchPattern { get; set; } = string.Empty;
        public int CoastlineHatchOpacity { get; set; } = 0;
        public int CoastlineHatchScale { get; set; } = 0;
        public string? CoastlineHatchBlendMode { get; set; } = string.Empty;
        public bool PaintCoastlineGradient { get; set; } = true;

        public bool IsSelected { get; set; } = false;

        public void RenderCoastline(SKCanvas canvas)
        {
            double colorAlphaStep = 1.0 / (256.0 / 8.0);

            Color coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (1 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath8, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (2 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath7, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (3 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath6, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (4 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath5, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (5 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath4, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (6 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath3, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (7 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath2, CoastlinePaint);

            coastRenderColor = Color.FromArgb((int)(CoastlineColor.A * (32 * colorAlphaStep)), CoastlineColor);
            CoastlinePaint.Color = coastRenderColor.ToSKColor();
            canvas.DrawPath(OuterPath1, CoastlinePaint);
        }

        public void RenderLandform(SKCanvas canvas)
        {
            LandformOutlinePaint.Color = LandformOutlineColor.ToSKColor();
            canvas.DrawPath(DrawPath, LandformOutlinePaint);

            double colorAlphaStep = 1.0 / (256.0 / 8.0);

            Color landformColor = Color.FromArgb((int)(LandformFillColor.A * (32 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.BlendMode = SKBlendMode.Src;
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath8, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (28 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.BlendMode = SKBlendMode.SrcATop;
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath7, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (24 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath6, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (20 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath5, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (16 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath4, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (12 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath3, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (8 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath2, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (4 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath1, LandformGradientPaint);
        }

        public override void Render(SKCanvas canvas)
        {
            // no-op for landforms, because coastline and landform are rendered on different canvases
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
