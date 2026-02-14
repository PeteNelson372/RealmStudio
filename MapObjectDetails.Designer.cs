namespace RealmStudioX
{
    partial class MapObjectDetails
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
            MapObjectDetailsOverlay = new ReaLTaiizor.Forms.DungeonForm();
            ObjectCharacteristicsCheckedListBox = new CheckedListBox();
            CloseCharacteristicsButton = new Button();
            AddCharacteristicButton = new FontAwesome.Sharp.IconButton();
            ObjectCharacteristicsTextbox = new TextBox();
            ApplyChangesButton = new Button();
            ObjectCharacteristicsLabel = new Label();
            ObjectTypeLabel = new Label();
            ObjectTypeCheckedList = new CheckedListBox();
            MapObjectDetailsOverlay.SuspendLayout();
            SuspendLayout();
            // 
            // MapObjectDetailsOverlay
            // 
            MapObjectDetailsOverlay.BackColor = Color.FromArgb(244, 241, 243);
            MapObjectDetailsOverlay.BorderColor = Color.FromArgb(38, 38, 38);
            MapObjectDetailsOverlay.Controls.Add(ObjectCharacteristicsCheckedListBox);
            MapObjectDetailsOverlay.Controls.Add(CloseCharacteristicsButton);
            MapObjectDetailsOverlay.Controls.Add(AddCharacteristicButton);
            MapObjectDetailsOverlay.Controls.Add(ObjectCharacteristicsTextbox);
            MapObjectDetailsOverlay.Controls.Add(ApplyChangesButton);
            MapObjectDetailsOverlay.Controls.Add(ObjectCharacteristicsLabel);
            MapObjectDetailsOverlay.Controls.Add(ObjectTypeLabel);
            MapObjectDetailsOverlay.Controls.Add(ObjectTypeCheckedList);
            MapObjectDetailsOverlay.Dock = DockStyle.Fill;
            MapObjectDetailsOverlay.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            MapObjectDetailsOverlay.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            MapObjectDetailsOverlay.Font = new Font("Segoe UI", 9F);
            MapObjectDetailsOverlay.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            MapObjectDetailsOverlay.ForeColor = Color.FromArgb(223, 219, 210);
            MapObjectDetailsOverlay.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            MapObjectDetailsOverlay.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            MapObjectDetailsOverlay.Location = new Point(0, 0);
            MapObjectDetailsOverlay.Name = "MapObjectDetailsOverlay";
            MapObjectDetailsOverlay.Padding = new Padding(20, 56, 20, 16);
            MapObjectDetailsOverlay.RoundCorners = true;
            MapObjectDetailsOverlay.Sizable = true;
            MapObjectDetailsOverlay.Size = new Size(388, 364);
            MapObjectDetailsOverlay.SmartBounds = true;
            MapObjectDetailsOverlay.StartPosition = FormStartPosition.CenterParent;
            MapObjectDetailsOverlay.TabIndex = 0;
            MapObjectDetailsOverlay.Text = "Object Characteristics";
            MapObjectDetailsOverlay.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // ObjectCharacteristicsCheckedListBox
            // 
            ObjectCharacteristicsCheckedListBox.CheckOnClick = true;
            ObjectCharacteristicsCheckedListBox.FormattingEnabled = true;
            ObjectCharacteristicsCheckedListBox.Location = new Point(199, 82);
            ObjectCharacteristicsCheckedListBox.Name = "ObjectCharacteristicsCheckedListBox";
            ObjectCharacteristicsCheckedListBox.Size = new Size(158, 148);
            ObjectCharacteristicsCheckedListBox.TabIndex = 115;
            ObjectCharacteristicsCheckedListBox.KeyDown += ObjectCharacteristicsCheckedListBox_KeyDown;
            // 
            // CloseCharacteristicsButton
            // 
            CloseCharacteristicsButton.DialogResult = DialogResult.Cancel;
            CloseCharacteristicsButton.ForeColor = SystemColors.ControlDarkDark;
            CloseCharacteristicsButton.Location = new Point(297, 285);
            CloseCharacteristicsButton.Name = "CloseCharacteristicsButton";
            CloseCharacteristicsButton.Size = new Size(60, 60);
            CloseCharacteristicsButton.TabIndex = 114;
            CloseCharacteristicsButton.Text = "&Close";
            CloseCharacteristicsButton.UseVisualStyleBackColor = true;
            CloseCharacteristicsButton.Click += CloseCharacteristicsButton_Click;
            // 
            // AddCharacteristicButton
            // 
            AddCharacteristicButton.BackColor = SystemColors.ControlLightLight;
            AddCharacteristicButton.FlatAppearance.BorderColor = SystemColors.ControlDarkDark;
            AddCharacteristicButton.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AddCharacteristicButton.ForeColor = SystemColors.ControlDarkDark;
            AddCharacteristicButton.IconChar = FontAwesome.Sharp.IconChar.Add;
            AddCharacteristicButton.IconColor = Color.Black;
            AddCharacteristicButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            AddCharacteristicButton.IconSize = 20;
            AddCharacteristicButton.Location = new Point(310, 236);
            AddCharacteristicButton.Name = "AddCharacteristicButton";
            AddCharacteristicButton.Size = new Size(47, 23);
            AddCharacteristicButton.TabIndex = 112;
            AddCharacteristicButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            AddCharacteristicButton.UseVisualStyleBackColor = false;
            AddCharacteristicButton.Click += AddCharacteristicButton_Click;
            AddCharacteristicButton.MouseHover += AddCharacteristicButton_MouseHover;
            // 
            // ObjectCharacteristicsTextbox
            // 
            ObjectCharacteristicsTextbox.Location = new Point(199, 236);
            ObjectCharacteristicsTextbox.Name = "ObjectCharacteristicsTextbox";
            ObjectCharacteristicsTextbox.Size = new Size(105, 23);
            ObjectCharacteristicsTextbox.TabIndex = 93;
            ObjectCharacteristicsTextbox.KeyDown += ObjectCharacteristicsTextbox_KeyDown;
            // 
            // ApplyChangesButton
            // 
            ApplyChangesButton.DialogResult = DialogResult.OK;
            ApplyChangesButton.ForeColor = SystemColors.ControlDarkDark;
            ApplyChangesButton.Location = new Point(231, 285);
            ApplyChangesButton.Name = "ApplyChangesButton";
            ApplyChangesButton.Size = new Size(60, 60);
            ApplyChangesButton.TabIndex = 92;
            ApplyChangesButton.Text = "&Apply";
            ApplyChangesButton.UseVisualStyleBackColor = true;
            ApplyChangesButton.Click += ApplyChangesButton_Click;
            // 
            // ObjectCharacteristicsLabel
            // 
            ObjectCharacteristicsLabel.ForeColor = SystemColors.ControlDarkDark;
            ObjectCharacteristicsLabel.Location = new Point(199, 56);
            ObjectCharacteristicsLabel.Name = "ObjectCharacteristicsLabel";
            ObjectCharacteristicsLabel.Size = new Size(158, 23);
            ObjectCharacteristicsLabel.TabIndex = 3;
            ObjectCharacteristicsLabel.Text = "Object Characteristics";
            ObjectCharacteristicsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ObjectTypeLabel
            // 
            ObjectTypeLabel.ForeColor = SystemColors.ControlDarkDark;
            ObjectTypeLabel.Location = new Point(23, 56);
            ObjectTypeLabel.Name = "ObjectTypeLabel";
            ObjectTypeLabel.Size = new Size(158, 23);
            ObjectTypeLabel.TabIndex = 1;
            ObjectTypeLabel.Text = "Object Type";
            ObjectTypeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ObjectTypeCheckedList
            // 
            ObjectTypeCheckedList.CheckOnClick = true;
            ObjectTypeCheckedList.FormattingEnabled = true;
            ObjectTypeCheckedList.Location = new Point(23, 82);
            ObjectTypeCheckedList.Name = "ObjectTypeCheckedList";
            ObjectTypeCheckedList.Size = new Size(158, 148);
            ObjectTypeCheckedList.TabIndex = 0;
            ObjectTypeCheckedList.SelectedIndexChanged += ObjectTypeCheckedList_SelectedIndexChanged;
            // 
            // MapObjectDetails
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(388, 364);
            Controls.Add(MapObjectDetailsOverlay);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(261, 65);
            Name = "MapObjectDetails";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Object Characteristics";
            TransparencyKey = Color.Fuchsia;
            MapObjectDetailsOverlay.ResumeLayout(false);
            MapObjectDetailsOverlay.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Forms.DungeonForm MapObjectDetailsOverlay;
        private CheckedListBox ObjectTypeCheckedList;
        private Label ObjectTypeLabel;
        private Label ObjectCharacteristicsLabel;
        private TextBox ObjectCharacteristicsTextbox;
        private Button ApplyChangesButton;
        private FontAwesome.Sharp.IconButton AddCharacteristicButton;
        private Button CloseCharacteristicsButton;
        private CheckedListBox ObjectCharacteristicsCheckedListBox;
    }
}