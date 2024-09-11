namespace RealmStudio
{
    partial class RealmStudioMainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TabPage OceanTab;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RealmStudioMainForm));
            toolStrip1 = new ToolStrip();
            MainTab = new TabControl();
            BackgroundTab = new TabPage();
            cyberSwitch1 = new ReaLTaiizor.Controls.CyberSwitch();
            label2 = new Label();
            LandTab = new TabPage();
            WaterTab = new TabPage();
            PathTab = new TabPage();
            SymbolTab = new TabPage();
            LabelTab = new TabPage();
            OverlayTab = new TabPage();
            RegionTab = new TabPage();
            DrawingTab = new TabPage();
            crownContextMenuStrip1 = new ReaLTaiizor.Controls.CrownContextMenuStrip();
            RealmStudioForm = new ReaLTaiizor.Forms.DungeonForm();
            MapRenderHScroll = new ReaLTaiizor.Controls.MaterialScrollBar();
            MapRenderVScroll = new ReaLTaiizor.Controls.MaterialScrollBar();
            SaveButton = new FontAwesome.Sharp.IconButton();
            AutosaveSwitch = new ReaLTaiizor.Controls.CyberSwitch();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            statusStrip3 = new StatusStrip();
            poisonPanel1 = new ReaLTaiizor.Controls.PoisonPanel();
            SKGLRenderControl = new SkiaSharp.Views.Desktop.SKGLControl();
            statusStrip2 = new StatusStrip();
            statusStrip1 = new StatusStrip();
            thunderControlBox1 = new ReaLTaiizor.Controls.ThunderControlBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            printToolStripMenuItem = new ToolStripMenuItem();
            printPreviewToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            cutToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            contentsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            themeToolStripMenuItem = new ToolStripMenuItem();
            realmToolStripMenuItem = new ToolStripMenuItem();
            OceanTab = new TabPage();
            OceanTab.SuspendLayout();
            MainTab.SuspendLayout();
            BackgroundTab.SuspendLayout();
            RealmStudioForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // OceanTab
            // 
            OceanTab.BackColor = Color.Transparent;
            OceanTab.Controls.Add(toolStrip1);
            OceanTab.ForeColor = SystemColors.ControlDark;
            OceanTab.Location = new Point(26, 4);
            OceanTab.Name = "OceanTab";
            OceanTab.Padding = new Padding(3);
            OceanTab.Size = new Size(230, 878);
            OceanTab.TabIndex = 1;
            OceanTab.Text = "Ocean";
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.Dock = DockStyle.Right;
            toolStrip1.Location = new Point(165, 3);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Size = new Size(62, 872);
            toolStrip1.Stretch = true;
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // MainTab
            // 
            MainTab.Alignment = TabAlignment.Left;
            MainTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            MainTab.Controls.Add(BackgroundTab);
            MainTab.Controls.Add(OceanTab);
            MainTab.Controls.Add(LandTab);
            MainTab.Controls.Add(WaterTab);
            MainTab.Controls.Add(PathTab);
            MainTab.Controls.Add(SymbolTab);
            MainTab.Controls.Add(LabelTab);
            MainTab.Controls.Add(OverlayTab);
            MainTab.Controls.Add(RegionTab);
            MainTab.Controls.Add(DrawingTab);
            MainTab.ItemSize = new Size(84, 22);
            MainTab.Location = new Point(12, 90);
            MainTab.Margin = new Padding(0);
            MainTab.Multiline = true;
            MainTab.Name = "MainTab";
            MainTab.SelectedIndex = 0;
            MainTab.Size = new Size(260, 886);
            MainTab.SizeMode = TabSizeMode.Fixed;
            MainTab.TabIndex = 4;
            // 
            // BackgroundTab
            // 
            BackgroundTab.BackColor = Color.Transparent;
            BackgroundTab.Controls.Add(cyberSwitch1);
            BackgroundTab.Controls.Add(label2);
            BackgroundTab.ForeColor = SystemColors.ControlDarkDark;
            BackgroundTab.Location = new Point(26, 4);
            BackgroundTab.Name = "BackgroundTab";
            BackgroundTab.Padding = new Padding(3);
            BackgroundTab.Size = new Size(230, 878);
            BackgroundTab.TabIndex = 0;
            BackgroundTab.Text = "Background";
            // 
            // cyberSwitch1
            // 
            cyberSwitch1.Alpha = 50;
            cyberSwitch1.BackColor = Color.Transparent;
            cyberSwitch1.Background = true;
            cyberSwitch1.Background_WidthPen = 2F;
            cyberSwitch1.BackgroundPen = false;
            cyberSwitch1.Checked = true;
            cyberSwitch1.ColorBackground = Color.FromArgb(223, 219, 210);
            cyberSwitch1.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            cyberSwitch1.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            cyberSwitch1.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            cyberSwitch1.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            cyberSwitch1.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            cyberSwitch1.ColorLighting = Color.FromArgb(223, 219, 210);
            cyberSwitch1.ColorPen_1 = Color.FromArgb(37, 52, 68);
            cyberSwitch1.ColorPen_2 = Color.FromArgb(41, 63, 86);
            cyberSwitch1.ColorValue = Color.ForestGreen;
            cyberSwitch1.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            cyberSwitch1.Font = new Font("Arial", 11F);
            cyberSwitch1.ForeColor = Color.FromArgb(245, 245, 245);
            cyberSwitch1.Lighting = true;
            cyberSwitch1.LinearGradient_Background = false;
            cyberSwitch1.LinearGradient_Value = false;
            cyberSwitch1.LinearGradientPen = false;
            cyberSwitch1.Location = new Point(6, 6);
            cyberSwitch1.Name = "cyberSwitch1";
            cyberSwitch1.PenWidth = 10;
            cyberSwitch1.RGB = false;
            cyberSwitch1.Rounding = true;
            cyberSwitch1.RoundingInt = 90;
            cyberSwitch1.Size = new Size(35, 20);
            cyberSwitch1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            cyberSwitch1.TabIndex = 12;
            cyberSwitch1.Tag = "Cyber";
            cyberSwitch1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            cyberSwitch1.Timer_RGB = 300;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.ForeColor = Color.Black;
            label2.Location = new Point(45, 9);
            label2.Name = "label2";
            label2.Size = new Size(36, 15);
            label2.TabIndex = 11;
            label2.Text = "Show";
            // 
            // LandTab
            // 
            LandTab.Location = new Point(26, 4);
            LandTab.Name = "LandTab";
            LandTab.Size = new Size(230, 878);
            LandTab.TabIndex = 2;
            LandTab.Text = "Land";
            LandTab.UseVisualStyleBackColor = true;
            // 
            // WaterTab
            // 
            WaterTab.Location = new Point(26, 4);
            WaterTab.Name = "WaterTab";
            WaterTab.Size = new Size(230, 878);
            WaterTab.TabIndex = 3;
            WaterTab.Text = "Water";
            WaterTab.UseVisualStyleBackColor = true;
            // 
            // PathTab
            // 
            PathTab.Location = new Point(26, 4);
            PathTab.Name = "PathTab";
            PathTab.Size = new Size(230, 878);
            PathTab.TabIndex = 4;
            PathTab.Text = "Paths";
            PathTab.UseVisualStyleBackColor = true;
            // 
            // SymbolTab
            // 
            SymbolTab.Location = new Point(26, 4);
            SymbolTab.Name = "SymbolTab";
            SymbolTab.Size = new Size(230, 878);
            SymbolTab.TabIndex = 9;
            SymbolTab.Text = "Symbols";
            SymbolTab.UseVisualStyleBackColor = true;
            // 
            // LabelTab
            // 
            LabelTab.Location = new Point(26, 4);
            LabelTab.Name = "LabelTab";
            LabelTab.Size = new Size(230, 878);
            LabelTab.TabIndex = 5;
            LabelTab.Text = "Labels";
            LabelTab.UseVisualStyleBackColor = true;
            // 
            // OverlayTab
            // 
            OverlayTab.Location = new Point(26, 4);
            OverlayTab.Name = "OverlayTab";
            OverlayTab.Size = new Size(230, 878);
            OverlayTab.TabIndex = 6;
            OverlayTab.Text = "Overlays";
            OverlayTab.UseVisualStyleBackColor = true;
            // 
            // RegionTab
            // 
            RegionTab.Location = new Point(26, 4);
            RegionTab.Name = "RegionTab";
            RegionTab.Size = new Size(230, 878);
            RegionTab.TabIndex = 7;
            RegionTab.Text = "Regions";
            RegionTab.UseVisualStyleBackColor = true;
            // 
            // DrawingTab
            // 
            DrawingTab.Location = new Point(26, 4);
            DrawingTab.Name = "DrawingTab";
            DrawingTab.Size = new Size(230, 878);
            DrawingTab.TabIndex = 8;
            DrawingTab.Text = "Drawing";
            DrawingTab.UseVisualStyleBackColor = true;
            // 
            // crownContextMenuStrip1
            // 
            crownContextMenuStrip1.BackColor = Color.FromArgb(60, 63, 65);
            crownContextMenuStrip1.ForeColor = Color.FromArgb(220, 220, 220);
            crownContextMenuStrip1.Name = "crownContextMenuStrip1";
            crownContextMenuStrip1.Size = new Size(61, 4);
            // 
            // RealmStudioForm
            // 
            RealmStudioForm.BackColor = Color.FromArgb(223, 219, 210);
            RealmStudioForm.BorderColor = Color.FromArgb(38, 38, 38);
            RealmStudioForm.CausesValidation = false;
            RealmStudioForm.Controls.Add(MapRenderHScroll);
            RealmStudioForm.Controls.Add(MapRenderVScroll);
            RealmStudioForm.Controls.Add(SaveButton);
            RealmStudioForm.Controls.Add(AutosaveSwitch);
            RealmStudioForm.Controls.Add(label1);
            RealmStudioForm.Controls.Add(pictureBox1);
            RealmStudioForm.Controls.Add(statusStrip3);
            RealmStudioForm.Controls.Add(poisonPanel1);
            RealmStudioForm.Controls.Add(SKGLRenderControl);
            RealmStudioForm.Controls.Add(MainTab);
            RealmStudioForm.Controls.Add(statusStrip2);
            RealmStudioForm.Controls.Add(statusStrip1);
            RealmStudioForm.Controls.Add(thunderControlBox1);
            RealmStudioForm.Controls.Add(menuStrip1);
            RealmStudioForm.Dock = DockStyle.Fill;
            RealmStudioForm.FillEdgeColorA = Color.FromArgb(69, 68, 63);
            RealmStudioForm.FillEdgeColorB = Color.FromArgb(69, 68, 63);
            RealmStudioForm.Font = new Font("Segoe UI", 9F);
            RealmStudioForm.FooterEdgeColor = Color.FromArgb(69, 68, 63);
            RealmStudioForm.ForeColor = Color.FromArgb(223, 219, 210);
            RealmStudioForm.HeaderEdgeColorA = Color.FromArgb(87, 85, 77);
            RealmStudioForm.HeaderEdgeColorB = Color.FromArgb(69, 68, 63);
            RealmStudioForm.Location = new Point(0, 0);
            RealmStudioForm.Name = "RealmStudioForm";
            RealmStudioForm.Padding = new Padding(20, 56, 20, 16);
            RealmStudioForm.RoundCorners = true;
            RealmStudioForm.Sizable = false;
            RealmStudioForm.Size = new Size(1583, 1024);
            RealmStudioForm.SmartBounds = true;
            RealmStudioForm.StartPosition = FormStartPosition.CenterScreen;
            RealmStudioForm.TabIndex = 3;
            RealmStudioForm.TabStop = false;
            RealmStudioForm.Text = "Realm Studio";
            RealmStudioForm.TitleColor = Color.FromArgb(223, 219, 210);
            // 
            // MapRenderHScroll
            // 
            MapRenderHScroll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MapRenderHScroll.Depth = 0;
            MapRenderHScroll.Location = new Point(271, 964);
            MapRenderHScroll.Margin = new Padding(0);
            MapRenderHScroll.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            MapRenderHScroll.Name = "MapRenderHScroll";
            MapRenderHScroll.Orientation = ReaLTaiizor.Enum.Material.MateScrollOrientation.Horizontal;
            MapRenderHScroll.ScrollbarSize = 12;
            MapRenderHScroll.Size = new Size(1087, 12);
            MapRenderHScroll.TabIndex = 13;
            MapRenderHScroll.Text = "materialScrollBar1";
            MapRenderHScroll.Scroll += MapRenderHScroll_Scroll;
            // 
            // MapRenderVScroll
            // 
            MapRenderVScroll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            MapRenderVScroll.Depth = 0;
            MapRenderVScroll.HighlightOnWheel = true;
            MapRenderVScroll.Location = new Point(1359, 112);
            MapRenderVScroll.Margin = new Padding(0);
            MapRenderVScroll.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            MapRenderVScroll.Name = "MapRenderVScroll";
            MapRenderVScroll.Orientation = ReaLTaiizor.Enum.Material.MateScrollOrientation.Vertical;
            MapRenderVScroll.ScrollbarSize = 12;
            MapRenderVScroll.Size = new Size(12, 852);
            MapRenderVScroll.TabIndex = 12;
            MapRenderVScroll.Text = "materialScrollBar1";
            MapRenderVScroll.Scroll += MapRenderVScroll_Scroll;
            // 
            // SaveButton
            // 
            SaveButton.BackColor = Color.Transparent;
            SaveButton.FlatAppearance.BorderSize = 0;
            SaveButton.FlatStyle = FlatStyle.Flat;
            SaveButton.ForeColor = Color.Orchid;
            SaveButton.IconChar = FontAwesome.Sharp.IconChar.Save;
            SaveButton.IconColor = Color.LightSlateGray;
            SaveButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            SaveButton.IconSize = 32;
            SaveButton.Location = new Point(185, 4);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(40, 40);
            SaveButton.TabIndex = 11;
            SaveButton.UseVisualStyleBackColor = false;
            // 
            // AutosaveSwitch
            // 
            AutosaveSwitch.Alpha = 50;
            AutosaveSwitch.BackColor = Color.Transparent;
            AutosaveSwitch.Background = true;
            AutosaveSwitch.Background_WidthPen = 2F;
            AutosaveSwitch.BackgroundPen = false;
            AutosaveSwitch.Checked = true;
            AutosaveSwitch.ColorBackground = Color.FromArgb(223, 219, 210);
            AutosaveSwitch.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            AutosaveSwitch.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            AutosaveSwitch.ColorBackground_Pen = Color.FromArgb(223, 219, 210);
            AutosaveSwitch.ColorBackground_Value_1 = Color.FromArgb(223, 219, 210);
            AutosaveSwitch.ColorBackground_Value_2 = Color.FromArgb(223, 219, 210);
            AutosaveSwitch.ColorLighting = Color.FromArgb(223, 219, 210);
            AutosaveSwitch.ColorPen_1 = Color.FromArgb(37, 52, 68);
            AutosaveSwitch.ColorPen_2 = Color.FromArgb(41, 63, 86);
            AutosaveSwitch.ColorValue = Color.ForestGreen;
            AutosaveSwitch.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            AutosaveSwitch.Font = new Font("Arial", 11F);
            AutosaveSwitch.ForeColor = Color.FromArgb(245, 245, 245);
            AutosaveSwitch.Lighting = true;
            AutosaveSwitch.LinearGradient_Background = false;
            AutosaveSwitch.LinearGradient_Value = false;
            AutosaveSwitch.LinearGradientPen = false;
            AutosaveSwitch.Location = new Point(132, 15);
            AutosaveSwitch.Name = "AutosaveSwitch";
            AutosaveSwitch.PenWidth = 10;
            AutosaveSwitch.RGB = false;
            AutosaveSwitch.Rounding = true;
            AutosaveSwitch.RoundingInt = 90;
            AutosaveSwitch.Size = new Size(35, 20);
            AutosaveSwitch.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            AutosaveSwitch.TabIndex = 10;
            AutosaveSwitch.Tag = "Cyber";
            AutosaveSwitch.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            AutosaveSwitch.Timer_RGB = 300;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(69, 68, 63);
            label1.ForeColor = Color.White;
            label1.Location = new Point(70, 17);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 9;
            label1.Text = "Autosave";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImageLayout = ImageLayout.None;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(9, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 41);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // statusStrip3
            // 
            statusStrip3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            statusStrip3.AutoSize = false;
            statusStrip3.BackColor = Color.FromArgb(69, 68, 63);
            statusStrip3.Dock = DockStyle.None;
            statusStrip3.Location = new Point(271, 90);
            statusStrip3.Name = "statusStrip3";
            statusStrip3.Size = new Size(1100, 22);
            statusStrip3.SizingGrip = false;
            statusStrip3.TabIndex = 7;
            statusStrip3.Text = "statusStrip3";
            // 
            // poisonPanel1
            // 
            poisonPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            poisonPanel1.HorizontalScrollbarBarColor = true;
            poisonPanel1.HorizontalScrollbarHighlightOnWheel = false;
            poisonPanel1.HorizontalScrollbarSize = 10;
            poisonPanel1.Location = new Point(1371, 90);
            poisonPanel1.Name = "poisonPanel1";
            poisonPanel1.Size = new Size(200, 886);
            poisonPanel1.TabIndex = 6;
            poisonPanel1.VerticalScrollbarBarColor = true;
            poisonPanel1.VerticalScrollbarHighlightOnWheel = false;
            poisonPanel1.VerticalScrollbarSize = 10;
            // 
            // SKGLRenderControl
            // 
            SKGLRenderControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SKGLRenderControl.BackColor = Color.White;
            SKGLRenderControl.BorderStyle = BorderStyle.FixedSingle;
            SKGLRenderControl.Location = new Point(272, 112);
            SKGLRenderControl.Margin = new Padding(0);
            SKGLRenderControl.Name = "SKGLRenderControl";
            SKGLRenderControl.Size = new Size(1087, 852);
            SKGLRenderControl.TabIndex = 5;
            SKGLRenderControl.VSync = false;
            SKGLRenderControl.PaintSurface += SKGLRenderControl_PaintSurface;
            SKGLRenderControl.MouseDown += SKGLRenderControl_MouseDown;
            SKGLRenderControl.MouseMove += SKGLRenderControl_MouseMove;
            SKGLRenderControl.MouseUp += SKGLRenderControl_MouseUp;
            // 
            // statusStrip2
            // 
            statusStrip2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statusStrip2.AutoSize = false;
            statusStrip2.BackColor = Color.FromArgb(69, 68, 63);
            statusStrip2.Dock = DockStyle.None;
            statusStrip2.GripMargin = new Padding(0);
            statusStrip2.Location = new Point(9, 976);
            statusStrip2.Name = "statusStrip2";
            statusStrip2.RenderMode = ToolStripRenderMode.Professional;
            statusStrip2.Size = new Size(1565, 22);
            statusStrip2.SizingGrip = false;
            statusStrip2.TabIndex = 3;
            statusStrip2.Text = "statusStrip2";
            // 
            // statusStrip1
            // 
            statusStrip1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statusStrip1.AutoSize = false;
            statusStrip1.BackColor = Color.FromArgb(69, 68, 63);
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(9, 996);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            statusStrip1.Size = new Size(1565, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // thunderControlBox1
            // 
            thunderControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            thunderControlBox1.BackColor = Color.Transparent;
            thunderControlBox1.DefaultLocation = false;
            thunderControlBox1.ForeColor = SystemColors.ControlLight;
            thunderControlBox1.Location = new Point(1498, 12);
            thunderControlBox1.Name = "thunderControlBox1";
            thunderControlBox1.Size = new Size(75, 23);
            thunderControlBox1.TabIndex = 0;
            thunderControlBox1.Text = "thunderControlBox1";
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.Transparent;
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, realmToolStripMenuItem, themeToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(20, 56);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1543, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, printToolStripMenuItem, printPreviewToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
            newToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(180, 22);
            newToolStripMenuItem.Text = "&New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(180, 22);
            openToolStripMenuItem.Text = "&Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(177, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(180, 22);
            saveToolStripMenuItem.Text = "&Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(180, 22);
            saveAsToolStripMenuItem.Text = "Save &As";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // printToolStripMenuItem
            // 
            printToolStripMenuItem.Image = (Image)resources.GetObject("printToolStripMenuItem.Image");
            printToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            printToolStripMenuItem.Name = "printToolStripMenuItem";
            printToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            printToolStripMenuItem.Size = new Size(180, 22);
            printToolStripMenuItem.Text = "&Print";
            printToolStripMenuItem.Click += printToolStripMenuItem_Click;
            // 
            // printPreviewToolStripMenuItem
            // 
            printPreviewToolStripMenuItem.Image = (Image)resources.GetObject("printPreviewToolStripMenuItem.Image");
            printPreviewToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            printPreviewToolStripMenuItem.Size = new Size(180, 22);
            printPreviewToolStripMenuItem.Text = "Print Pre&view";
            printPreviewToolStripMenuItem.Click += printPreviewToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator3, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(180, 22);
            undoToolStripMenuItem.Text = "&Undo";
            undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(180, 22);
            redoToolStripMenuItem.Text = "&Redo";
            redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(177, 6);
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Image = (Image)resources.GetObject("cutToolStripMenuItem.Image");
            cutToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            cutToolStripMenuItem.Size = new Size(180, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Image = (Image)resources.GetObject("copyToolStripMenuItem.Image");
            copyToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            copyToolStripMenuItem.Size = new Size(180, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Image = (Image)resources.GetObject("pasteToolStripMenuItem.Image");
            pasteToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteToolStripMenuItem.Size = new Size(180, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { contentsToolStripMenuItem, toolStripSeparator5, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            contentsToolStripMenuItem.Size = new Size(122, 22);
            contentsToolStripMenuItem.Text = "&Contents";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(122, 22);
            aboutToolStripMenuItem.Text = "&About...";
            // 
            // themeToolStripMenuItem
            // 
            themeToolStripMenuItem.Name = "themeToolStripMenuItem";
            themeToolStripMenuItem.Size = new Size(55, 20);
            themeToolStripMenuItem.Text = "Theme";
            themeToolStripMenuItem.Click += themeToolStripMenuItem_Click;
            // 
            // realmToolStripMenuItem
            // 
            realmToolStripMenuItem.Name = "realmToolStripMenuItem";
            realmToolStripMenuItem.Size = new Size(52, 20);
            realmToolStripMenuItem.Text = "Realm";
            // 
            // RealmStudioMainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.Disable;
            BackColor = Color.FromArgb(223, 219, 210);
            ClientSize = new Size(1583, 1024);
            Controls.Add(RealmStudioForm);
            DoubleBuffered = true;
            ForeColor = SystemColors.Control;
            FormBorderStyle = FormBorderStyle.None;
            MainMenuStrip = menuStrip1;
            MaximumSize = new Size(1920, 1200);
            MinimumSize = new Size(261, 65);
            Name = "RealmStudioMainForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Realm Studio";
            TransparencyKey = Color.Fuchsia;
            Load += RealmStudioMainForm_Load;
            Shown += RealmStudioMainForm_Shown;
            OceanTab.ResumeLayout(false);
            MainTab.ResumeLayout(false);
            BackgroundTab.ResumeLayout(false);
            BackgroundTab.PerformLayout();
            RealmStudioForm.ResumeLayout(false);
            RealmStudioForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private ReaLTaiizor.Controls.CrownContextMenuStrip crownContextMenuStrip1;
        private ReaLTaiizor.Forms.DungeonForm RealmStudioForm;
        private ReaLTaiizor.Controls.ThunderControlBox thunderControlBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem printToolStripMenuItem;
        private ToolStripMenuItem printPreviewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private StatusStrip statusStrip1;
        private StatusStrip statusStrip2;
        private TabControl MainTab;
        private TabPage BackgroundTab;
        private TabPage OceanTab;
        private ReaLTaiizor.Controls.PoisonPanel poisonPanel1;
        private SkiaSharp.Views.Desktop.SKGLControl SKGLRenderControl;
        private ToolStrip toolStrip1;
        private TabPage LandTab;
        private TabPage WaterTab;
        private TabPage PathTab;
        private TabPage LabelTab;
        private TabPage OverlayTab;
        private TabPage RegionTab;
        private StatusStrip statusStrip3;
        private TabPage DrawingTab;
        private PictureBox pictureBox1;
        private Label label1;
        private ReaLTaiizor.Controls.CyberSwitch AutosaveSwitch;
        private FontAwesome.Sharp.IconButton SaveButton;
        private TabPage SymbolTab;
        private ReaLTaiizor.Controls.MaterialScrollBar MapRenderHScroll;
        private ReaLTaiizor.Controls.MaterialScrollBar MapRenderVScroll;
        private ReaLTaiizor.Controls.CyberSwitch cyberSwitch1;
        private Label label2;
        private ToolStripMenuItem realmToolStripMenuItem;
        private ToolStripMenuItem themeToolStripMenuItem;
    }
}
