namespace RealmStudio
{
    partial class RealmConfiguration
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
            RealmConfigDialog = new ReaLTaiizor.Forms.DungeonForm();
            CancelConfigButton = new FontAwesome.Sharp.IconButton();
            OkayButton = new FontAwesome.Sharp.IconButton();
            groupBox5 = new GroupBox();
            dungeonListBox1 = new ReaLTaiizor.Controls.DungeonListBox();
            groupBox4 = new GroupBox();
            label6 = new Label();
            label5 = new Label();
            comboBox1 = new ComboBox();
            label4 = new Label();
            label7 = new Label();
            numericUpDown1 = new NumericUpDown();
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
            WH2840x1754Radio = new RadioButton();
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
            groupBox1 = new GroupBox();
            OtherRadioButton = new RadioButton();
            ShipRadioButton = new RadioButton();
            SolarSystemRadioButton = new RadioButton();
            DungeonRadioButton = new RadioButton();
            InteriorRadioButton = new RadioButton();
            CityRadioButton = new RadioButton();
            RegionRadioButton = new RadioButton();
            WorldRadioButton = new RadioButton();
            RealmNameTextBox = new ReaLTaiizor.Controls.PoisonTextBox();
            RealmConfigDialog.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WidthUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HeightUpDown).BeginInit();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // RealmConfigDialog
            // 
            RealmConfigDialog.BackColor = Color.FromArgb(244, 241, 243);
            RealmConfigDialog.BorderColor = Color.FromArgb(38, 38, 38);
            RealmConfigDialog.Controls.Add(CancelConfigButton);
            RealmConfigDialog.Controls.Add(OkayButton);
            RealmConfigDialog.Controls.Add(groupBox5);
            RealmConfigDialog.Controls.Add(groupBox4);
            RealmConfigDialog.Controls.Add(groupBox3);
            RealmConfigDialog.Controls.Add(groupBox2);
            RealmConfigDialog.Controls.Add(groupBox1);
            RealmConfigDialog.Controls.Add(RealmNameTextBox);
            RealmConfigDialog.Dock = DockStyle.Fill;
            RealmConfigDialog.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            RealmConfigDialog.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            RealmConfigDialog.Font = new Font("Segoe UI", 9F);
            RealmConfigDialog.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            RealmConfigDialog.ForeColor = Color.FromArgb(223, 219, 210);
            RealmConfigDialog.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            RealmConfigDialog.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            RealmConfigDialog.Location = new Point(0, 0);
            RealmConfigDialog.Name = "RealmConfigDialog";
            RealmConfigDialog.Padding = new Padding(20, 56, 20, 16);
            RealmConfigDialog.RoundCorners = true;
            RealmConfigDialog.Sizable = true;
            RealmConfigDialog.Size = new Size(601, 557);
            RealmConfigDialog.SmartBounds = true;
            RealmConfigDialog.StartPosition = FormStartPosition.CenterParent;
            RealmConfigDialog.TabIndex = 0;
            RealmConfigDialog.Text = "Realm Configuration";
            RealmConfigDialog.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CancelConfigButton
            // 
            CancelConfigButton.DialogResult = DialogResult.Cancel;
            CancelConfigButton.ForeColor = SystemColors.ControlDarkDark;
            CancelConfigButton.IconChar = FontAwesome.Sharp.IconChar.None;
            CancelConfigButton.IconColor = Color.Black;
            CancelConfigButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CancelConfigButton.Location = new Point(516, 466);
            CancelConfigButton.Name = "CancelConfigButton";
            CancelConfigButton.Size = new Size(60, 60);
            CancelConfigButton.TabIndex = 22;
            CancelConfigButton.Text = "&Cancel";
            CancelConfigButton.UseVisualStyleBackColor = true;
            // 
            // OkayButton
            // 
            OkayButton.DialogResult = DialogResult.OK;
            OkayButton.ForeColor = SystemColors.ControlDarkDark;
            OkayButton.IconChar = FontAwesome.Sharp.IconChar.None;
            OkayButton.IconColor = Color.Black;
            OkayButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            OkayButton.Location = new Point(450, 466);
            OkayButton.Name = "OkayButton";
            OkayButton.Size = new Size(60, 60);
            OkayButton.TabIndex = 21;
            OkayButton.Text = "O&K";
            OkayButton.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(dungeonListBox1);
            groupBox5.Location = new Point(371, 339);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(205, 103);
            groupBox5.TabIndex = 20;
            groupBox5.TabStop = false;
            groupBox5.Text = "Realm Map Theme";
            // 
            // dungeonListBox1
            // 
            dungeonListBox1.DrawMode = DrawMode.OwnerDrawFixed;
            dungeonListBox1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dungeonListBox1.FormattingEnabled = true;
            dungeonListBox1.IntegralHeight = false;
            dungeonListBox1.ItemHeight = 18;
            dungeonListBox1.Items.AddRange(new object[] { "Default Theme", "Theme 1", "Theme 2", "Theme 3", "Theme 4", "Theme 5", "Theme 6" });
            dungeonListBox1.Location = new Point(6, 22);
            dungeonListBox1.Name = "dungeonListBox1";
            dungeonListBox1.Size = new Size(193, 76);
            dungeonListBox1.TabIndex = 0;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label6);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(comboBox1);
            groupBox4.Controls.Add(label4);
            groupBox4.Controls.Add(label7);
            groupBox4.Controls.Add(numericUpDown1);
            groupBox4.Location = new Point(371, 214);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(205, 116);
            groupBox4.TabIndex = 19;
            groupBox4.TabStop = false;
            groupBox4.Text = "Realm Map Area";
            // 
            // label6
            // 
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(114, 81);
            label6.Name = "label6";
            label6.Size = new Size(85, 15);
            label6.TabIndex = 6;
            label6.Text = "75";
            label6.TextAlign = ContentAlignment.MiddleCenter;
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
            // comboBox1
            // 
            comboBox1.DropDownWidth = 120;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Centimeters", "Inches", "Feet", "Yards", "Meters", "Kilometers", "Miles", "Astronomical Units (AU)", "Light Years", "Parsecs" });
            comboBox1.Location = new Point(114, 26);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(85, 23);
            comboBox1.TabIndex = 4;
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
            label7.Size = new Size(93, 15);
            label7.TabIndex = 2;
            label7.Text = "Map Area Width";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(114, 55);
            numericUpDown1.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(85, 23);
            numericUpDown1.TabIndex = 1;
            numericUpDown1.TextAlign = HorizontalAlignment.Center;
            numericUpDown1.Value = new decimal(new int[] { 100, 0, 0, 0 });
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
            groupBox3.Location = new Point(371, 89);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(205, 116);
            groupBox3.TabIndex = 18;
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
            label1.Size = new Size(39, 15);
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
            // 
            // HeightUpDown
            // 
            HeightUpDown.Location = new Point(55, 51);
            HeightUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            HeightUpDown.Name = "HeightUpDown";
            HeightUpDown.Size = new Size(87, 23);
            HeightUpDown.TabIndex = 0;
            HeightUpDown.TextAlign = HorizontalAlignment.Center;
            HeightUpDown.Value = new decimal(new int[] { 1080, 0, 0, 0 });
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(WH7680x4320Radio);
            groupBox2.Controls.Add(WH7016x4960Radio);
            groupBox2.Controls.Add(WH4960x3508Radio);
            groupBox2.Controls.Add(WH3508x2480Radio);
            groupBox2.Controls.Add(WH2840x1754Radio);
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
            groupBox2.Location = new Point(147, 89);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(205, 437);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "Realm Map Size Presets";
            // 
            // WH7680x4320Radio
            // 
            WH7680x4320Radio.AutoSize = true;
            WH7680x4320Radio.ForeColor = SystemColors.ControlDarkDark;
            WH7680x4320Radio.Location = new Point(6, 397);
            WH7680x4320Radio.Name = "WH7680x4320Radio";
            WH7680x4320Radio.Size = new Size(137, 19);
            WH7680x4320Radio.TabIndex = 17;
            WH7680x4320Radio.TabStop = true;
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
            WH7016x4960Radio.Size = new Size(152, 19);
            WH7016x4960Radio.TabIndex = 16;
            WH7016x4960Radio.TabStop = true;
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
            WH4960x3508Radio.Size = new Size(152, 19);
            WH4960x3508Radio.TabIndex = 15;
            WH4960x3508Radio.TabStop = true;
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
            WH3508x2480Radio.Size = new Size(152, 19);
            WH3508x2480Radio.TabIndex = 14;
            WH3508x2480Radio.TabStop = true;
            WH3508x2480Radio.Text = "3508 x 2480 (A4 300 DPI)";
            WH3508x2480Radio.UseVisualStyleBackColor = true;
            WH3508x2480Radio.Click += WH3508x2480Radio_Click;
            // 
            // WH2840x1754Radio
            // 
            WH2840x1754Radio.AutoSize = true;
            WH2840x1754Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2840x1754Radio.Location = new Point(6, 297);
            WH2840x1754Radio.Name = "WH2840x1754Radio";
            WH2840x1754Radio.Size = new Size(152, 19);
            WH2840x1754Radio.TabIndex = 13;
            WH2840x1754Radio.TabStop = true;
            WH2840x1754Radio.Text = "2480 x 1754 (A5 300 DPI)";
            WH2840x1754Radio.UseVisualStyleBackColor = true;
            WH2840x1754Radio.Click += WH2840x1754Radio_Click;
            // 
            // WH1754x1240Radio
            // 
            WH1754x1240Radio.AutoSize = true;
            WH1754x1240Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1754x1240Radio.Location = new Point(6, 272);
            WH1754x1240Radio.Name = "WH1754x1240Radio";
            WH1754x1240Radio.Size = new Size(152, 19);
            WH1754x1240Radio.TabIndex = 12;
            WH1754x1240Radio.TabStop = true;
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
            WH3300x2250Radio.Size = new Size(185, 19);
            WH3300x2250Radio.TabIndex = 11;
            WH3300x2250Radio.TabStop = true;
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
            WH1280x720Radio.Size = new Size(115, 19);
            WH1280x720Radio.TabIndex = 10;
            WH1280x720Radio.TabStop = true;
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
            WH4096x2048Radio.Size = new Size(195, 19);
            WH4096x2048Radio.TabIndex = 9;
            WH4096x2048Radio.TabStop = true;
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
            WH3840x2160Radio.Size = new Size(157, 19);
            WH3840x2160Radio.TabIndex = 8;
            WH3840x2160Radio.TabStop = true;
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
            WH2048x1024Radio.Size = new Size(194, 19);
            WH2048x1024Radio.TabIndex = 7;
            WH2048x1024Radio.TabStop = true;
            WH2048x1024Radio.Text = "2048 x 1024 (Equirectangular 2k)";
            WH2048x1024Radio.UseVisualStyleBackColor = true;
            WH2048x1024Radio.Click += WH2048x1024Radio_Click;
            // 
            // WH2560x1080Radio
            // 
            WH2560x1080Radio.AutoSize = true;
            WH2560x1080Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2560x1080Radio.Location = new Point(6, 147);
            WH2560x1080Radio.Name = "WH2560x1080Radio";
            WH2560x1080Radio.Size = new Size(109, 19);
            WH2560x1080Radio.TabIndex = 6;
            WH2560x1080Radio.TabStop = true;
            WH2560x1080Radio.Text = "2560 x 1080 (2K)";
            WH2560x1080Radio.UseVisualStyleBackColor = true;
            WH2560x1080Radio.Click += WH2560x1080Radio_Click;
            // 
            // WH1920x1080Radio
            // 
            WH1920x1080Radio.AutoSize = true;
            WH1920x1080Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1920x1080Radio.Location = new Point(6, 122);
            WH1920x1080Radio.Name = "WH1920x1080Radio";
            WH1920x1080Radio.Size = new Size(169, 19);
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
            WH1600x1200Radio.Size = new Size(127, 19);
            WH1600x1200Radio.TabIndex = 4;
            WH1600x1200Radio.TabStop = true;
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
            WH1280x1024Radio.Size = new Size(125, 19);
            WH1280x1024Radio.TabIndex = 3;
            WH1280x1024Radio.TabStop = true;
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
            WH1024x768Radio.Size = new Size(113, 19);
            WH1024x768Radio.TabIndex = 2;
            WH1024x768Radio.TabStop = true;
            WH1024x768Radio.Text = "1024 x 768 (XGA)";
            WH1024x768Radio.UseVisualStyleBackColor = true;
            WH1024x768Radio.Click += WH1024x768Radio_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(OtherRadioButton);
            groupBox1.Controls.Add(ShipRadioButton);
            groupBox1.Controls.Add(SolarSystemRadioButton);
            groupBox1.Controls.Add(DungeonRadioButton);
            groupBox1.Controls.Add(InteriorRadioButton);
            groupBox1.Controls.Add(CityRadioButton);
            groupBox1.Controls.Add(RegionRadioButton);
            groupBox1.Controls.Add(WorldRadioButton);
            groupBox1.Location = new Point(23, 89);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(107, 437);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Realm Type";
            // 
            // OtherRadioButton
            // 
            OtherRadioButton.AutoSize = true;
            OtherRadioButton.ForeColor = SystemColors.ControlDarkDark;
            OtherRadioButton.Location = new Point(6, 197);
            OtherRadioButton.Name = "OtherRadioButton";
            OtherRadioButton.Size = new Size(55, 19);
            OtherRadioButton.TabIndex = 9;
            OtherRadioButton.TabStop = true;
            OtherRadioButton.Text = "Other";
            OtherRadioButton.UseVisualStyleBackColor = true;
            // 
            // ShipRadioButton
            // 
            ShipRadioButton.AutoSize = true;
            ShipRadioButton.ForeColor = SystemColors.ControlDarkDark;
            ShipRadioButton.Location = new Point(6, 172);
            ShipRadioButton.Name = "ShipRadioButton";
            ShipRadioButton.Size = new Size(48, 19);
            ShipRadioButton.TabIndex = 8;
            ShipRadioButton.TabStop = true;
            ShipRadioButton.Text = "Ship";
            ShipRadioButton.UseVisualStyleBackColor = true;
            // 
            // SolarSystemRadioButton
            // 
            SolarSystemRadioButton.AutoSize = true;
            SolarSystemRadioButton.ForeColor = SystemColors.ControlDarkDark;
            SolarSystemRadioButton.Location = new Point(6, 147);
            SolarSystemRadioButton.Name = "SolarSystemRadioButton";
            SolarSystemRadioButton.Size = new Size(92, 19);
            SolarSystemRadioButton.TabIndex = 7;
            SolarSystemRadioButton.TabStop = true;
            SolarSystemRadioButton.Text = "Solar System";
            SolarSystemRadioButton.UseVisualStyleBackColor = true;
            // 
            // DungeonRadioButton
            // 
            DungeonRadioButton.AutoSize = true;
            DungeonRadioButton.ForeColor = SystemColors.ControlDarkDark;
            DungeonRadioButton.Location = new Point(6, 122);
            DungeonRadioButton.Name = "DungeonRadioButton";
            DungeonRadioButton.Size = new Size(74, 19);
            DungeonRadioButton.TabIndex = 6;
            DungeonRadioButton.TabStop = true;
            DungeonRadioButton.Text = "Dungeon";
            DungeonRadioButton.UseVisualStyleBackColor = true;
            // 
            // InteriorRadioButton
            // 
            InteriorRadioButton.AutoSize = true;
            InteriorRadioButton.ForeColor = SystemColors.ControlDarkDark;
            InteriorRadioButton.Location = new Point(6, 97);
            InteriorRadioButton.Name = "InteriorRadioButton";
            InteriorRadioButton.Size = new Size(63, 19);
            InteriorRadioButton.TabIndex = 5;
            InteriorRadioButton.TabStop = true;
            InteriorRadioButton.Text = "Interior";
            InteriorRadioButton.UseVisualStyleBackColor = true;
            // 
            // CityRadioButton
            // 
            CityRadioButton.AutoSize = true;
            CityRadioButton.ForeColor = SystemColors.ControlDarkDark;
            CityRadioButton.Location = new Point(6, 72);
            CityRadioButton.Name = "CityRadioButton";
            CityRadioButton.Size = new Size(46, 19);
            CityRadioButton.TabIndex = 4;
            CityRadioButton.TabStop = true;
            CityRadioButton.Text = "City";
            CityRadioButton.UseVisualStyleBackColor = true;
            // 
            // RegionRadioButton
            // 
            RegionRadioButton.AutoSize = true;
            RegionRadioButton.ForeColor = SystemColors.ControlDarkDark;
            RegionRadioButton.Location = new Point(6, 47);
            RegionRadioButton.Name = "RegionRadioButton";
            RegionRadioButton.Size = new Size(62, 19);
            RegionRadioButton.TabIndex = 3;
            RegionRadioButton.TabStop = true;
            RegionRadioButton.Text = "Region";
            RegionRadioButton.UseVisualStyleBackColor = true;
            // 
            // WorldRadioButton
            // 
            WorldRadioButton.AutoSize = true;
            WorldRadioButton.ForeColor = SystemColors.ControlDarkDark;
            WorldRadioButton.Location = new Point(6, 22);
            WorldRadioButton.Name = "WorldRadioButton";
            WorldRadioButton.Size = new Size(57, 19);
            WorldRadioButton.TabIndex = 2;
            WorldRadioButton.TabStop = true;
            WorldRadioButton.Text = "World";
            WorldRadioButton.UseVisualStyleBackColor = true;
            // 
            // RealmNameTextBox
            // 
            // 
            // 
            // 
            RealmNameTextBox.CustomButton.Image = null;
            RealmNameTextBox.CustomButton.Location = new Point(531, 2);
            RealmNameTextBox.CustomButton.Name = "";
            RealmNameTextBox.CustomButton.Size = new Size(19, 19);
            RealmNameTextBox.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            RealmNameTextBox.CustomButton.TabIndex = 1;
            RealmNameTextBox.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            RealmNameTextBox.CustomButton.UseSelectable = true;
            RealmNameTextBox.CustomButton.Visible = false;
            RealmNameTextBox.Location = new Point(23, 59);
            RealmNameTextBox.MaxLength = 32767;
            RealmNameTextBox.Name = "RealmNameTextBox";
            RealmNameTextBox.PasswordChar = '\0';
            RealmNameTextBox.PromptText = "Enter Realm Name";
            RealmNameTextBox.ScrollBars = ScrollBars.None;
            RealmNameTextBox.SelectedText = "";
            RealmNameTextBox.SelectionLength = 0;
            RealmNameTextBox.SelectionStart = 0;
            RealmNameTextBox.ShortcutsEnabled = true;
            RealmNameTextBox.Size = new Size(553, 24);
            RealmNameTextBox.TabIndex = 0;
            RealmNameTextBox.UseSelectable = true;
            RealmNameTextBox.WaterMark = "Enter Realm Name";
            RealmNameTextBox.WaterMarkColor = Color.FromArgb(109, 109, 109);
            RealmNameTextBox.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // RealmConfiguration
            // 
            AcceptButton = OkayButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CancelConfigButton;
            ClientSize = new Size(601, 557);
            ControlBox = false;
            Controls.Add(RealmConfigDialog);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "RealmConfiguration";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Realm Configuration";
            TopMost = true;
            TransparencyKey = Color.Fuchsia;
            RealmConfigDialog.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)WidthUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)HeightUpDown).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm RealmConfigDialog;
        private ReaLTaiizor.Controls.PoisonTextBox RealmNameTextBox;
        private GroupBox groupBox1;
        private RadioButton WorldRadioButton;
        private RadioButton ShipRadioButton;
        private RadioButton SolarSystemRadioButton;
        private RadioButton DungeonRadioButton;
        private RadioButton InteriorRadioButton;
        private RadioButton CityRadioButton;
        private RadioButton RegionRadioButton;
        private GroupBox groupBox2;
        private RadioButton WH4096x2048Radio;
        private RadioButton WH3840x2160Radio;
        private RadioButton WH2048x1024Radio;
        private RadioButton WH2560x1080Radio;
        private RadioButton WH1920x1080Radio;
        private RadioButton WH1600x1200Radio;
        private RadioButton WH1280x1024Radio;
        private RadioButton WH1024x768Radio;
        private RadioButton OtherRadioButton;
        private RadioButton WH1280x720Radio;
        private RadioButton WH2840x1754Radio;
        private RadioButton WH1754x1240Radio;
        private RadioButton WH3300x2250Radio;
        private RadioButton WH7680x4320Radio;
        private RadioButton WH7016x4960Radio;
        private RadioButton WH4960x3508Radio;
        private RadioButton WH3508x2480Radio;
        private GroupBox groupBox3;
        private NumericUpDown WidthUpDown;
        private NumericUpDown HeightUpDown;
        private Label label1;
        private Label label2;
        private FontAwesome.Sharp.IconButton SwapResolutionButton;
        private FontAwesome.Sharp.IconButton LockAspectRatioButton;
        private GroupBox groupBox4;
        private Label label7;
        private NumericUpDown numericUpDown1;
        private Label AspectRatioLabel;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label5;
        private ComboBox comboBox1;
        private GroupBox groupBox5;
        private ReaLTaiizor.Controls.DungeonListBox dungeonListBox1;
        private FontAwesome.Sharp.IconButton CancelConfigButton;
        private FontAwesome.Sharp.IconButton OkayButton;
    }
}