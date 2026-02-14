namespace RealmStudioX
{
    internal sealed class LabelPresetUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        private MapStateMediator? _mapState;
        private LabelPreset? _deletingPreset;

        #region Proerty Setters/Getters
        internal MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        internal LabelPreset? DeletingPreset
        {
            get { return _deletingPreset; }
            set { _deletingPreset = value; }
        }

        #endregion

        #region Property Change Handler Methods

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                RaiseChanged();
            }
        }

        #endregion

        #region Label Preset UI Methods

        /*
        internal void AddLabelPresets()
        {
            MainForm.LabelPresetsListBox.DisplayMember = "LabelPresetName";
            MainForm.LabelPresetsListBox.Items.Clear();

            foreach (LabelPreset preset in LabelPresetManager.LabelPresets)
            {
                if (!string.IsNullOrEmpty(preset.LabelPresetTheme)
                    && AssetManager.CURRENT_THEME != null
                    && preset.LabelPresetTheme == AssetManager.CURRENT_THEME.ThemeName)
                {
                    MainForm.LabelPresetsListBox.Items.Add(preset);
                }
            }

            MainForm.LabelPresetsListBox.Refresh();

            if (MainForm.LabelPresetsListBox.Items.Count > 0)
            {
                MainForm.LabelPresetsListBox.SelectedIndex = 0;

                if (MainForm.LabelPresetsListBox.Items[0] is LabelPreset selectedPreset)
                {
                    SetLabelValuesFromPreset(selectedPreset);
                }
            }
        }

        internal void RemoveSelectedPreset()
        {
            //remove a preset (prevent default presets from being deleted or changed)

            if (MainForm.LabelPresetsListBox.SelectedIndex >= 0)
            {
                string? presetName = ((LabelPreset?)MainForm.LabelPresetsListBox.SelectedItem)?.LabelPresetName;

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

                    LabelPreset? existingPreset = LabelPresetManager.LabelPresets.Find(x => x.LabelPresetName == presetName && x.LabelPresetTheme == currentThemeName);

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
                                    DeletingPreset = existingPreset;
                                    bool presetDeleted = LabelPresetManager.Delete();

                                    if (presetDeleted)
                                    {
                                        // why reload all assets? can't the presets just be reloaded?
                                        LabelPresetManager.LoadLabelPresets();

                                        AddLabelPresets();

                                        MessageBox.Show("The label preset has been deleted.", "Preset Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                                    }
                                    else
                                    {
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

        internal void SetLabelValuesFromPreset(LabelPreset preset)
        {
            ArgumentNullException.ThrowIfNull(LabelManager.LabelMediator);

            LabelManager.LabelMediator.LabelColor = Color.FromArgb(preset.LabelColor);
            LabelManager.LabelMediator.OutlineColor = Color.FromArgb(preset.LabelOutlineColor);
            LabelManager.LabelMediator.OutlineWidth = preset.LabelOutlineWidth;
            LabelManager.LabelMediator.GlowColor = Color.FromArgb(preset.LabelGlowColor);

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
                    LabelManager.LabelMediator.SelectedLabelFont = new Font(family, fontsize, FontStyle.Regular, GraphicsUnit.Point);
                }
                catch
                {
                    // couldn't create the font, so try to find it in the list of embedded fonts
                    for (int i = 0; i < LabelManager.EMBEDDED_FONTS.Families.Length; i++)
                    {
                        if (LabelManager.EMBEDDED_FONTS.Families[i].Name == ff)
                        {
                            LabelManager.LabelMediator.SelectedLabelFont = new Font(LabelManager.EMBEDDED_FONTS.Families[i], fontsize, FontStyle.Regular, GraphicsUnit.Point);
                            break;
                        }
                    }
                }
            }
            else
            {
                LabelManager.LabelMediator.SelectedLabelFont = MapStateMediator.DefaultLabelFont;
            }

            int selectedFontIndex = 0;

            if (MainForm.FontFamilyCombo.Items != null && MainForm.FontFamilyCombo.Items.Count > 0)
            {
                for (int i = 0; i < MainForm.FontFamilyCombo.Items?.Count; i++)
                {
                    if (MainForm.FontFamilyCombo.Items != null && MainForm.FontFamilyCombo.Items[i] != null
                        && ((Font?)MainForm.FontFamilyCombo.Items[i])?.FontFamily.Name == LabelManager.LabelMediator.SelectedLabelFont.FontFamily.Name)
                    {
                        selectedFontIndex = i;
                        break;
                    }
                }

                MainForm.FontFamilyCombo.SelectedIndex = selectedFontIndex;
            }

            int selectedSizeIndex = 0;

            if (MainForm.FontSizeCombo.Items != null && MainForm.FontSizeCombo.Items.Count > 0)
            {
                for (int i = 0; i < MainForm.FontSizeCombo.Items?.Count; i++)
                {
                    if (MainForm.FontSizeCombo.Items != null && MainForm.FontSizeCombo.Items[i] != null
                        && MainForm.FontSizeCombo.Items[i]?.ToString() == Math.Round(LabelManager.LabelMediator.SelectedLabelFont.SizeInPoints).ToString())
                    {
                        selectedSizeIndex = i;
                        break;
                    }
                }

                MainForm.FontSizeCombo.SelectedIndex = selectedSizeIndex;
            }
        }

        internal void AddNewLabelPreset()
        {
            ArgumentNullException.ThrowIfNull(LabelManager.LabelMediator);

            LabelPreset? selectedPreset = null;

            if (MainForm.LabelPresetsListBox.SelectedIndex >= 0 && MainForm.LabelPresetsListBox.SelectedIndex < LabelPresetManager.LabelPresets.Count)
            {
                selectedPreset = (LabelPreset)MainForm.LabelPresetsListBox.Items[MainForm.LabelPresetsListBox.SelectedIndex];
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
                    LabelPreset? existingPreset = LabelPresetManager.LabelPresets.Find(x => x.LabelPresetName == presetName && x.LabelPresetTheme == currentThemeName);
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

                    Font f = new(LabelManager.LabelMediator.SelectedLabelFont.FontFamily,
                        LabelManager.LabelMediator.SelectedLabelFont.Size * 0.75F,
                        LabelManager.LabelMediator.SelectedLabelFont.Style, GraphicsUnit.Point);

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

                        if (LabelManager.LabelMediator != null)
                        {
                            preset.LabelColor = LabelManager.LabelMediator.LabelColor.ToArgb();
                            preset.LabelOutlineColor = LabelManager.LabelMediator.OutlineColor.ToArgb();
                            preset.LabelOutlineWidth = LabelManager.LabelMediator.OutlineWidth;
                            preset.LabelGlowColor = LabelManager.LabelMediator.GlowColor.ToArgb();
                            preset.LabelGlowStrength = (int)LabelManager.LabelMediator.GlowStrength;
                        }

                        preset.PresetXmlFilePath = presetFileName;

                        MapFileMethods.SerializeLabelPreset(preset);

                        SetLabelValuesFromPreset(preset);

                        LabelPresetManager.AddPreset(preset);
                    }
                }
            }
        }

        internal void SelectPreset(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < LabelPresetManager.LabelPresets.Count)
            {
                LabelPreset selectedPreset = (LabelPreset)MainForm.LabelPresetsListBox.Items[selectedIndex];
                SetLabelValuesFromPreset(selectedPreset);
            }
        }

        */
        #endregion
    }
}
