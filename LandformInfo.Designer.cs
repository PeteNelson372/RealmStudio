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
            groupBox2 = new GroupBox();
            label16 = new Label();
            label15 = new Label();
            CoastlineStyleList = new ListBox();
            CoastlineColorSelectionButton = new FontAwesome.Sharp.IconButton();
            label14 = new Label();
            CoastlineEffectDistanceTrack = new TrackBar();
            groupBox1 = new GroupBox();
            LandTextureNameLabel = new Label();
            label13 = new Label();
            PreviousLandTextureButton = new FontAwesome.Sharp.IconButton();
            NextLandTextureButton = new FontAwesome.Sharp.IconButton();
            LandformTexturePreviewPicture = new PictureBox();
            LandformOutlineColorSelectButton = new FontAwesome.Sharp.IconButton();
            label2 = new Label();
            NameTextbox = new TextBox();
            GuidLabel = new Label();
            label1 = new Label();
            LandformDataPanel.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CoastlineEffectDistanceTrack).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LandformTexturePreviewPicture).BeginInit();
            SuspendLayout();
            // 
            // LandformDataPanel
            // 
            LandformDataPanel.BackColor = Color.FromArgb(244, 241, 243);
            LandformDataPanel.BorderColor = Color.FromArgb(38, 38, 38);
            LandformDataPanel.Controls.Add(CloseLandformDataButton);
            LandformDataPanel.Controls.Add(ApplyChangesButton);
            LandformDataPanel.Controls.Add(StatusMessageLabel);
            LandformDataPanel.Controls.Add(groupBox2);
            LandformDataPanel.Controls.Add(groupBox1);
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
            LandformDataPanel.Size = new Size(392, 495);
            LandformDataPanel.SmartBounds = true;
            LandformDataPanel.StartPosition = FormStartPosition.CenterParent;
            LandformDataPanel.TabIndex = 0;
            LandformDataPanel.Text = "Landform Data";
            LandformDataPanel.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CloseLandformDataButton
            // 
            CloseLandformDataButton.ForeColor = SystemColors.ControlDarkDark;
            CloseLandformDataButton.Location = new Point(303, 409);
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
            ApplyChangesButton.Location = new Point(221, 409);
            ApplyChangesButton.Name = "ApplyChangesButton";
            ApplyChangesButton.Size = new Size(60, 60);
            ApplyChangesButton.TabIndex = 91;
            ApplyChangesButton.Text = "&Apply";
            ApplyChangesButton.UseVisualStyleBackColor = true;
            ApplyChangesButton.Click += ApplyChangesButton_Click;
            // 
            // StatusMessageLabel
            // 
            StatusMessageLabel.Location = new Point(34, 448);
            StatusMessageLabel.Name = "StatusMessageLabel";
            StatusMessageLabel.Size = new Size(64, 22);
            StatusMessageLabel.TabIndex = 90;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label16);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(CoastlineStyleList);
            groupBox2.Controls.Add(CoastlineColorSelectionButton);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(CoastlineEffectDistanceTrack);
            groupBox2.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(221, 101);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(148, 302);
            groupBox2.TabIndex = 19;
            groupBox2.TabStop = false;
            groupBox2.Text = "Coastline";
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
            // groupBox1
            // 
            groupBox1.BackColor = Color.Transparent;
            groupBox1.Controls.Add(LandTextureNameLabel);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(PreviousLandTextureButton);
            groupBox1.Controls.Add(NextLandTextureButton);
            groupBox1.Controls.Add(LandformTexturePreviewPicture);
            groupBox1.Controls.Add(LandformOutlineColorSelectButton);
            groupBox1.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = SystemColors.ControlText;
            groupBox1.Location = new Point(23, 101);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(148, 302);
            groupBox1.TabIndex = 18;
            groupBox1.TabStop = false;
            groupBox1.Text = "Landform";
            // 
            // LandTextureNameLabel
            // 
            LandTextureNameLabel.AutoEllipsis = true;
            LandTextureNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LandTextureNameLabel.Location = new Point(6, 266);
            LandTextureNameLabel.Name = "LandTextureNameLabel";
            LandTextureNameLabel.Size = new Size(136, 21);
            LandTextureNameLabel.TabIndex = 24;
            LandTextureNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.BackColor = Color.Transparent;
            label13.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label13.ForeColor = SystemColors.ControlDarkDark;
            label13.Location = new Point(6, 109);
            label13.Name = "label13";
            label13.Size = new Size(45, 15);
            label13.TabIndex = 23;
            label13.Text = "Texture";
            // 
            // PreviousLandTextureButton
            // 
            PreviousLandTextureButton.FlatStyle = FlatStyle.Flat;
            PreviousLandTextureButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PreviousLandTextureButton.IconChar = FontAwesome.Sharp.IconChar.AngleLeft;
            PreviousLandTextureButton.IconColor = Color.Black;
            PreviousLandTextureButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            PreviousLandTextureButton.IconSize = 24;
            PreviousLandTextureButton.Location = new Point(6, 239);
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
            NextLandTextureButton.Location = new Point(118, 239);
            NextLandTextureButton.Name = "NextLandTextureButton";
            NextLandTextureButton.Size = new Size(24, 24);
            NextLandTextureButton.TabIndex = 21;
            NextLandTextureButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            NextLandTextureButton.UseVisualStyleBackColor = true;
            NextLandTextureButton.Click += NextTextureButton_Click;
            // 
            // LandformTexturePreviewPicture
            // 
            LandformTexturePreviewPicture.Location = new Point(6, 127);
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
            NameTextbox.Size = new Size(286, 23);
            NameTextbox.TabIndex = 2;
            // 
            // GuidLabel
            // 
            GuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GuidLabel.ForeColor = SystemColors.ControlDarkDark;
            GuidLabel.Location = new Point(83, 56);
            GuidLabel.Name = "GuidLabel";
            GuidLabel.Size = new Size(286, 13);
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
            // LandformInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(392, 495);
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
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)CoastlineEffectDistanceTrack).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LandformTexturePreviewPicture).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm LandformDataPanel;
        private Label GuidLabel;
        private Label label1;
        private Label label2;
        private TextBox NameTextbox;
        private GroupBox groupBox1;
        private Label LandTextureNameLabel;
        private Label label13;
        private FontAwesome.Sharp.IconButton PreviousLandTextureButton;
        private FontAwesome.Sharp.IconButton NextLandTextureButton;
        private PictureBox LandformTexturePreviewPicture;
        private FontAwesome.Sharp.IconButton LandformOutlineColorSelectButton;
        private GroupBox groupBox2;
        private Label label16;
        private Label label15;
        private ListBox CoastlineStyleList;
        private FontAwesome.Sharp.IconButton CoastlineColorSelectionButton;
        private Label label14;
        private TrackBar CoastlineEffectDistanceTrack;
        private Label StatusMessageLabel;
        private Button ApplyChangesButton;
        private Button CloseLandformDataButton;
    }
}