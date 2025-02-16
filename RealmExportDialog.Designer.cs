namespace RealmStudio
{
    partial class RealmExportDialog
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
            ExportRealmOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CloseExportButton = new Button();
            OKExportButton = new Button();
            groupBox2 = new GroupBox();
            GIFRadio = new RadioButton();
            BMPRadio = new RadioButton();
            JPEGRadio = new RadioButton();
            PNGRadio = new RadioButton();
            groupBox1 = new GroupBox();
            HeightmapRadio = new RadioButton();
            MapLayersRadio = new RadioButton();
            UpscaledImageRadio = new RadioButton();
            ExportImageRadio = new RadioButton();
            ExportRealmOverlay.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // ExportRealmOverlay
            // 
            ExportRealmOverlay.BackColor = Color.FromArgb(244, 241, 243);
            ExportRealmOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            ExportRealmOverlay.Controls.Add(CloseExportButton);
            ExportRealmOverlay.Controls.Add(OKExportButton);
            ExportRealmOverlay.Controls.Add(groupBox2);
            ExportRealmOverlay.Controls.Add(groupBox1);
            ExportRealmOverlay.Dock = DockStyle.Fill;
            ExportRealmOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            ExportRealmOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            ExportRealmOverlay.Font = new Font("Segoe UI", 9F);
            ExportRealmOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            ExportRealmOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            ExportRealmOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            ExportRealmOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            ExportRealmOverlay.Location = new Point(0, 0);
            ExportRealmOverlay.Name = "ExportRealmOverlay";
            ExportRealmOverlay.Padding = new Padding(20, 56, 20, 16);
            ExportRealmOverlay.RoundCorners = true;
            ExportRealmOverlay.Sizable = true;
            ExportRealmOverlay.Size = new Size(479, 268);
            ExportRealmOverlay.SmartBounds = true;
            ExportRealmOverlay.StartPosition = FormStartPosition.CenterParent;
            ExportRealmOverlay.TabIndex = 0;
            ExportRealmOverlay.Text = "Export Realm";
            ExportRealmOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CloseExportButton
            // 
            CloseExportButton.DialogResult = DialogResult.Cancel;
            CloseExportButton.ForeColor = SystemColors.ControlDarkDark;
            CloseExportButton.Location = new Point(395, 196);
            CloseExportButton.Name = "CloseExportButton";
            CloseExportButton.Size = new Size(60, 60);
            CloseExportButton.TabIndex = 94;
            CloseExportButton.Text = "&Close";
            CloseExportButton.UseVisualStyleBackColor = true;
            CloseExportButton.Click += CloseExportButton_Click;
            // 
            // OKExportButton
            // 
            OKExportButton.DialogResult = DialogResult.OK;
            OKExportButton.ForeColor = SystemColors.ControlDarkDark;
            OKExportButton.Location = new Point(329, 196);
            OKExportButton.Name = "OKExportButton";
            OKExportButton.Size = new Size(60, 60);
            OKExportButton.TabIndex = 93;
            OKExportButton.Text = "O&K";
            OKExportButton.UseVisualStyleBackColor = true;
            OKExportButton.Click += OKExportButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(GIFRadio);
            groupBox2.Controls.Add(BMPRadio);
            groupBox2.Controls.Add(JPEGRadio);
            groupBox2.Controls.Add(PNGRadio);
            groupBox2.Location = new Point(255, 59);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(200, 124);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Export Format";
            // 
            // GIFRadio
            // 
            GIFRadio.AutoSize = true;
            GIFRadio.ForeColor = SystemColors.ControlDarkDark;
            GIFRadio.Location = new Point(6, 97);
            GIFRadio.Name = "GIFRadio";
            GIFRadio.Size = new Size(91, 19);
            GIFRadio.TabIndex = 3;
            GIFRadio.TabStop = true;
            GIFRadio.Text = "GIF File (.gif)";
            GIFRadio.UseVisualStyleBackColor = true;
            // 
            // BMPRadio
            // 
            BMPRadio.AutoSize = true;
            BMPRadio.ForeColor = SystemColors.ControlDarkDark;
            BMPRadio.Location = new Point(6, 72);
            BMPRadio.Name = "BMPRadio";
            BMPRadio.Size = new Size(110, 19);
            BMPRadio.TabIndex = 2;
            BMPRadio.TabStop = true;
            BMPRadio.Text = "BMP File (.bmp)";
            BMPRadio.UseVisualStyleBackColor = true;
            // 
            // JPEGRadio
            // 
            JPEGRadio.AutoSize = true;
            JPEGRadio.ForeColor = SystemColors.ControlDarkDark;
            JPEGRadio.Location = new Point(6, 47);
            JPEGRadio.Name = "JPEGRadio";
            JPEGRadio.Size = new Size(102, 19);
            JPEGRadio.TabIndex = 1;
            JPEGRadio.TabStop = true;
            JPEGRadio.Text = "JPEG File (.jpg)";
            JPEGRadio.UseVisualStyleBackColor = true;
            // 
            // PNGRadio
            // 
            PNGRadio.AutoSize = true;
            PNGRadio.Checked = true;
            PNGRadio.ForeColor = SystemColors.ControlDarkDark;
            PNGRadio.Location = new Point(6, 22);
            PNGRadio.Name = "PNGRadio";
            PNGRadio.Size = new Size(105, 19);
            PNGRadio.TabIndex = 0;
            PNGRadio.TabStop = true;
            PNGRadio.Text = "PNG File (.png)";
            PNGRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(HeightmapRadio);
            groupBox1.Controls.Add(MapLayersRadio);
            groupBox1.Controls.Add(UpscaledImageRadio);
            groupBox1.Controls.Add(ExportImageRadio);
            groupBox1.Location = new Point(23, 59);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(226, 124);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Export Realm As...";
            // 
            // HeightmapRadio
            // 
            HeightmapRadio.AutoSize = true;
            HeightmapRadio.ForeColor = SystemColors.ControlDarkDark;
            HeightmapRadio.Location = new Point(6, 97);
            HeightmapRadio.Name = "HeightmapRadio";
            HeightmapRadio.Size = new Size(138, 19);
            HeightmapRadio.TabIndex = 3;
            HeightmapRadio.Text = "Grayscale Heightmap";
            HeightmapRadio.UseVisualStyleBackColor = true;
            // 
            // MapLayersRadio
            // 
            MapLayersRadio.AutoSize = true;
            MapLayersRadio.ForeColor = SystemColors.ControlDarkDark;
            MapLayersRadio.Location = new Point(6, 72);
            MapLayersRadio.Name = "MapLayersRadio";
            MapLayersRadio.Size = new Size(85, 19);
            MapLayersRadio.TabIndex = 2;
            MapLayersRadio.Text = "Map Layers";
            MapLayersRadio.UseVisualStyleBackColor = true;
            // 
            // UpscaledImageRadio
            // 
            UpscaledImageRadio.AutoSize = true;
            UpscaledImageRadio.ForeColor = SystemColors.ControlDarkDark;
            UpscaledImageRadio.Location = new Point(6, 47);
            UpscaledImageRadio.Name = "UpscaledImageRadio";
            UpscaledImageRadio.Size = new Size(133, 19);
            UpscaledImageRadio.TabIndex = 1;
            UpscaledImageRadio.Text = "Upscaled Image (2X)";
            UpscaledImageRadio.UseVisualStyleBackColor = true;
            // 
            // ExportImageRadio
            // 
            ExportImageRadio.AutoSize = true;
            ExportImageRadio.Checked = true;
            ExportImageRadio.ForeColor = SystemColors.ControlDarkDark;
            ExportImageRadio.Location = new Point(6, 22);
            ExportImageRadio.Name = "ExportImageRadio";
            ExportImageRadio.Size = new Size(58, 19);
            ExportImageRadio.TabIndex = 0;
            ExportImageRadio.TabStop = true;
            ExportImageRadio.Text = "Image";
            ExportImageRadio.UseVisualStyleBackColor = true;
            // 
            // RealmExportDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(479, 268);
            Controls.Add(ExportRealmOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "RealmExportDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Export Realm";
            TransparencyKey = Color.Fuchsia;
            ExportRealmOverlay.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm ExportRealmOverlay;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button CloseExportButton;
        private Button OKExportButton;
        internal RadioButton ExportImageRadio;
        internal RadioButton MapLayersRadio;
        internal RadioButton UpscaledImageRadio;
        internal RadioButton JPEGRadio;
        internal RadioButton PNGRadio;
        internal RadioButton HeightmapRadio;
        internal RadioButton BMPRadio;
        internal RadioButton GIFRadio;
    }
}