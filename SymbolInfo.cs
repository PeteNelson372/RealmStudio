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
using System.Data;

namespace RealmStudio
{
    public partial class SymbolInfo : Form
    {
        private readonly MapSymbol symbol;
        private readonly MapSymbolCollection? collection;

        public SymbolInfo(MapSymbol symbol)
        {
            InitializeComponent();
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
                case SymbolTypeEnum.Terrain:
                    TerrainRadioButton.Checked = true;
                    break;
                case SymbolTypeEnum.Structure:
                    StructureRadioButton.Checked = true;
                    break;
                case SymbolTypeEnum.Vegetation:
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
    }
}
