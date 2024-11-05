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
using System.Diagnostics;

namespace RealmStudio
{
    public partial class AboutRealmStudio : Form
    {
        public AboutRealmStudio()
        {
            InitializeComponent();
        }

        private void AboutOkButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GithubPictureBox_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void RealmStudioSourceLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/PeteNelson372/RealmStudio",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void GPL3LicensePictureBox_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://www.gnu.org/licenses/",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void RealmStudioSupportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "mailto://support@brookmonte.com",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void ReleaseNotesLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string startupPath = Application.StartupPath;
            string releaseNotesPath = Path.GetFullPath(Path.Combine(startupPath, @"..\..\..\", "Resources", "ReleaseNotes.txt"));

            var attributes = File.GetAttributes(releaseNotesPath);

            File.SetAttributes(releaseNotesPath, attributes | FileAttributes.ReadOnly);

            var psi = new ProcessStartInfo
            {
                FileName = releaseNotesPath,
                UseShellExecute = true,
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }

        private void HelpFileLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://petenelson372.github.io/RealmStudioDocs/",
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi);
            }
            catch { }
        }
    }
}
