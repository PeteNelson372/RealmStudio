namespace RealmStudio
{
    partial class WorldAnvilIntegration
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
            WorldAnvilIntegrationOverlay = new ReaLTaiizor.Forms.DungeonForm();
            UserWorldsList = new ListView();
            WorldTitle = new ColumnHeader();
            WorldId = new ColumnHeader();
            RememberUserAPITokenCheck = new CheckBox();
            UserIdLabel = new Label();
            label4 = new Label();
            UserNameLabel = new Label();
            label2 = new Label();
            APITokenValidButton = new FontAwesome.Sharp.IconButton();
            ValidateTokenButton = new Button();
            label1 = new Label();
            APITokenTextBox = new TextBox();
            CloseButton = new Button();
            WorldAnvilIntegrationOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // WorldAnvilIntegrationOverlay
            // 
            WorldAnvilIntegrationOverlay.BackColor = Color.FromArgb(244, 241, 243);
            WorldAnvilIntegrationOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            WorldAnvilIntegrationOverlay.Controls.Add(UserWorldsList);
            WorldAnvilIntegrationOverlay.Controls.Add(RememberUserAPITokenCheck);
            WorldAnvilIntegrationOverlay.Controls.Add(UserIdLabel);
            WorldAnvilIntegrationOverlay.Controls.Add(label4);
            WorldAnvilIntegrationOverlay.Controls.Add(UserNameLabel);
            WorldAnvilIntegrationOverlay.Controls.Add(label2);
            WorldAnvilIntegrationOverlay.Controls.Add(APITokenValidButton);
            WorldAnvilIntegrationOverlay.Controls.Add(ValidateTokenButton);
            WorldAnvilIntegrationOverlay.Controls.Add(label1);
            WorldAnvilIntegrationOverlay.Controls.Add(APITokenTextBox);
            WorldAnvilIntegrationOverlay.Controls.Add(CloseButton);
            WorldAnvilIntegrationOverlay.Dock = DockStyle.Fill;
            WorldAnvilIntegrationOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            WorldAnvilIntegrationOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            WorldAnvilIntegrationOverlay.Font = new Font("Segoe UI", 9F);
            WorldAnvilIntegrationOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            WorldAnvilIntegrationOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            WorldAnvilIntegrationOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            WorldAnvilIntegrationOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            WorldAnvilIntegrationOverlay.Location = new Point(0, 0);
            WorldAnvilIntegrationOverlay.Name = "WorldAnvilIntegrationOverlay";
            WorldAnvilIntegrationOverlay.Padding = new Padding(20, 56, 20, 16);
            WorldAnvilIntegrationOverlay.RoundCorners = true;
            WorldAnvilIntegrationOverlay.Sizable = true;
            WorldAnvilIntegrationOverlay.Size = new Size(800, 450);
            WorldAnvilIntegrationOverlay.SmartBounds = true;
            WorldAnvilIntegrationOverlay.StartPosition = FormStartPosition.WindowsDefaultLocation;
            WorldAnvilIntegrationOverlay.TabIndex = 0;
            WorldAnvilIntegrationOverlay.Text = "World Anvil Integration Parameters";
            WorldAnvilIntegrationOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // UserWorldsList
            // 
            UserWorldsList.Columns.AddRange(new ColumnHeader[] { WorldTitle, WorldId });
            UserWorldsList.FullRowSelect = true;
            UserWorldsList.GridLines = true;
            UserWorldsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            UserWorldsList.Location = new Point(137, 179);
            UserWorldsList.MultiSelect = false;
            UserWorldsList.Name = "UserWorldsList";
            UserWorldsList.Size = new Size(588, 158);
            UserWorldsList.Sorting = SortOrder.Ascending;
            UserWorldsList.TabIndex = 10;
            UserWorldsList.UseCompatibleStateImageBehavior = false;
            UserWorldsList.View = View.Details;
            // 
            // WorldTitle
            // 
            WorldTitle.Text = "World Title";
            WorldTitle.Width = 300;
            // 
            // WorldId
            // 
            WorldId.Text = "World Identifier";
            WorldId.Width = 280;
            // 
            // RememberUserAPITokenCheck
            // 
            RememberUserAPITokenCheck.AutoSize = true;
            RememberUserAPITokenCheck.ForeColor = SystemColors.ControlDarkDark;
            RememberUserAPITokenCheck.Location = new Point(137, 95);
            RememberUserAPITokenCheck.Name = "RememberUserAPITokenCheck";
            RememberUserAPITokenCheck.Size = new Size(188, 19);
            RememberUserAPITokenCheck.TabIndex = 9;
            RememberUserAPITokenCheck.Text = "Remember WA User API Token";
            RememberUserAPITokenCheck.UseVisualStyleBackColor = true;
            // 
            // UserIdLabel
            // 
            UserIdLabel.BorderStyle = BorderStyle.FixedSingle;
            UserIdLabel.ForeColor = SystemColors.ControlDarkDark;
            UserIdLabel.Location = new Point(458, 126);
            UserIdLabel.Name = "UserIdLabel";
            UserIdLabel.Size = new Size(267, 23);
            UserIdLabel.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(408, 127);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 7;
            label4.Text = "User ID";
            // 
            // UserNameLabel
            // 
            UserNameLabel.BorderStyle = BorderStyle.FixedSingle;
            UserNameLabel.ForeColor = SystemColors.ControlDarkDark;
            UserNameLabel.Location = new Point(137, 126);
            UserNameLabel.Name = "UserNameLabel";
            UserNameLabel.Size = new Size(233, 23);
            UserNameLabel.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(66, 127);
            label2.Name = "label2";
            label2.Size = new Size(65, 15);
            label2.TabIndex = 5;
            label2.Text = "User Name";
            // 
            // APITokenValidButton
            // 
            APITokenValidButton.FlatStyle = FlatStyle.Flat;
            APITokenValidButton.IconChar = FontAwesome.Sharp.IconChar.Cancel;
            APITokenValidButton.IconColor = Color.Red;
            APITokenValidButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            APITokenValidButton.IconSize = 20;
            APITokenValidButton.Location = new Point(731, 65);
            APITokenValidButton.Name = "APITokenValidButton";
            APITokenValidButton.Size = new Size(25, 25);
            APITokenValidButton.TabIndex = 4;
            APITokenValidButton.UseVisualStyleBackColor = true;
            // 
            // ValidateTokenButton
            // 
            ValidateTokenButton.ForeColor = SystemColors.ControlDarkDark;
            ValidateTokenButton.Location = new Point(650, 66);
            ValidateTokenButton.Name = "ValidateTokenButton";
            ValidateTokenButton.Size = new Size(75, 23);
            ValidateTokenButton.TabIndex = 3;
            ValidateTokenButton.Text = "Validate";
            ValidateTokenButton.UseVisualStyleBackColor = true;
            ValidateTokenButton.Click += ValidateTokenButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(23, 66);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 2;
            label1.Text = "WA User API Token";
            // 
            // APITokenTextBox
            // 
            APITokenTextBox.Location = new Point(137, 66);
            APITokenTextBox.Name = "APITokenTextBox";
            APITokenTextBox.Size = new Size(507, 23);
            APITokenTextBox.TabIndex = 1;
            // 
            // CloseButton
            // 
            CloseButton.ForeColor = SystemColors.ControlDarkDark;
            CloseButton.Location = new Point(717, 371);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(60, 60);
            CloseButton.TabIndex = 0;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // WorldAnvilIntegration
            // 
            AcceptButton = CloseButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(WorldAnvilIntegrationOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "WorldAnvilIntegration";
            Text = "World Anvil Integration Parameters";
            TransparencyKey = Color.Fuchsia;
            WorldAnvilIntegrationOverlay.ResumeLayout(false);
            WorldAnvilIntegrationOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm WorldAnvilIntegrationOverlay;
        private Button CloseButton;
        private Button ValidateTokenButton;
        private Label label1;
        private TextBox APITokenTextBox;
        private FontAwesome.Sharp.IconButton APITokenValidButton;
        private Label UserNameLabel;
        private Label label2;
        private CheckBox RememberUserAPITokenCheck;
        private Label UserIdLabel;
        private Label label4;
        private ListView UserWorldsList;
        private ColumnHeader WorldTitle;
        private ColumnHeader WorldId;
    }
}