namespace RealmStudio
{
    partial class LandformInfo
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
            LandformDataPanel = new ReaLTaiizor.Forms.DungeonForm();
            CloseLandformDataButton = new Button();
            ApplyChangesButton = new Button();
            StatusMessageLabel = new Label();
            CoastlineGroup = new GroupBox();
            label16 = new Label();
            label15 = new Label();
            CoastlineStyleList = new ListBox();
            CoastlineColorSelectionButton = new FontAwesome.Sharp.IconButton();
            label14 = new Label();
            CoastlineEffectDistanceTrack = new TrackBar();
            LandformGroup = new GroupBox();
            label13 = new Label();
            LandformOutlineWidthTrack = new TrackBar();
            label88 = new Label();
            LandformBackgroundColorSelectButton = new FontAwesome.Sharp.IconButton();
            UseTextureForBackgroundCheck = new CheckBox();
            LandTextureNameLabel = new Label();
            PreviousLandTextureButton = new FontAwesome.Sharp.IconButton();
            NextLandTextureButton = new FontAwesome.Sharp.IconButton();
            LandformTexturePreviewPicture = new PictureBox();
            LandformOutlineColorSelectButton = new FontAwesome.Sharp.IconButton();
            label2 = new Label();
            NameTextbox = new TextBox();
            GuidLabel = new Label();
            label1 = new Label();
            ApplyThemeSettingsButton = new FontAwesome.Sharp.IconButton();
            label3 = new Label();
            LandformDataPanel.SuspendLayout();
            CoastlineGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CoastlineEffectDistanceTrack).BeginInit();
            LandformGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LandformOutlineWidthTrack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LandformTexturePreviewPicture).BeginInit();
            SuspendLayout();
            // 
            // LandformDataPanel
            // 
            LandformDataPanel.BackColor = Color.FromArgb(244, 241, 243);
            LandformDataPanel.BorderColor = Color.FromArgb(38, 38, 38);
            LandformDataPanel.Controls.Add(label3);
            LandformDataPanel.Controls.Add(ApplyThemeSettingsButton);
            LandformDataPanel.Controls.Add(CloseLandformDataButton);
            LandformDataPanel.Controls.Add(ApplyChangesButton);
            LandformDataPanel.Controls.Add(StatusMessageLabel);
            LandformDataPanel.Controls.Add(CoastlineGroup);
            LandformDataPanel.Controls.Add(LandformGroup);
            LandformDataPanel.Controls.Add(label2);
            LandformDataPanel.Controls.Add(NameTextbox);
            LandformDataPanel.Controls.Add(GuidLabel);
            LandformDataPanel.Controls.Add(label1);
            LandformDataPanel.Dock = DockStyle.Fill;
            LandformDataPanel.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            LandformDataPanel.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            LandformDataPanel.Font = new Font("Segoe UI", 9F);
            LandformDataPanel.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            LandformDataPanel.ForeColor = Color.FromArgb(223, 219, 210);
            LandformDataPanel.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            LandformDataPanel.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            LandformDataPanel.Location = new Point(0, 0);
            LandformDataPanel.Name = "LandformDataPanel";
            LandformDataPanel.Padding = new Padding(20, 56, 20, 16);
            LandformDataPanel.RoundCorners = true;
            LandformDataPanel.Sizable = true;
            LandformDataPanel.Size = new Size(359, 592);
            LandformDataPanel.SmartBounds = true;
            LandformDataPanel.StartPosition = FormStartPosition.CenterParent;
            LandformDataPanel.TabIndex = 0;
            LandformDataPanel.Text = "Landform Data";
            LandformDataPanel.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CloseLandformDataButton
            // 
            CloseLandformDataButton.ForeColor = SystemColors.ControlDarkDark;
            CloseLandformDataButton.Location = new Point(274, 513);
            CloseLandformDataButton.Name = "CloseLandformDataButton";
            CloseLandformDataButton.Size = new Size(60, 60);
            CloseLandformDataButton.TabIndex = 92;
            CloseLandformDataButton.Text = "&Close";
            CloseLandformDataButton.UseVisualStyleBackColor = true;
            CloseLandformDataButton.Click += CloseLandformDataButton_Click;
            // 
            // ApplyChangesButton
            // 
            ApplyChangesButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyChangesButton.Location = new Point(208, 513);
            ApplyChangesButton.Name = "ApplyChangesButton";
            ApplyChangesButton.Size = new Size(60, 60);
            ApplyChangesButton.TabIndex = 91;
            ApplyChangesButton.Text = "&Apply";
            ApplyChangesButton.UseVisualStyleBackColor = true;
            ApplyChangesButton.Click += ApplyChangesButton_Click;
            // 
            // StatusMessageLabel
            // 
            StatusMessageLabel.Location = new Point(23, 551);
            StatusMessageLabel.Name = "StatusMessageLabel";
            StatusMessageLabel.Size = new Size(148, 22);
            StatusMessageLabel.TabIndex = 90;
            // 
            // CoastlineGroup
            // 
            CoastlineGroup.Controls.Add(label16);
            CoastlineGroup.Controls.Add(label15);
            CoastlineGroup.Controls.Add(CoastlineStyleList);
            CoastlineGroup.Controls.Add(CoastlineColorSelectionButton);
            CoastlineGroup.Controls.Add(label14);
            CoastlineGroup.Controls.Add(CoastlineEffectDistanceTrack);
            CoastlineGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CoastlineGroup.Location = new Point(186, 101);
            CoastlineGroup.Name = "CoastlineGroup";
            CoastlineGroup.Size = new Size(148, 302);
            CoastlineGroup.TabIndex = 19;
            CoastlineGroup.TabStop = false;
            CoastlineGroup.Text = "Coastline";
            // 
            // label16
            // 
            label16.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label16.Location = new Point(6, 397);
            label16.Name = "label16";
            label16.Size = new Size(136, 15);
            label16.TabIndex = 29;
            label16.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.BackColor = Color.Transparent;
            label15.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.ForeColor = SystemColors.ControlDarkDark;
            label15.Location = new Point(6, 145);
            label15.Name = "label15";
            label15.Size = new Size(32, 15);
            label15.TabIndex = 24;
            label15.Text = "Style";
            // 
            // CoastlineStyleList
            // 
            CoastlineStyleList.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CoastlineStyleList.FormattingEnabled = true;
            CoastlineStyleList.ItemHeight = 15;
            CoastlineStyleList.Items.AddRange(new object[] { "None", "Uniform Band", "Uniform Blend", "Uniform Outline", "Three-Tiered", "Circular Pattern", "Dash Pattern", "Hatch Pattern" });
            CoastlineStyleList.Location = new Point(6, 163);
            CoastlineStyleList.Name = "CoastlineStyleList";
            CoastlineStyleList.Size = new Size(136, 124);
            CoastlineStyleList.TabIndex = 23;
            // 
            // CoastlineColorSelectionButton
            // 
            CoastlineColorSelectionButton.BackColor = Color.FromArgb(187, 156, 195, 183);
            CoastlineColorSelectionButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CoastlineColorSelectionButton.ForeColor = SystemColors.HighlightText;
            CoastlineColorSelectionButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            CoastlineColorSelectionButton.IconColor = Color.Tan;
            CoastlineColorSelectionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CoastlineColorSelectionButton.Location = new Point(6, 71);
            CoastlineColorSelectionButton.Name = "CoastlineColorSelectionButton";
            CoastlineColorSelectionButton.Size = new Size(136, 60);
            CoastlineColorSelectionButton.TabIndex = 22;
            CoastlineColorSelectionButton.Text = "Click to Select";
            CoastlineColorSelectionButton.UseVisualStyleBackColor = false;
            CoastlineColorSelectionButton.Click += CoastlineColorSelectionButton_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.BackColor = Color.Transparent;
            label14.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label14.ForeColor = SystemColors.ControlDarkDark;
            label14.Location = new Point(6, 27);
            label14.Name = "label14";
            label14.Size = new Size(85, 15);
            label14.TabIndex = 21;
            label14.Text = "Effect Distance";
            // 
            // CoastlineEffectDistanceTrack
            // 
            CoastlineEffectDistanceTrack.AutoSize = false;
            CoastlineEffectDistanceTrack.BackColor = Color.FromArgb(244, 241, 243);
            CoastlineEffectDistanceTrack.Location = new Point(6, 45);
            CoastlineEffectDistanceTrack.Maximum = 40;
            CoastlineEffectDistanceTrack.Name = "CoastlineEffectDistanceTrack";
            CoastlineEffectDistanceTrack.Size = new Size(136, 20);
            CoastlineEffectDistanceTrack.TabIndex = 20;
            CoastlineEffectDistanceTrack.TickStyle = TickStyle.None;
            CoastlineEffectDistanceTrack.Value = 16;
            CoastlineEffectDistanceTrack.Scroll += CoastlineEffectDistanceTrack_Scroll;
            // 
            // LandformGroup
            // 
            LandformGroup.BackColor = Color.Transparent;
            LandformGroup.Controls.Add(label13);
            LandformGroup.Controls.Add(LandformOutlineWidthTrack);
            LandformGroup.Controls.Add(label88);
            LandformGroup.Controls.Add(LandformBackgroundColorSelectButton);
            LandformGroup.Controls.Add(UseTextureForBackgroundCheck);
            LandformGroup.Controls.Add(LandTextureNameLabel);
            LandformGroup.Controls.Add(PreviousLandTextureButton);
            LandformGroup.Controls.Add(NextLandTextureButton);
            LandformGroup.Controls.Add(LandformTexturePreviewPicture);
            LandformGroup.Controls.Add(LandformOutlineColorSelectButton);
            LandformGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LandformGroup.ForeColor = SystemColors.ControlText;
            LandformGroup.Location = new Point(23, 101);
            LandformGroup.Name = "LandformGroup";
            LandformGroup.Size = new Size(148, 435);
            LandformGroup.TabIndex = 18;
            LandformGroup.TabStop = false;
            LandformGroup.Text = "Landform";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.BackColor = Color.Transparent;
            label13.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label13.ForeColor = SystemColors.ControlDarkDark;
            label13.Location = new Point(6, 101);
            label13.Name = "label13";
            label13.Size = new Size(81, 15);
            label13.TabIndex = 34;
            label13.Text = "Outline Width";
            // 
            // LandformOutlineWidthTrack
            // 
            LandformOutlineWidthTrack.AutoSize = false;
            LandformOutlineWidthTrack.BackColor = Color.FromArgb(244, 241, 243);
            LandformOutlineWidthTrack.Location = new Point(6, 119);
            LandformOutlineWidthTrack.Maximum = 16;
            LandformOutlineWidthTrack.Minimum = 1;
            LandformOutlineWidthTrack.Name = "LandformOutlineWidthTrack";
            LandformOutlineWidthTrack.Size = new Size(136, 20);
            LandformOutlineWidthTrack.TabIndex = 33;
            LandformOutlineWidthTrack.TickStyle = TickStyle.None;
            LandformOutlineWidthTrack.Value = 2;
            // 
            // label88
            // 
            label88.AutoSize = true;
            label88.BackColor = Color.Transparent;
            label88.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label88.ForeColor = SystemColors.ControlDarkDark;
            label88.Location = new Point(6, 145);
            label88.Name = "label88";
            label88.Size = new Size(103, 15);
            label88.TabIndex = 32;
            label88.Text = "Background Color";
            // 
            // LandformBackgroundColorSelectButton
            // 
            LandformBackgroundColorSelectButton.BackColor = Color.White;
            LandformBackgroundColorSelectButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LandformBackgroundColorSelectButton.ForeColor = SystemColors.ControlDarkDark;
            LandformBackgroundColorSelectButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            LandformBackgroundColorSelectButton.IconColor = Color.Tan;
            LandformBackgroundColorSelectButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LandformBackgroundColorSelectButton.Location = new Point(6, 163);
            LandformBackgroundColorSelectButton.Name = "LandformBackgroundColorSelectButton";
            LandformBackgroundColorSelectButton.Size = new Size(136, 60);
            LandformBackgroundColorSelectButton.TabIndex = 31;
            LandformBackgroundColorSelectButton.Text = "Click to Select";
            LandformBackgroundColorSelectButton.UseVisualStyleBackColor = false;
            LandformBackgroundColorSelectButton.Click += LandformBackgroundColorSelectButton_Click;
            // 
            // UseTextureForBackgroundCheck
            // 
            UseTextureForBackgroundCheck.AutoSize = true;
            UseTextureForBackgroundCheck.Checked = true;
            UseTextureForBackgroundCheck.CheckState = CheckState.Checked;
            UseTextureForBackgroundCheck.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UseTextureForBackgroundCheck.ForeColor = SystemColors.ControlDarkDark;
            UseTextureForBackgroundCheck.Location = new Point(6, 239);
            UseTextureForBackgroundCheck.Name = "UseTextureForBackgroundCheck";
            UseTextureForBackgroundCheck.Size = new Size(82, 19);
            UseTextureForBackgroundCheck.TabIndex = 29;
            UseTextureForBackgroundCheck.Text = "Texture Fill";
            UseTextureForBackgroundCheck.UseVisualStyleBackColor = true;
            // 
            // LandTextureNameLabel
            // 
            LandTextureNameLabel.AutoEllipsis = true;
            LandTextureNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LandTextureNameLabel.Location = new Point(6, 403);
            LandTextureNameLabel.Name = "LandTextureNameLabel";
            LandTextureNameLabel.Size = new Size(136, 21);
            LandTextureNameLabel.TabIndex = 24;
            LandTextureNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PreviousLandTextureButton
            // 
            PreviousLandTextureButton.FlatStyle = FlatStyle.Flat;
            PreviousLandTextureButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PreviousLandTextureButton.IconChar = FontAwesome.Sharp.IconChar.AngleLeft;
            PreviousLandTextureButton.IconColor = Color.Black;
            PreviousLandTextureButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            PreviousLandTextureButton.IconSize = 24;
            PreviousLandTextureButton.Location = new Point(6, 376);
            PreviousLandTextureButton.Name = "PreviousLandTextureButton";
            PreviousLandTextureButton.Size = new Size(24, 24);
            PreviousLandTextureButton.TabIndex = 22;
            PreviousLandTextureButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            PreviousLandTextureButton.UseVisualStyleBackColor = true;
            PreviousLandTextureButton.Click += PreviousTextureButton_Click;
            // 
            // NextLandTextureButton
            // 
            NextLandTextureButton.FlatStyle = FlatStyle.Flat;
            NextLandTextureButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            NextLandTextureButton.IconChar = FontAwesome.Sharp.IconChar.AngleRight;
            NextLandTextureButton.IconColor = Color.Black;
            NextLandTextureButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            NextLandTextureButton.IconSize = 24;
            NextLandTextureButton.Location = new Point(118, 376);
            NextLandTextureButton.Name = "NextLandTextureButton";
            NextLandTextureButton.Size = new Size(24, 24);
            NextLandTextureButton.TabIndex = 21;
            NextLandTextureButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            NextLandTextureButton.UseVisualStyleBackColor = true;
            NextLandTextureButton.Click += NextTextureButton_Click;
            // 
            // LandformTexturePreviewPicture
            // 
            LandformTexturePreviewPicture.Location = new Point(6, 264);
            LandformTexturePreviewPicture.Name = "LandformTexturePreviewPicture";
            LandformTexturePreviewPicture.Size = new Size(136, 136);
            LandformTexturePreviewPicture.TabIndex = 20;
            LandformTexturePreviewPicture.TabStop = false;
            // 
            // LandformOutlineColorSelectButton
            // 
            LandformOutlineColorSelectButton.BackColor = Color.FromArgb(61, 55, 40);
            LandformOutlineColorSelectButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LandformOutlineColorSelectButton.ForeColor = SystemColors.HighlightText;
            LandformOutlineColorSelectButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            LandformOutlineColorSelectButton.IconColor = Color.Tan;
            LandformOutlineColorSelectButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LandformOutlineColorSelectButton.Location = new Point(6, 27);
            LandformOutlineColorSelectButton.Name = "LandformOutlineColorSelectButton";
            LandformOutlineColorSelectButton.Size = new Size(136, 60);
            LandformOutlineColorSelectButton.TabIndex = 19;
            LandformOutlineColorSelectButton.Text = "Click to Select";
            LandformOutlineColorSelectButton.UseVisualStyleBackColor = false;
            LandformOutlineColorSelectButton.Click += LandformOutlineColorSelectButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(38, 75);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 3;
            label2.Text = "Name";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // NameTextbox
            // 
            NameTextbox.Location = new Point(83, 72);
            NameTextbox.Name = "NameTextbox";
            NameTextbox.Size = new Size(251, 23);
            NameTextbox.TabIndex = 2;
            // 
            // GuidLabel
            // 
            GuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GuidLabel.ForeColor = SystemColors.ControlDarkDark;
            GuidLabel.Location = new Point(83, 56);
            GuidLabel.Name = "GuidLabel";
            GuidLabel.Size = new Size(253, 13);
            GuidLabel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(23, 56);
            label1.Name = "label1";
            label1.Size = new Size(54, 13);
            label1.TabIndex = 0;
            label1.Text = "Identifier";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ApplyThemeSettingsButton
            // 
            ApplyThemeSettingsButton.IconChar = FontAwesome.Sharp.IconChar.ListSquares;
            ApplyThemeSettingsButton.IconColor = Color.Black;
            ApplyThemeSettingsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ApplyThemeSettingsButton.IconSize = 20;
            ApplyThemeSettingsButton.Location = new Point(186, 409);
            ApplyThemeSettingsButton.Name = "ApplyThemeSettingsButton";
            ApplyThemeSettingsButton.Size = new Size(30, 30);
            ApplyThemeSettingsButton.TabIndex = 93;
            ApplyThemeSettingsButton.UseVisualStyleBackColor = true;
            ApplyThemeSettingsButton.Click += ApplyThemeSettingsButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(222, 417);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 94;
            label3.Text = "Apply Theme";
            // 
            // LandformInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(359, 592);
            Controls.Add(LandformDataPanel);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "LandformInfo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Landform Data";
            TopMost = true;
            TransparencyKey = Color.Fuchsia;
            LandformDataPanel.ResumeLayout(false);
            LandformDataPanel.PerformLayout();
            CoastlineGroup.ResumeLayout(false);
            CoastlineGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)CoastlineEffectDistanceTrack).EndInit();
            LandformGroup.ResumeLayout(false);
            LandformGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LandformOutlineWidthTrack).EndInit();
            ((System.ComponentModel.ISupportInitialize)LandformTexturePreviewPicture).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm LandformDataPanel;
        private Label GuidLabel;
        private Label label1;
        private Label label2;
        private TextBox NameTextbox;
        private GroupBox LandformGroup;
        private Label LandTextureNameLabel;
        private FontAwesome.Sharp.IconButton PreviousLandTextureButton;
        private FontAwesome.Sharp.IconButton NextLandTextureButton;
        private PictureBox LandformTexturePreviewPicture;
        private FontAwesome.Sharp.IconButton LandformOutlineColorSelectButton;
        private GroupBox CoastlineGroup;
        private Label label16;
        private Label label15;
        private ListBox CoastlineStyleList;
        private FontAwesome.Sharp.IconButton CoastlineColorSelectionButton;
        private Label label14;
        private TrackBar CoastlineEffectDistanceTrack;
        private Label StatusMessageLabel;
        private Button ApplyChangesButton;
        private Button CloseLandformDataButton;
        private CheckBox UseTextureForBackgroundCheck;
        private Label label13;
        private TrackBar LandformOutlineWidthTrack;
        private Label label88;
        private FontAwesome.Sharp.IconButton LandformBackgroundColorSelectButton;
        private FontAwesome.Sharp.IconButton ApplyThemeSettingsButton;
        private Label label3;
    }
}