namespace RealmStudio
{
    partial class WonderdraftAssetImportDialog
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
            FormOverlay = new ReaLTaiizor.Forms.DungeonForm();
            AssetsImportCloseButton = new FontAwesome.Sharp.IconButton();
            AddCollectionButton = new FontAwesome.Sharp.IconButton();
            FilePreviewTree = new TreeView();
            ImportButton = new FontAwesome.Sharp.IconButton();
            ZipFilePathLabel = new Label();
            ChooseZipFileButton = new FontAwesome.Sharp.IconButton();
            FormOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // FormOverlay
            // 
            FormOverlay.BackColor = Color.FromArgb(244, 241, 243);
            FormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            FormOverlay.Controls.Add(AssetsImportCloseButton);
            FormOverlay.Controls.Add(AddCollectionButton);
            FormOverlay.Controls.Add(FilePreviewTree);
            FormOverlay.Controls.Add(ImportButton);
            FormOverlay.Controls.Add(ZipFilePathLabel);
            FormOverlay.Controls.Add(ChooseZipFileButton);
            FormOverlay.Dock = DockStyle.Fill;
            FormOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            FormOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            FormOverlay.Font = new Font("Segoe UI", 9F);
            FormOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            FormOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            FormOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            FormOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            FormOverlay.Location = new Point(0, 0);
            FormOverlay.Name = "FormOverlay";
            FormOverlay.Padding = new Padding(20, 56, 20, 16);
            FormOverlay.RoundCorners = true;
            FormOverlay.Sizable = true;
            FormOverlay.Size = new Size(541, 450);
            FormOverlay.SmartBounds = true;
            FormOverlay.StartPosition = FormStartPosition.CenterParent;
            FormOverlay.TabIndex = 0;
            FormOverlay.Text = "Import Wonderdraft Assets";
            FormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // AssetsImportCloseButton
            // 
            AssetsImportCloseButton.ForeColor = SystemColors.ControlDarkDark;
            AssetsImportCloseButton.IconChar = FontAwesome.Sharp.IconChar.None;
            AssetsImportCloseButton.IconColor = Color.Black;
            AssetsImportCloseButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AssetsImportCloseButton.Location = new Point(458, 371);
            AssetsImportCloseButton.Name = "AssetsImportCloseButton";
            AssetsImportCloseButton.Size = new Size(60, 60);
            AssetsImportCloseButton.TabIndex = 5;
            AssetsImportCloseButton.Text = "&Close";
            AssetsImportCloseButton.UseVisualStyleBackColor = true;
            AssetsImportCloseButton.Click += AssetsImportCloseButton_Click;
            // 
            // AddCollectionButton
            // 
            AddCollectionButton.ForeColor = SystemColors.ControlDarkDark;
            AddCollectionButton.IconChar = FontAwesome.Sharp.IconChar.FolderPlus;
            AddCollectionButton.IconColor = Color.Black;
            AddCollectionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AddCollectionButton.IconSize = 24;
            AddCollectionButton.Location = new Point(23, 215);
            AddCollectionButton.Name = "AddCollectionButton";
            AddCollectionButton.Size = new Size(60, 60);
            AddCollectionButton.TabIndex = 4;
            AddCollectionButton.Text = "Add";
            AddCollectionButton.TextImageRelation = TextImageRelation.TextAboveImage;
            AddCollectionButton.UseVisualStyleBackColor = true;
            AddCollectionButton.Click += AddCollectionButton_Click;
            // 
            // FilePreviewTree
            // 
            FilePreviewTree.CheckBoxes = true;
            FilePreviewTree.Location = new Point(89, 125);
            FilePreviewTree.Name = "FilePreviewTree";
            FilePreviewTree.Size = new Size(429, 150);
            FilePreviewTree.TabIndex = 3;
            // 
            // ImportButton
            // 
            ImportButton.ForeColor = SystemColors.ControlDarkDark;
            ImportButton.IconChar = FontAwesome.Sharp.IconChar.FileUpload;
            ImportButton.IconColor = Color.Black;
            ImportButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ImportButton.IconSize = 24;
            ImportButton.Location = new Point(23, 125);
            ImportButton.Name = "ImportButton";
            ImportButton.Size = new Size(60, 60);
            ImportButton.TabIndex = 2;
            ImportButton.Text = "Import";
            ImportButton.TextImageRelation = TextImageRelation.TextAboveImage;
            ImportButton.UseVisualStyleBackColor = true;
            ImportButton.Click += ImportButton_Click;
            // 
            // ZipFilePathLabel
            // 
            ZipFilePathLabel.BorderStyle = BorderStyle.FixedSingle;
            ZipFilePathLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ZipFilePathLabel.ForeColor = SystemColors.ControlDarkDark;
            ZipFilePathLabel.Location = new Point(89, 80);
            ZipFilePathLabel.Name = "ZipFilePathLabel";
            ZipFilePathLabel.Size = new Size(429, 23);
            ZipFilePathLabel.TabIndex = 1;
            // 
            // ChooseZipFileButton
            // 
            ChooseZipFileButton.ForeColor = SystemColors.ControlDarkDark;
            ChooseZipFileButton.IconChar = FontAwesome.Sharp.IconChar.FileZipper;
            ChooseZipFileButton.IconColor = Color.Black;
            ChooseZipFileButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ChooseZipFileButton.IconSize = 24;
            ChooseZipFileButton.Location = new Point(23, 59);
            ChooseZipFileButton.Name = "ChooseZipFileButton";
            ChooseZipFileButton.Size = new Size(60, 60);
            ChooseZipFileButton.TabIndex = 0;
            ChooseZipFileButton.Text = "Zip File";
            ChooseZipFileButton.TextImageRelation = TextImageRelation.TextAboveImage;
            ChooseZipFileButton.UseVisualStyleBackColor = true;
            ChooseZipFileButton.Click += ChooseZipFileButton_Click;
            // 
            // WonderdraftAssetImportDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 450);
            Controls.Add(FormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "WonderdraftAssetImportDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import Wonderdraft Assets";
            TransparencyKey = Color.Fuchsia;
            FormOverlay.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm FormOverlay;
        private Label ZipFilePathLabel;
        private FontAwesome.Sharp.IconButton ChooseZipFileButton;
        private FontAwesome.Sharp.IconButton AddCollectionButton;
        private TreeView FilePreviewTree;
        private FontAwesome.Sharp.IconButton ImportButton;
        private FontAwesome.Sharp.IconButton AssetsImportCloseButton;
    }
}