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
using System.Diagnostics;
using System.Windows.Forms.Integration;
using System.Windows.Media.Media3D;
//using System.Windows.Media.Media3D;

namespace RealmStudio
{
    public partial class ThreeDView : Form
    {

        private readonly ThreeDViewerControl ThreeDViewer;

        private static readonly ToolTip TOOLTIP = new();

        private ScaleTransform3D ScaleTransform = new(1, 1, 1);
        private RotateTransform3D YawRotation = new(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
        private RotateTransform3D PitchRotation = new(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0));
        private RotateTransform3D RollRotation = new(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0));

        private TranslateTransform3D PanTransform = new TranslateTransform3D(0, 0, 0);
        private TranslateTransform3D ElevateTransform = new TranslateTransform3D(0, 0, 0);
        private TranslateTransform3D ZoomTransform = new TranslateTransform3D(0, 0, 0);

        private static readonly Vector3D thetaAxis = new Vector3D(0, 1, 0);
        private static readonly Vector3D phiAxis = new Vector3D(-1, 0, 0);

        private QuaternionRotation3D PitchQuaternion = new QuaternionRotation3D(new Quaternion(-phiAxis, 0));
        private QuaternionRotation3D RotationQuaternion = new QuaternionRotation3D(new Quaternion(-thetaAxis, 0));

        private readonly Transform3DGroup ModelTransform3DGroup = new();
        private readonly Transform3DGroup CameraTransform3DGroup = new();

        private readonly ModelVisual3D GridlinesModel = new();
        private readonly GridLinesVisual3D GridLines = new()
        {
            Center = new Point3D(0, -0.5, 0),
            Normal = new Vector3D(0, 1, 0),
            Width = 20,
            Length = 20,
            MinorDistance = 1,
            MajorDistance = 10,
            Thickness = 0.02,
            Fill = System.Windows.Media.Brushes.Gray
        };

        public ThreeDView(string formTitle)
        {
            InitializeComponent();
            ThreeDViewOverlay.Text = formTitle;
            ThreeDViewer = new(this);
        }

        private void ThreeDView_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ThreeDView_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar.ToString().ToUpper(new System.Globalization.CultureInfo("en-us"))[0];
        }

        private void CloseFormButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadModelButton_Click(object sender, EventArgs e)
        {

        }

        private void ThreeDView_Load(object sender, EventArgs e)
        {
            // Create the ElementHost control for hosting the
            // WPF ThreeDViewerControl UserControl.
            ElementHost host = new()
            {
                Parent = TDContainerPanel,
                Dock = DockStyle.Fill,
                Child = ThreeDViewer,
            };

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.

            // Add the ElementHost control to the container panel's controls.
            TDContainerPanel.Controls.Add(host);

            // add gridlines
            GridlinesModel.SetName("GridLines");
            GridlinesModel.Children.Add(GridLines);
            ThreeDViewer.HelixTKViewport.Children.Add(GridlinesModel);
        }

        private void ModelGroup_Changed(object? sender, EventArgs e)
        {
            Vector3D lookedAtPoint = ThreeDViewer.ModelCamera.LookDirection;
            Point3D? newPosition = ThreeDViewer.ModelCamera.Position;

            Point3D p3d = (Point3D)(newPosition + lookedAtPoint);

            Debug.WriteLine("ModelGroup_Changed NP: " + newPosition.ToString());
            Debug.WriteLine("ModelGroup_Changed V3D: " + lookedAtPoint.ToString());

            Debug.WriteLine("ModelGroup_Changed P3D (NP + V3D): " + p3d.ToString());
        }

        private void ScaleTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ScaleTrack.Value.ToString(), ModelGroup, new Point(ScaleTrack.Right - 30, ScaleTrack.Top - 20), 2000);

            // scale up/down
            double scalePercent = ScaleTrack.Value / 100.0;
            ScaleTransform = new ScaleTransform3D(scalePercent, scalePercent, scalePercent);
            ApplyModelTransforms();
        }


        private void YawTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(YawTrack.Value.ToString(), ModelGroup, new Point(YawTrack.Right - 30, YawTrack.Top - 20), 2000);

            YawRotation = new(new AxisAngleRotation3D(new Vector3D(0, 1, 0), YawTrack.Value));
            ApplyModelTransforms();
        }

        private void PitchTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(PitchTrack.Value.ToString(), ModelGroup, new Point(PitchTrack.Right - 30, PitchTrack.Top - 20), 2000);

            PitchRotation = new(new AxisAngleRotation3D(new Vector3D(1, 0, 0), PitchTrack.Value));
            ApplyModelTransforms();
        }

        private void RollTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(RollTrack.Value.ToString(), ModelGroup, new Point(RollTrack.Right - 30, RollTrack.Top - 20), 2000);

            RollRotation = new(new AxisAngleRotation3D(new Vector3D(0, 0, 1), RollTrack.Value));
            ApplyModelTransforms();
        }

        private void PanTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(PanTrack.Value.ToString(), CameraGroup, new Point(PanTrack.Right - 30, PanTrack.Top - 20), 2000);

            double panValue = -(100.0 - PanTrack.Value) / 100.0;
            PanTransform = new TranslateTransform3D(panValue, 0, 0);

            ApplyCameraTransforms();
        }

        private void ElevateTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ElevateTrack.Value.ToString(), CameraGroup, new Point(ElevateTrack.Right - 30, ElevateTrack.Top - 20), 2000);

            double elevateValue = (100.0 - ElevateTrack.Value) / 100.0;
            ElevateTransform = new TranslateTransform3D(0, elevateValue, 0);

            ApplyCameraTransforms();
        }

        private void ZoomTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ZoomTrack.Value.ToString(), CameraGroup, new Point(ZoomTrack.Right - 30, ZoomTrack.Top - 20), 2000);

            double zoomValue = (250.0 - ZoomTrack.Value) / 100.0;
            ZoomTransform = new TranslateTransform3D(0, 0, zoomValue);

            ApplyCameraTransforms();
        }

        private void RotateTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(RotateTrack.Value.ToString(), CameraGroup, new Point(RotateTrack.Right - 30, RotateTrack.Top - 20), 2000);

            RotationQuaternion = new(new Quaternion(thetaAxis, RotateTrack.Value));
            ApplyCameraTransforms();
        }

        private void CameraPitchTrack_Scroll(object sender, EventArgs e)
        {
            double pitchValue = 90.0 - CameraPitchTrack.Value;

            TOOLTIP.Show(pitchValue.ToString(), CameraGroup, new Point(CameraPitchTrack.Right - 30, CameraPitchTrack.Top - 20), 2000);

            PitchQuaternion = new QuaternionRotation3D(new Quaternion(phiAxis, pitchValue));
            ApplyCameraTransforms();
        }

        private void ApplyModelTransforms()
        {
            ModelTransform3DGroup.Children.Clear();

            ModelTransform3DGroup.Children.Add(YawRotation);
            ModelTransform3DGroup.Children.Add(PitchRotation);
            ModelTransform3DGroup.Children.Add(RollRotation);
            ModelTransform3DGroup.Children.Add(ScaleTransform);

            ThreeDViewer.DisplayedModel.Transform = ModelTransform3DGroup;
        }

        private void ApplyCameraTransforms()
        {
            CameraTransform3DGroup.Children.Clear();

            // translation
            CameraTransform3DGroup.Children.Add(PanTransform);
            CameraTransform3DGroup.Children.Add(ElevateTransform);
            CameraTransform3DGroup.Children.Add(ZoomTransform);

            // rotation
            CameraTransform3DGroup.Children.Add(new RotateTransform3D(RotationQuaternion));
            CameraTransform3DGroup.Children.Add(new RotateTransform3D(PitchQuaternion));


            ThreeDViewer.ModelCamera.Transform = CameraTransform3DGroup;
        }

        private void ModelResetButton_Click(object sender, EventArgs e)
        {
            ScaleTransform = new(1, 1, 1);
            YawRotation = new(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
            PitchRotation = new(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0));
            RollRotation = new(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0));

            ScaleTrack.Value = 100;
            YawTrack.Value = 0;
            PitchTrack.Value = 0;
            RollTrack.Value = 0;

            ApplyModelTransforms();
        }

        private void ModelResetButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Reset model transformations", ModelGroup, new Point(ModelResetButton.Left, ModelResetButton.Top + 30), 2000);
        }

        private void ResetCameraButton_Click(object sender, EventArgs e)
        {
            PanTransform = new TranslateTransform3D(0, 0, 0);
            ElevateTransform = new TranslateTransform3D(0, 0, 0);
            ZoomTransform = new TranslateTransform3D(0, 0, 0);

            PitchQuaternion = new QuaternionRotation3D(new Quaternion(-phiAxis, 0));
            RotationQuaternion = new QuaternionRotation3D(new Quaternion(-thetaAxis, 0));

            PanTrack.Value = 100;
            ElevateTrack.Value = 100;
            ZoomTrack.Value = 100;
            CameraPitchTrack.Value = 90;
            RotateTrack.Value = 0;

            ApplyCameraTransforms();
        }

        private void ResetCameraButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Reset camera transformations", CameraGroup, new Point(ResetCameraButton.Left, ResetCameraButton.Top + 30), 2000);
        }

        private void PanCameraLabelButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Pan camera left/right", CameraGroup, new Point(PanCameraLabelButton.Left, PanCameraLabelButton.Top + 30), 2000);
        }

        private void ElevateCameraLabelButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Move camera up/down", CameraGroup, new Point(PanCameraLabelButton.Left, PanCameraLabelButton.Top + 30), 2000);
        }

        private void ZoomCameraLabelButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Zoom camera forward/back", CameraGroup, new Point(PanCameraLabelButton.Left, PanCameraLabelButton.Top + 30), 2000);
        }

        private void RotateCameraLabelButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Revolve camera around model", CameraGroup, new Point(PanCameraLabelButton.Left, PanCameraLabelButton.Top + 30), 2000);
        }

        private void PitchCameraLabelButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Tilt camera up/down", CameraGroup, new Point(PanCameraLabelButton.Left, PanCameraLabelButton.Top + 30), 2000);
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
    }
}
