using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;

namespace RealmStudio
{
    public partial class RealmStudioMainForm : Form
    {
        private int MAP_WIDTH = MapBuilder.MAP_DEFAULT_WIDTH;
        private int MAP_HEIGHT = MapBuilder.MAP_DEFAULT_HEIGHT;

        private static DrawingModeEnum CURRENT_DRAWING_MODE = DrawingModeEnum.None;

        private static RealmStudioMap CURRENT_MAP = new();

        private static Landform? CURRENT_LANDFORM = null;
        private MapWindrose? CURRENT_WINDROSE = null;

        private static int SELECTED_BRUSH_SIZE = 0;

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);

        private static float DrawingZoom = 1.0f;

        private readonly ToolTip TOOLTIP = new();

        private bool AUTOSAVE = true;

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

            int assetCount = AssetManager.LoadAllAssets();

            PopulateControlsWithAssets(assetCount);
        }

        private void RealmStudioMainForm_Shown(object sender, EventArgs e)
        {
            MapBuilder.DisposeMap(CURRENT_MAP);

            // this creates the CURRENT_MAP
            OpenRealmConfigurationDialog();

            LogoPictureBox.Hide();
            SKGLRenderControl.Show();
            SKGLRenderControl.Select();
            SKGLRenderControl.Refresh();
            SKGLRenderControl.Invalidate();

            Activate();

            SetDrawingModeLabel();
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
                        MapBuilder.DisposeMap(CURRENT_MAP);

                        // this creates the CURRENT_MAP
                        OpenRealmConfigurationDialog();
                    }
                }
                else if (result == DialogResult.No)
                {
                    MapBuilder.DisposeMap(CURRENT_MAP);

                    // this creates the CURRENT_MAP
                    OpenRealmConfigurationDialog();
                }
            }
            else
            {
                MapBuilder.DisposeMap(CURRENT_MAP);

                // this creates the CURRENT_MAP
                OpenRealmConfigurationDialog();
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

        #endregion

        #region Main Form Methods
        /******************************************************************************************************* 
         * MAIN FORM METHODS
         *******************************************************************************************************/

        private void OpenRealmConfigurationDialog()
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
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear the drawing mode (and uncheck all drawing, paint, and erase buttons) when switching tabs
            CURRENT_DRAWING_MODE = DrawingModeEnum.None;

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

            if (AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.BACKGROUND_TEXTURE_LIST.First().TexturePath);
            }

            BackgroundTextureBox.Image = AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureBitmap;
            BackgroundTextureNameLabel.Text = AssetManager.BACKGROUND_TEXTURE_LIST.First().TextureName;

            if (AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST.First().TexturePath);
            }

            LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST.First().TextureBitmap;
            LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST.First().TextureName;

            if (AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap == null)
            {
                AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.WATER_TEXTURE_LIST.First().TexturePath);
            }

            OceanTextureBox.Image = AssetManager.WATER_TEXTURE_LIST.First().TextureBitmap;
            OceanTextureNameLabel.Text = AssetManager.WATER_TEXTURE_LIST.First().TextureName;

            CoastlineStyleList.SelectedIndex = 6;  // default is dash pattern

            foreach (MapSymbolCollection collection in AssetManager.MAP_SYMBOL_COLLECTIONS)
            {
                //SymbolCollectionsListBox.Items.Add(collection.GetCollectionName());
            }

            //AddMapBoxesToLabelBoxTable(MapLabelMethods.MAP_BOX_TEXTURES);

            //AddMapFramesToFrameTable(OverlayMethods.MAP_FRAME_TEXTURES);

            foreach (LabelPreset preset in AssetManager.LABEL_PRESETS)
            {
                if (!string.IsNullOrEmpty(preset.LabelPresetTheme)
                    && AssetManager.CURRENT_THEME != null
                    && preset.LabelPresetTheme == AssetManager.CURRENT_THEME.ThemeName)
                {
                    //LabelPresetCombo.Items.Add(preset.LabelPresetName);
                }
            }

            /*
            if (LabelPresetCombo.Items.Count > 0)
            {
                LabelPresetCombo.SelectedIndex = 0;

                LabelPreset? selectedPreset = AssetManager.LABEL_PRESETS.Find(x => !string.IsNullOrEmpty((string?)LabelPresetCombo.Items[0]) && x.LabelPresetName == (string?)LabelPresetCombo.Items[0]);

                if (selectedPreset != null)
                {
                    SetLabelValuesFromPreset(selectedPreset);
                }
            }
            */
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


        public static void DeselectAllMapComponents(MapComponent selectedComponent)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);

            foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
            {
                if (selectedComponent != null && selectedComponent is Landform landform && landform == l) continue;
                l.IsSelected = false;
            }

            /*
            foreach (MapPaintedWaterFeature w in WaterFeatureMethods.PAINTED_WATERFEATURE_LIST)
            {
                if (selectedComponent != null && selectedComponent is MapPaintedWaterFeature waterFeature && waterFeature == w) continue;
                w.IsSelected = false;
            }

            foreach (MapRiver r in WaterFeatureMethods.MAP_RIVER_LIST)
            {
                if (selectedComponent != null && selectedComponent is MapRiver river && river == r) continue;
                r.IsSelected = false;
            }

            foreach (MapPath p in MapPathMethods.GetMapPathList())
            {
                if (selectedComponent != null && selectedComponent is MapPath mapPath && mapPath == p) continue;
                p.IsSelected = false;
            }

            foreach (MapSymbol s in SymbolMethods.PlacedSymbolList)
            {
                if (selectedComponent != null && selectedComponent is MapSymbol mapSymbol && mapSymbol == s) continue;
                s.SetIsSelected(false);
            }

            foreach (MapLabel l in MapLabelMethods.MAP_LABELS)
            {
                if (selectedComponent != null && selectedComponent is MapLabel mapLabel && mapLabel == l) continue;
                l.IsSelected = false;
            }

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
            foreach (MapLayer layer in CURRENT_MAP.MapLayers)
            {
                // if the surfaces haven't been created, create them
                layer.LayerSurface ??= SKSurface.Create(SKGLRenderControl.GRContext, false, new SKImageInfo(CURRENT_MAP.MapWidth, CURRENT_MAP.MapHeight));
            }


            // handle zoom-in and zoom-out (TODO: zoom in and out from center of map - how?)
            e.Surface.Canvas.Scale(DrawingZoom);

            // paint the SKGLRenderControl surface, compositing the surfaces from all of the layers
            e.Surface.Canvas.Clear(SKColors.Black);

            MapLayer selectionLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.SELECTIONLAYER);
            selectionLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);



            // render base layer
            MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BASELAYER);
            if (baseLayer.LayerSurface != null)
            {
                baseLayer.LayerSurface.Canvas.Clear(SKColors.White);
                baseLayer.Render(baseLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(baseLayer.LayerSurface, ScrollPoint);
            }

            // render ocean texture layer
            MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTURELAYER);
            if (oceanTextureLayer.LayerSurface != null)
            {
                oceanTextureLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureLayer.Render(oceanTextureLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(oceanTextureLayer.LayerSurface, ScrollPoint);
            }

            // render ocean texture overlay layer (ocean color fill)
            MapLayer oceanTextureOverlayLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANTEXTUREOVERLAYLAYER);
            if (oceanTextureOverlayLayer.LayerSurface != null)
            {
                oceanTextureOverlayLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanTextureOverlayLayer.Render(oceanTextureOverlayLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(oceanTextureOverlayLayer.LayerSurface, ScrollPoint);
            }

            // render ocean drawing layer (user-painted color)
            MapLayer oceanDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.OCEANDRAWINGLAYER);
            if (oceanDrawingLayer.LayerSurface != null)
            {
                oceanDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                oceanDrawingLayer.Render(oceanDrawingLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(oceanDrawingLayer.LayerSurface, ScrollPoint);
            }
            // render wind rose layer
            MapLayer windroseLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WINDROSELAYER);
            if (windroseLayer.LayerSurface != null)
            {
                windroseLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                CURRENT_WINDROSE?.Render(windroseLayer.LayerSurface.Canvas);

                windroseLayer.Render(windroseLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(windroseLayer.LayerSurface, ScrollPoint);
            }

            // render grid layer above ocean
            MapLayer aboveOceanGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.ABOVEOCEANGRIDLAYER);
            if (aboveOceanGridLayer.LayerSurface != null)
            {
                aboveOceanGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                aboveOceanGridLayer.Render(aboveOceanGridLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(aboveOceanGridLayer.LayerSurface, ScrollPoint);
            }

            // render landforms
            MapLayer landCoastlineLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDCOASTLINELAYER);
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER);
            MapLayer landDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDDRAWINGLAYER);

            if (landformLayer.LayerSurface != null && landCoastlineLayer.LayerSurface != null && landDrawingLayer.LayerSurface != null)
            {
                landCoastlineLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                landformLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                landDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                CURRENT_LANDFORM?.RenderCoastline(landCoastlineLayer.LayerSurface.Canvas);
                CURRENT_LANDFORM?.RenderLandform(landformLayer.LayerSurface.Canvas);

                // TODO: render CURRENT_LANDFORM landform drawing

                foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
                {
                    l.RenderCoastline(landCoastlineLayer.LayerSurface.Canvas);
                    l.RenderLandform(landformLayer.LayerSurface.Canvas);

                    // TODO: render landform drawing

                    if (l.IsSelected)
                    {
                        // draw an outline around the landform to show that it is selected
                        l.ContourPath.GetBounds(out SKRect boundRect);
                        using SKPath boundsPath = new();
                        boundsPath.AddRect(boundRect);

                        selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.LandformSelectPaint);
                    }
                }

                // paint landform coastline layer
                e.Surface.Canvas.DrawSurface(landCoastlineLayer.LayerSurface, ScrollPoint);

                // paint landform layer
                e.Surface.Canvas.DrawSurface(landformLayer.LayerSurface, ScrollPoint);

                // paint land drawing layer
                e.Surface.Canvas.DrawSurface(landDrawingLayer.LayerSurface, ScrollPoint);
            }


            // TODO: render water features

            // TODO: paint water layer

            // TODO: paint water drawing layer


            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER);
            if (belowSymbolGridLayer.LayerSurface != null)
            {
                belowSymbolGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                belowSymbolGridLayer.Render(belowSymbolGridLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(aboveOceanGridLayer.LayerSurface, ScrollPoint);
            }

            // TODO: path lower layer

            // TODO: symbol layer

            // TODO: path upper layer

            // TODO: region layer

            // TODO: region overlay layer

            // TODO: default grid layer

            // TODO: box layer

            // TODO: label layer

            // TODO: overlay layer

            // TODO: measure layer

            // TODO: drawing layer

            // render and paint vignette
            MapLayer vignetteLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.VIGNETTELAYER);
            if (vignetteLayer.LayerSurface != null)
            {
                vignetteLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                vignetteLayer.Render(vignetteLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(vignetteLayer.LayerSurface, ScrollPoint);
            }

            // paint selection layer
            e.Surface.Canvas.DrawSurface(selectionLayer.LayerSurface, ScrollPoint);

            // paint cursor layer
            MapLayer cursorLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.CURSORLAYER);
            e.Surface.Canvas.DrawSurface(cursorLayer.LayerSurface, ScrollPoint);

            // TODO: work layer
        }

        private void SKGLRenderControl_MouseDown(object sender, MouseEventArgs e)
        {
            // objects are created and/or initialized on mouse down
            if (e.Button == MouseButtons.Left)
            {
                LeftButtonMouseDownHandler(sender, e);
            }

            if (e.Button == MouseButtons.Right)
            {
                RightButtonMouseDownHandler(sender, e);
            }

        }

        private void SKGLRenderControl_MouseMove(object sender, MouseEventArgs e)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);
            Task.Run(() => UpdateDrawingPointLabel(e.Location.ToSKPoint(), zoomedScrolledPoint));

            DrawCircleCursor(zoomedScrolledPoint, SELECTED_BRUSH_SIZE);

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
            else if (e.Button == MouseButtons.None)
            {
                NoButtonMouseUpHandler(sender, e);
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
            if (ModifierKeys == Keys.Shift)
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

        #endregion

        #region SKGLRenderControl Mouse Down Event Handling Methods (called from event handlers)

        private void LeftButtonMouseDownHandler(object sender, MouseEventArgs e)
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
            }
        }

        private void RightButtonMouseDownHandler(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SKGLRenderControl Mouse Move Event Handling Methods (called from event handlers)

        #region Left Button Down Handler Method

        private void LeftButtonMouseMoveHandler(MouseEventArgs e, int brushRadius)
        {
            SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
                        Cursor = Cursors.Cross;

                        // TODO: undo/redo
                        if (CURRENT_LANDFORM != null)
                        {
                            CURRENT_LANDFORM.DrawPath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                            // compute contour path and inner and outer paths in a separate thread
                            Task.Run(() => LandformMethods.CreateInnerAndOuterPaths(CURRENT_MAP, CURRENT_LANDFORM));
                        }

                        SKGLRenderControl.Invalidate();
                    }
                    break;
                case DrawingModeEnum.LandErase:
                    {
                        Cursor = Cursors.Cross;

                        // TODO: undo/redo
                        LandformMethods.LandformErasePath.AddCircle(zoomedScrolledPoint.X, zoomedScrolledPoint.Y, brushRadius);

                        LandformMethods.EraseLandForm(CURRENT_MAP);
                        SKGLRenderControl.Invalidate();

                    }
                    break;
            }
        }

        #endregion

        #region Right Button Down Handler Method

        private void RightButtonMouseMoveHandler(object sender, MouseEventArgs e)
        {

        }
        #endregion

        #region No Button Down Handler Method

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
            }
        }

        #endregion

        #endregion

        #region SKGLRenderControl Mouse Up Event Handling Methods (called from event handers)

        private void LeftButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            switch (CURRENT_DRAWING_MODE)
            {
                case DrawingModeEnum.LandPaint:
                    {
                        Cursor = Cursors.Default;

                        SKPoint zoomedScrolledPoint = new((e.X / DrawingZoom) + DrawingPoint.X, (e.Y / DrawingZoom) + DrawingPoint.Y);

                        if (CURRENT_LANDFORM != null)
                        {
                            MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.LANDFORMLAYER).MapLayerComponents.Add(CURRENT_LANDFORM);
                            LandformMethods.MergeLandforms(CURRENT_MAP);

                            CURRENT_LANDFORM = null;

                            SKGLRenderControl.Invalidate();
                        }
                    }
                    break;
                case DrawingModeEnum.PlaceWindrose:
                    if (CURRENT_WINDROSE != null)
                    {
                        Cmd_AddWindrose cmd = new(CURRENT_MAP, CURRENT_WINDROSE);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        CURRENT_MAP.IsSaved = false;
                        CURRENT_WINDROSE = CreateWindrose();
                    }
                    break;
            }

        }

        private void RightButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void NoButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void DrawCircleCursor(SKPoint point, int brushSize)
        {
            MapLayer cursorLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.CURSORLAYER);
            cursorLayer.LayerSurface?.Canvas.Clear(SKColors.Transparent);

            if (brushSize > 0)
            {
                cursorLayer.LayerSurface?.Canvas.DrawCircle(point, brushSize / 2, PaintObjects.CursorCirclePaint);
            }
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
                AssetManager.WATER_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[AssetManager.SELECTED_OCEAN_TEXTURE_INDEX].TexturePath);
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
            SELECTED_BRUSH_SIZE = 0;
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
                CURRENT_WINDROSE = CreateWindrose();
            }
            else
            {
                CURRENT_DRAWING_MODE = DrawingModeEnum.None;
                CURRENT_WINDROSE = null;

                SetDrawingModeLabel();

                SKGLRenderControl.Invalidate();
            }
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
        }

        private void WaterFeaturePaintButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.WaterPaint;
            SetDrawingModeLabel();
        }

        private void WaterFeatureLakeButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.LakePaint;
            SetDrawingModeLabel();
        }

        private void WaterFeatureRiverButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.RiverPaint;
            SetDrawingModeLabel();
        }

        private void WaterFeatureEraseButton_Click(object sender, EventArgs e)
        {
            CURRENT_DRAWING_MODE = DrawingModeEnum.WaterErase;
            SetDrawingModeLabel();
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
            WaterFeatureMethods.WaterFeatureEraserSize = WaterEraseSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureEraserSize.ToString(), WaterEraseSizeTrack, new Point(WaterEraseSizeTrack.Right - 42, WaterEraseSizeTrack.Top - 58), 2000);
            SetSelectedBrushSize(WaterFeatureMethods.WaterFeatureEraserSize);
        }

        #endregion
    }
}
