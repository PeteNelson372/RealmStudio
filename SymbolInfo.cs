using System.Data;

namespace RealmStudio
{
    public partial class SymbolInfo : Form
    {
        private MapSymbol symbol;
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
                string newTag = new string(NewTagTextBox.Text.Where(char.IsLetter).ToArray()).ToLower();

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
            List<string> checkedTags = CheckedTagsListBox.CheckedItems.Cast<string>().ToList();
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
            tag = tag.Trim([' ', ',']).ToLower();

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
