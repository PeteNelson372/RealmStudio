namespace RealmStudio
{
    partial class DetailMapForm
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
            DetailMapFormOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CancelCreateDetailButton = new Button();
            OKButton = new Button();
            groupBox3 = new GroupBox();
            IncludeLabelsCheck = new CheckBox();
            IncludePathsCheck = new CheckBox();
            IncludeStructuresCheck = new CheckBox();
            IncludeVegetationCheck = new CheckBox();
            IncludeTerrainCheck = new CheckBox();
            groupBox2 = new GroupBox();
            DetailMapHeightLabel = new Label();
            DetailMapWidthLabel = new Label();
            label8 = new Label();
            label9 = new Label();
            MapZoomUpDown = new NumericUpDown();
            MapZoomTrack = new TrackBar();
            label6 = new Label();
            MapNameTextBox = new TextBox();
            groupBox1 = new GroupBox();
            MapHeightUpDown = new NumericUpDown();
            MapWidthUpDown = new NumericUpDown();
            MapLeftUpDown = new NumericUpDown();
            MapTopUpDown = new NumericUpDown();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            DetailMapFormOverlay.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MapZoomUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapZoomTrack).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MapHeightUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapWidthUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapLeftUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapTopUpDown).BeginInit();
            SuspendLayout();
            // 
            // DetailMapFormOverlay
            // 
            DetailMapFormOverlay.BackColor = Color.FromArgb(244, 241, 243);
            DetailMapFormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            DetailMapFormOverlay.Controls.Add(CancelCreateDetailButton);
            DetailMapFormOverlay.Controls.Add(OKButton);
            DetailMapFormOverlay.Controls.Add(groupBox3);
            DetailMapFormOverlay.Controls.Add(groupBox2);
            DetailMapFormOverlay.Controls.Add(MapZoomUpDown);
            DetailMapFormOverlay.Controls.Add(MapZoomTrack);
            DetailMapFormOverlay.Controls.Add(label6);
            DetailMapFormOverlay.Controls.Add(MapNameTextBox);
            DetailMapFormOverlay.Controls.Add(groupBox1);
            DetailMapFormOverlay.Controls.Add(label2);
            DetailMapFormOverlay.Dock = DockStyle.Fill;
            DetailMapFormOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            DetailMapFormOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            DetailMapFormOverlay.Font = new Font("Segoe UI", 9F);
            DetailMapFormOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            DetailMapFormOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            DetailMapFormOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            DetailMapFormOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            DetailMapFormOverlay.Location = new Point(0, 0);
            DetailMapFormOverlay.Name = "DetailMapFormOverlay";
            DetailMapFormOverlay.Padding = new Padding(20, 56, 20, 16);
            DetailMapFormOverlay.RoundCorners = true;
            DetailMapFormOverlay.Sizable = true;
            DetailMapFormOverlay.Size = new Size(426, 421);
            DetailMapFormOverlay.SmartBounds = true;
            DetailMapFormOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            DetailMapFormOverlay.TabIndex = 0;
            DetailMapFormOverlay.Text = "Create Detail Map";
            DetailMapFormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CancelCreateDetailButton
            // 
            CancelCreateDetailButton.DialogResult = DialogResult.Cancel;
            CancelCreateDetailButton.ForeColor = SystemColors.ControlDarkDark;
            CancelCreateDetailButton.Location = new Point(337, 335);
            CancelCreateDetailButton.Name = "CancelCreateDetailButton";
            CancelCreateDetailButton.Size = new Size(60, 60);
            CancelCreateDetailButton.TabIndex = 26;
            CancelCreateDetailButton.Text = "&Cancel";
            CancelCreateDetailButton.UseVisualStyleBackColor = true;
            CancelCreateDetailButton.Click += CancelCreateDetailButton_Click;
            // 
            // OKButton
            // 
            OKButton.DialogResult = DialogResult.OK;
            OKButton.ForeColor = SystemColors.ControlDarkDark;
            OKButton.Location = new Point(271, 335);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(60, 60);
            OKButton.TabIndex = 25;
            OKButton.Text = "O&K";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(IncludeLabelsCheck);
            groupBox3.Controls.Add(IncludePathsCheck);
            groupBox3.Controls.Add(IncludeStructuresCheck);
            groupBox3.Controls.Add(IncludeVegetationCheck);
            groupBox3.Controls.Add(IncludeTerrainCheck);
            groupBox3.Location = new Point(197, 143);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(200, 146);
            groupBox3.TabIndex = 13;
            groupBox3.TabStop = false;
            groupBox3.Text = "Include Map Features";
            // 
            // IncludeLabelsCheck
            // 
            IncludeLabelsCheck.AutoSize = true;
            IncludeLabelsCheck.ForeColor = SystemColors.ControlDarkDark;
            IncludeLabelsCheck.Location = new Point(6, 121);
            IncludeLabelsCheck.Name = "IncludeLabelsCheck";
            IncludeLabelsCheck.Size = new Size(59, 19);
            IncludeLabelsCheck.TabIndex = 4;
            IncludeLabelsCheck.Text = "Labels";
            IncludeLabelsCheck.UseVisualStyleBackColor = true;
            // 
            // IncludePathsCheck
            // 
            IncludePathsCheck.AutoSize = true;
            IncludePathsCheck.ForeColor = SystemColors.ControlDarkDark;
            IncludePathsCheck.Location = new Point(6, 96);
            IncludePathsCheck.Name = "IncludePathsCheck";
            IncludePathsCheck.Size = new Size(55, 19);
            IncludePathsCheck.TabIndex = 3;
            IncludePathsCheck.Text = "Paths";
            IncludePathsCheck.UseVisualStyleBackColor = true;
            // 
            // IncludeStructuresCheck
            // 
            IncludeStructuresCheck.AutoSize = true;
            IncludeStructuresCheck.ForeColor = SystemColors.ControlDarkDark;
            IncludeStructuresCheck.Location = new Point(6, 71);
            IncludeStructuresCheck.Name = "IncludeStructuresCheck";
            IncludeStructuresCheck.Size = new Size(122, 19);
            IncludeStructuresCheck.TabIndex = 2;
            IncludeStructuresCheck.Text = "Structure Symbols";
            IncludeStructuresCheck.UseVisualStyleBackColor = true;
            // 
            // IncludeVegetationCheck
            // 
            IncludeVegetationCheck.AutoSize = true;
            IncludeVegetationCheck.ForeColor = SystemColors.ControlDarkDark;
            IncludeVegetationCheck.Location = new Point(6, 46);
            IncludeVegetationCheck.Name = "IncludeVegetationCheck";
            IncludeVegetationCheck.Size = new Size(130, 19);
            IncludeVegetationCheck.TabIndex = 1;
            IncludeVegetationCheck.Text = "Vegetation Symbols";
            IncludeVegetationCheck.UseVisualStyleBackColor = true;
            // 
            // IncludeTerrainCheck
            // 
            IncludeTerrainCheck.AutoSize = true;
            IncludeTerrainCheck.ForeColor = SystemColors.ControlDarkDark;
            IncludeTerrainCheck.Location = new Point(6, 23);
            IncludeTerrainCheck.Name = "IncludeTerrainCheck";
            IncludeTerrainCheck.Size = new Size(110, 19);
            IncludeTerrainCheck.TabIndex = 0;
            IncludeTerrainCheck.Text = "Terrain Symbols";
            IncludeTerrainCheck.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(DetailMapHeightLabel);
            groupBox2.Controls.Add(DetailMapWidthLabel);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label9);
            groupBox2.Location = new Point(23, 295);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(159, 100);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "Detail Map";
            // 
            // DetailMapHeightLabel
            // 
            DetailMapHeightLabel.BackColor = SystemColors.ControlLight;
            DetailMapHeightLabel.BorderStyle = BorderStyle.FixedSingle;
            DetailMapHeightLabel.ForeColor = SystemColors.ControlDarkDark;
            DetailMapHeightLabel.Location = new Point(59, 58);
            DetailMapHeightLabel.Name = "DetailMapHeightLabel";
            DetailMapHeightLabel.Size = new Size(67, 23);
            DetailMapHeightLabel.TabIndex = 8;
            DetailMapHeightLabel.Text = "0";
            DetailMapHeightLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DetailMapWidthLabel
            // 
            DetailMapWidthLabel.BackColor = SystemColors.ControlLight;
            DetailMapWidthLabel.BorderStyle = BorderStyle.FixedSingle;
            DetailMapWidthLabel.ForeColor = SystemColors.ControlDarkDark;
            DetailMapWidthLabel.Location = new Point(59, 29);
            DetailMapWidthLabel.Name = "DetailMapWidthLabel";
            DetailMapWidthLabel.Size = new Size(67, 23);
            DetailMapWidthLabel.TabIndex = 7;
            DetailMapWidthLabel.Text = "0";
            DetailMapWidthLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = SystemColors.ControlDarkDark;
            label8.Location = new Point(6, 62);
            label8.Name = "label8";
            label8.Size = new Size(43, 15);
            label8.TabIndex = 6;
            label8.Text = "Height";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = SystemColors.ControlDarkDark;
            label9.Location = new Point(6, 29);
            label9.Name = "label9";
            label9.Size = new Size(39, 15);
            label9.TabIndex = 5;
            label9.Text = "Width";
            // 
            // MapZoomUpDown
            // 
            MapZoomUpDown.BorderStyle = BorderStyle.FixedSingle;
            MapZoomUpDown.DecimalPlaces = 1;
            MapZoomUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            MapZoomUpDown.Location = new Point(330, 92);
            MapZoomUpDown.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            MapZoomUpDown.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            MapZoomUpDown.Name = "MapZoomUpDown";
            MapZoomUpDown.Size = new Size(67, 23);
            MapZoomUpDown.TabIndex = 9;
            MapZoomUpDown.TextAlign = HorizontalAlignment.Center;
            MapZoomUpDown.Value = new decimal(new int[] { 40, 0, 0, 65536 });
            MapZoomUpDown.ValueChanged += MapZoomUpDown_ValueChanged;
            // 
            // MapZoomTrack
            // 
            MapZoomTrack.AutoSize = false;
            MapZoomTrack.Location = new Point(95, 94);
            MapZoomTrack.Maximum = 100;
            MapZoomTrack.Minimum = 10;
            MapZoomTrack.Name = "MapZoomTrack";
            MapZoomTrack.Size = new Size(229, 22);
            MapZoomTrack.TabIndex = 5;
            MapZoomTrack.TickFrequency = 5;
            MapZoomTrack.Value = 40;
            MapZoomTrack.Scroll += MapZoomTrack_Scroll;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(23, 94);
            label6.Name = "label6";
            label6.Size = new Size(66, 15);
            label6.TabIndex = 4;
            label6.Text = "Map Zoom";
            // 
            // MapNameTextBox
            // 
            MapNameTextBox.Location = new Point(95, 53);
            MapNameTextBox.Name = "MapNameTextBox";
            MapNameTextBox.Size = new Size(302, 23);
            MapNameTextBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(MapHeightUpDown);
            groupBox1.Controls.Add(MapWidthUpDown);
            groupBox1.Controls.Add(MapLeftUpDown);
            groupBox1.Controls.Add(MapTopUpDown);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(23, 136);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(159, 153);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Selected Map Coordinates";
            // 
            // MapHeightUpDown
            // 
            MapHeightUpDown.BorderStyle = BorderStyle.FixedSingle;
            MapHeightUpDown.Location = new Point(59, 113);
            MapHeightUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            MapHeightUpDown.Name = "MapHeightUpDown";
            MapHeightUpDown.Size = new Size(67, 23);
            MapHeightUpDown.TabIndex = 8;
            MapHeightUpDown.TextAlign = HorizontalAlignment.Center;
            MapHeightUpDown.ValueChanged += MapHeightUpDown_ValueChanged;
            // 
            // MapWidthUpDown
            // 
            MapWidthUpDown.BorderStyle = BorderStyle.FixedSingle;
            MapWidthUpDown.Location = new Point(59, 84);
            MapWidthUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            MapWidthUpDown.Name = "MapWidthUpDown";
            MapWidthUpDown.Size = new Size(67, 23);
            MapWidthUpDown.TabIndex = 7;
            MapWidthUpDown.TextAlign = HorizontalAlignment.Center;
            MapWidthUpDown.ValueChanged += MapWidthUpDown_ValueChanged;
            // 
            // MapLeftUpDown
            // 
            MapLeftUpDown.BorderStyle = BorderStyle.FixedSingle;
            MapLeftUpDown.Location = new Point(59, 55);
            MapLeftUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            MapLeftUpDown.Name = "MapLeftUpDown";
            MapLeftUpDown.Size = new Size(67, 23);
            MapLeftUpDown.TabIndex = 6;
            MapLeftUpDown.TextAlign = HorizontalAlignment.Center;
            MapLeftUpDown.ValueChanged += MapLeftUpDown_ValueChanged;
            // 
            // MapTopUpDown
            // 
            MapTopUpDown.BorderStyle = BorderStyle.FixedSingle;
            MapTopUpDown.Location = new Point(59, 26);
            MapTopUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            MapTopUpDown.Name = "MapTopUpDown";
            MapTopUpDown.Size = new Size(67, 23);
            MapTopUpDown.TabIndex = 5;
            MapTopUpDown.TextAlign = HorizontalAlignment.Center;
            MapTopUpDown.ValueChanged += MapTopUpDown_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(6, 115);
            label5.Name = "label5";
            label5.Size = new Size(43, 15);
            label5.TabIndex = 4;
            label5.Text = "Height";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(6, 86);
            label4.Name = "label4";
            label4.Size = new Size(39, 15);
            label4.TabIndex = 3;
            label4.Text = "Width";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(6, 57);
            label3.Name = "label3";
            label3.Size = new Size(27, 15);
            label3.TabIndex = 2;
            label3.Text = "Left";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(6, 28);
            label1.Name = "label1";
            label1.Size = new Size(27, 15);
            label1.TabIndex = 1;
            label1.Text = "Top";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(23, 56);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 1;
            label2.Text = "Map Name";
            // 
            // DetailMapForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(426, 421);
            Controls.Add(DetailMapFormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "DetailMapForm";
            Text = "Create Detail Map";
            TransparencyKey = Color.Fuchsia;
            DetailMapFormOverlay.ResumeLayout(false);
            DetailMapFormOverlay.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MapZoomUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapZoomTrack).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MapHeightUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapWidthUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapLeftUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapTopUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm DetailMapFormOverlay;
        private Label label2;
        private GroupBox groupBox1;
        private NumericUpDown MapTopUpDown;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label1;
        private NumericUpDown MapHeightUpDown;
        private NumericUpDown MapWidthUpDown;
        private NumericUpDown MapLeftUpDown;
        private TextBox MapNameTextBox;
        private NumericUpDown MapZoomUpDown;
        private TrackBar MapZoomTrack;
        private Label label6;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label DetailMapHeightLabel;
        private Label DetailMapWidthLabel;
        private Label label8;
        private Label label9;
        private CheckBox IncludeStructuresCheck;
        private CheckBox IncludeVegetationCheck;
        private CheckBox IncludeTerrainCheck;
        private CheckBox IncludeLabelsCheck;
        private CheckBox IncludePathsCheck;
        private Button CancelCreateDetailButton;
        private Button OKButton;
    }
}