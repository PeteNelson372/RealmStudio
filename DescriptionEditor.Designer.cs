namespace RealmStudio
{
    partial class DescriptionEditor
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
            DescrptionEditorOverlay = new ReaLTaiizor.Forms.DungeonForm();
            CloseDescriptionButton = new Button();
            DescriptionTextbox = new TextBox();
            DescrptionEditorOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // DescrptionEditorOverlay
            // 
            DescrptionEditorOverlay.BackColor = Color.FromArgb(244, 241, 243);
            DescrptionEditorOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            DescrptionEditorOverlay.Controls.Add(DescriptionTextbox);
            DescrptionEditorOverlay.Controls.Add(CloseDescriptionButton);
            DescrptionEditorOverlay.Dock = DockStyle.Fill;
            DescrptionEditorOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            DescrptionEditorOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            DescrptionEditorOverlay.Font = new Font("Segoe UI", 9F);
            DescrptionEditorOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            DescrptionEditorOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            DescrptionEditorOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            DescrptionEditorOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            DescrptionEditorOverlay.Location = new Point(0, 0);
            DescrptionEditorOverlay.Name = "DescrptionEditorOverlay";
            DescrptionEditorOverlay.Padding = new Padding(20, 56, 20, 16);
            DescrptionEditorOverlay.RoundCorners = true;
            DescrptionEditorOverlay.Sizable = true;
            DescrptionEditorOverlay.Size = new Size(598, 430);
            DescrptionEditorOverlay.SmartBounds = true;
            DescrptionEditorOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            DescrptionEditorOverlay.TabIndex = 0;
            DescrptionEditorOverlay.Text = "Description";
            DescrptionEditorOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // CloseDescriptionButton
            // 
            CloseDescriptionButton.DialogResult = DialogResult.OK;
            CloseDescriptionButton.ForeColor = SystemColors.ControlDarkDark;
            CloseDescriptionButton.Location = new Point(515, 351);
            CloseDescriptionButton.Name = "CloseDescriptionButton";
            CloseDescriptionButton.Size = new Size(60, 60);
            CloseDescriptionButton.TabIndex = 93;
            CloseDescriptionButton.Text = "&Close";
            CloseDescriptionButton.UseVisualStyleBackColor = true;
            CloseDescriptionButton.Click += CloseDescriptionButton_Click;
            // 
            // DescriptionTextbox
            // 
            DescriptionTextbox.AcceptsReturn = true;
            DescriptionTextbox.AcceptsTab = true;
            DescriptionTextbox.AllowDrop = true;
            DescriptionTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DescriptionTextbox.Location = new Point(23, 59);
            DescriptionTextbox.Multiline = true;
            DescriptionTextbox.Name = "DescriptionTextbox";
            DescriptionTextbox.ScrollBars = ScrollBars.Vertical;
            DescriptionTextbox.Size = new Size(552, 286);
            DescriptionTextbox.TabIndex = 94;
            // 
            // DescriptionEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(598, 430);
            Controls.Add(DescrptionEditorOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "DescriptionEditor";
            Text = "Description";
            TransparencyKey = Color.Fuchsia;
            DescrptionEditorOverlay.ResumeLayout(false);
            DescrptionEditorOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button CloseDescriptionButton;
        internal ReaLTaiizor.Forms.DungeonForm DescrptionEditorOverlay;
        internal TextBox DescriptionTextbox;
    }
}