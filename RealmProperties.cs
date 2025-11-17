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
    public partial class RealmProperties : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private RealmStudioMap _realm;
        private bool NameLocked;

        public RealmProperties(RealmStudioMap realm)
        {
            InitializeComponent();

            _realm = realm;

            RealmGuidLabel.Text = realm.MapGuid.ToString();
            NameTextbox.Text = realm.MapName;
            MapSizeLabel.Text = realm.MapWidth.ToString() + "x" + realm.MapHeight.ToString() + " pixels";
            MapAreaLabel.Text = realm.MapAreaWidth.ToString() + "x" + realm.MapAreaHeight.ToString() + " " + realm.MapAreaUnits;
            MapFilePathLabel.Text = realm.MapPath;
            RealmTypeLabel.Text = realm.RealmType.ToString();
        }

        private void RealmDescriptionButton_Click(object sender, EventArgs e)
        {
            DescriptionEditor descriptionEditor = new(typeof(RealmStudioMap), NameTextbox.Text, _realm.RealmDescription);
            descriptionEditor.DescriptionEditorOverlay.Text = "Realm Description Editor";

            DialogResult r = descriptionEditor.ShowDialog(this);

            if (r == DialogResult.OK)
            {
                _realm.RealmDescription = descriptionEditor.DescriptionText;
            }
        }

        private void GenerateRealmNameButton_Click(object sender, EventArgs e)
        {
            if (NameLocked)
            {
                return; // Do not generate a new name if the name is locked
            }

            List<INameGenerator> generators = RealmStudioMainForm.NAME_GENERATOR_CONFIG.GetSelectedNameGenerators();
            string generatedName = MapToolMethods.GenerateRandomPlaceName(generators);
            NameTextbox.Text = generatedName;
            _realm.MapName = generatedName;
        }

        private void GenerateRealmNameButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Generate Realm Name", this, new Point(GenerateRealmNameButton.Left, GenerateRealmNameButton.Top - 20), 3000);
        }

        private void LockNameButton_Click(object sender, EventArgs e)
        {
            NameLocked = !NameLocked;
            if (NameLocked)
            {
                LockNameButton.IconChar = FontAwesome.Sharp.IconChar.Lock;
            }
            else
            {
                LockNameButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
            }
        }

        private void RealmDescriptionButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Edit Realm Description", this, new Point(RealmDescriptionButton.Left, RealmDescriptionButton.Top - 20), 3000);
        }
    }
}
