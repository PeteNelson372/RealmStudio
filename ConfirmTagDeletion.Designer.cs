namespace RealmStudioX
{
    partial class ConfirmTagDeletion
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
            ConfirmTagDeletionButton = new FontAwesome.Sharp.IconButton();
            CloseTagDeletionButton = new FontAwesome.Sharp.IconButton();
            SuspendLayout();
            // 
            // ConfirmTagDeletionButton
            // 
            ConfirmTagDeletionButton.BackColor = Color.LimeGreen;
            ConfirmTagDeletionButton.DialogResult = DialogResult.OK;
            ConfirmTagDeletionButton.IconChar = FontAwesome.Sharp.IconChar.Check;
            ConfirmTagDeletionButton.IconColor = Color.White;
            ConfirmTagDeletionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ConfirmTagDeletionButton.IconSize = 14;
            ConfirmTagDeletionButton.Location = new Point(1, 1);
            ConfirmTagDeletionButton.Name = "ConfirmTagDeletionButton";
            ConfirmTagDeletionButton.Size = new Size(24, 24);
            ConfirmTagDeletionButton.TabIndex = 0;
            ConfirmTagDeletionButton.UseVisualStyleBackColor = false;
            // 
            // CloseTagDeletionButton
            // 
            CloseTagDeletionButton.BackColor = Color.OrangeRed;
            CloseTagDeletionButton.DialogResult = DialogResult.Cancel;
            CloseTagDeletionButton.IconChar = FontAwesome.Sharp.IconChar.Cancel;
            CloseTagDeletionButton.IconColor = Color.White;
            CloseTagDeletionButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            CloseTagDeletionButton.IconSize = 14;
            CloseTagDeletionButton.Location = new Point(26, 1);
            CloseTagDeletionButton.Name = "CloseTagDeletionButton";
            CloseTagDeletionButton.Size = new Size(24, 24);
            CloseTagDeletionButton.TabIndex = 1;
            CloseTagDeletionButton.UseVisualStyleBackColor = false;
            // 
            // ConfirmTagDeletion
            // 
            AcceptButton = ConfirmTagDeletionButton;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.WhiteSmoke;
            CancelButton = CloseTagDeletionButton;
            ClientSize = new Size(53, 28);
            ControlBox = false;
            Controls.Add(CloseTagDeletionButton);
            Controls.Add(ConfirmTagDeletionButton);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MaximumSize = new Size(53, 28);
            MinimizeBox = false;
            MinimumSize = new Size(53, 28);
            Name = "ConfirmTagDeletion";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            Text = "ConfirmTagDeletion";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private FontAwesome.Sharp.IconButton ConfirmTagDeletionButton;
        private FontAwesome.Sharp.IconButton CloseTagDeletionButton;
    }
}