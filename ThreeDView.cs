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
using FontAwesome.Sharp;
using HelixToolkit.Wpf;
using RealmStudio.Properties;
using SharpAvi.Codecs;
using SharpAvi.Output;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace RealmStudio
{
    /***
     * This form hosts the ThreeDViwerControl WPF UserControl that displays a 3D model.
     */
    public partial class ThreeDView : Form
    {
        private readonly ThreeDViewerControl ThreeDViewer;

        private readonly double initialCameraDistance;

        private readonly List<string>? ModelUpdateQueue;
        private string CloudTextureFileName = string.Empty;

        private string? LoadedModelString;

        private System.Timers.Timer? _animationTimer;
        private bool animationTimerEnabled;

        private double framePerSecond = 60.0;       // frames per second
        private double revolutionsPerMinute = 1.0;  // revolutions/minute

        private int cloudTextureOpacity = 64; // 0-255
        private double cloudRotationRate = 1.0; // percentage of world rotation rate

        private System.Windows.Media.Color cloudColor = Colors.White;
        private System.Windows.Media.Color sunlightColor = Colors.White;
        private System.Windows.Media.Color ambientLightColor = Colors.White;

        private double sunlightIntensity = 0.5; // 0-1
        private double ambientLightIntensity = 0.75; // 0-1

        private bool recordingEnabled;
        private int frameCount;

        private CancellationTokenSource tokenSource = new();
        private CancellationToken cancelToken;

        private AviWriter? aviWriter;
        private string aviTempFileName = string.Empty;
        private IAviVideoStream? videoStream;
        private Bitmap? sceneBackground;

        private readonly LocalStar localStarData = new();
        private readonly PlanetaryRing planetaryRingData = new();
        private readonly PlanetAtmosphere atmosphereData = new();

        private readonly List<Moon> moons = [];
        private Moon? selectedMoon;

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

            var camera = ThreeDViewer.HelixTKViewport.Camera;
            if (camera != null)
            {
                initialCameraDistance = camera.Position.ToVector3().Length();
            }

            LocalStarTextureCombo.SelectedIndex = 0;
            RingTextureCombo.SelectedIndex = 0;
            FrameRateCombo.SelectedIndex = 4;
            MoonTextureCombo.SelectedIndex = 5;
        }

        public ThreeDView(string formTitle, List<string> modelUpdateQueue)
        {
            InitializeComponent();
            ThreeDViewOverlay.Text = formTitle;
            ModelUpdateQueue = modelUpdateQueue;

            LoadModelButton.Enabled = false;

            // construct the WPF ThreeDViewerControl UserControl
            ThreeDViewer = new();

            var camera = ThreeDViewer.HelixTKViewport.Camera;
            if (camera != null)
            {
                initialCameraDistance = camera.Position.ToVector3().Length();
            }

            LocalStarTextureCombo.SelectedIndex = 0;
            RingTextureCombo.SelectedIndex = 0;
            FrameRateCombo.SelectedIndex = 4;
            MoonTextureCombo.SelectedIndex = 5;
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

            ThreeDViewer.HelixTKViewport.PanGesture = default!;
            ThreeDViewer.HelixTKViewport.PanGesture2 = new System.Windows.Input.MouseGesture(System.Windows.Input.MouseAction.LeftClick);

            ThreeDViewer.HelixTKViewport.RotateGesture = default!;
            ThreeDViewer.HelixTKViewport.RotateGesture2 = default!;
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
            var camera = ThreeDViewer.HelixTKViewport.Camera;
            if (camera == null || initialCameraDistance == 0)
                return 100;

            double currentDistance = camera.Position.ToVector3().Length();
            double zoomPercent = (initialCameraDistance / currentDistance) * 100;

            return zoomPercent;
        }

        #endregion


        #region Animation

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
            StopAnimationTimer();
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
            if (ThreeDViewer.HelixTKViewport.Camera != null)
            {
                // fit to the view, but don't change current direction
                ThreeDViewer.HelixTKViewport.FitView(ThreeDViewer.HelixTKViewport.Camera.LookDirection, ThreeDViewer.HelixTKViewport.Camera.UpDirection);
            }
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
            ThreeDViewer.HelixTKViewport.ShowViewCube = false;
            ThreeDViewer.HelixTKViewport.ShowCoordinateSystem = false;

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
                (int)ThreeDViewer.ThreeDViewGrid.ActualWidth,
                (int)ThreeDViewer.ThreeDViewGrid.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            rtb.Render(ThreeDViewer.ThreeDViewGrid);

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

            ThreeDViewer.HelixTKViewport.ShowViewCube = true;
            ThreeDViewer.HelixTKViewport.ShowCoordinateSystem = true;
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

        private void ThreeDViewer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.GetPosition((UIElement)sender);

            // Perform the hit test against a given portion of the visual object tree.
            HitTestResult result = VisualTreeHelper.HitTest(ThreeDViewer.HelixTKViewport, pt);

            if (result != null)
            {
                // Perform action on hit visual object.
                if (!string.IsNullOrEmpty(result.VisualHit.GetName()))
                {
                    if (result.VisualHit.GetName().StartsWith("Moon"))
                    {
                        // select the moon
                        selectedMoon = moons.FirstOrDefault(m => m.MoonName == result.VisualHit.GetName());
                        if (selectedMoon != null)
                        {
                            // update the UI with the selected moon data
                            MoonDistanceTrack.Value = selectedMoon.MoonDistance;
                            MoonSizeTrack.Value = (int)(selectedMoon.MoonRadius * 100.0f);
                            MoonOrbitTrack.Value = (int)selectedMoon.MoonOrbitRotation;
                            MoonPlaneTrack.Value = (int)selectedMoon.MoonPlaneRotation;
                            MoonRotationTrack.Value = (int)selectedMoon.MoonAxisRotation;
                            SelectMoonTintButton.BackColor = System.Drawing.Color.FromArgb(selectedMoon.MoonColor.A, selectedMoon.MoonColor.R, selectedMoon.MoonColor.G, selectedMoon.MoonColor.B);
                        }
                    }

                }
            }
        }

        #region Cloud Layer Event Handlers

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

        #endregion

        #region Lighting

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
                            dl.Color = System.Windows.Media.Color.FromArgb((byte)(sunlightIntensity * 255),
                                (byte)Math.Min(255, sunlightColor.R * sunlightIntensity),
                                (byte)Math.Min(255, sunlightColor.G * sunlightIntensity),
                                (byte)Math.Min(255, sunlightColor.B * sunlightIntensity));
                            break;
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
            System.Drawing.Color c = UtilityMethods.SelectColorFromDialog(
                this, System.Drawing.Color.FromArgb(localStarData.StarColor.A, localStarData.StarColor.R,
                localStarData.StarColor.G, localStarData.StarColor.B));

            if (c != System.Drawing.Color.Empty)
            {
                AmbientLightColorButton.BackColor = c;
                ambientLightColor = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);

                if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
                {
                    // find the ambient light
                    foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                    {
                        if (child is ModelVisual3D m3d && m3d.Content is AmbientLight al && al.GetName() == "AmbientLight")
                        {
                            al.Color = System.Windows.Media.Color.FromArgb((byte)(ambientLightIntensity * 255),
                                (byte)Math.Min(255, ambientLightColor.R * ambientLightIntensity),
                                (byte)Math.Min(255, ambientLightColor.G * ambientLightIntensity),
                                (byte)Math.Min(255, ambientLightColor.B * ambientLightIntensity));

                            break;
                        }
                    }
                }
            }
        }

        private void SunlightIntensityTrack_Scroll(object sender, EventArgs e)
        {
            // sunlight intensity is just the color of the light
            // the close to white, the more "intense" the light

            sunlightIntensity = SunlightIntensityTrack.Value / 100.0;

            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the sun light
                foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                {
                    if (child is ModelVisual3D m3d && m3d.Content is DirectionalLight dl && dl.GetName() == "Sunlight")
                    {
                        dl.Color = System.Windows.Media.Color.FromArgb((byte)(sunlightIntensity * 255),
                            (byte)Math.Min(255, sunlightColor.R * sunlightIntensity),
                            (byte)Math.Min(255, sunlightColor.G * sunlightIntensity),
                            (byte)Math.Min(255, sunlightColor.B * sunlightIntensity));
                        break;
                    }
                }
            }
        }

        private void AmbientLightIntensityTrack_Scroll(object sender, EventArgs e)
        {
            // ambient light intensity is just the color of the light
            // the close to white, the more "intense" the light

            ambientLightIntensity = AmbientLightIntensityTrack.Value / 100.0;

            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                // find the ambient light
                foreach (var child in ThreeDViewer.HelixTKViewport.Children)
                {
                    if (child is ModelVisual3D m3d && m3d.Content is AmbientLight al && al.GetName() == "AmbientLight")
                    {
                        al.Color = System.Windows.Media.Color.FromArgb((byte)(ambientLightIntensity * 255),
                            (byte)Math.Min(255, ambientLightColor.R * ambientLightIntensity),
                            (byte)Math.Min(255, ambientLightColor.G * ambientLightIntensity),
                            (byte)Math.Min(255, ambientLightColor.B * ambientLightIntensity));
                        break;
                    }
                }
            }
        }

        #endregion

        #region World Texture

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

                            ApplyEnabledEffects();

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

        #endregion

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
                localStarData.StarImageType = LocalStarTextureCombo.SelectedIndex switch
                {
                    0 => LocalStarImageType.Sun,
                    1 => LocalStarImageType.Nebula,
                    2 => LocalStarImageType.GasGiant,
                    3 => LocalStarImageType.Corona,
                    4 => LocalStarImageType.BlackHole,
                    _ => LocalStarImageType.Sun,
                };
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
            RotateLocalStarObjects();
        }

        private void LocalStarLocationUDTrack_Scroll(object sender, EventArgs e)
        {
            RotateLocalStarObjects();
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

        #region Planetary Ring Event Handlers

        private void ShowRingsSwitch_CheckedChanged()
        {
            if (ShowRingsSwitch.Checked)
            {
                ShowPlanetaryRing();
            }
            else
            {
                RemovePlanetaryRing();
            }
        }

        private void RingTextureCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (RingTextureCombo.SelectedIndex)
            {
                case 0:
                    {
                        planetaryRingData.RingImageResource = Resources.saturn_ring;
                    }
                    break;
                case 1:
                    {
                        planetaryRingData.RingImageResource = Resources.green_rings;
                    }
                    break;
                case 2:
                    {
                        planetaryRingData.RingImageResource = Resources.tan_rings;
                    }
                    break;
                case 3:
                    {
                        planetaryRingData.RingImageResource = Resources.light_tan_rings;
                    }
                    break;
                case 4:
                    {
                        planetaryRingData.RingImageResource = Resources.blue_rings;
                    }
                    break;
                case 5:
                    {
                        planetaryRingData.RingImageResource = Resources.fiona_rings;
                    }
                    break;
                default:
                    {
                        planetaryRingData.RingImageResource = Resources.saturn_ring;
                    }
                    break;

            }

            ShowPlanetaryRing();
        }

        private void InnerRadiusTrack_Scroll(object sender, EventArgs e)
        {
            planetaryRingData.InnerRadius = InnerRadiusTrack.Value / 10.0;
            ShowPlanetaryRing();
        }

        private void OuterRadiusTrack_Scroll(object sender, EventArgs e)
        {
            planetaryRingData.OuterRadius = OuterRadiusTrack.Value / 10.0;
            ShowPlanetaryRing();
        }

        private void RingAngleTrack_Scroll(object sender, EventArgs e)
        {
            planetaryRingData.RingTiltAngle = 90.0 - RingAngleTrack.Value;
            ShowPlanetaryRing();
        }

        private void RingTintColorButton_Click(object sender, EventArgs e)
        {
            planetaryRingData.RingColor = UtilityMethods.SelectColorFromDialog(this, planetaryRingData.RingColor);
            RingTintColorButton.BackColor = planetaryRingData.RingColor;
            ShowPlanetaryRing();
        }

        private void RingOpacityTrack_Scroll(object sender, EventArgs e)
        {
            planetaryRingData.RingBrushOpacity = RingOpacityTrack.Value / 100.0;
            ShowPlanetaryRing();
        }

        #endregion

        #region Planet Atmosphere Event Handlers
        private void ShowAtmosphereSwitch_CheckedChanged()
        {
            if (ShowAtmosphereSwitch.Checked)
            {
                ShowAtmosphere();
            }
            else
            {
                RemoveAtmosphere();
            }
        }

        private void SelectAtmosphereColorButton_Click(object sender, EventArgs e)
        {
            atmosphereData.AtmosphereColor = UtilityMethods.SelectColorFromDialog(this, atmosphereData.AtmosphereColor);
            SelectAtmosphereColorButton.BackColor = atmosphereData.AtmosphereColor;
            ShowAtmosphere();
        }

        private void AtmosphereAltitudeTrack_Scroll(object sender, EventArgs e)
        {
            atmosphereData.AtmosphereRadius = AtmosphereAltitudeTrack.Value / 100.0;
            ShowAtmosphere();
        }

        private void AtmosphereDensityTrack_Scroll(object sender, EventArgs e)
        {
            atmosphereData.AtmosphereOpacity = AtmosphereDensityTrack.Value / 100.0;
            ShowAtmosphere();
        }

        #endregion

        #region Scene Effect Event Handlers

        private void EnableBlurSwitch_CheckedChanged()
        {
            ApplyEnabledEffects();
        }

        private void BlurAmountTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void EnableBleachBypassSwitch_CheckedChanged()
        {
            ApplyEnabledEffects();
        }

        private void BleachBypassOpacityTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void EnableBloomSwitch_CheckedChanged()
        {
            ApplyEnabledEffects();
        }

        private void SceneIntensityTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void GlowIntensityTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void ThresholdValueTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void BlurWidthTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void EnableSepiaSwitch_CheckedChanged()
        {
            ApplyEnabledEffects();
        }

        private void SepiaAmountTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        private void EnableGrayscaleSwitch_CheckedChanged()
        {
            ApplyEnabledEffects();
        }

        private void DesaturationFactorTrack_Scroll(object sender, EventArgs e)
        {
            ApplyEnabledEffects();
        }

        #endregion

        #region Moon Event Handlers

        private void MoonDistanceTrack_Scroll(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            selectedMoon.MoonDistance = MoonDistanceTrack.Value;
            float distance = (selectedMoon.MoonDistance / 100.0f) - 2.5f;

            selectedMoon.MoonCenter = new Point3D(2, 2, 0) + new Vector3D(distance, distance, 0);

            UpdateSelectedMoon();
        }

        private void MoonSizeTrack_Scroll(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            double moonRadius = MoonSizeTrack.Value / 100.0;
            selectedMoon.MoonRadius = moonRadius;

            UpdateSelectedMoon();
        }

        private void MoonOrbitTrack_Scroll(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            selectedMoon.MoonOrbitRotation = MoonOrbitTrack.Value;
            UpdateSelectedMoon();
        }

        private void MoonPlaneTrack_Scroll(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            selectedMoon.MoonPlaneRotation = MoonPlaneTrack.Value;
            UpdateSelectedMoon();
        }

        private void MoonRotationTrack_Scroll(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            selectedMoon.MoonAxisRotation = MoonRotationTrack.Value;
            UpdateSelectedMoon();
        }

        private void SelectMoonTintButton_Click(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            selectedMoon.MoonColor = UtilityMethods.SelectMediaColorFromDialog(this, selectedMoon.MoonColor);
            SelectMoonTintButton.BackColor = System.Drawing.Color.FromArgb(selectedMoon.MoonColor.A, selectedMoon.MoonColor.R, selectedMoon.MoonColor.G, selectedMoon.MoonColor.B);
            UpdateSelectedMoon();
        }

        private void MoonTextureCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            if (MoonTextureCombo.SelectedIndex > -1)
            {
                string? textureName = (string?)MoonTextureCombo.Items[MoonTextureCombo.SelectedIndex];

                if (!string.IsNullOrEmpty(textureName))
                {
                    selectedMoon.MoonTextureName = textureName;
                }
                else
                {
                    selectedMoon.MoonTextureName = "Moon";
                }
            }

            UpdateSelectedMoon();
        }

        private void LoadMoonTextureButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Open Moon Texture",
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
                            if (selectedMoon != null && File.Exists(ofd.FileName))
                            {
                                BitmapImage moonImage = new();
                                moonImage.BeginInit();
                                moonImage.UriSource = new Uri(ofd.FileName, UriKind.RelativeOrAbsolute);
                                moonImage.EndInit();

                                selectedMoon.MoonTexturePath = ofd.FileName;
                                selectedMoon.MoonTextureName = "Custom Moon Texture";

                                System.Windows.Media.Color materialColor = selectedMoon.MoonColor;

                                selectedMoon.moonVisual.Material = new EmissiveMaterial(new ImageBrush(moonImage))
                                {
                                    Color = materialColor,
                                };

                                UpdateSelectedMoon();
                            }

                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                        }
                    }
                }
            }
            catch { }
        }

        private void CreateMoonButton_Click(object sender, EventArgs e)
        {
            int moonCount = moons.Count;

            Moon m = new()
            {
                MoonName = "Moon " + moonCount + 1,
                MoonRadius = MoonSizeTrack.Value / 100.0,
            };

            SphereVisual3D moon = new()
            {
                Center = m.MoonCenter,
                Radius = m.MoonRadius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 48, 48, 48)))
            };

            moon.SetName(m.MoonName);

            m.moonVisual = moon;

            selectedMoon = m;

            ThreeDViewer.HelixTKViewport.Children.Add(moon);
            moons.Add(m);

            UpdateSelectedMoon();
        }

        private void DeleteMoonButton_Click(object sender, EventArgs e)
        {
            if (selectedMoon == null)
            {
                return;
            }

            // remove the selected moon from the list of moons and from the viewport
            for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
            {
                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3d && sv3d.GetName() == selectedMoon.MoonName)
                {
                    ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    break;
                }
            }

            moons.Remove(selectedMoon);
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
            ThreeDViewer.MouseLeftButtonDown += ThreeDViewer_MouseLeftButtonDown;

            if (EnableAmbientLightSwitch.Checked)
            {
                ShowAmbientLight();
            }

            if (EnableSunlightSwitch.Checked)
            {
                ShowSunlight();
            }

            ThreeDViewer.HelixTKViewport.ResetCamera();
            ThreeDViewer.ModelCamera.UpDirection = new Vector3D(0, 0, 1);
            ThreeDViewer.ModelCamera.LookDirection = new Vector3D(1, 0, 0);
            ThreeDViewer.ModelCamera.Position = new Point3D(-6, 0, 0);
        }

        #region Cloud Layer Methods

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
                BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Transparent))
            };

            cloudLayer.SetName("CloudLayer");

            System.Windows.Media.Color materialColor = System.Windows.Media.Color.FromArgb((byte)cloudTextureOpacity, cloudColor.R, cloudColor.G, cloudColor.B);

            bool error = false;

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
                    error = true;
                    MessageBox.Show("No cloud texture file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!error)
            {
                ThreeDViewer.HelixTKViewport.Children.Add(cloudLayer);
            }
        }

        #endregion

        #region Sunlight Methods

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

            System.Windows.Media.Color sunColor = System.Windows.Media.Color.FromArgb((byte)(sunlightIntensity * 255),
                (byte)Math.Min(255, sunlightColor.R * sunlightIntensity),
                (byte)Math.Min(255, sunlightColor.G * sunlightIntensity),
                (byte)Math.Min(255, sunlightColor.B * sunlightIntensity));

            DirectionalLight directionalLight = new(sunColor, new Vector3D(0, -3, 0));
            directionalLight.SetName("Sunlight");
            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = directionalLight
            });
        }

        #endregion

        #region Ambient Light Methods

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
            System.Windows.Media.Color ambientColor = System.Windows.Media.Color.FromArgb((byte)(ambientLightIntensity * 255),
                (byte)Math.Min(255, ambientLightColor.R * ambientLightIntensity),
                (byte)Math.Min(255, ambientLightColor.G * ambientLightIntensity),
                (byte)Math.Min(255, ambientLightColor.B * ambientLightIntensity));

            AmbientLight ambient = new(ambientColor);
            ambient.SetName("AmbientLight");

            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = ambient
            });
        }

        #endregion

        #region Local Star Methods

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

        private void RotateLocalStarObjects()
        {
            // rotate the local star and the local star light
            // hangle and vangle are in degrees
            double hangle = LocalStarLocationLRTrack.Value;
            double vangle = LocalStarLocationUDTrack.Value;

            Quaternion hvQuaternion = Quaternion.Multiply(new Quaternion(new Vector3D(0, 0, 1), hangle), new Quaternion(new Vector3D(1, 0, 0), vangle));
            Quaternion combinedQuaternion = Quaternion.Multiply(hvQuaternion, localStarData.RotationQuaternion);

            RotateTransform3D rotateTransform = new(new QuaternionRotation3D(combinedQuaternion));
            //RotateTransform3D locationOnlyTransform = new(new QuaternionRotation3D(hvQuaternion));

            localStarData.LocationTransform = rotateTransform;

            // find the local star and local star light
            for (int i = 0; i < ThreeDViewer.HelixTKViewport.Children.Count; i++)
            {
                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3d && sv3d.GetName() == "LocalStar")
                {
                    sv3d.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcorona && sv3dcorona.GetName() == "LocalStarCorona")
                {
                    sv3dcorona.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dcoronaoutline && sv3dcoronaoutline.GetName() == "LocalStarCoronaOutline")
                {
                    sv3dcoronaoutline.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv && psv.GetName() == "AccretionDisk1")
                {
                    psv.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv2 && psv2.GetName() == "AccretionDisk2")
                {
                    psv2.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv3 && psv3.GetName() == "AccretionDisk3")
                {
                    psv3.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv4 && psv4.GetName() == "AccretionDisk4")
                {
                    psv4.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv5 && psv5.GetName() == "AccretionDisk5")
                {
                    psv5.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is ModelVisual3D m3d && m3d.Content is PointLight pl && pl.GetName() == "LocalStarLight")
                {
                    pl.Transform = localStarData.LocationTransform;
                }

                if (ThreeDViewer.HelixTKViewport.Children[i] is ParticleSystem ps && ps.GetName() == "LocalStarParticleSystem")
                {
                    ps.Transform = localStarData.LocationTransform;
                }
            }
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
                    if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv && psv.GetName() == "AccretionDisk1")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv && psv.GetName() == "AccretionDisk2")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv && psv.GetName() == "AccretionDisk3")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv && psv.GetName() == "AccretionDisk4")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is PieSliceVisual3D psv && psv.GetName() == "AccretionDisk5")
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

        #region Black Hole Local Star

        private void CreateBlackHoleLocalStar()
        {
            // TODO: refactor to use ellipsoid like planetary ring

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

            // STAR LIGHT ---------------------------------------------------------------------

            PointLight localStarlight = new(Colors.White, localStarData.Center)
            {
                //Range = localStarData.LightIntensity,
            };

            localStarlight.SetName("LocalStarLight");
            localStarlight.Color = Colors.White;

            // rotate and add objects ---------------------------------------------------------------------
            ThreeDViewer.HelixTKViewport.Children.Add(new ModelVisual3D
            {
                Content = localStarlight
            });


            // STAR CORONA OUTLINE ---------------------------------------------------------------------

            SphereVisual3D localStarCoronaOutline = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.05,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new SolidColorBrush(Colors.Black)),
                Material = new EmissiveMaterial()
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
                            new GradientStop(Colors.White, 1),
                        },
                    }
                }
            };

            localStarCoronaOutline.SetName("LocalStarCoronaOutline");

            // STAR CORONA ---------------------------------------------------------------------

            SphereVisual3D localStarCorona = new()
            {
                Center = localStarData.Center,
                Radius = localStarData.Radius * 1.3,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = new EmissiveMaterial(new RadialGradientBrush(Colors.White, Colors.Transparent))
                {
                    Color = materialColor
                }
            };

            localStarCorona.SetName("LocalStarCorona");

            System.Windows.Media.Color coronaMaterialColor = System.Windows.Media.Color.FromArgb(80, 255, 255, 255);

            localStarCorona.Material = new EmissiveMaterial()
            {
                Color = coronaMaterialColor,
                Brush = new RadialGradientBrush()
                {
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(Colors.Transparent, 0),
                        new GradientStop(Colors.Transparent, 0.5),
                        new GradientStop(coronaMaterialColor, 1),
                    },
                }
            };


            // STAR PARTICLE SYSTEM -------------------------------------------------------------

            byte[] particleResource = Resources.particle;

            BitmapImage particleImage = new();
            using MemoryStream sms = new();
            sms.Write(particleResource, 0, particleResource.Length);
            particleImage.BeginInit();
            particleImage.StreamSource = new MemoryStream(sms.ToArray());
            particleImage.EndInit();

            ImageBrush particleImageBrush = new(particleImage)
            {
                Opacity = 0.8,
            };

            ParticleSystem particleSystem = new()
            {
                Position = localStarData.Center,
                Texture = particleImageBrush,
                EmitRate = 150,
                LifeTime = 3.25,
                StartDirection = new Vector3D(0, 0, 1),
                StartRadius = localStarData.Radius * 1.05,
                StartSize = 0.02,
                StartVelocity = 0.01,
                StartVelocityRandomness = 0.05,
                SizeRate = 0.025,
                StartSpreading = 180,
                FadeOutTime = 0.25,
                //VelocityDamping = 0.999,
                Acceleration = 0.0,
                AliveParticles = 100,
            };

            particleSystem.SetName("LocalStarParticleSystem");

            ThreeDViewer.HelixTKViewport.Children.Add(localStarCoronaOutline);
            ThreeDViewer.HelixTKViewport.Children.Add(localStar);
            ThreeDViewer.HelixTKViewport.Children.Add(localStarCorona);

            // BLACK HOLE ACCRETION DISK ---------------------------------------------------------------------
            localStarData.RotationQuaternion = new Quaternion(new Vector3D(-1, -1, 0), 15);

            BuildAccretionDisk();

            ThreeDViewer.HelixTKViewport.Children.Add(particleSystem);

            // rotate the local star, star corona, and local star light to the correct position
            RotateLocalStarObjects();
        }

        private void BuildAccretionDisk()
        {
            // TODO: redo this with ellipsoid and texture

            // disk 1 innermost ring of accretion disk
            SolidColorBrush transparentBlackBrush = new(System.Windows.Media.Color.FromArgb(10, 0, 0, 0));
            SolidColorBrush whiteBrush = new(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
            SolidColorBrush transparentWhiteBrush = new(System.Windows.Media.Color.FromArgb(190, 255, 255, 255));

            // disk 1

            MaterialGroup innerRingMatGroup = new();
            innerRingMatGroup.Children.Add(new DiffuseMaterial(whiteBrush));
            innerRingMatGroup.Children.Add(new EmissiveMaterial(whiteBrush));
            innerRingMatGroup.Children.Add(new SpecularMaterial(transparentWhiteBrush, 100));

            PieSliceVisual3D ad1 = new()
            {
                Center = localStarData.Center,
                StartAngle = 0,
                EndAngle = 360,
                InnerRadius = localStarData.Radius,
                OuterRadius = localStarData.Radius * 1.25,
                ThetaDiv = 360,
                Visible = true,
                BackMaterial = innerRingMatGroup,
                Material = innerRingMatGroup,
            };

            ad1.SetName("AccretionDisk1");

            // disk 2

            SolidColorBrush waterBlueBrush = new(System.Windows.Media.Color.FromArgb(255, 219, 243, 250))
            {
                Opacity = 0.9
            };

            MaterialGroup ring2MatGroup = new();
            ring2MatGroup.Children.Add(new DiffuseMaterial(transparentBlackBrush));
            ring2MatGroup.Children.Add(new EmissiveMaterial(waterBlueBrush));
            ring2MatGroup.Children.Add(new SpecularMaterial(transparentWhiteBrush, 100));

            PieSliceVisual3D ad2 = new()
            {
                Center = localStarData.Center,
                StartAngle = 0,
                EndAngle = 360,
                InnerRadius = localStarData.Radius * 1.20,
                OuterRadius = localStarData.Radius * 1.45,
                ThetaDiv = 360,
                Visible = true,
                BackMaterial = ring2MatGroup,
                Material = ring2MatGroup
            };

            ad2.SetName("AccretionDisk2");

            // disk 3

            SolidColorBrush ghostWhiteBrush = new(System.Windows.Media.Color.FromArgb(255, 245, 251, 255))
            {
                Opacity = 0.8
            };

            MaterialGroup ring3MatGroup = new();
            ring3MatGroup.Children.Add(new DiffuseMaterial(transparentBlackBrush));
            ring3MatGroup.Children.Add(new EmissiveMaterial(ghostWhiteBrush));
            ring3MatGroup.Children.Add(new SpecularMaterial(transparentWhiteBrush, 100));

            PieSliceVisual3D ad3 = new()
            {
                Center = localStarData.Center,
                StartAngle = 0,
                EndAngle = 360,
                InnerRadius = localStarData.Radius * 1.40,
                OuterRadius = localStarData.Radius * 2.0,
                ThetaDiv = 360,
                Visible = true,
                BackMaterial = ring3MatGroup,
                Material = ring3MatGroup
            };

            ad3.SetName("AccretionDisk3");

            // disk 4

            SolidColorBrush bubblesBlueBrush = new(System.Windows.Media.Color.FromArgb(255, 229, 243, 253))
            {
                Opacity = 0.7
            };

            MaterialGroup ring4MatGroup = new();
            ring4MatGroup.Children.Add(new DiffuseMaterial(transparentBlackBrush));
            ring4MatGroup.Children.Add(new EmissiveMaterial(bubblesBlueBrush));
            ring4MatGroup.Children.Add(new SpecularMaterial(transparentWhiteBrush, 100));

            PieSliceVisual3D ad4 = new()
            {
                Center = localStarData.Center,
                StartAngle = 0,
                EndAngle = 360,
                InnerRadius = localStarData.Radius * 1.95,
                OuterRadius = localStarData.Radius * 2.5,
                ThetaDiv = 360,
                Visible = true,
                BackMaterial = ring4MatGroup,
                Material = ring4MatGroup
            };

            ad4.SetName("AccretionDisk4");

            // disk 5
            SolidColorBrush azurishWhiteBrush = new(System.Windows.Media.Color.FromArgb(255, 209, 229, 244))
            {
                Opacity = 0.6
            };

            MaterialGroup ring5MatGroup = new();
            ring5MatGroup.Children.Add(new DiffuseMaterial(transparentBlackBrush));
            ring5MatGroup.Children.Add(new EmissiveMaterial(azurishWhiteBrush));
            ring5MatGroup.Children.Add(new SpecularMaterial(transparentWhiteBrush, 100));

            PieSliceVisual3D ad5 = new()
            {
                Center = localStarData.Center,
                StartAngle = 0,
                EndAngle = 360,
                InnerRadius = localStarData.Radius * 2.4,
                OuterRadius = localStarData.Radius * 3.0,
                ThetaDiv = 360,
                Visible = true,
                BackMaterial = ring5MatGroup,
                Material = ring5MatGroup
            };

            ad5.SetName("AccretionDisk5");

            ThreeDViewer.HelixTKViewport.Children.Add(ad1);
            ThreeDViewer.HelixTKViewport.Children.Add(ad2);
            ThreeDViewer.HelixTKViewport.Children.Add(ad3);
            ThreeDViewer.HelixTKViewport.Children.Add(ad4);
            ThreeDViewer.HelixTKViewport.Children.Add(ad5);
        }

        #endregion

        #region Corona Local Star

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
                Material = new EmissiveMaterial()
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
                        new GradientStop(Colors.Transparent, 0),
                        new GradientStop(Colors.Transparent, 0.5),
                        new GradientStop(coronaMaterialColor, 1),
                    },
                }
            };

            // STAR PARTICLE SYSTEM -------------------------------------------------------------

            byte[] smokeResource = Resources.white_particle;

            BitmapImage smokeImage = new();
            using MemoryStream sms = new();
            sms.Write(smokeResource, 0, smokeResource.Length);
            smokeImage.BeginInit();
            smokeImage.StreamSource = new MemoryStream(sms.ToArray());
            smokeImage.EndInit();


            ImageBrush smokeImageBrush = new(smokeImage)
            {
                Opacity = 0.25
            };

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

            // rotate and add objects ---------------------------------------------------------------------

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

        #endregion

        #region Gas Giant Local Star

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
                BackMaterial = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(192, 64, 64, 64)))
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
                        new GradientStop(Colors.Transparent, 1)
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

        #endregion

        #region Nebula Local Star

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

        #endregion

        #region Sun Local Star

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
                BackMaterial = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(160, 64, 64, 64)))
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

        #endregion

        #endregion

        #region Planetary Ring Methods
        private void ShowPlanetaryRing()
        {
            RemovePlanetaryRing();
            BuildPlanetaryRing();
        }

        private void BuildPlanetaryRing()
        {
            // ring 1 innermost ring of planetary ring
            SolidColorBrush blackBrush = new(Colors.Black)
            {
                Opacity = planetaryRingData.RingBrushOpacity,
            };

            SolidColorBrush whiteBrush = new(Colors.White)
            {
                Opacity = planetaryRingData.RingBrushOpacity,
            };

            SolidColorBrush transparentBrush = new(Colors.Transparent);

            byte[] ringImageResource = planetaryRingData.RingImageResource;

            BitmapImage ringImage = new();
            using MemoryStream ms = new();
            ms.Write(ringImageResource, 0, ringImageResource.Length);
            ringImage.BeginInit();
            ringImage.StreamSource = new MemoryStream(ms.ToArray());
            ringImage.EndInit();

            // tint the image using the color selected by the user or the default color
            Bitmap b = DrawingMethods.BitmapImageToBitmap(ringImage);

            SKBitmap skb = b.ToSKBitmap();
            SKBitmap tintedBitmap = new(skb.Width, skb.Height);

            using SKCanvas canvas = new(tintedBitmap);

            SKPaint ringPaint = new()
            {
                Style = SKPaintStyle.Fill,
                ColorFilter = SKColorFilter.CreateBlendMode(
                    Extensions.ToSKColor(planetaryRingData.RingColor),
                    SKBlendMode.Modulate) // combine the tint with the bitmap color
            };

            canvas.DrawBitmap(skb, 0, 0, ringPaint);

            Bitmap rb = tintedBitmap.ToBitmap();

            BitmapImage ri = DrawingMethods.BitmapToBitmapImage(rb);

            ImageBrush ringBrush = new(ri)
            {
                Opacity = planetaryRingData.RingBrushOpacity,
                TileMode = TileMode.Tile,
                Viewport = new System.Windows.Rect(0, 0, 1, 1),
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                RelativeTransform = new RotateTransform(90),
            };

            MaterialGroup innerRingMatGroup = new();
            innerRingMatGroup.Children.Add(new DiffuseMaterial(blackBrush));
            innerRingMatGroup.Children.Add(new EmissiveMaterial(ringBrush));
            innerRingMatGroup.Children.Add(new SpecularMaterial(whiteBrush, 100));

            MaterialGroup transparentGroup = new();
            transparentGroup.Children.Add(new DiffuseMaterial(transparentBrush));

            // construct the tilt angle transform
            AxisAngleRotation3D rotation = new(new Vector3D(1, 1, 0), planetaryRingData.RingTiltAngle);
            RotateTransform3D rotationTransform = new(rotation);

            // the transparent ring0 is used to mask the
            // rings so that there is distance between the
            // planet surface and the rings;
            // the order that the rings are added to the viewport
            // is important; the transparent ring0 must be added first

            EllipsoidVisual3D ring0 = new()
            {
                Center = new Point3D(0, 0, 0),
                RadiusX = planetaryRingData.InnerRadius,
                RadiusY = planetaryRingData.InnerRadius,
                RadiusZ = 0.3,
                BackMaterial = transparentGroup,
                Material = transparentGroup,
                Visible = true,
                Transform = rotationTransform,
            };

            ring0.SetName("PlanetaryRing0");

            EllipsoidVisual3D ring1 = new()
            {
                Center = new Point3D(0, 0, 0),
                RadiusX = planetaryRingData.OuterRadius,
                RadiusY = planetaryRingData.OuterRadius,
                RadiusZ = 0.1,
                BackMaterial = innerRingMatGroup,
                Material = innerRingMatGroup,
                Visible = true,
                Transform = rotationTransform,
            };

            ring1.SetName("PlanetaryRing1");

            ThreeDViewer.HelixTKViewport.Children.Add(ring0);
            ThreeDViewer.HelixTKViewport.Children.Add(ring1);

        }

        private void RemovePlanetaryRing()
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is EllipsoidVisual3D ev3d && ev3d.GetName() == "PlanetaryRing0")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is EllipsoidVisual3D ev3d && ev3d.GetName() == "PlanetaryRing1")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }
            }
        }

        #endregion

        #region Atmosphere Methods

        private void RemoveAtmosphere()
        {
            if (ThreeDViewer.HelixTKViewport.Children.Count > 1)
            {
                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dAtmosphere && sv3dAtmosphere.GetName() == "Atmosphere")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }

                for (int i = ThreeDViewer.HelixTKViewport.Children.Count - 1; i >= 0; i--)
                {
                    if (ThreeDViewer.HelixTKViewport.Children[i] is SphereVisual3D sv3dAtmosphereOutline
                        && sv3dAtmosphereOutline.GetName() == "AtmosphereOutline")
                    {
                        ThreeDViewer.HelixTKViewport.Children.RemoveAt(i);
                    }
                }
            }
        }

        private void ShowAtmosphere()
        {
            RemoveAtmosphere();

            System.Windows.Media.Color atmosphereMaterialColor =
                System.Windows.Media.Color.FromArgb(255, atmosphereData.AtmosphereColor.R,
                atmosphereData.AtmosphereColor.G, atmosphereData.AtmosphereColor.B);

            DiffuseMaterial atmosphereMaterial = new()
            {
                AmbientColor = atmosphereMaterialColor,
                Color = atmosphereMaterialColor,
                Brush = new RadialGradientBrush()
                {
                    Opacity = atmosphereData.AtmosphereOpacity,
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    GradientStops =
                    {
                        new GradientStop(Colors.Transparent, 0),
                        new GradientStop(atmosphereMaterialColor, 1),
                    },
                }
            };

            SolidColorBrush whiteBrush = new(Colors.White)
            {
                Opacity = atmosphereData.AtmosphereOpacity,
            };

            MaterialGroup atmosphereMaterialGroup = new();
            atmosphereMaterialGroup.Children.Add(atmosphereMaterial);
            atmosphereMaterialGroup.Children.Add(new SpecularMaterial(whiteBrush, 100));


            // ATMOSPHERE ---------------------------------------------------------------------

            SphereVisual3D atmosphere = new()
            {
                Center = new Point3D(0, 0, 0),
                Radius = 1.0 + atmosphereData.AtmosphereRadius,
                ThetaDiv = 90,
                PhiDiv = 45,
                BackMaterial = atmosphereMaterialGroup,
                Material = atmosphereMaterialGroup,
            };

            atmosphere.SetName("Atmosphere");

            ThreeDViewer.HelixTKViewport.Children.Add(atmosphere);
        }

        #endregion

        #region Effects Methods

        private void ApplyEnabledEffects()
        {
            BlurEffect blurEffect = new()
            {
                Radius = BlurAmountTrack.Value,
                KernelType = KernelType.Gaussian,
                RenderingBias = RenderingBias.Quality
            };

            // apply the blur effect to the viewport  
            ThreeDViewer.G1.Effect = EnableBlurSwitch.Checked ? blurEffect : null;

            // apply the bleach bypass effect to the viewport  
            BleachBypassEffect bleachBypassEffect = new()
            {
                Opacity = BleachBypassOpacityTrack.Value / 100.0
            };

            ThreeDViewer.G2.Effect = EnableBleachBypassSwitch.Checked ? bleachBypassEffect : null;

            BloomEffect bloomEffect = new()
            {
                SceneIntensity = 0.5f,
                GlowIntensity = 0.5f,
                HighlightThreshold = 0.9f,
                HighlightIntensity = 0.5f,
                BlurWidth = BlurWidthTrack.Value / 10.0f,
            };

            ThreeDViewer.G3.Effect = EnableBloomSwitch.Checked ? bloomEffect : null;

            SepiaEffect sepiaEffect = new()
            {
                GrayscaleFactor = SepiaAmountTrack.Value / 100.0f,
            };

            ThreeDViewer.G4.Effect = EnableSepiaSwitch.Checked ? sepiaEffect : null;

            GrayscaleEffect grayscaleEffect = new()
            {
                DesaturationFactor = DesaturationFactorTrack.Value / 100.0f,
            };

            ThreeDViewer.G5.Effect = EnableGrayscaleSwitch.Checked ? grayscaleEffect : null;

            TDContainerPanel.Invalidate();
        }

        #endregion

        #region Moon Methods

        private void UpdateSelectedMoon()
        {
            if (selectedMoon == null)
            {
                return;
            }

            string textureBasePath = UtilityMethods.DEFAULT_ASSETS_FOLDER + Path.DirectorySeparatorChar
                + "Textures" + Path.DirectorySeparatorChar + "Planet" + Path.DirectorySeparatorChar;

            string texturePath = textureBasePath;

            switch (selectedMoon.MoonTextureName)
            {
                case "Mercury":
                    texturePath += "2k_mercury.png";
                    break;
                case "Venus Surface":
                    texturePath += "2k_venus_surface.png";
                    break;
                case "Venus Atmosphere":
                    texturePath += "2k_venus_atmosphere.png";
                    break;
                case "Earth - Blue Marble":
                    texturePath += "bluemarble-2048.png";
                    break;
                case "Earth - Day":
                    texturePath += "2k_earth_daymap.png";
                    break;
                case "Moon":
                    texturePath += "lroc_color_poles_2k.png";
                    break;
                case "NASA Lunar ROC":
                    texturePath += "LunarROC.png";
                    break;
                case "Mars":
                    texturePath += "2k_mars.png";
                    break;
                case "Phobos":
                    texturePath += "nasa_phobos.png";
                    break;
                case "Deimos":
                    texturePath += "deimos_texture_map.png";
                    break;
                case "Jupiter":
                    texturePath += "2k_jupiter.png";
                    break;
                case "Europa":
                    texturePath += "europa_texture.png";
                    break;
                case "Saturn":
                    texturePath += "2k_saturn.png";
                    break;
                case "Saturn - NASA Fictional":
                    texturePath += "nasa_saturn_fictional.png";
                    break;
                case "Uranus":
                    texturePath += "2k_uranus.png";
                    break;
                case "Neptune":
                    texturePath += "2k_neptune.png";
                    break;
                case "Pluto - NASA Fictional":
                    texturePath += "nasa_pluto_fictional.png";
                    break;
                case "Terrestrial - Alpine":
                    texturePath += "Alpine.png";
                    break;
                case "Terrestrial - Savannah":
                    texturePath += "Savannah.png";
                    break;
                case "Terrestrial - Swamp":
                    texturePath += "Swamp.png";
                    break;
                case "Terrestrial - Planet 1":
                    texturePath += "Terrestrial1.png";
                    break;
                case "Terrestrial - Planet 2":
                    texturePath += "Terrestrial2.png";
                    break;
                case "Terrestrial - Planet 3":
                    texturePath += "Terrestrial3.png";
                    break;
                case "Terrestrial - Planet 4":
                    texturePath += "Terrestrial4.png";
                    break;
                case "Terrestrial - Tropical":
                    texturePath += "Tropical.png";
                    break;
                case "Gas Giant":
                    texturePath += "gas giant-equirectangular-11-2048x1024.png";
                    break;
                case "Ceres":
                    texturePath += "2k_ceres_fictional.png";
                    break;
                case "Eris":
                    texturePath += "2k_eris_fictional.png";
                    break;
                case "Haumea":
                    texturePath += "2k_haumea_fictional.png";
                    break;
                case "Makemake":
                    texturePath += "2k_makemake_fictional.png";
                    break;
                case "Custom Moon Texture":
                    texturePath = selectedMoon.MoonTexturePath;
                    break;
                default:
                    texturePath += "lroc_color_poles_2k.png";
                    break;
            }

            if (!File.Exists(texturePath))
            {
                texturePath = textureBasePath + "lroc_color_poles_2k.png";
            }

            if (File.Exists(texturePath))
            {
                BitmapImage moonImage = new();
                moonImage.BeginInit();
                moonImage.UriSource = new Uri(texturePath, UriKind.RelativeOrAbsolute);
                moonImage.EndInit();

                selectedMoon.MoonTexturePath = texturePath;

                System.Windows.Media.Color materialColor = selectedMoon.MoonColor;

                selectedMoon.moonVisual.Material = new EmissiveMaterial(new ImageBrush(moonImage))
                {
                    Color = materialColor,
                };
            }
            else
            {
                System.Windows.Media.Color materialColor = selectedMoon.MoonColor;

                selectedMoon.moonVisual.Material = new EmissiveMaterial(new SolidColorBrush(materialColor))
                {
                    Color = materialColor,
                };
            }

            selectedMoon.moonVisual.Center = selectedMoon.MoonCenter;
            selectedMoon.moonVisual.Radius = selectedMoon.MoonRadius;

            // rotate the moon
            // hangle and vangle are in degrees

            // rotate around Z axis  
            Vector3D axis = new(0, 0, 1);

            // Get the matrix indicating the current transformation value  
            Matrix3D transformationMatrix = Matrix3D.Identity;

            // rotate around the moon center Z axis  
            transformationMatrix.RotateAt(new Quaternion(axis, selectedMoon.MoonAxisRotation), selectedMoon.moonVisual.Center);
            MatrixTransform3D zTransform = new(transformationMatrix);

            double hangle = selectedMoon.MoonOrbitRotation;
            double vangle = selectedMoon.MoonPlaneRotation;

            Quaternion hvQuaternion = Quaternion.Multiply(new Quaternion(new Vector3D(0, 0, 1), hangle), new Quaternion(new Vector3D(1, 0, 0), vangle));
            RotateTransform3D rotateTransform = new(new QuaternionRotation3D(hvQuaternion));

            // add rotations to the transform group; the order matters
            Transform3DGroup transformGroup = new();
            transformGroup.Children.Add(zTransform);
            transformGroup.Children.Add(rotateTransform);

            selectedMoon.moonVisual.Transform = transformGroup;
        }

        #endregion

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
                ThreeDViewer.HelixTKViewport.ShowViewCube = false;
                ThreeDViewer.HelixTKViewport.ShowCoordinateSystem = false;

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
                await RecordAnimationFramesAsync(cloudLayer);

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

                ThreeDViewer.HelixTKViewport.ShowViewCube = true;
                ThreeDViewer.HelixTKViewport.ShowCoordinateSystem = true;

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

        private async Task RecordAnimationFramesAsync(SphereVisual3D? cloudLayer)
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

                    RecordAnimationFrame(cloudLayer);
                    frameCount++;

                    ModelStatisticsLabel.Text = $"Recording video: {frameCount} of {frames} frames.";
                    ModelStatisticsLabel.Refresh();

                    RecordingProgressBar.Value = (int)((double)frameCount / frames * 100.0);
                    RecordingProgressBar.Refresh();
                }
            }
        }

        private void RecordAnimationFrame(SphereVisual3D? cloudLayer)
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

                // take a snapshot of the 3D view  
                using SKBitmap frameBitmap = new((int)ThreeDViewer.ThreeDViewGrid.ActualWidth, (int)ThreeDViewer.HelixTKViewport.ActualHeight);
                using SKCanvas canvas = new(frameBitmap);

                RenderTargetBitmap rtb = new(
                    (int)ThreeDViewer.ThreeDViewGrid.ActualWidth,
                    (int)ThreeDViewer.ThreeDViewGrid.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                rtb.Render(ThreeDViewer.ThreeDViewGrid);

                using Bitmap? b = DrawingMethods.BitmapSourceToBitmap(rtb);

                if (b != null)
                {
                    canvas.DrawBitmap(b.ToSKBitmap(), 0, 0);

                    byte[] bitmapData = DrawingMethods.BitmapToByteArray(b);

                    // write the video frame to the file  
                    videoStream.WriteFrame(true, frameBitmap.Bytes, 0, bitmapData.Length);
                }
            }
        }



        #endregion

        #region Data Structures

        internal sealed class LocalStar
        {
            internal Point3D Center { get; set; } = new Point3D(3, 3, 0);
            internal double Radius { get; set; } = 0.15;
            internal LocalStarImageType StarImageType { get; set; } = LocalStarImageType.Sun;
            internal System.Windows.Media.Color StarColor { get; set; } = Colors.Yellow;
            internal int LightIntensity { get; set; } = 10;
            internal Transform3D LocationTransform { get; set; } = Transform3D.Identity;
            internal Quaternion RotationQuaternion { get; set; } = Quaternion.Identity;
        }

        internal sealed class PlanetaryRing
        {
            internal byte[] RingImageResource = Resources.saturn_ring;
            internal double InnerRadius { get; set; } = 1.5;
            internal double OuterRadius { get; set; } = 2.5;
            internal System.Drawing.Color RingColor { get; set; } = System.Drawing.Color.White;
            internal double RingTiltAngle { get; set; }
            internal double RingBrushOpacity { get; set; } = 100;

        }

        internal sealed class PlanetAtmosphere
        {
            internal double AtmosphereRadius { get; set; } = 0.1;
            internal System.Drawing.Color AtmosphereColor { get; set; } = System.Drawing.Color.FromArgb(255, 103, 137, 175);
            internal double AtmosphereOpacity { get; set; } = 0.5;
        }

        internal sealed class Moon
        {
            internal SphereVisual3D moonVisual = new();
            internal string MoonName { get; set; } = string.Empty;
            internal Point3D MoonCenter { get; set; } = new Point3D(2, 2, 0);
            internal double MoonRadius { get; set; } = 0.1;
            internal string MoonTextureName { get; set; } = "Moon";
            internal string MoonTexturePath { get; set; } = string.Empty;
            internal System.Windows.Media.Color MoonColor { get; set; } = Colors.White;
            internal int MoonDistance { get; set; } = 250;
            internal double MoonOrbitRotation { get; set; }
            internal double MoonPlaneRotation { get; set; }
            internal double MoonAxisRotation { get; set; }
        }

        #endregion


    }
}
