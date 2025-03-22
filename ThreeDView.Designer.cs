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
            ThreeDViewOverlay = new ReaLTaiizor.Forms.DungeonForm();
            ThreeDViewControlBox = new ReaLTaiizor.Controls.ThunderControlBox();
            CameraGroup = new GroupBox();
            ChangeAxesButton = new FontAwesome.Sharp.IconButton();
            ResetCameraButton = new FontAwesome.Sharp.IconButton();
            ShowGridlinesCheck = new CheckBox();
            ModelGroup = new GroupBox();
            SaveModelButton = new FontAwesome.Sharp.IconButton();
            LoadModelButton = new FontAwesome.Sharp.IconButton();
            CloseFormButton = new Button();
            TDContainerPanel = new Panel();
            ModelStatisticsLabel = new Label();
            label1 = new Label();
            ThreeDViewOverlay.SuspendLayout();
            CameraGroup.SuspendLayout();
            ModelGroup.SuspendLayout();
            SuspendLayout();
            // 
            // ThreeDViewOverlay
            // 
            ThreeDViewOverlay.BackColor = Color.FromArgb(244, 241, 243);
            ThreeDViewOverlay.BorderColor = Color.FromArgb(50, 50, 50);
            ThreeDViewOverlay.Controls.Add(label1);
            ThreeDViewOverlay.Controls.Add(ModelStatisticsLabel);
            ThreeDViewOverlay.Controls.Add(ThreeDViewControlBox);
            ThreeDViewOverlay.Controls.Add(CameraGroup);
            ThreeDViewOverlay.Controls.Add(ShowGridlinesCheck);
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
            ThreeDViewOverlay.Size = new Size(1237, 744);
            ThreeDViewOverlay.SmartBounds = true;
            ThreeDViewOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            ThreeDViewOverlay.TabIndex = 0;
            ThreeDViewOverlay.Text = "Model View";
            ThreeDViewOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // ThreeDViewControlBox
            // 
            ThreeDViewControlBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ThreeDViewControlBox.BackColor = Color.Transparent;
            ThreeDViewControlBox.DefaultLocation = true;
            ThreeDViewControlBox.ForeColor = SystemColors.ControlLight;
            ThreeDViewControlBox.Location = new Point(1159, 3);
            ThreeDViewControlBox.Name = "ThreeDViewControlBox";
            ThreeDViewControlBox.Size = new Size(75, 23);
            ThreeDViewControlBox.TabIndex = 57;
            ThreeDViewControlBox.Text = "thunderControlBox1";
            // 
            // CameraGroup
            // 
            CameraGroup.Controls.Add(ChangeAxesButton);
            CameraGroup.Controls.Add(ResetCameraButton);
            CameraGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CameraGroup.Location = new Point(23, 230);
            CameraGroup.Name = "CameraGroup";
            CameraGroup.Size = new Size(86, 180);
            CameraGroup.TabIndex = 56;
            CameraGroup.TabStop = false;
            CameraGroup.Text = "Camera";
            // 
            // ChangeAxesButton
            // 
            ChangeAxesButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ChangeAxesButton.ForeColor = SystemColors.ControlDarkDark;
            ChangeAxesButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ChangeAxesButton.IconColor = Color.Black;
            ChangeAxesButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ChangeAxesButton.Location = new Point(12, 92);
            ChangeAxesButton.Name = "ChangeAxesButton";
            ChangeAxesButton.Size = new Size(60, 60);
            ChangeAxesButton.TabIndex = 39;
            ChangeAxesButton.Text = "Change Axis";
            ChangeAxesButton.UseVisualStyleBackColor = true;
            ChangeAxesButton.Click += ChangeAxesButton_Click;
            // 
            // ResetCameraButton
            // 
            ResetCameraButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ResetCameraButton.ForeColor = SystemColors.ControlDarkDark;
            ResetCameraButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ResetCameraButton.IconColor = Color.Black;
            ResetCameraButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ResetCameraButton.Location = new Point(12, 26);
            ResetCameraButton.Name = "ResetCameraButton";
            ResetCameraButton.Size = new Size(60, 60);
            ResetCameraButton.TabIndex = 38;
            ResetCameraButton.Text = "Reset";
            ResetCameraButton.UseVisualStyleBackColor = true;
            ResetCameraButton.Click += ResetCameraButton_Click;
            // 
            // ShowGridlinesCheck
            // 
            ShowGridlinesCheck.AutoSize = true;
            ShowGridlinesCheck.Checked = true;
            ShowGridlinesCheck.CheckState = CheckState.Checked;
            ShowGridlinesCheck.ForeColor = SystemColors.ControlDarkDark;
            ShowGridlinesCheck.Location = new Point(23, 416);
            ShowGridlinesCheck.Name = "ShowGridlinesCheck";
            ShowGridlinesCheck.Size = new Size(80, 19);
            ShowGridlinesCheck.TabIndex = 50;
            ShowGridlinesCheck.Text = "Show Grid";
            ShowGridlinesCheck.UseVisualStyleBackColor = true;
            ShowGridlinesCheck.CheckedChanged += ShowGridlinesCheck_CheckedChanged;
            // 
            // ModelGroup
            // 
            ModelGroup.Controls.Add(SaveModelButton);
            ModelGroup.Controls.Add(LoadModelButton);
            ModelGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ModelGroup.Location = new Point(23, 59);
            ModelGroup.Name = "ModelGroup";
            ModelGroup.Size = new Size(86, 165);
            ModelGroup.TabIndex = 48;
            ModelGroup.TabStop = false;
            ModelGroup.Text = "Model";
            // 
            // SaveModelButton
            // 
            SaveModelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SaveModelButton.ForeColor = SystemColors.ControlDarkDark;
            SaveModelButton.IconChar = FontAwesome.Sharp.IconChar.None;
            SaveModelButton.IconColor = Color.Black;
            SaveModelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SaveModelButton.Location = new Point(12, 92);
            SaveModelButton.Name = "SaveModelButton";
            SaveModelButton.Size = new Size(60, 60);
            SaveModelButton.TabIndex = 55;
            SaveModelButton.Text = "Save";
            SaveModelButton.UseVisualStyleBackColor = true;
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
            CloseFormButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CloseFormButton.ForeColor = SystemColors.ControlDarkDark;
            CloseFormButton.Location = new Point(1157, 665);
            CloseFormButton.Name = "CloseFormButton";
            CloseFormButton.Size = new Size(60, 60);
            CloseFormButton.TabIndex = 36;
            CloseFormButton.Text = "&Close";
            CloseFormButton.UseVisualStyleBackColor = true;
            CloseFormButton.Click += CloseFormButton_Click;
            // 
            // TDContainerPanel
            // 
            TDContainerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TDContainerPanel.BorderStyle = BorderStyle.FixedSingle;
            TDContainerPanel.Location = new Point(115, 59);
            TDContainerPanel.Name = "TDContainerPanel";
            TDContainerPanel.Size = new Size(1102, 600);
            TDContainerPanel.TabIndex = 0;
            // 
            // ModelStatisticsLabel
            // 
            ModelStatisticsLabel.ForeColor = Color.LightSlateGray;
            ModelStatisticsLabel.Location = new Point(115, 665);
            ModelStatisticsLabel.Name = "ModelStatisticsLabel";
            ModelStatisticsLabel.Size = new Size(260, 23);
            ModelStatisticsLabel.TabIndex = 58;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(286, 710);
            label1.Name = "label1";
            label1.Size = new Size(827, 13);
            label1.TabIndex = 59;
            label1.Text = "Click+drag left mouse button to move. Click+drag right mouse button to rotate. Turn Mouse Wheel to zoom. CLick Change Axis button to change rotation axis.";
            // 
            // ThreeDView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1237, 744);
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
            ResizeEnd += ThreeDView_SizeChanged;
            ThreeDViewOverlay.ResumeLayout(false);
            ThreeDViewOverlay.PerformLayout();
            CameraGroup.ResumeLayout(false);
            ModelGroup.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm ThreeDViewOverlay;
        private Panel TDContainerPanel;
        private Button CloseFormButton;
        private GroupBox ModelGroup;
        private FontAwesome.Sharp.IconButton LoadModelButton;
        private CheckBox ShowGridlinesCheck;
        private FontAwesome.Sharp.IconButton SaveModelButton;
        private GroupBox CameraGroup;
        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton ResetCameraButton;
        private ReaLTaiizor.Controls.ThunderControlBox ThreeDViewControlBox;
        private FontAwesome.Sharp.IconButton ChangeAxesButton;
        private Label ModelStatisticsLabel;
        private Label label1;
    }
}