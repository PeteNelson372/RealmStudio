namespace RealmStudioX
{
    partial class RiverInfo
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
            RiverInfoOverlay = new ReaLTaiizor.Forms.DungeonForm();
            LockNameButton = new FontAwesome.Sharp.IconButton();
            GenerateRiverNameButton = new FontAwesome.Sharp.IconButton();
            RiverDescriptionButton = new FontAwesome.Sharp.IconButton();
            StatusMessageLabel = new Label();
            label41 = new Label();
            RiverSourceFadeInSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            label40 = new Label();
            RiverWidthTrack = new TrackBar();
            CloseRiverFeatureDataButton = new Button();
            ApplyChangesButton = new Button();
            label2 = new Label();
            NameTextbox = new TextBox();
            GuidLabel = new Label();
            label1 = new Label();
            ShorelineColorSelectionButton = new FontAwesome.Sharp.IconButton();
            WaterColorSelectionButton = new FontAwesome.Sharp.IconButton();
            RiverInfoOverlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RiverWidthTrack).BeginInit();
            SuspendLayout();
            // 
            // RiverInfoOverlay
            // 
            RiverInfoOverlay.BackColor = Color.FromArgb(244, 241, 243);
            RiverInfoOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            RiverInfoOverlay.Controls.Add(LockNameButton);
            RiverInfoOverlay.Controls.Add(GenerateRiverNameButton);
            RiverInfoOverlay.Controls.Add(RiverDescriptionButton);
            RiverInfoOverlay.Controls.Add(StatusMessageLabel);
            RiverInfoOverlay.Controls.Add(label41);
            RiverInfoOverlay.Controls.Add(RiverSourceFadeInSwitch);
            RiverInfoOverlay.Controls.Add(label40);
            RiverInfoOverlay.Controls.Add(RiverWidthTrack);
            RiverInfoOverlay.Controls.Add(CloseRiverFeatureDataButton);
            RiverInfoOverlay.Controls.Add(ApplyChangesButton);
            RiverInfoOverlay.Controls.Add(label2);
            RiverInfoOverlay.Controls.Add(NameTextbox);
            RiverInfoOverlay.Controls.Add(GuidLabel);
            RiverInfoOverlay.Controls.Add(label1);
            RiverInfoOverlay.Controls.Add(ShorelineColorSelectionButton);
            RiverInfoOverlay.Controls.Add(WaterColorSelectionButton);
            RiverInfoOverlay.Dock = DockStyle.Fill;
            RiverInfoOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            RiverInfoOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            RiverInfoOverlay.Font = new Font("Segoe UI", 9F);
            RiverInfoOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            RiverInfoOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            RiverInfoOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            RiverInfoOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            RiverInfoOverlay.Location = new Point(0, 0);
            RiverInfoOverlay.Name = "RiverInfoOverlay";
            RiverInfoOverlay.Padding = new Padding(20, 56, 20, 16);
            RiverInfoOverlay.RoundCorners = true;
            RiverInfoOverlay.Sizable = true;
            RiverInfoOverlay.Size = new Size(394, 363);
            RiverInfoOverlay.SmartBounds = true;
            RiverInfoOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            RiverInfoOverlay.TabIndex = 0;
            RiverInfoOverlay.Text = "River Info";
            RiverInfoOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // LockNameButton
            // 
            LockNameButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
            LockNameButton.IconColor = SystemColors.ControlDarkDark;
            LockNameButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LockNameButton.IconSize = 14;
            LockNameButton.Location = new Point(341, 69);
            LockNameButton.Margin = new Padding(0);
            LockNameButton.Name = "LockNameButton";
            LockNameButton.Size = new Size(30, 28);
            LockNameButton.TabIndex = 112;
            LockNameButton.UseVisualStyleBackColor = true;
            LockNameButton.Click += LockNameButton_Click;
            // 
            // GenerateRiverNameButton
            // 
            GenerateRiverNameButton.IconChar = FontAwesome.Sharp.IconChar.FileSignature;
            GenerateRiverNameButton.IconColor = Color.Black;
            GenerateRiverNameButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            GenerateRiverNameButton.IconSize = 20;
            GenerateRiverNameButton.Location = new Point(308, 67);
            GenerateRiverNameButton.Name = "GenerateRiverNameButton";
            GenerateRiverNameButton.Size = new Size(30, 30);
            GenerateRiverNameButton.TabIndex = 111;
            GenerateRiverNameButton.UseVisualStyleBackColor = true;
            GenerateRiverNameButton.Click += GenerateRiverNameButton_Click;
            GenerateRiverNameButton.MouseHover += GenerateRiverNameButton_MouseHover;
            // 
            // RiverDescriptionButton
            // 
            RiverDescriptionButton.IconChar = FontAwesome.Sharp.IconChar.FileText;
            RiverDescriptionButton.IconColor = Color.Black;
            RiverDescriptionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            RiverDescriptionButton.IconSize = 20;
            RiverDescriptionButton.Location = new Point(352, 12);
            RiverDescriptionButton.Name = "RiverDescriptionButton";
            RiverDescriptionButton.Size = new Size(30, 30);
            RiverDescriptionButton.TabIndex = 108;
            RiverDescriptionButton.UseVisualStyleBackColor = true;
            RiverDescriptionButton.Click += RiverDescriptionButton_Click;
            RiverDescriptionButton.MouseHover += RiverDescriptionButton_MouseHover;
            // 
            // StatusMessageLabel
            // 
            StatusMessageLabel.Location = new Point(232, 257);
            StatusMessageLabel.Name = "StatusMessageLabel";
            StatusMessageLabel.Size = new Size(64, 22);
            StatusMessageLabel.TabIndex = 107;
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.BackColor = Color.Transparent;
            label41.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label41.ForeColor = SystemColors.ControlDarkDark;
            label41.Location = new Point(70, 312);
            label41.Name = "label41";
            label41.Size = new Size(86, 15);
            label41.TabIndex = 106;
            label41.Text = "Source Fade-In";
            // 
            // RiverSourceFadeInSwitch
            // 
            RiverSourceFadeInSwitch.Alpha = 50;
            RiverSourceFadeInSwitch.BackColor = Color.Transparent;
            RiverSourceFadeInSwitch.Background = true;
            RiverSourceFadeInSwitch.Background_WidthPen = 2F;
            RiverSourceFadeInSwitch.BackgroundPen = false;
            RiverSourceFadeInSwitch.Checked = true;
            RiverSourceFadeInSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            RiverSourceFadeInSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            RiverSourceFadeInSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            RiverSourceFadeInSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            RiverSourceFadeInSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            RiverSourceFadeInSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            RiverSourceFadeInSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            RiverSourceFadeInSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            RiverSourceFadeInSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            RiverSourceFadeInSwitch.ColorValue = Color.ForestGreen;
            RiverSourceFadeInSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            RiverSourceFadeInSwitch.Font = new Font("Arial", 11F);
            RiverSourceFadeInSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            RiverSourceFadeInSwitch.Lighting = true;
            RiverSourceFadeInSwitch.LinearGradient_Background = false;
            RiverSourceFadeInSwitch.LinearGradient_Value = false;
            RiverSourceFadeInSwitch.LinearGradientPen = false;
            RiverSourceFadeInSwitch.Location = new Point(23, 307);
            RiverSourceFadeInSwitch.Name = "RiverSourceFadeInSwitch";
            RiverSourceFadeInSwitch.PenWidth = 10;
            RiverSourceFadeInSwitch.RGB = false;
            RiverSourceFadeInSwitch.Rounding = true;
            RiverSourceFadeInSwitch.RoundingInt = 90;
            RiverSourceFadeInSwitch.Size = new Size(41, 20);
            RiverSourceFadeInSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            RiverSourceFadeInSwitch.TabIndex = 105;
            RiverSourceFadeInSwitch.Tag = "Cyber";
            RiverSourceFadeInSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            RiverSourceFadeInSwitch.Timer_RGB = 300;
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.BackColor = Color.Transparent;
            label40.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label40.ForeColor = SystemColors.ControlDarkDark;
            label40.Location = new Point(23, 264);
            label40.Name = "label40";
            label40.Size = new Size(39, 15);
            label40.TabIndex = 104;
            label40.Text = "Width";
            // 
            // RiverWidthTrack
            // 
            RiverWidthTrack.AutoSize = false;
            RiverWidthTrack.BackColor = SystemColors.Control;
            RiverWidthTrack.Location = new Point(23, 282);
            RiverWidthTrack.Maximum = 16;
            RiverWidthTrack.Minimum = 2;
            RiverWidthTrack.Name = "RiverWidthTrack";
            RiverWidthTrack.Size = new Size(136, 20);
            RiverWidthTrack.TabIndex = 103;
            RiverWidthTrack.TickStyle = TickStyle.None;
            RiverWidthTrack.Value = 4;
            RiverWidthTrack.Scroll += RiverWidthTrack_Scroll;
            // 
            // CloseRiverFeatureDataButton
            // 
            CloseRiverFeatureDataButton.ForeColor = SystemColors.ControlDarkDark;
            CloseRiverFeatureDataButton.Location = new Point(311, 284);
            CloseRiverFeatureDataButton.Name = "CloseRiverFeatureDataButton";
            CloseRiverFeatureDataButton.Size = new Size(60, 60);
            CloseRiverFeatureDataButton.TabIndex = 102;
            CloseRiverFeatureDataButton.Text = "&Close";
            CloseRiverFeatureDataButton.UseVisualStyleBackColor = true;
            CloseRiverFeatureDataButton.Click += CloseRiverFeatureDataButton_Click;
            // 
            // ApplyChangesButton
            // 
            ApplyChangesButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyChangesButton.Location = new Point(232, 284);
            ApplyChangesButton.Name = "ApplyChangesButton";
            ApplyChangesButton.Size = new Size(60, 60);
            ApplyChangesButton.TabIndex = 101;
            ApplyChangesButton.Text = "&Apply";
            ApplyChangesButton.UseVisualStyleBackColor = true;
            ApplyChangesButton.Click += ApplyChangesButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(23, 75);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 100;
            label2.Text = "Name";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // NameTextbox
            // 
            NameTextbox.Location = new Point(83, 72);
            NameTextbox.Name = "NameTextbox";
            NameTextbox.Size = new Size(219, 23);
            NameTextbox.TabIndex = 99;
            // 
            // GuidLabel
            // 
            GuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GuidLabel.ForeColor = SystemColors.ControlDarkDark;
            GuidLabel.Location = new Point(83, 56);
            GuidLabel.Name = "GuidLabel";
            GuidLabel.Size = new Size(286, 13);
            GuidLabel.TabIndex = 98;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(23, 56);
            label1.Name = "label1";
            label1.Size = new Size(54, 13);
            label1.TabIndex = 97;
            label1.Text = "Identifier";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ShorelineColorSelectionButton
            // 
            ShorelineColorSelectionButton.BackColor = Color.FromArgb(161, 144, 118);
            ShorelineColorSelectionButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShorelineColorSelectionButton.ForeColor = SystemColors.HighlightText;
            ShorelineColorSelectionButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            ShorelineColorSelectionButton.IconColor = Color.Tan;
            ShorelineColorSelectionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ShorelineColorSelectionButton.Location = new Point(23, 190);
            ShorelineColorSelectionButton.Name = "ShorelineColorSelectionButton";
            ShorelineColorSelectionButton.Size = new Size(136, 60);
            ShorelineColorSelectionButton.TabIndex = 96;
            ShorelineColorSelectionButton.Text = "Click to Select";
            ShorelineColorSelectionButton.UseVisualStyleBackColor = false;
            ShorelineColorSelectionButton.Click += ShorelineColorSelectionButton_Click;
            // 
            // WaterColorSelectionButton
            // 
            WaterColorSelectionButton.BackColor = Color.FromArgb(101, 140, 191, 197);
            WaterColorSelectionButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            WaterColorSelectionButton.ForeColor = SystemColors.HighlightText;
            WaterColorSelectionButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            WaterColorSelectionButton.IconColor = Color.Tan;
            WaterColorSelectionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            WaterColorSelectionButton.Location = new Point(23, 112);
            WaterColorSelectionButton.Name = "WaterColorSelectionButton";
            WaterColorSelectionButton.Size = new Size(136, 60);
            WaterColorSelectionButton.TabIndex = 95;
            WaterColorSelectionButton.Text = "Click to Select";
            WaterColorSelectionButton.UseVisualStyleBackColor = false;
            WaterColorSelectionButton.Click += WaterColorSelectionButton_Click;
            // 
            // RiverInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(394, 363);
            Controls.Add(RiverInfoOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "RiverInfo";
            Text = "River Info";
            TransparencyKey = Color.Fuchsia;
            RiverInfoOverlay.ResumeLayout(false);
            RiverInfoOverlay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RiverWidthTrack).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm RiverInfoOverlay;
        private Button CloseRiverFeatureDataButton;
        private Button ApplyChangesButton;
        private Label label2;
        private TextBox NameTextbox;
        private Label GuidLabel;
        private Label label1;
        private FontAwesome.Sharp.IconButton ShorelineColorSelectionButton;
        private FontAwesome.Sharp.IconButton WaterColorSelectionButton;
        private Label label41;
        private ReaLTaiizor.Controls.CyberSwitch RiverSourceFadeInSwitch;
        private Label label40;
        private TrackBar RiverWidthTrack;
        private Label StatusMessageLabel;
        private FontAwesome.Sharp.IconButton RiverDescriptionButton;
        private FontAwesome.Sharp.IconButton GenerateRiverNameButton;
        internal FontAwesome.Sharp.IconButton LockNameButton;
    }
}