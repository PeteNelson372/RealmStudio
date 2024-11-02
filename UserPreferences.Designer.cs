namespace RealmStudio
{
    partial class UserPreferences
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
            PreferencesFormOverlay = new ReaLTaiizor.Forms.DungeonForm();
            ClosePreferencesButton = new Button();
            AutosaveIntervalTooltipButton = new FontAwesome.Sharp.IconButton();
            DrawContourTooltipButton = new FontAwesome.Sharp.IconButton();
            AutosaveRealmSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            RealmDirectoryButton = new FontAwesome.Sharp.IconButton();
            DefaultRealmDirectoryTextbox = new TextBox();
            label10 = new Label();
            AutosaveDirButton = new FontAwesome.Sharp.IconButton();
            RealmAssetDirButton = new FontAwesome.Sharp.IconButton();
            label9 = new Label();
            label8 = new Label();
            DrawContoursWhilePaintingSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            label7 = new Label();
            label6 = new Label();
            DefaultExportFormatCombo = new ComboBox();
            AutosaveIntervalTrack = new TrackBar();
            label5 = new Label();
            AutosaveDirectoryTextBox = new TextBox();
            label4 = new Label();
            PlaySoundOnSaveSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            label3 = new Label();
            RealmAssetDirectoryTextBox = new TextBox();
            label2 = new Label();
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
            label1 = new Label();
            MeasurementUnitsCombo = new ComboBox();
            PreferencesFormOverlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AutosaveIntervalTrack).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // PreferencesFormOverlay
            // 
            PreferencesFormOverlay.BackColor = Color.FromArgb(244, 241, 243);
            PreferencesFormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            PreferencesFormOverlay.Controls.Add(ClosePreferencesButton);
            PreferencesFormOverlay.Controls.Add(AutosaveIntervalTooltipButton);
            PreferencesFormOverlay.Controls.Add(DrawContourTooltipButton);
            PreferencesFormOverlay.Controls.Add(AutosaveRealmSwitch);
            PreferencesFormOverlay.Controls.Add(RealmDirectoryButton);
            PreferencesFormOverlay.Controls.Add(DefaultRealmDirectoryTextbox);
            PreferencesFormOverlay.Controls.Add(label10);
            PreferencesFormOverlay.Controls.Add(AutosaveDirButton);
            PreferencesFormOverlay.Controls.Add(RealmAssetDirButton);
            PreferencesFormOverlay.Controls.Add(label9);
            PreferencesFormOverlay.Controls.Add(label8);
            PreferencesFormOverlay.Controls.Add(DrawContoursWhilePaintingSwitch);
            PreferencesFormOverlay.Controls.Add(label7);
            PreferencesFormOverlay.Controls.Add(label6);
            PreferencesFormOverlay.Controls.Add(DefaultExportFormatCombo);
            PreferencesFormOverlay.Controls.Add(AutosaveIntervalTrack);
            PreferencesFormOverlay.Controls.Add(label5);
            PreferencesFormOverlay.Controls.Add(AutosaveDirectoryTextBox);
            PreferencesFormOverlay.Controls.Add(label4);
            PreferencesFormOverlay.Controls.Add(PlaySoundOnSaveSwitch);
            PreferencesFormOverlay.Controls.Add(label3);
            PreferencesFormOverlay.Controls.Add(RealmAssetDirectoryTextBox);
            PreferencesFormOverlay.Controls.Add(label2);
            PreferencesFormOverlay.Controls.Add(groupBox2);
            PreferencesFormOverlay.Controls.Add(label1);
            PreferencesFormOverlay.Controls.Add(MeasurementUnitsCombo);
            PreferencesFormOverlay.Dock = DockStyle.Fill;
            PreferencesFormOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            PreferencesFormOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            PreferencesFormOverlay.Font = new Font("Segoe UI", 9F);
            PreferencesFormOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            PreferencesFormOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            PreferencesFormOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            PreferencesFormOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            PreferencesFormOverlay.Location = new Point(0, 0);
            PreferencesFormOverlay.Name = "PreferencesFormOverlay";
            PreferencesFormOverlay.Padding = new Padding(20, 56, 20, 16);
            PreferencesFormOverlay.RoundCorners = true;
            PreferencesFormOverlay.Sizable = false;
            PreferencesFormOverlay.Size = new Size(557, 772);
            PreferencesFormOverlay.SmartBounds = true;
            PreferencesFormOverlay.StartPosition = FormStartPosition.CenterParent;
            PreferencesFormOverlay.TabIndex = 0;
            PreferencesFormOverlay.Text = "Preferences";
            PreferencesFormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // ClosePreferencesButton
            // 
            ClosePreferencesButton.DialogResult = DialogResult.Cancel;
            ClosePreferencesButton.ForeColor = SystemColors.ControlDarkDark;
            ClosePreferencesButton.Location = new Point(474, 693);
            ClosePreferencesButton.Name = "ClosePreferencesButton";
            ClosePreferencesButton.Size = new Size(60, 60);
            ClosePreferencesButton.TabIndex = 61;
            ClosePreferencesButton.Text = "&Close";
            ClosePreferencesButton.UseVisualStyleBackColor = true;
            ClosePreferencesButton.Click += ClosePreferencesButton_Click;
            // 
            // AutosaveIntervalTooltipButton
            // 
            AutosaveIntervalTooltipButton.FlatAppearance.BorderSize = 0;
            AutosaveIntervalTooltipButton.FlatStyle = FlatStyle.Flat;
            AutosaveIntervalTooltipButton.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
            AutosaveIntervalTooltipButton.IconColor = Color.DeepSkyBlue;
            AutosaveIntervalTooltipButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AutosaveIntervalTooltipButton.IconSize = 20;
            AutosaveIntervalTooltipButton.Location = new Point(490, 205);
            AutosaveIntervalTooltipButton.Name = "AutosaveIntervalTooltipButton";
            AutosaveIntervalTooltipButton.Size = new Size(24, 24);
            AutosaveIntervalTooltipButton.TabIndex = 60;
            AutosaveIntervalTooltipButton.UseVisualStyleBackColor = true;
            AutosaveIntervalTooltipButton.MouseHover += AutosaveIntervalTooltipButton_MouseHover;
            // 
            // DrawContourTooltipButton
            // 
            DrawContourTooltipButton.FlatAppearance.BorderSize = 0;
            DrawContourTooltipButton.FlatStyle = FlatStyle.Flat;
            DrawContourTooltipButton.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
            DrawContourTooltipButton.IconColor = Color.DeepSkyBlue;
            DrawContourTooltipButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            DrawContourTooltipButton.IconSize = 20;
            DrawContourTooltipButton.Location = new Point(250, 286);
            DrawContourTooltipButton.Name = "DrawContourTooltipButton";
            DrawContourTooltipButton.Size = new Size(24, 24);
            DrawContourTooltipButton.TabIndex = 59;
            DrawContourTooltipButton.UseVisualStyleBackColor = true;
            DrawContourTooltipButton.MouseHover += DrawContourTooltipButton_MouseHover;
            // 
            // AutosaveRealmSwitch
            // 
            AutosaveRealmSwitch.Alpha = 50;
            AutosaveRealmSwitch.BackColor = Color.Transparent;
            AutosaveRealmSwitch.Background = true;
            AutosaveRealmSwitch.Background_WidthPen = 2F;
            AutosaveRealmSwitch.BackgroundPen = false;
            AutosaveRealmSwitch.Checked = true;
            AutosaveRealmSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            AutosaveRealmSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            AutosaveRealmSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            AutosaveRealmSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            AutosaveRealmSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            AutosaveRealmSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            AutosaveRealmSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            AutosaveRealmSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            AutosaveRealmSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            AutosaveRealmSwitch.ColorValue = Color.ForestGreen;
            AutosaveRealmSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            AutosaveRealmSwitch.Font = new Font("Arial", 11F);
            AutosaveRealmSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            AutosaveRealmSwitch.Lighting = true;
            AutosaveRealmSwitch.LinearGradient_Background = false;
            AutosaveRealmSwitch.LinearGradient_Value = false;
            AutosaveRealmSwitch.LinearGradientPen = false;
            AutosaveRealmSwitch.Location = new Point(203, 146);
            AutosaveRealmSwitch.Name = "AutosaveRealmSwitch";
            AutosaveRealmSwitch.PenWidth = 10;
            AutosaveRealmSwitch.RGB = false;
            AutosaveRealmSwitch.Rounding = true;
            AutosaveRealmSwitch.RoundingInt = 90;
            AutosaveRealmSwitch.Size = new Size(41, 20);
            AutosaveRealmSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            AutosaveRealmSwitch.TabIndex = 58;
            AutosaveRealmSwitch.Tag = "Cyber";
            AutosaveRealmSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            AutosaveRealmSwitch.Timer_RGB = 300;
            AutosaveRealmSwitch.CheckedChanged += AutosaveRealmSwitch_CheckedChanged;
            // 
            // RealmDirectoryButton
            // 
            RealmDirectoryButton.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            RealmDirectoryButton.IconColor = Color.Black;
            RealmDirectoryButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            RealmDirectoryButton.IconSize = 20;
            RealmDirectoryButton.Location = new Point(490, 58);
            RealmDirectoryButton.Name = "RealmDirectoryButton";
            RealmDirectoryButton.Size = new Size(44, 23);
            RealmDirectoryButton.TabIndex = 57;
            RealmDirectoryButton.UseVisualStyleBackColor = true;
            RealmDirectoryButton.Click += RealmDirectoryButton_Click;
            // 
            // DefaultRealmDirectoryTextbox
            // 
            DefaultRealmDirectoryTextbox.Location = new Point(203, 59);
            DefaultRealmDirectoryTextbox.Name = "DefaultRealmDirectoryTextbox";
            DefaultRealmDirectoryTextbox.Size = new Size(281, 23);
            DefaultRealmDirectoryTextbox.TabIndex = 56;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = SystemColors.ControlDarkDark;
            label10.Location = new Point(65, 62);
            label10.Name = "label10";
            label10.Size = new Size(132, 15);
            label10.TabIndex = 55;
            label10.Text = "Default Realm Directory";
            // 
            // AutosaveDirButton
            // 
            AutosaveDirButton.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            AutosaveDirButton.IconColor = Color.Black;
            AutosaveDirButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AutosaveDirButton.IconSize = 20;
            AutosaveDirButton.Location = new Point(490, 171);
            AutosaveDirButton.Name = "AutosaveDirButton";
            AutosaveDirButton.Size = new Size(44, 23);
            AutosaveDirButton.TabIndex = 54;
            AutosaveDirButton.UseVisualStyleBackColor = true;
            AutosaveDirButton.Click += AutosaveDirButton_Click;
            // 
            // RealmAssetDirButton
            // 
            RealmAssetDirButton.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            RealmAssetDirButton.IconColor = Color.Black;
            RealmAssetDirButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            RealmAssetDirButton.IconSize = 20;
            RealmAssetDirButton.Location = new Point(490, 116);
            RealmAssetDirButton.Name = "RealmAssetDirButton";
            RealmAssetDirButton.Size = new Size(44, 23);
            RealmAssetDirButton.TabIndex = 53;
            RealmAssetDirButton.UseVisualStyleBackColor = true;
            RealmAssetDirButton.Click += RealmAssetDirButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = SystemColors.ControlDarkDark;
            label9.Location = new Point(99, 328);
            label9.Name = "label9";
            label9.Size = new Size(95, 15);
            label9.TabIndex = 51;
            label9.Text = "Default Map Size";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = SystemColors.ControlDarkDark;
            label8.Location = new Point(31, 291);
            label8.Name = "label8";
            label8.Size = new Size(166, 15);
            label8.TabIndex = 50;
            label8.Text = "Draw Contours While Painting";
            // 
            // DrawContoursWhilePaintingSwitch
            // 
            DrawContoursWhilePaintingSwitch.Alpha = 50;
            DrawContoursWhilePaintingSwitch.BackColor = Color.Transparent;
            DrawContoursWhilePaintingSwitch.Background = true;
            DrawContoursWhilePaintingSwitch.Background_WidthPen = 2F;
            DrawContoursWhilePaintingSwitch.BackgroundPen = false;
            DrawContoursWhilePaintingSwitch.Checked = true;
            DrawContoursWhilePaintingSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            DrawContoursWhilePaintingSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            DrawContoursWhilePaintingSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            DrawContoursWhilePaintingSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            DrawContoursWhilePaintingSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            DrawContoursWhilePaintingSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            DrawContoursWhilePaintingSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            DrawContoursWhilePaintingSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            DrawContoursWhilePaintingSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            DrawContoursWhilePaintingSwitch.ColorValue = Color.ForestGreen;
            DrawContoursWhilePaintingSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            DrawContoursWhilePaintingSwitch.Font = new Font("Arial", 11F);
            DrawContoursWhilePaintingSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            DrawContoursWhilePaintingSwitch.Lighting = true;
            DrawContoursWhilePaintingSwitch.LinearGradient_Background = false;
            DrawContoursWhilePaintingSwitch.LinearGradient_Value = false;
            DrawContoursWhilePaintingSwitch.LinearGradientPen = false;
            DrawContoursWhilePaintingSwitch.Location = new Point(203, 286);
            DrawContoursWhilePaintingSwitch.Name = "DrawContoursWhilePaintingSwitch";
            DrawContoursWhilePaintingSwitch.PenWidth = 10;
            DrawContoursWhilePaintingSwitch.RGB = false;
            DrawContoursWhilePaintingSwitch.Rounding = true;
            DrawContoursWhilePaintingSwitch.RoundingInt = 90;
            DrawContoursWhilePaintingSwitch.Size = new Size(41, 20);
            DrawContoursWhilePaintingSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            DrawContoursWhilePaintingSwitch.TabIndex = 49;
            DrawContoursWhilePaintingSwitch.Tag = "Cyber";
            DrawContoursWhilePaintingSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            DrawContoursWhilePaintingSwitch.Timer_RGB = 300;
            DrawContoursWhilePaintingSwitch.CheckedChanged += DrawContoursWhilePaintingSwitch_CheckedChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = SystemColors.ControlDarkDark;
            label7.Location = new Point(87, 236);
            label7.Name = "label7";
            label7.Size = new Size(110, 15);
            label7.TabIndex = 47;
            label7.Text = "Play Sound on Save";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(74, 260);
            label6.Name = "label6";
            label6.Size = new Size(123, 15);
            label6.TabIndex = 46;
            label6.Text = "Default Export Format";
            // 
            // DefaultExportFormatCombo
            // 
            DefaultExportFormatCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            DefaultExportFormatCombo.FormattingEnabled = true;
            DefaultExportFormatCombo.Items.AddRange(new object[] { "PNG", "JPG", "BMP" });
            DefaultExportFormatCombo.Location = new Point(203, 257);
            DefaultExportFormatCombo.Name = "DefaultExportFormatCombo";
            DefaultExportFormatCombo.Size = new Size(121, 23);
            DefaultExportFormatCombo.TabIndex = 45;
            DefaultExportFormatCombo.SelectionChangeCommitted += DefaultExportFormatCombo_SelectionChangeCommitted;
            // 
            // AutosaveIntervalTrack
            // 
            AutosaveIntervalTrack.AutoSize = false;
            AutosaveIntervalTrack.Location = new Point(203, 201);
            AutosaveIntervalTrack.Maximum = 30;
            AutosaveIntervalTrack.Minimum = 5;
            AutosaveIntervalTrack.Name = "AutosaveIntervalTrack";
            AutosaveIntervalTrack.Size = new Size(281, 24);
            AutosaveIntervalTrack.TabIndex = 44;
            AutosaveIntervalTrack.TickFrequency = 5;
            AutosaveIntervalTrack.Value = 5;
            AutosaveIntervalTrack.Scroll += AutosaveIntervalTrack_Scroll;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(99, 210);
            label5.Name = "label5";
            label5.Size = new Size(98, 15);
            label5.TabIndex = 43;
            label5.Text = "Autosave Interval";
            // 
            // AutosaveDirectoryTextBox
            // 
            AutosaveDirectoryTextBox.Location = new Point(203, 172);
            AutosaveDirectoryTextBox.Name = "AutosaveDirectoryTextBox";
            AutosaveDirectoryTextBox.Size = new Size(281, 23);
            AutosaveDirectoryTextBox.TabIndex = 42;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(87, 175);
            label4.Name = "label4";
            label4.Size = new Size(107, 15);
            label4.TabIndex = 41;
            label4.Text = "Autosave Directory";
            // 
            // PlaySoundOnSaveSwitch
            // 
            PlaySoundOnSaveSwitch.Alpha = 50;
            PlaySoundOnSaveSwitch.BackColor = Color.Transparent;
            PlaySoundOnSaveSwitch.Background = true;
            PlaySoundOnSaveSwitch.Background_WidthPen = 2F;
            PlaySoundOnSaveSwitch.BackgroundPen = false;
            PlaySoundOnSaveSwitch.Checked = true;
            PlaySoundOnSaveSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            PlaySoundOnSaveSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            PlaySoundOnSaveSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            PlaySoundOnSaveSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            PlaySoundOnSaveSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            PlaySoundOnSaveSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            PlaySoundOnSaveSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            PlaySoundOnSaveSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            PlaySoundOnSaveSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            PlaySoundOnSaveSwitch.ColorValue = Color.ForestGreen;
            PlaySoundOnSaveSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            PlaySoundOnSaveSwitch.Font = new Font("Arial", 11F);
            PlaySoundOnSaveSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            PlaySoundOnSaveSwitch.Lighting = true;
            PlaySoundOnSaveSwitch.LinearGradient_Background = false;
            PlaySoundOnSaveSwitch.LinearGradient_Value = false;
            PlaySoundOnSaveSwitch.LinearGradientPen = false;
            PlaySoundOnSaveSwitch.Location = new Point(203, 231);
            PlaySoundOnSaveSwitch.Name = "PlaySoundOnSaveSwitch";
            PlaySoundOnSaveSwitch.PenWidth = 10;
            PlaySoundOnSaveSwitch.RGB = false;
            PlaySoundOnSaveSwitch.Rounding = true;
            PlaySoundOnSaveSwitch.RoundingInt = 90;
            PlaySoundOnSaveSwitch.Size = new Size(41, 20);
            PlaySoundOnSaveSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            PlaySoundOnSaveSwitch.TabIndex = 40;
            PlaySoundOnSaveSwitch.Tag = "Cyber";
            PlaySoundOnSaveSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            PlaySoundOnSaveSwitch.Timer_RGB = 300;
            PlaySoundOnSaveSwitch.CheckedChanged += PlaySoundOnSaveSwitch_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(105, 151);
            label3.Name = "label3";
            label3.Size = new Size(92, 15);
            label3.TabIndex = 14;
            label3.Text = "Autosave Realm";
            // 
            // RealmAssetDirectoryTextBox
            // 
            RealmAssetDirectoryTextBox.Location = new Point(203, 117);
            RealmAssetDirectoryTextBox.Name = "RealmAssetDirectoryTextBox";
            RealmAssetDirectoryTextBox.Size = new Size(281, 23);
            RealmAssetDirectoryTextBox.TabIndex = 13;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(72, 120);
            label2.Name = "label2";
            label2.Size = new Size(122, 15);
            label2.TabIndex = 12;
            label2.Text = "Realm Asset Directory";
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
            groupBox2.Location = new Point(203, 328);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(205, 425);
            groupBox2.TabIndex = 11;
            groupBox2.TabStop = false;
            groupBox2.Text = "Map Size";
            // 
            // WH7680x4320Radio
            // 
            WH7680x4320Radio.AutoSize = true;
            WH7680x4320Radio.ForeColor = SystemColors.ControlDarkDark;
            WH7680x4320Radio.Location = new Point(6, 397);
            WH7680x4320Radio.Name = "WH7680x4320Radio";
            WH7680x4320Radio.Size = new Size(137, 19);
            WH7680x4320Radio.TabIndex = 17;
            WH7680x4320Radio.Text = "7680 x 4320 (8K UHD)";
            WH7680x4320Radio.UseVisualStyleBackColor = true;
            WH7680x4320Radio.CheckedChanged += WH7680x4320Radio_CheckedChanged;
            // 
            // WH7016x4960Radio
            // 
            WH7016x4960Radio.AutoSize = true;
            WH7016x4960Radio.ForeColor = SystemColors.ControlDarkDark;
            WH7016x4960Radio.Location = new Point(6, 372);
            WH7016x4960Radio.Name = "WH7016x4960Radio";
            WH7016x4960Radio.Size = new Size(152, 19);
            WH7016x4960Radio.TabIndex = 16;
            WH7016x4960Radio.Text = "7016 x 4960 (A2 300 DPI)";
            WH7016x4960Radio.UseVisualStyleBackColor = true;
            WH7016x4960Radio.CheckedChanged += WH7016x4960Radio_CheckedChanged;
            // 
            // WH4960x3508Radio
            // 
            WH4960x3508Radio.AutoSize = true;
            WH4960x3508Radio.ForeColor = SystemColors.ControlDarkDark;
            WH4960x3508Radio.Location = new Point(6, 347);
            WH4960x3508Radio.Name = "WH4960x3508Radio";
            WH4960x3508Radio.Size = new Size(152, 19);
            WH4960x3508Radio.TabIndex = 15;
            WH4960x3508Radio.Text = "4960 x 3508 (A3 300 DPI)";
            WH4960x3508Radio.UseVisualStyleBackColor = true;
            WH4960x3508Radio.CheckedChanged += WH4960x3508Radio_CheckedChanged;
            // 
            // WH3508x2480Radio
            // 
            WH3508x2480Radio.AutoSize = true;
            WH3508x2480Radio.ForeColor = SystemColors.ControlDarkDark;
            WH3508x2480Radio.Location = new Point(6, 322);
            WH3508x2480Radio.Name = "WH3508x2480Radio";
            WH3508x2480Radio.Size = new Size(152, 19);
            WH3508x2480Radio.TabIndex = 14;
            WH3508x2480Radio.Text = "3508 x 2480 (A4 300 DPI)";
            WH3508x2480Radio.UseVisualStyleBackColor = true;
            WH3508x2480Radio.CheckedChanged += WH3508x2480Radio_CheckedChanged;
            // 
            // WH2480x1754Radio
            // 
            WH2480x1754Radio.AutoSize = true;
            WH2480x1754Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2480x1754Radio.Location = new Point(6, 297);
            WH2480x1754Radio.Name = "WH2480x1754Radio";
            WH2480x1754Radio.Size = new Size(152, 19);
            WH2480x1754Radio.TabIndex = 13;
            WH2480x1754Radio.Text = "2480 x 1754 (A5 300 DPI)";
            WH2480x1754Radio.UseVisualStyleBackColor = true;
            WH2480x1754Radio.CheckedChanged += WH2480x1754Radio_CheckedChanged;
            // 
            // WH1754x1240Radio
            // 
            WH1754x1240Radio.AutoSize = true;
            WH1754x1240Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1754x1240Radio.Location = new Point(6, 272);
            WH1754x1240Radio.Name = "WH1754x1240Radio";
            WH1754x1240Radio.Size = new Size(152, 19);
            WH1754x1240Radio.TabIndex = 12;
            WH1754x1240Radio.Text = "1754 x 1240 (A6 300 DPI)";
            WH1754x1240Radio.UseVisualStyleBackColor = true;
            WH1754x1240Radio.CheckedChanged += WH1754x1240Radio_CheckedChanged;
            // 
            // WH3300x2250Radio
            // 
            WH3300x2250Radio.AutoSize = true;
            WH3300x2250Radio.ForeColor = SystemColors.ControlDarkDark;
            WH3300x2250Radio.Location = new Point(6, 247);
            WH3300x2250Radio.Name = "WH3300x2250Radio";
            WH3300x2250Radio.Size = new Size(185, 19);
            WH3300x2250Radio.TabIndex = 11;
            WH3300x2250Radio.Text = "3300 x 2250 (US Letter 300 DPI)";
            WH3300x2250Radio.UseVisualStyleBackColor = true;
            WH3300x2250Radio.CheckedChanged += WH3300x2250Radio_CheckedChanged;
            // 
            // WH1280x720Radio
            // 
            WH1280x720Radio.AutoSize = true;
            WH1280x720Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1280x720Radio.Location = new Point(6, 47);
            WH1280x720Radio.Name = "WH1280x720Radio";
            WH1280x720Radio.Size = new Size(115, 19);
            WH1280x720Radio.TabIndex = 10;
            WH1280x720Radio.Text = "1280 x 720 (720P)";
            WH1280x720Radio.UseVisualStyleBackColor = true;
            WH1280x720Radio.CheckedChanged += WH1280x720Radio_CheckedChanged;
            // 
            // WH4096x2048Radio
            // 
            WH4096x2048Radio.AutoSize = true;
            WH4096x2048Radio.ForeColor = SystemColors.ControlDarkDark;
            WH4096x2048Radio.Location = new Point(6, 222);
            WH4096x2048Radio.Name = "WH4096x2048Radio";
            WH4096x2048Radio.Size = new Size(195, 19);
            WH4096x2048Radio.TabIndex = 9;
            WH4096x2048Radio.Text = "4096 x 2048 (Equirectangular 4K)";
            WH4096x2048Radio.UseVisualStyleBackColor = true;
            WH4096x2048Radio.CheckedChanged += WH4096x2048Radio_CheckedChanged;
            // 
            // WH3840x2160Radio
            // 
            WH3840x2160Radio.AutoSize = true;
            WH3840x2160Radio.ForeColor = SystemColors.ControlDarkDark;
            WH3840x2160Radio.Location = new Point(6, 197);
            WH3840x2160Radio.Name = "WH3840x2160Radio";
            WH3840x2160Radio.Size = new Size(157, 19);
            WH3840x2160Radio.TabIndex = 8;
            WH3840x2160Radio.Text = "3840 x 2160 (4K Ultra HD)";
            WH3840x2160Radio.UseVisualStyleBackColor = true;
            WH3840x2160Radio.CheckedChanged += WH3840x2160Radio_CheckedChanged;
            // 
            // WH2048x1024Radio
            // 
            WH2048x1024Radio.AutoSize = true;
            WH2048x1024Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2048x1024Radio.Location = new Point(6, 172);
            WH2048x1024Radio.Name = "WH2048x1024Radio";
            WH2048x1024Radio.Size = new Size(194, 19);
            WH2048x1024Radio.TabIndex = 7;
            WH2048x1024Radio.Text = "2048 x 1024 (Equirectangular 2k)";
            WH2048x1024Radio.UseVisualStyleBackColor = true;
            WH2048x1024Radio.CheckedChanged += WH2048x1024Radio_CheckedChanged;
            // 
            // WH2560x1080Radio
            // 
            WH2560x1080Radio.AutoSize = true;
            WH2560x1080Radio.ForeColor = SystemColors.ControlDarkDark;
            WH2560x1080Radio.Location = new Point(6, 147);
            WH2560x1080Radio.Name = "WH2560x1080Radio";
            WH2560x1080Radio.Size = new Size(109, 19);
            WH2560x1080Radio.TabIndex = 6;
            WH2560x1080Radio.Text = "2560 x 1080 (2K)";
            WH2560x1080Radio.UseVisualStyleBackColor = true;
            WH2560x1080Radio.CheckedChanged += WH2560x1080Radio_CheckedChanged;
            // 
            // WH1920x1080Radio
            // 
            WH1920x1080Radio.AutoSize = true;
            WH1920x1080Radio.Checked = true;
            WH1920x1080Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1920x1080Radio.Location = new Point(6, 122);
            WH1920x1080Radio.Name = "WH1920x1080Radio";
            WH1920x1080Radio.Size = new Size(169, 19);
            WH1920x1080Radio.TabIndex = 5;
            WH1920x1080Radio.TabStop = true;
            WH1920x1080Radio.Text = "1920 x 1080 (1080P Full HD)";
            WH1920x1080Radio.UseVisualStyleBackColor = true;
            WH1920x1080Radio.CheckedChanged += WH1920x1080Radio_CheckedChanged;
            // 
            // WH1600x1200Radio
            // 
            WH1600x1200Radio.AutoSize = true;
            WH1600x1200Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1600x1200Radio.Location = new Point(6, 97);
            WH1600x1200Radio.Name = "WH1600x1200Radio";
            WH1600x1200Radio.Size = new Size(127, 19);
            WH1600x1200Radio.TabIndex = 4;
            WH1600x1200Radio.Text = "1600 x 1200 (UXGA)";
            WH1600x1200Radio.UseVisualStyleBackColor = true;
            WH1600x1200Radio.CheckedChanged += WH1600x1200Radio_CheckedChanged;
            // 
            // WH1280x1024Radio
            // 
            WH1280x1024Radio.AutoSize = true;
            WH1280x1024Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1280x1024Radio.Location = new Point(6, 72);
            WH1280x1024Radio.Name = "WH1280x1024Radio";
            WH1280x1024Radio.Size = new Size(125, 19);
            WH1280x1024Radio.TabIndex = 3;
            WH1280x1024Radio.Text = "1280 x 1024 (SXGA)";
            WH1280x1024Radio.UseVisualStyleBackColor = true;
            WH1280x1024Radio.CheckedChanged += WH1280x1024Radio_CheckedChanged;
            // 
            // WH1024x768Radio
            // 
            WH1024x768Radio.AutoSize = true;
            WH1024x768Radio.ForeColor = SystemColors.ControlDarkDark;
            WH1024x768Radio.Location = new Point(6, 22);
            WH1024x768Radio.Name = "WH1024x768Radio";
            WH1024x768Radio.Size = new Size(113, 19);
            WH1024x768Radio.TabIndex = 2;
            WH1024x768Radio.Text = "1024 x 768 (XGA)";
            WH1024x768Radio.UseVisualStyleBackColor = true;
            WH1024x768Radio.CheckedChanged += WH1024x768Radio_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(87, 91);
            label1.Name = "label1";
            label1.Size = new Size(110, 15);
            label1.TabIndex = 1;
            label1.Text = "Measurement Units";
            // 
            // MeasurementUnitsCombo
            // 
            MeasurementUnitsCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            MeasurementUnitsCombo.FormattingEnabled = true;
            MeasurementUnitsCombo.Items.AddRange(new object[] { "US Customary", "Metric" });
            MeasurementUnitsCombo.Location = new Point(203, 88);
            MeasurementUnitsCombo.Name = "MeasurementUnitsCombo";
            MeasurementUnitsCombo.Size = new Size(121, 23);
            MeasurementUnitsCombo.TabIndex = 0;
            MeasurementUnitsCombo.SelectionChangeCommitted += MeasurementUnitsCombo_SelectionChangeCommitted;
            // 
            // UserPreferences
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(557, 772);
            Controls.Add(PreferencesFormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "UserPreferences";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Preferences";
            TransparencyKey = Color.Fuchsia;
            PreferencesFormOverlay.ResumeLayout(false);
            PreferencesFormOverlay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AutosaveIntervalTrack).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm PreferencesFormOverlay;
        private Label label1;
        private ComboBox MeasurementUnitsCombo;
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
        private Label label2;
        private TextBox RealmAssetDirectoryTextBox;
        private Label label3;
        private ReaLTaiizor.Controls.CyberSwitch PlaySoundOnSaveSwitch;
        private TextBox AutosaveDirectoryTextBox;
        private Label label4;
        private TrackBar AutosaveIntervalTrack;
        private Label label5;
        private Label label6;
        private ComboBox DefaultExportFormatCombo;
        private Label label7;
        private Label label8;
        private ReaLTaiizor.Controls.CyberSwitch DrawContoursWhilePaintingSwitch;
        private Label label9;
        private FontAwesome.Sharp.IconButton AutosaveDirButton;
        private FontAwesome.Sharp.IconButton RealmAssetDirButton;
        private FontAwesome.Sharp.IconButton RealmDirectoryButton;
        private TextBox DefaultRealmDirectoryTextbox;
        private Label label10;
        private ReaLTaiizor.Controls.CyberSwitch AutosaveRealmSwitch;
        private FontAwesome.Sharp.IconButton DrawContourTooltipButton;
        private FontAwesome.Sharp.IconButton AutosaveIntervalTooltipButton;
        private Button ClosePreferencesButton;
    }
}