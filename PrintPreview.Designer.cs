namespace RealmStudio
{
    partial class PrintPreview
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
            PrintPreviewOverlay = new ReaLTaiizor.Forms.DungeonForm();
            PrintStatusLabel = new Label();
            PrintRealmSheetsLabel = new Label();
            groupBox3 = new GroupBox();
            MaintainAspectRatioCheck = new CheckBox();
            label13 = new Label();
            label12 = new Label();
            RightMarginUpDown = new NumericUpDown();
            label11 = new Label();
            BottomMarginUpDown = new NumericUpDown();
            label4 = new Label();
            LeftMarginUpDown = new NumericUpDown();
            label3 = new Label();
            TopMarginUpDown = new NumericUpDown();
            label6 = new Label();
            label10 = new Label();
            SheetsDownUpDown = new NumericUpDown();
            label9 = new Label();
            SheetsAcrossUpDown = new NumericUpDown();
            label8 = new Label();
            FitToRadio = new RadioButton();
            ScalePercentageCombo = new ComboBox();
            ScaleRadio = new RadioButton();
            groupBox2 = new GroupBox();
            GrayscaleRadio = new RadioButton();
            ColorRadio = new RadioButton();
            groupBox1 = new GroupBox();
            LandscapeRadio = new RadioButton();
            PortraitRadio = new RadioButton();
            label7 = new Label();
            PrintQualityCombo = new ComboBox();
            label5 = new Label();
            PaperSizeCombo = new ComboBox();
            label2 = new Label();
            NumberCopiesUpDown = new NumericUpDown();
            label1 = new Label();
            ClosePrintPreviewButton = new Button();
            PrintButton = new Button();
            PrinterListCombo = new ComboBox();
            RealmPreviewPicture = new PictureBox();
            PrintPreviewOverlay.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RightMarginUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BottomMarginUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LeftMarginUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TopMarginUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SheetsDownUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SheetsAcrossUpDown).BeginInit();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumberCopiesUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RealmPreviewPicture).BeginInit();
            SuspendLayout();
            // 
            // PrintPreviewOverlay
            // 
            PrintPreviewOverlay.BackColor = Color.FromArgb(244, 241, 243);
            PrintPreviewOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            PrintPreviewOverlay.Controls.Add(PrintStatusLabel);
            PrintPreviewOverlay.Controls.Add(PrintRealmSheetsLabel);
            PrintPreviewOverlay.Controls.Add(groupBox3);
            PrintPreviewOverlay.Controls.Add(groupBox2);
            PrintPreviewOverlay.Controls.Add(groupBox1);
            PrintPreviewOverlay.Controls.Add(label7);
            PrintPreviewOverlay.Controls.Add(PrintQualityCombo);
            PrintPreviewOverlay.Controls.Add(label5);
            PrintPreviewOverlay.Controls.Add(PaperSizeCombo);
            PrintPreviewOverlay.Controls.Add(label2);
            PrintPreviewOverlay.Controls.Add(NumberCopiesUpDown);
            PrintPreviewOverlay.Controls.Add(label1);
            PrintPreviewOverlay.Controls.Add(ClosePrintPreviewButton);
            PrintPreviewOverlay.Controls.Add(PrintButton);
            PrintPreviewOverlay.Controls.Add(PrinterListCombo);
            PrintPreviewOverlay.Controls.Add(RealmPreviewPicture);
            PrintPreviewOverlay.Dock = DockStyle.Fill;
            PrintPreviewOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            PrintPreviewOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            PrintPreviewOverlay.Font = new Font("Segoe UI", 9F);
            PrintPreviewOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            PrintPreviewOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            PrintPreviewOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            PrintPreviewOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            PrintPreviewOverlay.Location = new Point(0, 0);
            PrintPreviewOverlay.Name = "PrintPreviewOverlay";
            PrintPreviewOverlay.Padding = new Padding(20, 56, 20, 16);
            PrintPreviewOverlay.RoundCorners = true;
            PrintPreviewOverlay.Sizable = true;
            PrintPreviewOverlay.Size = new Size(776, 673);
            PrintPreviewOverlay.SmartBounds = true;
            PrintPreviewOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            PrintPreviewOverlay.TabIndex = 0;
            PrintPreviewOverlay.Text = "Print Preview";
            PrintPreviewOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // PrintStatusLabel
            // 
            PrintStatusLabel.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            PrintStatusLabel.ForeColor = Color.FromArgb(0, 192, 0);
            PrintStatusLabel.Location = new Point(38, 625);
            PrintStatusLabel.Name = "PrintStatusLabel";
            PrintStatusLabel.Size = new Size(466, 19);
            PrintStatusLabel.TabIndex = 60;
            PrintStatusLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // PrintRealmSheetsLabel
            // 
            PrintRealmSheetsLabel.AutoSize = true;
            PrintRealmSheetsLabel.ForeColor = SystemColors.ControlDarkDark;
            PrintRealmSheetsLabel.Location = new Point(95, 594);
            PrintRealmSheetsLabel.Name = "PrintRealmSheetsLabel";
            PrintRealmSheetsLabel.Size = new Size(163, 15);
            PrintRealmSheetsLabel.TabIndex = 59;
            PrintRealmSheetsLabel.Text = "Printing the realm will require";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(MaintainAspectRatioCheck);
            groupBox3.Controls.Add(label13);
            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(RightMarginUpDown);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(BottomMarginUpDown);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(LeftMarginUpDown);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(TopMarginUpDown);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(SheetsDownUpDown);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(SheetsAcrossUpDown);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(FitToRadio);
            groupBox3.Controls.Add(ScalePercentageCombo);
            groupBox3.Controls.Add(ScaleRadio);
            groupBox3.ForeColor = SystemColors.ControlDarkDark;
            groupBox3.Location = new Point(534, 329);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(219, 252);
            groupBox3.TabIndex = 58;
            groupBox3.TabStop = false;
            groupBox3.Text = "Print Layout";
            // 
            // MaintainAspectRatioCheck
            // 
            MaintainAspectRatioCheck.AutoSize = true;
            MaintainAspectRatioCheck.Location = new Point(76, 225);
            MaintainAspectRatioCheck.Name = "MaintainAspectRatioCheck";
            MaintainAspectRatioCheck.Size = new Size(142, 19);
            MaintainAspectRatioCheck.TabIndex = 74;
            MaintainAspectRatioCheck.Text = "Maintain Aspect Ratio";
            MaintainAspectRatioCheck.UseVisualStyleBackColor = true;
            MaintainAspectRatioCheck.CheckedChanged += MaintainAspectRatioCheck_CheckedChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.ForeColor = SystemColors.ControlDarkDark;
            label13.Location = new Point(6, 44);
            label13.Name = "label13";
            label13.Size = new Size(49, 15);
            label13.TabIndex = 73;
            label13.Text = "(inches)";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(145, 78);
            label12.Name = "label12";
            label12.Size = new Size(35, 15);
            label12.TabIndex = 72;
            label12.Text = "Right";
            // 
            // RightMarginUpDown
            // 
            RightMarginUpDown.DecimalPlaces = 2;
            RightMarginUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            RightMarginUpDown.Location = new Point(145, 96);
            RightMarginUpDown.Name = "RightMarginUpDown";
            RightMarginUpDown.Size = new Size(65, 23);
            RightMarginUpDown.TabIndex = 71;
            RightMarginUpDown.TextAlign = HorizontalAlignment.Center;
            RightMarginUpDown.ValueChanged += RightMarginUpDown_ValueChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(76, 78);
            label11.Name = "label11";
            label11.Size = new Size(47, 15);
            label11.TabIndex = 70;
            label11.Text = "Bottom";
            // 
            // BottomMarginUpDown
            // 
            BottomMarginUpDown.DecimalPlaces = 2;
            BottomMarginUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            BottomMarginUpDown.Location = new Point(76, 96);
            BottomMarginUpDown.Name = "BottomMarginUpDown";
            BottomMarginUpDown.Size = new Size(63, 23);
            BottomMarginUpDown.TabIndex = 69;
            BottomMarginUpDown.TextAlign = HorizontalAlignment.Center;
            BottomMarginUpDown.ValueChanged += BottomMarginUpDown_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(145, 29);
            label4.Name = "label4";
            label4.Size = new Size(27, 15);
            label4.TabIndex = 68;
            label4.Text = "Left";
            // 
            // LeftMarginUpDown
            // 
            LeftMarginUpDown.DecimalPlaces = 2;
            LeftMarginUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            LeftMarginUpDown.Location = new Point(145, 47);
            LeftMarginUpDown.Name = "LeftMarginUpDown";
            LeftMarginUpDown.Size = new Size(65, 23);
            LeftMarginUpDown.TabIndex = 67;
            LeftMarginUpDown.TextAlign = HorizontalAlignment.Center;
            LeftMarginUpDown.ValueChanged += LeftMarginUpDown_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(76, 29);
            label3.Name = "label3";
            label3.Size = new Size(27, 15);
            label3.TabIndex = 66;
            label3.Text = "Top";
            // 
            // TopMarginUpDown
            // 
            TopMarginUpDown.DecimalPlaces = 2;
            TopMarginUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            TopMarginUpDown.Location = new Point(76, 47);
            TopMarginUpDown.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            TopMarginUpDown.Name = "TopMarginUpDown";
            TopMarginUpDown.Size = new Size(63, 23);
            TopMarginUpDown.TabIndex = 65;
            TopMarginUpDown.TextAlign = HorizontalAlignment.Center;
            TopMarginUpDown.ValueChanged += TopMarginUpDown_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(6, 29);
            label6.Name = "label6";
            label6.Size = new Size(50, 15);
            label6.TabIndex = 64;
            label6.Text = "Margins";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = SystemColors.ControlDarkDark;
            label10.Location = new Point(50, 198);
            label10.Name = "label10";
            label10.Size = new Size(20, 15);
            label10.TabIndex = 62;
            label10.Text = "by";
            // 
            // SheetsDownUpDown
            // 
            SheetsDownUpDown.Location = new Point(76, 196);
            SheetsDownUpDown.Name = "SheetsDownUpDown";
            SheetsDownUpDown.Size = new Size(40, 23);
            SheetsDownUpDown.TabIndex = 61;
            SheetsDownUpDown.TextAlign = HorizontalAlignment.Center;
            SheetsDownUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            SheetsDownUpDown.ValueChanged += SheetsDownUpDown_ValueChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = SystemColors.ControlDarkDark;
            label9.Location = new Point(121, 198);
            label9.Name = "label9";
            label9.Size = new Size(81, 15);
            label9.TabIndex = 60;
            label9.Text = "sheet(s) down";
            // 
            // SheetsAcrossUpDown
            // 
            SheetsAcrossUpDown.Location = new Point(76, 167);
            SheetsAcrossUpDown.Name = "SheetsAcrossUpDown";
            SheetsAcrossUpDown.Size = new Size(40, 23);
            SheetsAcrossUpDown.TabIndex = 59;
            SheetsAcrossUpDown.TextAlign = HorizontalAlignment.Center;
            SheetsAcrossUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            SheetsAcrossUpDown.ValueChanged += SheetsAcrossUpDown_ValueChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = SystemColors.ControlDarkDark;
            label8.Location = new Point(121, 169);
            label8.Name = "label8";
            label8.Size = new Size(84, 15);
            label8.TabIndex = 58;
            label8.Text = "sheet(s) across";
            // 
            // FitToRadio
            // 
            FitToRadio.AutoSize = true;
            FitToRadio.Checked = true;
            FitToRadio.ForeColor = SystemColors.ControlDarkDark;
            FitToRadio.Location = new Point(6, 167);
            FitToRadio.Name = "FitToRadio";
            FitToRadio.Size = new Size(54, 19);
            FitToRadio.TabIndex = 57;
            FitToRadio.TabStop = true;
            FitToRadio.Text = "Fit To";
            FitToRadio.UseVisualStyleBackColor = true;
            FitToRadio.CheckedChanged += FitToRadio_CheckedChanged;
            // 
            // ScalePercentageCombo
            // 
            ScalePercentageCombo.FormattingEnabled = true;
            ScalePercentageCombo.Items.AddRange(new object[] { "400%", "200%", "100%", "75%", "50%", "25%" });
            ScalePercentageCombo.Location = new Point(76, 138);
            ScalePercentageCombo.Name = "ScalePercentageCombo";
            ScalePercentageCombo.Size = new Size(63, 23);
            ScalePercentageCombo.TabIndex = 56;
            ScalePercentageCombo.SelectedIndexChanged += ScalePercentageCombo_SelectedIndexChanged;
            // 
            // ScaleRadio
            // 
            ScaleRadio.AutoSize = true;
            ScaleRadio.ForeColor = SystemColors.ControlDarkDark;
            ScaleRadio.Location = new Point(6, 139);
            ScaleRadio.Name = "ScaleRadio";
            ScaleRadio.Size = new Size(52, 19);
            ScaleRadio.TabIndex = 55;
            ScaleRadio.Text = "Scale";
            ScaleRadio.UseVisualStyleBackColor = true;
            ScaleRadio.CheckedChanged += ScaleRadio_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(GrayscaleRadio);
            groupBox2.Controls.Add(ColorRadio);
            groupBox2.ForeColor = SystemColors.ControlDarkDark;
            groupBox2.Location = new Point(534, 197);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(219, 56);
            groupBox2.TabIndex = 57;
            groupBox2.TabStop = false;
            groupBox2.Text = "Color";
            // 
            // GrayscaleRadio
            // 
            GrayscaleRadio.AutoSize = true;
            GrayscaleRadio.ForeColor = SystemColors.ControlDarkDark;
            GrayscaleRadio.Location = new Point(76, 22);
            GrayscaleRadio.Name = "GrayscaleRadio";
            GrayscaleRadio.Size = new Size(75, 19);
            GrayscaleRadio.TabIndex = 40;
            GrayscaleRadio.Text = "Grayscale";
            GrayscaleRadio.UseVisualStyleBackColor = true;
            GrayscaleRadio.CheckedChanged += GrayscaleRadio_CheckedChanged;
            // 
            // ColorRadio
            // 
            ColorRadio.AutoSize = true;
            ColorRadio.Checked = true;
            ColorRadio.ForeColor = SystemColors.ControlDarkDark;
            ColorRadio.Location = new Point(6, 22);
            ColorRadio.Name = "ColorRadio";
            ColorRadio.Size = new Size(54, 19);
            ColorRadio.TabIndex = 39;
            ColorRadio.TabStop = true;
            ColorRadio.Text = "Color";
            ColorRadio.UseVisualStyleBackColor = true;
            ColorRadio.CheckedChanged += ColorRadio_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(LandscapeRadio);
            groupBox1.Controls.Add(PortraitRadio);
            groupBox1.ForeColor = SystemColors.ControlDarkDark;
            groupBox1.Location = new Point(534, 135);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(219, 56);
            groupBox1.TabIndex = 56;
            groupBox1.TabStop = false;
            groupBox1.Text = "Page Orientation";
            // 
            // LandscapeRadio
            // 
            LandscapeRadio.AutoSize = true;
            LandscapeRadio.Checked = true;
            LandscapeRadio.ForeColor = SystemColors.ControlDarkDark;
            LandscapeRadio.Location = new Point(76, 22);
            LandscapeRadio.Name = "LandscapeRadio";
            LandscapeRadio.Size = new Size(81, 19);
            LandscapeRadio.TabIndex = 37;
            LandscapeRadio.TabStop = true;
            LandscapeRadio.Text = "Landscape";
            LandscapeRadio.UseVisualStyleBackColor = true;
            LandscapeRadio.CheckedChanged += LandscapeRadio_CheckedChanged_1;
            // 
            // PortraitRadio
            // 
            PortraitRadio.AutoSize = true;
            PortraitRadio.ForeColor = SystemColors.ControlDarkDark;
            PortraitRadio.Location = new Point(6, 22);
            PortraitRadio.Name = "PortraitRadio";
            PortraitRadio.Size = new Size(64, 19);
            PortraitRadio.TabIndex = 36;
            PortraitRadio.Text = "Portrait";
            PortraitRadio.UseVisualStyleBackColor = true;
            PortraitRadio.CheckedChanged += PortraitRadio_CheckedChanged_1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = SystemColors.ControlDarkDark;
            label7.Location = new Point(540, 303);
            label7.Name = "label7";
            label7.Size = new Size(73, 15);
            label7.TabIndex = 45;
            label7.Text = "Print Quality";
            // 
            // PrintQualityCombo
            // 
            PrintQualityCombo.FormattingEnabled = true;
            PrintQualityCombo.Location = new Point(619, 300);
            PrintQualityCombo.Name = "PrintQualityCombo";
            PrintQualityCombo.Size = new Size(134, 23);
            PrintQualityCombo.TabIndex = 44;
            PrintQualityCombo.SelectedIndexChanged += PrintQualityCombo_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(540, 274);
            label5.Name = "label5";
            label5.Size = new Size(60, 15);
            label5.TabIndex = 41;
            label5.Text = "Paper Size";
            // 
            // PaperSizeCombo
            // 
            PaperSizeCombo.FormattingEnabled = true;
            PaperSizeCombo.Location = new Point(619, 271);
            PaperSizeCombo.Name = "PaperSizeCombo";
            PaperSizeCombo.Size = new Size(134, 23);
            PaperSizeCombo.TabIndex = 40;
            PaperSizeCombo.SelectedIndexChanged += PaperSizeCombo_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(540, 108);
            label2.Name = "label2";
            label2.Size = new Size(104, 15);
            label2.TabIndex = 33;
            label2.Text = "Number of Copies";
            // 
            // NumberCopiesUpDown
            // 
            NumberCopiesUpDown.ForeColor = SystemColors.ControlDarkDark;
            NumberCopiesUpDown.Location = new Point(650, 106);
            NumberCopiesUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumberCopiesUpDown.Name = "NumberCopiesUpDown";
            NumberCopiesUpDown.Size = new Size(103, 23);
            NumberCopiesUpDown.TabIndex = 32;
            NumberCopiesUpDown.TextAlign = HorizontalAlignment.Center;
            NumberCopiesUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(540, 56);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 31;
            label1.Text = "Printer";
            // 
            // ClosePrintPreviewButton
            // 
            ClosePrintPreviewButton.DialogResult = DialogResult.Cancel;
            ClosePrintPreviewButton.ForeColor = SystemColors.ControlDarkDark;
            ClosePrintPreviewButton.Location = new Point(693, 594);
            ClosePrintPreviewButton.Name = "ClosePrintPreviewButton";
            ClosePrintPreviewButton.Size = new Size(60, 60);
            ClosePrintPreviewButton.TabIndex = 30;
            ClosePrintPreviewButton.Text = "&Close";
            ClosePrintPreviewButton.UseVisualStyleBackColor = true;
            ClosePrintPreviewButton.Click += ClosePrintPreviewButton_Click;
            // 
            // PrintButton
            // 
            PrintButton.ForeColor = SystemColors.ControlDarkDark;
            PrintButton.Location = new Point(627, 594);
            PrintButton.Name = "PrintButton";
            PrintButton.Size = new Size(60, 60);
            PrintButton.TabIndex = 29;
            PrintButton.Text = "&Print";
            PrintButton.UseVisualStyleBackColor = true;
            PrintButton.Click += PrintButton_Click;
            // 
            // PrinterListCombo
            // 
            PrinterListCombo.FormattingEnabled = true;
            PrinterListCombo.Location = new Point(540, 77);
            PrinterListCombo.Name = "PrinterListCombo";
            PrinterListCombo.Size = new Size(213, 23);
            PrinterListCombo.TabIndex = 1;
            PrinterListCombo.SelectedIndexChanged += PrinterListCombo_SelectedIndexChanged;
            // 
            // RealmPreviewPicture
            // 
            RealmPreviewPicture.BackColor = Color.WhiteSmoke;
            RealmPreviewPicture.BorderStyle = BorderStyle.FixedSingle;
            RealmPreviewPicture.Location = new Point(23, 59);
            RealmPreviewPicture.Name = "RealmPreviewPicture";
            RealmPreviewPicture.Size = new Size(500, 522);
            RealmPreviewPicture.SizeMode = PictureBoxSizeMode.Zoom;
            RealmPreviewPicture.TabIndex = 0;
            RealmPreviewPicture.TabStop = false;
            // 
            // PrintPreview
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(776, 673);
            Controls.Add(PrintPreviewOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "PrintPreview";
            Text = "Print Preview";
            TransparencyKey = Color.Fuchsia;
            PrintPreviewOverlay.ResumeLayout(false);
            PrintPreviewOverlay.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RightMarginUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)BottomMarginUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)LeftMarginUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)TopMarginUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)SheetsDownUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)SheetsAcrossUpDown).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumberCopiesUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)RealmPreviewPicture).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm PrintPreviewOverlay;
        private PictureBox RealmPreviewPicture;
        private ComboBox PrinterListCombo;
        private Button ClosePrintPreviewButton;
        private Button PrintButton;
        private NumericUpDown NumberCopiesUpDown;
        private Label label1;
        private Label label2;
        private Label label5;
        private ComboBox PaperSizeCombo;
        private Label label7;
        private ComboBox PrintQualityCombo;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private RadioButton LandscapeRadio;
        private RadioButton PortraitRadio;
        private GroupBox groupBox3;
        private Label label10;
        private NumericUpDown SheetsDownUpDown;
        private Label label9;
        private NumericUpDown SheetsAcrossUpDown;
        private Label label8;
        private RadioButton FitToRadio;
        private ComboBox ScalePercentageCombo;
        private RadioButton ScaleRadio;
        private RadioButton GrayscaleRadio;
        private RadioButton ColorRadio;
        private Label label11;
        private NumericUpDown BottomMarginUpDown;
        private Label label4;
        private NumericUpDown LeftMarginUpDown;
        private Label label3;
        private NumericUpDown TopMarginUpDown;
        private Label label6;
        private Label label12;
        private NumericUpDown RightMarginUpDown;
        private Label label13;
        private Label PrintRealmSheetsLabel;
        private CheckBox MaintainAspectRatioCheck;
        private Label PrintStatusLabel;
    }
}