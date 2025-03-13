namespace RealmStudio
{
    partial class SymbolCollectionForm
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
            SymbolCollectionFormOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CloseCollectionFormButton = new Button();
            SaveButton = new Button();
            OpenCollectionDirectoryButton = new Button();
            AddTagButton = new FontAwesome.Sharp.IconButton();
            ResetTagsButton = new FontAwesome.Sharp.IconButton();
            NewTagTextBox = new TextBox();
            TaggedSymbolCountLabel = new Label();
            AutoAdvanceCheck = new CheckBox();
            checkBox1 = new CheckBox();
            groupBox3 = new GroupBox();
            AvailableTagsPanel = new FlowLayoutPanel();
            ApplyButton = new FontAwesome.Sharp.IconButton();
            TagSearchTextBox = new TextBox();
            groupBox2 = new GroupBox();
            SymbolTagsPanel = new FlowLayoutPanel();
            SymbolCountLabel = new Label();
            groupBox1 = new GroupBox();
            CustomColorSymbolRadio = new RadioButton();
            GrayScaleSymbolRadio = new RadioButton();
            SymbolNameTextBox = new TextBox();
            label2 = new Label();
            SymbolTypeGroup = new GroupBox();
            MarkerRadioButton = new RadioButton();
            OtherRadioButton = new RadioButton();
            TerrainRadioButton = new RadioButton();
            VegetationRadioButton = new RadioButton();
            StructureRadioButton = new RadioButton();
            PreviousSymbolButton = new FontAwesome.Sharp.IconButton();
            FirstSymbolButton = new FontAwesome.Sharp.IconButton();
            NextSymbolButton = new FontAwesome.Sharp.IconButton();
            LastSymbolButton = new FontAwesome.Sharp.IconButton();
            SymbolPictureBox = new PictureBox();
            CollectionNameTextBox = new TextBox();
            label1 = new Label();
            CollectionPathLabel = new Label();
            ExcludeSymbolButton = new FontAwesome.Sharp.IconButton();
            SymbolCollectionFormOverlay.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            SymbolTypeGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SymbolPictureBox).BeginInit();
            SuspendLayout();
            // 
            // SymbolCollectionFormOverlay
            // 
            SymbolCollectionFormOverlay.BackColor = Color.FromArgb(244, 241, 243);
            SymbolCollectionFormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            SymbolCollectionFormOverlay.Controls.Add(ExcludeSymbolButton);
            SymbolCollectionFormOverlay.Controls.Add(CloseCollectionFormButton);
            SymbolCollectionFormOverlay.Controls.Add(SaveButton);
            SymbolCollectionFormOverlay.Controls.Add(OpenCollectionDirectoryButton);
            SymbolCollectionFormOverlay.Controls.Add(AddTagButton);
            SymbolCollectionFormOverlay.Controls.Add(ResetTagsButton);
            SymbolCollectionFormOverlay.Controls.Add(NewTagTextBox);
            SymbolCollectionFormOverlay.Controls.Add(TaggedSymbolCountLabel);
            SymbolCollectionFormOverlay.Controls.Add(AutoAdvanceCheck);
            SymbolCollectionFormOverlay.Controls.Add(checkBox1);
            SymbolCollectionFormOverlay.Controls.Add(groupBox3);
            SymbolCollectionFormOverlay.Controls.Add(groupBox2);
            SymbolCollectionFormOverlay.Controls.Add(SymbolCountLabel);
            SymbolCollectionFormOverlay.Controls.Add(groupBox1);
            SymbolCollectionFormOverlay.Controls.Add(SymbolNameTextBox);
            SymbolCollectionFormOverlay.Controls.Add(label2);
            SymbolCollectionFormOverlay.Controls.Add(SymbolTypeGroup);
            SymbolCollectionFormOverlay.Controls.Add(PreviousSymbolButton);
            SymbolCollectionFormOverlay.Controls.Add(FirstSymbolButton);
            SymbolCollectionFormOverlay.Controls.Add(NextSymbolButton);
            SymbolCollectionFormOverlay.Controls.Add(LastSymbolButton);
            SymbolCollectionFormOverlay.Controls.Add(SymbolPictureBox);
            SymbolCollectionFormOverlay.Controls.Add(CollectionNameTextBox);
            SymbolCollectionFormOverlay.Controls.Add(label1);
            SymbolCollectionFormOverlay.Controls.Add(CollectionPathLabel);
            SymbolCollectionFormOverlay.Dock = DockStyle.Fill;
            SymbolCollectionFormOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            SymbolCollectionFormOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            SymbolCollectionFormOverlay.Font = new Font("Segoe UI", 9F);
            SymbolCollectionFormOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            SymbolCollectionFormOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            SymbolCollectionFormOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            SymbolCollectionFormOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            SymbolCollectionFormOverlay.Location = new Point(0, 0);
            SymbolCollectionFormOverlay.Name = "SymbolCollectionFormOverlay";
            SymbolCollectionFormOverlay.Padding = new Padding(20, 56, 20, 16);
            SymbolCollectionFormOverlay.RoundCorners = true;
            SymbolCollectionFormOverlay.Sizable = true;
            SymbolCollectionFormOverlay.Size = new Size(737, 560);
            SymbolCollectionFormOverlay.SmartBounds = true;
            SymbolCollectionFormOverlay.StartPosition = FormStartPosition.CenterParent;
            SymbolCollectionFormOverlay.TabIndex = 0;
            SymbolCollectionFormOverlay.Text = "Add/Update Symbol Collection";
            SymbolCollectionFormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CloseCollectionFormButton
            // 
            CloseCollectionFormButton.ForeColor = SystemColors.ControlDarkDark;
            CloseCollectionFormButton.Location = new Point(654, 481);
            CloseCollectionFormButton.Name = "CloseCollectionFormButton";
            CloseCollectionFormButton.Size = new Size(60, 60);
            CloseCollectionFormButton.TabIndex = 114;
            CloseCollectionFormButton.Text = "&Close";
            CloseCollectionFormButton.UseVisualStyleBackColor = true;
            CloseCollectionFormButton.Click += CloseCollectionFormButton_Click;
            // 
            // SaveButton
            // 
            SaveButton.ForeColor = SystemColors.ControlDarkDark;
            SaveButton.Location = new Point(588, 481);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(60, 60);
            SaveButton.TabIndex = 113;
            SaveButton.Text = "&Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // OpenCollectionDirectoryButton
            // 
            OpenCollectionDirectoryButton.ForeColor = SystemColors.ControlDarkDark;
            OpenCollectionDirectoryButton.Location = new Point(23, 59);
            OpenCollectionDirectoryButton.Name = "OpenCollectionDirectoryButton";
            OpenCollectionDirectoryButton.Size = new Size(60, 60);
            OpenCollectionDirectoryButton.TabIndex = 112;
            OpenCollectionDirectoryButton.Text = "&Open";
            OpenCollectionDirectoryButton.UseVisualStyleBackColor = true;
            OpenCollectionDirectoryButton.Click += OpenCollectionDirectoryButton_Click;
            // 
            // AddTagButton
            // 
            AddTagButton.BackColor = SystemColors.ControlLightLight;
            AddTagButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            AddTagButton.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AddTagButton.ForeColor = SystemColors.ControlDarkDark;
            AddTagButton.IconChar = FontAwesome.Sharp.IconChar.Add;
            AddTagButton.IconColor = Color.Black;
            AddTagButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AddTagButton.IconSize = 20;
            AddTagButton.Location = new Point(471, 408);
            AddTagButton.Name = "AddTagButton";
            AddTagButton.Size = new Size(60, 23);
            AddTagButton.TabIndex = 111;
            AddTagButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            AddTagButton.UseVisualStyleBackColor = false;
            AddTagButton.Click += AddTagButton_Click;
            // 
            // ResetTagsButton
            // 
            ResetTagsButton.IconChar = FontAwesome.Sharp.IconChar.RotateBack;
            ResetTagsButton.IconColor = Color.Black;
            ResetTagsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ResetTagsButton.IconSize = 16;
            ResetTagsButton.Location = new Point(507, 175);
            ResetTagsButton.Name = "ResetTagsButton";
            ResetTagsButton.Size = new Size(24, 24);
            ResetTagsButton.TabIndex = 106;
            ResetTagsButton.UseVisualStyleBackColor = true;
            ResetTagsButton.Click += ResetTagsButton_Click;
            // 
            // NewTagTextBox
            // 
            NewTagTextBox.Location = new Point(354, 408);
            NewTagTextBox.Name = "NewTagTextBox";
            NewTagTextBox.Size = new Size(111, 23);
            NewTagTextBox.TabIndex = 110;
            // 
            // TaggedSymbolCountLabel
            // 
            TaggedSymbolCountLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TaggedSymbolCountLabel.ForeColor = SystemColors.ControlDarkDark;
            TaggedSymbolCountLabel.Location = new Point(89, 178);
            TaggedSymbolCountLabel.Name = "TaggedSymbolCountLabel";
            TaggedSymbolCountLabel.Size = new Size(120, 21);
            TaggedSymbolCountLabel.TabIndex = 109;
            TaggedSymbolCountLabel.Text = "Tagged 999 of 999";
            TaggedSymbolCountLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AutoAdvanceCheck
            // 
            AutoAdvanceCheck.AutoSize = true;
            AutoAdvanceCheck.Checked = true;
            AutoAdvanceCheck.CheckState = CheckState.Checked;
            AutoAdvanceCheck.ForeColor = SystemColors.ControlDarkDark;
            AutoAdvanceCheck.Location = new Point(543, 431);
            AutoAdvanceCheck.Name = "AutoAdvanceCheck";
            AutoAdvanceCheck.Size = new Size(123, 19);
            AutoAdvanceCheck.TabIndex = 108;
            AutoAdvanceCheck.Text = "Advance on Apply";
            AutoAdvanceCheck.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.ForeColor = SystemColors.ControlDarkDark;
            checkBox1.Location = new Point(543, 406);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(136, 19);
            checkBox1.TabIndex = 107;
            checkBox1.Text = "Apply to All Symbols";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(AvailableTagsPanel);
            groupBox3.Controls.Add(ApplyButton);
            groupBox3.Controls.Add(TagSearchTextBox);
            groupBox3.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox3.Location = new Point(537, 191);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(177, 210);
            groupBox3.TabIndex = 106;
            groupBox3.TabStop = false;
            groupBox3.Text = "Available Tags";
            // 
            // AvailableTagsPanel
            // 
            AvailableTagsPanel.AutoScroll = true;
            AvailableTagsPanel.Location = new Point(6, 55);
            AvailableTagsPanel.Name = "AvailableTagsPanel";
            AvailableTagsPanel.Size = new Size(165, 94);
            AvailableTagsPanel.TabIndex = 4;
            // 
            // ApplyButton
            // 
            ApplyButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyButton.IconChar = FontAwesome.Sharp.IconChar.Check;
            ApplyButton.IconColor = Color.Black;
            ApplyButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ApplyButton.IconSize = 36;
            ApplyButton.Location = new Point(6, 155);
            ApplyButton.Name = "ApplyButton";
            ApplyButton.Size = new Size(165, 48);
            ApplyButton.TabIndex = 3;
            ApplyButton.Text = "Apply";
            ApplyButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            ApplyButton.UseVisualStyleBackColor = true;
            ApplyButton.Click += ApplyButton_Click;
            // 
            // TagSearchTextBox
            // 
            TagSearchTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TagSearchTextBox.Location = new Point(6, 26);
            TagSearchTextBox.Name = "TagSearchTextBox";
            TagSearchTextBox.PlaceholderText = "Search Tags";
            TagSearchTextBox.Size = new Size(165, 23);
            TagSearchTextBox.TabIndex = 1;
            TagSearchTextBox.KeyUp += TagSearchTextBox_KeyUp;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(SymbolTagsPanel);
            groupBox2.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(354, 191);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(177, 210);
            groupBox2.TabIndex = 102;
            groupBox2.TabStop = false;
            groupBox2.Text = "Symbol Tags";
            // 
            // SymbolTagsPanel
            // 
            SymbolTagsPanel.AutoScroll = true;
            SymbolTagsPanel.AutoScrollMargin = new Size(10, 0);
            SymbolTagsPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SymbolTagsPanel.Location = new Point(6, 26);
            SymbolTagsPanel.Name = "SymbolTagsPanel";
            SymbolTagsPanel.Size = new Size(165, 177);
            SymbolTagsPanel.TabIndex = 107;
            // 
            // SymbolCountLabel
            // 
            SymbolCountLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SymbolCountLabel.ForeColor = SystemColors.ControlDarkDark;
            SymbolCountLabel.Location = new Point(89, 157);
            SymbolCountLabel.Name = "SymbolCountLabel";
            SymbolCountLabel.Size = new Size(120, 21);
            SymbolCountLabel.TabIndex = 105;
            SymbolCountLabel.Text = "Symbol 999 of 999";
            SymbolCountLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CustomColorSymbolRadio);
            groupBox1.Controls.Add(GrayScaleSymbolRadio);
            groupBox1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(215, 356);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(133, 75);
            groupBox1.TabIndex = 104;
            groupBox1.TabStop = false;
            groupBox1.Text = "Symbol Painting";
            // 
            // CustomColorSymbolRadio
            // 
            CustomColorSymbolRadio.AutoSize = true;
            CustomColorSymbolRadio.ForeColor = SystemColors.ControlDarkDark;
            CustomColorSymbolRadio.Location = new Point(6, 47);
            CustomColorSymbolRadio.Name = "CustomColorSymbolRadio";
            CustomColorSymbolRadio.Size = new Size(102, 19);
            CustomColorSymbolRadio.TabIndex = 1;
            CustomColorSymbolRadio.TabStop = true;
            CustomColorSymbolRadio.Text = "Custom Colors";
            CustomColorSymbolRadio.UseVisualStyleBackColor = true;
            // 
            // GrayScaleSymbolRadio
            // 
            GrayScaleSymbolRadio.AutoSize = true;
            GrayScaleSymbolRadio.ForeColor = SystemColors.ControlDarkDark;
            GrayScaleSymbolRadio.Location = new Point(6, 22);
            GrayScaleSymbolRadio.Name = "GrayScaleSymbolRadio";
            GrayScaleSymbolRadio.Size = new Size(75, 19);
            GrayScaleSymbolRadio.TabIndex = 0;
            GrayScaleSymbolRadio.TabStop = true;
            GrayScaleSymbolRadio.Text = "Grayscale";
            GrayScaleSymbolRadio.UseVisualStyleBackColor = true;
            // 
            // SymbolNameTextBox
            // 
            SymbolNameTextBox.Location = new Point(89, 358);
            SymbolNameTextBox.Name = "SymbolNameTextBox";
            SymbolNameTextBox.Size = new Size(120, 23);
            SymbolNameTextBox.TabIndex = 103;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(44, 361);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 102;
            label2.Text = "Name";
            // 
            // SymbolTypeGroup
            // 
            SymbolTypeGroup.Controls.Add(MarkerRadioButton);
            SymbolTypeGroup.Controls.Add(OtherRadioButton);
            SymbolTypeGroup.Controls.Add(TerrainRadioButton);
            SymbolTypeGroup.Controls.Add(VegetationRadioButton);
            SymbolTypeGroup.Controls.Add(StructureRadioButton);
            SymbolTypeGroup.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SymbolTypeGroup.Location = new Point(215, 191);
            SymbolTypeGroup.Name = "SymbolTypeGroup";
            SymbolTypeGroup.Size = new Size(133, 159);
            SymbolTypeGroup.TabIndex = 101;
            SymbolTypeGroup.TabStop = false;
            SymbolTypeGroup.Text = "Symbol Type";
            // 
            // MarkerRadioButton
            // 
            MarkerRadioButton.AutoSize = true;
            MarkerRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MarkerRadioButton.ForeColor = SystemColors.ControlDarkDark;
            MarkerRadioButton.Location = new Point(6, 101);
            MarkerRadioButton.Name = "MarkerRadioButton";
            MarkerRadioButton.Size = new Size(62, 19);
            MarkerRadioButton.TabIndex = 4;
            MarkerRadioButton.TabStop = true;
            MarkerRadioButton.Text = "Marker";
            MarkerRadioButton.UseVisualStyleBackColor = true;
            // 
            // OtherRadioButton
            // 
            OtherRadioButton.AutoSize = true;
            OtherRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OtherRadioButton.ForeColor = SystemColors.ControlDarkDark;
            OtherRadioButton.Location = new Point(6, 126);
            OtherRadioButton.Name = "OtherRadioButton";
            OtherRadioButton.Size = new Size(55, 19);
            OtherRadioButton.TabIndex = 3;
            OtherRadioButton.TabStop = true;
            OtherRadioButton.Text = "Other";
            OtherRadioButton.UseVisualStyleBackColor = true;
            // 
            // TerrainRadioButton
            // 
            TerrainRadioButton.AutoSize = true;
            TerrainRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TerrainRadioButton.ForeColor = SystemColors.ControlDarkDark;
            TerrainRadioButton.Location = new Point(6, 76);
            TerrainRadioButton.Name = "TerrainRadioButton";
            TerrainRadioButton.Size = new Size(61, 19);
            TerrainRadioButton.TabIndex = 2;
            TerrainRadioButton.TabStop = true;
            TerrainRadioButton.Text = "Terrain";
            TerrainRadioButton.UseVisualStyleBackColor = true;
            // 
            // VegetationRadioButton
            // 
            VegetationRadioButton.AutoSize = true;
            VegetationRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            VegetationRadioButton.ForeColor = SystemColors.ControlDarkDark;
            VegetationRadioButton.Location = new Point(6, 51);
            VegetationRadioButton.Name = "VegetationRadioButton";
            VegetationRadioButton.Size = new Size(81, 19);
            VegetationRadioButton.TabIndex = 1;
            VegetationRadioButton.TabStop = true;
            VegetationRadioButton.Text = "Vegetation";
            VegetationRadioButton.UseVisualStyleBackColor = true;
            // 
            // StructureRadioButton
            // 
            StructureRadioButton.AutoSize = true;
            StructureRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            StructureRadioButton.ForeColor = SystemColors.ControlDarkDark;
            StructureRadioButton.Location = new Point(6, 26);
            StructureRadioButton.Name = "StructureRadioButton";
            StructureRadioButton.Size = new Size(73, 19);
            StructureRadioButton.TabIndex = 0;
            StructureRadioButton.TabStop = true;
            StructureRadioButton.Text = "Structure";
            StructureRadioButton.UseVisualStyleBackColor = true;
            // 
            // PreviousSymbolButton
            // 
            PreviousSymbolButton.IconChar = FontAwesome.Sharp.IconChar.Backward;
            PreviousSymbolButton.IconColor = Color.Black;
            PreviousSymbolButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            PreviousSymbolButton.IconSize = 16;
            PreviousSymbolButton.Location = new Point(119, 328);
            PreviousSymbolButton.Name = "PreviousSymbolButton";
            PreviousSymbolButton.Size = new Size(24, 24);
            PreviousSymbolButton.TabIndex = 100;
            PreviousSymbolButton.UseVisualStyleBackColor = true;
            PreviousSymbolButton.Click += PreviousSymbolButton_Click;
            // 
            // FirstSymbolButton
            // 
            FirstSymbolButton.IconChar = FontAwesome.Sharp.IconChar.FastBackward;
            FirstSymbolButton.IconColor = Color.Black;
            FirstSymbolButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            FirstSymbolButton.IconSize = 16;
            FirstSymbolButton.Location = new Point(89, 328);
            FirstSymbolButton.Name = "FirstSymbolButton";
            FirstSymbolButton.Size = new Size(24, 24);
            FirstSymbolButton.TabIndex = 99;
            FirstSymbolButton.UseVisualStyleBackColor = true;
            FirstSymbolButton.Click += FirstSymbolButton_Click;
            // 
            // NextSymbolButton
            // 
            NextSymbolButton.IconChar = FontAwesome.Sharp.IconChar.Forward;
            NextSymbolButton.IconColor = Color.Black;
            NextSymbolButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            NextSymbolButton.IconSize = 16;
            NextSymbolButton.Location = new Point(155, 328);
            NextSymbolButton.Name = "NextSymbolButton";
            NextSymbolButton.Size = new Size(24, 24);
            NextSymbolButton.TabIndex = 98;
            NextSymbolButton.UseVisualStyleBackColor = true;
            NextSymbolButton.Click += NextSymbolButton_Click;
            // 
            // LastSymbolButton
            // 
            LastSymbolButton.IconChar = FontAwesome.Sharp.IconChar.FastForward;
            LastSymbolButton.IconColor = Color.Black;
            LastSymbolButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            LastSymbolButton.IconSize = 16;
            LastSymbolButton.Location = new Point(185, 328);
            LastSymbolButton.Name = "LastSymbolButton";
            LastSymbolButton.Size = new Size(24, 24);
            LastSymbolButton.TabIndex = 97;
            LastSymbolButton.UseVisualStyleBackColor = true;
            LastSymbolButton.Click += LastSymbolButton_Click;
            // 
            // SymbolPictureBox
            // 
            SymbolPictureBox.BorderStyle = BorderStyle.FixedSingle;
            SymbolPictureBox.Location = new Point(89, 202);
            SymbolPictureBox.Name = "SymbolPictureBox";
            SymbolPictureBox.Size = new Size(120, 120);
            SymbolPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            SymbolPictureBox.TabIndex = 96;
            SymbolPictureBox.TabStop = false;
            // 
            // CollectionNameTextBox
            // 
            CollectionNameTextBox.Location = new Point(188, 116);
            CollectionNameTextBox.Name = "CollectionNameTextBox";
            CollectionNameTextBox.Size = new Size(526, 23);
            CollectionNameTextBox.TabIndex = 95;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(89, 119);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 94;
            label1.Text = "CollectionName";
            // 
            // CollectionPathLabel
            // 
            CollectionPathLabel.BorderStyle = BorderStyle.FixedSingle;
            CollectionPathLabel.ForeColor = SystemColors.ControlDarkDark;
            CollectionPathLabel.Location = new Point(89, 81);
            CollectionPathLabel.Name = "CollectionPathLabel";
            CollectionPathLabel.Size = new Size(625, 23);
            CollectionPathLabel.TabIndex = 93;
            // 
            // ExcludeSymbolButton
            // 
            ExcludeSymbolButton.ForeColor = SystemColors.ControlDarkDark;
            ExcludeSymbolButton.IconChar = FontAwesome.Sharp.IconChar.TrashAlt;
            ExcludeSymbolButton.IconColor = Color.Black;
            ExcludeSymbolButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ExcludeSymbolButton.IconSize = 18;
            ExcludeSymbolButton.Location = new Point(89, 399);
            ExcludeSymbolButton.Name = "ExcludeSymbolButton";
            ExcludeSymbolButton.Size = new Size(120, 32);
            ExcludeSymbolButton.TabIndex = 115;
            ExcludeSymbolButton.Text = "Remove Symbol";
            ExcludeSymbolButton.TextAlign = ContentAlignment.MiddleRight;
            ExcludeSymbolButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            ExcludeSymbolButton.UseVisualStyleBackColor = true;
            ExcludeSymbolButton.Click += ExcludeSymbolButton_Click;
            // 
            // SymbolCollectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(737, 560);
            Controls.Add(SymbolCollectionFormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "SymbolCollectionForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add/Update Symbol Collection";
            TransparencyKey = Color.Fuchsia;
            SymbolCollectionFormOverlay.ResumeLayout(false);
            SymbolCollectionFormOverlay.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            SymbolTypeGroup.ResumeLayout(false);
            SymbolTypeGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SymbolPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm SymbolCollectionFormOverlay;
        private Label CollectionPathLabel;
        private PictureBox SymbolPictureBox;
        private TextBox CollectionNameTextBox;
        private Label label1;
        private FontAwesome.Sharp.IconButton LastSymbolButton;
        private FontAwesome.Sharp.IconButton FirstSymbolButton;
        private FontAwesome.Sharp.IconButton NextSymbolButton;
        private FontAwesome.Sharp.IconButton PreviousSymbolButton;
        private GroupBox SymbolTypeGroup;
        private RadioButton StructureRadioButton;
        private RadioButton OtherRadioButton;
        private RadioButton TerrainRadioButton;
        private RadioButton VegetationRadioButton;
        private GroupBox groupBox1;
        private RadioButton GrayScaleSymbolRadio;
        private TextBox SymbolNameTextBox;
        private Label label2;
        private Label SymbolCountLabel;
        private RadioButton CustomColorSymbolRadio;
        private GroupBox groupBox2;
        private FontAwesome.Sharp.IconButton ResetTagsButton;
        private GroupBox groupBox3;
        private TextBox TagSearchTextBox;
        private FlowLayoutPanel SymbolTagsPanel;
        private FontAwesome.Sharp.IconButton ApplyButton;
        private CheckBox AutoAdvanceCheck;
        private CheckBox checkBox1;
        private TextBox NewTagTextBox;
        private Label TaggedSymbolCountLabel;
        private FontAwesome.Sharp.IconButton AddTagButton;
        private FlowLayoutPanel AvailableTagsPanel;
        private Button OpenCollectionDirectoryButton;
        private Button SaveButton;
        private Button CloseCollectionFormButton;
        private RadioButton MarkerRadioButton;
        private FontAwesome.Sharp.IconButton ExcludeSymbolButton;
    }
}