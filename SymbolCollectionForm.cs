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
* support@brookmonte.com
*
***************************************************************************************************************************/
using FontAwesome.Sharp;
using SkiaSharp.Views.Desktop;
using System.Text.Json.Nodes;

namespace RealmStudio
{
    public partial class SymbolCollectionForm : Form
    {
        private int SYMBOL_NUMBER;
        private bool COLLECTION_SAVED;
        private MapSymbolCollection? COLLECTION;
        private readonly MapSymbolCollection ORIGINAL_COLLECTION = new();
        private MapSymbol? SELECTED_SYMBOL;
        private string SEARCH_STRING = string.Empty;

        private Font TagButtonFont { get; set; } = new Font("Segoe", 8.0F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly ToolTip TOOLTIP = new();

        public SymbolCollectionForm()
        {
            InitializeComponent();

            LoadAvailableTags();
        }

        /******************************************************************************************************
        * *****************************************************************************************************
        * Dialog Event Handlers
        * *****************************************************************************************************
        *******************************************************************************************************/

        private void OpenCollectionDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new()
            {
                InitialDirectory = AssetManager.DefaultSymbolDirectory
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                CollectionPathLabel.Text = fbd.SelectedPath;

                string collectionFilePath = fbd.SelectedPath + Path.DirectorySeparatorChar + AssetManager.CollectionFileName;

                if (File.Exists(collectionFilePath))
                {
                    try
                    {
                        COLLECTION = MapFileMethods.ReadCollectionFromXml(collectionFilePath);

                        if (COLLECTION != null)
                        {
                            // load symbol file into object
                            foreach (MapSymbol symbol in COLLECTION.GetCollectionMapSymbols())
                            {
                                if (!string.IsNullOrEmpty(symbol.SymbolFilePath))
                                {
                                    if (symbol.SymbolFormat == SymbolFileFormat.PNG
                                        || symbol.SymbolFormat == SymbolFileFormat.JPG
                                        || symbol.SymbolFormat == SymbolFileFormat.BMP)
                                    {
                                        symbol.SetSymbolBitmapFromPath(symbol.SymbolFilePath);
                                        SymbolMethods.AnalyzeSymbolBitmapColors(symbol);
                                    }
                                }

                                COLLECTION.SetCollectionPath(collectionFilePath);
                                symbol.CollectionPath = collectionFilePath;

                                if (!(symbol.SymbolTags.Count > 0))
                                {
                                    List<string> potentialTags = SymbolMethods.AutoTagSymbol(symbol);

                                    foreach (string tag in potentialTags)
                                    {
                                        // add the potential tags to the symbol
                                        symbol.AddSymbolTag(tag);
                                    }
                                }

                                if (symbol.SymbolType == MapSymbolType.NotSet)
                                {
                                    SymbolMethods.GuessSymbolTypeFromTags(symbol);
                                }

                                ORIGINAL_COLLECTION.AddCollectionMapSymbol(new MapSymbol(symbol));
                            }

                            SELECTED_SYMBOL = COLLECTION.GetCollectionMapSymbols().First();

                            if (SELECTED_SYMBOL != null)
                            {
                                SYMBOL_NUMBER = 0;
                            }

                            CollectionNameTextBox.Text = COLLECTION.GetCollectionName();

                            SymbolCountLabel.Text = "Symbol " + (SYMBOL_NUMBER + 1) + " of "
                                + COLLECTION.GetCollectionMapSymbols().Count.ToString();

                            TaggedSymbolCountLabel.Text = "Tagged " + COLLECTION.GetNumberOfTaggedSymbols().ToString()
                                + " of " + COLLECTION.GetNumberOfTaggedSymbols().ToString();

                            if (SELECTED_SYMBOL != null)
                            {
                                SetUIFromSymbol(SYMBOL_NUMBER);
                            }
                        }

                    }
                    catch { }
                }
                else
                {
                    DialogResult result = MessageBox.Show("No collection file found.\nCreate a new collection from assets in the selected directory?", "No Collection File Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    if (result == DialogResult.Yes)
                    {
                        COLLECTION = new();
                        COLLECTION.SetCollectionPath(collectionFilePath);

                        string[] pathParts = fbd.SelectedPath.Split(Path.DirectorySeparatorChar);
                        COLLECTION.SetCollectionName(pathParts[^1]);

                        CollectionNameTextBox.Text = COLLECTION.GetCollectionName();

                        List<MapSymbol> symbolList = MapFileMethods.ReadMapSymbolAssets(fbd.SelectedPath);

                        string wonderdraftSymbolsFilePath = fbd.SelectedPath + Path.DirectorySeparatorChar + AssetManager.WonderdraftSymbolsFileName;
                        List<WonderdraftSymbol> wdSymbols = ReadWonderdraftSymbolsFile(wonderdraftSymbolsFilePath);

                        foreach (var mapSymbol in symbolList)
                        {
                            mapSymbol.CollectionName = COLLECTION.GetCollectionName();

                            string symbolName = mapSymbol.SymbolName;

                            List<string> potentialTags = SymbolMethods.AutoTagSymbol(mapSymbol);

                            foreach (string tag in potentialTags)
                            {
                                // add the potential tags to the symbol
                                mapSymbol.AddSymbolTag(tag);
                            }

                            SymbolMethods.GuessSymbolTypeFromTags(mapSymbol);

                            foreach (WonderdraftSymbol s in wdSymbols)
                            {
                                if (!string.IsNullOrEmpty(s.name) && s.name.Trim() == symbolName)
                                {
                                    if (s.draw_mode == "custom_colors")
                                    {
                                        mapSymbol.UseCustomColors = true;
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(mapSymbol.CollectionPath))
                            {
                                mapSymbol.CollectionPath = collectionFilePath;
                            }

                            COLLECTION.AddCollectionMapSymbol(mapSymbol);
                        }

                        SymbolCountLabel.Text = "Symbol " + (SYMBOL_NUMBER + 1) + " of "
                            + COLLECTION.GetCollectionMapSymbols().Count.ToString();

                        TaggedSymbolCountLabel.Text = "Tagged " + COLLECTION.GetNumberOfTaggedSymbols().ToString()
                            + " of " + COLLECTION.GetNumberOfTaggedSymbols().ToString();

                        SELECTED_SYMBOL = COLLECTION.GetCollectionMapSymbols().First();

                        if (SELECTED_SYMBOL != null)
                        {
                            SYMBOL_NUMBER = 0;
                            SetUIFromSymbol(SYMBOL_NUMBER);
                        }
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (COLLECTION != null && COLLECTION.GetNumberOfTaggedSymbols() < COLLECTION.CollectionMapSymbols.Count)
            {
                DialogResult result = MessageBox.Show("Not all symbols have been tagged. Save collection anyway?", "Untagged Symbols", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (result == DialogResult.Yes)
                {
                    SaveCollection();
                }
            }
            else
            {
                SaveCollection();
            }
        }

        private void CloseCollectionFormButton_Click(object sender, EventArgs e)
        {
            if (COLLECTION != null)
            {
                bool allSymbolTypesAssigned = true;

                foreach (MapSymbol symbol in COLLECTION.GetCollectionMapSymbols())
                {
                    if (symbol.SymbolType == MapSymbolType.NotSet)
                    {
                        allSymbolTypesAssigned = false;
                        break;
                    }
                }

                if (!allSymbolTypesAssigned)
                {
                    DialogResult result = MessageBox.Show("Some symbols do not have a symbol type assigned. Close anyway?", "Symbol type not assigned", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    if (result == DialogResult.Yes)
                    {
                        if (!COLLECTION_SAVED)
                        {
                            result = MessageBox.Show("The collection.xml file has not been saved. Do you want to save it?", "Collection not saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                            if (result == DialogResult.Yes)
                            {
                                SaveCollection();
                                Close();
                            }
                            else if (result == DialogResult.No)
                            {
                                Close();
                            }
                        }
                    }
                }
                else
                {
                    if (!COLLECTION_SAVED)
                    {
                        DialogResult result = MessageBox.Show("The collection.xml file has not been saved. Do you want to save it?", "Collection not saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        if (result == DialogResult.Yes)
                        {
                            SaveCollection();
                            Close();
                        }
                        else if (result == DialogResult.No)
                        {
                            Close();
                        }
                    }
                    else
                    {
                        Close();
                    }
                }
            }
            else
            {
                Close();
            }
        }

        private void ExcludeSymbolButton_Click(object sender, EventArgs e)
        {
            if (COLLECTION != null && COLLECTION.CollectionMapSymbols.Count > 0 && SELECTED_SYMBOL != null)
            {
                // remove the current symbol from the symbol collection and reset UI
                for (int i = COLLECTION.CollectionMapSymbols.Count - 1; i >= 0; i--)
                {
                    if (COLLECTION.CollectionMapSymbols[i].SymbolGuid.ToString() == SELECTED_SYMBOL.SymbolGuid.ToString())
                    {
                        COLLECTION.CollectionMapSymbols.RemoveAt(i);
                        break;
                    }
                }

                SYMBOL_NUMBER--;

                SYMBOL_NUMBER = Math.Min(SYMBOL_NUMBER, COLLECTION.GetCollectionMapSymbols().Count - 1);
                SetUIFromSymbol(SYMBOL_NUMBER);

                AdvanceToNextSymbol();
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (COLLECTION != null)
            {
                if (SELECTED_SYMBOL != null)
                {
                    if (StructureRadioButton.Checked)
                    {
                        SELECTED_SYMBOL.SymbolType = MapSymbolType.Structure;
                    }
                    else if (VegetationRadioButton.Checked)
                    {
                        SELECTED_SYMBOL.SymbolType = MapSymbolType.Vegetation;
                    }
                    else if (TerrainRadioButton.Checked)
                    {
                        SELECTED_SYMBOL.SymbolType = MapSymbolType.Terrain;
                    }
                    else if (MarkerRadioButton.Checked)
                    {
                        SELECTED_SYMBOL.SymbolType = MapSymbolType.Marker;
                    }
                    else if (OtherRadioButton.Checked)
                    {
                        SELECTED_SYMBOL.SymbolType = MapSymbolType.Other;
                    }
                    else
                    {
                        SELECTED_SYMBOL.SymbolType = MapSymbolType.NotSet;
                    }

                    if (!string.IsNullOrEmpty(SymbolNameTextBox.Text))
                    {
                        SELECTED_SYMBOL.SymbolName = SymbolNameTextBox.Text;
                    }

                    if (GrayScaleSymbolRadio.Checked)
                    {
                        SELECTED_SYMBOL.IsGrayscale = true;
                    }

                    if (CustomColorSymbolRadio.Checked)
                    {
                        SELECTED_SYMBOL.UseCustomColors = true;
                    }

                    SELECTED_SYMBOL.ClearSymbolTags();

                    List<string> selectedTags = GetSelectedTags();

                    foreach (string tag in selectedTags)
                    {
                        if (!string.IsNullOrEmpty(tag))
                        {
                            if (!SELECTED_SYMBOL.SymbolTags.Contains(tag))
                            {
                                SELECTED_SYMBOL.AddSymbolTag(tag);
                            }
                        }
                    }

                    COLLECTION_SAVED = false;
                }

                TaggedSymbolCountLabel.Text = "Tagged " + COLLECTION.GetNumberOfTaggedSymbols().ToString()
                    + " of " + COLLECTION.GetNumberOfTaggedSymbols().ToString();

                TaggedSymbolCountLabel.Refresh();

                if (AutoAdvanceCheck.Checked)
                {
                    AdvanceToNextSymbol();
                }
            }
        }

        private void ResetTagsButton_Click(object sender, EventArgs e)
        {
            SELECTED_SYMBOL?.ClearSymbolTags();

            List<string> originalTags = ORIGINAL_COLLECTION.CollectionMapSymbols[SYMBOL_NUMBER].SymbolTags;

            foreach (string tag in originalTags)
            {
                SELECTED_SYMBOL?.AddSymbolTag(tag);
            }

            SetUIFromSymbol(SYMBOL_NUMBER);
        }

        private void TagSearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // filter the tag list based on what user enters in search box
            SEARCH_STRING = TagSearchTextBox.Text;
            FilterTags(SEARCH_STRING);
        }

        private void AddTagButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTagTextBox.Text) && !AssetManager.SYMBOL_TAGS.Contains(NewTagTextBox.Text))
            {
                AssetManager.SYMBOL_TAGS.Add(NewTagTextBox.Text);
                LoadAvailableTags();
                NewTagTextBox.Clear();
            }
        }

        private void AvailableTagButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is Button tagButton)
            {
                string? tag = (string?)tagButton.Tag;

                if (e.Button == MouseButtons.Left)
                {
                    if (!string.IsNullOrWhiteSpace(tag) && SELECTED_SYMBOL != null && !SELECTED_SYMBOL.SymbolTags.Contains(tag))
                    {
                        SELECTED_SYMBOL.SymbolTags.Add(tag);
                        SetUIFromSymbol(SYMBOL_NUMBER);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    ConfirmTagDeletion confirmTagDeletion = new()
                    {
                        Left = AvailableTagsPanel.PointToScreen(Point.Empty).X + tagButton.Right + 10,
                        Top = AvailableTagsPanel.PointToScreen(Point.Empty).Y + tagButton.Top,
                        Width = 53,
                        Height = 28,
                    };

                    DialogResult result = confirmTagDeletion.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        if (!string.IsNullOrWhiteSpace(tag) && AssetManager.SYMBOL_TAGS.Contains(tag))
                        {
                            AssetManager.SYMBOL_TAGS.Remove(tag);

                            if (SELECTED_SYMBOL != null && SELECTED_SYMBOL.SymbolTags.Contains(tag))
                            {
                                SELECTED_SYMBOL.SymbolTags.Remove(tag);
                            }

                            LoadAvailableTags();
                        }
                    }
                }
            }
        }

        private void TagButton_MouseHover(object? sender, EventArgs e)
        {
            if (sender is Button tagButton)
            {
                string? tag = (string?)tagButton.Tag;

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    TOOLTIP.Show(tag, SymbolTagsPanel, new Point(tagButton.Left + 10, tagButton.Top - 15), 2000);
                }
            }
        }

        private void TagButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button tagButton)
            {
                ConfirmTagDeletion confirmTagDeletion = new()
                {
                    Left = SymbolTagsPanel.PointToScreen(Point.Empty).X + tagButton.Right + 10,
                    Top = SymbolTagsPanel.PointToScreen(Point.Empty).Y + tagButton.Top,
                    Width = 53,
                    Height = 28,
                };

                DialogResult result = confirmTagDeletion.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string? tag = (string?)tagButton.Tag;

                    if (SELECTED_SYMBOL != null && !string.IsNullOrEmpty(tag) && SELECTED_SYMBOL.SymbolTags.Contains(tag))
                    {
                        SELECTED_SYMBOL.SymbolTags.Remove(tag);
                        SymbolTagsPanel.Controls.Remove(tagButton);
                        SymbolTagsPanel.Refresh();
                    }
                }
            }
        }

        /******************************************************************************************************
        * *****************************************************************************************************
        * Dialog Methods
        * *****************************************************************************************************
        *******************************************************************************************************/
        private void LoadAvailableTags()
        {
            AvailableTagsPanel.Controls.Clear();

            foreach (string tag in AssetManager.SYMBOL_TAGS)
            {
                IconButton availableTagButton = new()
                {
                    AutoEllipsis = true,
                    AutoSize = false,
                    Width = 140,
                    Height = 24,
                    Text = StringExtensions.Truncate(tag, 12, true),
                    ForeColor = SystemColors.ControlDarkDark,
                    BackColor = Color.Linen,
                    Font = TagButtonFont,
                    IconChar = IconChar.Check,
                    IconSize = 14,
                    ImageAlign = ContentAlignment.TopLeft,
                    TextAlign = ContentAlignment.TopRight,
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    MaximumSize = new Size(140, 24),
                    MinimumSize = new Size(140, 24),
                    Tag = tag,
                };

                availableTagButton.MouseUp += AvailableTagButton_MouseUp;

                AvailableTagsPanel.Controls.Add(availableTagButton);
            }
        }

        private void SetUIFromSymbol(int symbolNumber)
        {
            if (COLLECTION != null)
            {
                SymbolCountLabel.Text = "Symbol " + (SYMBOL_NUMBER + 1) + " of "
                    + COLLECTION.GetCollectionMapSymbols().Count.ToString();

                TaggedSymbolCountLabel.Text = "Tagged " + COLLECTION.GetNumberOfTaggedSymbols().ToString()
                    + " of " + COLLECTION.GetNumberOfTaggedSymbols().ToString();

                symbolNumber = Math.Max(symbolNumber, 0);
                SELECTED_SYMBOL = COLLECTION.GetCollectionMapSymbols()[symbolNumber];

                if (SELECTED_SYMBOL.SymbolType != MapSymbolType.NotSet)
                {
                    SetSymbolTypeRadio(SELECTED_SYMBOL);
                }

                SymbolNameTextBox.Text = GetSymbolName(SELECTED_SYMBOL);
                GrayScaleSymbolRadio.Checked = SELECTED_SYMBOL.IsGrayscale;
                CustomColorSymbolRadio.Checked = SELECTED_SYMBOL.UseCustomColors;

                SymbolTagsPanel.Controls.Clear();
                SymbolTagsPanel.Refresh();

                foreach (string tag in SELECTED_SYMBOL.SymbolTags)
                {
                    IconButton tagButton = new()
                    {
                        AutoEllipsis = true,
                        AutoSize = false,
                        Width = 75,
                        Height = 24,
                        Text = StringExtensions.Truncate(tag, 6, true),
                        ForeColor = SystemColors.ControlDarkDark,
                        BackColor = Color.Linen,
                        Font = TagButtonFont,
                        IconChar = IconChar.X,
                        IconSize = 14,
                        ImageAlign = ContentAlignment.TopLeft,
                        TextAlign = ContentAlignment.TopRight,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        MaximumSize = new Size(75, 24),
                        MinimumSize = new Size(75, 24),
                        Tag = tag,
                    };

                    tagButton.Click += TagButton_Click;
                    tagButton.MouseHover += TagButton_MouseHover;

                    SymbolTagsPanel.Controls.Add(tagButton);
                }

                SymbolNameTextBox.Text = SELECTED_SYMBOL.SymbolName;
                GrayScaleSymbolRadio.Checked = SELECTED_SYMBOL.IsGrayscale;
                CustomColorSymbolRadio.Checked = SELECTED_SYMBOL.UseCustomColors;

                SymbolPictureBox.Image = Extensions.ToBitmap(SELECTED_SYMBOL.SymbolBitmap);
            }
        }

        private List<string> GetSelectedTags()
        {
            List<string> symbolTags = [];

            foreach (Button symbolTag in SymbolTagsPanel.Controls)
            {
                string? tag = (string?)symbolTag.Tag;

                if (!string.IsNullOrEmpty(tag))
                {
                    symbolTags.Add(tag);
                }
            }

            return symbolTags;
        }

        private void SetSymbolTypeRadio(MapSymbol symbol)
        {
            switch (symbol.SymbolType)
            {
                case MapSymbolType.Terrain:
                    TerrainRadioButton.Checked = true;
                    break;
                case MapSymbolType.Structure:
                    StructureRadioButton.Checked = true;
                    break;
                case MapSymbolType.Vegetation:
                    VegetationRadioButton.Checked = true;
                    break;
                case MapSymbolType.Marker:
                    MarkerRadioButton.Checked = true;
                    break;
                case MapSymbolType.Other:
                    OtherRadioButton.Checked = true;
                    break;
                default:
                    TerrainRadioButton.Checked = false;
                    StructureRadioButton.Checked = false;
                    VegetationRadioButton.Checked = false;
                    MarkerRadioButton.Checked = false;
                    OtherRadioButton.Checked = false;
                    break;
            }
        }

        private void FirstSymbolButton_Click(object sender, EventArgs e)
        {
            SYMBOL_NUMBER = 0;
            SetUIFromSymbol(0);
        }

        private void NextSymbolButton_Click(object sender, EventArgs e)
        {
            AdvanceToNextSymbol();
        }

        private void PreviousSymbolButton_Click(object sender, EventArgs e)
        {
            SYMBOL_NUMBER = (SYMBOL_NUMBER > 0) ? SYMBOL_NUMBER - 1 : 0;
            SetUIFromSymbol(SYMBOL_NUMBER);
        }

        private void LastSymbolButton_Click(object sender, EventArgs e)
        {
            if (COLLECTION != null)
            {
                SYMBOL_NUMBER = COLLECTION.GetCollectionMapSymbols().Count - 1;
                SetUIFromSymbol(SYMBOL_NUMBER);
            }
        }

        private void AdvanceToNextSymbol()
        {
            if (COLLECTION != null && SELECTED_SYMBOL != null)
            {
                if (SELECTED_SYMBOL.SymbolType != MapSymbolType.NotSet)
                {
                    SYMBOL_NUMBER++;
                    SYMBOL_NUMBER = Math.Min(SYMBOL_NUMBER, COLLECTION.GetCollectionMapSymbols().Count - 1);
                    SetUIFromSymbol(SYMBOL_NUMBER);
                }
                else
                {
                    MessageBox.Show("Please select a symbol type (Structure, Vegetation, Terrain, Marker, Other) before advancing to the next symbol.", "Symbol type not set", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        private void SaveCollection()
        {
            if (COLLECTION != null && COLLECTION.GetCollectionPath().Length > 0)
            {
                MapFileMethods.SerializeSymbolCollection(COLLECTION);
                MessageBox.Show("Collection has been saved.", "Collection Saved", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                COLLECTION_SAVED = true;
            }
        }

        private void FilterTags(string searchString)
        {
            foreach (Button tagButton in AvailableTagsPanel.Controls)
            {
                tagButton.Show();
            }

            List<string> tags = [.. AssetManager.SYMBOL_TAGS];

            if (searchString.Length > 2)
            {
                for (int i = tags.Count - 1; i >= 0; i--)
                {
                    if (!tags[i].Contains(searchString))
                    {
                        tags.RemoveRange(i, 1);
                    }
                }
            }

            foreach (Button tagButton in AvailableTagsPanel.Controls)
            {
                if (!tags.Contains(tagButton.Tag))
                {
                    tagButton.Hide();
                }
            }
        }

        /******************************************************************************************************
        * *****************************************************************************************************
        * Static Methods
        * *****************************************************************************************************
        *******************************************************************************************************/

        internal static string GetSymbolName(MapSymbol symbol)
        {
            string symbolName = symbol.SymbolName;

            if (string.IsNullOrEmpty(symbolName))
            {
                symbolName = Path.GetFileNameWithoutExtension(symbol.SymbolFilePath);
                symbol.SymbolName = symbolName;
            }

            return symbolName;
        }

        private static List<WonderdraftSymbol> ReadWonderdraftSymbolsFile(string wonderdraftSymbolsFilePath)
        {
            List<WonderdraftSymbol> wonderdraftSymbols = [];

            try
            {
                // the .wonderdraft_symbols file isn't really needed - the drawing mode
                // is already determined from the bitmap colors
                if (File.Exists(wonderdraftSymbolsFilePath))
                {
                    string jsonString = File.ReadAllText(wonderdraftSymbolsFilePath);

                    var jsonObject = JsonNode.Parse(jsonString)?.AsObject();

                    if (jsonObject != null)
                    {
                        foreach (KeyValuePair<string, JsonNode?> kvPair in jsonObject)
                        {
                            WonderdraftSymbol ws = new()
                            {
                                name = kvPair.Key
                            };

                            if (kvPair.Value != null)
                            {
                                foreach (var kvp in kvPair.Value.AsObject().Where(kvp => kvp.Key == "draw_mode"))
                                {
                                    ws.draw_mode = (string?)kvp.Value;
                                }

                                wonderdraftSymbols.Add(ws);
                            }
                        }
                    }
                }
            }
            catch { }

            return wonderdraftSymbols;
        }
    }
}
