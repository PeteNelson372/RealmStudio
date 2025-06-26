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
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Timers;
using ComboBox = System.Windows.Forms.ComboBox;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private readonly string RELEASE_STATE = "Pre-Release";

        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static System.Timers.Timer? LOCATION_UPDATE_TIMER;

        internal static readonly ToolTip TOOLTIP = new();

        public static readonly NameGeneratorConfiguration NAME_GENERATOR_CONFIG = new();

        private readonly AppSplashScreen SPLASH_SCREEN = new();

        private static string MapCommandLinePath = string.Empty;

        private static float SELECTED_PATH_ANGLE = -1;

        // UI Mediators

        // UI mediator for the UI controls and values on the RealmStudioMainFOrm
        // not related to any map object (e.g. DrawingZoom)
        private MainFormUIMediator MainMediator { get; set; }

        // UI mediator for map background
        private BackgroundUIMediator BackgroundMediator { get; set; }

        // UI mediator for map boxes
        private BoxUIMediator BoxMediator { get; set; }

        // UI mediator for drawing layer
        private DrawingUIMediator DrawingMediator { get; set; }

        // UI mediator for the map frame
        private FrameUIMediator FrameMediator { get; set; }

        // UI mediator for MapGrid
        private MapGridUIMediator GridMediator { get; set; }

        // UI mediator for Labels
        private LabelUIMediator LabelMediator { get; set; }

        // UI mediator for the Ocean tab
        private OceanUIMediator OceanMediator { get; set; }

        // UI mediator for Map Paths
        private PathUIMediator PathMediator { get; set; }

        // UI Mediator for Label Presets
        private LabelPresetUIMediator PresetMediator { get; set; }

        // UI Mediator for Landforms
        private LandformUIMediator LandformMediator { get; set; }

        // UI mediator for MapMeasure
        private MapMeasureUIMediator MeasureMediator { get; set; }

        // UI mediator for MapScale
        private MapScaleUIMediator ScaleMediator { get; set; }

        // UI mediator for MapSymbols
        private SymbolUIMediator SymbolMediator { get; set; }

        // UI mediator for MapRegions
        private RegionUIMediator RegionMediator { get; set; }

        // UI mediator for the map vignette
        private VignetteUIMediator VignetteMediator { get; set; }

        // UI mediator for WaterFeatures (not Rivers)
        private WaterFeatureUIMediator WaterFeatureMediator { get; set; }

        // UI mediator for Windroses
        private WindroseUIMediator WindroseMediator { get; set; }

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
            MainMediator = new(this);
            MapStateMediator.MainUIMediator = MainMediator;

            BackgroundMediator = new(this);
            BackgroundManager.BackgroundMediator = BackgroundMediator;

            BoxMediator = new BoxUIMediator(this);
            BoxManager.BoxMediator = BoxMediator;

            DrawingMediator = new DrawingUIMediator(this);
            DrawingManager.DrawingMediator = DrawingMediator;

            FrameMediator = new FrameUIMediator(this);
            FrameManager.FrameMediator = FrameMediator;

            GridMediator = new MapGridUIMediator(this);
            MapGridManager.GridUIMediator = GridMediator;

            LabelMediator = new LabelUIMediator(this);
            LabelManager.LabelMediator = LabelMediator;

            LandformMediator = new LandformUIMediator(this);
            LandformManager.LandformMediator = LandformMediator;

            MeasureMediator = new(this);
            MapMeasureManager.MeasureUIMediator = MeasureMediator;

            OceanMediator = new(this);
            OceanManager.OceanMediator = OceanMediator;

            PathMediator = new PathUIMediator(this);
            PathManager.PathMediator = PathMediator;

            PresetMediator = new(this);
            LabelPresetManager.PresetMediator = PresetMediator;

            RegionMediator = new(this);
            RegionManager.RegionUIMediator = RegionMediator;

            ScaleMediator = new(this);
            MapScaleManager.ScaleMediator = ScaleMediator;

            SymbolMediator = new SymbolUIMediator(this);
            SymbolManager.SymbolMediator = SymbolMediator;

            VignetteMediator = new VignetteUIMediator(this);
            VignetteManager.VignetteMediator = VignetteMediator;

            WaterFeatureMediator = new WaterFeatureUIMediator(this);
            WaterFeatureManager.WaterFeatureMediator = WaterFeatureMediator;

            WindroseMediator = new(this);
            WindroseManager.WindroseMediator = WindroseMediator;


            ApplicationTimerManager = new(this)
            {
                SymbolUIMediator = SymbolMediator
            };


            // register map object mediators with the MapStateMediator so
            // they can be notified when the current map is changed
            // or other changes are made; in most cases, the map object
            // mediators handle changes directly with the associated manager
            // but in some cases, changes to the current map (and maybe to other objects)
            // result in UI changes that the map object mediators don't know about

            MapStateMediator.BoxUIMediator = BoxMediator;
            MapStateMediator.FrameUIMediator = FrameMediator;
            MapStateMediator.GridUIMediator = GridMediator;
            MapStateMediator.LandformUIMediator = LandformMediator;
            MapStateMediator.MeasureUIMediator = MeasureMediator;
            MapStateMediator.PathUIMediator = PathMediator;
            MapStateMediator.RegionUIMediator = RegionMediator;
            MapStateMediator.ScaleUIMediator = ScaleMediator;
            MapStateMediator.SymbolUIMediator = SymbolMediator;
            MapStateMediator.VignetteUIMediator = VignetteMediator;
            MapStateMediator.WaterFeatureUIMediator = WaterFeatureMediator;
            MapStateMediator.WindroseUIMediator = WindroseMediator;

            MapStateMediator.ApplicationTimerManager = ApplicationTimerManager;
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

            SPLASH_SCREEN.Close();

            Refresh();

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

            MainMediator.SetDrawingMode(MapDrawingMode.None, 0);

            PopulateControlsWithAssets(assetCount);
            PopulateFontPanelUI();
            LoadNameGeneratorConfigurationDialog();

            Activate();

            ApplicationTimerManager.StartAutosaveTimer();

            StartLocationUpdateTimer();

            if (Settings.Default.AutoCheckForUpdates)
            {
                ApplicationTimerManager.StartVersionCheckTimer();
            }

            SKGLRenderControl.Invalidate();
            Refresh();

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

        private void RealmStudioMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (LabelManager.CreatingLabel)
            {
                return; // do not handle key presses if creating a label
            }

            KeyHandler.HandleKey(e.KeyCode);
            e.Handled = true;
            SKGLRenderControl.Invalidate();
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

        private void NewVersionButton_Click(object sender, EventArgs e)
        {
            // if user preferences are set to automatically check for new versions,
            // Realm Studio will automatically check for a new version
            // two minutes after the main form is shown; if a new version is available, the NewVersionButton will be
            // shown and the user can click it to go to the latest release on GitHub
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/PeteNelson372/RealmStudio/releases/latest",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void NewVersionButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("A new version of Realm Studio is available. Click to go to Github to download it.", RealmStudioForm, new Point(NewVersionButton.Left, NewVersionButton.Top + 30), 3000);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            MainMediator.DrawingZoom = 1.0F;
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

            MainMediator.DrawingZoom = Math.Min(horizontalAspect, verticalAspect);
            ZoomLevelTrack.Value = Math.Max(1, (int)MainMediator.DrawingZoom);

            MapStateMediator.ScrollPoint = new SKPoint(0, 0);
            MapStateMediator.DrawingPoint = new SKPoint(0, 0);

            MapRenderHScroll.Value = 0;
            MapRenderVScroll.Value = 0;

            SKGLRenderControl.Invalidate();
        }

        private void Open3DViewButton_Click(object sender, EventArgs e)
        {
            ThreeDView tdv = new("3D Model Viewer");
            tdv.Show();
        }

        private void AreaSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.RealmAreaSelect, 0);
        }

        private void AreaSelectButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Select map area", RealmStudioForm, new Point(AreaSelectButton.Left, AreaSelectButton.Top + 30), 3000);
        }

        private void AddPresetColorButton_Click(object sender, EventArgs e)
        {

        }

        private void AddPresetColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            switch (MainTab.SelectedIndex)
            {
                case 1:
                    {
                        // ocean layer
                        Color selectedColor = UtilityMethods.SelectColor(this, e, OceanPaintColorSelectButton.BackColor);

                        if (selectedColor != Color.Empty)
                        {
                            if (OceanCustomColorButton1.Text == "")
                            {
                                OceanMediator.CustomColor1 = selectedColor;
                            }
                            else if (OceanCustomColorButton2.Text == "")
                            {
                                OceanMediator.CustomColor2 = selectedColor;
                            }
                            else if (OceanCustomColorButton3.Text == "")
                            {
                                OceanMediator.CustomColor3 = selectedColor;
                            }
                            else if (OceanCustomColorButton4.Text == "")
                            {
                                OceanMediator.CustomColor4 = selectedColor;
                            }
                            else if (OceanCustomColorButton5.Text == "")
                            {
                                OceanMediator.CustomColor5 = selectedColor;
                            }
                            else if (OceanCustomColorButton6.Text == "")
                            {
                                OceanMediator.CustomColor6 = selectedColor;
                            }
                            else if (OceanCustomColorButton7.Text == "")
                            {
                                OceanMediator.CustomColor7 = selectedColor;
                            }
                            else if (OceanCustomColorButton8.Text == "")
                            {
                                OceanMediator.CustomColor8 = selectedColor;
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        // land layer
                        Color selectedColor = UtilityMethods.SelectColor(this, e, LandColorSelectionButton.BackColor);

                        if (selectedColor != Color.Empty)
                        {
                            if (LandCustomColorButton1.Text == "")
                            {
                                LandformMediator.CustomColor1 = selectedColor;
                            }
                            else if (LandCustomColorButton2.Text == "")
                            {
                                LandformMediator.CustomColor2 = selectedColor;
                            }
                            else if (LandCustomColorButton3.Text == "")
                            {
                                LandformMediator.CustomColor3 = selectedColor;
                            }
                            else if (LandCustomColorButton4.Text == "")
                            {
                                LandformMediator.CustomColor4 = selectedColor;
                            }
                            else if (LandCustomColorButton5.Text == "")
                            {
                                LandformMediator.CustomColor5 = selectedColor;
                            }
                            else if (LandCustomColorButton6.Text == "")
                            {
                                LandformMediator.CustomColor6 = selectedColor;
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        // water layer
                        Color selectedColor = UtilityMethods.SelectColor(this, e, WaterPaintColorSelectButton.BackColor);

                        if (selectedColor != Color.Empty)
                        {
                            if (WaterCustomColor1.Text == "")
                            {
                                WaterFeatureMediator.CustomColor1 = selectedColor;
                            }
                            else if (WaterCustomColor2.Text == "")
                            {
                                WaterFeatureMediator.CustomColor2 = selectedColor;
                            }
                            else if (WaterCustomColor3.Text == "")
                            {
                                WaterFeatureMediator.CustomColor3 = selectedColor;
                            }
                            else if (WaterCustomColor4.Text == "")
                            {
                                WaterFeatureMediator.CustomColor4 = selectedColor;
                            }
                            else if (WaterCustomColor5.Text == "")
                            {
                                WaterFeatureMediator.CustomColor5 = selectedColor;
                            }
                            else if (WaterCustomColor6.Text == "")
                            {
                                WaterFeatureMediator.CustomColor6 = selectedColor;
                            }
                            else if (WaterCustomColor7.Text == "")
                            {
                                WaterFeatureMediator.CustomColor7 = selectedColor;
                            }
                            else if (WaterCustomColor8.Text == "")
                            {
                                WaterFeatureMediator.CustomColor8 = selectedColor;
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
            MainMediator.SetDrawingMode(MapDrawingMode.ColorSelect, 0);
        }

        private void SelectColorButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Select color from map", RealmStudioForm, new Point(SelectColorButton.Left, SelectColorButton.Top + 30), 3000);
        }

        private void ZoomLevelTrack_Scroll(object sender, EventArgs e)
        {
            MainMediator.DrawingZoom = ZoomLevelTrack.Value / 10.0F;
            SKGLRenderControl.Invalidate();
        }

        internal void NameGenerator_NameGenerated(object? sender, EventArgs e)
        {
            if (sender is NameGeneratorConfiguration ngc)
            {
                string selectedName = ngc.SelectedName;

                if (LabelManager.CreatingLabel && !string.IsNullOrEmpty(selectedName))
                {
                    if (LabelManager.LabelTextBox != null && !LabelManager.LabelTextBox.IsDisposed)
                    {
                        LabelManager.LabelTextBox.Text = selectedName;
                        LabelManager.LabelTextBox.Refresh();
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
                    HeightMapManager.ExportHeightMap3DModel(MapStateMediator.CurrentMap);
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
            SKPoint zoomedScrolledPoint = new((cursorPosition.X / MainMediator.DrawingZoom) + MapStateMediator.DrawingPoint.X,
                (cursorPosition.Y / MainMediator.DrawingZoom) + MapStateMediator.DrawingPoint.Y);

            CutCopyPasteManager.PasteSelectedComponentsAtPoint(zoomedScrolledPoint);
            SKGLRenderControl.Invalidate();
        }

        private void ClearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCopyPasteManager.ClearComponentSelection();
            SKGLRenderControl.Invalidate();
        }

        private void ThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show the theme list dialog
            ThemeList themeList = new();

            // get the current state of the UI as a theme
            // and send it to the ThemeList dialog
            MapTheme settingsTheme = ThemeManager.SaveCurentSettingsToTheme();

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
                    ThemeManager.ApplyTheme(selectedTheme, themeFilter);
                    MapStateMediator.CurrentMap.MapTheme = AssetManager.CURRENT_THEME.ThemeName;
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
                ScaleMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

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
                ScaleMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

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
                    Multiselect = false,
                    Filter = UtilityMethods.GetCommonImageFilter()
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            Bitmap b = (Bitmap)Bitmap.FromFile(ofd.FileName);

                            SKPath landformPath = LandformManager.TraceImage(ofd.FileName);

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

                                Landform? landform = LandformManager.CreateNewLandform(MapStateMediator.CurrentMap, landformPath, MapStateMediator.SelectedRealmArea);

                                if (landform != null)
                                {
                                    landformLayer.MapLayerComponents.Add(landform);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to trace map outline from " + ofd.FileName, "Map Trace Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                }
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

        private void RenderAsHeightMapMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (RenderAsHeightMapMenuItem.Checked)
            {
                MainMediator.SetDrawingMode(MapDrawingMode.HeightMapPaint, 0);

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
                MainMediator.SetDrawingMode(MapDrawingMode.None, 0);

                // force maintab selected index change
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.SelectedIndex = 0;
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.Refresh();
            }

            SKGLRenderControl.Invalidate();
        }

        private void DisplayWorldGlobeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorldGlobeManager.ShowWorldGlobe();
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

        private void CheckForNewReleaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReleaseChecker.FetchRealmStudioGithubReleasesAsync().ContinueWith(releasesTask =>
            {
                List<(string, string)> releases = releasesTask.Result;

                string? newReleaseVersion = ReleaseChecker.HasNewRelease(releases, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown");

                if (newReleaseVersion != null)
                {
                    string message = $"Realm Studio version {newReleaseVersion} is available for download from Github.\n\nThe About dialog has a link to the latest version.";
                    MessageBox.Show(message, "New Version Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string message = "You are using the latest version of Realm Studio.";
                    MessageBox.Show(message, "No New Version Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }
        #endregion

        #region Main Form Methods
        /******************************************************************************************************* 
         * MAIN FORM METHODS
         *******************************************************************************************************/

        private void PanMap(MouseEventArgs e)
        {
            // pan the map when middle button (mouse wheel) is held down and dragged
            if (MapStateMediator.PreviousMouseLocation == SKPoint.Empty)
            {
                MapStateMediator.PreviousMouseLocation = e.Location.ToSKPoint();
            }

            int xDelta = (int)(e.Location.X - MapStateMediator.PreviousMouseLocation.X);
            int yDelta = (int)(e.Location.Y - MapStateMediator.PreviousMouseLocation.Y);

            SKPoint scrollPoint = MapStateMediator.ScrollPoint;
            SKPoint drawPoint = MapStateMediator.DrawingPoint;

            scrollPoint.X += xDelta;
            scrollPoint.Y += yDelta;

            drawPoint.X += -xDelta;
            drawPoint.Y += -yDelta;

            MapStateMediator.ScrollPoint = scrollPoint;
            MapStateMediator.DrawingPoint = drawPoint;

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

                MainMediator.SetDrawingMode(MapDrawingMode.None, 0);

                BackgroundMediator.Reset();
                LandformMediator.Reset();
                OceanMediator.Reset();
                PathMediator.Reset();
                WaterFeatureMediator.Reset();

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
                ScaleMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

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
                VignetteManager.Delete();
                VignetteManager.Create();
            }

            return result;
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear the drawing mode (and uncheck all drawing, paint, and erase buttons) when switching tabs
            MainMediator.SetDrawingMode(MapDrawingMode.None, 0);

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
                    BackgroundMediator.NotifyUpdate(null);
                    break;
                case 1:
                    OceanToolPanel.Visible = true;
                    OceanMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 2:
                    BackgroundToolPanel.Visible = false;
                    LandformMediator.NotifyUpdate(null);

                    if (RenderAsHeightMapMenuItem.Checked)
                    {
                        MainMediator.SetDrawingMode(MapDrawingMode.HeightMapPaint, 0);

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
                    WaterFeatureMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 4:
                    PathToolPanel.Visible = true;
                    //PathMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 5:
                    SymbolToolPanel.Visible = true;
                    SymbolMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 6:
                    LabelToolPanel.Visible = true;
                    LabelMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 7:
                    OverlayToolPanel.Visible = true;
                    FrameMediator.NotifyUpdate(null);
                    GridMediator.NotifyUpdate(null);
                    ScaleMediator.NotifyUpdate(null);
                    MeasureMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 8:
                    RegionToolPanel.Visible = true;
                    RegionMediator.NotifyUpdate(null);
                    BackgroundToolPanel.Visible = false;
                    break;
                case 9:
                    DrawingToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
                    break;
            }

            MainTab.Invalidate();
        }

        private void SetZoomLevel(int upDown)
        {
            MainMediator.ChangeDrawingZoom(upDown);
        }

        private void PopulateControlsWithAssets(int assetCount)
        {
            SetStatusText("Loaded: " + assetCount + " assets.");

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
                ThemeManager.ApplyTheme(AssetManager.CURRENT_THEME, tf);
            }

            // background texture
            BackgroundTextureBox.Image = BackgroundMediator.BackgroundTextureList[BackgroundMediator.BackgroundTextureIndex].TextureBitmap;
            BackgroundTextureNameLabel.Text = BackgroundMediator.BackgroundTextureList[BackgroundMediator.BackgroundTextureIndex].TextureName;

            // landform texture
            LandformTexturePreviewPicture.Image = LandformMediator.LandTextureList[LandformMediator.LandformTextureIndex].TextureBitmap;
            LandTextureNameLabel.Text = LandformMediator.LandTextureList[LandformMediator.LandformTextureIndex].TextureName;

            // ocean texture
            OceanTextureBox.Image = OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureBitmap;
            OceanTextureNameLabel.Text = OceanMediator.OceanTextureList[OceanMediator.OceanTextureIndex].TextureName;

            // path texture
            if (PathMediator.PathTextureList.First().TextureBitmap == null)
            {
                PathMediator.PathTextureList.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(PathMediator.PathTextureList.First().TexturePath);
            }

            PathTexturePreviewPicture.Image = PathMediator.PathTextureList.First().TextureBitmap;
            PathTextureNameLabel.Text = PathMediator.PathTextureList.First().TextureName;

            // drawing texture
            DrawingFillTextureBox.Image = DrawingMediator.DrawingTextureList[DrawingMediator.DrawingTextureIndex].TextureBitmap;
            DrawingFillTextureNameLabel.Text = DrawingMediator.DrawingTextureList[DrawingMediator.DrawingTextureIndex].TextureName;
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

        private void ExportMapAsImage(RealmMapExportFormat exportFormat, bool upscale = false)
        {
            SaveFileDialog ofd = new()
            {
                Title = "Export Map",
                DefaultExt = exportFormat.ToString().ToLowerInvariant(),
                RestoreDirectory = true,
                ShowHelp = true,
                AddExtension = true,
                CheckPathExists = true,
                ShowHiddenFiles = false,
                ValidateNames = true,
                Filter = UtilityMethods.GetCommonImageFilter()
            };

            int filterIndex = 0;
            string[] filterStrings = ofd.Filter.Split('|');

            for (int i = 2; i < filterStrings.Length; i++)      // skip all image strings filter
            {
                if (filterStrings[i].Contains(exportFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    filterIndex = (i / 2) + 1;
                    break;
                }
            }

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

                    LandformManager.RemoveDeletedLandforms();

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

                    LandformManager.RemoveDeletedLandforms();

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
                AddExtension = true,
                CheckPathExists = true,
                ShowHiddenFiles = false,
                ValidateNames = true,
                Filter = UtilityMethods.GetCommonImageFilter()
            };


            int filterIndex = 0;
            string[] filterStrings = ofd.Filter.Split('|');

            for (int i = 2; i < filterStrings.Length; i++)      // skip all image strings filter
            {
                if (filterStrings[i].Contains(exportFormat.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    filterIndex = (i / 2) + 1;
                    break;
                }
            }

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

                    LandformManager.RemoveDeletedLandforms();

                    SKSurface s = SKSurface.Create(new SKImageInfo(MapStateMediator.CurrentMap.MapWidth, MapStateMediator.CurrentMap.MapHeight));
                    s.Canvas.Clear(SKColors.Black);

                    // render the map as a height map
                    HeightMapManager.RenderHeightMapToCanvas(MapStateMediator.CurrentMap, s.Canvas, new SKPoint(0, 0), null);

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
                    LandformManager.RemoveDeletedLandforms();

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
                        LandformManager.RemoveDeletedLandforms();

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
            ArgumentNullException.ThrowIfNull(FrameManager.FrameMediator);
            ArgumentNullException.ThrowIfNull(OceanManager.OceanMediator);

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
                        ScaleMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

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

                MapRenderHScroll.Maximum = MapStateMediator.CurrentMap.MapWidth;
                MapRenderVScroll.Maximum = MapStateMediator.CurrentMap.MapHeight;

                if (string.IsNullOrEmpty(MapStateMediator.CurrentMap.MapTheme))
                {
                    MapStateMediator.CurrentMap.MapTheme = AssetManager.DEFAULT_THEME_NAME;
                }


                MapTheme? theme = AssetManager.THEME_LIST.FirstOrDefault(t => t.ThemeName == MapStateMediator.CurrentMap.MapTheme);

                if (theme != null)
                {
                    // apply the theme to the map
                    ThemeFilter tf = new();
                    ThemeManager.ApplyTheme(theme, tf);
                }


                // theme is applied and map is loaded, so set any values in the map that are not set when it is loaded, and then
                // set the UI controls to match the values in the map

                // maps store scale and opacity values as the scale value;
                // mediators store them as the values used to scale and set opacity of the bitmap
                // the mediator translates trackbar value to/from the scale and opacity values for the bitmap

                // *** INITIALIZE BACKGROUND ***
                // set background texture index from the background texture bitmap
                // set background texture name label to match the name in the map
                // set background texture picture to match the picture in the map,
                // set background texture scale trackbar and background mirror switch

                MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);

                if (baseLayer.MapLayerComponents.Count > 0)
                {
                    BackgroundMediator.Initialize(
                        BackgroundMediator.BackgroundTextureIndex,
                        ((MapImage)baseLayer.MapLayerComponents[0]).Scale,
                        ((MapImage)baseLayer.MapLayerComponents[0]).MirrorImage);
                }

                // *** INITIALIZE OCEAN ***
                // set ocean texture index from the ocean texture bitmap
                // set ocean texture name label to match the name in the map (done in Initialize)
                // set ocean texture picture to match the picture in the map (done in Initialize)
                // set ocean texture scale trackbar, ocean opacity trackbar, and ocean mirror switch
                // set paint objects on LayerPaointStroke objects in the ocean drawing layer

                MapLayer oceanLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);
                MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANDRAWINGLAYER);

                if (oceanLayer.MapLayerComponents.Count > 0)
                {
                    int oceanTextureIndex = 0;

                    for (int i = 0; i < OceanManager.OceanMediator.OceanTextureList.Count; i++)
                    {
                        if (OceanManager.OceanMediator.OceanTextureList[i].TextureName == ((MapImage)oceanLayer.MapLayerComponents[0]).ImageName)
                        {
                            oceanTextureIndex = i;
                            break;
                        }
                    }

                    OceanMediator.Initialize(
                        OceanMediator.OceanTextureIndex,
                        ((MapImage)oceanLayer.MapLayerComponents[0]).ImageName,
                        ((MapImage)oceanLayer.MapLayerComponents[0]).Scale,
                        ((MapImage)oceanLayer.MapLayerComponents[0]).Opacity,
                        ((MapImage)oceanLayer.MapLayerComponents[0]).MirrorImage);
                }

                // finalize loading of ocean drawing layer
                for (int i = 0; i < oceanDrawingLayer.MapLayerComponents.Count; i++)
                {
                    if (oceanDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                    {
                        paintStroke.ParentMap = MapStateMediator.CurrentMap;

                        if (!paintStroke.Erase)
                        {
                            paintStroke.ShaderPaint = PaintObjects.OceanPaint;
                        }
                        else
                        {
                            paintStroke.ShaderPaint = PaintObjects.OceanEraserPaint;
                        }
                    }
                }


                // *** INITIALIZE LANDFORMS ***
                // landform texture index and other UI control values come from the selected theme
                LandformManager.FinalizeLandforms(SKGLRenderControl);

                // *** INITIALIZE WATER FEATURES ***
                // water feature UI control values come from the selected theme
                WaterFeatureManager.FinalizeWaterFeatures();
                WaterFeatureManager.FinalizeWindroses();

                // *** INITIALIZE PATHS ***
                // path UI control values come from the selected theme
                PathManager.FinalizeMapPaths();

                // *** INITIALIZE SYMBOLS ***
                // symbol UI control values come from the selected theme
                SymbolManager.FinalizeMapSymbols();

                // *** INITIALIZE LABELS AND BOXES ***
                // label UI control values come from the selected theme
                BoxManager.FinalizeMapBoxes();

                // *** INITIALIZE OVERLAYS ***
                // *** FINALIZE MAP FRAME ***
                MapStateMediator.CurrentMapFrame = (PlacedMapFrame?)FrameManager.GetComponentById(Guid.Empty);

                if (MapStateMediator.CurrentMapFrame != null)
                {
                    FrameManager.FrameMediator.Initialize(MapStateMediator.CurrentMapFrame.FrameScale,
                        MapStateMediator.CurrentMapFrame.FrameTint,
                        MapStateMediator.CurrentMapFrame.FrameEnabled);
                }

                // *** FINALIZE MAP GRID ***
                MapGridManager.FinalizeMapGrid();

                // *** FINALIZE MAP REGIONS ***
                RegionManager.FinalizeMapRegions();

                // *** FINALIZE MAP VIGNETTE ***
                VignetteManager.FinalizeMapVignette();

                HeightMapManager.ConvertMapImageToMapHeightMap(MapStateMediator.CurrentMap);

                SetStatusText("Loaded: " + MapStateMediator.CurrentMap.MapName);

                UpdateMapNameAndSize();

                MapStateMediator.CurrentMap.IsSaved = true;

                SKGLRenderControl.Invalidate();
            }
            catch
            {
                MessageBox.Show("An error has occurred while opening the map. The map file may be corrupt.", "Error Loading Map", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                MapStateMediator.CurrentMap = MapBuilder.CreateMap("", "DEFAULT", MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT, SKGLRenderControl.GRContext);

                // TODO: generalize
                ScaleMediator.ScaleUnitsText = MapStateMediator.CurrentMap.MapAreaUnits;

                VignetteManager.Delete();
                VignetteManager.Create();

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
                        ScaleMediator.ScaleFont = FontPanelManager.FontPanelSelectedFont.SelectedFont;
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

                            Cmd_ChangeLabelAttributes cmd = new(MapStateMediator.SelectedMapLabel, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, newFont);
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
            e.Surface.Canvas.Scale(MainMediator.DrawingZoom);

            // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
            e.Surface.Canvas.Clear(SKColors.White);

            if (SKGLRenderControl.GRContext != null
                && MapStateMediator.CurrentMap != null
                && MapStateMediator.CurrentMap.MapLayers.Count == MapBuilder.MAP_LAYER_COUNT)
            {
                if (RenderAsHeightMapMenuItem.Checked)
                {
                    HeightMapManager.RenderHeightMapToCanvas(MapStateMediator.CurrentMap,
                        e.Surface.Canvas,
                        MapStateMediator.ScrollPoint,
                        MapStateMediator.SelectedRealmArea);
                }
                else
                {
                    MapRenderMethods.RenderMapToCanvas(MapStateMediator.CurrentMap, e.Surface.Canvas, MapStateMediator.ScrollPoint);
                }

                if (MainMediator.CurrentDrawingMode != MapDrawingMode.ColorSelect
                    && MainMediator.CurrentDrawingMode != MapDrawingMode.None)
                {
                    float symbolScale = (float)(SymbolMediator.SymbolScale / 100.0F);
                    float symbolRotation = SymbolMediator.SymbolRotation;
                    bool mirrorSymbol = SymbolMediator.MirrorSymbol;
                    bool useAreaBrush = SymbolMediator.UseAreaBrush;

                    MapRenderMethods.DrawCursor(e.Surface.Canvas,
                        MainMediator.CurrentDrawingMode,
                        new SKPoint(MapStateMediator.CurrentCursorPoint.X + MapStateMediator.ScrollPoint.X,
                            MapStateMediator.CurrentCursorPoint.Y + MapStateMediator.ScrollPoint.Y),
                        MainMediator.SelectedBrushSize,
                        symbolScale,
                        symbolRotation,
                        mirrorSymbol,
                        useAreaBrush);
                }

                // work layer
                MapRenderMethods.RenderWorkLayer(MapStateMediator.CurrentMap, e.Surface.Canvas, MapStateMediator.ScrollPoint);
                MapRenderMethods.RenderWorkLayer2(MapStateMediator.CurrentMap, e.Surface.Canvas, MapStateMediator.ScrollPoint);
            }
        }

        private void SKGLRenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (LabelManager.CreatingLabel)
            {
                return; // do not handle mouse move if creating a label
            }

            MapStateMediator.CurrentMouseLocation = e.Location.ToSKPoint();
            MapStateMediator.CurrentCursorPoint = MainFormUIMediator.CalculateCursorPoint(e);

            // objects are created and/or initialized on mouse down
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseDownHandler(MainMediator.SelectedBrushSize);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                MiddleButtonMouseDownHandler(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseDownHandler();
            }
        }

        private void SKGLRenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainMediator.CurrentDrawingMode == MapDrawingMode.ColorSelect)
            {
                Cursor = AssetManager.EYEDROPPER_CURSOR;
            }

            if (LabelManager.CreatingLabel)
            {
                return; // do not handle mouse move if creating a label
            }

            MapStateMediator.CurrentMouseLocation = e.Location.ToSKPoint();
            MapStateMediator.CurrentCursorPoint = MainFormUIMediator.CalculateCursorPoint(e);

            // objects are drawn or moved on mouse move
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseMoveHandler();
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
                NoButtonMouseMoveHandler();
            }

            SKGLRenderControl.Invalidate();
        }

        private void SKGLRenderControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (LabelManager.CreatingLabel)
            {
                return; // do not handle mouse move if creating a label
            }

            MapStateMediator.CurrentMouseLocation = e.Location.ToSKPoint();
            MapStateMediator.CurrentCursorPoint = MainFormUIMediator.CalculateCursorPoint(e);

            // objects are finalized or reset on mouse up
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseUpHandler(e);
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
            if (MainMediator.CurrentDrawingMode == MapDrawingMode.ColorSelect)
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

            if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.LandErase)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandEraserSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.LandPaint)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.LandColor)
            {
                LandformMediator.LandPaintBrushSize = RealmMapMethods.GetNewBrushSize(LandColorBrushSizeTrack, sizeDelta);
                MainMediator.SelectedBrushSize = LandformMediator.LandPaintBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.LandColorErase)
            {
                LandformMediator.LandPaintEraserSize = RealmMapMethods.GetNewBrushSize(LandColorEraserSizeTrack, sizeDelta);
                MainMediator.SelectedBrushSize = LandformMediator.LandPaintEraserSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.OceanErase)
            {
                OceanMediator.OceanPaintEraserSize = RealmMapMethods.GetNewBrushSize(OceanEraserSizeTrack, sizeDelta);
                MainMediator.SelectedBrushSize = OceanMediator.OceanPaintEraserSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.OceanPaint)
            {
                OceanMediator.OceanPaintBrushSize = RealmMapMethods.GetNewBrushSize(OceanBrushSizeTrack, sizeDelta);
                MainMediator.SelectedBrushSize = OceanMediator.OceanPaintBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.WaterPaint)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.WaterErase)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterEraserSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.LakePaint)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.WaterColor)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterColorBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.WaterColorErase)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(WaterColorEraserSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.MapHeightIncrease)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.MapHeightDecrease)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LandBrushSizeTrack, sizeDelta);
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingLine)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LineBrushSizeTrack, sizeDelta);
                DrawingMediator.DrawingLineBrushSize = MainMediator.SelectedBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingPaint)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LineBrushSizeTrack, sizeDelta);
                DrawingMediator.DrawingLineBrushSize = MainMediator.SelectedBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingPolygon)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LineBrushSizeTrack, sizeDelta);
                DrawingMediator.DrawingLineBrushSize = MainMediator.SelectedBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingEllipse)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LineBrushSizeTrack, sizeDelta);
                DrawingMediator.DrawingLineBrushSize = MainMediator.SelectedBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingRectangle)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LineBrushSizeTrack, sizeDelta);
                DrawingMediator.DrawingLineBrushSize = MainMediator.SelectedBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingErase)
            {
                MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(LineBrushSizeTrack, sizeDelta);
                DrawingMediator.DrawingLineBrushSize = MainMediator.SelectedBrushSize;
            }
            else if (ModifierKeys == Keys.None && MainMediator.CurrentDrawingMode == MapDrawingMode.DrawingStamp)
            {
                int newValue = (int)Math.Max(DrawingStampScaleTrack.Minimum, DrawingStampScaleTrack.Value + sizeDelta);
                newValue = (int)Math.Min(DrawingStampScaleTrack.Maximum, newValue);
                DrawingMediator.DrawingStampScale = newValue / 100.0F;
            }
            else if (ModifierKeys != Keys.Control && MainMediator.CurrentDrawingMode == MapDrawingMode.SymbolPlace)
            {
                if (AreaBrushSwitch.Enabled && SymbolMediator.UseAreaBrush)
                {
                    // area brush size is changed when AreaBrushSwitch is enabled and checked
                    MainMediator.SelectedBrushSize = RealmMapMethods.GetNewBrushSize(AreaBrushSizeTrack, sizeDelta);
                    SymbolMediator.AreaBrushSize = MainMediator.SelectedBrushSize;
                }
                else
                {
                    int newValue = (int)Math.Max(SymbolScaleUpDown.Minimum, SymbolScaleUpDown.Value + sizeDelta);
                    newValue = (int)Math.Min(SymbolScaleUpDown.Maximum, newValue);
                    SymbolMediator.SymbolScale = newValue;
                }
            }
            else if (ModifierKeys != Keys.Control && MainMediator.CurrentDrawingMode == MapDrawingMode.LabelSelect)
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

                ZoomLevelTrack.Value = (int)(MainMediator.DrawingZoom * 10.0F);
            }

            SKGLRenderControl.Invalidate();
        }

        private void SKGLRenderControl_Enter(object sender, EventArgs e)
        {
            if (MainMediator.CurrentDrawingMode == MapDrawingMode.ColorSelect)
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

        private void LeftButtonMouseDownHandler(int brushSize)
        {
            // has the map scale been clicked?
            MapStateMediator.SelectedMapScale = MapScaleManager.SelectMapScale(MapStateMediator.CurrentCursorPoint);

            if (MapStateMediator.SelectedMapScale != null && MapStateMediator.SelectedMapScale.IsSelected)
            {
                MainMediator.SetDrawingMode(MapDrawingMode.SelectMapScale, 0);
                Cursor.Current = Cursors.SizeAll;
            }

            switch (MainMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.OceanPaint:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        OceanManager.StartColorPainting(ApplicationTimerManager, SKGLRenderControl);
                    }
                    break;
                case MapDrawingMode.OceanErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        OceanManager.StartColorErasing(SKGLRenderControl);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandformSelect:
                    {
                        Cursor = Cursors.Default;

                        MapStateMediator.SelectedLandform = LandformManager.SelectLandformAtPoint(MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandformHeightMapSelect:
                    {
                        Cursor = Cursors.Default;

                        MapStateMediator.SelectedLandform = LandformManager.SelectLandformAtPoint(MapStateMediator.CurrentCursorPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;

                        Cursor = Cursors.Cross;

                        Landform? newLandform = LandformManager.CreateNewLandform(MapStateMediator.CurrentMap, null, SKRect.Empty);

                        if (newLandform != null)
                        {
                            MapStateMediator.CurrentLandform = newLandform;
                            MapStateMediator.CurrentLandform.DrawPath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, brushSize / 2);
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;
                        LandformManager.LandformErasePath.Reset();
                    }
                    break;
                case MapDrawingMode.LandColor:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        LandformManager.StartColorPainting(ApplicationTimerManager, SKGLRenderControl);
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        LandformManager.StartColorErasing(SKGLRenderControl);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    {
                        MapStateMediator.SelectedWaterFeature = (IWaterFeature?)WaterFeatureMediator.SelectWaterFeatureAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    {
                        Cursor = Cursors.Cross;
                        WaterFeatureManager.Create();
                    }
                    break;
                case MapDrawingMode.LakePaint:
                    {
                        Cursor = Cursors.Cross;

                        MapStateMediator.CurrentWaterFeature = WaterFeatureManager.CreateLake(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint,
                            brushSize, WaterColorSelectionButton.BackColor, ShorelineColorSelectionButton.BackColor);

                        if (MapStateMediator.CurrentWaterFeature != null)
                        {
                            Cmd_AddNewWaterFeature cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentWaterFeature);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            WaterFeatureManager.MergeWaterFeatures(MapStateMediator.CurrentMap);

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    {
                        Cursor = Cursors.Cross;

                        if (MapStateMediator.CurrentRiver == null)
                        {
                            MapStateMediator.CurrentRiver = WaterFeatureManager.CreateRiver(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, WaterColorSelectionButton.BackColor,
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

                                MapStateMediator.SelectedRiverPoint = WaterFeatureManager.SelectRiverPointAtPoint(river, MapStateMediator.CurrentCursorPoint, false);

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

                        WaterFeatureManager.StartColorPainting(ApplicationTimerManager, SKGLRenderControl);
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        MapStateMediator.CurrentMap.IsSaved = false;
                        Cursor = Cursors.Cross;

                        WaterFeatureManager.StartColorErasing(SKGLRenderControl);
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        if (MapStateMediator.CurrentMapPath == null)
                        {
                            MapStateMediator.CurrentMapPath = (MapPath?)PathManager.Create();
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

                                MapStateMediator.SelectedMapPathPoint = PathManager.SelectMapPathPointAtPoint(MapStateMediator.SelectedMapPath,
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
                        if (SymbolMediator.UseAreaBrush)
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
                    int eraserRadius = SymbolMediator.AreaBrushSize / 2;

                    SKPoint eraserCursorPoint = new(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                    SymbolManager.RemovePlacedSymbolsFromArea(eraserCursorPoint, eraserRadius);
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
                            MapMeasureManager.Create();
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
                            RegionManager.Create();
                        }

                        if (MapStateMediator.CurrentMapRegion != null)
                        {
                            if (ModifierKeys == Keys.Shift)
                            {
                                RegionManager.SnapRegionToLandformCoastline();
                            }
                            else
                            {
                                MapRegionPoint mrp = new(MapStateMediator.CurrentCursorPoint);
                                MapStateMediator.CurrentMapRegion.MapRegionPoints.Add(mrp);
                            }
                        }

                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (MapStateMediator.CurrentMapRegion != null && RegionManager.NewRegionPoint != null)
                        {
                            Cmd_AddMapRegionPoint cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, RegionManager.NewRegionPoint, RegionManager.NextRegionPointIndex);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    Cursor = Cursors.Cross;
                    MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                    break;
                case MapDrawingMode.DrawingLine:
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        Cmd_DrawOnCanvas cmd = new(MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER), SKGLRenderControl.GRContext);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingPaint:
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        Cmd_DrawOnCanvas cmd = new(MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER), SKGLRenderControl.GRContext);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingRectangle:
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        if (DrawingMediator.FillDrawnShape)
                        {
                            // initialize the fill paint
                            DrawingMediator.FillPaint.Color = DrawingMediator.DrawingFillColor.ToSKColor();
                            DrawingMediator.FillPaint.StrokeWidth = DrawingMediator.DrawingLineBrushSize;

                            if (DrawingMediator.FillType == DrawingFillType.Texture)
                            {
                                Bitmap? fillTexture = DrawingMediator.DrawingTextureList[DrawingMediator.DrawingTextureIndex].TextureBitmap;

                                if (fillTexture != null)
                                {
                                    // if the fill type is texture, we need to create a shader from the bitmap
                                    SKShader fillShader = SKShader.CreateBitmap(fillTexture.ToSKBitmap(), SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                                    DrawingMediator.FillPaint.Shader = fillShader;
                                }
                            }
                            else
                            {
                                DrawingMediator.FillPaint.Shader = null; // reset shader if not using texture fill
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingEllipse:
                    {
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        if (DrawingMediator.FillDrawnShape)
                        {
                            // initialize the fill paint
                            DrawingMediator.FillPaint.Color = DrawingMediator.DrawingFillColor.ToSKColor();
                            DrawingMediator.FillPaint.StrokeWidth = DrawingMediator.DrawingLineBrushSize;

                            if (DrawingMediator.FillType == DrawingFillType.Texture)
                            {
                                Bitmap? fillTexture = DrawingMediator.DrawingTextureList[DrawingMediator.DrawingTextureIndex].TextureBitmap;

                                if (fillTexture != null)
                                {
                                    // if the fill type is texture, we need to create a shader from the bitmap
                                    SKShader fillShader = SKShader.CreateBitmap(fillTexture.ToSKBitmap(), SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                                    DrawingMediator.FillPaint.Shader = fillShader;
                                }
                            }
                            else
                            {
                                DrawingMediator.FillPaint.Shader = null; // reset shader if not using texture fill
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingPolygon:
                    {
                        DrawingMediator.PolygonPoints.Add(MapStateMediator.CurrentCursorPoint);
                        Cursor = Cursors.Cross;
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                        if (DrawingMediator.FillDrawnShape)
                        {
                            // initialize the fill paint
                            DrawingMediator.FillPaint.Color = DrawingMediator.DrawingFillColor.ToSKColor();
                            DrawingMediator.FillPaint.StrokeWidth = DrawingMediator.DrawingLineBrushSize;

                            if (DrawingMediator.FillType == DrawingFillType.Texture)
                            {
                                Bitmap? fillTexture = DrawingMediator.DrawingTextureList[DrawingMediator.DrawingTextureIndex].TextureBitmap;

                                if (fillTexture != null)
                                {
                                    // if the fill type is texture, we need to create a shader from the bitmap
                                    SKShader fillShader = SKShader.CreateBitmap(fillTexture.ToSKBitmap(), SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                                    DrawingMediator.FillPaint.Shader = fillShader;
                                }
                            }
                            else
                            {
                                DrawingMediator.FillPaint.Shader = null; // reset shader if not using texture fill
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingStamp:
                    {
                        Cursor = Cursors.Cross;

                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas;
                        if (canvas == null) return;

                        if (DrawingMediator.DrawingStampBitmap != null)
                        {
                            // place the stamp at the cursor position
                            DrawingManager.PlaceStampAtCursor(canvas, MapStateMediator.CurrentCursorPoint);
                        }

                        SKGLRenderControl.Invalidate();
                    }
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

        private void RightButtonMouseDownHandler()
        {
            switch (MainMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.LabelSelect:
                    {
                        if (MapStateMediator.SelectedMapLabel != null)
                        {
                            LabelManager.CreatingLabel = true;

                            Font labelFont = new(MapStateMediator.SelectedMapLabel.LabelFont.FontFamily,
                                MapStateMediator.SelectedMapLabel.LabelFont.Size * MainMediator.DrawingZoom,
                                MapStateMediator.SelectedMapLabel.LabelFont.Style, GraphicsUnit.Pixel);

                            Size labelSize = TextRenderer.MeasureText(MapStateMediator.SelectedMapLabel.LabelText, labelFont,
                                new Size(int.MaxValue, int.MaxValue),
                                TextFormatFlags.Default | TextFormatFlags.SingleLine | TextFormatFlags.TextBoxControl);

                            Rectangle textBoxRect = LabelManager.GetLabelTextBoxRect(MapStateMediator.SelectedMapLabel,
                                MapStateMediator.DrawingPoint, MainMediator.DrawingZoom, labelSize);

                            LabelManager.LabelTextBox = LabelManager.CreateLabelEditTextBox(SKGLRenderControl, MapStateMediator.SelectedMapLabel, textBoxRect);

                            if (LabelManager.LabelTextBox != null)
                            {
                                LabelManager.LabelTextBox.KeyPress += LabelMediator.LabelTextBox_KeyPress;

                                SKGLRenderControl.Controls.Add(LabelManager.LabelTextBox);

                                LabelManager.LabelTextBox.BringToFront();
                                LabelManager.LabelTextBox.Select(LabelManager.LabelTextBox.Text.Length, 0);
                                LabelManager.LabelTextBox.Focus();
                                LabelManager.LabelTextBox.ScrollToCaret();

                                LabelManager.LabelTextBox.Tag = MapStateMediator.SelectedMapLabel.LabelPath;

                                LabelManager.Delete();
                            }

                            SKGLRenderControl.Refresh();
                        }
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    if (MapStateMediator.CurrentMapMeasure != null)
                    {
                        MapMeasureManager.EndMapMeasure(MapStateMediator.CurrentMapMeasure, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                        MainMediator.SetDrawingMode(MapDrawingMode.None, 0);
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    if (MapStateMediator.CurrentMapRegion != null)
                    {
                        RegionManager.EndMapRegion();

                        MapStateMediator.CurrentMap.IsSaved = false;

                        // reset everything
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER2).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapStateMediator.CurrentMapRegion = null;
                        MainMediator.SetDrawingMode(MapDrawingMode.None, 0);
                    }

                    break;
            }
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Move Event Handling Methods (called from event handlers)

        #region Left Button Move Handler Method

        private void LeftButtonMouseMoveHandler()
        {
            switch (MainMediator.CurrentDrawingMode)
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

                            MapStateMediator.CurrentLandform.DrawPath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, MainMediator.SelectedBrushSize / 2);

                            bool createPathsWhilePainting = Settings.Default.CalculateContoursWhilePainting;

                            if (createPathsWhilePainting)
                            {
                                // compute contour path and inner and outer paths in a separate thread
                                Task.Run(() => LandformManager.CreateAllPathsFromDrawnPath(MapStateMediator.CurrentMap, MapStateMediator.CurrentLandform));
                            }
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        LandformManager.LandformErasePath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, MainMediator.SelectedBrushSize / 2);
                        LandformManager.EraseFromLandform(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, MainMediator.SelectedBrushSize / 2);

                        LandformManager.LandformErasePath.Reset();

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandformSelect:
                    {
                        if (MapStateMediator.SelectedLandform != null)
                        {
                            LandformManager.MoveLandform(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);

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
                            MapStateMediator.CurrentWaterFeature.WaterFeaturePath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, MainMediator.SelectedBrushSize / 2);

                            // compute contour path and inner and outer paths in a separate thread
                            Task.Run(() => WaterFeatureManager.CreateInnerAndOuterPaths(MapStateMediator.CurrentMap, MapStateMediator.CurrentWaterFeature));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterErase:
                    {
                        Cursor = Cursors.Cross;

                        WaterFeatureManager.WaterFeaturErasePath.AddCircle(MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y, MainMediator.SelectedBrushSize / 2);

                        WaterFeatureManager.EraseWaterFeature(MapStateMediator.CurrentMap);

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
                            WaterFeatureManager.ConstructRiverPaths(MapStateMediator.CurrentRiver);
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    if (MapStateMediator.SelectedWaterFeature != null && MapStateMediator.SelectedWaterFeature is River river && MapStateMediator.SelectedRiverPoint != null)
                    {
                        // move the selected point on the path
                        WaterFeatureManager.MoveSelectedRiverPoint(river, MapStateMediator.SelectedRiverPoint, MapStateMediator.CurrentCursorPoint);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    {
                        Cursor = Cursors.Cross;
                        const int minimumPathPointCount = 5;

                        SKPoint newPathPoint = PathManager.GetNewPathPoint(MapStateMediator.CurrentMapPath, ModifierKeys, SELECTED_PATH_ANGLE, MapStateMediator.CurrentCursorPoint, minimumPathPointCount);

                        PathManager.AddNewPathPoint(MapStateMediator.CurrentMapPath, newPathPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        if (MapStateMediator.SelectedMapPath != null)
                        {
                            PathManager.MovePath(MapStateMediator.SelectedMapPath, MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);

                            MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    if (MapStateMediator.SelectedMapPathPoint != null)
                    {
                        // move the selected point on the path
                        PathManager.MoveSelectedMapPathPoint(MapStateMediator.SelectedMapPath, MapStateMediator.SelectedMapPathPoint, MapStateMediator.CurrentCursorPoint);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.SymbolPlace:
                    if (!SymbolMediator.UseAreaBrush)
                    {
                        SymbolManager.PlaceSelectedSymbolAtCursor(MapStateMediator.CurrentCursorPoint);
                    }

                    MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.SymbolErase:
                    int eraserRadius = SymbolMediator.AreaBrushSize / 2;

                    SymbolManager.RemovePlacedSymbolsFromArea(MapStateMediator.CurrentCursorPoint, eraserRadius);

                    MapStateMediator.CurrentMap.IsSaved = false;
                    break;
                case MapDrawingMode.SymbolColor:

                    if (SymbolMediator.UseAreaBrush)
                    {
                        int colorBrushRadius = SymbolMediator.AreaBrushSize / 2;

                        Color[] symbolColors = [SymbolColor1Button.BackColor, SymbolColor2Button.BackColor, SymbolColor3Button.BackColor];
                        SymbolManager.ColorSymbolsInArea(MapStateMediator.CurrentCursorPoint, colorBrushRadius, symbolColors, RandomizeColorCheck.Checked);

                    }
                    else
                    {
                        SymbolManager.ColorSymbolAtPoint();
                    }

                    MapStateMediator.CurrentMap.IsSaved = false;
                    SKGLRenderControl.Invalidate();
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
                        BoxManager.MoveBox(MapStateMediator.SelectedPlacedMapBox, MapStateMediator.CurrentCursorPoint);
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

                        if (LabelManager.CurrentMapLabelPath != null)
                        {
                            LabelManager.DrawLabelPathOnWorkLayer(MapStateMediator.CurrentMap, LabelManager.CurrentMapLabelPath);
                        }

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
                            MapMeasureManager.DrawMapMeasureOnWorkLayer(MapStateMediator.CurrentMapMeasure,
                                MapStateMediator.CurrentCursorPoint, MapStateMediator.PreviousCursorPoint);
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        if (MapStateMediator.CurrentMapRegion != null)
                        {
                            RegionManager.DrawRegionOnWorkLayer();

                            if (ModifierKeys == Keys.Shift)
                            {
                                RegionManager.DrawCoastlinePointOnWorkLayer2();
                            }

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

                            if (!RegionManager.EditingRegion)
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
                        HeightMapManager.ChangeHeightMapAreaHeight(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, MainMediator.SelectedBrushSize / 2, brushStrength);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightDecrease:
                    {
                        float brushStrength = (float)BrushStrengthUpDown.Value;
                        HeightMapManager.ChangeHeightMapAreaHeight(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, MainMediator.SelectedBrushSize / 2, -brushStrength);

                        MapStateMediator.CurrentMap.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingLine:
                    {
                        // draw a line from the previous cursor point to the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas;
                        if (canvas == null) return;

                        using SKPaint paint = new()
                        {
                            Style = SKPaintStyle.StrokeAndFill,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Round
                        };

                        canvas.DrawLine(MapStateMediator.PreviousCursorPoint, MapStateMediator.CurrentCursorPoint, paint);

                        // update the previous cursor point
                        MapStateMediator.PreviousCursorPoint = MapStateMediator.CurrentCursorPoint;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingPaint:
                    {
                        // draw with a brush at the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas;
                        if (canvas == null) return;

                        DrawingManager.Paint(canvas, MapStateMediator.CurrentCursorPoint, DrawingMediator.DrawingLineBrushSize);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingRectangle:
                    {
                        // draw a rectangle from the previous cursor point to the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas;
                        canvas?.Clear(SKColors.Transparent);
                        if (canvas == null) return;

                        using SKPaint strokepaint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Butt
                        };

                        SKRect rect = new(MapStateMediator.PreviousCursorPoint.X, MapStateMediator.PreviousCursorPoint.Y,
                            MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                        if (RealmStudioMainForm.ModifierKeys == Keys.Control)
                        {
                            // if the ctrl key is pressed, make the rectangle a square
                            float size = Math.Max(rect.Width, rect.Height);
                            rect = new SKRect(rect.Left, rect.Top, rect.Left + size, rect.Top + size);
                        }

                        if (DrawingMediator.FillDrawnShape)
                        {
                            // draw the filled rectangle first if the fill is enabled
                            canvas.DrawRect(rect, DrawingMediator.FillPaint);
                        }

                        canvas.DrawRect(rect, strokepaint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingErase:
                    {
                        // erase at the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas;
                        if (canvas == null) return;

                        using SKPaint paint = new()
                        {
                            Style = SKPaintStyle.Fill,
                            Color = SKColors.Transparent,
                            StrokeWidth = MainMediator.SelectedBrushSize / 2,
                            IsAntialias = true,
                            BlendMode = SKBlendMode.Clear
                        };

                        canvas.DrawCircle(MapStateMediator.CurrentCursorPoint, MainMediator.SelectedBrushSize / 2, paint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingEllipse:
                    {
                        // draw an ellipse from the previous cursor point to the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas;
                        canvas?.Clear(SKColors.Transparent);
                        if (canvas == null) return;
                        using SKPaint strokepaint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Butt
                        };

                        SKRect rect = new(MapStateMediator.PreviousCursorPoint.X, MapStateMediator.PreviousCursorPoint.Y,
                            MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                        if (RealmStudioMainForm.ModifierKeys == Keys.Control)
                        {
                            // if the ctrl key is pressed, make the ellipse a circle
                            float size = Math.Max(rect.Width, rect.Height);
                            rect = new SKRect(rect.Left, rect.Top, rect.Left + size, rect.Top + size);
                        }

                        // draw the filled ellipse first if the fill is enabled
                        if (DrawingMediator.FillDrawnShape)
                        {
                            // draw the filled rectangle first if the fill is enabled
                            canvas.DrawOval(rect, DrawingMediator.FillPaint);
                        }

                        // draw the outline of the ellipse
                        canvas.DrawOval(rect, strokepaint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawingPolygon:
                    {
                        // draw a polygon from the previous cursor point to the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas;
                        canvas?.Clear(SKColors.Transparent);
                        if (canvas == null) return;

                        using SKPaint strokepaint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Round,
                            StrokeJoin = SKStrokeJoin.Round
                        };

                        if (DrawingMediator.PolygonPoints.Count < 1)
                        {
                            return;
                        }
                        else if (DrawingMediator.PolygonPoints.Count == 1)
                        {
                            canvas.DrawLine(MapStateMediator.PreviousCursorPoint, MapStateMediator.CurrentCursorPoint, strokepaint);
                        }
                        else if (DrawingMediator.PolygonPoints.Count > 1)
                        {
                            DrawingMediator.PolygonPoints.Add(MapStateMediator.CurrentCursorPoint);

                            SKPath polyPath = DrawingMethods.GetLinePathFromPoints(DrawingMediator.PolygonPoints);

                            // draw the filled polygon first if the fill is enabled
                            if (DrawingMediator.FillDrawnShape)
                            {
                                // draw the filled rectangle first if the fill is enabled
                                canvas.DrawPath(polyPath, DrawingMediator.FillPaint);
                            }

                            // draw the outline of the polygon
                            canvas.DrawPath(polyPath, strokepaint);

                            DrawingMediator.PolygonPoints.RemoveAt(DrawingMediator.PolygonPoints.Count - 1);
                        }

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

        private void NoButtonMouseMoveHandler()
        {
            switch (MainMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.PlaceWindrose:
                    {
                        WindroseManager.MoveWindrose(MapStateMediator.CurrentWindrose, MapStateMediator.CurrentCursorPoint);
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        MapStateMediator.SelectedRiverPoint = WaterFeatureManager.GetSelectedRiverPoint(MapStateMediator.SelectedWaterFeature, MapStateMediator.CurrentCursorPoint);
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        MapStateMediator.SelectedMapPathPoint = PathManager.GetSelectedPathPoint(MapStateMediator.SelectedMapPath, MapStateMediator.CurrentCursorPoint);
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        if (MapStateMediator.CurrentMapRegion != null)
                        {
                            // draw the region on the work layer
                            RegionManager.DrawRegionOnWorkLayer();
                        }

                        if (ModifierKeys == Keys.Shift)
                        {
                            RegionManager.DrawCoastlinePointOnWorkLayer2();
                        }

                        SKGLRenderControl.Invalidate();
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
                                RegionManager.DrawRegionPointOnWorkLayer();
                            }
                        }
                    }
                    break;
            }

            SKGLRenderControl.Invalidate();
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Up Event Handling Methods (called from event handers)

        #region Left Button Up Handler

        private void LeftButtonMouseUpHandler(MouseEventArgs e)
        {
            ApplicationTimerManager.SymbolAreaBrushTimerEnabled = false;

            switch (MainMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.OceanPaint:
                    {
                        OceanManager.StopColorPainting(ApplicationTimerManager);
                    }
                    break;
                case MapDrawingMode.OceanErase:
                    {
                        OceanManager.StopColorErasing();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    if (MapStateMediator.CurrentLandform != null)
                    {
                        Cmd_AddNewLandform cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentLandform);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        LandformManager.MergeLandforms(MapStateMediator.CurrentMap);

                        MapStateMediator.CurrentLandform = null;
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        LandformManager.EraseFromLandform(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint, MainMediator.SelectedBrushSize / 2);
                        LandformManager.LandformErasePath.Reset();
                    }
                    break;
                case MapDrawingMode.LandColor:
                    {
                        LandformManager.StopColorPainting(ApplicationTimerManager);
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        LandformManager.StopColorErasing();
                    }
                    break;
                case MapDrawingMode.PlaceWindrose:
                    WindroseManager.AddWindrowseAtCurrentPoint();
                    break;
                case MapDrawingMode.WaterPaint:
                    {
                        WaterFeatureManager.AddWaterFeatureToMap();
                    }
                    break;
                case MapDrawingMode.LakePaint:
                    MapStateMediator.CurrentWaterFeature = null;
                    break;
                case MapDrawingMode.RiverPaint:
                    if (MapStateMediator.CurrentRiver != null)
                    {
                        WaterFeatureManager.ConstructRiverPaths(MapStateMediator.CurrentRiver);

                        Cmd_AddNewRiver cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentRiver);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        MapStateMediator.CurrentRiver = null;
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        MapStateMediator.SelectedRiverPoint = null;
                    }
                    break;
                case MapDrawingMode.WaterColor:
                    {
                        WaterFeatureManager.StopColorPainting(ApplicationTimerManager);
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        WaterFeatureManager.StopColorErasing();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    if (MapStateMediator.CurrentMapPath != null)
                    {
                        MapStateMediator.CurrentMapPath.BoundaryPath = PathManager.GenerateMapPathBoundaryPath(MapStateMediator.CurrentMapPath.PathPoints);

                        Cmd_AddNewMapPath cmd = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapPath);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        MapStateMediator.CurrentMapPath = null;
                        SELECTED_PATH_ANGLE = -1;

                        MapStateMediator.CurrentMap.IsSaved = false;
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        MapStateMediator.SelectedMapPath = PathUIMediator.SelectMapPathAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        if (MapStateMediator.CurrentMapPath == null)
                        {
                            MapStateMediator.SelectedMapPath = PathUIMediator.SelectMapPathAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                            PathUIMediator.SetShowPathPoints();
                        }

                        MapStateMediator.SelectedMapPathPoint = null;
                    }
                    break;
                case MapDrawingMode.SymbolSelect:
                    {
                        MapStateMediator.SelectedMapSymbol = SymbolUIMediator.SelectMapSymbolAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint.ToDrawingPoint());

                        if (MapStateMediator.SelectedMapSymbol != null)
                        {
                            MapStateMediator.SelectedMapSymbol.IsSelected = !MapStateMediator.SelectedMapSymbol.IsSelected;
                        }
                    }
                    break;
                case MapDrawingMode.DrawLabel:
                    if (!LabelManager.CreatingLabel)
                    {
                        LabelManager.CreatingLabel = true;

                        Font tbFont = new(LabelMediator.SelectedLabelFont.FontFamily,
                                LabelMediator.SelectedLabelFont.Size * 0.75F, LabelMediator.SelectedLabelFont.Style, GraphicsUnit.Point);

                        Size labelSize = TextRenderer.MeasureText("...Label...", tbFont,
                            new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.Default | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.ExternalLeading | TextFormatFlags.SingleLine);

                        Rectangle labelRect = new(e.X - (labelSize.Width / 2), e.Y - (labelSize.Height / 2), labelSize.Width, labelSize.Height);

                        LabelManager.LabelTextBox = LabelManager.CreateNewLabelTextbox(SKGLRenderControl, tbFont,
                            labelRect, FontColorSelectButton.BackColor);

                        if (LabelManager.LabelTextBox != null)
                        {
                            LabelManager.LabelTextBox.KeyPress += LabelMediator.LabelTextBox_KeyPress;

                            SKGLRenderControl.Controls.Add(LabelManager.LabelTextBox);

                            LabelManager.LabelTextBox.BringToFront();
                            LabelManager.LabelTextBox.Select(LabelManager.LabelTextBox.Text.Length, 0);
                            LabelManager.LabelTextBox.Focus();
                            LabelManager.LabelTextBox.ScrollToCaret();
                        }
                    }
                    else
                    {
                        if (LabelManager.LabelTextBox != null)
                        {
                            LabelManager.LabelTextBox.BringToFront();
                            LabelManager.LabelTextBox.Focus();
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
                    BoxManager.Create();

                    MapStateMediator.CurrentMap.IsSaved = false;
                    MapStateMediator.SelectedPlacedMapBox = null;

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
                        if (RegionManager.EditingRegion)
                        {
                            RegionManager.EditingRegion = false;
                        }
                        else
                        {
                            RegionUIMediator.SelectRegionAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                        }
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

                        MainMediator.SetDrawingMode(MapDrawingMode.None, 0);
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    {
                        MapStateMediator.SelectedRealmArea = new(MapStateMediator.PreviousCursorPoint.X, MapStateMediator.PreviousCursorPoint.Y, MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawRect((SKRect)MapStateMediator.SelectedRealmArea, PaintObjects.LandformAreaSelectPaint);
                    }
                    break;
                case MapDrawingMode.MapHeightIncrease:
                    {
                        // update the 3D view if it is open and updates are enabled
                        if (HeightMapManager.CurrentHeightMapView != null && Update3DViewSwitch.Checked)
                        {
                            try
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                SKBitmap? heightMapBitmap = HeightMapManager.GetBitmapForThreeDView(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, MapStateMediator.SelectedRealmArea);
                                if (heightMapBitmap != null)
                                {
                                    // generate the 3D model from the height information in the selected area
                                    Task updateTask = Task.Run(() =>
                                    {
                                        List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                                        // convert the list of strings to a single string
                                        string objModelString = string.Join(Environment.NewLine, objModelStringList);

                                        lock (HeightMapManager.LandFormObjModelList) // Ensure thread safety
                                        {
                                            HeightMapManager.LandFormObjModelList.Add(objModelString);
                                        }
                                    });

                                    updateTask.Wait();
                                    HeightMapManager.CurrentHeightMapView.UpdateView();
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
                        if (HeightMapManager.CurrentHeightMapView != null && Update3DViewSwitch.Checked)
                        {
                            try
                            {
                                Cursor.Current = Cursors.WaitCursor;

                                SKBitmap? heightMapBitmap = HeightMapManager.GetBitmapForThreeDView(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, MapStateMediator.SelectedRealmArea);
                                if (heightMapBitmap != null)
                                {
                                    // generate the 3D model from the height information in the selected area
                                    Task updateTask = Task.Run(() =>
                                    {
                                        List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                                        // convert the list of strings to a single string
                                        string objModelString = string.Join(Environment.NewLine, objModelStringList);

                                        lock (HeightMapManager.LandFormObjModelList) // Ensure thread safety
                                        {
                                            HeightMapManager.LandFormObjModelList.Add(objModelString);
                                        }
                                    });

                                    updateTask.Wait();
                                    HeightMapManager.CurrentHeightMapView.UpdateView();
                                }
                            }
                            finally
                            {
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        break;
                    }

                case MapDrawingMode.DrawingRectangle:
                    {
                        // finalize rectangle drawing and add the rectangle
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        SKRect rect = new(MapStateMediator.PreviousCursorPoint.X, MapStateMediator.PreviousCursorPoint.Y,
                            MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                        using SKPaint paint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Butt
                        };

                        if (RealmStudioMainForm.ModifierKeys == Keys.Control)
                        {
                            // if the ctrl key is pressed, make the rectangle a square
                            float size = Math.Max(rect.Width, rect.Height);
                            rect = new SKRect(rect.Left, rect.Top, rect.Left + size, rect.Top + size);
                        }

                        Cmd_DrawOnCanvas cmd = new(MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER), SKGLRenderControl.GRContext);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        if (DrawingMediator.FillDrawnShape)
                        {
                            // draw the filled rectangle first if the fill is enabled
                            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas.DrawRect(rect, DrawingMediator.FillPaint);
                        }

                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas.DrawRect(rect, paint);
                    }
                    break;
                case MapDrawingMode.DrawingEllipse:
                    {
                        // finalize ellipse drawing and add the ellipse
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        SKRect rect = new(MapStateMediator.PreviousCursorPoint.X, MapStateMediator.PreviousCursorPoint.Y,
                            MapStateMediator.CurrentCursorPoint.X, MapStateMediator.CurrentCursorPoint.Y);

                        using SKPaint paint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Butt
                        };

                        if (RealmStudioMainForm.ModifierKeys == Keys.Control)
                        {
                            // if the ctrl key is pressed, make the ellipse a circle
                            float size = Math.Max(rect.Width, rect.Height);
                            rect = new SKRect(rect.Left, rect.Top, rect.Left + size, rect.Top + size);
                        }

                        Cmd_DrawOnCanvas cmd = new(MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER), SKGLRenderControl.GRContext);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        if (DrawingMediator.FillDrawnShape)
                        {
                            // draw the filled ellipse first if the fill is enabled
                            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas.DrawOval(rect, DrawingMediator.FillPaint);
                        }

                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas.DrawOval(rect, paint);
                    }
                    break;
            }

            SKGLRenderControl.Invalidate();

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
            switch (MainMediator.CurrentDrawingMode)
            {
                case MapDrawingMode.LandformSelect:

                    LandformSelectButton.Checked = false;

                    MapStateMediator.SelectedLandform = LandformManager.SelectLandformAtPoint(MapStateMediator.CurrentCursorPoint);

                    if (MapStateMediator.SelectedLandform != null)
                    {
                        LandformInfo landformInfo = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedLandform, AssetManager.CURRENT_THEME, SKGLRenderControl);
                        landformInfo.ShowDialog(this);
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    MapComponent? selectedWaterFeature = WaterFeatureMediator.SelectWaterFeatureAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

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

                    break;
                case MapDrawingMode.PathSelect:
                    MapPath? selectedPath = PathUIMediator.SelectMapPathAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                    if (selectedPath != null)
                    {
                        MapStateMediator.SelectedMapPath = selectedPath;

                        MapPathInfo pathInfo = new(MapStateMediator.CurrentMap, selectedPath, SKGLRenderControl);
                        pathInfo.ShowDialog(this);
                    }
                    break;
                case MapDrawingMode.SymbolSelect:
                    MapStateMediator.SelectedMapSymbol = SymbolUIMediator.SelectMapSymbolAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint.ToDrawingPoint());

                    if (MapStateMediator.SelectedMapSymbol != null)
                    {
                        MapSymbolInfo msi = new(MapStateMediator.CurrentMap, MapStateMediator.SelectedMapSymbol);
                        msi.ShowDialog(this);
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    RegionUIMediator.SelectRegionAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

                    if (MapStateMediator.CurrentMapRegion != null)
                    {
                        MapRegionInfo mri = new(MapStateMediator.CurrentMap, MapStateMediator.CurrentMapRegion, SKGLRenderControl);
                        mri.ShowDialog(this);
                    }
                    break;
                case MapDrawingMode.DrawingPolygon:
                    {
                        MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        // draw a polygon from the previous cursor point to the current cursor point
                        SKCanvas? canvas = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER).LayerSurface?.Canvas;
                        if (canvas == null) return;

                        using SKPaint strokepaint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = DrawingMediator.DrawingLineColor.ToSKColor(),
                            StrokeWidth = DrawingMediator.DrawingLineBrushSize,
                            IsAntialias = true,
                            StrokeCap = SKStrokeCap.Round,
                            StrokeJoin = SKStrokeJoin.Round
                        };

                        if (DrawingMediator.PolygonPoints.Count < 2)
                        {
                            // if there are not enough points to draw a polygon, just return
                            return;
                        }
                        else if (DrawingMediator.PolygonPoints.Count == 2)
                        {
                            canvas.DrawLine(MapStateMediator.PreviousCursorPoint, MapStateMediator.CurrentCursorPoint, strokepaint);
                        }
                        else
                        {
                            DrawingMediator.PolygonPoints.Add(MapStateMediator.CurrentCursorPoint);

                            SKPath polyPath = DrawingMethods.GetLinePathFromPoints(DrawingMediator.PolygonPoints);

                            Cmd_DrawOnCanvas cmd = new(MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER), SKGLRenderControl.GRContext);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            if (DrawingMediator.FillDrawnShape)
                            {
                                // draw the filled rectangle first if the fill is enabled
                                canvas.DrawPath(polyPath, DrawingMediator.FillPaint);
                            }

                            // draw the outline of the polygon
                            canvas.DrawPath(polyPath, strokepaint);
                        }

                        DrawingMediator.PolygonPoints.Clear(); // clear the polygon points after drawing

                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }

            SKGLRenderControl.Invalidate();
        }

        #endregion

        #endregion

        #region SKGLRenderControl KeyDown Handler
        //private void SKGLRenderControl_KeyDown(object sender, KeyEventArgs e)
        //{
        //    -- for some unknown reason, this method was not being called when a key
        //    -- is pressed in the SKGLRenderControl, when it was before.
        //    -- key presses are now handled in the RealmStudioMainForm keydown event handler.
        //    KeyHandler.HandleKey(e.KeyCode);
        //    e.Handled = true;
        //    SKGLRenderControl.Invalidate();
        //}
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
            BackgroundMediator.BackgroundTextureIndex++;
        }

        private void PreviousBackgroundTextureButton_Click(object sender, EventArgs e)
        {
            BackgroundMediator.BackgroundTextureIndex--;
        }

        private void BackgroundTextureScaleTrack_Scroll(object sender, EventArgs e)
        {
            BackgroundMediator.BackgroundTextureScale = BackgroundTextureScaleTrack.Value / 100.0F;
            TOOLTIP.Show(BackgroundMediator.BackgroundTextureScale.ToString(), BackgroundTextureGroup, new Point(BackgroundTextureScaleTrack.Right - 30, BackgroundTextureScaleTrack.Top - 20), 2000);
        }

        private void MirrorBackgroundSwitch_CheckedChanged()
        {
            BackgroundMediator.MirrorBackgroundTexture = MirrorBackgroundSwitch.Checked;
        }

        private void FillBackgroundButton_Click(object sender, EventArgs e)
        {
            BackgroundManager.FillBackgroundTexture();
            SKGLRenderControl.Invalidate();
        }

        private void ClearBackgroundButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.CurrentMap.IsSaved = false;
            BackgroundManager.ClearBackgroundTexture();
            SKGLRenderControl.Invalidate();
        }

        private void VignetteColorSelectionButton_MouseUp(object sender, MouseEventArgs e)
        {
            VignetteMediator.VignetteColor = UtilityMethods.SelectColor(this, e, VignetteColorSelectionButton.BackColor);
        }

        private void VignetteStrengthTrack_Scroll(object sender, EventArgs e)
        {
            VignetteMediator.VignetteStrength = VignetteStrengthTrack.Value;
            TOOLTIP.Show(VignetteMediator.VignetteStrength.ToString(), VignetteGroupBox, new Point(VignetteStrengthTrack.Right - 30, VignetteStrengthTrack.Top - 20), 2000);
        }

        private void OvalVignetteRadio_CheckedChanged(object sender, EventArgs e)
        {
            VignetteMediator.VignetteShape = VignetteShapeType.Oval;
        }

        private void RectangleVignetteRadio_CheckedChanged(object sender, EventArgs e)
        {
            VignetteMediator.VignetteShape = VignetteShapeType.Rectangle;
        }

        #endregion

        #region Ocean Tab Event Handlers

        private void ShowOceanLayerSwitch_CheckedChanged()
        {
            OceanMediator.ShowOceanLayers = ShowOceanLayerSwitch.Checked;
        }

        private void OceanTextureOpacityTrack_Scroll(object sender, EventArgs e)
        {
            OceanMediator.OceanTextureOpacity = OceanTextureOpacityTrack.Value / 100.0F;
            TOOLTIP.Show(OceanMediator.OceanTextureOpacity.ToString(), OceanTextureGroup, new Point(OceanTextureOpacityTrack.Right - 30, OceanTextureOpacityTrack.Top - 20), 2000);
        }

        private void PreviousOceanTextureButton_Click(object sender, EventArgs e)
        {
            OceanMediator.OceanTextureIndex--;
        }

        private void NextOceanTextureButton_Click(object sender, EventArgs e)
        {
            OceanMediator.OceanTextureIndex++;
        }

        private void OceanScaleTextureTrack_Scroll(object sender, EventArgs e)
        {
            OceanMediator.OceanTextureScale = OceanScaleTextureTrack.Value / 100.0F;
            TOOLTIP.Show(OceanMediator.OceanTextureScale.ToString(), OceanTextureGroup, new Point(OceanScaleTextureTrack.Right - 30, OceanScaleTextureTrack.Top - 20), 2000);
        }

        private void MirrorOceanTextureSwitch_CheckedChanged()
        {
            OceanMediator.MirrorOceanTexture = MirrorOceanTextureSwitch.Checked;
        }

        private void OceanApplyTextureButton_Click(object sender, EventArgs e)
        {
            OceanManager.ApplyOceanTexture();
            SKGLRenderControl.Invalidate();
        }
        private void OceanRemoveTextureButton_Click(object sender, EventArgs e)
        {
            OceanManager.RemoveOceanTexture();
            SKGLRenderControl.Invalidate();
        }


        private void OceanColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            OceanMediator.OceanFillColor = UtilityMethods.SelectColor(this, e, OceanColorSelectButton.BackColor);
        }

        private void OceanColorFillButton_Click(object sender, EventArgs e)
        {
            OceanManager.FillOceanColor();
            SKGLRenderControl.Invalidate();
        }

        private void OceanColorClearButton_Click(object sender, EventArgs e)
        {
            OceanManager.ClearOceanColor();
            SKGLRenderControl.Invalidate();
        }

        private void OceanPaintButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.OceanPaint, OceanMediator.OceanPaintBrushSize);
        }

        private void OceanColorEraseButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.OceanErase, OceanMediator.OceanPaintBrushSize);
        }

        private void OceanPaintColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, OceanPaintColorSelectButton.BackColor);
            OceanMediator.OceanPaintColor = selectedColor;
        }

        private void OceanPaintColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            OceanMediator.OceanPaintColor = UtilityMethods.SelectColor(this, e, OceanPaintColorSelectButton.BackColor);
        }

        private void OceanSoftBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
            OceanMediator.OceanPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void OceanHardBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
            OceanMediator.OceanPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void OceanPatternBrush1Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush1;
            OceanMediator.OceanPaintBrush = ColorPaintBrush.PatternBrush1;
        }

        private void OceanPatternBrush2Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush2;
            OceanMediator.OceanPaintBrush = ColorPaintBrush.PatternBrush2;
        }

        private void OceanPatternBrush3Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush3;
            OceanMediator.OceanPaintBrush = ColorPaintBrush.PatternBrush3;
        }

        private void OceanPatternBrush4Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush4;
            OceanMediator.OceanPaintBrush = ColorPaintBrush.PatternBrush4;
        }

        private void OceanBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMediator.OceanPaintBrushSize = OceanBrushSizeTrack.Value;
            MainMediator.SelectedBrushSize = OceanMediator.OceanPaintBrushSize;

            TOOLTIP.Show(OceanMediator.OceanPaintBrushSize.ToString(), OceanToolPanel, new Point(OceanBrushSizeTrack.Right - 30, OceanBrushSizeTrack.Top - 20), 2000);
        }

        private void OceanEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMediator.OceanPaintEraserSize = OceanEraserSizeTrack.Value;
            MainMediator.SelectedBrushSize = OceanMediator.OceanPaintEraserSize;

            TOOLTIP.Show(OceanMediator.OceanPaintEraserSize.ToString(), OceanToolPanel, new Point(OceanEraserSizeTrack.Right - 30, OceanEraserSizeTrack.Top - 20), 2000);
        }

        private void OceanBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMediator.OceanBrushVelocity = OceanBrushVelocityTrack.Value / 100.0F;
            MainMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / OceanMediator.OceanBrushVelocity);

            TOOLTIP.Show(OceanMediator.OceanBrushVelocity.ToString(), OceanToolPanel, new Point(OceanBrushVelocityTrack.Right - 30, OceanBrushVelocityTrack.Top - 20), 2000);
        }

        private void OceanButton91CBB8_Click(object sender, EventArgs e)
        {
            OceanMediator.SetOceanColorFromPreset("#91CBB8");
        }

        private void OceanButton88B5BB_Click(object sender, EventArgs e)
        {
            OceanMediator.SetOceanColorFromPreset("#88B5BB");
        }

        private void OceanButton6BA5B9_Click(object sender, EventArgs e)
        {
            OceanMediator.SetOceanColorFromPreset("#6BA5B9");
        }

        private void OceanButton42718D_Click(object sender, EventArgs e)
        {
            OceanMediator.SetOceanColorFromPreset("#42718D");
        }

        private void OceanCustomColorButton1_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor1 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor1;
        }

        private void OceanCustomColorButton2_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor2 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor2;
        }

        private void OceanCustomColorButton3_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor3 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor3;
        }

        private void OceanCustomColorButton4_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor4 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor4;
        }

        private void OceanCustomColorButton5_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor5 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor5;
        }

        private void OceanCustomColorButton6_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor6 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor6;
        }

        private void OceanCustomColorButton7_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor7 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor7;
        }

        private void OceanCustomColorButton8_Click(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                OceanMediator.CustomColor8 = Color.White;
                return;
            }

            OceanMediator.OceanPaintColor = OceanMediator.CustomColor8;
        }
        #endregion

        #region Land Tab Event Handlers
        /******************************************************************************************************* 
        * LAND TAB EVENT HANDLERS
        *******************************************************************************************************/
        private void ShowLandLayerSwitch_CheckedChanged()
        {
            LandformMediator.ShowLandformLayers = ShowLandLayerSwitch.Checked;
        }

        private void LandformSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LandformSelect, 0);
        }

        private void LandformPaintButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LandPaint, LandformMediator.LandformBrushSize);
        }

        private void LandformEraseButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LandErase, LandformMediator.LandformEraserSize);
        }

        private void LandBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMediator.LandformBrushSize = LandBrushSizeTrack.Value;
            MainMediator.SelectedBrushSize = LandformMediator.LandformBrushSize;

            TOOLTIP.Show(LandformMediator.LandformBrushSize.ToString(), LandformValuesGroup, new Point(LandBrushSizeTrack.Right - 30, LandBrushSizeTrack.Top - 20), 2000);
        }

        private void LandformOutlineColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            LandformMediator.LandOutlineColor = UtilityMethods.SelectColor(this, e, LandformOutlineColorSelectButton.BackColor);
        }

        private void LandformOutlineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMediator.LandOutlineWidth = LandformOutlineWidthTrack.Value;
            TOOLTIP.Show(LandformMediator.LandOutlineWidth.ToString(), LandformValuesGroup, new Point(LandformOutlineWidthTrack.Right - 30, LandformOutlineWidthTrack.Top - 20), 2000);
        }

        private void LandformBackgroundColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            LandformMediator.LandBackgroundColor = UtilityMethods.SelectColor(this, e, LandformBackgroundColorSelectButton.BackColor);
        }

        private void PreviousTextureButton_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformTextureIndex--;
        }

        private void NextTextureButton_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformTextureIndex++;
        }

        private void UseTextureForBackgroundCheck_CheckedChanged(object sender, EventArgs e)
        {
            LandformMediator.UseTextureBackground = UseTextureForBackgroundCheck.Checked;
        }

        private void CoastlineEffectDistanceTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMediator.CoastlineEffectDistance = CoastlineEffectDistanceTrack.Value;
            TOOLTIP.Show(LandformMediator.CoastlineEffectDistance.ToString(), CoastlineValuesGroup, new Point(CoastlineEffectDistanceTrack.Right - 30, CoastlineEffectDistanceTrack.Top - 20), 2000);
        }

        private void CoastlineColorSelectionButton_MouseUp(object sender, MouseEventArgs e)
        {
            LandformMediator.CoastlineColor = UtilityMethods.SelectColor(this, e, CoastlineColorSelectionButton.BackColor);
        }

        private void CoastlineStyleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CoastlineStyleList.SelectedIndex > -1)
            {
                LandformMediator.CoastlineStyle = (string)CoastlineStyleList.Items[CoastlineStyleList.SelectedIndex];
            }
        }

        private void LandEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMediator.LandformEraserSize = LandEraserSizeTrack.Value;
            MainMediator.SelectedBrushSize = LandformMediator.LandformEraserSize;

            TOOLTIP.Show(LandformMediator.LandformEraserSize.ToString(), LandEraserGroup, new Point(LandEraserSizeTrack.Right - 30, LandEraserSizeTrack.Top - 20), 2000);
        }

        private void LandformFillButton_Click(object sender, EventArgs e)
        {
            LandformManager.FillMapWithLandForm(SKGLRenderControl);
        }

        private void LandformClearButton_Click(object sender, EventArgs e)
        {
            LandformManager.ClearAllLandforms();
            SKGLRenderControl.Invalidate();
        }

        private void LandColorButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LandColor, LandformMediator.LandPaintBrushSize);
        }

        private void LandColorEraseButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LandColorErase, LandformMediator.LandPaintEraserSize);
        }

        private void LandColorSelectionButton_MouseUp(object sender, MouseEventArgs e)
        {
            LandformMediator.LandPaintColor = UtilityMethods.SelectColor(this, e, LandColorSelectionButton.BackColor);
        }

        private void LandButtonE6D0AB_Click(object sender, EventArgs e)
        {
            LandformMediator.SetLandColorFromPreset("#E6D0AB");
        }

        private void LandButtonD8B48F_Click(object sender, EventArgs e)
        {
            LandformMediator.SetLandColorFromPreset("#D8B48F");
        }

        private void LandButtonBEBB8E_Click(object sender, EventArgs e)
        {
            LandformMediator.SetLandColorFromPreset("#BEBB8E");
        }

        private void LandButtonD7C293_Click(object sender, EventArgs e)
        {
            LandformMediator.SetLandColorFromPreset("#D7C293");
        }

        private void LandButtonAD9C7E_Click(object sender, EventArgs e)
        {
            LandformMediator.SetLandColorFromPreset("#AD9C7E");
        }

        private void LandButton3D3728_Click(object sender, EventArgs e)
        {
            LandformMediator.SetLandColorFromPreset("#3D3728");
        }

        private void LandCustomColorButton1_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                LandformMediator.CustomColor1 = Color.White;
                return;
            }

            LandformMediator.LandPaintColor = LandformMediator.CustomColor1;
        }

        private void LandCustomColorButton2_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                LandformMediator.CustomColor2 = Color.White;
                return;
            }

            LandformMediator.LandPaintColor = LandformMediator.CustomColor2;
        }

        private void LandCustomColorButton3_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                LandformMediator.CustomColor3 = Color.White;
                return;
            }

            LandformMediator.LandPaintColor = LandformMediator.CustomColor3;
        }

        private void LandCustomColorButton4_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                LandformMediator.CustomColor4 = Color.White;
                return;
            }

            LandformMediator.LandPaintColor = LandformMediator.CustomColor4;
        }

        private void LandCustomColorButton5_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                LandformMediator.CustomColor5 = Color.White;
                return;
            }

            LandformMediator.LandPaintColor = LandformMediator.CustomColor5;
        }

        private void LandCustomColorButton6_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                LandformMediator.CustomColor6 = Color.White;
                return;
            }

            LandformMediator.LandPaintColor = LandformMediator.CustomColor6;
        }

        private void LandSoftBrushButton_Click(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrush = ColorPaintBrush.SoftBrush;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void LandHardBrushButton_Click(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrush = ColorPaintBrush.HardBrush;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void LandPatternBrush1Button_Click(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrush = ColorPaintBrush.PatternBrush1;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush1;
        }

        private void LandPatternBrush2Button_Click(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrush = ColorPaintBrush.PatternBrush2;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush2;
        }

        private void LandPatternBrush3Button_Click(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrush = ColorPaintBrush.PatternBrush3;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush3;
        }

        private void LandPatternBrush4Button_Click(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrush = ColorPaintBrush.PatternBrush4;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush4;
        }

        private void LandColorBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMediator.LandPaintBrushSize = LandColorBrushSizeTrack.Value;
            MainMediator.SelectedBrushSize = LandformMediator.LandPaintBrushSize;
            TOOLTIP.Show(LandformMediator.LandPaintBrushSize.ToString(), LandToolPanel, new Point(LandColorBrushSizeTrack.Right - 30, LandColorBrushSizeTrack.Top - 20), 2000);
        }

        private void LandBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((LandBrushVelocityTrack.Value / 100.0F).ToString(), LandToolPanel, new Point(LandBrushVelocityTrack.Right - 30, LandBrushVelocityTrack.Top - 20), 2000);
            MainMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (LandBrushVelocityTrack.Value / 100.0));
        }

        private void LandColorEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMediator.LandPaintEraserSize = LandColorEraserSizeTrack.Value;
            MainMediator.SelectedBrushSize = LandformMediator.LandPaintEraserSize;

            TOOLTIP.Show(LandformMediator.LandPaintEraserSize.ToString(), LandToolPanel, new Point(LandColorEraserSizeTrack.Right - 20, LandColorEraserSizeTrack.Top - 20), 2000);
        }

        #endregion

        #region Landform Generation

        private void LandformGenerateButton_Click(object sender, EventArgs e)
        {
            LandformManager.GenerateRandomLandform();
            SKGLRenderControl.Invalidate();
        }

        private void RegionMenuItem_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformGenerationType = GeneratedLandformType.Continent;
        }

        private void ContinentMenuItem_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformGenerationType = GeneratedLandformType.Continent;
        }

        private void IslandMenuItem_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformGenerationType = GeneratedLandformType.Island;
        }

        private void ArchipelagoMenuItem_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformGenerationType = GeneratedLandformType.Archipelago;
        }

        private void AtollMenuItem_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformGenerationType = GeneratedLandformType.Atoll;
        }

        private void WorldMenuItem_Click(object sender, EventArgs e)
        {
            LandformMediator.LandformGenerationType = GeneratedLandformType.World;
        }

        #endregion

        #region Landform HeightMap Event Handlers

        private void HeightMapLandformSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LandformHeightMapSelect, 0);
        }

        private void HeightUpButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.MapHeightIncrease, LandformMediator.LandformBrushSize);
        }

        private void HeightDownButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.MapHeightDecrease, LandformMediator.LandformBrushSize);
        }

        private void Show3DViewButton_Click(object sender, EventArgs e)
        {
            HeightMapManager.ShowHeightMap3DModel();
        }

        #endregion

        #region Windrose Event Handlers

        private void WindrosePlaceButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MainMediator.CurrentDrawingMode == MapDrawingMode.PlaceWindrose ?
                MapDrawingMode.None : MapDrawingMode.PlaceWindrose, 0);

            if (MainMediator.CurrentDrawingMode == MapDrawingMode.PlaceWindrose)
            {
                WindroseManager.Create();
            }
            else
            {
                MapStateMediator.CurrentWindrose = null;
            }

        }

        private void WindroseColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            WindroseMediator.WindroseColor = UtilityMethods.SelectColor(this, e, WindroseColorSelectButton.BackColor);
        }

        private void WindroseClearButton_Click(object sender, EventArgs e)
        {
            WindroseManager.RemoveAllWindroses();
            SKGLRenderControl.Invalidate();
        }

        private void WindroseDirectionsUpDown_ValueChanged(object sender, EventArgs e)
        {
            WindroseMediator.DirectionCount = (int)WindroseDirectionsUpDown.Value;
        }

        private void WindroseLineWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            WindroseMediator.LineWidth = (int)WindroseLineWidthUpDown.Value;
        }

        private void WindroseInnerRadiusUpDown_ValueChanged(object sender, EventArgs e)
        {
            WindroseMediator.InnerCircleRadius = (int)WindroseInnerRadiusUpDown.Value;
        }

        private void WindroseOuterRadiusUpDown_VisibleChanged(object sender, EventArgs e)
        {
            WindroseMediator.OuterRadius = (int)WindroseOuterRadiusUpDown.Value;
        }

        private void WindroseInnerCircleTrack_ValueChanged(object sender, EventArgs e)
        {
            WindroseMediator.InnerCircleCount = (int)WindroseInnerCircleTrack.Value;
        }

        private void WindroseFadeOutSwitch_CheckedChanged()
        {
            WindroseMediator.FadeOut = WindroseFadeOutSwitch.Checked;
        }
        #endregion

        #region Water Tab Event Handlers

        private void ShowWaterLayerSwitch_CheckedChanged()
        {
            WaterFeatureMediator.ShowWaterFeatureLayers = ShowWaterLayerSwitch.Checked;
        }

        private void WaterFeatureSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.WaterFeatureSelect, 0);
        }

        private void WaterFeaturePaintButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.WaterPaint, WaterFeatureMediator.WaterFeatureBrushSize);
        }

        private void WaterFeatureLakeButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LakePaint, WaterFeatureMediator.WaterFeatureBrushSize);
        }

        private void WaterFeatureRiverButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.RiverPaint, 0);
        }

        private void WaterFeatureEraseButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.WaterErase, WaterFeatureMediator.WaterFeatureEraserSize);
        }

        private void WaterBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterFeatureBrushSize = WaterBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMediator.WaterFeatureBrushSize.ToString(), WaterValuesGroup, new Point(WaterBrushSizeTrack.Right - 30, WaterBrushSizeTrack.Top - 20), 2000);
            MainMediator.SelectedBrushSize = WaterFeatureMediator.WaterFeatureBrushSize;
        }

        private void WaterColorSelectionButton_MouseUp(object sender, MouseEventArgs e)
        {
            WaterFeatureMediator.WaterColor = UtilityMethods.SelectColor(this, e, WaterColorSelectionButton.BackColor);
        }

        private void ShorelineColorSelectionButton_MouseUp(object sender, MouseEventArgs e)
        {
            WaterFeatureMediator.ShorelineColor = UtilityMethods.SelectColor(this, e, ShorelineColorSelectionButton.BackColor);
        }

        private void RiverWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMediator.RiverWidth = RiverWidthTrack.Value;
            TOOLTIP.Show(WaterFeatureMediator.RiverWidth.ToString(), RiverValuesGroup, new Point(RiverWidthTrack.Right - 30, RiverWidthTrack.Top - 20), 2000);
        }

        private void RiverSourceFadeInSwitch_CheckedChanged()
        {
            WaterFeatureMediator.RiverSourceFadeIn = RiverSourceFadeInSwitch.Checked;
        }

        private void RiverTextureSwitch_CheckedChanged()
        {
            WaterFeatureMediator.RenderRiverTexture = RiverTextureSwitch.Checked;
        }

        private void EditRiverPointsSwitch_CheckedChanged()
        {
            WaterFeatureMediator.EditRiverPoints = EditRiverPointsSwitch.Checked;
            MainMediator.SetDrawingMode(MapDrawingMode.RiverEdit, 0);
        }

        private void WaterEraseSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterFeatureEraserSize = WaterEraserSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMediator.WaterFeatureEraserSize.ToString(), WaterEraserGroup, new Point(WaterEraserSizeTrack.Right - 30, WaterEraserSizeTrack.Top - 20), 2000);
            MainMediator.SelectedBrushSize = WaterFeatureMediator.WaterFeatureEraserSize;
        }

        private void WaterSoftBrushButton_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterPaintBrush = ColorPaintBrush.SoftBrush;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void WaterHardBrushButton_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterPaintBrush = ColorPaintBrush.HardBrush;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void WaterPatternBrush1Button_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterPaintBrush = ColorPaintBrush.PatternBrush1;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush1;
        }

        private void WaterPatternBrush2Button_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterPaintBrush = ColorPaintBrush.PatternBrush2;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush2;
        }

        private void WaterPatternBrush3Button_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterPaintBrush = ColorPaintBrush.PatternBrush3;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush3;
        }

        private void WaterPatternBrush4Button_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterPaintBrush = ColorPaintBrush.PatternBrush4;
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush4;
        }

        private void WaterColorBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterColorBrushSize = WaterColorBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMediator.WaterColorBrushSize.ToString(), WaterToolPanel, new Point(WaterColorBrushSizeTrack.Right - 30, WaterColorBrushSizeTrack.Top - 20), 2000);
            MainMediator.SelectedBrushSize = WaterFeatureMediator.WaterColorBrushSize;
        }

        private void WaterBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((WaterBrushVelocityTrack.Value / 100.0F).ToString(), WaterToolPanel, new Point(WaterBrushVelocityTrack.Right - 30, WaterBrushVelocityTrack.Top - 20), 2000);
            MainMediator.CurrentBrushVelocity = Math.Max(1, MapStateMediator.BasePaintEventInterval / (WaterBrushVelocityTrack.Value / 100.0));
        }

        private void WaterColorEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMediator.WaterColorEraserSize = WaterColorEraserSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMediator.WaterColorEraserSize.ToString(), WaterToolPanel, new Point(WaterColorEraserSizeTrack.Right - 30, WaterColorEraserSizeTrack.Top - 20), 2000);
            MainMediator.SelectedBrushSize = WaterFeatureMediator.WaterColorEraserSize;
        }

        private void WaterColorButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.WaterColor, WaterFeatureMediator.WaterColorBrushSize);
        }

        private void WaterColorEraseButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.WaterColorErase, WaterFeatureMediator.WaterColorEraserSize);
        }

        private void WaterPaintColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            WaterFeatureMediator.WaterPaintColor = UtilityMethods.SelectColor(this, e, WaterPaintColorSelectButton.BackColor);
        }

        private void WaterButton91CBB8_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.SetWaterColorFromPreset("#91CBB8");
        }

        private void WaterButton88B5BB_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.SetWaterColorFromPreset("#88B5BB");
        }

        private void WaterButton6BA5B9_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.SetWaterColorFromPreset("#6BA5B9");
        }

        private void WaterButton42718D_Click(object sender, EventArgs e)
        {
            WaterFeatureMediator.SetWaterColorFromPreset("#42718D");
        }

        private void WaterCustomColor1_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor1 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor1;
        }

        private void WaterCustomColor2_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor2 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor2;
        }

        private void WaterCustomColor3_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor3 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor3;
        }

        private void WaterCustomColor4_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor4 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor4;
        }

        private void WaterCustomColor5_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor5 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor5;
        }

        private void WaterCustomColor6_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor6 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor6;
        }

        private void WaterCustomColor7_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor7 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor7;
        }

        private void WaterCustomColor8_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                WaterFeatureMediator.CustomColor8 = Color.White;
                return;
            }

            WaterFeatureMediator.WaterPaintColor = WaterFeatureMediator.CustomColor8;
        }

        #endregion

        #region Path Tab Event Handlers

        private void ShowPathLayerSwitch_CheckedChanged()
        {
            PathMediator.ShowPathLayers = ShowPathLayerSwitch.Checked;
        }

        private void PathSelectButton_Click(object sender, EventArgs e)
        {
            PathMediator.EnableDisablePathSelection();
        }

        private void DrawPathButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
            MainMediator.SetDrawingMode(MapDrawingMode.PathPaint, 0);

            PathUIMediator.ClearPathPointSelection();
        }

        private void DrawOverSymbolsSwitch_CheckedChanged()
        {
            PathMediator.DrawOverSymbols = DrawOverSymbolsSwitch.Checked;
        }

        private void PathWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            PathMediator.PathWidth = PathWidthTrack.Value;
            TOOLTIP.Show(PathMediator.PathWidth.ToString(), MapPathValuesGroup, new Point(PathWidthTrack.Right - 30, PathWidthTrack.Top - 20), 2000);
        }

        private void PathColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            PathMediator.PathColor = UtilityMethods.SelectColor(this, e, PathColorSelectButton.BackColor);
        }

        private void EditPathPointSwitch_CheckedChanged()
        {
            Cursor = Cursors.Default;
            PathMediator.EditPathPoints = EditPathPointSwitch.Checked;
            PathMediator.SelectOrEditPaths();
        }

        private void PreviousPathTextureButton_Click(object sender, EventArgs e)
        {
            PathMediator.PathTextureIndex--;
        }

        private void NextPathTextureButton_Click(object sender, EventArgs e)
        {
            PathMediator.PathTextureIndex++;
        }

        private void PathTextureOpacityTrack_Scroll(object sender, EventArgs e)
        {
            PathMediator.PathTextureOpacity = PathTextureOpacityTrack.Value;
        }

        private void PathTextureScaleTrack_Scroll(object sender, EventArgs e)
        {
            PathMediator.PathTextureScale = PathTextureScaleTrack.Value / 100.0f;
        }

        private void PathTypeRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton pathTypeButton = (RadioButton)sender;

            if (pathTypeButton.Checked)
            {
                PathMediator.SetPathTypeFromButtonName(pathTypeButton.Name);
            }
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

        private void TowerDistanceUpDown_ValueChanged(object sender, EventArgs e)
        {
            PathMediator.PathTowerDistance = (float)TowerDistanceUpDown.Value;
        }

        private void TowerSizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            PathMediator.PathTowerSize = (float)TowerSizeUpDown.Value;
        }

        #endregion

        #region Symbol Tab Event Handlers

        private void ShowSymbolLayerSwitch_CheckedChanged()
        {
            SymbolMediator.Enabled = ShowSymbolLayerSwitch.Checked;
        }

        private void SymbolSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.SymbolSelect, 0);
        }

        private void EraseSymbolsButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.SymbolErase, AreaBrushSizeTrack.Value);
            SymbolMediator.AreaBrushSize = MainMediator.SelectedBrushSize;
        }

        private void ColorSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.ColorSymbols();
        }

        private void StructuresSymbolButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SelectSymbolsOfType(MapSymbolType.Structure);
        }

        private void VegetationSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SelectSymbolsOfType(MapSymbolType.Vegetation);
        }

        private void TerrainSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SelectSymbolsOfType(MapSymbolType.Terrain);
        }

        private void MarkerSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SelectSymbolsOfType(MapSymbolType.Marker);
        }

        private void OtherSymbolsButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SelectSymbolsOfType(MapSymbolType.Other);
        }

        private void SymbolScaleTrack_Scroll(object sender, EventArgs e)
        {
            SymbolMediator.SymbolScale = SymbolScaleTrack.Value;
            TOOLTIP.Show(SymbolMediator.SymbolScale.ToString(), SymbolScaleGroup, new Point(SymbolScaleTrack.Right - 30, SymbolScaleTrack.Top - 20), 2000);
        }

        private void SymbolScaleUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolMediator.SymbolScale = (float)SymbolScaleUpDown.Value;
        }

        private void LockSymbolScaleButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SymbolScaleLocked = !SymbolMediator.SymbolScaleLocked;
        }

        private void ResetSymbolColorsButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.ResetSymbolColorButtons();
        }

        private void SymbolColor1Button_MouseUp(object sender, MouseEventArgs e)
        {
            SymbolMediator.ColorButtonMouseUp(sender, e);
            SKGLRenderControl.Invalidate();
        }

        private void SymbolColor2Button_MouseUp(object sender, MouseEventArgs e)
        {
            SymbolMediator.ColorButtonMouseUp(sender, e);
            SKGLRenderControl.Invalidate();
        }

        private void SymbolColor3Button_MouseUp(object sender, MouseEventArgs e)
        {
            SymbolMediator.ColorButtonMouseUp(sender, e);
            SKGLRenderControl.Invalidate();
        }

        private void AreaBrushSwitch_CheckedChanged()
        {
            SymbolMediator.UseAreaBrush = AreaBrushSwitch.Checked;
            MainMediator.SelectedBrushSize = AreaBrushSizeTrack.Value;
            SKGLRenderControl.Invalidate();
        }

        private void AreaBrushSizeTrack_Scroll(object sender, EventArgs e)
        {
            MainMediator.SelectedBrushSize = AreaBrushSizeTrack.Value;
            SymbolMediator.AreaBrushSize = MainMediator.SelectedBrushSize;

            TOOLTIP.Show(SymbolMediator.AreaBrushSize.ToString(), SymbolPlacementGroup, new Point(AreaBrushSizeTrack.Right - 30, AreaBrushSizeTrack.Top - 20), 2000);

            SKGLRenderControl.Invalidate();
        }

        private void SymbolRotationTrack_Scroll(object sender, EventArgs e)
        {
            SymbolMediator.SymbolRotation = SymbolRotationTrack.Value;
            TOOLTIP.Show(SymbolRotationTrack.Value.ToString(), SymbolPlacementGroup, new Point(SymbolRotationTrack.Right - 30, SymbolRotationTrack.Top - 20), 2000);
        }

        private void SymbolRotationUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolMediator.SymbolRotation = (float)SymbolRotationUpDown.Value;
        }

        private void SymbolPlacementRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolMediator.SymbolPlacementRate = (float)SymbolPlacementRateUpDown.Value;
        }

        private void SymbolPlacementDensityUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolMediator.SymbolPlacementDensity = (float)SymbolPlacementDensityUpDown.Value;
        }

        private void ResetSymbolPlacementRateButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SymbolPlacementRate = 1.0F;
        }

        private void ResetSymbolPlacementDensityButton_Click(object sender, EventArgs e)
        {
            SymbolMediator.SymbolPlacementDensity = 1.0F;
        }

        private void SymbolCollectionsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SymbolMediator.SymbolCollectionsListItemCheck(e);
        }

        private void SymbolTagsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SymbolMediator.SymbolTagsListItemCheck(e);
        }

        private void SymbolSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            SymbolMediator.SearchSymbols(SymbolSearchTextBox.Text);
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

        private void FontColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            LabelMediator.LabelColor = UtilityMethods.SelectColor(this, e, FontColorSelectButton.BackColor);
        }

        private void OutlineColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            LabelMediator.OutlineColor = UtilityMethods.SelectColor(this, e, OutlineColorSelectButton.BackColor);
        }

        private void GlowColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            LabelMediator.GlowColor = UtilityMethods.SelectColor(this, e, GlowColorSelectButton.BackColor);
        }

        private void OutlineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            LabelMediator.OutlineWidth = OutlineWidthTrack.Value / 10.0F;
            TOOLTIP.Show(LabelMediator.OutlineWidth.ToString(), LabelOutlineGroup, new Point(OutlineWidthTrack.Right - 30, OutlineWidthTrack.Top - 20), 2000);
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
            MainMediator.SetDrawingMode(MapDrawingMode.DrawArcLabelPath, 0);
        }

        private void BezierTextPathButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawBezierLabelPath, 0);
        }

        private void LabelSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.LabelSelect, 0);
        }

        private void PlaceLabelButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawLabel, 0);
        }

        private void CreateBoxButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawBox, 0);
        }

        private void SelectBoxTintButton_MouseUp(object sender, MouseEventArgs e)
        {
            BoxMediator.BoxTint = UtilityMethods.SelectColor(this, e, SelectBoxTintButton.BackColor);
        }

        private void GenerateNameButton_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.None)
            {
                List<INameGenerator> generators = NAME_GENERATOR_CONFIG.GetSelectedNameGenerators();
                string generatedName = MapToolMethods.GenerateRandomPlaceName(generators);

                if (LabelManager.CreatingLabel)
                {
                    if (LabelManager.LabelTextBox != null && !LabelManager.LabelTextBox.IsDisposed)
                    {
                        LabelManager.LabelTextBox.Text = generatedName;
                        LabelManager.LabelTextBox.Refresh();
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
            MainMediator.OverlayLayerEnabled = ShowOverlayLayerSwitch.Checked;
        }

        #region Frame Event Handlers

        private void EnableFrameSwitch_CheckedChanged()
        {
            FrameMediator.FrameEnabled = EnableFrameSwitch.Checked;
        }

        private void FrameScaleTrack_Scroll(object sender, EventArgs e)
        {
            FrameMediator.FrameScale = FrameScaleTrack.Value / 100.0F;
            TOOLTIP.Show(FrameMediator.FrameScale.ToString(), FrameValuesGroup, new Point(FrameScaleTrack.Right - 30, FrameScaleTrack.Top - 20), 2000);
        }

        private void FrameTintColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            FrameMediator.FrameTint = UtilityMethods.SelectColor(this, e, FrameTintColorSelectButton.BackColor);
        }

        #endregion

        #region Scale Event Handlers

        private void ScaleButton_Click(object sender, EventArgs e)
        {
            ScaleMediator.ScalePanelVisible = !ScaleMediator.ScalePanelVisible;
        }

        private void CreateScaleButton_Click(object sender, EventArgs e)
        {
            MapScaleManager.Create();
            SKGLRenderControl.Invalidate();
        }

        private void DeleteScaleButton_Click(object sender, EventArgs e)
        {
            // remove any existing scale
            MapScaleManager.Delete();
            SKGLRenderControl.Invalidate();
        }

        private void ScaleWidthTrack_Scroll(object sender, EventArgs e)
        {
            ScaleMediator.ScaleWidth = ScaleWidthTrack.Value;
            TOOLTIP.Show(ScaleMediator.ScaleWidth.ToString(), MapScaleGroupBox, new Point(ScaleWidthTrack.Right - 30, ScaleWidthTrack.Top - 20), 2000);
        }

        private void ScaleHeightTrack_Scroll(object sender, EventArgs e)
        {
            ScaleMediator.ScaleHeight = ScaleHeightTrack.Value;
            TOOLTIP.Show(ScaleMediator.ScaleHeight.ToString(), MapScaleGroupBox, new Point(ScaleHeightTrack.Right - 30, ScaleHeightTrack.Top - 20), 2000);
        }

        private void ScaleSegmentCountTrack_Scroll(object sender, EventArgs e)
        {
            ScaleMediator.SegmentCount = ScaleSegmentCountTrack.Value;
            TOOLTIP.Show(ScaleMediator.SegmentCount.ToString(), MapScaleGroupBox, new Point(ScaleSegmentCountTrack.Right - 30, ScaleSegmentCountTrack.Top - 20), 2000);
        }

        private void ScaleSegmentDistanceUpDown_ValueChanged(object sender, EventArgs e)
        {
            ScaleMediator.SegmentDistance = (float)ScaleSegmentDistanceUpDown.Value;
        }

        private void ScaleLineWidthTrack_Scroll(object sender, EventArgs e)
        {
            ScaleMediator.ScaleLineWidth = ScaleLineWidthTrack.Value;
            TOOLTIP.Show(ScaleMediator.ScaleLineWidth.ToString(), MapScaleGroupBox, new Point(ScaleLineWidthTrack.Right - 30, ScaleLineWidthTrack.Top - 20), 2000);
        }

        private void ScaleUnitsTextBox_TextChanged(object sender, EventArgs e)
        {
            ScaleMediator.ScaleUnitsText = ScaleUnitsTextBox.Text;
        }

        private void ScaleNumbersNoneRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.None;
        }

        private void ScaleNumbersEndsRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.Ends;
        }

        private void ScaleNumbersEveryOtherRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.EveryOther;
        }

        private void ScaleNumbersAllRadio_CheckedChanged(object sender, EventArgs e)
        {
            ScaleMediator.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.All;
        }

        private void ScaleColorsResetButton_Click(object sender, EventArgs e)
        {
            ScaleMediator.ResetScaleColors();
        }

        private void ScaleColor1Button_MouseUp(object sender, MouseEventArgs e)
        {
            ScaleMediator.ScaleColor1 = UtilityMethods.SelectColor(this, e, ScaleColor1Button.BackColor);
        }

        private void ScaleColor2Button_MouseUp(object sender, MouseEventArgs e)
        {
            ScaleMediator.ScaleColor2 = UtilityMethods.SelectColor(this, e, ScaleColor2Button.BackColor);
        }

        private void ScaleColor3Button_MouseUp(object sender, MouseEventArgs e)
        {
            ScaleMediator.ScaleColor3 = UtilityMethods.SelectColor(this, e, ScaleColor3Button.BackColor);
        }

        private void SelectScaleFontColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            ScaleMediator.ScaleFontColor = UtilityMethods.SelectColor(this, e, SelectScaleFontColorButton.BackColor);
        }

        private void SelectScaleOutlineColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            ScaleMediator.ScaleNumberOutlineColor = UtilityMethods.SelectColor(this, e, SelectScaleOutlineColorButton.BackColor);
        }

        private void SelectScaleFontButton_Click(object sender, EventArgs e)
        {
            FontPanelManager.FontPanelOpener = FontPanelOpener.ScaleFontButton;
            FontSelectionPanel.Visible = !FontSelectionPanel.Visible;
        }

        private void ScaleOutlineWidthTrack_Scroll(object sender, EventArgs e)
        {
            ScaleMediator.ScaleOutlineWidth = ScaleOutlineWidthTrack.Value;
            TOOLTIP.Show(ScaleMediator.ScaleOutlineWidth.ToString(), ScaleOutlineGroupBox, new Point(ScaleOutlineWidthTrack.Right - 30, ScaleOutlineWidthTrack.Top - 20), 2000);
        }

        #endregion

        #region Grid Event Handlers

        private void GridButton_Click(object sender, EventArgs e)
        {
            MapGridManager.Create();
            SKGLRenderControl.Invalidate();
        }

        private void EnableGridSwitch_CheckedChanged()
        {
            GridMediator.GridEnabled = EnableGridSwitch.Checked;
        }

        private void SquareGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            GridMediator.GridType = MapGridType.Square;
        }

        private void FlatHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            GridMediator.GridType = MapGridType.FlatHex;
        }

        private void PointedHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            GridMediator.GridType = MapGridType.PointedHex;
        }

        private void GridLayerUpDown_SelectedItemChanged(object sender, EventArgs e)
        {
            GridMediator.GridLayerName = (string?)GridLayerUpDown.SelectedItem;
            MapGridManager.Create();
        }

        private void GridSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            GridMediator.GridSize = GridSizeTrack.Value;
            TOOLTIP.Show(GridSizeTrack.Value.ToString(), GridValuesGroup, new Point(GridSizeTrack.Right - 30, GridSizeTrack.Top - 20), 2000);
        }

        private void GridLineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            GridMediator.GridLineWidth = GridLineWidthTrack.Value;
            TOOLTIP.Show(GridLineWidthTrack.Value.ToString(), GridValuesGroup, new Point(GridLineWidthTrack.Right - 30, GridLineWidthTrack.Top - 20), 2000);
        }

        private void GridColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            GridMediator.GridColor = UtilityMethods.SelectColor(this, e, GridColorSelectButton.BackColor);
        }

        private void ShowGridSizeSwitch_CheckedChanged()
        {
            GridMediator.ShowGridSize = ShowGridSizeSwitch.Checked;
        }

        #endregion

        #region Measure Event Handlers

        private void MeasureButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawMapMeasure, 0);
        }

        private void SelectMeasureColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            MeasureMediator.MapMeasureColor = UtilityMethods.SelectColor(this, e, SelectMeasureColorButton.BackColor);
        }

        private void UseScaleUnitsSwitch_CheckedChanged()
        {
            MeasureMediator.UseScaleUnits = UseScaleUnitsSwitch.Checked;
        }

        private void MeasureAreaSwitch_CheckedChanged()
        {
            MeasureMediator.MeasureArea = MeasureAreaSwitch.Checked;
        }

        private void ClearMeasureButton_Click(object sender, EventArgs e)
        {
            MapMeasureManager.Delete();
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
            MainMediator.SetDrawingMode(MapDrawingMode.RegionSelect, 0);
        }

        private void CreateRegionButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.RegionPaint, 0);
            MapStateMediator.CurrentMapRegion = null;
        }

        private void RegionColorSelectButton_MouseUp(object sender, MouseEventArgs e)
        {
            RegionMediator.RegionColor = UtilityMethods.SelectColor(this, e, RegionColorSelectButton.BackColor);
        }

        private void RegionBorderWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            RegionMediator.RegionBorderWidth = RegionBorderWidthTrack.Value;
            TOOLTIP.Show(RegionMediator.RegionBorderWidth.ToString(), RegionValuesGroup, new Point(RegionBorderWidthTrack.Right - 30, RegionBorderWidthTrack.Top - 20), 2000);
        }

        private void RegionBorderSmoothingTrack_ValueChanged(object sender, EventArgs e)
        {
            RegionMediator.RegionBorderSmoothing = RegionBorderSmoothingTrack.Value;
            TOOLTIP.Show(RegionMediator.RegionBorderSmoothing.ToString(), RegionValuesGroup, new Point(RegionBorderSmoothingTrack.Right - 30, RegionBorderSmoothingTrack.Top - 20), 2000);
        }

        private void RegionOpacityTrack_ValueChanged(object sender, EventArgs e)
        {
            RegionMediator.RegionInnerOpacity = RegionOpacityTrack.Value;
            TOOLTIP.Show(RegionMediator.RegionInnerOpacity.ToString(), RegionValuesGroup, new Point(RegionOpacityTrack.Right - 30, RegionOpacityTrack.Top - 20), 2000);
        }

        private void RegionSolidBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (RegionSolidBorderRadio.Checked)
            {
                RegionMediator.RegionBorderType = PathType.SolidLinePath;
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
                RegionMediator.RegionBorderType = PathType.DottedLinePath;
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
                RegionMediator.RegionBorderType = PathType.DashedLinePath;
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
                RegionMediator.RegionBorderType = PathType.DashDotLinePath;
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
                RegionMediator.RegionBorderType = PathType.DashDotDotLinePath;
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
                RegionMediator.RegionBorderType = PathType.DoubleSolidBorderPath;
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
                RegionMediator.RegionBorderType = PathType.LineAndDashesPath;
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
                RegionMediator.RegionBorderType = PathType.BorderedGradientPath;
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
                RegionMediator.RegionBorderType = PathType.BorderedLightSolidPath;
            }
        }

        private void LightSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedLightSolidRadio.Checked = true;
        }

        #endregion

        #region Drawing Tab Event Handlers

        private void ShowDrawingLayerSwitch_CheckedChanged()
        {
            MapLayer drawingLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.DRAWINGLAYER);

            drawingLayer.ShowLayer = ShowDrawingLayerSwitch.Checked;
        }

        private void DrawingSelectButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingSelect, 0);
        }

        private void PencilDrawButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingLine, DrawingMediator.DrawingLineBrushSize);
        }

        private void PaintDrawButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingPaint, DrawingMediator.DrawingLineBrushSize);
        }

        private void PlaceRectangleButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingRectangle, DrawingMediator.DrawingLineBrushSize);
        }

        private void PlaceEllipseButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingEllipse, DrawingMediator.DrawingLineBrushSize);
        }

        private void PlacePolygonButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingPolygon, DrawingMediator.DrawingLineBrushSize);
        }

        private void PlaceStampButton_Click(object sender, EventArgs e)
        {
            if (MainMediator.CurrentDrawingMode != MapDrawingMode.DrawingStamp)
            {
                MainMediator.SetDrawingMode(MapDrawingMode.DrawingStamp, 0);
            }
            else
            {
                MainMediator.SetDrawingMode(MapDrawingMode.None, 0);
            }
        }

        private void DrawingFillButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingFill, 0);
        }

        private void EraseDrawingButton_Click(object sender, EventArgs e)
        {
            MainMediator.SetDrawingMode(MapDrawingMode.DrawingErase, DrawingMediator.DrawingLineBrushSize);
        }

        private void LineBrushSizeTrack_Scroll(object sender, EventArgs e)
        {
            DrawingMediator.DrawingLineBrushSize = LineBrushSizeTrack.Value;
            MainMediator.SelectedBrushSize = DrawingMediator.DrawingLineBrushSize;

            TOOLTIP.Show(DrawingMediator.DrawingLineBrushSize.ToString(), DrawingTab, new Point(LineBrushSizeTrack.Right - 30, LineBrushSizeTrack.Top - 20), 2000);
        }

        private void FillShapeButton_Click(object sender, EventArgs e)
        {
            DrawingMediator.FillDrawnShape = FillShapeButton.Checked;
        }

        private void DrawingSoftBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.SoftBrush;
            DrawingMediator.DrawingPaintBrush = ColorPaintBrush.SoftBrush;
        }

        private void DrawingHardBrushButton_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.HardBrush;
            DrawingMediator.DrawingPaintBrush = ColorPaintBrush.HardBrush;
        }

        private void DrawingPatternBrush1Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush1;
            DrawingMediator.DrawingPaintBrush = ColorPaintBrush.PatternBrush1;
        }

        private void DrawingPatternBrush2Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush2;
            DrawingMediator.DrawingPaintBrush = ColorPaintBrush.PatternBrush2;
        }

        private void DrawingPatternBrush3Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush3;
            DrawingMediator.DrawingPaintBrush = ColorPaintBrush.PatternBrush3;
        }

        private void DrawingPatternBrush4Button_Click(object sender, EventArgs e)
        {
            MapStateMediator.SelectedColorPaintBrush = ColorPaintBrush.PatternBrush4;
            DrawingMediator.DrawingPaintBrush = ColorPaintBrush.PatternBrush4;
        }

        private void SelectPaintColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            DrawingMediator.DrawingLineColor = UtilityMethods.SelectColor(this, e, SelectPaintColorButton.BackColor);
        }

        private void SelectFillColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            DrawingMediator.DrawingFillColor = UtilityMethods.SelectColor(this, e, SelectFillColorButton.BackColor);
        }

        private void PreviousDrawingFillTextureButton_Click(object sender, EventArgs e)
        {
            DrawingMediator.DrawingTextureIndex--;
        }

        private void NextDrawingFillTextureButton_Click(object sender, EventArgs e)
        {
            DrawingMediator.DrawingTextureIndex++;
        }

        private void ColorMenuItem_Click(object sender, EventArgs e)
        {
            ColorMenuItem.Checked = !ColorMenuItem.Checked;

            if (ColorMenuItem.Checked)
            {
                TextureMenuItem.Checked = false;
                DrawingMediator.FillType = DrawingFillType.Color;
            }
        }

        private void TextureMenuItem_Click(object sender, EventArgs e)
        {
            TextureMenuItem.Checked = !TextureMenuItem.Checked;

            if (TextureMenuItem.Checked)
            {
                ColorMenuItem.Checked = false;
                DrawingMediator.FillType = DrawingFillType.Texture;
            }
        }


        private void SelectStampButton_Click(object sender, EventArgs e)
        {
            try
            {
                string defaultStampDir = AssetManager.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "Stamps" + Path.DirectorySeparatorChar;

                OpenFileDialog ofd = new()
                {
                    Title = "Open Image to Stamp",
                    DefaultExt = "png",
                    InitialDirectory = defaultStampDir,
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    ShowHelp = false,           // enabling the help button causes the dialog not to display files
                    Multiselect = false,
                    Filter = UtilityMethods.GetCommonImageFilter()
                };

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    if (ofd.FileName != "")
                    {
                        try
                        {
                            Bitmap b = (Bitmap)Bitmap.FromFile(ofd.FileName);

                            if (b.Height > 0 && b.Width > 0)
                            {
                                DrawingMediator.DrawingStampBitmap = (Bitmap?)b.Clone();
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

        private void DrawingStampScaleTrack_Scroll(object sender, EventArgs e)
        {
            DrawingMediator.DrawingStampScale = DrawingStampScaleTrack.Value / 100.0F;
        }

        private void DrawingStampRotationTrack_Scroll(object sender, EventArgs e)
        {
            DrawingMediator.DrawingStampRotation = DrawingStampRotationTrack.Value;
        }

        private void DrawingStampOpacityTrack_Scroll(object sender, EventArgs e)
        {
            DrawingMediator.DrawingStampOpacity = DrawingStampOpacityTrack.Value / 100.0F;
        }

        private void LayerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
