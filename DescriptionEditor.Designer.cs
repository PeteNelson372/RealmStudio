namespace RealmStudioX
{
    partial class DescriptionEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DescriptionEditor));
            DescriptionEditorOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CreateDescriptionArticleButton = new FontAwesome.Sharp.IconButton();
            CharacteristicsLabel = new Label();
            MapObjectDetailButton = new FontAwesome.Sharp.IconButton();
            DescriptionAIButton = new FontAwesome.Sharp.IconButton();
            DescriptionTextbox = new TextBox();
            CloseDescriptionButton = new Button();
            DescriptionEditorOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // DescriptionEditorOverlay
            // 
            DescriptionEditorOverlay.BackColor = Color.FromArgb(244, 241, 243);
            DescriptionEditorOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            DescriptionEditorOverlay.Controls.Add(CreateDescriptionArticleButton);
            DescriptionEditorOverlay.Controls.Add(CharacteristicsLabel);
            DescriptionEditorOverlay.Controls.Add(MapObjectDetailButton);
            DescriptionEditorOverlay.Controls.Add(DescriptionAIButton);
            DescriptionEditorOverlay.Controls.Add(DescriptionTextbox);
            DescriptionEditorOverlay.Controls.Add(CloseDescriptionButton);
            DescriptionEditorOverlay.Dock = DockStyle.Fill;
            DescriptionEditorOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            DescriptionEditorOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            DescriptionEditorOverlay.Font = new Font("Segoe UI", 9F);
            DescriptionEditorOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            DescriptionEditorOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            DescriptionEditorOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            DescriptionEditorOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            DescriptionEditorOverlay.Location = new Point(0, 0);
            DescriptionEditorOverlay.Name = "DescriptionEditorOverlay";
            DescriptionEditorOverlay.Padding = new Padding(20, 56, 20, 16);
            DescriptionEditorOverlay.RoundCorners = true;
            DescriptionEditorOverlay.Sizable = true;
            DescriptionEditorOverlay.Size = new Size(598, 430);
            DescriptionEditorOverlay.SmartBounds = true;
            DescriptionEditorOverlay.StartPosition = FormStartPosition.CenterParent;
            DescriptionEditorOverlay.TabIndex = 0;
            DescriptionEditorOverlay.Text = "Description";
            DescriptionEditorOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CreateDescriptionArticleButton
            // 
            CreateDescriptionArticleButton.Font = new Font("Segoe UI", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreateDescriptionArticleButton.ForeColor = SystemColors.ControlDarkDark;
            CreateDescriptionArticleButton.IconChar = FontAwesome.Sharp.IconChar.FileUpload;
            CreateDescriptionArticleButton.IconColor = Color.FromArgb(140, 10, 10);
            CreateDescriptionArticleButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CreateDescriptionArticleButton.IconSize = 28;
            CreateDescriptionArticleButton.Location = new Point(155, 351);
            CreateDescriptionArticleButton.Name = "CreateDescriptionArticleButton";
            CreateDescriptionArticleButton.Size = new Size(60, 60);
            CreateDescriptionArticleButton.TabIndex = 98;
            CreateDescriptionArticleButton.Text = "Article";
            CreateDescriptionArticleButton.TextImageRelation = TextImageRelation.TextAboveImage;
            CreateDescriptionArticleButton.UseVisualStyleBackColor = true;
            CreateDescriptionArticleButton.Click += CreateDescriptionArticleButton_Click;
            CreateDescriptionArticleButton.MouseHover += CreateDescriptionArticleButton_MouseHover;
            // 
            // CharacteristicsLabel
            // 
            CharacteristicsLabel.AutoEllipsis = true;
            CharacteristicsLabel.BackColor = SystemColors.ButtonFace;
            CharacteristicsLabel.ForeColor = SystemColors.ControlDarkDark;
            CharacteristicsLabel.Location = new Point(221, 351);
            CharacteristicsLabel.Name = "CharacteristicsLabel";
            CharacteristicsLabel.Size = new Size(288, 60);
            CharacteristicsLabel.TabIndex = 97;
            // 
            // MapObjectDetailButton
            // 
            MapObjectDetailButton.IconChar = FontAwesome.Sharp.IconChar.CircleInfo;
            MapObjectDetailButton.IconColor = SystemColors.Highlight;
            MapObjectDetailButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            MapObjectDetailButton.IconSize = 36;
            MapObjectDetailButton.Location = new Point(89, 351);
            MapObjectDetailButton.Name = "MapObjectDetailButton";
            MapObjectDetailButton.Size = new Size(60, 60);
            MapObjectDetailButton.TabIndex = 96;
            MapObjectDetailButton.UseVisualStyleBackColor = true;
            MapObjectDetailButton.Click += MapObjectDetailButton_Click;
            MapObjectDetailButton.MouseHover += MapObjectDetailButton_MouseHover;
            // 
            // DescriptionAIButton
            // 
            DescriptionAIButton.BackgroundImage = (Image)resources.GetObject("DescriptionAIButton.BackgroundImage");
            DescriptionAIButton.BackgroundImageLayout = ImageLayout.Zoom;
            DescriptionAIButton.IconChar = FontAwesome.Sharp.IconChar.None;
            DescriptionAIButton.IconColor = Color.Black;
            DescriptionAIButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            DescriptionAIButton.Location = new Point(23, 351);
            DescriptionAIButton.Name = "DescriptionAIButton";
            DescriptionAIButton.Size = new Size(60, 60);
            DescriptionAIButton.TabIndex = 95;
            DescriptionAIButton.UseVisualStyleBackColor = true;
            DescriptionAIButton.Click += DescriptionAIButton_Click;
            DescriptionAIButton.MouseHover += DescriptionAIButton_MouseHover;
            // 
            // DescriptionTextbox
            // 
            DescriptionTextbox.AcceptsReturn = true;
            DescriptionTextbox.AcceptsTab = true;
            DescriptionTextbox.AllowDrop = true;
            DescriptionTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DescriptionTextbox.Location = new Point(23, 59);
            DescriptionTextbox.Multiline = true;
            DescriptionTextbox.Name = "DescriptionTextbox";
            DescriptionTextbox.ScrollBars = ScrollBars.Vertical;
            DescriptionTextbox.Size = new Size(552, 286);
            DescriptionTextbox.TabIndex = 94;
            // 
            // CloseDescriptionButton
            // 
            CloseDescriptionButton.DialogResult = DialogResult.OK;
            CloseDescriptionButton.ForeColor = SystemColors.ControlDarkDark;
            CloseDescriptionButton.Location = new Point(515, 351);
            CloseDescriptionButton.Name = "CloseDescriptionButton";
            CloseDescriptionButton.Size = new Size(60, 60);
            CloseDescriptionButton.TabIndex = 93;
            CloseDescriptionButton.Text = "&Close";
            CloseDescriptionButton.UseVisualStyleBackColor = true;
            CloseDescriptionButton.Click += CloseDescriptionButton_Click;
            // 
            // DescriptionEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(598, 430);
            Controls.Add(DescriptionEditorOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "DescriptionEditor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Description";
            TransparencyKey = Color.Fuchsia;
            DescriptionEditorOverlay.ResumeLayout(false);
            DescriptionEditorOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button CloseDescriptionButton;
        internal ReaLTaiizor.Forms.DungeonForm DescriptionEditorOverlay;
        internal TextBox DescriptionTextbox;
        private FontAwesome.Sharp.IconButton DescriptionAIButton;
        private FontAwesome.Sharp.IconButton MapObjectDetailButton;
        private Label CharacteristicsLabel;
        private FontAwesome.Sharp.IconButton CreateDescriptionArticleButton;
    }
}