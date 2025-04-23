/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
using HelixToolkit.Wpf;
using RealmStudio.Properties;
using SharpAvi.Codecs;
using SharpAvi.Output;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace RealmStudio
{
    /***
     * This form hosts the ThreeDViwerControl WPF UserControl that displays a 3D model.
     */
    public partial class ThreeDView : Form
    {
        private readonly ThreeDViewerControl ThreeDViewer;

        private double initialCameraDistance;

        private readonly List<string>? ModelUpdateQueue;
        private string CloudTextureFileName = string.Empty;

        private string? LoadedModelString;

        private System.Timers.Timer? _animationTimer;
        private bool animationTimerEnabled;

        private double framePerSecond = 60.0;       // frames per second
        private double revolutionsPerMinute = 1.0;  // revolutions/minute

        private int cloudTextureOpacity = 64; // 0-255
        private double cloudRotationRate = 1.0; // percentage of world rotation rate

        private System.Windows.Media.Color cloudColor = System.Windows.Media.Colors.White;
        private System.Windows.Media.Color sunlightColor = System.Windows.Media.Colors.White;
        private System.Windows.Media.Color ambientLightColor = System.Windows.Media.Color.FromArgb(64, 128, 128, 128);

        private bool recordingEnabled;
        private int frameCount;

        private CancellationTokenSource tokenSource = new();
        private CancellationToken cancelToken;

        private AviWriter? aviWriter;
        private string aviTempFileName = string.Empty;
        private IAviVideoStream? videoStream;
        private Bitmap? sceneBackground;

        private readonly LocalStar localStarData = new();

        private readonly ModelVisual3D GridlinesModel = new();
        private readonly GridLinesVisual3D GridLines = new()
        {
            Center = new Point3D(0, -0.5, 0),
            Normal = new Vector3D(0, 1, 0),
            Width = 20,
            Length = 20,
            MinorDistance = 1,
            MajorDistance = 10,
            Thickness = 0.002,
            Fill = System.Windows.Media.Brushes.Gray
        };

        #region Constructors

        public ThreeDView(string formTitle)
        {
            InitializeComponent();
            ThreeDViewOverlay.Text = formTitle;

            // construct the WPF ThreeDViewerControl UserControl
            ThreeDViewer = new();

            var camera = ThreeDViewer.HelixTKViewport.Camera as ProjectionCamera;
            if (camera != null)
            {
                initialCameraDistance = camera.Position.ToVector3D().Length;
            }

            FrameRateCombo.SelectedIndex = 4;
        }

        public ThreeDView(string formTitle, List<string> modelUpdateQueue)
        {
            InitializeComponent();
            ThreeDViewOverlay.Text = formTitle;
            ModelUpdateQueue = modelUpdateQueue;

            LoadModelButton.Enabled = false;

            // construct the WPF ThreeDViewerControl UserControl
            ThreeDViewer = new();

            var camera = ThreeDViewer.HelixTKViewport.Camera as ProjectionCamera;
            if (camera != null)
            {
                initialCameraDistance = camera.Position.ToVector3D().Length;
            }

            FrameRateCombo.SelectedIndex = 4;
        }

        #endregion

        #region Form Event Handlers

        private void ThreeDView_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAnimationTimer();

            ModelUpdateQueue?.Clear();

            if (ParentForm != null && HeightMapManager.CurrentHeightMapView == this)
            {
                HeightMapManager.CurrentHeightMapView = null;
            }
        }

        private void ThreeDView_Load(object sender, EventArgs e)
        {
            // Create the ElementHost control for hosting the
            // WPF ThreeDViewerControl UserControl
            // and assign the WPF UserControl to the ElementHost control's
            // Child property.
            ElementHost host = new()
            {
                Parent = TDContainerPanel,
                Dock = DockStyle.Fill,
                Child = ThreeDViewer,
            };

            // add the ElementHost control to the container panel's controls.
            TDContainerPanel.Controls.Add(host);

            // add handlers for events
            ThreeDViewer.ModelGroup.Changed += ModelGroup_Changed;

            // add gridlines
            GridlinesModel.SetName("GridLines");
            GridlinesModel.Children.Add(GridLines);
            ThreeDViewer.HelixTKViewport.Children.Add(GridlinesModel);

            ThreeDViewer.HelixTKViewport.PanGesture = null;
            ThreeDViewer.HelixTKViewport.PanGesture2 = new System.Windows.Input.MouseGesture(System.Windows.Input.MouseAction.LeftClick);

            ThreeDViewer.HelixTKViewport.RotateGesture = null;
            ThreeDViewer.HelixTKViewport.RotateGesture2 = null;
            ThreeDViewer.HelixTKViewport.RotateGesture2 = new System.Windows.Input.MouseGesture(System.Windows.Input.MouseAction.RightClick);

            ThreeDViewer.HelixTKViewport.ZoomSensitivity = 1.0;

            ThreeDViewer.HelixTKViewport.InfiniteSpin = true;
        }

        #endregion

        #region 3D Model Loading and Update

        public void UpdateView()
        {
            // update the model from the queue
            // this is used when the heightmap is updated and the 3D
            // view of the heightmap is being automatically updated
            if (ModelUpdateQueue != null && ModelUpdateQueue.Count > 0)
            {
                string? objModelString = ModelUpdateQueue.Last();

                if (!string.IsNullOrEmpty(objModelString))
                {
                    LoadModelFromQueue(objModelString);
                    ModelUpdateQueue.Clear();
                }
            }
        }

        private void LoadModelFromQueue(string objModelString)
        {
            try
            {
                if (!string.IsNullOrEmpty(objModelString))
                {
                    LoadedModelString = objModelString;

                    ObjReader objReader = new();

                    using MemoryStream ms = new(Encoding.ASCII.GetBytes(objModelString));
                    Model3DGroup modelGroup = objReader.Read(ms);

                    int vertexCount = 0;
                    int faceCount = 0;

                    for (int i = 0; i < modelGroup.Children.Count; i++)
                    {
                        if (modelGroup.Children[i] is GeometryModel3D gm3d)
                        {
                            MeshGeometry3D mg3d = (MeshGeometry3D)gm3d.Geometry;
                            vertexCount = mg3d.Positions.Count;
                            faceCount = mg3d.TriangleIndices.Count / 3;
                            break;
                        }
                    }

                    ModelStatisticsLabel.Text = "Loaded " + vertexCount + " vertices; " + faceCount + " faces.";
                    LoadModelGroupIntoViewer(modelGroup);
                }
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
                MessageBox.Show("Error loading model: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void LoadModelGroupIntoViewer(Model3DGroup modelGroup)
        {
            // this method loads a model group into the 3D viewer;
            // this is used when the model is loaded from a file or
            // when the model is updated from the heightmap

            ThreeDViewer.HelixTKViewport.Items.Add(new DefaultLights());

            // create a directional light matching the default one in the ThreeDViewerControl
            DirectionalLight light = new()
            {
                Color = System.Windows.Media.Colors.White,
                Direction = new Vector3D(3, -4, 5)
            };

            GeometryModel3D materialModel = new()
            {
                Material = new DiffuseMaterial()
                {
                    Brush = new System.Windows.Media.SolidColorBrush()
                    {
                        Color = System.Windows.Media.Colors.Red,
                        Opacity = 1
                    },
                },
            };

            if (modelGroup != null)
            {
                modelGroup.SetName("DisplayedModel");
                modelGroup.Children.Add(materialModel);

                ThreeDViewer.ModelGroup.Children.Clear();
                ThreeDViewer.ModelGroup.Children.Add(modelGroup);
                ThreeDViewer.ModelGroup.Children.Add(light);
                ThreeDViewer.ModelGroup.Children.Add(materialModel);

                ThreeDViewer.HelixTKViewport.ResetCamera();
                ThreeDViewer.HelixTKViewport.FitView(new Vector3D(0, 0, 1), new Vector3D(0, 1, 0));
            }
        }

        #endregion

        #region 3D Model Event Handlers
        private void ModelGroup_Changed(object? sender, EventArgs e)
        {
            // no-op at this time
        }

        #endregion

        #region 3D Model Methods
        public double GetZoomPercentage()
        {
            var camera = ThreeDViewer.HelixTKViewport.Camera as ProjectionCamera;
            if (camera == null || initialCameraDistance == 0)
                return 100;

            double currentDistance = camera.Position.ToVector3D().Length;
            double zoomPercent = (initialCameraDistance / currentDistance) * 100;

            return zoomPercent;
        }

        #endregion

        #region Animation

        public bool AnimationEnabled
        {
            get { return animationTimerEnabled; }
            set
            {
                animationTimerEnabled = value;

                if (animationTimerEnabled)
                {
                    StartAnimationTimer();
                }
                else
                {
                    StopAnimationTimer();
                }
            }
        }

        internal void StartAnimationTimer()
        {
            // stop the animation timer
            StopAnimationTimer();

            double animationIntervalMillis = 1000.0 / framePerSecond; // 1000 ms / fps

            // start the autosave timer
            _animationTimer = new System.Timers.Timer
            {
                Interval = animationIntervalMillis,
                AutoReset = true,
                SynchronizingObject = this,
            };

            _animationTimer.Elapsed += new ElapsedEventHandler(AnimationTimerEventHandler);
            _animationTimer.Start();
        }

        private void AnimationTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            double revolutionsPerSecond = revolutionsPerMinute / 60.0;

            double degreesPerFrame = 360.0 / (framePerSecond / revolutionsPerSecond); // degrees per frame

            SphereVisual3D worldGlobe = (SphereVisual3D)ThreeDViewer.HelixTKViewport.Children[0];

            // rotate around Z axis
            Vector3D axis = new(0, 0, 1);

            // Get the matrix indicating the current transformation value
            Matrix3D transformationMatrix = worldGlobe.Content.Transform.Value;

            // rotate around the world globe center Z axis
            transformationMatrix.RotateAt(new Quaternion(axis, degreesPerFrame), worldGlobe.Center);

            // do the rotation transform
            worldGlobe.Content.Transform = new MatrixTransform3D(transformationMatrix);

            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the cloud layer
                foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                {
                    if (child is SphereVisual3D cloudLayer && cloudLayer.GetName() == "CloudLayer")
                    {
                        // rotate the cloud layer, if there is one
                        // Get the matrix indicating the current transformation value
                        Matrix3D cloudTransformationMatrix = cloudLayer.Content.Transform.Value;
                        // rotate around the cloud layer center Z axis
                        cloudTransformationMatrix.RotateAt(new Quaternion(axis, degreesPerFrame * cloudRotationRate), cloudLayer.Center);
                        // do the rotation transform
                        cloudLayer.Content.Transform = new MatrixTransform3D(cloudTransformationMatrix);
                    }
                }
            }
        }

        internal void StopAnimationTimer()
        {
            _animationTimer?.Stop();
            _animationTimer?.Dispose();
            _animationTimer = null;
        }

        #endregion

        #region Form Control Event Handlers (not World Globe specific)

        private void CloseFormButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadModelButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Open 3D Model",
                    DefaultExt = "obj",
                    Filter = "3D Model files|*.obj;*.stl;*.3ds;*.lwo;*.off|OBJ files (*.obj)|*.obj|All files (*.*)|*.*",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        Model3DGroup? modelGroup = null;

                        LoadedModelString = File.ReadAllText(ofd.FileName);

                        try
                        {
                            switch (Path.GetExtension(ofd.FileName).ToUpper(System.Globalization.CultureInfo.CurrentCulture))
                            {
                                case ".3DS":
                                    StudioReader tdsReader = new();
                                    modelGroup = tdsReader.Read(ofd.FileName);
                                    break;
                                case ".LWO":
                                    LwoReader lwoReader = new();
                                    modelGroup = lwoReader.Read(ofd.FileName);
                                    break;
                                case ".OFF":
                                    OffReader offReader = new();
                                    modelGroup = offReader.Read(ofd.FileName);
                                    break;
                                case ".OBJ":
                                    ObjReader objReader = new();
                                    modelGroup = objReader.Read(ofd.FileName);
                                    break;
                                case ".STL":
                                    StLReader stlReader = new();
                                    modelGroup = stlReader.Read(ofd.FileName);
                                    break;
                                default:
                                    {
                                        throw new Exception("Unsupported 3D model format.");
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                            throw;
                        }

                        if (modelGroup != null)
                        {
                            LoadModelGroupIntoViewer(modelGroup);

                            int vertexCount = 0;
                            int faceCount = 0;

                            for (int i = 0; i < modelGroup.Children.Count; i++)
                            {
                                if (modelGroup.Children[i] is GeometryModel3D gm3d)
                                {
                                    MeshGeometry3D mg3d = (MeshGeometry3D)gm3d.Geometry;
                                    vertexCount = mg3d.Positions.Count;
                                    faceCount = mg3d.TriangleIndices.Count / 3;
                                    break;
                                }
                            }

                            ModelStatisticsLabel.Text = "Loaded " + vertexCount + " vertices; " + faceCount + " faces.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
                MessageBox.Show("Error loading model: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        private void SaveModelButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LoadedModelString))
            {
                HeightMapTo3DModel.WriteObjModelToFile([.. LoadedModelString.Split(Environment.NewLine)]);
            }
        }

        private void ShowGridlinesCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowGridlinesCheck.Checked)
            {
                ThreeDViewer.HelixTKViewport.Children.Add(GridlinesModel);
            }
            else
            {
                ThreeDViewer.HelixTKViewport.Children.Remove(GridlinesModel);
            }
        }

        private void ResetCameraButton_Click(object sender, EventArgs e)
        {
            ThreeDViewer.HelixTKViewport.ResetCamera();
        }

        private void ThreeDView_SizeChanged(object sender, EventArgs e)
        {
            ThreeDViewer.HelixTKViewport.FitView(new Vector3D(0, 0, 1), new Vector3D(0, 1, 0));
        }

        private void ChangeAxesButton_Click(object sender, EventArgs e)
        {
            Vector3D xUp = new(1, 0, 0);
            Vector3D yUp = new(0, 1, 0);
            Vector3D zUp = new(0, 0, 1);

            Vector3D xDir = new(0, 1, 0);
            Vector3D yDir = new(0, 0, 1);
            Vector3D zDir = new(1, 0, 0);

            Vector3D newUp;
            Vector3D newDir;

            Vector3D currentUp = ThreeDViewer.ModelCamera.UpDirection;

            if (currentUp == xUp)
            {
                newUp = yUp;
                newDir = yDir;
            }
            else if (currentUp == yUp)
            {
                newUp = zUp;
                newDir = zDir;
            }
            else if (currentUp == zUp)
            {
                newUp = xUp;
                newDir = xDir;
            }
            else
            {
                newUp = yUp;
                newDir = yDir;
            }

            ThreeDViewer.HelixTKViewport.ResetCamera();
            ThreeDViewer.ModelCamera.UpDirection = newUp;
            ThreeDViewer.ModelCamera.LookDirection = newDir;

            ThreeDViewer.HelixTKViewport.FitView(newDir, newUp);
        }

        private void EnableAnimationSwitch_CheckedChanged()
        {
            if (EnableAnimationSwitch.Checked)
            {
                AnimationEnabled = true;
            }
            else
            {
                AnimationEnabled = false;
            }
        }

        private void LoadBackgroundButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Open 3D View Background Image",
                    DefaultExt = "png",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false,
                    Filter = UtilityMethods.GetCommonImageFilter()
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            Bitmap b = (Bitmap)Bitmap.FromFile(ofd.FileName);
                            sceneBackground = b;

                            // apply the bitmap to the background of the 3D view
                            ThreeDViewer.HelixTKViewport.Background = new ImageBrush(new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute)));

                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                            throw;
                        }

                    }
                }
            }
            catch { }
        }

        private void CaptureSnapshotButton_Click(object sender, EventArgs e)
        {
            SKBitmap? formattedBackground = null;

            if (sceneBackground != null)
            {
                formattedBackground = DrawingMethods.ResizeSKBitmap(sceneBackground.ToSKBitmap(),
                    new SKSizeI((int)ThreeDViewer.HelixTKViewport.ActualWidth, (int)ThreeDViewer.HelixTKViewport.ActualHeight));
            }

            // take a snapshot of the 3D view
            using SKBitmap frameBitmap = new((int)ThreeDViewer.HelixTKViewport.ActualWidth, (int)ThreeDViewer.HelixTKViewport.ActualHeight);
            using SKCanvas canvas = new(frameBitmap);

            RenderTargetBitmap rtb = new(
                (int)ThreeDViewer.HelixTKViewport.Viewport.ActualWidth,
                (int)ThreeDViewer.HelixTKViewport.Viewport.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            rtb.Render(ThreeDViewer.HelixTKViewport.Viewport);

            using Bitmap? b = DrawingMethods.BitmapSourceToBitmap(rtb);

            if (b != null)
            {
                if (formattedBackground != null)
                {
                    // composite the background onto the frameBitmap
                    canvas.DrawBitmap(formattedBackground, 0, 0);
                }

                canvas.DrawBitmap(b.ToSKBitmap(), 0, 0);
            }

            // save the snapshot to a file
            SaveFileDialog sfd = new()
            {
                DefaultExt = "png",
                CheckWriteAccess = true,
                ExpandedMode = true,
                AddExtension = true,
                SupportMultiDottedExtensions = false,
                AddToRecent = true,
                Filter = UtilityMethods.GetCommonImageFilter(),
                Title = "Save World Globe Snapshot",
                FilterIndex = 3,
            };

            DialogResult sfdresult = sfd.ShowDialog();

            if (sfdresult == DialogResult.OK)
            {
                FileStream fs = new(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None);

                switch (Path.GetExtension(sfd.FileName).ToLower(System.Globalization.CultureInfo.CurrentCulture))
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".jfif":
                    case ".jpe:":
                        frameBitmap.Encode(fs, SKEncodedImageFormat.Jpeg, 100);
                        break;
                    case ".png":
                        frameBitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
                        break;
                    case ".bmp":
                    case ".dib":
                    case ".rle":
                        frameBitmap.Encode(fs, SKEncodedImageFormat.Bmp, 100);
                        break;
                    case ".gif":
                        frameBitmap.Encode(fs, SKEncodedImageFormat.Gif, 100);
                        break;
                    default:
                        frameBitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
                        break;
                }

                ModelStatisticsLabel.Text = "Snapshot saved: " + Path.GetFileName(sfd.FileName);
                ModelStatisticsLabel.Refresh();
            }
        }

        #endregion

        #region Form Control Event Handlers (World Globe specific)

        private void FrameRateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopAnimationTimer();
            if (FrameRateCombo.SelectedIndex < 0)
            {
                return;
            }

            string? fpsStr = (string?)FrameRateCombo.Items[FrameRateCombo.SelectedIndex];

            if (fpsStr != null)
            {
                framePerSecond = double.Parse(fpsStr, System.Globalization.CultureInfo.CurrentCulture);
            }

            if (animationTimerEnabled)
            {
                StartAnimationTimer();
            }
        }

        private void RotationRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            StopAnimationTimer();
            revolutionsPerMinute = (double)RotationRateUpDown.Value;

            if (animationTimerEnabled)
            {
                StartAnimationTimer();
            }
        }

        private void ClearBackgroundButton_Click(object sender, EventArgs e)
        {
            ThreeDViewer.HelixTKViewport.Background = null;
        }

        private void EnableCloudsSwitch_CheckedChanged()
        {
            if (EnableCloudsSwitch.Checked)
            {
                ShowCloudLayer();
            }
            else
            {
                RemoveCloudLayer();
            }
        }

        private void ShowHideCloudPanelButton_Click(object sender, EventArgs e)
        {
            CloudsPanel.Visible = !CloudsPanel.Visible;
        }

        private void LoadCloudTextureButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Select Cloud Texture",
                    DefaultExt = "png",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false,
                    Filter = UtilityMethods.GetCommonImageFilter()
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            CloudTextureFileName = ofd.FileName;

                            if (EnableCloudsSwitch.Checked)
                            {
                                ShowCloudLayer();
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                            throw;
                        }
                    }
                }
            }
            catch { }
        }

        private void CloudTextureOpacityTrack_Scroll(object sender, EventArgs e)
        {
            cloudTextureOpacity = CloudTextureOpacityTrack.Value;
            ShowCloudLayer();
        }

        private void DefaultCloudTextureRadio_CheckedChanged(object sender, EventArgs e)
        {
            ShowCloudLayer();
        }

        private void CustomCloudTextureRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CloudTextureFileName))
            {
                ShowCloudLayer();
            }
            else
            {
                CustomCloudTextureRadio.Checked = false;
            }
        }

        private void CloudRotationRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            cloudRotationRate = (double)CloudRotationRateUpDown.Value;
        }

        private void SelectCloudColorButton_Click(object sender, EventArgs e)
        {
            System.Drawing.Color c = UtilityMethods.SelectColorFromDialog(this, System.Drawing.Color.FromArgb(cloudColor.A, cloudColor.R, cloudColor.G, cloudColor.B));
            if (c != System.Drawing.Color.Empty)
            {
                SelectCloudColorButton.BackColor = c;
                cloudColor = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
                ShowCloudLayer();
            }
        }

        private void ShowHideLightingPanelButton_Click(object sender, EventArgs e)
        {
            LightingPanel.Visible = !LightingPanel.Visible;
        }

        private void ShowHideFeaturesButton_Click(object sender, EventArgs e)
        {
            FeaturesPanel.Visible = !FeaturesPanel.Visible;
        }

        private void EnableSunlightSwitch_CheckedChanged()
        {
            if (EnableSunlightSwitch.Checked)
            {
                ShowSunlight();
            }
            else
            {
                RemoveSunlight();
            }
        }

        private void SunlightHorizontalDirectionTrack_Scroll(object sender, EventArgs e)
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the sunlight directional light
                for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is DirectionalLight dl && dl.GetName() == "Sunlight")
                    {
                        // rotate the sunlight
                        // hangle and vangle are in degrees
                        double hangle = SunlightHorizontalDirectionTrack.Value;
                        double vangle = SunlightVerticalDirectionTrack.Value;

                        Quaternion combinedQuaternion = Quaternion.Multiply(new Quaternion(new Vector3D(0, 0, 1), hangle), new Quaternion(new Vector3D(1, 0, 0), vangle));
                        dl.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                        break;
                    }
                }
            }
        }

        private void SunlightVerticalDirectionTrack_Scroll(object sender, EventArgs e)
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the sunlight directional light
                for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is DirectionalLight dl && dl.GetName() == "Sunlight")
                    {
                        // hangle and vangle are in degrees (0 - 359)
                        double hangle = SunlightHorizontalDirectionTrack.Value;
                        double vangle = SunlightVerticalDirectionTrack.Value;

                        Quaternion combinedQuaternion = Quaternion.Multiply(new Quaternion(new Vector3D(0, 0, 1), hangle), new Quaternion(new Vector3D(1, 0, 0), vangle));
                        dl.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                        break;
                    }
                }
            }
        }

        private void SunlightColorButton_Click(object sender, EventArgs e)
        {
            System.Drawing.Color c = UtilityMethods.SelectColorFromDialog(this, System.Drawing.Color.FromArgb(sunlightColor.A, sunlightColor.R, sunlightColor.G, sunlightColor.B));
            if (c != System.Drawing.Color.Empty)
            {
                SunlightColorButton.BackColor = c;
                sunlightColor = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);

                if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
                {
                    // find the sunlight directional light
                    foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                    {
                        if (child is ModelVisual3D m3d && m3d.Content is DirectionalLight dl && dl.GetName() == "Sunlight")
                        {
                            dl.Color = sunlightColor;
                        }
                    }
                }
            }
        }

        private void EnableAmbientLightSwitch_CheckedChanged()
        {
            if (EnableAmbientLightSwitch.Checked)
            {
                ShowAmbientLight();
            }
            else
            {
                RemoveAmbientLight();
            }
        }

        private void AmbientLightColorButton_Click(object sender, EventArgs e)
        {
            System.Drawing.Color c = UtilityMethods.SelectColorFromDialog(this, System.Drawing.Color.FromArgb(localStarData.StarColor.A, localStarData.StarColor.R, localStarData.StarColor.G, localStarData.StarColor.B));
            if (c != System.Drawing.Color.Empty)
            {
                AmbientLightColorButton.BackColor = c;

                if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
                {
                    // find the ambient light
                    foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                    {
                        if (child is ModelVisual3D m3d && m3d.Content is AmbientLight al && al.GetName() == "AmbientLight")
                        {
                            al.Color = ambientLightColor;
                            break;
                        }
                    }
                }
            }
        }

        private void LoadWorldTextureButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Open 3D View Background Image",
                    DefaultExt = "png",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false,
                    Filter = UtilityMethods.GetCommonImageFilter()
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            SphereVisual3D worldGlobe = new()
                            {
                                Center = new Point3D(0, 0, 0),
                                Radius = 1,
                                ThetaDiv = 180,
                                PhiDiv = 90
                            };

                            worldGlobe.SetName("WorldGlobe");

                            BitmapImage b = new();
                            b.BeginInit();
                            b.UriSource = new Uri(ofd.FileName, UriKind.Absolute);
                            b.EndInit();

                            worldGlobe.Material = new DiffuseMaterial(new ImageBrush(b));

                            ThreeDViewer.HelixTKViewport.Children.Clear();

                            ThreeDViewer.HelixTKViewport.Children.Add(worldGlobe);

                            if (EnableAmbientLightSwitch.Checked)
                            {
                                ShowAmbientLight();
                            }

                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                            throw;
                        }
                    }
                }
            }
            catch { }
        }

        #region Local Star Control Event Handlers

        private void ShowLocalStarSwitch_CheckedChanged()
        {
            if (ShowLocalStarSwitch.Checked)
            {
                ShowLocalStar();
            }
            else
            {
                RemoveLocalStar();
            }
        }

        private void LocalStarTextureCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LocalStarTextureCombo.SelectedIndex >= 0)
            {
                switch (LocalStarTextureCombo.SelectedIndex)
                {
                    case 0:
                        localStarData.StarImageType = LocalStarImageType.Sun;
                        break;
                    case 1:
                        localStarData.StarImageType = LocalStarImageType.Nebula;
                        break;
                    case 2:
                        localStarData.StarImageType = LocalStarImageType.GasGiant;
                        break;
                    case 3:
                        localStarData.StarImageType = LocalStarImageType.Corona;
                        break;
                    default:
                        localStarData.StarImageType = LocalStarImageType.Sun;
                        break;
                }

                ShowLocalStar();
            }
        }

        private void LocalStarSizeTrack_Scroll(object sender, EventArgs e)
        {
            localStarData.Radius = LocalStarSizeTrack.Value / 100.0;
            ShowLocalStar();
        }

        private void LocalStarLocationLRTrack_Scroll(object sender, EventArgs e)
        {
            // rotate the local star and the local star light
            // hangle and vangle are in degrees
            double hangle = LocalStarLocationLRTrack.Value;
            double vangle = LocalStarLocationUDTrack.Value;

            Quaternion combinedQuaternion = Quaternion.Multiply(new Quaternion(new Vector3D(0, 0, 1), hangle), new Quaternion(new Vector3D(1, 0, 0), vangle));

            // find the local star and local star light
            for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
            {
                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3d && sv3d.GetName() == "LocalStar")
                {
                    sv3d.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                    localStarData.LocationTransform = sv3d.Transform.Clone();
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcorona && sv3dcorona.GetName() == "LocalStarCorona")
                {
                    sv3dcorona.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcoronaoutline && sv3dcoronaoutline.GetName() == "LocalStarCoronaOutline")
                {
                    sv3dcoronaoutline.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is PointLight pl && pl.GetName() == "LocalStarLight")
                {
                    pl.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is ParticleSystem ps && ps.GetName() == "LocalStarParticleSystem")
                {
                    ps.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }
            }
        }

        private void LocalStarLocationUDTrack_Scroll(object sender, EventArgs e)
        {
            // rotate the local star and the local star light
            // hangle and vangle are in degrees
            double hangle = LocalStarLocationLRTrack.Value;
            double vangle = LocalStarLocationUDTrack.Value;

            Quaternion combinedQuaternion = Quaternion.Multiply(new Quaternion(new Vector3D(0, 0, 1), hangle), new Quaternion(new Vector3D(1, 0, 0), vangle));

            // find the local star and local star light
            for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
            {
                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3d && sv3d.GetName() == "LocalStar")
                {
                    sv3d.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                    localStarData.LocationTransform = sv3d.Transform.Clone();
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcorona && sv3dcorona.GetName() == "LocalStarCorona")
                {
                    sv3dcorona.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcoronaoutline && sv3dcoronaoutline.GetName() == "LocalStarCoronaOutline")
                {
                    sv3dcoronaoutline.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is PointLight pl && pl.GetName() == "LocalStarLight")
                {
                    pl.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is ParticleSystem ps && ps.GetName() == "LocalStarParticleSystem")
                {
                    ps.Transform = new RotateTransform3D(new QuaternionRotation3D(combinedQuaternion));
                }
            }
        }

        private void LocalStarColorButton_Click(object sender, EventArgs e)
        {
            System.Drawing.Color c = UtilityMethods.SelectColorFromDialog(this, System.Drawing.Color.FromArgb(localStarData.StarColor.A, localStarData.StarColor.R, localStarData.StarColor.G, localStarData.StarColor.B));
            if (c != System.Drawing.Color.Empty)
            {
                LocalStarColorButton.BackColor = c;
                localStarData.StarColor = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);

                ShowLocalStar();
            }
        }

        private void LocalStarLightIntensityTrack_Scroll(object sender, EventArgs e)
        {
            localStarData.LightIntensity = LocalStarLightIntensityTrack.Value;
            ShowLocalStar();
        }

        #endregion

        #endregion

        #region World Globe Methods

        internal void ShowWorldGlobe(SKBitmap worldTexture)
        {
            // change to Z-up for the camera

            ThreeDViewer.ModelCamera.UpDirection = new Vector3D(0, 0, 1);

            SphereVisual3D worldGlobe = new()
            {
                Center = new Point3D(0, 0, 0),
                Radius = 1,
                ThetaDiv = 180,
                PhiDiv = 90
            };

            worldGlobe.SetName("WorldGlobe");

            // transform the world map texture to a BitmapImage;
            // converting the bitmap to a stream, then saving the stream to a BitmapImage
            // is really weird, but that's apparently the only way to do it without saving the bitmap to a file
            BitmapImage bImage = new();
            using MemoryStream ms = new();
            worldTexture.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bImage.BeginInit();
            bImage.StreamSource = new MemoryStream(ms.ToArray());
            bImage.EndInit();

            worldGlobe.Material = new DiffuseMaterial(new ImageBrush(bImage));

            ThreeDViewer.HelixTKViewport.Children.Clear();

            ThreeDViewer.HelixTKViewport.Children.Add(worldGlobe);

            if (EnableAmbientLightSwitch.Checked)
            {
                ShowAmbientLight();
            }

            ThreeDViewer.HelixTKViewport.ResetCamera();
            ThreeDViewer.ModelCamera.UpDirection = new Vector3D(0, 0, 1);
            ThreeDViewer.ModelCamera.LookDirection = new Vector3D(1, 0, 0);
            ThreeDViewer.ModelCamera.Position = new Point3D(-6, 0, 0);
        }

        private void RemoveCloudLayer()
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find and remove the cloud layer
                for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3d && sv3d.GetName() == "CloudLayer")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void ShowCloudLayer()
        {
            RemoveCloudLayer();

            SphereVisual3D cloudLayer = new()
            {
                Center = new Point3D(0, 0, 0),
                Radius = 1.05,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent))
            };

            cloudLayer.SetName("CloudLayer");

            System.Windows.Media.Color materialColor = System.Windows.Media.Color.FromArgb((byte)cloudTextureOpacity, cloudColor.R, cloudColor.G, cloudColor.B);

            if (DefaultCloudTextureRadio.Checked)
            {
                BitmapImage cloudImage = new();
                using MemoryStream ms = new();
                ms.Write(Resources.cloud_combined_2048, 0, Resources.cloud_combined_2048.Length);
                cloudImage.BeginInit();
                cloudImage.StreamSource = new MemoryStream(ms.ToArray());
                cloudImage.EndInit();

                cloudLayer.Material = new EmissiveMaterial(new ImageBrush(cloudImage))
                {
                    Color = materialColor
                };
            }
            else
            {
                if (!string.IsNullOrEmpty(CloudTextureFileName))
                {
                    BitmapImage cloudImage = new();
                    cloudImage.BeginInit();
                    cloudImage.UriSource = new Uri(CloudTextureFileName, UriKind.Absolute);
                    cloudImage.EndInit();
                    cloudLayer.Material = new EmissiveMaterial(new ImageBrush(cloudImage))
                    {
                        Color = materialColor
                    };
                }
                else
                {
                    MessageBox.Show("No cloud texture file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            ThreeDViewer.HelixTKViewport.Children.Add(cloudLayer);
        }

        private void RemoveSunlight()
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the sunlight directional light
                for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is DirectionalLight dl && dl.GetName() == "Sunlight")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void ShowSunlight()
        {
            // directional light
            // -Y is left, -Z is up
            DirectionalLight directionalLight = new(Colors.White, new Vector3D(0, -3, 0));
            directionalLight.SetName("Sunlight");
            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = directionalLight
            });
        }

        private void RemoveAmbientLight()
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the ambient light
                for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is AmbientLight al && al.GetName() == "AmbientLight")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void ShowAmbientLight()
        {
            AmbientLight ambient = new(ambientLightColor);
            ambient.SetName("AmbientLight");

            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = ambient
            });
        }

        private void ShowLocalStar()
        {
            RemoveLocalStar();

            // change to Z-up for the camera

            ThreeDViewer.ModelCamera.UpDirection = new Vector3D(0, 0, 1);

            switch (localStarData.StarImageType)
            {
                case LocalStarImageType.Sun:
                    {
                        CreateSunLocalStar();
                    }
                    break;
                case LocalStarImageType.Nebula:
                    {
                        CreateNebulaLocalStar();
                    }
                    break;
                case LocalStarImageType.GasGiant:
                    {
                        CreateGasGiantLocalStar();
                    }
                    break;
                case LocalStarImageType.Corona:
                    {
                        CreateCoronaLocalStar();
                    }
                    break;
                case LocalStarImageType.BlackHole:
                    {
                        CreateBlackHoleLocalStar();
                    }
                    break;
                default:
                    {
                        CreateSunLocalStar();
                    }
                    break;
            }
        }

        private void CreateBlackHoleLocalStar()
        {
            // BLACK HOLE -----------------------------------------------------------------------
            // create a sphere to represent the black hole
            SphereVisual3D localStar = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Black))
            };

            localStar.SetName("LocalStar");

            System.Windows.Media.Color materialColor = Colors.Black;

            localStar.Material = new EmissiveMaterial(new SolidColorBrush(Colors.Black))
            {
                Color = materialColor
            };

            // BLACK HOLE ACCRETION DISK ---------------------------------------------------------------------
        }

        private void CreateCoronaLocalStar()
        {
            double zoomPercentage = (100.0 + LocalStarSizeTrack.Value - 15.0) / 100.0;

            // STAR -----------------------------------------------------------------------
            // create a sphere to represent the local star
            SphereVisual3D localStar = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0)))
            };

            localStar.SetName("LocalStar");

            System.Windows.Media.Color materialColor = Colors.Black;

            localStar.Material = new EmissiveMaterial(new SolidColorBrush(Colors.Black))
            {
                Color = materialColor
            };

            // STAR CORONA OUTLINE ---------------------------------------------------------------------

            SphereVisual3D localStarCoronaOutline = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.03,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new SolidColorBrush(Colors.Black)),
            };

            localStarCoronaOutline.Material = new EmissiveMaterial()
            {
                Color = localStarData.StarColor,
                Brush = new RadialGradientBrush()
                {
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(Colors.Transparent, 0),
                        new GradientStop(Colors.Transparent, 0.5),
                        new GradientStop(localStarData.StarColor, 1),
                    },
                }
            };

            localStarCoronaOutline.SetName("LocalStarCoronaOutline");

            // STAR CORONA ---------------------------------------------------------------------

            SphereVisual3D localStarCorona = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.2,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent))
                {
                    Color = materialColor
                }
            };

            localStarCorona.SetName("LocalStarCorona");

            System.Windows.Media.Color coronaMaterialColor = System.Windows.Media.Color.FromArgb(80, localStarData.StarColor.R, localStarData.StarColor.G, localStarData.StarColor.B);

            localStarCorona.Material = new EmissiveMaterial()
            {
                Color = coronaMaterialColor,
                Brush = new RadialGradientBrush()
                {
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(System.Windows.Media.Colors.Transparent, 0),
                        new GradientStop(Colors.Transparent, 0.5),
                        new GradientStop(coronaMaterialColor, 1),
                    },
                }
            };

            byte[] smokeResource = Resources.white_particle;

            BitmapImage smokeImage = new();
            using MemoryStream sms = new();
            sms.Write(smokeResource, 0, smokeResource.Length);
            smokeImage.BeginInit();
            smokeImage.StreamSource = new MemoryStream(sms.ToArray());
            smokeImage.EndInit();


            ImageBrush smokeImageBrush = new ImageBrush(smokeImage)
            {
                Opacity = 0.25
            };


            // STAR PARTICLE SYSTEM -------------------------------------------------------------

            ParticleSystem particleSystem = new()
            {
                Position = localStarData.Center,
                Texture = smokeImageBrush,
                EmitRate = 10,
                LifeTime = 5,
                StartDirection = new Vector3D(1, 0, 0),
                StartRadius = localStarCoronaOutline.Radius,
                StartSize = 0.22 * zoomPercentage,
                StartVelocity = 0.001,
                StartVelocityRandomness = 0.001,
                SizeRate = 0.025,
                StartSpreading = 90,
                FadeOutTime = 3,
                //VelocityDamping = 0.999,
                Acceleration = 0,
                AliveParticles = 100,
            };

            particleSystem.SetName("LocalStarParticleSystem");

            // STAR LIGHT ---------------------------------------------------------------------

            PointLight localStarlight = new(localStarData.StarColor, localStarData.Center)
            {
                Range = localStarData.LightIntensity,
            };

            localStarlight.SetName("LocalStarLight");

            // rotate the local star, star corona, and local star light
            if (localStarData.LocationTransform != null)
            {
                localStar.Transform = localStarData.LocationTransform.Clone();
                localStarCorona.Transform = localStarData.LocationTransform.Clone();
                localStarCoronaOutline.Transform = localStarData.LocationTransform.Clone();
                localStarlight.Transform = localStarData.LocationTransform.Clone();
                particleSystem.Transform = localStarData.LocationTransform.Clone();
            }


            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = localStarlight
            });

            ThreeDViewer.HelixTKViewport.Children.Add(localStarCoronaOutline);
            ThreeDViewer.HelixTKViewport.Children.Add(localStar);

            ThreeDViewer.HelixTKViewport.Children.Add(localStarCorona);
            ThreeDViewer.HelixTKViewport.Children.Add(particleSystem);
        }

        private void CreateGasGiantLocalStar()
        {
            // STAR -----------------------------------------------------------------------
            // create a sphere to represent the local star
            SphereVisual3D localStar = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(192, 64, 64, 64)))
            };

            localStar.SetName("LocalStar");

            System.Windows.Media.Color materialColor = localStarData.StarColor;

            byte[] starImageResource = Resources.gas_giant;

            BitmapImage starImage = new();
            using MemoryStream ms = new();
            ms.Write(starImageResource, 0, starImageResource.Length);
            starImage.BeginInit();
            starImage.StreamSource = new MemoryStream(ms.ToArray());
            starImage.EndInit();

            localStar.Material = new EmissiveMaterial(new ImageBrush(starImage))
            {
                Color = materialColor
            };

            // STAR CORONA ---------------------------------------------------------------------

            SphereVisual3D localStarCorona = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.1,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent))
                {
                    Color = materialColor
                }
            };

            localStarCorona.SetName("LocalStarCorona");

            System.Windows.Media.Color coronaMaterialColor = System.Windows.Media.Color.FromArgb(64, localStarData.StarColor.R, localStarData.StarColor.G, localStarData.StarColor.B);

            localStarCorona.Material = new EmissiveMaterial()
            {
                Color = coronaMaterialColor,
                Brush = new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent)
                {
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(coronaMaterialColor, 0),
                        new GradientStop(System.Windows.Media.Colors.Transparent, 1)
                    },
                }
            };

            // STAR LIGHT ---------------------------------------------------------------------

            PointLight localStarlight = new(localStarData.StarColor, localStarData.Center)
            {
                Range = localStarData.LightIntensity,
            };

            localStarlight.SetName("LocalStarLight");

            // rotate the local star, star corona, and local star light
            if (localStarData.LocationTransform != null)
            {
                localStar.Transform = localStarData.LocationTransform.Clone();
                localStarCorona.Transform = localStarData.LocationTransform.Clone();

                localStarlight.Transform = localStarData.LocationTransform.Clone();
            }

            ThreeDViewer.HelixTKViewport.Children.Add(localStar);
            ThreeDViewer.HelixTKViewport.Children.Add(localStarCorona);
            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = localStarlight
            });
        }

        private void CreateNebulaLocalStar()
        {
            // STAR -----------------------------------------------------------------------
            // create a sphere to represent the local star
            SphereVisual3D localStar = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(192, 64, 64, 64)))
            };

            localStar.SetName("LocalStar");

            System.Windows.Media.Color materialColor = localStarData.StarColor;

            byte[] starImageResource = Resources.nebula;

            BitmapImage starImage = new();
            using MemoryStream ms = new();
            ms.Write(starImageResource, 0, starImageResource.Length);
            starImage.BeginInit();
            starImage.StreamSource = new MemoryStream(ms.ToArray());
            starImage.EndInit();

            localStar.Material = new EmissiveMaterial(new ImageBrush(starImage))
            {
                Color = materialColor
            };

            // STAR CORONA ---------------------------------------------------------------------

            SphereVisual3D localStarCorona = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.1,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent))
                {
                    Color = materialColor
                }
            };

            localStarCorona.SetName("LocalStarCorona");

            System.Windows.Media.Color coronaMaterialColor = System.Windows.Media.Color.FromArgb(64, localStarData.StarColor.R, localStarData.StarColor.G, localStarData.StarColor.B);

            localStarCorona.Material = new EmissiveMaterial()
            {
                Color = coronaMaterialColor,
                Brush = new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent)
                {
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(coronaMaterialColor, 0),
                        new GradientStop(System.Windows.Media.Colors.Transparent, 1)
                    },
                }
            };

            // STAR LIGHT ---------------------------------------------------------------------

            PointLight localStarlight = new(localStarData.StarColor, localStarData.Center)
            {
                Range = localStarData.LightIntensity,
            };

            localStarlight.SetName("LocalStarLight");

            // rotate the local star, star corona, and local star light
            if (localStarData.LocationTransform != null)
            {
                localStar.Transform = localStarData.LocationTransform.Clone();
                localStarCorona.Transform = localStarData.LocationTransform.Clone();

                localStarlight.Transform = localStarData.LocationTransform.Clone();
            }

            ThreeDViewer.HelixTKViewport.Children.Add(localStar);
            ThreeDViewer.HelixTKViewport.Children.Add(localStarCorona);
            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = localStarlight
            });
        }

        private void CreateSunLocalStar()
        {
            // STAR -----------------------------------------------------------------------
            // create a sphere to represent the local star
            SphereVisual3D localStar = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(160, 64, 64, 64)))
            };

            localStar.SetName("LocalStar");

            System.Windows.Media.Color materialColor = localStarData.StarColor;

            byte[] starImageResource = Resources.sun_texture;

            BitmapImage starImage = new();
            using MemoryStream ms = new();
            ms.Write(starImageResource, 0, starImageResource.Length);
            starImage.BeginInit();
            starImage.StreamSource = new MemoryStream(ms.ToArray());
            starImage.EndInit();

            localStar.Material = new EmissiveMaterial(new ImageBrush(starImage))
            {
                Color = materialColor
            };

            // STAR CORONA ---------------------------------------------------------------------

            SphereVisual3D localStarCorona = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.1,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent))
                {
                    Color = materialColor
                }
            };

            localStarCorona.SetName("LocalStarCorona");

            System.Windows.Media.Color coronaMaterialColor = System.Windows.Media.Color.FromArgb(64, localStarData.StarColor.R, localStarData.StarColor.G, localStarData.StarColor.B);

            localStarCorona.Material = new EmissiveMaterial()
            {
                Color = coronaMaterialColor,
                Brush = new RadialGradientBrush(materialColor, System.Windows.Media.Colors.Transparent)
                {
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(coronaMaterialColor, 0),
                        new GradientStop(System.Windows.Media.Colors.Transparent, 1)
                    },
                }
            };

            // STAR LIGHT ---------------------------------------------------------------------

            PointLight localStarlight = new(localStarData.StarColor, localStarData.Center)
            {
                Range = localStarData.LightIntensity,
            };

            localStarlight.SetName("LocalStarLight");

            // rotate the local star, star corona, and local star light
            if (localStarData.LocationTransform != null)
            {
                localStar.Transform = localStarData.LocationTransform.Clone();
                localStarCorona.Transform = localStarData.LocationTransform.Clone();

                localStarlight.Transform = localStarData.LocationTransform.Clone();
            }

            ThreeDViewer.HelixTKViewport.Children.Add(localStar);
            ThreeDViewer.HelixTKViewport.Children.Add(localStarCorona);
            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = localStarlight
            });
        }

        private void RemoveLocalStar()
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find and remove the local star and all of the associated objects
                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3d && sv3d.GetName() == "LocalStar")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcorona && sv3dcorona.GetName() == "LocalStarCorona")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcoronaOutline
                        && sv3dcoronaOutline.GetName() == "LocalStarCoronaOutline")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is PointLight al && al.GetName() == "LocalStarLight")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }


                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is ParticleSystem ps && ps.GetName() == "LocalStarParticleSystem")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }
            }
        }

        #endregion

        #region AVI Recording

        private void RecordButton_Click(object sender, EventArgs e)
        {
            if (!recordingEnabled)
            {
                RecordAnimation();
            }
            else
            {
                StopAnimationRecording();
            }
        }

        private void StopAnimationRecording()
        {
            tokenSource.Cancel();
        }

        private async void RecordAnimation()
        {
            double revolutionsPerSecond = revolutionsPerMinute / 60.0;
            double degreesPerFrame = 360.0 / (framePerSecond / revolutionsPerSecond); // degrees per frame

            frameCount = 0;
            int frames = (int)(360.0 / degreesPerFrame);

            try
            {
                cancelToken = tokenSource.Token;

                RecordButton.BackColor = System.Drawing.Color.IndianRed;
                RecordButton.IconChar = FontAwesome.Sharp.IconChar.Stop;
                RecordButton.Text = "Stop";

                RecordButton.Refresh();

                // disable all the controls in the dialog except the record button
                LightingPanel.Visible = false;
                CloudsPanel.Visible = false;
                FeaturesPanel.Visible = false;

                LoadModelButton.Enabled = false;
                SaveModelButton.Enabled = false;
                ResetCameraButton.Enabled = false;
                ChangeAxesButton.Enabled = false;
                LoadSceneTextureButton.Enabled = false;
                ResetSceneButton.Enabled = false;
                ShowHideLightingPanelButton.Enabled = false;
                ShowHideCloudPanelButton.Enabled = false;
                ShowHideFeaturesButton.Enabled = false;
                ShowGridlinesCheck.Enabled = false;
                EnableAnimationSwitch.Enabled = false;
                RotationRateUpDown.Enabled = false;
                FrameRateCombo.Enabled = false;
                LoadWorldTextureButton.Enabled = false;
                CloseFormButton.Enabled = false;
                ThreeDViewControlBox.Enabled = false;

                SphereVisual3D? cloudLayer = null;

                if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
                {
                    // find the cloud layer
                    foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                    {
                        if (child is SphereVisual3D clouds && clouds.GetName() == "CloudLayer")
                        {
                            cloudLayer = clouds;
                            break;
                        }
                    }
                }

                SKBitmap? formattedBackground = null;

                if (sceneBackground != null)
                {
                    formattedBackground = DrawingMethods.ResizeSKBitmap(sceneBackground.ToSKBitmap(),
                        new SKSizeI((int)ThreeDViewer.HelixTKViewport.ActualWidth, (int)ThreeDViewer.HelixTKViewport.ActualHeight));
                }

                // initialize AVI recording

                aviTempFileName = Path.GetTempFileName();

                aviWriter = new AviWriter(aviTempFileName)
                {
                    FramesPerSecond = (decimal)framePerSecond,
                    EmitIndex1 = true,
                };

                // create the video stream
                videoStream = aviWriter.AddMJpegWpfVideoStream((int)ThreeDViewer.HelixTKViewport.ActualWidth,
                    (int)ThreeDViewer.HelixTKViewport.ActualHeight, 100);

                recordingEnabled = true;

                // record video in a separate thread so that the user can cancel the recording
                await RecordAnimationFramesAsync(cloudLayer, formattedBackground);

            }
            catch (OperationCanceledException)
            {
                // recording was cancelled;
                // capture and ignore the exception; it's handled in the finally block
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
                MessageBox.Show("Error recording video: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                tokenSource.Dispose();
                tokenSource = new CancellationTokenSource();

                // reset the record button
                RecordButton.BackColor = System.Drawing.Color.MediumSeaGreen;
                RecordButton.IconChar = FontAwesome.Sharp.IconChar.Play;
                RecordButton.Text = "Record";

                // re-enable all the controls in the dialog

                LoadModelButton.Enabled = true;
                SaveModelButton.Enabled = true;
                ResetCameraButton.Enabled = true;
                ChangeAxesButton.Enabled = true;
                LoadSceneTextureButton.Enabled = true;
                ResetSceneButton.Enabled = true;
                ShowHideLightingPanelButton.Enabled = true;
                ShowHideCloudPanelButton.Enabled = true;
                ShowHideFeaturesButton.Enabled = true;
                ShowGridlinesCheck.Enabled = true;
                EnableAnimationSwitch.Enabled = true;
                RotationRateUpDown.Enabled = true;
                FrameRateCombo.Enabled = true;
                LoadWorldTextureButton.Enabled = true;
                CloseFormButton.Enabled = true;
                ThreeDViewControlBox.Enabled = true;

                recordingEnabled = false;

                ModelStatisticsLabel.Text = "Video complete.";
                ModelStatisticsLabel.Refresh();

                RecordingProgressBar.Value = 0;
                RecordingProgressBar.Refresh();

                DialogResult result = DialogResult.OK;
                if (frameCount < frames)
                {
                    result = MessageBox.Show("The video recording was cancelled. Save anyway?", "Cancelled", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                }

                if (result == DialogResult.OK)
                {
                    // recording complete; save the video
                    frameCount = 0;
                    aviWriter?.Close();
                    aviWriter = null;

                    SaveFileDialog sfd = new()
                    {
                        DefaultExt = "avi",
                        CheckWriteAccess = true,
                        ExpandedMode = true,
                        AddExtension = true,
                        SupportMultiDottedExtensions = false,
                        AddToRecent = true,
                        Filter = "AVI file|*.avi",
                        Title = "Save World Globe Animation",
                    };

                    DialogResult sfdresult = sfd.ShowDialog();

                    if (sfdresult == DialogResult.OK)
                    {
                        File.Copy(aviTempFileName, sfd.FileName, true);
                        File.Delete(aviTempFileName);

                        ModelStatisticsLabel.Text = "Video file saved: " + Path.GetFileName(sfd.FileName);
                        ModelStatisticsLabel.Refresh();
                    }
                    else
                    {
                        File.Delete(aviTempFileName);
                    }
                }
            }
        }

        private async Task RecordAnimationFramesAsync(SphereVisual3D? cloudLayer, SKBitmap? background)
        {
            // this code allows the UI to refresh (and handle user interaction, like clicking the Stop button)
            // while video is being recorded
            // the technique was taken from: https://stackoverflow.com/questions/21592036/how-to-let-the-ui-refresh-during-a-long-running-ui-operation

            static async Task idleYield() => await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);

            double revolutionsPerSecond = revolutionsPerMinute / 60.0;
            double degreesPerFrame = 360.0 / (framePerSecond / revolutionsPerSecond); // degrees per frame  

            frameCount = 0;
            int frames = (int)(360.0 / degreesPerFrame);

            TaskCompletionSource<bool> cancellationTcs = new();

            using (cancelToken.Register(() =>
                cancellationTcs.SetCanceled(), useSynchronizationContext: true))
            {
                while (frameCount < frames)
                {
                    Cursor.Current = Cursors.Default;

                    await Task.WhenAny(idleYield(), cancellationTcs.Task);
                    cancelToken.ThrowIfCancellationRequested();

                    RecordAnimationFrame(cloudLayer, background);
                    frameCount++;

                    ModelStatisticsLabel.Text = $"Recording video: {frameCount} of {frames} frames.";
                    ModelStatisticsLabel.Refresh();

                    RecordingProgressBar.Value = (int)((double)frameCount / frames * 100.0);
                    RecordingProgressBar.Refresh();
                }
            }
        }

        private void RecordAnimationFrame(SphereVisual3D? cloudLayer, SKBitmap? background)
        {

            double revolutionsPerSecond = revolutionsPerMinute / 60.0;

            double degreesPerFrame = 360.0 / (framePerSecond / revolutionsPerSecond); // degrees per frame

            SphereVisual3D worldGlobe = (SphereVisual3D)ThreeDViewer.HelixTKViewport.Children[0];

            // rotate around Z axis
            Vector3D axis = new(0, 0, 1);

            // Get the matrix indicating the current transformation value
            Matrix3D transformationMatrix = worldGlobe.Content.Transform.Value;

            // rotate around the world globe center Z axis
            transformationMatrix.RotateAt(new Quaternion(axis, degreesPerFrame), worldGlobe.Center);

            // do the rotation transform
            worldGlobe.Content.Transform = new MatrixTransform3D(transformationMatrix);

            // rotate the cloud layer, if there is one
            if (cloudLayer != null)
            {
                // Get the matrix indicating the current transformation value
                Matrix3D cloudTransformationMatrix = cloudLayer.Content.Transform.Value;

                // rotate around the cloud layer center Z axis
                cloudTransformationMatrix.RotateAt(new Quaternion(axis, degreesPerFrame * cloudRotationRate), cloudLayer.Center);

                // do the rotation transform
                cloudLayer.Content.Transform = new MatrixTransform3D(cloudTransformationMatrix);
            }

            if (aviWriter != null && videoStream != null)
            {
                // NOTE: when the viewport has a background (like a starfield), using Viewport3D RenderBitmap method
                // to create the video frames results in a lot of weird visual artifacts in the video;
                // if a background is not included in the video, the video looks fine.
                // So, I am using RenderTargetBitmap to take a snapshot of the 3D view,
                // which does not include the background, then compositing the background into the
                // video frame using a Skia canvas

                // take a snapshot of the 3D view
                using SKBitmap frameBitmap = new((int)ThreeDViewer.HelixTKViewport.ActualWidth, (int)ThreeDViewer.HelixTKViewport.ActualHeight);
                using SKCanvas canvas = new(frameBitmap);

                RenderTargetBitmap rtb = new(
                    (int)ThreeDViewer.HelixTKViewport.Viewport.ActualWidth,
                    (int)ThreeDViewer.HelixTKViewport.Viewport.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                rtb.Render(ThreeDViewer.HelixTKViewport.Viewport);

                using Bitmap? b = DrawingMethods.BitmapSourceToBitmap(rtb);

                if (b != null)
                {
                    if (background != null)
                    {
                        // composite the background onto the frameBitmap
                        canvas.DrawBitmap(background, 0, 0);
                    }

                    canvas.DrawBitmap(b.ToSKBitmap(), 0, 0);

                    byte[] bitmapData = DrawingMethods.BitmapToByteArray(b);

                    // write the video frame to the file
                    videoStream.WriteFrame(true, frameBitmap.Bytes, 0, bitmapData.Length);
                }
            }
        }

        #endregion


    }

    #region Data Structures

    internal class LocalStar
    {
        internal Point3D Center { get; set; } = new Point3D(3, 3, 0);
        internal double Radius { get; set; } = 0.15;
        internal LocalStarImageType StarImageType { get; set; } = LocalStarImageType.Sun;
        internal System.Windows.Media.Color StarColor { get; set; } = System.Windows.Media.Colors.Yellow;
        internal int LightIntensity { get; set; } = 10;
        internal Transform3D? LocationTransform { get; set; }
    }

    #endregion
}
