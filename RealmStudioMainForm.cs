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
using SkiaSharp;
using SkiaSharp.Components;
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

        // the objects currently being drawn, before being added to map layers
        private static Landform? CURRENT_LANDFORM = null;
        private static MapWindrose? CURRENT_WINDROSE = null;
        private static WaterFeature? CURRENT_WATERFEATURE = null;
        private static River? CURRENT_RIVER = null;
        private static MapPath? CURRENT_PATH = null;

        private static MapPath? SELECTED_PATH = null;
        private static MapPathPoint? SELECTED_PATHPOINT = null;

        private static int SELECTED_BRUSH_SIZE = 0;

        private static SKPoint ScrollPoint = new(0, 0);
        private static SKPoint DrawingPoint = new(0, 0);

        private static SKPoint PREVIOUS_CLICK_POINT = new(0, 0);

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

            /*

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


            // render water features and rivers
            MapLayer waterLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERLAYER);
            MapLayer waterDrawingLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.WATERDRAWINGLAYER);

            if (waterLayer.LayerSurface != null && waterDrawingLayer.LayerSurface != null)
            {
                waterLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                waterDrawingLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                // water features
                CURRENT_WATERFEATURE?.Render(waterLayer.LayerSurface.Canvas);
                CURRENT_RIVER?.Render(waterLayer.LayerSurface.Canvas);

                foreach (IWaterFeature w in waterLayer.MapLayerComponents.Cast<IWaterFeature>())
                {
                    if (w is WaterFeature wf)
                    {
                        wf.Render(waterLayer.LayerSurface.Canvas);

                        if (wf.IsSelected)
                        {
                            // draw an outline around the landform to show that it is selected
                            wf.WaterFeaturePath.GetBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.WaterFeatureSelectPaint);
                        }
                    }
                    else if (w is River r)
                    {
                        r.Render(waterLayer.LayerSurface.Canvas);

                        // TODO: render river drawing

                        if (r.IsSelected)
                        {
                            if (r.RiverBoundaryPath != null)
                            {
                                // draw an outline around the path to show that it is selected
                                r.RiverBoundaryPath.GetTightBounds(out SKRect boundRect);
                                using SKPath boundsPath = new();
                                boundsPath.AddRect(boundRect);

                                selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.RiverSelectPaint);
                            }
                        }

                        if (r.ShowRiverPoints)
                        {
                            List<MapRiverPoint> controlPoints = r.GetRiverControlPoints();

                            foreach (MapRiverPoint p in controlPoints)
                            {
                                waterDrawingLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, 2.0F, PaintObjects.RiverControlPointPaint);
                                waterDrawingLayer.LayerSurface.Canvas.DrawCircle(p.RiverPoint.X, p.RiverPoint.Y, 2.0F, PaintObjects.RiverControlPointOutlinePaint);
                            }
                        }
                    }
                }

                // TODO: render water feature drawing on water drawing layer

                // paint water layer
                e.Surface.Canvas.DrawSurface(waterLayer.LayerSurface, ScrollPoint);

                // paint water drawing layer
                e.Surface.Canvas.DrawSurface(waterDrawingLayer.LayerSurface, ScrollPoint);
            }


            // render grid below symbols
            MapLayer belowSymbolGridLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.BELOWSYMBOLSGRIDLAYER);
            if (belowSymbolGridLayer.LayerSurface != null)
            {
                belowSymbolGridLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);
                belowSymbolGridLayer.Render(belowSymbolGridLayer.LayerSurface.Canvas);
                e.Surface.Canvas.DrawSurface(aboveOceanGridLayer.LayerSurface, ScrollPoint);
            }

            // path lower layer
            MapLayer pathLowerLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHLOWERLAYER);
            if (pathLowerLayer.LayerSurface != null)
            {
                pathLowerLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                if (CURRENT_PATH != null && !CURRENT_PATH.DrawOverSymbols)
                {
                    CURRENT_PATH.Render(pathLowerLayer.LayerSurface.Canvas);
                }

                foreach (MapPath mp in pathLowerLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(pathLowerLayer.LayerSurface.Canvas);

                    if (mp.IsSelected)
                    {
                        if (mp.BoundaryPath != null)
                        {
                            // draw an outline around the path to show that it is selected
                            mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                        }
                    }

                    if (mp.ShowPathPoints)
                    {
                        List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                        foreach (MapPathPoint p in controlPoints)
                        {
                            if (p.IsSelected)
                            {
                                pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathSelectedControlPointPaint);
                            }
                            else
                            {
                                pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointPaint);
                            }

                            pathLowerLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointOutlinePaint);
                        }
                    }
                }

                e.Surface.Canvas.DrawSurface(pathLowerLayer.LayerSurface, ScrollPoint);
            }


            // TODO: symbol layer

            // path upper layer
            MapLayer pathUpperLayer = MapBuilder.GetMapLayerByIndex(CURRENT_MAP, MapBuilder.PATHUPPERLAYER);
            if (pathUpperLayer.LayerSurface != null)
            {
                pathUpperLayer.LayerSurface.Canvas.Clear(SKColors.Transparent);

                if (CURRENT_PATH != null && CURRENT_PATH.DrawOverSymbols)
                {
                    CURRENT_PATH.Render(pathUpperLayer.LayerSurface.Canvas);
                }

                foreach (MapPath mp in pathUpperLayer.MapLayerComponents.Cast<MapPath>())
                {
                    mp.Render(pathUpperLayer.LayerSurface.Canvas);

                    if (mp.IsSelected)
                    {
                        if (mp.BoundaryPath != null)
                        {
                            // draw an outline around the path to show that it is selected
                            mp.BoundaryPath.GetTightBounds(out SKRect boundRect);
                            using SKPath boundsPath = new();
                            boundsPath.AddRect(boundRect);

                            selectionLayer.LayerSurface?.Canvas.DrawPath(boundsPath, PaintObjects.MapPathSelectPaint);
                        }
                    }

                    if (mp.ShowPathPoints)
                    {
                        List<MapPathPoint> controlPoints = mp.GetMapPathControlPoints();

                        foreach (MapPathPoint p in controlPoints)
                        {
                            if (p.IsSelected)
                            {
                                pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathSelectedControlPointPaint);
                            }
                            else
                            {
                                pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointPaint);
                            }

                            pathUpperLayer.LayerSurface.Canvas.DrawCircle(p.MapPoint.X, p.MapPoint.Y, 2.0F, PaintObjects.MapPathControlPointOutlinePaint);
                        }
                    }
                }

                e.Surface.Canvas.DrawSurface(pathUpperLayer.LayerSurface, ScrollPoint);
            }

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
                LeftButtonMouseDownHandler(sender, e, SELECTED_BRUSH_SIZE);
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
                        PREVIOUS_CLICK_POINT = zoomedScrolledPoint;

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

            }
        }



        private void RightButtonMouseDownHandler(object sender, MouseEventArgs e)
        {
            // no right mouse button down events yet
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
                                Width = zoomedScrolledPoint.X - PREVIOUS_CLICK_POINT.X,
                                Height = zoomedScrolledPoint.Y - PREVIOUS_CLICK_POINT.Y,
                            };

                            foreach (MapPathPoint point in SELECTED_PATH.PathPoints)
                            {
                                SKPoint p = point.MapPoint;
                                p.X += delta.Width;
                                p.Y += delta.Height;
                                point.MapPoint = p;
                            }

                            SELECTED_PATH.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(SELECTED_PATH.PathPoints);
                            PREVIOUS_CLICK_POINT = zoomedScrolledPoint;
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
                        Cmd_AddWindrose cmd = new(CURRENT_MAP, CURRENT_WINDROSE);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

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
            }

        }

        private void RightButtonMouseUpHandler(object sender, MouseEventArgs e)
        {
            // no right button mouse up events yet
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
                    case DrawingModeEnum.PathSelect:
                        if (SELECTED_PATH != null)
                        {
                            Cmd_RemoveMapPath cmd = new(CURRENT_MAP, SELECTED_PATH);
                            CommandManager.AddCommand(cmd);
                            cmd.DoOperation();

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

                                CURRENT_MAP.IsSaved = false;
                                SKGLRenderControl.Invalidate();
                            }
                        }
                        break;
                }
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
            WaterFeatureMethods.WaterFeatureEraserSize = WaterEraseSizeTrack.Value;
            TOOLTIP.Show(WaterFeatureMethods.WaterFeatureEraserSize.ToString(), WaterEraseSizeTrack, new Point(WaterEraseSizeTrack.Right - 42, WaterEraseSizeTrack.Top - 58), 2000);
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

    }
}
