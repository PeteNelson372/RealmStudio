namespace RealmStudio
{
    partial class FontSelectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontSelectionDialog));
            FontSizeCombo = new ComboBox();
            FontFamilyCombo = new ComboBox();
            toolStrip1 = new ToolStrip();
            BigFontButton = new FontAwesome.Sharp.IconToolStripButton();
            SmallFontButton = new FontAwesome.Sharp.IconToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            BoldFontButton = new FontAwesome.Sharp.IconToolStripButton();
            ItalicFontButton = new FontAwesome.Sharp.IconToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripButton1 = new ToolStripButton();
            ExampleTextLabel = new Label();
            CloseFormButton = new Button();
            OKButton = new Button();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // FontSizeCombo
            // 
            FontSizeCombo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FontSizeCombo.ForeColor = SystemColors.ControlDarkDark;
            FontSizeCombo.Items.AddRange(new object[] { "5", "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30", "32", "34", "36", "42", "48", "56", "64", "72", "80", "96", "120", "144" });
            FontSizeCombo.Location = new Point(255, 13);
            FontSizeCombo.Name = "FontSizeCombo";
            FontSizeCombo.Size = new Size(77, 29);
            FontSizeCombo.TabIndex = 37;
            FontSizeCombo.TextChanged += FontSizeCombo_TextChanged;
            // 
            // FontFamilyCombo
            // 
            FontFamilyCombo.DrawMode = DrawMode.OwnerDrawFixed;
            FontFamilyCombo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FontFamilyCombo.Location = new Point(12, 12);
            FontFamilyCombo.Name = "FontFamilyCombo";
            FontFamilyCombo.Size = new Size(237, 30);
            FontFamilyCombo.TabIndex = 38;
            FontFamilyCombo.SelectedIndexChanged += FontFamilyCombo_SelectedIndexChanged;
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = Color.WhiteSmoke;
            toolStrip1.CanOverflow = false;
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { BigFontButton, SmallFontButton, toolStripSeparator1, BoldFontButton, ItalicFontButton, toolStripSeparator2, toolStripButton1 });
            toolStrip1.Location = new Point(335, 10);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(134, 32);
            toolStrip1.TabIndex = 39;
            // 
            // BigFontButton
            // 
            BigFontButton.AutoSize = false;
            BigFontButton.BackColor = SystemColors.ControlLightLight;
            BigFontButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BigFontButton.IconChar = FontAwesome.Sharp.IconChar.Font;
            BigFontButton.IconColor = Color.Black;
            BigFontButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            BigFontButton.IconSize = 20;
            BigFontButton.ImageScaling = ToolStripItemImageScaling.None;
            BigFontButton.ImageTransparentColor = Color.Magenta;
            BigFontButton.Name = "BigFontButton";
            BigFontButton.Size = new Size(29, 29);
            BigFontButton.ToolTipText = "Increase Font Size";
            BigFontButton.Click += BigFontButton_Click;
            // 
            // SmallFontButton
            // 
            SmallFontButton.AutoSize = false;
            SmallFontButton.BackColor = SystemColors.ControlLightLight;
            SmallFontButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SmallFontButton.IconChar = FontAwesome.Sharp.IconChar.Font;
            SmallFontButton.IconColor = Color.Black;
            SmallFontButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SmallFontButton.IconSize = 14;
            SmallFontButton.ImageScaling = ToolStripItemImageScaling.None;
            SmallFontButton.ImageTransparentColor = Color.Magenta;
            SmallFontButton.Name = "SmallFontButton";
            SmallFontButton.Size = new Size(29, 29);
            SmallFontButton.Click += SmallFontButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 32);
            // 
            // BoldFontButton
            // 
            BoldFontButton.AutoSize = false;
            BoldFontButton.BackColor = SystemColors.ControlLightLight;
            BoldFontButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BoldFontButton.IconChar = FontAwesome.Sharp.IconChar.Bold;
            BoldFontButton.IconColor = Color.Black;
            BoldFontButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            BoldFontButton.IconSize = 18;
            BoldFontButton.ImageScaling = ToolStripItemImageScaling.None;
            BoldFontButton.ImageTransparentColor = Color.Magenta;
            BoldFontButton.Name = "BoldFontButton";
            BoldFontButton.Size = new Size(29, 29);
            BoldFontButton.ToolTipText = "Bold";
            BoldFontButton.Click += BoldFontButton_Click;
            // 
            // ItalicFontButton
            // 
            ItalicFontButton.AutoSize = false;
            ItalicFontButton.BackColor = SystemColors.ControlLightLight;
            ItalicFontButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ItalicFontButton.IconChar = FontAwesome.Sharp.IconChar.Italic;
            ItalicFontButton.IconColor = Color.Black;
            ItalicFontButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ItalicFontButton.IconSize = 18;
            ItalicFontButton.ImageScaling = ToolStripItemImageScaling.None;
            ItalicFontButton.ImageTransparentColor = Color.Magenta;
            ItalicFontButton.Name = "ItalicFontButton";
            ItalicFontButton.Size = new Size(29, 29);
            ItalicFontButton.Text = "iconToolStripButton2";
            ItalicFontButton.Click += ItalicFontButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 32);
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 29);
            toolStripButton1.Text = "toolStripButton1";
            // 
            // ExampleTextLabel
            // 
            ExampleTextLabel.BackColor = SystemColors.ControlLightLight;
            ExampleTextLabel.BorderStyle = BorderStyle.FixedSingle;
            ExampleTextLabel.Location = new Point(12, 50);
            ExampleTextLabel.Name = "ExampleTextLabel";
            ExampleTextLabel.Size = new Size(320, 50);
            ExampleTextLabel.TabIndex = 40;
            // 
            // CloseFormButton
            // 
            CloseFormButton.DialogResult = DialogResult.Cancel;
            CloseFormButton.Location = new Point(404, 54);
            CloseFormButton.Name = "CloseFormButton";
            CloseFormButton.Size = new Size(54, 50);
            CloseFormButton.TabIndex = 42;
            CloseFormButton.Text = "&Close";
            CloseFormButton.UseVisualStyleBackColor = true;
            CloseFormButton.Click += CloseFormButton_Click;
            // 
            // OKButton
            // 
            OKButton.DialogResult = DialogResult.OK;
            OKButton.Location = new Point(344, 54);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(54, 50);
            OKButton.TabIndex = 41;
            OKButton.Text = "O&K";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // FontSelectionDialog
            // 
            AcceptButton = OKButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            CancelButton = CloseFormButton;
            ClientSize = new Size(468, 116);
            Controls.Add(CloseFormButton);
            Controls.Add(OKButton);
            Controls.Add(ExampleTextLabel);
            Controls.Add(toolStrip1);
            Controls.Add(FontFamilyCombo);
            Controls.Add(FontSizeCombo);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "FontSelectionDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            Text = "Select Font";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private ComboBox FontSizeCombo;
        private ComboBox FontFamilyCombo;
        private ToolStrip toolStrip1;
        private FontAwesome.Sharp.IconToolStripButton BigFontButton;
        private FontAwesome.Sharp.IconToolStripButton SmallFontButton;
        private FontAwesome.Sharp.IconToolStripButton BoldFontButton;
        private FontAwesome.Sharp.IconToolStripButton ItalicFontButton;
        private Label ExampleTextLabel;
        private Button CloseFormButton;
        private Button OKButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButton1;
    }
}