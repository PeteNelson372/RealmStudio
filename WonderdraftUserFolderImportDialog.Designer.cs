namespace RealmStudio
{
    partial class WonderdraftUserFolderImportDialog
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
            CheckAllCheckbox = new CheckBox();
            CollectionListBox = new CheckedListBox();
            AddAssetsButton = new FontAwesome.Sharp.IconButton();
            ImportFoldersButton = new FontAwesome.Sharp.IconButton();
            WDUserFolderLabel = new Label();
            ChooseUserFolderButton = new FontAwesome.Sharp.IconButton();
            UserFolderImportCloseButton = new Button();
            FormOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // FormOverlay
            // 
            FormOverlay.BackColor = Color.FromArgb(244, 241, 243);
            FormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            FormOverlay.Controls.Add(UserFolderImportCloseButton);
            FormOverlay.Controls.Add(CheckAllCheckbox);
            FormOverlay.Controls.Add(CollectionListBox);
            FormOverlay.Controls.Add(AddAssetsButton);
            FormOverlay.Controls.Add(ImportFoldersButton);
            FormOverlay.Controls.Add(WDUserFolderLabel);
            FormOverlay.Controls.Add(ChooseUserFolderButton);
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
            FormOverlay.Text = "Import Wonderdraft User Folder";
            FormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CheckAllCheckbox
            // 
            CheckAllCheckbox.AutoSize = true;
            CheckAllCheckbox.ForeColor = SystemColors.ControlDarkDark;
            CheckAllCheckbox.Location = new Point(89, 136);
            CheckAllCheckbox.Name = "CheckAllCheckbox";
            CheckAllCheckbox.Size = new Size(127, 19);
            CheckAllCheckbox.TabIndex = 7;
            CheckAllCheckbox.Text = "Check/Uncheck All";
            CheckAllCheckbox.UseVisualStyleBackColor = true;
            // 
            // CollectionListBox
            // 
            CollectionListBox.FormattingEnabled = true;
            CollectionListBox.Location = new Point(89, 161);
            CollectionListBox.Name = "CollectionListBox";
            CollectionListBox.Size = new Size(429, 184);
            CollectionListBox.TabIndex = 6;
            // 
            // AddAssetsButton
            // 
            AddAssetsButton.ForeColor = SystemColors.ControlDarkDark;
            AddAssetsButton.IconChar = FontAwesome.Sharp.IconChar.FolderPlus;
            AddAssetsButton.IconColor = Color.Black;
            AddAssetsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AddAssetsButton.IconSize = 24;
            AddAssetsButton.Location = new Point(23, 285);
            AddAssetsButton.Name = "AddAssetsButton";
            AddAssetsButton.Size = new Size(60, 60);
            AddAssetsButton.TabIndex = 4;
            AddAssetsButton.Text = "Add";
            AddAssetsButton.TextImageRelation = TextImageRelation.TextAboveImage;
            AddAssetsButton.UseVisualStyleBackColor = true;
            AddAssetsButton.Click += AddAssetsButton_Click;
            // 
            // ImportFoldersButton
            // 
            ImportFoldersButton.ForeColor = SystemColors.ControlDarkDark;
            ImportFoldersButton.IconChar = FontAwesome.Sharp.IconChar.FileUpload;
            ImportFoldersButton.IconColor = Color.Black;
            ImportFoldersButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ImportFoldersButton.IconSize = 24;
            ImportFoldersButton.Location = new Point(23, 161);
            ImportFoldersButton.Name = "ImportFoldersButton";
            ImportFoldersButton.Size = new Size(60, 60);
            ImportFoldersButton.TabIndex = 2;
            ImportFoldersButton.Text = "Import";
            ImportFoldersButton.TextImageRelation = TextImageRelation.TextAboveImage;
            ImportFoldersButton.UseVisualStyleBackColor = true;
            ImportFoldersButton.Click += ImportFoldersButton_Click;
            // 
            // WDUserFolderLabel
            // 
            WDUserFolderLabel.BorderStyle = BorderStyle.FixedSingle;
            WDUserFolderLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            WDUserFolderLabel.ForeColor = SystemColors.ControlDarkDark;
            WDUserFolderLabel.Location = new Point(89, 80);
            WDUserFolderLabel.Name = "WDUserFolderLabel";
            WDUserFolderLabel.Size = new Size(429, 23);
            WDUserFolderLabel.TabIndex = 1;
            // 
            // ChooseUserFolderButton
            // 
            ChooseUserFolderButton.ForeColor = SystemColors.ControlDarkDark;
            ChooseUserFolderButton.IconChar = FontAwesome.Sharp.IconChar.FolderBlank;
            ChooseUserFolderButton.IconColor = Color.Black;
            ChooseUserFolderButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ChooseUserFolderButton.IconSize = 24;
            ChooseUserFolderButton.Location = new Point(23, 59);
            ChooseUserFolderButton.Name = "ChooseUserFolderButton";
            ChooseUserFolderButton.Size = new Size(60, 60);
            ChooseUserFolderButton.TabIndex = 0;
            ChooseUserFolderButton.Text = "Folders";
            ChooseUserFolderButton.TextImageRelation = TextImageRelation.TextAboveImage;
            ChooseUserFolderButton.UseVisualStyleBackColor = true;
            ChooseUserFolderButton.Click += ChooseUserFolderButton_Click;
            ChooseUserFolderButton.MouseHover += ChooseUserFolderButton_MouseHover;
            // 
            // UserFolderImportCloseButton
            // 
            UserFolderImportCloseButton.ForeColor = SystemColors.ControlDarkDark;
            UserFolderImportCloseButton.Location = new Point(458, 371);
            UserFolderImportCloseButton.Name = "UserFolderImportCloseButton";
            UserFolderImportCloseButton.Size = new Size(60, 60);
            UserFolderImportCloseButton.TabIndex = 8;
            UserFolderImportCloseButton.Text = "&Close";
            UserFolderImportCloseButton.UseVisualStyleBackColor = true;
            UserFolderImportCloseButton.Click += UserFolderImportCloseButton_Click;
            // 
            // WonderdraftUserFolderImportDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 450);
            Controls.Add(FormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "WonderdraftUserFolderImportDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import Wonderdraft User Folder";
            TransparencyKey = Color.Fuchsia;
            FormOverlay.ResumeLayout(false);
            FormOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm FormOverlay;
        private Label WDUserFolderLabel;
        private FontAwesome.Sharp.IconButton ChooseUserFolderButton;
        private FontAwesome.Sharp.IconButton AddAssetsButton;
        private FontAwesome.Sharp.IconButton ImportFoldersButton;
        private CheckedListBox CollectionListBox;
        private CheckBox CheckAllCheckbox;
        private Button UserFolderImportCloseButton;
    }
}