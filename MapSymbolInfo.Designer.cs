namespace RealmStudio
{
    partial class MapSymbolInfo
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
            dungeonForm1 = new ReaLTaiizor.Forms.DungeonForm();
            CloseFormButton = new Button();
            OceanTextureGroup = new GroupBox();
            ResetSymbolColorsButton = new FontAwesome.Sharp.IconButton();
            SymbolColor3Button = new FontAwesome.Sharp.IconButton();
            SymbolColor2Button = new FontAwesome.Sharp.IconButton();
            SymbolColor1Button = new FontAwesome.Sharp.IconButton();
            SymbolFormatLabel = new Label();
            SymbolGuidLabel = new Label();
            SymbolPathLabel = new Label();
            CollectionNameLabel = new Label();
            SymbolNameLabel = new Label();
            panel1 = new Panel();
            UseCustomColorsRadio = new RadioButton();
            GrayScaleSymbolRadio = new RadioButton();
            OtherRadioButton = new RadioButton();
            TerrainRadioButton = new RadioButton();
            VegetationRadioButton = new RadioButton();
            StructureRadioButton = new RadioButton();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            label32 = new Label();
            groupBox14 = new GroupBox();
            ApplyTagsToSymbolButton = new FontAwesome.Sharp.IconButton();
            NewTagTextBox = new TextBox();
            AddTagButton = new FontAwesome.Sharp.IconButton();
            CheckedTagsListBox = new CheckedListBox();
            dungeonForm1.SuspendLayout();
            OceanTextureGroup.SuspendLayout();
            panel1.SuspendLayout();
            groupBox14.SuspendLayout();
            SuspendLayout();
            // 
            // dungeonForm1
            // 
            dungeonForm1.BackColor = Color.FromArgb(244, 241, 243);
            dungeonForm1.BorderColor = Color.FromArgb(38, 38, 38);
            dungeonForm1.Controls.Add(CloseFormButton);
            dungeonForm1.Controls.Add(OceanTextureGroup);
            dungeonForm1.Controls.Add(groupBox14);
            dungeonForm1.Dock = DockStyle.Fill;
            dungeonForm1.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            dungeonForm1.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            dungeonForm1.Font = new Font("Segoe UI", 9F);
            dungeonForm1.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            dungeonForm1.ForeColor = Color.FromArgb(223, 219, 210);
            dungeonForm1.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            dungeonForm1.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            dungeonForm1.Location = new Point(0, 0);
            dungeonForm1.Name = "dungeonForm1";
            dungeonForm1.Padding = new Padding(20, 56, 20, 16);
            dungeonForm1.RoundCorners = true;
            dungeonForm1.Sizable = true;
            dungeonForm1.Size = new Size(511, 375);
            dungeonForm1.SmartBounds = true;
            dungeonForm1.StartPosition = FormStartPosition.CenterParent;
            dungeonForm1.TabIndex = 0;
            dungeonForm1.Text = "Symbol Info";
            dungeonForm1.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CloseFormButton
            // 
            CloseFormButton.ForeColor = SystemColors.ControlDarkDark;
            CloseFormButton.Location = new Point(421, 293);
            CloseFormButton.Name = "CloseFormButton";
            CloseFormButton.Size = new Size(60, 60);
            CloseFormButton.TabIndex = 35;
            CloseFormButton.Text = "&Close";
            CloseFormButton.UseVisualStyleBackColor = true;
            CloseFormButton.Click += CloseFormButton_Click;
            // 
            // OceanTextureGroup
            // 
            OceanTextureGroup.BackColor = Color.Transparent;
            OceanTextureGroup.Controls.Add(ResetSymbolColorsButton);
            OceanTextureGroup.Controls.Add(SymbolColor3Button);
            OceanTextureGroup.Controls.Add(SymbolColor2Button);
            OceanTextureGroup.Controls.Add(SymbolColor1Button);
            OceanTextureGroup.Controls.Add(SymbolFormatLabel);
            OceanTextureGroup.Controls.Add(SymbolGuidLabel);
            OceanTextureGroup.Controls.Add(SymbolPathLabel);
            OceanTextureGroup.Controls.Add(CollectionNameLabel);
            OceanTextureGroup.Controls.Add(SymbolNameLabel);
            OceanTextureGroup.Controls.Add(panel1);
            OceanTextureGroup.Controls.Add(OtherRadioButton);
            OceanTextureGroup.Controls.Add(TerrainRadioButton);
            OceanTextureGroup.Controls.Add(VegetationRadioButton);
            OceanTextureGroup.Controls.Add(StructureRadioButton);
            OceanTextureGroup.Controls.Add(label5);
            OceanTextureGroup.Controls.Add(label4);
            OceanTextureGroup.Controls.Add(label3);
            OceanTextureGroup.Controls.Add(label2);
            OceanTextureGroup.Controls.Add(label1);
            OceanTextureGroup.Controls.Add(label32);
            OceanTextureGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            OceanTextureGroup.ForeColor = SystemColors.ControlText;
            OceanTextureGroup.Location = new Point(23, 59);
            OceanTextureGroup.Name = "OceanTextureGroup";
            OceanTextureGroup.Size = new Size(304, 294);
            OceanTextureGroup.TabIndex = 34;
            OceanTextureGroup.TabStop = false;
            OceanTextureGroup.Text = "Symbol Data";
            // 
            // ResetSymbolColorsButton
            // 
            ResetSymbolColorsButton.BackColor = SystemColors.Control;
            ResetSymbolColorsButton.IconChar = FontAwesome.Sharp.IconChar.RotateBack;
            ResetSymbolColorsButton.IconColor = Color.Black;
            ResetSymbolColorsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ResetSymbolColorsButton.IconSize = 14;
            ResetSymbolColorsButton.Location = new Point(195, 234);
            ResetSymbolColorsButton.Margin = new Padding(0);
            ResetSymbolColorsButton.Name = "ResetSymbolColorsButton";
            ResetSymbolColorsButton.Size = new Size(46, 46);
            ResetSymbolColorsButton.TabIndex = 59;
            ResetSymbolColorsButton.UseVisualStyleBackColor = false;
            ResetSymbolColorsButton.Click += ResetSymbolColorsButton_Click;
            // 
            // SymbolColor3Button
            // 
            SymbolColor3Button.BackColor = Color.FromArgb(161, 214, 202, 171);
            SymbolColor3Button.FlatStyle = FlatStyle.Flat;
            SymbolColor3Button.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolColor3Button.ForeColor = SystemColors.HighlightText;
            SymbolColor3Button.IconChar = FontAwesome.Sharp.IconChar.Palette;
            SymbolColor3Button.IconColor = Color.Tan;
            SymbolColor3Button.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SymbolColor3Button.IconSize = 24;
            SymbolColor3Button.Location = new Point(146, 234);
            SymbolColor3Button.Margin = new Padding(1, 3, 3, 3);
            SymbolColor3Button.Name = "SymbolColor3Button";
            SymbolColor3Button.Size = new Size(46, 46);
            SymbolColor3Button.TabIndex = 58;
            SymbolColor3Button.UseVisualStyleBackColor = false;
            SymbolColor3Button.Click += SymbolColor3Button_Click;
            // 
            // SymbolColor2Button
            // 
            SymbolColor2Button.BackColor = Color.FromArgb(53, 45, 32);
            SymbolColor2Button.FlatStyle = FlatStyle.Flat;
            SymbolColor2Button.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolColor2Button.ForeColor = SystemColors.HighlightText;
            SymbolColor2Button.IconChar = FontAwesome.Sharp.IconChar.Palette;
            SymbolColor2Button.IconColor = Color.Tan;
            SymbolColor2Button.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SymbolColor2Button.IconSize = 24;
            SymbolColor2Button.Location = new Point(98, 234);
            SymbolColor2Button.Margin = new Padding(1, 3, 1, 3);
            SymbolColor2Button.Name = "SymbolColor2Button";
            SymbolColor2Button.Size = new Size(46, 46);
            SymbolColor2Button.TabIndex = 57;
            SymbolColor2Button.UseVisualStyleBackColor = false;
            SymbolColor2Button.Click += SymbolColor2Button_Click;
            // 
            // SymbolColor1Button
            // 
            SymbolColor1Button.BackColor = Color.FromArgb(85, 44, 36);
            SymbolColor1Button.FlatStyle = FlatStyle.Flat;
            SymbolColor1Button.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolColor1Button.ForeColor = SystemColors.HighlightText;
            SymbolColor1Button.IconChar = FontAwesome.Sharp.IconChar.Palette;
            SymbolColor1Button.IconColor = Color.Tan;
            SymbolColor1Button.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SymbolColor1Button.IconSize = 24;
            SymbolColor1Button.Location = new Point(50, 234);
            SymbolColor1Button.Margin = new Padding(1, 3, 1, 3);
            SymbolColor1Button.Name = "SymbolColor1Button";
            SymbolColor1Button.Size = new Size(46, 46);
            SymbolColor1Button.TabIndex = 56;
            SymbolColor1Button.UseVisualStyleBackColor = false;
            SymbolColor1Button.Click += SymbolColor1Button_Click;
            // 
            // SymbolFormatLabel
            // 
            SymbolFormatLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolFormatLabel.Location = new Point(116, 112);
            SymbolFormatLabel.Name = "SymbolFormatLabel";
            SymbolFormatLabel.Size = new Size(182, 18);
            SymbolFormatLabel.TabIndex = 55;
            // 
            // SymbolGuidLabel
            // 
            SymbolGuidLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolGuidLabel.Location = new Point(116, 91);
            SymbolGuidLabel.Name = "SymbolGuidLabel";
            SymbolGuidLabel.Size = new Size(182, 18);
            SymbolGuidLabel.TabIndex = 54;
            // 
            // SymbolPathLabel
            // 
            SymbolPathLabel.AutoEllipsis = true;
            SymbolPathLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolPathLabel.Location = new Point(116, 70);
            SymbolPathLabel.Name = "SymbolPathLabel";
            SymbolPathLabel.Size = new Size(182, 18);
            SymbolPathLabel.TabIndex = 53;
            // 
            // CollectionNameLabel
            // 
            CollectionNameLabel.AutoEllipsis = true;
            CollectionNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CollectionNameLabel.Location = new Point(116, 49);
            CollectionNameLabel.Name = "CollectionNameLabel";
            CollectionNameLabel.Size = new Size(182, 18);
            CollectionNameLabel.TabIndex = 52;
            // 
            // SymbolNameLabel
            // 
            SymbolNameLabel.AutoEllipsis = true;
            SymbolNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolNameLabel.Location = new Point(116, 25);
            SymbolNameLabel.Name = "SymbolNameLabel";
            SymbolNameLabel.Size = new Size(182, 18);
            SymbolNameLabel.TabIndex = 51;
            // 
            // panel1
            // 
            panel1.Controls.Add(UseCustomColorsRadio);
            panel1.Controls.Add(GrayScaleSymbolRadio);
            panel1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            panel1.Location = new Point(0, 179);
            panel1.Name = "panel1";
            panel1.Size = new Size(304, 33);
            panel1.TabIndex = 50;
            // 
            // UseCustomColorsRadio
            // 
            UseCustomColorsRadio.AutoSize = true;
            UseCustomColorsRadio.Enabled = false;
            UseCustomColorsRadio.ForeColor = SystemColors.ControlDarkDark;
            UseCustomColorsRadio.Location = new Point(143, 8);
            UseCustomColorsRadio.Name = "UseCustomColorsRadio";
            UseCustomColorsRadio.Size = new Size(160, 19);
            UseCustomColorsRadio.TabIndex = 41;
            UseCustomColorsRadio.TabStop = true;
            UseCustomColorsRadio.Text = "Paint with Custom Colors";
            UseCustomColorsRadio.UseVisualStyleBackColor = true;
            // 
            // GrayScaleSymbolRadio
            // 
            GrayScaleSymbolRadio.AutoSize = true;
            GrayScaleSymbolRadio.Enabled = false;
            GrayScaleSymbolRadio.ForeColor = SystemColors.ControlDarkDark;
            GrayScaleSymbolRadio.Location = new Point(6, 8);
            GrayScaleSymbolRadio.Name = "GrayScaleSymbolRadio";
            GrayScaleSymbolRadio.Size = new Size(118, 19);
            GrayScaleSymbolRadio.TabIndex = 40;
            GrayScaleSymbolRadio.TabStop = true;
            GrayScaleSymbolRadio.Text = "Grayscale Symbol";
            GrayScaleSymbolRadio.UseVisualStyleBackColor = true;
            // 
            // OtherRadioButton
            // 
            OtherRadioButton.AutoSize = true;
            OtherRadioButton.Font = new Font("Segoe UI", 9F);
            OtherRadioButton.ForeColor = SystemColors.ControlDarkDark;
            OtherRadioButton.Location = new Point(238, 156);
            OtherRadioButton.Name = "OtherRadioButton";
            OtherRadioButton.Size = new Size(55, 19);
            OtherRadioButton.TabIndex = 49;
            OtherRadioButton.Text = "Other";
            OtherRadioButton.UseVisualStyleBackColor = true;
            // 
            // TerrainRadioButton
            // 
            TerrainRadioButton.AutoSize = true;
            TerrainRadioButton.Font = new Font("Segoe UI", 9F);
            TerrainRadioButton.ForeColor = SystemColors.ControlDarkDark;
            TerrainRadioButton.Location = new Point(172, 156);
            TerrainRadioButton.Name = "TerrainRadioButton";
            TerrainRadioButton.Size = new Size(60, 19);
            TerrainRadioButton.TabIndex = 48;
            TerrainRadioButton.Text = "Terrain";
            TerrainRadioButton.UseVisualStyleBackColor = true;
            // 
            // VegetationRadioButton
            // 
            VegetationRadioButton.AutoSize = true;
            VegetationRadioButton.Font = new Font("Segoe UI", 9F);
            VegetationRadioButton.ForeColor = SystemColors.ControlDarkDark;
            VegetationRadioButton.Location = new Point(85, 156);
            VegetationRadioButton.Name = "VegetationRadioButton";
            VegetationRadioButton.Size = new Size(81, 19);
            VegetationRadioButton.TabIndex = 47;
            VegetationRadioButton.Text = "Vegetation";
            VegetationRadioButton.UseVisualStyleBackColor = true;
            // 
            // StructureRadioButton
            // 
            StructureRadioButton.AutoSize = true;
            StructureRadioButton.Font = new Font("Segoe UI", 9F);
            StructureRadioButton.ForeColor = SystemColors.ControlDarkDark;
            StructureRadioButton.Location = new Point(6, 156);
            StructureRadioButton.Name = "StructureRadioButton";
            StructureRadioButton.Size = new Size(73, 19);
            StructureRadioButton.TabIndex = 46;
            StructureRadioButton.Text = "Structure";
            StructureRadioButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(6, 133);
            label5.Margin = new Padding(3);
            label5.Name = "label5";
            label5.Size = new Size(31, 15);
            label5.TabIndex = 24;
            label5.Text = "Type";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(6, 112);
            label4.Margin = new Padding(3);
            label4.Name = "label4";
            label4.Size = new Size(45, 15);
            label4.TabIndex = 23;
            label4.Text = "Format";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(6, 91);
            label3.Margin = new Padding(3);
            label3.Name = "label3";
            label3.Size = new Size(54, 15);
            label3.TabIndex = 22;
            label3.Text = "Identifier";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(6, 70);
            label2.Margin = new Padding(3);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 21;
            label2.Text = "Path";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(6, 49);
            label1.Margin = new Padding(3);
            label1.Name = "label1";
            label1.Size = new Size(96, 15);
            label1.TabIndex = 20;
            label1.Text = "Collection Name";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.BackColor = Color.Transparent;
            label32.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label32.ForeColor = SystemColors.ControlDarkDark;
            label32.Location = new Point(6, 28);
            label32.Margin = new Padding(3);
            label32.Name = "label32";
            label32.Size = new Size(82, 15);
            label32.TabIndex = 19;
            label32.Text = "Symbol Name";
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(ApplyTagsToSymbolButton);
            groupBox14.Controls.Add(NewTagTextBox);
            groupBox14.Controls.Add(AddTagButton);
            groupBox14.Controls.Add(CheckedTagsListBox);
            groupBox14.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox14.Location = new Point(333, 59);
            groupBox14.Name = "groupBox14";
            groupBox14.Size = new Size(148, 228);
            groupBox14.TabIndex = 33;
            groupBox14.TabStop = false;
            groupBox14.Text = "Symbol Tags";
            // 
            // ApplyTagsToSymbolButton
            // 
            ApplyTagsToSymbolButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyTagsToSymbolButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyTagsToSymbolButton.IconChar = FontAwesome.Sharp.IconChar.Tags;
            ApplyTagsToSymbolButton.IconColor = Color.Black;
            ApplyTagsToSymbolButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ApplyTagsToSymbolButton.IconSize = 18;
            ApplyTagsToSymbolButton.Location = new Point(6, 193);
            ApplyTagsToSymbolButton.Name = "ApplyTagsToSymbolButton";
            ApplyTagsToSymbolButton.Size = new Size(135, 24);
            ApplyTagsToSymbolButton.TabIndex = 30;
            ApplyTagsToSymbolButton.Text = "Apply Tags";
            ApplyTagsToSymbolButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            ApplyTagsToSymbolButton.UseVisualStyleBackColor = true;
            ApplyTagsToSymbolButton.Click += ApplyButton_Click;
            // 
            // NewTagTextBox
            // 
            NewTagTextBox.BorderStyle = BorderStyle.FixedSingle;
            NewTagTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            NewTagTextBox.Location = new Point(6, 164);
            NewTagTextBox.Name = "NewTagTextBox";
            NewTagTextBox.Size = new Size(105, 23);
            NewTagTextBox.TabIndex = 29;
            // 
            // AddTagButton
            // 
            AddTagButton.IconChar = FontAwesome.Sharp.IconChar.Tag;
            AddTagButton.IconColor = Color.Black;
            AddTagButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AddTagButton.IconSize = 16;
            AddTagButton.Location = new Point(117, 164);
            AddTagButton.Name = "AddTagButton";
            AddTagButton.Size = new Size(24, 24);
            AddTagButton.TabIndex = 28;
            AddTagButton.UseVisualStyleBackColor = true;
            AddTagButton.Click += AddTagButton_Click;
            // 
            // CheckedTagsListBox
            // 
            CheckedTagsListBox.CheckOnClick = true;
            CheckedTagsListBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CheckedTagsListBox.ForeColor = SystemColors.ControlDarkDark;
            CheckedTagsListBox.FormattingEnabled = true;
            CheckedTagsListBox.Location = new Point(6, 28);
            CheckedTagsListBox.Name = "CheckedTagsListBox";
            CheckedTagsListBox.Size = new Size(135, 130);
            CheckedTagsListBox.TabIndex = 0;
            // 
            // MapSymbolInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CloseFormButton;
            ClientSize = new Size(511, 375);
            ControlBox = false;
            Controls.Add(dungeonForm1);
            FormBorderStyle = FormBorderStyle.None;
            MaximumSize = new Size(1920, 1200);
            MinimumSize = new Size(261, 65);
            Name = "MapSymbolInfo";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Symbol Info";
            TransparencyKey = Color.Fuchsia;
            dungeonForm1.ResumeLayout(false);
            OceanTextureGroup.ResumeLayout(false);
            OceanTextureGroup.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm dungeonForm1;
        private GroupBox groupBox14;
        private CheckedListBox CheckedTagsListBox;
        private GroupBox OceanTextureGroup;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label32;
        private Label label5;
        private RadioButton OtherRadioButton;
        private RadioButton TerrainRadioButton;
        private RadioButton VegetationRadioButton;
        private RadioButton StructureRadioButton;
        private Panel panel1;
        private RadioButton UseCustomColorsRadio;
        private RadioButton GrayScaleSymbolRadio;
        private TextBox NewTagTextBox;
        private FontAwesome.Sharp.IconButton AddTagButton;
        private FontAwesome.Sharp.IconButton ApplyTagsToSymbolButton;
        private Label SymbolFormatLabel;
        private Label SymbolGuidLabel;
        private Label SymbolPathLabel;
        private Label CollectionNameLabel;
        private Label SymbolNameLabel;
        private Button CloseFormButton;
        private FontAwesome.Sharp.IconButton ResetSymbolColorsButton;
        private FontAwesome.Sharp.IconButton SymbolColor3Button;
        private FontAwesome.Sharp.IconButton SymbolColor2Button;
        private FontAwesome.Sharp.IconButton SymbolColor1Button;
    }
}