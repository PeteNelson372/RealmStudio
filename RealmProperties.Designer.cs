namespace RealmStudio
{
    partial class RealmProperties
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
            RealmPropertiesOverlay = new ReaLTaiizor.Forms.DungeonForm();
            LockNameButton = new FontAwesome.Sharp.IconButton();
            GenerateRealmNameButton = new FontAwesome.Sharp.IconButton();
            RealmDescriptionButton = new FontAwesome.Sharp.IconButton();
            CloseRealmPropertiesButton = new Button();
            ApplyChangesButton = new Button();
            RealmTypeLabel = new Label();
            RealmTypeLbl = new Label();
            MapFilePathLabel = new Label();
            MapFilePathLbl = new Label();
            MapAreaLabel = new Label();
            MapAreaLbl = new Label();
            MapSizeLabel = new Label();
            MapSizeLbl = new Label();
            RealmNameLabel = new Label();
            NameTextbox = new TextBox();
            RealmGuidLabel = new Label();
            GuidLabel = new Label();
            RealmPropertiesOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // RealmPropertiesOverlay
            // 
            RealmPropertiesOverlay.BackColor = Color.FromArgb(244, 241, 243);
            RealmPropertiesOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            RealmPropertiesOverlay.Controls.Add(LockNameButton);
            RealmPropertiesOverlay.Controls.Add(GenerateRealmNameButton);
            RealmPropertiesOverlay.Controls.Add(RealmDescriptionButton);
            RealmPropertiesOverlay.Controls.Add(CloseRealmPropertiesButton);
            RealmPropertiesOverlay.Controls.Add(ApplyChangesButton);
            RealmPropertiesOverlay.Controls.Add(RealmTypeLabel);
            RealmPropertiesOverlay.Controls.Add(RealmTypeLbl);
            RealmPropertiesOverlay.Controls.Add(MapFilePathLabel);
            RealmPropertiesOverlay.Controls.Add(MapFilePathLbl);
            RealmPropertiesOverlay.Controls.Add(MapAreaLabel);
            RealmPropertiesOverlay.Controls.Add(MapAreaLbl);
            RealmPropertiesOverlay.Controls.Add(MapSizeLabel);
            RealmPropertiesOverlay.Controls.Add(MapSizeLbl);
            RealmPropertiesOverlay.Controls.Add(RealmNameLabel);
            RealmPropertiesOverlay.Controls.Add(NameTextbox);
            RealmPropertiesOverlay.Controls.Add(RealmGuidLabel);
            RealmPropertiesOverlay.Controls.Add(GuidLabel);
            RealmPropertiesOverlay.Dock = DockStyle.Fill;
            RealmPropertiesOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            RealmPropertiesOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            RealmPropertiesOverlay.Font = new Font("Segoe UI", 9F);
            RealmPropertiesOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            RealmPropertiesOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            RealmPropertiesOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            RealmPropertiesOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            RealmPropertiesOverlay.Location = new Point(0, 0);
            RealmPropertiesOverlay.MinimumSize = new Size(261, 65);
            RealmPropertiesOverlay.Name = "RealmPropertiesOverlay";
            RealmPropertiesOverlay.Padding = new Padding(20, 56, 20, 16);
            RealmPropertiesOverlay.RoundCorners = true;
            RealmPropertiesOverlay.Sizable = false;
            RealmPropertiesOverlay.Size = new Size(497, 321);
            RealmPropertiesOverlay.SmartBounds = true;
            RealmPropertiesOverlay.StartPosition = FormStartPosition.CenterParent;
            RealmPropertiesOverlay.TabIndex = 0;
            RealmPropertiesOverlay.Text = "Realm Properties";
            RealmPropertiesOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // LockNameButton
            // 
            LockNameButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
            LockNameButton.IconColor = SystemColors.ControlDarkDark;
            LockNameButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LockNameButton.IconSize = 14;
            LockNameButton.Location = new Point(444, 85);
            LockNameButton.Margin = new Padding(0);
            LockNameButton.Name = "LockNameButton";
            LockNameButton.Size = new Size(30, 28);
            LockNameButton.TabIndex = 166;
            LockNameButton.UseVisualStyleBackColor = true;
            LockNameButton.Click += LockNameButton_Click;
            // 
            // GenerateRealmNameButton
            // 
            GenerateRealmNameButton.IconChar = FontAwesome.Sharp.IconChar.FileSignature;
            GenerateRealmNameButton.IconColor = Color.Black;
            GenerateRealmNameButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            GenerateRealmNameButton.IconSize = 20;
            GenerateRealmNameButton.Location = new Point(411, 85);
            GenerateRealmNameButton.Name = "GenerateRealmNameButton";
            GenerateRealmNameButton.Size = new Size(30, 30);
            GenerateRealmNameButton.TabIndex = 165;
            GenerateRealmNameButton.UseVisualStyleBackColor = true;
            GenerateRealmNameButton.Click += GenerateRealmNameButton_Click;
            GenerateRealmNameButton.MouseHover += GenerateRealmNameButton_MouseHover;
            // 
            // RealmDescriptionButton
            // 
            RealmDescriptionButton.IconChar = FontAwesome.Sharp.IconChar.FileText;
            RealmDescriptionButton.IconColor = Color.Black;
            RealmDescriptionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            RealmDescriptionButton.IconSize = 20;
            RealmDescriptionButton.Location = new Point(455, 12);
            RealmDescriptionButton.Name = "RealmDescriptionButton";
            RealmDescriptionButton.Size = new Size(30, 30);
            RealmDescriptionButton.TabIndex = 96;
            RealmDescriptionButton.UseVisualStyleBackColor = true;
            RealmDescriptionButton.Click += RealmDescriptionButton_Click;
            RealmDescriptionButton.MouseHover += RealmDescriptionButton_MouseHover;
            // 
            // CloseRealmPropertiesButton
            // 
            CloseRealmPropertiesButton.DialogResult = DialogResult.Cancel;
            CloseRealmPropertiesButton.ForeColor = SystemColors.ControlDarkDark;
            CloseRealmPropertiesButton.Location = new Point(414, 242);
            CloseRealmPropertiesButton.Name = "CloseRealmPropertiesButton";
            CloseRealmPropertiesButton.Size = new Size(60, 60);
            CloseRealmPropertiesButton.TabIndex = 93;
            CloseRealmPropertiesButton.Text = "&Cancel";
            CloseRealmPropertiesButton.UseVisualStyleBackColor = true;
            // 
            // ApplyChangesButton
            // 
            ApplyChangesButton.DialogResult = DialogResult.OK;
            ApplyChangesButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyChangesButton.Location = new Point(348, 242);
            ApplyChangesButton.Name = "ApplyChangesButton";
            ApplyChangesButton.Size = new Size(60, 60);
            ApplyChangesButton.TabIndex = 92;
            ApplyChangesButton.Text = "&Apply";
            ApplyChangesButton.UseVisualStyleBackColor = true;
            // 
            // RealmTypeLabel
            // 
            RealmTypeLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RealmTypeLabel.ForeColor = SystemColors.ControlDarkDark;
            RealmTypeLabel.Location = new Point(107, 201);
            RealmTypeLabel.Name = "RealmTypeLabel";
            RealmTypeLabel.Size = new Size(298, 13);
            RealmTypeLabel.TabIndex = 15;
            // 
            // RealmTypeLbl
            // 
            RealmTypeLbl.AutoSize = true;
            RealmTypeLbl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RealmTypeLbl.ForeColor = SystemColors.ControlDarkDark;
            RealmTypeLbl.Location = new Point(24, 201);
            RealmTypeLbl.Name = "RealmTypeLbl";
            RealmTypeLbl.Size = new Size(63, 13);
            RealmTypeLbl.TabIndex = 14;
            RealmTypeLbl.Text = "Realm Type";
            RealmTypeLbl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MapFilePathLabel
            // 
            MapFilePathLabel.AutoEllipsis = true;
            MapFilePathLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapFilePathLabel.ForeColor = SystemColors.ControlDarkDark;
            MapFilePathLabel.Location = new Point(107, 172);
            MapFilePathLabel.Name = "MapFilePathLabel";
            MapFilePathLabel.Size = new Size(298, 13);
            MapFilePathLabel.TabIndex = 13;
            // 
            // MapFilePathLbl
            // 
            MapFilePathLbl.AutoSize = true;
            MapFilePathLbl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapFilePathLbl.ForeColor = SystemColors.ControlDarkDark;
            MapFilePathLbl.Location = new Point(24, 172);
            MapFilePathLbl.Name = "MapFilePathLbl";
            MapFilePathLbl.Size = new Size(77, 13);
            MapFilePathLbl.TabIndex = 12;
            MapFilePathLbl.Text = "Map File Path";
            MapFilePathLbl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MapAreaLabel
            // 
            MapAreaLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapAreaLabel.ForeColor = SystemColors.ControlDarkDark;
            MapAreaLabel.Location = new Point(107, 145);
            MapAreaLabel.Name = "MapAreaLabel";
            MapAreaLabel.Size = new Size(298, 13);
            MapAreaLabel.TabIndex = 11;
            // 
            // MapAreaLbl
            // 
            MapAreaLbl.AutoSize = true;
            MapAreaLbl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapAreaLbl.ForeColor = SystemColors.ControlDarkDark;
            MapAreaLbl.Location = new Point(24, 145);
            MapAreaLbl.Name = "MapAreaLbl";
            MapAreaLbl.Size = new Size(56, 13);
            MapAreaLbl.TabIndex = 10;
            MapAreaLbl.Text = "Map Area";
            MapAreaLbl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MapSizeLabel
            // 
            MapSizeLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapSizeLabel.ForeColor = SystemColors.ControlDarkDark;
            MapSizeLabel.Location = new Point(107, 120);
            MapSizeLabel.Name = "MapSizeLabel";
            MapSizeLabel.Size = new Size(298, 13);
            MapSizeLabel.TabIndex = 9;
            // 
            // MapSizeLbl
            // 
            MapSizeLbl.AutoSize = true;
            MapSizeLbl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapSizeLbl.ForeColor = SystemColors.ControlDarkDark;
            MapSizeLbl.Location = new Point(23, 120);
            MapSizeLbl.Name = "MapSizeLbl";
            MapSizeLbl.Size = new Size(53, 13);
            MapSizeLbl.TabIndex = 8;
            MapSizeLbl.Text = "Map Size";
            MapSizeLbl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // RealmNameLabel
            // 
            RealmNameLabel.AutoSize = true;
            RealmNameLabel.ForeColor = SystemColors.ControlDarkDark;
            RealmNameLabel.Location = new Point(38, 92);
            RealmNameLabel.Name = "RealmNameLabel";
            RealmNameLabel.Size = new Size(39, 15);
            RealmNameLabel.TabIndex = 7;
            RealmNameLabel.Text = "Name";
            RealmNameLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // NameTextbox
            // 
            NameTextbox.Location = new Point(107, 89);
            NameTextbox.Name = "NameTextbox";
            NameTextbox.Size = new Size(298, 23);
            NameTextbox.TabIndex = 6;
            // 
            // RealmGuidLabel
            // 
            RealmGuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RealmGuidLabel.ForeColor = SystemColors.ControlDarkDark;
            RealmGuidLabel.Location = new Point(107, 68);
            RealmGuidLabel.Name = "RealmGuidLabel";
            RealmGuidLabel.Size = new Size(286, 13);
            RealmGuidLabel.TabIndex = 5;
            // 
            // GuidLabel
            // 
            GuidLabel.AutoSize = true;
            GuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GuidLabel.ForeColor = SystemColors.ControlDarkDark;
            GuidLabel.Location = new Point(23, 68);
            GuidLabel.Name = "GuidLabel";
            GuidLabel.Size = new Size(54, 13);
            GuidLabel.TabIndex = 4;
            GuidLabel.Text = "Identifier";
            GuidLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // RealmProperties
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(497, 321);
            Controls.Add(RealmPropertiesOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "RealmProperties";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Realm Properties";
            TransparencyKey = Color.Fuchsia;
            RealmPropertiesOverlay.ResumeLayout(false);
            RealmPropertiesOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm RealmPropertiesOverlay;
        private Label MapSizeLabel;
        private Label MapSizeLbl;
        private Label RealmNameLabel;
        private Label RealmGuidLabel;
        private Label GuidLabel;
        private Label MapAreaLabel;
        private Label MapAreaLbl;
        private Label MapFilePathLabel;
        private Label MapFilePathLbl;
        private Label RealmTypeLabel;
        private Label RealmTypeLbl;
        public TextBox NameTextbox;
        private Button ApplyChangesButton;
        private Button CloseRealmPropertiesButton;
        private FontAwesome.Sharp.IconButton RealmDescriptionButton;
        internal FontAwesome.Sharp.IconButton LockNameButton;
        private FontAwesome.Sharp.IconButton GenerateRealmNameButton;
    }
}