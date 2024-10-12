namespace RealmStudio
{
    partial class LoadingStatusForm
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
            LoadingStatusFormOverlay = new ReaLTaiizor.Forms.DungeonForm();
            LoadingProgressBar = new ProgressBar();
            LoadingStatusFormOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // LoadingStatusFormOverlay
            // 
            LoadingStatusFormOverlay.BackColor = Color.FromArgb(38, 38, 38);
            LoadingStatusFormOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            LoadingStatusFormOverlay.Controls.Add(LoadingProgressBar);
            LoadingStatusFormOverlay.Dock = DockStyle.Fill;
            LoadingStatusFormOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            LoadingStatusFormOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            LoadingStatusFormOverlay.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoadingStatusFormOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            LoadingStatusFormOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            LoadingStatusFormOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            LoadingStatusFormOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            LoadingStatusFormOverlay.Location = new Point(0, 0);
            LoadingStatusFormOverlay.Name = "LoadingStatusFormOverlay";
            LoadingStatusFormOverlay.Padding = new Padding(20, 56, 20, 16);
            LoadingStatusFormOverlay.RoundCorners = true;
            LoadingStatusFormOverlay.Sizable = false;
            LoadingStatusFormOverlay.Size = new Size(519, 95);
            LoadingStatusFormOverlay.SmartBounds = true;
            LoadingStatusFormOverlay.StartPosition = FormStartPosition.CenterParent;
            LoadingStatusFormOverlay.TabIndex = 0;
            LoadingStatusFormOverlay.Text = "Loading...";
            LoadingStatusFormOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // LoadingProgressBar
            // 
            LoadingProgressBar.Location = new Point(12, 53);
            LoadingProgressBar.Name = "LoadingProgressBar";
            LoadingProgressBar.Size = new Size(495, 30);
            LoadingProgressBar.Style = ProgressBarStyle.Continuous;
            LoadingProgressBar.TabIndex = 0;
            // 
            // LoadingStatusForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            CausesValidation = false;
            ClientSize = new Size(519, 95);
            ControlBox = false;
            Controls.Add(LoadingStatusFormOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(261, 65);
            Name = "LoadingStatusForm";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Loading...";
            TopMost = true;
            TransparencyKey = Color.Fuchsia;
            LoadingStatusFormOverlay.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm LoadingStatusFormOverlay;
        private ProgressBar LoadingProgressBar;
    }
}