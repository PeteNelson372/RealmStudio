/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using RealmStudio.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Svg.Skia;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Timers;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using TextBox = System.Windows.Forms.TextBox;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private readonly string RELEASE_STATE = "Pre-Release";

        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static System.Timers.Timer? LOCATION_UPDATE_TIMER;

        internal static readonly System.Windows.Forms.ToolTip TOOLTIP = new();

        private TextBox? LABEL_TEXT_BOX;

        public static readonly NameGeneratorConfiguration NAME_GENERATOR_CONFIG = new();

        private static bool CREATING_LABEL;

        private readonly AppSplashScreen SPLASH_SCREEN = new();

        private static string MapCommandLinePath = string.Empty;

        private static float SELECTED_PATH_ANGLE = -1;

        public ThreeDView? CurrentHeightMapView { get; set; }
        public List<string> LandFormObjModelList { get; set; } = [];

        // UI Mediators

        // UI mediator for the UI controls and values on the RealmStudioMainFOrm
        // not related to any map object (e.g. DrawingZoom)
        private MainFormUIMediator MainUIMediator { get; set; }

        // UI mediator for map boxes
        private BoxUIMediator BoxMediator { get; set; }

        // UI mediator for the map frame
        private FrameUIMediator FrameMediator { get; set; }

        // UI mediator for MapGrid
        private MapGridUIMediator GridUIMediator { get; set; }

        // UI mediator for Labels
        private LabelUIMediator LabelMediator { get; set; }

        // UI Mediator for Label Presets
        private LabelPresetUIMediator PresetMediator { get; set; }

        // UI mediator for MapMeasure
        private MapMeasureUIMediator MeasureUIMediator { get; set; }

        // UI mediator for MapScale
        private MapScaleUIMediator ScaleUIMediator { get; set; }

        // UI mediator for MapSymbols
        private MapSymbolUIMediator SymbolUIMediator { get; set; }

        // UI mediator for MapRegions
        private RegionUIMediator MapRegionUIMediator { get; set; }

        private TimerManager ApplicationTimerManager { get; set; }

        #region Constructor
        /******************************************************************************************************* 
        * MAIN FORM CONSTRUCTOR
        *******************************************************************************************************/
        public RealmStudioMainForm(string[] args)
        {
            AssetManager.InitializeAssetDirectories();

            InitializeComponent();

            if (args.Length > 0)
            {
                MapCommandLinePath = args[0];

                if (!File.Exists(MapCommandLinePath))
                {
                    MapCommandLinePath = string.Empty;
                }
            }

            SKGLRenderControl.Hide();
            SKGLRenderControl.MouseWheel += SKGLRenderControl_MouseWheel;

            // make sure the loading status form displays at the center of the main window
            AssetManager.LOADING_STATUS_FORM.Top = Top + (Height / 2) - (AssetManager.LOADING_STATUS_FORM.Height / 2);
            AssetManager.LOADING_STATUS_FORM.Left = Left + (Width / 2) - (AssetManager.LOADING_STATUS_FORM.Width / 2);

            string assetDirectory = AssetManager.ASSET_DIRECTORY;

            if (string.IsNullOrEmpty(assetDirectory))
            {
                Settings.Default.MapAssetDirectory = UtilityMethods.DEFAULT_ASSETS_FOLDER;
                AssetManager.ASSET_DIRECTORY = UtilityMethods.DEFAULT_ASSETS_FOLDER;
            }
            else
            {
                Settings.Default.MapAssetDirectory = assetDirectory;
            }

            Settings.Default.Save();

            // create and initialize managers and UI mediators
            MainUIMediator = new(this);
            MapStateMediator.MainUIMediator = MainUIMediator;

            BoxMediator = new BoxUIMediator(this);
            BoxManager.BoxMediator = BoxMediator;

            FrameMediator = new FrameUIMediator(this);
            FrameManager.FrameMediator = FrameMediator;

            GridUIMediator = new MapGridUIMediator(this);
            MapGridManager.GridUIMediator = GridUIMediator;

            LabelMediator = new LabelUIMediator(this);
            LabelManager.LabelMediator = LabelMediator;

            MapRegionUIMediator = new(this);
            RegionManager.RegionUIMediator = MapRegionUIMediator;

            MeasureUIMediator = new(this);
            MapMeasureManager.MeasureUIMediator = MeasureUIMediator;

            PresetMediator = new(this);
            LabelPresetManager.PresetMediator = PresetMediator;

            ScaleUIMediator = new(this);
            MapScaleManager.ScaleUIMediator = ScaleUIMediator;

            SymbolUIMediator = new MapSymbolUIMediator(this);
            SymbolManager.SymbolUIMediator = SymbolUIMediator;


            ApplicationTimerManager = new(this)
            {
                SymbolUIMediator = SymbolUIMediator
            };


            // register map object mediators with the MapStateMediator so
            // they can be notified when the current map is changed
            // or other changes are made; in most cases, the map object
            // mediators handle changes directly with the associated manager
            // but in some cases, changes to the current map (and maybe to other objects)
            // result in UI changes that the map object mediators don't know about

            MapStateMediator.BoxMediator = BoxMediator;
            MapStateMediator.FrameMediator = FrameMediator;
            MapStateMediator.GridUIMediator = GridUIMediator;
            MapStateMediator.SymbolUIMediator = SymbolUIMediator;
            MapStateMediator.RegionUIMediator = MapRegionUIMediator;
            MapStateMediator.MeasureUIMediator = MeasureUIMediator;
            MapStateMediator.ScaleUIMediator = ScaleUIMediator;
        }

        #endregion

        #region Main Form Event Handlers
        /******************************************************************************************************* 
        * MAIN FORM EVENT HANDLERS
        *******************************************************************************************************/
        private void RealmStudioMainForm_Load(object sender, EventArgs e)
        {
            BackgroundToolPanel.Visible = true;
            OceanToolPanel.Visible = false;
            LandToolPanel.Visible = false;
            WaterToolPanel.Visible = false;
            PathToolPanel.Visible = false;
            SymbolToolPanel.Visible = false;
            LabelToolPanel.Visible = false;
            OverlayToolPanel.Visible = false;
            RegionToolPanel.Visible = false;
            DrawingToolPanel.Visible = false;

            LogoPictureBox.Hide();

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (version != null)
            {
                SPLASH_SCREEN.VersionLabel.Text = string.Concat(RELEASE_STATE + " Version ", version);
            }
            else
            {
                SPLASH_SCREEN.VersionLabel.Text = RELEASE_STATE + " Version Unknown";
            }

            SPLASH_SCREEN.ShowDialog(this);
        }

        private void RealmStudioMainForm_Shown(object sender, EventArgs e)
        {

            MapBuilder.DisposeMap(MapStateMediator.CurrentMap);

            SKGLRenderControl.CreateControl();
            SKGLRenderControl.Show();
            SKGLRenderControl.Select();

            Cursor = Cursors.WaitCursor;

            LogoPictureBox.Show();

            Refresh();

            SPLASH_SCREEN.Close();

            AssetManager.LOADING_STATUS_FORM.Show(this);

            int assetCount = AssetManager.LoadAllAssets();

            AssetManager.LOADING_STATUS_FORM.Hide();

            LogoPictureBox.Hide();

            if (!string.IsNullOrEmpty(MapCommandLinePath))
            {
                OpenMap(MapCommandLinePath);
            }
            else
            {
                // this creates the MapStateMediator.CurrentMap
                DialogResult result = OpenRealmConfigurationDialog();

                if (result != DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            PopulateControlsWithAssets(assetCount);
            PopulateFontPanelUI();
            LoadNameGeneratorConfigurationDialog();

            if (AssetManager.CURRENT_THEME != null)
            {
                ThemeFilter themeFilter = new();
                ApplyTheme(AssetManager.CURRENT_THEME, themeFilter);
            }

            Activate();

            ApplicationTimerManager.StartAutosaveTimer();

            StartLocationUpdateTimer();

            SKGLRenderControl.Invalidate();

            Cursor = Cursors.Default;
        }

        private void RealmStudioMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationTimerManager.AutosaveEnabled = false;
            ApplicationTimerManager.BrushTimerEnabled = false;
            // stop timers

            StopLocationUpdateTimer();

            // save user preferences
            Settings.Default.Save();

            // save symbol tags and collections
            SymbolManager.SaveSymbolTags();
            SymbolManager.SaveCollections();

            // close the name generator dialog
            NAME_GENERATOR_CONFIG.Close();

            // save the map
            if (!MapStateMediator.CurrentMap.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Exit Application", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMapAs();

                    if (saveResult == DialogResult.Cancel)
                    {
                        // cancel application shutdown if the user cancels
                        e.Cancel = true;
                    }
                    else
                    {
                        MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    // cancel application shutdown if the user cancels
                    e.Cancel = true;
                }
            }
            else
            {
                MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
            }
        }

        private void AutosaveSwitch_CheckedChanged()
        {
            Settings.Default.RealmAutosave = AutosaveSwitch.Checked;
            Settings.Default.Save();
            ApplicationTimerManager.AutosaveEnabled = AutosaveSwitch.Checked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // save the map
            SaveMap();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            // open a map
            OpenExistingMap();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.DrawingZoom = 1.0F;
            ZoomLevelTrack.Value = 10;

            MapStateMediator.ScrollPoint = new SKPoint(0, 0);
            MapStateMediator.DrawingPoint = new SKPoint(0, 0);

            MapRenderHScroll.Value = 0;
            MapRenderVScroll.Value = 0;

            SKGLRenderControl.Invalidate();
        }

        private void ZoomToFitButton_Click(object sender, EventArgs e)
        {
            float horizontalAspect = (float)SKGLRenderControl.Width / MapStateMediator.CurrentMap.MapWidth;
            float verticalAspect = (float)SKGLRenderControl.Height / MapStateMediator.CurrentMap.MapHeight;

            MainUIMediator.DrawingZoom = Math.Min(horizontalAspect, verticalAspect);
            ZoomLevelTrack.Value = Math.Max(1, (int)MainUIMediator.DrawingZoom);

            MapStateMediator.ScrollPoint = new SKPoint(0, 0);
            MapStateMediator.DrawingPoint = new SKPoint(0, 0);

            MapRenderHScroll.Value = 0;
            MapRenderVScroll.Value = 0;

            SKGLRenderControl.Invalidate();
        }

        private void MainTab_KeyDown(object sender, KeyEventArgs e)
        {
            // why was this set to true?
            // setting e.SuppressKeyPress prevents controls (like trackbars)
            // from responding to key presses
            //e.SuppressKeyPress = true;
        }

        private void Open3DViewButton_Click(object sender, EventArgs e)
        {
            ThreeDView tdv = new("3D Model Viewer");
            tdv.Show();
        }

        private void AreaSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.RealmAreaSelect, 0);
        }

        private void AreaSelectButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Select map area", RealmStudioForm, new Point(AreaSelectButton.Left, AreaSelectButton.Top + 30), 3000);
        }

        private void AddPresetColorButton_Click(object sender, EventArgs e)
        {
            switch (MainTab.SelectedIndex)
            {
                case 1:
                    {
                        // ocean layer
                        Color selectedColor = UtilityMethods.SelectColorFromDialog(this, OceanPaintColorSelectButton.BackColor);

                        if (selectedColor != Color.Empty)
                        {
                            Color oceanColor = selectedColor;

                            if (OceanCustomColorButton1.Text == "")
                            {
                                OceanCustomColorButton1.BackColor = oceanColor;
                                OceanCustomColorButton1.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton1.Refresh();
                            }
                            else if (OceanCustomColorButton2.Text == "")
                            {
                                OceanCustomColorButton2.BackColor = oceanColor;
                                OceanCustomColorButton2.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton2.Refresh();
                            }
                            else if (OceanCustomColorButton3.Text == "")
                            {
                                OceanCustomColorButton3.BackColor = oceanColor;
                                OceanCustomColorButton3.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton3.Refresh();
                            }
                            else if (OceanCustomColorButton4.Text == "")
                            {
                                OceanCustomColorButton4.BackColor = oceanColor;
                                OceanCustomColorButton4.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton4.Refresh();
                            }
                            else if (OceanCustomColorButton5.Text == "")
                            {
                                OceanCustomColorButton5.BackColor = oceanColor;
                                OceanCustomColorButton5.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton5.Refresh();
                            }
                            else if (OceanCustomColorButton6.Text == "")
                            {
                                OceanCustomColorButton6.BackColor = oceanColor;
                                OceanCustomColorButton6.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton6.Refresh();
                            }
                            else if (OceanCustomColorButton7.Text == "")
                            {
                                OceanCustomColorButton7.BackColor = oceanColor;
                                OceanCustomColorButton7.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton7.Refresh();
                            }
                            else if (OceanCustomColorButton8.Text == "")
                            {
                                OceanCustomColorButton8.BackColor = oceanColor;
                                OceanCustomColorButton8.Text = ColorTranslator.ToHtml(oceanColor);
                                OceanCustomColorButton8.Refresh();
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        // land layer
                        Color selectedColor = UtilityMethods.SelectColorFromDialog(this, LandColorSelectionButton.BackColor);

                        if (selectedColor != Color.Empty)
                        {
                            Color landColor = selectedColor;

                            if (LandCustomColorButton1.Text == "")
                            {
                                LandCustomColorButton1.BackColor = landColor;
                                LandCustomColorButton1.Text = ColorTranslator.ToHtml(landColor);
                                LandCustomColorButton1.Refresh();
                            }
                            else if (LandCustomColorButton2.Text == "")
                            {
                                LandCustomColorButton2.BackColor = landColor;
                                LandCustomColorButton2.Text = ColorTranslator.ToHtml(landColor);
                                LandCustomColorButton2.Refresh();
                            }
                            else if (LandCustomColorButton3.Text == "")
                            {
                                LandCustomColorButton3.BackColor = landColor;
                                LandCustomColorButton3.Text = ColorTranslator.ToHtml(landColor);
                                LandCustomColorButton3.Refresh();
                            }
                            else if (LandCustomColorButton4.Text == "")
                            {
                                LandCustomColorButton4.BackColor = landColor;
                                LandCustomColorButton4.Text = ColorTranslator.ToHtml(landColor);
                                LandCustomColorButton4.Refresh();
                            }
                            else if (LandCustomColorButton5.Text == "")
                            {
                                LandCustomColorButton5.BackColor = landColor;
                                LandCustomColorButton5.Text = ColorTranslator.ToHtml(landColor);
                                LandCustomColorButton5.Refresh();
                            }
                            else if (LandCustomColorButton6.Text == "")
                            {
                                LandCustomColorButton6.BackColor = landColor;
                                LandCustomColorButton6.Text = ColorTranslator.ToHtml(landColor);
                                LandCustomColorButton6.Refresh();
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        // water layer
                        Color selectedColor = UtilityMethods.SelectColorFromDialog(this, WaterPaintColorSelectButton.BackColor);

                        if (selectedColor != Color.Empty)
                        {
                            Color waterColor = selectedColor;

                            if (WaterCustomColor1.Text == "")
                            {
                                WaterCustomColor1.BackColor = waterColor;
                                WaterCustomColor1.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor1.Refresh();
                            }
                            else if (WaterCustomColor2.Text == "")
                            {
                                WaterCustomColor2.BackColor = waterColor;
                                WaterCustomColor2.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor2.Refresh();
                            }
                            else if (WaterCustomColor3.Text == "")
                            {
                                WaterCustomColor3.BackColor = waterColor;
                                WaterCustomColor3.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor3.Refresh();
                            }
                            else if (WaterCustomColor4.Text == "")
                            {
                                WaterCustomColor4.BackColor = waterColor;
                                WaterCustomColor4.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor4.Refresh();
                            }
                            else if (WaterCustomColor5.Text == "")
                            {
                                WaterCustomColor5.BackColor = waterColor;
                                WaterCustomColor5.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor5.Refresh();
                            }
                            else if (WaterCustomColor6.Text == "")
                            {
                                WaterCustomColor6.BackColor = waterColor;
                                WaterCustomColor6.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor6.Refresh();
                            }
                            else if (WaterCustomColor7.Text == "")
                            {
                                WaterCustomColor7.BackColor = waterColor;
                                WaterCustomColor7.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor7.Refresh();
                            }
                            else if (WaterCustomColor8.Text == "")
                            {
                                WaterCustomColor8.BackColor = waterColor;
                                WaterCustomColor8.Text = ColorTranslator.ToHtml(waterColor);
                                WaterCustomColor8.Refresh();
                            }
                        }
                    }
                    break;
            }
        }

        private void AddPresetColorButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Add custom preset color", RealmStudioForm, new Point(AddPresetColorButton.Left, AddPresetColorButton.Top + 30), 3000);
        }

        private void SelectColorButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.ColorSelect, 0);
        }

        private void SelectColorButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Select color from map", RealmStudioForm, new Point(SelectColorButton.Left, SelectColorButton.Top + 30), 3000);
        }

        private void ZoomLevelTrack_Scroll(object sender, EventArgs e)
        {
            MainUIMediator.DrawingZoom = ZoomLevelTrack.Value / 10.0F;
            SKGLRenderControl.Invalidate();
        }

        internal void NameGenerator_NameGenerated(object? sender, EventArgs e)
        {
            if (sender is NameGeneratorConfiguration ngc)
            {
                string selectedName = ngc.SelectedName;

                if (CREATING_LABEL && !string.IsNullOrEmpty(selectedName))
                {
                    if (LABEL_TEXT_BOX != null && !LABEL_TEXT_BOX.IsDisposed)
                    {
                        LABEL_TEXT_BOX.Text = selectedName;
                        LABEL_TEXT_BOX.Refresh();
                    }
                }
            }
        }

        #endregion

        #region Main Menu Event Handlers

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MapStateMediator.CurrentMap.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Save Map", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult == DialogResult.OK)
                    {
                        CreateNewMap();
                        Cursor = Cursors.Default;
                    }
                }
                else if (result == DialogResult.No)
                {
                    CreateNewMap();
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                CreateNewMap();
                Cursor = Cursors.Default;
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MapStateMediator.CurrentMap.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Save Map", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult == DialogResult.OK)
                    {
                        OpenExistingMap();
                        Cursor = Cursors.Default;
                    }
                }
                else if (result == DialogResult.No)
                {
                    OpenExistingMap();
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                OpenExistingMap();
                Cursor = Cursors.Default;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MapStateMediator.CurrentMap.IsSaved)
            {
                return;
            }

            if (!string.IsNullOrEmpty(MapStateMediator.CurrentMap.MapPath))
            {
                try
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();

                    // serialize the map to disk as an XML file
                    MapFileMethods.SaveMap(MapStateMediator.CurrentMap);
                    MapStateMediator.CurrentMap.IsSaved = true;

                    if (Settings.Default.PlaySoundOnSave)
                    {
                        UtilityMethods.PlaySaveSound();
                        SetStatusText("Realm " + Path.GetFileNameWithoutExtension(MapStateMediator.CurrentMap.MapPath) + " has been saved.");
                    }
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("An error has occurred while saving the map. The map file map be corrupted.", "Map Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            else
            {
                SaveMap();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMapAs();
        }

        private void ExportMapMenuItem_Click(object sender, EventArgs e)
        {
            RealmExportDialog exportDialog = new();

            DialogResult exportresult = exportDialog.ShowDialog();

            RealmExportType exportType = RealmExportType.NotSet;
            RealmMapExportFormat exportFormat = RealmMapExportFormat.NotSet;

            if (exportresult == DialogResult.OK)
            {
                if (exportDialog.ExportImageRadio.Checked)
                {
                    exportType = RealmExportType.BitmapImage;
                }
                else if (exportDialog.UpscaledImageRadio.Checked)
                {
                    exportType = RealmExportType.UpscaledImage;
                }
                else if (exportDialog.MapLayersRadio.Checked)
                {
                    exportType = RealmExportType.MapLayers;
                }
                else if (exportDialog.HeightmapRadio.Checked)
                {
                    exportType = RealmExportType.Heightmap;
                }
                else if (exportDialog.HeightMap3DRadio.Checked)
                {
                    exportType = RealmExportType.HeightMap3DModel;
                }

                if (exportDialog.PNGRadio.Checked)
                {
                    exportFormat = RealmMapExportFormat.PNG;
                }
                else if (exportDialog.JPEGRadio.Checked)
                {
                    exportFormat = RealmMapExportFormat.JPG;
                }
                else if (exportDialog.BMPRadio.Checked)
                {
                    exportFormat = RealmMapExportFormat.BMP;
                }
                else if (exportDialog.GIFRadio.Checked)
                {
                    exportFormat = RealmMapExportFormat.GIF;
                }
            }

            switch (exportType)
            {
                case RealmExportType.BitmapImage:
                    ExportMapAsImage(exportFormat);
                    break;
                case RealmExportType.UpscaledImage:
                    ExportMapAsImage(exportFormat, true);
                    break;
                case RealmExportType.MapLayers:
                    ExportMapAsLayers(exportFormat);
                    break;
                case RealmExportType.Heightmap:
                    ExportHeightMap(exportFormat);
                    break;
                case RealmExportType.HeightMap3DModel:
                    MapHeightMapMethods.ExportHeightMap3DModel(MapStateMediator.CurrentMap);
                    break;
            }
        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreview printPreview = new(MapStateMediator.CurrentMap);
            printPreview.ShowDialog();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MapStateMediator.CurrentMap.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Exit Application", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult == DialogResult.OK)
                    {
                        Close();
                        MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
                        Application.Exit();
                    }
                }
                else if (result == DialogResult.No)
                {
                    MapStateMediator.CurrentMap.IsSaved = true;
                    Close();
                    MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
                    Application.Exit();
                }
            }
            else
            {
                Close();
                MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
                Application.Exit();
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandManager.Undo();
            SKGLRenderControl.Invalidate();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommandManager.Redo();
            SKGLRenderControl.Invalidate();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MapStateMediator.SelectedRealmArea != SKRect.Empty)
            {
                // get all objects within the selected area and cut them

                // the object that is cut/copied must lie completely within the selected area
                // if it is not a "point" object (e.g. a symbol)

                CutCopyPasteManager.SelectedMapComponents = RealmMapMethods.SelectMapComponentsInArea(MapStateMediator.CurrentMap, MapStateMediator.SelectedRealmArea);
                CutCopyPasteManager.CutSelectedComponents();

                SKGLRenderControl.Invalidate();
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCopyPasteManager.CopySelectedComponents();
            SKGLRenderControl.Invalidate();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if the list of copied items is not empty
            // then paste them at the cursor point, updating their position
            Point cursorPosition = SKGLRenderControl.PointToClient(Cursor.Position);
            SKPoint zoomedScrolledPoint = new((cursorPosition.X / MainUIMediator.DrawingZoom) + MapStateMediator.DrawingPoint.X,
                (cursorPosition.Y / MainUIMediator.DrawingZoom) + MapStateMediator.DrawingPoint.Y);

            CutCopyPasteManager.PasteSelectedComponentsAtPoint(zoomedScrolledPoint);
            SKGLRenderControl.Invalidate();
        }

        private void ClearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCopyPasteManager.ClearComponentSelection();
            SKGLRenderControl.Invalidate();
        }

        private void RenderAsHeightMapMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (RenderAsHeightMapMenuItem.Checked)
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.HeightMapPaint, 0);

                // if needed, render landforms to height map layer as a map image
                // and add a mapheightmap to the height map layer to hold the height map;
                // as the user paints on the map, the height map image will be updated

                RealmMapMethods.AddMapImagesToHeightMapLayer(MapStateMediator.CurrentMap);

                // force maintab selected index change
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.SelectedIndex = 0;
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.Refresh();
            }
            else
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

                // force maintab selected index change
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.SelectedIndex = 0;
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.Refresh();
            }

            SKGLRenderControl.Invalidate();
        }

        private void ThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show the theme list dialog
            ThemeList themeList = new();

            // get the current state of the UI as a theme
            // and send it to the ThemeList dialog
            MapTheme settingsTheme = SaveCurentSettingsToTheme();

            themeList.SetThemes([.. AssetManager.THEME_LIST]);
            themeList.SettingsTheme = settingsTheme;

            DialogResult result = themeList.ShowDialog();

            if (result == DialogResult.OK)
            {
                // on OK result, apply the selected theme
                MapTheme selectedTheme = themeList.GetSelectedTheme();

                if (!string.IsNullOrEmpty(selectedTheme.ThemeName))
                {
                    AssetManager.CURRENT_THEME = selectedTheme;
                    ThemeFilter themeFilter = themeList.GetThemeFilter();
                    ApplyTheme(selectedTheme, themeFilter);
                }
            }
        }

        private void MapPropertiesMenuItem_Click(object sender, EventArgs e)
        {
            RealmProperties propertiesDialog = new(MapStateMediator.CurrentMap);
            DialogResult result = propertiesDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                MapStateMediator.CurrentMap.MapName = propertiesDialog.NameTextbox.Text;
                UpdateMapNameAndSize();
            }
        }

        private void ChangeMapSizeMenuItem_Click(object sender, EventArgs e)
        {
            if (!MapStateMediator.CurrentMap.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Save Map", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult != DialogResult.OK)
                    {
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            RealmStudioMap? resizedMap = RealmMapMethods.CreateResizedMap(this, MapStateMediator.CurrentMap);

            if (resizedMap != null)
            {
                MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
                MapStateMediator.CurrentMap = resizedMap;

                // TODO: generalize
                ScaleUIMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

                MAP_WIDTH = MapStateMediator.CurrentMap.MapWidth;
                MAP_HEIGHT = MapStateMediator.CurrentMap.MapHeight;

                MapRenderHScroll.Maximum = MAP_WIDTH;
                MapRenderVScroll.Maximum = MAP_HEIGHT;

                MapRenderHScroll.Value = 0;
                MapRenderVScroll.Value = 0;

                UpdateMapNameAndSize();

                SKGLRenderControl.Invalidate();
            }
        }

        private void CreateDetailMapMenuItem_Click(object sender, EventArgs e)
        {
            if (!MapStateMediator.CurrentMap.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Save Map", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult != DialogResult.OK)
                    {
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            RealmStudioMap? detailMap = RealmMapMethods.CreateDetailMap(this, MapStateMediator.CurrentMap, MapStateMediator.SelectedRealmArea);

            if (detailMap != null)
            {
                MapBuilder.DisposeMap(MapStateMediator.CurrentMap);
                MapStateMediator.CurrentMap = detailMap;

                // TODO: generalize
                ScaleUIMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

                MAP_WIDTH = MapStateMediator.CurrentMap.MapWidth;
                MAP_HEIGHT = MapStateMediator.CurrentMap.MapHeight;

                MapRenderHScroll.Maximum = MAP_WIDTH;
                MapRenderVScroll.Maximum = MAP_HEIGHT;

                MapRenderHScroll.Value = 0;
                MapRenderVScroll.Value = 0;

                UpdateMapNameAndSize();

                SKGLRenderControl.Invalidate();
            }
        }

        private void TraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Open Image to Trace",
                    DefaultExt = "png",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false
                };

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                string sep = string.Empty;

                foreach (var c in codecs)
                {
                    if (!string.IsNullOrEmpty(c.CodecName) && !string.IsNullOrEmpty(c.FilenameExtension))
                    {
                        string codecName = c.CodecName[8..].Replace("Codec", "Files").Trim();
                        ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, codecName, c.FilenameExtension.ToLowerInvariant());
                        sep = "|";
                    }
                }

                ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "All Files", "*.*");

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            Bitmap b = (Bitmap)Bitmap.FromFile(ofd.FileName);

                            SKPath landformPath = LandformMethods.TraceImage(ofd.FileName);

                            if (!landformPath.IsEmpty)
                            {
                                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);

                                if (MapStateMediator.SelectedRealmArea != SKRect.Empty)
                                {
                                    landformPath.Transform(SKMatrix.CreateScale(MapStateMediator.SelectedRealmArea.Width / b.Width, MapStateMediator.SelectedRealmArea.Height / b.Height));
                                }
                                else
                                {
                                    // TODO: scale the landform path to fit the map, maintaining the aspect ratio of the original image
                                    //landformPath.Transform(SKMatrix.CreateScale((float)MapStateMediator.CurrentMap.MapWidth / b.Width, (float)MapStateMediator.CurrentMap.MapHeight / b.Height));
                                }

                                Landform landform = LandformMethods.CreateNewLandform(MapStateMediator.CurrentMap, landformPath, MapStateMediator.SelectedRealmArea, UseTextureForBackgroundCheck.Checked, SKGLRenderControl);

                                SetLandformData(landform);

                                landformLayer.MapLayerComponents.Add(landform);
                            }
                            else
                            {
                                MessageBox.Show("Failed to trace map outline from " + ofd.FileName, "Map Trace Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                            throw;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                }
            }
            catch { }
        }

        private void CreateSymbolCollectionMenuItem_Click(object sender, EventArgs e)
        {
            SymbolCollectionForm symbolCollectionForm = new();
            symbolCollectionForm.ShowDialog(this);
        }

        private void WDAssetZipFileMenuItem_Click(object sender, EventArgs e)
        {
            WonderdraftAssetImportDialog dlg = new();
            dlg.ShowDialog(this);
        }

        private void WDUserFolderMenuItem_Click(object sender, EventArgs e)
        {
            WonderdraftUserFolderImportDialog dlg = new();
            dlg.ShowDialog(this);
        }

        private void ReloadAllAssetsMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            AssetManager.LOADING_STATUS_FORM.ResetLoadingProgress();
            AssetManager.LOADING_STATUS_FORM.Show(this);

            int assetCount = AssetManager.LoadAllAssets();
            PopulateControlsWithAssets(assetCount);
            PopulateFontPanelUI();
            LoadNameGeneratorConfigurationDialog();

            AssetManager.LOADING_STATUS_FORM.Hide();

            Cursor = Cursors.Default;
        }

        private void PreferencesMenuItem_Click(object sender, EventArgs e)
        {
            UserPreferences preferencesDlg = new();
            preferencesDlg.ShowDialog(this);

            AutosaveSwitch.Checked = Settings.Default.RealmAutosave;
            ApplicationTimerManager.AutosaveEnabled = AutosaveSwitch.Checked;
        }

        private void NameGeneratorConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NAME_GENERATOR_CONFIG.Show();
        }

        private void HelpContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://petenelson372.github.io/RealmStudioDocs/",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            AboutRealmStudio aboutRealmStudio = new();

            if (!string.IsNullOrEmpty(version))
            {
                aboutRealmStudio.RealmStudioVersionLabel.Text = string.Concat(RELEASE_STATE + " Version ", version);
            }
            else
            {
                aboutRealmStudio.RealmStudioVersionLabel.Text = RELEASE_STATE + " Version Unknown";
            }

            aboutRealmStudio.ShowDialog();
        }
        #endregion

        #region Main Form Methods
        /******************************************************************************************************* 
         * MAIN FORM METHODS
         *******************************************************************************************************/

        private void PanMap(MouseEventArgs e)
        {
            // pan the map when middle button (mouse wheel) is held down and dragged

            int xDelta = (int)(e.Location.X - MapStateMediator.PreviousMouseLocation.X);
            int yDelta = (int)(e.Location.Y - MapStateMediator.PreviousMouseLocation.Y);

            MapStateMediator.ScrollPoint = new SKPoint(MapStateMediator.ScrollPoint.X + xDelta, MapStateMediator.ScrollPoint.Y + yDelta);
            MapStateMediator.DrawingPoint = new SKPoint(MapStateMediator.DrawingPoint.X - xDelta, MapStateMediator.DrawingPoint.Y - yDelta);

            MapRenderHScroll.Value = Math.Max(0, Math.Min((int)MapStateMediator.DrawingPoint.X, MapStateMediator.CurrentMap.MapWidth));

            MapRenderVScroll.Value = Math.Max(0, Math.Min((int)MapStateMediator.DrawingPoint.Y, MapStateMediator.CurrentMap.MapHeight));

            MapStateMediator.PreviousMouseLocation = e.Location.ToSKPoint();
        }

        private void CreateNewMap()
        {
            MapBuilder.DisposeMap(MapStateMediator.CurrentMap);

            // this creates the MapStateMediator.CurrentMap
            DialogResult newResult = OpenRealmConfigurationDialog();

            if (newResult == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;

                AssetManager.LOADING_STATUS_FORM.ResetLoadingProgress();
                AssetManager.LOADING_STATUS_FORM.Show(this);

                MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

                int assetCount = AssetManager.LoadAllAssets();

                PopulateControlsWithAssets(assetCount);

                AssetManager.LOADING_STATUS_FORM.Hide();

                LogoPictureBox.Hide();
                SKGLRenderControl.Show();
                SKGLRenderControl.Select();
                SKGLRenderControl.Refresh();
                SKGLRenderControl.Invalidate();

                Activate();
            }
        }

        private void StartLocationUpdateTimer()
        {
            // stop the location update timer if it is running
            StopLocationUpdateTimer();

            // start the location update timer
            LOCATION_UPDATE_TIMER = new System.Timers.Timer
            {
                Interval = 50,
                AutoReset = true,
                SynchronizingObject = this,
            };

            LOCATION_UPDATE_TIMER.Elapsed += new ElapsedEventHandler(LocationUpdateTimerEventHandler);
            LOCATION_UPDATE_TIMER.Start();
        }

        private void LocationUpdateTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            UpdateDrawingPointLabel();
        }

        private static void StopLocationUpdateTimer()
        {
            LOCATION_UPDATE_TIMER?.Stop();
            LOCATION_UPDATE_TIMER?.Dispose();
            LOCATION_UPDATE_TIMER = null;
        }

        private DialogResult OpenRealmConfigurationDialog()
        {
            RealmConfiguration rcd = new();

            rcd.Top = Top + (Height / 2) - (rcd.Height / 2);
            rcd.Left = Left + (Width / 2) - (rcd.Width / 2);

            DialogResult result = rcd.ShowDialog();

            if (result == DialogResult.OK)
            {
                // create the map from the settings on the dialog
                MapStateMediator.CurrentMap = MapBuilder.CreateMap(rcd.Map, SKGLRenderControl.GRContext);

                // TODO: this should be generalized
                ScaleUIMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

                MAP_WIDTH = MapStateMediator.CurrentMap.MapWidth;
                MAP_HEIGHT = MapStateMediator.CurrentMap.MapHeight;

                MapRenderHScroll.Maximum = MAP_WIDTH;
                MapRenderVScroll.Maximum = MAP_HEIGHT;

                MapRenderHScroll.Value = 0;
                MapRenderVScroll.Value = 0;

                if (string.IsNullOrEmpty(MapStateMediator.CurrentMap.MapName))
                {
                    MapStateMediator.CurrentMap.MapName = "Default";
                }

                UpdateMapNameAndSize();

                if (MapStateMediator.CurrentMap.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                {
                    MessageBox.Show("Application startup error. RealmStudio will close.");
                    Application.Exit();
                }

                // create the map vignette and add it to the vignette layer
                BackgroundMethods.RemoveVignette(MapStateMediator.CurrentMap);

                BackgroundMethods.AddVignette(MapStateMediator.CurrentMap, VignetteColorSelectionButton.BackColor.ToArgb(), VignetteStrengthTrack.Value,
                    RectangleVignetteRadio.Checked, SKGLRenderControl);
            }

            return result;
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear the drawing mode (and uncheck all drawing, paint, and erase buttons) when switching tabs
            MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            BackgroundToolPanel.Visible = true;
            OceanToolPanel.Visible = false;
            LandToolPanel.Visible = false;
            WaterToolPanel.Visible = false;
            PathToolPanel.Visible = false;
            SymbolToolPanel.Visible = false;
            LabelToolPanel.Visible = false;
            OverlayToolPanel.Visible = false;
            RegionToolPanel.Visible = false;
            DrawingToolPanel.Visible = false;
            HeightMapToolsPanel.Visible = false;

            switch (MainTab.SelectedIndex)
            {
                case 0:
                    BackgroundToolPanel.Visible = true;
                    break;
                case 1:
                    OceanToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 2:
                    BackgroundToolPanel.Visible = false;

                    if (RenderAsHeightMapMenuItem.Checked)
                    {
                        MainUIMediator.SetDrawingMode(MapDrawingMode.HeightMapPaint, 0);

                        LandToolPanel.Visible = false;
                        LandToolStrip.Visible = false;
                        BackgroundToolPanel.Visible = true;
                        HeightMapToolsPanel.Visible = true;
                    }
                    else
                    {
                        LandToolPanel.Visible = true;
                        LandToolStrip.Visible = true;
                        BackgroundToolPanel.Visible = false;
                        HeightMapToolsPanel.Visible = false;
                    }
                    break;
                case 3:
                    WaterToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 4:
                    PathToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 5:
                    SymbolToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 6:
                    LabelToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 7:
                    OverlayToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 8:
                    RegionToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
                case 9:
                    DrawingToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
            }

            Refresh();
        }

        private void SetZoomLevel(int upDown)
        {
            MainUIMediator.ChangeDrawingZoom(upDown);
        }

        private void PopulateControlsWithAssets(int assetCount)
        {
            SetStatusText("Loaded: " + assetCount + " assets.");

            // background texture
            if (AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.BACKGROUND_TEXTURE_LIST.First().TexturePath);
            }

            BackgroundTextureBox.Image = AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap;
            BackgroundTextureNameLabel.Text = AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureName;

            // landform texture
            if (AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST.First().TexturePath);
            }

            LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap;
            LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST.First().TextureName;

            // ocean texture
            if (AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.WATER_TEXTURE_LIST.First().TexturePath);
            }

            OceanTextureBox.Image = AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap;
            OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST.First().TextureName;

            // path texture
            if (AssetManager.PATH_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.PATH_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.PATH_TEXTURE_LIST.First().TexturePath);
            }

            PathTexturePreviewPicture.Image = AssetManager.PATH_TEXTURE_LIST.First().TextureBitmap;
            PathTextureNameLabel.Text = AssetManager.PATH_TEXTURE_LIST.First().TextureName;

            CoastlineStyleList.SelectedIndex = 6;  // default is dash pattern


            // symbol collections
            SymbolCollectionsListBox.Items.Clear();
            foreach (MapSymbolCollection collection in AssetManager.MAP_SYMBOL_COLLECTIONS)
            {
                SymbolCollectionsListBox.Items.Add(collection.GetCollectionName());
            }

            // symbol tags
            SymbolTagsListBox.Items.Clear();
            foreach (string tag in AssetManager.SYMBOL_TAGS)
            {
                SymbolTagsListBox.Items.Add(tag);
            }

            PresetMediator.AddLabelPresets();

            BoxMediator.AddMapBoxesToLabelBoxTable(AssetManager.MAP_BOX_LIST);

            FrameMediator.AddMapFramesToFrameTable(AssetManager.MAP_FRAME_TEXTURES);

            // apply default theme in main form
            if (AssetManager.CURRENT_THEME != null)
            {
                ThemeFilter tf = new();
                ApplyTheme(AssetManager.CURRENT_THEME, tf);
            }
        }

        public void LoadNameGeneratorConfigurationDialog()
        {
            NAME_GENERATOR_CONFIG.NameGenerated += new EventHandler(NameGenerator_NameGenerated);

            NAME_GENERATOR_CONFIG.NameGeneratorsListBox.Items.Clear();
            NAME_GENERATOR_CONFIG.NameGeneratorsListBox.DisplayMember = "NameGeneratorName";

            foreach (NameGenerator nameGenerator in MapToolMethods.NameGenerators)
            {
                NAME_GENERATOR_CONFIG.NameGeneratorsListBox.Items.Add(nameGenerator);

                NAME_GENERATOR_CONFIG.NameGeneratorsListBox.SetItemChecked(
                    NAME_GENERATOR_CONFIG.NameGeneratorsListBox.Items.Count - 1, true);
            }

            NAME_GENERATOR_CONFIG.NamebasesListBox.Items.Clear();
            NAME_GENERATOR_CONFIG.NamebasesListBox.DisplayMember = "NameBaseName";

            foreach (NameBase nameBase in MapToolMethods.NameBases)
            {
                if (!NAME_GENERATOR_CONFIG.NamebasesListBox.Items.Contains(nameBase))
                {
                    NAME_GENERATOR_CONFIG.NamebasesListBox.Items.Add(nameBase);
                    NAME_GENERATOR_CONFIG.NamebasesListBox.SetItemChecked(
                        NAME_GENERATOR_CONFIG.NamebasesListBox.Items.Count - 1, true);
                }
            }


            MapToolMethods.NameLanguages.Sort(new NameBaseLanguageComparer());
            NAME_GENERATOR_CONFIG.LanguagesListBox.DisplayMember = "Language";

            foreach (NameBaseLanguage language in MapToolMethods.NameLanguages)
            {
                bool languageFound = false;
                foreach (NameBaseLanguage l in NAME_GENERATOR_CONFIG.LanguagesListBox.Items)
                {
                    if (l.Language == language.Language)
                    {
                        languageFound = true;
                    }
                }

                if (!languageFound)
                {
                    NAME_GENERATOR_CONFIG.LanguagesListBox.Items.Add(language);
                }
            }

            for (int i = 0; i < NAME_GENERATOR_CONFIG.LanguagesListBox.Items.Count; i++)
            {
                NAME_GENERATOR_CONFIG.LanguagesListBox.SetItemChecked(i, true);
            }
        }

        internal void SetStatusText(string text)
        {
            // set the test of the first status strip text box
            ApplicationStatusStrip.Items[0].Text = text;
        }

        private void UpdateDrawingPointLabel()
        {
            DrawingPointLabel.Text = "Cursor Point: "
                + ((int)MapStateMediator.CurrentMouseLocation.X).ToString()
                + " , "
                + ((int)MapStateMediator.CurrentMouseLocation.Y).ToString()
                + "   Map Point: "
                + ((int)MapStateMediator.CurrentCursorPoint.X).ToString()
                + " , "
                + ((int)MapStateMediator.CurrentCursorPoint.Y).ToString();

            ApplicationStatusStrip.Invalidate();
        }

        private void UpdateMapNameAndSize()
        {
            MapNameLabel.Text = MapStateMediator.CurrentMap.MapName;
            MapSizeLabel.Text = "Map Size: " + MapStateMediator.CurrentMap.MapWidth.ToString() + " x " + MapStateMediator.CurrentMap.MapHeight.ToString();

            MapStatusStrip.Refresh();
        }



        private void ColorPresetButtonMouseClickHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ModifierKeys == Keys.Control)
                {
                    ResetColorPresetButton((Button)sender);
                }
                else
                {
                    if (MainTab.SelectedTab != null)
                    {
                        if (MainTab.SelectedTab.Text == "Ocean")
                        {
                            SetOceanPaintColorFromCustomPresetButton((Button)sender);
                        }
                        else if (MainTab.SelectedTab.Text == "Land")
                        {
                            SetLandPaintColorFromCustomPresetButton((Button)sender);
                        }
                        else if (MainTab.SelectedTab.Text == "Water")
                        {
                            SetWaterPaintColorFromCustomPresetButton((Button)sender);
                        }

                        ((Button)sender).Refresh();
                    }
                }
            }
        }

        private static void ResetColorPresetButton(Button b)
        {
            b.Text = string.Empty;
            b.BackColor = Color.White;
            b.Refresh();
        }

        private void ExportMapAsImage(RealmMapExportFormat exportFormat, bool upscale = false)
        {
            SaveFileDialog ofd = new()
            {
                Title = "Export Map",
                DefaultExt = exportFormat.ToString().ToLowerInvariant(),
                RestoreDirectory = true,
                ShowHelp = true,
                Filter = "",
                AddExtension = true,
                CheckPathExists = true,
                ShowHiddenFiles = false,
                ValidateNames = true,
            };

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            foreach (ImageCodecInfo c in codecs)
            {
                if (c.CodecName != null && c.FilenameExtension != null)
                {
                    string codecName = c.CodecName[8..].Replace("Codec", "Files").Trim();
                    ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, codecName, c.FilenameExtension.ToLowerInvariant());
                    sep = "|";
                }
            }

            ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "All Files", "*.*");

            int filterIndex = 0;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].FilenameExtension != null)
                {
                    if (codecs[i].FilenameExtension.Contains(exportFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        filterIndex = i + 1;
                        break;
                    }
                }
            }

#pragma warning restore CS8602 // Dereference of a possibly null reference.

            ofd.FilterIndex = filterIndex;

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = ofd.FileName;
                try
                {
                    ApplicationTimerManager.AutosaveEnabled = false;
                    ApplicationTimerManager.BrushTimerEnabled = false;
                    ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;

                    StopLocationUpdateTimer();


                    SKSurface s = SKSurface.Create(new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight));
                    s.Canvas.Clear();

                    LandformMethods.RemoveDeletedLandforms(MapStateMediator.CurrentMap);

                    MapRenderMethods.RenderMapForExport(MapStateMediator.CurrentMap, s.Canvas);

                    Bitmap bitmap = s.Snapshot().ToBitmap();

                    if (upscale)
                    {
                        Bitmap upscaledBitmap = new(bitmap.Width * 2, bitmap.Height * 2);
                        using (Graphics g = Graphics.FromImage(upscaledBitmap))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(bitmap, 0, 0, bitmap.Width * 2, bitmap.Height * 2);
                        }
                        bitmap = upscaledBitmap;
                    }

                    bitmap.Save(filename);

                    MessageBox.Show("Map exported to " + ofd.FileName, "Map Exported", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("AN error occurred while exporting map to " + ofd.FileName, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    ApplicationTimerManager.AutosaveEnabled = true;
                    StartLocationUpdateTimer();
                }
            }
        }

        private void ExportMapAsLayers(RealmMapExportFormat exportFormat)
        {
            SaveFileDialog ofd = new()
            {
                Title = "Export Map",
                DefaultExt = "zip",
                RestoreDirectory = true,
                ShowHelp = true,
                Filter = "",
                AddExtension = true,
                CheckPathExists = true,
                ShowHiddenFiles = false,
                ValidateNames = true,
            };

            string sep = string.Empty;

            ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "Zip File", "zip");
            sep = "|";
            ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "All Files", "*.*");

            ofd.FilterIndex = 1;

            // create a zip file and add each of the layer bitmaps to it
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = ofd.FileName;
                try
                {
                    ApplicationTimerManager.AutosaveEnabled = false;
                    ApplicationTimerManager.BrushTimerEnabled = false;
                    ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;

                    StopLocationUpdateTimer();

                    LandformMethods.RemoveDeletedLandforms(MapStateMediator.CurrentMap);

                    using (FileStream fileStream = new(filename, FileMode.OpenOrCreate))
                    {
                        RealmMapMethods.ExportMapLayersAsZipFile(fileStream, MapStateMediator.CurrentMap, exportFormat);
                    }

                    MessageBox.Show("Map layers exported to " + ofd.FileName, "Map Exported", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("An error occurred while exporting map layers to " + ofd.FileName, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                finally
                {
                    ApplicationTimerManager.AutosaveEnabled = true;
                    StartLocationUpdateTimer();
                }
            }
        }

        private void ExportHeightMap(RealmMapExportFormat exportFormat)
        {
            SaveFileDialog ofd = new()
            {
                Title = "Export Map",
                DefaultExt = exportFormat.ToString().ToLowerInvariant(),
                RestoreDirectory = true,
                ShowHelp = true,
                Filter = "",
                AddExtension = true,
                CheckPathExists = true,
                ShowHiddenFiles = false,
                ValidateNames = true,
            };

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            foreach (ImageCodecInfo c in codecs)
            {
                if (c.CodecName != null && c.FilenameExtension != null)
                {
                    string codecName = c.CodecName[8..].Replace("Codec", "Files").Trim();
                    ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, codecName, c.FilenameExtension.ToLowerInvariant());
                    sep = "|";
                }
            }

            ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "All Files", "*.*");

            int filterIndex = 0;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].FilenameExtension != null)
                {
                    if (codecs[i].FilenameExtension.Contains(exportFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        filterIndex = i + 1;
                        break;
                    }
                }
            }

#pragma warning restore CS8602 // Dereference of a possibly null reference.

            ofd.FilterIndex = filterIndex;

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = ofd.FileName;
                try
                {
                    ApplicationTimerManager.AutosaveEnabled = false;
                    ApplicationTimerManager.BrushTimerEnabled = false;
                    ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;


                    StopLocationUpdateTimer();


                    LandformMethods.RemoveDeletedLandforms(MapStateMediator.CurrentMap);

                    SKSurface s = SKSurface.Create(new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight));
                    s.Canvas.Clear(SKColors.Black);

                    // render the map as a height map
                    MapHeightMapMethods.RenderHeightMapToCanvas(MapStateMediator.CurrentMap, s.Canvas, new SKPoint(0, 0), null);

                    Bitmap bitmap = s.Snapshot().ToBitmap();

                    bitmap.Save(filename);

                    ApplicationTimerManager.AutosaveEnabled = true;
                    StartLocationUpdateTimer();
                    MessageBox.Show("Height map exported to " + ofd.FileName, "Height Map Exported", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("Failed to export height map to " + ofd.FileName, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        #endregion

        #region Map File Open and Save Methods
        /******************************************************************************************************* 
         * MAP FILE OPEN AND SAVE METHODS
         *******************************************************************************************************/

        private DialogResult SaveMap()
        {
            if (MapStateMediator.CurrentMap.IsSaved)
            {
                return DialogResult.OK;
            }

            if (!string.IsNullOrEmpty(MapStateMediator.CurrentMap.MapPath) && MapStateMediator.CurrentMap.MapName != "Default")
            {
                try
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();
                    LandformMethods.RemoveDeletedLandforms(MapStateMediator.CurrentMap);

                    MapFileMethods.SaveMap(MapStateMediator.CurrentMap);
                    MapStateMediator.CurrentMap.IsSaved = true;

                    if (Settings.Default.PlaySoundOnSave)
                    {
                        UtilityMethods.PlaySaveSound();
                    }

                    SetStatusText("Realm " + Path.GetFileNameWithoutExtension(MapStateMediator.CurrentMap.MapPath) + " has been saved.");
                    return DialogResult.OK;
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("An error has occurred while saving the map. The map file map be corrupted.", "Map Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return DialogResult.Cancel;
                }
            }
            else
            {
                return SaveMapAs();
            }
        }

        private DialogResult SaveMapAs()
        {
            SaveFileDialog sfd = new()
            {
                DefaultExt = "rsmapx",
                CheckWriteAccess = true,
                ExpandedMode = true,
                AddExtension = true,
                SupportMultiDottedExtensions = false,
                AddToRecent = true,
                Filter = "Realm Studio Map|*.rsmapx",
                Title = "Save Map",
            };

            if (!string.IsNullOrEmpty(MapStateMediator.CurrentMap.MapPath))
            {
                sfd.FileName = MapStateMediator.CurrentMap.MapPath;
            }
            else if (!string.IsNullOrEmpty(MapStateMediator.CurrentMap.MapName))
            {
                sfd.FileName = MapStateMediator.CurrentMap.MapName;
            }

            DialogResult result = sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    MapStateMediator.CurrentMap.MapPath = sfd.FileName;
                    MapStateMediator.CurrentMap.MapName = Path.GetFileNameWithoutExtension(sfd.FileName);

                    UpdateMapNameAndSize();

                    try
                    {
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();
                        LandformMethods.RemoveDeletedLandforms(MapStateMediator.CurrentMap);

                        MapFileMethods.SaveMap(MapStateMediator.CurrentMap);
                        MapStateMediator.CurrentMap.IsSaved = true;

                        if (Settings.Default.PlaySoundOnSave)
                        {
                            UtilityMethods.PlaySaveSound();
                        }

                        SetStatusText("Realm " + Path.GetFileNameWithoutExtension(sfd.FileName) + " has been saved.");
                    }
                    catch (Exception ex)
                    {
                        Program.LOGGER.Error(ex);
                        MessageBox.Show("An error has occurred while saving the map. The map file map be corrupted.", "Map Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }

            return result;
        }

        private void OpenExistingMap()
        {
            try
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Open or Create Map",
                    DefaultExt = "rsmapx",
                    Filter = "Realm Studio map files (*.rsmapx)|*.rsmapx|All files (*.*)|*.*",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            OpenMap(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            Program.LOGGER.Error(ex);
                            throw;
                        }


                        UpdateMapNameAndSize();
                        SKGLRenderControl.Invalidate();
                        Refresh();
                    }
                }
            }
            catch { }
        }

        private void OpenMap(string mapFilePath)
        {
            // open an existing map
            try
            {
                Cursor = Cursors.WaitCursor;

                SetStatusText("Loading: " + Path.GetFileName(mapFilePath));

                try
                {
                    MapBuilder.DisposeMap(MapStateMediator.CurrentMap);

                    // make a backup of the map to be opened in case it fails on open
                    RealmMapMethods.SaveRealmFileBackup(mapFilePath);

                    RealmStudioMap? openedMap = MapFileMethods.OpenMap(mapFilePath);

                    if (openedMap != null)
                    {
                        MapStateMediator.CurrentMap = openedMap;

                        // TODO: this should be generalized (when the CurrentMap is assigned,
                        // the ScaleUnitsText should automatically be updated)
                        ScaleUIMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

                        SKImageInfo imageInfo = new(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                        if (MapStateMediator.CurrentMap.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                        {
                            if (MapStateMediator.CurrentMap.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                            {
                                MapBuilder.ConstructMissingLayersForMap(MapStateMediator.CurrentMap, SKGLRenderControl.GRContext);
                            }
                        }

                        foreach (MapLayer ml in MapStateMediator.CurrentMap.MapLayers)
                        {
                            ml.LayerSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, false, imageInfo);
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to open map. Map is null.");
                    }
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    throw;
                }

                MapStateMediator.CurrentMap.IsSaved = true;

                MapRenderHScroll.Maximum = MapStateMediator.CurrentMap.MapWidth;
                MapRenderVScroll.Maximum = MapStateMediator.CurrentMap.MapHeight;

                RealmMapMethods.FinalizeMap(MapStateMediator.CurrentMap, SKGLRenderControl);

                MapStateMediator.CurrentMapGrid = MapGridManager.FinalizeMapGrid(MapStateMediator.CurrentMap);

                if (MapStateMediator.CurrentMapGrid != null)
                {
                    MapStateMediator.CurrentMapGrid.ParentMap = MapStateMediator.CurrentMap;
                    MapGridManager.Update(MapStateMediator.CurrentMap, null, null);
                }

                MapHeightMapMethods.ConvertMapImageToMapHeightMap(MapStateMediator.CurrentMap);

                SetStatusText("Loaded: " + MapStateMediator.CurrentMap.MapName);

                UpdateMapNameAndSize();

                SKGLRenderControl.Invalidate();
            }
            catch
            {
                MessageBox.Show("An error has occurred while opening the map. The map file may be corrupt.", "Error Loading Map", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                MapStateMediator.CurrentMap = MapBuilder.CreateMap("", "DEFAULT", MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT, SKGLRenderControl.GRContext);

                // TODO: generalize
                ScaleUIMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

                BackgroundMethods.RemoveVignette(MapStateMediator.CurrentMap);
                BackgroundMethods.AddVignette(MapStateMediator.CurrentMap, VignetteColorSelectionButton.BackColor.ToArgb(),
                    VignetteStrengthTrack.Value, RectangleVignetteRadio.Checked, SKGLRenderControl);

                MapStateMediator.CurrentMap.IsSaved = false;
                throw;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Form Panel Event Handlers and Methods
        private void PopulateFontPanelUI()
        {
            InstalledFontCollection installedFontCollection = new();

            FontFamilyCombo.DrawItem += FontFamilyCombo_DrawItem;
            FontFamilyCombo.DisplayMember = "Name";

            // add embedded fonts
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.Aladin_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.BarlowCondensed_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.Bilbo_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.CinzelDecorative_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.EastSeaDokdo_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.FrederickatheGreat_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.GentiumBookPlus_Bold);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.IMFellDWPica_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.IMFellEnglish_Italic);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.Katibeh_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.Lancelot_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.MarkoOne_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.Merriweather_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.Metamorphous_Regular);
            LabelManager.AddEmbeddedResourceFont(Properties.Resources.UncialAntiqua_Regular);

            Font aladinFont = new(LabelManager.EMBEDDED_FONTS.Families[0], 12.0F);
            Font barlowCondensedFont = new(LabelManager.EMBEDDED_FONTS.Families[1], 12.0F);
            Font bilboFont = new(LabelManager.EMBEDDED_FONTS.Families[2], 12.0F);
            Font cinzelDecorativeFont = new(LabelManager.EMBEDDED_FONTS.Families[3], 12.0F);
            Font eastSeaDokdoFont = new(LabelManager.EMBEDDED_FONTS.Families[4], 12.0F);
            Font frederickaFont = new(LabelManager.EMBEDDED_FONTS.Families[5], 12.0F);
            Font gentiumBookPlusFont = new(LabelManager.EMBEDDED_FONTS.Families[6], 12.0F);
            Font imFellDWPicaFont = new(LabelManager.EMBEDDED_FONTS.Families[7], 12.0F);
            Font imFellEnglishItalicFont = new(LabelManager.EMBEDDED_FONTS.Families[8], 12.0F);
            Font katibehFont = new(LabelManager.EMBEDDED_FONTS.Families[9], 12.0F);
            Font lancelotFont = new(LabelManager.EMBEDDED_FONTS.Families[10], 12.0F);
            Font markoOneFont = new(LabelManager.EMBEDDED_FONTS.Families[11], 12.0F);
            Font merriweatherFont = new(LabelManager.EMBEDDED_FONTS.Families[12], 12.0F);
            Font metamorphousFont = new(LabelManager.EMBEDDED_FONTS.Families[13], 12.0F);
            Font uncialAntiquaFont = new(LabelManager.EMBEDDED_FONTS.Families[14], 12.0F);

            FontFamilyCombo.Items.Add(aladinFont);
            FontFamilyCombo.Items.Add(barlowCondensedFont);
            FontFamilyCombo.Items.Add(bilboFont);
            FontFamilyCombo.Items.Add(cinzelDecorativeFont);
            FontFamilyCombo.Items.Add(eastSeaDokdoFont);
            FontFamilyCombo.Items.Add(frederickaFont);
            FontFamilyCombo.Items.Add(gentiumBookPlusFont);
            FontFamilyCombo.Items.Add(imFellDWPicaFont);
            FontFamilyCombo.Items.Add(imFellEnglishItalicFont);
            FontFamilyCombo.Items.Add(katibehFont);
            FontFamilyCombo.Items.Add(lancelotFont);
            FontFamilyCombo.Items.Add(markoOneFont);
            FontFamilyCombo.Items.Add(merriweatherFont);
            FontFamilyCombo.Items.Add(metamorphousFont);
            FontFamilyCombo.Items.Add(uncialAntiquaFont);

            // Get the array of FontFamily objects.
            foreach (FontFamily t in installedFontCollection.Families.Where(t => t.IsStyleAvailable(FontStyle.Regular) && t.IsStyleAvailable(FontStyle.Underline) && t.IsStyleAvailable(FontStyle.Italic) && t.IsStyleAvailable(FontStyle.Bold)))
            {
                bool allowedName = t.Name.All((c => Char.IsLetterOrDigit(c) || c == '_' || c == ' '));
                if (allowedName && t.Name != "Sans Serif Collection")
                {
                    // limit the list of fonts that can be used to those that Skia can render
                    string fontTypefaceFamily = SKTypeface.FromFamilyName(t.Name).FamilyName;

                    if (fontTypefaceFamily == t.Name)
                    {
                        Font f = new(t, 12, FontStyle.Regular, GraphicsUnit.Point);
                        FontFamilyCombo.Items.Add(f);
                    }
                }
            }


            int fontIndex = 0;

            fontIndex = FontFamilyCombo.Items.IndexOf(FontPanelManager.FontPanelSelectedFont.SelectedFont);

            if (FontFamilyCombo.Items != null && fontIndex < 0)
            {
                // find by family name
                for (int i = 0; i < FontFamilyCombo.Items?.Count; i++)
                {
                    Font? f = FontFamilyCombo.Items[i] as Font;

                    string? fontFamilyName = f?.FontFamily.Name;

                    if (!string.IsNullOrEmpty(fontFamilyName) && fontFamilyName == FontPanelManager.FontPanelSelectedFont.SelectedFont.FontFamily.Name)
                    {
                        fontIndex = i;
                        break;
                    }
                }
            }

            FontFamilyCombo.SelectedIndex = (fontIndex >= 0) ? fontIndex : 0;

            FontSizeCombo.SelectedIndex = 7; // 12 points

            SetFont();
            SetExampleText();
        }

        private void SetFont()
        {
            if (FontFamilyCombo.Items == null || FontFamilyCombo.Items.Count <= 0) return;

            Font? selectedFont = (Font?)FontFamilyCombo.Items[FontFamilyCombo.SelectedIndex];

            if (selectedFont != null)
            {
                FontFamily ff = selectedFont.FontFamily;

                if (FontSizeCombo.SelectedIndex >= 0)
                {
                    string? selectedFontSize = (string?)FontSizeCombo.Items[FontSizeCombo.SelectedIndex];

                    if (float.TryParse(selectedFontSize, out float fontSize))
                    {
                        fontSize *= 1.33F;

                        FontStyle fs = FontStyle.Regular;

                        if (FontPanelManager.FontPanelSelectedFont.IsBold)
                        {
                            fs |= FontStyle.Bold;
                        }

                        if (FontPanelManager.FontPanelSelectedFont.IsItalic)
                        {
                            fs |= FontStyle.Italic;
                        }

                        try
                        {
                            FontPanelManager.FontPanelSelectedFont.SelectedFont = new Font(ff, fontSize, fs, GraphicsUnit.Point);
                        }
                        catch { }
                    }
                }
            }
        }

        private void SetExampleText()
        {
            Font textFont = new(FontPanelManager.FontPanelSelectedFont.SelectedFont.FontFamily,
                FontPanelManager.FontPanelSelectedFont.SelectedFont.Size * 0.75F, FontPanelManager.FontPanelSelectedFont.SelectedFont.Style, GraphicsUnit.Point);

            ExampleTextLabel.Font = textFont;
            ExampleTextLabel.Text = "The quick brown fox";
        }

        private void FontFamilyCombo_DrawItem(object? sender, DrawItemEventArgs e)
        {
            ComboBox? comboBox = (ComboBox?)sender;

            if (comboBox != null)
            {
                Font? font = (Font?)comboBox.Items[e.Index];

                if (font != null)
                {
                    e.DrawBackground();

                    PanoseFontFamilyTypes familyType = LabelManager.PanoseFontFamilyType(e.Graphics, font);

                    // Marlett is a special case of a font family that does not render as text, but its PanoseFontFamily indicates it does
                    if (familyType == PanoseFontFamilyTypes.PanFamilyDecorative
                        || familyType == PanoseFontFamilyTypes.PanFamilyPictorial
                        || familyType == PanoseFontFamilyTypes.PanNoFit
                        || font.Name == "Marlett")
                    {
                        Font f = new("Segoe UI", 12, FontStyle.Regular, GraphicsUnit.Point);
                        e.Graphics.DrawString(font.Name, f, Brushes.Black, e.Bounds.X, e.Bounds.Y, StringFormat.GenericTypographic);

                    }
                    else
                    {
                        e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds.X, e.Bounds.Y, StringFormat.GenericTypographic);
                    }
                }
            }
        }

        private void FontFamilyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FontFamilyCombo.SelectedIndex >= 0)
            {
                Font? selectedFont = (Font?)FontFamilyCombo.Items[FontFamilyCombo.SelectedIndex];

                if (selectedFont != null)
                {
                    FontFamilyCombo.Text = selectedFont.Name;

                    SetFont();
                    SetExampleText();
                }
            }
        }

        private void FontSizeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFont();
            SetExampleText();
        }

        private void IncreaseFontSizeButton_Click(object sender, EventArgs e)
        {
            int sizeIndex = FontSizeCombo.SelectedIndex;

            if (sizeIndex < FontSizeCombo.Items.Count - 1)
            {
                sizeIndex++;
                FontSizeCombo.SelectedIndex = sizeIndex;
            }
        }

        private void DecreaseFontSizeButton_Click(object sender, EventArgs e)
        {
            int sizeIndex = FontSizeCombo.SelectedIndex;

            if (sizeIndex > 0)
            {
                sizeIndex--;
                FontSizeCombo.SelectedIndex = sizeIndex;
            }
        }

        private void SetBoldFontButton_Click(object sender, EventArgs e)
        {
            FontPanelManager.FontPanelSelectedFont.IsBold = !FontPanelManager.FontPanelSelectedFont.IsBold;

            if (FontPanelManager.FontPanelSelectedFont.IsBold)
            {
                SetBoldFontButton.BackColor = ColorTranslator.FromHtml("#D2F1C1");
            }
            else
            {
                SetBoldFontButton.BackColor = Color.FromArgb(223, 219, 210);
            }

            SetFont();
            SetExampleText();
        }

        private void SetItalicFontButton_Click(object sender, EventArgs e)
        {
            FontPanelManager.FontPanelSelectedFont.IsItalic = !FontPanelManager.FontPanelSelectedFont.IsItalic;

            if (FontPanelManager.FontPanelSelectedFont.IsItalic)
            {
                SetItalicFontButton.BackColor = ColorTranslator.FromHtml("#D2F1C1");
            }
            else
            {
                SetItalicFontButton.BackColor = Color.FromArgb(223, 219, 210);
            }

            SetFont();
            SetExampleText();
        }

        private void FontPanelOKButton_Click(object sender, EventArgs e)
        {
            switch (FontPanelManager.FontPanelOpener)
            {
                case FontPanelOpener.ScaleFontButton:
                    {
                        ScaleUIMediator.ScaleFont = FontPanelManager.FontPanelSelectedFont.SelectedFont;
                    }
                    break;
                case FontPanelOpener.LabelFontButton:
                    {
                        SelectLabelFontButton.Font = new Font(FontPanelManager.FontPanelSelectedFont.SelectedFont.FontFamily, 12);

                        SelectLabelFontButton.Refresh();

                        LabelMediator.SelectedLabelFont = FontPanelManager.FontPanelSelectedFont.SelectedFont;

                        if (MapStateMediator.SelectedMapLabel != null)
                        {
                            Font newFont = new(FontPanelManager.FontPanelSelectedFont.SelectedFont.FontFamily, FontPanelManager.FontPanelSelectedFont.SelectedFont.Size * 0.75F, FontPanelManager.FontPanelSelectedFont.SelectedFont.Style, GraphicsUnit.Point);

                            Color labelColor = FontColorSelectButton.BackColor;
                            Color outlineColor = OutlineColorSelectButton.BackColor;
                            float outlineWidth = OutlineWidthTrack.Value / 10F;
                            Color glowColor = GlowColorSelectButton.BackColor;
                            int glowStrength = GlowStrengthTrack.Value;

                            Cmd_ChangeLabelAttributes cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapLabel, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, newFont);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }

            FontSelectionPanel.Visible = false;
        }

        private void FontPanelCloseButton_Click(object sender, EventArgs e)
        {
            FontSelectionPanel.Visible = false;
        }

        #endregion

        #region Theme Methods

        private MapTheme SaveCurentSettingsToTheme()
        {
            MapTheme theme = new()
            {
                // label presets for the theme are serialized to a file at the time they are created
                // and loaded when the theme is loaded/selected; they are not stored with the theme

                // background
                BackgroundTexture = BackgroundMethods.GetSelectedMapTexture(),
                BackgroundTextureScale = BackgroundTextureScaleTrack.Value,
                MirrorBackgroundTexture = MirrorBackgroundSwitch.Checked,

                // vignette color and strength
                VignetteColor = VignetteColorSelectionButton.BackColor.ToArgb(),
                VignetteStrength = VignetteStrengthTrack.Value,
                RectangleVignette = RectangleVignetteRadio.Checked,

                // ocean
                OceanTexture = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX],

                OceanTextureOpacity = OceanTextureOpacityTrack.Value,
                OceanTextureScale = OceanScaleTextureTrack.Value,
                MirrorOceanTexture = MirrorOceanTextureSwitch.Checked,
                OceanColor = OceanColorSelectButton.BackColor.ToArgb()
            };

            // save ocean custom colors
            theme.OceanColorPalette?.Add(OceanCustomColorButton1.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton2.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton3.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton4.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton5.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton6.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton7.BackColor.ToArgb());
            theme.OceanColorPalette?.Add(OceanCustomColorButton8.BackColor.ToArgb());

            // landform
            theme.LandformOutlineColor = LandformOutlineColorSelectButton.BackColor.ToArgb();
            theme.LandformOutlineWidth = LandformOutlineWidthTrack.Value;

            theme.LandformBackgroundColor = LandformBackgroundColorSelectButton.BackColor.ToArgb();
            theme.FillLandformWithTexture = UseTextureForBackgroundCheck.Checked;

            theme.LandformTexture = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX];

            theme.LandShorelineStyle = LandGradientDirection.None.ToString();   // light-to-dark shading vs. dark-to-light shading; not used now

            theme.LandformCoastlineColor = CoastlineColorSelectionButton.BackColor.ToArgb();
            theme.LandformCoastlineEffectDistance = CoastlineEffectDistanceTrack.Value;

            if (CoastlineStyleList.SelectedIndex > -1)
            {
                theme.LandformCoastlineStyle = (string)CoastlineStyleList.Items[CoastlineStyleList.SelectedIndex];
            }

            // save land custom colors
            theme.LandformColorPalette?.Add(LandCustomColorButton1.BackColor.ToArgb());
            theme.LandformColorPalette?.Add(LandCustomColorButton2.BackColor.ToArgb());
            theme.LandformColorPalette?.Add(LandCustomColorButton3.BackColor.ToArgb());
            theme.LandformColorPalette?.Add(LandCustomColorButton4.BackColor.ToArgb());
            theme.LandformColorPalette?.Add(LandCustomColorButton5.BackColor.ToArgb());
            theme.LandformColorPalette?.Add(LandCustomColorButton6.BackColor.ToArgb());

            // freshwater
            theme.FreshwaterColor = WaterColorSelectionButton.BackColor.ToArgb();
            theme.FreshwaterShorelineColor = ShorelineColorSelectionButton.BackColor.ToArgb();

            theme.RiverWidth = RiverWidthTrack.Value;
            theme.RiverSourceFadeIn = RiverSourceFadeInSwitch.Checked;

            // save freshwater custom colors
            theme.FreshwaterColorPalette?.Add(WaterCustomColor1.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor2.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor3.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor4.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor5.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor6.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor7.BackColor.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterCustomColor8.BackColor.ToArgb());

            // path
            theme.PathColor = PathColorSelectButton.BackColor.ToArgb();
            theme.PathWidth = PathWidthTrack.Value;
            theme.PathStyle = GetSelectedPathType();

            // label
            FontConverter cvt = new();
            string? fontString = cvt.ConvertToString(LabelMediator.SelectedLabelFont);
            theme.LabelFont = fontString ?? string.Empty;
            theme.LabelColor = FontColorSelectButton.BackColor.ToArgb();
            theme.LabelOutlineColor = OutlineColorSelectButton.BackColor.ToArgb();
            theme.LabelOutlineWidth = OutlineWidthTrack.Value;
            theme.LabelGlowColor = GlowColorSelectButton.BackColor.ToArgb();
            theme.LabelGlowStrength = GlowStrengthTrack.Value;

            // symbols
            if (theme.SymbolCustomColors != null)
            {
                theme.SymbolCustomColors[0] = SymbolColor1Button.BackColor.ToArgb();
                theme.SymbolCustomColors[1] = SymbolColor2Button.BackColor.ToArgb();
                theme.SymbolCustomColors[2] = SymbolColor3Button.BackColor.ToArgb();
            }

            return theme;
        }


        private void ApplyTheme(MapTheme theme, ThemeFilter themeFilter)
        {
            if (theme == null || themeFilter == null) return;

            try
            {
                if (themeFilter.ApplyBackgroundSettings)
                {
                    if (theme.BackgroundTexture != null)
                    {
                        for (int i = 0; i < AssetManager.BACKGROUND_TEXTURE_LIST.Count; i++)
                        {
                            if (AssetManager.BACKGROUND_TEXTURE_LIST[i].TextureName == theme.BackgroundTexture.TextureName)
                            {
                                AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX = i;
                                break;
                            }
                        }

                        if (AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap == null)
                        {
                            AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap
                                = (Bitmap?)Bitmap.FromFile(AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TexturePath);
                        }

                        MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);
                        baseLayer.MapLayerComponents.Clear();

                        float scale = BackgroundTextureScaleTrack.Value / 100F;
                        bool mirrorBackground = MirrorBackgroundSwitch.Checked;

                        BackgroundMethods.ClearBackgroundImage(MapStateMediator.CurrentMap);

                        if (scale > 0.0F)
                        {
                            BackgroundMethods.ApplyBackgroundTexture(MapStateMediator.CurrentMap, BackgroundMethods.GetSelectedBackgroundImage(), scale, mirrorBackground);
                        }

                        // background texture
                        BackgroundTextureBox.Image = BackgroundMethods.GetSelectedBackgroundImage();
                        BackgroundTextureNameLabel.Text = BackgroundMethods.GetCurrentImageName();

                        BackgroundTextureBox.Refresh();
                        BackgroundTextureNameLabel.Refresh();
                    }

                    BackgroundTextureScaleTrack.Value = (int)((theme.BackgroundTextureScale != null) ? theme.BackgroundTextureScale : 100);
                    BackgroundTextureScaleTrack.Refresh();

                    MirrorBackgroundSwitch.Checked = (bool)((theme.MirrorBackgroundTexture != null) ? theme.MirrorBackgroundTexture : false);
                    MirrorBackgroundSwitch.Refresh();

                    VignetteColorSelectionButton.BackColor = Color.FromArgb(theme.VignetteColor ?? Color.FromArgb(201, 151, 123).ToArgb());
                    VignetteColorSelectionButton.Refresh();

                    VignetteStrengthTrack.Value = (int)((theme.VignetteStrength != null) ? theme.VignetteStrength : 148);
                    VignetteStrengthTrack.Refresh();

                    RectangleVignetteRadio.Checked = (bool)((theme.RectangleVignette != null) ? theme.RectangleVignette : false);
                    OvalVignetteRadio.Checked = (bool)((theme.RectangleVignette != null) ? !theme.RectangleVignette : true);

                    for (int i = 0; i < MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
                    {
                        if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                        {
                            v.VignetteColor = VignetteColorSelectionButton.BackColor.ToArgb();
                            v.VignetteStrength = VignetteStrengthTrack.Value;
                            v.RectangleVignette = RectangleVignetteRadio.Checked;
                        }
                    }
                }

                if (themeFilter.ApplyOceanSettings)
                {
                    if (AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap == null)
                    {
                        AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.WATER_TEXTURE_LIST.First().TexturePath);
                    }

                    OceanTextureBox.Image = AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap;
                    OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST.First().TextureName;

                    // 0 to 100% (100% is 100% opaque)
                    OceanTextureOpacityTrack.Value = (int)((theme.OceanTextureOpacity != null) ? theme.OceanTextureOpacity : 100);
                    OceanTextureOpacityTrack.Refresh();

                    OceanScaleTextureTrack.Value = (int)((theme.OceanTextureScale != null) ? theme.OceanTextureScale : 100);
                    OceanScaleTextureTrack.Refresh();

                    MirrorOceanTextureSwitch.Checked = theme.MirrorOceanTexture ?? false;
                    MirrorOceanTextureSwitch.Refresh();

                    OceanColorSelectButton.BackColor = Color.FromArgb(theme.OceanColor ?? Color.FromKnownColor(KnownColor.ControlLight).ToArgb());
                    OceanColorSelectButton.Refresh();

                    MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);
                    oceanTextureLayer.MapLayerComponents.Clear();

                    MapLayer oceanTextureOverLayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
                    oceanTextureOverLayLayer.MapLayerComponents.Clear();

                    if (theme.OceanTexture != null)
                    {
                        for (int i = 0; i < AssetManager.WATER_TEXTURE_LIST.Count; i++)
                        {
                            if (AssetManager.WATER_TEXTURE_LIST[i].TextureName == theme.OceanTexture.TextureName)
                            {
                                AssetManager.SELECTED_OCEAN_TEXTURE_INDEX = i;
                                break;
                            }
                        }

                        if (AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap == null)
                        {
                            AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap
                                = (Bitmap?)Bitmap.FromFile(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);
                        }

                        Bitmap? b = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap;

                        if (b != null)
                        {
                            Bitmap newB = DrawingMethods.SetBitmapOpacity(b, OceanTextureOpacityTrack.Value / 100F);

                            OceanTextureBox.Image = newB;
                            OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureName;

                            float scale = OceanScaleTextureTrack.Value / 100.0F;
                            bool mirrorOceanTexture = MirrorOceanTextureSwitch.Checked;

                            if (scale > 0.0F)
                            {
                                OceanMethods.ApplyOceanTexture(MapStateMediator.CurrentMap, (Bitmap)OceanTextureBox.Image, scale, mirrorOceanTexture);
                            }
                        }
                    }

                    OceanTextureBox.Refresh();

                    Color fillColor = OceanColorSelectButton.BackColor;

                    if (fillColor.ToArgb() != Color.White.ToArgb())
                    {
                        SKBitmap b = new(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                        using (SKCanvas canvas = new(b))
                        {
                            canvas.Clear(Extensions.ToSKColor(fillColor));
                        }

                        Cmd_SetOceanColor cmd = new(MapStateMediator.CurrentMap, b);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();
                    }
                }

                if (themeFilter.ApplyOceanColorPaletteSettings)
                {
                    if (theme.OceanColorPalette?.Count > 0)
                    {
                        OceanCustomColorButton1.BackColor = Color.FromArgb(theme.OceanColorPalette[0] ?? Color.White.ToArgb());
                        OceanCustomColorButton1.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton1.Text = ColorTranslator.ToHtml(OceanCustomColorButton1.BackColor);

                        OceanCustomColorButton1.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 1)
                    {
                        OceanCustomColorButton2.BackColor = Color.FromArgb(theme.OceanColorPalette[1] ?? Color.White.ToArgb());
                        OceanCustomColorButton2.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton2.Text = ColorTranslator.ToHtml(OceanCustomColorButton2.BackColor);

                        OceanCustomColorButton2.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 2)
                    {
                        OceanCustomColorButton3.BackColor = Color.FromArgb(theme.OceanColorPalette[2] ?? Color.White.ToArgb());
                        OceanCustomColorButton3.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton3.Text = ColorTranslator.ToHtml(OceanCustomColorButton3.BackColor);

                        OceanCustomColorButton3.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 3)
                    {
                        OceanCustomColorButton4.BackColor = Color.FromArgb(theme.OceanColorPalette[3] ?? Color.White.ToArgb());
                        OceanCustomColorButton4.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton4.Text = ColorTranslator.ToHtml(OceanCustomColorButton4.BackColor);

                        OceanCustomColorButton4.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 4)
                    {
                        OceanCustomColorButton5.BackColor = Color.FromArgb(theme.OceanColorPalette[4] ?? Color.White.ToArgb());
                        OceanCustomColorButton5.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton5.Text = ColorTranslator.ToHtml(OceanCustomColorButton5.BackColor);

                        OceanCustomColorButton5.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 5)
                    {
                        OceanCustomColorButton6.BackColor = Color.FromArgb(theme.OceanColorPalette[5] ?? Color.White.ToArgb());
                        OceanCustomColorButton6.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton6.Text = ColorTranslator.ToHtml(OceanCustomColorButton6.BackColor);

                        OceanCustomColorButton6.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 6)
                    {
                        OceanCustomColorButton7.BackColor = Color.FromArgb(theme.OceanColorPalette[6] ?? Color.White.ToArgb());
                        OceanCustomColorButton7.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton7.Text = ColorTranslator.ToHtml(OceanCustomColorButton7.BackColor);

                        OceanCustomColorButton7.Refresh();
                    }

                    if (theme.OceanColorPalette?.Count > 7)
                    {
                        OceanCustomColorButton8.BackColor = Color.FromArgb(theme.OceanColorPalette[7] ?? Color.White.ToArgb());
                        OceanCustomColorButton8.ForeColor = SystemColors.ControlDark;
                        OceanCustomColorButton8.Text = ColorTranslator.ToHtml(OceanCustomColorButton8.BackColor);

                        OceanCustomColorButton8.Refresh();
                    }

                    if (OceanCustomColorButton1.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton2.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton3.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton4.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton5.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton6.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton7.BackColor.ToArgb() == Color.White.ToArgb()
                        && OceanCustomColorButton8.BackColor.ToArgb() == Color.White.ToArgb())
                    {
                        OceanCustomColorButton1.Text = "";
                        OceanCustomColorButton2.Text = "";
                        OceanCustomColorButton3.Text = "";
                        OceanCustomColorButton4.Text = "";
                        OceanCustomColorButton5.Text = "";
                        OceanCustomColorButton6.Text = "";
                        OceanCustomColorButton7.Text = "";
                        OceanCustomColorButton8.Text = "";
                    }
                }

                if (themeFilter.ApplyLandSettings)
                {

                    LandformOutlineColorSelectButton.BackColor = Color.FromArgb(theme.LandformOutlineColor ?? Color.FromArgb(62, 55, 40).ToArgb());
                    LandformOutlineColorSelectButton.Refresh();

                    if (AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap == null)
                    {
                        AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST.First().TexturePath);
                    }

                    LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap;
                    LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST.First().TextureName;

                    if (theme.LandformTexture != null)
                    {
                        for (int i = 0; i < AssetManager.LAND_TEXTURE_LIST.Count; i++)
                        {
                            if (AssetManager.LAND_TEXTURE_LIST[i].TextureName == theme.LandformTexture.TextureName)
                            {
                                AssetManager.SELECTED_LAND_TEXTURE_INDEX = i;
                                break;
                            }
                        }

                        if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
                        {
                            AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap
                                = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
                        }

                        LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap;
                        LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureName;
                    }

                    LandformTexturePreviewPicture.Refresh();

                    UseTextureForBackgroundCheck.Checked = (bool)((theme.FillLandformWithTexture != null) ? theme.FillLandformWithTexture : true);

                    CoastlineColorSelectionButton.BackColor = Color.FromArgb(theme.LandformCoastlineColor ?? Color.FromArgb(187, 156, 195, 183).ToArgb());
                    CoastlineColorSelectionButton.Refresh();

                    CoastlineEffectDistanceTrack.Value = theme.LandformCoastlineEffectDistance ?? 16;
                    CoastlineEffectDistanceTrack.Refresh();

                    if (theme.LandformCoastlineStyle != null)
                    {
                        for (int i = 0; i < CoastlineStyleList.Items.Count; i++)
                        {
                            if (CoastlineStyleList.Items[i].ToString() == theme.LandformCoastlineStyle)
                            {
                                CoastlineStyleList.SelectedIndex = i;
                                break;
                            }

                            CoastlineStyleList.Refresh();
                        }
                    }
                }

                if (themeFilter.ApplyLandformColorPaletteSettings)
                {
                    if (theme.LandformColorPalette?.Count > 0)
                    {
                        LandCustomColorButton1.BackColor = Color.FromArgb(theme.LandformColorPalette[0] ?? Color.White.ToArgb());
                        LandCustomColorButton1.ForeColor = SystemColors.ControlDark;
                        LandCustomColorButton1.Text = ColorTranslator.ToHtml(LandCustomColorButton1.BackColor);

                        LandCustomColorButton1.Refresh();
                    }

                    if (theme.LandformColorPalette?.Count > 1)
                    {
                        LandCustomColorButton2.BackColor = Color.FromArgb(theme.LandformColorPalette[1] ?? Color.White.ToArgb());
                        LandCustomColorButton2.ForeColor = SystemColors.ControlDark;
                        LandCustomColorButton2.Text = ColorTranslator.ToHtml(LandCustomColorButton2.BackColor);

                        LandCustomColorButton2.Refresh();
                    }

                    if (theme.LandformColorPalette?.Count > 2)
                    {
                        LandCustomColorButton3.BackColor = Color.FromArgb(theme.LandformColorPalette[2] ?? Color.White.ToArgb());
                        LandCustomColorButton3.ForeColor = SystemColors.ControlDark;
                        LandCustomColorButton3.Text = ColorTranslator.ToHtml(LandCustomColorButton3.BackColor);

                        LandCustomColorButton3.Refresh();
                    }

                    if (theme.LandformColorPalette?.Count > 3)
                    {
                        LandCustomColorButton4.BackColor = Color.FromArgb(theme.LandformColorPalette[3] ?? Color.White.ToArgb());
                        LandCustomColorButton4.ForeColor = SystemColors.ControlDark;
                        LandCustomColorButton4.Text = ColorTranslator.ToHtml(LandCustomColorButton4.BackColor);

                        LandCustomColorButton4.Refresh();
                    }

                    if (theme.LandformColorPalette?.Count > 4)
                    {
                        LandCustomColorButton5.BackColor = Color.FromArgb(theme.LandformColorPalette[4] ?? Color.White.ToArgb());
                        LandCustomColorButton5.ForeColor = SystemColors.ControlDark;
                        LandCustomColorButton5.Text = ColorTranslator.ToHtml(LandCustomColorButton5.BackColor);

                        LandCustomColorButton5.Refresh();
                    }

                    if (theme.LandformColorPalette?.Count > 5)
                    {
                        LandCustomColorButton6.BackColor = Color.FromArgb(theme.LandformColorPalette[5] ?? Color.White.ToArgb());
                        LandCustomColorButton6.ForeColor = SystemColors.ControlDark;
                        LandCustomColorButton6.Text = ColorTranslator.ToHtml(LandCustomColorButton6.BackColor);

                        LandCustomColorButton6.Refresh();
                    }

                    if (LandCustomColorButton1.BackColor.ToArgb() == Color.White.ToArgb()
                        && LandCustomColorButton2.BackColor.ToArgb() == Color.White.ToArgb()
                        && LandCustomColorButton3.BackColor.ToArgb() == Color.White.ToArgb()
                        && LandCustomColorButton4.BackColor.ToArgb() == Color.White.ToArgb()
                        && LandCustomColorButton5.BackColor.ToArgb() == Color.White.ToArgb()
                        && LandCustomColorButton6.BackColor.ToArgb() == Color.White.ToArgb())
                    {
                        LandCustomColorButton1.Text = "";
                        LandCustomColorButton2.Text = "";
                        LandCustomColorButton3.Text = "";
                        LandCustomColorButton4.Text = "";
                        LandCustomColorButton5.Text = "";
                        LandCustomColorButton6.Text = "";
                    }
                }

                if (themeFilter.ApplyFreshwaterSettings)
                {
                    if (theme.FreshwaterColor != Color.Empty.ToArgb())
                    {
                        WaterColorSelectionButton.BackColor = Color.FromArgb(theme.FreshwaterColor ?? Color.FromArgb(101, 140, 191, 197).ToArgb());
                        WaterColorSelectionButton.Refresh();
                    }
                    else
                    {
                        WaterColorSelectionButton.BackColor = WaterFeatureMethods.DEFAULT_WATER_COLOR;
                        WaterColorSelectionButton.Refresh();
                    }

                    if (theme.FreshwaterShorelineColor != Color.Empty.ToArgb())
                    {
                        ShorelineColorSelectionButton.BackColor = Color.FromArgb(theme.FreshwaterShorelineColor ?? Color.FromArgb(161, 144, 118).ToArgb());
                        ShorelineColorSelectionButton.Refresh();
                    }
                    else
                    {
                        ShorelineColorSelectionButton.BackColor = WaterFeatureMethods.DEFAULT_WATER_OUTLINE_COLOR;
                        ShorelineColorSelectionButton.Refresh();
                    }

                    RiverWidthTrack.Value = theme.RiverWidth ?? 4;
                    RiverWidthTrack.Refresh();

                    RiverSourceFadeInSwitch.Checked = theme.RiverSourceFadeIn ?? true;
                }
                else
                {
                    ShorelineColorSelectionButton.BackColor = WaterFeatureMethods.DEFAULT_WATER_OUTLINE_COLOR;
                    ShorelineColorSelectionButton.Refresh();

                    WaterColorSelectionButton.BackColor = WaterFeatureMethods.DEFAULT_WATER_COLOR;
                    WaterColorSelectionButton.Refresh();
                }

                if (themeFilter.ApplyFreshwaterColorPaletteSettings)
                {
                    if (theme.FreshwaterColorPalette?.Count > 0)
                    {
                        WaterCustomColor1.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[0] ?? Color.White.ToArgb());
                        WaterCustomColor1.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor1.Text = ColorTranslator.ToHtml(WaterCustomColor1.BackColor);

                        WaterCustomColor1.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 1)
                    {
                        WaterCustomColor2.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[1] ?? Color.White.ToArgb());
                        WaterCustomColor2.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor2.Text = ColorTranslator.ToHtml(WaterCustomColor2.BackColor);

                        WaterCustomColor2.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 2)
                    {
                        WaterCustomColor3.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[2] ?? Color.White.ToArgb());
                        WaterCustomColor3.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor3.Text = ColorTranslator.ToHtml(WaterCustomColor3.BackColor);

                        WaterCustomColor3.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 3)
                    {
                        WaterCustomColor4.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[3] ?? Color.White.ToArgb());
                        WaterCustomColor4.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor4.Text = ColorTranslator.ToHtml(WaterCustomColor4.BackColor);

                        WaterCustomColor4.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 4)
                    {
                        WaterCustomColor5.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[4] ?? Color.White.ToArgb());
                        WaterCustomColor5.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor5.Text = ColorTranslator.ToHtml(WaterCustomColor5.BackColor);

                        WaterCustomColor5.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 5)
                    {
                        WaterCustomColor6.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[5] ?? Color.White.ToArgb());
                        WaterCustomColor6.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor6.Text = ColorTranslator.ToHtml(WaterCustomColor6.BackColor);

                        WaterCustomColor6.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 6)
                    {
                        WaterCustomColor7.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[6] ?? Color.White.ToArgb());
                        WaterCustomColor7.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor7.Text = ColorTranslator.ToHtml(WaterCustomColor7.BackColor);

                        WaterCustomColor7.Refresh();
                    }

                    if (theme.FreshwaterColorPalette?.Count > 7)
                    {
                        WaterCustomColor8.BackColor = Color.FromArgb(theme.FreshwaterColorPalette[7] ?? Color.White.ToArgb());
                        WaterCustomColor8.ForeColor = SystemColors.ControlDark;
                        WaterCustomColor8.Text = ColorTranslator.ToHtml(WaterCustomColor8.BackColor);

                        WaterCustomColor8.Refresh();
                    }

                    if (WaterCustomColor1.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor2.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor3.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor4.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor5.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor6.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor7.BackColor.ToArgb() == Color.White.ToArgb()
                        && WaterCustomColor8.BackColor.ToArgb() == Color.White.ToArgb())
                    {
                        WaterCustomColor1.Text = "";
                        WaterCustomColor2.Text = "";
                        WaterCustomColor3.Text = "";
                        WaterCustomColor4.Text = "";
                        WaterCustomColor5.Text = "";
                        WaterCustomColor6.Text = "";
                        WaterCustomColor7.Text = "";
                        WaterCustomColor8.Text = "";
                    }
                }

                if (themeFilter.ApplyPathSetSettings)
                {
                    PathColorSelectButton.BackColor = Color.FromArgb(theme.PathColor ?? Color.FromArgb(75, 49, 26).ToArgb());
                    PathColorSelectButton.Refresh();

                    PathWidthTrack.Value = theme.PathWidth ?? 8;
                    PathWidthTrack.Refresh();

                    SetSelectedPathType(theme.PathStyle ?? PathType.SolidLinePath);
                }

                if (themeFilter.ApplySymbolSettings && theme.SymbolCustomColors != null)
                {
                    SymbolColor1Button.BackColor = Color.FromArgb(theme.SymbolCustomColors[0] ?? Color.White.ToArgb());
                    SymbolColor1Button.Refresh();

                    SymbolColor2Button.BackColor = Color.FromArgb(theme.SymbolCustomColors[1] ?? Color.White.ToArgb());
                    SymbolColor2Button.Refresh();

                    SymbolColor3Button.BackColor = Color.FromArgb(theme.SymbolCustomColors[2] ?? Color.White.ToArgb());
                    SymbolColor3Button.Refresh();

                }

                if (themeFilter.ApplyLabelPresetSettings)
                {
                    LabelPresetManager.PresetMediator?.AddLabelPresets();
                }

                if (themeFilter.ApplyLabelSettings)
                {

                    if (!string.IsNullOrEmpty(theme.LabelFont))
                    {
                        Font? themeFont = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);

                        FontConverter cvt = new();
                        themeFont = (Font?)cvt.ConvertFromString(theme.LabelFont);

                        if (themeFont != null && !theme.LabelFont.Contains(themeFont.FontFamily.Name))
                        {
                            themeFont = null;
                        }

                        if (themeFont == null)
                        {
                            string[] fontParts = theme.LabelFont.Split(',');

                            if (fontParts.Length == 2)
                            {
                                string ff = fontParts[0];
                                string fs = fontParts[1];

                                // remove any non-numeric characters from the font size string (but allow . and -)
                                fs = string.Join(",", new string(
                                    [.. fs.Where(c => char.IsBetween(c, '0', '9') || c == '.' || c == '-' || char.IsWhiteSpace(c))]).Split((char[]?)null,
                                    StringSplitOptions.RemoveEmptyEntries));

                                bool success = float.TryParse(fs, out float fontsize);

                                if (!success)
                                {
                                    fontsize = 12.0F;
                                }

                                try
                                {
                                    FontFamily family = new(ff);
                                    themeFont = new Font(family, fontsize, FontStyle.Regular, GraphicsUnit.Point);
                                }
                                catch
                                {
                                    // couldn't create the font, so try to find it in the list of embedded fonts
                                    for (int i = 0; i < LabelManager.EMBEDDED_FONTS.Families.Length; i++)
                                    {
                                        if (LabelManager.EMBEDDED_FONTS.Families[i].Name == ff)
                                        {
                                            themeFont = new Font(LabelManager.EMBEDDED_FONTS.Families[i], fontsize, FontStyle.Regular, GraphicsUnit.Point);
                                        }
                                    }
                                }
                            }
                        }

                        if (themeFont != null)
                        {
                            FontPanelManager.FontPanelSelectedFont.SelectedFont = new Font(themeFont.FontFamily, 12);
                            FontPanelManager.FontPanelSelectedFont.FontSize = themeFont.SizeInPoints;
                            LabelMediator.SelectedLabelFont = themeFont;
                            SelectLabelFontButton.Font = new Font(themeFont.FontFamily, 14);
                            SelectLabelFontButton.Refresh();
                        }
                    }

                    LabelMediator.LabelColor = Color.FromArgb(theme.LabelColor ?? Color.FromArgb(61, 53, 30).ToArgb());
                    LabelMediator.OutlineColor = Color.FromArgb(theme.LabelOutlineColor ?? Color.FromArgb(161, 214, 202, 171).ToArgb());
                    LabelMediator.OutlineWidth = (int?)theme.LabelOutlineWidth ?? 0;
                    LabelMediator.GlowColor = Color.FromArgb(theme.LabelGlowColor ?? Color.White.ToArgb());
                    LabelMediator.GlowStrength = theme.LabelGlowStrength ?? 0;

                }
            }
            catch
            {
                // error loading the theme
            }
        }

        #endregion

        #region ScrollBar Event Handlers
        /******************************************************************************************************* 
        * SCROLLBAR EVENT HANDLERS
        *******************************************************************************************************/

        private void MapRenderVScroll_Scroll(object sender, ScrollEventArgs e)
        {
            MapStateMediator.ScrollPoint = new SKPoint(MapStateMediator.ScrollPoint.X, -e.NewValue);
            MapStateMediator.DrawingPoint = new SKPoint(MapStateMediator.DrawingPoint.X, e.NewValue);

            SKGLRenderControl.Invalidate();
        }

        private void MapRenderHScroll_Scroll(object sender, ScrollEventArgs e)
        {
            MapStateMediator.ScrollPoint = new SKPoint(-e.NewValue, MapStateMediator.ScrollPoint.Y);
            MapStateMediator.DrawingPoint = new SKPoint(e.NewValue, MapStateMediator.DrawingPoint.Y);

            SKGLRenderControl.Invalidate();
        }
        #endregion

        #region SKGLRenderControl Event Handlers
        /******************************************************************************************************* 
        * SKGLCONTROL EVENT HANDLERS
        *******************************************************************************************************/

        private void SKGLRenderControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            // handle zoom-in and zoom-out (TODO: zoom in and out from center of map - how?)
            e.Surface.Canvas.Scale(MainUIMediator.DrawingZoom);

            // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
            e.Surface.Canvas.Clear(SKColors.White);

            if (SKGLRenderControl.GRContext != null && MapStateMediator.CurrentMap != null && MapStateMediator.CurrentMap.MapLayers.Count == MapBuilder.MAP_LAYER_COUNT)
            {
                float symbolScale = (float)(SymbolUIMediator.SymbolScale / 100.0F);
                float symbolRotation = SymbolUIMediator.SymbolRotation;
                bool mirrorSymbol = SymbolUIMediator.MirrorSymbol;
                bool useAreaBrush = SymbolUIMediator.UseAreaBrush;

                if (RenderAsHeightMapMenuItem.Checked)
                {
                    MapHeightMapMethods.RenderHeightMapToCanvas(MapStateMediator.CurrentMap,
                        e.Surface.Canvas,
                        MapStateMediator.ScrollPoint,
                        MapStateMediator.SelectedRealmArea);
                }
                else
                {
                    MapRenderMethods.RenderMapToCanvas(MapStateMediator.CurrentMap,
                        e.Surface.Canvas,
                        MapStateMediator.ScrollPoint,
                        MapStateMediator.CurrentLandform,
                        MapStateMediator.CurrentWaterFeature,
                        MapStateMediator.CurrentRiver,
                        MapStateMediator.CurrentMapPath,
                        MapStateMediator.CurrentWindrose);
                }

                if (MainUIMediator.CurrentDrawingMode != MapDrawingMode.ColorSelect && MainUIMediator.CurrentDrawingMode != MapDrawingMode.None)
                {
                    MapRenderMethods.DrawCursor(e.Surface.Canvas,
                        MainUIMediator.CurrentDrawingMode,
                        new SKPoint(MapStateMediator.CurrentCursorPoint.X - MapStateMediator.ScrollPoint.X,
                            MapStateMediator.CurrentCursorPoint.Y - MapStateMediator.ScrollPoint.Y),
                        MainUIMediator.SelectedBrushSize,
                        symbolScale,
                        symbolRotation,
                        mirrorSymbol,
                        useAreaBrush);
                }

                // work layer
                MapRenderMethods.RenderWorkLayer(MapStateMediator.CurrentMap, e.Surface.Canvas, MapStateMediator.ScrollPoint);
            }
        }

        private void SKGLRenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            MapStateMediator.CurrentMouseLocation = e.Location.ToSKPoint();
            MapStateMediator.CurrentCursorPoint = MainFormUIMediator.CalculateCursorPoint(e);

            // objects are created and/or initialized on mouse down
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseDownHandler(e, MainUIMediator.SelectedBrushSize);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MiddleButtonMouseDownHandler(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseDownHandler(e);
            }
        }

        private void SKGLRenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.ColorSelect)
            {
                Cursor = AssetManager.EYEDROPPER_CURSOR;
            }

            MapStateMediator.CurrentMouseLocation = e.Location.ToSKPoint();
            MapStateMediator.CurrentCursorPoint = MainFormUIMediator.CalculateCursorPoint(e);

            // objects are drawn or moved on mouse move
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseMoveHandler(e, MainUIMediator.SelectedBrushSize / 2);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseMoveHandler(sender, e);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MiddleButtonMouseMoveHandler(sender, e);
            }
            else if (e.Button == MouseButtons.None)
            {
                NoButtonMouseMoveHandler(e);
            }

            SKGLRenderControl.Invalidate();
        }

        private void SKGLRenderControl_MouseUp(object sender, MouseEventArgs e)
        {
            MapStateMediator.CurrentMouseLocation = e.Location.ToSKPoint();
            MapStateMediator.CurrentCursorPoint = MainFormUIMediator.CalculateCursorPoint(e);

            // objects are finalized or reset on mouse up
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseUpHandler(e, MainUIMediator.SelectedBrushSize / 2);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseUpHandler(sender, e);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MiddleButtonMouseUpHandler(sender, e);
            }
        }

        private void SKGLRenderControl_MouseEnter(object sender, EventArgs e)
        {
            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.ColorSelect)
            {
                Cursor = AssetManager.EYEDROPPER_CURSOR;
            }
            else
            {
                Cursor = Cursors.Cross;
            }
        }

        private void SKGLRenderControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void SKGLRenderControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            const int cursorDelta = 5;
            int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;

            if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.LandErase)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandEraserSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.LandPaint)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.LandColor)
            {
                LandformMethods.LandformColorBrushSize = RealmMapMethods.GetNewBrushSize(LandColorBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.LandColorErase)
            {
                LandformMethods.LandformColorEraserBrushSize = RealmMapMethods.GetNewBrushSize(LandColorEraserSizeTrack, sizeDelta);
                MainUIMediator.SelectedBrushSize = LandformMethods.LandformColorEraserBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.OceanErase)
            {
                OceanMethods.OceanPaintEraserSize = RealmMapMethods.GetNewBrushSize(OceanEraserSizeTrack, sizeDelta);
                MainUIMediator.SelectedBrushSize = OceanMethods.OceanPaintEraserSize;
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.OceanPaint)
            {
                OceanMethods.OceanPaintBrushSize = RealmMapMethods.GetNewBrushSize(OceanBrushSizeTrack, sizeDelta);
                MainUIMediator.SelectedBrushSize = OceanMethods.OceanPaintBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.WaterPaint)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.WaterErase)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterEraserSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.LakePaint)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.WaterColor)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterColorBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.WaterColorErase)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterColorEraserSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.MapHeightIncrease)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainUIMediator.CurrentDrawingMode == MapDrawingMode.MapHeightDecrease)
            {
                MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys != Keys.Control && MainUIMediator.CurrentDrawingMode == MapDrawingMode.SymbolPlace)
            {
                if (AreaBrushSwitch.Enabled && SymbolUIMediator.UseAreaBrush)
                {
                    // area brush size is changed when AreaBrushSwitch is enabled and checked
                    MainUIMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(AreaBrushSizeTrack, sizeDelta);
                    SymbolUIMediator.AreaBrushSize = MainUIMediator.SelectedBrushSize;
                }
                else
                {
                    int newValue = (int)Math.Max(SymbolScaleUpDown.Minimum, SymbolScaleUpDown.Value + sizeDelta);
                    newValue = (int)Math.Min(SymbolScaleUpDown.Maximum, newValue);
                    SymbolUIMediator.SymbolScale = newValue;
                }
            }
            else if (ModifierKeys != Keys.Control && MainUIMediator.CurrentDrawingMode == MapDrawingMode.LabelSelect)
            {
                if (MapStateMediator.SelectedMapLabel != null)
                {
                    int newBrushSize = RealmMapMethods.GetNewBrushSize(LabelRotationTrack, sizeDelta);
                    LabelRotationUpDown.Value = Math.Min(LabelRotationUpDown.Maximum, newBrushSize);
                }
            }
            else if (ModifierKeys == Keys.Control)
            {
                SetZoomLevel(e.Delta);

                ZoomLevelTrack.Value = (int)(MainUIMediator.DrawingZoom * 10.0F);
            }

            SKGLRenderControl.Invalidate();
        }

        private void SKGLRenderControl_Enter(object sender, EventArgs e)
        {
            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.ColorSelect)
            {
                Cursor = AssetManager.EYEDROPPER_CURSOR;
            }
            else
            {
                Cursor = Cursors.Cross;
            }
        }

        private void SKGLRenderControl_Leave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void SKGLRenderControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // this is needed to allow keydown events for arrow keys to be sent
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }
        #endregion

        #region SKGLRenderControl Mouse Down Event Handling Methods (called from event handlers)

        #region Left Button Down Handler

        private void LeftButtonMouseDownHandler(MouseEventArgs e, int brushSize)
        {
            // has the map scale been clicked?
            MapStateMediator.SelectedMapScale = MapScaleManager.SelectMapScale(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

            if (MapStateMediator.SelectedMapScale != null)
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.SelectMapScale, 0);
                Cursor.Current = Cursors.SizeAll;
            }

            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.OceanPaint:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke == null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, OceanPaintColorSelectButton.BackColor.ToSKColor(),
                                MapStateMediator.SelectedColorPaintBrush, MainUIMediator.SelectedBrushSize / 2, MapBuilder.OCEANDRAWINGLAYER)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                            };

                            Cmd_AddOceanPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MainUIMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (OceanBrushVelocityTrack.Value / 100.0));

                            ApplicationTimerManager.BrushTimerEnabled = true;
                        }
                    }
                    break;
                case MapDrawingMode.OceanErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke == null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, SKColors.Transparent,
                                ColorPaintBrush.HardBrush, MainUIMediator.SelectedBrushSize / 2, MapBuilder.OCEANDRAWINGLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                            };

                            Cmd_AddOceanPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandformSelect:
                    {
                        Cursor = Cursors.Default;

                        MapStateMediator.SelectedLandform = LandformMethods.SelectLandformAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandformHeightMapSelect:
                    {
                        Cursor = Cursors.Default;

                        MapStateMediator.SelectedLandform = LandformMethods.SelectLandformAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;

                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentLandform = LandformMethods.CreateNewLandform(MapStateMediator.CurrentMap, null, SKRect.Empty,
                            UseTextureForBackgroundCheck.Checked, SKGLRenderControl);

                        SetLandformData(MapStateMediator.CurrentLandform);

                        MapStateMediator.CurrentLandform.DrawPath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, brushSize / 2);

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;
                        LandformMethods.LandformErasePath.Reset();
                    }
                    break;
                case MapDrawingMode.LandColor:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke == null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, LandColorSelectionButton.BackColor.ToSKColor(),
                                MapStateMediator.SelectedColorPaintBrush, MainUIMediator.SelectedBrushSize / 2, MapBuilder.LANDDRAWINGLAYER)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                            };

                            Cmd_AddLandPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MainUIMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (LandBrushVelocityTrack.Value / 100.0));

                            ApplicationTimerManager.BrushTimerEnabled = true;
                        }
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke == null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, SKColors.Transparent,
                                ColorPaintBrush.HardBrush, MainUIMediator.SelectedBrushSize / 2, MapBuilder.LANDDRAWINGLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                            };

                            Cmd_AddLandPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    {
                        MapStateMediator.SelectedWaterFeature = (IWaterFeature?)SelectWaterFeatureAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentWaterFeature = new()
                        {
                            ParentMap = MapStateMediator.CurrentMap,
                            WaterFeatureType = WaterFeatureType.Other,
                            WaterFeatureColor = WaterColorSelectionButton.BackColor,
                            WaterFeatureShorelineColor = ShorelineColorSelectionButton.BackColor
                        };

                        WaterFeatureMethods.ConstructWaterFeaturePaintObjects(MapStateMediator.CurrentWaterFeature);
                    }
                    break;
                case MapDrawingMode.LakePaint:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentWaterFeature = WaterFeatureMethods.CreateLake(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint,
                            brushSize, WaterColorSelectionButton.BackColor, ShorelineColorSelectionButton.BackColor);

                        if (MapStateMediator.CurrentWaterFeature != null)
                        {
                            Cmd_AddNewWaterFeature cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentWaterFeature);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            WaterFeatureMethods.MergeWaterFeatures(MapStateMediator.CurrentMap);

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    {
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentRiver == null)
                        {
                            MapStateMediator.CurrentRiver = WaterFeatureMethods.CreateRiver(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, WaterColorSelectionButton.BackColor,
                                ShorelineColorSelectionButton.BackColor, RiverWidthTrack.Value, RiverSourceFadeInSwitch.Checked,
                                RiverTextureSwitch.Checked);

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        // if the ctrl key is pressed and the user clicks on a river, find
                        // the nearest point on the river and make it a river control point
                        // so it can be moved
                        if (ModifierKeys == Keys.Control)
                        {
                            if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river)
                            {
                                foreach (MapRiverPoint mp in river.RiverPoints)
                                {
                                    mp.IsSelected = false;
                                }

                                MapStateMediator.SelectedRiverPoint = WaterFeatureMethods.SelectRiverPointAtPoint(river, MapStateMediator.CurrentCursorPoint, false);

                                if (MapStateMediator.SelectedRiverPoint != null)
                                {
                                    MapStateMediator.SelectedRiverPoint.IsControlPoint = true;
                                }
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterColor:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke == null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, WaterPaintColorSelectButton.BackColor.ToSKColor(),
                                MapStateMediator.SelectedColorPaintBrush, MainUIMediator.SelectedBrushSize / 2, MapBuilder.WATERDRAWINGLAYER)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                            };

                            Cmd_AddWaterPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MainUIMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (LandBrushVelocityTrack.Value / 100.0));

                            ApplicationTimerManager.BrushTimerEnabled = true;
                        }
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke == null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = new LayerPaintStroke(MapStateMediator.CurrentMap, SKColors.Empty,
                                ColorPaintBrush.HardBrush, MainUIMediator.SelectedBrushSize / 2, MapBuilder.WATERDRAWINGLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                            };

                            Cmd_AddWaterPaintStroke cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLayerPaintStroke);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        if (MapStateMediator.CurrentMapPath == null)
                        {
                            MapStateMediator.CurrentMapPath = MapPathMethods.CreatePath(MapStateMediator.CurrentMap,
                                MapStateMediator.CurrentCursorPoint, GetSelectedPathType(), PathColorSelectButton.BackColor,
                                PathWidthTrack.Value, DrawOverSymbolsSwitch.Checked,
                                AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX]);

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        // if the ctrl key is pressed and the user clicks on a path, find
                        // the nearest point on the path and make it a path control point
                        // so it can be moved
                        if (ModifierKeys == Keys.Control)
                        {
                            if (MapStateMediator.SelectedMapPath != null)
                            {
                                foreach (MapPathPoint mp in MapStateMediator.SelectedMapPath.PathPoints)
                                {
                                    mp.IsSelected = false;
                                }

                                MapStateMediator.SelectedMapPathPoint = MapPathMethods.SelectMapPathPointAtPoint(MapStateMediator.SelectedMapPath,
                                    MapStateMediator.CurrentCursorPoint, false);

                                if (MapStateMediator.SelectedMapPathPoint != null)
                                {
                                    MapStateMediator.SelectedMapPathPoint.IsControlPoint = true;
                                }
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.SymbolPlace:
                    {
                        if (SymbolUIMediator.UseAreaBrush)
                        {
                            ApplicationTimerManager.SymbolAreaBrushTimerEnabled = true;
                        }
                        else
                        {
                            SymbolManager.PlaceSelectedSymbolAtCursor(MapStateMediator.CurrentCursorPoint);
                        }

                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                    }
                    break;
                case MapDrawingMode.SymbolErase:
                    int eraserRadius = SymbolUIMediator.AreaBrushSize / 2;

                    SKPoint eraserCursorPoint = new(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                    SymbolManager.RemovePlacedSymbolsFromArea(MapStateMediator.CurrentMap, eraserCursorPoint, eraserRadius);
                    break;
                case MapDrawingMode.DrawArcLabelPath:
                    {
                        Cursor = Cursors.Cross;

                        LabelManager.ResetLabelPath();
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                    }
                    break;
                case MapDrawingMode.DrawBezierLabelPath:
                    {
                        Cursor = Cursors.Cross;
                        LabelManager.ResetLabelPath();

                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                    }
                    break;
                case MapDrawingMode.DrawBox:
                    {
                        if (BoxMediator.Box != null)
                        {
                            // initialize new box
                            Cursor = Cursors.Cross;

                            MapStateMediator.CurrentCursorPoint = MapStateMediator.CurrentCursorPoint;
                            MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                            // creates a temporary box to show on the work layer as the box is being drawn
                            MapStateMediator.SelectedPlacedMapBox = BoxManager.CreatePlacedMapBox(BoxMediator.Box, MapStateMediator.CurrentCursorPoint,
                                SelectBoxTintButton.BackColor);
                        }
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    {
                        if (MapStateMediator.CurrentMapMeasure == null)
                        {
                            MapMeasureManager.Create(MapStateMediator.CurrentMap, null);
                            MapMeasureManager.AddMeasurePoint(MapStateMediator.CurrentCursorPoint);
                            MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                        }
                        else
                        {
                            MapMeasureManager.AddMeasurePoint(MapStateMediator.CurrentCursorPoint);
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        Cursor = Cursors.Cross;

                        // initialize region
                        if (MapStateMediator.CurrentMapRegion == null)
                        {
                            RegionManager.Create(MapStateMediator.CurrentMap, MapRegionUIMediator);
                        }

                        if (MapStateMediator.CurrentMapRegion != null)
                        {
                            if (ModifierKeys == Keys.Shift)
                            {
                                RegionManager.SnapRegionToLandformCoastline(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion,
                                    MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                            }
                            else
                            {
                                MapRegionPoint mrp = new(MapStateMediator.CurrentCursorPoint);
                                MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);
                            }

                            MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (MapStateMediator.CurrentMapRegion != null && RegionManager.NEW_REGION_POINT != null)
                        {
                            Cmd_AddMapRegionPoint cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, RegionManager.NEW_REGION_POINT, RegionManager.NEXT_REGION_POINT_INDEX);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            // reset
                            RegionManager.NEW_REGION_POINT = null;
                            RegionManager.NEXT_REGION_POINT_INDEX = -1;
                            RegionManager.PREVIOUS_REGION_POINT_INDEX = -1;
                        }

                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    Cursor = Cursors.Cross;
                    MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                    break;
            }
        }

        #endregion

        #region Middle Button Down Handler

        private static void MiddleButtonMouseDownHandler(MouseEventArgs e)
        {
            MapStateMediator.PreviousMouseLocation = e.Location.ToSKPoint();
        }

        #endregion

        #region Right Button Down Handler

        private void RightButtonMouseDownHandler(MouseEventArgs e)
        {
            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.LabelSelect:
                    {
                        if (MapStateMediator.SelectedMapLabel != null)
                        {
                            CREATING_LABEL = true;

                            Font labelFont = new(MapStateMediator.SelectedMapLabel.LabelFont.FontFamily,
                                MapStateMediator.SelectedMapLabel.LabelFont.Size * MainUIMediator.DrawingZoom,
                                MapStateMediator.SelectedMapLabel.LabelFont.Style, GraphicsUnit.Pixel);

                            Size labelSize = TextRenderer.MeasureText(MapStateMediator.SelectedMapLabel.LabelText, labelFont,
                                new Size(int.MaxValue, int.MaxValue),
                                TextFormatFlags.Default | TextFormatFlags.SingleLine | TextFormatFlags.TextBoxControl);

                            Rectangle textBoxRect = LabelManager.GetLabelTextBoxRect(MapStateMediator.SelectedMapLabel,
                                MapStateMediator.DrawingPoint, MainUIMediator.DrawingZoom, labelSize);

                            LABEL_TEXT_BOX = LabelManager.CreateLabelEditTextBox(SKGLRenderControl, MapStateMediator.SelectedMapLabel, textBoxRect);

                            if (LABEL_TEXT_BOX != null)
                            {
                                LABEL_TEXT_BOX.KeyPress += LabelMediator.LabelTextBox_KeyPress;

                                SKGLRenderControl.Controls.Add(LABEL_TEXT_BOX);

                                LABEL_TEXT_BOX.BringToFront();
                                LABEL_TEXT_BOX.Select(LABEL_TEXT_BOX.Text.Length, 0);
                                LABEL_TEXT_BOX.Focus();
                                LABEL_TEXT_BOX.ScrollToCaret();

                                LABEL_TEXT_BOX.Tag = MapStateMediator.SelectedMapLabel.LabelPath;

                                LabelManager.Delete(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapLabel);
                            }

                            SKGLRenderControl.Refresh();
                        }
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    if (MapStateMediator.CurrentMapMeasure != null)
                    {
                        MapMeasureManager.EndMapMeasure(MapStateMediator.CurrentMapMeasure, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                        MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    if (MapStateMediator.CurrentMapRegion != null)
                    {
                        RegionManager.EndMapRegion(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, MapStateMediator.CurrentCursorPoint);

                        MapStateMediator.CurrentMap.IsSaved = false;

                        // reset everything
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                        MapStateMediator.CurrentMapRegion = null;
                        MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);
                    }

                    break;
            }
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Move Event Handling Methods (called from event handlers)

        #region Left Button Move Handler Method

        private void LeftButtonMouseMoveHandler(MouseEventArgs e, int brushRadius)
        {
            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.OceanErase:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentLayerPaintStroke?.AddLayerPaintStrokePoint(MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    {
                        if (MapStateMediator.CurrentLandform != null
                            && MapStateMediator.CurrentCursorPoint.X > 0 && MapStateMediator.CurrentCursorPoint.X < MapStateMediator.CurrentMap.MapWidth
                            && MapStateMediator.CurrentCursorPoint.Y > 0 && MapStateMediator.CurrentCursorPoint.Y < MapStateMediator.CurrentMap.MapHeight)
                        {
                            MapStateMediator.CurrentLandform.IsModified = true;

                            MapStateMediator.CurrentLandform.DrawPath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, brushRadius);

                            bool createPathsWhilePainting = Settings.Default.CalculateContoursWhilePainting;

                            if (createPathsWhilePainting)
                            {
                                // compute contour path and inner and outer paths in a separate thread
                                Task.Run(() => LandformMethods.CreateAllPathsFromDrawnPath(MapStateMediator.CurrentMap, MapStateMediator.CurrentLandform));
                            }
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        LandformMethods.LandformErasePath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, brushRadius);
                        LandformMethods.EraseFromLandform(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, brushRadius);

                        LandformMethods.LandformErasePath.Reset();

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandformSelect:
                    {
                        if (MapStateMediator.SelectedLandform != null)
                        {
                            LandformMethods.MoveLandform(MapStateMediator.SelectedLandform, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);

                            MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentLayerPaintStroke?.AddLayerPaintStrokePoint(MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    {
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentWaterFeature != null)
                        {
                            MapStateMediator.CurrentWaterFeature.WaterFeaturePath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, brushRadius);

                            // compute contour path and inner and outer paths in a separate thread
                            Task.Run(() => WaterFeatureMethods.CreateInnerAndOuterPaths(MapStateMediator.CurrentMap, MapStateMediator.CurrentWaterFeature));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterErase:
                    {
                        Cursor = Cursors.Cross;

                        WaterFeatureMethods.WaterFeaturErasePath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, brushRadius);

                        WaterFeatureMethods.EraseWaterFeature(MapStateMediator.CurrentMap);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentLayerPaintStroke?.AddLayerPaintStrokePoint(MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentRiver?.RiverPoints.Add(new MapRiverPoint(MapStateMediator.CurrentCursorPoint));

                        if (MapStateMediator.CurrentRiver != null)
                        {
                            WaterFeatureMethods.ConstructRiverPaths(MapStateMediator.CurrentRiver);
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river && MapStateMediator.SelectedRiverPoint != null)
                    {
                        // move the selected point on the path
                        WaterFeatureMethods.MoveSelectedRiverPoint(river, MapStateMediator.SelectedRiverPoint, MapStateMediator.CurrentCursorPoint);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    {
                        Cursor = Cursors.Cross;
                        const int minimumPathPointCount = 5;

                        SKPoint newPathPoint = MapPathMethods.GetNewPathPoint(MapStateMediator.CurrentMapPath, ModifierKeys, SELECTED_PATH_ANGLE, MapStateMediator.CurrentCursorPoint, minimumPathPointCount);

                        MapPathMethods.AddNewPathPoint(MapStateMediator.CurrentMapPath, newPathPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        if (MapStateMediator.SelectedMapPath != null)
                        {
                            MapPathMethods.MovePath(MapStateMediator.SelectedMapPath, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);

                            MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    if (MapStateMediator.SelectedMapPathPoint != null)
                    {
                        // move the selected point on the path
                        MapPathMethods.MoveSelectedMapPathPoint(MapStateMediator.SelectedMapPath, MapStateMediator.SelectedMapPathPoint, MapStateMediator.CurrentCursorPoint);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.SymbolPlace:
                    if (!SymbolUIMediator.UseAreaBrush)
                    {
                        SymbolManager.PlaceSelectedSymbolAtCursor(MapStateMediator.CurrentCursorPoint);
                    }

                    MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.SymbolErase:
                    int eraserRadius = SymbolUIMediator.AreaBrushSize / 2;

                    SymbolManager.RemovePlacedSymbolsFromArea(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, eraserRadius);

                    MapStateMediator.CurrentMap.IsSaved = false;
                    break;
                case MapDrawingMode.SymbolColor:

                    int colorBrushRadius = SymbolUIMediator.AreaBrushSize / 2;

                    Color[] symbolColors = [SymbolColor1Button.BackColor, SymbolColor2Button.BackColor, SymbolColor3Button.BackColor];
                    SymbolManager.ColorSymbolsInArea(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, colorBrushRadius, symbolColors, RandomizeColorCheck.Checked);

                    MapStateMediator.CurrentMap.IsSaved = false;
                    break;
                case MapDrawingMode.SymbolSelect:
                    SymbolManager.MoveSymbol(MapStateMediator.SelectedMapSymbol, MapStateMediator.CurrentCursorPoint);

                    MapStateMediator.CurrentMap.IsSaved = false;
                    SKGLRenderControl.Invalidate();

                    break;
                case MapDrawingMode.LabelSelect:
                    if (MapStateMediator.SelectedMapLabel != null)
                    {
                        LabelManager.MoveLabel(MapStateMediator.SelectedMapLabel, MapStateMediator.CurrentCursorPoint);
                    }
                    else if (MapStateMediator.SelectedPlacedMapBox != null)
                    {
                        LabelManager.MoveBox(MapStateMediator.SelectedPlacedMapBox, MapStateMediator.CurrentCursorPoint);
                    }

                    MapStateMediator.CurrentMap.IsSaved = false;
                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.DrawArcLabelPath:
                    {
                        LabelManager.ResetLabelPath();
                        LabelManager.CurrentMapLabelPath = LabelManager.CreateNewArcPath(MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);

                        LabelManager.DrawLabelPathOnWorkLayer(MapStateMediator.CurrentMap, LabelManager.CurrentMapLabelPath);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawBezierLabelPath:
                    {
                        LabelManager.CurrentMapPathLabelPoints.Add(MapStateMediator.CurrentCursorPoint);
                        LabelManager.ConstructBezierPathFromPoints();

                        LabelManager.DrawLabelPathOnWorkLayer(MapStateMediator.CurrentMap, LabelManager.CurrentMapLabelPath);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawBox:
                    // draw box as mouse is moved
                    if (BoxMediator.Box != null && MapStateMediator.SelectedPlacedMapBox != null)
                    {
                        BoxMediator.DrawBoxOnWorkLayer(MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    {
                        if (MapStateMediator.CurrentMapMeasure != null)
                        {
                            MapMeasureManager.DrawMapMeasureOnWorkLayer(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapMeasure,
                                MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        if (MapStateMediator.CurrentMapRegion != null)
                        {
                            RegionManager.DrawRegionOnWorkLayer(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (MapStateMediator.CurrentMapRegion != null && MapStateMediator.CurrentMapRegion.IsSelected)
                        {
                            MapRegionPoint? selectedMapRegionPoint = RegionManager.GetSelectedMapRegionPoint(MapStateMediator.CurrentMapRegion, MapStateMediator.CurrentCursorPoint);

                            if (selectedMapRegionPoint != null)
                            {
                                selectedMapRegionPoint.RegionPoint = MapStateMediator.CurrentCursorPoint;
                            }

                            if (!RegionManager.EDITING_REGION)
                            {
                                RegionManager.MoveRegion(MapStateMediator.CurrentMapRegion, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                                MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                            }
                        }
                    }
                    break;
                case MapDrawingMode.SelectMapScale:
                    if (MapStateMediator.SelectedMapScale != null)
                    {
                        MapScaleManager.MoveMapScale(MapStateMediator.SelectedMapScale, MapStateMediator.CurrentCursorPoint);
                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    {
                        MapStateMediator.SelectedRealmArea = RealmMapMethods.DrawSelectedRealmAreaOnWorkLayer(MapStateMediator.CurrentMap,
                            MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightIncrease:
                    {
                        float brushStrength = (float)BrushStrengthUpDown.Value;
                        MapHeightMapMethods.ChangeHeightMapAreaHeight(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, brushRadius, brushStrength);

                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightDecrease:
                    {
                        float brushStrength = (float)BrushStrengthUpDown.Value;
                        MapHeightMapMethods.ChangeHeightMapAreaHeight(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, brushRadius, -brushStrength);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }

        }

        #endregion

        #region Middle Button Move Handler Method
        private void MiddleButtonMouseMoveHandler(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            PanMap(e);
            SKGLRenderControl.Invalidate();
        }

        #endregion

        #region Right Button Move Handler Method

        private void RightButtonMouseMoveHandler(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region No Button Move Handler Method

        private void NoButtonMouseMoveHandler(MouseEventArgs e)
        {
            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.PlaceWindrose:
                    {
                        BackgroundMethods.MoveWindrose(MapStateMediator.CurrentWindrose, MapStateMediator.CurrentCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        MapStateMediator.SelectedRiverPoint = WaterFeatureMethods.GetSelectedRiverPoint(MapStateMediator.SelectedWaterFeature, MapStateMediator.CurrentCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        MapStateMediator.SelectedMapPathPoint = MapPathMethods.GetSelectedPathPoint(MapStateMediator.SelectedMapPath, MapStateMediator.CurrentCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        if (MapStateMediator.CurrentMapRegion == null || MapStateMediator.CurrentMapRegion.MapRegionPoints.Count == 1)
                        {
                            if (ModifierKeys == Keys.Shift)
                            {
                                RegionManager.DrawCoastlinePointOnWorkLayer(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                            }
                        }
                        else
                        {
                            RegionManager.DrawRegionOnWorkLayer(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion,
                                MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (MapStateMediator.CurrentMapRegion != null && MapStateMediator.CurrentMapRegion.IsSelected)
                        {
                            bool pointSelected = RegionManager.IsRegionPointSelected(MapStateMediator.CurrentMapRegion, MapStateMediator.CurrentCursorPoint);

                            if (!pointSelected)
                            {
                                // cursor is not on a region point; is it on a line segment between vertices of the region?
                                // if so draw a yellow circle at that point
                                RegionManager.DrawRegionPointOnWorkLayer(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, MapStateMediator.CurrentCursorPoint);
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Up Event Handling Methods (called from event handers)

        #region Left Button Up Handler

        private void LeftButtonMouseUpHandler(MouseEventArgs e, int brushRadius)
        {
            ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;

            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.OceanPaint:
                    {
                        ApplicationTimerManager.BrushTimerEnabled = false;
                        MapStateMediator.CurrentLayerPaintStroke = null;
                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.OceanErase:
                    {
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke != null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = null;
                            MapStateMediator.CurrentMap.IsSaved = false;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    if (MapStateMediator.CurrentLandform != null)
                    {
                        Cmd_AddNewLandform cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLandform);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        LandformMethods.MergeLandforms(MapStateMediator.CurrentMap);

                        MapStateMediator.CurrentLandform = null;

                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        LandformMethods.EraseFromLandform(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, brushRadius);

                        LandformMethods.LandformErasePath.Reset();

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandColor:
                    {
                        Cursor = Cursors.Cross;

                        ApplicationTimerManager.BrushTimerEnabled = false;

                        if (MapStateMediator.CurrentLayerPaintStroke != null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = null;
                            MapStateMediator.CurrentMap.IsSaved = false;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentLayerPaintStroke = null;
                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PlaceWindrose:
                    if (MapStateMediator.CurrentWindrose != null)
                    {
                        MapStateMediator.CurrentWindrose.X = (int)MapStateMediator.CurrentCursorPoint.X;
                        MapStateMediator.CurrentWindrose.Y = (int)MapStateMediator.CurrentCursorPoint.Y;

                        Cmd_AddWindrose cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentWindrose);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        MapStateMediator.CurrentWindrose = CreateWindrose();

                        MapStateMediator.CurrentMap.IsSaved = false;
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    if (MapStateMediator.CurrentWaterFeature != null)
                    {
                        Cmd_AddNewWaterFeature cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentWaterFeature);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        WaterFeatureMethods.MergeWaterFeatures(MapStateMediator.CurrentMap);

                        MapStateMediator.CurrentWaterFeature = null;

                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LakePaint:
                    if (MapStateMediator.CurrentWaterFeature != null)
                    {
                        MapStateMediator.CurrentWaterFeature = null;

                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    if (MapStateMediator.CurrentRiver != null)
                    {
                        WaterFeatureMethods.ConstructRiverPaths(MapStateMediator.CurrentRiver);

                        Cmd_AddNewRiver cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentRiver);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        MapStateMediator.CurrentRiver = null;

                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        MapStateMediator.SelectedRiverPoint = null;
                    }
                    break;
                case MapDrawingMode.WaterColor:
                    {
                        ApplicationTimerManager.BrushTimerEnabled = false;

                        if (MapStateMediator.CurrentLayerPaintStroke != null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = null;
                            MapStateMediator.CurrentMap.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentLayerPaintStroke != null)
                        {
                            MapStateMediator.CurrentLayerPaintStroke = null;
                            MapStateMediator.CurrentMap.IsSaved = false;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    if (MapStateMediator.CurrentMapPath != null)
                    {
                        MapStateMediator.CurrentMapPath.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(MapStateMediator.CurrentMapPath.PathPoints);

                        Cmd_AddNewMapPath cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapPath);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        MapStateMediator.CurrentMapPath = null;
                        SELECTED_PATH_ANGLE = -1;

                        MapStateMediator.CurrentMap.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        MapStateMediator.SelectedMapPath = SelectMapPathAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        MapStateMediator.SelectedMapPathPoint = null;
                    }
                    break;
                case MapDrawingMode.SymbolSelect:
                    {
                        MapStateMediator.SelectedMapSymbol = MapSymbolUIMediator.SelectMapSymbolAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint.ToDrawingPoint());

                        if (MapStateMediator.SelectedMapSymbol != null)
                        {
                            MapStateMediator.SelectedMapSymbol.IsSelected = !MapStateMediator.SelectedMapSymbol.IsSelected;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawLabel:
                    if (!CREATING_LABEL)
                    {
                        CREATING_LABEL = true;

                        Font tbFont = new(LabelMediator.SelectedLabelFont.FontFamily,
                                LabelMediator.SelectedLabelFont.Size * 0.75F, LabelMediator.SelectedLabelFont.Style, GraphicsUnit.Point);

                        Size labelSize = TextRenderer.MeasureText("...Label...", tbFont,
                            new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.Default | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.ExternalLeading | TextFormatFlags.SingleLine);

                        Rectangle labelRect = new(e.X - (labelSize.Width / 2), e.Y - (labelSize.Height / 2), labelSize.Width, labelSize.Height);

                        LABEL_TEXT_BOX = LabelManager.CreateNewLabelTextbox(SKGLRenderControl, tbFont,
                            labelRect, FontColorSelectButton.BackColor);

                        if (LABEL_TEXT_BOX != null)
                        {
                            LABEL_TEXT_BOX.KeyPress += LabelMediator.LabelTextBox_KeyPress;

                            SKGLRenderControl.Controls.Add(LABEL_TEXT_BOX);

                            SKGLRenderControl.Refresh();

                            LABEL_TEXT_BOX.BringToFront();
                            LABEL_TEXT_BOX.Select(LABEL_TEXT_BOX.Text.Length, 0);
                            LABEL_TEXT_BOX.Focus();
                            LABEL_TEXT_BOX.ScrollToCaret();
                        }
                    }
                    break;
                case MapDrawingMode.LabelSelect:
                    {
                        LabelMediator.SelectLabelOrBox();
                    }
                    break;
                case MapDrawingMode.DrawBox:
                    // clear the work layer
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                    // finalize box drawing and add the box
                    BoxManager.Create(MapStateMediator.CurrentMap, null);

                    MapStateMediator.CurrentMap.IsSaved = false;
                    MapStateMediator.SelectedPlacedMapBox = null;

                    SKGLRenderControl.Invalidate();

                    break;
                case MapDrawingMode.DrawMapMeasure:
                    {
                        if (MapStateMediator.CurrentMapMeasure != null)
                        {
                            if (!MapStateMediator.CurrentMapMeasure.MeasurePoints.Contains(MapStateMediator.PreviousCursorPoint))
                            {
                                MapStateMediator.CurrentMapMeasure.MeasurePoints.Add(MapStateMediator.PreviousCursorPoint);
                            }

                            MapStateMediator.CurrentMapMeasure.MeasurePoints.Add(MapStateMediator.CurrentCursorPoint);

                            float lineLength = SKPoint.Distance(MapStateMediator.PreviousCursorPoint, MapStateMediator.CurrentCursorPoint);
                            MapStateMediator.CurrentMapMeasure.TotalMeasureLength += lineLength;
                        }

                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (RegionManager.EDITING_REGION)
                        {
                            RegionManager.EDITING_REGION = false;
                        }
                        else
                        {
                            MapRegion? selectedRegion = RegionManager.SelectRegionAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                            if (selectedRegion != null)
                            {
                                if (selectedRegion.IsSelected)
                                {
                                    MapStateMediator.CurrentMapRegion = selectedRegion;
                                }
                                else
                                {
                                    MapStateMediator.CurrentMapRegion = null;
                                }
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.ColorSelect:
                    {
                        // eyedropper color select function
                        using SKSurface s = SKSurface.Create(new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight));
                        s.Canvas.Clear();

                        MapRenderMethods.RenderMapForExport(MapStateMediator.CurrentMap, s.Canvas);

                        using Bitmap colorBitmap = s.Snapshot().ToBitmap();

                        Color pixelColor = colorBitmap.GetPixel((int)MapStateMediator.CurrentCursorPoint.X, (int)MapStateMediator.CurrentCursorPoint.Y);

                        switch (MainTab.SelectedIndex)
                        {
                            case 1:
                                // ocean layer
                                OceanPaintColorSelectButton.BackColor = pixelColor;
                                OceanPaintColorSelectButton.Refresh();
                                break;
                            case 2:
                                // land layer
                                LandColorSelectionButton.BackColor = pixelColor;
                                LandColorSelectionButton.Refresh();
                                break;
                            case 3:
                                // water layer
                                WaterPaintColorSelectButton.BackColor = pixelColor;
                                WaterPaintColorSelectButton.Refresh();
                                break;
                        }
                    }
                    break;
                case MapDrawingMode.SelectMapScale:
                    if (MapStateMediator.SelectedMapScale != null)
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.SelectedMapScale.IsSelected = false;
                        MapStateMediator.SelectedMapScale = null;

                        MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

                        MapStateMediator.CurrentMap.IsSaved = false;
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    {
                        MapStateMediator.SelectedRealmArea = new(MapStateMediator.PreviousCursorPoint.X, MapStateMediator.PreviousCursorPoint.Y, MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawRect((SKRect)MapStateMediator.SelectedRealmArea, PaintObjects.LandformAreaSelectPaint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightIncrease:
                    {
                        // update the 3D view if it is open and updates are enabled
                        if (CurrentHeightMapView != null && Update3DViewSwitch.Checked)
                        {
                            try
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                SKBitmap? heightMapBitmap = MapHeightMapMethods.GetBitmapForThreeDView(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, MapStateMediator.SelectedRealmArea);
                                if (heightMapBitmap != null)
                                {
                                    // generate the 3D model from the height information in the selected area
                                    Task updateTask = Task.Run(() =>
                                    {
                                        List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                                        // convert the list of strings to a single string
                                        string objModelString = string.Join(Environment.NewLine, objModelStringList);

                                        lock (LandFormObjModelList) // Ensure thread safety
                                        {
                                            LandFormObjModelList.Add(objModelString);
                                        }
                                    });

                                    updateTask.Wait();
                                    CurrentHeightMapView.UpdateView();
                                }
                            }
                            finally
                            {
                                Cursor.Current = Cursors.Default;
                            }
                        }
                    }
                    break;
                case MapDrawingMode.MapHeightDecrease:
                    {
                        // update the 3D view if it is open and updates are enabled
                        if (CurrentHeightMapView != null && Update3DViewSwitch.Checked)
                        {
                            try
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                SKBitmap? heightMapBitmap = MapHeightMapMethods.GetBitmapForThreeDView(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, MapStateMediator.SelectedRealmArea);
                                if (heightMapBitmap != null)
                                {
                                    // generate the 3D model from the height information in the selected area
                                    Task updateTask = Task.Run(() =>
                                    {
                                        List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                                        // convert the list of strings to a single string
                                        string objModelString = string.Join(Environment.NewLine, objModelStringList);

                                        lock (LandFormObjModelList) // Ensure thread safety
                                        {
                                            LandFormObjModelList.Add(objModelString);
                                        }
                                    });

                                    updateTask.Wait();
                                    CurrentHeightMapView.UpdateView();
                                }
                            }
                            finally
                            {
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        break;
                    }
            }

        }

        #endregion

        #region Middle Button Up Handler

        private void MiddleButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

        #endregion

        #region Right Button Up Handler

        private void RightButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            switch (MainUIMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.LandformSelect:

                    LandformSelectButton.Checked = false;

                    MapStateMediator.SelectedLandform = LandformMethods.SelectLandformAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                    SKGLRenderControl.Invalidate();

                    if (MapStateMediator.SelectedLandform != null)
                    {
                        LandformInfo landformInfo = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, AssetManager.CURRENT_THEME, SKGLRenderControl);
                        landformInfo.ShowDialog(this);
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    MapComponent? selectedWaterFeature = SelectWaterFeatureAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                    SKGLRenderControl.Invalidate();

                    if (selectedWaterFeature != null && selectedWaterFeature is WaterFeature wf)
                    {
                        MapStateMediator.SelectedWaterFeature = (WaterFeature)selectedWaterFeature;

                        WaterFeatureInfo waterFeatureInfo = new(MapStateMediator.CurrentMap, wf, SKGLRenderControl);
                        waterFeatureInfo.ShowDialog(this);
                    }
                    else if (selectedWaterFeature != null && selectedWaterFeature is River r)
                    {
                        MapStateMediator.SelectedWaterFeature = (River)selectedWaterFeature;

                        RiverInfo riverInfo = new(MapStateMediator.CurrentMap, r, SKGLRenderControl);
                        riverInfo.ShowDialog(this);
                    }

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.PathSelect:
                    MapPath? selectedPath = SelectMapPathAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                    SKGLRenderControl.Invalidate();

                    if (selectedPath != null)
                    {
                        MapStateMediator.SelectedMapPath = selectedPath;

                        MapPathInfo pathInfo = new(MapStateMediator.CurrentMap, selectedPath, SKGLRenderControl);
                        pathInfo.ShowDialog(this);
                    }

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.SymbolSelect:
                    MapSymbol? selectedSymbol = MapSymbolUIMediator.SelectMapSymbolAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint.ToDrawingPoint());
                    if (selectedSymbol != null)
                    {
                        MapStateMediator.SelectedMapSymbol = selectedSymbol;

                        MapSymbolInfo msi = new(MapStateMediator.CurrentMap, selectedSymbol);
                        msi.ShowDialog(this);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    MapRegion? selectedRegion = RegionManager.SelectRegionAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                    if (selectedRegion != null)
                    {
                        MapStateMediator.CurrentMapRegion = selectedRegion;

                        MapRegionInfo mri = new(MapStateMediator.CurrentMap, selectedRegion, SKGLRenderControl);
                        mri.ShowDialog(this);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #region SKGLRenderControl KeyDown Handler
        private void SKGLRenderControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // deselect selected symbols, map frames, boxes,
                // clear any label paths that have been drawn
                // deselect all selected objects
                // what else?

                // stop and dispose of the brush timer
                ApplicationTimerManager.BrushTimerEnabled = false;

                // stop placing symbols
                ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;

                // clear drawing mode and set brush size to 0
                MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

                // dispose of any map label path
                LabelManager.CurrentMapLabelPath?.Dispose();
                LabelManager.CurrentMapLabelPath = new();

                // clear other selected and current objects?

                // clear the work layer
                MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                // unselect anything selected
                RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, null);

                // dispose of any label text box that is drawn
                if (LABEL_TEXT_BOX != null)
                {
                    SKGLRenderControl.Controls.Remove(LABEL_TEXT_BOX);
                    LABEL_TEXT_BOX.Dispose();
                }

                // clear all selections
                MapStateMediator.PreviousSelectedRealmArea = SKRect.Empty;
                MapStateMediator.SelectedRealmArea = SKRect.Empty;
                MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                FontSelectionPanel.Visible = false;

                // re-render everything
                SKGLRenderControl.Invalidate();
                Refresh();
            }

            if (e.KeyCode == Keys.Delete)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.LandformSelect:
                        if (MapStateMediator.SelectedLandform != null)
                        {
                            Cmd_RemoveLandform cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MapStateMediator.SelectedLandform = null;
                            MapStateMediator.CurrentMap.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                    case MapDrawingMode.WaterFeatureSelect:
                        // delete water features, rivers
                        if (MapStateMediator.SelectedWaterFeature != null)
                        {
                            Cmd_RemoveWaterFeature cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedWaterFeature);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MapStateMediator.SelectedWaterFeature = null;
                            MapStateMediator.SelectedRiverPoint = null;

                            MapStateMediator.CurrentMap.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }

                        break;
                    case MapDrawingMode.RiverEdit:
                        {
                            if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river && MapStateMediator.SelectedRiverPoint != null)
                            {
                                Cmd_RemoveRiverPoint cmd = new(river, MapStateMediator.SelectedRiverPoint);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                MapStateMediator.SelectedRiverPoint = null;

                                MapStateMediator.CurrentMap.IsSaved = false;

                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                    case MapDrawingMode.PathSelect:
                        if (MapStateMediator.SelectedMapPath != null)
                        {
                            Cmd_RemoveMapPath cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapPath);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            MapStateMediator.SelectedMapPath = null;
                            MapStateMediator.SelectedMapPathPoint = null;

                            MapStateMediator.CurrentMap.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }

                        break;
                    case MapDrawingMode.PathEdit:
                        {
                            if (MapStateMediator.SelectedMapPath != null && MapStateMediator.SelectedMapPathPoint != null)
                            {
                                Cmd_RemovePathPoint cmd = new(MapStateMediator.SelectedMapPath, MapStateMediator.SelectedMapPathPoint);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                MapStateMediator.SelectedMapPathPoint = null;

                                MapStateMediator.CurrentMap.IsSaved = false;

                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (MapStateMediator.SelectedMapSymbol != null)
                            {
                                Cmd_RemoveSymbol cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapSymbol);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                MapStateMediator.SelectedMapSymbol = null;
                                MapStateMediator.CurrentMap.IsSaved = false;

                                SKGLRenderControl.Invalidate();
                            }
                        }

                        break;
                    case MapDrawingMode.LabelSelect:
                        {
                            if (MapStateMediator.SelectedMapLabel != null)
                            {
                                Cmd_DeleteLabel cmd = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapLabel);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                MapStateMediator.CurrentMap.IsSaved = false;
                                MapStateMediator.SelectedMapLabel = null;
                            }

                            if (MapStateMediator.SelectedPlacedMapBox != null)
                            {
                                BoxManager.Delete(MapStateMediator.CurrentMap, null);
                            }

                            SKGLRenderControl.Invalidate();
                        }
                        break;
                    case MapDrawingMode.RegionSelect:
                        {
                            if (MapStateMediator.CurrentMapRegion != null)
                            {
                                bool pointSelected = false;

                                foreach (MapRegionPoint mrp in MapStateMediator.CurrentMapRegion.MapRegionPoints)
                                {
                                    if (mrp.IsSelected)
                                    {
                                        pointSelected = true;
                                        Cmd_DeleteMapRegionPoint cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, mrp);
                                        CommandManager.AddCommand(cmd);
                                        cmd.DoOperation();

                                        break;
                                    }
                                }

                                if (!pointSelected)
                                {
                                    Cmd_DeleteMapRegion cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion);
                                    CommandManager.AddCommand(cmd);
                                    cmd.DoOperation();

                                    MapStateMediator.CurrentMapRegion = null;
                                }

                                MapStateMediator.CurrentMap.IsSaved = false;

                                SKGLRenderControl.Invalidate();

                                return;
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.PageUp)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 5);
                            }
                            else if (ModifierKeys == Keys.None)
                            {
                                SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 1);
                            }
                        }
                        break;
                    case MapDrawingMode.RegionSelect:
                        {
                            RegionManager.MoveSelectedRegionInRenderOrder(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, ComponentMoveDirection.Up);
                        }
                        break;
                }

                SKGLRenderControl.Invalidate();
            }

            if (e.KeyCode == Keys.PageDown)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 5);
                            }
                            else if (ModifierKeys == Keys.None)
                            {
                                SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 1);
                            }
                        }
                        break;
                    case MapDrawingMode.RegionSelect:
                        {
                            RegionManager.MoveSelectedRegionInRenderOrder(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, ComponentMoveDirection.Down);
                        }
                        break;
                }

                SKGLRenderControl.Invalidate();
            }

            if (e.KeyCode == Keys.End)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            // move symbol to bottom of render order
                            SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 1, true);
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Home)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            // move symbol to top of render order
                            SymbolManager.MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 1, true);
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (MapStateMediator.SelectedMapSymbol != null)
                            {
                                MapStateMediator.SelectedMapSymbol.Y = Math.Min(MapStateMediator.SelectedMapSymbol.Y + 1, MapStateMediator.CurrentMap.MapHeight);
                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Up)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (MapStateMediator.SelectedMapSymbol != null)
                            {
                                MapStateMediator.SelectedMapSymbol.Y = Math.Max(0, MapStateMediator.SelectedMapSymbol.Y - 1);
                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Left)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (MapStateMediator.SelectedMapSymbol != null)
                            {
                                if (MapStateMediator.SelectedMapSymbol != null)
                                {
                                    MapStateMediator.SelectedMapSymbol.X = Math.Max(0, MapStateMediator.SelectedMapSymbol.X - 1);
                                    SKGLRenderControl.Invalidate();
                                }
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Right)
            {
                switch (MainUIMediator.CurrentDrawingMode)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (MapStateMediator.SelectedMapSymbol != null)
                            {
                                MapStateMediator.SelectedMapSymbol.X = Math.Min(MapStateMediator.SelectedMapSymbol.X + 1, MapStateMediator.CurrentMap.MapWidth);
                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                }
            }

            e.Handled = true;
        }
        #endregion

        #region Background Tab Event Handlers
        /******************************************************************************************************* 
        * BACKGROUND TAB EVENT HANDLERS
        *******************************************************************************************************/
        private void ShowBaseLayerSwitch_CheckedChanged()
        {
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);
            baseLayer.ShowLayer = ShowBaseLayerSwitch.Checked;
            SKGLRenderControl.Invalidate();
        }

        private void NextBackgroundTextureButton_Click(object sender, EventArgs e)
        {
            Bitmap? nextImage = BackgroundMethods.NextBackgroundTexture();

            if (nextImage != null)
            {
                BackgroundTextureBox.Image = nextImage;
                BackgroundTextureNameLabel.Text = BackgroundMethods.GetCurrentImageName();
            }
        }

        private void PreviousBackgroundTextureButton_Click(object sender, EventArgs e)
        {
            Bitmap? previousImage = BackgroundMethods.PreviousBackgroundTexture();

            if (previousImage != null)
            {
                BackgroundTextureBox.Image = previousImage;
                BackgroundTextureNameLabel.Text = BackgroundMethods.GetCurrentImageName();
            }
        }

        private void BackgroundTextureScaleTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show((BackgroundTextureScaleTrack.Value / 100.0F).ToString(), BackgroundTextureGroup, new Point(BackgroundTextureScaleTrack.Right - 30, BackgroundTextureScaleTrack.Top - 20), 2000);
        }

        private void FillBackgroundButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;

            BackgroundMethods.ApplyBackgroundTexture(MapStateMediator.CurrentMap,
                BackgroundMethods.GetSelectedBackgroundImage(),
                BackgroundTextureScaleTrack.Value / 100.0F,
                MirrorBackgroundSwitch.Checked);

            SKGLRenderControl.Invalidate();
        }

        private void ClearBackgroundButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;
            BackgroundMethods.ClearBackgroundImage(MapStateMediator.CurrentMap);
            SKGLRenderControl.Invalidate();
        }

        private void VignetteColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, VignetteColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                VignetteColorSelectionButton.BackColor = selectedColor;
                VignetteColorSelectionButton.Refresh();

                BackgroundMethods.ChangeVignetteColor(MapStateMediator.CurrentMap, selectedColor);
                SKGLRenderControl.Invalidate();
            }
        }

        private void VignetteStrengthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(VignetteStrengthTrack.Value.ToString(), VignetteGroupBox, new Point(VignetteStrengthTrack.Right - 30, VignetteStrengthTrack.Top - 20), 2000);
            BackgroundMethods.ChangeVignetteStrength(MapStateMediator.CurrentMap, VignetteStrengthTrack.Value);
            SKGLRenderControl.Invalidate();
        }

        private void OvalVignetteRadio_CheckedChanged(object sender, EventArgs e)
        {
            BackgroundMethods.ChangeVignetteShape(MapStateMediator.CurrentMap, RectangleVignetteRadio.Checked);
            SKGLRenderControl.Invalidate();
        }

        private void RectangleVignetteRadio_CheckedChanged(object sender, EventArgs e)
        {
            BackgroundMethods.ChangeVignetteShape(MapStateMediator.CurrentMap, RectangleVignetteRadio.Checked);
            SKGLRenderControl.Invalidate();
        }

        #endregion

        #region Ocean Tab Methods
        private void SetOceanColorFromPreset(string htmlColor)
        {
            Color oceanColor = ColorTranslator.FromHtml(htmlColor);

            OceanPaintColorSelectButton.BackColor = oceanColor;
            OceanPaintColorSelectButton.Refresh();
        }

        private void SetOceanPaintColorFromCustomPresetButton(Button b)
        {
            if (b.Text != "")
            {
                Color oceanColor = b.BackColor;

                OceanPaintColorSelectButton.BackColor = oceanColor;
                OceanPaintColorSelectButton.Refresh();
            }
        }

        #endregion

        #region Ocean Tab Event Handlers

        private void ShowOceanLayerSwitch_CheckedChanged()
        {
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANDRAWINGLAYER);

            oceanTextureLayer.ShowLayer = ShowOceanLayerSwitch.Checked;

            oceanTextureOverlayLayer.ShowLayer = ShowOceanLayerSwitch.Checked;

            oceanDrawingLayer.ShowLayer = ShowOceanLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void OceanTextureOpacityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(OceanTextureOpacityTrack.Value.ToString(), OceanTextureGroup, new Point(OceanTextureOpacityTrack.Right - 30, OceanTextureOpacityTrack.Top - 20), 2000);

            Bitmap b = new(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);

            if (b != null)
            {
                Bitmap newB = DrawingMethods.SetBitmapOpacity(b, OceanTextureOpacityTrack.Value / 100F);
                OceanTextureBox.Image = newB;
            }

            OceanTextureBox.Refresh();
        }


        private void PreviousOceanTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_OCEAN_TEXTURE_INDEX > 0)
            {
                AssetManager.SELECTED_OCEAN_TEXTURE_INDEX--;
            }

            if (AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);
            }

            Bitmap b = new(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);

            if (b != null)
            {
                Bitmap newB = DrawingMethods.SetBitmapOpacity(b, OceanTextureOpacityTrack.Value / 100F);
                OceanTextureBox.Image = newB;
            }

            OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureName;
        }

        private void NextOceanTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_OCEAN_TEXTURE_INDEX < AssetManager.WATER_TEXTURE_LIST.Count - 1)
            {
                AssetManager.SELECTED_OCEAN_TEXTURE_INDEX++;
            }

            if (AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);
            }

            Bitmap b = new(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);

            if (b != null)
            {
                Bitmap newB = DrawingMethods.SetBitmapOpacity(b, OceanTextureOpacityTrack.Value / 100F);
                OceanTextureBox.Image = newB;
            }

            OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureName;
        }

        private void OceanScaleTextureTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show((OceanScaleTextureTrack.Value / 100.0F).ToString(), OceanTextureGroup, new Point(OceanScaleTextureTrack.Right - 30, OceanScaleTextureTrack.Top - 20), 2000);
        }

        private void OceanApplyTextureButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count < 1
                && OceanTextureBox.Image != null)
            {
                float scale = OceanScaleTextureTrack.Value / 100.0F;
                bool mirrorBackground = MirrorOceanTextureSwitch.Checked;
                Bitmap? textureBitmap = (Bitmap?)OceanTextureBox.Image;

                OceanMethods.ApplyOceanTexture(MapStateMediator.CurrentMap, textureBitmap, scale, mirrorBackground);
            }

            SKGLRenderControl.Invalidate();
        }
        private void OceanRemoveTextureButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerTexture = (MapImage)MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER).MapLayerComponents.First();

                Cmd_ClearOceanTexture cmd = new(MapStateMediator.CurrentMap, layerTexture);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void OceanColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, OceanColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                OceanColorSelectButton.BackColor = selectedColor;
                OceanColorSelectButton.Refresh();
            }
        }

        private void OceanColorFillButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;

            // get the user-selected ocean color
            Color fillColor = OceanColorSelectButton.BackColor;
            SKBitmap b = new(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

            using (SKCanvas canvas = new(b))
            {
                canvas.Clear(Extensions.ToSKColor(fillColor));
            }

            Cmd_SetOceanColor cmd = new(MapStateMediator.CurrentMap, b);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            SKGLRenderControl.Invalidate();
        }

        private void OceanColorClearButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;

            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (oceanTextureOverlayLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerColor = (MapImage)MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTUREOVERLAYLAYER).MapLayerComponents.First();

                Cmd_ClearOceanColor cmd = new(MapStateMediator.CurrentMap, layerColor);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void OceanPaintButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.OceanPaint, OceanMethods.OceanPaintBrushSize);
        }

        private void OceanColorEraseButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.OceanErase, OceanMethods.OceanPaintBrushSize);
        }

        private void OceanPaintColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, OceanPaintColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                OceanPaintColorSelectButton.BackColor = selectedColor;
                OceanPaintColorSelectButton.Refresh();
            }
        }

        private void OceanSoftBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void OceanHardBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void OceanBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMethods.OceanPaintBrushSize = OceanBrushSizeTrack.Value;
            TOOLTIP.Show(OceanMethods.OceanPaintBrushSize.ToString(), OceanToolPanel, new Point(OceanBrushSizeTrack.Right - 30, OceanBrushSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = OceanMethods.OceanPaintBrushSize;
        }

        private void OceanEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMethods.OceanPaintEraserSize = OceanEraserSizeTrack.Value;
            TOOLTIP.Show(OceanMethods.OceanPaintEraserSize.ToString(), OceanToolPanel, new Point(OceanEraserSizeTrack.Right - 30, OceanEraserSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = OceanMethods.OceanPaintEraserSize;
        }

        private void OceanBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((OceanBrushVelocityTrack.Value / 100.0F).ToString(), OceanToolPanel, new Point(OceanBrushVelocityTrack.Right - 30, OceanBrushVelocityTrack.Top - 20), 2000);
            MainUIMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (OceanBrushVelocityTrack.Value / 100.0));
        }

        private void OceanAddColorPresetButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, OceanPaintColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                Color oceanColor = selectedColor;

                if (OceanCustomColorButton1.Text == "")
                {
                    OceanCustomColorButton1.BackColor = oceanColor;
                    OceanCustomColorButton1.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton1.Refresh();
                }
                else if (OceanCustomColorButton2.Text == "")
                {
                    OceanCustomColorButton2.BackColor = oceanColor;
                    OceanCustomColorButton2.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton2.Refresh();
                }
                else if (OceanCustomColorButton3.Text == "")
                {
                    OceanCustomColorButton3.BackColor = oceanColor;
                    OceanCustomColorButton3.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton3.Refresh();
                }
                else if (OceanCustomColorButton4.Text == "")
                {
                    OceanCustomColorButton4.BackColor = oceanColor;
                    OceanCustomColorButton4.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton4.Refresh();
                }
                else if (OceanCustomColorButton5.Text == "")
                {
                    OceanCustomColorButton5.BackColor = oceanColor;
                    OceanCustomColorButton5.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton5.Refresh();
                }
                else if (OceanCustomColorButton6.Text == "")
                {
                    OceanCustomColorButton6.BackColor = oceanColor;
                    OceanCustomColorButton6.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton6.Refresh();
                }
                else if (OceanCustomColorButton7.Text == "")
                {
                    OceanCustomColorButton7.BackColor = oceanColor;
                    OceanCustomColorButton7.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton7.Refresh();
                }
                else if (OceanCustomColorButton8.Text == "")
                {
                    OceanCustomColorButton8.BackColor = oceanColor;
                    OceanCustomColorButton8.Text = ColorTranslator.ToHtml(oceanColor);
                    OceanCustomColorButton8.Refresh();
                }
            }
        }
        private void OceanColorPickerButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.ColorSelect, 0);
            Cursor = AssetManager.EYEDROPPER_CURSOR;
        }

        private void OceanButton91CBB8_Click(object sender, EventArgs e)
        {
            SetOceanColorFromPreset("#91CBB8");
        }

        private void OceanButton88B5BB_Click(object sender, EventArgs e)
        {
            SetOceanColorFromPreset("#88B5BB");
        }

        private void OceanButton6BA5B9_Click(object sender, EventArgs e)
        {
            SetOceanColorFromPreset("#6BA5B9");
        }

        private void OceanButton42718D_Click(object sender, EventArgs e)
        {
            SetOceanColorFromPreset("#42718D");
        }

        private void OceanCustomColorButton1_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton2_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton3_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton4_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton5_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton6_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton7_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void OceanCustomColorButton8_Click(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }
        #endregion

        #region Land Tab Event Handlers
        /******************************************************************************************************* 
        * LAND TAB EVENT HANDLERS
        *******************************************************************************************************/
        private void ShowLandLayerSwitch_CheckedChanged()
        {
            MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDCOASTLINELAYER);
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDDRAWINGLAYER);

            landCoastlineLayer.ShowLayer = ShowLandLayerSwitch.Checked;

            landformLayer.ShowLayer = ShowLandLayerSwitch.Checked;

            landDrawingLayer.ShowLayer = ShowLandLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void LandformSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LandformSelect, 0);
        }

        private void LandformPaintButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LandPaint, LandformMethods.LandformBrushSize);
        }

        private void LandformEraseButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LandErase, LandformMethods.LandformEraserSize);
        }

        private void LandBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            MainUIMediator.SelectedBrushSize = LandformMethods.LandformEraserSize;
            LandformMethods.LandformBrushSize = LandBrushSizeTrack.Value;
            TOOLTIP.Show(LandBrushSizeTrack.Value.ToString(), LandformValuesGroup, new Point(LandBrushSizeTrack.Right - 30, LandBrushSizeTrack.Top - 20), 2000);
        }

        private void LandformOutlineColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, LandformOutlineColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                LandformOutlineColorSelectButton.BackColor = selectedColor;
                LandformOutlineColorSelectButton.Refresh();
            }
        }

        private void LandformOutlineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(LandformOutlineWidthTrack.Value.ToString(), LandformValuesGroup, new Point(LandformOutlineWidthTrack.Right - 30, LandformOutlineWidthTrack.Top - 20), 2000);
        }

        private void LandformBackgroundColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, LandformOutlineColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                LandformBackgroundColorSelectButton.BackColor = selectedColor;
                LandformBackgroundColorSelectButton.Refresh();
            }
        }

        private void PreviousTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_LAND_TEXTURE_INDEX > 0)
            {
                AssetManager.SELECTED_LAND_TEXTURE_INDEX--;
            }

            if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
            }

            LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap;
            LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureName;
        }

        private void NextTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_LAND_TEXTURE_INDEX < AssetManager.LAND_TEXTURE_LIST.Count - 1)
            {
                AssetManager.SELECTED_LAND_TEXTURE_INDEX++;
            }

            if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
            }

            LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap;
            LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureName;
        }

        private void UseTextureForBackgroundCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (MapStateMediator.CurrentLandform != null)
            {
                MapStateMediator.CurrentLandform.FillWithTexture = UseTextureForBackgroundCheck.Checked;
                SKGLRenderControl.Invalidate();
            }
        }

        private void CoastlineEffectDistanceTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(CoastlineEffectDistanceTrack.Value.ToString(), CoastlineValuesGroup, new Point(CoastlineEffectDistanceTrack.Right - 30, CoastlineEffectDistanceTrack.Top - 20), 2000);
        }

        private void CoastlineColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, CoastlineColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                CoastlineColorSelectionButton.BackColor = selectedColor;
                CoastlineColorSelectionButton.Refresh();
            }
        }

        private void LandEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformEraserSize = LandEraserSizeTrack.Value;
            TOOLTIP.Show(LandEraserSizeTrack.Value.ToString(), LandEraserGroup, new Point(LandEraserSizeTrack.Right - 30, LandEraserSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = LandformMethods.LandformEraserSize;

        }

        private void LandformFillButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LANDFORMLAYER);

            if (landformLayer.MapLayerComponents.Count > 0)
            {
                MessageBox.Show("Landforms have already been drawn. Please clear them before filling the map.", "Landforms Already Drawn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                Landform landform = new()
                {
                    ParentMap = MapStateMediator.CurrentMap,
                    Width = MapStateMediator.CurrentMap.MapWidth,
                    Height = MapStateMediator.CurrentMap.MapHeight,
                    LandformRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                        new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight)),
                    CoastlineRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                        new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight))
                };

                SetLandformData(landform);

                LandformMethods.FillMapWithLandForm(MapStateMediator.CurrentMap, landform);

                MapStateMediator.CurrentMap.IsSaved = false;
                SKGLRenderControl.Invalidate();
            }
        }

        private void LandformClearButton_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("This action will clear all landform drawing and any drawn landforms.\nPlease confirm.", "Clear All?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (confirmResult != DialogResult.OK)
            {
                return;
            }

            MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            Cmd_ClearAllLandforms cmd = new(MapStateMediator.CurrentMap);

            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            SKGLRenderControl.Invalidate();
        }

        private void LandColorButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LandColor, LandformMethods.LandformColorBrushSize);
        }

        private void LandColorEraseButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LandColorErase, LandformMethods.LandformColorEraserBrushSize);
        }

        private void LandColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, LandColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                LandColorSelectionButton.BackColor = selectedColor;
                LandColorSelectionButton.Refresh();
            }
        }

        private void LandAddColorPresetButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, Color.Empty);

            if (selectedColor != Color.Empty)
            {
                Color landColor = selectedColor;

                if (LandCustomColorButton1.Text == "")
                {
                    LandCustomColorButton1.BackColor = landColor;
                    LandCustomColorButton1.Text = ColorTranslator.ToHtml(landColor);
                    LandCustomColorButton1.Refresh();
                }
                else if (LandCustomColorButton2.Text == "")
                {
                    LandCustomColorButton2.BackColor = landColor;
                    LandCustomColorButton2.Text = ColorTranslator.ToHtml(landColor);
                    LandCustomColorButton2.Refresh();
                }
                else if (LandCustomColorButton3.Text == "")
                {
                    LandCustomColorButton3.BackColor = landColor;
                    LandCustomColorButton3.Text = ColorTranslator.ToHtml(landColor);
                    LandCustomColorButton3.Refresh();
                }
                else if (LandCustomColorButton4.Text == "")
                {
                    LandCustomColorButton4.BackColor = landColor;
                    LandCustomColorButton4.Text = ColorTranslator.ToHtml(landColor);
                    LandCustomColorButton4.Refresh();
                }
                else if (LandCustomColorButton5.Text == "")
                {
                    LandCustomColorButton5.BackColor = landColor;
                    LandCustomColorButton5.Text = ColorTranslator.ToHtml(landColor);
                    LandCustomColorButton5.Refresh();
                }
                else if (LandCustomColorButton6.Text == "")
                {
                    LandCustomColorButton6.BackColor = landColor;
                    LandCustomColorButton6.Text = ColorTranslator.ToHtml(landColor);
                    LandCustomColorButton6.Refresh();
                }
            }
        }

        private void LandColorSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.ColorSelect, 0);
            Cursor = AssetManager.EYEDROPPER_CURSOR;
        }

        private void LandButtonE6D0AB_Click(object sender, EventArgs e)
        {
            SetLandColorFromPreset("#E6D0AB");
        }

        private void LandButtonD8B48F_Click(object sender, EventArgs e)
        {
            SetLandColorFromPreset("#D8B48F");
        }

        private void LandButtonBEBB8E_Click(object sender, EventArgs e)
        {
            SetLandColorFromPreset("#BEBB8E");
        }

        private void LandButtonD7C293_Click(object sender, EventArgs e)
        {
            SetLandColorFromPreset("#D7C293");
        }

        private void LandButtonAD9C7E_Click(object sender, EventArgs e)
        {
            SetLandColorFromPreset("#AD9C7E");
        }

        private void LandButton3D3728_Click(object sender, EventArgs e)
        {
            SetLandColorFromPreset("#3D3728");
        }

        private void LandCustomColorButton1_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void LandCustomColorButton2_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void LandCustomColorButton3_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void LandCustomColorButton4_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void LandCustomColorButton5_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void LandCustomColorButton6_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void LandSoftBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void LandHardBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void LandColorBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformColorBrushSize = LandColorBrushSizeTrack.Value;
            TOOLTIP.Show(LandformMethods.LandformColorBrushSize.ToString(), LandToolPanel, new Point(LandColorBrushSizeTrack.Right - 30, LandColorBrushSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = LandformMethods.LandformColorBrushSize;
        }

        private void LandBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((LandBrushVelocityTrack.Value / 100.0F).ToString(), LandToolPanel, new Point(LandBrushVelocityTrack.Right - 30, LandBrushVelocityTrack.Top - 20), 2000);
            MainUIMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (LandBrushVelocityTrack.Value / 100.0));
        }

        private void LandColorEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformColorEraserBrushSize = LandColorEraserSizeTrack.Value;
            TOOLTIP.Show(LandformMethods.LandformColorEraserBrushSize.ToString(), LandToolPanel, new Point(LandColorEraserSizeTrack.Right - 20, LandColorEraserSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = LandformMethods.LandformColorEraserBrushSize;
        }

        #endregion

        #region Landform Generation

        private void LandformGenerateButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);

            SKGLRenderControl.Invalidate();

            Cursor = Cursors.WaitCursor;

            GetSelectedLandformType();

            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

            RealmGenerationMethods.GenerateRandomLandform(MapStateMediator.CurrentMap, SKGLRenderControl, MapStateMediator.SelectedRealmArea, MapStateMediator.GeneratedLandformType);

            MapStateMediator.SelectedRealmArea = SKRect.Empty;

            MapStateMediator.CurrentMap.IsSaved = false;
            Cursor = Cursors.Default;

            SKGLRenderControl.Invalidate();
        }

        internal void GetSelectedLandformType()
        {
            if (RegionMenuItem.Checked)
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.Region;
            }
            else if (ContinentMenuItem.Checked)
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.Continent;
            }
            else if (IslandMenuItem.Checked)
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.Island;
            }
            else if (ArchipelagoMenuItem.Checked)
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.Archipelago;
            }
            else if (AtollMenuItem.Checked)
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.Atoll;
            }
            else if (WorldMenuItem.Checked)
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.World;
            }
            else
            {
                MapStateMediator.GeneratedLandformType = GeneratedLandformType.NotSet;
            }
        }

        private void RegionMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            RegionMenuItem.Checked = true;
            MapStateMediator.GeneratedLandformType = GeneratedLandformType.Region;
        }

        private void ContinentMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            ContinentMenuItem.Checked = true;
            MapStateMediator.GeneratedLandformType = GeneratedLandformType.Continent;
        }

        private void IslandMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            IslandMenuItem.Checked = true;
            MapStateMediator.GeneratedLandformType = GeneratedLandformType.Island;
        }

        private void ArchipelagoMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            ArchipelagoMenuItem.Checked = true;
            MapStateMediator.GeneratedLandformType = GeneratedLandformType.Archipelago;
        }

        private void AtollMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            AtollMenuItem.Checked = true;
            MapStateMediator.GeneratedLandformType = GeneratedLandformType.Atoll;
        }

        private void WorldMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            WorldMenuItem.Checked = true;
            MapStateMediator.GeneratedLandformType = GeneratedLandformType.World;
        }

        #endregion

        #region Landform HeightMap

        private void HeightMapLandformSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LandformHeightMapSelect, 0);
        }

        private void HeightUpButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.MapHeightIncrease, LandformMethods.LandformBrushSize);
        }

        private void HeightDownButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.MapHeightDecrease, LandformMethods.LandformBrushSize);
        }

        private void Show3DViewButton_Click(object sender, EventArgs e)
        {
            SKBitmap? heightMapBitmap = MapHeightMapMethods.GetBitmapForThreeDView(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, MapStateMediator.SelectedRealmArea);
            if (heightMapBitmap != null)
            {
                Cursor.Current = Cursors.WaitCursor;

                ThreeDView td = new("Height Map 3D View", LandFormObjModelList);
                CurrentHeightMapView = td;
                td.Show();

                Task updateTask = Task.Run(() =>
                {
                    // generate the 3D model from the height information in the selected area
                    List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                    // convert the list of strings to a single string
                    string objModelString = string.Join(Environment.NewLine, objModelStringList);

                    lock (LandFormObjModelList) // Ensure thread safety
                    {
                        LandFormObjModelList.Add(objModelString);
                    }
                });

                updateTask.Wait();
                CurrentHeightMapView.UpdateView();
            }
        }

        #endregion

        #region Landform Methods
        /******************************************************************************************************* 
        * LANDFORM METHODS
        *******************************************************************************************************/

        internal void SetLandformData(Landform landform)
        {
            landform.FillWithTexture = UseTextureForBackgroundCheck.Checked;

            // set the landform data from values of the UI controls
            if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
            }

            landform.LandformTexture = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX];

            landform.LandformBackgroundColor = LandformBackgroundColorSelectButton.BackColor;

            if (!landform.FillWithTexture || landform.LandformTexture == null || landform.LandformTexture.TextureBitmap == null)
            {
                landform.LandformFillPaint.Shader?.Dispose();
                landform.LandformFillPaint.Shader = null;

                SKShader flpShader = SKShader.CreateColor(landform.LandformBackgroundColor.ToSKColor());
                landform.LandformFillPaint.Shader = flpShader;
            }
            else
            {
                Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight);

                // create and set a shader from the texture
                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                landform.LandformFillPaint.Shader = flpShader;
            }

            MapTexture? dashTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Watercolor Dashes");

            if (dashTexture != null)
            {
                dashTexture.TextureBitmap ??= new Bitmap(dashTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(dashTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                landform.DashShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            MapTexture? lineHatchTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Line Hatch");

            if (lineHatchTexture != null)
            {
                lineHatchTexture.TextureBitmap ??= new Bitmap(lineHatchTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(lineHatchTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                landform.LineHatchBitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            landform.LandformOutlineColor = LandformOutlineColorSelectButton.BackColor;
            landform.LandformOutlineWidth = LandformOutlineWidthTrack.Value;

            landform.LandformFillColor = Color.FromArgb(landform.LandformOutlineColor.A / 4, landform.LandformOutlineColor);

            landform.CoastlineColor = CoastlineColorSelectionButton.BackColor;

            landform.CoastlineEffectDistance = CoastlineEffectDistanceTrack.Value;

            if (CoastlineStyleList.Items != null
                && CoastlineStyleList.SelectedIndex > 0
                && CoastlineStyleList.Items[CoastlineStyleList.SelectedIndex] != null
                && !string.IsNullOrEmpty(CoastlineStyleList.Items[CoastlineStyleList.SelectedIndex].ToString()))
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                landform.CoastlineStyleName = CoastlineStyleList.Items[CoastlineStyleList.SelectedIndex].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
            }

            landform.IsModified = true;
        }

        private void SetLandPaintColorFromCustomPresetButton(Button b)
        {
            if (b.Text != "")
            {
                Color landColor = b.BackColor;

                LandColorSelectionButton.BackColor = landColor;

                LandColorSelectionButton.Refresh();
            }
        }

        private void SetLandColorFromPreset(string htmlColor)
        {
            Color landColor = ColorTranslator.FromHtml(htmlColor);

            LandColorSelectionButton.BackColor = landColor;

            LandColorSelectionButton.Refresh();
        }

        private void UncheckAllLandformTypeMenuItems()
        {
            RegionMenuItem.Checked = false;
            ContinentMenuItem.Checked = false;
            IslandMenuItem.Checked = false;
            ArchipelagoMenuItem.Checked = false;
            AtollMenuItem.Checked = false;
            WorldMenuItem.Checked = false;
        }

        #endregion

        #region Windrose Event Handlers

        private void WindrosePlaceButton_Click(object sender, EventArgs e)
        {
            if (MainUIMediator.CurrentDrawingMode != MapDrawingMode.PlaceWindrose)
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.PlaceWindrose, 0);
                MapStateMediator.CurrentWindrose = CreateWindrose();
            }
            else
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);
                MapStateMediator.CurrentWindrose = null;
            }

            SKGLRenderControl.Invalidate();
        }

        private void WindroseColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, WindroseColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                WindroseColorSelectButton.BackColor = selectedColor;
                WindroseColorSelectButton.Refresh();

                UpdateCurrentWindrose();
            }
        }

        private void WindroseClearButton_Click(object sender, EventArgs e)
        {
            for (int i = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WINDROSELAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WINDROSELAYER).MapLayerComponents[i] is MapWindrose)
                {
                    MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WINDROSELAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            SKGLRenderControl.Invalidate();
        }

        private void WindroseDirectionsUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateCurrentWindrose();
        }

        private void WindroseLineWidthUpDOwn_ValueChanged(object sender, EventArgs e)
        {
            UpdateCurrentWindrose();
        }

        private void WindroseInnerRadiusUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateCurrentWindrose();
        }

        private void WindroseOuterRadiusUpDown_VisibleChanged(object sender, EventArgs e)
        {
            UpdateCurrentWindrose();
        }

        private void WindroseInnerCircleTrack_ValueChanged(object sender, EventArgs e)
        {
            UpdateCurrentWindrose();
        }

        private void WindroseFadeOutSwitch_CheckedChanged()
        {
            UpdateCurrentWindrose();
        }
        #endregion

        #region Windrose Methods

        private MapWindrose CreateWindrose()
        {
            MapWindrose windrose = new()
            {
                ParentMap = MapStateMediator.CurrentMap,
                InnerCircles = WindroseInnerCircleTrack.Value,
                InnerRadius = (int)WindroseInnerRadiusUpDown.Value,
                FadeOut = WindroseFadeOutSwitch.Checked,
                LineWidth = (int)WindroseLineWidthUpDown.Value,
                OuterRadius = (int)WindroseOuterRadiusUpDown.Value,
                WindroseColor = WindroseColorSelectButton.BackColor,
                DirectionCount = (int)WindroseDirectionsUpDown.Value,
            };

            windrose.WindrosePaint = new()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = windrose.LineWidth,
                Color = windrose.WindroseColor.ToSKColor(),
                IsAntialias = true,
            };

            return windrose;
        }

        private void UpdateCurrentWindrose()
        {
            if (MapStateMediator.CurrentWindrose != null)
            {
                MapStateMediator.CurrentWindrose.InnerCircles = WindroseInnerCircleTrack.Value;
                MapStateMediator.CurrentWindrose.InnerRadius = (int)WindroseInnerRadiusUpDown.Value;
                MapStateMediator.CurrentWindrose.FadeOut = WindroseFadeOutSwitch.Checked;
                MapStateMediator.CurrentWindrose.LineWidth = (int)WindroseLineWidthUpDown.Value;
                MapStateMediator.CurrentWindrose.OuterRadius = (int)WindroseOuterRadiusUpDown.Value;
                MapStateMediator.CurrentWindrose.WindroseColor = WindroseColorSelectButton.BackColor;
                MapStateMediator.CurrentWindrose.DirectionCount = (int)WindroseDirectionsUpDown.Value;

                MapStateMediator.CurrentWindrose.WindrosePaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = MapStateMediator.CurrentWindrose.LineWidth,
                    Color = MapStateMediator.CurrentWindrose.WindroseColor.ToSKColor(),
                    IsAntialias = true,
                };
            }
        }

        #endregion

        #region Water Tab Event Handlers

        private void ShowWaterLayerSwitch_CheckedChanged()
        {
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WATERLAYER);
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WATERDRAWINGLAYER);

            waterLayer.ShowLayer = ShowWaterLayerSwitch.Checked;

            waterDrawingLayer.ShowLayer = ShowWaterLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void WaterFeatureSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.WaterFeatureSelect, 0);
        }

        private void WaterFeaturePaintButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.WaterPaint, 0);
        }

        private void WaterFeatureLakeButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LakePaint, 0);
        }

        private void WaterFeatureRiverButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.RiverPaint, 0);
        }

        private void WaterFeatureEraseButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.WaterErase, WaterFeatureMethods.WaterFeatureEraserSize);
        }

        private void WaterBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterFeatureBrushSize = WaterBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureBrushSize.ToString(), WaterValuesGroup, new Point(WaterBrushSizeTrack.Right - 30, WaterBrushSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = WaterFeatureMethods.WaterFeatureBrushSize;
        }

        private void WaterColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, WaterColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                WaterColorSelectionButton.BackColor = selectedColor;
                WaterColorSelectionButton.Refresh();
            }
        }

        private void ShorelineColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, ShorelineColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                ShorelineColorSelectionButton.BackColor = selectedColor;
                ShorelineColorSelectionButton.Refresh();
            }
        }

        private void RiverWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(RiverWidthTrack.Value.ToString(), RiverValuesGroup, new Point(RiverWidthTrack.Right - 30, RiverWidthTrack.Top - 20), 2000);
        }

        private void RiverTextureSwitch_CheckedChanged()
        {
            if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river)
            {
                river.RenderRiverTexture = RiverTextureSwitch.Checked;
                WaterFeatureMethods.ConstructRiverPaintObjects(river);

                SKGLRenderControl.Invalidate();
            }
        }

        private void EditRiverPointsSwitch_CheckedChanged()
        {
            if (EditRiverPointsSwitch.Checked)
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.RiverEdit, 0);

                if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river)
                {
                    river.ShowRiverPoints = true;
                }
            }
            else
            {
                if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river)
                {
                    river.ShowRiverPoints = false;
                }
            }

            SKGLRenderControl.Invalidate();
        }

        private void WaterEraseSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterFeatureEraserSize = WaterEraserSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureEraserSize.ToString(), WaterEraserGroup, new Point(WaterEraserSizeTrack.Right - 30, WaterEraserSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = WaterFeatureMethods.WaterFeatureEraserSize;
        }

        private void WaterSoftBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void WaterHardBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void WaterColorBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterColorBrushSize = WaterColorBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterColorBrushSize.ToString(), WaterToolPanel, new Point(WaterColorBrushSizeTrack.Right - 30, WaterColorBrushSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = WaterFeatureMethods.WaterColorBrushSize;
        }

        private void WaterBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((WaterBrushVelocityTrack.Value / 100.0F).ToString(), WaterToolPanel, new Point(WaterBrushVelocityTrack.Right - 30, WaterBrushVelocityTrack.Top - 20), 2000);
            MainUIMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (WaterBrushVelocityTrack.Value / 100.0));
        }

        private void WaterColorEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterColorEraserSize = WaterColorEraserSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterColorEraserSize.ToString(), WaterToolPanel, new Point(WaterColorEraserSizeTrack.Right - 30, WaterColorEraserSizeTrack.Top - 20), 2000);
            MainUIMediator.SelectedBrushSize = WaterFeatureMethods.WaterColorEraserSize;
        }

        private void WaterColorButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.WaterColor, WaterFeatureMethods.WaterColorBrushSize);
        }

        private void WaterColorEraseButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.WaterColorErase, WaterFeatureMethods.WaterColorEraserSize);
        }

        private void WaterPaintColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, WaterPaintColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                WaterPaintColorSelectButton.BackColor = selectedColor;

                WaterPaintColorSelectButton.Refresh();
            }
        }

        private void WaterButton91CBB8_Click(object sender, EventArgs e)
        {
            SetWaterColorFromPreset("#91CBB8");
        }

        private void WaterButton88B5BB_Click(object sender, EventArgs e)
        {
            SetWaterColorFromPreset("#88B5BB");
        }

        private void WaterButton6BA5B9_Click(object sender, EventArgs e)
        {
            SetWaterColorFromPreset("#6BA5B9");
        }

        private void WaterButton42718D_Click(object sender, EventArgs e)
        {
            SetWaterColorFromPreset("#42718D");
        }

        private void WaterCustomColor1_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor2_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor3_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor4_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor5_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor6_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor7_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        private void WaterCustomColor8_MouseClick(object sender, MouseEventArgs e)
        {
            ColorPresetButtonMouseClickHandler(sender, e);
        }

        #endregion

        #region Water Tab Methods

        internal MapComponent? SelectWaterFeatureAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            MapComponent? selectedWaterFeature = null;

            List<MapComponent> waterFeatureComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER).MapLayerComponents;

            for (int i = 0; i < waterFeatureComponents.Count; i++)
            {
                if (waterFeatureComponents[i] is WaterFeature waterFeature)
                {
                    SKPath boundaryPath = waterFeature.WaterFeaturePath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            waterFeature.IsSelected = !waterFeature.IsSelected;

                            if (waterFeature.IsSelected)
                            {
                                selectedWaterFeature = waterFeature;
                            }
                            break;
                        }
                    }
                }
                else if (waterFeatureComponents[i] is River river)
                {
                    SKPath? boundaryPath = river.RiverBoundaryPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            river.IsSelected = !river.IsSelected;

                            if (river.IsSelected)
                            {
                                selectedWaterFeature = river;
                            }
                            else
                            {
                                EditRiverPointsSwitch.Checked = false;
                                river.ShowRiverPoints = false;
                            }
                            break;
                        }
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedWaterFeature);
            return selectedWaterFeature;
        }

        private void SetWaterColorFromPreset(string htmlColor)
        {
            Color waterColor = ColorTranslator.FromHtml(htmlColor);

            WaterPaintColorSelectButton.BackColor = waterColor;
            WaterPaintColorSelectButton.Refresh();
        }

        private void SetWaterPaintColorFromCustomPresetButton(Button b)
        {
            if (b.Text != "")
            {
                Color waterColor = b.BackColor;

                WaterPaintColorSelectButton.BackColor = waterColor;

                WaterPaintColorSelectButton.Refresh();
            }
        }
        #endregion

        #region Path Tab Event Handlers

        private void ShowPathLayerSwitch_CheckedChanged()
        {
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);

            pathLowerLayer.ShowLayer = ShowPathLayerSwitch.Checked;

            pathUpperLayer.ShowLayer = ShowPathLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void PathSelectButton_Click(object sender, EventArgs e)
        {
            if (MainUIMediator.CurrentDrawingMode != MapDrawingMode.PathSelect && MainUIMediator.CurrentDrawingMode != MapDrawingMode.PathEdit)
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.PathSelect, 0);
            }
            else
            {
                MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);
            }

            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathSelect)
            {
                if (EditPathPointSwitch.Checked)
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }
                }
                else
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathEdit)
            {
                if (EditPathPointSwitch.Checked)
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }
                }
                else
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else
            {
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }
            }

            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.None)
            {
                MapStateMediator.SelectedMapPath = null;
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }
            }

            MainUIMediator.SetDrawingMode(MainUIMediator.CurrentDrawingMode, MainUIMediator.SelectedBrushSize);
        }

        private void DrawPathButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
            MainUIMediator.SetDrawingMode(MapDrawingMode.PathPaint, 0);

            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
            foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }

            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
            foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }
        }

        private void PathWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(PathWidthTrack.Value.ToString(), MapPathValuesGroup, new Point(PathWidthTrack.Right - 30, PathWidthTrack.Top - 20), 2000);
        }

        private void PathColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, PathColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                PathColorSelectButton.BackColor = selectedColor;
            }
        }

        private void EditPathPointSwitch_CheckedChanged()
        {
            Cursor = Cursors.Default;
            MainUIMediator.SelectedBrushSize = 0;

            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathSelect)
            {
                if (EditPathPointSwitch.Checked)
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }
                }
                else
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.PathEdit)
            {
                if (EditPathPointSwitch.Checked)
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }
                }
                else
                {
                    MainUIMediator.CurrentDrawingMode = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else
            {
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }
            }

            if (MainUIMediator.CurrentDrawingMode == MapDrawingMode.None)
            {
                MapStateMediator.SelectedMapPath = null;
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }
            }

            MainUIMediator.SetDrawingMode(MainUIMediator.CurrentDrawingMode, MainUIMediator.SelectedBrushSize);
        }

        private void PreviousPathTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_PATH_TEXTURE_INDEX > 0)
            {
                AssetManager.SELECTED_PATH_TEXTURE_INDEX--;
            }

            if (AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TexturePath);
            }

            PathTexturePreviewPicture.Image = AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap;
            PathTextureNameLabel.Text = AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureName;
        }

        private void NextPathTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_PATH_TEXTURE_INDEX < AssetManager.PATH_TEXTURE_LIST.Count - 1)
            {
                AssetManager.SELECTED_PATH_TEXTURE_INDEX++;
            }

            if (AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TexturePath);
            }

            PathTexturePreviewPicture.Image = AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap;
            PathTextureNameLabel.Text = AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureName;
        }

        private void SolidLinePictureBox_Click(object sender, EventArgs e)
        {
            SolidLineRadio.Checked = !SolidLineRadio.Checked;
        }

        private void DottedLinePictureBox_Click(object sender, EventArgs e)
        {
            DottedLineRadio.Checked = !DottedLineRadio.Checked;
        }

        private void DashedLinePictureBox_Click(object sender, EventArgs e)
        {
            DashedLineRadio.Checked = !DashedLineRadio.Checked;
        }

        private void DashDotPictureBox_Click(object sender, EventArgs e)
        {
            DashDotLineRadio.Checked = !DashDotLineRadio.Checked;
        }

        private void DashDotDotPictureBox_Click(object sender, EventArgs e)
        {
            DashDotDotLineRadio.Checked = !DashDotDotLineRadio.Checked;
        }

        private void DoubleSolidBorderPictureBox_Click(object sender, EventArgs e)
        {
            DoubleSolidBorderRadio.Checked = !DoubleSolidBorderRadio.Checked;
        }

        private void ChevronPictureBox_Click(object sender, EventArgs e)
        {
            ChevronLineRadio.Checked = !ChevronLineRadio.Checked;
        }

        private void LineDashPictureBox_Click(object sender, EventArgs e)
        {
            LineAndDashesRadio.Checked = !LineAndDashesRadio.Checked;
        }

        private void SmallDashesPictureBox_Click(object sender, EventArgs e)
        {
            SmallDashesRadio.Checked = !SmallDashesRadio.Checked;
        }

        private void ThickLinePictureBox_Click(object sender, EventArgs e)
        {
            ThickLineRadio.Checked = !ThickLineRadio.Checked;
        }

        private void BlackBorderLinePictureBox_Click(object sender, EventArgs e)
        {
            BlackBorderPathRadio.Checked = !BlackBorderPathRadio.Checked;
        }

        private void BorderedGradientPictureBox_Click(object sender, EventArgs e)
        {
            BorderedGradientRadio.Checked = !BorderedGradientRadio.Checked;
        }

        private void BorderedLightSolidPictureBox_Click(object sender, EventArgs e)
        {
            BorderedLightSolidRadio.Checked = !BorderedLightSolidRadio.Checked;
        }

        private void BearTracksPictureBox_Click(object sender, EventArgs e)
        {
            BearTracksRadio.Checked = !BearTracksRadio.Checked;
        }

        private void BirdTracksPictureBox_Click(object sender, EventArgs e)
        {
            BirdTracksRadio.Checked = !BirdTracksRadio.Checked;
        }

        private void FootPrintsPictureBox_Click(object sender, EventArgs e)
        {
            FootPrintsRadio.Checked = !FootPrintsRadio.Checked;
        }

        private void RailroadTracksPictureBox_Click(object sender, EventArgs e)
        {
            RailroadTracksRadio.Checked = !RailroadTracksRadio.Checked;
        }

        #endregion

        #region Path Tab Methods
        private PathType GetSelectedPathType()
        {
            if (SolidLineRadio.Checked) return PathType.SolidLinePath;
            if (DottedLineRadio.Checked) return PathType.DottedLinePath;
            if (DashedLineRadio.Checked) return PathType.DashedLinePath;
            if (DashDotLineRadio.Checked) return PathType.DashDotLinePath;
            if (DashDotDotLineRadio.Checked) return PathType.DashDotDotLinePath;
            if (ChevronLineRadio.Checked) return PathType.ChevronLinePath;
            if (LineAndDashesRadio.Checked) return PathType.LineAndDashesPath;
            if (SmallDashesRadio.Checked) return PathType.ShortIrregularDashPath;
            if (ThickLineRadio.Checked) return PathType.ThickSolidLinePath;
            if (BlackBorderPathRadio.Checked) return PathType.SolidBlackBorderPath;
            if (BorderedGradientRadio.Checked) return PathType.BorderedGradientPath;
            if (BorderedLightSolidRadio.Checked) return PathType.BorderedLightSolidPath;
            if (DoubleSolidBorderRadio.Checked) return PathType.DoubleSolidBorderPath;
            if (BearTracksRadio.Checked) return PathType.BearTracksPath;
            if (BirdTracksRadio.Checked) return PathType.BirdTracksPath;
            if (FootPrintsRadio.Checked) return PathType.FootprintsPath;
            if (RailroadTracksRadio.Checked) return PathType.RailroadTracksPath;
            if (TexturePathRadio.Checked) return PathType.TexturedPath;
            if (BorderTexturePathRadio.Checked) return PathType.BorderAndTexturePath;

            return PathType.SolidLinePath;
        }

        private void SetSelectedPathType(PathType pathType)
        {
            if (pathType == PathType.SolidLinePath) { SolidLineRadio.Checked = true; return; }
            if (pathType == PathType.DottedLinePath) { DottedLineRadio.Checked = true; return; }
            if (pathType == PathType.DashedLinePath) { DashedLineRadio.Checked = true; return; }
            if (pathType == PathType.DashDotLinePath) { DashDotLineRadio.Checked = true; return; }
            if (pathType == PathType.DashDotDotLinePath) { DashDotDotLineRadio.Checked = true; return; }
            if (pathType == PathType.ChevronLinePath) { ChevronLineRadio.Checked = true; return; }
            if (pathType == PathType.LineAndDashesPath) { LineAndDashesRadio.Checked = true; return; }
            if (pathType == PathType.ShortIrregularDashPath) { SmallDashesRadio.Checked = true; return; }
            if (pathType == PathType.ThickSolidLinePath) { ThickLineRadio.Checked = true; return; }
            if (pathType == PathType.SolidBlackBorderPath) { BlackBorderPathRadio.Checked = true; return; }
            if (pathType == PathType.BorderedGradientPath) { BorderedGradientRadio.Checked = true; return; }
            if (pathType == PathType.BorderedLightSolidPath) { BorderedLightSolidRadio.Checked = true; return; }
            if (pathType == PathType.DoubleSolidBorderPath) { DoubleSolidBorderRadio.Checked = true; return; }
            if (pathType == PathType.BearTracksPath) { BearTracksRadio.Checked = true; return; }
            if (pathType == PathType.BirdTracksPath) { BirdTracksRadio.Checked = true; return; }
            if (pathType == PathType.FootprintsPath) { FootPrintsRadio.Checked = true; return; }
            if (pathType == PathType.RailroadTracksPath) { RailroadTracksRadio.Checked = true; return; }
            if (pathType == PathType.TexturedPath) { TexturePathRadio.Checked = true; return; }
            if (pathType == PathType.BorderAndTexturePath) { BorderTexturePathRadio.Checked = true; return; }
        }

        internal static MapPath? SelectMapPathAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            MapPath? selectedMapPath = null;

            List<MapComponent> mapPathUpperComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER).MapLayerComponents;

            for (int i = 0; i < mapPathUpperComponents.Count; i++)
            {
                if (mapPathUpperComponents[i] is MapPath mapPath)
                {
                    SKPath? boundaryPath = mapPath.BoundaryPath;

                    if (boundaryPath.PointCount > 0)
                    {
                        boundaryPath.GetBounds(out SKRect boundRect);

                        if (boundRect.Contains(mapClickPoint))
                        {
                            mapPath.IsSelected = !mapPath.IsSelected;
                            if (mapPath.IsSelected)
                            {
                                selectedMapPath = mapPath;
                            }
                            break;
                        }
                    }
                    else
                    {
                        mapPathUpperComponents.Remove(mapPath);
                    }
                }
            }

            List<MapComponent> mapPathLowerComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER).MapLayerComponents;

            for (int i = 0; i < mapPathLowerComponents.Count; i++)
            {
                if (mapPathLowerComponents[i] is MapPath mapPath)
                {
                    SKPath? boundaryPath = mapPath.BoundaryPath;

                    if (boundaryPath.PointCount > 0)
                    {
                        boundaryPath.GetBounds(out SKRect boundRect);

                        if (boundRect.Contains(mapClickPoint))
                        {
                            mapPath.IsSelected = !mapPath.IsSelected;
                            if (mapPath.IsSelected)
                            {
                                selectedMapPath = mapPath;
                            }
                            break;
                        }
                    }
                    else
                    {
                        mapPathLowerComponents.Remove(mapPath);
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedMapPath);
            return selectedMapPath;
        }
        #endregion

        #region Symbol Tab Event Handlers

        private void ShowSymbolLayerSwitch_CheckedChanged()
        {
            SymbolUIMediator.Enabled = ShowSymbolLayerSwitch.Checked;
        }

        private void SymbolSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolSelect, 0);
        }

        private void EraseSymbolsButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolErase, AreaBrushSizeTrack.Value);
            SymbolUIMediator.AreaBrushSize = MainUIMediator.SelectedBrushSize;
        }

        private void ColorSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.ColorSymbols();
        }

        private void StructuresSymbolButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SelectSymbolsOfType(MapSymbolType.Structure);
        }

        private void VegetationSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SelectSymbolsOfType(MapSymbolType.Vegetation);
        }

        private void TerrainSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SelectSymbolsOfType(MapSymbolType.Terrain);
        }

        private void MarkerSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SelectSymbolsOfType(MapSymbolType.Marker);
        }

        private void OtherSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SelectSymbolsOfType(MapSymbolType.Other);
        }

        private void SymbolScaleTrack_Scroll(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolScale = SymbolScaleTrack.Value;
            TOOLTIP.Show(SymbolUIMediator.SymbolScale.ToString(), SymbolScaleGroup, new Point(SymbolScaleTrack.Right - 30, SymbolScaleTrack.Top - 20), 2000);
        }

        private void SymbolScaleUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolScale = (float)SymbolScaleUpDown.Value;
        }

        private void LockSymbolScaleButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolScaleLocked = !SymbolUIMediator.SymbolScaleLocked;
        }

        private void ResetSymbolColorsButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.ResetSymbolColorButtons();
        }

        private void SymbolColor1Button_MouseUp(object sender, MouseEventArgs e)
        {
            SymbolUIMediator.ColorButtonMouseUp(sender, e);
            SKGLRenderControl.Invalidate();
        }

        private void SymbolColor2Button_MouseUp(object sender, MouseEventArgs e)
        {
            SymbolUIMediator.ColorButtonMouseUp(sender, e);
        }

        private void SymbolColor3Button_MouseUp(object sender, MouseEventArgs e)
        {
            SymbolUIMediator.ColorButtonMouseUp(sender, e);
        }

        private void AreaBrushSwitch_CheckedChanged()
        {
            SymbolUIMediator.UseAreaBrush = AreaBrushSwitch.Checked;
        }

        private void AreaBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(AreaBrushSizeTrack.Value.ToString(), SymbolPlacementGroup, new Point(AreaBrushSizeTrack.Right - 30, AreaBrushSizeTrack.Top - 20), 2000);

            MainUIMediator.SelectedBrushSize = AreaBrushSizeTrack.Value;
            SymbolUIMediator.AreaBrushSize = MainUIMediator.SelectedBrushSize;
            SKGLRenderControl.Invalidate();
        }

        private void SymbolRotationTrack_Scroll(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolRotation = SymbolRotationTrack.Value;
            TOOLTIP.Show(SymbolRotationTrack.Value.ToString(), SymbolPlacementGroup, new Point(SymbolRotationTrack.Right - 30, SymbolRotationTrack.Top - 20), 2000);
        }

        private void SymbolRotationUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolRotation = (float)SymbolRotationUpDown.Value;
        }

        private void SymbolPlacementRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolPlacementRate = (float)SymbolPlacementRateUpDown.Value;
        }

        private void SymbolPlacementDensityUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolPlacementDensity = (float)SymbolPlacementDensityUpDown.Value;
        }

        private void ResetSymbolPlacementRateButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolPlacementRate = 1.0F;
        }

        private void ResetSymbolPlacementDensityButton_Click(object sender, EventArgs e)
        {
            SymbolUIMediator.SymbolPlacementDensity = 1.0F;
        }

        private void SymbolCollectionsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SymbolUIMediator.SymbolCollectionsListItemCheck(e);
        }

        private void SymbolTagsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SymbolUIMediator.SymbolTagsListItemCheck(e);
        }

        private void SymbolSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            SymbolUIMediator.SearchSymbols(SymbolSearchTextBox.Text);
        }

        #endregion

        #region Label Tab Event Handlers

        private void ShowLabelLayerSwitch_CheckedChanged()
        {
            LabelMediator.Enabled = ShowLabelLayerSwitch.Checked;
        }

        private void LabelPresetsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PresetMediator.SelectPreset(LabelPresetsListBox.SelectedIndex);
        }

        private void AddPresetButton_Click(object sender, EventArgs e)
        {
            PresetMediator.AddNewLabelPreset();
        }

        private void RemovePresetButton_Click(object sender, EventArgs e)
        {
            PresetMediator.RemoveSelectedPreset();
        }

        private void SelectLabelFontButton_Click(object sender, EventArgs e)
        {
            FontPanelManager.FontPanelOpener = FontPanelOpener.LabelFontButton;
            FontSelectionPanel.Visible = true;
        }

        private void FontColorSelectButton_Click(object sender, EventArgs e)
        {
            Color labelColor = UtilityMethods.SelectColorFromDialog(this, FontColorSelectButton.BackColor);
            LabelMediator.LabelColor = labelColor;
        }

        private void OutlineColorSelectButton_Click(object sender, EventArgs e)
        {
            Color outlineColor = UtilityMethods.SelectColorFromDialog(this, OutlineColorSelectButton.BackColor);
            LabelMediator.OutlineColor = outlineColor;
        }

        private void OutlineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            LabelMediator.OutlineWidth = OutlineWidthTrack.Value / 10.0F;
            TOOLTIP.Show(LabelMediator.OutlineWidth.ToString(), LabelOutlineGroup, new Point(OutlineWidthTrack.Right - 30, OutlineWidthTrack.Top - 20), 2000);
        }

        private void GlowColorSelectButton_Click(object sender, EventArgs e)
        {
            Color glowColor = UtilityMethods.SelectColorFromDialog(this, GlowColorSelectButton.BackColor);
            LabelMediator.GlowColor = glowColor;
        }

        private void GlowWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            LabelMediator.GlowStrength = GlowStrengthTrack.Value;
            TOOLTIP.Show(LabelMediator.GlowStrength.ToString(), LabelGlowGroup, new Point(GlowStrengthTrack.Right - 30, GlowStrengthTrack.Top - 20), 2000);
        }

        private void LabelRotationUpDown_ValueChanged(object sender, EventArgs e)
        {
            LabelMediator.LabelRotation = (float)LabelRotationUpDown.Value;
        }

        private void LabelRotationTrack_Scroll(object sender, EventArgs e)
        {
            LabelMediator.LabelRotation = LabelRotationTrack.Value;
            TOOLTIP.Show(LabelMediator.LabelRotation.ToString(), LabelRotationGroup, new Point(LabelRotationTrack.Right - 30, LabelRotationTrack.Top - 20), 2000);
        }

        private void CircleTextPathButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.DrawArcLabelPath, 0);
        }

        private void BezierTextPathButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.DrawBezierLabelPath, 0);
        }

        private void LabelSelectButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.LabelSelect, 0);
        }

        private void PlaceLabelButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.DrawLabel, 0);
        }

        private void CreateBoxButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.DrawBox, 0);
        }

        private void SelectBoxTintButton_Click(object sender, EventArgs e)
        {
            Color boxColor = UtilityMethods.SelectColorFromDialog(this, SelectBoxTintButton.BackColor);
            BoxMediator.BoxTint = boxColor;
        }

        private void GenerateNameButton_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.None)
            {
                List<INameGenerator> generators = NAME_GENERATOR_CONFIG.GetSelectedNameGenerators();
                string generatedName = MapToolMethods.GenerateRandomPlaceName(generators);

                if (CREATING_LABEL)
                {
                    if (LABEL_TEXT_BOX != null && !LABEL_TEXT_BOX.IsDisposed)
                    {
                        LABEL_TEXT_BOX.Text = generatedName;
                        LABEL_TEXT_BOX.Refresh();
                    }
                }
            }
            else if (ModifierKeys == Keys.Shift)
            {
                NAME_GENERATOR_CONFIG.Show();
            }
        }
        #endregion


        #region Overlay Tab Event Handlers (Frame, Grid, Scale, Measure)

        private void ShowOverlayLayerSwitch_CheckedChanged()
        {
            MainUIMediator.OverlayLayerEnabled = ShowOverlayLayerSwitch.Checked;
        }

        #region Frame Event Handlers

        private void EnableFrameSwitch_CheckedChanged()
        {
            FrameMediator.FrameEnabled = EnableFrameSwitch.Checked;
        }

        private void FrameScaleTrack_ValueChanged(object sender, EventArgs e)
        {
            FrameMediator.FrameScale = FrameScaleTrack.Value;
            TOOLTIP.Show((FrameMediator.FrameScale / 100F).ToString(), FrameValuesGroup, new Point(FrameScaleTrack.Right - 30, FrameScaleTrack.Top - 20), 2000);
        }

        private void FrameTintColorSelectButton_Click(object sender, EventArgs e)
        {
            Color frameColor = UtilityMethods.SelectColorFromDialog(this, FrameTintColorSelectButton.BackColor);
            FrameMediator.FrameTint = frameColor;
        }

        #endregion

        #region Scale Event Handlers

        private void ScaleButton_Click(object sender, EventArgs e)
        {
            ScaleUIMediator.ScalePanelVisible = !ScaleUIMediator.ScalePanelVisible;
        }

        private void CreateScaleButton_Click(object sender, EventArgs e)
        {
            MapScaleManager.Create(MapStateMediator.CurrentMap, ScaleUIMediator);
            SKGLRenderControl.Invalidate();
        }

        private void DeleteScaleButton_Click(object sender, EventArgs e)
        {
            // remove any existing scale
            MapScaleManager.Delete(MapStateMediator.CurrentMap, null);
            SKGLRenderControl.Invalidate();
        }

        private void ScaleWidthTrack_Scroll(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleWidth = ScaleWidthTrack.Value;
            TOOLTIP.Show(ScaleUIMediator.ScaleWidth.ToString(), MapScaleGroupBox, new Point(ScaleWidthTrack.Right - 30, ScaleWidthTrack.Top - 20), 2000);
        }

        private void ScaleHeightTrack_Scroll(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleHeight = ScaleHeightTrack.Value;
            TOOLTIP.Show(ScaleUIMediator.ScaleHeight.ToString(), MapScaleGroupBox, new Point(ScaleHeightTrack.Right - 30, ScaleHeightTrack.Top - 20), 2000);
        }

        private void ScaleSegmentCountTrack_Scroll(object sender, EventArgs e)
        {
            ScaleUIMediator.SegmentCount = ScaleSegmentCountTrack.Value;
            TOOLTIP.Show(ScaleUIMediator.SegmentCount.ToString(), MapScaleGroupBox, new Point(ScaleSegmentCountTrack.Right - 30, ScaleSegmentCountTrack.Top - 20), 2000);
        }

        private void ScaleLineWidthTrack_Scroll(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleLineWidth = ScaleLineWidthTrack.Value;
            TOOLTIP.Show(ScaleUIMediator.ScaleLineWidth.ToString(), MapScaleGroupBox, new Point(ScaleLineWidthTrack.Right - 30, ScaleLineWidthTrack.Top - 20), 2000);
        }

        private void ScaleUnitsTextBox_TextChanged(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleUnitsText = ScaleUnitsTextBox.Text;
        }

        private void ScaleNumbersNoneRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.None;
        }

        private void ScaleNumbersEndsRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.Ends;
        }

        private void ScaleNumbersEveryOtherRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.EveryOther;
        }

        private void ScaleNumbersAllRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.All;
        }

        private void ScaleColorsResetButton_Click(object sender, EventArgs e)
        {
            ScaleUIMediator.ResetScaleColors();
        }

        private void ScaleColor1Button_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, ScaleColor1Button.BackColor);
            ScaleUIMediator.ScaleColor1 = scaleColor;
        }

        private void ScaleColor2Button_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, ScaleColor2Button.BackColor);
            ScaleUIMediator.ScaleColor2 = scaleColor;
        }

        private void ScaleColor3Button_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, ScaleColor3Button.BackColor);
            ScaleUIMediator.ScaleColor3 = scaleColor;
        }

        private void SelectScaleFontButton_Click(object sender, EventArgs e)
        {
            FontPanelManager.FontPanelOpener = FontPanelOpener.ScaleFontButton;
            FontSelectionPanel.Visible = !FontSelectionPanel.Visible;
        }

        private void SelectScaleFontColorButton_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, SelectScaleFontColorButton.BackColor);
            ScaleUIMediator.ScaleFontColor = scaleColor;
        }

        private void SelectScaleOutlineColorButton_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, SelectScaleOutlineColorButton.BackColor);
            ScaleUIMediator.ScaleNumberOutlineColor = scaleColor;
        }

        private void ScaleOutlineWidthTrack_Scroll(object sender, EventArgs e)
        {
            ScaleUIMediator.ScaleOutlineWidth = ScaleOutlineWidthTrack.Value;
            TOOLTIP.Show(ScaleUIMediator.ScaleOutlineWidth.ToString(), ScaleOutlineGroupBox, new Point(ScaleOutlineWidthTrack.Right - 30, ScaleOutlineWidthTrack.Top - 20), 2000);
        }

        #endregion

        #region Grid Event Handlers

        private void GridButton_Click(object sender, EventArgs e)
        {
            MapGridManager.Create(MapStateMediator.CurrentMap, null);
            SKGLRenderControl.Invalidate();
        }

        private void EnableGridSwitch_CheckedChanged()
        {
            GridUIMediator.GridEnabled = EnableGridSwitch.Checked;
        }

        private void SquareGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            GridUIMediator.GridType = MapGridType.Square;
        }

        private void FlatHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            GridUIMediator.GridType = MapGridType.FlatHex;
        }

        private void PointedHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            GridUIMediator.GridType = MapGridType.PointedHex;
        }

        private void GridLayerUpDown_SelectedItemChanged(object sender, EventArgs e)
        {
            GridUIMediator.GridLayerName = (string?)GridLayerUpDown.SelectedItem;
            MapGridManager.Create(MapStateMediator.CurrentMap, null);
        }

        private void GridSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            GridUIMediator.GridSize = GridSizeTrack.Value;
            TOOLTIP.Show(GridSizeTrack.Value.ToString(), GridValuesGroup, new Point(GridSizeTrack.Right - 30, GridSizeTrack.Top - 20), 2000);
        }

        private void GridLineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            GridUIMediator.GridLineWidth = GridLineWidthTrack.Value;
            TOOLTIP.Show(GridLineWidthTrack.Value.ToString(), GridValuesGroup, new Point(GridLineWidthTrack.Right - 30, GridLineWidthTrack.Top - 20), 2000);
        }

        private void GridColorSelectButton_Click(object sender, EventArgs e)
        {
            Color gridColor = UtilityMethods.SelectColorFromDialog(this, GridColorSelectButton.BackColor);
            GridUIMediator.GridColor = gridColor;
        }

        private void ShowGridSizeSwitch_CheckedChanged()
        {
            GridUIMediator.ShowGridSize = ShowGridSizeSwitch.Checked;
        }

        #endregion

        #region Measure Event Handlers

        private void MeasureButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.DrawMapMeasure, 0);
        }

        private void SelectMeasureColorButton_Click(object sender, EventArgs e)
        {
            Color measureColor = UtilityMethods.SelectColorFromDialog(this, SelectMeasureColorButton.BackColor);
            MeasureUIMediator.MapMeasureColor = measureColor;
        }

        private void UseScaleUnitsSwitch_CheckedChanged()
        {
            MeasureUIMediator.UseScaleUnits = UseScaleUnitsSwitch.Checked;
        }

        private void MeasureAreaSwitch_CheckedChanged()
        {
            MeasureUIMediator.MeasureArea = MeasureAreaSwitch.Checked;
        }

        private void ClearMeasureButton_Click(object sender, EventArgs e)
        {
            MapMeasureManager.Delete(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapMeasure);
            SKGLRenderControl.Invalidate();
        }

        #endregion

        #endregion 

        #region Region Tab Event Handlers

        private void ShowRegionLayerSwitch_CheckedChanged()
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.REGIONLAYER);

            regionLayer.ShowLayer = ShowRegionLayerSwitch.Checked;

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.REGIONOVERLAYLAYER);

            regionOverlayLayer.ShowLayer = ShowRegionLayerSwitch.Checked;
        }

        private void SelectRegionButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.RegionSelect, 0);
        }

        private void CreateRegionButton_Click(object sender, EventArgs e)
        {
            MainUIMediator.SetDrawingMode(MapDrawingMode.RegionPaint, 0);
            MapStateMediator.CurrentMapRegion = null;
        }

        private void RegionColorSelectButton_Click(object sender, EventArgs e)
        {
            Color regionColor = UtilityMethods.SelectColorFromDialog(this, RegionColorSelectButton.BackColor);
            MapRegionUIMediator.RegionColor = regionColor;
        }

        private void RegionBorderWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            MapRegionUIMediator.RegionBorderWidth = RegionBorderWidthTrack.Value;
            TOOLTIP.Show(MapRegionUIMediator.RegionBorderWidth.ToString(), RegionValuesGroup, new Point(RegionBorderWidthTrack.Right - 30, RegionBorderWidthTrack.Top - 20), 2000);
        }

        private void RegionBorderSmoothingTrack_ValueChanged(object sender, EventArgs e)
        {
            MapRegionUIMediator.RegionBorderSmoothing = RegionBorderSmoothingTrack.Value;
            TOOLTIP.Show(MapRegionUIMediator.RegionBorderSmoothing.ToString(), RegionValuesGroup, new Point(RegionBorderSmoothingTrack.Right - 30, RegionBorderSmoothingTrack.Top - 20), 2000);
        }

        private void RegionOpacityTrack_ValueChanged(object sender, EventArgs e)
        {
            MapRegionUIMediator.RegionInnerOpacity = RegionOpacityTrack.Value;
            TOOLTIP.Show(MapRegionUIMediator.RegionInnerOpacity.ToString(), RegionValuesGroup, new Point(RegionOpacityTrack.Right - 30, RegionOpacityTrack.Top - 20), 2000);
        }

        private void RegionSolidBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionSolidBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.SolidLinePath;
            }
        }

        private void SolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionSolidBorderRadio.Checked = true;
        }

        private void RegionDottedBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionDottedBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.DottedLinePath;
            }
        }

        private void DottedRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDottedBorderRadio.Checked = true;
        }

        private void RegionDashBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionDashBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.DashedLinePath;
            }
        }

        private void DashedRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashBorderRadio.Checked = true;
        }

        private void RegionDashDotBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionDashDotBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.DashDotLinePath;
            }
        }

        private void DashDotRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashDotBorderRadio.Checked = true;
        }

        private void RegionDashDotDotBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionDashDotBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.DashDotDotLinePath;
            }
        }

        private void DashDotDotRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashDotDotBorderRadio.Checked = true;
        }

        private void RegionDoubleSolidBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionDoubleSolidBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.DoubleSolidBorderPath;
            }
        }

        private void DoubleSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDoubleSolidBorderRadio.Checked = true;
        }

        private void RegionSolidAndDashesBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionSolidAndDashesBorderRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.LineAndDashesPath;
            }
        }

        private void SolidAndDashRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionSolidAndDashesBorderRadio.Checked = true;
        }

        private void RegionBorderedGradientRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionBorderedGradientRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.BorderedGradientPath;
            }
        }

        private void GradientRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedGradientRadio.Checked = true;
        }

        private void RegionBorderedLightSolidRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionBorderedLightSolidRadio.Checked)
            {
                MapRegionUIMediator.RegionBorderType = PathType.BorderedLightSolidPath;
            }
        }

        private void LightSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedLightSolidRadio.Checked = true;
        }

        #endregion


    }
}
