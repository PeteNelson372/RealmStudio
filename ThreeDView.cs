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
using System.IO;
using System.Text;
using System.Windows.Forms.Integration;
using System.Windows.Media.Media3D;

namespace RealmStudio
{
    /***
     * This form hosts the ThreeDViwerControl WPF UserControl that displays a 3D model.
     */
    public partial class ThreeDView : Form
    {
        private readonly ThreeDViewerControl ThreeDViewer;
        private readonly List<string>? ModelUpdateQueue;

        private string? LoadedModelString;

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

        public ThreeDView(string formTitle)
        {
            InitializeComponent();
            ThreeDViewOverlay.Text = formTitle;

            // construct the WPF ThreeDViewerControl UserControl
            ThreeDViewer = new();
        }

        public ThreeDView(string formTitle, List<string> modelUpdateQueue)
        {
            InitializeComponent();
            ThreeDViewOverlay.Text = formTitle;
            ModelUpdateQueue = modelUpdateQueue;

            LoadModelButton.Enabled = false;

            // construct the WPF ThreeDViewerControl UserControl
            ThreeDViewer = new();
        }

        public void UpdateView()
        {
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

        private void ThreeDView_FormClosing(object sender, FormClosingEventArgs e)
        {
            ModelUpdateQueue?.Clear();

            if (ParentForm != null && HeightMapManager.CurrentHeightMapView == this)
            {
                HeightMapManager.CurrentHeightMapView = null;
            }
        }

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

        private void LoadModelGroupIntoViewer(Model3DGroup modelGroup)
        {
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

        private void ModelGroup_Changed(object? sender, EventArgs e)
        {
            //Vector3D lookedAtPoint = ThreeDViewer.ModelCamera.LookDirection;
            //Point3D? cameraPosition = ThreeDViewer.ModelCamera.Position;

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
    }
}
