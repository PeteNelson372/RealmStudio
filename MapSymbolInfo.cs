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
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Data;

namespace RealmStudio
{
    public sealed partial class MapSymbolInfo : Form
    {
        private readonly RealmStudioMap Map;
        private readonly MapSymbol symbol;
        private readonly MapSymbolCollection? collection;
        private readonly Color[] originalColors = new Color[3];

        public MapSymbolInfo(RealmStudioMap map, MapSymbol symbol)
        {
            InitializeComponent();
            Map = map;
            this.symbol = symbol;

            foreach (MapSymbolCollection symbolCollection in AssetManager.MAP_SYMBOL_COLLECTIONS)
            {
                if (symbolCollection.GetCollectionPath() == symbol.CollectionPath)
                {
                    collection = symbolCollection;
                    break;
                }
            }

            SymbolNameLabel.Text = this.symbol.SymbolName;
            CollectionNameLabel.Text = this.symbol.CollectionName;
            SymbolPathLabel.Text = this.symbol.SymbolFilePath;
            SymbolGuidLabel.Text = this.symbol.SymbolGuid.ToString();
            SymbolFormatLabel.Text = this.symbol.SymbolFormat.ToString();

            switch (this.symbol.SymbolType)
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
            }

            if (this.symbol.IsGrayscale)
            {
                GrayScaleSymbolRadio.Checked = true;
            }

            if (this.symbol.UseCustomColors)
            {
                UseCustomColorsRadio.Checked = true;
            }

            AddTagsToListBox(AssetManager.SYMBOL_TAGS);

            foreach (string tag in this.symbol.SymbolTags)
            {
                // check the symbol tags
                CheckTagInTagList(tag, CheckState.Checked);
            }

            originalColors[0] = SymbolColor1Button.BackColor;
            originalColors[1] = SymbolColor2Button.BackColor;
            originalColors[2] = SymbolColor3Button.BackColor;

            if (this.symbol.CustomSymbolColors[0] != SKColors.Empty
                && this.symbol.CustomSymbolColors[0].ToString() != SymbolColor1Button.BackColor.ToSKColor().ToString())
            {
                SymbolColor1Button.BackColor = this.symbol.CustomSymbolColors[0].ToDrawingColor();
                originalColors[0] = SymbolColor1Button.BackColor;
            }

            if (this.symbol.CustomSymbolColors[1] != SKColors.Empty
                && this.symbol.CustomSymbolColors[1].ToString() != SymbolColor2Button.BackColor.ToSKColor().ToString())
            {
                SymbolColor2Button.BackColor = this.symbol.CustomSymbolColors[1].ToDrawingColor();
                originalColors[1] = SymbolColor2Button.BackColor;
            }

            if (this.symbol.CustomSymbolColors[2] != SKColors.Empty
                && this.symbol.CustomSymbolColors[2].ToString() != SymbolColor3Button.BackColor.ToSKColor().ToString())
            {
                SymbolColor3Button.BackColor = this.symbol.CustomSymbolColors[2].ToDrawingColor();
                originalColors[2] = SymbolColor3Button.BackColor;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddTagButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTagTextBox.Text) && !CheckedTagsListBox.Items.Contains(NewTagTextBox.Text))
            {
                string newTag = new string([.. NewTagTextBox.Text.Where(char.IsLetter)]).ToLowerInvariant();

                if (!string.IsNullOrEmpty(newTag))
                {
                    AddTagToTagList(newTag);
                    AssetManager.AddSymbolTag(newTag);

                    if (collection != null)
                    {
                        collection.IsModified = true;
                    }
                }

                NewTagTextBox.Clear();
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            symbol.ClearSymbolTags();

            List<string> selectedTags = GetSelectedTags();

            foreach (string tag in selectedTags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    if (!symbol.SymbolTags.Contains(tag))
                    {
                        symbol.AddSymbolTag(tag);

                        if (collection != null)
                        {
                            collection.IsModified = true;
                        }
                    }
                }
            }
        }

        private List<string> GetSelectedTags()
        {
            List<string> checkedTags = [.. CheckedTagsListBox.CheckedItems.Cast<string>()];
            return checkedTags;
        }

        private void AddTagsToListBox(List<string> tags)
        {
            foreach (string tag in tags)
            {
                // add the tags in the symbol to the tag list box
                AddTagToTagList(tag);
            }
        }

        private void AddTagToTagList(string tag)
        {
            tag = tag.Trim([' ', ',']).ToLowerInvariant();

            if (!CheckedTagsListBox.Items.Contains(tag))
            {
                CheckedTagsListBox.Items.Add(tag);
            }
        }

        private void CheckTagInTagList(string tag, CheckState checkState)
        {
            int tagIndex = CheckedTagsListBox.Items.IndexOf(tag);
            if (tagIndex > -1)
            {
                CheckedTagsListBox.SetItemCheckState(tagIndex, checkState);
            }
        }

        private void CloseFormButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SymbolColor1Button_Click(object sender, EventArgs e)
        {
            Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor1Button.BackColor);

            SymbolColor1Button.BackColor = c;
            SymbolColor1Button.Refresh();

            SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
            SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
            SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

            if (symbol.IsGrayscale || symbol.UseCustomColors)
            {
                Cmd_PaintSymbol cmd = new(symbol, paintColor1, paintColor1, paintColor2, paintColor3);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        private void SymbolColor2Button_Click(object sender, EventArgs e)
        {
            Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor2Button.BackColor);

            SymbolColor2Button.BackColor = c;
            SymbolColor2Button.Refresh();

            SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
            SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
            SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

            if (symbol.IsGrayscale || symbol.UseCustomColors)
            {
                Cmd_PaintSymbol cmd = new(symbol, paintColor2, paintColor1, paintColor2, paintColor3);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        private void SymbolColor3Button_Click(object sender, EventArgs e)
        {
            Color c = UtilityMethods.SelectColorFromDialog(this, SymbolColor3Button.BackColor);

            SymbolColor3Button.BackColor = c;
            SymbolColor3Button.Refresh();

            SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
            SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
            SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

            if (symbol.IsGrayscale || symbol.UseCustomColors)
            {
                Cmd_PaintSymbol cmd = new(symbol, paintColor3, paintColor1, paintColor2, paintColor3);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }

        private void ResetSymbolColorsButton_Click(object sender, EventArgs e)
        {
            SymbolColor1Button.BackColor = originalColors[0];
            SymbolColor2Button.BackColor = originalColors[1];
            SymbolColor3Button.BackColor = originalColors[2];

            SKColor paintColor1 = SymbolColor1Button.BackColor.ToSKColor();
            SKColor paintColor2 = SymbolColor2Button.BackColor.ToSKColor();
            SKColor paintColor3 = SymbolColor3Button.BackColor.ToSKColor();

            if (symbol.IsGrayscale || symbol.UseCustomColors)
            {
                Cmd_PaintSymbol cmd = new(symbol, paintColor3, paintColor1, paintColor2, paintColor3);
                CommandManager.AddCommand(cmd);
                cmd.DoOperation();
            }
        }
    }
}
