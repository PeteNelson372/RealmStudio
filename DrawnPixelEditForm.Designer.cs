namespace RealmStudioX
{
    partial class DrawnPixelEditForm
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
            DrawnPixelEditFormOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CancelCreateDetailButton = new Button();
            OKButton = new Button();
            SelectFillColorButton = new FontAwesome.Sharp.IconButton();
            PixelEditor = new Cyotek.Windows.Forms.ImageBox();
            ClearPixelEditsButton = new FontAwesome.Sharp.IconButton();
            DrawnPixelEditFormOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // DrawnPixelEditFormOverlay
            // 
            DrawnPixelEditFormOverlay.BackColor = Color.FromArgb(244, 241, 243);
            DrawnPixelEditFormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            DrawnPixelEditFormOverlay.Controls.Add(ClearPixelEditsButton);
            DrawnPixelEditFormOverlay.Controls.Add(CancelCreateDetailButton);
            DrawnPixelEditFormOverlay.Controls.Add(OKButton);
            DrawnPixelEditFormOverlay.Controls.Add(SelectFillColorButton);
            DrawnPixelEditFormOverlay.Controls.Add(PixelEditor);
            DrawnPixelEditFormOverlay.Dock = DockStyle.Fill;
            DrawnPixelEditFormOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            DrawnPixelEditFormOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            DrawnPixelEditFormOverlay.Font = new Font("Segoe UI", 9F);
            DrawnPixelEditFormOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            DrawnPixelEditFormOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            DrawnPixelEditFormOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            DrawnPixelEditFormOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            DrawnPixelEditFormOverlay.Location = new Point(0, 0);
            DrawnPixelEditFormOverlay.Name = "DrawnPixelEditFormOverlay";
            DrawnPixelEditFormOverlay.Padding = new Padding(20, 56, 20, 16);
            DrawnPixelEditFormOverlay.RoundCorners = true;
            DrawnPixelEditFormOverlay.Sizable = false;
            DrawnPixelEditFormOverlay.Size = new Size(695, 593);
            DrawnPixelEditFormOverlay.SmartBounds = true;
            DrawnPixelEditFormOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            DrawnPixelEditFormOverlay.TabIndex = 0;
            DrawnPixelEditFormOverlay.Text = "Pixel Edit";
            DrawnPixelEditFormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CancelCreateDetailButton
            // 
            CancelCreateDetailButton.DialogResult = DialogResult.Cancel;
            CancelCreateDetailButton.ForeColor = SystemColors.ControlDarkDark;
            CancelCreateDetailButton.Location = new Point(612, 512);
            CancelCreateDetailButton.Name = "CancelCreateDetailButton";
            CancelCreateDetailButton.Size = new Size(60, 60);
            CancelCreateDetailButton.TabIndex = 32;
            CancelCreateDetailButton.Text = "&Cancel";
            CancelCreateDetailButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            OKButton.DialogResult = DialogResult.OK;
            OKButton.ForeColor = SystemColors.ControlDarkDark;
            OKButton.Location = new Point(546, 512);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(60, 60);
            OKButton.TabIndex = 31;
            OKButton.Text = "O&K";
            OKButton.UseVisualStyleBackColor = true;
            // 
            // SelectFillColorButton
            // 
            SelectFillColorButton.BackColor = Color.Transparent;
            SelectFillColorButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SelectFillColorButton.ForeColor = SystemColors.ControlDarkDark;
            SelectFillColorButton.IconChar = FontAwesome.Sharp.IconChar.Palette;
            SelectFillColorButton.IconColor = Color.Tan;
            SelectFillColorButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SelectFillColorButton.Location = new Point(541, 59);
            SelectFillColorButton.Name = "SelectFillColorButton";
            SelectFillColorButton.Size = new Size(136, 60);
            SelectFillColorButton.TabIndex = 30;
            SelectFillColorButton.Text = "Click to Select";
            SelectFillColorButton.UseVisualStyleBackColor = false;
            SelectFillColorButton.MouseUp += SelectFillColorButton_MouseUp;
            // 
            // PixelEditor
            // 
            PixelEditor.AllowFreePan = false;
            PixelEditor.AllowZoom = false;
            PixelEditor.AutoCenter = false;
            PixelEditor.AutoPan = false;
            PixelEditor.AutoScroll = false;
            PixelEditor.BorderStyle = BorderStyle.FixedSingle;
            PixelEditor.Location = new Point(23, 59);
            PixelEditor.Name = "PixelEditor";
            PixelEditor.PanMode = Cyotek.Windows.Forms.ImageBoxPanMode.Middle;
            PixelEditor.ShowPixelGrid = true;
            PixelEditor.Size = new Size(513, 513);
            PixelEditor.TabIndex = 0;
            PixelEditor.VirtualSize = new Size(64, 64);
            PixelEditor.MouseClick += PixelEditor_MouseClick;
            PixelEditor.MouseMove += PixelEditor_MouseMove;
            // 
            // ClearPixelEditsButton
            // 
            ClearPixelEditsButton.ForeColor = SystemColors.ControlDarkDark;
            ClearPixelEditsButton.IconChar = FontAwesome.Sharp.IconChar.SquareMinus;
            ClearPixelEditsButton.IconColor = Color.Black;
            ClearPixelEditsButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ClearPixelEditsButton.IconSize = 32;
            ClearPixelEditsButton.Location = new Point(542, 417);
            ClearPixelEditsButton.Name = "ClearPixelEditsButton";
            ClearPixelEditsButton.Size = new Size(136, 60);
            ClearPixelEditsButton.TabIndex = 33;
            ClearPixelEditsButton.Text = "Clear All Changes";
            ClearPixelEditsButton.TextImageRelation = TextImageRelation.TextAboveImage;
            ClearPixelEditsButton.UseVisualStyleBackColor = true;
            ClearPixelEditsButton.Click += ClearPixelEditsButton_Click;
            // 
            // DrawnPixelEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(695, 593);
            Controls.Add(DrawnPixelEditFormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "DrawnPixelEditForm";
            Text = "Pixel Edit";
            TransparencyKey = Color.Fuchsia;
            DrawnPixelEditFormOverlay.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm DrawnPixelEditFormOverlay;
        private Cyotek.Windows.Forms.ImageBox PixelEditor;
        internal FontAwesome.Sharp.IconButton SelectFillColorButton;
        private Button CancelCreateDetailButton;
        private Button OKButton;
        private FontAwesome.Sharp.IconButton ClearPixelEditsButton;
    }
}