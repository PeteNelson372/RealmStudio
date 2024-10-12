namespace RealmStudio
{
    partial class ThemeList
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
            ThemeListOverlay = new ReaLTaiizor.Forms.DungeonForm();
            ApplyThemeButton = new FontAwesome.Sharp.IconButton();
            SaveThemeButton = new FontAwesome.Sharp.IconButton();
            CloseThemeDialogButton = new FontAwesome.Sharp.IconButton();
            ApplyLabelPresetSettingsCheck = new CheckBox();
            ApplySymbolSettingsCheck = new CheckBox();
            ApplyPathSettingsCheck = new CheckBox();
            ApplyFreshwaterColorPaletteSettingsCheck = new CheckBox();
            ApplyWaterSettingsCheck = new CheckBox();
            ApplyLandformColorPaletteSettingsCheck = new CheckBox();
            ApplyLandformSettingsCheck = new CheckBox();
            ApplyOceanColorPaletteSettingsCheck = new CheckBox();
            ApplyOceanSettingsCheck = new CheckBox();
            ApplyBackgroundSettingsCheck = new CheckBox();
            CheckAllCheck = new CheckBox();
            label1 = new Label();
            ThemeListComboBox = new ComboBox();
            ThemeListOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // ThemeListOverlay
            // 
            ThemeListOverlay.BackColor = Color.FromArgb(244, 241, 243);
            ThemeListOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            ThemeListOverlay.Controls.Add(ApplyThemeButton);
            ThemeListOverlay.Controls.Add(SaveThemeButton);
            ThemeListOverlay.Controls.Add(CloseThemeDialogButton);
            ThemeListOverlay.Controls.Add(ApplyLabelPresetSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplySymbolSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyPathSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyFreshwaterColorPaletteSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyWaterSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyLandformColorPaletteSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyLandformSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyOceanColorPaletteSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyOceanSettingsCheck);
            ThemeListOverlay.Controls.Add(ApplyBackgroundSettingsCheck);
            ThemeListOverlay.Controls.Add(CheckAllCheck);
            ThemeListOverlay.Controls.Add(label1);
            ThemeListOverlay.Controls.Add(ThemeListComboBox);
            ThemeListOverlay.Dock = DockStyle.Fill;
            ThemeListOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            ThemeListOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            ThemeListOverlay.Font = new Font("Segoe UI", 9F);
            ThemeListOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            ThemeListOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            ThemeListOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            ThemeListOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            ThemeListOverlay.Location = new Point(0, 0);
            ThemeListOverlay.Name = "ThemeListOverlay";
            ThemeListOverlay.Padding = new Padding(20, 56, 20, 16);
            ThemeListOverlay.RoundCorners = true;
            ThemeListOverlay.Sizable = true;
            ThemeListOverlay.Size = new Size(261, 530);
            ThemeListOverlay.SmartBounds = true;
            ThemeListOverlay.StartPosition = FormStartPosition.CenterParent;
            ThemeListOverlay.TabIndex = 0;
            ThemeListOverlay.Text = "Save or Apply Theme";
            ThemeListOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // ApplyThemeButton
            // 
            ApplyThemeButton.BackColor = SystemColors.ControlLightLight;
            ApplyThemeButton.DialogResult = DialogResult.OK;
            ApplyThemeButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            ApplyThemeButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyThemeButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyThemeButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ApplyThemeButton.IconColor = Color.Black;
            ApplyThemeButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ApplyThemeButton.IconSize = 18;
            ApplyThemeButton.Location = new Point(31, 451);
            ApplyThemeButton.Name = "ApplyThemeButton";
            ApplyThemeButton.Size = new Size(60, 60);
            ApplyThemeButton.TabIndex = 94;
            ApplyThemeButton.Text = "&Apply";
            ApplyThemeButton.UseVisualStyleBackColor = false;
            // 
            // SaveThemeButton
            // 
            SaveThemeButton.BackColor = SystemColors.ControlLightLight;
            SaveThemeButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            SaveThemeButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SaveThemeButton.ForeColor = SystemColors.ControlDarkDark;
            SaveThemeButton.IconChar = FontAwesome.Sharp.IconChar.None;
            SaveThemeButton.IconColor = Color.Black;
            SaveThemeButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SaveThemeButton.IconSize = 18;
            SaveThemeButton.Location = new Point(97, 451);
            SaveThemeButton.Name = "SaveThemeButton";
            SaveThemeButton.Size = new Size(60, 60);
            SaveThemeButton.TabIndex = 93;
            SaveThemeButton.Text = "&Save As";
            SaveThemeButton.UseVisualStyleBackColor = false;
            SaveThemeButton.Click += SaveThemeButton_Click;
            // 
            // CloseThemeDialogButton
            // 
            CloseThemeDialogButton.BackColor = SystemColors.ControlLightLight;
            CloseThemeDialogButton.DialogResult = DialogResult.Cancel;
            CloseThemeDialogButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            CloseThemeDialogButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CloseThemeDialogButton.ForeColor = SystemColors.ControlDarkDark;
            CloseThemeDialogButton.IconChar = FontAwesome.Sharp.IconChar.None;
            CloseThemeDialogButton.IconColor = Color.Black;
            CloseThemeDialogButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CloseThemeDialogButton.IconSize = 18;
            CloseThemeDialogButton.Location = new Point(163, 451);
            CloseThemeDialogButton.Name = "CloseThemeDialogButton";
            CloseThemeDialogButton.Size = new Size(60, 60);
            CloseThemeDialogButton.TabIndex = 92;
            CloseThemeDialogButton.Text = "&Close";
            CloseThemeDialogButton.UseVisualStyleBackColor = false;
            // 
            // ApplyLabelPresetSettingsCheck
            // 
            ApplyLabelPresetSettingsCheck.AutoSize = true;
            ApplyLabelPresetSettingsCheck.Checked = true;
            ApplyLabelPresetSettingsCheck.CheckState = CheckState.Checked;
            ApplyLabelPresetSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyLabelPresetSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyLabelPresetSettingsCheck.Location = new Point(23, 394);
            ApplyLabelPresetSettingsCheck.Name = "ApplyLabelPresetSettingsCheck";
            ApplyLabelPresetSettingsCheck.Size = new Size(104, 21);
            ApplyLabelPresetSettingsCheck.TabIndex = 12;
            ApplyLabelPresetSettingsCheck.Text = "Label Presets";
            ApplyLabelPresetSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplySymbolSettingsCheck
            // 
            ApplySymbolSettingsCheck.AutoSize = true;
            ApplySymbolSettingsCheck.Checked = true;
            ApplySymbolSettingsCheck.CheckState = CheckState.Checked;
            ApplySymbolSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplySymbolSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplySymbolSettingsCheck.Location = new Point(23, 367);
            ApplySymbolSettingsCheck.Name = "ApplySymbolSettingsCheck";
            ApplySymbolSettingsCheck.Size = new Size(76, 21);
            ApplySymbolSettingsCheck.TabIndex = 11;
            ApplySymbolSettingsCheck.Text = "Symbols";
            ApplySymbolSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyPathSettingsCheck
            // 
            ApplyPathSettingsCheck.AutoSize = true;
            ApplyPathSettingsCheck.Checked = true;
            ApplyPathSettingsCheck.CheckState = CheckState.Checked;
            ApplyPathSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyPathSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyPathSettingsCheck.Location = new Point(23, 340);
            ApplyPathSettingsCheck.Name = "ApplyPathSettingsCheck";
            ApplyPathSettingsCheck.Size = new Size(58, 21);
            ApplyPathSettingsCheck.TabIndex = 10;
            ApplyPathSettingsCheck.Text = "Paths";
            ApplyPathSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyFreshwaterColorPaletteSettingsCheck
            // 
            ApplyFreshwaterColorPaletteSettingsCheck.AutoSize = true;
            ApplyFreshwaterColorPaletteSettingsCheck.Checked = true;
            ApplyFreshwaterColorPaletteSettingsCheck.CheckState = CheckState.Checked;
            ApplyFreshwaterColorPaletteSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyFreshwaterColorPaletteSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyFreshwaterColorPaletteSettingsCheck.Location = new Point(23, 313);
            ApplyFreshwaterColorPaletteSettingsCheck.Name = "ApplyFreshwaterColorPaletteSettingsCheck";
            ApplyFreshwaterColorPaletteSettingsCheck.Size = new Size(175, 21);
            ApplyFreshwaterColorPaletteSettingsCheck.TabIndex = 9;
            ApplyFreshwaterColorPaletteSettingsCheck.Text = "Fresh Water Color Palette";
            ApplyFreshwaterColorPaletteSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyWaterSettingsCheck
            // 
            ApplyWaterSettingsCheck.AutoSize = true;
            ApplyWaterSettingsCheck.Checked = true;
            ApplyWaterSettingsCheck.CheckState = CheckState.Checked;
            ApplyWaterSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyWaterSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyWaterSettingsCheck.Location = new Point(23, 286);
            ApplyWaterSettingsCheck.Name = "ApplyWaterSettingsCheck";
            ApplyWaterSettingsCheck.Size = new Size(200, 21);
            ApplyWaterSettingsCheck.TabIndex = 8;
            ApplyWaterSettingsCheck.Text = "Fresh Water, Lakes and Rivers";
            ApplyWaterSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyLandformColorPaletteSettingsCheck
            // 
            ApplyLandformColorPaletteSettingsCheck.AutoSize = true;
            ApplyLandformColorPaletteSettingsCheck.Checked = true;
            ApplyLandformColorPaletteSettingsCheck.CheckState = CheckState.Checked;
            ApplyLandformColorPaletteSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyLandformColorPaletteSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyLandformColorPaletteSettingsCheck.Location = new Point(23, 259);
            ApplyLandformColorPaletteSettingsCheck.Name = "ApplyLandformColorPaletteSettingsCheck";
            ApplyLandformColorPaletteSettingsCheck.Size = new Size(162, 21);
            ApplyLandformColorPaletteSettingsCheck.TabIndex = 7;
            ApplyLandformColorPaletteSettingsCheck.Text = "Landform Color Palette";
            ApplyLandformColorPaletteSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyLandformSettingsCheck
            // 
            ApplyLandformSettingsCheck.AutoSize = true;
            ApplyLandformSettingsCheck.Checked = true;
            ApplyLandformSettingsCheck.CheckState = CheckState.Checked;
            ApplyLandformSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyLandformSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyLandformSettingsCheck.Location = new Point(23, 232);
            ApplyLandformSettingsCheck.Name = "ApplyLandformSettingsCheck";
            ApplyLandformSettingsCheck.Size = new Size(89, 21);
            ApplyLandformSettingsCheck.TabIndex = 6;
            ApplyLandformSettingsCheck.Text = "Landforms";
            ApplyLandformSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyOceanColorPaletteSettingsCheck
            // 
            ApplyOceanColorPaletteSettingsCheck.AutoSize = true;
            ApplyOceanColorPaletteSettingsCheck.Checked = true;
            ApplyOceanColorPaletteSettingsCheck.CheckState = CheckState.Checked;
            ApplyOceanColorPaletteSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyOceanColorPaletteSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyOceanColorPaletteSettingsCheck.Location = new Point(23, 205);
            ApplyOceanColorPaletteSettingsCheck.Name = "ApplyOceanColorPaletteSettingsCheck";
            ApplyOceanColorPaletteSettingsCheck.Size = new Size(143, 21);
            ApplyOceanColorPaletteSettingsCheck.TabIndex = 5;
            ApplyOceanColorPaletteSettingsCheck.Text = "Ocean Color Palette";
            ApplyOceanColorPaletteSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyOceanSettingsCheck
            // 
            ApplyOceanSettingsCheck.AutoSize = true;
            ApplyOceanSettingsCheck.Checked = true;
            ApplyOceanSettingsCheck.CheckState = CheckState.Checked;
            ApplyOceanSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyOceanSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyOceanSettingsCheck.Location = new Point(23, 178);
            ApplyOceanSettingsCheck.Name = "ApplyOceanSettingsCheck";
            ApplyOceanSettingsCheck.Size = new Size(64, 21);
            ApplyOceanSettingsCheck.TabIndex = 4;
            ApplyOceanSettingsCheck.Text = "Ocean";
            ApplyOceanSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyBackgroundSettingsCheck
            // 
            ApplyBackgroundSettingsCheck.AutoSize = true;
            ApplyBackgroundSettingsCheck.Checked = true;
            ApplyBackgroundSettingsCheck.CheckState = CheckState.Checked;
            ApplyBackgroundSettingsCheck.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplyBackgroundSettingsCheck.ForeColor = SystemColors.ControlDarkDark;
            ApplyBackgroundSettingsCheck.Location = new Point(23, 151);
            ApplyBackgroundSettingsCheck.Name = "ApplyBackgroundSettingsCheck";
            ApplyBackgroundSettingsCheck.Size = new Size(96, 21);
            ApplyBackgroundSettingsCheck.TabIndex = 3;
            ApplyBackgroundSettingsCheck.Text = "Background";
            ApplyBackgroundSettingsCheck.UseVisualStyleBackColor = true;
            // 
            // CheckAllCheck
            // 
            CheckAllCheck.AutoSize = true;
            CheckAllCheck.Checked = true;
            CheckAllCheck.CheckState = CheckState.Checked;
            CheckAllCheck.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CheckAllCheck.ForeColor = SystemColors.ControlDarkDark;
            CheckAllCheck.Location = new Point(23, 121);
            CheckAllCheck.Name = "CheckAllCheck";
            CheckAllCheck.Size = new Size(146, 24);
            CheckAllCheck.TabIndex = 2;
            CheckAllCheck.Text = "Apply Settings To";
            CheckAllCheck.UseVisualStyleBackColor = true;
            CheckAllCheck.CheckedChanged += CheckAllCheck_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(23, 56);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 1;
            label1.Text = "Themes";
            // 
            // ThemeListComboBox
            // 
            ThemeListComboBox.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ThemeListComboBox.FormattingEnabled = true;
            ThemeListComboBox.Location = new Point(23, 74);
            ThemeListComboBox.Name = "ThemeListComboBox";
            ThemeListComboBox.Size = new Size(200, 28);
            ThemeListComboBox.TabIndex = 0;
            ThemeListComboBox.SelectionChangeCommitted += ThemeListComboBox_SelectionChangeCommitted;
            // 
            // ThemeList
            // 
            AcceptButton = ApplyThemeButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CloseThemeDialogButton;
            ClientSize = new Size(261, 530);
            Controls.Add(ThemeListOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "ThemeList";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Save or Apply Theme";
            TransparencyKey = Color.Fuchsia;
            ThemeListOverlay.ResumeLayout(false);
            ThemeListOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm ThemeListOverlay;
        private Label label1;
        private ComboBox ThemeListComboBox;
        private CheckBox CheckAllCheck;
        private CheckBox ApplyPathSettingsCheck;
        private CheckBox ApplyFreshwaterColorPaletteSettingsCheck;
        private CheckBox ApplyWaterSettingsCheck;
        private CheckBox ApplyLandformColorPaletteSettingsCheck;
        private CheckBox ApplyLandformSettingsCheck;
        private CheckBox ApplyOceanColorPaletteSettingsCheck;
        private CheckBox ApplyOceanSettingsCheck;
        private CheckBox ApplyBackgroundSettingsCheck;
        private CheckBox ApplyLabelPresetSettingsCheck;
        private CheckBox ApplySymbolSettingsCheck;
        private FontAwesome.Sharp.IconButton ApplyThemeButton;
        private FontAwesome.Sharp.IconButton SaveThemeButton;
        private FontAwesome.Sharp.IconButton CloseThemeDialogButton;
    }
}