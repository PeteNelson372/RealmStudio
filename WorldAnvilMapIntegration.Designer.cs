namespace RealmStudio
{
    partial class WorldAnvilMapIntegration
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
            WAMapIntegrationOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CreateMapArticleButton = new FontAwesome.Sharp.IconButton();
            CreateMapButton = new FontAwesome.Sharp.IconButton();
            ImageIdValidButton = new FontAwesome.Sharp.IconButton();
            ValidateImageIdButton = new Button();
            label5 = new Label();
            MapImageIdTextBox = new TextBox();
            MapTitleLabel = new Label();
            MapTitleTextBox = new TextBox();
            MapUserIdLabel = new Label();
            label4 = new Label();
            MapWorldIdLabel = new Label();
            label3 = new Label();
            MapIdLabel = new Label();
            MapGuidLabel = new Label();
            CloseButton = new Button();
            WAMapIntegrationOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // WAMapIntegrationOverlay
            // 
            WAMapIntegrationOverlay.BackColor = Color.FromArgb(244, 241, 243);
            WAMapIntegrationOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            WAMapIntegrationOverlay.Controls.Add(CreateMapArticleButton);
            WAMapIntegrationOverlay.Controls.Add(CreateMapButton);
            WAMapIntegrationOverlay.Controls.Add(ImageIdValidButton);
            WAMapIntegrationOverlay.Controls.Add(ValidateImageIdButton);
            WAMapIntegrationOverlay.Controls.Add(label5);
            WAMapIntegrationOverlay.Controls.Add(MapImageIdTextBox);
            WAMapIntegrationOverlay.Controls.Add(MapTitleLabel);
            WAMapIntegrationOverlay.Controls.Add(MapTitleTextBox);
            WAMapIntegrationOverlay.Controls.Add(MapUserIdLabel);
            WAMapIntegrationOverlay.Controls.Add(label4);
            WAMapIntegrationOverlay.Controls.Add(MapWorldIdLabel);
            WAMapIntegrationOverlay.Controls.Add(label3);
            WAMapIntegrationOverlay.Controls.Add(MapIdLabel);
            WAMapIntegrationOverlay.Controls.Add(MapGuidLabel);
            WAMapIntegrationOverlay.Controls.Add(CloseButton);
            WAMapIntegrationOverlay.Dock = DockStyle.Fill;
            WAMapIntegrationOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            WAMapIntegrationOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            WAMapIntegrationOverlay.Font = new Font("Segoe UI", 9F);
            WAMapIntegrationOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            WAMapIntegrationOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            WAMapIntegrationOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            WAMapIntegrationOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            WAMapIntegrationOverlay.Location = new Point(0, 0);
            WAMapIntegrationOverlay.Name = "WAMapIntegrationOverlay";
            WAMapIntegrationOverlay.Padding = new Padding(20, 56, 20, 16);
            WAMapIntegrationOverlay.RoundCorners = true;
            WAMapIntegrationOverlay.Sizable = true;
            WAMapIntegrationOverlay.Size = new Size(564, 337);
            WAMapIntegrationOverlay.SmartBounds = true;
            WAMapIntegrationOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            WAMapIntegrationOverlay.TabIndex = 0;
            WAMapIntegrationOverlay.Text = "World Anvil Map Integration";
            WAMapIntegrationOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CreateMapArticleButton
            // 
            CreateMapArticleButton.Enabled = false;
            CreateMapArticleButton.Font = new Font("Segoe UI", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreateMapArticleButton.ForeColor = SystemColors.ControlDarkDark;
            CreateMapArticleButton.IconChar = FontAwesome.Sharp.IconChar.FileUpload;
            CreateMapArticleButton.IconColor = Color.FromArgb(140, 10, 10);
            CreateMapArticleButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CreateMapArticleButton.IconSize = 28;
            CreateMapArticleButton.Location = new Point(349, 258);
            CreateMapArticleButton.Name = "CreateMapArticleButton";
            CreateMapArticleButton.Size = new Size(60, 60);
            CreateMapArticleButton.TabIndex = 99;
            CreateMapArticleButton.Text = "Article";
            CreateMapArticleButton.TextImageRelation = TextImageRelation.TextAboveImage;
            CreateMapArticleButton.UseVisualStyleBackColor = true;
            CreateMapArticleButton.Click += CreateMapArticleButton_Click;
            CreateMapArticleButton.MouseEnter += CreateMapArticleButton_MouseEnter;
            // 
            // CreateMapButton
            // 
            CreateMapButton.Enabled = false;
            CreateMapButton.Font = new Font("Segoe UI", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreateMapButton.ForeColor = SystemColors.ControlDarkDark;
            CreateMapButton.IconChar = FontAwesome.Sharp.IconChar.MapLocationDot;
            CreateMapButton.IconColor = Color.FromArgb(140, 10, 10);
            CreateMapButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CreateMapButton.IconSize = 34;
            CreateMapButton.ImageAlign = ContentAlignment.BottomCenter;
            CreateMapButton.Location = new Point(415, 258);
            CreateMapButton.Name = "CreateMapButton";
            CreateMapButton.Size = new Size(60, 60);
            CreateMapButton.TabIndex = 21;
            CreateMapButton.Text = "Map";
            CreateMapButton.TextImageRelation = TextImageRelation.TextAboveImage;
            CreateMapButton.UseVisualStyleBackColor = true;
            CreateMapButton.Click += CreateMapButton_Click;
            CreateMapButton.MouseHover += CreateMapButton_MouseHover;
            // 
            // ImageIdValidButton
            // 
            ImageIdValidButton.FlatStyle = FlatStyle.Flat;
            ImageIdValidButton.IconChar = FontAwesome.Sharp.IconChar.Cancel;
            ImageIdValidButton.IconColor = Color.Red;
            ImageIdValidButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ImageIdValidButton.IconSize = 20;
            ImageIdValidButton.Location = new Point(516, 57);
            ImageIdValidButton.Name = "ImageIdValidButton";
            ImageIdValidButton.Size = new Size(25, 25);
            ImageIdValidButton.TabIndex = 20;
            ImageIdValidButton.UseVisualStyleBackColor = true;
            // 
            // ValidateImageIdButton
            // 
            ValidateImageIdButton.ForeColor = SystemColors.ControlDarkDark;
            ValidateImageIdButton.Location = new Point(435, 58);
            ValidateImageIdButton.Name = "ValidateImageIdButton";
            ValidateImageIdButton.Size = new Size(75, 23);
            ValidateImageIdButton.TabIndex = 19;
            ValidateImageIdButton.Text = "Validate";
            ValidateImageIdButton.UseVisualStyleBackColor = true;
            ValidateImageIdButton.Click += ValidateImageIdButton_Click;
            ValidateImageIdButton.MouseHover += ValidateImageIdButton_MouseHover;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(23, 62);
            label5.Name = "label5";
            label5.Size = new Size(117, 15);
            label5.TabIndex = 18;
            label5.Text = "Map Image Identifier";
            // 
            // MapImageIdTextBox
            // 
            MapImageIdTextBox.Location = new Point(143, 59);
            MapImageIdTextBox.Name = "MapImageIdTextBox";
            MapImageIdTextBox.Size = new Size(286, 23);
            MapImageIdTextBox.TabIndex = 17;
            // 
            // MapTitleLabel
            // 
            MapTitleLabel.AutoSize = true;
            MapTitleLabel.ForeColor = SystemColors.ControlDarkDark;
            MapTitleLabel.Location = new Point(23, 96);
            MapTitleLabel.Name = "MapTitleLabel";
            MapTitleLabel.Size = new Size(57, 15);
            MapTitleLabel.TabIndex = 16;
            MapTitleLabel.Text = "Map Title";
            // 
            // MapTitleTextBox
            // 
            MapTitleTextBox.Location = new Point(143, 93);
            MapTitleTextBox.Name = "MapTitleTextBox";
            MapTitleTextBox.Size = new Size(286, 23);
            MapTitleTextBox.TabIndex = 15;
            // 
            // MapUserIdLabel
            // 
            MapUserIdLabel.BorderStyle = BorderStyle.FixedSingle;
            MapUserIdLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapUserIdLabel.ForeColor = SystemColors.ControlDarkDark;
            MapUserIdLabel.Location = new Point(143, 202);
            MapUserIdLabel.Name = "MapUserIdLabel";
            MapUserIdLabel.Size = new Size(286, 23);
            MapUserIdLabel.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(23, 203);
            label4.Name = "label4";
            label4.Size = new Size(106, 13);
            label4.TabIndex = 12;
            label4.Text = "Map User Identifier";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MapWorldIdLabel
            // 
            MapWorldIdLabel.BorderStyle = BorderStyle.FixedSingle;
            MapWorldIdLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapWorldIdLabel.ForeColor = SystemColors.ControlDarkDark;
            MapWorldIdLabel.Location = new Point(143, 166);
            MapWorldIdLabel.Name = "MapWorldIdLabel";
            MapWorldIdLabel.Size = new Size(286, 23);
            MapWorldIdLabel.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(22, 167);
            label3.Name = "label3";
            label3.Size = new Size(115, 13);
            label3.TabIndex = 10;
            label3.Text = "Map World Identifier";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MapIdLabel
            // 
            MapIdLabel.BorderStyle = BorderStyle.FixedSingle;
            MapIdLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapIdLabel.ForeColor = SystemColors.ControlDarkDark;
            MapIdLabel.Location = new Point(143, 129);
            MapIdLabel.Name = "MapIdLabel";
            MapIdLabel.Size = new Size(286, 23);
            MapIdLabel.TabIndex = 7;
            // 
            // MapGuidLabel
            // 
            MapGuidLabel.AutoSize = true;
            MapGuidLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MapGuidLabel.ForeColor = SystemColors.ControlDarkDark;
            MapGuidLabel.Location = new Point(23, 130);
            MapGuidLabel.Name = "MapGuidLabel";
            MapGuidLabel.Size = new Size(80, 13);
            MapGuidLabel.TabIndex = 6;
            MapGuidLabel.Text = "Map Identifier";
            MapGuidLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // CloseButton
            // 
            CloseButton.ForeColor = SystemColors.ControlDarkDark;
            CloseButton.Location = new Point(481, 258);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(60, 60);
            CloseButton.TabIndex = 1;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // WorldAnvilMapIntegration
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(564, 337);
            Controls.Add(WAMapIntegrationOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "WorldAnvilMapIntegration";
            Text = "World Anvil Map Integration";
            TransparencyKey = Color.Fuchsia;
            Shown += WorldAnvilMapIntegration_Shown;
            WAMapIntegrationOverlay.ResumeLayout(false);
            WAMapIntegrationOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm WAMapIntegrationOverlay;
        private Button CloseButton;
        private Label MapIdLabel;
        private Label MapGuidLabel;
        private Label MapWorldIdLabel;
        private Label label3;
        private Label MapUserIdLabel;
        private Label label4;
        private Label MapTitleLabel;
        private TextBox MapTitleTextBox;
        private FontAwesome.Sharp.IconButton ImageIdValidButton;
        private Button ValidateImageIdButton;
        private Label label5;
        private TextBox MapImageIdTextBox;
        private FontAwesome.Sharp.IconButton CreateMapButton;
        private FontAwesome.Sharp.IconButton CreateMapArticleButton;
    }
}