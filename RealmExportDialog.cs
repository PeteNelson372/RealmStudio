/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
    public partial class RealmExportDialog : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        public RealmExportDialog()
        {
            InitializeComponent();
        }

        private void CloseExportButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OKExportButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExportImageRadio_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Export the realm as an image file with the selected format", ExportTypeGroup, new Point(ExportImageRadio.Left, ExportImageRadio.Top + 30), 3000);
        }

        private void UpscaledImageRadio_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Export the realm as a 2X-upscaled image file with the selected format", ExportTypeGroup, new Point(ExportImageRadio.Left, ExportImageRadio.Top + 30), 3000);
        }

        private void MapLayersRadio_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Export the realm as a ZIP file containing a separate image for each map layer", ExportTypeGroup, new Point(ExportImageRadio.Left, ExportImageRadio.Top + 30), 3000);
        }

        private void HeightmapRadio_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Export a grayscale heightmap of the realm in the selected format", ExportTypeGroup, new Point(ExportImageRadio.Left, ExportImageRadio.Top + 30), 3000);
        }

        private void HeightMap3DRadio_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Export a Wavefront OBJ 3D model of the realm", ExportTypeGroup, new Point(ExportImageRadio.Left, ExportImageRadio.Top + 30), 3000);
        }
    }
}
