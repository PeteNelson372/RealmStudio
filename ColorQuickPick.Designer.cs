namespace RealmStudioX
{
    partial class ColorQuickPick
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
            SelectColorGrid = new Cyotek.Windows.Forms.ColorGrid();
            SuspendLayout();
            // 
            // SelectColorGrid
            // 
            SelectColorGrid.EditMode = Cyotek.Windows.Forms.ColorEditingMode.None;
            SelectColorGrid.Location = new Point(13, 13);
            SelectColorGrid.Name = "SelectColorGrid";
            SelectColorGrid.Palette = Cyotek.Windows.Forms.ColorPalette.Standard256;
            SelectColorGrid.SelectedCellStyle = Cyotek.Windows.Forms.ColorGridSelectedCellStyle.None;
            SelectColorGrid.ShowCustomColors = false;
            SelectColorGrid.Size = new Size(314, 318);
            SelectColorGrid.TabIndex = 0;
            SelectColorGrid.ColorChanged += SelectColorGrid_ColorChanged;
            // 
            // ColorQuickPick
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(339, 341);
            Controls.Add(SelectColorGrid);
            Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(261, 67);
            Name = "ColorQuickPick";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Color";
            TransparencyKey = Color.Fuchsia;
            Paint += ColorQuickPick_Paint;
            ResumeLayout(false);
        }

        #endregion

        private Cyotek.Windows.Forms.ColorGrid SelectColorGrid;
    }
}