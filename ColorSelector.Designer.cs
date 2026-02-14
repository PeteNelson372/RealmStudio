namespace RealmStudioX
{
    partial class ColorSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorSelector));
            ColorSelectorWheel = new Cyotek.Windows.Forms.ColorWheel();
            ColorSelectorEditor = new Cyotek.Windows.Forms.ColorEditor();
            ColorSelectorGrid = new Cyotek.Windows.Forms.ColorGrid();
            ColorSelectorManager = new Cyotek.Windows.Forms.ColorEditorManager();
            ColorSelectorPicker = new Cyotek.Windows.Forms.ScreenColorPicker();
            ColorPreviewLabel = new Label();
            CloseButton = new Button();
            OKButton = new Button();
            SuspendLayout();
            // 
            // ColorSelectorWheel
            // 
            ColorSelectorWheel.Alpha = 1D;
            ColorSelectorWheel.DisplayLightness = true;
            ColorSelectorWheel.Location = new Point(12, 13);
            ColorSelectorWheel.Name = "ColorSelectorWheel";
            ColorSelectorWheel.ShowAngleArrow = true;
            ColorSelectorWheel.ShowCenterLines = true;
            ColorSelectorWheel.ShowSaturationRing = true;
            ColorSelectorWheel.Size = new Size(250, 297);
            ColorSelectorWheel.TabIndex = 0;
            ColorSelectorWheel.ColorChanged += ColorSelectorWheel_ColorChanged;
            // 
            // ColorSelectorEditor
            // 
            ColorSelectorEditor.Location = new Point(269, 13);
            ColorSelectorEditor.Margin = new Padding(4, 3, 4, 3);
            ColorSelectorEditor.Name = "ColorSelectorEditor";
            ColorSelectorEditor.PreserveAlphaChannel = true;
            ColorSelectorEditor.Size = new Size(208, 297);
            ColorSelectorEditor.TabIndex = 1;
            ColorSelectorEditor.ColorChanged += ColorSelectorEditor_ColorChanged;
            // 
            // ColorSelectorGrid
            // 
            ColorSelectorGrid.CellBorderStyle = Cyotek.Windows.Forms.ColorCellBorderStyle.None;
            ColorSelectorGrid.Location = new Point(12, 316);
            ColorSelectorGrid.Name = "ColorSelectorGrid";
            ColorSelectorGrid.SelectedCellStyle = Cyotek.Windows.Forms.ColorGridSelectedCellStyle.Standard;
            ColorSelectorGrid.ShowCustomColors = false;
            ColorSelectorGrid.Size = new Size(250, 148);
            ColorSelectorGrid.TabIndex = 2;
            // 
            // ColorSelectorManager
            // 
            ColorSelectorManager.Color = Color.Empty;
            ColorSelectorManager.ColorEditor = ColorSelectorEditor;
            ColorSelectorManager.ColorGrid = ColorSelectorGrid;
            ColorSelectorManager.ColorWheel = ColorSelectorWheel;
            ColorSelectorManager.ScreenColorPicker = ColorSelectorPicker;
            ColorSelectorManager.ColorChanged += ColorSelectorManager_ColorChanged;
            // 
            // ColorSelectorPicker
            // 
            ColorSelectorPicker.BackgroundImage = (Image)resources.GetObject("ColorSelectorPicker.BackgroundImage");
            ColorSelectorPicker.BackgroundImageLayout = ImageLayout.Center;
            ColorSelectorPicker.Color = Color.Empty;
            ColorSelectorPicker.Location = new Point(269, 316);
            ColorSelectorPicker.Name = "ColorSelectorPicker";
            ColorSelectorPicker.Size = new Size(88, 78);
            ColorSelectorPicker.Selected += ColorSelectorPicker_Selected;
            // 
            // ColorPreviewLabel
            // 
            ColorPreviewLabel.BackColor = Color.White;
            ColorPreviewLabel.BorderStyle = BorderStyle.FixedSingle;
            ColorPreviewLabel.Location = new Point(363, 316);
            ColorPreviewLabel.Name = "ColorPreviewLabel";
            ColorPreviewLabel.Size = new Size(114, 78);
            ColorPreviewLabel.TabIndex = 3;
            // 
            // CloseButton
            // 
            CloseButton.DialogResult = DialogResult.Cancel;
            CloseButton.Location = new Point(423, 414);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(54, 50);
            CloseButton.TabIndex = 14;
            CloseButton.Text = "&Cancel";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // OKButton
            // 
            OKButton.DialogResult = DialogResult.OK;
            OKButton.Location = new Point(363, 414);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(54, 50);
            OKButton.TabIndex = 13;
            OKButton.Text = "O&K";
            OKButton.UseVisualStyleBackColor = true;
            // 
            // ColorSelector
            // 
            AcceptButton = OKButton;
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CloseButton;
            ClientSize = new Size(489, 480);
            Controls.Add(ColorSelectorPicker);
            Controls.Add(CloseButton);
            Controls.Add(OKButton);
            Controls.Add(ColorPreviewLabel);
            Controls.Add(ColorSelectorGrid);
            Controls.Add(ColorSelectorEditor);
            Controls.Add(ColorSelectorWheel);
            Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ColorSelector";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Color";
            TopMost = true;
            Paint += ColorSelector_Paint;
            ResumeLayout(false);
        }

        #endregion

        private Cyotek.Windows.Forms.ColorWheel ColorSelectorWheel;
        private Cyotek.Windows.Forms.ColorEditor ColorSelectorEditor;
        private Cyotek.Windows.Forms.ColorGrid ColorSelectorGrid;
        private Cyotek.Windows.Forms.ColorEditorManager ColorSelectorManager;
        private Label ColorPreviewLabel;
        private Button CloseButton;
        private Button OKButton;
        private Cyotek.Windows.Forms.ScreenColorPicker ColorSelectorPicker;
    }
}