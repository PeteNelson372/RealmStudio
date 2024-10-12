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
using RealmStudio.Properties;

namespace RealmStudio
{
    internal class ThemeMethods
    {
        public static void SaveTheme(MapTheme theme)
        {
            if (theme != null)
            {
                // serialze theme as XML to a file

                // validate the data entered in the dialog
                //  - verify theme name is entered
                //  - what other data is required?
                // select a path

                string assetDirectory = Resources.ASSET_DIRECTORY;
                string themeDirectory = assetDirectory + Path.DirectorySeparatorChar + "Themes";

                SaveFileDialog sfd = new()
                {
                    Title = "Save Theme",
                    AddExtension = true,
                    CreatePrompt = true,
                    DefaultExt = "mctheme",
                    Filter = "Realm Studio Theme files (*.rstheme)|*.rstheme|All files (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = theme.ThemeName,
                    CheckPathExists = true,
                    InitialDirectory = themeDirectory
                };

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // write the theme as XML
                    theme.ThemePath = sfd.FileName;
                    theme.ThemeName = Path.GetFileNameWithoutExtension(sfd.FileName);

                    MapFileMethods.SerializeTheme(theme);
                }
            }
        }
    }
}
