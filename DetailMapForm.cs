using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Globalization;

namespace RealmStudio
{
    public partial class DetailMapForm : Form
    {
        private readonly RealmStudioMainForm mainForm;
        private readonly RealmStudioMap currentMap;
        private SKRect selectedArea;

        public RealmStudioMap? detailMap;


        public DetailMapForm(RealmStudioMainForm mainForm, RealmStudioMap currentMap, SKRect selectedArea)
        {
            InitializeComponent();

            this.mainForm = mainForm;
            this.currentMap = currentMap;
            this.selectedArea = selectedArea;

            MapTopUpDown.Value = (decimal)selectedArea.Top;
            MapLeftUpDown.Value = (decimal)selectedArea.Left;
            MapWidthUpDown.Value = (decimal)selectedArea.Width;
            MapHeightUpDown.Value = (decimal)selectedArea.Height;

            DetailMapWidthLabel.Text = (MapWidthUpDown.Value * MapZoomUpDown.Value).ToString(CultureInfo.InvariantCulture);
            DetailMapHeightLabel.Text = (MapHeightUpDown.Value * MapZoomUpDown.Value).ToString(CultureInfo.InvariantCulture);
        }

        private void CancelCreateDetailButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MapZoomTrack_Scroll(object sender, EventArgs e)
        {
            MapZoomUpDown.Value = (decimal)(MapZoomTrack.Value / 10.0);
        }

        private void MapZoomUpDown_ValueChanged(object sender, EventArgs e)
        {
            MapZoomTrack.Value = (int)(MapZoomUpDown.Value * 10);

            int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
            int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

            DetailMapWidthLabel.Text = detailMapWidth.ToString(CultureInfo.InvariantCulture);
            DetailMapHeightLabel.Text = detailMapHeight.ToString(CultureInfo.InvariantCulture);
        }

        private void MapTopUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void MapLeftUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void MapWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void MapHeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            ChangeDetailMapLocationAndSize();
        }

        private void ChangeDetailMapLocationAndSize()
        {
            selectedArea.Top = (float)MapTopUpDown.Value;
            selectedArea.Left = (float)MapLeftUpDown.Value;
            selectedArea.Right = (float)MapLeftUpDown.Value + (float)MapWidthUpDown.Value;
            selectedArea.Bottom = (float)MapTopUpDown.Value + (float)MapHeightUpDown.Value;

            int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
            int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

            DetailMapWidthLabel.Text = detailMapWidth.ToString();
            DetailMapHeightLabel.Text = detailMapHeight.ToString();

            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.WORKLAYER);
            workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            workLayer.LayerSurface?.Canvas.DrawRect(selectedArea, PaintObjects.LandformAreaSelectPaint);
            mainForm.SKGLRenderControl.Invalidate();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Create the detail map?", "Confirm Detail Map", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                CreateDetailMap(IncludeTerrainCheck.Checked, IncludeVegetationCheck.Checked,
                    IncludeStructuresCheck.Checked, IncludeLabelsCheck.Checked, IncludePathsCheck.Checked);
            }
        }

        private void CreateDetailMap(bool includeTerrainSymbols, bool includeVegetationSymbols,
            bool includeStructureSymbols, bool includeLabels, bool includePaths)
        {
            int detailMapWidth = Math.Min((int)(MapWidthUpDown.Value * MapZoomUpDown.Value), 10000);
            int detailMapHeight = Math.Min((int)(MapHeightUpDown.Value * MapZoomUpDown.Value), 10000);

            // create the new map at the given size
            detailMap = new()
            {
                MapWidth = detailMapWidth,
                MapHeight = detailMapHeight,
                MapName = MapNameTextBox.Text,
            };

            detailMap = MapBuilder.CreateMap(detailMap, mainForm.SKGLRenderControl.GRContext);
            detailMap.IsSaved = false;

            // the location and size of each symbol, landforms, painted colors, paths, and labels
            // has to be determined based on the location, size, and scale of the current map
            // versus the new detail map
            float scaleX = detailMap.MapWidth / selectedArea.Width;
            float scaleY = detailMap.MapHeight / selectedArea.Height;

            float deltaX = -selectedArea.Left * scaleX;
            float deltaY = -selectedArea.Top * scaleY;

            // get the landforms within or intersecting the selected area, then translate and scale them
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.LANDFORMLAYER);
            MapLayer detailLandformLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.LANDFORMLAYER);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform lf)
                {
                    SKPath lfContour = lf.ContourPath;

                    if (lfContour != null)
                    {
                        foreach (SKPoint p in lfContour.Points)
                        {
                            if (selectedArea.Contains(p))
                            {
                                // landform path is in or intersects the selected area
                                SKPath transformedPath = new(lfContour);
                                transformedPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                                Landform detailLf = new()
                                {
                                    ParentMap = detailMap,
                                    DrawPath = transformedPath,
                                    CoastlineColor = lf.CoastlineColor,
                                    CoastlineEffectDistance = lf.CoastlineEffectDistance,
                                    CoastlineFillPaint = lf.CoastlineFillPaint,
                                    CoastlineHatchBlendMode = lf.CoastlineHatchBlendMode,
                                    CoastlineHatchOpacity = lf.CoastlineHatchOpacity,
                                    CoastlineHatchScale = lf.CoastlineHatchScale,
                                    CoastlinePaint = lf.CoastlinePaint,
                                    CoastlineStyleName = lf.CoastlineStyleName,
                                    DashShader = lf.DashShader,
                                    LandformFillColor = lf.LandformFillColor,
                                    LandformFillPaint = lf.LandformFillPaint,
                                    LandformGradientPaint = lf.LandformGradientPaint,
                                    LandformName = lf.LandformName,
                                    LandformOutlineColor = lf.LandformOutlineColor,
                                    LandformOutlineWidth = lf.LandformOutlineWidth,
                                    LandformTexture = lf.LandformTexture,
                                    LineHatchBitmapShader = lf.LineHatchBitmapShader,
                                    PaintCoastlineGradient = lf.PaintCoastlineGradient,
                                    ShorelineStyle = lf.ShorelineStyle,
                                };

                                SKImageInfo lfImageInfo = new(detailMap.MapWidth, detailMap.MapHeight);

                                detailLf.LandformRenderSurface ??= SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false, lfImageInfo);
                                detailLf.CoastlineRenderSurface ??= SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false, lfImageInfo);

                                LandformMethods.CreateAllPathsFromDrawnPath(detailMap, detailLf);
                                detailLandformLayer.MapLayerComponents.Add(detailLf);

                                break;
                            }
                        }
                    }
                }

                // TODO:layer paint strokes in land layer
            }

            // go through the current map to get textures, painted colors, etc. and assign them to the detail map

            // texture needs to be resized to new map size
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.BASELAYER);
            MapLayer detailBaseLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.BASELAYER);

            foreach (MapImage mi in baseLayer.MapLayerComponents.Cast<MapImage>())
            {
                Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), detailMap.MapWidth, detailMap.MapHeight);

                MapImage BackgroundTexture = new()
                {
                    Width = detailMap.MapWidth,
                    Height = detailMap.MapHeight,
                    MapImageBitmap = resizedBitmap.ToSKBitmap(),
                };

                detailBaseLayer.MapLayerComponents.Add(BackgroundTexture);
            }

            // texture needs to be resized to new map size
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANTEXTURELAYER);
            MapLayer detailOceanTextureLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.OCEANTEXTURELAYER);

            foreach (MapImage mi in oceanTextureLayer.MapLayerComponents.Cast<MapImage>())
            {
                Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), detailMap.MapWidth, detailMap.MapHeight);

                MapImage OceanTexture = new()
                {
                    Width = detailMap.MapWidth,
                    Height = detailMap.MapHeight,
                    MapImageBitmap = resizedBitmap.ToSKBitmap(),
                };

                detailOceanTextureLayer.MapLayerComponents.Add(OceanTexture);
            }

            // texture needs to be resized to new map size
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer detailOceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            foreach (MapImage mi in oceanTextureOverlayLayer.MapLayerComponents.Cast<MapImage>())
            {
                Bitmap resizedBitmap = new(mi.MapImageBitmap.ToBitmap(), detailMap.MapWidth, detailMap.MapHeight);

                MapImage OceanColor = new()
                {
                    Width = detailMap.MapWidth,
                    Height = detailMap.MapHeight,
                    MapImageBitmap = resizedBitmap.ToSKBitmap(),
                };

                detailOceanTextureOverlayLayer.MapLayerComponents.Add(OceanColor);
            }

            // ocean drawing layer
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.OCEANDRAWINGLAYER);
            MapLayer detailOceanDrawingLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.OCEANDRAWINGLAYER);

            foreach (LayerPaintStroke lps in oceanDrawingLayer.MapLayerComponents.Cast<LayerPaintStroke>())
            {
                LayerPaintStroke newPaintStroke = new()
                {
                    ParentMap = detailMap,
                    StrokeColor = lps.StrokeColor,
                    PaintBrush = lps.PaintBrush,
                    BrushRadius = (int)(lps.BrushRadius * scaleX),
                    MapLayerIdentifier = lps.MapLayerIdentifier,
                    Erase = lps.Erase,
                    Rendered = false,
                };

                foreach (LayerPaintStrokePoint point in lps.PaintStrokePoints)
                {
                    LayerPaintStrokePoint newStrokePoint = new()
                    {
                        StrokeLocation = new SKPoint((point.X * scaleX) + deltaX, (point.Y * scaleY) + deltaY),
                        StrokeRadius = (int)(point.StrokeRadius * scaleX)
                    };

                    newPaintStroke.PaintStrokePoints.Add(newStrokePoint);
                }

                SKImageInfo imageInfo = new(detailMap.MapWidth, detailMap.MapHeight);
                newPaintStroke.RenderSurface ??= SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false, imageInfo);
                detailOceanDrawingLayer.MapLayerComponents.Add(newPaintStroke);
            }


            // land drawing layer
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.LANDDRAWINGLAYER);
            MapLayer detailLandDrawingLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.LANDDRAWINGLAYER);

            for (int i = 0; i < landDrawingLayer.MapLayerComponents.Count; i++)
            {
                // construct a new LayerPaintStroke from the existing one
                // and create a new paint surface for it

                if (landDrawingLayer.MapLayerComponents[i] is LayerPaintStroke lps)
                {
                    LayerPaintStroke newPaintStroke = new()
                    {
                        ParentMap = detailMap,
                        StrokeColor = lps.StrokeColor,
                        PaintBrush = lps.PaintBrush,
                        BrushRadius = (int)(lps.BrushRadius * scaleX),
                        MapLayerIdentifier = lps.MapLayerIdentifier,
                        Erase = lps.Erase,
                        Rendered = false,
                    };

                    foreach (LayerPaintStrokePoint point in lps.PaintStrokePoints)
                    {
                        LayerPaintStrokePoint newStrokePoint = new()
                        {
                            StrokeLocation = new SKPoint((point.X * scaleX) + deltaX, (point.Y * scaleY) + deltaY),
                            StrokeRadius = (int)(point.StrokeRadius * scaleX)
                        };

                        newPaintStroke.PaintStrokePoints.Add(newStrokePoint);
                    }

                    SKImageInfo imageInfo = new(detailMap.MapWidth, detailMap.MapHeight);
                    newPaintStroke.RenderSurface ??= SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false, imageInfo);
                    detailLandDrawingLayer.MapLayerComponents.Add(newPaintStroke);
                }
            }

            // water layer objects - these need to be treated basically the same as landforms
            // get the water features within or intersecting the selected area, then translate and scale them
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.WATERLAYER);
            MapLayer detailWaterLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.WATERLAYER);

            for (int i = 0; i < waterLayer.MapLayerComponents.Count; i++)
            {
                if (waterLayer.MapLayerComponents[i] is WaterFeature wf)
                {
                    SKPath wfPath = wf.WaterFeaturePath;
                    SKPath wfContour = wf.ContourPath;

                    if (wfContour != null)
                    {
                        foreach (SKPoint p in wfContour.Points)
                        {
                            if (selectedArea.Contains(p))
                            {
                                // water feature path is in or intersects the selected area
                                SKPath transformedContourPath = new(wfContour);
                                transformedContourPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                                SKPath transformedWfPath = new(wfPath);
                                transformedWfPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                                WaterFeature detailWf = new()
                                {
                                    WaterFeaturePath = transformedWfPath,
                                    ShallowWaterPaint = wf.ShallowWaterPaint,
                                    ShorelineEffectDistance = wf.ShorelineEffectDistance,
                                    WaterFeatureName = wf.WaterFeatureName,
                                    ParentMap = detailMap,
                                    WaterFeatureColor = wf.WaterFeatureColor,
                                    WaterFeatureShorelineColor = wf.WaterFeatureShorelineColor,
                                    WaterFeatureType = wf.WaterFeatureType,
                                };

                                SKImageInfo lfImageInfo = new(detailMap.MapWidth, detailMap.MapHeight);

                                WaterFeatureMethods.CreateInnerAndOuterPaths(detailMap, detailWf);
                                WaterFeatureMethods.ConstructWaterFeaturePaintObjects(detailWf);

                                detailWaterLayer.MapLayerComponents.Add(detailWf);

                                break;
                            }
                        }
                    }
                }
                else if (waterLayer.MapLayerComponents[i] is River r)
                {
                    foreach (MapRiverPoint mrp in r.RiverPoints)
                    {
                        if (selectedArea.Contains(mrp.RiverPoint) && r.RiverPath?.PointCount > 2)
                        {
                            River detailRiver = new()
                            {
                                MapRiverName = r.MapRiverName,
                                ParentMap = detailMap,
                                RiverColor = r.RiverColor,
                                RiverShorelineColor = r.RiverShorelineColor,
                                RiverSourceFadeIn = r.RiverSourceFadeIn,
                                RiverWidth = r.RiverWidth * scaleX,      // scale up?
                                ShorelineEffectDistance = r.ShorelineEffectDistance,
                                RiverBoundaryPath = r.RiverBoundaryPath,
                                RiverPath = r.RiverPath,
                            };

                            SKPath transformedRiverPath = new(detailRiver.RiverPath);
                            transformedRiverPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                            detailRiver.RiverPath = transformedRiverPath;

                            foreach (SKPoint p in transformedRiverPath.Points)
                            {
                                MapRiverPoint newMrp = new()
                                {
                                    RiverPoint = p
                                };

                                detailRiver.RiverPoints.Add(newMrp);
                            }

                            WaterFeatureMethods.ConstructRiverPaintObjects(detailRiver);
                            WaterFeatureMethods.ConstructRiverPaths(detailRiver);

                            detailWaterLayer.MapLayerComponents.Add(detailRiver);

                            break;
                        }
                    }
                }
            }

            // water drawing layer
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.WATERDRAWINGLAYER);
            MapLayer detailWaterDrawingLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.WATERDRAWINGLAYER);

            for (int i = 0; i < waterDrawingLayer.MapLayerComponents.Count; i++)
            {
                // construct a new LayerPaintStroke from the existing one
                // and create a new paint surface for it

                if (waterDrawingLayer.MapLayerComponents[i] is LayerPaintStroke lps)
                {
                    LayerPaintStroke newPaintStroke = new()
                    {
                        ParentMap = detailMap,
                        StrokeColor = lps.StrokeColor,
                        PaintBrush = lps.PaintBrush,
                        BrushRadius = (int)(lps.BrushRadius * scaleX),
                        MapLayerIdentifier = lps.MapLayerIdentifier,
                        Erase = lps.Erase,
                        Rendered = false,
                    };

                    foreach (LayerPaintStrokePoint point in lps.PaintStrokePoints)
                    {
                        LayerPaintStrokePoint newStrokePoint = new()
                        {
                            StrokeLocation = new SKPoint((point.X * scaleX) + deltaX, (point.Y * scaleY) + deltaY),
                            StrokeRadius = (int)(point.StrokeRadius * scaleX)
                        };

                        newPaintStroke.PaintStrokePoints.Add(newStrokePoint);
                    }

                    SKImageInfo imageInfo = new(detailMap.MapWidth, detailMap.MapHeight);
                    newPaintStroke.RenderSurface ??= SKSurface.Create(mainForm.SKGLRenderControl.GRContext, false, imageInfo);
                    detailWaterDrawingLayer.MapLayerComponents.Add(newPaintStroke);
                }
            }

            // gather the symbols in the selected area
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.SYMBOLLAYER);
            MapLayer detailSymbolLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.SYMBOLLAYER);
            List<MapSymbol> gatheredSymbols = [];

            for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol ms)
                {
                    if (selectedArea.Contains(ms.X, ms.Y))
                    {
                        if (includeTerrainSymbols && ms.SymbolType == SymbolTypeEnum.Terrain)
                        {
                            gatheredSymbols.Add(ms);
                        }

                        if (includeVegetationSymbols && ms.SymbolType == SymbolTypeEnum.Vegetation)
                        {
                            gatheredSymbols.Add(ms);
                        }

                        if (includeStructureSymbols && ms.SymbolType == SymbolTypeEnum.Structure)
                        {
                            gatheredSymbols.Add(ms);
                        }
                    }
                }
            }

            // scale the symbols and add them to the detail map
            foreach (MapSymbol ms in gatheredSymbols)
            {
                MapSymbol newSymbol = new(ms)
                {
                    X = (int)((ms.X * scaleX) + deltaX),
                    Y = (int)((ms.Y * scaleY) + deltaY),
                    SymbolWidth = (int)(ms.SymbolWidth * scaleX),
                    SymbolHeight = (int)(ms.SymbolHeight * scaleY),
                    SymbolPaint = ms.SymbolPaint,
                };

                Bitmap resizedPlacedBitmap = new(ms.PlacedBitmap.ToBitmap(), newSymbol.SymbolWidth, newSymbol.SymbolHeight);
                newSymbol.PlacedBitmap = resizedPlacedBitmap.ToSKBitmap();

                detailSymbolLayer.MapLayerComponents.Add(newSymbol);
            }


            // get map paths

            if (includePaths)
            {
                List<MapPath> gatheredPaths = [];

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.PATHLOWERLAYER);
                MapLayer detailPathLowerLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.PATHLOWERLAYER);

                for (int i = 0; i < pathLowerLayer.MapLayerComponents.Count; i++)
                {
                    if (pathLowerLayer.MapLayerComponents[i] is MapPath mp)
                    {
                        foreach (MapPathPoint point in mp.PathPoints)
                        {
                            if (selectedArea.Contains(point.MapPoint.X, point.MapPoint.Y))
                            {
                                gatheredPaths.Add(mp);
                                break;
                            }
                        }
                    }
                }

                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.PATHUPPERLAYER);
                MapLayer detailPathUpperLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.PATHUPPERLAYER);

                for (int i = 0; i < pathUpperLayer.MapLayerComponents.Count; i++)
                {
                    if (pathUpperLayer.MapLayerComponents[i] is MapPath mp)
                    {
                        foreach (MapPathPoint point in mp.PathPoints)
                        {
                            if (selectedArea.Contains(point.MapPoint.X, point.MapPoint.Y))
                            {
                                gatheredPaths.Add(mp);
                                break;
                            }
                        }
                    }
                }

                foreach (MapPath mp in gatheredPaths)
                {
                    if (mp.PathPoints.Count > 0)
                    {
                        MapPath newPath = new()
                        {
                            DrawOverSymbols = mp.DrawOverSymbols,
                            IsSelected = false,
                            MapPathName = mp.MapPathName,
                            ParentMap = detailMap,
                            PathColor = mp.PathColor,
                            PathWidth = mp.PathWidth * scaleX,
                            X = (int)((mp.X * scaleX) + deltaX),
                            Y = (int)((mp.Y * scaleY) + deltaY),
                            PathType = mp.PathType,
                        };

                        foreach (MapPathPoint point in mp.PathPoints)
                        {
                            MapPathPoint newPathPoint = new()
                            {
                                MapPoint = new SKPoint((point.MapPoint.X * scaleX) + deltaX, (point.MapPoint.Y * scaleY) + deltaY),
                            };

                            newPath.PathPoints.Add(newPathPoint);
                        }

                        if (mp.PathTexture != null)
                        {
                            newPath.PathTexture = new MapTexture
                            {
                                TextureName = mp.PathTexture.TextureName,
                                TexturePath = mp.PathTexture.TexturePath,
                                TextureBitmap = (Bitmap?)Bitmap.FromFile(mp.PathTexture.TexturePath)
                            };
                        }

                        newPath.PathPaint = null;   // force to null so that PathPaint is constructed
                        MapPathMethods.ConstructPathPaint(newPath);

                        if (mp.DrawOverSymbols)
                        {
                            detailPathUpperLayer.MapLayerComponents.Add(newPath);
                        }
                        else
                        {
                            detailPathLowerLayer.MapLayerComponents.Add(newPath);
                        }
                    }
                }
            }

            if (includeLabels)
            {
                // get labels
                MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.LABELLAYER);
                MapLayer detailLabelLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.LABELLAYER);
                List<MapLabel> gatheredLabels = [];

                for (int i = 0; i < labelLayer.MapLayerComponents.Count; i++)
                {
                    if (labelLayer.MapLayerComponents[i] is MapLabel ml)
                    {
                        SKRect mlBoundingRect = new(ml.X, ml.Y, ml.X + ml.Width, ml.Y + ml.Height);
                        if (selectedArea.IntersectsWith(mlBoundingRect))
                        {
                            gatheredLabels.Add(ml);
                        }
                    }
                }

                foreach (MapLabel ml in gatheredLabels)
                {
                    MapLabel newLabel = new()
                    {
                        X = (int)((ml.X * scaleX) + deltaX),
                        Y = (int)((ml.Y * scaleY) + deltaY),
                        Height = (int)(ml.Height * scaleX),
                        Width = (int)(ml.Width * scaleY),
                        LabelColor = ml.LabelColor,
                        LabelFont = ml.LabelFont,
                        LabelGlowColor = ml.LabelGlowColor,
                        LabelGlowStrength = ml.LabelGlowStrength,
                        LabelOutlineColor = ml.LabelOutlineColor,
                        LabelOutlineWidth = ml.LabelOutlineWidth * scaleX,  // scale
                        LabelPaint = ml.LabelPaint?.Clone(),
                        LabelPath = ml.LabelPath,
                        LabelRotationDegrees = ml.LabelRotationDegrees,
                        LabelSKFont = ml.LabelSKFont,
                        LabelText = ml.LabelText,
                        RenderComponent = ml.RenderComponent,
                    };

                    Font labelFont = new(newLabel.LabelFont.FontFamily, newLabel.LabelFont.Size * scaleY,
                        newLabel.LabelFont.Style, GraphicsUnit.Pixel);

                    newLabel.LabelFont = labelFont;

                    SKFont skLabelFont = MapLabelMethods.GetSkLabelFont(labelFont);
                    SKPaint paint = MapLabelMethods.CreateLabelPaint(newLabel.LabelColor);

                    newLabel.LabelPaint = paint;
                    newLabel.LabelSKFont.Dispose();
                    newLabel.LabelSKFont = skLabelFont;

                    if (ml.LabelPath != null)
                    {
                        SKPath transformedLabelPath = new(ml.LabelPath);
                        transformedLabelPath.Transform(SKMatrix.CreateScaleTranslation(scaleX, scaleY, deltaX, deltaY));

                        newLabel.LabelPath = transformedLabelPath;
                    }

                    detailLabelLayer.MapLayerComponents.Add(newLabel);
                }
            }


            // vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(currentMap, MapBuilder.VIGNETTELAYER);
            MapLayer detailVignetteLayer = MapBuilder.GetMapLayerByIndex(detailMap, MapBuilder.VIGNETTELAYER);

            foreach (MapVignette mv in vignetteLayer.MapLayerComponents.Cast<MapVignette>())
            {
                mv.ParentMap = detailMap;
                detailVignetteLayer.MapLayerComponents.Add(mv);
            }
        }
    }
}
