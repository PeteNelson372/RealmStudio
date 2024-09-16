using SkiaSharp;
using SkiaSharp.Components;
using SkiaSharp.Views.Desktop;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

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

        public SKPaint LandformFillPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            BlendMode = SKBlendMode.Src,
        };

        public SKPaint LandformGradientPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeWidth = 2,
            BlendMode = SKBlendMode.SrcATop
        };

        public SKPaint LandformOutlinePaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true,
            BlendMode = SKBlendMode.Src,
        };

        public SKPaint CoastlinePaint { get; set; } = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };

        public SKPaint CoastlineFillPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };

        public SKShader? DashShader { get; set; } = null;
        public SKShader? LineHatchBitmapShader { get; set; } = null;

        public MapTexture? LandformTexture { get; set; } = null;
        public Color LandformOutlineColor { get; set; } = ColorTranslator.FromHtml("#3D3728");
        public Color LandformFillColor { get; set; } = ColorTranslator.FromHtml("#AC964F");
        public int LandformOutlineWidth { get; set; } = 2;
        public GradientDirectionEnum ShorelineStyle { get; set; } = GradientDirectionEnum.None;
        public Color CoastlineColor { get; set; } = ColorTranslator.FromHtml("#BB9CC3B7");
        public int CoastlineEffectDistance { get; set; } = 16;
        public string CoastlineStyleName { get; set; } = "None";
        public string? CoastlineHatchPattern { get; set; } = string.Empty;
        public int CoastlineHatchOpacity { get; set; } = 0;
        public int CoastlineHatchScale { get; set; } = 0;
        public string? CoastlineHatchBlendMode { get; set; } = string.Empty;
        public bool PaintCoastlineGradient { get; set; } = true;

        public bool IsSelected { get; set; } = false;

        #region COASTLINE RENDERING METHODS
        /******************************************************************************************************* 
        * COASTLINE RENDERING METHODS
        *******************************************************************************************************/
        public void RenderCoastline(SKCanvas canvas)
        {
            if (!string.IsNullOrEmpty(CoastlineStyleName))
            {
                switch (CoastlineStyleName)
                {
                    case "None":
                        break;
                    case "Uniform Band":
                        DrawUniformBandCoastlineEffect(canvas);
                        break;
                    case "Uniform Blend":
                        DrawUniformBlendCoastlineEffect(canvas);
                        break;
                    case "Uniform Outline":
                        DrawUniformOutlineCoastlineEffect(canvas);
                        break;
                    case "Three-Tiered":
                        DrawThreeTieredCoastlineEffect(canvas);
                        break;
                    case "Circular Pattern":
                        DrawRadialPatternCoastlineEffect(canvas);
                        break;
                    case "Dash Pattern":
                        DrawDashPatternCoastlineEffect(canvas);
                        break;
                    case "Hatch Pattern":
                        DrawHatchPatternCoastlineEffect(canvas);
                        break;
                    case "User Defined":
                        DrawUserDefinedHatchEffect(canvas);
                        break;
                }
            }
        }

        #region USER-DEFINED COASTLINE
        private void DrawUserDefinedHatchEffect(SKCanvas canvas)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region HATCH PATTERN COASTLINE
        private void DrawHatchPatternCoastlineEffect(SKCanvas canvas)
        {
            double colorAlphaStep = 1.0 / (256.0 / 8.0);
            CoastlineFillPaint.StrokeWidth = CoastlineEffectDistance / 8.0F;

            // outer path 1
            Color gradientColor = Color.FromArgb((int)(CoastlineColor.A * (32 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath1, CoastlineFillPaint);

            // outer path 2
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (26 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath2, CoastlineFillPaint);

            // outer path 3
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (20 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath3, CoastlineFillPaint);

            // outer path 4
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (14 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath4, CoastlineFillPaint);

            // outer path 5
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (8 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath5, CoastlineFillPaint);

            // outer path 6
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (4 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath6, CoastlineFillPaint);

            // outer path 7
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (2 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath7, CoastlineFillPaint);

            // outer path 8
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (1 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                LineHatchBitmapShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath8, CoastlineFillPaint);
        }
        #endregion

        #region DASH PATTERN COASTLINE
        private void DrawDashPatternCoastlineEffect(SKCanvas canvas)
        {
            double colorAlphaStep = 1.0 / (256.0 / 8.0);
            CoastlineFillPaint.StrokeWidth = CoastlineEffectDistance / 8.0F;
         
            // outer path 1
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(CoastlineColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath1, CoastlineFillPaint);

            // outer path 2
            Color gradientColor = Color.FromArgb((int)(CoastlineColor.A * (8 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath2, CoastlineFillPaint);

            // outer path 3
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (7 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath3, CoastlineFillPaint);

            // outer path 4
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (6 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath4, CoastlineFillPaint);

            // outer path 5
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (5 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath5, CoastlineFillPaint);

            // outer path 6
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (4 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath6, CoastlineFillPaint);

            // outer path 7
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (3 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath7, CoastlineFillPaint);

            // outer path 8
            gradientColor = Color.FromArgb((int)(CoastlineColor.A * (2 * colorAlphaStep)), CoastlineColor);

            CoastlineFillPaint.Shader?.Dispose();
            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(Extensions.ToSKColor(gradientColor)),
                DashShader,
                SKBlendMode.Modulate);

            canvas.DrawPath(OuterPath8, CoastlineFillPaint);

        }
        #endregion

        #region RADIAL PATTERN COASTLINE
        private void DrawRadialPatternCoastlineEffect(SKCanvas canvas)
        {
            Color coastlineBandColor = CoastlineColor;

            float coastEffectPathWidth = CoastlineEffectDistance / 8.0F;

            SKRect pathBounds = ContourPath.Bounds;

            SKShader gradient = SKShader.CreateRadialGradient(new SKPoint(pathBounds.MidX, pathBounds.MidY), coastEffectPathWidth, [Extensions.ToSKColor(coastlineBandColor), SKColors.Transparent], SKShaderTileMode.Mirror);

            // outer path 1
            SKColor gradientColor = CoastlineColor.ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath1, CoastlineFillPaint);

            // outer path 2
            gradientColor = Color.FromArgb(CoastlineColor.A / 2, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath2, CoastlineFillPaint);

            // outer path 3
            gradientColor = Color.FromArgb(CoastlineColor.A / 3, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath3, CoastlineFillPaint);

            // outer path 4
            gradientColor = Color.FromArgb(CoastlineColor.A / 4, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath4, CoastlineFillPaint);

            // outer path 5
            gradientColor = Color.FromArgb(CoastlineColor.A / 5, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath5, CoastlineFillPaint);

            // outer path 6
            gradientColor = Color.FromArgb(CoastlineColor.A / 6, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath6, CoastlineFillPaint);

            // outer path 7
            gradientColor = Color.FromArgb(CoastlineColor.A / 7, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath7, CoastlineFillPaint);

            // outer path 8
            gradientColor = Color.FromArgb(CoastlineColor.A / 8, CoastlineColor).ToSKColor();

            CoastlineFillPaint.Shader = SKShader.CreateCompose(
                SKShader.CreateColor(gradientColor),
                gradient,
                SKBlendMode.Plus);

            canvas.DrawPath(OuterPath8, CoastlineFillPaint);
        }
        #endregion

        #region THREE-TIER COASTLINE
        private void DrawThreeTieredCoastlineEffect(SKCanvas canvas)
        {
            CoastlineFillPaint.Shader?.Dispose();

            CoastlineFillPaint.StrokeWidth = CoastlineEffectDistance / 8.0F;

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 1.0F), CoastlineColor));

            // draw gradient path OuterPath1
            canvas.DrawPath(OuterPath1, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 1.0F), CoastlineColor));

            // draw gradient path OuterPath2
            canvas.DrawPath(OuterPath2, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath3
            canvas.DrawPath(OuterPath3, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath4
            canvas.DrawPath(OuterPath4, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath5
            canvas.DrawPath(OuterPath5, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 4.0F), CoastlineColor));

            // draw gradient path OuterPath6
            canvas.DrawPath(OuterPath6, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 4.0F), CoastlineColor));

            // draw gradient path OuterPath7
            canvas.DrawPath(OuterPath7, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 4.0F), CoastlineColor));

            // draw gradient path OuterPath8
            canvas.DrawPath(OuterPath8, CoastlineFillPaint);
        }
        #endregion

        #region UNIFORM OUTLINE COASTLINE
        private void DrawUniformOutlineCoastlineEffect(SKCanvas canvas)
        {
            CoastlineFillPaint.Shader?.Dispose();

            CoastlineFillPaint.StrokeWidth = CoastlineEffectDistance / 8.0F;

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath1
            canvas.DrawPath(OuterPath1, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath2
            canvas.DrawPath(OuterPath2, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath3
            canvas.DrawPath(OuterPath3, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath4
            canvas.DrawPath(OuterPath4, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath5
            canvas.DrawPath(OuterPath5, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 2.0F), CoastlineColor));

            // draw gradient path OuterPath6
            canvas.DrawPath(OuterPath6, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Style = SKPaintStyle.Stroke;

            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 1.0F), CoastlineColor));

            // draw gradient path OuterPath7
            canvas.DrawPath(OuterPath7, CoastlineFillPaint);

            //===========
            CoastlineFillPaint.Color = Extensions.ToSKColor(Color.FromArgb((byte)(CoastlineColor.A / 1.0F), CoastlineColor));

            // draw gradient path OuterPath8
            canvas.DrawPath(OuterPath8, CoastlineFillPaint);
        }
        #endregion

        #region UNIFORM BLEND COASTLINE
        private void DrawUniformBlendCoastlineEffect(SKCanvas canvas)
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
        #endregion

        #region UNIFORM BAND COASTLINE
        private void DrawUniformBandCoastlineEffect(SKCanvas canvas)
        {
            CoastlineFillPaint.Shader?.Dispose();

            CoastlineFillPaint.StrokeWidth = CoastlineEffectDistance / 8.0F;

            CoastlineFillPaint.Color = CoastlineColor.ToSKColor();

            // draw gradient path OuterPath1
            canvas.DrawPath(OuterPath1, CoastlineFillPaint);

            // draw gradient path OuterPath2
            canvas.DrawPath(OuterPath2, CoastlineFillPaint);

            // draw gradient path OuterPath3
            canvas.DrawPath(OuterPath3, CoastlineFillPaint);

            // draw gradient path OuterPath4
            canvas.DrawPath(OuterPath4, CoastlineFillPaint);

            // draw gradient path OuterPath5
            canvas.DrawPath(OuterPath5, CoastlineFillPaint);

            // draw gradient path OuterPath6
            canvas.DrawPath(OuterPath6, CoastlineFillPaint);

            // draw gradient path OuterPath7
            canvas.DrawPath(OuterPath7, CoastlineFillPaint);

            // draw gradient path OuterPath8
            canvas.DrawPath(OuterPath8, CoastlineFillPaint);
        }
        #endregion

        #endregion

        #region LANDFORM RENDERING METHODS
        /******************************************************************************************************* 
        * LANDFORM RENDERING METHODS
        *******************************************************************************************************/

        public void RenderLandform(SKCanvas canvas)
        {
            canvas.DrawPath(DrawPath, LandformFillPaint);

            double colorAlphaStep = 1.0 / (256.0 / 8.0);

            Color landformColor = Color.FromArgb((int)(LandformFillColor.A * (4 * colorAlphaStep)), LandformFillColor);
            
            LandformGradientPaint.BlendMode = SKBlendMode.SrcATop;
            LandformGradientPaint.Color = landformColor.ToSKColor();
            LandformGradientPaint.StrokeWidth = CoastlineEffectDistance / 8;

            canvas.DrawPath(InnerPath8, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (8 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath7, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (12 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath6, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (16 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath5, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (20 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath4, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (24 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath3, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (28 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath2, LandformGradientPaint);

            landformColor = Color.FromArgb((int)(LandformFillColor.A * (32 * colorAlphaStep)), LandformFillColor);
            LandformGradientPaint.Color = landformColor.ToSKColor();
            canvas.DrawPath(InnerPath1, LandformGradientPaint);

            LandformOutlinePaint.Color = LandformOutlineColor.ToSKColor();
            canvas.DrawPath(ContourPath, LandformOutlinePaint);
        }

        #endregion

        #region XML SERIALIZATION
        /******************************************************************************************************* 
        * XML SERIALIZATION METHODS
        *******************************************************************************************************/
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
        #endregion

        #region NO-OP RENDER METHOD
        public override void Render(SKCanvas canvas)
        {
            // no-op for landforms, because coastline and landform are rendered on different canvases
        }
        #endregion
    }
}
