namespace RealmStudio
{
    partial class ThreeDView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ZoomCameraLabelButton = new FontAwesome.Sharp.IconButton();
            PanCameraLabelButton = new FontAwesome.Sharp.IconButton();
            ElevateCameraLabelButton = new FontAwesome.Sharp.IconButton();
            RotateCameraLabelButton = new FontAwesome.Sharp.IconButton();
            PitchCameraLabelButton = new FontAwesome.Sharp.IconButton();
            ThreeDViewOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CameraGroup = new GroupBox();
            ResetCameraButton = new FontAwesome.Sharp.IconButton();
            CameraPitchTrack = new TrackBar();
            RotateTrack = new TrackBar();
            ZoomTrack = new TrackBar();
            ElevateTrack = new TrackBar();
            PanTrack = new TrackBar();
            ModelGroup = new GroupBox();
            ModelResetButton = new FontAwesome.Sharp.IconButton();
            label4 = new Label();
            RollTrack = new TrackBar();
            label3 = new Label();
            PitchTrack = new TrackBar();
            label2 = new Label();
            YawTrack = new TrackBar();
            label1 = new Label();
            ScaleTrack = new TrackBar();
            LoadModelButton = new FontAwesome.Sharp.IconButton();
            CloseFormButton = new Button();
            TDContainerPanel = new Panel();
            ShowGridlinesCheck = new CheckBox();
            ThreeDViewOverlay.SuspendLayout();
            CameraGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CameraPitchTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RotateTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ZoomTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ElevateTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PanTrack).BeginInit();
            ModelGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RollTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PitchTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)YawTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ScaleTrack).BeginInit();
            SuspendLayout();
            // 
            // ZoomCameraLabelButton
            // 
            ZoomCameraLabelButton.CausesValidation = false;
            ZoomCameraLabelButton.FlatAppearance.BorderSize = 0;
            ZoomCameraLabelButton.FlatStyle = FlatStyle.Flat;
            ZoomCameraLabelButton.IconChar = FontAwesome.Sharp.IconChar.ArrowsV;
            ZoomCameraLabelButton.IconColor = Color.Black;
            ZoomCameraLabelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ZoomCameraLabelButton.IconSize = 18;
            ZoomCameraLabelButton.Location = new Point(18, 75);
            ZoomCameraLabelButton.Name = "ZoomCameraLabelButton";
            ZoomCameraLabelButton.Rotation = 45D;
            ZoomCameraLabelButton.Size = new Size(30, 23);
            ZoomCameraLabelButton.TabIndex = 53;
            ZoomCameraLabelButton.TabStop = false;
            ZoomCameraLabelButton.UseVisualStyleBackColor = true;
            ZoomCameraLabelButton.MouseHover += ZoomCameraLabelButton_MouseHover;
            // 
            // PanCameraLabelButton
            // 
            PanCameraLabelButton.CausesValidation = false;
            PanCameraLabelButton.FlatAppearance.BorderSize = 0;
            PanCameraLabelButton.FlatStyle = FlatStyle.Flat;
            PanCameraLabelButton.IconChar = FontAwesome.Sharp.IconChar.ArrowsV;
            PanCameraLabelButton.IconColor = Color.Black;
            PanCameraLabelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            PanCameraLabelButton.IconSize = 18;
            PanCameraLabelButton.Location = new Point(18, 23);
            PanCameraLabelButton.Name = "PanCameraLabelButton";
            PanCameraLabelButton.Rotation = 90D;
            PanCameraLabelButton.Size = new Size(30, 23);
            PanCameraLabelButton.TabIndex = 54;
            PanCameraLabelButton.TabStop = false;
            PanCameraLabelButton.UseVisualStyleBackColor = true;
            PanCameraLabelButton.MouseHover += PanCameraLabelButton_MouseHover;
            // 
            // ElevateCameraLabelButton
            // 
            ElevateCameraLabelButton.CausesValidation = false;
            ElevateCameraLabelButton.FlatAppearance.BorderSize = 0;
            ElevateCameraLabelButton.FlatStyle = FlatStyle.Flat;
            ElevateCameraLabelButton.IconChar = FontAwesome.Sharp.IconChar.ArrowsV;
            ElevateCameraLabelButton.IconColor = Color.Black;
            ElevateCameraLabelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ElevateCameraLabelButton.IconSize = 18;
            ElevateCameraLabelButton.Location = new Point(18, 46);
            ElevateCameraLabelButton.Name = "ElevateCameraLabelButton";
            ElevateCameraLabelButton.Size = new Size(30, 23);
            ElevateCameraLabelButton.TabIndex = 55;
            ElevateCameraLabelButton.TabStop = false;
            ElevateCameraLabelButton.UseVisualStyleBackColor = true;
            ElevateCameraLabelButton.MouseHover += ElevateCameraLabelButton_MouseHover;
            // 
            // RotateCameraLabelButton
            // 
            RotateCameraLabelButton.CausesValidation = false;
            RotateCameraLabelButton.FlatAppearance.BorderSize = 0;
            RotateCameraLabelButton.FlatStyle = FlatStyle.Flat;
            RotateCameraLabelButton.IconChar = FontAwesome.Sharp.IconChar.ArrowRotateLeft;
            RotateCameraLabelButton.IconColor = Color.Black;
            RotateCameraLabelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            RotateCameraLabelButton.IconSize = 18;
            RotateCameraLabelButton.Location = new Point(18, 101);
            RotateCameraLabelButton.Name = "RotateCameraLabelButton";
            RotateCameraLabelButton.Size = new Size(30, 23);
            RotateCameraLabelButton.TabIndex = 57;
            RotateCameraLabelButton.TabStop = false;
            RotateCameraLabelButton.UseVisualStyleBackColor = true;
            RotateCameraLabelButton.MouseHover += RotateCameraLabelButton_MouseHover;
            // 
            // PitchCameraLabelButton
            // 
            PitchCameraLabelButton.CausesValidation = false;
            PitchCameraLabelButton.FlatAppearance.BorderSize = 0;
            PitchCameraLabelButton.FlatStyle = FlatStyle.Flat;
            PitchCameraLabelButton.IconChar = FontAwesome.Sharp.IconChar.ArrowDownUpAcrossLine;
            PitchCameraLabelButton.IconColor = Color.Black;
            PitchCameraLabelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            PitchCameraLabelButton.IconSize = 18;
            PitchCameraLabelButton.Location = new Point(18, 127);
            PitchCameraLabelButton.Name = "PitchCameraLabelButton";
            PitchCameraLabelButton.Size = new Size(30, 23);
            PitchCameraLabelButton.TabIndex = 59;
            PitchCameraLabelButton.TabStop = false;
            PitchCameraLabelButton.UseVisualStyleBackColor = true;
            PitchCameraLabelButton.MouseHover += PitchCameraLabelButton_MouseHover;
            // 
            // ThreeDViewOverlay
            // 
            ThreeDViewOverlay.BackColor = Color.FromArgb(244, 241, 243);
            ThreeDViewOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            ThreeDViewOverlay.Controls.Add(ShowGridlinesCheck);
            ThreeDViewOverlay.Controls.Add(CameraGroup);
            ThreeDViewOverlay.Controls.Add(ModelGroup);
            ThreeDViewOverlay.Controls.Add(CloseFormButton);
            ThreeDViewOverlay.Controls.Add(TDContainerPanel);
            ThreeDViewOverlay.Dock = DockStyle.Fill;
            ThreeDViewOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            ThreeDViewOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            ThreeDViewOverlay.Font = new Font("Segoe UI", 9F);
            ThreeDViewOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            ThreeDViewOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            ThreeDViewOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            ThreeDViewOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            ThreeDViewOverlay.Location = new Point(0, 0);
            ThreeDViewOverlay.Name = "ThreeDViewOverlay";
            ThreeDViewOverlay.Padding = new Padding(20, 56, 20, 16);
            ThreeDViewOverlay.RoundCorners = true;
            ThreeDViewOverlay.Sizable = true;
            ThreeDViewOverlay.Size = new Size(800, 575);
            ThreeDViewOverlay.SmartBounds = true;
            ThreeDViewOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            ThreeDViewOverlay.TabIndex = 0;
            ThreeDViewOverlay.Text = "Model View";
            ThreeDViewOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CameraGroup
            // 
            CameraGroup.Controls.Add(ResetCameraButton);
            CameraGroup.Controls.Add(PitchCameraLabelButton);
            CameraGroup.Controls.Add(CameraPitchTrack);
            CameraGroup.Controls.Add(RotateCameraLabelButton);
            CameraGroup.Controls.Add(RotateTrack);
            CameraGroup.Controls.Add(ElevateCameraLabelButton);
            CameraGroup.Controls.Add(PanCameraLabelButton);
            CameraGroup.Controls.Add(ZoomCameraLabelButton);
            CameraGroup.Controls.Add(ZoomTrack);
            CameraGroup.Controls.Add(ElevateTrack);
            CameraGroup.Controls.Add(PanTrack);
            CameraGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CameraGroup.Location = new Point(23, 305);
            CameraGroup.Name = "CameraGroup";
            CameraGroup.Size = new Size(167, 185);
            CameraGroup.TabIndex = 49;
            CameraGroup.TabStop = false;
            CameraGroup.Text = "Camera";
            // 
            // ResetCameraButton
            // 
            ResetCameraButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ResetCameraButton.ForeColor = SystemColors.ControlDarkDark;
            ResetCameraButton.IconChar = FontAwesome.Sharp.IconChar.ArrowRotateForward;
            ResetCameraButton.IconColor = Color.Black;
            ResetCameraButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ResetCameraButton.IconSize = 15;
            ResetCameraButton.Location = new Point(138, 156);
            ResetCameraButton.Name = "ResetCameraButton";
            ResetCameraButton.Size = new Size(23, 23);
            ResetCameraButton.TabIndex = 60;
            ResetCameraButton.UseVisualStyleBackColor = true;
            ResetCameraButton.Click += ResetCameraButton_Click;
            ResetCameraButton.MouseHover += ResetCameraButton_MouseHover;
            // 
            // CameraPitchTrack
            // 
            CameraPitchTrack.AutoSize = false;
            CameraPitchTrack.Location = new Point(44, 130);
            CameraPitchTrack.Maximum = 179;
            CameraPitchTrack.Name = "CameraPitchTrack";
            CameraPitchTrack.Size = new Size(117, 20);
            CameraPitchTrack.TabIndex = 58;
            CameraPitchTrack.TickStyle = TickStyle.None;
            CameraPitchTrack.Value = 90;
            CameraPitchTrack.Scroll += CameraPitchTrack_Scroll;
            // 
            // RotateTrack
            // 
            RotateTrack.AutoSize = false;
            RotateTrack.Location = new Point(44, 104);
            RotateTrack.Maximum = 359;
            RotateTrack.Name = "RotateTrack";
            RotateTrack.Size = new Size(117, 20);
            RotateTrack.TabIndex = 56;
            RotateTrack.TickStyle = TickStyle.None;
            RotateTrack.Scroll += RotateTrack_Scroll;
            // 
            // ZoomTrack
            // 
            ZoomTrack.AutoSize = false;
            ZoomTrack.Location = new Point(44, 78);
            ZoomTrack.Maximum = 500;
            ZoomTrack.Name = "ZoomTrack";
            ZoomTrack.Size = new Size(117, 20);
            ZoomTrack.TabIndex = 52;
            ZoomTrack.TickStyle = TickStyle.None;
            ZoomTrack.Value = 250;
            ZoomTrack.Scroll += ZoomTrack_Scroll;
            // 
            // ElevateTrack
            // 
            ElevateTrack.AutoSize = false;
            ElevateTrack.Location = new Point(44, 52);
            ElevateTrack.Maximum = 200;
            ElevateTrack.Name = "ElevateTrack";
            ElevateTrack.Size = new Size(117, 20);
            ElevateTrack.TabIndex = 50;
            ElevateTrack.TickStyle = TickStyle.None;
            ElevateTrack.Value = 100;
            ElevateTrack.Scroll += ElevateTrack_Scroll;
            // 
            // PanTrack
            // 
            PanTrack.AutoSize = false;
            PanTrack.Location = new Point(44, 26);
            PanTrack.Maximum = 200;
            PanTrack.Name = "PanTrack";
            PanTrack.Size = new Size(117, 20);
            PanTrack.TabIndex = 48;
            PanTrack.TickStyle = TickStyle.None;
            PanTrack.Value = 100;
            PanTrack.Scroll += PanTrack_Scroll;
            // 
            // ModelGroup
            // 
            ModelGroup.Controls.Add(ModelResetButton);
            ModelGroup.Controls.Add(label4);
            ModelGroup.Controls.Add(RollTrack);
            ModelGroup.Controls.Add(label3);
            ModelGroup.Controls.Add(PitchTrack);
            ModelGroup.Controls.Add(label2);
            ModelGroup.Controls.Add(YawTrack);
            ModelGroup.Controls.Add(label1);
            ModelGroup.Controls.Add(ScaleTrack);
            ModelGroup.Controls.Add(LoadModelButton);
            ModelGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ModelGroup.Location = new Point(23, 59);
            ModelGroup.Name = "ModelGroup";
            ModelGroup.Size = new Size(167, 226);
            ModelGroup.TabIndex = 48;
            ModelGroup.TabStop = false;
            ModelGroup.Text = "Model";
            // 
            // ModelResetButton
            // 
            ModelResetButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ModelResetButton.ForeColor = SystemColors.ControlDarkDark;
            ModelResetButton.IconChar = FontAwesome.Sharp.IconChar.ArrowRotateForward;
            ModelResetButton.IconColor = Color.Black;
            ModelResetButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ModelResetButton.IconSize = 15;
            ModelResetButton.Location = new Point(138, 196);
            ModelResetButton.Name = "ModelResetButton";
            ModelResetButton.Size = new Size(23, 23);
            ModelResetButton.TabIndex = 54;
            ModelResetButton.UseVisualStyleBackColor = true;
            ModelResetButton.Click += ModelResetButton_Click;
            ModelResetButton.MouseHover += ModelResetButton_MouseHover;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(18, 170);
            label4.Name = "label4";
            label4.Size = new Size(27, 15);
            label4.TabIndex = 53;
            label4.Text = "Roll";
            // 
            // RollTrack
            // 
            RollTrack.AutoSize = false;
            RollTrack.Location = new Point(44, 170);
            RollTrack.Maximum = 359;
            RollTrack.Name = "RollTrack";
            RollTrack.Size = new Size(117, 20);
            RollTrack.TabIndex = 52;
            RollTrack.TickStyle = TickStyle.None;
            RollTrack.Scroll += RollTrack_Scroll;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(12, 144);
            label3.Name = "label3";
            label3.Size = new Size(34, 15);
            label3.TabIndex = 51;
            label3.Text = "Pitch";
            // 
            // PitchTrack
            // 
            PitchTrack.AutoSize = false;
            PitchTrack.Location = new Point(44, 144);
            PitchTrack.Maximum = 359;
            PitchTrack.Name = "PitchTrack";
            PitchTrack.Size = new Size(117, 20);
            PitchTrack.TabIndex = 50;
            PitchTrack.TickStyle = TickStyle.None;
            PitchTrack.Scroll += PitchTrack_Scroll;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(18, 118);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 49;
            label2.Text = "Yaw";
            // 
            // YawTrack
            // 
            YawTrack.AutoSize = false;
            YawTrack.Location = new Point(44, 118);
            YawTrack.Maximum = 359;
            YawTrack.Name = "YawTrack";
            YawTrack.Size = new Size(117, 20);
            YawTrack.TabIndex = 48;
            YawTrack.TickStyle = TickStyle.None;
            YawTrack.Scroll += YawTrack_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(12, 92);
            label1.Name = "label1";
            label1.Size = new Size(34, 15);
            label1.TabIndex = 47;
            label1.Text = "Scale";
            // 
            // ScaleTrack
            // 
            ScaleTrack.AutoSize = false;
            ScaleTrack.Location = new Point(44, 92);
            ScaleTrack.Maximum = 200;
            ScaleTrack.Minimum = 1;
            ScaleTrack.Name = "ScaleTrack";
            ScaleTrack.Size = new Size(117, 20);
            ScaleTrack.TabIndex = 46;
            ScaleTrack.TickStyle = TickStyle.None;
            ScaleTrack.Value = 100;
            ScaleTrack.Scroll += ScaleTrack_Scroll;
            // 
            // LoadModelButton
            // 
            LoadModelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoadModelButton.ForeColor = SystemColors.ControlDarkDark;
            LoadModelButton.IconChar = FontAwesome.Sharp.IconChar.None;
            LoadModelButton.IconColor = Color.Black;
            LoadModelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LoadModelButton.Location = new Point(12, 26);
            LoadModelButton.Name = "LoadModelButton";
            LoadModelButton.Size = new Size(60, 60);
            LoadModelButton.TabIndex = 38;
            LoadModelButton.Text = "Load";
            LoadModelButton.UseVisualStyleBackColor = true;
            LoadModelButton.Click += LoadModelButton_Click;
            // 
            // CloseFormButton
            // 
            CloseFormButton.ForeColor = SystemColors.ControlDarkDark;
            CloseFormButton.Location = new Point(717, 496);
            CloseFormButton.Name = "CloseFormButton";
            CloseFormButton.Size = new Size(60, 60);
            CloseFormButton.TabIndex = 36;
            CloseFormButton.Text = "&Close";
            CloseFormButton.UseVisualStyleBackColor = true;
            CloseFormButton.Click += CloseFormButton_Click;
            // 
            // TDContainerPanel
            // 
            TDContainerPanel.BorderStyle = BorderStyle.FixedSingle;
            TDContainerPanel.Location = new Point(196, 59);
            TDContainerPanel.Name = "TDContainerPanel";
            TDContainerPanel.Size = new Size(581, 431);
            TDContainerPanel.TabIndex = 0;
            // 
            // ShowGridlinesCheck
            // 
            ShowGridlinesCheck.AutoSize = true;
            ShowGridlinesCheck.Checked = true;
            ShowGridlinesCheck.CheckState = CheckState.Checked;
            ShowGridlinesCheck.ForeColor = SystemColors.ControlDarkDark;
            ShowGridlinesCheck.Location = new Point(196, 496);
            ShowGridlinesCheck.Name = "ShowGridlinesCheck";
            ShowGridlinesCheck.Size = new Size(110, 19);
            ShowGridlinesCheck.TabIndex = 50;
            ShowGridlinesCheck.Text = "Show Grid Lines";
            ShowGridlinesCheck.UseVisualStyleBackColor = true;
            ShowGridlinesCheck.CheckedChanged += ShowGridlinesCheck_CheckedChanged;
            // 
            // ThreeDView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 575);
            ControlBox = false;
            Controls.Add(ThreeDViewOverlay);
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(261, 65);
            Name = "ThreeDView";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Model View";
            TransparencyKey = Color.Fuchsia;
            FormClosing += ThreeDView_FormClosing;
            Load += ThreeDView_Load;
            KeyPress += ThreeDView_KeyPress;
            ThreeDViewOverlay.ResumeLayout(false);
            ThreeDViewOverlay.PerformLayout();
            CameraGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CameraPitchTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)RotateTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)ZoomTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)ElevateTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)PanTrack).EndInit();
            ModelGroup.ResumeLayout(false);
            ModelGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RollTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)PitchTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)YawTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)ScaleTrack).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm ThreeDViewOverlay;
        private Panel TDContainerPanel;
        private Button CloseFormButton;
        private GroupBox ModelGroup;
        private Label label4;
        private TrackBar RollTrack;
        private Label label3;
        private TrackBar PitchTrack;
        private Label label2;
        private TrackBar YawTrack;
        private Label label1;
        private TrackBar ScaleTrack;
        private FontAwesome.Sharp.IconButton LoadModelButton;
        private GroupBox CameraGroup;
        private TrackBar PanTrack;
        private TrackBar ElevateTrack;
        private TrackBar ZoomTrack;
        private FontAwesome.Sharp.IconButton ZoomCameraLabelButton;
        private TrackBar RotateTrack;
        private TrackBar CameraPitchTrack;
        private FontAwesome.Sharp.IconButton ModelResetButton;
        private FontAwesome.Sharp.IconButton ResetCameraButton;
        private FontAwesome.Sharp.IconButton PanCameraLabelButton;
        private FontAwesome.Sharp.IconButton ElevateCameraLabelButton;
        private FontAwesome.Sharp.IconButton RotateCameraLabelButton;
        private FontAwesome.Sharp.IconButton PitchCameraLabelButton;
        private CheckBox ShowGridlinesCheck;
    }
}