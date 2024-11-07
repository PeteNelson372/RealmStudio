namespace RealmStudio
{
    partial class NameGeneratorConfiguration
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
            NameConfigOverlayForm = new ReaLTaiizor.Forms.DungeonForm();
            ClearNameListButton = new FontAwesome.Sharp.IconButton();
            ClipboardButton = new FontAwesome.Sharp.IconButton();
            GenerateNamesButton = new Button();
            GeneratedNamesList = new ListBox();
            SelectAllLanguagesCheck = new CheckBox();
            label3 = new Label();
            LanguagesListBox = new CheckedListBox();
            SelectAllNamebasesCheck = new CheckBox();
            label2 = new Label();
            NamebasesListBox = new CheckedListBox();
            SelectAllNameGeneratorsCheck = new CheckBox();
            label1 = new Label();
            NameGeneratorsListBox = new CheckedListBox();
            CloseNameConfigurationButton = new Button();
            ApplySelectedNameButton = new Button();
            materialCheckBox1 = new ReaLTaiizor.Controls.MaterialCheckBox();
            materialCheckBox2 = new ReaLTaiizor.Controls.MaterialCheckBox();
            materialCheckBox3 = new ReaLTaiizor.Controls.MaterialCheckBox();
            materialCheckBox4 = new ReaLTaiizor.Controls.MaterialCheckBox();
            NameConfigOverlayForm.SuspendLayout();
            SuspendLayout();
            // 
            // NameConfigOverlayForm
            // 
            NameConfigOverlayForm.BackColor = Color.FromArgb(244, 241, 243);
            NameConfigOverlayForm.BorderColor = Color.FromArgb(38, 38, 38);
            NameConfigOverlayForm.Controls.Add(ClearNameListButton);
            NameConfigOverlayForm.Controls.Add(ClipboardButton);
            NameConfigOverlayForm.Controls.Add(GenerateNamesButton);
            NameConfigOverlayForm.Controls.Add(GeneratedNamesList);
            NameConfigOverlayForm.Controls.Add(SelectAllLanguagesCheck);
            NameConfigOverlayForm.Controls.Add(label3);
            NameConfigOverlayForm.Controls.Add(LanguagesListBox);
            NameConfigOverlayForm.Controls.Add(SelectAllNamebasesCheck);
            NameConfigOverlayForm.Controls.Add(label2);
            NameConfigOverlayForm.Controls.Add(NamebasesListBox);
            NameConfigOverlayForm.Controls.Add(SelectAllNameGeneratorsCheck);
            NameConfigOverlayForm.Controls.Add(label1);
            NameConfigOverlayForm.Controls.Add(NameGeneratorsListBox);
            NameConfigOverlayForm.Controls.Add(CloseNameConfigurationButton);
            NameConfigOverlayForm.Controls.Add(ApplySelectedNameButton);
            NameConfigOverlayForm.Dock = DockStyle.Fill;
            NameConfigOverlayForm.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            NameConfigOverlayForm.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            NameConfigOverlayForm.Font = new Font("Segoe UI", 9F);
            NameConfigOverlayForm.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            NameConfigOverlayForm.ForeColor = Color.FromArgb(223, 219, 210);
            NameConfigOverlayForm.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            NameConfigOverlayForm.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            NameConfigOverlayForm.Location = new Point(0, 0);
            NameConfigOverlayForm.Name = "NameConfigOverlayForm";
            NameConfigOverlayForm.Padding = new Padding(20, 56, 20, 16);
            NameConfigOverlayForm.RoundCorners = true;
            NameConfigOverlayForm.Sizable = true;
            NameConfigOverlayForm.Size = new Size(563, 431);
            NameConfigOverlayForm.SmartBounds = true;
            NameConfigOverlayForm.StartPosition = FormStartPosition.CenterParent;
            NameConfigOverlayForm.TabIndex = 0;
            NameConfigOverlayForm.Text = "Configure Name Generation";
            NameConfigOverlayForm.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // ClearNameListButton
            // 
            ClearNameListButton.IconChar = FontAwesome.Sharp.IconChar.TrashAlt;
            ClearNameListButton.IconColor = Color.Black;
            ClearNameListButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ClearNameListButton.IconSize = 14;
            ClearNameListButton.Location = new Point(516, 75);
            ClearNameListButton.Name = "ClearNameListButton";
            ClearNameListButton.Size = new Size(24, 24);
            ClearNameListButton.TabIndex = 63;
            ClearNameListButton.UseVisualStyleBackColor = true;
            ClearNameListButton.Click += ClearNameListButton_Click;
            // 
            // ClipboardButton
            // 
            ClipboardButton.IconChar = FontAwesome.Sharp.IconChar.ClipboardList;
            ClipboardButton.IconColor = Color.Black;
            ClipboardButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            ClipboardButton.IconSize = 24;
            ClipboardButton.Location = new Point(370, 362);
            ClipboardButton.Name = "ClipboardButton";
            ClipboardButton.Size = new Size(54, 50);
            ClipboardButton.TabIndex = 62;
            ClipboardButton.UseVisualStyleBackColor = true;
            ClipboardButton.Click += CopyToClipboardButton_Click;
            // 
            // GenerateNamesButton
            // 
            GenerateNamesButton.DialogResult = DialogResult.OK;
            GenerateNamesButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GenerateNamesButton.ForeColor = SystemColors.ControlDarkDark;
            GenerateNamesButton.Location = new Point(370, 104);
            GenerateNamesButton.Name = "GenerateNamesButton";
            GenerateNamesButton.Size = new Size(170, 50);
            GenerateNamesButton.TabIndex = 60;
            GenerateNamesButton.Text = "&Generate Names";
            GenerateNamesButton.UseVisualStyleBackColor = true;
            GenerateNamesButton.Click += GenerateNamesButton_Click;
            // 
            // GeneratedNamesList
            // 
            GeneratedNamesList.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GeneratedNamesList.FormattingEnabled = true;
            GeneratedNamesList.ItemHeight = 15;
            GeneratedNamesList.Location = new Point(370, 160);
            GeneratedNamesList.Name = "GeneratedNamesList";
            GeneratedNamesList.Size = new Size(170, 184);
            GeneratedNamesList.TabIndex = 59;
            GeneratedNamesList.SelectedIndexChanged += GeneratedNamesList_SelectedIndexChanged;
            // 
            // SelectAllLanguagesCheck
            // 
            SelectAllLanguagesCheck.AutoSize = true;
            SelectAllLanguagesCheck.Checked = true;
            SelectAllLanguagesCheck.CheckState = CheckState.Checked;
            SelectAllLanguagesCheck.ForeColor = SystemColors.ControlDarkDark;
            SelectAllLanguagesCheck.Location = new Point(196, 79);
            SelectAllLanguagesCheck.Name = "SelectAllLanguagesCheck";
            SelectAllLanguagesCheck.Size = new Size(74, 19);
            SelectAllLanguagesCheck.TabIndex = 58;
            SelectAllLanguagesCheck.Text = "Select All";
            SelectAllLanguagesCheck.UseVisualStyleBackColor = true;
            SelectAllLanguagesCheck.CheckedChanged += SelectAllLanguagesCheck_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(196, 56);
            label3.Name = "label3";
            label3.Size = new Size(82, 20);
            label3.TabIndex = 57;
            label3.Text = "Languages";
            // 
            // LanguagesListBox
            // 
            LanguagesListBox.CheckOnClick = true;
            LanguagesListBox.FormattingEnabled = true;
            LanguagesListBox.Location = new Point(196, 104);
            LanguagesListBox.Name = "LanguagesListBox";
            LanguagesListBox.Size = new Size(160, 310);
            LanguagesListBox.TabIndex = 56;
            // 
            // SelectAllNamebasesCheck
            // 
            SelectAllNamebasesCheck.AutoSize = true;
            SelectAllNamebasesCheck.Checked = true;
            SelectAllNamebasesCheck.CheckState = CheckState.Checked;
            SelectAllNamebasesCheck.ForeColor = SystemColors.ControlDarkDark;
            SelectAllNamebasesCheck.Location = new Point(23, 241);
            SelectAllNamebasesCheck.Name = "SelectAllNamebasesCheck";
            SelectAllNamebasesCheck.Size = new Size(74, 19);
            SelectAllNamebasesCheck.TabIndex = 55;
            SelectAllNamebasesCheck.Text = "Select All";
            SelectAllNamebasesCheck.UseVisualStyleBackColor = true;
            SelectAllNamebasesCheck.CheckedChanged += SelectAllNamebasesCheck_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(23, 218);
            label2.Name = "label2";
            label2.Size = new Size(87, 20);
            label2.TabIndex = 54;
            label2.Text = "Namebases";
            // 
            // NamebasesListBox
            // 
            NamebasesListBox.CheckOnClick = true;
            NamebasesListBox.FormattingEnabled = true;
            NamebasesListBox.Location = new Point(23, 266);
            NamebasesListBox.Name = "NamebasesListBox";
            NamebasesListBox.Size = new Size(160, 148);
            NamebasesListBox.TabIndex = 53;
            // 
            // SelectAllNameGeneratorsCheck
            // 
            SelectAllNameGeneratorsCheck.AutoSize = true;
            SelectAllNameGeneratorsCheck.Checked = true;
            SelectAllNameGeneratorsCheck.CheckState = CheckState.Checked;
            SelectAllNameGeneratorsCheck.ForeColor = SystemColors.ControlDarkDark;
            SelectAllNameGeneratorsCheck.Location = new Point(23, 79);
            SelectAllNameGeneratorsCheck.Name = "SelectAllNameGeneratorsCheck";
            SelectAllNameGeneratorsCheck.Size = new Size(74, 19);
            SelectAllNameGeneratorsCheck.TabIndex = 52;
            SelectAllNameGeneratorsCheck.Text = "Select All";
            SelectAllNameGeneratorsCheck.UseVisualStyleBackColor = true;
            SelectAllNameGeneratorsCheck.CheckedChanged += SelectAllNameGeneratorsCheck_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(23, 56);
            label1.Name = "label1";
            label1.Size = new Size(129, 20);
            label1.TabIndex = 51;
            label1.Text = "Name Generators";
            // 
            // NameGeneratorsListBox
            // 
            NameGeneratorsListBox.CheckOnClick = true;
            NameGeneratorsListBox.Location = new Point(23, 104);
            NameGeneratorsListBox.Name = "NameGeneratorsListBox";
            NameGeneratorsListBox.Size = new Size(160, 94);
            NameGeneratorsListBox.TabIndex = 50;
            // 
            // CloseNameConfigurationButton
            // 
            CloseNameConfigurationButton.DialogResult = DialogResult.OK;
            CloseNameConfigurationButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CloseNameConfigurationButton.ForeColor = SystemColors.ControlDarkDark;
            CloseNameConfigurationButton.Location = new Point(490, 365);
            CloseNameConfigurationButton.Name = "CloseNameConfigurationButton";
            CloseNameConfigurationButton.Size = new Size(54, 50);
            CloseNameConfigurationButton.TabIndex = 49;
            CloseNameConfigurationButton.Text = "&Close";
            CloseNameConfigurationButton.UseVisualStyleBackColor = true;
            CloseNameConfigurationButton.Click += CloseNameConfigurationButton_Click;
            // 
            // ApplySelectedNameButton
            // 
            ApplySelectedNameButton.DialogResult = DialogResult.OK;
            ApplySelectedNameButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ApplySelectedNameButton.ForeColor = SystemColors.ControlDarkDark;
            ApplySelectedNameButton.Location = new Point(430, 364);
            ApplySelectedNameButton.Name = "ApplySelectedNameButton";
            ApplySelectedNameButton.Size = new Size(54, 50);
            ApplySelectedNameButton.TabIndex = 48;
            ApplySelectedNameButton.Text = "&Apply";
            ApplySelectedNameButton.UseVisualStyleBackColor = true;
            ApplySelectedNameButton.Click += ApplySelectedNameButton_Click;
            // 
            // materialCheckBox1
            // 
            materialCheckBox1.AutoSize = true;
            materialCheckBox1.Depth = 0;
            materialCheckBox1.Location = new Point(0, 0);
            materialCheckBox1.Margin = new Padding(0);
            materialCheckBox1.MouseLocation = new Point(-1, -1);
            materialCheckBox1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialCheckBox1.Name = "materialCheckBox1";
            materialCheckBox1.ReadOnly = false;
            materialCheckBox1.Ripple = true;
            materialCheckBox1.Size = new Size(10, 10);
            materialCheckBox1.TabIndex = 0;
            materialCheckBox1.Text = "materialCheckBox1";
            materialCheckBox1.UseAccentColor = false;
            materialCheckBox1.UseVisualStyleBackColor = true;
            // 
            // materialCheckBox2
            // 
            materialCheckBox2.AutoSize = true;
            materialCheckBox2.Depth = 0;
            materialCheckBox2.Location = new Point(0, 0);
            materialCheckBox2.Margin = new Padding(0);
            materialCheckBox2.MouseLocation = new Point(-1, -1);
            materialCheckBox2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialCheckBox2.Name = "materialCheckBox2";
            materialCheckBox2.ReadOnly = false;
            materialCheckBox2.Ripple = true;
            materialCheckBox2.Size = new Size(10, 10);
            materialCheckBox2.TabIndex = 0;
            materialCheckBox2.Text = "materialCheckBox2";
            materialCheckBox2.UseAccentColor = false;
            materialCheckBox2.UseVisualStyleBackColor = true;
            // 
            // materialCheckBox3
            // 
            materialCheckBox3.AutoSize = true;
            materialCheckBox3.Depth = 0;
            materialCheckBox3.Location = new Point(0, 0);
            materialCheckBox3.Margin = new Padding(0);
            materialCheckBox3.MouseLocation = new Point(-1, -1);
            materialCheckBox3.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialCheckBox3.Name = "materialCheckBox3";
            materialCheckBox3.ReadOnly = false;
            materialCheckBox3.Ripple = true;
            materialCheckBox3.Size = new Size(10, 10);
            materialCheckBox3.TabIndex = 0;
            materialCheckBox3.Text = "materialCheckBox3";
            materialCheckBox3.UseAccentColor = false;
            materialCheckBox3.UseVisualStyleBackColor = true;
            // 
            // materialCheckBox4
            // 
            materialCheckBox4.AutoSize = true;
            materialCheckBox4.Depth = 0;
            materialCheckBox4.Location = new Point(0, 0);
            materialCheckBox4.Margin = new Padding(0);
            materialCheckBox4.MouseLocation = new Point(-1, -1);
            materialCheckBox4.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialCheckBox4.Name = "materialCheckBox4";
            materialCheckBox4.ReadOnly = false;
            materialCheckBox4.Ripple = true;
            materialCheckBox4.Size = new Size(10, 10);
            materialCheckBox4.TabIndex = 0;
            materialCheckBox4.Text = "materialCheckBox4";
            materialCheckBox4.UseAccentColor = false;
            materialCheckBox4.UseVisualStyleBackColor = true;
            // 
            // NameGeneratorConfiguration
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(563, 431);
            Controls.Add(NameConfigOverlayForm);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "NameGeneratorConfiguration";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Configure Name Generation";
            TopMost = true;
            TransparencyKey = Color.Fuchsia;
            NameConfigOverlayForm.ResumeLayout(false);
            NameConfigOverlayForm.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm NameConfigOverlayForm;
        private Button CloseNameConfigurationButton;
        private Button ApplySelectedNameButton;
        private Label label1;
        private ReaLTaiizor.Controls.MaterialCheckBox materialCheckBox1;
        private ReaLTaiizor.Controls.MaterialCheckBox materialCheckBox2;
        private ReaLTaiizor.Controls.MaterialCheckBox materialCheckBox3;
        private ReaLTaiizor.Controls.MaterialCheckBox materialCheckBox4;
        private CheckBox SelectAllLanguagesCheck;
        private Label label3;
        private CheckBox SelectAllNamebasesCheck;
        private Label label2;
        private CheckBox SelectAllNameGeneratorsCheck;
        private Button GenerateNamesButton;
        private ListBox GeneratedNamesList;
        public CheckedListBox NameGeneratorsListBox;
        public CheckedListBox LanguagesListBox;
        public CheckedListBox NamebasesListBox;
        private FontAwesome.Sharp.IconButton ClipboardButton;
        private FontAwesome.Sharp.IconButton ClearNameListButton;
    }
}