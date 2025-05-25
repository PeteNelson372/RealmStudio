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
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class MapPathInfo : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private readonly RealmStudioMap Map;
        private readonly MapPath MapPath;
        private readonly SKGLControl RenderControl;

        private int SelectedPathTextureIndex;

        public MapPathInfo(RealmStudioMap map, MapPath mapPath, SKGLControl renderControl)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.PathUIMediator);
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            InitializeComponent();

            Map = map;
            MapPath = mapPath;
            RenderControl = renderControl;

            GuidLabel.Text = MapPath.MapPathGuid.ToString();
            NameTextbox.Text = MapPath.MapPathName;

            PathWidthTrack.Value = (int)MapPath.PathWidth;
            PathColorSelectButton.BackColor = MapPath.PathColor;
            DrawOverSymbolsSwitch.Checked = MapPath.DrawOverSymbols;

            PathTexturePreviewPicture.Image = MapPath.PathTexture?.TextureBitmap;

            PathTextureNameLabel.Text = MapPath.PathTexture?.TextureName;

            for (int i = 0; i < LandformManager.LandformMediator.LandTextureList.Count; i++)
            {
                if (MapStateMediator.PathUIMediator.PathTextureList[i].TexturePath == MapPath.PathTexture?.TexturePath)
                {
                    SelectedPathTextureIndex = i;
                    break;
                }
            }

            SetSelectedPathType(MapPath.PathType);
        }

        private void CloseRiverFeatureDataButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PathWidthTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(PathWidthTrack.Value.ToString(), this, new Point(PathWidthTrack.Right - 30, PathWidthTrack.Top - 20), 2000);
        }

        private void PathColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, PathColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                PathColorSelectButton.BackColor = selectedColor;
            }
        }

        private void PreviousPathTextureButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.PathUIMediator);

            if (SelectedPathTextureIndex > 0)
            {
                SelectedPathTextureIndex--;
            }

            if (MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureBitmap == null)
            {
                MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TexturePath);
            }

            PathTexturePreviewPicture.Image = MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureBitmap;
            PathTextureNameLabel.Text = MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureName;
        }

        private void NextPathTextureButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.PathUIMediator);

            if (SelectedPathTextureIndex < MapStateMediator.PathUIMediator.PathTextureList.Count - 1)
            {
                SelectedPathTextureIndex++;
            }

            if (MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureBitmap == null)
            {
                MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TexturePath);
            }

            PathTexturePreviewPicture.Image = MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureBitmap;
            PathTextureNameLabel.Text = MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex].TextureName;
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.PathUIMediator);

            // TODO: does this dialog need to have a UI mediator or somehow hook into the main form PathMediator?
            // there is a lot of redundant code here

            MapPath.MapPathName = NameTextbox.Text;
            MapPath.PathColor = PathColorSelectButton.BackColor;
            MapPath.DrawOverSymbols = DrawOverSymbolsSwitch.Checked;
            MapPath.PathWidth = PathWidthTrack.Value;
            MapPath.PathType = GetSelectedPathType();

            MapPath.PathTexture = MapStateMediator.PathUIMediator.PathTextureList[SelectedPathTextureIndex];

            MapPath.PathPaint?.Dispose();
            MapPath.PathPaint = null;

            PathManager.ConstructPathPaint(MapPath);

            MapPath.BoundaryPath = PathManager.GenerateMapPathBoundaryPath(MapPath.PathPoints);

            for (int i = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents[i] is MapPath mp
                    && mp.MapPathGuid.ToString() == MapPath.MapPathGuid.ToString())
                {
                    MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            for (int i = MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Count - 1; i >= 0; i--)
            {
                if (MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents[i] is MapPath mp
                    && mp.MapPathGuid.ToString() == MapPath.MapPathGuid.ToString())
                {
                    MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.RemoveAt(i);
                }
            }

            if (MapPath.DrawOverSymbols)
            {
                MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHUPPERLAYER).MapLayerComponents.Add(MapPath);
            }
            else
            {
                MapBuilder.GetMapLayerByIndex(Map, MapBuilder.PATHLOWERLAYER).MapLayerComponents.Add(MapPath);
            }

            TOOLTIP.Show("Map path data changes applied", this, new Point(StatusMessageLabel.Left, StatusMessageLabel.Top), 3000);

            RenderControl.Invalidate();
        }

        private PathType GetSelectedPathType()
        {
            if (SolidLineRadio.Checked) return PathType.SolidLinePath;
            if (DottedLineRadio.Checked) return PathType.DottedLinePath;
            if (DashedLineRadio.Checked) return PathType.DashedLinePath;
            if (DashDotLineRadio.Checked) return PathType.DashDotLinePath;
            if (DashDotDotLineRadio.Checked) return PathType.DashDotDotLinePath;
            if (ChevronLineRadio.Checked) return PathType.ChevronLinePath;
            if (LineAndDashesRadio.Checked) return PathType.LineAndDashesPath;
            if (SmallDashesRadio.Checked) return PathType.ShortIrregularDashPath;
            if (ThickLineRadio.Checked) return PathType.ThickSolidLinePath;
            if (BlackBorderPathRadio.Checked) return PathType.SolidBlackBorderPath;
            if (BorderedGradientRadio.Checked) return PathType.BorderedGradientPath;
            if (BorderedLightSolidRadio.Checked) return PathType.BorderedLightSolidPath;
            if (DoubleSolidBorderRadio.Checked) return PathType.DoubleSolidBorderPath;
            if (BearTracksRadio.Checked) return PathType.BearTracksPath;
            if (BirdTracksRadio.Checked) return PathType.BirdTracksPath;
            if (FootPrintsRadio.Checked) return PathType.FootprintsPath;
            if (RailroadTracksRadio.Checked) return PathType.RailroadTracksPath;
            if (TexturePathRadio.Checked) return PathType.TexturedPath;
            if (BorderTexturePathRadio.Checked) return PathType.BorderAndTexturePath;

            return PathType.SolidLinePath;
        }

        private void SetSelectedPathType(PathType pathType)
        {
            if (pathType == PathType.SolidLinePath) { SolidLineRadio.Checked = true; return; }
            if (pathType == PathType.DottedLinePath) { DottedLineRadio.Checked = true; return; }
            if (pathType == PathType.DashedLinePath) { DashedLineRadio.Checked = true; return; }
            if (pathType == PathType.DashDotLinePath) { DashDotLineRadio.Checked = true; return; }
            if (pathType == PathType.DashDotDotLinePath) { DashDotDotLineRadio.Checked = true; return; }
            if (pathType == PathType.ChevronLinePath) { ChevronLineRadio.Checked = true; return; }
            if (pathType == PathType.LineAndDashesPath) { LineAndDashesRadio.Checked = true; return; }
            if (pathType == PathType.ShortIrregularDashPath) { SmallDashesRadio.Checked = true; return; }
            if (pathType == PathType.ThickSolidLinePath) { ThickLineRadio.Checked = true; return; }
            if (pathType == PathType.SolidBlackBorderPath) { BlackBorderPathRadio.Checked = true; return; }
            if (pathType == PathType.BorderedGradientPath) { BorderedGradientRadio.Checked = true; return; }
            if (pathType == PathType.BorderedLightSolidPath) { BorderedLightSolidRadio.Checked = true; return; }
            if (pathType == PathType.DoubleSolidBorderPath) { DoubleSolidBorderRadio.Checked = true; return; }
            if (pathType == PathType.BearTracksPath) { BearTracksRadio.Checked = true; return; }
            if (pathType == PathType.BirdTracksPath) { BirdTracksRadio.Checked = true; return; }
            if (pathType == PathType.FootprintsPath) { FootPrintsRadio.Checked = true; return; }
            if (pathType == PathType.RailroadTracksPath) { RailroadTracksRadio.Checked = true; return; }
            if (pathType == PathType.TexturedPath) { TexturePathRadio.Checked = true; return; }
            if (pathType == PathType.BorderAndTexturePath) { BorderTexturePathRadio.Checked = true; return; }
        }

        private void SolidLinePictureBox_Click(object sender, EventArgs e)
        {
            SolidLineRadio.Checked = !SolidLineRadio.Checked;
        }

        private void DottedLinePictureBox_Click(object sender, EventArgs e)
        {
            DottedLineRadio.Checked = !DottedLineRadio.Checked;
        }

        private void DashedLinePictureBox_Click(object sender, EventArgs e)
        {
            DashedLineRadio.Checked = !DashedLineRadio.Checked;
        }

        private void DashDotPictureBox_Click(object sender, EventArgs e)
        {
            DashDotLineRadio.Checked = !DashDotLineRadio.Checked;
        }

        private void DashDotDotPictureBox_Click(object sender, EventArgs e)
        {
            DashDotDotLineRadio.Checked = !DashDotDotLineRadio.Checked;
        }

        private void DoubleSolidBorderPictureBox_Click(object sender, EventArgs e)
        {
            DoubleSolidBorderRadio.Checked = !DoubleSolidBorderRadio.Checked;
        }

        private void ChevronPictureBox_Click(object sender, EventArgs e)
        {
            ChevronLineRadio.Checked = !ChevronLineRadio.Checked;
        }

        private void LineDashPictureBox_Click(object sender, EventArgs e)
        {
            LineAndDashesRadio.Checked = !LineAndDashesRadio.Checked;
        }

        private void SmallDashesPictureBox_Click(object sender, EventArgs e)
        {
            SmallDashesRadio.Checked = !SmallDashesRadio.Checked;
        }

        private void ThickLinePictureBox_Click(object sender, EventArgs e)
        {
            ThickLineRadio.Checked = !ThickLineRadio.Checked;
        }

        private void BlackBorderLinePictureBox_Click(object sender, EventArgs e)
        {
            BlackBorderPathRadio.Checked = !BlackBorderPathRadio.Checked;
        }

        private void BorderedGradientPictureBox_Click(object sender, EventArgs e)
        {
            BorderedGradientRadio.Checked = !BorderedGradientRadio.Checked;
        }
        private void BorderedLightSolidPictureBox_Click(object sender, EventArgs e)
        {
            BorderedLightSolidRadio.Checked = !BorderedLightSolidRadio.Checked;
        }

        private void BearTracksPictureBox_Click(object sender, EventArgs e)
        {
            BearTracksRadio.Checked = !BearTracksRadio.Checked;
        }

        private void BirdTracksPictureBox_Click(object sender, EventArgs e)
        {
            BirdTracksRadio.Checked = !BirdTracksRadio.Checked;
        }

        private void FootPrintsPictureBox_Click(object sender, EventArgs e)
        {
            FootPrintsRadio.Checked = !FootPrintsRadio.Checked;
        }

        private void RailroadTracksPictureBox_Click(object sender, EventArgs e)
        {
            RailroadTracksRadio.Checked = !RailroadTracksRadio.Checked;
        }

        private void PathDescriptionButton_Click(object sender, EventArgs e)
        {
            DescriptionEditor descriptionEditor = new();
            descriptionEditor.DescrptionEditorOverlay.Text = "Path Description Editor";
            descriptionEditor.DescriptionText = MapPath.MapPathDescription ?? string.Empty;
            DialogResult r = descriptionEditor.ShowDialog(this);

            if (r == DialogResult.OK)
            {
                MapPath.MapPathDescription = descriptionEditor.DescriptionText;
            }
        }

        private void PathDescriptionButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Edit Path Description", this, new Point(PathDescriptionButton.Left, PathDescriptionButton.Top - 20), 3000);
        }
    }
}
