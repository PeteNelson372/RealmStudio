namespace RealmStudio
{
    partial class OpenCreateMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenCreateMap));
            OpenCreateMapOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CreateButton = new Button();
            CancelConfigButton = new Button();
            OpenButton = new Button();
            CreateNewMapGroupBox = new GroupBox();
            RealmNameTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            groupBox4 = new GroupBox();
            MapAreaHeightLabel = new Label();
            label5 = new Label();
            MapAreaUnitCombo = new ComboBox();
            label4 = new Label();
            label7 = new Label();
            MapAreaWidthUpDown = new NumericUpDown();
            groupBox5 = new GroupBox();
            MapThemeList = new ListBox();
            groupBox3 = new GroupBox();
            AspectRatioLabel = new Label();
            label3 = new Label();
            SwapResolutionButton = new FontAwesome.Sharp.IconButton();
            LockAspectRatioButton = new FontAwesome.Sharp.IconButton();
            label2 = new Label();
            label1 = new Label();
            WidthUpDown = new NumericUpDown();
            HeightUpDown = new NumericUpDown();
            groupBox2 = new GroupBox();
            WH7680x4320Radio = new RadioButton();
            WH7016x4960Radio = new RadioButton();
            WH4960x3508Radio = new RadioButton();
            WH3508x2480Radio = new RadioButton();
            WH2480x1754Radio = new RadioButton();
            WH1754x1240Radio = new RadioButton();
            WH3300x2250Radio = new RadioButton();
            WH1280x720Radio = new RadioButton();
            WH4096x2048Radio = new RadioButton();
            WH3840x2160Radio = new RadioButton();
            WH2048x1024Radio = new RadioButton();
            WH2560x1080Radio = new RadioButton();
            WH1920x1080Radio = new RadioButton();
            WH1600x1200Radio = new RadioButton();
            WH1280x1024Radio = new RadioButton();
            WH1024x768Radio = new RadioButton();
            RealmTypeGroupBox = new GroupBox();
            pictureBox8 = new PictureBox();
            CityRadioButton = new RadioButton();
            pictureBox7 = new PictureBox();
            OtherRadioButton = new RadioButton();
            pictureBox6 = new PictureBox();
            ShipRadioButton = new RadioButton();
            pictureBox5 = new PictureBox();
            SolarSystemRadioButton = new RadioButton();
            pictureBox4 = new PictureBox();
            DungeonRadioButton = new RadioButton();
            pictureBox3 = new PictureBox();
            InteriorRadioButton = new RadioButton();
            pictureBox2 = new PictureBox();
            RegionRadioButton = new RadioButton();
            pictureBox1 = new PictureBox();
            WorldRadioButton = new RadioButton();
            OpenExistingMapGroup = new GroupBox();
            ClearSelectedMapButton = new FontAwesome.Sharp.IconButton();
            label6 = new Label();
            OpenMapIconButton = new FontAwesome.Sharp.IconButton();
            MapInfoTextBox = new TextBox();
            MapFileListBox = new ListBox();
            OpenCreateMapOverlay.SuspendLayout();
            CreateNewMapGroupBox.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MapAreaWidthUpDown).BeginInit();
            groupBox5.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WidthUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HeightUpDown).BeginInit();
            groupBox2.SuspendLayout();
            RealmTypeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            OpenExistingMapGroup.SuspendLayout();
            SuspendLayout();
            // 
            // OpenCreateMapOverlay
            // 
            OpenCreateMapOverlay.BackColor = Color.FromArgb(244, 241, 243);
            OpenCreateMapOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            OpenCreateMapOverlay.Controls.Add(CreateButton);
            OpenCreateMapOverlay.Controls.Add(CancelConfigButton);
            OpenCreateMapOverlay.Controls.Add(OpenButton);
            OpenCreateMapOverlay.Controls.Add(CreateNewMapGroupBox);
            OpenCreateMapOverlay.Controls.Add(OpenExistingMapGroup);
            OpenCreateMapOverlay.Dock = DockStyle.Fill;
            OpenCreateMapOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            OpenCreateMapOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            OpenCreateMapOverlay.Font = new Font("Segoe UI", 9F);
            OpenCreateMapOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            OpenCreateMapOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            OpenCreateMapOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            OpenCreateMapOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            OpenCreateMapOverlay.Location = new Point(0, 0);
            OpenCreateMapOverlay.Name = "OpenCreateMapOverlay";
            OpenCreateMapOverlay.Padding = new Padding(20, 56, 20, 16);
            OpenCreateMapOverlay.RoundCorners = true;
            OpenCreateMapOverlay.Sizable = true;
            OpenCreateMapOverlay.Size = new Size(877, 748);
            OpenCreateMapOverlay.SmartBounds = true;
            OpenCreateMapOverlay.StartPosition = FormStartPosition.CenterScreen;
            OpenCreateMapOverlay.TabIndex = 0;
            OpenCreateMapOverlay.Text = "Create or Select Map";
            OpenCreateMapOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CreateButton
            // 
            CreateButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreateButton.ForeColor = SystemColors.ControlDarkDark;
            CreateButton.Location = new Point(509, 669);
            CreateButton.Name = "CreateButton";
            CreateButton.Size = new Size(60, 60);
            CreateButton.TabIndex = 26;
            CreateButton.Text = "&Create";
            CreateButton.UseVisualStyleBackColor = true;
            CreateButton.Click += CreateButton_Click;
            // 
            // CancelConfigButton
            // 
            CancelConfigButton.DialogResult = DialogResult.Cancel;
            CancelConfigButton.ForeColor = SystemColors.ControlDarkDark;
            CancelConfigButton.Location = new Point(794, 669);
            CancelConfigButton.Name = "CancelConfigButton";
            CancelConfigButton.Size = new Size(60, 60);
            CancelConfigButton.TabIndex = 25;
            CancelConfigButton.Text = "Ca&ncel";
            CancelConfigButton.UseVisualStyleBackColor = true;
            CancelConfigButton.Click += CancelConfigButton_Click;
            // 
            // OpenButton
            // 
            OpenButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OpenButton.ForeColor = SystemColors.ControlDarkDark;
            OpenButton.Location = new Point(728, 669);
            OpenButton.Name = "OpenButton";
            OpenButton.Size = new Size(60, 60);
            OpenButton.TabIndex = 3;
            OpenButton.Text = "&Open";
            OpenButton.UseVisualStyleBackColor = true;
            OpenButton.Click += OkayButton_Click;
            // 
            // CreateNewMapGroupBox
            // 
            CreateNewMapGroupBox.Controls.Add(RealmNameTextBox);
            CreateNewMapGroupBox.Controls.Add(groupBox4);
            CreateNewMapGroupBox.Controls.Add(groupBox5);
            CreateNewMapGroupBox.Controls.Add(groupBox3);
            CreateNewMapGroupBox.Controls.Add(groupBox2);
            CreateNewMapGroupBox.Controls.Add(RealmTypeGroupBox);
            CreateNewMapGroupBox.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CreateNewMapGroupBox.Location = new Point(23, 59);
            CreateNewMapGroupBox.Name = "CreateNewMapGroupBox";
            CreateNewMapGroupBox.Size = new Size(480, 670);
            CreateNewMapGroupBox.TabIndex = 2;
            CreateNewMapGroupBox.TabStop = false;
            CreateNewMapGroupBox.Text = "Create New Map";
            // 
            // RealmNameTextBox
            // 
            // 
            // 
            // 
            RealmNameTextBox.CustomButton.Image = null;
            RealmNameTextBox.CustomButton.Location = new Point(425, 1);
            RealmNameTextBox.CustomButton.Name = "";
            RealmNameTextBox.CustomButton.Size = new Size(27, 27);
            RealmNameTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            RealmNameTextBox.CustomButton.TabIndex = 1;
            RealmNameTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            RealmNameTextBox.CustomButton.UseSelectable = true;
            RealmNameTextBox.CustomButton.Visible = false;
            RealmNameTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
            RealmNameTextBox.Location = new Point(6, 27);
            RealmNameTextBox.MaxLength = 32767;
            RealmNameTextBox.Name = "RealmNameTextBox";
            RealmNameTextBox.PasswordChar = '\0';
            RealmNameTextBox.PromptText = "Enter Realm Name";
            RealmNameTextBox.ScrollBars = ScrollBars.None;
            RealmNameTextBox.SelectedText = "";
            RealmNameTextBox.SelectionLength = 0;
            RealmNameTextBox.SelectionStart = 0;
            RealmNameTextBox.ShortcutsEnabled = true;
            RealmNameTextBox.Size = new Size(453, 29);
            RealmNameTextBox.TabIndex = 23;
            RealmNameTextBox.UseSelectable = true;
            RealmNameTextBox.WaterMark = "Enter Realm Name";
            RealmNameTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            RealmNameTextBox.WaterMarkFont = new Font("Segoe UI", 9.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(MapAreaHeightLabel);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(MapAreaUnitCombo);
            groupBox4.Controls.Add(label4);
            groupBox4.Controls.Add(label7);
            groupBox4.Controls.Add(MapAreaWidthUpDown);
            groupBox4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox4.Location = new Point(6, 534);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(223, 117);
            groupBox4.TabIndex = 22;
            groupBox4.TabStop = false;
            groupBox4.Text = "Realm Map Area";
            // 
            // MapAreaHeightLabel
            // 
            MapAreaHeightLabel.ForeColor = SystemColors.ControlDarkDark;
            MapAreaHeightLabel.Location = new Point(114, 81);
            MapAreaHeightLabel.Name = "MapAreaHeightLabel";
            MapAreaHeightLabel.Size = new Size(103, 15);
            MapAreaHeightLabel.TabIndex = 6;
            MapAreaHeightLabel.Text = "75";
            MapAreaHeightLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(15, 81);
            label5.Name = "label5";
            label5.Size = new Size(97, 15);
            label5.TabIndex = 5;
            label5.Text = "Map Area Height";
            // 
            // MapAreaUnitCombo
            // 
            MapAreaUnitCombo.DropDownWidth = 120;
            MapAreaUnitCombo.FormattingEnabled = true;
            MapAreaUnitCombo.Items.AddRange(new object[] { "Centimeters", "Inches", "Feet", "Yards", "Meters", "Kilometers", "Miles", "Astronomical Units (AU)", "Light Years", "Parsecs" });
            MapAreaUnitCombo.Location = new Point(114, 26);
            MapAreaUnitCombo.Name = "MapAreaUnitCombo";
            MapAreaUnitCombo.Size = new Size(103, 23);
            MapAreaUnitCombo.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(20, 26);
            label4.Name = "label4";
            label4.Size = new Size(88, 15);
            label4.TabIndex = 3;
            label4.Text = "Map Area Units";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = SystemColors.ControlDarkDark;
            label7.Location = new Point(15, 57);
            label7.Name = "label7";
            label7.Size = new Size(94, 15);
            label7.TabIndex = 2;
            label7.Text = "Map Area Width";
            // 
            // MapAreaWidthUpDown
            // 
            MapAreaWidthUpDown.Location = new Point(114, 55);
            MapAreaWidthUpDown.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            MapAreaWidthUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            MapAreaWidthUpDown.Name = "MapAreaWidthUpDown";
            MapAreaWidthUpDown.Size = new Size(103, 23);
            MapAreaWidthUpDown.TabIndex = 1;
            MapAreaWidthUpDown.TextAlign = HorizontalAlignment.Center;
            MapAreaWidthUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
            MapAreaWidthUpDown.ValueChanged += MapAreaWidthUpDown_ValueChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(MapThemeList);
            groupBox5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox5.Location = new Point(6, 412);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(223, 116);
            groupBox5.TabIndex = 21;
            groupBox5.TabStop = false;
            groupBox5.Text = "Realm Map Theme";
            // 
            // MapThemeList
            // 
            MapThemeList.FormattingEnabled = true;
            MapThemeList.Location = new Point(6, 22);
            MapThemeList.Name = "MapThemeList";
            MapThemeList.Size = new Size(211, 79);
            MapThemeList.TabIndex = 0;
            MapThemeList.SelectedIndexChanged += MapThemeList_SelectedIndexChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(AspectRatioLabel);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(SwapResolutionButton);
            groupBox3.Controls.Add(LockAspectRatioButton);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(WidthUpDown);
            groupBox3.Controls.Add(HeightUpDown);
            groupBox3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox3.Location = new Point(246, 76);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(213, 116);
            groupBox3.TabIndex = 19;
            groupBox3.TabStop = false;
            groupBox3.Text = "Realm Map Size";
            // 
            // AspectRatioLabel
            // 
            AspectRatioLabel.ForeColor = SystemColors.ControlDarkDark;
            AspectRatioLabel.Location = new Point(90, 87);
            AspectRatioLabel.Name = "AspectRatioLabel";
            AspectRatioLabel.Size = new Size(100, 15);
            AspectRatioLabel.TabIndex = 7;
            AspectRatioLabel.Text = "1.78";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(11, 87);
            label3.Name = "label3";
            label3.Size = new Size(73, 15);
            label3.TabIndex = 6;
            label3.Text = "Aspect Ratio";
            // 
            // SwapResolutionButton
            // 
            SwapResolutionButton.FlatStyle = FlatStyle.Flat;
            SwapResolutionButton.IconChar = FontAwesome.Sharp.IconChar.ArrowRightArrowLeft;
            SwapResolutionButton.IconColor = Color.Black;
            SwapResolutionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SwapResolutionButton.IconSize = 20;
            SwapResolutionButton.Location = new Point(148, 47);
            SwapResolutionButton.Name = "SwapResolutionButton";
            SwapResolutionButton.Rotation = 90D;
            SwapResolutionButton.Size = new Size(25, 25);
            SwapResolutionButton.TabIndex = 5;
            SwapResolutionButton.UseVisualStyleBackColor = true;
            SwapResolutionButton.Click += SwapResolutionButton_Click;
            // 
            // LockAspectRatioButton
            // 
            LockAspectRatioButton.FlatStyle = FlatStyle.Flat;
            LockAspectRatioButton.IconChar = FontAwesome.Sharp.IconChar.Lock;
            LockAspectRatioButton.IconColor = Color.Black;
            LockAspectRatioButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LockAspectRatioButton.IconSize = 20;
            LockAspectRatioButton.Location = new Point(148, 17);
            LockAspectRatioButton.Name = "LockAspectRatioButton";
            LockAspectRatioButton.Size = new Size(25, 25);
            LockAspectRatioButton.TabIndex = 4;
            LockAspectRatioButton.UseVisualStyleBackColor = true;
            LockAspectRatioButton.Click += LockAspectRatioButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(6, 53);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 3;
            label2.Text = "Height";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(6, 26);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 2;
            label1.Text = "Width";
            // 
            // WidthUpDown
            // 
            WidthUpDown.Location = new Point(55, 22);
            WidthUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            WidthUpDown.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            WidthUpDown.Name = "WidthUpDown";
            WidthUpDown.Size = new Size(87, 23);
            WidthUpDown.TabIndex = 1;
            WidthUpDown.TextAlign = HorizontalAlignment.Center;
            WidthUpDown.Value = new decimal(new int[] { 1920, 0, 0, 0 });
            WidthUpDown.ValueChanged += WidthUpDown_ValueChanged;
            // 
            // HeightUpDown
            // 
            HeightUpDown.Location = new Point(55, 51);
            HeightUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            HeightUpDown.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            HeightUpDown.Name = "HeightUpDown";
            HeightUpDown.Size = new Size(87, 23);
            HeightUpDown.TabIndex = 0;
            HeightUpDown.TextAlign = HorizontalAlignment.Center;
            HeightUpDown.Value = new decimal(new int[] { 1080, 0, 0, 0 });
            HeightUpDown.ValueChanged += HeightUpDown_ValueChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(WH7680x4320Radio);
            groupBox2.Controls.Add(WH7016x4960Radio);
            groupBox2.Controls.Add(WH4960x3508Radio);
            groupBox2.Controls.Add(WH3508x2480Radio);
            groupBox2.Controls.Add(WH2480x1754Radio);
            groupBox2.Controls.Add(WH1754x1240Radio);
            groupBox2.Controls.Add(WH3300x2250Radio);
            groupBox2.Controls.Add(WH1280x720Radio);
            groupBox2.Controls.Add(WH4096x2048Radio);
            groupBox2.Controls.Add(WH3840x2160Radio);
            groupBox2.Controls.Add(WH2048x1024Radio);
            groupBox2.Controls.Add(WH2560x1080Radio);
            groupBox2.Controls.Add(WH1920x1080Radio);
            groupBox2.Controls.Add(WH1600x1200Radio);
            groupBox2.Controls.Add(WH1280x1024Radio);
            groupBox2.Controls.Add(WH1024x768Radio);
            groupBox2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(246, 198);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(213, 453);
            groupBox2.TabIndex = 11;
            groupBox2.TabStop = false;
            groupBox2.Text = "Realm Map Size Presets";
            // 
            // WH7680x4320Radio
            // 
            WH7680x4320Radio.AutoSize = true;
            WH7680x4320Radio.ForeColor = SystemColors.ControlDarkDark;
            WH7680x4320Radio.Location = new Point(6, 397);
            WH7680x4320Radio.Name = "WH7680x4320Radio";
            WH7680x4320Radio.Size = new Size(146, 19);
            WH7680x4320Radio.TabIndex = 17;
            WH7680x4320Radio.Text = "7680 x 4320 (8K UHD)";
            WH7680x4320Radio.UseVisualStyleBackColor = true;
            WH7680x4320Radio.Click += WH7680x4320Radio_Click;
            // 
            // WH7016x4960Radio
            // 
            WH7016x4960Radio.AutoSize = true;
            WH7016x4960Radio.ForeColor = SystemColors.ControlDarkDark;
            WH7016x4960Radio.Location = new Point(6, 372);
            WH7016x4960Radio.Name = "WH7016x4960Radio";
            WH7016x4960Radio.Size = new Size(163, 19);
            WH7016x4960Radio.TabIndex = 16;
            WH7016x4960Radio.Text = "7016 x 4960 (A2 300 DPI)";
            WH7016x4960Radio.UseVisualStyleBackColor = true;
            WH7016x4960Radio.Click += WH7016x4960Radio_Click;
            // 
            // WH4960x3508Radio
            // 
            WH4960x3508Radio.AutoSize = true;
            WH4960x3508Radio.ForeColor = SystemColors.ControlDarkDark;
            WH4960x3508Radio.Location = new Point(6, 347);
            WH4960x3508Radio.Name = "WH4960x3508Radio";
            WH4960x3508Radio.Size = new Size(166, 19);
            WH4960x3508Radio.TabIndex = 15;
            WH4960x3508Radio.Text = "4960 x 3508 (A3 300 DPI)";
            WH4960x3508Radio.UseVisualStyleBackColor = true;
            WH4960x3508Radio.Click += WH4960x3508Radio_Click;
            // 
            // WH3508x2480Radio
            // 
            WH3508x2480Radio.AutoSize = true;
            WH3508x2480Radio.ForeColor = SystemColors.ControlDarkDark;
            WH3508x2480Radio.Location = new Point(6, 322);
            WH3508x2480Radio.Name = "WH3508x2480Radio";
            WH3508x2480Radio.Size = new Size(166, 19);
            WH3508x2480Radio.TabIndex = 14;
            WH3508x2480Radio.Text = "3508 x 2480 (A4 300 DPI)";
            WH3508x2480Radio.UseVisualStyleBackColor = true;
            WH3508x2480Radio.Click += WH3508x2480Radio_Click;
            // 
            // WH2480x1754Radio
            // 
            WH2480x1754Radio.AutoSize = true;
            WH2480x1754Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2480x1754Radio.Location = new Point(6, 297);
            WH2480x1754Radio.Name = "WH2480x1754Radio";
            WH2480x1754Radio.Size = new Size(163, 19);
            WH2480x1754Radio.TabIndex = 13;
            WH2480x1754Radio.Text = "2480 x 1754 (A5 300 DPI)";
            WH2480x1754Radio.UseVisualStyleBackColor = true;
            WH2480x1754Radio.Click += WH2480x1754Radio_Click;
            // 
            // WH1754x1240Radio
            // 
            WH1754x1240Radio.AutoSize = true;
            WH1754x1240Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1754x1240Radio.Location = new Point(6, 272);
            WH1754x1240Radio.Name = "WH1754x1240Radio";
            WH1754x1240Radio.Size = new Size(161, 19);
            WH1754x1240Radio.TabIndex = 12;
            WH1754x1240Radio.Text = "1754 x 1240 (A6 300 DPI)";
            WH1754x1240Radio.UseVisualStyleBackColor = true;
            WH1754x1240Radio.Click += WH1754x1240Radio_Click;
            // 
            // WH3300x2250Radio
            // 
            WH3300x2250Radio.AutoSize = true;
            WH3300x2250Radio.ForeColor = SystemColors.ControlDarkDark;
            WH3300x2250Radio.Location = new Point(6, 247);
            WH3300x2250Radio.Name = "WH3300x2250Radio";
            WH3300x2250Radio.Size = new Size(199, 19);
            WH3300x2250Radio.TabIndex = 11;
            WH3300x2250Radio.Text = "3300 x 2250 (US Letter 300 DPI)";
            WH3300x2250Radio.UseVisualStyleBackColor = true;
            WH3300x2250Radio.Click += WH3300x2250Radio_Click;
            // 
            // WH1280x720Radio
            // 
            WH1280x720Radio.AutoSize = true;
            WH1280x720Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1280x720Radio.Location = new Point(6, 47);
            WH1280x720Radio.Name = "WH1280x720Radio";
            WH1280x720Radio.Size = new Size(121, 19);
            WH1280x720Radio.TabIndex = 10;
            WH1280x720Radio.Text = "1280 x 720 (720P)";
            WH1280x720Radio.UseVisualStyleBackColor = true;
            WH1280x720Radio.Click += WH1280x720Radio_Click;
            // 
            // WH4096x2048Radio
            // 
            WH4096x2048Radio.AutoSize = true;
            WH4096x2048Radio.ForeColor = SystemColors.ControlDarkDark;
            WH4096x2048Radio.Location = new Point(6, 222);
            WH4096x2048Radio.Name = "WH4096x2048Radio";
            WH4096x2048Radio.Size = new Size(204, 19);
            WH4096x2048Radio.TabIndex = 9;
            WH4096x2048Radio.Text = "4096 x 2048 (Equirectangular 4K)";
            WH4096x2048Radio.UseVisualStyleBackColor = true;
            WH4096x2048Radio.Click += WH4096x2048Radio_Click;
            // 
            // WH3840x2160Radio
            // 
            WH3840x2160Radio.AutoSize = true;
            WH3840x2160Radio.ForeColor = SystemColors.ControlDarkDark;
            WH3840x2160Radio.Location = new Point(6, 197);
            WH3840x2160Radio.Name = "WH3840x2160Radio";
            WH3840x2160Radio.Size = new Size(165, 19);
            WH3840x2160Radio.TabIndex = 8;
            WH3840x2160Radio.Text = "3840 x 2160 (4K Ultra HD)";
            WH3840x2160Radio.UseVisualStyleBackColor = true;
            WH3840x2160Radio.Click += WH3840x2160Radio_Click;
            // 
            // WH2048x1024Radio
            // 
            WH2048x1024Radio.AutoSize = true;
            WH2048x1024Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2048x1024Radio.Location = new Point(6, 172);
            WH2048x1024Radio.Name = "WH2048x1024Radio";
            WH2048x1024Radio.Size = new Size(202, 19);
            WH2048x1024Radio.TabIndex = 7;
            WH2048x1024Radio.Text = "2048 x 1024 (Equirectangular 2K)";
            WH2048x1024Radio.UseVisualStyleBackColor = true;
            WH2048x1024Radio.Click += WH2048x1024Radio_Click;
            // 
            // WH2560x1080Radio
            // 
            WH2560x1080Radio.AutoSize = true;
            WH2560x1080Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2560x1080Radio.Location = new Point(6, 147);
            WH2560x1080Radio.Name = "WH2560x1080Radio";
            WH2560x1080Radio.Size = new Size(116, 19);
            WH2560x1080Radio.TabIndex = 6;
            WH2560x1080Radio.Text = "2560 x 1080 (2K)";
            WH2560x1080Radio.UseVisualStyleBackColor = true;
            WH2560x1080Radio.Click += WH2560x1080Radio_Click;
            // 
            // WH1920x1080Radio
            // 
            WH1920x1080Radio.AutoSize = true;
            WH1920x1080Radio.Checked = true;
            WH1920x1080Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1920x1080Radio.Location = new Point(6, 122);
            WH1920x1080Radio.Name = "WH1920x1080Radio";
            WH1920x1080Radio.Size = new Size(176, 19);
            WH1920x1080Radio.TabIndex = 5;
            WH1920x1080Radio.TabStop = true;
            WH1920x1080Radio.Text = "1920 x 1080 (1080P Full HD)";
            WH1920x1080Radio.UseVisualStyleBackColor = true;
            WH1920x1080Radio.Click += WH1920x1080Radio_Click;
            // 
            // WH1600x1200Radio
            // 
            WH1600x1200Radio.AutoSize = true;
            WH1600x1200Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1600x1200Radio.Location = new Point(6, 97);
            WH1600x1200Radio.Name = "WH1600x1200Radio";
            WH1600x1200Radio.Size = new Size(131, 19);
            WH1600x1200Radio.TabIndex = 4;
            WH1600x1200Radio.Text = "1600 x 1200 (UXGA)";
            WH1600x1200Radio.UseVisualStyleBackColor = true;
            WH1600x1200Radio.Click += WH1600x1200Radio_Click;
            // 
            // WH1280x1024Radio
            // 
            WH1280x1024Radio.AutoSize = true;
            WH1280x1024Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1280x1024Radio.Location = new Point(6, 72);
            WH1280x1024Radio.Name = "WH1280x1024Radio";
            WH1280x1024Radio.Size = new Size(130, 19);
            WH1280x1024Radio.TabIndex = 3;
            WH1280x1024Radio.Text = "1280 x 1024 (SXGA)";
            WH1280x1024Radio.UseVisualStyleBackColor = true;
            WH1280x1024Radio.Click += WH1280x1024Radio_Click;
            // 
            // WH1024x768Radio
            // 
            WH1024x768Radio.AutoSize = true;
            WH1024x768Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1024x768Radio.Location = new Point(6, 22);
            WH1024x768Radio.Name = "WH1024x768Radio";
            WH1024x768Radio.Size = new Size(117, 19);
            WH1024x768Radio.TabIndex = 2;
            WH1024x768Radio.Text = "1024 x 768 (XGA)";
            WH1024x768Radio.UseVisualStyleBackColor = true;
            WH1024x768Radio.Click += WH1024x768Radio_Click;
            // 
            // RealmTypeGroupBox
            // 
            RealmTypeGroupBox.Controls.Add(pictureBox8);
            RealmTypeGroupBox.Controls.Add(CityRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox7);
            RealmTypeGroupBox.Controls.Add(OtherRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox6);
            RealmTypeGroupBox.Controls.Add(ShipRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox5);
            RealmTypeGroupBox.Controls.Add(SolarSystemRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox4);
            RealmTypeGroupBox.Controls.Add(DungeonRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox3);
            RealmTypeGroupBox.Controls.Add(InteriorRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox2);
            RealmTypeGroupBox.Controls.Add(RegionRadioButton);
            RealmTypeGroupBox.Controls.Add(pictureBox1);
            RealmTypeGroupBox.Controls.Add(WorldRadioButton);
            RealmTypeGroupBox.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RealmTypeGroupBox.Location = new Point(6, 76);
            RealmTypeGroupBox.Name = "RealmTypeGroupBox";
            RealmTypeGroupBox.Size = new Size(223, 330);
            RealmTypeGroupBox.TabIndex = 2;
            RealmTypeGroupBox.TabStop = false;
            RealmTypeGroupBox.Text = "Realm Type";
            // 
            // pictureBox8
            // 
            pictureBox8.BackgroundImage = (Image)resources.GetObject("pictureBox8.BackgroundImage");
            pictureBox8.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox8.Location = new Point(6, 98);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(32, 32);
            pictureBox8.TabIndex = 15;
            pictureBox8.TabStop = false;
            // 
            // CityRadioButton
            // 
            CityRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            CityRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CityRadioButton.ForeColor = SystemColors.ControlDarkDark;
            CityRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            CityRadioButton.Location = new Point(46, 98);
            CityRadioButton.Name = "CityRadioButton";
            CityRadioButton.Size = new Size(124, 32);
            CityRadioButton.TabIndex = 14;
            CityRadioButton.TabStop = true;
            CityRadioButton.Text = "City, Town, Village";
            CityRadioButton.UseVisualStyleBackColor = true;
            CityRadioButton.Click += CityRadioButton_Click;
            // 
            // pictureBox7
            // 
            pictureBox7.BackgroundImage = (Image)resources.GetObject("pictureBox7.BackgroundImage");
            pictureBox7.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox7.Location = new Point(6, 288);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(32, 32);
            pictureBox7.TabIndex = 13;
            pictureBox7.TabStop = false;
            // 
            // OtherRadioButton
            // 
            OtherRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            OtherRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            OtherRadioButton.ForeColor = SystemColors.ControlDarkDark;
            OtherRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            OtherRadioButton.Location = new Point(46, 288);
            OtherRadioButton.Name = "OtherRadioButton";
            OtherRadioButton.Size = new Size(110, 32);
            OtherRadioButton.TabIndex = 12;
            OtherRadioButton.TabStop = true;
            OtherRadioButton.Text = "Other";
            OtherRadioButton.UseVisualStyleBackColor = true;
            OtherRadioButton.Click += OtherRadioButton_Click;
            // 
            // pictureBox6
            // 
            pictureBox6.BackgroundImage = (Image)resources.GetObject("pictureBox6.BackgroundImage");
            pictureBox6.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox6.Location = new Point(6, 250);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(32, 32);
            pictureBox6.TabIndex = 11;
            pictureBox6.TabStop = false;
            // 
            // ShipRadioButton
            // 
            ShipRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            ShipRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ShipRadioButton.ForeColor = SystemColors.ControlDarkDark;
            ShipRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            ShipRadioButton.Location = new Point(46, 250);
            ShipRadioButton.Name = "ShipRadioButton";
            ShipRadioButton.Size = new Size(110, 32);
            ShipRadioButton.TabIndex = 10;
            ShipRadioButton.TabStop = true;
            ShipRadioButton.Text = "Ship";
            ShipRadioButton.UseVisualStyleBackColor = true;
            ShipRadioButton.Click += ShipRadioButton_Click;
            // 
            // pictureBox5
            // 
            pictureBox5.BackgroundImage = (Image)resources.GetObject("pictureBox5.BackgroundImage");
            pictureBox5.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox5.Location = new Point(6, 212);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(32, 32);
            pictureBox5.TabIndex = 9;
            pictureBox5.TabStop = false;
            // 
            // SolarSystemRadioButton
            // 
            SolarSystemRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            SolarSystemRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SolarSystemRadioButton.ForeColor = SystemColors.ControlDarkDark;
            SolarSystemRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            SolarSystemRadioButton.Location = new Point(46, 212);
            SolarSystemRadioButton.Name = "SolarSystemRadioButton";
            SolarSystemRadioButton.Size = new Size(110, 32);
            SolarSystemRadioButton.TabIndex = 8;
            SolarSystemRadioButton.TabStop = true;
            SolarSystemRadioButton.Text = "Solar System";
            SolarSystemRadioButton.UseVisualStyleBackColor = true;
            SolarSystemRadioButton.Click += SolarSystemRadioButton_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.BackgroundImage = (Image)resources.GetObject("pictureBox4.BackgroundImage");
            pictureBox4.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox4.Location = new Point(6, 174);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(32, 32);
            pictureBox4.TabIndex = 7;
            pictureBox4.TabStop = false;
            // 
            // DungeonRadioButton
            // 
            DungeonRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            DungeonRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DungeonRadioButton.ForeColor = SystemColors.ControlDarkDark;
            DungeonRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            DungeonRadioButton.Location = new Point(46, 174);
            DungeonRadioButton.Name = "DungeonRadioButton";
            DungeonRadioButton.Size = new Size(87, 32);
            DungeonRadioButton.TabIndex = 6;
            DungeonRadioButton.TabStop = true;
            DungeonRadioButton.Text = "Dungeon";
            DungeonRadioButton.UseVisualStyleBackColor = true;
            DungeonRadioButton.Click += DungeonRadioButton_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox3.Location = new Point(6, 136);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(32, 32);
            pictureBox3.TabIndex = 5;
            pictureBox3.TabStop = false;
            // 
            // InteriorRadioButton
            // 
            InteriorRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            InteriorRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            InteriorRadioButton.ForeColor = SystemColors.ControlDarkDark;
            InteriorRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            InteriorRadioButton.Location = new Point(46, 136);
            InteriorRadioButton.Name = "InteriorRadioButton";
            InteriorRadioButton.Size = new Size(76, 32);
            InteriorRadioButton.TabIndex = 4;
            InteriorRadioButton.TabStop = true;
            InteriorRadioButton.Text = "Interior";
            InteriorRadioButton.UseVisualStyleBackColor = true;
            InteriorRadioButton.Click += InteriorRadioButton_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox2.Location = new Point(6, 60);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(32, 32);
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // RegionRadioButton
            // 
            RegionRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            RegionRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RegionRadioButton.ForeColor = SystemColors.ControlDarkDark;
            RegionRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            RegionRadioButton.Location = new Point(46, 60);
            RegionRadioButton.Name = "RegionRadioButton";
            RegionRadioButton.Size = new Size(76, 32);
            RegionRadioButton.TabIndex = 2;
            RegionRadioButton.TabStop = true;
            RegionRadioButton.Text = "Region";
            RegionRadioButton.UseVisualStyleBackColor = true;
            RegionRadioButton.Click += RegionRadioButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(6, 22);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(32, 32);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // WorldRadioButton
            // 
            WorldRadioButton.BackgroundImageLayout = ImageLayout.Zoom;
            WorldRadioButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            WorldRadioButton.ForeColor = SystemColors.ControlDarkDark;
            WorldRadioButton.ImageAlign = ContentAlignment.MiddleLeft;
            WorldRadioButton.Location = new Point(46, 22);
            WorldRadioButton.Name = "WorldRadioButton";
            WorldRadioButton.Size = new Size(67, 32);
            WorldRadioButton.TabIndex = 0;
            WorldRadioButton.TabStop = true;
            WorldRadioButton.Text = "World";
            WorldRadioButton.UseVisualStyleBackColor = true;
            WorldRadioButton.Click += WorldRadioButton_Click;
            // 
            // OpenExistingMapGroup
            // 
            OpenExistingMapGroup.Controls.Add(ClearSelectedMapButton);
            OpenExistingMapGroup.Controls.Add(label6);
            OpenExistingMapGroup.Controls.Add(OpenMapIconButton);
            OpenExistingMapGroup.Controls.Add(MapInfoTextBox);
            OpenExistingMapGroup.Controls.Add(MapFileListBox);
            OpenExistingMapGroup.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            OpenExistingMapGroup.Location = new Point(509, 59);
            OpenExistingMapGroup.Name = "OpenExistingMapGroup";
            OpenExistingMapGroup.Size = new Size(345, 604);
            OpenExistingMapGroup.TabIndex = 0;
            OpenExistingMapGroup.TabStop = false;
            OpenExistingMapGroup.Text = "Open Existing Map";
            // 
            // ClearSelectedMapButton
            // 
            ClearSelectedMapButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ClearSelectedMapButton.ForeColor = SystemColors.ControlDarkDark;
            ClearSelectedMapButton.IconChar = FontAwesome.Sharp.IconChar.Square;
            ClearSelectedMapButton.IconColor = Color.Black;
            ClearSelectedMapButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ClearSelectedMapButton.IconSize = 24;
            ClearSelectedMapButton.Location = new Point(279, 535);
            ClearSelectedMapButton.Name = "ClearSelectedMapButton";
            ClearSelectedMapButton.Size = new Size(60, 60);
            ClearSelectedMapButton.TabIndex = 5;
            ClearSelectedMapButton.Text = "Clear";
            ClearSelectedMapButton.TextAlign = ContentAlignment.TopCenter;
            ClearSelectedMapButton.TextImageRelation = TextImageRelation.TextAboveImage;
            ClearSelectedMapButton.UseVisualStyleBackColor = true;
            ClearSelectedMapButton.Click += ClearSelectedMapButton_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(6, 470);
            label6.Name = "label6";
            label6.Size = new Size(89, 17);
            label6.TabIndex = 4;
            label6.Text = "Selected Map";
            // 
            // OpenMapIconButton
            // 
            OpenMapIconButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OpenMapIconButton.ForeColor = SystemColors.ControlDarkDark;
            OpenMapIconButton.IconChar = FontAwesome.Sharp.IconChar.File;
            OpenMapIconButton.IconColor = Color.Black;
            OpenMapIconButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            OpenMapIconButton.IconSize = 20;
            OpenMapIconButton.Location = new Point(279, 469);
            OpenMapIconButton.Name = "OpenMapIconButton";
            OpenMapIconButton.Size = new Size(60, 60);
            OpenMapIconButton.TabIndex = 3;
            OpenMapIconButton.Text = "Select Map";
            OpenMapIconButton.TextAlign = ContentAlignment.TopCenter;
            OpenMapIconButton.TextImageRelation = TextImageRelation.TextAboveImage;
            OpenMapIconButton.UseVisualStyleBackColor = true;
            OpenMapIconButton.Click += OpenMapIconButton_Click;
            // 
            // MapInfoTextBox
            // 
            MapInfoTextBox.BorderStyle = BorderStyle.FixedSingle;
            MapInfoTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapInfoTextBox.Location = new Point(6, 495);
            MapInfoTextBox.Multiline = true;
            MapInfoTextBox.Name = "MapInfoTextBox";
            MapInfoTextBox.ReadOnly = true;
            MapInfoTextBox.ScrollBars = ScrollBars.Both;
            MapInfoTextBox.Size = new Size(267, 94);
            MapInfoTextBox.TabIndex = 2;
            // 
            // MapFileListBox
            // 
            MapFileListBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapFileListBox.FormattingEnabled = true;
            MapFileListBox.Location = new Point(6, 27);
            MapFileListBox.Name = "MapFileListBox";
            MapFileListBox.Size = new Size(333, 424);
            MapFileListBox.TabIndex = 0;
            MapFileListBox.SelectedIndexChanged += MapFileListBox_SelectedIndexChanged;
            // 
            // OpenCreateMap
            // 
            AcceptButton = OpenButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CancelConfigButton;
            ClientSize = new Size(877, 748);
            Controls.Add(OpenCreateMapOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "OpenCreateMap";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Create or Select Map";
            TransparencyKey = Color.Fuchsia;
            Shown += OpenCreateMap_Shown;
            OpenCreateMapOverlay.ResumeLayout(false);
            CreateNewMapGroupBox.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MapAreaWidthUpDown).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)WidthUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)HeightUpDown).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            RealmTypeGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            OpenExistingMapGroup.ResumeLayout(false);
            OpenExistingMapGroup.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm OpenCreateMapOverlay;
        private GroupBox OpenExistingMapGroup;
        private ListBox MapFileListBox;
        private GroupBox CreateNewMapGroupBox;
        private GroupBox RealmTypeGroupBox;
        private RadioButton WorldRadioButton;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private RadioButton RegionRadioButton;
        private PictureBox pictureBox3;
        private RadioButton InteriorRadioButton;
        private PictureBox pictureBox4;
        private RadioButton DungeonRadioButton;
        private PictureBox pictureBox5;
        private RadioButton SolarSystemRadioButton;
        private PictureBox pictureBox6;
        private RadioButton ShipRadioButton;
        private PictureBox pictureBox7;
        private RadioButton OtherRadioButton;
        private GroupBox groupBox2;
        private RadioButton WH7680x4320Radio;
        private RadioButton WH7016x4960Radio;
        private RadioButton WH4960x3508Radio;
        private RadioButton WH3508x2480Radio;
        private RadioButton WH2480x1754Radio;
        private RadioButton WH1754x1240Radio;
        private RadioButton WH3300x2250Radio;
        private RadioButton WH1280x720Radio;
        private RadioButton WH4096x2048Radio;
        private RadioButton WH3840x2160Radio;
        private RadioButton WH2048x1024Radio;
        private RadioButton WH2560x1080Radio;
        private RadioButton WH1920x1080Radio;
        private RadioButton WH1600x1200Radio;
        private RadioButton WH1280x1024Radio;
        private RadioButton WH1024x768Radio;
        private GroupBox groupBox3;
        private Label AspectRatioLabel;
        private Label label3;
        private FontAwesome.Sharp.IconButton SwapResolutionButton;
        private FontAwesome.Sharp.IconButton LockAspectRatioButton;
        private Label label2;
        private Label label1;
        private NumericUpDown WidthUpDown;
        private NumericUpDown HeightUpDown;
        private GroupBox groupBox5;
        private ListBox MapThemeList;
        private Button OpenButton;
        private GroupBox groupBox4;
        private Label MapAreaHeightLabel;
        private Label label5;
        private ComboBox MapAreaUnitCombo;
        private Label label4;
        private Label label7;
        private NumericUpDown MapAreaWidthUpDown;
        private ReaLTaiizor.Controls.PoisonTextBox RealmNameTextBox;
        private PictureBox pictureBox8;
        private RadioButton CityRadioButton;
        private TextBox MapInfoTextBox;
        private FontAwesome.Sharp.IconButton OpenMapIconButton;
        private Label label6;
        private Button CancelConfigButton;
        private FontAwesome.Sharp.IconButton ClearSelectedMapButton;
        private Button CreateButton;
    }
}