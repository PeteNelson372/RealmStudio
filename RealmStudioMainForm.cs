using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static DrawingModeEnum CURRENT_DRAWING_MODE = DrawingModeEnum.None;

        private static RealmStudioMap? CURRENT_MAP = null;

        private static Landform? CURRENT_LANDFORM = null;

        private static int SELECTED_BRUSH_SIZE = 0;

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);

        private static float DrawingZoom = 1.0f;

        private readonly ToolTip TOOLTIP = new();

        private bool AUTOSAVE = true;

        private static readonly SKPaint LandformSelectPaint = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = SKColors.Firebrick,
            StrokeWidth = 2,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F),
        };

        private static readonly SKPaint CursorCirclePaint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            PathEffect = SKPathEffect.CreateDash([5F, 5F], 10F),
        };

        public RealmStudioMainForm()
        {
            InitializeComponent();
            SKGLRenderControl.Hide();
            SKGLRenderControl.MouseWheel += SKGLRenderControl_MouseWheel;
        }

        #region Main Form Event Handlers

        private void RealmStudioMainForm_Load(object sender, EventArgs e)
        {
            int assetCount = AssetManager.LoadAllAssets();

            PopulateControlsWithAssets(assetCount);
        }

        private void RealmStudioMainForm_Shown(object sender, EventArgs e)
        {
            RealmConfiguration rcd = new();
            DialogResult result = rcd.ShowDialog();

            if (result == DialogResult.OK)
            {
                // create the map from the settings on the dialog
                CURRENT_MAP = MapBuilder.CreateMap(rcd.map);

                MAP_WIDTH = CURRENT_MAP.MapWidth;
                MAP_HEIGHT = CURRENT_MAP.MapHeight;

                MapRenderHScroll.Maximum = MAP_WIDTH;
                MapRenderVScroll.Maximum = MAP_HEIGHT;

                MapRenderHScroll.Value = 0;
                MapRenderVScroll.Value = 0;

                if (string.IsNullOrEmpty(CURRENT_MAP.MapName))
                {
                    CURRENT_MAP.MapName = "Default";
                }

                UpdateMapNameAndSize();

                if (CURRENT_MAP.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                {
                    MessageBox.Show("Application startup error. RealmStudio will close.");
                    Application.Exit();
                }
            }

            LogoPictureBox.Hide();
            SKGLRenderControl.Show();
            SKGLRenderControl.Select();
            SKGLRenderControl.Refresh();
            SKGLRenderControl.Invalidate();

            Activate();
        }

        private void RealmStudioMainForm_Activated(object sender, EventArgs e)
        {
        }

        private void AutosaveSwitch_Click(object sender, EventArgs e)
        {
            AUTOSAVE = AutosaveSwitch.Checked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // save the map
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            DrawingZoom = 1.0F;

            ScrollPoint.X = 0;
            ScrollPoint.Y = 0;
            DrawingPoint.X = 0;
            DrawingPoint.Y = 0;

            MapRenderHScroll.Value = 0;
            MapRenderVScroll.Value = 0;

            SKGLRenderControl.Invalidate();
        }

        private void ZoomToFitButton_Click(object sender, EventArgs e)
        {
            if (CURRENT_MAP == null) return;

            float horizontalAspect = (float) SKGLRenderControl.Width / CURRENT_MAP.MapWidth;
            float verticalAspect = (float) SKGLRenderControl.Height / CURRENT_MAP.MapHeight;

            DrawingZoom = Math.Min(horizontalAspect, verticalAspect);

            ScrollPoint.X = 0;
            ScrollPoint.Y = 0;
            DrawingPoint.X = 0;
            DrawingPoint.Y = 0;

            MapRenderHScroll.Value = 0;
            MapRenderVScroll.Value = 0;

            SKGLRenderControl.Invalidate();
        }

        #region Main Menu Event Handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #endregion

        #region SKGLRenderControl Event Handlers

        private void SKGLRenderControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            if (CURRENT_MAP != null)
            {
                foreach (MapLayer layer in CURRENT_MAP.MapLayers)
                {
                    // if the surfaces haven't been created, create them
                    layer.LayerSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
                }


                // handle zoom-in and zoom-out (TODO: zoom in and out from center of map - how?)
                e.Surface.Canvas.Scale(DrawingZoom);

                // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
                e.Surface.Canvas.Clear(SKColors.Wheat);

                MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.SELECTIONLAYER);
                selectionLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

                MapLayer cursorLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.CURSORLAYER);

                MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER);
                if (baseLayer.LayerSurface != null)
                {
                    baseLayer.LayerSurface.Canvas.Clear(SKColors.White);
                    baseLayer.Render(baseLayer.LayerSurface.Canvas);
                    e.Surface.Canvas.DrawSurface(baseLayer.LayerSurface, ScrollPoint);
                }

                MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);
                if (oceanTextureLayer.LayerSurface != null)
                {
                    oceanTextureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    oceanTextureLayer.Render(oceanTextureLayer.LayerSurface.Canvas);
                    e.Surface.Canvas.DrawSurface(oceanTextureLayer.LayerSurface, ScrollPoint);
                }

                MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
                if (oceanTextureOverlayLayer.LayerSurface != null)
                {
                    oceanTextureOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    oceanTextureOverlayLayer.Render(oceanTextureOverlayLayer.LayerSurface.Canvas);
                    e.Surface.Canvas.DrawSurface(oceanTextureOverlayLayer.LayerSurface, ScrollPoint);
                }

                MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANDRAWINGLAYER);
                if (oceanDrawingLayer.LayerSurface != null)
                {
                    oceanDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    oceanDrawingLayer.Render(oceanDrawingLayer.LayerSurface.Canvas);
                    e.Surface.Canvas.DrawSurface(oceanDrawingLayer.LayerSurface, ScrollPoint);
                }

                MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER);
                if (windroseLayer.LayerSurface != null)
                {
                    windroseLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    windroseLayer.Render(windroseLayer.LayerSurface.Canvas);
                    e.Surface.Canvas.DrawSurface(windroseLayer.LayerSurface, ScrollPoint);
                }

                MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER);
                if (aboveOceanGridLayer.LayerSurface != null)
                {
                    aboveOceanGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    aboveOceanGridLayer.Render(aboveOceanGridLayer.LayerSurface.Canvas);
                    e.Surface.Canvas.DrawSurface(aboveOceanGridLayer.LayerSurface, ScrollPoint);
                }


                // paint landforms
                MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDCOASTLINELAYER);
                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
                if (landformLayer.LayerSurface != null && landCoastlineLayer.LayerSurface != null)
                {
                    landCoastlineLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                    landformLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                    CURRENT_LANDFORM?.RenderCoastline(landCoastlineLayer.LayerSurface.Canvas);
                    CURRENT_LANDFORM?.RenderLandform(landformLayer.LayerSurface.Canvas);

                    foreach (Landform l in landformLayer.MapLayerComponents)
                    {
                        l.RenderCoastline(landCoastlineLayer.LayerSurface.Canvas);
                        l.RenderLandform(landformLayer.LayerSurface.Canvas);

                        if (l.IsSelected)
                        {
                            // draw an outline around the landform to show that it is selected
                            l.ContourPath.GetBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, LandformSelectPaint);
                        }
                    }

                    e.Surface.Canvas.DrawSurface(landCoastlineLayer.LayerSurface, ScrollPoint);
                    e.Surface.Canvas.DrawSurface(landformLayer.LayerSurface, ScrollPoint);
                }

                e.Surface.Canvas.DrawSurface(selectionLayer.LayerSurface, ScrollPoint);
                e.Surface.Canvas.DrawSurface(cursorLayer.LayerSurface, ScrollPoint);

            }

        }

        private void SKGLRenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            // objects are created and/or initialized on mouse down
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseDownHandler(sender, e);
            }

            if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseDownHandler(sender, e);
            }

        }

        private void SKGLRenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (CURRENT_MAP == null) return;

            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);
            Task.Run(() => UpdateDrawingPointLabel(e.Location.ToSKPoint(), zoomedScrolledPoint));

            DrawCircleCursor(zoomedScrolledPoint, SELECTED_BRUSH_SIZE);

            // objects are drawn or moved on mouse move
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseMoveHandler(e, LandformMethods.LandformBrushSize / 2);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseMoveHandler(sender, e);
            }
            else if (e.Button == MouseButtons.None)
            {
                NoButtonMouseMoveHandler(sender, e, LandformMethods.LandformBrushSize / 2);
            }

            SKGLRenderControl.Invalidate();
        }

        private void SKGLRenderControl_MouseUp(object sender, MouseEventArgs e)
        {
            // objects are finalized or reset on mouse up
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseUpHandler(sender, e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseUpHandler(sender, e);
            }
            else if (e.Button == MouseButtons.None)
            {
                NoButtonMouseUpHandler(sender, e);
            }

        }

        private void SKGLRenderControl_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void SKGLRenderControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void SKGLRenderControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                SetZoomLevel(e.Delta);

                SKGLRenderControl.Invalidate();
            }
        }

        private void SKGLRenderControl_Enter(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void SKGLRenderControl_Leave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        #endregion

        #region SKGLRenderControl Mouse Down Event Handling Methods (called from event handlers)

        private void LeftButtonMouseDownHandler(object sender, MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandformSelect:
                    {
                        Cursor = Cursors.Default;
                        if (CURRENT_MAP != null)
                        {
                            SelectLandformAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case DrawingModeEnum.LandPaint:
                    {
                        Cursor = Cursors.Cross;
                        CURRENT_LANDFORM = new();
                    }
                    break;
            }
        }

        private void RightButtonMouseDownHandler(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SKGLRenderControl Mouse Move Event Handling Methods (called from event handlers)

        #region Left Button Down Handler Method

        private void LeftButtonMouseMoveHandler(MouseEventArgs e, int brushRadius)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
                        Cursor = Cursors.Cross;

                        // TODO: undo/redo
                        if (CURRENT_LANDFORM != null && CURRENT_MAP != null)
                        {
                            CURRENT_LANDFORM.DrawPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                            // compute contour path and inner and outer paths in a separate thread
                            Task.Run(() => LandformMethods.CreateInnerAndOuterPaths(CURRENT_MAP, CURRENT_LANDFORM));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        // TODO: undo/redo
                        if (CURRENT_MAP != null)
                        {
                            LandformMethods.LandformErasePath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                            LandformMethods.EraseLandForm(CURRENT_MAP);
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Right Button Down Handler Method

        private void RightButtonMouseMoveHandler(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region No Button Down Handler Method

        private void NoButtonMouseMoveHandler(object sender, MouseEventArgs e, int brushRadius)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Up Event Handling Methods (called from event handers)

        private void LeftButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
                        Cursor = Cursors.Default;

                        SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

                        if (CURRENT_LANDFORM != null && CURRENT_MAP != null)
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER).MapLayerComponents.Add(CURRENT_LANDFORM);
                            LandformMethods.MergeLandforms(CURRENT_MAP);

                            CURRENT_LANDFORM = null;

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
            }

        }

        private void RightButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void NoButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void DrawCircleCursor(SKPoint point, int brushSize)
        {
            if (CURRENT_MAP == null) return;

            MapLayer cursorLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.CURSORLAYER);
            cursorLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            if (brushSize > 0)
            {
                cursorLayer.LayerSurface?.Canvas.DrawCircle(point, brushSize / 2, CursorCirclePaint);
            }
        }

        #endregion

        #region ScrollBar Event Handlers
        private void MapRenderVScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.Y = (float)-e.NewValue;
            DrawingPoint.Y = (float)e.NewValue;

            SKGLRenderControl.Invalidate();
        }

        private void MapRenderHScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.X = (float)-e.NewValue;
            DrawingPoint.X = (float)e.NewValue;

            SKGLRenderControl.Invalidate();
        }
        #endregion

        #region Main Form Methods

        private static void SetZoomLevel(int upDown)
        {
            // TODO: scrollbars need to be updated
            // increase/decrease zoom by 10%, limiting to no less than 10% and no greater than 800%
            DrawingZoom = (upDown < 0) ? Math.Max(0.1f, DrawingZoom - 0.1f) : Math.Min(8.0f, DrawingZoom + 0.1f);
        }

        private void PopulateControlsWithAssets(int assetCount)
        {
            SetStatusText("Loaded: " + assetCount + " assets.");

            foreach (MapSymbolCollection collection in AssetManager.MAP_SYMBOL_COLLECTIONS)
            {
                //SymbolCollectionsListBox.Items.Add(collection.GetCollectionName());
            }

            //AddMapBoxesToLabelBoxTable(MapLabelMethods.MAP_BOX_TEXTURES);

            //AddMapFramesToFrameTable(OverlayMethods.MAP_FRAME_TEXTURES);

            foreach (LabelPreset preset in AssetManager.LABEL_PRESETS)
            {
                if (!string.IsNullOrEmpty(preset.LabelPresetTheme)
                    && AssetManager.CURRENT_THEME != null
                    && preset.LabelPresetTheme == AssetManager.CURRENT_THEME.ThemeName)
                {
                    //LabelPresetCombo.Items.Add(preset.LabelPresetName);
                }
            }

            /*
            if (LabelPresetCombo.Items.Count > 0)
            {
                LabelPresetCombo.SelectedIndex = 0;

                LabelPreset? selectedPreset = AssetManager.LABEL_PRESETS.Find(x => !string.IsNullOrEmpty((string?)LabelPresetCombo.Items[0]) && x.LabelPresetName == (string?)LabelPresetCombo.Items[0]);

                if (selectedPreset != null)
                {
                    SetLabelValuesFromPreset(selectedPreset);
                }
            }
            */
        }

        public void SetStatusText(string text)
        {
            // set the test of the first status strip text box
            ApplicationStatusStrip.Items[0].Text = text;
        }

        private void UpdateDrawingPointLabel(SKPoint cursorPoint, SKPoint mapPoint)
        {
            DrawingPointLabel.Text = "Cursor Point: "
                + ((int)cursorPoint.X).ToString()
                + " , "
                + ((int)cursorPoint.Y).ToString()
                + "   Map Point: "
                + ((int)mapPoint.X).ToString()
                + " , "
                + ((int)mapPoint.Y).ToString();

            ApplicationStatusStrip.Invalidate();
        }

        private void UpdateMapNameAndSize()
        {
            if (CURRENT_MAP != null)
            {
                MapNameLabel.Text = CURRENT_MAP.MapName;
                MapSizeLabel.Text = "Map Size: " + CURRENT_MAP.MapWidth.ToString() + " x " + CURRENT_MAP.MapHeight.ToString();
            }
        }

        public static void DeselectAllMapComponents(MapComponent selectedComponent)
        {
            if (CURRENT_MAP == null) return;

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

            foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                if (selectedComponent != null && selectedComponent is Landform landform && landform == l) continue;
                l.IsSelected = false;
            }

            /*
            foreach (MapPaintedWaterFeature w in WaterFeatureMethods.PAINTED_WATERFEATURE_LIST)
            {
                if (selectedComponent != null && selectedComponent is MapPaintedWaterFeature waterFeature && waterFeature == w) continue;
                w.IsSelected = false;
            }

            foreach (MapRiver r in WaterFeatureMethods.MAP_RIVER_LIST)
            {
                if (selectedComponent != null && selectedComponent is MapRiver river && river == r) continue;
                r.IsSelected = false;
            }

            foreach (MapPath p in MapPathMethods.GetMapPathList())
            {
                if (selectedComponent != null && selectedComponent is MapPath mapPath && mapPath == p) continue;
                p.IsSelected = false;
            }

            foreach (MapSymbol s in SymbolMethods.PlacedSymbolList)
            {
                if (selectedComponent != null && selectedComponent is MapSymbol mapSymbol && mapSymbol == s) continue;
                s.SetIsSelected(false);
            }

            foreach (MapLabel l in MapLabelMethods.MAP_LABELS)
            {
                if (selectedComponent != null && selectedComponent is MapLabel mapLabel && mapLabel == l) continue;
                l.IsSelected = false;
            }

            foreach (PlacedMapBox b in MapLabelMethods.MAP_BOXES)
            {
                if (selectedComponent != null && selectedComponent is PlacedMapBox mapBox && mapBox == b) continue;
                b.IsSelected = false;
            }

            foreach (MapRegion r in MapRegionMethods.MAP_REGION_LIST)
            {
                if (selectedComponent != null && selectedComponent is MapRegion region && region == r) continue;
                r.IsSelected = false;
            }
            */
        }

        private static void SetSelectedBrushSize(int brushSize)
        {
            SELECTED_BRUSH_SIZE = brushSize;
        }

        #endregion

        #region Land Tab Event Handlers

        private void LandformSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LandformSelect;
            SELECTED_BRUSH_SIZE = 0;
        }

        private void LandformPaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LandPaint;
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        private void LandformEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LandErase;

            // TODO: define landform eraser size
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        private void LandBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformBrushSize = LandBrushSizeTrack.Value;
            TOOLTIP.Show(LandformMethods.LandformBrushSize.ToString(), LandBrushSizeTrack, new Point(LandBrushSizeTrack.Right - 42, LandBrushSizeTrack.Top - 58), 2000);
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        #endregion

        #region LANDFORM METHODS
        internal static Landform? SelectLandformAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            Landform? selectedLandform = null;

            List<MapComponent> landformComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformComponents.Count; i++)
            {
                if (landformComponents[i] is Landform mapLandform)
                {
                    SKPath boundaryPath = mapLandform.DrawPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            mapLandform.IsSelected = !mapLandform.IsSelected;

                            if (mapLandform.IsSelected)
                            {
                                selectedLandform = mapLandform;
                            }
                            break;
                        }
                    }
                }
            }

#pragma warning disable CS8604 // Possible null reference argument.
            DeselectAllMapComponents(selectedLandform);
#pragma warning restore CS8604 // Possible null reference argument.

            return selectedLandform;
        }

        #endregion
    }

}
