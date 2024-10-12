﻿namespace RealmStudio
{
    partial class ThemeNameEntry
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
            ThemeNameEntryOverlay = new ReaLTaiizor.Forms.DungeonForm();
            ThemeNameOkButton = new FontAwesome.Sharp.IconButton();
            ThemeNameCancelButton = new FontAwesome.Sharp.IconButton();
            ThemeNameTextBox = new TextBox();
            ThemeNameEntryOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // ThemeNameEntryOverlay
            // 
            ThemeNameEntryOverlay.BackColor = Color.FromArgb(244, 241, 243);
            ThemeNameEntryOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            ThemeNameEntryOverlay.Controls.Add(ThemeNameOkButton);
            ThemeNameEntryOverlay.Controls.Add(ThemeNameCancelButton);
            ThemeNameEntryOverlay.Controls.Add(ThemeNameTextBox);
            ThemeNameEntryOverlay.Dock = DockStyle.Fill;
            ThemeNameEntryOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            ThemeNameEntryOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            ThemeNameEntryOverlay.Font = new Font("Segoe UI", 9F);
            ThemeNameEntryOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            ThemeNameEntryOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            ThemeNameEntryOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            ThemeNameEntryOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            ThemeNameEntryOverlay.Location = new Point(0, 0);
            ThemeNameEntryOverlay.Name = "ThemeNameEntryOverlay";
            ThemeNameEntryOverlay.Padding = new Padding(20, 56, 20, 16);
            ThemeNameEntryOverlay.RoundCorners = true;
            ThemeNameEntryOverlay.Sizable = true;
            ThemeNameEntryOverlay.Size = new Size(457, 184);
            ThemeNameEntryOverlay.SmartBounds = true;
            ThemeNameEntryOverlay.StartPosition = FormStartPosition.CenterParent;
            ThemeNameEntryOverlay.TabIndex = 0;
            ThemeNameEntryOverlay.Text = "Theme Name";
            ThemeNameEntryOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // ThemeNameOkButton
            // 
            ThemeNameOkButton.BackColor = SystemColors.ControlLightLight;
            ThemeNameOkButton.DialogResult = DialogResult.OK;
            ThemeNameOkButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            ThemeNameOkButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ThemeNameOkButton.ForeColor = SystemColors.ControlDarkDark;
            ThemeNameOkButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ThemeNameOkButton.IconColor = Color.Black;
            ThemeNameOkButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ThemeNameOkButton.IconSize = 18;
            ThemeNameOkButton.Location = new Point(308, 105);
            ThemeNameOkButton.Name = "ThemeNameOkButton";
            ThemeNameOkButton.Size = new Size(60, 60);
            ThemeNameOkButton.TabIndex = 94;
            ThemeNameOkButton.Text = "O&K";
            ThemeNameOkButton.UseVisualStyleBackColor = false;
            // 
            // ThemeNameCancelButton
            // 
            ThemeNameCancelButton.BackColor = SystemColors.ControlLightLight;
            ThemeNameCancelButton.DialogResult = DialogResult.Cancel;
            ThemeNameCancelButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            ThemeNameCancelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ThemeNameCancelButton.ForeColor = SystemColors.ControlDarkDark;
            ThemeNameCancelButton.IconChar = FontAwesome.Sharp.IconChar.None;
            ThemeNameCancelButton.IconColor = Color.Black;
            ThemeNameCancelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ThemeNameCancelButton.IconSize = 18;
            ThemeNameCancelButton.Location = new Point(374, 105);
            ThemeNameCancelButton.Name = "ThemeNameCancelButton";
            ThemeNameCancelButton.Size = new Size(60, 60);
            ThemeNameCancelButton.TabIndex = 93;
            ThemeNameCancelButton.Text = "&Cancel";
            ThemeNameCancelButton.UseVisualStyleBackColor = false;
            // 
            // ThemeNameTextBox
            // 
            ThemeNameTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ThemeNameTextBox.Location = new Point(23, 59);
            ThemeNameTextBox.Name = "ThemeNameTextBox";
            ThemeNameTextBox.Size = new Size(411, 27);
            ThemeNameTextBox.TabIndex = 0;
            // 
            // ThemeNameEntry
            // 
            AcceptButton = ThemeNameOkButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = ThemeNameCancelButton;
            ClientSize = new Size(457, 184);
            Controls.Add(ThemeNameEntryOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "ThemeNameEntry";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Theme Name";
            TopMost = true;
            TransparencyKey = Color.Fuchsia;
            ThemeNameEntryOverlay.ResumeLayout(false);
            ThemeNameEntryOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm ThemeNameEntryOverlay;
        private FontAwesome.Sharp.IconButton ThemeNameCancelButton;
        private FontAwesome.Sharp.IconButton ThemeNameOkButton;
        public TextBox ThemeNameTextBox;
    }
}