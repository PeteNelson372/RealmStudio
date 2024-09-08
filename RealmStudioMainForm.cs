using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private static DrawingModeEnum CURRENT_DRAWING_MODE = DrawingModeEnum.LandPaint;

        private static int MAP_WIDTH = 8000;
        private static int MAP_HEIGHT = 8000;

        private static SKSurface? BackgroundSurface = null;
        private static SKSurface? LandSurface = null;
        private static SKSurface? CoastSurface = null;
        private static SKSurface? CoastSurface2 = null;

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);
        private static SKPoint ZoomPoint = new(0, 0);

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
            }
        }

        #endregion

        #region SKGLRenderControl Event Handlers

        private void SKGLRenderControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            // if the surfaces haven't been created, create them
            BackgroundSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, true, new SKImageInfo(MAP_WIDTH, MAP_HEIGHT));

            LandSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, true, new SKImageInfo(MAP_WIDTH, MAP_HEIGHT));

            CoastSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, true, new SKImageInfo(MAP_WIDTH, MAP_HEIGHT));

            CoastSurface2 ??= SKSurface.Create(SKGLRenderControl.GRContext, true, new SKImageInfo(MAP_WIDTH, MAP_HEIGHT));

            // handle zoom-in and zoom-out (zoom in and out from center of map - how?)
            e.Surface.Canvas.Scale(DrawingZoom);

            // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
            e.Surface.Canvas.Clear(SKColors.Black);

            LandSurface?.Canvas.DrawPath(LandformPath, LandformPaint);
            CoastSurface?.Canvas.DrawPath(CoastPath, CoastlinePaint);
            CoastSurface2?.Canvas.DrawPath(CoastPath2, CoastlinePaint2);

            BackgroundSurface.Canvas.Clear(SKColors.White);

            e.Surface.Canvas.DrawSurface(BackgroundSurface, ScrollPoint);

            e.Surface.Canvas.DrawSurface(CoastSurface2, ScrollPoint);
            e.Surface.Canvas.DrawSurface(CoastSurface, ScrollPoint);
            e.Surface.Canvas.DrawSurface(LandSurface, ScrollPoint);
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
                        LandformPath?.Dispose();
                        LandformPath = new()
                        {
                            FillType = SKPathFillType.Winding,
                        };

                        CoastPath?.Dispose();
                        CoastPath = new()
                        {
                            FillType = SKPathFillType.Winding,
                        };

                        CoastPath2?.Dispose();
                        CoastPath2 = new()
                        {
                            FillType = SKPathFillType.Winding,
                        };
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

        private void LeftButtonMouseMoveHandler(MouseEventArgs e, object brushRadius)
        {
            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
                        SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

                        LandformPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, 30);
                        CoastPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, 36);
                        CoastPath2.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, 42);

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
