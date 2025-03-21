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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using Control = System.Windows.Forms.Control;
using Image = System.Drawing.Image;
using TextBox = System.Windows.Forms.TextBox;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private readonly string RELEASE_STATE = "Pre-Release";

        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static MapDrawingMode CURRENT_DRAWING_MODE = MapDrawingMode.None;

        private static RealmStudioMap CURRENT_MAP = new();

        // the objects currently being drawn, before being added to map layers
        private static Landform? CURRENT_LANDFORM;
        private static MapWindrose? CURRENT_WINDROSE;
        private static WaterFeature? CURRENT_WATERFEATURE;
        private static River? CURRENT_RIVER;
        private static MapPath? CURRENT_MAP_PATH;
        private static MapGrid? CURRENT_MAP_GRID;
        private static MapMeasure? CURRENT_MAP_MEASURE;
        private static MapRegion? CURRENT_MAP_REGION;
        private static LayerPaintStroke? CURRENT_LAYER_PAINT_STROKE;
        private static LayerPaintStroke? CURRENT_LAND_ERASE_STROKE;

        // objects that are currently selected
        private static Landform? SELECTED_LANDFORM;
        private static MapPath? SELECTED_PATH;
        private static MapPathPoint? SELECTED_PATHPOINT;
        private static MapSymbol? SELECTED_MAP_SYMBOL;
        private static MapBox? SELECTED_MAP_BOX;
        private static PlacedMapBox? SELECTED_PLACED_MAP_BOX;
        private static MapLabel? SELECTED_MAP_LABEL;
        private static MapScale? SELECTED_MAP_SCALE;
        private static IWaterFeature? SELECTED_WATERFEATURE;
        private static MapRiverPoint? SELECTED_RIVERPOINT;
        private static ColorPaintBrush SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.SoftBrush;
        private static GeneratedLandformType SELECTED_LANDFORM_TYPE = GeneratedLandformType.NotSet;
        private static SKRect SELECTED_REALM_AREA = SKRect.Empty;
        private static SKRect PREVIOUS_SELECTED_AREA = SKRect.Empty;

        private static readonly List<MapComponent> SELECTED_MAP_COMPONENTS = [];

        private static readonly PrivateFontCollection EMBEDDED_FONTS = new();

        private static Font SELECTED_LABEL_FONT = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private readonly static Font DEFAULT_LABEL_FONT = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private static Font SELECTED_MAP_SCALE_FONT = new("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private static readonly FontSelection FONT_PANEL_SELECTED_FONT = new();
        private static FontPanelOpener FONT_PANEL_OPENER = FontPanelOpener.NotSet;

        private static int SELECTED_BRUSH_SIZE;

        private static float PLACEMENT_RATE = 1.0F;
        private static float PLACEMENT_DENSITY = 1.0F;
        private static float DrawingZoom = 1.0f;

        private static readonly double BASE_MILLIS_PER_PAINT_EVENT = 10.0;
        private static double BRUSH_VELOCITY = 2.0;

        private static System.Timers.Timer? BRUSH_TIMER;
        private static System.Timers.Timer? AUTOSAVE_TIMER;
        private static System.Timers.Timer? SYMBOL_AREA_BRUSH_TIMER;

        private static System.Timers.Timer? LOCATION_UPDATE_TIMER;

        private static readonly int BACKUP_COUNT = 5;

        private static SKPath CURRENT_MAP_LABEL_PATH = new();
        private static readonly List<SKPoint> CURRENT_MAP_LABEL_PATH_POINTS = [];

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);

        private static Point PREVIOUS_MOUSE_LOCATION = new(0, 0);

        private static SKPoint MOUSE_LOCATION = new(0, 0);
        private static SKPoint CURSOR_POINT = new(0, 0);
        private static SKPoint PREVIOUS_CURSOR_POINT = new(0, 0);

        private static readonly System.Windows.Forms.ToolTip TOOLTIP = new();

        private TextBox? LABEL_TEXT_BOX;

        public static readonly NameGeneratorConfiguration NAME_GENERATOR_CONFIG = new();

        private static bool SYMBOL_SCALE_LOCKED;
        private static bool CREATING_LABEL;

        private readonly AppSplashScreen SPLASH_SCREEN = new();

        private static string MapCommandLinePath = string.Empty;

        private static float SELECTED_PATH_ANGLE = -1;

        public ThreeDView? CurrentHeightMapView { get; set; }

        #region Constructor
        /******************************************************************************************************* 
        * MAIN FORM CONSTRUCTOR
        *******************************************************************************************************/
        public RealmStudioMainForm(string[] args)
        {
            AssetManager.InitializeAssetDirectory();

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

            MapBuilder.DisposeMap(CURRENT_MAP);

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
                // this creates the CURRENT_MAP
                DialogResult result = OpenRealmConfigurationDialog();

                if (result != DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            SetDrawingModeLabel();
            PopulateControlsWithAssets(assetCount);
            PopulateFontPanelUI();
            LoadNameGeneratorConfigurationDialog();

            if (AssetManager.CURRENT_THEME != null)
            {
                ThemeFilter themeFilter = new();
                ApplyTheme(AssetManager.CURRENT_THEME, themeFilter);
            }

            Activate();

            StartAutosaveTimer();

            StartLocationUpdateTimer();

            SKGLRenderControl.Invalidate();

            Cursor = Cursors.Default;

        }

        private void AutosaveTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (AutosaveSwitch.Checked)
                {
                    PruneOldBackupsOfMap();

                    if (!CURRENT_MAP.IsSaved)
                    {
                        if (!string.IsNullOrWhiteSpace(CURRENT_MAP.MapPath))
                        {
                            MapFileMethods.SaveMap(CURRENT_MAP);
                        }

                        SaveRealmBackup(CURRENT_MAP, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
            }
        }

        private static void SaveRealmFileBackup(string filepath)
        {
            string autosaveDirectory = Settings.Default.AutosaveDirectory;

            if (string.IsNullOrEmpty(autosaveDirectory))
            {
                autosaveDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
            }

            string fileNameNoExtension = Path.GetFileNameWithoutExtension(filepath);
            string saveTime = DateTime.Now.ToFileTimeUtc().ToString();

            string saveFilename = fileNameNoExtension + "_" + saveTime + ".rsmapx";
            string autosaveFullPath = autosaveDirectory + Path.DirectorySeparatorChar + saveFilename;

            File.Copy(filepath, autosaveFullPath, true);
        }

        private void SaveRealmBackup(RealmStudioMap map, bool useMapNameForBackup = false)
        {
            string currentmapFileName = map.MapPath;

            try
            {
                // realm autosave folder (location where map backups are saved during autosave)
                string autosaveDirectory = Settings.Default.AutosaveDirectory;

                if (string.IsNullOrEmpty(autosaveDirectory))
                {
                    autosaveDirectory = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;
                }

                string autosaveFilename = map.MapGuid.ToString();

                if (useMapNameForBackup)
                {
                    autosaveFilename = map.MapName;
                }

                string saveTime = DateTime.Now.ToFileTimeUtc().ToString();

                autosaveFilename += "_" + saveTime + ".rsmapx";

                string autosaveFullPath = autosaveDirectory + Path.DirectorySeparatorChar + autosaveFilename;

                map.MapPath = autosaveFullPath;

                MapFileMethods.SaveMap(map);

                if (Settings.Default.PlaySoundOnSave)
                {
                    UtilityMethods.PlaySaveSound();
                    SetStatusText("A backup of the realm has been saved.");
                }
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
                MessageBox.Show("An error has occurred while saving a backup copy of the map.", "Map Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            finally
            {
                map.MapPath = currentmapFileName;
            }
        }

        private void RealmStudioMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // stop timers
            StopBrushTimer();
            StopAutosaveTimer();
            StopLocationUpdateTimer();

            // save user preferences
            Settings.Default.Save();

            // save symbol tags and collections
            SymbolMethods.SaveSymbolTags();
            SymbolMethods.SaveCollections();

            // close the name generator dialog
            NAME_GENERATOR_CONFIG.Close();

            // save the map
            if (!CURRENT_MAP.IsSaved)
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
                        MapBuilder.DisposeMap(CURRENT_MAP);
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
                MapBuilder.DisposeMap(CURRENT_MAP);
            }
        }

        private void AutosaveSwitch_CheckedChanged()
        {
            Settings.Default.RealmAutosave = AutosaveSwitch.Checked;
            Settings.Default.Save();

            if (AutosaveSwitch.Checked)
            {
                StartAutosaveTimer();
            }
            else
            {
                StopAutosaveTimer();
            }
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
            DrawingZoom = 1.0F;
            ZoomLevelTrack.Value = 10;

            ScrollPoint.X = 0;
            ScrollPoint.Y = 0;
            DrawingPoint.X = 0;
            DrawingPoint.Y = 0;

            MapRenderHScroll.Value = 0;
            MapRenderVScroll.Value = 0;

            SKGLRenderControl.Invalidate();
        }

        private void ZoomToFitButton_Click(object sender, EventArgs e)
        {
            float horizontalAspect = (float)SKGLRenderControl.Width / CURRENT_MAP.MapWidth;
            float verticalAspect = (float)SKGLRenderControl.Height / CURRENT_MAP.MapHeight;

            DrawingZoom = Math.Min(horizontalAspect, verticalAspect);
            ZoomLevelTrack.Value = Math.Max(1, (int)DrawingZoom);

            ScrollPoint.X = 0;
            ScrollPoint.Y = 0;
            DrawingPoint.X = 0;
            DrawingPoint.Y = 0;

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
            CURRENT_DRAWING_MODE = MapDrawingMode.RealmAreaSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
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
            CURRENT_DRAWING_MODE = MapDrawingMode.ColorSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void SelectColorButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Select color from map", RealmStudioForm, new Point(SelectColorButton.Left, SelectColorButton.Top + 30), 3000);
        }

        private void ZoomLevelTrack_Scroll(object sender, EventArgs e)
        {
            DrawingZoom = ZoomLevelTrack.Value / 10.0F;
            SKGLRenderControl.Invalidate();
        }

        void NameGenerator_NameGenerated(object? sender, EventArgs e)
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

        #region Main Form Methods

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
                    StopAutosaveTimer();
                    StopBrushTimer();
                    StopLocationUpdateTimer();
                    StopSymbolAreaBrushTimer();

                    SKSurface s = SKSurface.Create(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
                    s.Canvas.Clear();

                    MapRenderMethods.RenderMapForExport(CURRENT_MAP, s.Canvas);

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

                    StartAutosaveTimer();
                    StartLocationUpdateTimer();
                    MessageBox.Show("Map exported to " + ofd.FileName, "Map Exported", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("Failed to export map to " + ofd.FileName, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
                    StopAutosaveTimer();
                    StopBrushTimer();
                    StopLocationUpdateTimer();
                    StopSymbolAreaBrushTimer();

                    using (FileStream fileStream = new(filename, FileMode.OpenOrCreate))
                    {
                        RealmMapMethods.ExportMapLayersAsZipFile(fileStream, CURRENT_MAP, exportFormat);
                    }

                    StartAutosaveTimer();
                    StartLocationUpdateTimer();
                    MessageBox.Show("Map layers exported to " + ofd.FileName, "Map Exported", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                }
                catch (Exception ex)
                {
                    Program.LOGGER.Error(ex);
                    MessageBox.Show("Failed to export map layers to " + ofd.FileName, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
                    StopAutosaveTimer();
                    StopBrushTimer();
                    StopLocationUpdateTimer();
                    StopSymbolAreaBrushTimer();

                    SKSurface s = SKSurface.Create(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
                    s.Canvas.Clear(SKColors.Black);

                    // render the map as a height map
                    RealmMapMethods.RenderHeightMapToCanvas(CURRENT_MAP, s.Canvas, ScrollPoint);

                    Bitmap bitmap = s.Snapshot().ToBitmap();

                    bitmap.Save(filename);

                    StartAutosaveTimer();
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

        private static void ExportHeightMap3DModel()
        {
            SKSurface s = SKSurface.Create(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
            s.Canvas.Clear(SKColors.Black);

            // render the map as a height map
            RealmMapMethods.RenderHeightMapToCanvas(CURRENT_MAP, s.Canvas, ScrollPoint);

            SKBitmap heightMap = SKBitmap.FromImage(s.Snapshot());

            Cursor.Current = Cursors.WaitCursor;

            List<string> objModelString = HeightMapTo3DModel.GenerateOBJ(heightMap, Byte.MaxValue / 2.0F);

            Cursor.Current = Cursors.Default;

            HeightMapTo3DModel.WriteObjModelToFile(objModelString);
        }

        private static void AddMapImagesToHeightMapLayer()
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
            MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.HEIGHTMAPLAYER);

            if (heightMapLayer.MapLayerComponents.Count == 2)
            {
                // heightmap images have already been created
                return;
            }

            using SKBitmap b = new(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
            using SKCanvas canvas = new(b);

            canvas.Clear(SKColors.Black);

            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                if (landformLayer.MapLayerComponents[i] is Landform l)
                {
                    l.RenderLandformForHeightMap(canvas);
                }
            }

            heightMapLayer.MapLayerComponents.Clear();

            MapImage landformImage = new()
            {
                MapImageBitmap = b.Copy()
            };

            heightMapLayer.MapLayerComponents.Add(landformImage);

            using SKBitmap b2 = new(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
            b2.Erase(SKColors.Transparent);

            MapHeightMap heightMap = new()
            {
                HeightMap = new float[CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight],
                MapImageBitmap = b2.Copy(),
            };

            heightMapLayer.MapLayerComponents.Add(heightMap);
        }

        private static void SelectMapComponentsInArea(RealmStudioMap map, SKRect selectedArea)
        {
            SELECTED_MAP_COMPONENTS.Clear();

            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER);
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHUPPERLAYER);
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.WATERLAYER);

            for (int i = symbolLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (symbolLayer.MapLayerComponents[i] is MapSymbol ms)
                {
                    if (selectedArea.Contains(ms.X, ms.Y))
                    {
                        SELECTED_MAP_COMPONENTS.Add(ms);
                    }
                }
            }

            for (int i = pathLowerLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (pathLowerLayer.MapLayerComponents[i] is MapPath mp)
                {
                    mp.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(mp.PathPoints);
                    SKRect pathBounds = mp.BoundaryPath.ComputeTightBounds();

                    if (selectedArea.Contains(pathBounds))
                    {
                        SELECTED_MAP_COMPONENTS.Add(mp);
                    }
                }
            }

            for (int i = pathUpperLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (pathUpperLayer.MapLayerComponents[i] is MapPath mp)
                {
                    mp.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(mp.PathPoints);
                    SKRect pathBounds = mp.BoundaryPath.ComputeTightBounds();

                    if (selectedArea.Contains(pathBounds))
                    {
                        SELECTED_MAP_COMPONENTS.Add(mp);
                    }
                }
            }

            for (int i = waterLayer.MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (waterLayer.MapLayerComponents[i] is IWaterFeature iwf)
                {
                    if (iwf is WaterFeature wf)
                    {
                        SKRect pathBounds = wf.ContourPath.ComputeTightBounds();

                        if (selectedArea.Contains(pathBounds))
                        {
                            SELECTED_MAP_COMPONENTS.Add(wf);
                        }
                    }
                    else if (iwf is River mr)
                    {
                        if (mr.RiverBoundaryPath != null && mr.RiverBoundaryPath.PointCount > 0)
                        {
                            SKRect pathBounds = mr.RiverBoundaryPath.ComputeTightBounds();

                            if (selectedArea.Contains(pathBounds))
                            {
                                SELECTED_MAP_COMPONENTS.Add(mr);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Main Menu Event Handlers

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CURRENT_MAP.IsSaved)
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
            if (!CURRENT_MAP.IsSaved)
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
            if (CURRENT_MAP.IsSaved)
            {
                return;
            }

            if (!string.IsNullOrEmpty(CURRENT_MAP.MapPath))
            {
                try
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();

                    // serialize the map to disk as an XML file
                    MapFileMethods.SaveMap(CURRENT_MAP);
                    CURRENT_MAP.IsSaved = true;

                    if (Settings.Default.PlaySoundOnSave)
                    {
                        UtilityMethods.PlaySaveSound();
                        SetStatusText("Realm " + Path.GetFileNameWithoutExtension(CURRENT_MAP.MapPath) + " has been saved.");
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
                    ExportHeightMap3DModel();
                    break;
            }
        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreview printPreview = new(CURRENT_MAP);
            printPreview.ShowDialog();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CURRENT_MAP.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Exit Application", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult == DialogResult.OK)
                    {
                        Close();
                        MapBuilder.DisposeMap(CURRENT_MAP);
                        Application.Exit();
                    }
                }
                else if (result == DialogResult.No)
                {
                    CURRENT_MAP.IsSaved = true;
                    Close();
                    MapBuilder.DisposeMap(CURRENT_MAP);
                    Application.Exit();
                }
            }
            else
            {
                Close();
                MapBuilder.DisposeMap(CURRENT_MAP);
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
            if (SELECTED_REALM_AREA != SKRect.Empty)
            {
                // get all objects within the selected area and cut them
                // create a command class for this so it can be undone

                // does it make sense for this to work for anything other than symbols?
                // labels? boxes? water features? landforms? paths? regions?

                // the object that is cut/copied must lie completely within the selected area
                // if it is not a "point" object (e.g. a symbol)

                SelectMapComponentsInArea(CURRENT_MAP, SELECTED_REALM_AREA);

                Cmd_CutOrCopyFromArea cmd = new(CURRENT_MAP, SELECTED_MAP_COMPONENTS, SELECTED_REALM_AREA, true);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                PREVIOUS_SELECTED_AREA = SELECTED_REALM_AREA;
                SELECTED_REALM_AREA = SKRect.Empty;
                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                SKGLRenderControl.Invalidate();
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SELECTED_REALM_AREA != SKRect.Empty)
            {
                // get all objects within the selected area and copy them
                // create a command class for this so it can be undone
                SelectMapComponentsInArea(CURRENT_MAP, SELECTED_REALM_AREA);

                // do not cut the selected objects from their layer
                Cmd_CutOrCopyFromArea cmd = new(CURRENT_MAP, SELECTED_MAP_COMPONENTS, SELECTED_REALM_AREA, false);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                PREVIOUS_SELECTED_AREA = SELECTED_REALM_AREA;
                SELECTED_REALM_AREA = SKRect.Empty;
                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                SKGLRenderControl.Invalidate();
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if the list of copied items is not empty
            // then paste them at the cursor point, updating their position

            if (SELECTED_MAP_COMPONENTS.Count > 0)
            {
                Point cursorPosition = SKGLRenderControl.PointToClient(Cursor.Position);

                SKPoint zoomedScrolledPoint = new((cursorPosition.X / DrawingZoom) + DrawingPoint.X, (cursorPosition.Y / DrawingZoom) + DrawingPoint.Y);
                Cmd_PasteSelectedComponents cmd = new(CURRENT_MAP, SELECTED_MAP_COMPONENTS, PREVIOUS_SELECTED_AREA, zoomedScrolledPoint);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void ClearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SELECTED_MAP_COMPONENTS.Clear();
            PREVIOUS_SELECTED_AREA = SKRect.Empty;
            SELECTED_REALM_AREA = SKRect.Empty;
            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
        }

        private void RenderAsHeightMapMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (RenderAsHeightMapMenuItem.Checked)
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.HeightMapPaint;
                SetDrawingModeLabel();
                SetSelectedBrushSize(0);

                // if needed, render landforms to height map layer as a map image
                // and add a mapheightmap to the height map layer to hold the height map;
                // as the user paints on the map, the height map image will be updated

                AddMapImagesToHeightMapLayer();

                // force maintab selected index change
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.SelectedIndex = 0;
                MainTab.SelectedIndex = 2;  // select landform tab
                MainTab.Refresh();
            }
            else
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.None;
                SetDrawingModeLabel();
                SetSelectedBrushSize(0);

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
            RealmProperties propertiesDialog = new(CURRENT_MAP);
            DialogResult result = propertiesDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                CURRENT_MAP.MapName = propertiesDialog.NameTextbox.Text;
                UpdateMapNameAndSize();
            }
        }

        private void ChangeMapSizeMenuItem_Click(object sender, EventArgs e)
        {
            if (!CURRENT_MAP.IsSaved)
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

            RealmStudioMap? resizedMap = RealmMapMethods.CreateResizedMap(this, CURRENT_MAP);

            if (resizedMap != null)
            {
                MapBuilder.DisposeMap(CURRENT_MAP);
                CURRENT_MAP = resizedMap;

                MAP_WIDTH = CURRENT_MAP.MapWidth;
                MAP_HEIGHT = CURRENT_MAP.MapHeight;

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
            if (!CURRENT_MAP.IsSaved)
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

            RealmStudioMap? detailMap = RealmMapMethods.CreateDetailMap(this, CURRENT_MAP, SELECTED_REALM_AREA);

            if (detailMap != null)
            {
                MapBuilder.DisposeMap(CURRENT_MAP);
                CURRENT_MAP = detailMap;

                MAP_WIDTH = CURRENT_MAP.MapWidth;
                MAP_HEIGHT = CURRENT_MAP.MapHeight;

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
                                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

                                if (SELECTED_REALM_AREA != SKRect.Empty)
                                {
                                    landformPath.Transform(SKMatrix.CreateScale(SELECTED_REALM_AREA.Width / b.Width, SELECTED_REALM_AREA.Height / b.Height));
                                }
                                else
                                {
                                    // TODO: scale the landform path to fit the map, maintaining the aspect ratio of the original image
                                    //landformPath.Transform(SKMatrix.CreateScale((float)CURRENT_MAP.MapWidth / b.Width, (float)CURRENT_MAP.MapHeight / b.Height));
                                }

                                Landform landform = new()
                                {
                                    ParentMap = CURRENT_MAP,
                                    IsModified = true,
                                    DrawPath = new(landformPath),
                                    LandformRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                        new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight)),
                                    CoastlineRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                        new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                                };

                                if (SELECTED_REALM_AREA != SKRect.Empty)
                                {
                                    landform.X = (int)SELECTED_REALM_AREA.Left;
                                    landform.Y = (int)SELECTED_REALM_AREA.Top;
                                }
                                else
                                {
                                    landform.X = 0;
                                    landform.Y = 0;
                                }

                                LandformMethods.CreateAllPathsFromDrawnPath(CURRENT_MAP, landform);

                                landform.ContourPath.GetBounds(out SKRect boundsRect);

                                landform.Width = (int)boundsRect.Width;
                                landform.Height = (int)boundsRect.Height;

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

            if (!Settings.Default.RealmAutosave)
            {
                AutosaveSwitch.Checked = false;
                StopAutosaveTimer();
            }
            else
            {
                AutosaveSwitch.Checked = true;
                StartAutosaveTimer();
            }
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

        private void CreateNewMap()
        {
            MapBuilder.DisposeMap(CURRENT_MAP);

            // this creates the CURRENT_MAP
            DialogResult newResult = OpenRealmConfigurationDialog();

            if (newResult == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;

                AssetManager.LOADING_STATUS_FORM.ResetLoadingProgress();
                AssetManager.LOADING_STATUS_FORM.Show(this);

                SetDrawingModeLabel();

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

        private void StartAutosaveTimer()
        {
            // stop the autosave timer
            StopAutosaveTimer();

            bool autosave = Settings.Default.RealmAutosave && AutosaveSwitch.Checked;

            if (autosave)
            {
                int saveIntervalMinutes = Settings.Default.AutosaveInterval;

                // save interval cannot be less than 5 minutes (300000 milliseconds)
                int saveIntervalMillis = Math.Max(5 * 60 * 1000, saveIntervalMinutes * 60 * 1000);

                // start the autosave timer
                AUTOSAVE_TIMER = new System.Timers.Timer
                {
                    Interval = saveIntervalMillis,
                    AutoReset = true,
                    SynchronizingObject = this,
                };

                AUTOSAVE_TIMER.Elapsed += new ElapsedEventHandler(AutosaveTimerEventHandler);
                AUTOSAVE_TIMER.Start();
            }
        }

        private void StartBrushTimer(int brushIntervalMillis)
        {
            // stop the brush timer if it is running
            StopBrushTimer();

            // start the brush timer
            BRUSH_TIMER = new System.Timers.Timer
            {
                Interval = brushIntervalMillis,
                AutoReset = true,
                SynchronizingObject = SKGLRenderControl,
            };

            BRUSH_TIMER.Elapsed += new ElapsedEventHandler(BrushTimerEventHandler);
            BRUSH_TIMER.Start();
        }

        private void StartSymbolAreaBrushTimer(int brushIntervalMillis)
        {
            // stop the symbols area brush timer if it is running
            StopSymbolAreaBrushTimer();

            // start the brush timer
            SYMBOL_AREA_BRUSH_TIMER = new System.Timers.Timer
            {
                Interval = brushIntervalMillis,
                AutoReset = true,
                SynchronizingObject = SKGLRenderControl,
            };

            SYMBOL_AREA_BRUSH_TIMER.Elapsed += new ElapsedEventHandler(SymbolAreaBrushTimerEventHandler);
            SYMBOL_AREA_BRUSH_TIMER.Start();
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

        private static void StopBrushTimer()
        {
            BRUSH_TIMER?.Stop();
            BRUSH_TIMER?.Dispose();
            BRUSH_TIMER = null;
        }

        private static void StopAutosaveTimer()
        {
            AUTOSAVE_TIMER?.Stop();
            AUTOSAVE_TIMER?.Dispose();
            AUTOSAVE_TIMER = null;
        }

        private static void StopSymbolAreaBrushTimer()
        {
            SYMBOL_AREA_BRUSH_TIMER?.Stop();
            SYMBOL_AREA_BRUSH_TIMER?.Dispose();
            SYMBOL_AREA_BRUSH_TIMER = null;
        }

        private static void StopLocationUpdateTimer()
        {
            LOCATION_UPDATE_TIMER?.Stop();
            LOCATION_UPDATE_TIMER?.Dispose();
            LOCATION_UPDATE_TIMER = null;
        }

        private static void PruneOldBackupsOfMap()
        {
            // realm autosave folder (location where map backups are saved during autosave)
            string defaultAutosaveFolder = UtilityMethods.DEFAULT_AUTOSAVE_FOLDER;

            string autosaveDirectory = Settings.Default.AutosaveDirectory;

            if (string.IsNullOrEmpty(autosaveDirectory))
            {
                autosaveDirectory = defaultAutosaveFolder;
            }

            string autosaveFilename = CURRENT_MAP.MapGuid.ToString();

            string oldestFilePath = string.Empty;
            DateTime? oldestCreationDateTime = null;

            var files = from file in Directory.EnumerateFiles(autosaveDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(autosaveFilename)
                        select new
                        {
                            File = file
                        };

            // keep 5 backups of the map
            if (files.Count() >= BACKUP_COUNT)
            {
                foreach (var f in files)
                {
                    DateTime creationDateTime = File.GetCreationTimeUtc(f.File);
                    string path = Path.GetFullPath(f.File);

                    if (string.IsNullOrEmpty(oldestFilePath) || creationDateTime < oldestCreationDateTime)
                    {
                        oldestFilePath = path;
                        oldestCreationDateTime = creationDateTime;
                    }
                }

                if (!string.IsNullOrEmpty(oldestFilePath)
                    && File.Exists(oldestFilePath)
                    && oldestFilePath.Contains("autosave")
                    && oldestFilePath.StartsWith(autosaveDirectory)
                    && oldestFilePath.EndsWith(".rsmapx"))
                {
                    try
                    {
                        File.Delete(oldestFilePath);
                    }
                    catch (Exception ex)
                    {
                        Program.LOGGER.Error(ex);
                        throw new Exception("Prune backup failed");
                    }
                }
            }
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
                CURRENT_MAP = MapBuilder.CreateMap(rcd.Map, SKGLRenderControl.GRContext);

                MAP_WIDTH = CURRENT_MAP.MapWidth;
                MAP_HEIGHT = CURRENT_MAP.MapHeight;

                MapRenderHScroll.Maximum = MAP_WIDTH;
                MapRenderVScroll.Maximum = MAP_HEIGHT;

                MapRenderHScroll.Value = 0;
                MapRenderVScroll.Value = 0;

                if (string.IsNullOrEmpty(CURRENT_MAP.MapName))
                {
                    CURRENT_MAP.MapName = "Default";
                }

                UpdateMapNameAndSize();

                if (CURRENT_MAP.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                {
                    MessageBox.Show("Application startup error. RealmStudio will close.");
                    Application.Exit();
                }

                // create the map vignette and add it to the vignette layer
                MapVignette vignette = new()
                {
                    ParentMap = CURRENT_MAP,
                    VignetteColor = VignetteColorSelectionButton.BackColor.ToArgb(),
                    VignetteStrength = VignetteStrengthTrack.Value,
                    RectangleVignette = RectangleVignetteRadio.Checked,
                    VignetteRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                        new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight)),
                };

                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Add(vignette);
            }

            return result;
        }


        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear the drawing mode (and uncheck all drawing, paint, and erase buttons) when switching tabs
            CURRENT_DRAWING_MODE = MapDrawingMode.None;
            SetDrawingModeLabel();

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
                        CURRENT_DRAWING_MODE = MapDrawingMode.HeightMapPaint;
                        SetDrawingModeLabel();
                        SetSelectedBrushSize(0);

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

        private static void SetZoomLevel(int upDown)
        {
            // TODO: scrollbars need to be updated
            // increase/decrease zoom by 10%, limiting to no less than 10% and no greater than 800%
            DrawingZoom = (upDown < 0) ? Math.Max(0.1f, DrawingZoom - 0.1f) : Math.Min(8.0f, DrawingZoom + 0.1f);
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

            LabelPresetsListBox.DisplayMember = "LabelPresetName";
            LabelPresetsListBox.Items.Clear();

            foreach (LabelPreset preset in AssetManager.LABEL_PRESETS)
            {
                if (!string.IsNullOrEmpty(preset.LabelPresetTheme)
                    && AssetManager.CURRENT_THEME != null
                    && preset.LabelPresetTheme == AssetManager.CURRENT_THEME.ThemeName)
                {
                    LabelPresetsListBox.Items.Add(preset);
                }
            }

            if (LabelPresetsListBox.Items.Count > 0)
            {
                LabelPresetsListBox.SelectedIndex = 0;

                LabelPreset? selectedPreset = AssetManager.LABEL_PRESETS.Find(x => !string.IsNullOrEmpty(((LabelPreset?)LabelPresetsListBox.Items[0])?.LabelPresetName)
                    && x.LabelPresetName == ((LabelPreset?)LabelPresetsListBox.Items[0])?.LabelPresetName);

                if (selectedPreset != null)
                {
                    SetLabelValuesFromPreset(selectedPreset);
                }
            }

            LabelPresetsListBox.Refresh();

            AddMapBoxesToLabelBoxTable(AssetManager.MAP_BOX_LIST);

            AddMapFramesToFrameTable(AssetManager.MAP_FRAME_TEXTURES);

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

        public void SetStatusText(string text)
        {
            // set the test of the first status strip text box
            ApplicationStatusStrip.Items[0].Text = text;
        }

        private void UpdateDrawingPointLabel()
        {
            DrawingPointLabel.Text = "Cursor Point: "
                + ((int)MOUSE_LOCATION.X).ToString()
                + " , "
                + ((int)MOUSE_LOCATION.Y).ToString()
                + "   Map Point: "
                + ((int)CURSOR_POINT.X).ToString()
                + " , "
                + ((int)CURSOR_POINT.Y).ToString();

            ApplicationStatusStrip.Invalidate();
        }

        private void UpdateMapNameAndSize()
        {
            MapNameLabel.Text = CURRENT_MAP.MapName;
            MapSizeLabel.Text = "Map Size: " + CURRENT_MAP.MapWidth.ToString() + " x " + CURRENT_MAP.MapHeight.ToString();

            MapStatusStrip.Refresh();
        }

        private void SetDrawingModeLabel()
        {
            string modeText = "Drawing Mode: ";

            modeText += CURRENT_DRAWING_MODE switch
            {
                MapDrawingMode.None => "None",
                MapDrawingMode.LandPaint => "Paint Landform",
                MapDrawingMode.LandErase => "Erase Landform",
                MapDrawingMode.LandColorErase => "Erase Landform Color",
                MapDrawingMode.LandColor => "Color Landform",
                MapDrawingMode.OceanErase => "Erase Ocean",
                MapDrawingMode.OceanPaint => "Paint Ocean",
                MapDrawingMode.ColorSelect => "Select Color",
                MapDrawingMode.LandformSelect => "Select Landform",
                MapDrawingMode.WaterPaint => "Paint Water Feature",
                MapDrawingMode.WaterErase => "Erase Water Feature",
                MapDrawingMode.WaterColor => "Color Water Feature",
                MapDrawingMode.WaterColorErase => "Erase Water Feature Color",
                MapDrawingMode.LakePaint => "Paint Lake",
                MapDrawingMode.RiverPaint => "Paint River",
                MapDrawingMode.RiverEdit => "Edit River",
                MapDrawingMode.WaterFeatureSelect => "Select Water Feature",
                MapDrawingMode.PathPaint => "Draw Path",
                MapDrawingMode.PathSelect => "Select Path",
                MapDrawingMode.PathEdit => "Edit Path",
                MapDrawingMode.SymbolErase => "Erase Symbol",
                MapDrawingMode.SymbolPlace => "Place Symbol",
                MapDrawingMode.SymbolSelect => "Select Symbol",
                MapDrawingMode.SymbolColor => "Color Symbol",
                MapDrawingMode.DrawBezierLabelPath => "Draw Bezier Label Path",
                MapDrawingMode.DrawArcLabelPath => "Draw Arc Label Path",
                MapDrawingMode.DrawLabel => "Place Label",
                MapDrawingMode.LabelSelect => "Select Label",
                MapDrawingMode.DrawBox => "Draw Box",
                MapDrawingMode.PlaceWindrose => "Place Windrose",
                MapDrawingMode.SelectMapScale => "Move Map Scale",
                MapDrawingMode.DrawMapMeasure => "Draw Map Measure",
                MapDrawingMode.RegionPaint => "Draw Region",
                MapDrawingMode.RegionSelect => "Select Region",
                MapDrawingMode.RealmAreaSelect => "Select Area",
                MapDrawingMode.HeightMapPaint => "Paint Height Map",
                MapDrawingMode.MapHeightIncrease => "Increase Map Height",
                MapDrawingMode.MapHeightDecrease => "Decrease Map Height",
                _ => "Undefined",
            };

            modeText += ". Selected Brush: ";

            switch (SELECTED_COLOR_PAINT_BRUSH)
            {
                case ColorPaintBrush.SoftBrush:
                    modeText += "Soft Brush";
                    break;
                case ColorPaintBrush.HardBrush:
                    modeText += "Hard Brush";
                    break;
                case ColorPaintBrush.None:
                    break;
                default:
                    modeText += "None";
                    break;
            }

            DrawingModeLabel.Text = modeText;
            ApplicationStatusStrip.Refresh();
        }


        public static void DeselectAllMapComponents(MapComponent? selectedComponent)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

            foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                if (selectedComponent != null && selectedComponent is Landform landform && landform == l) continue;
                l.IsSelected = false;
            }

            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER);
            foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
            {
                if (selectedComponent == null)
                {
                    if (w is WaterFeature wf)
                    {
                        wf.IsSelected = false;
                    }
                    else if (w is River r)
                    {
                        r.IsSelected = false;
                    }
                }
                else if (selectedComponent != null)
                {
                    if (w is WaterFeature wf)
                    {
                        if (wf == selectedComponent)
                        {
                            continue;
                        }
                        else
                        {
                            wf.IsSelected = false;
                        }
                    }
                    else if (w is River r)
                    {
                        if (r == selectedComponent)
                        {
                            continue;
                        }
                        else
                        {
                            r.IsSelected = false;
                        }
                    }
                }
            }

            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);

            foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
            {
                if (selectedComponent != null && selectedComponent is MapPath mapPath && mapPath == mp) continue;
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }

            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);

            foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
            {
                if (selectedComponent != null && selectedComponent is MapPath mapPath && mapPath == mp) continue;
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }

            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.SYMBOLLAYER);

            foreach (MapSymbol symbol in symbolLayer.MapLayerComponents.Cast<MapSymbol>())
            {
                if (selectedComponent != null && selectedComponent is MapSymbol s && s == symbol) continue;
                symbol.IsSelected = false;
            }

            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LABELLAYER);

            foreach (MapLabel label in labelLayer.MapLayerComponents.Cast<MapLabel>())
            {
                if (selectedComponent != null && selectedComponent is MapLabel l && l == label) continue;
                label.IsSelected = false;
            }

            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BOXLAYER);

            foreach (PlacedMapBox box in boxLayer.MapLayerComponents.Cast<PlacedMapBox>())
            {
                if (selectedComponent != null && selectedComponent is PlacedMapBox b && b == box) continue;
                box.IsSelected = false;
            }

            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.REGIONLAYER);
            foreach (MapRegion r in regionLayer.MapLayerComponents.Cast<MapRegion>())
            {
                if (selectedComponent != null && selectedComponent is MapRegion region && region == r) continue;
                r.IsSelected = false;
            }
        }

        private static void SetSelectedBrushSize(int brushSize)
        {
            SELECTED_BRUSH_SIZE = brushSize;
        }

        public static void RenderMapToCanvas(SKCanvas renderCanvas, SKPoint scrollPoint)
        {
            // background
            MapRenderMethods.RenderBackground(CURRENT_MAP, renderCanvas, scrollPoint);

            // ocean layers
            MapRenderMethods.RenderOcean(CURRENT_MAP, renderCanvas, scrollPoint);

            // wind roses
            MapRenderMethods.RenderWindroses(CURRENT_MAP, CURRENT_WINDROSE, renderCanvas, scrollPoint);

            // lower grid layer (above ocean)
            MapRenderMethods.RenderLowerGrid(CURRENT_MAP, renderCanvas, scrollPoint);

            // landforms
            MapRenderMethods.RenderLandforms(CURRENT_MAP, CURRENT_LANDFORM, renderCanvas, scrollPoint);

            // water features
            MapRenderMethods.RenderWaterFeatures(CURRENT_MAP, CURRENT_WATERFEATURE, CURRENT_RIVER, renderCanvas, scrollPoint);

            // upper grid layer (above water features)
            MapRenderMethods.RenderUpperGrid(CURRENT_MAP, renderCanvas, scrollPoint);

            // lower path layer
            MapRenderMethods.RenderLowerMapPaths(CURRENT_MAP, CURRENT_MAP_PATH, renderCanvas, scrollPoint);

            // symbol layer
            MapRenderMethods.RenderSymbols(CURRENT_MAP, renderCanvas, scrollPoint);

            // upper path layer
            MapRenderMethods.RenderUpperMapPaths(CURRENT_MAP, CURRENT_MAP_PATH, renderCanvas, scrollPoint);

            // region and region overlay layers
            MapRenderMethods.RenderRegions(CURRENT_MAP, renderCanvas, scrollPoint);

            // default grid layer
            MapRenderMethods.RenderDefaultGrid(CURRENT_MAP, renderCanvas, scrollPoint);

            // box layer
            MapRenderMethods.RenderBoxes(CURRENT_MAP, renderCanvas, scrollPoint);

            // label layer
            MapRenderMethods.RenderLabels(CURRENT_MAP, renderCanvas, scrollPoint);

            // overlay layer (map scale)
            MapRenderMethods.RenderOverlays(CURRENT_MAP, renderCanvas, scrollPoint);

            // render frame
            MapRenderMethods.RenderFrame(CURRENT_MAP, renderCanvas, scrollPoint);

            // measure layer
            MapRenderMethods.RenderMeasures(CURRENT_MAP, renderCanvas, scrollPoint);

            // TODO: drawing layer
            //MapRenderMethods.RenderDrawing(CURRENT_MAP, renderCanvas, scrollPoint);

            // vignette layer
            MapRenderMethods.RenderVignette(CURRENT_MAP, renderCanvas, scrollPoint);
        }

        private void DrawCursor(SKCanvas canvas, SKPoint point, int brushSize)
        {
            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.SymbolPlace:
                    {
                        if (SymbolMethods.SelectedSymbolTableMapSymbol != null && !AreaBrushSwitch.Checked)
                        {
                            float symbolScale = (float)(SymbolScaleTrack.Value / 100.0F);
                            float symbolRotation = SymbolRotationTrack.Value;

                            if (SymbolMethods.SelectedSymbolTableMapSymbol.SymbolFormat != SymbolFileFormat.Vector)
                            {
                                SKBitmap? symbolBitmap = SymbolMethods.SelectedSymbolTableMapSymbol.ColorMappedBitmap;
                                if (symbolBitmap != null)
                                {
                                    // TODO: use matrix for scaling and rotation
                                    SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleSKBitmap(symbolBitmap, symbolScale);
                                    SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateSKBitmap(scaledSymbolBitmap, symbolRotation, MirrorSymbolSwitch.Checked);

                                    canvas.DrawBitmap(rotatedAndScaledBitmap,
                                        new SKPoint(point.X - (rotatedAndScaledBitmap.Width / 2), point.Y - (rotatedAndScaledBitmap.Height / 2)), null);

                                    scaledSymbolBitmap.Dispose();
                                    rotatedAndScaledBitmap.Dispose();
                                }
                            }
                            else
                            {
                                // display the SVG as the cursor
                                SKBitmap? symbolBitmap = SymbolMethods.GetBitmapForVectorSymbol(SymbolMethods.SelectedSymbolTableMapSymbol,
                                    0, 0, symbolRotation, symbolScale);

                                if (symbolBitmap != null)
                                {
                                    SKPoint pt = new(point.X - (symbolBitmap.Width / 2), point.Y - (symbolBitmap.Height / 2));
                                    canvas.DrawBitmap(symbolBitmap, pt);
                                }
                            }
                        }
                        else if (AreaBrushSwitch.Checked)
                        {
                            canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                        else
                        {
                            canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                    }
                    break;
                default:
                    {
                        if (brushSize > 0)
                        {
                            canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                    }
                    break;
            }
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

        #endregion

        #region Map File Open and Save Methods
        /******************************************************************************************************* 
         * MAP FILE OPEN AND SAVE METHODS
         *******************************************************************************************************/

        private DialogResult SaveMap()
        {
            if (CURRENT_MAP.IsSaved)
            {
                return DialogResult.OK;
            }

            if (!string.IsNullOrEmpty(CURRENT_MAP.MapPath) && CURRENT_MAP.MapName != "Default")
            {
                try
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();

                    MapFileMethods.SaveMap(CURRENT_MAP);
                    CURRENT_MAP.IsSaved = true;

                    if (Settings.Default.PlaySoundOnSave)
                    {
                        UtilityMethods.PlaySaveSound();
                    }

                    SetStatusText("Realm " + Path.GetFileNameWithoutExtension(CURRENT_MAP.MapPath) + " has been saved.");
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

            if (!string.IsNullOrEmpty(CURRENT_MAP.MapPath))
            {
                sfd.FileName = CURRENT_MAP.MapPath;
            }
            else if (!string.IsNullOrEmpty(CURRENT_MAP.MapName))
            {
                sfd.FileName = CURRENT_MAP.MapName;
            }

            DialogResult result = sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    CURRENT_MAP.MapPath = sfd.FileName;
                    CURRENT_MAP.MapName = Path.GetFileNameWithoutExtension(sfd.FileName);

                    UpdateMapNameAndSize();

                    try
                    {
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();

                        MapFileMethods.SaveMap(CURRENT_MAP);
                        CURRENT_MAP.IsSaved = true;

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
                    MapBuilder.DisposeMap(CURRENT_MAP);

                    // make a backup of the map to be opened in case it fails on open
                    SaveRealmFileBackup(mapFilePath);

                    RealmStudioMap? openedMap = MapFileMethods.OpenMap(mapFilePath);

                    if (openedMap != null)
                    {
                        CURRENT_MAP = openedMap;
                        SKImageInfo imageInfo = new(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                        if (CURRENT_MAP.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                        {
                            if (CURRENT_MAP.MapLayers.Count < MapBuilder.MAP_LAYER_COUNT)
                            {
                                MapBuilder.ConstructMissingLayersForMap(CURRENT_MAP, SKGLRenderControl.GRContext);
                            }
                        }

                        foreach (MapLayer ml in CURRENT_MAP.MapLayers)
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

                CURRENT_MAP.IsSaved = true;

                MapRenderHScroll.Maximum = CURRENT_MAP.MapWidth;
                MapRenderVScroll.Maximum = CURRENT_MAP.MapHeight;

                // finalize loading of ocean drawing layer
                MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANDRAWINGLAYER);

                for (int i = 0; i < oceanDrawingLayer.MapLayerComponents.Count; i++)
                {
                    if (oceanDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                    {
                        paintStroke.ParentMap = CURRENT_MAP;

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

                // finalize loading of landforms
                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
                SKImageInfo lfImageInfo = new(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
                {
                    if (landformLayer.MapLayerComponents[i] is Landform landform)
                    {
                        landform.ParentMap = CURRENT_MAP;
                        landform.IsModified = true;

                        landform.LandformRenderSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, false, lfImageInfo);
                        landform.CoastlineRenderSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, false, lfImageInfo);

                        LandformMethods.CreateInnerAndOuterPathsFromContourPoints(CURRENT_MAP, landform);

                        landform.ContourPath.GetBounds(out SKRect boundsRect);
                        landform.Width = (int)boundsRect.Width;
                        landform.Height = (int)boundsRect.Height;

                        if (landform.LandformTexture != null)
                        {
                            if (landform.LandformTexture.TextureBitmap != null)
                            {
                                Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                                // create and set a shader from the texture
                                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                                landform.LandformFillPaint.Shader = flpShader;
                            }
                            else
                            {
                                Bitmap b = new(landform.LandformTexture.TexturePath);
                                Bitmap resizedBitmap = new(b, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                                landform.LandformTexture.TextureBitmap = new(resizedBitmap);

                                // create and set a shader from the texture
                                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                                landform.LandformFillPaint.Shader = flpShader;
                            }
                        }
                        else
                        {
                            // texture is not set in the landform object, so use default
                            if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
                            {
                                AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
                            }

                            landform.LandformTexture = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX];

                            if (landform.LandformTexture.TextureBitmap != null)
                            {
                                Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                                // create and set a shader from the texture
                                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                                landform.LandformFillPaint.Shader = flpShader;
                            }
                        }

                        if (!landform.FillWithTexture || landform.LandformTexture == null || landform.LandformTexture.TextureBitmap == null)
                        {
                            landform.LandformFillPaint.Shader?.Dispose();
                            landform.LandformFillPaint.Shader = null;

                            SKShader flpShader = SKShader.CreateColor(landform.LandformBackgroundColor.ToSKColor());
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
                    }
                }

                // finalize loading of land drawing layer
                MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDDRAWINGLAYER);

                for (int i = 0; i < landDrawingLayer.MapLayerComponents.Count; i++)
                {
                    if (landDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                    {
                        paintStroke.ParentMap = CURRENT_MAP;
                        paintStroke.RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false, lfImageInfo);
                        paintStroke.Rendered = false;

                        if (paintStroke.Erase)
                        {
                            paintStroke.ShaderPaint = PaintObjects.LandColorEraserPaint;
                        }
                        else
                        {
                            paintStroke.ShaderPaint = PaintObjects.LandColorPaint;
                        }
                    }
                }


                // finalize loading of water features and rivers
                MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER);

                for (int i = 0; i < waterLayer.MapLayerComponents.Count; i++)
                {
                    if (waterLayer.MapLayerComponents[i] is WaterFeature waterFeature)
                    {
                        waterFeature.ParentMap = CURRENT_MAP;
                        WaterFeatureMethods.CreateInnerAndOuterPaths(CURRENT_MAP, waterFeature);
                        WaterFeatureMethods.ConstructWaterFeaturePaintObjects(waterFeature);
                    }
                    else if (waterLayer.MapLayerComponents[i] is River river)
                    {
                        river.ParentMap = CURRENT_MAP;
                        WaterFeatureMethods.ConstructRiverPaths(river);
                        WaterFeatureMethods.ConstructRiverPaintObjects(river);
                    }
                }

                // finalize loading of water drawing layer
                MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERDRAWINGLAYER);

                for (int i = 0; i < waterDrawingLayer.MapLayerComponents.Count; i++)
                {
                    if (waterDrawingLayer.MapLayerComponents[i] is LayerPaintStroke paintStroke)
                    {
                        paintStroke.ParentMap = CURRENT_MAP;

                        if (!paintStroke.Erase)
                        {
                            paintStroke.ShaderPaint = PaintObjects.WaterColorPaint;
                        }
                        else
                        {
                            paintStroke.ShaderPaint = PaintObjects.WaterColorEraserPaint;
                        }
                    }
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);

                for (int i = 0; i < pathLowerLayer.MapLayerComponents.Count; i++)
                {
                    if (pathLowerLayer.MapLayerComponents[i] is MapPath mapPath)
                    {
                        mapPath.ParentMap = CURRENT_MAP;
                        MapPathMethods.ConstructPathPaint(mapPath);

                        if (mapPath.PathPoints.Count > 1)
                        {
                            SKPath path = MapPathMethods.GenerateMapPathBoundaryPath(mapPath.PathPoints);

                            if (path.PointCount > 0)
                            {
                                mapPath.BoundaryPath?.Dispose();
                                mapPath.BoundaryPath = new(path);
                                path.Dispose();
                            }
                            else
                            {
                                pathLowerLayer.MapLayerComponents.Remove(mapPath);
                            }
                        }
                        else
                        {
                            pathLowerLayer.MapLayerComponents.Remove(mapPath);
                        }
                    }
                }

                for (int i = 0; i < pathUpperLayer.MapLayerComponents.Count; i++)
                {
                    if (pathUpperLayer.MapLayerComponents[i] is MapPath mapPath)
                    {
                        mapPath.ParentMap = CURRENT_MAP;
                        MapPathMethods.ConstructPathPaint(mapPath);

                        if (mapPath.PathPoints.Count > 1)
                        {
                            SKPath path = MapPathMethods.GenerateMapPathBoundaryPath(mapPath.PathPoints);

                            if (path.PointCount > 0)
                            {
                                mapPath.BoundaryPath?.Dispose();
                                mapPath.BoundaryPath = new(path);
                                path.Dispose();
                            }
                            else
                            {
                                pathUpperLayer.MapLayerComponents.Remove(mapPath);
                            }
                        }
                        else
                        {
                            pathUpperLayer.MapLayerComponents.Remove(mapPath);
                        }
                    }
                }


                // finalize loading of symbols
                MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.SYMBOLLAYER);

                for (int i = 0; i < symbolLayer.MapLayerComponents.Count; i++)
                {
                    if (symbolLayer.MapLayerComponents[i] is MapSymbol symbol)
                    {
                        SKColor? paintColor = symbol.CustomSymbolColors[0];

                        // reconstruct paint object for grayscale symbols
                        if (symbol.IsGrayscale && paintColor != null)
                        {
                            SKPaint paint = new()
                            {
                                ColorFilter = SKColorFilter.CreateBlendMode((SKColor)paintColor,
                                    SKBlendMode.Modulate) // combine the selected color with the bitmap colors
                            };

                            symbol.SymbolPaint = paint;

                            if (symbol.PlacedBitmap != null)
                            {
                                if (symbol.Width != symbol.PlacedBitmap.Width || symbol.Height != symbol.PlacedBitmap.Height)
                                {
                                    // resize the placed bitmap to match the size set in the symbol - this shouldn't be necessary
                                    SKBitmap resizedPlacedBitmap = new(symbol.Width, symbol.Height);

                                    symbol.PlacedBitmap.ScalePixels(resizedPlacedBitmap, SKSamplingOptions.Default);
                                    symbol.SetPlacedBitmap(resizedPlacedBitmap);

                                }
                            }
                        }
                    }
                }

                // finalize loading of grid
                MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER);
                for (int i = 0; i < defaultGridLayer.MapLayerComponents.Count; i++)
                {
                    if (defaultGridLayer.MapLayerComponents[i] is MapGrid grid)
                    {
                        CURRENT_MAP_GRID = grid;
                        CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
                        CURRENT_MAP_GRID.GridEnabled = true;
                        break;
                    }
                }

                if (CURRENT_MAP_GRID == null)
                {
                    MapLayer oceanGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER);
                    for (int i = 0; i < oceanGridLayer.MapLayerComponents.Count; i++)
                    {
                        if (oceanGridLayer.MapLayerComponents[i] is MapGrid grid)
                        {
                            CURRENT_MAP_GRID = grid;
                            CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.ABOVEOCEANGRIDLAYER;
                            CURRENT_MAP_GRID.GridEnabled = true;
                            break;
                        }
                    }
                }

                if (CURRENT_MAP_GRID == null)
                {
                    MapLayer symbolGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER);
                    for (int i = 0; i < symbolGridLayer.MapLayerComponents.Count; i++)
                    {
                        if (symbolGridLayer.MapLayerComponents[i] is MapGrid grid)
                        {
                            CURRENT_MAP_GRID = grid;
                            CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.BELOWSYMBOLSGRIDLAYER;
                            CURRENT_MAP_GRID.GridEnabled = true;
                            break;
                        }
                    }
                }

                if (CURRENT_MAP_GRID != null)
                {
                    CURRENT_MAP_GRID.ParentMap = CURRENT_MAP;
                    CURRENT_MAP_GRID.GridEnabled = true;

                    switch (CURRENT_MAP_GRID.GridType)
                    {
                        case MapGridType.Square:
                            SquareGridRadio.Checked = true;
                            break;
                        case MapGridType.PointedHex:
                            PointedHexGridRadio.Checked = true;
                            break;
                        case MapGridType.FlatHex:
                            FlatHexGridRadio.Checked = true;
                            break;
                    }

                    GridSizeTrack.Value = CURRENT_MAP_GRID.GridSize;
                    GridLineWidthTrack.Value = CURRENT_MAP_GRID.GridLineWidth;
                    GridColorSelectButton.BackColor = CURRENT_MAP_GRID.GridColor;

                    if (CURRENT_MAP_GRID.GridLayerIndex == MapBuilder.ABOVEOCEANGRIDLAYER)
                    {
                        GridLayerUpDown.SelectedItem = "Above Ocean";
                    }
                    else if (CURRENT_MAP_GRID.GridLayerIndex == MapBuilder.BELOWSYMBOLSGRIDLAYER)
                    {
                        GridLayerUpDown.SelectedItem = "Below Symbols";
                    }
                    else
                    {
                        GridLayerUpDown.SelectedItem = "Default";
                    }
                }

                // finalize loading of wind roses
                MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER);
                for (int i = 0; i < windroseLayer.MapLayerComponents.Count; i++)
                {
                    if (windroseLayer.MapLayerComponents[i] is MapWindrose windrose)
                    {
                        windrose.ParentMap = CURRENT_MAP;

                        windrose.WindrosePaint = new()
                        {
                            Style = SKPaintStyle.Stroke,
                            StrokeWidth = windrose.LineWidth,
                            Color = windrose.WindroseColor.ToSKColor(),
                            IsAntialias = true,
                        };
                    }
                }

                // finalize loading of placed map boxes
                MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BOXLAYER);
                for (int i = 0; i < boxLayer.MapLayerComponents.Count; i++)
                {
                    if (boxLayer.MapLayerComponents[i] is PlacedMapBox box && box.BoxBitmap != null)
                    {
                        SKRectI center = new((int)box.BoxCenterLeft, (int)box.BoxCenterTop,
                            (int)(box.Width - box.BoxCenterRight), (int)(box.Height - box.BoxCenterBottom));

                        if (center.IsEmpty || center.Left < 0 || center.Right <= 0 || center.Top < 0 || center.Bottom <= 0)
                        {
                        }
                        else if (center.Width <= 0 || center.Height <= 0)
                        {
                            // swap 
                            if (center.Right < center.Left)
                            {
                                (center.Left, center.Right) = (center.Right, center.Left);
                            }

                            if (center.Bottom < center.Top)
                            {
                                (center.Top, center.Bottom) = (center.Bottom, center.Top);
                            }
                        }

                        SKBitmap[] bitmapSlices = DrawingMethods.SliceNinePatchBitmap(box.BoxBitmap, center);

                        box.PatchA = bitmapSlices[0].Copy();   // top-left corner
                        box.PatchB = bitmapSlices[1].Copy();   // top
                        box.PatchC = bitmapSlices[2].Copy();   // top-right corner
                        box.PatchD = bitmapSlices[3].Copy();   // left size
                        box.PatchE = bitmapSlices[4].Copy();   // middle
                        box.PatchF = bitmapSlices[5].Copy();   // right side
                        box.PatchG = bitmapSlices[6].Copy();   // bottom-left corner
                        box.PatchH = bitmapSlices[7].Copy();   // bottom
                        box.PatchI = bitmapSlices[8].Copy();   // bottom-right corner
                    }
                }

                // finalize loading of regions
                MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.REGIONLAYER);
                for (int i = 0; i < regionLayer.MapLayerComponents.Count; i++)
                {
                    if (regionLayer.MapLayerComponents[i] is MapRegion region)
                    {
                        region.ParentMap = CURRENT_MAP;
                        SKPathEffect? regionBorderEffect = MapRegionMethods.ConstructRegionBorderEffect(region);
                        MapRegionMethods.ConstructRegionPaintObjects(region, regionBorderEffect);
                    }
                }

                // finalize loading of vignette
                MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER);
                for (int i = 0; i < vignetteLayer.MapLayerComponents.Count; i++)
                {
                    if (vignetteLayer.MapLayerComponents[i] is MapVignette vignette)
                    {
                        vignette.ParentMap = CURRENT_MAP;
                        vignette.Width = CURRENT_MAP.MapWidth;
                        vignette.Height = CURRENT_MAP.MapHeight;
                        vignette.VignetteRenderSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, false, lfImageInfo);
                    }
                }

                // convert MapImage objects in HeightMap layer to MapHeightMap objects
                MapLayer heightMapLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.HEIGHTMAPLAYER);
                if (heightMapLayer != null)
                {
                    if (heightMapLayer.MapLayerComponents.Count == 2)
                    {
                        if (heightMapLayer.MapLayerComponents[1] is MapImage mi && mi.MapImageBitmap != null)
                        {
                            MapHeightMap mhm = new()
                            {
                                MapImageBitmap = mi.MapImageBitmap.Copy(),
                                HeightMap = new float[CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight]
                            };

                            mhm.MapImageBitmap ??= new SKBitmap(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                            for (int i = 0; i < mhm.MapImageBitmap.Width; i++)
                            {
                                for (int j = 0; j < mhm.MapImageBitmap.Height; j++)
                                {
                                    mhm.HeightMap[i, j] = mhm.MapImageBitmap.GetPixel(i, j).Red;
                                }
                            }

                            heightMapLayer.MapLayerComponents[1] = mhm;
                        }
                        else if (heightMapLayer.MapLayerComponents[1] is MapHeightMap mhm)
                        {
                            mhm.MapImageBitmap ??= new SKBitmap(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);
                            mhm.HeightMap ??= new float[CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight];

                            for (int i = 0; i < mhm.MapImageBitmap.Width; i++)
                            {
                                for (int j = 0; j < mhm.MapImageBitmap.Height; j++)
                                {
                                    mhm.HeightMap[i, j] = mhm.MapImageBitmap.GetPixel(i, j).Red;
                                }
                            }
                        }
                    }
                    else
                    {
                        AddMapImagesToHeightMapLayer();
                    }
                }

                SetStatusText("Loaded: " + CURRENT_MAP.MapName);

                UpdateMapNameAndSize();

                SKGLRenderControl.Invalidate();
            }
            catch
            {
                MessageBox.Show("An error has occurred while opening the map. The map file may be corrupt.", "Error Loading Map", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                CURRENT_MAP = MapBuilder.CreateMap("", "DEFAULT", MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT, SKGLRenderControl.GRContext);

                for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count - 1; i > 0; i--)
                {
                    if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette)
                    {
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.RemoveAt(i);
                        break;
                    }
                }

                MapVignette vignette = new()
                {
                    ParentMap = CURRENT_MAP,
                    VignetteColor = VignetteColorSelectionButton.BackColor.ToArgb(),
                    VignetteStrength = VignetteStrengthTrack.Value,
                    RectangleVignette = RectangleVignetteRadio.Checked,
                    VignetteRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                        new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight)),
                };

                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Add(vignette);

                CURRENT_MAP.IsSaved = false;
                throw;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

#pragma warning disable SYSLIB1054
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, ref uint pcFonts);

        [DllImport("gdi32", CharSet = CharSet.Ansi)]
        private static extern int GetOutlineTextMetrics(
            IntPtr hdc,            // handle to DC
            int cbData,            // size in bytes for text metrics
            IntPtr lpOtm         // pointer to buffer to receive outline text metrics structure
        );
#pragma warning restore SYSLIB1054

        /// <summary>
        /// Gets the <see cref="PanoseFontFamilyTypes"/> for the specified font.
        /// </summary>
        /// <param name="graphics">A graphics object to use when detecting the Panose
        /// family.</param>
        /// <param name="font">The font to check.</param>
        /// <returns>The Panose font family type.</returns>
        public static PanoseFontFamilyTypes PanoseFontFamilyType(Graphics graphics, Font font)
        {
            byte bFamilyType = 0;

            IntPtr hdc = graphics.GetHdc();
            IntPtr hFontOld = SelectObject(hdc, font.ToHfont());

            int bufSize = GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
            IntPtr lpOtm = Marshal.AllocCoTaskMem(bufSize);
            Marshal.WriteInt32(lpOtm, bufSize);
            int success = GetOutlineTextMetrics(hdc, bufSize, lpOtm);
            if (success != 0)
            {
                int offset = 61;
                bFamilyType = Marshal.ReadByte(lpOtm, offset);
            }

            Marshal.FreeCoTaskMem(lpOtm);

            SelectObject(hdc, hFontOld);
            graphics.ReleaseHdc(hdc);

            return (PanoseFontFamilyTypes)bFamilyType;
        }

        #endregion

        #region Font Panel Methods

        private static void AddEmbeddedResourceFont(byte[] fontData)
        {
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;

            EMBEDDED_FONTS.AddMemoryFont(fontPtr, fontData.Length);
            AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
            Marshal.FreeCoTaskMem(fontPtr);
        }

        private void PopulateFontPanelUI()
        {
            InstalledFontCollection installedFontCollection = new();

            FontFamilyCombo.DrawItem += FontFamilyCombo_DrawItem;
            FontFamilyCombo.DisplayMember = "Name";

            // add embedded fonts
            AddEmbeddedResourceFont(Properties.Resources.Aladin_Regular);
            AddEmbeddedResourceFont(Properties.Resources.BarlowCondensed_Regular);
            AddEmbeddedResourceFont(Properties.Resources.Bilbo_Regular);
            AddEmbeddedResourceFont(Properties.Resources.CinzelDecorative_Regular);
            AddEmbeddedResourceFont(Properties.Resources.EastSeaDokdo_Regular);
            AddEmbeddedResourceFont(Properties.Resources.FrederickatheGreat_Regular);
            AddEmbeddedResourceFont(Properties.Resources.GentiumBookPlus_Bold);
            AddEmbeddedResourceFont(Properties.Resources.IMFellDWPica_Regular);
            AddEmbeddedResourceFont(Properties.Resources.IMFellEnglish_Italic);
            AddEmbeddedResourceFont(Properties.Resources.Katibeh_Regular);
            AddEmbeddedResourceFont(Properties.Resources.Lancelot_Regular);
            AddEmbeddedResourceFont(Properties.Resources.MarkoOne_Regular);
            AddEmbeddedResourceFont(Properties.Resources.Merriweather_Regular);
            AddEmbeddedResourceFont(Properties.Resources.Metamorphous_Regular);
            AddEmbeddedResourceFont(Properties.Resources.UncialAntiqua_Regular);

            Font aladinFont = new(EMBEDDED_FONTS.Families[0], 12.0F);
            Font barlowCondensedFont = new(EMBEDDED_FONTS.Families[1], 12.0F);
            Font bilboFont = new(EMBEDDED_FONTS.Families[2], 12.0F);
            Font cinzelDecorativeFont = new(EMBEDDED_FONTS.Families[3], 12.0F);
            Font eastSeaDokdoFont = new(EMBEDDED_FONTS.Families[4], 12.0F);
            Font frederickaFont = new(EMBEDDED_FONTS.Families[5], 12.0F);
            Font gentiumBookPlusFont = new(EMBEDDED_FONTS.Families[6], 12.0F);
            Font imFellDWPicaFont = new(EMBEDDED_FONTS.Families[7], 12.0F);
            Font imFellEnglishItalicFont = new(EMBEDDED_FONTS.Families[8], 12.0F);
            Font katibehFont = new(EMBEDDED_FONTS.Families[9], 12.0F);
            Font lancelotFont = new(EMBEDDED_FONTS.Families[10], 12.0F);
            Font markoOneFont = new(EMBEDDED_FONTS.Families[11], 12.0F);
            Font merriweatherFont = new(EMBEDDED_FONTS.Families[12], 12.0F);
            Font metamorphousFont = new(EMBEDDED_FONTS.Families[13], 12.0F);
            Font uncialAntiquaFont = new(EMBEDDED_FONTS.Families[14], 12.0F);

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

            fontIndex = FontFamilyCombo.Items.IndexOf(FONT_PANEL_SELECTED_FONT.SelectedFont);

            if (FontFamilyCombo.Items != null && fontIndex < 0)
            {
                // find by family name
                for (int i = 0; i < FontFamilyCombo.Items?.Count; i++)
                {
                    Font? f = FontFamilyCombo.Items[i] as Font;

                    string? fontFamilyName = f?.FontFamily.Name;

                    if (!string.IsNullOrEmpty(fontFamilyName) && fontFamilyName == FONT_PANEL_SELECTED_FONT.SelectedFont.FontFamily.Name)
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

                        if (FONT_PANEL_SELECTED_FONT.IsBold)
                        {
                            fs |= FontStyle.Bold;
                        }

                        if (FONT_PANEL_SELECTED_FONT.IsItalic)
                        {
                            fs |= FontStyle.Italic;
                        }

                        try
                        {
                            FONT_PANEL_SELECTED_FONT.SelectedFont = new Font(ff, fontSize, fs, GraphicsUnit.Point);
                        }
                        catch { }
                    }
                }
            }
        }

        private void SetExampleText()
        {
            Font textFont = new(FONT_PANEL_SELECTED_FONT.SelectedFont.FontFamily,
                FONT_PANEL_SELECTED_FONT.SelectedFont.Size * 0.75F, FONT_PANEL_SELECTED_FONT.SelectedFont.Style, GraphicsUnit.Point);

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

                    PanoseFontFamilyTypes familyType = PanoseFontFamilyType(e.Graphics, font);

                    //Debug.WriteLine("FAMILY TYPE: " + familyType);

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
            FONT_PANEL_SELECTED_FONT.IsBold = !FONT_PANEL_SELECTED_FONT.IsBold;

            if (FONT_PANEL_SELECTED_FONT.IsBold)
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
            FONT_PANEL_SELECTED_FONT.IsItalic = !FONT_PANEL_SELECTED_FONT.IsItalic;

            if (FONT_PANEL_SELECTED_FONT.IsItalic)
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
            switch (FONT_PANEL_OPENER)
            {
                case FontPanelOpener.ScaleFontButton:
                    {
                        SELECTED_MAP_SCALE_FONT = FONT_PANEL_SELECTED_FONT.SelectedFont;
                    }
                    break;
                case FontPanelOpener.LabelFontButton:
                    {
                        SelectLabelFontButton.Font = new Font(FONT_PANEL_SELECTED_FONT.SelectedFont.FontFamily, 12);

                        SelectLabelFontButton.Refresh();

                        SELECTED_LABEL_FONT = FONT_PANEL_SELECTED_FONT.SelectedFont;

                        if (SELECTED_MAP_LABEL != null)
                        {
                            Font newFont = new(FONT_PANEL_SELECTED_FONT.SelectedFont.FontFamily, FONT_PANEL_SELECTED_FONT.SelectedFont.Size * 0.75F, FONT_PANEL_SELECTED_FONT.SelectedFont.Style, GraphicsUnit.Point);

                            Color labelColor = FontColorSelectButton.BackColor;
                            Color outlineColor = OutlineColorSelectButton.BackColor;
                            float outlineWidth = OutlineWidthTrack.Value / 10F;
                            Color glowColor = GlowColorSelectButton.BackColor;
                            int glowStrength = GlowStrengthTrack.Value;

                            Cmd_ChangeLabelAttributes cmd = new(CURRENT_MAP, SELECTED_MAP_LABEL, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, newFont);
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
            string? fontString = cvt.ConvertToString(SELECTED_LABEL_FONT);
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

                        MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER);
                        baseLayer.MapLayerComponents.Clear();

                        float scale = BackgroundTextureScaleTrack.Value / 100F;
                        bool mirrorBackground = MirrorBackgroundSwitch.Checked;

                        BackgroundMethods.ClearBackgroundImage(CURRENT_MAP);

                        if (scale > 0.0F)
                        {
                            BackgroundMethods.ApplyBackgroundTexture(CURRENT_MAP, BackgroundMethods.GetSelectedBackgroundImage(), scale, mirrorBackground);
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

                    for (int i = 0; i < MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
                    {
                        if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
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

                    MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);
                    oceanTextureLayer.MapLayerComponents.Clear();

                    MapLayer oceanTextureOverLayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
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
                                OceanMethods.ApplyOceanTexture(CURRENT_MAP, (Bitmap)OceanTextureBox.Image, scale, mirrorOceanTexture);
                            }
                        }
                    }

                    OceanTextureBox.Refresh();

                    Color fillColor = OceanColorSelectButton.BackColor;

                    if (fillColor.ToArgb() != Color.White.ToArgb())
                    {
                        SKBitmap b = new(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                        using (SKCanvas canvas = new(b))
                        {
                            canvas.Clear(Extensions.ToSKColor(fillColor));
                        }

                        Cmd_SetOceanColor cmd = new(CURRENT_MAP, b);
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
                    LabelPresetsListBox.Items.Clear();

                    foreach (LabelPreset preset in AssetManager.LABEL_PRESETS)
                    {
                        if (!string.IsNullOrEmpty(preset.LabelPresetTheme)
                            && AssetManager.CURRENT_THEME != null
                            && preset.LabelPresetTheme == AssetManager.CURRENT_THEME.ThemeName)
                        {
                            LabelPresetsListBox.Items.Add(preset);
                        }
                    }

                    if (LabelPresetsListBox.Items.Count > 0)
                    {
                        LabelPresetsListBox.SelectedIndex = 0;

                        LabelPreset? selectedPreset = AssetManager.LABEL_PRESETS.Find(x => !string.IsNullOrEmpty(((LabelPreset?)LabelPresetsListBox.Items[0])?.LabelPresetName)
                            && x.LabelPresetName == ((LabelPreset?)LabelPresetsListBox.Items[0])?.LabelPresetName);

                        if (selectedPreset != null)
                        {
                            SetLabelValuesFromPreset(selectedPreset);
                        }
                    }

                    LabelPresetsListBox.Refresh();
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
                                    for (int i = 0; i < EMBEDDED_FONTS.Families.Length; i++)
                                    {
                                        if (EMBEDDED_FONTS.Families[i].Name == ff)
                                        {
                                            themeFont = new Font(EMBEDDED_FONTS.Families[i], fontsize, FontStyle.Regular, GraphicsUnit.Point);
                                        }
                                    }
                                }
                            }
                        }

                        if (themeFont != null)
                        {
                            FONT_PANEL_SELECTED_FONT.SelectedFont = new Font(themeFont.FontFamily, 12);
                            FONT_PANEL_SELECTED_FONT.FontSize = themeFont.SizeInPoints;
                            SELECTED_LABEL_FONT = themeFont;
                            SelectLabelFontButton.Font = new Font(themeFont.FontFamily, 14);
                            SelectLabelFontButton.Refresh();
                        }
                    }

                    FontColorSelectButton.BackColor = Color.FromArgb(theme.LabelColor ?? Color.FromArgb(61, 53, 30).ToArgb());
                    FontColorSelectButton.Refresh();

                    OutlineColorSelectButton.BackColor = Color.FromArgb(theme.LabelOutlineColor ?? Color.FromArgb(161, 214, 202, 171).ToArgb());
                    OutlineColorSelectButton.Refresh();

                    OutlineWidthTrack.Value = (int?)theme.LabelOutlineWidth ?? 0;
                    OutlineWidthTrack.Refresh();

                    GlowColorSelectButton.BackColor = Color.FromArgb(theme.LabelGlowColor ?? Color.White.ToArgb());
                    GlowStrengthTrack.Value = theme.LabelGlowStrength ?? 0;
                    GlowStrengthTrack.Refresh();
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
            ScrollPoint.Y = -e.NewValue;
            DrawingPoint.Y = e.NewValue;
            SKGLRenderControl.Invalidate();
        }

        private void MapRenderHScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.X = -e.NewValue;
            DrawingPoint.X = e.NewValue;
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
            e.Surface.Canvas.Scale(DrawingZoom);

            // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
            e.Surface.Canvas.Clear(SKColors.White);

            if (SKGLRenderControl.GRContext != null && CURRENT_MAP != null && CURRENT_MAP.MapLayers.Count == MapBuilder.MAP_LAYER_COUNT)
            {
                if (RenderAsHeightMapMenuItem.Checked)
                {
                    RealmMapMethods.RenderHeightMapToCanvas(CURRENT_MAP, e.Surface.Canvas, ScrollPoint);

                    if (CURRENT_DRAWING_MODE != MapDrawingMode.ColorSelect)
                    {
                        DrawCursor(e.Surface.Canvas, new SKPoint(CURSOR_POINT.X - ScrollPoint.X, CURSOR_POINT.Y - ScrollPoint.Y), SELECTED_BRUSH_SIZE);
                    }
                }
                else
                {
                    RenderMapToCanvas(e.Surface.Canvas, ScrollPoint);

                    if (CURRENT_DRAWING_MODE != MapDrawingMode.ColorSelect && CURRENT_DRAWING_MODE != MapDrawingMode.None)
                    {
                        DrawCursor(e.Surface.Canvas, new SKPoint(CURSOR_POINT.X - ScrollPoint.X, CURSOR_POINT.Y - ScrollPoint.Y), SELECTED_BRUSH_SIZE);
                    }

                    // work layer
                    MapRenderMethods.RenderWorkLayer(CURRENT_MAP, e.Surface.Canvas, ScrollPoint);
                }
            }
        }

        private void SKGLRenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            // objects are created and/or initialized on mouse down
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseDownHandler(e, SELECTED_BRUSH_SIZE);
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
            if (CURRENT_DRAWING_MODE == MapDrawingMode.ColorSelect)
            {
                Cursor = AssetManager.EYEDROPPER_CURSOR;
            }

            MOUSE_LOCATION = e.Location.ToSKPoint();
            CURSOR_POINT = new((e.X / DrawingZoom) - DrawingPoint.X, (e.Y / DrawingZoom) - DrawingPoint.Y);

            // objects are drawn or moved on mouse move
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseMoveHandler(e, SELECTED_BRUSH_SIZE / 2);
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
            // objects are finalized or reset on mouse up
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseUpHandler(e, SELECTED_BRUSH_SIZE / 2);
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
            if (CURRENT_DRAWING_MODE == MapDrawingMode.ColorSelect)
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
            int cursorDelta = 5;

            if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.LandErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandEraserSizeTrack.Minimum, Math.Min(newValue, LandEraserSizeTrack.Maximum));

                // landform eraser
                LandEraserSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.LandPaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandBrushSizeTrack.Minimum, Math.Min(newValue, LandBrushSizeTrack.Maximum));

                LandBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.LandColor)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandColorBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandColorBrushSizeTrack.Minimum, Math.Min(newValue, LandColorBrushSizeTrack.Maximum));

                LandColorBrushSizeTrack.Value = newValue;
                LandformMethods.LandformColorBrushSize = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.LandColorErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandColorEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandColorEraserSizeTrack.Minimum, Math.Min(newValue, LandColorEraserSizeTrack.Maximum));

                // land color eraser
                LandColorEraserSizeTrack.Value = newValue;
                LandformMethods.LandformColorEraserBrushSize = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.OceanErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = OceanEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(OceanEraserSizeTrack.Minimum, Math.Min(newValue, OceanEraserSizeTrack.Maximum));

                OceanEraserSizeTrack.Value = newValue;
                OceanMethods.OceanPaintEraserSize = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.OceanPaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = OceanBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(OceanBrushSizeTrack.Minimum, Math.Min(newValue, OceanBrushSizeTrack.Maximum));

                OceanBrushSizeTrack.Value = newValue;
                OceanMethods.OceanPaintBrushSize = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.WaterPaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterBrushSizeTrack.Minimum, Math.Min(newValue, WaterBrushSizeTrack.Maximum));

                WaterBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.WaterErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterEraserSizeTrack.Minimum, Math.Min(newValue, WaterEraserSizeTrack.Maximum));

                WaterEraserSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.LakePaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterBrushSizeTrack.Minimum, Math.Min(newValue, WaterBrushSizeTrack.Maximum));

                WaterBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.WaterColor)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterColorBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterColorBrushSizeTrack.Minimum, Math.Min(newValue, WaterColorBrushSizeTrack.Maximum));

                WaterColorBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.WaterColorErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterColorEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterColorEraserSizeTrack.Minimum, Math.Min(newValue, WaterColorEraserSizeTrack.Maximum));

                WaterColorEraserSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.MapHeightIncrease)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandBrushSizeTrack.Minimum, Math.Min(newValue, LandBrushSizeTrack.Maximum));

                LandBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.None && CURRENT_DRAWING_MODE == MapDrawingMode.MapHeightDecrease)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandBrushSizeTrack.Minimum, Math.Min(newValue, LandBrushSizeTrack.Maximum));

                LandBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys != Keys.Control && CURRENT_DRAWING_MODE == MapDrawingMode.SymbolPlace)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;

                if (AreaBrushSwitch.Enabled && AreaBrushSwitch.Checked)
                {
                    // area brush size is changed when AreaBrushSwitch is enabled and checked
                    int newValue = AreaBrushSizeTrack.Value + sizeDelta;
                    newValue = Math.Max(AreaBrushSizeTrack.Minimum, Math.Min(newValue, AreaBrushSizeTrack.Maximum));

                    AreaBrushSizeTrack.Value = newValue;
                    SELECTED_BRUSH_SIZE = newValue;
                }
                else
                {
                    int newValue = (int)Math.Max(SymbolScaleUpDown.Minimum, SymbolScaleUpDown.Value + sizeDelta);
                    newValue = (int)Math.Min(SymbolScaleUpDown.Maximum, newValue);

                    SymbolScaleUpDown.Value = newValue;
                }

                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys != Keys.Control && CURRENT_DRAWING_MODE == MapDrawingMode.LabelSelect)
            {
                if (SELECTED_MAP_LABEL != null)
                {
                    int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                    int newValue = Math.Max(LabelRotationTrack.Minimum, LabelRotationTrack.Value + sizeDelta);

                    LabelRotationTrack.Value = Math.Min(LabelRotationTrack.Maximum, newValue);
                    LabelRotationUpDown.Value = Math.Min(LabelRotationUpDown.Maximum, newValue);

                    SKGLRenderControl.Invalidate();
                }
            }
            else if (ModifierKeys == Keys.Control)
            {
                SetZoomLevel(e.Delta);

                ZoomLevelTrack.Value = (int)(DrawingZoom * 10.0F);

                SKGLRenderControl.Invalidate();
            }
        }

        private void SKGLRenderControl_Enter(object sender, EventArgs e)
        {
            if (CURRENT_DRAWING_MODE == MapDrawingMode.ColorSelect)
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

        private void LeftButtonMouseDownHandler(MouseEventArgs e, int brushSize)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            // has the map scale been clicked?
            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    SELECTED_MAP_SCALE = (MapScale?)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents[i];
                }
            }

            if (SELECTED_MAP_SCALE != null)
            {
                SKRect scaleRect = new(SELECTED_MAP_SCALE.X, SELECTED_MAP_SCALE.Y,
                    SELECTED_MAP_SCALE.X + SELECTED_MAP_SCALE.Width, SELECTED_MAP_SCALE.Y + SELECTED_MAP_SCALE.Height);

                if (scaleRect.Contains(zoomedScrolledPoint))
                {
                    Cursor = Cursors.SizeAll;
                    SELECTED_MAP_SCALE.IsSelected = true;
                    CURRENT_DRAWING_MODE = MapDrawingMode.SelectMapScale;
                }
            }

            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.OceanPaint:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE == null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = new LayerPaintStroke(CURRENT_MAP, OceanPaintColorSelectButton.BackColor.ToSKColor(),
                                SELECTED_COLOR_PAINT_BRUSH, SELECTED_BRUSH_SIZE / 2, MapBuilder.OCEANDRAWINGLAYER)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            Cmd_AddOceanPaintStroke cmd = new(CURRENT_MAP, CURRENT_LAYER_PAINT_STROKE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            BRUSH_VELOCITY = Math.Max(1, BASE_MILLIS_PER_PAINT_EVENT / (OceanBrushVelocityTrack.Value / 100.0));

                            StartBrushTimer((int)BRUSH_VELOCITY);
                        }
                    }
                    break;
                case MapDrawingMode.OceanErase:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE == null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = new LayerPaintStroke(CURRENT_MAP, SKColors.Transparent,
                                ColorPaintBrush.HardBrush, SELECTED_BRUSH_SIZE / 2, MapBuilder.OCEANDRAWINGLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            Cmd_AddOceanPaintStroke cmd = new(CURRENT_MAP, CURRENT_LAYER_PAINT_STROKE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandformSelect:
                    {
                        Cursor = Cursors.Default;

                        SELECTED_LANDFORM = SelectLandformAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    {
                        CURRENT_MAP.IsSaved = false;

                        Cursor = Cursors.Cross;

                        CURRENT_LANDFORM = new()
                        {
                            ParentMap = CURRENT_MAP,
                            Width = CURRENT_MAP.MapWidth,
                            Height = CURRENT_MAP.MapHeight,
                            IsModified = true,
                            FillWithTexture = UseTextureForBackgroundCheck.Checked,

                            LandformRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight)),

                            CoastlineRenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                        };

                        SetLandformData(CURRENT_LANDFORM);

                        CURRENT_LANDFORM.IsModified = true;

                        CURRENT_LANDFORM.DrawPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushSize / 2);

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAND_ERASE_STROKE == null)
                        {
                            CURRENT_LAND_ERASE_STROKE = new LayerPaintStroke(CURRENT_MAP, SKColors.White,
                                ColorPaintBrush.HardBrush, SELECTED_BRUSH_SIZE / 2, MapBuilder.LANDFORMLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER).MapLayerComponents.Add(CURRENT_LAND_ERASE_STROKE);
                        }

                        CURRENT_LAND_ERASE_STROKE?.AddLayerPaintStrokePoint(zoomedScrolledPoint);

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandColor:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE == null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = new LayerPaintStroke(CURRENT_MAP, LandColorSelectionButton.BackColor.ToSKColor(),
                                SELECTED_COLOR_PAINT_BRUSH, SELECTED_BRUSH_SIZE / 2, MapBuilder.LANDDRAWINGLAYER)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            Cmd_AddLandPaintStroke cmd = new(CURRENT_MAP, CURRENT_LAYER_PAINT_STROKE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            BRUSH_VELOCITY = Math.Max(1, BASE_MILLIS_PER_PAINT_EVENT / (LandBrushVelocityTrack.Value / 100.0));

                            StartBrushTimer((int)BRUSH_VELOCITY);
                        }
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE == null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = new LayerPaintStroke(CURRENT_MAP, SKColors.Transparent, ColorPaintBrush.HardBrush, SELECTED_BRUSH_SIZE / 2, MapBuilder.LANDDRAWINGLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            Cmd_AddLandPaintStroke cmd = new(CURRENT_MAP, CURRENT_LAYER_PAINT_STROKE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    {
                        SELECTED_WATERFEATURE = (IWaterFeature?)SelectWaterFeatureAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_WATERFEATURE = new()
                        {
                            ParentMap = CURRENT_MAP,
                            WaterFeatureType = WaterFeatureType.Other,
                            WaterFeatureColor = WaterColorSelectionButton.BackColor,
                            WaterFeatureShorelineColor = ShorelineColorSelectionButton.BackColor
                        };

                        WaterFeatureMethods.ConstructWaterFeaturePaintObjects(CURRENT_WATERFEATURE);
                    }
                    break;
                case MapDrawingMode.LakePaint:
                    {
                        // TODO: undo/redo
                        Cursor = Cursors.Cross;

                        CURRENT_WATERFEATURE = new()
                        {
                            ParentMap = CURRENT_MAP,
                            X = (int)zoomedScrolledPoint.X,
                            Y = (int)zoomedScrolledPoint.Y,
                            Width = brushSize,
                            Height = brushSize,
                            WaterFeatureType = WaterFeatureType.Lake,
                            WaterFeatureColor = WaterColorSelectionButton.BackColor,
                            WaterFeatureShorelineColor = ShorelineColorSelectionButton.BackColor,
                        };

                        SKPath? lakePath = WaterFeatureMethods.GenerateRandomLakePath(zoomedScrolledPoint, brushSize);

                        if (lakePath != null)
                        {
                            CURRENT_WATERFEATURE.WaterFeaturePath = lakePath;
                            WaterFeatureMethods.CreateInnerAndOuterPaths(CURRENT_MAP, CURRENT_WATERFEATURE);
                            WaterFeatureMethods.ConstructWaterFeaturePaintObjects(CURRENT_WATERFEATURE);
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER).MapLayerComponents.Add(CURRENT_WATERFEATURE);

                            WaterFeatureMethods.MergeWaterFeatures(CURRENT_MAP);
                        }
                        else
                        {
                            CURRENT_WATERFEATURE = null;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    {
                        Cursor = Cursors.Cross;

                        if (CURRENT_RIVER == null)
                        {
                            // initialize river
                            CURRENT_RIVER = new River()
                            {
                                ParentMap = CURRENT_MAP,
                                RiverColor = WaterColorSelectionButton.BackColor,
                                RiverShorelineColor = ShorelineColorSelectionButton.BackColor,
                                RiverWidth = RiverWidthTrack.Value,
                                RiverSourceFadeIn = RiverSourceFadeInSwitch.Checked,
                                RenderRiverTexture = RiverTextureSwitch.Checked,
                            };

                            WaterFeatureMethods.ConstructRiverPaintObjects(CURRENT_RIVER);
                            CURRENT_RIVER.RiverPoints.Add(new MapRiverPoint(zoomedScrolledPoint));

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
                            if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river)
                            {
                                foreach (MapRiverPoint mp in river.RiverPoints)
                                {
                                    mp.IsSelected = false;
                                }

                                SELECTED_RIVERPOINT = WaterFeatureMethods.SelectRiverPointAtPoint(river, zoomedScrolledPoint, false);

                                if (SELECTED_RIVERPOINT != null)
                                {
                                    SELECTED_RIVERPOINT.IsControlPoint = true;
                                }
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterColor:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE == null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = new LayerPaintStroke(CURRENT_MAP, WaterPaintColorSelectButton.BackColor.ToSKColor(),
                                SELECTED_COLOR_PAINT_BRUSH, SELECTED_BRUSH_SIZE / 2, MapBuilder.WATERDRAWINGLAYER)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false,
                                new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            Cmd_AddWaterPaintStroke cmd = new(CURRENT_MAP, CURRENT_LAYER_PAINT_STROKE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            BRUSH_VELOCITY = Math.Max(1, BASE_MILLIS_PER_PAINT_EVENT / (LandBrushVelocityTrack.Value / 100.0));

                            StartBrushTimer((int)BRUSH_VELOCITY);
                        }
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE == null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = new LayerPaintStroke(CURRENT_MAP, SKColors.Empty, ColorPaintBrush.HardBrush, SELECTED_BRUSH_SIZE / 2, MapBuilder.WATERDRAWINGLAYER, true)
                            {
                                RenderSurface = SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                            };

                            Cmd_AddWaterPaintStroke cmd = new(CURRENT_MAP, CURRENT_LAYER_PAINT_STROKE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    {
                        Cursor = Cursors.Cross;
                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                        if (CURRENT_MAP_PATH == null)
                        {
                            // initialize map path
                            CURRENT_MAP_PATH = new MapPath
                            {
                                ParentMap = CURRENT_MAP,
                                PathType = GetSelectedPathType(),
                                PathColor = PathColorSelectButton.BackColor,
                                PathWidth = PathWidthTrack.Value,
                                DrawOverSymbols = DrawOverSymbolsSwitch.Checked,
                            };

                            if (AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap == null)
                            {
                                AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX].TexturePath);
                            }

                            CURRENT_MAP_PATH.PathTexture = AssetManager.PATH_TEXTURE_LIST[AssetManager.SELECTED_PATH_TEXTURE_INDEX];

                            MapPathMethods.ConstructPathPaint(CURRENT_MAP_PATH);
                            CURRENT_MAP_PATH.PathPoints.Add(new MapPathPoint(zoomedScrolledPoint));

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
                            if (SELECTED_PATH != null)
                            {
                                foreach (MapPathPoint mp in SELECTED_PATH.PathPoints)
                                {
                                    mp.IsSelected = false;
                                }

                                SELECTED_PATHPOINT = MapPathMethods.SelectMapPathPointAtPoint(SELECTED_PATH, zoomedScrolledPoint, false);

                                if (SELECTED_PATHPOINT != null)
                                {
                                    SELECTED_PATHPOINT.IsControlPoint = true;
                                }
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.SymbolPlace:
                    {
                        if (AreaBrushSwitch.Checked)
                        {
                            int brushInterval = (int)(200.0F / PLACEMENT_RATE);
                            StartSymbolAreaBrushTimer(brushInterval);
                        }
                        else
                        {
                            PlaceSelectedSymbolAtCursor(zoomedScrolledPoint);
                        }

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                    }
                    break;
                case MapDrawingMode.SymbolErase:
                    int eraserRadius = AreaBrushSizeTrack.Value / 2;

                    SKPoint eraserCursorPoint = new(zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

                    SymbolMethods.RemovePlacedSymbolsFromArea(CURRENT_MAP, eraserCursorPoint, eraserRadius);
                    break;
                case MapDrawingMode.DrawArcLabelPath:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_MAP_LABEL_PATH.Dispose();
                        CURRENT_MAP_LABEL_PATH = new();

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                    }
                    break;
                case MapDrawingMode.DrawBezierLabelPath:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_MAP_LABEL_PATH.Dispose();
                        CURRENT_MAP_LABEL_PATH = new();

                        CURRENT_MAP_LABEL_PATH_POINTS.Clear();

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                    }
                    break;
                case MapDrawingMode.DrawBox:
                    {
                        if (SELECTED_MAP_BOX != null)
                        {
                            // initialize new box
                            Cursor = Cursors.Cross;

                            PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                            SELECTED_PLACED_MAP_BOX = new()
                            {
                                X = (int)zoomedScrolledPoint.X,
                                Y = (int)zoomedScrolledPoint.Y,

                                BoxCenterLeft = SELECTED_MAP_BOX.BoxCenterLeft,
                                BoxCenterTop = SELECTED_MAP_BOX.BoxCenterTop,
                                BoxCenterRight = SELECTED_MAP_BOX.BoxCenterRight,
                                BoxCenterBottom = SELECTED_MAP_BOX.BoxCenterBottom,

                                BoxTint = SelectBoxTintButton.BackColor
                            };

                            PaintObjects.BoxPaint.Dispose();

                            PaintObjects.BoxPaint = new()
                            {
                                Style = SKPaintStyle.Fill,
                                ColorFilter = SKColorFilter.CreateBlendMode(
                                    Extensions.ToSKColor(SelectBoxTintButton.BackColor),
                                    SKBlendMode.Modulate) // combine the tint with the bitmap color
                            };

                            SELECTED_PLACED_MAP_BOX.BoxPaint = PaintObjects.BoxPaint.Clone();
                        }
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    {
                        if (CURRENT_MAP_MEASURE == null)
                        {
                            // make sure there is only one measure object
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();

                            CURRENT_MAP_MEASURE = new(CURRENT_MAP)
                            {
                                UseMapUnits = UseScaleUnitsSwitch.Checked,
                                MeasureArea = MeasureAreaSwitch.Checked,
                                MeasureLineColor = SelectMeasureColorButton.BackColor
                            };

                            PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER).MapLayerComponents.Add(CURRENT_MAP_MEASURE);
                        }
                        else
                        {
                            if (!CURRENT_MAP_MEASURE.MeasurePoints.Contains(zoomedScrolledPoint))
                            {
                                CURRENT_MAP_MEASURE.MeasurePoints.Add(zoomedScrolledPoint);
                            }
                        }
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        // TODO: refactor to pull this code into a separate method
                        Cursor = Cursors.Cross;

                        // initialize region
                        if (CURRENT_MAP_REGION == null)
                        {
                            CURRENT_MAP_REGION = new()
                            {
                                ParentMap = CURRENT_MAP
                            };

                            SetRegionData(CURRENT_MAP_REGION);
                        }

                        if (ModifierKeys == Keys.Shift)
                        {
                            // find the closest point to the current point
                            // on the contour path of a coastline;
                            // if the nearest point on the coastline
                            // is within 5 pixels of the current point,
                            // then set the region point to be the point
                            // on the coastline
                            // then check the *previous* region point; if the previous
                            // region point is on the coastline of the same landform
                            // then add all of the points on the coastline contour
                            // to the region points

                            int coastlinePointIndex = -1;
                            SKPoint coastlinePoint = SKPoint.Empty;
                            Landform? landform1 = null;
                            Landform? landform2 = null;

                            float currentDistance = float.MaxValue;

                            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

                            // get the distance from the point the cursor was clicked to the contour points of all landforms
                            foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
                            {
                                for (int i = 0; i < lf.ContourPoints.Count; i++)
                                {
                                    SKPoint p = lf.ContourPoints[i];
                                    float distance = SKPoint.Distance(zoomedScrolledPoint, p);

                                    if (distance < currentDistance && distance < 5)
                                    {
                                        landform1 = lf;
                                        coastlinePointIndex = i;
                                        coastlinePoint = p;
                                        currentDistance = distance;
                                    }
                                }

                                if (coastlinePointIndex >= 0) break;
                            }

                            int previousCoastlinePointIndex = -1;
                            currentDistance = float.MaxValue;

                            if (landform1 != null && coastlinePointIndex >= 0)
                            {
                                MapRegionPoint mrp = new(landform1.ContourPoints[coastlinePointIndex]);
                                CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);

                                if (CURRENT_MAP_REGION.MapRegionPoints.Count > 1 && coastlinePointIndex > 1)
                                {
                                    SKPoint previousPoint = CURRENT_MAP_REGION.MapRegionPoints[^2].RegionPoint;

                                    foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
                                    {
                                        for (int i = 0; i < lf.ContourPoints.Count; i++)
                                        {
                                            SKPoint p = lf.ContourPoints[i];
                                            float distance = SKPoint.Distance(previousPoint, p);

                                            if (distance < currentDistance && !coastlinePoint.Equals(previousPoint))
                                            {
                                                landform2 = lf;
                                                previousCoastlinePointIndex = i;
                                                currentDistance = distance;
                                            }
                                        }

                                        if (previousCoastlinePointIndex >= 0) break;
                                    }
                                }
                            }

                            if (landform1 != null && landform2 != null
                                && landform1.LandformGuid.ToString() == landform2.LandformGuid.ToString()
                                && coastlinePointIndex >= 0 && previousCoastlinePointIndex >= 0)
                            {
                                CURRENT_MAP_REGION.MapRegionPoints.Clear();

                                if (zoomedScrolledPoint.Y < PREVIOUS_CURSOR_POINT.Y)
                                {
                                    // drag mouse up to snap to west coast of landform
                                    for (int i = previousCoastlinePointIndex; i < landform1.ContourPoints.Count - 1; i++)
                                    {
                                        MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                                        CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);
                                    }

                                    for (int i = 0; i <= coastlinePointIndex; i++)
                                    {
                                        MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                                        CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);
                                    }
                                }
                                else
                                {
                                    // drag mouse down to snap region to east coast of landform
                                    if (coastlinePointIndex > previousCoastlinePointIndex)
                                    {
                                        for (int i = previousCoastlinePointIndex; i <= coastlinePointIndex; i++)
                                        {
                                            MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                                            CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);
                                        }
                                    }
                                    else
                                    {
                                        for (int i = coastlinePointIndex; i <= previousCoastlinePointIndex; i++)
                                        {
                                            MapRegionPoint mrp = new(landform1.ContourPoints[i]);
                                            CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MapRegionPoint mrp = new(zoomedScrolledPoint);
                            CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);
                        }

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (CURRENT_MAP_REGION != null && MapRegionMethods.NEW_REGION_POINT != null)
                        {
                            Cmd_AddMapRegionPoint cmd = new(CURRENT_MAP, CURRENT_MAP_REGION, MapRegionMethods.NEW_REGION_POINT, MapRegionMethods.NEXT_REGION_POINT_INDEX);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            // reset
                            MapRegionMethods.NEW_REGION_POINT = null;
                            MapRegionMethods.NEXT_REGION_POINT_INDEX = -1;
                            MapRegionMethods.PREVIOUS_REGION_POINT_INDEX = -1;
                        }

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    Cursor = Cursors.Cross;
                    PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                    break;

            }
        }

        private static void MiddleButtonMouseDownHandler(MouseEventArgs e)
        {
            PREVIOUS_MOUSE_LOCATION = e.Location;
        }

        private void RightButtonMouseDownHandler(MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.LabelSelect:
                    {
                        if (SELECTED_MAP_LABEL != null)
                        {
                            CREATING_LABEL = true;

                            Font labelFont = new(SELECTED_MAP_LABEL.LabelFont.FontFamily, SELECTED_MAP_LABEL.LabelFont.Size * DrawingZoom,
                                SELECTED_MAP_LABEL.LabelFont.Style, GraphicsUnit.Pixel);

                            Size labelSize = TextRenderer.MeasureText(SELECTED_MAP_LABEL.LabelText, labelFont,
                                new Size(int.MaxValue, int.MaxValue),
                                TextFormatFlags.Default | TextFormatFlags.SingleLine | TextFormatFlags.TextBoxControl);

                            int tbX = (int)((SELECTED_MAP_LABEL.X - DrawingPoint.X) * DrawingZoom);
                            int tbY = (int)((SELECTED_MAP_LABEL.Y - DrawingPoint.Y) * DrawingZoom) - (labelSize.Height / 2);
                            int tbW = (int)(SELECTED_MAP_LABEL.Width * DrawingZoom);
                            int tbH = (int)(SELECTED_MAP_LABEL.Height * 1.5F * DrawingZoom);

                            LABEL_TEXT_BOX = new()
                            {
                                Parent = SKGLRenderControl,
                                Name = Guid.NewGuid().ToString(),
                                Left = tbX,
                                Top = tbY,
                                Width = tbW,
                                Height = tbH,
                                Margin = Padding.Empty,
                                Padding = Padding.Empty,
                                AutoSize = true,
                                Font = SELECTED_MAP_LABEL.LabelFont,
                                Visible = true,
                                BackColor = Color.AliceBlue,
                                ForeColor = SELECTED_MAP_LABEL.LabelColor,
                                BorderStyle = BorderStyle.None,
                                Multiline = false,
                                TextAlign = HorizontalAlignment.Left,
                                Text = SELECTED_MAP_LABEL.LabelText,
                            };

                            LABEL_TEXT_BOX.KeyPress += LabelTextBox_KeyPress;

                            SKGLRenderControl.Controls.Add(LABEL_TEXT_BOX);

                            LABEL_TEXT_BOX.BringToFront();
                            LABEL_TEXT_BOX.Select(LABEL_TEXT_BOX.Text.Length, 0);
                            LABEL_TEXT_BOX.Focus();
                            LABEL_TEXT_BOX.ScrollToCaret();

                            LABEL_TEXT_BOX.Tag = SELECTED_MAP_LABEL.LabelPath;

                            // delete the currently selected label
                            MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LABELLAYER);

                            for (int i = labelLayer.MapLayerComponents.Count - 1; i >= 0; i--)
                            {
                                if (labelLayer.MapLayerComponents[i] is MapLabel l && l.LabelGuid.ToString() == SELECTED_MAP_LABEL.LabelGuid.ToString())
                                {
                                    labelLayer.MapLayerComponents.RemoveAt(i);
                                }
                            }

                            SKGLRenderControl.Refresh();

                        }
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    if (CURRENT_MAP_MEASURE != null)
                    {
                        CURRENT_MAP_MEASURE.MeasurePoints.Add(zoomedScrolledPoint);

                        float lineLength = SKPoint.Distance(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint);
                        CURRENT_MAP_MEASURE.TotalMeasureLength += lineLength;
                        CURRENT_MAP_MEASURE.RenderValue = true;

                        // reset everything
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                        PREVIOUS_CURSOR_POINT = SKPoint.Empty;

                        CURRENT_MAP_MEASURE = null;
                        CURRENT_DRAWING_MODE = MapDrawingMode.None;
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    if (CURRENT_MAP_REGION != null)
                    {
                        MapRegionPoint mrp = new(zoomedScrolledPoint);
                        CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);

                        CURRENT_MAP.IsSaved = false;

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        Cmd_AddMapRegion cmd = new(CURRENT_MAP, CURRENT_MAP_REGION);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        CURRENT_MAP_REGION.IsSelected = false;

                        // reset everything

                        CURRENT_MAP_REGION = null;
                        CURRENT_DRAWING_MODE = MapDrawingMode.None;
                        SetDrawingModeLabel();
                    }

                    break;
            }
        }

        #endregion

        #region SKGLRenderControl Mouse Move Event Handling Methods (called from event handlers)

        #region Left Button Move Handler Method

        private void LeftButtonMouseMoveHandler(MouseEventArgs e, int brushRadius)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.OceanErase:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_LAYER_PAINT_STROKE?.AddLayerPaintStrokePoint(zoomedScrolledPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    {
                        if (CURRENT_LANDFORM != null
                            && zoomedScrolledPoint.X > 0 && zoomedScrolledPoint.X < CURRENT_MAP.MapWidth
                            && zoomedScrolledPoint.Y > 0 && zoomedScrolledPoint.Y < CURRENT_MAP.MapHeight)
                        {
                            CURRENT_LANDFORM.IsModified = true;

                            CURRENT_LANDFORM.DrawPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                            bool createPathsWhilePainting = Settings.Default.CalculateContoursWhilePainting;

                            if (createPathsWhilePainting)
                            {
                                // compute contour path and inner and outer paths in a separate thread
                                Task.Run(() => LandformMethods.CreateAllPathsFromDrawnPath(CURRENT_MAP, CURRENT_LANDFORM));
                            }
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        Cursor = Cursors.Cross;
                        MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
                        MapLayer landcoastLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDCOASTLINELAYER);

                        LandformMethods.LandformErasePath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                        CURRENT_LAND_ERASE_STROKE?.AddLayerPaintStrokePoint(zoomedScrolledPoint);
                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_LAYER_PAINT_STROKE?.AddLayerPaintStrokePoint(zoomedScrolledPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    {
                        Cursor = Cursors.Cross;

                        if (CURRENT_WATERFEATURE != null)
                        {
                            CURRENT_WATERFEATURE.WaterFeaturePath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                            // compute contour path and inner and outer paths in a separate thread
                            Task.Run(() => WaterFeatureMethods.CreateInnerAndOuterPaths(CURRENT_MAP, CURRENT_WATERFEATURE));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterErase:
                    {
                        Cursor = Cursors.Cross;

                        WaterFeatureMethods.WaterFeaturErasePath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                        WaterFeatureMethods.EraseWaterFeature(CURRENT_MAP);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_LAYER_PAINT_STROKE?.AddLayerPaintStrokePoint(zoomedScrolledPoint);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_RIVER?.RiverPoints.Add(new MapRiverPoint(zoomedScrolledPoint));

                        if (CURRENT_RIVER != null)
                        {
                            WaterFeatureMethods.ConstructRiverPaths(CURRENT_RIVER);
                        }

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river && SELECTED_RIVERPOINT != null)
                    {
                        // move the selected point on the path
                        WaterFeatureMethods.MoveSelectedRiverPoint(river, SELECTED_RIVERPOINT, zoomedScrolledPoint);

                        CURRENT_MAP.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    {
                        Cursor = Cursors.Cross;

                        SKPoint newPathPoint = zoomedScrolledPoint;
                        MapPathPoint? previousPoint = CURRENT_MAP_PATH?.PathPoints.First();

                        if (ModifierKeys == Keys.Shift && CURRENT_MAP_PATH?.PathPoints.Count > 5)
                        {
                            // draw straight path, clamped to 5 degree angles
                            if (previousPoint != null)
                            {
                                if (SELECTED_PATH_ANGLE == -1)
                                {
                                    float lineAngle = DrawingMethods.CalculateLineAngle(previousPoint.MapPoint, zoomedScrolledPoint);

                                    lineAngle = (lineAngle < 0) ? lineAngle + 360 : lineAngle;

                                    lineAngle = (float)(Math.Round(lineAngle / 5, MidpointRounding.AwayFromZero) * 5);

                                    SELECTED_PATH_ANGLE = lineAngle;
                                }

                                float distance = SKPoint.Distance(previousPoint.MapPoint, zoomedScrolledPoint);
                                newPathPoint = DrawingMethods.PointOnCircle(distance, SELECTED_PATH_ANGLE, previousPoint.MapPoint);
                            }
                        }
                        else if (ModifierKeys == Keys.Control)
                        {
                            // draw straight horizontal or vertical path
                            if (previousPoint != null)
                            {
                                if (SELECTED_PATH_ANGLE == -1)
                                {
                                    float lineAngle = DrawingMethods.CalculateLineAngle(previousPoint.MapPoint, zoomedScrolledPoint);

                                    lineAngle = (lineAngle < 0) ? lineAngle + 360 : lineAngle;
                                    SELECTED_PATH_ANGLE = lineAngle;
                                }

                                if (SELECTED_PATH_ANGLE >= 0 && SELECTED_PATH_ANGLE < 45)
                                {
                                    newPathPoint.Y = previousPoint.MapPoint.Y;
                                }
                                else if (SELECTED_PATH_ANGLE >= 45 && SELECTED_PATH_ANGLE < 135)
                                {
                                    newPathPoint.X = previousPoint.MapPoint.X;
                                }
                                else if (SELECTED_PATH_ANGLE >= 135 && SELECTED_PATH_ANGLE < 225)
                                {
                                    newPathPoint.Y = previousPoint.MapPoint.Y;
                                }
                                else if (SELECTED_PATH_ANGLE >= 225 && SELECTED_PATH_ANGLE < 315)
                                {
                                    newPathPoint.X = previousPoint.MapPoint.X;
                                }
                                else if (SELECTED_PATH_ANGLE >= 315 && SELECTED_PATH_ANGLE < 360)
                                {
                                    newPathPoint.Y = previousPoint.MapPoint.Y;
                                }
                            }
                        }

                        // make the spacing between points consistent
                        float spacing = 4.0F;

                        if (CURRENT_MAP_PATH?.PathType == PathType.RailroadTracksPath)
                        {
                            // railroad track points are further apart to make them look better
                            spacing = 2.0F;
                        }

                        if (CURRENT_MAP_PATH != null
                            && SKPoint.Distance(CURRENT_MAP_PATH.PathPoints.Last().MapPoint, newPathPoint) >= CURRENT_MAP_PATH.PathWidth / spacing)
                        {
                            CURRENT_MAP_PATH?.PathPoints.Add(new MapPathPoint(newPathPoint));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        if (SELECTED_PATH != null)
                        {
                            SELECTED_PATH.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(SELECTED_PATH.PathPoints);
                            SELECTED_PATH.BoundaryPath.GetTightBounds(out SKRect boundsRect);

                            // move the entire selected path with the mouse
                            SizeF delta = new()
                            {
                                Width = zoomedScrolledPoint.X - PREVIOUS_CURSOR_POINT.X,
                                Height = zoomedScrolledPoint.Y - PREVIOUS_CURSOR_POINT.Y,
                            };

                            foreach (MapPathPoint point in SELECTED_PATH.PathPoints)
                            {
                                SKPoint p = point.MapPoint;
                                p.X = p.X + delta.Width - (int)(boundsRect.MidX - PREVIOUS_CURSOR_POINT.X);
                                p.Y = p.Y + delta.Height - (int)(boundsRect.MidY - PREVIOUS_CURSOR_POINT.Y);
                                point.MapPoint = p;
                            }

                            SELECTED_PATH.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(SELECTED_PATH.PathPoints);
                            PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    if (SELECTED_PATHPOINT != null)
                    {
                        // move the selected point on the path
                        MapPathMethods.MoveSelectedMapPathPoint(SELECTED_PATH, SELECTED_PATHPOINT, zoomedScrolledPoint);

                        CURRENT_MAP.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.SymbolPlace:
                    if (!AreaBrushSwitch.Checked)
                    {
                        PlaceSelectedSymbolAtCursor(zoomedScrolledPoint);
                    }

                    PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.SymbolErase:
                    int eraserRadius = AreaBrushSizeTrack.Value / 2;

                    SymbolMethods.RemovePlacedSymbolsFromArea(CURRENT_MAP, zoomedScrolledPoint, eraserRadius);

                    CURRENT_MAP.IsSaved = false;
                    break;
                case MapDrawingMode.SymbolColor:

                    int colorBrushRadius = AreaBrushSizeTrack.Value / 2;

                    Color[] symbolColors = [SymbolColor1Button.BackColor, SymbolColor2Button.BackColor, SymbolColor3Button.BackColor];
                    SymbolMethods.ColorSymbolsInArea(CURRENT_MAP, zoomedScrolledPoint, colorBrushRadius, symbolColors, RandomizeColorCheck.Checked);

                    CURRENT_MAP.IsSaved = false;
                    break;
                case MapDrawingMode.SymbolSelect:
                    if (SELECTED_MAP_SYMBOL != null && SELECTED_MAP_SYMBOL.IsSelected)
                    {
                        SELECTED_MAP_SYMBOL.X = (int)zoomedScrolledPoint.X - SELECTED_MAP_SYMBOL.Width / 2;
                        SELECTED_MAP_SYMBOL.Y = (int)zoomedScrolledPoint.Y - SELECTED_MAP_SYMBOL.Height / 2;

                        CURRENT_MAP.IsSaved = false;
                    }

                    break;
                case MapDrawingMode.LabelSelect:
                    if (SELECTED_MAP_LABEL != null)
                    {
                        if (SELECTED_MAP_LABEL.LabelPath != null)
                        {
                            float dx = zoomedScrolledPoint.X - (SELECTED_MAP_LABEL.X + (SELECTED_MAP_LABEL.Width / 2));
                            float dy = zoomedScrolledPoint.Y - (SELECTED_MAP_LABEL.Y + (SELECTED_MAP_LABEL.Height / 2));
                            SELECTED_MAP_LABEL.LabelPath.Transform(SKMatrix.CreateTranslation(dx, dy));
                        }
                        else
                        {
                            SELECTED_MAP_LABEL.X = (int)zoomedScrolledPoint.X - SELECTED_MAP_LABEL.Width / 2;
                            SELECTED_MAP_LABEL.Y = (int)zoomedScrolledPoint.Y;
                        }

                    }
                    else if (SELECTED_PLACED_MAP_BOX != null)
                    {
                        SELECTED_PLACED_MAP_BOX.X = (int)zoomedScrolledPoint.X - (SELECTED_PLACED_MAP_BOX.Width / 2);
                        SELECTED_PLACED_MAP_BOX.Y = (int)zoomedScrolledPoint.Y - (SELECTED_PLACED_MAP_BOX.Height / 2);
                    }

                    CURRENT_MAP.IsSaved = false;
                    break;
                case MapDrawingMode.DrawArcLabelPath:
                    {

                        CURRENT_MAP_LABEL_PATH.Dispose();
                        CURRENT_MAP_LABEL_PATH = new();

                        if (zoomedScrolledPoint.Y > PREVIOUS_CURSOR_POINT.Y)
                        {
                            // start on the left and drag right and down to draw an arc downward (open part of the arc facing down)
                            SKRect r = new(PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);
                            CURRENT_MAP_LABEL_PATH.AddArc(r, 180, 180);
                        }
                        else
                        {
                            // start on the right and drag left and up to draw an arc upward (open part of the arc facing up)
                            SKRect r = new(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y);
                            CURRENT_MAP_LABEL_PATH.AddArc(r, 180, -180);
                        }

                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawPath(CURRENT_MAP_LABEL_PATH, PaintObjects.LabelPathPaint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawBezierLabelPath:
                    {
                        CURRENT_MAP_LABEL_PATH_POINTS.Add(zoomedScrolledPoint);
                        ConstructBezierPathFromPoints();

                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawPath(CURRENT_MAP_LABEL_PATH, PaintObjects.LabelPathPaint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawBox:
                    // draw box as mouse is moved
                    if (SELECTED_MAP_BOX != null && SELECTED_PLACED_MAP_BOX != null)
                    {
                        SKRect boxRect = new(PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

                        if (boxRect.Width > 0 && boxRect.Height > 0)
                        {
                            SKBitmap? b = SELECTED_MAP_BOX.BoxBitmap.ToSKBitmap();

                            if (b != null)
                            {
                                SKBitmap resizedBitmap = b.Resize(new SKSizeI((int)boxRect.Width, (int)boxRect.Height), SKSamplingOptions.Default);

                                SELECTED_PLACED_MAP_BOX.SetBoxBitmap(resizedBitmap);
                                SELECTED_PLACED_MAP_BOX.Width = resizedBitmap.Width;
                                SELECTED_PLACED_MAP_BOX.Height = resizedBitmap.Height;

                                MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawBitmap(resizedBitmap, PREVIOUS_CURSOR_POINT, PaintObjects.BoxPaint);
                                SKGLRenderControl.Invalidate();
                            }
                        }
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    {
                        if (CURRENT_MAP_MEASURE != null)
                        {
                            MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                            if (CURRENT_MAP_MEASURE.MeasureArea && CURRENT_MAP_MEASURE.MeasurePoints.Count > 1)
                            {
                                SKPath path = new();

                                path.MoveTo(CURRENT_MAP_MEASURE.MeasurePoints.First());

                                for (int i = 1; i < CURRENT_MAP_MEASURE.MeasurePoints.Count; i++)
                                {
                                    path.LineTo(CURRENT_MAP_MEASURE.MeasurePoints[i]);
                                }

                                path.LineTo(zoomedScrolledPoint);

                                path.Close();

                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawPath(path, CURRENT_MAP_MEASURE.MeasureAreaPaint);
                            }
                            else
                            {
                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawLine(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint, CURRENT_MAP_MEASURE.MeasureLinePaint);
                            }

                            // render measure value and units
                            SKPoint measureValuePoint = new(zoomedScrolledPoint.X + 30, zoomedScrolledPoint.Y + 20);

                            float lineLength = SKPoint.Distance(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint);
                            float totalLength = CURRENT_MAP_MEASURE.TotalMeasureLength + lineLength;

                            CURRENT_MAP_MEASURE.RenderDistanceLabel(MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas, measureValuePoint, totalLength);

                            if (CURRENT_MAP_MEASURE.MeasureArea && CURRENT_MAP_MEASURE.MeasurePoints.Count > 1)
                            {
                                // temporarity add the point at the mouse position
                                CURRENT_MAP_MEASURE.MeasurePoints.Add(zoomedScrolledPoint);

                                // calculate the polygon area
                                float area = DrawingMethods.CalculatePolygonArea(CURRENT_MAP_MEASURE.MeasurePoints);

                                // remove the temporarily added point
                                CURRENT_MAP_MEASURE.MeasurePoints.RemoveAt(CURRENT_MAP_MEASURE.MeasurePoints.Count - 1);

                                // display the area label
                                SKPoint measureAreaPoint = new(zoomedScrolledPoint.X + 30, zoomedScrolledPoint.Y + 40);

                                CURRENT_MAP_MEASURE.RenderAreaLabel(MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas, measureAreaPoint, area);
                            }
                        }
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    if (CURRENT_MAP_REGION != null)
                    {
                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        if (CURRENT_MAP_REGION.MapRegionPoints.Count > 1)
                        {
                            // temporarily add the layer click point for rendering
                            MapRegionPoint mrp = new(zoomedScrolledPoint);
                            CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);

                            // render
                            CURRENT_MAP_REGION.Render(MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas);

                            // remove it
                            CURRENT_MAP_REGION.MapRegionPoints.RemoveAt(CURRENT_MAP_REGION.MapRegionPoints.Count - 1);
                        }
                        else
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawLine(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint, CURRENT_MAP_REGION.RegionBorderPaint);
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (CURRENT_MAP_REGION != null && CURRENT_MAP_REGION.IsSelected)
                        {
                            MapRegionPoint? selectedMapRegionPoint = null;
                            foreach (MapRegionPoint p in CURRENT_MAP_REGION.MapRegionPoints)
                            {
                                using SKPath path = new();
                                path.AddCircle(p.RegionPoint.X, p.RegionPoint.Y, 5);

                                if (path.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                                {
                                    // editing (moving) the selected point
                                    p.IsSelected = true;
                                    selectedMapRegionPoint = p;
                                    MapRegionMethods.EDITING_REGION = true;
                                }
                                else
                                {
                                    p.IsSelected = false;
                                }
                            }

                            if (selectedMapRegionPoint != null)
                            {
                                selectedMapRegionPoint.RegionPoint = zoomedScrolledPoint;
                            }

                            if (!MapRegionMethods.EDITING_REGION)
                            {
                                // not moving a point, so drag the region
                                float xDelta = zoomedScrolledPoint.X - PREVIOUS_CURSOR_POINT.X;
                                float yDelta = zoomedScrolledPoint.Y - PREVIOUS_CURSOR_POINT.Y;

                                foreach (MapRegionPoint p in CURRENT_MAP_REGION.MapRegionPoints)
                                {
                                    SKPoint newPoint = p.RegionPoint;
                                    newPoint.X += xDelta;
                                    newPoint.Y += yDelta;
                                    p.RegionPoint = newPoint;

                                }

                                PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                            }
                        }
                    }
                    break;
                case MapDrawingMode.SelectMapScale:
                    if (SELECTED_MAP_SCALE != null)
                    {
                        SELECTED_MAP_SCALE.X = (int)zoomedScrolledPoint.X - SELECTED_MAP_SCALE.Width / 2;
                        SELECTED_MAP_SCALE.Y = (int)zoomedScrolledPoint.Y - SELECTED_MAP_SCALE.Height / 2;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    {
                        SELECTED_REALM_AREA = new(PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawRect(SELECTED_REALM_AREA, PaintObjects.LandformAreaSelectPaint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightIncrease:
                    {
                        float brushStrength = (float)BrushStrengthUpDown.Value;
                        RealmMapMethods.ChangeHeightMapAreaHeight(CURRENT_MAP, zoomedScrolledPoint, brushRadius, brushStrength);

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightDecrease:
                    {
                        float brushStrength = (float)BrushStrengthUpDown.Value;
                        RealmMapMethods.ChangeHeightMapAreaHeight(CURRENT_MAP, zoomedScrolledPoint, brushRadius, -brushStrength);

                        CURRENT_MAP.IsSaved = false;
                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }

        }

        #endregion

        #region Right Button Move Handler Method

        private void RightButtonMouseMoveHandler(object sender, MouseEventArgs e)
        {

        }
        #endregion

        #region Middle Button Move Handler Method
        private void MiddleButtonMouseMoveHandler(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;

            // pan the map when middle button (mouse wheel) is held down and dragged

            int xDelta = e.Location.X - PREVIOUS_MOUSE_LOCATION.X;
            int yDelta = e.Location.Y - PREVIOUS_MOUSE_LOCATION.Y;

            ScrollPoint.X += xDelta;
            DrawingPoint.X += -xDelta;

            MapRenderHScroll.Value = Math.Max(0, Math.Min((int)DrawingPoint.X, CURRENT_MAP.MapWidth));

            ScrollPoint.Y += yDelta;
            DrawingPoint.Y += -yDelta;

            MapRenderVScroll.Value = Math.Max(0, Math.Min((int)DrawingPoint.Y, CURRENT_MAP.MapHeight));

            PREVIOUS_MOUSE_LOCATION = e.Location;

            SKGLRenderControl.Invalidate();
        }

        #endregion

        #region No Button Move Handler Method

        private void NoButtonMouseMoveHandler(MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.PlaceWindrose:
                    if (CURRENT_WINDROSE != null)
                    {
                        CURRENT_WINDROSE.X = (int)zoomedScrolledPoint.X;
                        CURRENT_WINDROSE.Y = (int)zoomedScrolledPoint.Y;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river)
                        {
                            foreach (MapRiverPoint mp in river.RiverPoints)
                            {
                                mp.IsSelected = false;
                            }

                            SELECTED_RIVERPOINT = WaterFeatureMethods.SelectRiverPointAtPoint(river, zoomedScrolledPoint);
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        if (SELECTED_PATH != null)
                        {
                            foreach (MapPathPoint mp in SELECTED_PATH.PathPoints)
                            {
                                mp.IsSelected = false;
                            }

                            SELECTED_PATHPOINT = MapPathMethods.SelectMapPathPointAtPoint(SELECTED_PATH, zoomedScrolledPoint);
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionPaint:
                    {
                        if (CURRENT_MAP_REGION == null || CURRENT_MAP_REGION.MapRegionPoints.Count == 1)
                        {
                            if (ModifierKeys == Keys.Shift)
                            {
                                // find the closest point to the current point
                                // on the contour path of a coastline;
                                // if the nearest point on the coastline
                                // is within 5 pixels of the current point,
                                // then set the region point to be the point
                                // on the coastline
                                // then check the *previous* region point; if the previous
                                // region point is on the coastline of the same landform
                                // then add all of the points on the coastline contour
                                // to the region points

                                int coastlinePointIndex = -1;
                                SKPoint coastlinePoint = SKPoint.Empty;

                                float currentDistance = float.MaxValue;

                                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
                                MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                                workLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

                                // get the distance from the cursor point to the contour points of all landforms
                                foreach (Landform lf in landformLayer.MapLayerComponents.Cast<Landform>())
                                {
                                    for (int i = 0; i < lf.ContourPoints.Count; i++)
                                    {
                                        SKPoint p = lf.ContourPoints[i];
                                        float distance = SKPoint.Distance(zoomedScrolledPoint, p);

                                        if (distance < currentDistance && distance < 5)
                                        {
                                            coastlinePointIndex = i;
                                            coastlinePoint = p;
                                            currentDistance = distance;
                                        }
                                    }

                                    if (coastlinePointIndex >= 0) break;
                                }

                                if (coastlinePoint != SKPoint.Empty)
                                {
                                    workLayer.LayerSurface?.Canvas.DrawCircle(coastlinePoint, 5, PaintObjects.MapPathSelectedControlPointPaint);
                                }
                            }
                        }
                        else
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                            if (CURRENT_MAP_REGION.MapRegionPoints.Count > 1)
                            {
                                // temporarily add the layer click point for rendering
                                MapRegionPoint mrp = new(zoomedScrolledPoint);
                                CURRENT_MAP_REGION.MapRegionPoints.Add(mrp);

                                // render
                                CURRENT_MAP_REGION.Render(MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas);

                                // remove it
                                CURRENT_MAP_REGION.MapRegionPoints.RemoveAt(CURRENT_MAP_REGION.MapRegionPoints.Count - 1);
                            }
                            else
                            {
                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawLine(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint, CURRENT_MAP_REGION.RegionBorderPaint);
                            }

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (CURRENT_MAP_REGION != null && CURRENT_MAP_REGION.IsSelected)
                        {
                            bool pointSelected = false;
                            foreach (MapRegionPoint p in CURRENT_MAP_REGION.MapRegionPoints)
                            {
                                using SKPath path = new();
                                path.AddCircle(p.RegionPoint.X, p.RegionPoint.Y, 5);

                                if (path.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                                {
                                    pointSelected = true;
                                    p.IsSelected = true;
                                }
                                else
                                {
                                    if (!MapRegionMethods.EDITING_REGION)
                                    {
                                        p.IsSelected = false;
                                    }
                                }
                            }

                            if (!pointSelected)
                            {
                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                                MapRegionMethods.PREVIOUS_REGION_POINT_INDEX = -1;
                                MapRegionMethods.NEXT_REGION_POINT_INDEX = -1;
                                MapRegionMethods.NEW_REGION_POINT = null;

                                // is the cursor on a line segment between 2 region vertices? if so, draw a circle at the cursor location
                                for (int i = 0; i < CURRENT_MAP_REGION.MapRegionPoints.Count - 1; i++)
                                {
                                    if (DrawingMethods.LineContainsPoint(zoomedScrolledPoint, CURRENT_MAP_REGION.MapRegionPoints[i].RegionPoint, CURRENT_MAP_REGION.MapRegionPoints[i + 1].RegionPoint))
                                    {
                                        MapRegionMethods.EDITING_REGION = true;

                                        MapRegionMethods.PREVIOUS_REGION_POINT_INDEX = i;
                                        MapRegionMethods.NEXT_REGION_POINT_INDEX = i + 1;

                                        MapRegionMethods.NEW_REGION_POINT = new MapRegionPoint(zoomedScrolledPoint);

                                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, MapRegionMethods.POINT_CIRCLE_RADIUS, PaintObjects.RegionNewPointFillPaint);
                                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, MapRegionMethods.POINT_CIRCLE_RADIUS, PaintObjects.RegionPointOutlinePaint);

                                        break;
                                    }
                                }

                                if (DrawingMethods.LineContainsPoint(zoomedScrolledPoint, CURRENT_MAP_REGION.MapRegionPoints[0].RegionPoint,
                                    CURRENT_MAP_REGION.MapRegionPoints[^1].RegionPoint))
                                {
                                    MapRegionMethods.EDITING_REGION = true;

                                    MapRegionMethods.PREVIOUS_REGION_POINT_INDEX = 0;
                                    MapRegionMethods.NEXT_REGION_POINT_INDEX = CURRENT_MAP_REGION.MapRegionPoints.Count;

                                    MapRegionMethods.NEW_REGION_POINT = new MapRegionPoint(zoomedScrolledPoint);

                                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, MapRegionMethods.POINT_CIRCLE_RADIUS, PaintObjects.RegionNewPointFillPaint);
                                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawCircle(zoomedScrolledPoint, MapRegionMethods.POINT_CIRCLE_RADIUS, PaintObjects.RegionPointOutlinePaint);
                                }
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

        private void LeftButtonMouseUpHandler(MouseEventArgs e, int brushRadius)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            StopSymbolAreaBrushTimer();

            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.OceanPaint:
                    {
                        StopBrushTimer();

                        if (CURRENT_LAYER_PAINT_STROKE != null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = null;
                            CURRENT_MAP.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.OceanErase:
                    {
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE != null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = null;
                            CURRENT_MAP.IsSaved = false;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandPaint:
                    if (CURRENT_LANDFORM != null)
                    {
                        // TODO: undo/redo

                        bool createPathsWhilePainting = Settings.Default.CalculateContoursWhilePainting;

                        if (!createPathsWhilePainting)
                        {
                            // compute contour path and inner and outer paths in a separate thread
                            LandformMethods.CreateAllPathsFromDrawnPath(CURRENT_MAP, CURRENT_LANDFORM);
                        }

                        CURRENT_LANDFORM.IsModified = true;

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER).MapLayerComponents.Add(CURRENT_LANDFORM);
                        LandformMethods.MergeLandforms(CURRENT_MAP);

                        CURRENT_LANDFORM = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

                        for (int i = landformLayer.MapLayerComponents.Count - 1; i >= 0; i--)
                        {
                            if (landformLayer.MapLayerComponents[i] is LayerPaintStroke)
                            {
                                landformLayer.MapLayerComponents.RemoveAt(i);
                            }
                        }

                        CURRENT_LAND_ERASE_STROKE = null;

                        Landform? erasedLandform = GetLandformIntersectingCircle(CURRENT_MAP, zoomedScrolledPoint, brushRadius);

                        if (erasedLandform != null)
                        {
                            LandformMethods.EraseLandForm(erasedLandform);

                            LandformMethods.MergeLandforms(CURRENT_MAP);

                            LandformMethods.CreateAllPathsFromDrawnPath(CURRENT_MAP, erasedLandform);

                            erasedLandform.IsModified = true;
                        }

                        CURRENT_MAP.IsSaved = false;
                        SKGLRenderControl.Refresh();
                    }
                    break;
                case MapDrawingMode.LandColor:
                    {
                        Cursor = Cursors.Cross;

                        StopBrushTimer();

                        if (CURRENT_LAYER_PAINT_STROKE != null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = null;
                            CURRENT_MAP.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LandColorErase:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_LAYER_PAINT_STROKE = null;
                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PlaceWindrose:
                    if (CURRENT_WINDROSE != null)
                    {
                        CURRENT_WINDROSE.X = (int)zoomedScrolledPoint.X;
                        CURRENT_WINDROSE.Y = (int)zoomedScrolledPoint.Y;

                        Cmd_AddWindrose cmd = new(CURRENT_MAP, CURRENT_WINDROSE);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        CURRENT_WINDROSE = CreateWindrose();

                        CURRENT_MAP.IsSaved = false;
                    }
                    break;
                case MapDrawingMode.WaterPaint:
                    if (CURRENT_WATERFEATURE != null)
                    {
                        // TODO: undo/redo
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER).MapLayerComponents.Add(CURRENT_WATERFEATURE);
                        WaterFeatureMethods.MergeWaterFeatures(CURRENT_MAP);

                        CURRENT_WATERFEATURE = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.LakePaint:
                    if (CURRENT_WATERFEATURE != null)
                    {
                        CURRENT_WATERFEATURE = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverPaint:
                    if (CURRENT_RIVER != null)
                    {
                        WaterFeatureMethods.ConstructRiverPaths(CURRENT_RIVER);
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER).MapLayerComponents.Add(CURRENT_RIVER);

                        CURRENT_RIVER = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RiverEdit:
                    {
                        SELECTED_RIVERPOINT = null;
                    }
                    break;
                case MapDrawingMode.WaterColor:
                    {
                        StopBrushTimer();

                        if (CURRENT_LAYER_PAINT_STROKE != null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = null;
                            CURRENT_MAP.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.WaterColorErase:
                    {
                        Cursor = Cursors.Cross;

                        if (CURRENT_LAYER_PAINT_STROKE != null)
                        {
                            CURRENT_LAYER_PAINT_STROKE = null;
                            CURRENT_MAP.IsSaved = false;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathPaint:
                    if (CURRENT_MAP_PATH != null)
                    {
                        CURRENT_MAP_PATH.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(CURRENT_MAP_PATH.PathPoints);

                        if (CURRENT_MAP_PATH.DrawOverSymbols)
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Add(CURRENT_MAP_PATH);
                        }
                        else
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Add(CURRENT_MAP_PATH);
                        }

                        CURRENT_MAP_PATH = null;
                        SELECTED_PATH_ANGLE = -1;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathSelect:
                    {
                        SELECTED_PATH = SelectMapPathAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.PathEdit:
                    {
                        SELECTED_PATHPOINT = null;
                    }
                    break;
                case MapDrawingMode.SymbolSelect:
                    {
                        SELECTED_MAP_SYMBOL = SelectMapSymbolAtPoint(CURRENT_MAP, zoomedScrolledPoint.ToDrawingPoint());

                        if (SELECTED_MAP_SYMBOL != null)
                        {
                            SELECTED_MAP_SYMBOL.IsSelected = !SELECTED_MAP_SYMBOL.IsSelected;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawLabel:
                    if (!CREATING_LABEL)
                    {
                        CREATING_LABEL = true;

                        Font tbFont = new(SELECTED_LABEL_FONT.FontFamily,
                            SELECTED_LABEL_FONT.Size * 0.75F, SELECTED_LABEL_FONT.Style, GraphicsUnit.Point);

                        Size labelSize = TextRenderer.MeasureText("...Label...", tbFont,
                            new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.Default | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.ExternalLeading | TextFormatFlags.SingleLine);

                        LABEL_TEXT_BOX = new()
                        {
                            Parent = SKGLRenderControl,
                            Name = Guid.NewGuid().ToString(),
                            Left = e.X - (labelSize.Width / 2),
                            Top = e.Y - (labelSize.Height / 2),
                            Width = labelSize.Width,
                            Height = labelSize.Height,
                            Margin = System.Windows.Forms.Padding.Empty,
                            AutoSize = false,
                            Font = tbFont,
                            Visible = true,
                            PlaceholderText = "...Label...",
                            BackColor = Color.Peru,
                            ForeColor = FontColorSelectButton.BackColor,
                            BorderStyle = BorderStyle.FixedSingle,
                            Multiline = false,
                            TextAlign = HorizontalAlignment.Center,
                            Text = "...Label...",
                        };

                        LABEL_TEXT_BOX.KeyPress += LabelTextBox_KeyPress;

                        SKGLRenderControl.Controls.Add(LABEL_TEXT_BOX);

                        SKGLRenderControl.Refresh();
                        Refresh();

                        LABEL_TEXT_BOX.BringToFront();
                        LABEL_TEXT_BOX.Select(LABEL_TEXT_BOX.Text.Length, 0);
                        LABEL_TEXT_BOX.Focus();
                        LABEL_TEXT_BOX.ScrollToCaret();

                    }
                    break;
                case MapDrawingMode.LabelSelect:
                    {
                        MapLabel? selectedLabel = SelectLabelAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                        if (selectedLabel != null)
                        {
                            bool isSelected = selectedLabel.IsSelected;

                            selectedLabel.IsSelected = !isSelected;

                            if (selectedLabel.IsSelected)
                            {
                                SELECTED_MAP_LABEL = selectedLabel;
                                LabelRotationTrack.Value = (int)SELECTED_MAP_LABEL.LabelRotationDegrees;
                                LabelRotationUpDown.Value = (int)SELECTED_MAP_LABEL.LabelRotationDegrees;
                            }
                            else
                            {
                                SELECTED_MAP_LABEL = null;
                            }

                            SKGLRenderControl.Invalidate();
                        }
                        else
                        {
                            SELECTED_MAP_LABEL = null;

                            PlacedMapBox? selectedMapBox = SelectMapBoxAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                            if (selectedMapBox != null)
                            {
                                bool isSelected = !selectedMapBox.IsSelected;
                                selectedMapBox.IsSelected = isSelected;

                                if (selectedMapBox.IsSelected)
                                {
                                    SELECTED_PLACED_MAP_BOX = selectedMapBox;
                                }
                                else
                                {
                                    SELECTED_PLACED_MAP_BOX = null;
                                }
                            }
                            else
                            {
                                SELECTED_PLACED_MAP_BOX = null;
                            }

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case MapDrawingMode.DrawBox:
                    // finalize box drawing

                    // clear the work layer
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                    if (SELECTED_PLACED_MAP_BOX != null && SELECTED_PLACED_MAP_BOX.BoxBitmap != null)
                    {
                        SKRectI center = new((int)SELECTED_PLACED_MAP_BOX.BoxCenterLeft,
                            (int)SELECTED_PLACED_MAP_BOX.BoxCenterTop,
                            (int)(SELECTED_PLACED_MAP_BOX.Width - SELECTED_PLACED_MAP_BOX.BoxCenterRight),
                            (int)(SELECTED_PLACED_MAP_BOX.Height - SELECTED_PLACED_MAP_BOX.BoxCenterBottom));

                        if (center.IsEmpty || center.Left < 0 || center.Right <= 0 || center.Top < 0 || center.Bottom <= 0)
                        {
                            return;
                        }
                        else if (center.Width <= 0 || center.Height <= 0)
                        {
                            // swap 
                            if (center.Right < center.Left)
                            {
                                (center.Left, center.Right) = (center.Right, center.Left);
                            }

                            if (center.Bottom < center.Top)
                            {
                                (center.Top, center.Bottom) = (center.Bottom, center.Top);
                            }
                        }

                        SKBitmap[] bitmapSlices = DrawingMethods.SliceNinePatchBitmap(SELECTED_PLACED_MAP_BOX.BoxBitmap, center);

                        SELECTED_PLACED_MAP_BOX.PatchA = bitmapSlices[0].Copy();   // top-left corner
                        SELECTED_PLACED_MAP_BOX.PatchB = bitmapSlices[1].Copy();   // top
                        SELECTED_PLACED_MAP_BOX.PatchC = bitmapSlices[2].Copy();   // top-right corner
                        SELECTED_PLACED_MAP_BOX.PatchD = bitmapSlices[3].Copy();   // left size
                        SELECTED_PLACED_MAP_BOX.PatchE = bitmapSlices[4].Copy();   // middle
                        SELECTED_PLACED_MAP_BOX.PatchF = bitmapSlices[5].Copy();   // right side
                        SELECTED_PLACED_MAP_BOX.PatchG = bitmapSlices[6].Copy();   // bottom-left corner
                        SELECTED_PLACED_MAP_BOX.PatchH = bitmapSlices[7].Copy();   // bottom
                        SELECTED_PLACED_MAP_BOX.PatchI = bitmapSlices[8].Copy();   // bottom-right corner

                        Cmd_AddLabelBox cmd = new(CURRENT_MAP, SELECTED_PLACED_MAP_BOX);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        CURRENT_MAP.IsSaved = false;
                        SELECTED_PLACED_MAP_BOX = null;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.DrawMapMeasure:
                    {
                        if (CURRENT_MAP_MEASURE != null)
                        {
                            if (!CURRENT_MAP_MEASURE.MeasurePoints.Contains(PREVIOUS_CURSOR_POINT))
                            {
                                CURRENT_MAP_MEASURE.MeasurePoints.Add(PREVIOUS_CURSOR_POINT);
                            }

                            CURRENT_MAP_MEASURE.MeasurePoints.Add(zoomedScrolledPoint);

                            float lineLength = SKPoint.Distance(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint);
                            CURRENT_MAP_MEASURE.TotalMeasureLength += lineLength;
                        }

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    {
                        if (MapRegionMethods.EDITING_REGION)
                        {
                            MapRegionMethods.EDITING_REGION = false;
                        }
                        else
                        {
                            MapRegion? selectedRegion = SelectRegionAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                            if (selectedRegion != null)
                            {
                                if (selectedRegion.IsSelected)
                                {
                                    CURRENT_MAP_REGION = selectedRegion;
                                }
                                else
                                {
                                    CURRENT_MAP_REGION = null;
                                }
                            }
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.ColorSelect:
                    {
                        // eyedropper color select function
                        using SKSurface s = SKSurface.Create(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
                        s.Canvas.Clear();

                        MapRenderMethods.RenderMapForExport(CURRENT_MAP, s.Canvas);

                        using Bitmap colorBitmap = s.Snapshot().ToBitmap();

                        Color pixelColor = colorBitmap.GetPixel((int)zoomedScrolledPoint.X, (int)zoomedScrolledPoint.Y);

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
                    if (SELECTED_MAP_SCALE != null)
                    {
                        Cursor = Cursors.Cross;
                        SELECTED_MAP_SCALE.IsSelected = false;
                        SELECTED_MAP_SCALE = null;
                        CURRENT_DRAWING_MODE = MapDrawingMode.None;
                        SetDrawingModeLabel();

                        CURRENT_MAP.IsSaved = false;
                    }
                    break;
                case MapDrawingMode.RealmAreaSelect:
                    {
                        SELECTED_REALM_AREA = new(PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

                        MapLayer workLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER);
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawRect((SKRect)SELECTED_REALM_AREA, PaintObjects.LandformAreaSelectPaint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.MapHeightIncrease:
                    {
                        // update the 3D view if it is open and updates are enabled
                        if (CurrentHeightMapView != null && Update3DViewSwitch.Checked)
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            try
                            {
                                SKBitmap? heightMapBitmap = GetBitmapForThreeDView();
                                if (heightMapBitmap != null)
                                {
                                    // generate the 3D model from the height information in the selected area
                                    List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                                    heightMapBitmap.Dispose();

                                    // convert the list of strings to a single string
                                    string objModelString = string.Join(Environment.NewLine, objModelStringList);

                                    CurrentHeightMapView.LoadModelFromString(objModelString);
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
                            Cursor.Current = Cursors.WaitCursor;

                            try
                            {
                                SKBitmap? heightMapBitmap = GetBitmapForThreeDView();
                                if (heightMapBitmap != null)
                                {
                                    // generate the 3D model from the height information in the selected area
                                    List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                                    heightMapBitmap.Dispose();

                                    // convert the list of strings to a single string
                                    string objModelString = string.Join(Environment.NewLine, objModelStringList);

                                    CurrentHeightMapView.LoadModelFromString(objModelString);
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

        private void RightButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case MapDrawingMode.LandformSelect:

                    LandformSelectButton.Checked = false;

                    SELECTED_LANDFORM = SelectLandformAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                    SKGLRenderControl.Invalidate();

                    if (SELECTED_LANDFORM != null)
                    {
                        LandformInfo landformInfo = new(CURRENT_MAP, SELECTED_LANDFORM, AssetManager.CURRENT_THEME, SKGLRenderControl);
                        landformInfo.ShowDialog(this);
                    }
                    break;
                case MapDrawingMode.WaterFeatureSelect:
                    MapComponent? selectedWaterFeature = SelectWaterFeatureAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                    SKGLRenderControl.Invalidate();

                    if (selectedWaterFeature != null && selectedWaterFeature is WaterFeature wf)
                    {
                        SELECTED_WATERFEATURE = (WaterFeature)selectedWaterFeature;

                        WaterFeatureInfo waterFeatureInfo = new(CURRENT_MAP, wf, SKGLRenderControl);
                        waterFeatureInfo.ShowDialog(this);
                    }
                    else if (selectedWaterFeature != null && selectedWaterFeature is River r)
                    {
                        SELECTED_WATERFEATURE = (River)selectedWaterFeature;

                        RiverInfo riverInfo = new(CURRENT_MAP, r, SKGLRenderControl);
                        riverInfo.ShowDialog(this);
                    }

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.PathSelect:
                    MapPath? selectedPath = SelectMapPathAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                    SKGLRenderControl.Invalidate();

                    if (selectedPath != null)
                    {
                        SELECTED_PATH = selectedPath;

                        MapPathInfo pathInfo = new(CURRENT_MAP, selectedPath, SKGLRenderControl);
                        pathInfo.ShowDialog(this);
                    }

                    SKGLRenderControl.Invalidate();
                    break;
                case MapDrawingMode.SymbolSelect:
                    MapSymbol? selectedSymbol = SelectMapSymbolAtPoint(CURRENT_MAP, zoomedScrolledPoint.ToDrawingPoint());
                    if (selectedSymbol != null)
                    {
                        SELECTED_MAP_SYMBOL = selectedSymbol;

                        MapSymbolInfo msi = new(CURRENT_MAP, selectedSymbol);
                        msi.ShowDialog(this);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case MapDrawingMode.RegionSelect:
                    MapRegion? selectedRegion = SelectRegionAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                    if (selectedRegion != null)
                    {
                        CURRENT_MAP_REGION = selectedRegion;

                        MapRegionInfo mri = new(CURRENT_MAP, selectedRegion, SKGLRenderControl);
                        mri.ShowDialog(this);

                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }
        }

        private void MiddleButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

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
                StopBrushTimer();

                // stop placing symbols
                StopSymbolAreaBrushTimer();

                // clear drawing mode and set brush size to 0
                CURRENT_DRAWING_MODE = MapDrawingMode.None;
                SetDrawingModeLabel();
                SetSelectedBrushSize(0);

                // dispose of any map label path
                CURRENT_MAP_LABEL_PATH.Dispose();
                CURRENT_MAP_LABEL_PATH = new();

                // clear the work layer
                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                // unselect anything selected
                DeselectAllMapComponents(null);

                // dispose of any label text box that is drawn
                if (LABEL_TEXT_BOX != null)
                {
                    SKGLRenderControl.Controls.Remove(LABEL_TEXT_BOX);
                    LABEL_TEXT_BOX.Dispose();
                }

                // clear all selections
                SELECTED_MAP_COMPONENTS.Clear();
                PREVIOUS_SELECTED_AREA = SKRect.Empty;
                SELECTED_REALM_AREA = SKRect.Empty;
                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

                FontSelectionPanel.Visible = false;

                // re-render everything
                SKGLRenderControl.Invalidate();
                Refresh();
            }

            if (e.KeyCode == Keys.Delete)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.LandformSelect:
                        if (SELECTED_LANDFORM != null)
                        {
                            Cmd_RemoveLandform cmd = new(CURRENT_MAP, SELECTED_LANDFORM);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            SELECTED_LANDFORM = null;
                            CURRENT_MAP.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                    case MapDrawingMode.WaterFeatureSelect:
                        // delete water features, rivers
                        if (SELECTED_WATERFEATURE != null)
                        {
                            Cmd_RemoveWaterFeature cmd = new(CURRENT_MAP, SELECTED_WATERFEATURE);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            SELECTED_WATERFEATURE = null;
                            SELECTED_RIVERPOINT = null;

                            CURRENT_MAP.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }

                        break;
                    case MapDrawingMode.RiverEdit:
                        {
                            if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river && SELECTED_RIVERPOINT != null)
                            {
                                Cmd_RemoveRiverPoint cmd = new(river, SELECTED_RIVERPOINT);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                SELECTED_RIVERPOINT = null;

                                CURRENT_MAP.IsSaved = false;

                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                    case MapDrawingMode.PathSelect:
                        if (SELECTED_PATH != null)
                        {
                            Cmd_RemoveMapPath cmd = new(CURRENT_MAP, SELECTED_PATH);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            SELECTED_PATH = null;
                            SELECTED_PATHPOINT = null;

                            CURRENT_MAP.IsSaved = false;
                            SKGLRenderControl.Invalidate();
                        }

                        break;
                    case MapDrawingMode.PathEdit:
                        {
                            if (SELECTED_PATH != null && SELECTED_PATHPOINT != null)
                            {
                                Cmd_RemovePathPoint cmd = new(SELECTED_PATH, SELECTED_PATHPOINT);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                SELECTED_PATHPOINT = null;

                                CURRENT_MAP.IsSaved = false;

                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (SELECTED_MAP_SYMBOL != null)
                            {
                                Cmd_RemoveSymbol cmd = new(CURRENT_MAP, SELECTED_MAP_SYMBOL);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                SELECTED_MAP_SYMBOL = null;
                                CURRENT_MAP.IsSaved = false;

                                SKGLRenderControl.Invalidate();
                            }
                        }

                        break;
                    case MapDrawingMode.LabelSelect:
                        {
                            if (SELECTED_MAP_LABEL != null)
                            {
                                Cmd_DeleteLabel cmd = new(CURRENT_MAP, SELECTED_MAP_LABEL);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                CURRENT_MAP.IsSaved = false;
                                SELECTED_MAP_LABEL = null;

                            }

                            if (SELECTED_PLACED_MAP_BOX != null)
                            {
                                Cmd_DeleteLabelBox cmd = new(CURRENT_MAP, SELECTED_PLACED_MAP_BOX);
                                CommandManager.AddCommand(cmd);
                                cmd.DoOperation();

                                CURRENT_MAP.IsSaved = false;
                                SELECTED_PLACED_MAP_BOX = null;
                            }

                            SKGLRenderControl.Invalidate();
                        }
                        break;
                    case MapDrawingMode.RegionSelect:
                        {
                            if (CURRENT_MAP_REGION != null)
                            {
                                bool pointSelected = false;

                                foreach (MapRegionPoint mrp in CURRENT_MAP_REGION.MapRegionPoints)
                                {
                                    if (mrp.IsSelected)
                                    {
                                        pointSelected = true;
                                        Cmd_DeleteMapRegionPoint cmd = new(CURRENT_MAP, CURRENT_MAP_REGION, mrp);
                                        CommandManager.AddCommand(cmd);
                                        cmd.DoOperation();

                                        break;
                                    }
                                }

                                if (!pointSelected)
                                {
                                    Cmd_DeleteMapRegion cmd = new(CURRENT_MAP, CURRENT_MAP_REGION);
                                    CommandManager.AddCommand(cmd);
                                    cmd.DoOperation();

                                    CURRENT_MAP_REGION = null;
                                }

                                CURRENT_MAP.IsSaved = false;

                                SKGLRenderControl.Invalidate();

                                return;
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.PageUp)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 5);
                            }
                            else if (ModifierKeys == Keys.None)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 1);
                            }
                        }
                        break;
                    case MapDrawingMode.RegionSelect:
                        {
                            MapRegionMethods.MoveSelectedRegionInRenderOrder(CURRENT_MAP, CURRENT_MAP_REGION, ComponentMoveDirection.Up);
                        }
                        break;
                }

                SKGLRenderControl.Invalidate();
            }

            if (e.KeyCode == Keys.PageDown)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 5);
                            }
                            else if (ModifierKeys == Keys.None)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 1);
                            }
                        }
                        break;
                    case MapDrawingMode.RegionSelect:
                        {
                            MapRegionMethods.MoveSelectedRegionInRenderOrder(CURRENT_MAP, CURRENT_MAP_REGION, ComponentMoveDirection.Down);
                        }
                        break;
                }

                SKGLRenderControl.Invalidate();
            }

            if (e.KeyCode == Keys.End)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            // move symbol to bottom of render order
                            MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Down, 1, true);
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Home)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            // move symbol to top of render order
                            MoveSelectedSymbolInRenderOrder(ComponentMoveDirection.Up, 1, true);
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (SELECTED_MAP_SYMBOL != null)
                            {
                                SELECTED_MAP_SYMBOL.Y = Math.Min(SELECTED_MAP_SYMBOL.Y + 1, CURRENT_MAP.MapHeight);
                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Up)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (SELECTED_MAP_SYMBOL != null)
                            {
                                SELECTED_MAP_SYMBOL.Y = Math.Max(0, SELECTED_MAP_SYMBOL.Y - 1);
                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Left)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (SELECTED_MAP_SYMBOL != null)
                            {
                                if (SELECTED_MAP_SYMBOL != null)
                                {
                                    SELECTED_MAP_SYMBOL.X = Math.Max(0, SELECTED_MAP_SYMBOL.X - 1);
                                    SKGLRenderControl.Invalidate();
                                }
                            }
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Right)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case MapDrawingMode.SymbolSelect:
                        {
                            if (SELECTED_MAP_SYMBOL != null)
                            {
                                SELECTED_MAP_SYMBOL.X = Math.Min(SELECTED_MAP_SYMBOL.X + 1, CURRENT_MAP.MapWidth);
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
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER);
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
            CURRENT_MAP.IsSaved = false;

            BackgroundMethods.ApplyBackgroundTexture(CURRENT_MAP,
                BackgroundMethods.GetSelectedBackgroundImage(),
                BackgroundTextureScaleTrack.Value / 100.0F,
                MirrorBackgroundSwitch.Checked);

            SKGLRenderControl.Invalidate();
        }

        private void ClearBackgroundButton_Click(object sender, EventArgs e)
        {
            CURRENT_MAP.IsSaved = false;
            BackgroundMethods.ClearBackgroundImage(CURRENT_MAP);
            SKGLRenderControl.Invalidate();
        }

        private void VignetteColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, VignetteColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                VignetteColorSelectionButton.BackColor = selectedColor;
                VignetteColorSelectionButton.Refresh();

                BackgroundMethods.ChangeVignetteColor(CURRENT_MAP, selectedColor);
                SKGLRenderControl.Invalidate();
            }
        }

        private void VignetteStrengthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(VignetteStrengthTrack.Value.ToString(), VignetteGroupBox, new Point(VignetteStrengthTrack.Right - 30, VignetteStrengthTrack.Top - 20), 2000);
            BackgroundMethods.ChangeVignetteStrength(CURRENT_MAP, VignetteStrengthTrack.Value);
            SKGLRenderControl.Invalidate();
        }

        private void OvalVignetteRadio_CheckedChanged(object sender, EventArgs e)
        {
            BackgroundMethods.ChangeVignetteShape(CURRENT_MAP, RectangleVignetteRadio.Checked);
            SKGLRenderControl.Invalidate();
        }

        private void RectangleVignetteRadio_CheckedChanged(object sender, EventArgs e)
        {
            BackgroundMethods.ChangeVignetteShape(CURRENT_MAP, RectangleVignetteRadio.Checked);
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
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANDRAWINGLAYER);

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
            CURRENT_MAP.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count < 1
                && OceanTextureBox.Image != null)
            {
                float scale = OceanScaleTextureTrack.Value / 100.0F;
                bool mirrorBackground = MirrorOceanTextureSwitch.Checked;
                Bitmap? textureBitmap = (Bitmap?)OceanTextureBox.Image;

                OceanMethods.ApplyOceanTexture(CURRENT_MAP, textureBitmap, scale, mirrorBackground);
            }

            SKGLRenderControl.Invalidate();
        }
        private void OceanRemoveTextureButton_Click(object sender, EventArgs e)
        {
            CURRENT_MAP.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerTexture = (MapImage)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER).MapLayerComponents.First();

                Cmd_ClearOceanTexture cmd = new(CURRENT_MAP, layerTexture);
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
            CURRENT_MAP.IsSaved = false;

            // get the user-selected ocean color
            Color fillColor = OceanColorSelectButton.BackColor;
            SKBitmap b = new(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

            using (SKCanvas canvas = new(b))
            {
                canvas.Clear(Extensions.ToSKColor(fillColor));
            }

            Cmd_SetOceanColor cmd = new(CURRENT_MAP, b);
            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            SKGLRenderControl.Invalidate();
        }

        private void OceanColorClearButton_Click(object sender, EventArgs e)
        {
            CURRENT_MAP.IsSaved = false;

            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTUREOVERLAYLAYER);

            if (oceanTextureOverlayLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerColor = (MapImage)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTUREOVERLAYLAYER).MapLayerComponents.First();

                Cmd_ClearOceanColor cmd = new(CURRENT_MAP, layerColor);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void OceanPaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.OceanPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(OceanMethods.OceanPaintBrushSize);
        }

        private void OceanColorEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.OceanErase;
            SetDrawingModeLabel();
            SetSelectedBrushSize(OceanMethods.OceanPaintEraserSize);
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
            SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.SoftBrush;
        }

        private void OceanHardBrushButton_Click(object sender, EventArgs e)
        {
            SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.HardBrush;
        }

        private void OceanBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMethods.OceanPaintBrushSize = OceanBrushSizeTrack.Value;
            TOOLTIP.Show(OceanMethods.OceanPaintBrushSize.ToString(), OceanToolPanel, new Point(OceanBrushSizeTrack.Right - 30, OceanBrushSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(OceanMethods.OceanPaintBrushSize);
        }

        private void OceanEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            OceanMethods.OceanPaintEraserSize = OceanEraserSizeTrack.Value;
            TOOLTIP.Show(OceanMethods.OceanPaintEraserSize.ToString(), OceanToolPanel, new Point(OceanEraserSizeTrack.Right - 30, OceanEraserSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(OceanMethods.OceanPaintEraserSize);
        }

        private void OceanBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((OceanBrushVelocityTrack.Value / 100.0F).ToString(), OceanToolPanel, new Point(OceanBrushVelocityTrack.Right - 30, OceanBrushVelocityTrack.Top - 20), 2000);
            BRUSH_VELOCITY = Math.Max(1, BASE_MILLIS_PER_PAINT_EVENT / (OceanBrushVelocityTrack.Value / 100.0));
        }

        private void BrushTimerEventHandler(Object? eventObject, EventArgs eventArgs)
        {
            Point cursorPoint = SKGLRenderControl.PointToClient(Cursor.Position);

            SKPoint zoomedScrolledPoint = new((cursorPoint.X / DrawingZoom) + DrawingPoint.X, (cursorPoint.Y / DrawingZoom) + DrawingPoint.Y);
            CURRENT_LAYER_PAINT_STROKE?.AddLayerPaintStrokePoint(zoomedScrolledPoint);

            SKGLRenderControl.Invalidate();

            SKGLRenderControl.Invalidate();
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
            CURRENT_DRAWING_MODE = MapDrawingMode.ColorSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);

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
            MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDCOASTLINELAYER);
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDDRAWINGLAYER);

            landCoastlineLayer.ShowLayer = ShowLandLayerSwitch.Checked;

            landformLayer.ShowLayer = ShowLandLayerSwitch.Checked;

            landDrawingLayer.ShowLayer = ShowLandLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void LandformSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LandformSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void LandformPaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LandPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        private void LandformEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LandErase;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformEraserSize);
        }

        private void LandBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformBrushSize = LandBrushSizeTrack.Value;
            TOOLTIP.Show(LandBrushSizeTrack.Value.ToString(), LandformValuesGroup, new Point(LandBrushSizeTrack.Right - 30, LandBrushSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
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
            if (CURRENT_LANDFORM != null)
            {
                CURRENT_LANDFORM.FillWithTexture = UseTextureForBackgroundCheck.Checked;
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
            SetSelectedBrushSize(LandformMethods.LandformEraserSize);

        }

        private void LandformFillButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.None;

            if (LandformTexturePreviewPicture.Image != null)
            {
                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

                if (landformLayer.MapLayerComponents.Count > 0)
                {
                    MessageBox.Show("Landforms have already been drawn. Please clear them before filling the map.", "Landforms Already Drawn", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    Landform landform = new();
                    SetLandformData(landform);

                    LandformMethods.FillMapWithLandForm(CURRENT_MAP, landform);

                    CURRENT_MAP.IsSaved = false;
                    SKGLRenderControl.Invalidate();
                }
            }
            else
            {
                MessageBox.Show("Please select a landform texture.", "Select Texture", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void LandformClearButton_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show("This action will clear all landform drawing and any drawn landforms.\nPlease confirm.", "Clear All?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            if (confirmResult != DialogResult.OK)
            {
                return;
            }

            CURRENT_DRAWING_MODE = MapDrawingMode.None;

            SetDrawingModeLabel();
            SetSelectedBrushSize(0);

            Cmd_ClearAllLandforms cmd = new(CURRENT_MAP);

            CommandManager.AddCommand(cmd);
            cmd.DoOperation();

            SKGLRenderControl.Invalidate();
        }

        private void LandColorButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LandColor;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformColorBrushSize);
        }

        private void LandColorEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LandColorErase;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformColorEraserBrushSize);
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
            CURRENT_DRAWING_MODE = MapDrawingMode.ColorSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);

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
            SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.SoftBrush;
        }

        private void LandHardBrushButton_Click(object sender, EventArgs e)
        {
            SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.HardBrush;
        }

        private void LandColorBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformColorBrushSize = LandColorBrushSizeTrack.Value;
            TOOLTIP.Show(LandformMethods.LandformColorBrushSize.ToString(), LandToolPanel, new Point(LandColorBrushSizeTrack.Right - 30, LandColorBrushSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(LandformMethods.LandformColorBrushSize);
        }

        private void LandBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((LandBrushVelocityTrack.Value / 100.0F).ToString(), LandToolPanel, new Point(LandBrushVelocityTrack.Right - 30, LandBrushVelocityTrack.Top - 20), 2000);
            BRUSH_VELOCITY = Math.Max(1, BASE_MILLIS_PER_PAINT_EVENT / (LandBrushVelocityTrack.Value / 100.0));
        }

        private void LandColorEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformColorEraserBrushSize = LandColorEraserSizeTrack.Value;
            TOOLTIP.Show(LandformMethods.LandformColorEraserBrushSize.ToString(), LandToolPanel, new Point(LandColorEraserSizeTrack.Right - 20, LandColorEraserSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(LandformMethods.LandformColorEraserBrushSize);
        }


        #region Landform Generation

        private void LandformGenerateButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.None;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();

            SKGLRenderControl.Invalidate();

            Cursor = Cursors.WaitCursor;

            GetSelectedLandformType();

            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);

            GenerateRandomLandform(CURRENT_MAP, SELECTED_REALM_AREA, SELECTED_LANDFORM_TYPE);

            SELECTED_REALM_AREA = SKRect.Empty;

            CURRENT_MAP.IsSaved = false;
            Cursor = Cursors.Default;

            SKGLRenderControl.Invalidate();
        }

        private void RegionMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            RegionMenuItem.Checked = true;
            SELECTED_LANDFORM_TYPE = GeneratedLandformType.Region;
        }

        private void ContinentMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            ContinentMenuItem.Checked = true;
            SELECTED_LANDFORM_TYPE = GeneratedLandformType.Continent;
        }

        private void IslandMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            IslandMenuItem.Checked = true;
            SELECTED_LANDFORM_TYPE = GeneratedLandformType.Island;
        }

        private void ArchipelagoMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            ArchipelagoMenuItem.Checked = true;
            SELECTED_LANDFORM_TYPE = GeneratedLandformType.Archipelago;
        }

        private void AtollMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            AtollMenuItem.Checked = true;
            SELECTED_LANDFORM_TYPE = GeneratedLandformType.Atoll;
        }

        private void WorldMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAllLandformTypeMenuItems();
            WorldMenuItem.Checked = true;
            SELECTED_LANDFORM_TYPE = GeneratedLandformType.World;
        }

        #endregion

        #region Landform HeightMap
        private void HeightUpButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.MapHeightIncrease;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        private void HeightDownButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.MapHeightDecrease;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        private void Show3DViewButton_Click(object sender, EventArgs e)
        {
            SKBitmap? heightMapBitmap = GetBitmapForThreeDView();
            if (heightMapBitmap != null)
            {
                // generate the 3D model from the height information in the selected area
                List<string> objModelStringList = HeightMapTo3DModel.GenerateOBJ(heightMapBitmap, Byte.MaxValue / 2.0F);

                heightMapBitmap.Dispose();

                // convert the list of strings to a single string
                string objModelString = string.Join(Environment.NewLine, objModelStringList);

                ThreeDView td = new("Height Map 3D View");
                CurrentHeightMapView = td;
                td.Show();
                td.LoadModelFromString(objModelString);
            }
        }

        private static SKBitmap? GetBitmapForThreeDView()
        {
            SKBitmap? heightMapBitmap = null;

            try
            {
                if (SELECTED_REALM_AREA != SKRect.Empty)
                {
                    SKBitmap extractedBitmap = ExtractRectFromHeightMap(SELECTED_REALM_AREA);
                    heightMapBitmap = extractedBitmap.Copy();
                }
                else if (SELECTED_LANDFORM != null)
                {
                    // extract a heightmap bitmap using the area of the selected landform bounds
                    // then create a 3D model from the bitmap and display it in the ThreeDView
                    Cursor.Current = Cursors.WaitCursor;

                    SELECTED_LANDFORM.ContourPath.GetTightBounds(out SKRect landformBounds);

                    // inflate the bounds by 20 pixels to give the height map some border
                    landformBounds.Inflate(20, 20);
                    landformBounds.Left = Math.Max(0, landformBounds.Left - 10);
                    landformBounds.Top = Math.Max(0, landformBounds.Top - 10);

                    SKBitmap extractedBitmap = ExtractRectFromHeightMap(landformBounds);

                    heightMapBitmap = extractedBitmap.Copy();
                }
                else
                {
                    // generate the 3D model from the entire map
                    using SKSurface s = SKSurface.Create(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
                    s.Canvas.Clear(SKColors.Black);

                    RealmMapMethods.RenderHeightMapToCanvas(CURRENT_MAP, s.Canvas, new SKPoint(0, 0));

                    heightMapBitmap = SKBitmap.FromImage(s.Snapshot()).Copy();
                }
            }
            catch { }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return heightMapBitmap;
        }

        private static SKBitmap ExtractRectFromHeightMap(SKRect extractRect)
        {
            using SKSurface s = SKSurface.Create(new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
            s.Canvas.Clear(SKColors.Black);

            RealmMapMethods.RenderHeightMapToCanvas(CURRENT_MAP, s.Canvas, new SKPoint(0, 0));

            SKBitmap heightMap = SKBitmap.FromImage(s.Snapshot());

            // extract the section of the heightmap within the bounding box from the heightmap
            SKBitmap extractedBitmap = new((int)extractRect.Width, (int)extractRect.Height);
            using SKCanvas canvas = new SKCanvas(extractedBitmap);

            canvas.Clear(SKColors.Black);

            canvas.DrawBitmap(heightMap, extractRect, new SKRect(0, 0, extractRect.Width, extractRect.Height));

            return extractedBitmap;
        }

        #endregion

        #endregion

        #region Landform Methods
        /******************************************************************************************************* 
        * LANDFORM METHODS
        *******************************************************************************************************/
        internal void GetSelectedLandformType()
        {
            if (RegionMenuItem.Checked)
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.Region;
            }
            else if (ContinentMenuItem.Checked)
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.Continent;
            }
            else if (IslandMenuItem.Checked)
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.Island;
            }
            else if (ArchipelagoMenuItem.Checked)
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.Archipelago;
            }
            else if (AtollMenuItem.Checked)
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.Atoll;
            }
            else if (WorldMenuItem.Checked)
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.World;
            }
            else
            {
                SELECTED_LANDFORM_TYPE = GeneratedLandformType.NotSet;
            }
        }

        internal void GenerateRandomLandform(RealmStudioMap map, SKRect selectedArea, GeneratedLandformType selectedLandformType)
        {
            SKPoint location = new(map.MapWidth / 2, map.MapHeight / 2);
            SKSize size = new(map.MapWidth / 2, map.MapHeight / 2);

            if (!selectedArea.IsEmpty)
            {
                location = new SKPoint(selectedArea.MidX, selectedArea.MidY);
                size = selectedArea.Size;
            }

            switch (selectedLandformType)
            {
                case GeneratedLandformType.Archipelago:
                    {
                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth * 0.8F, map.MapHeight * 0.8F);
                        }

                        CreateLandformFromGeneratedPaths(location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.Atoll:
                    {
                        CreateLandformFromGeneratedPaths(location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.Continent:
                    {
                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth * 0.8F, map.MapHeight * 0.8F);
                        }

                        CreateLandformFromGeneratedPaths(location, size, GeneratedLandformType.Island);

                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth - 4, map.MapHeight - 4);
                        }

                        CreateLandformFromGeneratedPaths(location, size, GeneratedLandformType.Archipelago);
                    }
                    break;
                case GeneratedLandformType.Island:
                    {
                        CreateLandformFromGeneratedPaths(location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.Region:
                    {
                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth - 4, map.MapHeight - 4);
                        }

                        CreateLandformFromGeneratedPaths(location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.World:
                    {
                        GenerateWorldLandforms(map, new SKSizeI(map.MapWidth, map.MapHeight));
                    }
                    break;
            }

            LandformMethods.MergeLandforms(map);

            map.IsSaved = false;
        }

        internal void GenerateWorldLandforms(RealmStudioMap map, SKSizeI size)
        {
            LoadingStatusForm progressForm = new();
            progressForm.Location = new Point(((Location.X + Width) / 2) - (progressForm.Width / 2), ((Location.Y + Height) / 2) - (progressForm.Height / 2));

            progressForm.Show(this);
            progressForm.Hide();

            progressForm.SetStatusText("Generating world landforms...");
            int progressPercentage = 0;

            progressForm.Show(this);

            bool hasNorthernIcecap = Random.Shared.Next(0, 2) == 1;
            bool hasSouthernIcecap = Random.Shared.Next(0, 2) == 1;

            SKSizeI northernIcecapSize = SKSizeI.Empty;
            SKSizeI southernIcecapSize = SKSizeI.Empty;

            // if the realm has a northern ice cap, there is a 75% chance it has a southern ice cap also
            if (hasNorthernIcecap && !hasSouthernIcecap)
            {
                hasSouthernIcecap = Random.Shared.NextDouble() > 0.25;
            }

            // if the realm has a southern ice cap, there is a 75% chance it has a northern ice cap also
            if (hasSouthernIcecap && !hasNorthernIcecap)
            {
                hasNorthernIcecap = Random.Shared.NextDouble() > 0.25;
            }

            if (hasNorthernIcecap)
            {
                progressPercentage += 2;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generating northern icecap");

                northernIcecapSize.Width = Random.Shared.Next((int)(map.MapWidth / 4.0), map.MapWidth - 8);
                northernIcecapSize.Height = Random.Shared.Next((int)(map.MapHeight / 8.0), (int)(map.MapWidth / 4.0));

                SKPoint northernIcecapLocation = new(map.MapWidth / 2, 4);

                CreateLandformFromGeneratedPaths(northernIcecapLocation, northernIcecapSize, GeneratedLandformType.Icecap, false);

                progressPercentage += 3;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generated northern icecap");

                SetStatusText("Generated northern icecap");
            }

            if (hasSouthernIcecap)
            {
                progressPercentage += 2;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generating southern icecap");

                southernIcecapSize.Width = Random.Shared.Next((int)(map.MapWidth / 4.0), map.MapWidth - 8);
                southernIcecapSize.Height = Random.Shared.Next((int)(map.MapHeight / 8.0), (int)(map.MapWidth / 4.0));

                SKPoint southernIcecapLocation = new(map.MapWidth / 2, map.MapHeight - southernIcecapSize.Height - 4);

                CreateLandformFromGeneratedPaths(southernIcecapLocation, southernIcecapSize, GeneratedLandformType.Icecap, true);

                progressPercentage += 3;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generated southern icecap");

                SetStatusText("Generated southern icecap");
            }

            int numDivisions = Random.Shared.Next(1, 8);  // max 7 subdivisions of the remaining map area for now

            progressPercentage += 1;
            progressForm.SetStatusPercentage(progressPercentage);
            progressForm.SetStatusText("Creating " + numDivisions + " landforms");

            SetStatusText("Creating " + numDivisions + " landforms");

            SKRectI remainingMapRect = new(0, 0, size.Width, size.Height);

            if (hasNorthernIcecap)
            {
                remainingMapRect.Top = northernIcecapSize.Height;
            }

            if (hasSouthernIcecap)
            {
                remainingMapRect.Bottom = map.MapHeight - southernIcecapSize.Height;
            }

            List<SKRectI> continentRects = [];

            if (numDivisions == 1)
            {
                continentRects.Add(remainingMapRect);
            }
            else
            {
                Debug.Assert(remainingMapRect.Top < remainingMapRect.Bottom);

                int minRectSize = 300;

                if (remainingMapRect.Width < remainingMapRect.Height)
                {
                    minRectSize = (remainingMapRect.Width / numDivisions) - Random.Shared.Next(10, 50);
                }
                else if (remainingMapRect.Height < remainingMapRect.Width)
                {
                    minRectSize = (remainingMapRect.Height / numDivisions) - Random.Shared.Next(10, 50);
                }

                List<SKRectI> rects = DrawingMethods.SubdivideRect2(remainingMapRect, numDivisions, minRectSize);

                int tries = 0;
                while (rects.Count < numDivisions && tries < 10)
                {
                    tries++;
                    minRectSize -= 20;

                    minRectSize = Math.Max(50, minRectSize);

                    rects = DrawingMethods.SubdivideRect2(remainingMapRect, numDivisions, minRectSize);
                }

                if (rects.Count == 0)
                {
                    SetStatusText("Map rectangle subdivision failed");
                    progressForm.Hide();
                    progressForm.Dispose();

                    return;
                }

                continentRects.AddRange(rects);

                progressPercentage += 1;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Landform rectangles calculated");

                SetStatusText("Landform rectangles calculated");
            }

            int landformCount = 1;

            int progressStep = (100 - progressPercentage) / continentRects.Count;

            foreach (SKRectI rect in continentRects)
            {
                Debug.Assert(rect.Width > 0 && rect.Height > 0 && rect.Width < int.MaxValue && rect.Height < int.MaxValue);

                progressForm.SetStatusText("Generating landform " + landformCount);

                SetStatusText("Generating landform " + landformCount);

                bool success = false;
                int tries = 0;

                while (!success && tries < 10)
                {
                    tries++;

                    double landformTypeRandom = Random.Shared.NextDouble();
                    GeneratedLandformType generateLandformType = GeneratedLandformType.Continent;

                    if (landformTypeRandom >= 0.0 && landformTypeRandom < 0.5)
                    {
                        // 50% chance of generating continent
                        generateLandformType = GeneratedLandformType.Continent;
                    }
                    else if (landformTypeRandom >= 0.5 && landformTypeRandom < 0.75)
                    {
                        // 25% chance of generating island
                        generateLandformType = GeneratedLandformType.Island;
                    }
                    else if (landformTypeRandom >= 0.75 && landformTypeRandom < 0.90)
                    {
                        // 15% chance of generating archipelago
                        generateLandformType = GeneratedLandformType.Archipelago;
                    }
                    else
                    {
                        // 10% chance of generating atoll
                        generateLandformType = GeneratedLandformType.Atoll;
                    }

                    success = CreateLandformFromGeneratedPaths(new SKPoint(rect.MidX, rect.MidY), new SKSize(rect.Width, rect.Height), generateLandformType, false);
                }

                if (success)
                {
                    progressPercentage += progressStep;
                    progressForm.SetStatusPercentage(progressPercentage);
                    progressForm.SetStatusText("Landform " + landformCount + " generated");

                    SetStatusText("Landform " + landformCount + " generated");
                }
                else
                {
                    SetStatusText("Landform creation failed");
                }

                landformCount++;
            }

            progressForm.Hide();
            progressForm.Dispose();

        }

        private bool CreateLandformFromGeneratedPaths(SKPoint location, SKSize size, GeneratedLandformType selectedLandformType, bool flipVertical = false)
        {
            List<SKPath> generatedLandformPaths = LandformMethods.GenerateRandomLandformPaths(location, size, selectedLandformType, flipVertical);

            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

            bool success = false;

            foreach (SKPath path in generatedLandformPaths)
            {
                if (!path.IsEmpty)
                {
                    Landform landform = new()
                    {
                        ParentMap = CURRENT_MAP,
                        IsModified = true,
                        DrawPath = new(path),

                        LandformRenderSurface =
                        SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight)),

                        CoastlineRenderSurface =
                        SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight))
                    };

                    LandformMethods.CreateAllPathsFromDrawnPath(CURRENT_MAP, landform);

                    landform.ContourPath.GetBounds(out SKRect boundsRect);
                    landform.X = (int)location.X;
                    landform.Y = (int)location.Y;
                    landform.Width = (int)boundsRect.Width;
                    landform.Height = (int)boundsRect.Height;

                    SetLandformData(landform);

                    landformLayer.MapLayerComponents.Add(landform);

                    success = true;
                }
            }

            return success;
        }

        private void SetLandformData(Landform landform)
        {
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
                Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

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

        internal static Landform? GetLandformIntersectingCircle(RealmStudioMap map, SKPoint mapPoint, int circleRadius)
        {
            using SKPath circlePath = new();
            circlePath.AddCircle(mapPoint.X, mapPoint.Y, circleRadius);

            List<MapComponent> landformComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformComponents.Count; i++)
            {
                if (landformComponents[i] is Landform mapLandform)
                {
                    if (mapLandform.DrawPath != null && mapLandform.DrawPath.PointCount > 0)
                    {
                        if (!mapLandform.DrawPath.Op(circlePath, SKPathOp.Intersect).IsEmpty)
                        {
                            return mapLandform;
                        }
                    }
                }
            }

            return null;
        }

        internal static Landform? SelectLandformAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
        {
            Landform? selectedLandform = null;

            List<MapComponent> landformComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformComponents.Count; i++)
            {
                if (landformComponents[i] is Landform mapLandform)
                {
                    SKPath boundaryPath = mapLandform.DrawPath;

                    if (boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            mapLandform.IsSelected = !mapLandform.IsSelected;

                            if (mapLandform.IsSelected)
                            {
                                selectedLandform = mapLandform;
                            }
                            break;
                        }
                    }
                }
            }

            DeselectAllMapComponents(selectedLandform);
            return selectedLandform;
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
            if (CURRENT_DRAWING_MODE != MapDrawingMode.PlaceWindrose)
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.PlaceWindrose;
                SetDrawingModeLabel();

                CURRENT_WINDROSE = CreateWindrose();
            }
            else
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.None;
                SetDrawingModeLabel();

                CURRENT_WINDROSE = null;
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
            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER).MapLayerComponents[i] is MapWindrose)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER).MapLayerComponents.RemoveAt(i);
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
                ParentMap = CURRENT_MAP,
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
            if (CURRENT_WINDROSE != null)
            {
                CURRENT_WINDROSE.InnerCircles = WindroseInnerCircleTrack.Value;
                CURRENT_WINDROSE.InnerRadius = (int)WindroseInnerRadiusUpDown.Value;
                CURRENT_WINDROSE.FadeOut = WindroseFadeOutSwitch.Checked;
                CURRENT_WINDROSE.LineWidth = (int)WindroseLineWidthUpDown.Value;
                CURRENT_WINDROSE.OuterRadius = (int)WindroseOuterRadiusUpDown.Value;
                CURRENT_WINDROSE.WindroseColor = WindroseColorSelectButton.BackColor;
                CURRENT_WINDROSE.DirectionCount = (int)WindroseDirectionsUpDown.Value;

                CURRENT_WINDROSE.WindrosePaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = CURRENT_WINDROSE.LineWidth,
                    Color = CURRENT_WINDROSE.WindroseColor.ToSKColor(),
                    IsAntialias = true,
                };
            }
        }

        #endregion

        #region Water Tab Event Handlers

        private void ShowWaterLayerSwitch_CheckedChanged()
        {
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER);
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERDRAWINGLAYER);

            waterLayer.ShowLayer = ShowWaterLayerSwitch.Checked;

            waterDrawingLayer.ShowLayer = ShowWaterLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void WaterFeatureSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.WaterFeatureSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void WaterFeaturePaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.WaterPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureBrushSize);
        }

        private void WaterFeatureLakeButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LakePaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureBrushSize);
        }

        private void WaterFeatureRiverButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.RiverPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void WaterFeatureEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.WaterErase;
            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureEraserSize);
        }

        private void WaterBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterFeatureBrushSize = WaterBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureBrushSize.ToString(), WaterValuesGroup, new Point(WaterBrushSizeTrack.Right - 30, WaterBrushSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureBrushSize);
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
            if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river)
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
                CURRENT_DRAWING_MODE = MapDrawingMode.RiverEdit;
                SetDrawingModeLabel();
                SetSelectedBrushSize(0);

                if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river)
                {
                    river.ShowRiverPoints = true;
                }
            }
            else
            {
                if (SELECTED_WATERFEATURE != null && SELECTED_WATERFEATURE is River river)
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
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureEraserSize);
        }

        private void WaterSoftBrushButton_Click(object sender, EventArgs e)
        {
            SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.SoftBrush;
        }

        private void WaterHardBrushButton_Click(object sender, EventArgs e)
        {
            SELECTED_COLOR_PAINT_BRUSH = ColorPaintBrush.HardBrush;
        }

        private void WaterColorBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterColorBrushSize = WaterColorBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterColorBrushSize.ToString(), WaterToolPanel, new Point(WaterColorBrushSizeTrack.Right - 30, WaterColorBrushSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(WaterFeatureMethods.WaterColorBrushSize);
        }

        private void WaterBrushVelocityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((WaterBrushVelocityTrack.Value / 100.0F).ToString(), WaterToolPanel, new Point(WaterBrushVelocityTrack.Right - 30, WaterBrushVelocityTrack.Top - 20), 2000);
            BRUSH_VELOCITY = Math.Max(1, BASE_MILLIS_PER_PAINT_EVENT / (WaterBrushVelocityTrack.Value / 100.0));
        }

        private void WaterColorEraserSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterColorEraserSize = WaterColorEraserSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterColorEraserSize.ToString(), WaterToolPanel, new Point(WaterColorEraserSizeTrack.Right - 30, WaterColorEraserSizeTrack.Top - 20), 2000);
            SetSelectedBrushSize(WaterFeatureMethods.WaterColorEraserSize);
        }

        private void WaterColorButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.WaterColor;

            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterColorBrushSize);
        }

        private void WaterColorEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.WaterColorErase;

            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterColorEraserSize);
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

            DeselectAllMapComponents(selectedWaterFeature);
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
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);

            pathLowerLayer.ShowLayer = ShowPathLayerSwitch.Checked;

            pathUpperLayer.ShowLayer = ShowPathLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void PathSelectButton_Click(object sender, EventArgs e)
        {
            if (CURRENT_DRAWING_MODE != MapDrawingMode.PathSelect && CURRENT_DRAWING_MODE != MapDrawingMode.PathEdit)
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.PathSelect;
                SetSelectedBrushSize(0);
            }
            else
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.None;
                SetSelectedBrushSize(0);
            }

            if (CURRENT_DRAWING_MODE == MapDrawingMode.PathSelect)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
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
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else if (CURRENT_DRAWING_MODE == MapDrawingMode.PathEdit)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
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
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else
            {
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }
            }

            if (CURRENT_DRAWING_MODE == MapDrawingMode.None)
            {
                SELECTED_PATH = null;
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }
            }

            SetDrawingModeLabel();

            SKGLRenderControl.Invalidate();
        }

        private void DrawPathButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
            CURRENT_DRAWING_MODE = MapDrawingMode.PathPaint;
            SetSelectedBrushSize(0);

            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
            foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }

            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
            foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
            {
                mp.IsSelected = false;
                mp.ShowPathPoints = false;
            }

            SetDrawingModeLabel();

            SKGLRenderControl.Invalidate();
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
            SetSelectedBrushSize(0);

            if (CURRENT_DRAWING_MODE == MapDrawingMode.PathSelect)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
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
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else if (CURRENT_DRAWING_MODE == MapDrawingMode.PathEdit)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathEdit;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        if (mp.IsSelected)
                        {
                            mp.ShowPathPoints = true;
                        }
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
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
                    CURRENT_DRAWING_MODE = MapDrawingMode.PathSelect;

                    MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                    foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }

                    MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                    foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                    {
                        mp.ShowPathPoints = false;
                    }
                }
            }
            else
            {
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.ShowPathPoints = false;
                }
            }

            if (CURRENT_DRAWING_MODE == MapDrawingMode.None)
            {
                SELECTED_PATH = null;
                MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }

                MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.IsSelected = false;
                    mp.ShowPathPoints = false;
                }
            }

            SetDrawingModeLabel();

            SKGLRenderControl.Invalidate();
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

            DeselectAllMapComponents(selectedMapPath);
            return selectedMapPath;
        }
        #endregion

        #region Symbol Tab Event Handlers

        private void ShowSymbolLayerSwitch_CheckedChanged()
        {
            MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.SYMBOLLAYER);

            symbolLayer.ShowLayer = ShowSymbolLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void SymbolSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.SymbolSelect;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();
        }

        private void EraseSymbolsButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.SymbolErase;
            SetSelectedBrushSize(AreaBrushSizeTrack.Value);
            SetDrawingModeLabel();
        }

        private void ColorSymbolsButton_Click(object sender, EventArgs e)
        {
            if (SELECTED_MAP_SYMBOL != null && SELECTED_MAP_SYMBOL.IsSelected)
            {
                // if a symbol has been selected and is grayscale or custom colored, then color it with the
                // selected custom colors

                SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
                SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
                SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

                if (SELECTED_MAP_SYMBOL.IsGrayscale || SELECTED_MAP_SYMBOL.UseCustomColors)
                {
                    Cmd_PaintSymbol cmd = new(SELECTED_MAP_SYMBOL, paintColor1, paintColor1, paintColor2, paintColor3);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    SKGLRenderControl.Invalidate();
                }

                SELECTED_MAP_SYMBOL.IsSelected = false;
                SELECTED_MAP_SYMBOL = null;
            }
            else
            {
                CURRENT_DRAWING_MODE = MapDrawingMode.SymbolColor;
                SetDrawingModeLabel();

                if (AreaBrushSwitch.Checked)
                {
                    SetSelectedBrushSize(AreaBrushSizeTrack.Value);
                }
                else
                {
                    SetSelectedBrushSize(0);
                }
            }
        }

        private void StructuresSymbolButton_Click(object sender, EventArgs e)
        {
            SelectSymbolsOfType(MapSymbolType.Structure);
        }

        private void VegetationSymbolsButton_Click(object sender, EventArgs e)
        {
            SelectSymbolsOfType(MapSymbolType.Vegetation);
        }

        private void TerrainSymbolsButton_Click(object sender, EventArgs e)
        {
            SelectSymbolsOfType(MapSymbolType.Terrain);
        }

        private void MarkerSymbolsButton_Click(object sender, EventArgs e)
        {
            SelectSymbolsOfType(MapSymbolType.Marker);
        }

        private void OtherSymbolsButton_Click(object sender, EventArgs e)
        {
            SelectSymbolsOfType(MapSymbolType.Other);
        }

        private void SymbolScaleTrack_Scroll(object sender, EventArgs e)
        {
            if (!SYMBOL_SCALE_LOCKED)
            {
                TOOLTIP.Show(SymbolScaleTrack.Value.ToString(), SymbolScaleGroup, new Point(SymbolScaleTrack.Right - 30, SymbolScaleTrack.Top - 20), 2000);

                SymbolScaleUpDown.Value = SymbolScaleTrack.Value;
                SymbolScaleUpDown.Refresh();
            }
        }

        private void SymbolScaleUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!SYMBOL_SCALE_LOCKED)
            {
                SymbolScaleTrack.Value = (int)SymbolScaleUpDown.Value;
                SymbolScaleTrack.Refresh();
            }
        }

        private void LockSymbolScaleButton_Click(object sender, EventArgs e)
        {
            SYMBOL_SCALE_LOCKED = !SYMBOL_SCALE_LOCKED;

            if (SYMBOL_SCALE_LOCKED)
            {
                LockSymbolScaleButton.IconChar = FontAwesome.Sharp.IconChar.Lock;
                SymbolScaleTrack.Enabled = false;
                SymbolScaleUpDown.Enabled = false;
            }
            else
            {
                LockSymbolScaleButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
                SymbolScaleTrack.Enabled = true;
                SymbolScaleUpDown.Enabled = true;
            }

            LockSymbolScaleButton.Refresh();
            SymbolScaleTrack.Refresh();
            SymbolScaleUpDown.Refresh();
        }

        private void ResetSymbolColorsButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.CURRENT_THEME != null && AssetManager.CURRENT_THEME.SymbolCustomColors != null)
            {
                SymbolColor1Button.BackColor = Color.FromArgb(AssetManager.CURRENT_THEME.SymbolCustomColors[0] ?? Color.FromArgb(85, 44, 36).ToArgb());
                SymbolColor2Button.BackColor = Color.FromArgb(AssetManager.CURRENT_THEME.SymbolCustomColors[1] ?? Color.FromArgb(255, 53, 45, 32).ToArgb());
                SymbolColor3Button.BackColor = Color.FromArgb(AssetManager.CURRENT_THEME.SymbolCustomColors[2] ?? Color.FromArgb(161, 214, 202, 171).ToArgb());
            }
            else
            {
                SymbolColor1Button.BackColor = Color.FromArgb(255, 85, 44, 36);
                SymbolColor2Button.BackColor = Color.FromArgb(255, 53, 45, 32);
                SymbolColor3Button.BackColor = Color.FromArgb(161, 214, 202, 171);
            }

            SymbolColor1Button.Refresh();
            SymbolColor2Button.Refresh();
            SymbolColor3Button.Refresh();
        }

        private void SymbolColor1Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor1Button.BackColor);

                SymbolColor1Button.BackColor = c;
                SymbolColor1Button.Refresh();

                List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
                AddSymbolsToSymbolTable(selectedSymbols);
            }
            else if (e.Button == MouseButtons.Right && SELECTED_MAP_SYMBOL != null)
            {
                if (SELECTED_MAP_SYMBOL.IsGrayscale || SELECTED_MAP_SYMBOL.UseCustomColors)
                {
                    // if a symbol has been selected and is grayscale or custom colored, then color it with the
                    // selected custom colors

                    SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
                    SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
                    SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

                    Cmd_PaintSymbol cmd = new(SELECTED_MAP_SYMBOL, paintColor1, paintColor1, paintColor2, paintColor3);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    SKGLRenderControl.Invalidate();
                }
            }
        }

        private void SymbolColor2Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor2Button.BackColor);

                SymbolColor2Button.BackColor = c;
                SymbolColor2Button.Refresh();

                List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
                AddSymbolsToSymbolTable(selectedSymbols);
            }
            else if (e.Button == MouseButtons.Right && SELECTED_MAP_SYMBOL != null)
            {
                if (SELECTED_MAP_SYMBOL.IsGrayscale || SELECTED_MAP_SYMBOL.UseCustomColors)
                {
                    // if a symbol has been selected and is grayscale or custom colored, then color it with the
                    // selected custom colors

                    SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
                    SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
                    SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

                    Cmd_PaintSymbol cmd = new(SELECTED_MAP_SYMBOL, paintColor2, paintColor1, paintColor2, paintColor3);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    SKGLRenderControl.Invalidate();
                }
            }
        }

        private void SymbolColor3Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor3Button.BackColor);

                SymbolColor3Button.BackColor = c;
                SymbolColor3Button.Refresh();

                List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
                AddSymbolsToSymbolTable(selectedSymbols);
            }
            else if (e.Button == MouseButtons.Right && SELECTED_MAP_SYMBOL != null)
            {
                if (SELECTED_MAP_SYMBOL.IsGrayscale || SELECTED_MAP_SYMBOL.UseCustomColors)
                {
                    // if a symbol has been selected and is grayscale or custom colored, then color it with the
                    // selected custom colors

                    SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
                    SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
                    SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

                    Cmd_PaintSymbol cmd = new(SELECTED_MAP_SYMBOL, paintColor3, paintColor1, paintColor2, paintColor3);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    SKGLRenderControl.Invalidate();
                }
            }
        }

        private void AreaBrushSwitch_CheckedChanged()
        {
            if (AreaBrushSwitch.Checked)
            {
                SetSelectedBrushSize(AreaBrushSizeTrack.Value);
            }
            else
            {
                SetSelectedBrushSize(0);
            }
        }

        private void AreaBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(AreaBrushSizeTrack.Value.ToString(), SymbolPlacementGroup, new Point(AreaBrushSizeTrack.Right - 30, AreaBrushSizeTrack.Top - 20), 2000);

            SELECTED_BRUSH_SIZE = AreaBrushSizeTrack.Value;
            SKGLRenderControl.Invalidate();
        }

        private void SymbolRotationTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(SymbolRotationTrack.Value.ToString(), SymbolPlacementGroup, new Point(SymbolRotationTrack.Right - 30, SymbolRotationTrack.Top - 20), 2000);
            SymbolRotationUpDown.Value = SymbolRotationTrack.Value;
        }

        private void SymbolRotationUpDown_ValueChanged(object sender, EventArgs e)
        {
            SymbolRotationTrack.Value = (int)SymbolRotationUpDown.Value;
        }

        private void SymbolPlacementRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            PLACEMENT_RATE = (float)SymbolPlacementRateUpDown.Value;
        }

        private void SymbolPlacementDensityUpDown_ValueChanged(object sender, EventArgs e)
        {
            PLACEMENT_DENSITY = (float)SymbolPlacementDensityUpDown.Value;
        }

        private void ResetSymbolPlacementRateButton_Click(object sender, EventArgs e)
        {
            SymbolPlacementRateUpDown.Value = 1.0M;
            SymbolPlacementRateUpDown.Refresh();

            PLACEMENT_RATE = (float)SymbolPlacementRateUpDown.Value;
        }

        private void ResetSymbolPlacementDensityButton_Click(object sender, EventArgs e)
        {
            SymbolPlacementDensityUpDown.Value = 1.0M;
            SymbolPlacementDensityUpDown.Refresh();

            PLACEMENT_DENSITY = (float)SymbolPlacementDensityUpDown.Value;
        }

        private void SymbolCollectionsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            List<string> checkedCollections = [];
            foreach (var item in SymbolCollectionsListBox.CheckedItems)
            {
                checkedCollections.Add(item.ToString());
            }

            if (e.NewValue == CheckState.Checked)
            {
                checkedCollections.Add(SymbolCollectionsListBox.Items[e.Index].ToString());
            }
            else
            {
                checkedCollections.Remove(SymbolCollectionsListBox.Items[e.Index].ToString());
            }

#pragma warning restore CS8604 // Possible null reference argument.
            List<string> selectedTags = [.. SymbolTagsListBox.CheckedItems.Cast<string>()];
            List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SymbolMethods.SELECTED_SYMBOL_TYPE, checkedCollections, selectedTags);
            AddSymbolsToSymbolTable(filteredSymbols);
        }

        private void SymbolTagsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            List<string> checkedTags = [];
            foreach (var item in SymbolTagsListBox.CheckedItems)
            {
                checkedTags.Add(item.ToString());
            }

            if (e.NewValue == CheckState.Checked)
            {
                checkedTags.Add(SymbolTagsListBox.Items[e.Index].ToString());
            }
            else
            {
                checkedTags.Remove(SymbolTagsListBox.Items[e.Index].ToString());
            }

#pragma warning restore CS8604 // Possible null reference argument.
            List<string> selectedCollections = [.. SymbolCollectionsListBox.CheckedItems.Cast<string>()];
            List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SymbolMethods.SELECTED_SYMBOL_TYPE, selectedCollections, checkedTags);
            AddSymbolsToSymbolTable(filteredSymbols);
        }

        private void SymbolSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // filter symbol list based on text entered by the user

            if (SymbolSearchTextBox.Text.Length > 2)
            {
                List<string> selectedCollections = [.. SymbolCollectionsListBox.CheckedItems.Cast<string>()];
                List<string> selectedTags = [.. SymbolTagsListBox.CheckedItems.Cast<string>()];
                List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SymbolMethods.SELECTED_SYMBOL_TYPE, selectedCollections, selectedTags, SymbolSearchTextBox.Text);

                AddSymbolsToSymbolTable(filteredSymbols);
            }
            else if (SymbolSearchTextBox.Text.Length == 0)
            {
                List<string> selectedCollections = [.. SymbolCollectionsListBox.CheckedItems.Cast<string>()];
                List<string> selectedTags = [.. SymbolTagsListBox.CheckedItems.Cast<string>()];
                List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SymbolMethods.SELECTED_SYMBOL_TYPE, selectedCollections, selectedTags);

                AddSymbolsToSymbolTable(filteredSymbols);
            }
        }

        private void SymbolAreaBrushTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            float symbolScale = SymbolScaleTrack.Value / 100.0F;
            float symbolRotation = SymbolRotationTrack.Value;
            SELECTED_BRUSH_SIZE = AreaBrushSizeTrack.Value;

            PlaceSelectedSymbolInArea(CURSOR_POINT, symbolScale, symbolRotation, (int)(AreaBrushSizeTrack.Value / 2.0F));
        }

        #endregion

        #region Symbol Tab Methods

        private void SelectSymbolsOfType(MapSymbolType symbolType)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.SymbolPlace;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);

            if (SymbolMethods.SELECTED_SYMBOL_TYPE != symbolType)
            {
                SymbolMethods.SELECTED_SYMBOL_TYPE = symbolType;
                List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();

                AddSymbolsToSymbolTable(selectedSymbols);
                AreaBrushSwitch.Checked = false;
                AreaBrushSwitch.Enabled = false;
            }

            if (SymbolMethods.SELECTED_SYMBOL_TYPE == MapSymbolType.Vegetation || SymbolMethods.SELECTED_SYMBOL_TYPE == MapSymbolType.Terrain)
            {
                AreaBrushSwitch.Enabled = true;
            }

            if (SymbolTable.Controls.Count > 0)
            {
                if (SymbolMethods.SelectedSymbolTableMapSymbol == null || SymbolMethods.SelectedSymbolTableMapSymbol.SymbolType != symbolType)
                {
                    PictureBox pb = (PictureBox)SymbolTable.Controls[0];
                    SelectPrimarySymbolInSymbolTable(pb);
                }
            }
        }

        private List<MapSymbol> GetFilteredMapSymbols()
        {
            List<string> selectedCollections = [.. SymbolCollectionsListBox.CheckedItems.Cast<string>()];
            List<string> selectedTags = [.. SymbolTagsListBox.CheckedItems.Cast<string>()];
            List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SymbolMethods.SELECTED_SYMBOL_TYPE, selectedCollections, selectedTags);

            return filteredSymbols;
        }

        private void AddSymbolsToSymbolTable(List<MapSymbol> symbols)
        {
            const int PBWIDTH = 120;
            const int PBHEIGHT = 45;
            int PBBASEHEIGHT = 680;

            SymbolTable.AutoScroll = false;
            SymbolTable.VerticalScroll.Enabled = true;
            SymbolTable.Hide();
            SymbolToolPanel.Refresh();
            SymbolTable.Controls.Clear();
            SymbolTable.RowCount = 0;
            SymbolTable.Refresh();

            Bitmap pbm = new(PBWIDTH - 2, PBHEIGHT - 8);

            for (int i = 0; i < symbols.Count; i++)
            {
                MapSymbol symbol = symbols[i];

                if (symbol.SymbolFormat != SymbolFileFormat.Vector)
                {
                    symbol.ColorMappedBitmap = symbol.SymbolBitmap?.Copy();

                    Bitmap colorMappedBitmap = (Bitmap)Extensions.ToBitmap(symbol.ColorMappedBitmap).Clone();

                    if (colorMappedBitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    {
                        Bitmap clone = new(colorMappedBitmap.Width, colorMappedBitmap.Height, PixelFormat.Format32bppPArgb);

                        using Graphics gr = Graphics.FromImage(clone);
                        gr.DrawImage(colorMappedBitmap, new Rectangle(0, 0, clone.Width, clone.Height));
                        colorMappedBitmap = new(clone);
                        clone.Dispose();
                    }

                    if (symbol.UseCustomColors)
                    {
                        SymbolMethods.MapCustomColorsToColorableBitmap(ref colorMappedBitmap, SymbolColor1Button.BackColor, SymbolColor2Button.BackColor, SymbolColor3Button.BackColor);
                    }
                    else if (symbol.IsGrayscale)
                    {
                        // TODO: color the grayscale with custom color (using SymbolColor1Button background color)?
                    }

                    symbol.ColorMappedBitmap = Extensions.ToSKBitmap((Bitmap)colorMappedBitmap.Clone());

                    pbm = DrawingMethods.ScaleBitmap(colorMappedBitmap, PBWIDTH - 2, PBHEIGHT - 8);
                    colorMappedBitmap.Dispose();
                }
                else
                {
                    // display the SVG
                    using MemoryStream ms = new(Encoding.ASCII.GetBytes(symbol.SymbolSVG));
                    var skSvg = new SKSvg();
                    skSvg.Load(ms);

                    using SKBitmap b = new(new SKImageInfo(PBHEIGHT - 8, PBHEIGHT - 8));
                    using SKCanvas c = new(b);

                    /*
                    using (var paint = new SKPaint())
                    {
                        paint.IsAntialias = true;
                        

                        //could also tint the svg:
                        //paint.ColorFilter = SKColorFilter.CreateBlendMode(TintColor.ToSKColor(), SKBlendMode.SrcIn);

                    }
                    */

                    if (skSvg.Picture != null)
                    {
                        SKMatrix matrix = SKMatrix.CreateScale((PBHEIGHT - 8) / skSvg.Picture.CullRect.Width,
                            (PBHEIGHT - 8) / skSvg.Picture.CullRect.Height);

                        c.DrawPicture(skSvg.Picture, in matrix);

                        pbm = Extensions.ToBitmap(b.Copy());
                    }
                }

                PictureBox pb = new()
                {
                    Width = PBWIDTH,
                    Height = PBHEIGHT,
                    Tag = symbol,
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    Image = pbm,
                    Margin = new Padding(0, 0, 0, 0),
                    Padding = new Padding(0, 4, 0, 4),
                    BorderStyle = BorderStyle.None,
                };

                pb.MouseHover += SymbolPictureBox_MouseHover;
                pb.MouseClick += SymbolPictureBox_MouseClick;
                pb.Paint += SymbolPictureBox_Paint;

                SymbolTable.Controls.Add(pb);
                SymbolTable.RowStyles.Add(new RowStyle(SizeType.Absolute, PBHEIGHT));
            }

            SymbolTable.RowCount = symbols.Count;
            SymbolTable.Width = 130;

            SymbolTable.Height = Math.Min(PBBASEHEIGHT, (symbols.Count * PBHEIGHT));
            SymbolTable.VerticalScroll.Maximum = (symbols.Count * PBHEIGHT) + (symbols.Count * 2);
            SymbolTable.HorizontalScroll.Maximum = 0;
            SymbolTable.HorizontalScroll.Enabled = false;
            SymbolTable.HorizontalScroll.Visible = false;
            SymbolTable.AutoScroll = true;

            SymbolTable.Show();

            SymbolTable.Refresh();
            SymbolToolPanel.Refresh();
        }

        private void SymbolPictureBox_Paint(object? sender, PaintEventArgs e)
        {
            PictureBox? pb = (PictureBox?)sender;
            if (pb != null)
            {
                ControlPaint.DrawBorder(e.Graphics, pb.ClientRectangle, Color.LightGray, ButtonBorderStyle.Dashed);
            }
        }

        private void SymbolPictureBox_MouseHover(object? sender, EventArgs e)
        {
            PictureBox? pb = (PictureBox?)sender;

            if (pb != null && pb.Tag is MapSymbol s)
            {
                TOOLTIP.Show(s.SymbolName, pb);
            }
        }

        private void SymbolPictureBox_MouseClick(object? sender, EventArgs e)
        {
            PictureBox? pb = (PictureBox?)sender;
            if (pb == null) return;

            MapSymbol? clickedSymbol = pb.Tag as MapSymbol;

            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                // can't select more than one marker symbol
                if (ModifierKeys == Keys.Shift && clickedSymbol?.SymbolType != MapSymbolType.Marker)
                {
                    // secondary symbol selection - for additional symbols to be used when painting symbols to the map (forests, etc.)
                    if (CURRENT_DRAWING_MODE == MapDrawingMode.SymbolPlace)
                    {
                        if (pb.BackColor == Color.AliceBlue)
                        {
                            pb.BackColor = SystemColors.Control;
                            pb.Refresh();

                            if (pb.Tag is MapSymbol s)
                            {
                                SymbolMethods.SecondarySelectedSymbols.Remove(s);
                            }
                        }
                        else
                        {
                            pb.BackColor = Color.AliceBlue;
                            pb.Refresh();

                            if (pb.Tag is MapSymbol s)
                            {
                                SymbolMethods.SecondarySelectedSymbols.Add(s);
                            }
                        }
                    }
                }
                else if (ModifierKeys == Keys.None)
                {
                    CURRENT_DRAWING_MODE = MapDrawingMode.SymbolPlace;
                    SetDrawingModeLabel();
                    SetSelectedBrushSize(0);

                    // primary symbol selection                    
                    if (pb.Tag is MapSymbol)
                    {
                        SelectPrimarySymbolInSymbolTable(pb);
                    }
                }
            }
            else if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                if (pb.Tag is MapSymbol s)
                {
                    SymbolInfo si = new(s);
                    si.ShowDialog();
                }
            }
        }

        private void SelectPrimarySymbolInSymbolTable(PictureBox pb)
        {
            if (pb.Tag is MapSymbol s)
            {
                if (SymbolMethods.SelectedSymbolTableMapSymbol == null || s.SymbolGuid.ToString() != SymbolMethods.SelectedSymbolTableMapSymbol.SymbolGuid.ToString())
                {
                    foreach (Control control in SymbolTable.Controls)
                    {
                        if (control != pb)
                        {
                            control.BackColor = SystemColors.Control;
                            control.Refresh();
                        }
                    }

                    SymbolMethods.SecondarySelectedSymbols.Clear();
                    Color pbBackColor = pb.BackColor;

                    if (pbBackColor == SystemColors.Control)
                    {
                        // clicked symbol is not selected, so select it
                        pb.BackColor = Color.LightSkyBlue;
                        pb.Refresh();

                        SymbolMethods.SelectedSymbolTableMapSymbol = s;
                    }
                    else
                    {
                        // clicked symbol is already selected, so deselect it
                        pb.BackColor = SystemColors.Control;
                        pb.Refresh();

                        SymbolMethods.SelectedSymbolTableMapSymbol = null;
                        CURRENT_DRAWING_MODE = MapDrawingMode.None;
                        SetDrawingModeLabel();
                    }
                }
            }
        }

        private void PlaceSelectedSymbolAtCursor(SKPoint mouseCursorPoint)
        {
            if (SymbolMethods.SelectedSymbolTableMapSymbol != null)
            {
                float symbolScale = SymbolScaleTrack.Value / 100.0F;
                float symbolRotation = SymbolRotationTrack.Value;

                if (SymbolMethods.SelectedSymbolTableMapSymbol.SymbolFormat != SymbolFileFormat.Vector)
                {
                    SKBitmap? symbolBitmap = SymbolMethods.SelectedSymbolTableMapSymbol.SymbolBitmap;
                    if (symbolBitmap != null)
                    {
                        SKBitmap rotatedAndScaledBitmap = RotateAndScaleSymbolBitmap(symbolBitmap, symbolScale, symbolRotation);
                        SKPoint cursorPoint = new(mouseCursorPoint.X - (rotatedAndScaledBitmap.Width / 2), mouseCursorPoint.Y - (rotatedAndScaledBitmap.Height / 2));

                        PlaceSelectedMapSymbolAtPoint(cursorPoint, symbolScale, symbolRotation);
                    }
                }
                else
                {
                    PlaceVectorSymbolAtPoint(mouseCursorPoint, symbolScale, symbolRotation);
                }
            }
        }

        private void PlaceVectorSymbolAtPoint(SKPoint mouseCursorPoint, float symbolScale, float symbolRotation)
        {
            MapSymbol? symbolToPlace = SymbolMethods.SelectedSymbolTableMapSymbol;

            if (symbolToPlace != null)
            {
                SKBitmap? b = SymbolMethods.GetBitmapForVectorSymbol(symbolToPlace,
                    0, 0, symbolRotation, symbolScale);

                if (b != null)
                {
                    float bitmapRadius = b.Width / 2;
                    float placementDensityRadius = bitmapRadius / PLACEMENT_DENSITY;

                    SKPoint cursorPoint = new(mouseCursorPoint.X - (b.Width / 2), mouseCursorPoint.Y - (b.Height / 2));

                    bool canPlaceSymbol = SymbolMethods.CanPlaceSymbol(CURRENT_MAP, cursorPoint, placementDensityRadius);

                    if (canPlaceSymbol)
                    {
                        symbolToPlace.CustomSymbolColors[0] = SymbolColor1Button.BackColor.ToSKColor();
                        symbolToPlace.CustomSymbolColors[1] = SymbolColor2Button.BackColor.ToSKColor();
                        symbolToPlace.CustomSymbolColors[2] = SymbolColor3Button.BackColor.ToSKColor();

                        symbolToPlace.Width = b.Width;
                        symbolToPlace.Height = b.Height;

                        SymbolMethods.PlaceVectorSymbolOnMap(CURRENT_MAP, SymbolMethods.SelectedSymbolTableMapSymbol, b, cursorPoint);
                    }
                }
            }
        }

        private void PlaceSelectedMapSymbolAtPoint(SKPoint cursorPoint, float symbolScale, float symbolRotation)
        {
            MapSymbol? symbolToPlace = SymbolMethods.SelectedSymbolTableMapSymbol;

            if (symbolToPlace != null)
            {
                if (SymbolMethods.SecondarySelectedSymbols.Count > 0)
                {
                    int selectedIndex = Random.Shared.Next(0, SymbolMethods.SecondarySelectedSymbols.Count + 1);

                    if (selectedIndex > 0)
                    {
                        symbolToPlace = SymbolMethods.SecondarySelectedSymbols[selectedIndex - 1];
                    }
                }

                symbolToPlace.X = (int)cursorPoint.X;
                symbolToPlace.Y = (int)cursorPoint.Y;

                SKBitmap? symbolBitmap = symbolToPlace.ColorMappedBitmap;

                if (symbolBitmap != null)
                {
                    SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleSKBitmap(symbolBitmap, symbolScale);
                    SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateSKBitmap(scaledSymbolBitmap, symbolRotation, MirrorSymbolSwitch.Checked);

                    float bitmapRadius = rotatedAndScaledBitmap.Width / 2;

                    // PLACEMENT_RATE controls the timer used for placing symbols with the area brush
                    // PLACEMENT_DENSITY controls how close together symbols can be placed

                    // decreasing this value increases the density of symbol placement on the map
                    // so high values of placement density on the placement density updown increase placement density on the map
                    float placementDensityRadius = bitmapRadius / PLACEMENT_DENSITY;

                    bool canPlaceSymbol = SymbolMethods.CanPlaceSymbol(CURRENT_MAP, cursorPoint, placementDensityRadius);

                    if (canPlaceSymbol)
                    {
                        symbolToPlace.CustomSymbolColors[0] = SymbolColor1Button.BackColor.ToSKColor();
                        symbolToPlace.CustomSymbolColors[1] = SymbolColor2Button.BackColor.ToSKColor();
                        symbolToPlace.CustomSymbolColors[2] = SymbolColor3Button.BackColor.ToSKColor();

                        symbolToPlace.Width = rotatedAndScaledBitmap.Width;
                        symbolToPlace.Height = rotatedAndScaledBitmap.Height;

                        SymbolMethods.PlaceSymbolOnMap(CURRENT_MAP, symbolToPlace, rotatedAndScaledBitmap, cursorPoint);
                    }
                }
            }
        }

        private void PlaceSelectedSymbolInArea(SKPoint mouseCursorPoint, float symbolScale, float symbolRotation, int areaBrushSize)
        {
            if (SymbolMethods.SelectedSymbolTableMapSymbol != null)
            {
                SKBitmap? symbolBitmap = SymbolMethods.SelectedSymbolTableMapSymbol.SymbolBitmap;
                if (symbolBitmap != null)
                {
                    SKBitmap rotatedAndScaledBitmap = RotateAndScaleSymbolBitmap(symbolBitmap, symbolScale, symbolRotation);

                    SKPoint cursorPoint = new(mouseCursorPoint.X - (rotatedAndScaledBitmap.Width / 2), mouseCursorPoint.Y - (rotatedAndScaledBitmap.Height / 2));

                    float bitmapRadius = rotatedAndScaledBitmap.Width / 2;

                    // decreasing this value increases the density of symbol placement on the map
                    // so high values of placement density on the placement density updown increase placement density on the map
                    float placementDensityRadius = bitmapRadius / PLACEMENT_DENSITY;

                    List<SKPoint> areaPoints = DrawingMethods.GetPointsInCircle(cursorPoint, (int)Math.Ceiling((double)areaBrushSize), (int)placementDensityRadius);

                    foreach (SKPoint p in areaPoints)
                    {
                        SKPoint point = p;

                        // 1% randomization of point location
                        point.X = Random.Shared.Next((int)(p.X * 0.99F), (int)(p.X * 1.01F));
                        point.Y = Random.Shared.Next((int)(p.Y * 0.99F), (int)(p.Y * 1.01F));

                        PlaceSelectedMapSymbolAtPoint(point, symbolScale, symbolRotation);
                    }
                }
            }
        }

        private SKBitmap RotateAndScaleSymbolBitmap(SKBitmap symbolBitmap, float symbolScale, float symbolRotation)
        {
            SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleSKBitmap(symbolBitmap, symbolScale);

            SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateSKBitmap(scaledSymbolBitmap, symbolRotation, MirrorSymbolSwitch.Checked);

            return rotatedAndScaledBitmap;
        }

        internal static MapSymbol? SelectMapSymbolAtPoint(RealmStudioMap map, PointF mapClickPoint)
        {
            MapSymbol? selectedSymbol = null;

            List<MapComponent> mapSymbolComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER).MapLayerComponents;

            for (int i = 0; i < mapSymbolComponents.Count; i++)
            {
                if (mapSymbolComponents[i] is MapSymbol mapSymbol)
                {
                    RectangleF symbolRect = new(mapSymbol.X, mapSymbol.Y, mapSymbol.Width, mapSymbol.Height);

                    if (symbolRect.Contains(mapClickPoint))
                    {
                        selectedSymbol = mapSymbol;
                    }
                }
            }

            DeselectAllMapComponents(selectedSymbol);
            return selectedSymbol;
        }

        private static void MoveSelectedSymbolInRenderOrder(ComponentMoveDirection direction, int amount = 1, bool toTopBottom = false)
        {
            if (SELECTED_MAP_SYMBOL != null)
            {
                // find the selected symbol in the Symbol Layer MapComponents
                MapLayer symbolLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.SYMBOLLAYER);

                List<MapComponent> symbolComponents = symbolLayer.MapLayerComponents;
                MapSymbol? selectedSymbol = null;

                int selectedSymbolIndex = 0;

                for (int i = 0; i < symbolComponents.Count; i++)
                {
                    MapComponent symbolComponent = symbolComponents[i];
                    if (symbolComponent is MapSymbol symbol && symbol.SymbolGuid.ToString() == SELECTED_MAP_SYMBOL.SymbolGuid.ToString())
                    {
                        selectedSymbolIndex = i;
                        selectedSymbol = symbol;
                        break;
                    }
                }

                if (direction == ComponentMoveDirection.Up)
                {
                    // moving a symbol up in render order means increasing its index
                    if (selectedSymbol != null && selectedSymbolIndex < symbolComponents.Count - 1)
                    {
                        if (toTopBottom)
                        {
                            symbolComponents.RemoveAt(selectedSymbolIndex);
                            symbolComponents.Add(selectedSymbol);
                        }
                        else
                        {
                            int moveLocation;

                            if (selectedSymbolIndex + amount < symbolComponents.Count - 1)
                            {
                                moveLocation = selectedSymbolIndex + amount;
                            }
                            else
                            {
                                moveLocation = symbolComponents.Count - 1;
                            }

                            symbolComponents[selectedSymbolIndex] = symbolComponents[moveLocation];
                            symbolComponents[moveLocation] = selectedSymbol;
                        }
                    }
                }
                else if (direction == ComponentMoveDirection.Down)
                {
                    // moving a symbol down in render order means decreasing its index
                    if (selectedSymbol != null && selectedSymbolIndex > 0)
                    {
                        if (toTopBottom)
                        {
                            symbolComponents.RemoveAt(selectedSymbolIndex);
                            symbolComponents.Insert(0, selectedSymbol);
                        }
                        else
                        {
                            int moveLocation;

                            if (selectedSymbolIndex - amount >= 0)
                            {
                                moveLocation = selectedSymbolIndex - amount;
                            }
                            else
                            {
                                moveLocation = 0;
                            }

                            symbolComponents[selectedSymbolIndex] = symbolComponents[moveLocation];
                            symbolComponents[moveLocation] = selectedSymbol;
                        }
                    }
                }
            }
        }
        #endregion

        #region Label Tab Methods

        private void LabelTextBox_KeyPress(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LABELLAYER);

                TextBox tb = (TextBox)sender;

                Font labelFont = tb.Font;
                Color labelColor = FontColorSelectButton.BackColor;
                Color outlineColor = OutlineColorSelectButton.BackColor;
                float outlineWidth = OutlineWidthTrack.Value / 10F;

                Color glowColor = GlowColorSelectButton.BackColor;
                int glowStrength = GlowStrengthTrack.Value;

                int labelRotationDegrees = LabelRotationTrack.Value;

                if (((KeyPressEventArgs)e).KeyChar == (char)Keys.Escape)
                {
                    ((KeyPressEventArgs)e).Handled = false; // pass the event up

                    CREATING_LABEL = false;

                    // dispose of the text box, as it isn't needed once the label text has been entered
                    SKGLRenderControl.Controls.Remove(tb);
                    tb.Dispose();
                }
                else if (((KeyPressEventArgs)e).KeyChar == (char)Keys.Return)
                {
                    ((KeyPressEventArgs)e).Handled = true;
                    CREATING_LABEL = false;

                    if (SELECTED_MAP_LABEL != null)
                    {
                        labelFont = SELECTED_MAP_LABEL.LabelFont;
                        labelColor = SELECTED_MAP_LABEL.LabelColor;
                        outlineColor = SELECTED_MAP_LABEL.LabelOutlineColor;
                        outlineWidth = SELECTED_MAP_LABEL.LabelOutlineWidth;
                        glowColor = SELECTED_MAP_LABEL.LabelGlowColor;
                        glowStrength = SELECTED_MAP_LABEL.LabelGlowStrength;
                        labelRotationDegrees = (int)SELECTED_MAP_LABEL.LabelRotationDegrees;
                    }

                    if (!string.IsNullOrEmpty(tb.Text))
                    {
                        // create a new MapLabel object and render it
                        MapLabel label = new()
                        {
                            LabelText = tb.Text,
                            LabelFont = labelFont,
                            IsSelected = true,
                            LabelColor = labelColor,
                            LabelOutlineColor = outlineColor,
                            LabelOutlineWidth = outlineWidth,
                            LabelGlowColor = glowColor,
                            LabelGlowStrength = glowStrength,
                            LabelRotationDegrees = labelRotationDegrees,
                        };

                        SKFont skLabelFont = MapLabelMethods.GetSkLabelFont(labelFont);
                        SKPaint paint = MapLabelMethods.CreateLabelPaint(labelColor);

                        label.LabelPaint = paint;
                        label.LabelSKFont.Dispose();
                        label.LabelSKFont = skLabelFont;

                        label.LabelSKFont.MeasureText(label.LabelText, out SKRect bounds, label.LabelPaint);

                        float descent = labelFont.FontFamily.GetCellDescent(labelFont.Style);
                        float descentPixel =
                            labelFont.Size * descent / labelFont.FontFamily.GetEmHeight(FontStyle.Regular);

                        // TODO: drawing zoom has to be taken into account?
                        float xDiff = (tb.Width - bounds.Width) / 2;
                        float yDiff = ((tb.Height - bounds.Height) / 2) + descentPixel / 2;

                        SKPoint zoomedScrolledPoint = new(((tb.Left + xDiff) / DrawingZoom) + DrawingPoint.X, ((tb.Top + yDiff) / DrawingZoom) + DrawingPoint.Y);

                        label.X = (int)zoomedScrolledPoint.X;
                        label.Y = (int)zoomedScrolledPoint.Y;

                        if (SELECTED_MAP_LABEL != null)
                        {
                            label.X = SELECTED_MAP_LABEL.X;
                            label.Y = SELECTED_MAP_LABEL.Y;
                        }

                        label.Width = (int)bounds.Width;
                        label.Height = (int)bounds.Height;

                        if (tb.Tag != null && tb.Tag is SKPath path)
                        {
                            label.LabelPath = path;
                        }
                        else if (CURRENT_MAP_LABEL_PATH?.PointCount > 0)
                        {
                            label.LabelPath = new(CURRENT_MAP_LABEL_PATH);

                            CURRENT_MAP_LABEL_PATH.Dispose();
                            CURRENT_MAP_LABEL_PATH = new();

                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                        }

                        Cmd_AddLabel cmd = new(CURRENT_MAP, label);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        DeselectAllMapComponents(label);

                        SELECTED_MAP_LABEL = (MapLabel?)labelLayer.MapLayerComponents.Last();

                        CURRENT_MAP.IsSaved = false;
                    }

                    CURRENT_DRAWING_MODE = MapDrawingMode.LabelSelect;
                    SetDrawingModeLabel();

                    // dispose of the text box, as it isn't needed once the label text has been entered
                    SKGLRenderControl.Controls.Remove(tb);
                    tb.Dispose();

                    SKGLRenderControl.Refresh();
                }
                else
                {
                    if (tb.Text.StartsWith("...Label..."))
                    {
                        tb.Text = tb.Text["...Label...".Length..];
                    }

                    SKFontStyle fs = SKFontStyle.Normal;

                    if (labelFont.Bold && labelFont.Italic)
                    {
                        fs = SKFontStyle.BoldItalic;
                    }
                    else if (labelFont.Bold)
                    {
                        fs = SKFontStyle.Bold;
                    }
                    else if (labelFont.Italic)
                    {
                        fs = SKFontStyle.Italic;
                    }

                    List<string> resourceNames = [.. Assembly.GetExecutingAssembly().GetManifestResourceNames()];

                    SKTypeface? fontTypeface = null;

                    foreach (string resourceName in resourceNames)
                    {
                        if (resourceName.Contains(labelFont.FontFamily.Name))
                        {
                            fontTypeface = SKTypeface.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
                            break;
                        }
                    }

                    fontTypeface ??= SKTypeface.FromFamilyName(labelFont.FontFamily.Name, fs);

                    SKFont paintFont = new(fontTypeface, labelFont.SizeInPoints, 1, 0);
                    SKPaint labelPaint = MapLabelMethods.CreateLabelPaint(labelColor);

                    float lblWidth = paintFont.MeasureText(tb.Text, labelPaint);
                    int tbWidth = (int)Math.Max(lblWidth, tb.Width);
                    tb.Width = tbWidth;

                    SKGLRenderControl.Refresh();
                }
            }
        }

        private void AddMapBoxesToLabelBoxTable(List<MapBox> mapBoxes)
        {
            LabelBoxStyleTable.Hide();
            LabelBoxStyleTable.Controls.Clear();
            foreach (MapBox box in mapBoxes)
            {
                if (box.BoxBitmap == null && !string.IsNullOrEmpty(box.BoxBitmapPath))
                {
                    if (File.Exists(box.BoxBitmapPath))
                    {
                        box.BoxBitmap ??= new Bitmap(box.BoxBitmapPath);
                    }
                }

                if (box.BoxBitmap != null)
                {
                    PictureBox pb = new()
                    {
                        Tag = box,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = (Image)box.BoxBitmap.Clone(),
                    };

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                    pb.MouseClick += MapBoxPictureBox_MouseClick;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                    LabelBoxStyleTable.Controls.Add(pb);
                }

            }
            LabelBoxStyleTable.Show();
            LabelBoxStyleTable.Refresh();
        }

        private void UpdateSelectedLabelOnUIChange()
        {
            if (SELECTED_MAP_LABEL != null)
            {
                Color labelColor = FontColorSelectButton.BackColor;
                Color outlineColor = OutlineColorSelectButton.BackColor;
                float outlineWidth = OutlineWidthTrack.Value / 10F;
                Color glowColor = GlowColorSelectButton.BackColor;
                int glowStrength = GlowStrengthTrack.Value;

                Font tbFont = new(SELECTED_LABEL_FONT.FontFamily,
                    SELECTED_LABEL_FONT.Size * 0.75F, SELECTED_LABEL_FONT.Style, GraphicsUnit.Point);

                Cmd_ChangeLabelAttributes cmd = new(CURRENT_MAP, SELECTED_MAP_LABEL, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, tbFont);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private static MapLabel? SelectLabelAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            MapLabel? selectedLabel = null;

            List<MapComponent> mapLabelComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER).MapLayerComponents;

            for (int i = 0; i < mapLabelComponents.Count; i++)
            {
                if (mapLabelComponents[i] is MapLabel mapLabel)
                {
                    SKRect labelRect = new(mapLabel.X, mapLabel.Y, mapLabel.X + mapLabel.Width, mapLabel.Y + mapLabel.Height);

                    if (labelRect.Contains(zoomedScrolledPoint))
                    {
                        selectedLabel = mapLabel;
                    }
                }
            }

            DeselectAllMapComponents(selectedLabel);

            return selectedLabel;
        }

        private static PlacedMapBox? SelectMapBoxAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            PlacedMapBox? selectedBox = null;

            List<MapComponent> mapLabelComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.BOXLAYER).MapLayerComponents;

            for (int i = 0; i < mapLabelComponents.Count; i++)
            {
                if (mapLabelComponents[i] is PlacedMapBox mapBox)
                {
                    SKRect boxRect = new(mapBox.X, mapBox.Y, mapBox.X + mapBox.Width, mapBox.Y + mapBox.Height);

                    if (boxRect.Contains(zoomedScrolledPoint))
                    {
                        selectedBox = mapBox;
                    }
                }
            }

            DeselectAllMapComponents(selectedBox);

            return selectedBox;
        }

        private static void ConstructBezierPathFromPoints()
        {
            CURRENT_MAP_LABEL_PATH.Dispose();
            CURRENT_MAP_LABEL_PATH = new();

            if (CURRENT_MAP_LABEL_PATH_POINTS.Count > 2)
            {
                CURRENT_MAP_LABEL_PATH.MoveTo(CURRENT_MAP_LABEL_PATH_POINTS[0]);

                for (int j = 0; j < CURRENT_MAP_LABEL_PATH_POINTS.Count; j += 3)
                {
                    if (j < CURRENT_MAP_LABEL_PATH_POINTS.Count - 2)
                    {
                        CURRENT_MAP_LABEL_PATH.CubicTo(CURRENT_MAP_LABEL_PATH_POINTS[j], CURRENT_MAP_LABEL_PATH_POINTS[j + 1], CURRENT_MAP_LABEL_PATH_POINTS[j + 2]);
                    }
                }
            }
        }

        private void SetLabelValuesFromPreset(LabelPreset preset)
        {
            FontColorSelectButton.BackColor = Color.FromArgb(preset.LabelColor);
            FontColorSelectButton.Refresh();

            OutlineColorSelectButton.BackColor = Color.FromArgb(preset.LabelOutlineColor);
            OutlineColorSelectButton.Refresh();

            OutlineWidthTrack.Value = (int)(preset.LabelOutlineWidth * 10F);
            OutlineWidthTrack.Refresh();

            GlowColorSelectButton.BackColor = Color.FromArgb(preset.LabelGlowColor);
            GlowColorSelectButton.Refresh();

            GlowStrengthTrack.Value = preset.LabelGlowStrength;
            GlowStrengthTrack.Refresh();

            string fontString = preset.LabelFontString;

            string[] fontParts = fontString.Split(',');

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
                    SELECTED_LABEL_FONT = new Font(family, fontsize, FontStyle.Regular, GraphicsUnit.Point);
                    SelectLabelFontButton.Font = new Font(SELECTED_LABEL_FONT.FontFamily, 14);
                }
                catch
                {
                    // couldn't create the font, so try to find it in the list of embedded fonts
                    for (int i = 0; i < EMBEDDED_FONTS.Families.Length; i++)
                    {
                        if (EMBEDDED_FONTS.Families[i].Name == ff)
                        {
                            SELECTED_LABEL_FONT = new Font(EMBEDDED_FONTS.Families[i], fontsize, FontStyle.Regular, GraphicsUnit.Point);
                            SelectLabelFontButton.Font = new Font(SELECTED_LABEL_FONT.FontFamily, 14);
                        }
                    }
                }
            }
            else
            {
                SELECTED_LABEL_FONT = DEFAULT_LABEL_FONT;
            }

            SelectLabelFontButton.Refresh();

            int selectedFontIndex = 0;

            if (FontFamilyCombo.Items != null && FontFamilyCombo.Items.Count > 0)
            {
                for (int i = 0; i < FontFamilyCombo.Items?.Count; i++)
                {
                    if (FontFamilyCombo.Items != null && FontFamilyCombo.Items[i] != null
                        && ((Font?)FontFamilyCombo.Items[i])?.FontFamily.Name == SELECTED_LABEL_FONT.FontFamily.Name)
                    {
                        selectedFontIndex = i;
                        break;
                    }
                }

                FontFamilyCombo.SelectedIndex = selectedFontIndex;
            }



            int selectedSizeIndex = 0;

            if (FontSizeCombo.Items != null && FontSizeCombo.Items.Count > 0)
            {
                for (int i = 0; i < FontSizeCombo.Items?.Count; i++)
                {
                    if (FontSizeCombo.Items != null && FontSizeCombo.Items[i] != null
                        && FontSizeCombo.Items[i]?.ToString() == Math.Round(SELECTED_LABEL_FONT.SizeInPoints).ToString())
                    {
                        selectedSizeIndex = i;
                        break;
                    }
                }

                FontSizeCombo.SelectedIndex = selectedSizeIndex;
            }
        }

        #endregion

        #region Label Tab Event Handlers

        private void ShowLabelLayerSwitch_CheckedChanged()
        {
            MapLayer labellLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LABELLAYER);

            labellLayer.ShowLayer = ShowLabelLayerSwitch.Checked;

            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BOXLAYER);

            boxLayer.ShowLayer = ShowLabelLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        private void MapBoxPictureBox_MouseClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                if (ModifierKeys == Keys.None)
                {
                    PictureBox pb = (PictureBox)sender;

                    if (pb.Tag is MapBox b)
                    {
                        foreach (Control control in LabelBoxStyleTable.Controls)
                        {
                            if (control != pb)
                            {
                                control.BackColor = SystemColors.Control;
                            }
                        }

                        Color pbBackColor = pb.BackColor;

                        if (pbBackColor.ToArgb() == SystemColors.Control.ToArgb())
                        {
                            // clicked symbol is not selected, so select it
                            pb.BackColor = Color.LightSkyBlue;
                            SELECTED_MAP_BOX = b;
                        }
                        else
                        {
                            // clicked symbol is already selected, so deselect it
                            pb.BackColor = SystemColors.Control;
                            SELECTED_MAP_BOX = null;
                        }
                    }
                }
            }
        }

        private void LabelPresetsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LabelPresetsListBox.SelectedIndex >= 0 && LabelPresetsListBox.SelectedIndex < AssetManager.LABEL_PRESETS.Count)
            {
                LabelPreset selectedPreset = (LabelPreset)LabelPresetsListBox.Items[LabelPresetsListBox.SelectedIndex];
                SetLabelValuesFromPreset(selectedPreset);
            }
        }

        private void AddPresetButton_Click(object sender, EventArgs e)
        {
            LabelPreset? selectedPreset = null;

            if (LabelPresetsListBox.SelectedIndex >= 0 && LabelPresetsListBox.SelectedIndex < AssetManager.LABEL_PRESETS.Count)
            {
                selectedPreset = (LabelPreset)LabelPresetsListBox.Items[LabelPresetsListBox.SelectedIndex];
            }

            LabelPresetNameEntry presetDialog = new();

            if (selectedPreset != null)
            {
                presetDialog.PresetNameTextBox.Text = selectedPreset.LabelPresetName;
            }

            DialogResult result = presetDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string presetName = presetDialog.PresetNameTextBox.Text;

                string currentThemeName = string.Empty;

                if (AssetManager.CURRENT_THEME != null && !string.IsNullOrEmpty(AssetManager.CURRENT_THEME.ThemeName))
                {
                    currentThemeName = AssetManager.CURRENT_THEME.ThemeName;
                }
                else
                {
                    currentThemeName = "DEFAULT";
                }

                string presetFileName = AssetManager.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "LabelPresets" + Path.DirectorySeparatorChar + Guid.NewGuid().ToString() + ".mclblprst";

                if (presetName == selectedPreset?.LabelPresetName)
                {
                    presetFileName = selectedPreset.PresetXmlFilePath;
                }

                bool makeNewPreset = true;

                if (File.Exists(presetFileName))
                {
                    LabelPreset? existingPreset = AssetManager.LABEL_PRESETS.Find(x => x.LabelPresetName == presetName && x.LabelPresetTheme == currentThemeName);
                    if (existingPreset != null && existingPreset.IsDefault)
                    {
                        makeNewPreset = false;
                    }
                    else
                    {
                        DialogResult r = MessageBox.Show("A label preset named " + presetName + " for theme " + currentThemeName + " already exists. Replace it?", "Replace Preset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (r == DialogResult.No)
                        {
                            makeNewPreset = false;
                        }
                    }
                }

                if (makeNewPreset)
                {
                    LabelPreset preset = new()
                    {
                        PresetXmlFilePath = presetFileName
                    };

                    FontConverter cvt = new();

                    Font f = new(SELECTED_LABEL_FONT.FontFamily, SELECTED_LABEL_FONT.Size * 0.75F, SELECTED_LABEL_FONT.Style, GraphicsUnit.Point);

                    string? fontString = cvt.ConvertToString(f);

                    if (!string.IsNullOrEmpty(fontString))
                    {
                        preset.LabelPresetName = presetName;
                        if (AssetManager.CURRENT_THEME != null && !string.IsNullOrEmpty(AssetManager.CURRENT_THEME.ThemeName))
                        {
                            preset.LabelPresetTheme = AssetManager.CURRENT_THEME.ThemeName;
                        }
                        else
                        {
                            preset.LabelPresetTheme = "DEFAULT";
                        }

                        preset.LabelFontString = fontString;
                        preset.LabelColor = FontColorSelectButton.BackColor.ToArgb();
                        preset.LabelOutlineColor = OutlineColorSelectButton.BackColor.ToArgb();
                        preset.LabelOutlineWidth = OutlineWidthTrack.Value / 10F;
                        preset.LabelGlowColor = GlowColorSelectButton.BackColor.ToArgb();
                        preset.LabelGlowStrength = GlowStrengthTrack.Value;

                        preset.PresetXmlFilePath = presetFileName;

                        MapFileMethods.SerializeLabelPreset(preset);

                        int assetCount = AssetManager.LoadAllAssets();

                        PopulateControlsWithAssets(assetCount);
                    }
                }
            }
        }

        private void RemovePresetButton_Click(object sender, EventArgs e)
        {
            //remove a preset (prevent default presets from being deleted or changed)

            if (LabelPresetsListBox.SelectedIndex >= 0)
            {
                string? presetName = ((LabelPreset?)LabelPresetsListBox.SelectedItem)?.LabelPresetName;

                if (!string.IsNullOrEmpty(presetName))
                {
                    string currentThemeName = string.Empty;

                    if (AssetManager.CURRENT_THEME != null && !string.IsNullOrEmpty(AssetManager.CURRENT_THEME.ThemeName))
                    {
                        currentThemeName = AssetManager.CURRENT_THEME.ThemeName;
                    }
                    else
                    {
                        currentThemeName = "DEFAULT";
                    }

                    LabelPreset? existingPreset = AssetManager.LABEL_PRESETS.Find(x => x.LabelPresetName == presetName && x.LabelPresetTheme == currentThemeName);

                    if (existingPreset != null && !existingPreset.IsDefault)
                    {
                        if (!string.IsNullOrEmpty(existingPreset.PresetXmlFilePath))
                        {
                            if (File.Exists(existingPreset.PresetXmlFilePath))
                            {
                                DialogResult r = MessageBox.Show("The label preset named " + presetName + " for theme " + currentThemeName + " will be deleted. Continue?", "Delete Label Preset",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                                if (r == DialogResult.Yes)
                                {
                                    try
                                    {
                                        File.Delete(existingPreset.PresetXmlFilePath);

                                        int assetCount = AssetManager.LoadAllAssets();

                                        PopulateControlsWithAssets(assetCount);

                                        MessageBox.Show("The label preset has been deleted.", "Preset Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    }
                                    catch (Exception ex)
                                    {
                                        Program.LOGGER.Error(ex);
                                        MessageBox.Show("The label preset could not be deleted.", "Preset Not Deleted", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("The selected label preset cannot be deleted.", "Preset Not Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }
        }

        private void SelectLabelFontButton_Click(object sender, EventArgs e)
        {
            FONT_PANEL_OPENER = FontPanelOpener.LabelFontButton;
            FontSelectionPanel.Visible = true;
        }

        private void FontColorSelectButton_Click(object sender, EventArgs e)
        {
            Color labelColor = UtilityMethods.SelectColorFromDialog(this, FontColorSelectButton.BackColor);

            if (labelColor.ToArgb() != Color.Empty.ToArgb())
            {
                FontColorSelectButton.BackColor = labelColor;

                FontColorSelectButton.Refresh();

                UpdateSelectedLabelOnUIChange();
            }
        }

        private void OutlineColorSelectButton_Click(object sender, EventArgs e)
        {
            Color outlineColor = UtilityMethods.SelectColorFromDialog(this, OutlineColorSelectButton.BackColor);

            if (outlineColor.ToArgb() != Color.Empty.ToArgb())
            {
                OutlineColorSelectButton.BackColor = outlineColor;
                OutlineColorSelectButton.Refresh();

                UpdateSelectedLabelOnUIChange();
            }
        }

        private void OutlineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((OutlineWidthTrack.Value / 10.0F).ToString(), LabelOutlineGroup, new Point(OutlineWidthTrack.Right - 30, OutlineWidthTrack.Top - 20), 2000);

            UpdateSelectedLabelOnUIChange();
        }

        private void GlowColorSelectButton_Click(object sender, EventArgs e)
        {
            Color outlineColor = UtilityMethods.SelectColorFromDialog(this, GlowColorSelectButton.BackColor);

            if (outlineColor.ToArgb() != Color.Empty.ToArgb())
            {
                GlowColorSelectButton.BackColor = outlineColor;
                GlowColorSelectButton.Refresh();

                UpdateSelectedLabelOnUIChange();
            }
        }

        private void GlowWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(GlowStrengthTrack.Value.ToString(), LabelGlowGroup, new Point(GlowStrengthTrack.Right - 30, GlowStrengthTrack.Top - 20), 2000);

            UpdateSelectedLabelOnUIChange();
        }

        private void LabelRotationUpDown_ValueChanged(object sender, EventArgs e)
        {
            LabelRotationTrack.Value = (int)LabelRotationUpDown.Value;

            if (SELECTED_MAP_LABEL != null)
            {
                Cmd_ChangeLabelRotation cmd = new(SELECTED_MAP_LABEL, (float)LabelRotationTrack.Value);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void LabelRotationTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(LabelRotationTrack.Value.ToString(), LabelRotationGroup, new Point(LabelRotationTrack.Right - 30, LabelRotationTrack.Top - 20), 2000);

            LabelRotationUpDown.Value = LabelRotationTrack.Value;

            if (SELECTED_MAP_LABEL != null)
            {
                Cmd_ChangeLabelRotation cmd = new(SELECTED_MAP_LABEL, LabelRotationTrack.Value);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void CircleTextPathButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.DrawArcLabelPath;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();
        }

        private void BezierTextPathButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.DrawBezierLabelPath;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();
        }

        private void LabelSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.LabelSelect;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();
        }

        private void PlaceLabelButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.DrawLabel;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();
        }

        private void CreateBoxButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.DrawBox;
            SetSelectedBrushSize(0);
            SetDrawingModeLabel();
        }

        private void SelectBoxTintButton_Click(object sender, EventArgs e)
        {
            Color boxColor = UtilityMethods.SelectColorFromDialog(this, SelectBoxTintButton.BackColor);

            if (boxColor.ToArgb() != Color.Empty.ToArgb())
            {
                SelectBoxTintButton.BackColor = boxColor;

                SelectBoxTintButton.Refresh();

                if (SELECTED_PLACED_MAP_BOX != null)
                {
                    Cmd_ChangeBoxColor cmd = new(SELECTED_PLACED_MAP_BOX, boxColor);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    SKGLRenderControl.Invalidate();
                }
            }
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

        #region Overlay Tab Methods (Frame, Grid, Scale, Measure)

        private void AddMapFramesToFrameTable(List<MapFrame> mapFrames)
        {
            FrameStyleTable.Hide();
            FrameStyleTable.Controls.Clear();
            foreach (MapFrame frame in mapFrames)
            {
                if (frame.FrameBitmapPath != null)
                {
                    if (frame.FrameBitmap == null)
                    {
                        SKImage image = SKImage.FromEncodedData(frame.FrameBitmapPath);
                        frame.FrameBitmap = SKBitmap.FromImage(image);
                    }

                    PictureBox pb = new()
                    {
                        Tag = frame,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = frame.FrameBitmap.ToBitmap(),
                    };

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                    pb.MouseClick += FramePictureBox_MouseClick;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                    FrameStyleTable.Controls.Add(pb);
                }
            }
            FrameStyleTable.Show();
        }

        private void CreateGrid()
        {
            CURRENT_MAP_GRID = new MapGrid
            {
                ParentMap = CURRENT_MAP,
                GridEnabled = true,
                GridColor = GridColorSelectButton.BackColor,
                GridLineWidth = GridLineWidthTrack.Value,
                GridSize = GridSizeTrack.Value,
                Width = CURRENT_MAP.MapWidth,
                Height = CURRENT_MAP.MapHeight,
            };

            if (SquareGridRadio.Checked)
            {
                CURRENT_MAP_GRID.GridType = MapGridType.Square;
            }
            else if (FlatHexGridRadio.Checked)
            {
                CURRENT_MAP_GRID.GridType = MapGridType.FlatHex;
                CURRENT_MAP_GRID.GridSize /= 2;
            }
            else if (PointedHexGridRadio.Checked)
            {
                CURRENT_MAP_GRID.GridType = MapGridType.PointedHex;
                CURRENT_MAP_GRID.GridSize /= 2;
            }

            string? selectedLayerItem = (string?)GridLayerUpDown.SelectedItem;

            if (selectedLayerItem != null)
            {
                CURRENT_MAP_GRID.GridLayerIndex = selectedLayerItem switch
                {
                    "Default" => MapBuilder.DEFAULTGRIDLAYER,
                    "Above Ocean" => MapBuilder.ABOVEOCEANGRIDLAYER,
                    "Below Symbols" => MapBuilder.BELOWSYMBOLSGRIDLAYER,
                    _ => MapBuilder.DEFAULTGRIDLAYER,
                };
            }
            else
            {
                CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
            }

            CURRENT_MAP_GRID.GridPaint = new()
            {
                Style = SKPaintStyle.Stroke,
                Color = CURRENT_MAP_GRID.GridColor.ToSKColor(),
                StrokeWidth = CURRENT_MAP_GRID.GridLineWidth,
                StrokeJoin = SKStrokeJoin.Bevel
            };
        }

        private static void RemoveGrid()
        {
            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region Overlay Tab Event Handlers (Frame, Grid, Scale, Measure)

        private void ShowOverlayLayerSwitch_CheckedChanged()
        {
            MapLayer overlayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER);

            overlayLayer.ShowLayer = ShowOverlayLayerSwitch.Checked;

            MapLayer frameLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER);

            frameLayer.ShowLayer = ShowOverlayLayerSwitch.Checked;

            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER);

            aboveOceanGridLayer.ShowLayer = ShowOverlayLayerSwitch.Checked;

            MapLayer belowSymbolsGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER);

            belowSymbolsGridLayer.ShowLayer = ShowOverlayLayerSwitch.Checked;

            MapLayer defaultGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER);

            defaultGridLayer.ShowLayer = ShowOverlayLayerSwitch.Checked;

            MapLayer measureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER);

            measureLayer.ShowLayer = ShowOverlayLayerSwitch.Checked;

            SKGLRenderControl.Invalidate();
        }

        #region Frame Event Handlers

        private void FramePictureBox_MouseClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                if (ModifierKeys == Keys.None)
                {
                    PictureBox pb = (PictureBox)sender;

                    if (pb.Tag is MapFrame frame)
                    {
                        foreach (Control control in FrameStyleTable.Controls)
                        {
                            if (control != pb)
                            {
                                control.BackColor = SystemColors.Control;
                            }
                        }

                        Color pbBackColor = pb.BackColor;

                        if (pbBackColor.ToArgb() == SystemColors.Control.ToArgb())
                        {
                            // clicked symbol is not selected, so select it
                            pb.BackColor = Color.LightSkyBlue;

                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER).MapLayerComponents.Clear();

                            Cmd_CreateMapFrame cmd = new(CURRENT_MAP, frame, FrameTintColorSelectButton.BackColor, (float)(FrameScaleTrack.Value / 100F));
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

                            CURRENT_MAP.IsSaved = false;

                            SKGLRenderControl.Invalidate();
                        }
                        else
                        {
                            // clicked symbol is already selected, so deselect it
                            pb.BackColor = SystemColors.Control;
                        }
                    }
                }
            }
        }

        private void EnableFrameSwitch_CheckedChanged()
        {
            if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER).MapLayerComponents.Count > 0)
            {
                ((PlacedMapFrame)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER).MapLayerComponents[0])
                    .FrameEnabled = EnableFrameSwitch.Checked;
                SKGLRenderControl.Invalidate();
            }
        }

        private void FrameScaleTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show((FrameScaleTrack.Value / 100F).ToString(), FrameValuesGroup, new Point(FrameScaleTrack.Right - 30, FrameScaleTrack.Top - 20), 2000);

            if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER).MapLayerComponents.Count > 0)
            {
                // there can only be one frame on the map, so get it and update the frame scale
                PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER).MapLayerComponents[0];
                placedFrame.FrameScale = FrameScaleTrack.Value / 100F;
                OverlayMethods.CompletePlacedFrame(placedFrame);

                SKGLRenderControl.Invalidate();
            }
        }

        private void FrameTintColorSelectButton_Click(object sender, EventArgs e)
        {
            Color frameColor = UtilityMethods.SelectColorFromDialog(this, FrameTintColorSelectButton.BackColor);

            if (frameColor.ToArgb() != Color.Empty.ToArgb())
            {
                FrameTintColorSelectButton.BackColor = frameColor;
                FrameTintColorSelectButton.Refresh();

                PlacedMapFrame placedFrame = (PlacedMapFrame)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.FRAMELAYER).MapLayerComponents[0];

                Cmd_ChangeFrameColor cmd = new(placedFrame, FrameTintColorSelectButton.BackColor);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        #endregion

        #region Scale Event Handlers

        private void ScaleButton_Click(object sender, EventArgs e)
        {
            MapScaleCreatorPanel.Visible = !MapScaleCreatorPanel.Visible;

            if (MapScaleCreatorPanel.Visible)
            {
                ScaleUnitsTextBox.Text = CURRENT_MAP.MapAreaUnits;
            }
        }

        private void CreateScaleButton_Click(object sender, EventArgs e)
        {
            MapScale mapScale = new()
            {
                Width = ScaleWidthTrack.Value,
                Height = ScaleHeightTrack.Value,
                ScaleSegmentCount = ScaleSegmentCountTrack.Value,
                ScaleLineWidth = ScaleLineWidthTrack.Value,
                ScaleColor1 = ScaleColor1Button.BackColor,
                ScaleColor2 = ScaleColor2Button.BackColor,
                ScaleColor3 = ScaleColor3Button.BackColor,
                ScaleDistance = (float)ScaleSegmentDistanceUpDown.Value,
                ScaleDistanceUnit = ScaleUnitsTextBox.Text,
                ScaleFontColor = SelectScaleFontColorButton.BackColor,
                ScaleOutlineWidth = ScaleOutlineWidthTrack.Value,
                ScaleOutlineColor = SelectScaleOutlineColorButton.BackColor,
                ScaleFont = SELECTED_MAP_SCALE_FONT,
                ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.None
            };

            if (ScaleNumbersNoneRadio.Checked)
            {
                mapScale.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.None;
            }

            if (ScaleNumbersEndsRadio.Checked)
            {
                mapScale.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.Ends;
            }

            if (ScaleNumbersEveryOtherRadio.Checked)
            {
                mapScale.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.EveryOther;
            }

            if (ScaleNumbersAllRadio.Checked)
            {
                mapScale.ScaleNumbersDisplayType = ScaleNumbersDisplayLocation.All;
            }

            // initial position of the scale is near the bottom-left corner of the map
            mapScale.X = 100;
            mapScale.Y = CURRENT_MAP.MapHeight - 100;

            // make sure there is only one scale - remove any existing scale
            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents.Add(mapScale);

            SKGLRenderControl.Invalidate();
        }

        private void DeleteScaleButton_Click(object sender, EventArgs e)
        {
            // remove any existing scale
            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents[i] is MapScale)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OVERLAYLAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            SKGLRenderControl.Invalidate();
        }

        private void ScaleWidthTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ScaleWidthTrack.Value.ToString(), MapScaleGroupBox, new Point(ScaleWidthTrack.Right - 30, ScaleWidthTrack.Top - 20), 2000);
        }

        private void ScaleHeightTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ScaleHeightTrack.Value.ToString(), MapScaleGroupBox, new Point(ScaleHeightTrack.Right - 30, ScaleHeightTrack.Top - 20), 2000);
        }

        private void ScaleSegmentCountTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ScaleSegmentCountTrack.Value.ToString(), MapScaleGroupBox, new Point(ScaleSegmentCountTrack.Right - 30, ScaleSegmentCountTrack.Top - 20), 2000);
        }

        private void ScaleLineWidthTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ScaleLineWidthTrack.Value.ToString(), MapScaleGroupBox, new Point(ScaleLineWidthTrack.Right - 30, ScaleLineWidthTrack.Top - 20), 2000);
        }

        private void ScaleColorsResetButton_Click(object sender, EventArgs e)
        {
            ScaleColor1Button.BackColor = Color.Black;
            ScaleColor2Button.BackColor = Color.White;
            ScaleColor3Button.BackColor = Color.Black;
        }

        private void ScaleColor1Button_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, ScaleColor1Button.BackColor);

            if (scaleColor.ToArgb() != Color.Empty.ToArgb())
            {
                ScaleColor1Button.BackColor = scaleColor;
                ScaleColor1Button.Refresh();
            }
        }

        private void ScaleColor2Button_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, ScaleColor2Button.BackColor);

            if (scaleColor.ToArgb() != Color.Empty.ToArgb())
            {
                ScaleColor2Button.BackColor = scaleColor;
                ScaleColor2Button.Refresh();
            }
        }

        private void ScaleColor3Button_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, ScaleColor3Button.BackColor);

            if (scaleColor.ToArgb() != Color.Empty.ToArgb())
            {
                ScaleColor3Button.BackColor = scaleColor;
                ScaleColor3Button.Refresh();
            }
        }

        private void SelectScaleFontButton_Click(object sender, EventArgs e)
        {
            FONT_PANEL_OPENER = FontPanelOpener.ScaleFontButton;
            FontSelectionPanel.Visible = !FontSelectionPanel.Visible;
        }

        private void SelectScaleFontColorButton_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, SelectScaleFontColorButton.BackColor);

            if (scaleColor.ToArgb() != Color.Empty.ToArgb())
            {
                SelectScaleFontColorButton.BackColor = scaleColor;
                SelectScaleFontColorButton.Refresh();
            }
        }

        private void SelectScaleOutlineColorButton_Click(object sender, EventArgs e)
        {
            Color scaleColor = UtilityMethods.SelectColorFromDialog(this, SelectScaleOutlineColorButton.BackColor);

            if (scaleColor.ToArgb() != Color.Empty.ToArgb())
            {
                SelectScaleOutlineColorButton.BackColor = scaleColor;
                SelectScaleOutlineColorButton.Refresh();
            }
        }

        private void ScaleOutlineWidthTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(ScaleOutlineWidthTrack.Value.ToString(), ScaleOutlineGroupBox, new Point(ScaleOutlineWidthTrack.Right - 30, ScaleOutlineWidthTrack.Top - 20), 2000);
        }

        #endregion

        #region Grid Event Handlers

        private void GridButton_Click(object sender, EventArgs e)
        {
            if (EnableGridSwitch.Checked)
            {
                // make sure there is only one grid
                RemoveGrid();

                CreateGrid();

                if (CURRENT_MAP_GRID != null)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, CURRENT_MAP_GRID.GridLayerIndex).MapLayerComponents.Add(CURRENT_MAP_GRID);
                }

                SKGLRenderControl.Invalidate();
            }
            else
            {
                // make sure there is only one grid
                RemoveGrid();

                SKGLRenderControl.Invalidate();
            }
        }

        private void EnableGridSwitch_CheckedChanged()
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridEnabled = EnableGridSwitch.Checked;
                SKGLRenderControl.Invalidate();
            }
        }

        private void SquareGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridType = MapGridType.Square;
                SKGLRenderControl.Invalidate();
            }
        }

        private void FlatHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridType = MapGridType.FlatHex;
                SKGLRenderControl.Invalidate();
            }
        }

        private void PointedHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridType = MapGridType.PointedHex;
                SKGLRenderControl.Invalidate();
            }
        }

        private void GridLayerUpDown_SelectedItemChanged(object sender, EventArgs e)
        {
            if (EnableGridSwitch.Checked)
            {
                // make sure there is only one grid
                RemoveGrid();

                CreateGrid();

                if (CURRENT_MAP_GRID != null)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, CURRENT_MAP_GRID.GridLayerIndex).MapLayerComponents.Add(CURRENT_MAP_GRID);
                }

                SKGLRenderControl.Invalidate();
            }
            else
            {
                // make sure there is only one grid
                RemoveGrid();

                SKGLRenderControl.Invalidate();
            }
        }

        private void GridSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(GridSizeTrack.Value.ToString(), GridValuesGroup, new Point(GridSizeTrack.Right - 30, GridSizeTrack.Top - 20), 2000);

            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridSize = GridSizeTrack.Value;
                SKGLRenderControl.Invalidate();
            }
        }

        private void GridLineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(GridLineWidthTrack.Value.ToString(), GridValuesGroup, new Point(GridLineWidthTrack.Right - 30, GridLineWidthTrack.Top - 20), 2000);

            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridLineWidth = GridLineWidthTrack.Value;

                CURRENT_MAP_GRID.GridPaint = new()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = CURRENT_MAP_GRID.GridColor.ToSKColor(),
                    StrokeWidth = CURRENT_MAP_GRID.GridLineWidth,
                    StrokeJoin = SKStrokeJoin.Bevel
                };

                SKGLRenderControl.Invalidate();
            }
        }

        private void GridColorSelectButton_Click(object sender, EventArgs e)
        {
            Color gridColor = UtilityMethods.SelectColorFromDialog(this, GridColorSelectButton.BackColor);

            if (gridColor.ToArgb() != Color.Empty.ToArgb())
            {
                GridColorSelectButton.BackColor = gridColor;
                GridColorSelectButton.Refresh();

                if (CURRENT_MAP_GRID != null)
                {
                    CURRENT_MAP_GRID.GridColor = gridColor;

                    CURRENT_MAP_GRID.GridPaint = new()
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = CURRENT_MAP_GRID.GridColor.ToSKColor(),
                        StrokeWidth = CURRENT_MAP_GRID.GridLineWidth,
                        StrokeJoin = SKStrokeJoin.Bevel
                    };

                    SKGLRenderControl.Invalidate();
                }
            }
        }

        private void ShowGridSizeSwitch_CheckedChanged()
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.ShowGridSize = ShowGridSizeSwitch.Checked;
                SKGLRenderControl.Invalidate();
            }
        }

        #endregion

        #region Measure Event Handlers

        private void MeasureButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.DrawMapMeasure;
        }

        private void SelectMeasureColorButton_Click(object sender, EventArgs e)
        {
            Color measureColor = UtilityMethods.SelectColorFromDialog(this, SelectMeasureColorButton.BackColor);

            if (measureColor.ToArgb() != Color.Empty.ToArgb())
            {
                SelectMeasureColorButton.BackColor = measureColor;
                SelectMeasureColorButton.Refresh();

                if (CURRENT_MAP_MEASURE != null)
                {
                    CURRENT_MAP_MEASURE.MeasureLineColor = measureColor;

                    CURRENT_MAP_MEASURE.MeasureLinePaint = new()
                    {
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1,
                        Color = CURRENT_MAP_MEASURE.MeasureLineColor.ToSKColor()
                    };

                    CURRENT_MAP_MEASURE.MeasureAreaPaint = new()
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        StrokeWidth = 1,
                        Color = CURRENT_MAP_MEASURE.MeasureLineColor.ToSKColor()
                    };

                    SKGLRenderControl.Invalidate();
                }
            }
        }

        private void UseScaleUnitsSwitch_CheckedChanged()
        {
            if (CURRENT_MAP_MEASURE != null)
            {
                CURRENT_MAP_MEASURE.UseMapUnits = UseScaleUnitsSwitch.Checked;

                SKGLRenderControl.Invalidate();
            }
        }

        private void MeasureAreaSwitch_CheckedChanged()
        {
            if (CURRENT_MAP_MEASURE != null)
            {
                CURRENT_MAP_MEASURE.MeasureArea = MeasureAreaSwitch.Checked;

                SKGLRenderControl.Invalidate();
            }
        }

        private void ClearMeasureButton_Click(object sender, EventArgs e)
        {
            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.MEASURELAYER).MapLayerComponents.Clear();
            SKGLRenderControl.Invalidate();
        }

        #endregion

        #endregion 

        #region Region Methods

        internal void SetRegionData(MapRegion mapRegion)
        {
            if (mapRegion == null) { return; }

            mapRegion.RegionBorderColor = RegionColorSelectButton.BackColor;
            mapRegion.RegionBorderWidth = RegionBorderWidthTrack.Value;
            mapRegion.RegionInnerOpacity = RegionOpacityTrack.Value;
            mapRegion.RegionBorderSmoothing = RegionBorderSmoothingTrack.Value;

            if (RegionSolidBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.SolidLinePath;
            }

            SKPathEffect? regionBorderEffect;
            if (RegionDottedBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.DottedLinePath;
            }

            if (RegionDashBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.DashedLinePath;
            }

            if (RegionDashDotBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.DashDotLinePath;

            }

            if (RegionDashDotDotBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.DashDotDotLinePath;
            }

            if (RegionDoubleSolidBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.DoubleSolidBorderPath;
            }

            if (RegionSolidAndDashesBorderRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.LineAndDashesPath;
            }

            if (RegionBorderedGradientRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.BorderedGradientPath;
            }

            if (RegionBorderedLightSolidRadio.Checked)
            {
                mapRegion.RegionBorderType = PathType.BorderedLightSolidPath;
            }

            regionBorderEffect = MapRegionMethods.ConstructRegionBorderEffect(mapRegion);
            MapRegionMethods.ConstructRegionPaintObjects(mapRegion, regionBorderEffect);

            SKGLRenderControl.Invalidate();
        }

        internal static MapRegion? SelectRegionAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            MapRegion? selectedRegion = null;

            List<MapComponent> mapRegionComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.REGIONLAYER).MapLayerComponents;

            for (int i = 0; i < mapRegionComponents.Count; i++)
            {
                if (mapRegionComponents[i] is MapRegion mapRegion)
                {
                    SKPath? boundaryPath = mapRegion.BoundaryPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(zoomedScrolledPoint.X, zoomedScrolledPoint.Y))
                        {
                            mapRegion.IsSelected = !mapRegion.IsSelected;

                            if (mapRegion.IsSelected)
                            {
                                selectedRegion = mapRegion;
                            }
                            break;
                        }
                    }
                }
            }

            DeselectAllMapComponents(selectedRegion);

            return selectedRegion;
        }

        #endregion

        #region Region Tab Event Handlers

        private void ShowRegionLayerSwitch_CheckedChanged()
        {
            MapLayer regionLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.REGIONLAYER);

            regionLayer.ShowLayer = ShowRegionLayerSwitch.Checked;

            MapLayer regionOverlayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.REGIONOVERLAYLAYER);

            regionOverlayLayer.ShowLayer = ShowRegionLayerSwitch.Checked;
        }

        private void SelectRegionButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.RegionSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void CreateRegionButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = MapDrawingMode.RegionPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);

            CURRENT_MAP_REGION = null;
        }

        private void RegionColorSelectButton_Click(object sender, EventArgs e)
        {
            Color regionColor = UtilityMethods.SelectColorFromDialog(this, RegionColorSelectButton.BackColor);

            if (regionColor.ToArgb() != Color.Empty.ToArgb())
            {
                RegionColorSelectButton.BackColor = regionColor;
                RegionColorSelectButton.Refresh();

                if (CURRENT_MAP_REGION != null)
                {
                    SetRegionData(CURRENT_MAP_REGION);
                    SKGLRenderControl.Invalidate();
                }
            }
        }

        private void RegionBorderWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(RegionBorderWidthTrack.Value.ToString(), RegionValuesGroup, new Point(RegionBorderWidthTrack.Right - 30, RegionBorderWidthTrack.Top - 20), 2000);

            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void RegionBorderSmoothingTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(RegionBorderSmoothingTrack.Value.ToString(), RegionValuesGroup, new Point(RegionBorderSmoothingTrack.Right - 30, RegionBorderSmoothingTrack.Top - 20), 2000);

            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void RegionOpacityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(RegionOpacityTrack.Value.ToString(), RegionValuesGroup, new Point(RegionOpacityTrack.Right - 30, RegionOpacityTrack.Top - 20), 2000);

            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void RegionSolidBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void SolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionSolidBorderRadio.Checked = true;
        }

        private void RegionDottedBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void DottedRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDottedBorderRadio.Checked = true;
        }

        private void RegionDashBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void DashedRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashBorderRadio.Checked = true;
        }

        private void RegionDashDotBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void DashDotRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashDotBorderRadio.Checked = true;
        }

        private void RegionDashDotDotBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void DashDotDotRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDashDotDotBorderRadio.Checked = true;
        }

        private void RegionDoubleSolidBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void DoubleSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionDoubleSolidBorderRadio.Checked = true;
        }

        private void RegionSolidAndDashesBorderRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void SolidAndDashRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionSolidAndDashesBorderRadio.Checked = true;
        }

        private void RegionBorderedGradientRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void GradientRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedGradientRadio.Checked = true;
        }

        private void RegionBorderedLightSolidRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_REGION != null)
            {
                SetRegionData(CURRENT_MAP_REGION);
                SKGLRenderControl.Invalidate();
            }
        }

        private void LightSolidRegionBorderPicture_Click(object sender, EventArgs e)
        {
            RegionBorderedLightSolidRadio.Checked = true;
        }

        #endregion
    }
}
