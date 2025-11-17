using System.ComponentModel;
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

namespace RealmStudio
{
    public partial class NameGeneratorConfiguration : Form
    {
        const int NumberOfNamesToGenerate = 10;
        const int NumberOfNamesToKeep = 30;

        public event EventHandler? NameGenerated;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedName { get; set; } = string.Empty;

        public NameGeneratorConfiguration()
        {
            InitializeComponent();
        }

        public List<INameGenerator> GetSelectedNameGenerators()
        {
            for (int i = 0; i < NameGeneratorsListBox.Items.Count; i++)
            {
                ((NameGenerator)NameGeneratorsListBox.Items[i]).IsSelected = NameGeneratorsListBox.GetItemChecked(i);
            }

            for (int i = 0; i < NamebasesListBox.Items.Count; i++)
            {
                ((NameBase)NamebasesListBox.Items[i]).IsNameBaseSelected = NamebasesListBox.GetItemChecked(i);
            }

            for (int i = 0; i < LanguagesListBox.Items.Count; i++)
            {
                ((NameBaseLanguage)LanguagesListBox.Items[i]).IsLanguageSelected = LanguagesListBox.GetItemChecked(i);
            }

            List<INameGenerator> generators = [];

            foreach (NameGenerator generator in NameGeneratorsListBox.Items)
            {
                if (generator.IsSelected)
                {
                    generators.Add(generator);
                }
            }

            foreach (NameBase nameBase in NamebasesListBox.Items)
            {
                if (nameBase.IsNameBaseSelected)
                {
                    generators.Add(nameBase);

                    foreach (NameBaseLanguage language in nameBase.Languages)
                    {
                        foreach (NameBaseLanguage l in LanguagesListBox.Items)
                        {
                            if (l.Language == language.Language && l.IsLanguageSelected)
                            {
                                if (!generators.Contains(l))
                                {
                                    generators.Add(l);
                                }
                            }
                        }
                    }
                }
            }

            foreach (NameBaseLanguage l in LanguagesListBox.Items)
            {
                if (l.IsLanguageSelected)
                {
                    if (!generators.Contains(l))
                    {
                        generators.Add(l);
                    }
                }
            }

            return generators;
        }

        private void ApplySelectedNameButton_Click(object sender, EventArgs e)
        {
            // apply name generator settings
            OnNameGenerated(EventArgs.Empty);
        }

        private void CloseNameConfigurationButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        protected virtual void OnNameGenerated(EventArgs e)
        {
            NameGenerated?.Invoke(this, e);
        }

        private void GenerateNamesButton_Click(object sender, EventArgs e)
        {
            List<INameGenerator> generators = GetSelectedNameGenerators();

            if (generators.Count > 0)
            {
                int generatedNameCount = 0;

                int guardCount = 0;
                int maxTries = 100;

                while (generatedNameCount < NumberOfNamesToGenerate && guardCount < maxTries)
                {
                    guardCount++;
                    string name = MapToolMethods.GenerateRandomPlaceName(generators);

                    if (!string.IsNullOrEmpty(name))
                    {
                        generatedNameCount++;
                        GeneratedNamesList.Items.Add(name);

                        if (GeneratedNamesList.Items.Count > NumberOfNamesToKeep)
                        {
                            GeneratedNamesList.Items.RemoveAt(0);
                        }
                    }
                }
            }

            GeneratedNamesList.Refresh();
            GeneratedNamesList.TopIndex = GeneratedNamesList.Items.Count - 1;
        }

        private void GeneratedNamesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GeneratedNamesList.SelectedItem != null && !string.IsNullOrEmpty((string?)GeneratedNamesList.SelectedItem))
            {
                SelectedName = (string)GeneratedNamesList.SelectedItem;
            }
        }

        private void CopyToClipboardButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedName))
            {
                Clipboard.SetText(SelectedName);
            }
            else
            {
                string clipboardString = string.Empty;

                foreach (string s in GeneratedNamesList.Items)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        clipboardString += s + "\n";
                    }
                }

                if (clipboardString != string.Empty)
                {
                    Clipboard.SetText(clipboardString);
                }
            }

            GeneratedNamesList.SelectedItems.Clear();
            SelectedName = string.Empty;
        }

        private void SelectAllNameGeneratorsCheck_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < NameGeneratorsListBox.Items.Count; i++)
            {
                NameGeneratorsListBox.SetItemChecked(i, SelectAllNameGeneratorsCheck.Checked);
            }
        }

        private void SelectAllNamebasesCheck_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < NamebasesListBox.Items.Count; i++)
            {
                NamebasesListBox.SetItemChecked(i, SelectAllNamebasesCheck.Checked);
            }
        }

        private void SelectAllLanguagesCheck_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < LanguagesListBox.Items.Count; i++)
            {
                LanguagesListBox.SetItemChecked(i, SelectAllLanguagesCheck.Checked);
            }
        }

        private void ClearNameListButton_Click(object sender, EventArgs e)
        {
            GeneratedNamesList.Items.Clear();
        }
    }
}
