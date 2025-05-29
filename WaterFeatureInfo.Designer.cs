namespace RealmStudio
{
    partial class WaterFeatureInfo
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
            WaterFeatureInfoOverlay = new ReaLTaiizor.Forms.DungeonForm();
            LockNameButton = new FontAwesome.Sharp.IconButton();
            GenerateWaterFeatureNameButton = new FontAwesome.Sharp.IconButton();
            WaterFeatureDescriptionButton = new FontAwesome.Sharp.IconButton();
            StatusMessageLabel = new Label();
            CloseWaterFeatureDataButton = new Button();
            ApplyChangesButton = new Button();
            label2 = new Label();
            NameTextbox = new TextBox();
            GuidLabel = new Label();
            label1 = new Label();
            ShorelineColorSelectionButton = new FontAwesome.Sharp.IconButton();
            WaterColorSelectionButton = new FontAwesome.Sharp.IconButton();
            WaterFeatureInfoOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // WaterFeatureInfoOverlay
            // 
            WaterFeatureInfoOverlay.BackColor = Color.FromArgb(244, 241, 243);
            WaterFeatureInfoOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            WaterFeatureInfoOverlay.Controls.Add(LockNameButton);
            WaterFeatureInfoOverlay.Controls.Add(GenerateWaterFeatureNameButton);
            WaterFeatureInfoOverlay.Controls.Add(WaterFeatureDescriptionButton);
            WaterFeatureInfoOverlay.Controls.Add(StatusMessageLabel);
            WaterFeatureInfoOverlay.Controls.Add(CloseWaterFeatureDataButton);
            WaterFeatureInfoOverlay.Controls.Add(ApplyChangesButton);
            WaterFeatureInfoOverlay.Controls.Add(label2);
            WaterFeatureInfoOverlay.Controls.Add(NameTextbox);
            WaterFeatureInfoOverlay.Controls.Add(GuidLabel);
            WaterFeatureInfoOverlay.Controls.Add(label1);
            WaterFeatureInfoOverlay.Controls.Add(ShorelineColorSelectionButton);
            WaterFeatureInfoOverlay.Controls.Add(WaterColorSelectionButton);
            WaterFeatureInfoOverlay.Dock = DockStyle.Fill;
            WaterFeatureInfoOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            WaterFeatureInfoOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            WaterFeatureInfoOverlay.Font = new Font("Segoe UI", 9F);
            WaterFeatureInfoOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            WaterFeatureInfoOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            WaterFeatureInfoOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            WaterFeatureInfoOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            WaterFeatureInfoOverlay.Location = new Point(0, 0);
            WaterFeatureInfoOverlay.Name = "WaterFeatureInfoOverlay";
            WaterFeatureInfoOverlay.Padding = new Padding(20, 56, 20, 16);
            WaterFeatureInfoOverlay.RoundCorners = true;
            WaterFeatureInfoOverlay.Sizable = true;
            WaterFeatureInfoOverlay.Size = new Size(389, 284);
            WaterFeatureInfoOverlay.SmartBounds = true;
            WaterFeatureInfoOverlay.StartPosition = FormStartPosition.CenterParent;
            WaterFeatureInfoOverlay.TabIndex = 0;
            WaterFeatureInfoOverlay.Text = "Water Feature Info";
            WaterFeatureInfoOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // LockNameButton
            // 
            LockNameButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
            LockNameButton.IconColor = SystemColors.ControlDarkDark;
            LockNameButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LockNameButton.IconSize = 14;
            LockNameButton.Location = new Point(336, 69);
            LockNameButton.Margin = new Padding(0);
            LockNameButton.Name = "LockNameButton";
            LockNameButton.Size = new Size(30, 28);
            LockNameButton.TabIndex = 111;
            LockNameButton.UseVisualStyleBackColor = true;
            LockNameButton.Click += LockNameButton_Click;
            // 
            // GenerateWaterFeatureNameButton
            // 
            GenerateWaterFeatureNameButton.IconChar = FontAwesome.Sharp.IconChar.FileSignature;
            GenerateWaterFeatureNameButton.IconColor = Color.Black;
            GenerateWaterFeatureNameButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            GenerateWaterFeatureNameButton.IconSize = 20;
            GenerateWaterFeatureNameButton.Location = new Point(303, 67);
            GenerateWaterFeatureNameButton.Name = "GenerateWaterFeatureNameButton";
            GenerateWaterFeatureNameButton.Size = new Size(30, 30);
            GenerateWaterFeatureNameButton.TabIndex = 110;
            GenerateWaterFeatureNameButton.UseVisualStyleBackColor = true;
            GenerateWaterFeatureNameButton.Click += GenerateWaterFeatureNameButton_Click;
            GenerateWaterFeatureNameButton.MouseHover += GenerateWaterFeatureNameButton_MouseHover;
            // 
            // WaterFeatureDescriptionButton
            // 
            WaterFeatureDescriptionButton.IconChar = FontAwesome.Sharp.IconChar.FileText;
            WaterFeatureDescriptionButton.IconColor = Color.Black;
            WaterFeatureDescriptionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            WaterFeatureDescriptionButton.IconSize = 20;
            WaterFeatureDescriptionButton.Location = new Point(347, 12);
            WaterFeatureDescriptionButton.Name = "WaterFeatureDescriptionButton";
            WaterFeatureDescriptionButton.Size = new Size(30, 30);
            WaterFeatureDescriptionButton.TabIndex = 109;
            WaterFeatureDescriptionButton.UseVisualStyleBackColor = true;
            WaterFeatureDescriptionButton.Click += WaterFeatureDescriptionButton_Click;
            WaterFeatureDescriptionButton.MouseHover += WaterFeatureDescriptionButton_MouseHover;
            // 
            // StatusMessageLabel
            // 
            StatusMessageLabel.Location = new Point(224, 165);
            StatusMessageLabel.Name = "StatusMessageLabel";
            StatusMessageLabel.Size = new Size(64, 22);
            StatusMessageLabel.TabIndex = 95;
            // 
            // CloseWaterFeatureDataButton
            // 
            CloseWaterFeatureDataButton.ForeColor = SystemColors.ControlDarkDark;
            CloseWaterFeatureDataButton.Location = new Point(306, 190);
            CloseWaterFeatureDataButton.Name = "CloseWaterFeatureDataButton";
            CloseWaterFeatureDataButton.Size = new Size(60, 60);
            CloseWaterFeatureDataButton.TabIndex = 94;
            CloseWaterFeatureDataButton.Text = "&Close";
            CloseWaterFeatureDataButton.UseVisualStyleBackColor = true;
            CloseWaterFeatureDataButton.Click += CloseWaterFeatureDataButton_Click;
            // 
            // ApplyChangesButton
            // 
            ApplyChangesButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyChangesButton.Location = new Point(224, 190);
            ApplyChangesButton.Name = "ApplyChangesButton";
            ApplyChangesButton.Size = new Size(60, 60);
            ApplyChangesButton.TabIndex = 93;
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
            label2.TabIndex = 27;
            label2.Text = "Name";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // NameTextbox
            // 
            NameTextbox.Location = new Point(83, 72);
            NameTextbox.Name = "NameTextbox";
            NameTextbox.Size = new Size(214, 23);
            NameTextbox.TabIndex = 26;
            // 
            // GuidLabel
            // 
            GuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GuidLabel.ForeColor = SystemColors.ControlDarkDark;
            GuidLabel.Location = new Point(83, 56);
            GuidLabel.Name = "GuidLabel";
            GuidLabel.Size = new Size(286, 13);
            GuidLabel.TabIndex = 25;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(23, 56);
            label1.Name = "label1";
            label1.Size = new Size(54, 13);
            label1.TabIndex = 24;
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
            ShorelineColorSelectionButton.TabIndex = 23;
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
            WaterColorSelectionButton.TabIndex = 22;
            WaterColorSelectionButton.Text = "Click to Select";
            WaterColorSelectionButton.UseVisualStyleBackColor = false;
            WaterColorSelectionButton.Click += WaterColorSelectionButton_Click;
            // 
            // WaterFeatureInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(389, 284);
            Controls.Add(WaterFeatureInfoOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "WaterFeatureInfo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Water Feature Info";
            TransparencyKey = Color.Fuchsia;
            WaterFeatureInfoOverlay.ResumeLayout(false);
            WaterFeatureInfoOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm WaterFeatureInfoOverlay;
        private FontAwesome.Sharp.IconButton ShorelineColorSelectionButton;
        private FontAwesome.Sharp.IconButton WaterColorSelectionButton;
        private Label label2;
        private TextBox NameTextbox;
        private Label GuidLabel;
        private Label label1;
        private Button CloseWaterFeatureDataButton;
        private Button ApplyChangesButton;
        private Label StatusMessageLabel;
        private FontAwesome.Sharp.IconButton WaterFeatureDescriptionButton;
        private FontAwesome.Sharp.IconButton GenerateWaterFeatureNameButton;
        internal FontAwesome.Sharp.IconButton LockNameButton;
    }
}