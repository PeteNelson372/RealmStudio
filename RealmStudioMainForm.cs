/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using RealmStudio.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;
using Control = System.Windows.Forms.Control;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static DrawingModeEnum CURRENT_DRAWING_MODE = DrawingModeEnum.None;

        private static RealmStudioMap CURRENT_MAP = new();

        // the objects currently being drawn, before being added to map layers
        private static Landform? CURRENT_LANDFORM = null;
        private static MapWindrose? CURRENT_WINDROSE = null;
        private static WaterFeature? CURRENT_WATERFEATURE = null;
        private static River? CURRENT_RIVER = null;
        private static MapPath? CURRENT_PATH = null;
        private static MapFrame? CURRENT_FRAME = null;
        private static MapGrid? CURRENT_MAP_GRID = null;
        private static MapMeasure? CURRENT_MAP_MEASURE = null;

        // objects that are currently selected
        private static MapPath? SELECTED_PATH = null;
        private static MapPathPoint? SELECTED_PATHPOINT = null;
        private static MapSymbol? SELECTED_MAP_SYMBOL = null;
        private static MapBox? SELECTED_MAP_BOX = null;
        private static PlacedMapBox? SELECTED_PLACED_MAP_BOX = null;
        private static MapLabel? SELECTED_MAP_LABEL = null;
        private static PlacedMapFrame? SELECTED_PLACED_MAP_FRAME = null;

        private static Font SELECTED_LABEL_FONT = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private FontSelectionDialog? FONT_SELECTION_DIALOG = null;

        private static SymbolTypeEnum SELECTED_SYMBOL_TYPE = SymbolTypeEnum.NotSet;

        private static int SELECTED_BRUSH_SIZE = 0;

        private static float PLACEMENT_RATE = 1.0F;
        private static float PLACEMENT_DENSITY = 1.0F;
        private static float DrawingZoom = 1.0f;

        private static SKPath CURRENT_MAP_LABEL_PATH = new();
        private static List<SKPoint> CURRENT_MAP_LABEL_PATH_POINTS = [];

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);

        private static SKPoint PREVIOUS_CURSOR_POINT = new(0, 0);

        private static readonly ToolTip TOOLTIP = new();

        private TextBox? LABEL_TEXT_BOX;

        private static bool AUTOSAVE = true;
        private static bool SYMBOL_SCALE_LOCKED = false;
        private static bool CREATING_LABEL = false;
        // 

        #region Constructor
        /******************************************************************************************************* 
        * MAIN FORM CONSTRUCTOR
        *******************************************************************************************************/
        public RealmStudioMainForm()
        {
            InitializeComponent();
            SKGLRenderControl.Hide();
            SKGLRenderControl.MouseWheel += SKGLRenderControl_MouseWheel;
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
        }

        private void RealmStudioMainForm_Shown(object sender, EventArgs e)
        {
            MapBuilder.DisposeMap(CURRENT_MAP);

            // this creates the CURRENT_MAP
            DialogResult result = OpenRealmConfigurationDialog();

            if (result == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;

                SetDrawingModeLabel();

                int assetCount = AssetManager.LoadAllAssets();

                PopulateControlsWithAssets(assetCount);

                LogoPictureBox.Hide();
                SKGLRenderControl.Show();
                SKGLRenderControl.Select();
                SKGLRenderControl.Refresh();
                SKGLRenderControl.Invalidate();

                Activate();
            }

            Cursor = Cursors.Default;
        }

        private void RealmStudioMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SymbolMethods.SaveSymbolTags();
            //SymbolMethods.SaveCollections();

            //NAME_GENERATOR_SETTINGS_DIALOG.Close();

            if (!CURRENT_MAP.IsSaved)
            {
                DialogResult result =
                    MessageBox.Show("The map has not been saved. Do you want to save the map?", "Exit Application", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    DialogResult saveResult = SaveMap();

                    if (saveResult == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        MapBuilder.DisposeMap(CURRENT_MAP);
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                MapBuilder.DisposeMap(CURRENT_MAP);
            }
        }

        private void AutosaveSwitch_Click(object sender, EventArgs e)
        {
            AUTOSAVE = AutosaveSwitch.Checked;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // save the map
            SaveMap();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            DrawingZoom = 1.0F;

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
            e.SuppressKeyPress = true;
        }

        #endregion

        #region Main Menu Event Handlers

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
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
                        // this creates the CURRENT_MAP
                        MapBuilder.DisposeMap(CURRENT_MAP);

                        // this creates the CURRENT_MAP
                        DialogResult newResult = OpenRealmConfigurationDialog();

                        if (newResult == DialogResult.OK)
                        {
                            Cursor = Cursors.WaitCursor;

                            SetDrawingModeLabel();

                            int assetCount = AssetManager.LoadAllAssets();

                            PopulateControlsWithAssets(assetCount);

                            LogoPictureBox.Hide();
                            SKGLRenderControl.Show();
                            SKGLRenderControl.Select();
                            SKGLRenderControl.Refresh();
                            SKGLRenderControl.Invalidate();

                            Activate();
                        }

                        Cursor = Cursors.Default;
                    }
                }
                else if (result == DialogResult.No)
                {
                    // this creates the CURRENT_MAP
                    MapBuilder.DisposeMap(CURRENT_MAP);

                    // this creates the CURRENT_MAP
                    DialogResult newResult = OpenRealmConfigurationDialog();

                    if (newResult == DialogResult.OK)
                    {
                        Cursor = Cursors.WaitCursor;

                        SetDrawingModeLabel();

                        int assetCount = AssetManager.LoadAllAssets();

                        PopulateControlsWithAssets(assetCount);

                        LogoPictureBox.Hide();
                        SKGLRenderControl.Show();
                        SKGLRenderControl.Select();
                        SKGLRenderControl.Refresh();
                        SKGLRenderControl.Invalidate();

                        Activate();
                    }

                    Cursor = Cursors.Default;
                }
            }
            else
            {
                // this creates the CURRENT_MAP
                MapBuilder.DisposeMap(CURRENT_MAP);

                // this creates the CURRENT_MAP
                DialogResult newResult = OpenRealmConfigurationDialog();

                if (newResult == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;

                    SetDrawingModeLabel();

                    int assetCount = AssetManager.LoadAllAssets();

                    PopulateControlsWithAssets(assetCount);

                    LogoPictureBox.Hide();
                    SKGLRenderControl.Show();
                    SKGLRenderControl.Select();
                    SKGLRenderControl.Refresh();
                    SKGLRenderControl.Invalidate();

                    Activate();
                }

                Cursor = Cursors.Default;
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PrintPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
                        MapBuilder.DisposeMap(CURRENT_MAP);
                        Application.Exit();
                    }
                }
                else if (result == DialogResult.No)
                {
                    CURRENT_MAP.IsSaved = true;
                    MapBuilder.DisposeMap(CURRENT_MAP);
                    Application.Exit();
                }
            }
            else
            {
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

        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MapPropertiesMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ChangeMapSizeMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void CreateDetailMapMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void CreateSymbolCollectionMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void WDAssetZipFileMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void WDUserFolderMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ReloadAllAssetsMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PreferencesMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void HelpContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Main Form Methods
        /******************************************************************************************************* 
         * MAIN FORM METHODS
         *******************************************************************************************************/

        private DialogResult OpenRealmConfigurationDialog()
        {
            RealmConfiguration rcd = new();
            DialogResult result = rcd.ShowDialog();

            if (result == DialogResult.OK)
            {
                // create the map from the settings on the dialog
                CURRENT_MAP = MapBuilder.CreateMap(rcd.map);

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
                MapVignette vignette = new(CURRENT_MAP)
                {
                    VignetteColor = VignetteColorSelectionButton.BackColor,
                    VignetteStrength = VignetteStrengthTrack.Value
                };

                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Add(vignette);
            }

            return result;
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear the drawing mode (and uncheck all drawing, paint, and erase buttons) when switching tabs
            CURRENT_DRAWING_MODE = DrawingModeEnum.None;
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
                    LandToolPanel.Visible = true;
                    BackgroundToolPanel.Visible = false;
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

            // TODO: apply default theme in main form
            if (AssetManager.CURRENT_THEME != null)
            {
                ThemeFilter tf = new();
                ApplyTheme(AssetManager.CURRENT_THEME, tf);
            }
        }

        public void SetStatusText(string text)
        {
            // set the test of the first status strip text box
            ApplicationStatusStrip.Items[0].Text = text;
        }

        private void UpdateDrawingPointLabel(SKPoint cursorPoint, SKPoint mapPoint)
        {
            DrawingPointLabel.Text = "Cursor Point: "
                + ((int)cursorPoint.X).ToString()
                + " , "
                + ((int)cursorPoint.Y).ToString()
                + "   Map Point: "
                + ((int)mapPoint.X).ToString()
                + " , "
                + ((int)mapPoint.Y).ToString();

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

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.None:
                    modeText += "None";
                    break;
                case DrawingModeEnum.LandPaint:
                    modeText += "Landform Paint";
                    break;
                case DrawingModeEnum.LandErase:
                    modeText += "Landform Erase";
                    break;
                case DrawingModeEnum.LandColorErase:
                    modeText += "Landform Color Erase";
                    break;
                case DrawingModeEnum.LandColor:
                    modeText += "Landform Color";
                    break;
                case DrawingModeEnum.OceanErase:
                    modeText += "Ocean Erase";
                    break;
                case DrawingModeEnum.OceanPaint:
                    modeText += "Ocean Paint";
                    break;
                case DrawingModeEnum.ColorSelect:
                    modeText += "Color Select";
                    break;
                case DrawingModeEnum.LandformSelect:
                    modeText += "Landform Select";
                    break;
                case DrawingModeEnum.WaterPaint:
                    modeText += "Water Feature Paint";
                    break;
                case DrawingModeEnum.WaterErase:
                    modeText += "Water Feature Erase";
                    break;
                case DrawingModeEnum.WaterColor:
                    modeText += "Water Feature Color";
                    break;
                case DrawingModeEnum.WaterColorErase:
                    modeText += "Water Feature Color Erase";
                    break;
                case DrawingModeEnum.LakePaint:
                    modeText += "Lake Paint";
                    break;
                case DrawingModeEnum.RiverPaint:
                    modeText += "River Paint";
                    break;
                case DrawingModeEnum.WaterFeatureSelect:
                    modeText += "Water Feature Select";
                    break;
                case DrawingModeEnum.PathPaint:
                    modeText += "Draw Path";
                    break;
                case DrawingModeEnum.PathSelect:
                    modeText += "Select Path";
                    break;
                case DrawingModeEnum.PathEdit:
                    modeText += "Edit Path";
                    break;
                case DrawingModeEnum.SymbolErase:
                    modeText += "Erase Symbol";
                    break;
                case DrawingModeEnum.SymbolPlace:
                    modeText += "Place Symbol";
                    break;
                case DrawingModeEnum.SymbolSelect:
                    modeText += "Select Symbol";
                    break;
                case DrawingModeEnum.SymbolColor:
                    modeText += "Symbol Color";
                    break;
                case DrawingModeEnum.DrawBezierLabelPath:
                    modeText += "Draw Bezier Label Path";
                    break;
                case DrawingModeEnum.DrawArcLabelPath:
                    modeText += "Draw Arc Label Path";
                    break;
                case DrawingModeEnum.DrawLabel:
                    modeText += "Place Label";
                    break;
                case DrawingModeEnum.LabelSelect:
                    modeText += "Select Label";
                    break;
                case DrawingModeEnum.DrawBox:
                    modeText += "Draw Box";
                    break;
                case DrawingModeEnum.PlaceWindrose:
                    modeText += "Place Windrose";
                    break;
                case DrawingModeEnum.SelectMapScale:
                    modeText += "Move Map Scale";
                    break;
                case DrawingModeEnum.DrawMapMeasure:
                    modeText += "Draw Map Measure";
                    break;
                case DrawingModeEnum.RegionPaint:
                    modeText += "Draw Region";
                    break;
                case DrawingModeEnum.RegionSelect:
                    modeText += "Select Region";
                    break;
                case DrawingModeEnum.LandformAreaSelect:
                    modeText += "Select Landform Area";
                    break;
                default:
                    modeText += "Undefined";
                    break;
            }

            /*
            modeText += ". Selected Brush: ";

            switch (MapPaintMethods.GetSelectedColorBrushType())
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
            */

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
                if (selectedComponent != null)
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

            /*
            foreach (PlacedMapBox b in MapLabelMethods.MAP_BOXES)
            {
                if (selectedComponent != null && selectedComponent is PlacedMapBox mapBox && mapBox == b) continue;
                b.IsSelected = false;
            }

            foreach (MapRegion r in MapRegionMethods.MAP_REGION_LIST)
            {
                if (selectedComponent != null && selectedComponent is MapRegion region && region == r) continue;
                r.IsSelected = false;
            }
            */
        }

        private static void SetSelectedBrushSize(int brushSize)
        {
            SELECTED_BRUSH_SIZE = brushSize;
        }

        private void DrawCursor(SKPoint point, int brushSize)
        {
            MapLayer cursorLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.CURSORLAYER);
            cursorLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.SymbolPlace:
                    {
                        if (SymbolMethods.SelectedSymbolTableMapSymbol != null && !AreaBrushSwitch.Checked)
                        {
                            SKBitmap? symbolBitmap = SymbolMethods.SelectedSymbolTableMapSymbol.ColorMappedBitmap;
                            if (symbolBitmap != null)
                            {
                                float symbolScale = (float)(SymbolScaleTrack.Value / 100.0F * DrawingZoom);
                                float symbolRotation = SymbolRotationTrack.Value;
                                SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleBitmap(symbolBitmap, symbolScale);

                                SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateBitmap(scaledSymbolBitmap, symbolRotation, MirrorSymbolSwitch.Checked);

                                if (rotatedAndScaledBitmap != null)
                                {
                                    cursorLayer.LayerSurface?.Canvas.DrawBitmap(rotatedAndScaledBitmap,
                                        new SKPoint(point.X - (rotatedAndScaledBitmap.Width / 2), point.Y - (rotatedAndScaledBitmap.Height / 2)), null);
                                }
                            }
                        }
                        else if (AreaBrushSwitch.Checked)
                        {
                            cursorLayer.LayerSurface?.Canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                        else
                        {
                            cursorLayer.LayerSurface?.Canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                    }
                    break;
                default:
                    {
                        if (brushSize > 0)
                        {
                            cursorLayer.LayerSurface?.Canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
                        }
                    }
                    break;
            }

        }
        #endregion

        #region Map File Open and Save Methods
        /******************************************************************************************************* 
         * MAP FILE OPEN AND SAVE METHODS
         *******************************************************************************************************/

        private static DialogResult SaveMap()
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
                    try
                    {
                        MapFileMethods.SaveMap(CURRENT_MAP);
                        CURRENT_MAP.IsSaved = true;
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

        #endregion

        #region Theme Methods

        private void ApplyTheme(MapTheme theme, ThemeFilter themeFilter)
        {
            if (theme == null || themeFilter == null) return;

            if (themeFilter.ApplyBackgroundSettings)
            {
                if (theme.BackgroundTexture != null)
                {
                    // background texture
                    if (AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap == null)
                    {
                        AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.BACKGROUND_TEXTURE_LIST.First().TexturePath);
                    }

                    BackgroundTextureBox.Image = AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap;
                    BackgroundTextureNameLabel.Text = AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureName;

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

                    BackgroundTextureBox.Image = AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap;
                    BackgroundTextureNameLabel.Text = AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureName;
                }

                if (theme.VignetteColor != null)
                {
                    VignetteColorSelectionButton.BackColor = (Color)theme.VignetteColor;
                    VignetteColorSelectionButton.Refresh();
                }

                if (theme.VignetteStrength != null)
                {
                    VignetteStrengthTrack.Value = (int)theme.VignetteStrength;
                    VignetteStrengthTrack.Refresh();
                }

                for (int i = 0; i < MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
                {
                    if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                    {
                        v.VignetteColor = VignetteColorSelectionButton.BackColor;
                        v.VignetteStrength = VignetteStrengthTrack.Value;
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

                if (theme.OceanTextureOpacity != null)
                {
                    OceanTextureOpacityTrack.Value = (int)theme.OceanTextureOpacity;
                    OceanTextureOpacityTrack.Refresh();
                }

                if (theme.OceanColor != null)
                {
                    OceanColorSelectButton.BackColor = (Color)theme.OceanColor;
                    OceanColorSelectButton.Refresh();
                }
            }

            if (themeFilter.ApplyOceanColorPaletteSettings)
            {
                if (theme.OceanColorPalette.Count > 0)
                {
                    OceanCustomColorButton1.BackColor = theme.OceanColorPalette[0];
                    OceanCustomColorButton1.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton1.Text = ColorTranslator.ToHtml(OceanCustomColorButton1.BackColor);

                    OceanCustomColorButton1.Refresh();
                }

                if (theme.OceanColorPalette.Count > 1)
                {
                    OceanCustomColorButton2.BackColor = theme.OceanColorPalette[1];
                    OceanCustomColorButton2.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton2.Text = ColorTranslator.ToHtml(OceanCustomColorButton2.BackColor);

                    OceanCustomColorButton2.Refresh();
                }

                if (theme.OceanColorPalette.Count > 2)
                {
                    OceanCustomColorButton3.BackColor = theme.OceanColorPalette[2];
                    OceanCustomColorButton3.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton3.Text = ColorTranslator.ToHtml(OceanCustomColorButton3.BackColor);

                    OceanCustomColorButton3.Refresh();
                }

                if (theme.OceanColorPalette.Count > 3)
                {
                    OceanCustomColorButton4.BackColor = theme.OceanColorPalette[3];
                    OceanCustomColorButton4.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton4.Text = ColorTranslator.ToHtml(OceanCustomColorButton4.BackColor);

                    OceanCustomColorButton4.Refresh();
                }

                if (theme.OceanColorPalette.Count > 4)
                {
                    OceanCustomColorButton5.BackColor = theme.OceanColorPalette[4];
                    OceanCustomColorButton5.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton5.Text = ColorTranslator.ToHtml(OceanCustomColorButton5.BackColor);

                    OceanCustomColorButton5.Refresh();
                }

                if (theme.OceanColorPalette.Count > 5)
                {
                    OceanCustomColorButton6.BackColor = theme.OceanColorPalette[5];
                    OceanCustomColorButton6.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton6.Text = ColorTranslator.ToHtml(OceanCustomColorButton6.BackColor);

                    OceanCustomColorButton6.Refresh();
                }

                if (theme.OceanColorPalette.Count > 6)
                {
                    OceanCustomColorButton7.BackColor = theme.OceanColorPalette[6];
                    OceanCustomColorButton7.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton7.Text = ColorTranslator.ToHtml(OceanCustomColorButton7.BackColor);

                    OceanCustomColorButton7.Refresh();
                }

                if (theme.OceanColorPalette.Count > 7)
                {
                    OceanCustomColorButton8.BackColor = theme.OceanColorPalette[7];
                    OceanCustomColorButton8.ForeColor = SystemColors.ControlDark;
                    OceanCustomColorButton8.Text = ColorTranslator.ToHtml(OceanCustomColorButton8.BackColor);

                    OceanCustomColorButton8.Refresh();
                }
            }

            if (themeFilter.ApplyLandSettings)
            {
                if (theme.LandformOutlineColor != null)
                {
                    LandformOutlineColorSelectButton.BackColor = (Color)theme.LandformOutlineColor;
                    LandformOutlineColorSelectButton.Refresh();
                }

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


                if (theme.LandformCoastlineColor != null)
                {
                    CoastlineColorSelectionButton.BackColor = (Color)theme.LandformCoastlineColor;
                    CoastlineColorSelectionButton.Refresh();
                }

                if (theme.LandformCoastlineEffectDistance != null)
                {
                    CoastlineEffectDistanceTrack.Value = (int)theme.LandformCoastlineEffectDistance;
                    CoastlineEffectDistanceTrack.Refresh();
                }

                if (theme.LandformCoastlineStyle != null)
                {
                    for (int i = 0; i < CoastlineStyleList.Items.Count; i++)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (CoastlineStyleList.Items[i].ToString() == theme.LandformCoastlineStyle)
                        {
                            CoastlineStyleList.SelectedIndex = i;
                            break;
                        }

                        CoastlineStyleList.Refresh();

#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                }
            }

            if (themeFilter.ApplyLandformColorPaletteSettings)
            {
                if (theme.LandformColorPalette.Count > 0)
                {
                    LandCustomColorButton1.BackColor = theme.LandformColorPalette[0];
                    LandCustomColorButton1.ForeColor = SystemColors.ControlDark;
                    LandCustomColorButton1.Text = ColorTranslator.ToHtml(LandCustomColorButton1.BackColor);

                    LandCustomColorButton1.Refresh();
                }

                if (theme.LandformColorPalette.Count > 1)
                {
                    LandCustomColorButton2.BackColor = theme.LandformColorPalette[1];
                    LandCustomColorButton2.ForeColor = SystemColors.ControlDark;
                    LandCustomColorButton2.Text = ColorTranslator.ToHtml(LandCustomColorButton2.BackColor);

                    LandCustomColorButton2.Refresh();
                }

                if (theme.LandformColorPalette.Count > 2)
                {
                    LandCustomColorButton3.BackColor = theme.LandformColorPalette[2];
                    LandCustomColorButton3.ForeColor = SystemColors.ControlDark;
                    LandCustomColorButton3.Text = ColorTranslator.ToHtml(LandCustomColorButton3.BackColor);

                    LandCustomColorButton3.Refresh();
                }

                if (theme.LandformColorPalette.Count > 3)
                {
                    LandCustomColorButton4.BackColor = theme.LandformColorPalette[3];
                    LandCustomColorButton4.ForeColor = SystemColors.ControlDark;
                    LandCustomColorButton4.Text = ColorTranslator.ToHtml(LandCustomColorButton4.BackColor);

                    LandCustomColorButton4.Refresh();
                }

                if (theme.LandformColorPalette.Count > 4)
                {
                    LandCustomColorButton5.BackColor = theme.LandformColorPalette[4];
                    LandCustomColorButton5.ForeColor = SystemColors.ControlDark;
                    LandCustomColorButton5.Text = ColorTranslator.ToHtml(LandCustomColorButton5.BackColor);

                    LandCustomColorButton5.Refresh();
                }

                if (theme.LandformColorPalette.Count > 5)
                {
                    LandCustomColorButton6.BackColor = theme.LandformColorPalette[5];
                    LandCustomColorButton6.ForeColor = SystemColors.ControlDark;
                    LandCustomColorButton6.Text = ColorTranslator.ToHtml(LandCustomColorButton6.BackColor);

                    LandCustomColorButton6.Refresh();
                }
            }

            if (themeFilter.ApplyFreshwaterSettings)
            {
                if (theme.FreshwaterColor != null)
                {
                    WaterColorSelectionButton.BackColor = (Color)theme.FreshwaterColor;
                    WaterColorSelectionButton.Refresh();
                }
                else
                {
                    WaterColorSelectionButton.BackColor = WaterFeatureMethods.DEFAULT_WATER_COLOR;
                    WaterColorSelectionButton.Refresh();
                }

                if (theme.FreshwaterShorelineColor != null)
                {
                    ShorelineColorSelectionButton.BackColor = (Color)theme.FreshwaterShorelineColor;
                    ShorelineColorSelectionButton.Refresh();
                }
                else
                {
                    ShorelineColorSelectionButton.BackColor = WaterFeatureMethods.DEFAULT_WATER_OUTLINE_COLOR;
                    ShorelineColorSelectionButton.Refresh();
                }

                if (theme.RiverWidth != null)
                {
                    RiverWidthTrack.Value = (int)theme.RiverWidth;
                    RiverWidthTrack.Refresh();
                }

                if (theme.RiverSourceFadeIn != null)
                {
                    RiverSourceFadeInSwitch.Checked = (bool)theme.RiverSourceFadeIn;
                }
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
                if (theme.FreshwaterColorPalette.Count > 0)
                {
                    WaterCustomColor1.BackColor = theme.FreshwaterColorPalette[0];
                    WaterCustomColor1.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor1.Text = ColorTranslator.ToHtml(WaterCustomColor1.BackColor);

                    WaterCustomColor1.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 1)
                {
                    WaterCustomColor2.BackColor = theme.FreshwaterColorPalette[1];
                    WaterCustomColor2.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor2.Text = ColorTranslator.ToHtml(WaterCustomColor2.BackColor);

                    WaterCustomColor2.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 2)
                {
                    WaterCustomColor3.BackColor = theme.FreshwaterColorPalette[2];
                    WaterCustomColor3.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor3.Text = ColorTranslator.ToHtml(WaterCustomColor3.BackColor);

                    WaterCustomColor3.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 3)
                {
                    WaterCustomColor4.BackColor = theme.FreshwaterColorPalette[3];
                    WaterCustomColor4.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor4.Text = ColorTranslator.ToHtml(WaterCustomColor4.BackColor);

                    WaterCustomColor4.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 4)
                {
                    WaterCustomColor5.BackColor = theme.FreshwaterColorPalette[4];
                    WaterCustomColor5.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor5.Text = ColorTranslator.ToHtml(WaterCustomColor5.BackColor);

                    WaterCustomColor5.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 5)
                {
                    WaterCustomColor6.BackColor = theme.FreshwaterColorPalette[5];
                    WaterCustomColor6.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor6.Text = ColorTranslator.ToHtml(WaterCustomColor6.BackColor);

                    WaterCustomColor6.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 6)
                {
                    WaterCustomColor7.BackColor = theme.FreshwaterColorPalette[6];
                    WaterCustomColor7.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor7.Text = ColorTranslator.ToHtml(WaterCustomColor7.BackColor);

                    WaterCustomColor7.Refresh();
                }

                if (theme.FreshwaterColorPalette.Count > 7)
                {
                    WaterCustomColor8.BackColor = theme.FreshwaterColorPalette[7];
                    WaterCustomColor8.ForeColor = SystemColors.ControlDark;
                    WaterCustomColor8.Text = ColorTranslator.ToHtml(WaterCustomColor8.BackColor);

                    WaterCustomColor8.Refresh();
                }
            }

            if (themeFilter.ApplyPathSetSettings)
            {
                if (theme.PathColor != null && !((Color)theme.PathColor).IsEmpty)
                {
                    PathColorSelectButton.BackColor = (Color)theme.PathColor;
                    PathColorSelectButton.Refresh();
                }

                if (theme.PathWidth != null)
                {
                    PathWidthTrack.Value = (int)theme.PathWidth;
                    PathWidthTrack.Refresh();
                }
            }

            if (themeFilter.ApplySymbolSettings)
            {
                if (theme.SymbolCustomColors != null && theme.SymbolCustomColors[0] != Color.Empty)
                {
                    SymbolColor1Button.BackColor = theme.SymbolCustomColors[0];
                    SymbolColor1Button.Refresh();
                }

                if (theme.SymbolCustomColors != null && theme.SymbolCustomColors[1] != Color.Empty)
                {
                    SymbolColor2Button.BackColor = theme.SymbolCustomColors[1];
                    SymbolColor2Button.Refresh();
                }

                if (theme.SymbolCustomColors != null && theme.SymbolCustomColors[2] != Color.Empty)
                {
                    SymbolColor3Button.BackColor = theme.SymbolCustomColors[2];
                    SymbolColor3Button.Refresh();
                }
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
        }

        #endregion

        #region ScrollBar Event Handlers
        /******************************************************************************************************* 
        * SCROLLBAR EVENT HANDLERS
        *******************************************************************************************************/

        private void MapRenderVScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.Y = (float)-e.NewValue;
            DrawingPoint.Y = (float)e.NewValue;

            SKGLRenderControl.Invalidate();
        }

        private void MapRenderHScroll_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollPoint.X = (float)-e.NewValue;
            DrawingPoint.X = (float)e.NewValue;

            SKGLRenderControl.Invalidate();
        }
        #endregion

        #region SKGLRenderControl Event Handlers
        /******************************************************************************************************* 
        * SKGLCONTROL EVENT HANDLERS
        *******************************************************************************************************/
        private void SKGLRenderControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            // TODO: can rendering be reworked similar to what is described
            // here: https://stackoverflow.com/questions/64737621/c-sharp-skiasharp-opentk-winform-how-to-draw-from-a-background-thread
            // to improve performance of rendering? (rendering when many (1000's) of symbols
            // have been added is slow)


            // create the map layers, if needed
            MapRenderMethods.CreateMapLayers(CURRENT_MAP, SKGLRenderControl.GRContext);

            // handle zoom-in and zoom-out (TODO: zoom in and out from center of map - how?)
            e.Surface.Canvas.Scale(DrawingZoom);

            // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
            e.Surface.Canvas.Clear(SKColors.Black);

            // The order that the layers are rendered here matters;
            // each layer has to be rendered separately in the correct order
            MapRenderMethods.ClearSelectionLayer(CURRENT_MAP);

            MapRenderMethods.RenderBackground(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderOcean(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderWindroses(CURRENT_MAP, CURRENT_WINDROSE, e, ScrollPoint);

            // TODO: lower grid layer (above ocean)
            MapRenderMethods.RenderLowerGrid(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderLandforms(CURRENT_MAP, CURRENT_LANDFORM, e, ScrollPoint);

            MapRenderMethods.RenderWaterFeatures(CURRENT_MAP, CURRENT_WATERFEATURE, CURRENT_RIVER, e, ScrollPoint);

            // TODO: upper grid layer (above water features)
            MapRenderMethods.RenderUpperGrid(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderLowerMapPaths(CURRENT_MAP, CURRENT_PATH, e, ScrollPoint);

            MapRenderMethods.RenderSymbols(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderUpperMapPaths(CURRENT_MAP, CURRENT_PATH, e, ScrollPoint);

            // TODO: region layer

            // TODO: region overlay layer
            MapRenderMethods.RenderRegions(CURRENT_MAP, e, ScrollPoint);

            // box layer
            MapRenderMethods.RenderBoxes(CURRENT_MAP, e, ScrollPoint);

            // label layer
            MapRenderMethods.RenderLabels(CURRENT_MAP, e, ScrollPoint);

            // TODO: default grid layer
            MapRenderMethods.RenderDefaultGrid(CURRENT_MAP, e, ScrollPoint);

            // render frame
            MapRenderMethods.RenderFrame(CURRENT_MAP, e, ScrollPoint);

            // TODO: measure layer
            MapRenderMethods.RenderMeasures(CURRENT_MAP, e, ScrollPoint);

            // TODO: drawing layer
            MapRenderMethods.RenderDrawing(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderVignette(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderSelections(CURRENT_MAP, e, ScrollPoint);

            MapRenderMethods.RenderCursor(CURRENT_MAP, e, ScrollPoint);

            // work layer
            MapRenderMethods.RenderWorkLayer(CURRENT_MAP, e, ScrollPoint);
        }

        private void SKGLRenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            // objects are created and/or initialized on mouse down
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseDownHandler(sender, e, SELECTED_BRUSH_SIZE);
            }

            if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseDownHandler(sender, e, SELECTED_BRUSH_SIZE);
            }

        }

        private void SKGLRenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);
            Task.Run(() => UpdateDrawingPointLabel(e.Location.ToSKPoint(), zoomedScrolledPoint));

            // objects are drawn or moved on mouse move
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseMoveHandler(e, SELECTED_BRUSH_SIZE / 2);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseMoveHandler(sender, e);
            }
            else if (e.Button == MouseButtons.None)
            {
                NoButtonMouseMoveHandler(sender, e, SELECTED_BRUSH_SIZE / 2);
            }

            DrawCursor(zoomedScrolledPoint, SELECTED_BRUSH_SIZE);

            SKGLRenderControl.Invalidate();
        }

        private void SKGLRenderControl_MouseUp(object sender, MouseEventArgs e)
        {
            // objects are finalized or reset on mouse up
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseUpHandler(sender, e);
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
            Cursor = Cursors.Cross;
        }

        private void SKGLRenderControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void SKGLRenderControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            int cursorDelta = 5;

            if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.LandErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandEraserSizeTrack.Minimum, Math.Min(newValue, LandEraserSizeTrack.Maximum));

                // landform eraser
                LandEraserSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.LandPaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = LandBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(LandBrushSizeTrack.Minimum, Math.Min(newValue, LandBrushSizeTrack.Maximum));

                LandBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.LandColor)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                //int newValue = LandColorBrushSizeTrack.Value + sizeDelta;
                //newValue = Math.Max(LandColorBrushSizeTrack.Minimum, Math.Min(newValue, LandColorBrushSizeTrack.Maximum));

                //LandColorBrushSizeTrack.Value = newValue;
                //SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.LandColorErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                //int newValue = LandColorEraserSizeTrack.Value + sizeDelta;
                //newValue = Math.Max(LandColorEraserSizeTrack.Minimum, Math.Min(newValue, LandColorEraserSizeTrack.Maximum));

                // land color eraser
                //LandColorEraserSizeTrack.Value = newValue;
                //SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.OceanErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                //int newValue = OceanBrushSizeTrack.Value + sizeDelta;
                //newValue = Math.Max(OceanBrushSizeTrack.Minimum, Math.Min(newValue, OceanBrushSizeTrack.Maximum));

                //OceanEraserSizeTrack.Value = newValue;
                //SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.OceanPaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                //int newValue = OceanBrushSizeTrack.Value + sizeDelta;
                //newValue = Math.Max(OceanBrushSizeTrack.Minimum, Math.Min(newValue, OceanBrushSizeTrack.Maximum));

                //OceanBrushSizeTrack.Value = newValue;
                //SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.WaterPaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterBrushSizeTrack.Minimum, Math.Min(newValue, WaterBrushSizeTrack.Maximum));

                WaterBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.WaterErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterEraserSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterEraserSizeTrack.Minimum, Math.Min(newValue, WaterEraserSizeTrack.Maximum));

                WaterEraserSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.LakePaint)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = WaterBrushSizeTrack.Value + sizeDelta;
                newValue = Math.Max(WaterBrushSizeTrack.Minimum, Math.Min(newValue, WaterBrushSizeTrack.Maximum));

                WaterBrushSizeTrack.Value = newValue;
                SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.WaterColor)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                //int newValue = WaterColorBrushSizeTrack.Value + sizeDelta;
                //newValue = Math.Max(WaterColorBrushSizeTrack.Minimum, Math.Min(newValue, WaterColorBrushSizeTrack.Maximum));

                //WaterColorBrushSizeTrack.Value = newValue;
                //SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Control && CURRENT_DRAWING_MODE == DrawingModeEnum.WaterColorErase)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                //int newValue = WaterColorBrushSizeTrack.Value + sizeDelta;
                //newValue = Math.Max(WaterColorBrushSizeTrack.Minimum, Math.Min(newValue, WaterColorBrushSizeTrack.Maximum));

                //WaterColorBrushSizeTrack.Value = newValue;
                //SELECTED_BRUSH_SIZE = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (CURRENT_DRAWING_MODE == DrawingModeEnum.SymbolPlace)
            {
                // TODO: should area brush size be changed when AreaBrushSwitch is checked?
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = (int)Math.Max(SymbolScaleUpDown.Minimum, SymbolScaleUpDown.Value + sizeDelta);
                newValue = (int)Math.Min(SymbolScaleUpDown.Maximum, newValue);

                SymbolScaleUpDown.Value = newValue;
                SKGLRenderControl.Invalidate();
            }
            else if (CURRENT_DRAWING_MODE == DrawingModeEnum.LabelSelect)
            {
                int sizeDelta = e.Delta < 0 ? -cursorDelta : cursorDelta;
                int newValue = Math.Max(LabelRotationTrack.Minimum, LabelRotationTrack.Value + sizeDelta);
                LabelRotationTrack.Value = Math.Min(LabelRotationTrack.Maximum, newValue);

                SKGLRenderControl.Invalidate();
            }
            else if (ModifierKeys == Keys.Shift)
            {
                SetZoomLevel(e.Delta);

                SKGLRenderControl.Invalidate();
            }
        }

        private void SKGLRenderControl_Enter(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
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

        private void LeftButtonMouseDownHandler(object sender, MouseEventArgs e, int brushSize)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandformSelect:
                    {
                        Cursor = Cursors.Default;

                        SelectLandformAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.LandPaint:
                    {
                        CURRENT_MAP.IsSaved = false;
                        Cursor = Cursors.Cross;
                        CURRENT_LANDFORM = new();
                        SetLandformData(CURRENT_LANDFORM);
                    }
                    break;
                case DrawingModeEnum.WaterFeatureSelect:
                    {
                        SelectWaterFeatureAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.WaterPaint:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_WATERFEATURE = new()
                        {
                            ParentMap = CURRENT_MAP,
                            WaterFeatureType = WaterFeatureTypeEnum.Other,
                            WaterFeatureColor = WaterColorSelectionButton.BackColor,
                            WaterFeatureShorelineColor = ShorelineColorSelectionButton.BackColor
                        };

                        WaterFeatureMethods.ConstructWaterFeaturePaintObjects(CURRENT_WATERFEATURE);
                    }
                    break;
                case DrawingModeEnum.LakePaint:
                    {
                        // TODO: undo/redo
                        Cursor = Cursors.Cross;

                        CURRENT_WATERFEATURE = new()
                        {
                            ParentMap = CURRENT_MAP,
                            WaterFeatureType = WaterFeatureTypeEnum.Lake,
                            WaterFeatureColor = WaterColorSelectionButton.BackColor,
                            WaterFeatureShorelineColor = ShorelineColorSelectionButton.BackColor,
                        };

                        WaterFeatureMethods.ConstructWaterFeaturePaintObjects(CURRENT_WATERFEATURE);

                        SKPath? lakePath = WaterFeatureMethods.GenerateRandomLakePath(zoomedScrolledPoint, brushSize);

                        if (lakePath != null)
                        {
                            CURRENT_WATERFEATURE.WaterFeaturePath = lakePath;
                        }
                        else
                        {
                            CURRENT_WATERFEATURE = null;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.RiverPaint:
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
                            };

                            WaterFeatureMethods.ConstructRiverPaintObjects(CURRENT_RIVER);
                            CURRENT_RIVER.RiverPoints.Add(new MapRiverPoint(zoomedScrolledPoint));
                        }
                    }
                    break;
                case DrawingModeEnum.PathPaint:
                    {
                        Cursor = Cursors.Cross;
                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                        if (CURRENT_PATH == null)
                        {
                            // initialize map path
                            CURRENT_PATH = new MapPath
                            {
                                ParentMap = CURRENT_MAP,
                                PathType = GetSelectedPathType(),
                                PathColor = PathColorSelectButton.BackColor,
                                PathWidth = PathWidthTrack.Value,
                                DrawOverSymbols = DrawOverSymbolsSwitch.Checked,
                            };

                            if (PathTexturePreviewPicture.Image != null)
                            {
                                CURRENT_PATH.PathTexture = new Bitmap(PathTexturePreviewPicture.Image, PathWidthTrack.Value, PathWidthTrack.Value).ToSKBitmap();
                            }

                            MapPathMethods.ConstructPathPaint(CURRENT_PATH);
                            CURRENT_PATH.PathPoints.Add(new MapPathPoint(zoomedScrolledPoint));
                        }
                    }
                    break;
                case DrawingModeEnum.SymbolPlace:
                    {
                        if (AreaBrushSwitch.Checked)
                        {
                            SELECTED_BRUSH_SIZE = AreaBrushSizeTrack.Value;
                            SKGLRenderControl.Invalidate();

                            float symbolScale = SymbolScaleTrack.Value / 100.0F;
                            float symbolRotation = SymbolRotationTrack.Value;
                            float areaBrushSize = AreaBrushSizeTrack.Value / 2.0F;

                            Task.Run(() => PlaceSelectedSymbolInArea(new SKPoint(zoomedScrolledPoint.X, zoomedScrolledPoint.Y), symbolScale, symbolRotation, (int)areaBrushSize));
                        }
                        else
                        {
                            PlaceSelectedSymbolAtCursor(zoomedScrolledPoint);
                        }

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                    }
                    break;
                case DrawingModeEnum.DrawArcLabelPath:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_MAP_LABEL_PATH.Dispose();
                        CURRENT_MAP_LABEL_PATH = new();

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();
                    }
                    break;
                case DrawingModeEnum.DrawBezierLabelPath:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_MAP_LABEL_PATH.Dispose();
                        CURRENT_MAP_LABEL_PATH = new();

                        CURRENT_MAP_LABEL_PATH_POINTS.Clear();

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();
                    }
                    break;
                case DrawingModeEnum.DrawBox:
                    {
                        // initialize new box
                        Cursor = Cursors.Cross;

                        PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;

                        SELECTED_PLACED_MAP_BOX = new();
                    }
                    break;
                case DrawingModeEnum.DrawMapMeasure:
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

            }
        }

        private void RightButtonMouseDownHandler(object sender, MouseEventArgs e, int brushSize)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LabelSelect:
                    {
                        if (SELECTED_MAP_LABEL != null)
                        {
                            CREATING_LABEL = true;

                            Size labelSize = TextRenderer.MeasureText(SELECTED_MAP_LABEL.LabelText, SELECTED_LABEL_FONT,
                                new Size(int.MaxValue, int.MaxValue),
                                TextFormatFlags.Default | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.ExternalLeading | TextFormatFlags.SingleLine);

                            LABEL_TEXT_BOX = new()
                            {
                                Parent = SKGLRenderControl,
                                Name = Guid.NewGuid().ToString(),
                                Left = Math.Max(0, (int)zoomedScrolledPoint.X - (labelSize.Width / 2)),
                                Top = Math.Max(0, (int)zoomedScrolledPoint.Y - (labelSize.Height / 2)),
                                Width = labelSize.Width,
                                Height = labelSize.Height,
                                Margin = System.Windows.Forms.Padding.Empty,
                                AutoSize = false,
                                Font = SELECTED_LABEL_FONT,
                                Visible = true,
                                BackColor = Color.AliceBlue,
                                ForeColor = FontColorSelectButton.BackColor,
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
                case DrawingModeEnum.DrawMapMeasure:
                    if (CURRENT_MAP_MEASURE != null)
                    {
                        CURRENT_MAP_MEASURE.MeasurePoints.Add(zoomedScrolledPoint);

                        float lineLength = SKPoint.Distance(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint);
                        CURRENT_MAP_MEASURE.TotalMeasureLength += lineLength;
                        CURRENT_MAP_MEASURE.RenderValue = true;

                        // reset everything
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(); // not sure if this needs to be done
                        PREVIOUS_CURSOR_POINT = SKPoint.Empty;

                        CURRENT_MAP_MEASURE = null;
                        CURRENT_DRAWING_MODE = DrawingModeEnum.None;
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
                case DrawingModeEnum.LandPaint:
                    {
                        Cursor = Cursors.Cross;

                        if (CURRENT_LANDFORM != null)
                        {
                            CURRENT_LANDFORM.DrawPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                            // compute contour path and inner and outer paths in a separate thread

                            // TODO: can the creation of paths be divided up to more tasks?
                            // perhaps await a task to create the contour path, then when it is complete,
                            // create separate tasks to compute each inner and outer path
                            Task.Run(() => LandformMethods.CreateInnerAndOuterPaths(CURRENT_MAP, CURRENT_LANDFORM));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        LandformMethods.LandformErasePath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                        LandformMethods.EraseLandForm(CURRENT_MAP);
                        SKGLRenderControl.Invalidate();

                    }
                    break;
                case DrawingModeEnum.WaterPaint:
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
                case DrawingModeEnum.RiverPaint:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_RIVER?.RiverPoints.Add(new MapRiverPoint(zoomedScrolledPoint));

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.PathPaint:
                    {
                        Cursor = Cursors.Cross;

                        CURRENT_PATH?.PathPoints.Add(new MapPathPoint(zoomedScrolledPoint));

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.PathSelect:
                    {
                        if (SELECTED_PATH != null)
                        {
                            SizeF delta = new()
                            {
                                Width = zoomedScrolledPoint.X - PREVIOUS_CURSOR_POINT.X,
                                Height = zoomedScrolledPoint.Y - PREVIOUS_CURSOR_POINT.Y,
                            };

                            foreach (MapPathPoint point in SELECTED_PATH.PathPoints)
                            {
                                SKPoint p = point.MapPoint;
                                p.X += delta.Width;
                                p.Y += delta.Height;
                                point.MapPoint = p;
                            }

                            SELECTED_PATH.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(SELECTED_PATH.PathPoints);
                            PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                        }
                    }
                    break;
                case DrawingModeEnum.PathEdit:
                    if (SELECTED_PATHPOINT != null)
                    {
                        MapPathMethods.MoveSelectedMapPathPoint(SELECTED_PATH, SELECTED_PATHPOINT, zoomedScrolledPoint);

                        CURRENT_MAP.IsSaved = false;
                    }
                    break;
                case DrawingModeEnum.SymbolPlace:
                    if (AreaBrushSwitch.Checked)
                    {
                        float symbolScale = SymbolScaleTrack.Value / 100.0F;
                        float symbolRotation = SymbolRotationTrack.Value;
                        SELECTED_BRUSH_SIZE = AreaBrushSizeTrack.Value;

                        PlaceSelectedSymbolInArea(zoomedScrolledPoint, symbolScale, symbolRotation, (int)(AreaBrushSizeTrack.Value / 2.0F));
                    }

                    PREVIOUS_CURSOR_POINT = zoomedScrolledPoint;
                    SKGLRenderControl.Invalidate();
                    break;
                case DrawingModeEnum.SymbolColor:

                    int colorBrushRadius = AreaBrushSizeTrack.Value / 2;

                    Color[] symbolColors = { SymbolColor1Button.BackColor, SymbolColor2Button.BackColor, SymbolColor3Button.BackColor };
                    SymbolMethods.ColorSymbolsInArea(CURRENT_MAP, zoomedScrolledPoint, colorBrushRadius, symbolColors);

                    CURRENT_MAP.IsSaved = false;
                    break;
                case DrawingModeEnum.SymbolSelect:
                    if (SELECTED_MAP_SYMBOL != null && SELECTED_MAP_SYMBOL.IsSelected)
                    {
                        SELECTED_MAP_SYMBOL.X = (int)zoomedScrolledPoint.X - SELECTED_MAP_SYMBOL.Width / 2;
                        SELECTED_MAP_SYMBOL.Y = (int)zoomedScrolledPoint.Y - SELECTED_MAP_SYMBOL.Height / 2;

                        CURRENT_MAP.IsSaved = false;
                    }

                    break;
                case DrawingModeEnum.LabelSelect:
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
                        SELECTED_PLACED_MAP_BOX.X = (int)zoomedScrolledPoint.X;
                        SELECTED_PLACED_MAP_BOX.Y = (int)zoomedScrolledPoint.Y;
                    }
                    CURRENT_MAP.IsSaved = false;
                    break;
                case DrawingModeEnum.DrawArcLabelPath:
                    {
                        SKRect r = new(PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);
                        CURRENT_MAP_LABEL_PATH.Dispose();
                        CURRENT_MAP_LABEL_PATH = new();

                        CURRENT_MAP_LABEL_PATH.AddArc(r, 180, 180);

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawPath(CURRENT_MAP_LABEL_PATH, PaintObjects.LabelPathPaint);
                    }
                    break;
                case DrawingModeEnum.DrawBezierLabelPath:
                    {
                        CURRENT_MAP_LABEL_PATH_POINTS.Add(zoomedScrolledPoint);
                        ConstructBezierPathFromPoints();

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.DrawPath(CURRENT_MAP_LABEL_PATH, PaintObjects.LabelPathPaint);
                    }
                    break;
                case DrawingModeEnum.DrawBox:
                    // draw box as mouse is moved
                    if (SELECTED_MAP_BOX != null)
                    {
                        SKRect boxRect = new(PREVIOUS_CURSOR_POINT.X, PREVIOUS_CURSOR_POINT.Y, zoomedScrolledPoint.X, zoomedScrolledPoint.Y);

                        if (boxRect.Width > 0 && boxRect.Height > 0)
                        {
                            Bitmap? b = SELECTED_MAP_BOX.BoxBitmap;

                            if (b != null)
                            {
                                using Bitmap resizedBitmap = new(b, (int)boxRect.Width, (int)boxRect.Height);

                                SELECTED_PLACED_MAP_BOX ??= new();

                                SELECTED_PLACED_MAP_BOX.SetBoxBitmap(new(resizedBitmap));
                                SELECTED_PLACED_MAP_BOX.X = (int)PREVIOUS_CURSOR_POINT.X;
                                SELECTED_PLACED_MAP_BOX.Y = (int)PREVIOUS_CURSOR_POINT.Y;
                                SELECTED_PLACED_MAP_BOX.Width = resizedBitmap.Width;
                                SELECTED_PLACED_MAP_BOX.Height = resizedBitmap.Height;

                                SELECTED_PLACED_MAP_BOX.BoxCenterLeft = SELECTED_MAP_BOX.BoxCenterLeft;
                                SELECTED_PLACED_MAP_BOX.BoxCenterTop = SELECTED_MAP_BOX.BoxCenterTop;
                                SELECTED_PLACED_MAP_BOX.BoxCenterRight = SELECTED_MAP_BOX.BoxCenterRight;
                                SELECTED_PLACED_MAP_BOX.BoxCenterBottom = SELECTED_MAP_BOX.BoxCenterBottom;

                                SELECTED_PLACED_MAP_BOX.BoxTint = SelectBoxTintButton.BackColor;

                                using SKPaint boxPaint = new()
                                {
                                    Style = SKPaintStyle.Fill,
                                    ColorFilter = SKColorFilter.CreateBlendMode(
                                        Extensions.ToSKColor(SELECTED_PLACED_MAP_BOX.BoxTint),
                                        SKBlendMode.Modulate) // combine the tint with the bitmap color
                                };

                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();

                                MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas
                                    .DrawBitmap(Extensions.ToSKBitmap(resizedBitmap), PREVIOUS_CURSOR_POINT, boxPaint);

                                SKGLRenderControl.Invalidate();
                            }
                        }
                    }
                    break;
                case DrawingModeEnum.DrawMapMeasure:

                    if (CURRENT_MAP_MEASURE != null)
                    {
                        SKCanvas? workCanvas = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas;

                        if (workCanvas == null) { return; }

                        workCanvas.Clear();

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

                            workCanvas.DrawPath(path, CURRENT_MAP_MEASURE.MeasureAreaPaint);
                        }
                        else
                        {
                            workCanvas.DrawLine(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint, CURRENT_MAP_MEASURE.MeasureLinePaint);
                        }

                        // render measure value and units
                        SKPoint measureValuePoint = new(zoomedScrolledPoint.X + 30, zoomedScrolledPoint.Y + 20);

                        float lineLength = SKPoint.Distance(PREVIOUS_CURSOR_POINT, zoomedScrolledPoint);
                        float totalLength = CURRENT_MAP_MEASURE.TotalMeasureLength + lineLength;

                        CURRENT_MAP_MEASURE.RenderDistanceLabel(workCanvas, measureValuePoint, totalLength);

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

                            CURRENT_MAP_MEASURE.RenderAreaLabel(workCanvas, measureAreaPoint, area);
                        }
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

        #region No Button Move Handler Method

        private void NoButtonMouseMoveHandler(object sender, MouseEventArgs e, int brushRadius)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.PlaceWindrose:
                    if (CURRENT_WINDROSE != null)
                    {
                        CURRENT_WINDROSE.X = (int)zoomedScrolledPoint.X;
                        CURRENT_WINDROSE.Y = (int)zoomedScrolledPoint.Y;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.PathEdit:
                    {
                        if (SELECTED_PATH != null)
                        {
                            foreach (MapPathPoint mp in SELECTED_PATH.PathPoints)
                            {
                                mp.IsSelected = false;
                            }

                            SELECTED_PATHPOINT = MapPathMethods.SelectMapPathPointAtPoint(SELECTED_PATH, zoomedScrolledPoint);
                        }
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Up Event Handling Methods (called from event handers)

        private void LeftButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);
            Cursor = Cursors.Default;

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    if (CURRENT_LANDFORM != null)
                    {
                        // TODO: undo/redo
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER).MapLayerComponents.Add(CURRENT_LANDFORM);
                        LandformMethods.MergeLandforms(CURRENT_MAP);

                        CURRENT_LANDFORM = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.PlaceWindrose:
                    if (CURRENT_WINDROSE != null)
                    {
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER).MapLayerComponents.Add(CURRENT_WINDROSE);

                        Cmd_AddWindrose cmd = new(CURRENT_MAP, CURRENT_WINDROSE);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        CURRENT_WINDROSE = CreateWindrose();

                        CURRENT_MAP.IsSaved = false;
                    }
                    break;
                case DrawingModeEnum.WaterPaint:
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
                case DrawingModeEnum.LakePaint:
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
                case DrawingModeEnum.RiverPaint:
                    if (CURRENT_RIVER != null)
                    {
                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER).MapLayerComponents.Add(CURRENT_RIVER);

                        CURRENT_RIVER = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.PathPaint:
                    if (CURRENT_PATH != null)
                    {
                        CURRENT_PATH.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(CURRENT_PATH.PathPoints);

                        if (CURRENT_PATH.DrawOverSymbols)
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Add(CURRENT_PATH);
                        }
                        else
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Add(CURRENT_PATH);
                        }

                        CURRENT_PATH = null;

                        CURRENT_MAP.IsSaved = false;

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.PathSelect:
                    {
                        SELECTED_PATH = SelectMapPathAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                    }
                    break;
                case DrawingModeEnum.PathEdit:
                    {
                        SELECTED_PATHPOINT = null;
                    }
                    break;
                case DrawingModeEnum.SymbolSelect:
                    {
                        SELECTED_MAP_SYMBOL = SelectMapSymbolAtPoint(CURRENT_MAP, zoomedScrolledPoint.ToDrawingPoint());

                        if (SELECTED_MAP_SYMBOL != null)
                        {
                            SELECTED_MAP_SYMBOL.IsSelected = !SELECTED_MAP_SYMBOL.IsSelected;
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.DrawLabel:
                    if (!CREATING_LABEL)
                    {
                        CREATING_LABEL = true;

                        Font tbFont = SELECTED_LABEL_FONT;

                        Size labelSize = TextRenderer.MeasureText("...Label...", SELECTED_LABEL_FONT,
                            new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.Default | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.ExternalLeading | TextFormatFlags.SingleLine);

                        LABEL_TEXT_BOX = new()
                        {
                            Parent = SKGLRenderControl,
                            Name = Guid.NewGuid().ToString(),
                            Left = (int)zoomedScrolledPoint.X,
                            Top = (int)zoomedScrolledPoint.Y,
                            Width = labelSize.Width,
                            Height = labelSize.Height,
                            Margin = System.Windows.Forms.Padding.Empty,
                            AutoSize = false,
                            Font = tbFont,
                            Visible = true,
                            PlaceholderText = "...Label...",
                            BackColor = Color.AliceBlue,
                            ForeColor = FontColorSelectButton.BackColor,
                            BorderStyle = BorderStyle.None,
                            Multiline = false,
                            TextAlign = HorizontalAlignment.Center,
                            Text = "...Label...",
                        };

                        LABEL_TEXT_BOX.KeyPress += LabelTextBox_KeyPress;

                        SKGLRenderControl.Controls.Add(LABEL_TEXT_BOX);

                        SKGLRenderControl.Refresh();

                        LABEL_TEXT_BOX.BringToFront();
                        LABEL_TEXT_BOX.Select(LABEL_TEXT_BOX.Text.Length, 0);
                        LABEL_TEXT_BOX.Focus();
                        LABEL_TEXT_BOX.ScrollToCaret();

                    }
                    break;
                case DrawingModeEnum.LabelSelect:
                    {
                        MapLabel? selectedLabel = SelectLabelAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                        if (selectedLabel != null)
                        {
                            bool isSelected = selectedLabel.IsSelected;

                            selectedLabel.IsSelected = !isSelected;

                            if (selectedLabel.IsSelected)
                            {
                                SELECTED_MAP_LABEL = selectedLabel;
                            }
                            else
                            {
                                SELECTED_MAP_LABEL = null;
                            }

                            SKGLRenderControl.Invalidate();
                        }
                        else
                        {
                            PlacedMapBox? selectedMapBox = SelectMapBoxAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                            if (selectedMapBox != null)
                            {
                                bool isSelected = selectedMapBox.IsSelected;
                                selectedMapBox.IsSelected = !isSelected;

                                if (selectedMapBox.IsSelected)
                                {
                                    SELECTED_PLACED_MAP_BOX = selectedMapBox;
                                }
                                else
                                {
                                    SELECTED_PLACED_MAP_BOX = null;
                                }
                            }
                        }
                    }
                    break;
                case DrawingModeEnum.DrawBox:
                    // finalize box drawing
                    if (SELECTED_PLACED_MAP_BOX != null)
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

                        SKBitmap[] bitmapSlices = DrawingMethods.SliceNinePatchBitmap(SELECTED_PLACED_MAP_BOX.BoxBitmap.ToSKBitmap(), center);

                        SELECTED_PLACED_MAP_BOX.Patch_A = bitmapSlices[0].Copy();   // top-left corner
                        SELECTED_PLACED_MAP_BOX.Patch_B = bitmapSlices[1].Copy();   // top
                        SELECTED_PLACED_MAP_BOX.Patch_C = bitmapSlices[2].Copy();   // top-right corner
                        SELECTED_PLACED_MAP_BOX.Patch_D = bitmapSlices[3].Copy();   // left size
                        SELECTED_PLACED_MAP_BOX.Patch_E = bitmapSlices[4].Copy();   // middle
                        SELECTED_PLACED_MAP_BOX.Patch_F = bitmapSlices[5].Copy();   // right side
                        SELECTED_PLACED_MAP_BOX.Patch_G = bitmapSlices[6].Copy();   // bottom-left corner
                        SELECTED_PLACED_MAP_BOX.Patch_H = bitmapSlices[7].Copy();   // bottom
                        SELECTED_PLACED_MAP_BOX.Patch_I = bitmapSlices[8].Copy();   // bottom-right corner

                        Cmd_AddLabelBox cmd = new(CURRENT_MAP, SELECTED_PLACED_MAP_BOX);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        CURRENT_MAP.IsSaved = false;
                        SELECTED_PLACED_MAP_BOX = null;

                        MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.DrawMapMeasure:
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
            }

        }

        private void RightButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandformSelect:

                    LandformSelectButton.Checked = false;

                    Landform? selectedLandform = SelectLandformAtPoint(CURRENT_MAP, zoomedScrolledPoint);
                    SKGLRenderControl.Invalidate();

                    if (selectedLandform != null)
                    {
                        //LandformData landformData = new();
                        //landformData.SetMapLandform(selectedLandform);
                        //landformData.ShowDialog(this);
                    }
                    break;
                case DrawingModeEnum.WaterFeatureSelect:
                    MapComponent? selectedWaterFeature = SelectWaterFeatureAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                    if (selectedWaterFeature != null && selectedWaterFeature is WaterFeature)
                    {
                        // TODO: info dialog for water feature
                        //MessageBox.Show("selected water feature");
                    }
                    else if (selectedWaterFeature != null && selectedWaterFeature is River)
                    {
                        // TODO: info dialog for river
                        //MessageBox.Show("selected river");
                    }
                    break;
                case DrawingModeEnum.PathSelect:
                    MapPath? selectedPath = SelectMapPathAtPoint(CURRENT_MAP, zoomedScrolledPoint);

                    if (selectedPath != null)
                    {
                        // TODO: info dialog for path
                        //MessageBox.Show("selected path");
                    }
                    break;
                case DrawingModeEnum.SymbolSelect:
                    MapSymbol? selectedSymbol = SelectMapSymbolAtPoint(CURRENT_MAP, zoomedScrolledPoint.ToDrawingPoint());
                    if (selectedSymbol != null)
                    {
                        MapSymbolInfo msi = new(selectedSymbol);
                        msi.ShowDialog();

                        SKGLRenderControl.Invalidate();
                    }
                    break;
            }
        }

        private void MiddleButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            // no middle button mouse up events yet
        }

        #endregion

        #region SKGLRenderControl KeyDown Handler
        private void SKGLRenderControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    // TODO: delete water features, rivers
                    case DrawingModeEnum.PathSelect:
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
                    case DrawingModeEnum.PathEdit:
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
                    case DrawingModeEnum.SymbolSelect:
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
                    case DrawingModeEnum.LabelSelect:
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
                }
            }

            if (e.KeyCode == Keys.PageUp)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case DrawingModeEnum.SymbolSelect:
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum.Up, 5);
                            }
                            else if (ModifierKeys == Keys.None)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum.Up, 1);
                            }

                            SKGLRenderControl.Invalidate();
                        }
                        break;
                    case DrawingModeEnum.RegionSelect:
                        {
                            //MoveSelectedRegionInRenderOrder(ComponentMoveDirectionEnum.Up);
                        }
                        break;

                }
            }

            if (e.KeyCode == Keys.PageDown)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case DrawingModeEnum.SymbolSelect:
                        {
                            if (ModifierKeys == Keys.Control)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum.Down, 5);
                            }
                            else if (ModifierKeys == Keys.None)
                            {
                                MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum.Down, 1);
                            }

                            SKGLRenderControl.Invalidate();
                        }
                        break;
                    case DrawingModeEnum.RegionSelect:
                        {
                            //MoveSelectedRegionInRenderOrder(ComponentMoveDirectionEnum.Down);
                        }
                        break;

                }
            }

            if (e.KeyCode == Keys.End)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case DrawingModeEnum.SymbolSelect:
                        {
                            // move symbol to bottom of render order
                            MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum.Down, 1, true);
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Home)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case DrawingModeEnum.SymbolSelect:
                        {
                            // move symbol to top of render order
                            MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum.Up, 1, true);
                            SKGLRenderControl.Invalidate();
                        }
                        break;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                switch (CURRENT_DRAWING_MODE)
                {
                    case DrawingModeEnum.SymbolSelect:
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
                    case DrawingModeEnum.SymbolSelect:
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
                    case DrawingModeEnum.SymbolSelect:
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
                    case DrawingModeEnum.SymbolSelect:
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
        private void NextBackgroundTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX < AssetManager.BACKGROUND_TEXTURE_LIST.Count - 1)
            {
                AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX++;
            }

            if (AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TexturePath);
            }

            BackgroundTextureBox.Image = AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap;
            BackgroundTextureNameLabel.Text = AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureName;
        }

        private void PreviousBackgroundTextureButton_Click(object sender, EventArgs e)
        {
            if (AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX > 0)
            {
                AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX--;
            }

            if (AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TexturePath);
            }

            BackgroundTextureBox.Image = AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap;
            BackgroundTextureNameLabel.Text = AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureName;
        }

        private void FillBackgroundButton_Click(object sender, EventArgs e)
        {
#pragma warning disable CS8604 // Possible null reference argument.

            CURRENT_MAP.IsSaved = false;

            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER);

            if (baseLayer.MapLayerComponents.Count < 1
                && AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap != null)
            {
                Bitmap resizedBitmap = new(AssetManager.BACKGROUND_TEXTURE_LIST[AssetManager.SELECTED_BACKGROUND_TEXTURE_INDEX].TextureBitmap,
                    CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                Cmd_SetBackgroundTexture cmd = new(CURRENT_MAP, Extensions.ToSKBitmap(resizedBitmap));
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
#pragma warning restore CS8604 // Possible null reference argument.
        }

        private void ClearBackgroundButton_Click(object sender, EventArgs e)
        {
            CURRENT_MAP.IsSaved = false;

            MapLayer backgroundLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER);

            if (backgroundLayer.MapLayerComponents.Count > 0)
            {
                MapImage layerTexture = (MapImage)MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER).MapLayerComponents.First();

                Cmd_ClearBackgroundTexture cmd = new(CURRENT_MAP, layerTexture);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void VignetteColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, VignetteColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                VignetteColorSelectionButton.BackColor = selectedColor;
                VignetteColorSelectionButton.Refresh();

                for (int i = 0; i < MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
                {
                    if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                    {
                        v.VignetteColor = selectedColor;
                    }
                }

                SKGLRenderControl.Invalidate();
            }
        }

        private void VignetteStrengthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(VignetteStrengthTrack.Value.ToString(), VignetteStrengthTrack, new Point(VignetteStrengthTrack.Right - 42, VignetteStrengthTrack.Top - 58), 2000);

            for (int i = 0; i < MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents.Count; i++)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER).MapLayerComponents[i] is MapVignette v)
                {
                    v.VignetteStrength = VignetteStrengthTrack.Value;
                }
            }

            SKGLRenderControl.Invalidate();
        }

        #endregion

        #region Ocean Tab Event Handlers
        private void OceanTextureOpacityTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(OceanTextureOpacityTrack.Value.ToString(), OceanTextureGroup, new Point(OceanTextureGroup.Left + 120, OceanTextureGroup.Bottom - 90), 2000);

            Bitmap b = new(AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);

            if (b != null)
            {
                Bitmap newB = new(b.Width, b.Height, PixelFormat.Format32bppArgb);

                using Graphics g = Graphics.FromImage(newB);

                //create a color matrix object  
                ColorMatrix matrix = new()
                {
                    //set the opacity  
                    Matrix33 = OceanTextureOpacityTrack.Value / 100F
                };

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image
                g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, attributes);

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

            OceanTextureBox.Image = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap;
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

            OceanTextureBox.Image = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap;
            OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureName;
        }

        private void OceanApplyTextureButton_Click(object sender, EventArgs e)
        {
#pragma warning disable CS8604 // Possible null reference argument
            CURRENT_MAP.IsSaved = false;

            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);

            if (oceanTextureLayer.MapLayerComponents.Count < 1
                && OceanTextureBox.Image != null)
            {
                Bitmap resizedBitmap = new(OceanTextureBox.Image, CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                Cmd_SetOceanTexture cmd = new(CURRENT_MAP, Extensions.ToSKBitmap(resizedBitmap));
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
#pragma warning restore CS8604 // Possible null reference argument.
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
        #endregion

        #region Land Tab Event Handlers
        /******************************************************************************************************* 
        * LAND TAB EVENT HANDLERS
        *******************************************************************************************************/
        private void LandformSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LandformSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void LandformPaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LandPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformBrushSize);
        }

        private void LandformEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LandErase;
            SetDrawingModeLabel();
            SetSelectedBrushSize(LandformMethods.LandformEraserSize);
        }

        private void LandBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            LandformMethods.LandformBrushSize = LandBrushSizeTrack.Value;
            TOOLTIP.Show(LandformMethods.LandformBrushSize.ToString(), LandBrushSizeTrack, new Point(LandBrushSizeTrack.Right - 42, LandBrushSizeTrack.Top - 58), 2000);
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

        private void CoastlineEffectDistanceTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(CoastlineEffectDistanceTrack.Value.ToString(), CoastlineEffectDistanceTrack, new Point(CoastlineEffectDistanceTrack.Right - 42, CoastlineEffectDistanceTrack.Top - 58), 2000);
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
            TOOLTIP.Show(LandEraserSizeTrack.Value.ToString(), LandEraserSizeTrack, new Point(LandEraserSizeTrack.Right - 42, LandEraserSizeTrack.Top - 58), 2000);
            SetSelectedBrushSize(LandformMethods.LandformEraserSize);

        }
        #endregion

        #region Landform Methods
        /******************************************************************************************************* 
        * LANDFORM METHODS
        *******************************************************************************************************/

        private void SetLandformData(Landform landform)
        {
            // set the landform data from values of the UI controls
            if (AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX].TexturePath);
            }

            landform.LandformTexture = AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_LAND_TEXTURE_INDEX];

            if (landform.LandformTexture.TextureBitmap != null)
            {
                Bitmap resizedBitmap = new(landform.LandformTexture.TextureBitmap, CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight);

                // create and set a shader from the selected texture
                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                landform.LandformFillPaint.Shader = flpShader;
            }

            MapTexture? dashTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Watercolor Dashes");

            if (dashTexture != null)
            {
                dashTexture.TextureBitmap ??= new Bitmap(dashTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(dashTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKFilterQuality.High);

                landform.DashShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            MapTexture? lineHatchTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Line Hatch");

            if (lineHatchTexture != null)
            {
                lineHatchTexture.TextureBitmap ??= new Bitmap(lineHatchTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(lineHatchTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKFilterQuality.High);

                landform.LineHatchBitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            landform.LandformOutlineColor = LandformOutlineColorSelectButton.BackColor;

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

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
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

#pragma warning disable CS8604 // Possible null reference argument.
            DeselectAllMapComponents(selectedLandform);
#pragma warning restore CS8604 // Possible null reference argument.

            return selectedLandform;
        }

        #endregion

        #region Windrose Event Handlers

        private void WindrosePlaceButton_Click(object sender, EventArgs e)
        {
            if (CURRENT_DRAWING_MODE != DrawingModeEnum.PlaceWindrose)
            {
                CURRENT_DRAWING_MODE = DrawingModeEnum.PlaceWindrose;
                SetDrawingModeLabel();

                CURRENT_WINDROSE = CreateWindrose();
            }
            else
            {
                CURRENT_DRAWING_MODE = DrawingModeEnum.None;
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

        private void WaterFeatureSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.WaterFeatureSelect;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void WaterFeaturePaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.WaterPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureBrushSize);
        }

        private void WaterFeatureLakeButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LakePaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureBrushSize);
        }

        private void WaterFeatureRiverButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.RiverPaint;
            SetDrawingModeLabel();
            SetSelectedBrushSize(0);
        }

        private void WaterFeatureEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.WaterErase;
            SetDrawingModeLabel();
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureEraserSize);
        }

        private void WaterBrushSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterFeatureBrushSize = WaterBrushSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureBrushSize.ToString(), WaterBrushSizeTrack, new Point(WaterBrushSizeTrack.Right - 42, WaterBrushSizeTrack.Top - 58), 2000);
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
            TOOLTIP.Show(RiverWidthTrack.Value.ToString(), RiverWidthTrack, new Point(RiverWidthTrack.Right - 42, RiverWidthTrack.Top - 58), 2000);
        }

        private void WaterEraseSizeTrack_ValueChanged(object sender, EventArgs e)
        {
            WaterFeatureMethods.WaterFeatureEraserSize = WaterEraserSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureEraserSize.ToString(), WaterEraserSizeTrack, new Point(WaterEraserSizeTrack.Right - 42, WaterEraserSizeTrack.Top - 58), 2000);
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureEraserSize);
        }

        #endregion

        #region Water Feature Methods

        internal static MapComponent? SelectWaterFeatureAtPoint(RealmStudioMap map, SKPoint mapClickPoint)
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
                            break;
                        }
                    }
                }
            }

#pragma warning disable CS8604 // Possible null reference argument.
            DeselectAllMapComponents(selectedWaterFeature);
#pragma warning restore CS8604 // Possible null reference argument.

            return selectedWaterFeature;
        }


        #endregion

        #region Path Tab Event Handlers

        private void PathSelectButton_Click(object sender, EventArgs e)
        {
            if (CURRENT_DRAWING_MODE != DrawingModeEnum.PathSelect && CURRENT_DRAWING_MODE != DrawingModeEnum.PathEdit)
            {
                CURRENT_DRAWING_MODE = DrawingModeEnum.PathSelect;
            }
            else
            {
                CURRENT_DRAWING_MODE = DrawingModeEnum.None;
            }

            if (CURRENT_DRAWING_MODE == DrawingModeEnum.PathSelect)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathEdit;

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
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathSelect;

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
            else if (CURRENT_DRAWING_MODE == DrawingModeEnum.PathEdit)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathEdit;

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
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathSelect;

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

            if (CURRENT_DRAWING_MODE == DrawingModeEnum.None)
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
            CURRENT_DRAWING_MODE = DrawingModeEnum.PathPaint;
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

            Refresh();
            SKGLRenderControl.Invalidate();
        }

        private void PathWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(PathWidthTrack.Value.ToString(), PathWidthTrack, new Point(PathWidthTrack.Right - 42, PathWidthTrack.Top - 58), 2000);
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

            if (CURRENT_DRAWING_MODE == DrawingModeEnum.PathSelect)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathEdit;

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
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathSelect;

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
            else if (CURRENT_DRAWING_MODE == DrawingModeEnum.PathEdit)
            {
                if (EditPathPointSwitch.Checked)
                {
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathEdit;

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
                    CURRENT_DRAWING_MODE = DrawingModeEnum.PathSelect;

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

            if (CURRENT_DRAWING_MODE == DrawingModeEnum.None)
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

            Refresh();
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
        private PathTypeEnum GetSelectedPathType()
        {
            if (SolidLineRadio.Checked) return PathTypeEnum.SolidLinePath;
            if (DottedLineRadio.Checked) return PathTypeEnum.DottedLinePath;
            if (DashedLineRadio.Checked) return PathTypeEnum.DashedLinePath;
            if (DashDotLineRadio.Checked) return PathTypeEnum.DashDotLinePath;
            if (DashDotDotLineRadio.Checked) return PathTypeEnum.DashDotDotLinePath;
            if (ChevronLineRadio.Checked) return PathTypeEnum.ChevronLinePath;
            if (LineAndDashesRadio.Checked) return PathTypeEnum.LineAndDashesPath;
            if (SmallDashesRadio.Checked) return PathTypeEnum.ShortIrregularDashPath;
            if (ThickLineRadio.Checked) return PathTypeEnum.ThickSolidLinePath;
            if (BlackBorderPathRadio.Checked) return PathTypeEnum.SolidBlackBorderPath;
            if (BorderedGradientRadio.Checked) return PathTypeEnum.BorderedGradientPath;
            if (BorderedLightSolidRadio.Checked) return PathTypeEnum.BorderedLightSolidPath;
            if (DoubleSolidBorderRadio.Checked) return PathTypeEnum.DoubleSolidBorderPath;
            if (BearTracksRadio.Checked) return PathTypeEnum.BearTracksPath;
            if (BirdTracksRadio.Checked) return PathTypeEnum.BirdTracksPath;
            if (FootPrintsRadio.Checked) return PathTypeEnum.FootprintsPath;
            if (RailroadTracksRadio.Checked) return PathTypeEnum.RailroadTracksPath;
            if (TexturePathRadio.Checked) return PathTypeEnum.TexturedPath;
            if (BorderTexturePathRadio.Checked) return PathTypeEnum.BorderAndTexturePath;

            return PathTypeEnum.SolidLinePath;
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

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            mapPath.IsSelected = !mapPath.IsSelected;

                            if (mapPath.IsSelected)
                            {
                                selectedMapPath = mapPath;
                            }
                            break;
                        }
                    }
                }
            }

            List<MapComponent> mapPathLowerComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.PATHLOWERLAYER).MapLayerComponents;

            for (int i = 0; i < mapPathLowerComponents.Count; i++)
            {
                if (mapPathLowerComponents[i] is MapPath mapPath)
                {
                    SKPath? boundaryPath = mapPath.BoundaryPath;

                    if (boundaryPath != null && boundaryPath.PointCount > 0)
                    {
                        if (boundaryPath.Contains(mapClickPoint.X, mapClickPoint.Y))
                        {
                            mapPath.IsSelected = !mapPath.IsSelected;

                            if (mapPath.IsSelected)
                            {
                                selectedMapPath = mapPath;
                            }
                            break;
                        }
                    }
                }
            }

#pragma warning disable CS8604 // Possible null reference argument.
            DeselectAllMapComponents(selectedMapPath);
#pragma warning restore CS8604 // Possible null reference argument.

            return selectedMapPath;
        }
        #endregion

        #region Symbol Tab Event Handlers
        private void SymbolSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.SymbolSelect;
            SELECTED_BRUSH_SIZE = 0;
            SetDrawingModeLabel();
        }

        private void EraseSymbolsButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.SymbolErase;
            SetDrawingModeLabel();
        }

        private void ColorSymbolsButton_Click(object sender, EventArgs e)
        {
            if (SELECTED_MAP_SYMBOL != null)
            {
                // if a symbol has been selected and is grayscale, then color it with the
                // selected custom color

                SKColor paintColor = SymbolColor1Button.BackColor.ToSKColor();

                if (SELECTED_MAP_SYMBOL.IsGrayscale)
                {
                    Cmd_PaintSymbol cmd = new(SELECTED_MAP_SYMBOL, paintColor);
                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();

                    SKGLRenderControl.Invalidate();
                }
            }
            else
            {
                CURRENT_DRAWING_MODE = DrawingModeEnum.SymbolColor;
                SetDrawingModeLabel();

                if (AreaBrushSwitch.Checked)
                {
                    SELECTED_BRUSH_SIZE = AreaBrushSizeTrack.Value;
                }
            }
        }

        private void StructuresSymbolButton_Click(object sender, EventArgs e)
        {
            SELECTED_SYMBOL_TYPE = SymbolTypeEnum.Structure;
            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();

            AddSymbolsToSymbolTable(selectedSymbols);
            AreaBrushSwitch.Checked = false;
            AreaBrushSwitch.Enabled = false;
        }

        private void VegetationSymbolsButton_Click(object sender, EventArgs e)
        {
            SELECTED_SYMBOL_TYPE = SymbolTypeEnum.Vegetation;
            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();

            AddSymbolsToSymbolTable(selectedSymbols);
            AreaBrushSwitch.Enabled = true;
        }

        private void TerrainSymbolsButton_Click(object sender, EventArgs e)
        {
            SELECTED_SYMBOL_TYPE = SymbolTypeEnum.Terrain;
            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();

            AddSymbolsToSymbolTable(selectedSymbols);
            AreaBrushSwitch.Enabled = true;
        }

        private void OtherSymbolsButton_Click(object sender, EventArgs e)
        {
            SELECTED_SYMBOL_TYPE = SymbolTypeEnum.Other;
            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();

            AddSymbolsToSymbolTable(selectedSymbols);
            AreaBrushSwitch.Checked = false;
            AreaBrushSwitch.Enabled = false;
        }

        private void SymbolScaleTrack_Scroll(object sender, EventArgs e)
        {
            if (!SYMBOL_SCALE_LOCKED)
            {
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
            // TODO: default symbol colors are set from selected theme
            SymbolColor1Button.BackColor = Color.FromArgb(255, 85, 44, 36);
            SymbolColor1Button.Refresh();

            SymbolColor2Button.BackColor = Color.FromArgb(255, 53, 45, 32);
            SymbolColor2Button.Refresh();

            SymbolColor3Button.BackColor = Color.FromArgb(161, 214, 202, 171);
            SymbolColor3Button.Refresh();

        }

        private void SymbolColor1Button_Click(object sender, EventArgs e)
        {
            Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor1Button.BackColor);

            SymbolColor1Button.BackColor = c;
            SymbolColor1Button.Refresh();

            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
            AddSymbolsToSymbolTable(selectedSymbols);
        }

        private void SymbolColor2Button_Click(object sender, EventArgs e)
        {
            Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor2Button.BackColor);

            SymbolColor2Button.BackColor = c;
            SymbolColor2Button.Refresh();

            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
            AddSymbolsToSymbolTable(selectedSymbols);
        }

        private void SymbolColor3Button_Click(object sender, EventArgs e)
        {
            Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor3Button.BackColor);

            SymbolColor3Button.BackColor = c;
            SymbolColor3Button.Refresh();

            List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
            AddSymbolsToSymbolTable(selectedSymbols);
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
            TOOLTIP.Show(AreaBrushSizeTrack.Value.ToString(), AreaBrushSizeTrack, new Point(AreaBrushSizeTrack.Right - 42, AreaBrushSizeTrack.Top - 94), 2000);

            SELECTED_BRUSH_SIZE = AreaBrushSizeTrack.Value;
            SKGLRenderControl.Invalidate();
        }

        private void SymbolRotationTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(SymbolRotationTrack.Value.ToString(), SymbolRotationTrack, new Point(SymbolRotationTrack.Right - 38, SymbolRotationTrack.Top - 170), 2000);
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
            List<string> selectedTags = SymbolTagsListBox.CheckedItems.Cast<string>().ToList();
            List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SELECTED_SYMBOL_TYPE, checkedCollections, selectedTags);
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
            List<string> selectedCollections = SymbolCollectionsListBox.CheckedItems.Cast<string>().ToList();
            List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SELECTED_SYMBOL_TYPE, selectedCollections, checkedTags);
            AddSymbolsToSymbolTable(filteredSymbols);
        }

        private void SymbolSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // filter symbol list based on text entered by the user

            if (SymbolSearchTextBox.Text.Length > 2)
            {
                List<string> selectedCollections = SymbolCollectionsListBox.CheckedItems.Cast<string>().ToList();
                List<string> selectedTags = SymbolTagsListBox.CheckedItems.Cast<string>().ToList();
                List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SELECTED_SYMBOL_TYPE, selectedCollections, selectedTags, SymbolSearchTextBox.Text);

                AddSymbolsToSymbolTable(filteredSymbols);
            }
            else if (SymbolSearchTextBox.Text.Length == 0)
            {
                List<string> selectedCollections = SymbolCollectionsListBox.CheckedItems.Cast<string>().ToList();
                List<string> selectedTags = SymbolTagsListBox.CheckedItems.Cast<string>().ToList();
                List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SELECTED_SYMBOL_TYPE, selectedCollections, selectedTags);

                AddSymbolsToSymbolTable(filteredSymbols);
            }
        }


        #endregion

        #region Symbol Tab Methods
        private List<MapSymbol> GetFilteredMapSymbols()
        {
            List<string> selectedCollections = SymbolCollectionsListBox.CheckedItems.Cast<string>().ToList();
            List<string> selectedTags = SymbolTagsListBox.CheckedItems.Cast<string>().ToList();
            List<MapSymbol> filteredSymbols = SymbolMethods.GetFilteredSymbolList(SELECTED_SYMBOL_TYPE, selectedCollections, selectedTags);

            return filteredSymbols;
        }

        private void AddSymbolsToSymbolTable(List<MapSymbol> symbols)
        {
            SymbolTable.Hide();
            SymbolTable.Controls.Clear();
            foreach (MapSymbol symbol in symbols)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                symbol.ColorMappedBitmap = symbol.SymbolBitmap;
#pragma warning restore CS8604 // Possible null reference argument.

                Bitmap colorMappedBitmap = Extensions.ToBitmap(symbol.ColorMappedBitmap);

                if (symbol.UseCustomColors)
                {
                    SymbolMethods.MapCustomColorsToColorableBitmap(ref colorMappedBitmap, SymbolColor1Button.BackColor, SymbolColor2Button.BackColor, SymbolColor3Button.BackColor);
                }

                symbol.ColorMappedBitmap = Extensions.ToSKBitmap(colorMappedBitmap);

                PictureBox pb = new()
                {
                    Tag = symbol,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = colorMappedBitmap,
                };

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                pb.MouseHover += SymbolPictureBox_MouseHover;
                pb.MouseClick += SymbolPictureBox_MouseClick;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                SymbolTable.Controls.Add(pb);
            }
            SymbolTable.Show();

            Refresh();
        }

        private void SymbolPictureBox_MouseHover(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;

            if (pb.Tag is MapSymbol s)
            {
                TOOLTIP.Show(s.SymbolName, pb);
            }
        }

        private void SymbolPictureBox_MouseClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                if (ModifierKeys == Keys.Shift)
                {
                    // secondary symbol selection - for additional symbols to be used when painting symbols to the map (forests, etc.)
                    if (CURRENT_DRAWING_MODE == DrawingModeEnum.SymbolPlace)
                    {
                        PictureBox pb = (PictureBox)sender;

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
                    // primary symbol selection                    

                    PictureBox pb = (PictureBox)sender;

                    if (pb.Tag is MapSymbol s)
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

                            CURRENT_DRAWING_MODE = DrawingModeEnum.SymbolPlace;
                            SetDrawingModeLabel();
                        }
                        else
                        {
                            // clicked symbol is already selected, so deselect it
                            pb.BackColor = SystemColors.Control;
                            pb.Refresh();

                            SymbolMethods.SelectedSymbolTableMapSymbol = null;
                            CURRENT_DRAWING_MODE = DrawingModeEnum.None;
                            SetDrawingModeLabel();
                        }
                    }
                }
            }
            else if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                PictureBox pb = (PictureBox)sender;
                if (pb.Tag is MapSymbol s)
                {
                    SymbolInfo si = new(s);
                    si.ShowDialog();
                }
            }
        }

        private void PlaceSelectedSymbolAtCursor(SKPoint mouseCursorPoint)
        {
            if (SymbolMethods.SelectedSymbolTableMapSymbol != null)
            {
                SKBitmap? symbolBitmap = SymbolMethods.SelectedSymbolTableMapSymbol.SymbolBitmap;
                if (symbolBitmap != null)
                {
                    float symbolScale = SymbolScaleTrack.Value / 100.0F;
                    float symbolRotation = SymbolRotationTrack.Value;

                    SKBitmap rotatedAndScaledBitmap = RotateAndScaleSymbolBitmap(symbolBitmap, symbolScale, symbolRotation);
                    SKPoint cursorPoint = new(mouseCursorPoint.X - (rotatedAndScaledBitmap.Width / 2), mouseCursorPoint.Y - (rotatedAndScaledBitmap.Height / 2));

                    PlaceSelectedMapSymbolAtPoint(cursorPoint, PREVIOUS_CURSOR_POINT, symbolScale, symbolRotation);
                }
            }
        }

        private void PlaceSelectedMapSymbolAtPoint(SKPoint cursorPoint, SKPoint previousPoint, float symbolScale, float symbolRotation)
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
                    SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleBitmap(symbolBitmap, symbolScale);
                    SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateBitmap(scaledSymbolBitmap, symbolRotation, MirrorSymbolSwitch.Checked);

                    if (rotatedAndScaledBitmap != null && AreaBrushSwitch.Checked)
                    {
                        float bitmapSize = rotatedAndScaledBitmap.Width + rotatedAndScaledBitmap.Height;

                        // increasing this value reduces the rate of symbol placement on the map
                        // so high values of placement rate on the placement rate trackbar or updown increase placement rate on the map
                        float placementRateSize = bitmapSize / PLACEMENT_RATE;

                        float pointDistanceSquared = SKPoint.DistanceSquared(previousPoint, cursorPoint);

                        if (pointDistanceSquared > placementRateSize)
                        {
                            bool canPlaceSymbol = SymbolMethods.CanPlaceSymbol(CURRENT_MAP, symbolToPlace, rotatedAndScaledBitmap, cursorPoint, PLACEMENT_DENSITY);

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
                    else
                    {
                        symbolToPlace.CustomSymbolColors[0] = SymbolColor1Button.BackColor.ToSKColor();
                        symbolToPlace.CustomSymbolColors[1] = SymbolColor2Button.BackColor.ToSKColor();
                        symbolToPlace.CustomSymbolColors[2] = SymbolColor3Button.BackColor.ToSKColor();

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

                    int exclusionRadius = (int)Math.Ceiling(PLACEMENT_DENSITY * ((rotatedAndScaledBitmap.Width + rotatedAndScaledBitmap.Height) / 2.0F));

                    List<SKPoint> areaPoints = DrawingMethods.GetPointsInCircle(cursorPoint, (int)Math.Ceiling((double)areaBrushSize), exclusionRadius);

                    foreach (SKPoint p in areaPoints)
                    {
                        SKPoint point = p;

                        // 1% randomization of point location
                        point.X = Random.Shared.Next((int)(p.X * 0.99F), (int)(p.X * 1.01F));
                        point.Y = Random.Shared.Next((int)(p.Y * 0.99F), (int)(p.Y * 1.01F));

                        PlaceSelectedMapSymbolAtPoint(point, PREVIOUS_CURSOR_POINT, symbolScale, symbolRotation);
                    }
                }
            }
        }

        private SKBitmap RotateAndScaleSymbolBitmap(SKBitmap symbolBitmap, float symbolScale, float symbolRotation)
        {
            SKBitmap scaledSymbolBitmap = DrawingMethods.ScaleBitmap(symbolBitmap, symbolScale);

            SKBitmap rotatedAndScaledBitmap = DrawingMethods.RotateBitmap(scaledSymbolBitmap, symbolRotation, MirrorSymbolSwitch.Checked);

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

#pragma warning disable CS8604 // Possible null reference argument.
            DeselectAllMapComponents(selectedSymbol);
#pragma warning restore CS8604 // Possible null reference argument.

            return selectedSymbol;
        }

        private static void MoveSelectedSymbolInRenderOrder(ComponentMoveDirectionEnum direction, int amount = 1, bool toTopBottom = false)
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

                if (direction == ComponentMoveDirectionEnum.Up)
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
                else if (direction == ComponentMoveDirectionEnum.Down)
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
                TextBox tb = (TextBox)sender;

                Font labelFont = SELECTED_LABEL_FONT;
                Color labelColor = FontColorSelectButton.BackColor;

                if (((KeyPressEventArgs)e).KeyChar == (char)Keys.Return)
                {
                    ((KeyPressEventArgs)e).Handled = true;
                    CREATING_LABEL = false;

                    Color outlineColor = OutlineColorSelectButton.BackColor;
                    float outlineWidth = OutlineWidthTrack.Value / 100F;

                    Color glowColor = GlowColorSelectButton.BackColor;
                    int glowStrength = GlowStrengthTrack.Value;

                    int labelRotationDegrees = LabelRotationTrack.Value;

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

                        SKPaint paint = MapLabelMethods.CreateLabelPaint(labelFont, labelColor, LabelTextAlignEnum.AlignLeft);
                        SKFont paintFont = paint.ToFont();

                        label.LabelPaint = paint;
                        label.LabelSKFont.Dispose();
                        label.LabelSKFont = paintFont;

                        SKRect bounds = new();
                        paint.MeasureText(label.LabelText, ref bounds);

                        Point labelPoint = new(tb.Left, tb.Top);

                        label.X = labelPoint.X;
                        label.Y = labelPoint.Y + (int)bounds.Height / 2;
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

                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear();
                        }

                        Cmd_AddLabel cmd = new(CURRENT_MAP, label);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        DeselectAllMapComponents(label);

                        MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LABELLAYER);
                        SELECTED_MAP_LABEL = (MapLabel?)labelLayer.MapLayerComponents.Last();

                        CURRENT_MAP.IsSaved = false;
                    }

                    CURRENT_DRAWING_MODE = DrawingModeEnum.LabelSelect;
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
                        tb.Text = tb.Text.Substring("...Label...".Length);
                    }

                    SKPaint paint = MapLabelMethods.CreateLabelPaint(labelFont, labelColor, LabelTextAlignEnum.AlignLeft);
                    SKRect bounds = new();

                    paint.MeasureText(tb.Text, ref bounds);
                    int tbWidth = (int)Math.Max(bounds.Width, tb.Width);
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
                if (box.BoxBitmap == null && box.BoxBitmapPath != null)
                {
                    box.BoxBitmap ??= new Bitmap(box.BoxBitmapPath);
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
                float outlineWidth = OutlineWidthTrack.Value / 100F;
                Color glowColor = GlowColorSelectButton.BackColor;
                int glowStrength = GlowStrengthTrack.Value;

                Cmd_ChangeLabelAttributes cmd = new(SELECTED_MAP_LABEL, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, SELECTED_LABEL_FONT);
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

        private PlacedMapBox? SelectMapBoxAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
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

            OutlineWidthTrack.Value = (int)(preset.LabelOutlineWidth * 100F);
            OutlineWidthTrack.Refresh();

            GlowColorSelectButton.BackColor = Color.FromArgb(preset.LabelGlowColor);
            GlowColorSelectButton.Refresh();

            GlowStrengthTrack.Value = preset.LabelGlowStrength;
            GlowStrengthTrack.Refresh();

            string fontString = preset.LabelFontString;
            FontConverter cvt = new();
            Font? font = (Font?)cvt.ConvertFromString(fontString);

            if (font != null)
            {
                SELECTED_LABEL_FONT = font;
                SelectLabelFontButton.Font = new Font(font.FontFamily, 14);
                SelectLabelFontButton.Refresh();
            }
        }

        #endregion

        #region Label Tab Event Handlers
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
            LabelPresetNameEntry presetDialog = new();
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

                string presetFileName = Resources.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "LabelPresets" + Path.DirectorySeparatorChar + Guid.NewGuid().ToString() + ".mclblprst";

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
                    LabelPreset preset = new();

                    FontConverter cvt = new();
                    string? fontString = cvt.ConvertToString(SELECTED_LABEL_FONT);

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
                        preset.LabelOutlineWidth = OutlineWidthTrack.Value / 100F;
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
                string? presetName = (string?)LabelPresetsListBox.SelectedItem;

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
            if (FONT_SELECTION_DIALOG == null)
            {
                FONT_SELECTION_DIALOG = new FontSelectionDialog(this, SELECTED_LABEL_FONT);
                FONT_SELECTION_DIALOG.FontSelected += new EventHandler(FontSelector_FontSelected);
            }

            if (FONT_SELECTION_DIALOG != null)
            {
                if (!FONT_SELECTION_DIALOG.Visible)
                {
                    FONT_SELECTION_DIALOG.Show(this);
                }
                else
                {
                    FONT_SELECTION_DIALOG.Hide();
                }
            }

        }

        private void FontSelector_FontSelected(object? sender, EventArgs e)
        {
            if (FONT_SELECTION_DIALOG != null)
            {
                if (FONT_SELECTION_DIALOG.SelectedFont != null)
                {
                    SelectLabelFontButton.Font = new Font(FONT_SELECTION_DIALOG.SelectedFont.FontFamily, 12);
                    SelectLabelFontButton.Refresh();

                    SELECTED_LABEL_FONT = FONT_SELECTION_DIALOG.SelectedFont;

                    if (SELECTED_MAP_LABEL != null)
                    {
                        Color labelColor = FontColorSelectButton.BackColor;
                        Color outlineColor = OutlineColorSelectButton.BackColor;
                        float outlineWidth = OutlineWidthTrack.Value / 100F;
                        Color glowColor = GlowColorSelectButton.BackColor;
                        int glowStrength = GlowStrengthTrack.Value;

                        Cmd_ChangeLabelAttributes cmd = new(SELECTED_MAP_LABEL, labelColor, outlineColor, outlineWidth, glowColor, glowStrength, SELECTED_LABEL_FONT);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        SKGLRenderControl.Invalidate();
                    }
                }
            }
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
            UpdateSelectedLabelOnUIChange();
        }

        private void LabelRotationTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(LabelRotationTrack.Value.ToString(), LabelRotationTrack, new Point(LabelRotationTrack.Right - 38, LabelRotationTrack.Top - 40), 2000);

            if (SELECTED_MAP_LABEL != null)
            {
                Cmd_ChangeLabelRotation cmd = new(SELECTED_MAP_LABEL, (float)LabelRotationTrack.Value);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();

                SKGLRenderControl.Invalidate();
            }
        }

        private void CircleTextPathButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.DrawArcLabelPath;
            SetDrawingModeLabel();
        }

        private void BezierTextPathButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.DrawBezierLabelPath;
            SetDrawingModeLabel();
        }

        private void LabelSelectButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LabelSelect;
            SetDrawingModeLabel();
        }

        private void PlaceLabelButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.DrawLabel;
            SetDrawingModeLabel();
        }

        private void CreateBoxButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.DrawBox;
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
            /*
            if (ModifierKeys == Keys.None)
            {
                string generatedName = MapToolMethods.GenerateRandomPlaceName();

                if (MapLabelMethods.CreatingLabel)
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
                NAME_GENERATOR_SETTINGS_DIALOG.Show();
            }
            */
        }
        #endregion

        #region Overlay Tab Methods

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
                        frame.FrameBitmap ??= SKBitmap.FromImage(image);
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
                CURRENT_MAP_GRID.GridType = GridTypeEnum.Square;
            }
            else if (FlatHexGridRadio.Checked)
            {
                CURRENT_MAP_GRID.GridType = GridTypeEnum.FlatHex;
                CURRENT_MAP_GRID.GridSize /= 2;
            }
            else if (PointedHexGridRadio.Checked)
            {
                CURRENT_MAP_GRID.GridType = GridTypeEnum.PointedHex;
                CURRENT_MAP_GRID.GridSize /= 2;
            }

            string? selectedLayerItem = (string?)GridLayerUpDown.SelectedItem;

            if (selectedLayerItem != null)
            {
                switch (selectedLayerItem)
                {
                    case "Default":
                        CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
                        break;
                    case "Above Ocean":
                        CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.ABOVEOCEANGRIDLAYER;
                        break;
                    case "Below Symbols":
                        CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.BELOWSYMBOLSGRIDLAYER;
                        break;
                    default:
                        CURRENT_MAP_GRID.GridLayerIndex = MapBuilder.DEFAULTGRIDLAYER;
                        break;
                }
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
            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.Count - 1; i > 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.DEFAULTGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.Count - 1; i > 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.Count - 1; i > 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents[i] is MapGrid)
                {
                    MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER).MapLayerComponents.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region Overlay Tab Event Handlers

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

                            CURRENT_FRAME = frame;

                            OverlayMethods.RemoveAllFrames(CURRENT_MAP);

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
                            CURRENT_FRAME = null;
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
            TOOLTIP.Show((FrameScaleTrack.Value / 100F).ToString(), FrameScaleTrack, new Point(FrameScaleTrack.Right - 78, FrameScaleTrack.Top - 150), 2000);

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
                CURRENT_MAP_GRID.GridType = GridTypeEnum.Square;

                SKGLRenderControl.Invalidate();
            }
        }

        private void FlatHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridType = GridTypeEnum.FlatHex;

                SKGLRenderControl.Invalidate();
            }
        }

        private void PointedHexGridRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridType = GridTypeEnum.PointedHex;

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
            TOOLTIP.Show(GridSizeTrack.Value.ToString(), GridSizeTrack, new Point(GridSizeTrack.Right - 38, GridSizeTrack.Top - 255), 2000);

            if (CURRENT_MAP_GRID != null)
            {
                CURRENT_MAP_GRID.GridSize = GridSizeTrack.Value;
                SKGLRenderControl.Invalidate();
            }
        }

        private void GridLineWidthTrack_ValueChanged(object sender, EventArgs e)
        {
            TOOLTIP.Show(GridLineWidthTrack.Value.ToString(), GridLineWidthTrack, new Point(GridLineWidthTrack.Right - 38, GridLineWidthTrack.Top - 300), 2000);

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
            CURRENT_DRAWING_MODE = DrawingModeEnum.DrawMapMeasure;
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
    }
}
