﻿namespace RealmStudio
{
    partial class AppSplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppSplashScreen));
            pictureBox1 = new PictureBox();
            VersionLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = Properties.Resources.ZeroSum_white___no_background;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(108, 89);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // VersionLabel
            // 
            VersionLabel.BackColor = Color.Transparent;
            VersionLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            VersionLabel.ForeColor = SystemColors.ControlDarkDark;
            VersionLabel.Location = new Point(302, 405);
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new Size(306, 23);
            VersionLabel.TabIndex = 1;
            VersionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AppSplashScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(640, 480);
            Controls.Add(VersionLabel);
            Controls.Add(pictureBox1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "AppSplashScreen";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AppSplashScreen";
            TopMost = true;
            TransparencyKey = Color.FromArgb(239, 136, 190);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        public Label VersionLabel;
    }
}