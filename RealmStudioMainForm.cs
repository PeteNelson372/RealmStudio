using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static DrawingModeEnum CURRENT_DRAWING_MODE = DrawingModeEnum.LandPaint;

        private static RealmStudioMap? CURRENT_MAP = null;

        private static Landform? CURRENT_LANDFORM = null;

        private static SKSurface? BackgroundSurface = null;
        private static SKSurface? LandSurface = null;
        private static SKSurface? CoastSurface = null;
        private static SKSurface? CoastSurface2 = null;

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);

        private static float DrawingZoom = 1.0f;

        private static SKPath LandformPath = new()
        {
            FillType = SKPathFillType.Winding,
        };

        private static SKPath CoastPath = new()
        {
            FillType = SKPathFillType.Winding,
        };

        private static SKPath CoastPath2 = new()
        {
            FillType = SKPathFillType.Winding,
        };

        private static SKPaint LandformPaint = new()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.AliceBlue,
            IsAntialias = true,
        };

        private static SKPaint CoastlinePaint = new()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Aqua,
            IsAntialias = true,
        };

        private static SKPaint CoastlinePaint2 = new()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.CadetBlue,
            IsAntialias = true,
        };

        public RealmStudioMainForm()
        {
            InitializeComponent();
            SKGLRenderControl.MouseWheel += SKGLRenderControl_MouseWheel;
        }

        #region Main Form Event Handlers

        private void RealmStudioMainForm_Load(object sender, EventArgs e)
        {

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

                if (CURRENT_MAP.MapLayers.Count == 0)
                {
                    MessageBox.Show("MapLayers not created.");
                    Application.Exit();
                }
            }
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
                BackgroundSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, true, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));

                foreach (MapLayer layer in CURRENT_MAP.MapLayers)
                {
                    // if the surfaces haven't been created, create them
                    // TODO: for landform and land coastline,
                    // create multiple layers or create multiple surfaces in a single layer?
                    layer.LayerSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, true, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
                }


                // handle zoom-in and zoom-out (TODO: zoom in and out from center of map - how?)
                e.Surface.Canvas.Scale(DrawingZoom);

                // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
                e.Surface.Canvas.Clear(SKColors.Black);

                //LandSurface?.Canvas.DrawPath(LandformPath, LandformPaint);
                //CoastSurface?.Canvas.DrawPath(CoastPath, CoastlinePaint);
                //CoastSurface2?.Canvas.DrawPath(CoastPath2, CoastlinePaint2);

                BackgroundSurface.Canvas.Clear(SKColors.White);

                e.Surface.Canvas.DrawSurface(BackgroundSurface, ScrollPoint);

                //e.Surface.Canvas.DrawSurface(CoastSurface2, ScrollPoint);
                //e.Surface.Canvas.DrawSurface(CoastSurface, ScrollPoint);

                if (CURRENT_LANDFORM != null)
                {
                    MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
                    landformLayer.LayerSurface?.Canvas.DrawPath(CURRENT_LANDFORM.DrawPath, LandformPaint);

                    e.Surface.Canvas.DrawSurface(landformLayer.LayerSurface, ScrollPoint);
                }
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
            // objects are drawn or moved on mouse move

            int brushRadius = 30;

            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseMoveHandler(e, brushRadius);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseMoveHandler(sender, e);
            }
            else if (e.Button == MouseButtons.None)
            {
                NoButtonMouseMoveHandler(sender, e, brushRadius);
            }
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

        private void SKGLRenderControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                SetZoomLevel(e.Delta);

                SKGLRenderControl.Invalidate();
            }
        }

        #endregion

        #region SKGLRenderControl Mouse Down Event Handling Methods (called from event handlers)

        private void LeftButtonMouseDownHandler(object sender, MouseEventArgs e)
        {
            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
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
            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
                        SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

                        if (CURRENT_LANDFORM != null && CURRENT_MAP != null)
                        {
                            CURRENT_LANDFORM.DrawPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);
                            //CURRENT_LANDFORM.ContourPath = DrawingMethods.GetContourPathFromPath(CURRENT_LANDFORM.DrawPath,
                            //    CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight, out List<SKPoint> contourPoints);

                        }

                        SKGLRenderControl.Invalidate();
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

        private void NoButtonMouseMoveHandler(object sender, MouseEventArgs e, object brushRadius)
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
                        SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

                        if (CURRENT_LANDFORM != null && CURRENT_MAP != null)
                        {
                            CURRENT_LANDFORM.ContourPath = DrawingMethods.GetContourPathFromPath(CURRENT_LANDFORM.DrawPath,
                                CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight, out List<SKPoint> contourPoints);

                        }

                        SKGLRenderControl.Invalidate();
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

        #endregion

        #region ScrollBar Event Handlers
        private void MapRenderVScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.Y = MAP_HEIGHT * ((float)-e.NewValue / 100);
            DrawingPoint.Y = MAP_HEIGHT * ((float)e.NewValue / 100);

            SKGLRenderControl.Invalidate();
        }

        private void MapRenderHScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.X = MAP_WIDTH * ((float)-e.NewValue / 100);
            DrawingPoint.X = MAP_WIDTH * ((float)e.NewValue / 100);

            SKGLRenderControl.Invalidate();
        }
        #endregion

        #region Drawing Methods

        private static void SetZoomLevel(int upDown)
        {
            // TODO: scrollbars need to be updated
            // increase/decrease zoom by 10%, limiting to no less than 10% and no greater than 800%
            DrawingZoom = (upDown < 0) ? Math.Max(0.1f, DrawingZoom - 0.1f) : Math.Min(8.0f, DrawingZoom + 0.1f);
        }

        #endregion



    }

}
