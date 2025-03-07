/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class Landform : MapComponent, IXmlSerializable
    {
        public RealmStudioMap ParentMap { get; set; } = new RealmStudioMap();

        public string LandformName { get; set; } = string.Empty;

        public Guid LandformGuid { get; set; } = Guid.NewGuid();

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

        public SKPaint LandformHeightMapFillPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = false,
            BlendMode = SKBlendMode.Src,
            Color = Color.FromArgb(255, 25, 25, 25).ToSKColor()
        };

        public SKPaint LandformHeightMapOutlinePaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = false,
            BlendMode = SKBlendMode.Src,
            Color = Color.FromArgb(255, 25, 25, 25).ToSKColor()
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

        public SKShader? DashShader { get; set; }
        public SKShader? LineHatchBitmapShader { get; set; }
        public MapTexture? LandformTexture { get; set; }
        public Color LandformOutlineColor { get; set; } = Color.FromArgb(255, 62, 55, 40);
        public Color LandformBackgroundColor { get; set; } = Color.White;
        public Color LandformFillColor { get; set; } = ColorTranslator.FromHtml("#AC964F");
        public int LandformOutlineWidth { get; set; } = 2;
        public GradientDirectionEnum ShorelineStyle { get; set; } = GradientDirectionEnum.None;
        public Color CoastlineColor { get; set; } = ColorTranslator.FromHtml("#BB9CC3B7");
        public int CoastlineEffectDistance { get; set; } = 16;
        public string CoastlineStyleName { get; set; } = "None";
        public string? CoastlineHatchPattern { get; set; } = string.Empty;
        public int CoastlineHatchOpacity { get; set; }
        public int CoastlineHatchScale { get; set; }
        public string? CoastlineHatchBlendMode { get; set; } = string.Empty;
        public bool PaintCoastlineGradient { get; set; } = true;
        public bool FillWithTexture { get; set; } = true;
        public bool IsSelected { get; set; }
        public bool IsModified { get; set; } = true;

        public SKSurface? LandformRenderSurface { get; set; }
        public SKSurface? CoastlineRenderSurface { get; set; }

        public Landform() { }

        public Landform(Landform original)
        {
            ParentMap = original.ParentMap;
            DrawPath = new(original.DrawPath);
            CoastlineColor = original.CoastlineColor;
            CoastlineEffectDistance = original.CoastlineEffectDistance;
            CoastlineFillPaint = original.CoastlineFillPaint.Clone();
            CoastlineHatchBlendMode = original.CoastlineHatchBlendMode;
            CoastlineHatchOpacity = original.CoastlineHatchOpacity;
            CoastlineHatchScale = original.CoastlineHatchScale;
            CoastlinePaint = original.CoastlinePaint.Clone();
            CoastlineStyleName = original.CoastlineStyleName;
            DashShader = original.DashShader;
            LandformFillColor = original.LandformFillColor;
            LandformFillPaint = original.LandformFillPaint.Clone();
            LandformGradientPaint = original.LandformGradientPaint.Clone();
            LandformName = original.LandformName;
            LandformOutlineColor = original.LandformOutlineColor;
            LandformOutlineWidth = original.LandformOutlineWidth;
            FillWithTexture = original.FillWithTexture;

            if (original.LandformTexture != null)
            {
                LandformTexture = new(original.LandformTexture.TextureName, original.LandformTexture.TexturePath);
                LandformTexture.TextureBitmap = (Bitmap?)Bitmap.FromFile(LandformTexture.TexturePath);
            }

            LineHatchBitmapShader = original.LineHatchBitmapShader;
            PaintCoastlineGradient = original.PaintCoastlineGradient;
            ShorelineStyle = original.ShorelineStyle;
        }

        #region NO-OP RENDER METHOD
        public override void Render(SKCanvas canvas)
        {
            // no-op for landforms, because coastline and landform are rendered on different layers
        }
        #endregion

        #region COASTLINE RENDERING METHODS
        /******************************************************************************************************* 
        * COASTLINE RENDERING METHODS
        *******************************************************************************************************/
        public void RenderCoastline(SKCanvas canvas)
        {
            if (CoastlineRenderSurface == null)
            {
                return;
            }

            if (!IsModified)
            {
                canvas.DrawSurface(CoastlineRenderSurface, new SKPoint(0, 0));
            }
            else
            {
                SKCanvas coastlineCanvas = CoastlineRenderSurface.Canvas;

                coastlineCanvas.Clear(SKColors.Transparent);

                if (!string.IsNullOrEmpty(CoastlineStyleName))
                {
                    switch (CoastlineStyleName)
                    {
                        case "None":
                            break;
                        case "Uniform Band":
                            DrawUniformBandCoastlineEffect(coastlineCanvas);
                            break;
                        case "Uniform Blend":
                            DrawUniformBlendCoastlineEffect(coastlineCanvas);
                            break;
                        case "Uniform Outline":
                            DrawUniformOutlineCoastlineEffect(coastlineCanvas);
                            break;
                        case "Three-Tiered":
                            DrawThreeTieredCoastlineEffect(coastlineCanvas);
                            break;
                        case "Circular Pattern":
                            DrawRadialPatternCoastlineEffect(coastlineCanvas);
                            break;
                        case "Dash Pattern":
                            DrawDashPatternCoastlineEffect(coastlineCanvas);
                            break;
                        case "Hatch Pattern":
                            DrawHatchPatternCoastlineEffect(coastlineCanvas);
                            break;
                        case "User Defined":
                            DrawUserDefinedHatchEffect(coastlineCanvas);
                            break;
                    }

                    canvas.DrawSurface(CoastlineRenderSurface, new SKPoint(0, 0));
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
            if (LandformRenderSurface == null)
            {
                return;
            }

            if (!IsModified)
            {
                canvas.DrawSurface(LandformRenderSurface, new SKPoint(0, 0));
            }
            else
            {
                SKCanvas landformCanvas = LandformRenderSurface.Canvas;

                landformCanvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

                landformCanvas.Clear(SKColors.Transparent);

                landformCanvas.DrawPath(DrawPath, LandformFillPaint);

                double colorAlphaStep = 1.0 / (256.0 / 8.0);

                Color landformColor = Color.FromArgb((int)(LandformFillColor.A * (4 * colorAlphaStep)), LandformFillColor);

                LandformGradientPaint.BlendMode = SKBlendMode.SrcATop;
                LandformGradientPaint.Color = landformColor.ToSKColor();
                LandformGradientPaint.StrokeWidth = CoastlineEffectDistance / 8;

                landformCanvas.DrawPath(InnerPath8, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (8 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath7, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (12 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath6, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (16 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath5, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (20 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath4, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (24 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath3, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (28 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath2, LandformGradientPaint);

                landformColor = Color.FromArgb((int)(LandformFillColor.A * (32 * colorAlphaStep)), LandformFillColor);
                LandformGradientPaint.Color = landformColor.ToSKColor();
                landformCanvas.DrawPath(InnerPath1, LandformGradientPaint);

                LandformOutlinePaint.Color = LandformOutlineColor.ToSKColor();
                landformCanvas.DrawPath(ContourPath, LandformOutlinePaint);

                canvas.DrawSurface(LandformRenderSurface, new SKPoint(0, 0));

                IsModified = false;
            }
        }

        public void RenderLandformForHeightMap(SKCanvas canvas)
        {
            canvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

            canvas.Clear(SKColors.Transparent);

            canvas.DrawPath(DrawPath, LandformHeightMapFillPaint);

            canvas.DrawPath(ContourPath, LandformHeightMapOutlinePaint);

            IsModified = true;
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.

            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();
            XDocument mapLandformDoc = XDocument.Parse(content);

            IEnumerable<XElement?> nameElemEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformName"));
            if (nameElemEnum != null && nameElemEnum.Any() && nameElemEnum.First() != null)
            {
                string? name = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformName").Value).FirstOrDefault();
                LandformName = name;
            }
            else
            {
                LandformName = string.Empty;
            }

            IEnumerable<XElement?> guidElemEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformGuid"));
            if (guidElemEnum != null && guidElemEnum.Any() && guidElemEnum.First() != null)
            {
                string? mapGuid = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformGuid").Value).FirstOrDefault();
                LandformGuid = Guid.Parse(mapGuid);
            }
            else
            {
                LandformGuid = Guid.NewGuid();
            }

            IEnumerable<XElement?> fillWithTextureElem = mapLandformDoc.Descendants().Select(x => x.Element(ns + "FillWithTexture"));
            if (fillWithTextureElem != null && fillWithTextureElem.Any() && fillWithTextureElem.First() != null)
            {
                string? fillWithTexture = mapLandformDoc.Descendants().Select(x => x.Element(ns + "FillWithTexture").Value).FirstOrDefault();

                if (bool.TryParse(fillWithTexture, out bool boolFill))
                {
                    FillWithTexture = boolFill;
                }
                else
                {
                    FillWithTexture = true;
                }
            }
            else
            {
                FillWithTexture = true;
            }

            string txName = string.Empty;
            string txPath = string.Empty;

            IEnumerable<XElement?> landformTextureElem = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformTexture"));
            if (landformTextureElem != null && landformTextureElem.Any() && landformTextureElem.First() != null)
            {
                foreach (var item in landformTextureElem.Descendants())
                {
                    if (item.Name.LocalName == "TextureName")
                    {
                        txName = item.Value;
                    }
                    else if (item.Name.LocalName == "TexturePath")
                    {
                        txPath = item.Value;
                    }
                }
            }

            if (!string.IsNullOrEmpty(txName) && !string.IsNullOrEmpty(txPath))
            {
                LandformTexture = new(txName, txPath);

                Bitmap b = new(LandformTexture.TexturePath);
                Bitmap resizedBitmap = new(b, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                LandformTexture.TextureBitmap = new(resizedBitmap);

                // create and set a shader from the texture
                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                LandformFillPaint.Shader = flpShader;
            }
            else
            {
                // texture is not set in the landform object, so use default
                if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
                {
                    AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
                }

                LandformTexture = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX];

                if (LandformTexture.TextureBitmap != null)
                {
                    Bitmap resizedBitmap = new(LandformTexture.TextureBitmap, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                    // create and set a shader from the texture
                    SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                        SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                    LandformFillPaint.Shader = flpShader;
                }
            }

            if (!FillWithTexture || LandformTexture == null || LandformTexture.TextureBitmap == null)
            {
                LandformFillPaint.Shader?.Dispose();
                LandformFillPaint.Shader = null;

                SKShader flpShader = SKShader.CreateColor(LandformBackgroundColor.ToSKColor());
                LandformFillPaint.Shader = flpShader;
            }

            IEnumerable<XElement> colorElem = mapLandformDoc.Descendants(ns + "LandformOutlineColor");
            if (colorElem != null && colorElem.Any() && colorElem.First() != null)
            {
                string? color = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformOutlineColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    LandformOutlineColor = ColorTranslator.FromHtml(color);
                }
                else
                {
                    if (int.TryParse(color, out int colorArgb))
                    {
                        LandformOutlineColor = Color.FromArgb(colorArgb);
                    }
                    else
                    {
                        LandformOutlineColor = Color.FromArgb(255, 62, 55, 40);
                    }
                }
            }
            else
            {
                LandformOutlineColor = Color.FromArgb(255, 62, 55, 40);
            }

            LandformFillColor = Color.FromArgb(LandformOutlineColor.A / 4, LandformOutlineColor);

            IEnumerable<XElement?> widthElem = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformOutlineWidth"));
            if (widthElem != null && widthElem.Any() && widthElem.First() != null)
            {
                string? width = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformOutlineWidth").Value).FirstOrDefault();

                if (int.TryParse(width, out int n))
                {
                    LandformOutlineWidth = n;
                }
                else
                {
                    LandformOutlineWidth = 2;
                }
            }
            else
            {
                LandformOutlineWidth = 2;
            }

            IEnumerable<XElement> backgroundColorElem = mapLandformDoc.Descendants(ns + "LandformBackgroundColor");
            if (backgroundColorElem != null && backgroundColorElem.Any() && backgroundColorElem.First() != null)
            {
                string? color = mapLandformDoc.Descendants().Select(x => x.Element(ns + "LandformBackgroundColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    LandformBackgroundColor = ColorTranslator.FromHtml(color);
                }
                else if (int.TryParse(color, out int colorArgb))
                {
                    LandformBackgroundColor = Color.FromArgb(colorArgb);
                }
                else
                {
                    LandformBackgroundColor = Color.White;
                }
            }
            else
            {
                LandformBackgroundColor = Color.White;
            }

            LandformFillColor = Color.FromArgb(LandformOutlineColor.A / 4, LandformOutlineColor);

            // shoreline style is not used now
            IEnumerable<XElement?> styleEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "ShorelineStyle"));
            if (styleEnum != null && styleEnum.Any() && styleEnum.First() != null)
            {
                string? styleName = mapLandformDoc.Descendants().Select(x => x.Element(ns + "ShorelineStyle").Value).FirstOrDefault();

                GradientDirectionEnum shorelineStyleEnum = GradientDirectionEnum.LightToDark;
                if (Enum.TryParse<GradientDirectionEnum>(styleName, out shorelineStyleEnum))
                {
                    ShorelineStyle = shorelineStyleEnum;
                }
                else
                {
                    ShorelineStyle = GradientDirectionEnum.LightToDark;
                }
            }
            else
            {
                ShorelineStyle = GradientDirectionEnum.LightToDark;
            }

            colorElem = mapLandformDoc.Descendants(ns + "CoastlineColor");
            if (colorElem != null && colorElem.Any() && colorElem.First() != null)
            {
                string? color = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    CoastlineColor = ColorTranslator.FromHtml(color);
                }
                else if (int.TryParse(color, out int colorArgb))
                {
                    CoastlineColor = Color.FromArgb(colorArgb);
                }
                else
                {
                    CoastlineColor = ColorTranslator.FromHtml("#BB9CC3B7");
                }
            }
            else
            {
                CoastlineColor = ColorTranslator.FromHtml("#BB9CC3B7");
            }

            IEnumerable<XElement?> distanceElem = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineEffectDistance"));
            if (distanceElem != null && distanceElem.Any() && distanceElem.First() != null)
            {
                string? distance = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineEffectDistance").Value).FirstOrDefault();

                if (int.TryParse(distance, out int n))
                {
                    if (n > 0)
                    {
                        CoastlineEffectDistance = n;
                    }
                    else
                    {
                        CoastlineEffectDistance = 16;
                    }
                }
                else
                {
                    CoastlineEffectDistance = 16;
                }
            }
            else
            {
                CoastlineEffectDistance = 16;
            }

            IEnumerable<XElement?> styleNameEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineStyleName"));
            if (styleNameEnum != null && styleNameEnum.Any() && styleNameEnum.First() != null)
            {
                string? styleName = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineStyleName").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(styleName))
                {
                    CoastlineStyleName = styleName;
                }
                else
                {
                    CoastlineStyleName = "Dash Pattern";
                }
            }
            else
            {
                CoastlineStyleName = "Dash Pattern";
            }

            IEnumerable<XElement?> patternNameEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchPattern"));
            if (patternNameEnum != null && patternNameEnum.First() != null)
            {
                string? patternName = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchPattern").Value).FirstOrDefault();
                CoastlineHatchPattern = patternName;
            }

            IEnumerable<XElement?> opacityEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchOpacity"));
            if (opacityEnum != null && opacityEnum.First() != null)
            {
                string? opacity = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchOpacity").Value).FirstOrDefault();
                CoastlineHatchOpacity = int.Parse(opacity);
            }
            else
            {
                CoastlineHatchOpacity = 255;
            }

            IEnumerable<XElement?> hatchScaleEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchScale"));
            if (hatchScaleEnum != null && hatchScaleEnum.First() != null)
            {
                string? hatchScale = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchScale").Value).FirstOrDefault();
                CoastlineHatchScale = int.Parse(hatchScale);
            }

            IEnumerable<XElement?> blendModeEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchBlendMode"));
            if (blendModeEnum != null && blendModeEnum.First() != null)
            {
                string? blendMode = mapLandformDoc.Descendants().Select(x => x.Element(ns + "CoastlineHatchBlendMode").Value).FirstOrDefault();
                CoastlineHatchBlendMode = blendMode;
            }

            IEnumerable<XElement?> boolElem = mapLandformDoc.Descendants().Select(x => x.Element(ns + "PaintCoastlineGradient"));
            if (boolElem != null && boolElem.First() != null)
            {
                string? paintCoastlineGradient = mapLandformDoc.Descendants().Select(x => x.Element(ns + "PaintCoastlineGradient").Value).FirstOrDefault();
                PaintCoastlineGradient = bool.Parse(paintCoastlineGradient);
            }
            else
            {
                PaintCoastlineGradient = true;
            }

            IEnumerable<XElement?> pathElemEnum = mapLandformDoc.Descendants().Select(x => x.Element(ns + "ContourPath"));
            if (pathElemEnum != null && pathElemEnum.Any() && pathElemEnum.First() != null)
            {
                string? contourPath = mapLandformDoc.Descendants().Select(x => x.Element(ns + "ContourPath").Value).FirstOrDefault();
                ContourPath = SKPath.ParseSvgPathData(contourPath);
                DrawPath = new(ContourPath);
            }
            else
            {
                throw new Exception("Landform path is not set. Cannot load this map.");
            }

            IsModified = true;

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);

            // landform name
            writer.WriteStartElement("LandformName");
            writer.WriteString(LandformName);
            writer.WriteEndElement();

            // landform GUID
            writer.WriteStartElement("LandformGuid");
            writer.WriteString(LandformGuid.ToString());
            writer.WriteEndElement();

            if (LandformTexture != null)
            {
                // landform texture
                writer.WriteStartElement("LandformTexture");
                writer.WriteStartElement("TextureName");
                writer.WriteString(LandformTexture.TextureName);
                writer.WriteEndElement();
                writer.WriteStartElement("TexturePath");
                writer.WriteString(LandformTexture.TexturePath);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            // fill with texture
            writer.WriteStartElement("FillWithTexture");
            writer.WriteValue(FillWithTexture);
            writer.WriteEndElement();

            // landform outline color
            int outlinecolor = LandformOutlineColor.ToArgb();
            writer.WriteStartElement("LandformOutlineColor");
            writer.WriteValue(outlinecolor);
            writer.WriteEndElement();

            // landform background color
            int backgroundcolor = LandformBackgroundColor.ToArgb();
            writer.WriteStartElement("LandformBackgroundColor");
            writer.WriteValue(backgroundcolor);
            writer.WriteEndElement();

            // landform outline width
            writer.WriteStartElement("LandformOutlineWidth");
            writer.WriteValue(LandformOutlineWidth);
            writer.WriteEndElement();

            // shoreline style
            writer.WriteStartElement("ShorelineStyle");
            writer.WriteString(ShorelineStyle.ToString());
            writer.WriteEndElement();

            // coastline color
            int coastcolor = CoastlineColor.ToArgb();
            writer.WriteStartElement("CoastlineColor");
            writer.WriteValue(coastcolor);
            writer.WriteEndElement();

            // coastline effect distance
            writer.WriteStartElement("CoastlineEffectDistance");
            writer.WriteValue(CoastlineEffectDistance);
            writer.WriteEndElement();

            if (CoastlineStyleName != null)
            {
                // coastline style
                writer.WriteStartElement("CoastlineStyleName");
                writer.WriteString(CoastlineStyleName);
                writer.WriteEndElement();
            }

            if (CoastlineHatchPattern != null)
            {
                // coastline hatch pattern
                writer.WriteStartElement("CoastlineHatchPattern");
                writer.WriteString(CoastlineHatchPattern);
                writer.WriteEndElement();
            }

            // coastline hatch opacity
            writer.WriteStartElement("CoastlineHatchOpacity");
            writer.WriteValue(CoastlineHatchOpacity);
            writer.WriteEndElement();

            // coastline hatch scale
            writer.WriteStartElement("CoastlineHatchScale");
            writer.WriteValue(CoastlineHatchScale);
            writer.WriteEndElement();

            // coastline hatch blend mode
            writer.WriteStartElement("CoastlineHatchBlendMode");
            writer.WriteString(CoastlineHatchBlendMode);
            writer.WriteEndElement();

            // paint coastline gradient
            writer.WriteStartElement("PaintCoastlineGradient");
            writer.WriteValue(PaintCoastlineGradient);
            writer.WriteEndElement();

            // landform contourPath path
            writer.WriteStartElement("ContourPath");
            string pathSvg = ContourPath.ToSvgPathData();
            writer.WriteValue(pathSvg);
            writer.WriteEndElement();
        }
        #endregion
    }
}
