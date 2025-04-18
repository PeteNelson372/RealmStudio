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
            groupBox2 = new GroupBox();
            ShowGridlinesCheck = new CheckBox();
            ShowHideLightingPanelButton = new FontAwesome.Sharp.IconButton();
            ClearBackgroundButton = new FontAwesome.Sharp.IconButton();
            LoadBackgroundButton = new FontAwesome.Sharp.IconButton();
            AnimationGroup = new GroupBox();
            RecordButton = new FontAwesome.Sharp.IconButton();
            ShowHideFeaturesButton = new FontAwesome.Sharp.IconButton();
            ShowHideCloudPanelButton = new FontAwesome.Sharp.IconButton();
            label3 = new Label();
            FrameRateCombo = new ComboBox();
            label2 = new Label();
            RotationRateUpDown = new NumericUpDown();
            AnimateRotationLabel = new Label();
            EnableAnimationSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            label1 = new Label();
            ModelStatisticsLabel = new Label();
            ThreeDViewControlBox = new ReaLTaiizor.Controls.ThunderControlBox();
            CameraGroup = new GroupBox();
            ChangeAxesButton = new FontAwesome.Sharp.IconButton();
            ResetCameraButton = new FontAwesome.Sharp.IconButton();
            ModelGroup = new GroupBox();
            SaveModelButton = new FontAwesome.Sharp.IconButton();
            LoadModelButton = new FontAwesome.Sharp.IconButton();
            CloseFormButton = new Button();
            TDContainerPanel = new Panel();
            FeaturesPanel = new Panel();
            MoonGroup = new GroupBox();
            ExpandMoonGroup = new FontAwesome.Sharp.IconButton();
            EffectsGroup = new GroupBox();
            ExpandEffectsGroup = new FontAwesome.Sharp.IconButton();
            RingGroup = new GroupBox();
            ExpandRingGroup = new FontAwesome.Sharp.IconButton();
            AtmosphereGroup = new GroupBox();
            ExpandAtmosphereGroup = new FontAwesome.Sharp.IconButton();
            LocalStarGroup = new GroupBox();
            ExpandStarGroup = new FontAwesome.Sharp.IconButton();
            LightingPanel = new Panel();
            LightingGroup = new GroupBox();
            label12 = new Label();
            AmbientLightColorButton = new FontAwesome.Sharp.IconButton();
            label11 = new Label();
            EnableAmbientLightSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            label10 = new Label();
            SunlightColorButton = new FontAwesome.Sharp.IconButton();
            label9 = new Label();
            SunlightVerticalDirectionTrack = new TrackBar();
            label8 = new Label();
            SunlightHorizontalDirectionTrack = new TrackBar();
            label7 = new Label();
            EnableSunlightSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            CloudsPanel = new Panel();
            CloudsGroup = new GroupBox();
            SelectCloudColorButton = new FontAwesome.Sharp.IconButton();
            label6 = new Label();
            CloudRotationRateUpDown = new NumericUpDown();
            label32 = new Label();
            CloudTextureOpacityTrack = new TrackBar();
            LoadCloudTextureButton = new FontAwesome.Sharp.IconButton();
            CustomCloudTextureRadio = new RadioButton();
            label5 = new Label();
            DefaultCloudTextureRadio = new RadioButton();
            label4 = new Label();
            EnableCloudsSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            ThreeDViewOverlay.SuspendLayout();
            groupBox2.SuspendLayout();
            AnimationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RotationRateUpDown).BeginInit();
            CameraGroup.SuspendLayout();
            ModelGroup.SuspendLayout();
            TDContainerPanel.SuspendLayout();
            FeaturesPanel.SuspendLayout();
            MoonGroup.SuspendLayout();
            EffectsGroup.SuspendLayout();
            RingGroup.SuspendLayout();
            AtmosphereGroup.SuspendLayout();
            LocalStarGroup.SuspendLayout();
            LightingPanel.SuspendLayout();
            LightingGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SunlightVerticalDirectionTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SunlightHorizontalDirectionTrack).BeginInit();
            CloudsPanel.SuspendLayout();
            CloudsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CloudRotationRateUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CloudTextureOpacityTrack).BeginInit();
            SuspendLayout();
            // 
            // ThreeDViewOverlay
            // 
            ThreeDViewOverlay.BackColor = Color.FromArgb(244, 241, 243);
            ThreeDViewOverlay.BorderColor = Color.FromArgb(50, 50, 50);
            ThreeDViewOverlay.Controls.Add(groupBox2);
            ThreeDViewOverlay.Controls.Add(AnimationGroup);
            ThreeDViewOverlay.Controls.Add(label1);
            ThreeDViewOverlay.Controls.Add(ModelStatisticsLabel);
            ThreeDViewOverlay.Controls.Add(ThreeDViewControlBox);
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
            ThreeDViewOverlay.Size = new Size(1200, 744);
            ThreeDViewOverlay.SmartBounds = true;
            ThreeDViewOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            ThreeDViewOverlay.TabIndex = 0;
            ThreeDViewOverlay.Text = "Model View";
            ThreeDViewOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(ShowGridlinesCheck);
            groupBox2.Controls.Add(ShowHideLightingPanelButton);
            groupBox2.Controls.Add(ClearBackgroundButton);
            groupBox2.Controls.Add(LoadBackgroundButton);
            groupBox2.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(23, 275);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(153, 190);
            groupBox2.TabIndex = 63;
            groupBox2.TabStop = false;
            groupBox2.Text = "Scene";
            // 
            // ShowGridlinesCheck
            // 
            ShowGridlinesCheck.AutoSize = true;
            ShowGridlinesCheck.Checked = true;
            ShowGridlinesCheck.CheckState = CheckState.Checked;
            ShowGridlinesCheck.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowGridlinesCheck.ForeColor = SystemColors.ControlDarkDark;
            ShowGridlinesCheck.Location = new Point(6, 161);
            ShowGridlinesCheck.Name = "ShowGridlinesCheck";
            ShowGridlinesCheck.Size = new Size(80, 19);
            ShowGridlinesCheck.TabIndex = 70;
            ShowGridlinesCheck.Text = "Show Grid";
            ShowGridlinesCheck.UseVisualStyleBackColor = true;
            ShowGridlinesCheck.CheckedChanged += ShowGridlinesCheck_CheckedChanged;
            // 
            // ShowHideLightingPanelButton
            // 
            ShowHideLightingPanelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowHideLightingPanelButton.ForeColor = SystemColors.ControlDarkDark;
            ShowHideLightingPanelButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ShowHideLightingPanelButton.IconColor = Color.Black;
            ShowHideLightingPanelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ShowHideLightingPanelButton.Location = new Point(6, 92);
            ShowHideLightingPanelButton.Name = "ShowHideLightingPanelButton";
            ShowHideLightingPanelButton.Size = new Size(60, 60);
            ShowHideLightingPanelButton.TabIndex = 69;
            ShowHideLightingPanelButton.Text = "Lighting";
            ShowHideLightingPanelButton.UseVisualStyleBackColor = true;
            ShowHideLightingPanelButton.Click += ShowHideLightingPanelButton_Click;
            // 
            // ClearBackgroundButton
            // 
            ClearBackgroundButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ClearBackgroundButton.ForeColor = SystemColors.ControlDarkDark;
            ClearBackgroundButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ClearBackgroundButton.IconColor = Color.Black;
            ClearBackgroundButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ClearBackgroundButton.Location = new Point(72, 26);
            ClearBackgroundButton.Name = "ClearBackgroundButton";
            ClearBackgroundButton.Size = new Size(60, 60);
            ClearBackgroundButton.TabIndex = 39;
            ClearBackgroundButton.Text = "Clear";
            ClearBackgroundButton.UseVisualStyleBackColor = true;
            ClearBackgroundButton.Click += ClearBackgroundButton_Click;
            // 
            // LoadBackgroundButton
            // 
            LoadBackgroundButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoadBackgroundButton.ForeColor = SystemColors.ControlDarkDark;
            LoadBackgroundButton.IconChar = FontAwesome.Sharp.IconChar.None;
            LoadBackgroundButton.IconColor = Color.Black;
            LoadBackgroundButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LoadBackgroundButton.Location = new Point(6, 26);
            LoadBackgroundButton.Name = "LoadBackgroundButton";
            LoadBackgroundButton.Size = new Size(60, 60);
            LoadBackgroundButton.TabIndex = 38;
            LoadBackgroundButton.Text = "Load Texture";
            LoadBackgroundButton.UseVisualStyleBackColor = true;
            LoadBackgroundButton.Click += LoadBackgroundButton_Click;
            // 
            // AnimationGroup
            // 
            AnimationGroup.Controls.Add(RecordButton);
            AnimationGroup.Controls.Add(ShowHideFeaturesButton);
            AnimationGroup.Controls.Add(ShowHideCloudPanelButton);
            AnimationGroup.Controls.Add(label3);
            AnimationGroup.Controls.Add(FrameRateCombo);
            AnimationGroup.Controls.Add(label2);
            AnimationGroup.Controls.Add(RotationRateUpDown);
            AnimationGroup.Controls.Add(AnimateRotationLabel);
            AnimationGroup.Controls.Add(EnableAnimationSwitch);
            AnimationGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AnimationGroup.Location = new Point(23, 476);
            AnimationGroup.Name = "AnimationGroup";
            AnimationGroup.Size = new Size(153, 256);
            AnimationGroup.TabIndex = 62;
            AnimationGroup.TabStop = false;
            AnimationGroup.Text = "World Animation";
            AnimationGroup.Visible = false;
            // 
            // RecordButton
            // 
            RecordButton.BackColor = Color.MediumSeaGreen;
            RecordButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RecordButton.ForeColor = SystemColors.Control;
            RecordButton.IconChar = FontAwesome.Sharp.IconChar.Play;
            RecordButton.IconColor = Color.Black;
            RecordButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            RecordButton.IconSize = 32;
            RecordButton.Location = new Point(6, 186);
            RecordButton.Name = "RecordButton";
            RecordButton.Size = new Size(60, 60);
            RecordButton.TabIndex = 71;
            RecordButton.Text = "Record";
            RecordButton.UseVisualStyleBackColor = false;
            RecordButton.Click += RecordButton_Click;
            // 
            // ShowHideFeaturesButton
            // 
            ShowHideFeaturesButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowHideFeaturesButton.ForeColor = SystemColors.ControlDarkDark;
            ShowHideFeaturesButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ShowHideFeaturesButton.IconColor = Color.Black;
            ShowHideFeaturesButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ShowHideFeaturesButton.Location = new Point(72, 120);
            ShowHideFeaturesButton.Name = "ShowHideFeaturesButton";
            ShowHideFeaturesButton.Size = new Size(60, 60);
            ShowHideFeaturesButton.TabIndex = 70;
            ShowHideFeaturesButton.Text = "Features";
            ShowHideFeaturesButton.UseVisualStyleBackColor = true;
            ShowHideFeaturesButton.Click += ShowHideFeaturesButton_Click;
            // 
            // ShowHideCloudPanelButton
            // 
            ShowHideCloudPanelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowHideCloudPanelButton.ForeColor = SystemColors.ControlDarkDark;
            ShowHideCloudPanelButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ShowHideCloudPanelButton.IconColor = Color.Black;
            ShowHideCloudPanelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ShowHideCloudPanelButton.Location = new Point(6, 120);
            ShowHideCloudPanelButton.Name = "ShowHideCloudPanelButton";
            ShowHideCloudPanelButton.Size = new Size(60, 60);
            ShowHideCloudPanelButton.TabIndex = 69;
            ShowHideCloudPanelButton.Text = "Clouds";
            ShowHideCloudPanelButton.UseVisualStyleBackColor = true;
            ShowHideCloudPanelButton.Click += ShowHideCloudPanelButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(66, 94);
            label3.Name = "label3";
            label3.Size = new Size(66, 15);
            label3.TabIndex = 67;
            label3.Text = "Frame Rate";
            // 
            // FrameRateCombo
            // 
            FrameRateCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            FrameRateCombo.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FrameRateCombo.Items.AddRange(new object[] { "10", "15", "20", "30", "60", "120" });
            FrameRateCombo.Location = new Point(6, 91);
            FrameRateCombo.MaxDropDownItems = 6;
            FrameRateCombo.Name = "FrameRateCombo";
            FrameRateCombo.Size = new Size(54, 23);
            FrameRateCombo.TabIndex = 66;
            FrameRateCombo.SelectedIndexChanged += FrameRateCombo_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(66, 64);
            label2.Name = "label2";
            label2.Size = new Size(78, 15);
            label2.TabIndex = 65;
            label2.Text = "Rotation Rate";
            // 
            // RotationRateUpDown
            // 
            RotationRateUpDown.DecimalPlaces = 1;
            RotationRateUpDown.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RotationRateUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            RotationRateUpDown.Location = new Point(6, 62);
            RotationRateUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            RotationRateUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            RotationRateUpDown.Name = "RotationRateUpDown";
            RotationRateUpDown.Size = new Size(54, 23);
            RotationRateUpDown.TabIndex = 64;
            RotationRateUpDown.TextAlign = HorizontalAlignment.Center;
            RotationRateUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            RotationRateUpDown.ValueChanged += RotationRateUpDown_ValueChanged;
            // 
            // AnimateRotationLabel
            // 
            AnimateRotationLabel.AutoSize = true;
            AnimateRotationLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AnimateRotationLabel.ForeColor = SystemColors.ControlDarkDark;
            AnimateRotationLabel.Location = new Point(47, 35);
            AnimateRotationLabel.Name = "AnimateRotationLabel";
            AnimateRotationLabel.Size = new Size(90, 15);
            AnimateRotationLabel.TabIndex = 63;
            AnimateRotationLabel.Text = "Enable Rotation";
            // 
            // EnableAnimationSwitch
            // 
            EnableAnimationSwitch.Alpha = 50;
            EnableAnimationSwitch.BackColor = Color.Transparent;
            EnableAnimationSwitch.Background = true;
            EnableAnimationSwitch.Background_WidthPen = 2F;
            EnableAnimationSwitch.BackgroundPen = false;
            EnableAnimationSwitch.Checked = false;
            EnableAnimationSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            EnableAnimationSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            EnableAnimationSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            EnableAnimationSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            EnableAnimationSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            EnableAnimationSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            EnableAnimationSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            EnableAnimationSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            EnableAnimationSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            EnableAnimationSwitch.ColorValue = Color.ForestGreen;
            EnableAnimationSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            EnableAnimationSwitch.Font = new Font("Arial", 11F);
            EnableAnimationSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            EnableAnimationSwitch.Lighting = true;
            EnableAnimationSwitch.LinearGradient_Background = false;
            EnableAnimationSwitch.LinearGradient_Value = false;
            EnableAnimationSwitch.LinearGradientPen = false;
            EnableAnimationSwitch.Location = new Point(6, 30);
            EnableAnimationSwitch.Name = "EnableAnimationSwitch";
            EnableAnimationSwitch.PenWidth = 10;
            EnableAnimationSwitch.RGB = false;
            EnableAnimationSwitch.Rounding = true;
            EnableAnimationSwitch.RoundingInt = 90;
            EnableAnimationSwitch.Size = new Size(35, 20);
            EnableAnimationSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            EnableAnimationSwitch.TabIndex = 62;
            EnableAnimationSwitch.Tag = "Cyber";
            EnableAnimationSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            EnableAnimationSwitch.Timer_RGB = 300;
            EnableAnimationSwitch.CheckedChanged += EnableAnimationSwitch_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(252, 712);
            label1.Name = "label1";
            label1.Size = new Size(825, 13);
            label1.TabIndex = 59;
            label1.Text = "Click+drag left mouse button to move. Click+drag right mouse button to rotate. Turn Mouse Wheel to zoom. Click Change Axis button to change rotation axis.";
            // 
            // ModelStatisticsLabel
            // 
            ModelStatisticsLabel.ForeColor = Color.LightSlateGray;
            ModelStatisticsLabel.Location = new Point(182, 662);
            ModelStatisticsLabel.Name = "ModelStatisticsLabel";
            ModelStatisticsLabel.Size = new Size(260, 23);
            ModelStatisticsLabel.TabIndex = 58;
            // 
            // ThreeDViewControlBox
            // 
            ThreeDViewControlBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ThreeDViewControlBox.BackColor = Color.Transparent;
            ThreeDViewControlBox.DefaultLocation = true;
            ThreeDViewControlBox.ForeColor = SystemColors.ControlLight;
            ThreeDViewControlBox.Location = new Point(1122, 3);
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
            CameraGroup.Location = new Point(23, 167);
            CameraGroup.Name = "CameraGroup";
            CameraGroup.Size = new Size(153, 102);
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
            ChangeAxesButton.Location = new Point(72, 26);
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
            ResetCameraButton.Location = new Point(6, 26);
            ResetCameraButton.Name = "ResetCameraButton";
            ResetCameraButton.Size = new Size(60, 60);
            ResetCameraButton.TabIndex = 38;
            ResetCameraButton.Text = "Reset";
            ResetCameraButton.UseVisualStyleBackColor = true;
            ResetCameraButton.Click += ResetCameraButton_Click;
            // 
            // ModelGroup
            // 
            ModelGroup.Controls.Add(SaveModelButton);
            ModelGroup.Controls.Add(LoadModelButton);
            ModelGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ModelGroup.Location = new Point(23, 59);
            ModelGroup.Name = "ModelGroup";
            ModelGroup.Size = new Size(153, 102);
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
            SaveModelButton.Location = new Point(72, 26);
            SaveModelButton.Name = "SaveModelButton";
            SaveModelButton.Size = new Size(60, 60);
            SaveModelButton.TabIndex = 55;
            SaveModelButton.Text = "Save";
            SaveModelButton.UseVisualStyleBackColor = true;
            SaveModelButton.Click += SaveModelButton_Click;
            // 
            // LoadModelButton
            // 
            LoadModelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoadModelButton.ForeColor = SystemColors.ControlDarkDark;
            LoadModelButton.IconChar = FontAwesome.Sharp.IconChar.None;
            LoadModelButton.IconColor = Color.Black;
            LoadModelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LoadModelButton.Location = new Point(6, 26);
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
            CloseFormButton.Location = new Point(1122, 665);
            CloseFormButton.Name = "CloseFormButton";
            CloseFormButton.Size = new Size(60, 60);
            CloseFormButton.TabIndex = 36;
            CloseFormButton.Text = "&Close";
            CloseFormButton.UseVisualStyleBackColor = true;
            CloseFormButton.Click += CloseFormButton_Click;
            // 
            // TDContainerPanel
            // 
            TDContainerPanel.Anchor = AnchorStyles.None;
            TDContainerPanel.BorderStyle = BorderStyle.FixedSingle;
            TDContainerPanel.Controls.Add(FeaturesPanel);
            TDContainerPanel.Controls.Add(LightingPanel);
            TDContainerPanel.Controls.Add(CloudsPanel);
            TDContainerPanel.Location = new Point(182, 59);
            TDContainerPanel.Name = "TDContainerPanel";
            TDContainerPanel.Size = new Size(1000, 600);
            TDContainerPanel.TabIndex = 0;
            // 
            // FeaturesPanel
            // 
            FeaturesPanel.BackColor = Color.WhiteSmoke;
            FeaturesPanel.Controls.Add(MoonGroup);
            FeaturesPanel.Controls.Add(EffectsGroup);
            FeaturesPanel.Controls.Add(RingGroup);
            FeaturesPanel.Controls.Add(AtmosphereGroup);
            FeaturesPanel.Controls.Add(LocalStarGroup);
            FeaturesPanel.Location = new Point(794, 307);
            FeaturesPanel.Name = "FeaturesPanel";
            FeaturesPanel.Size = new Size(200, 288);
            FeaturesPanel.TabIndex = 2;
            FeaturesPanel.Visible = false;
            // 
            // MoonGroup
            // 
            MoonGroup.Controls.Add(ExpandMoonGroup);
            MoonGroup.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MoonGroup.Location = new Point(3, 196);
            MoonGroup.Name = "MoonGroup";
            MoonGroup.Size = new Size(194, 40);
            MoonGroup.TabIndex = 4;
            MoonGroup.TabStop = false;
            MoonGroup.Text = "Moon";
            // 
            // ExpandMoonGroup
            // 
            ExpandMoonGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExpandMoonGroup.ForeColor = SystemColors.ControlDarkDark;
            ExpandMoonGroup.IconChar = FontAwesome.Sharp.IconChar.PlusSquare;
            ExpandMoonGroup.IconColor = Color.Black;
            ExpandMoonGroup.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ExpandMoonGroup.IconSize = 18;
            ExpandMoonGroup.Location = new Point(169, 11);
            ExpandMoonGroup.Name = "ExpandMoonGroup";
            ExpandMoonGroup.Size = new Size(23, 23);
            ExpandMoonGroup.TabIndex = 2;
            ExpandMoonGroup.UseVisualStyleBackColor = true;
            ExpandMoonGroup.Click += ExpandMoonGroup_Click;
            // 
            // EffectsGroup
            // 
            EffectsGroup.Controls.Add(ExpandEffectsGroup);
            EffectsGroup.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            EffectsGroup.Location = new Point(3, 150);
            EffectsGroup.Name = "EffectsGroup";
            EffectsGroup.Size = new Size(194, 40);
            EffectsGroup.TabIndex = 3;
            EffectsGroup.TabStop = false;
            EffectsGroup.Text = "Effects";
            // 
            // ExpandEffectsGroup
            // 
            ExpandEffectsGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExpandEffectsGroup.ForeColor = SystemColors.ControlDarkDark;
            ExpandEffectsGroup.IconChar = FontAwesome.Sharp.IconChar.PlusSquare;
            ExpandEffectsGroup.IconColor = Color.Black;
            ExpandEffectsGroup.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ExpandEffectsGroup.IconSize = 18;
            ExpandEffectsGroup.Location = new Point(169, 11);
            ExpandEffectsGroup.Name = "ExpandEffectsGroup";
            ExpandEffectsGroup.Size = new Size(23, 23);
            ExpandEffectsGroup.TabIndex = 2;
            ExpandEffectsGroup.UseVisualStyleBackColor = true;
            ExpandEffectsGroup.Click += ExpandEffectsGroup_Click;
            // 
            // RingGroup
            // 
            RingGroup.Controls.Add(ExpandRingGroup);
            RingGroup.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RingGroup.Location = new Point(3, 104);
            RingGroup.Name = "RingGroup";
            RingGroup.Size = new Size(194, 40);
            RingGroup.TabIndex = 2;
            RingGroup.TabStop = false;
            RingGroup.Text = "Ring";
            // 
            // ExpandRingGroup
            // 
            ExpandRingGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExpandRingGroup.ForeColor = SystemColors.ControlDarkDark;
            ExpandRingGroup.IconChar = FontAwesome.Sharp.IconChar.PlusSquare;
            ExpandRingGroup.IconColor = Color.Black;
            ExpandRingGroup.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ExpandRingGroup.IconSize = 18;
            ExpandRingGroup.Location = new Point(169, 11);
            ExpandRingGroup.Name = "ExpandRingGroup";
            ExpandRingGroup.Size = new Size(23, 23);
            ExpandRingGroup.TabIndex = 2;
            ExpandRingGroup.UseVisualStyleBackColor = true;
            ExpandRingGroup.Click += ExpandRingGroup_Click;
            // 
            // AtmosphereGroup
            // 
            AtmosphereGroup.Controls.Add(ExpandAtmosphereGroup);
            AtmosphereGroup.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AtmosphereGroup.Location = new Point(3, 58);
            AtmosphereGroup.Name = "AtmosphereGroup";
            AtmosphereGroup.Size = new Size(194, 40);
            AtmosphereGroup.TabIndex = 1;
            AtmosphereGroup.TabStop = false;
            AtmosphereGroup.Text = "Atmosphere";
            // 
            // ExpandAtmosphereGroup
            // 
            ExpandAtmosphereGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExpandAtmosphereGroup.ForeColor = SystemColors.ControlDarkDark;
            ExpandAtmosphereGroup.IconChar = FontAwesome.Sharp.IconChar.PlusSquare;
            ExpandAtmosphereGroup.IconColor = Color.Black;
            ExpandAtmosphereGroup.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ExpandAtmosphereGroup.IconSize = 18;
            ExpandAtmosphereGroup.Location = new Point(169, 11);
            ExpandAtmosphereGroup.Name = "ExpandAtmosphereGroup";
            ExpandAtmosphereGroup.Size = new Size(23, 23);
            ExpandAtmosphereGroup.TabIndex = 2;
            ExpandAtmosphereGroup.UseVisualStyleBackColor = true;
            ExpandAtmosphereGroup.Click += ExpandAtmosphereGroup_Click;
            // 
            // LocalStarGroup
            // 
            LocalStarGroup.Controls.Add(ExpandStarGroup);
            LocalStarGroup.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LocalStarGroup.Location = new Point(2, 12);
            LocalStarGroup.Name = "LocalStarGroup";
            LocalStarGroup.Size = new Size(194, 40);
            LocalStarGroup.TabIndex = 0;
            LocalStarGroup.TabStop = false;
            LocalStarGroup.Text = "Local Star";
            // 
            // ExpandStarGroup
            // 
            ExpandStarGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExpandStarGroup.ForeColor = SystemColors.ControlDarkDark;
            ExpandStarGroup.IconChar = FontAwesome.Sharp.IconChar.PlusSquare;
            ExpandStarGroup.IconColor = Color.Black;
            ExpandStarGroup.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ExpandStarGroup.IconSize = 18;
            ExpandStarGroup.Location = new Point(169, 11);
            ExpandStarGroup.Name = "ExpandStarGroup";
            ExpandStarGroup.Size = new Size(23, 23);
            ExpandStarGroup.TabIndex = 2;
            ExpandStarGroup.UseVisualStyleBackColor = true;
            ExpandStarGroup.Click += ExpandStarGroup_Click;
            // 
            // LightingPanel
            // 
            LightingPanel.Controls.Add(LightingGroup);
            LightingPanel.Location = new Point(3, 3);
            LightingPanel.Name = "LightingPanel";
            LightingPanel.Size = new Size(200, 364);
            LightingPanel.TabIndex = 1;
            LightingPanel.Visible = false;
            // 
            // LightingGroup
            // 
            LightingGroup.BackColor = Color.WhiteSmoke;
            LightingGroup.Controls.Add(label12);
            LightingGroup.Controls.Add(AmbientLightColorButton);
            LightingGroup.Controls.Add(label11);
            LightingGroup.Controls.Add(EnableAmbientLightSwitch);
            LightingGroup.Controls.Add(label10);
            LightingGroup.Controls.Add(SunlightColorButton);
            LightingGroup.Controls.Add(label9);
            LightingGroup.Controls.Add(SunlightVerticalDirectionTrack);
            LightingGroup.Controls.Add(label8);
            LightingGroup.Controls.Add(SunlightHorizontalDirectionTrack);
            LightingGroup.Controls.Add(label7);
            LightingGroup.Controls.Add(EnableSunlightSwitch);
            LightingGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LightingGroup.Location = new Point(3, 3);
            LightingGroup.Name = "LightingGroup";
            LightingGroup.Size = new Size(194, 358);
            LightingGroup.TabIndex = 0;
            LightingGroup.TabStop = false;
            LightingGroup.Text = "Lighting";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.ForeColor = SystemColors.ControlDarkDark;
            label12.Location = new Point(72, 305);
            label12.Name = "label12";
            label12.Size = new Size(115, 15);
            label12.TabIndex = 79;
            label12.Text = "Ambient Light Color";
            // 
            // AmbientLightColorButton
            // 
            AmbientLightColorButton.BackColor = Color.White;
            AmbientLightColorButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AmbientLightColorButton.ForeColor = SystemColors.ControlDarkDark;
            AmbientLightColorButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            AmbientLightColorButton.IconColor = Color.Tan;
            AmbientLightColorButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AmbientLightColorButton.IconSize = 32;
            AmbientLightColorButton.Location = new Point(6, 282);
            AmbientLightColorButton.Name = "AmbientLightColorButton";
            AmbientLightColorButton.Size = new Size(60, 60);
            AmbientLightColorButton.TabIndex = 78;
            AmbientLightColorButton.Text = "Select Color";
            AmbientLightColorButton.UseVisualStyleBackColor = false;
            AmbientLightColorButton.Click += AmbientLightColorButton_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.ForeColor = SystemColors.ControlDarkDark;
            label11.Location = new Point(47, 254);
            label11.Name = "label11";
            label11.Size = new Size(121, 15);
            label11.TabIndex = 77;
            label11.Text = "Enable Ambient Light";
            // 
            // EnableAmbientLightSwitch
            // 
            EnableAmbientLightSwitch.Alpha = 50;
            EnableAmbientLightSwitch.BackColor = Color.Transparent;
            EnableAmbientLightSwitch.Background = true;
            EnableAmbientLightSwitch.Background_WidthPen = 2F;
            EnableAmbientLightSwitch.BackgroundPen = false;
            EnableAmbientLightSwitch.Checked = true;
            EnableAmbientLightSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            EnableAmbientLightSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            EnableAmbientLightSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            EnableAmbientLightSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            EnableAmbientLightSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            EnableAmbientLightSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            EnableAmbientLightSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            EnableAmbientLightSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            EnableAmbientLightSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            EnableAmbientLightSwitch.ColorValue = Color.ForestGreen;
            EnableAmbientLightSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            EnableAmbientLightSwitch.Font = new Font("Arial", 11F);
            EnableAmbientLightSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            EnableAmbientLightSwitch.Lighting = true;
            EnableAmbientLightSwitch.LinearGradient_Background = false;
            EnableAmbientLightSwitch.LinearGradient_Value = false;
            EnableAmbientLightSwitch.LinearGradientPen = false;
            EnableAmbientLightSwitch.Location = new Point(6, 249);
            EnableAmbientLightSwitch.Name = "EnableAmbientLightSwitch";
            EnableAmbientLightSwitch.PenWidth = 10;
            EnableAmbientLightSwitch.RGB = false;
            EnableAmbientLightSwitch.Rounding = true;
            EnableAmbientLightSwitch.RoundingInt = 90;
            EnableAmbientLightSwitch.Size = new Size(35, 20);
            EnableAmbientLightSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            EnableAmbientLightSwitch.TabIndex = 76;
            EnableAmbientLightSwitch.Tag = "Cyber";
            EnableAmbientLightSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            EnableAmbientLightSwitch.Timer_RGB = 300;
            EnableAmbientLightSwitch.CheckedChanged += EnableAmbientLightSwitch_CheckedChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.ForeColor = SystemColors.ControlDarkDark;
            label10.Location = new Point(72, 194);
            label10.Name = "label10";
            label10.Size = new Size(83, 15);
            label10.TabIndex = 75;
            label10.Text = "Sunlight Color";
            // 
            // SunlightColorButton
            // 
            SunlightColorButton.BackColor = Color.White;
            SunlightColorButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SunlightColorButton.ForeColor = SystemColors.ControlDarkDark;
            SunlightColorButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            SunlightColorButton.IconColor = Color.Tan;
            SunlightColorButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SunlightColorButton.IconSize = 32;
            SunlightColorButton.Location = new Point(6, 171);
            SunlightColorButton.Name = "SunlightColorButton";
            SunlightColorButton.Size = new Size(60, 60);
            SunlightColorButton.TabIndex = 74;
            SunlightColorButton.Text = "Select Color";
            SunlightColorButton.UseVisualStyleBackColor = false;
            SunlightColorButton.Click += SunlightColorButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ControlDarkDark;
            label9.Location = new Point(6, 127);
            label9.Name = "label9";
            label9.Size = new Size(164, 15);
            label9.TabIndex = 73;
            label9.Text = "Sunlight Direction (Up/Down)";
            // 
            // SunlightVerticalDirectionTrack
            // 
            SunlightVerticalDirectionTrack.AutoSize = false;
            SunlightVerticalDirectionTrack.BackColor = SystemColors.Control;
            SunlightVerticalDirectionTrack.Location = new Point(6, 145);
            SunlightVerticalDirectionTrack.Maximum = 359;
            SunlightVerticalDirectionTrack.Name = "SunlightVerticalDirectionTrack";
            SunlightVerticalDirectionTrack.RightToLeft = RightToLeft.No;
            SunlightVerticalDirectionTrack.Size = new Size(182, 20);
            SunlightVerticalDirectionTrack.TabIndex = 72;
            SunlightVerticalDirectionTrack.TickStyle = TickStyle.None;
            SunlightVerticalDirectionTrack.Scroll += SunlightVerticalDirectionTrack_Scroll;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.ForeColor = SystemColors.ControlDarkDark;
            label8.Location = new Point(6, 74);
            label8.Name = "label8";
            label8.Size = new Size(166, 15);
            label8.TabIndex = 71;
            label8.Text = "Sunlight Direction (Left/Right)";
            // 
            // SunlightHorizontalDirectionTrack
            // 
            SunlightHorizontalDirectionTrack.AutoSize = false;
            SunlightHorizontalDirectionTrack.BackColor = SystemColors.Control;
            SunlightHorizontalDirectionTrack.Location = new Point(6, 92);
            SunlightHorizontalDirectionTrack.Maximum = 359;
            SunlightHorizontalDirectionTrack.Name = "SunlightHorizontalDirectionTrack";
            SunlightHorizontalDirectionTrack.RightToLeft = RightToLeft.No;
            SunlightHorizontalDirectionTrack.Size = new Size(182, 20);
            SunlightHorizontalDirectionTrack.TabIndex = 70;
            SunlightHorizontalDirectionTrack.TickStyle = TickStyle.None;
            SunlightHorizontalDirectionTrack.Scroll += SunlightHorizontalDirectionTrack_Scroll;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.ForeColor = SystemColors.ControlDarkDark;
            label7.Location = new Point(47, 34);
            label7.Name = "label7";
            label7.Size = new Size(89, 15);
            label7.TabIndex = 65;
            label7.Text = "Enable Sunlight";
            // 
            // EnableSunlightSwitch
            // 
            EnableSunlightSwitch.Alpha = 50;
            EnableSunlightSwitch.BackColor = Color.Transparent;
            EnableSunlightSwitch.Background = true;
            EnableSunlightSwitch.Background_WidthPen = 2F;
            EnableSunlightSwitch.BackgroundPen = false;
            EnableSunlightSwitch.Checked = false;
            EnableSunlightSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            EnableSunlightSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            EnableSunlightSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            EnableSunlightSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            EnableSunlightSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            EnableSunlightSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            EnableSunlightSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            EnableSunlightSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            EnableSunlightSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            EnableSunlightSwitch.ColorValue = Color.ForestGreen;
            EnableSunlightSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            EnableSunlightSwitch.Font = new Font("Arial", 11F);
            EnableSunlightSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            EnableSunlightSwitch.Lighting = true;
            EnableSunlightSwitch.LinearGradient_Background = false;
            EnableSunlightSwitch.LinearGradient_Value = false;
            EnableSunlightSwitch.LinearGradientPen = false;
            EnableSunlightSwitch.Location = new Point(6, 29);
            EnableSunlightSwitch.Name = "EnableSunlightSwitch";
            EnableSunlightSwitch.PenWidth = 10;
            EnableSunlightSwitch.RGB = false;
            EnableSunlightSwitch.Rounding = true;
            EnableSunlightSwitch.RoundingInt = 90;
            EnableSunlightSwitch.Size = new Size(35, 20);
            EnableSunlightSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            EnableSunlightSwitch.TabIndex = 64;
            EnableSunlightSwitch.Tag = "Cyber";
            EnableSunlightSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            EnableSunlightSwitch.Timer_RGB = 300;
            EnableSunlightSwitch.CheckedChanged += EnableSunlightSwitch_CheckedChanged;
            // 
            // CloudsPanel
            // 
            CloudsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CloudsPanel.Controls.Add(CloudsGroup);
            CloudsPanel.Location = new Point(794, 3);
            CloudsPanel.Name = "CloudsPanel";
            CloudsPanel.Size = new Size(200, 298);
            CloudsPanel.TabIndex = 0;
            CloudsPanel.Visible = false;
            // 
            // CloudsGroup
            // 
            CloudsGroup.BackColor = Color.WhiteSmoke;
            CloudsGroup.Controls.Add(SelectCloudColorButton);
            CloudsGroup.Controls.Add(label6);
            CloudsGroup.Controls.Add(CloudRotationRateUpDown);
            CloudsGroup.Controls.Add(label32);
            CloudsGroup.Controls.Add(CloudTextureOpacityTrack);
            CloudsGroup.Controls.Add(LoadCloudTextureButton);
            CloudsGroup.Controls.Add(CustomCloudTextureRadio);
            CloudsGroup.Controls.Add(label5);
            CloudsGroup.Controls.Add(DefaultCloudTextureRadio);
            CloudsGroup.Controls.Add(label4);
            CloudsGroup.Controls.Add(EnableCloudsSwitch);
            CloudsGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CloudsGroup.Location = new Point(3, 3);
            CloudsGroup.Name = "CloudsGroup";
            CloudsGroup.Size = new Size(194, 292);
            CloudsGroup.TabIndex = 1;
            CloudsGroup.TabStop = false;
            CloudsGroup.Text = "Clouds";
            // 
            // SelectCloudColorButton
            // 
            SelectCloudColorButton.BackColor = Color.White;
            SelectCloudColorButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SelectCloudColorButton.ForeColor = SystemColors.ControlDarkDark;
            SelectCloudColorButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            SelectCloudColorButton.IconColor = Color.Tan;
            SelectCloudColorButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SelectCloudColorButton.IconSize = 32;
            SelectCloudColorButton.Location = new Point(13, 181);
            SelectCloudColorButton.Name = "SelectCloudColorButton";
            SelectCloudColorButton.Size = new Size(60, 60);
            SelectCloudColorButton.TabIndex = 73;
            SelectCloudColorButton.Text = "Select Color";
            SelectCloudColorButton.UseVisualStyleBackColor = false;
            SelectCloudColorButton.Click += SelectCloudColorButton_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(83, 249);
            label6.Name = "label6";
            label6.Size = new Size(78, 15);
            label6.TabIndex = 72;
            label6.Text = "Rotation Rate";
            // 
            // CloudRotationRateUpDown
            // 
            CloudRotationRateUpDown.DecimalPlaces = 2;
            CloudRotationRateUpDown.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CloudRotationRateUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            CloudRotationRateUpDown.Location = new Point(13, 247);
            CloudRotationRateUpDown.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            CloudRotationRateUpDown.Name = "CloudRotationRateUpDown";
            CloudRotationRateUpDown.Size = new Size(64, 23);
            CloudRotationRateUpDown.TabIndex = 71;
            CloudRotationRateUpDown.TextAlign = HorizontalAlignment.Center;
            CloudRotationRateUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            CloudRotationRateUpDown.ValueChanged += CloudRotationRateUpDown_ValueChanged;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.BackColor = Color.Transparent;
            label32.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label32.ForeColor = SystemColors.ControlDarkDark;
            label32.Location = new Point(6, 137);
            label32.Name = "label32";
            label32.Size = new Size(93, 17);
            label32.TabIndex = 70;
            label32.Text = "Cloud Opacity";
            // 
            // CloudTextureOpacityTrack
            // 
            CloudTextureOpacityTrack.AutoSize = false;
            CloudTextureOpacityTrack.BackColor = SystemColors.Control;
            CloudTextureOpacityTrack.Location = new Point(6, 155);
            CloudTextureOpacityTrack.Maximum = 255;
            CloudTextureOpacityTrack.Name = "CloudTextureOpacityTrack";
            CloudTextureOpacityTrack.RightToLeft = RightToLeft.No;
            CloudTextureOpacityTrack.Size = new Size(182, 20);
            CloudTextureOpacityTrack.TabIndex = 69;
            CloudTextureOpacityTrack.TickStyle = TickStyle.None;
            CloudTextureOpacityTrack.Value = 64;
            CloudTextureOpacityTrack.Scroll += CloudTextureOpacityTrack_Scroll;
            // 
            // LoadCloudTextureButton
            // 
            LoadCloudTextureButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoadCloudTextureButton.ForeColor = SystemColors.ControlDarkDark;
            LoadCloudTextureButton.IconChar = FontAwesome.Sharp.IconChar.FileImage;
            LoadCloudTextureButton.IconColor = Color.Black;
            LoadCloudTextureButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LoadCloudTextureButton.IconSize = 18;
            LoadCloudTextureButton.Location = new Point(75, 101);
            LoadCloudTextureButton.Name = "LoadCloudTextureButton";
            LoadCloudTextureButton.Size = new Size(23, 23);
            LoadCloudTextureButton.TabIndex = 68;
            LoadCloudTextureButton.UseVisualStyleBackColor = true;
            LoadCloudTextureButton.Click += LoadCloudTextureButton_Click;
            // 
            // CustomCloudTextureRadio
            // 
            CustomCloudTextureRadio.AutoSize = true;
            CustomCloudTextureRadio.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CustomCloudTextureRadio.ForeColor = SystemColors.ControlDarkDark;
            CustomCloudTextureRadio.Location = new Point(6, 103);
            CustomCloudTextureRadio.Name = "CustomCloudTextureRadio";
            CustomCloudTextureRadio.Size = new Size(67, 19);
            CustomCloudTextureRadio.TabIndex = 67;
            CustomCloudTextureRadio.Text = "Custom";
            CustomCloudTextureRadio.UseVisualStyleBackColor = true;
            CustomCloudTextureRadio.CheckedChanged += CustomCloudTextureRadio_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(6, 58);
            label5.Name = "label5";
            label5.Size = new Size(92, 17);
            label5.TabIndex = 66;
            label5.Text = "Cloud Texture";
            // 
            // DefaultCloudTextureRadio
            // 
            DefaultCloudTextureRadio.AutoSize = true;
            DefaultCloudTextureRadio.Checked = true;
            DefaultCloudTextureRadio.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DefaultCloudTextureRadio.ForeColor = SystemColors.ControlDarkDark;
            DefaultCloudTextureRadio.Location = new Point(6, 78);
            DefaultCloudTextureRadio.Name = "DefaultCloudTextureRadio";
            DefaultCloudTextureRadio.Size = new Size(171, 19);
            DefaultCloudTextureRadio.TabIndex = 65;
            DefaultCloudTextureRadio.TabStop = true;
            DefaultCloudTextureRadio.Text = "Default (NASA Blue Marble)";
            DefaultCloudTextureRadio.UseVisualStyleBackColor = true;
            DefaultCloudTextureRadio.CheckedChanged += DefaultCloudTextureRadio_CheckedChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(47, 29);
            label4.Name = "label4";
            label4.Size = new Size(82, 15);
            label4.TabIndex = 64;
            label4.Text = "Enable Clouds";
            // 
            // EnableCloudsSwitch
            // 
            EnableCloudsSwitch.Alpha = 50;
            EnableCloudsSwitch.BackColor = Color.Transparent;
            EnableCloudsSwitch.Background = true;
            EnableCloudsSwitch.Background_WidthPen = 2F;
            EnableCloudsSwitch.BackgroundPen = false;
            EnableCloudsSwitch.Checked = false;
            EnableCloudsSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            EnableCloudsSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            EnableCloudsSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            EnableCloudsSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            EnableCloudsSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            EnableCloudsSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            EnableCloudsSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            EnableCloudsSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            EnableCloudsSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            EnableCloudsSwitch.ColorValue = Color.ForestGreen;
            EnableCloudsSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            EnableCloudsSwitch.Font = new Font("Arial", 11F);
            EnableCloudsSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            EnableCloudsSwitch.Lighting = true;
            EnableCloudsSwitch.LinearGradient_Background = false;
            EnableCloudsSwitch.LinearGradient_Value = false;
            EnableCloudsSwitch.LinearGradientPen = false;
            EnableCloudsSwitch.Location = new Point(6, 26);
            EnableCloudsSwitch.Name = "EnableCloudsSwitch";
            EnableCloudsSwitch.PenWidth = 10;
            EnableCloudsSwitch.RGB = false;
            EnableCloudsSwitch.Rounding = true;
            EnableCloudsSwitch.RoundingInt = 90;
            EnableCloudsSwitch.Size = new Size(35, 20);
            EnableCloudsSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            EnableCloudsSwitch.TabIndex = 63;
            EnableCloudsSwitch.Tag = "Cyber";
            EnableCloudsSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            EnableCloudsSwitch.Timer_RGB = 300;
            EnableCloudsSwitch.CheckedChanged += EnableCloudsSwitch_CheckedChanged;
            // 
            // ThreeDView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 744);
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
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            AnimationGroup.ResumeLayout(false);
            AnimationGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RotationRateUpDown).EndInit();
            CameraGroup.ResumeLayout(false);
            ModelGroup.ResumeLayout(false);
            TDContainerPanel.ResumeLayout(false);
            FeaturesPanel.ResumeLayout(false);
            MoonGroup.ResumeLayout(false);
            EffectsGroup.ResumeLayout(false);
            RingGroup.ResumeLayout(false);
            AtmosphereGroup.ResumeLayout(false);
            LocalStarGroup.ResumeLayout(false);
            LightingPanel.ResumeLayout(false);
            LightingGroup.ResumeLayout(false);
            LightingGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SunlightVerticalDirectionTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)SunlightHorizontalDirectionTrack).EndInit();
            CloudsPanel.ResumeLayout(false);
            CloudsGroup.ResumeLayout(false);
            CloudsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)CloudRotationRateUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)CloudTextureOpacityTrack).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm ThreeDViewOverlay;
        private Panel TDContainerPanel;
        private Button CloseFormButton;
        private GroupBox ModelGroup;
        private FontAwesome.Sharp.IconButton LoadModelButton;
        private FontAwesome.Sharp.IconButton SaveModelButton;
        private GroupBox CameraGroup;
        private FontAwesome.Sharp.IconButton ResetCameraButton;
        private ReaLTaiizor.Controls.ThunderControlBox ThreeDViewControlBox;
        private FontAwesome.Sharp.IconButton ChangeAxesButton;
        private Label ModelStatisticsLabel;
        private Label label1;
        internal Label AnimateRotationLabel;
        internal ReaLTaiizor.Controls.CyberSwitch EnableAnimationSwitch;
        private GroupBox groupBox2;
        private FontAwesome.Sharp.IconButton ClearBackgroundButton;
        private FontAwesome.Sharp.IconButton LoadBackgroundButton;
        private Label label2;
        private NumericUpDown RotationRateUpDown;
        private Label label3;
        private ComboBox FrameRateCombo;
        internal GroupBox AnimationGroup;
        private FontAwesome.Sharp.IconButton ShowHideCloudPanelButton;
        private Panel CloudsPanel;
        private GroupBox CloudsGroup;
        private Label label4;
        internal ReaLTaiizor.Controls.CyberSwitch EnableCloudsSwitch;
        private Label label5;
        private RadioButton DefaultCloudTextureRadio;
        private FontAwesome.Sharp.IconButton LoadCloudTextureButton;
        private RadioButton CustomCloudTextureRadio;
        private Label label32;
        internal TrackBar CloudTextureOpacityTrack;
        private NumericUpDown CloudRotationRateUpDown;
        private Label label6;
        internal FontAwesome.Sharp.IconButton SelectCloudColorButton;
        private FontAwesome.Sharp.IconButton ShowHideLightingPanelButton;
        private Panel LightingPanel;
        private GroupBox LightingGroup;
        private Panel FeaturesPanel;
        internal ReaLTaiizor.Controls.CyberSwitch EnableSunlightSwitch;
        private Label label7;
        internal FontAwesome.Sharp.IconButton SunlightColorButton;
        private Label label9;
        internal TrackBar SunlightVerticalDirectionTrack;
        private Label label8;
        internal TrackBar SunlightHorizontalDirectionTrack;
        private Label label10;
        private FontAwesome.Sharp.IconButton ShowHideFeaturesButton;
        private GroupBox LocalStarGroup;
        private FontAwesome.Sharp.IconButton ExpandStarGroup;
        private GroupBox AtmosphereGroup;
        private FontAwesome.Sharp.IconButton ExpandAtmosphereGroup;
        private GroupBox MoonGroup;
        private FontAwesome.Sharp.IconButton ExpandMoonGroup;
        private GroupBox EffectsGroup;
        private FontAwesome.Sharp.IconButton ExpandEffectsGroup;
        private GroupBox RingGroup;
        private FontAwesome.Sharp.IconButton ExpandRingGroup;
        private Label label12;
        internal FontAwesome.Sharp.IconButton AmbientLightColorButton;
        private Label label11;
        internal ReaLTaiizor.Controls.CyberSwitch EnableAmbientLightSwitch;
        private CheckBox ShowGridlinesCheck;
        private FontAwesome.Sharp.IconButton RecordButton;
    }
}