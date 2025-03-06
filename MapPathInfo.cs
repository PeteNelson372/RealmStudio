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

            for (int i = 0; i < AssetManager.LAND_TEXTURE_LIST.Count; i++)
            {
                if (AssetManager.PATH_TEXTURE_LIST[i].TexturePath == MapPath.PathTexture?.TexturePath)
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
            if (SelectedPathTextureIndex > 0)
            {
                SelectedPathTextureIndex--;
            }

            if (AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureBitmap == null)
            {
                AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TexturePath);
            }

            PathTexturePreviewPicture.Image = AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureBitmap;
            PathTextureNameLabel.Text = AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureName;
        }

        private void NextPathTextureButton_Click(object sender, EventArgs e)
        {
            if (SelectedPathTextureIndex < AssetManager.PATH_TEXTURE_LIST.Count - 1)
            {
                SelectedPathTextureIndex++;
            }

            if (AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureBitmap == null)
            {
                AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TexturePath);
            }

            PathTexturePreviewPicture.Image = AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureBitmap;
            PathTextureNameLabel.Text = AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex].TextureName;
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            MapPath.MapPathName = NameTextbox.Text;
            MapPath.PathColor = PathColorSelectButton.BackColor;
            MapPath.DrawOverSymbols = DrawOverSymbolsSwitch.Checked;
            MapPath.PathWidth = PathWidthTrack.Value;
            MapPath.PathType = GetSelectedPathType();

            MapPath.PathTexture = AssetManager.PATH_TEXTURE_LIST[SelectedPathTextureIndex];

            MapPath.PathPaint?.Dispose();
            MapPath.PathPaint = null;

            MapPathMethods.ConstructPathPaint(MapPath);

            MapPath.BoundaryPath = MapPathMethods.GenerateMapPathBoundaryPath(MapPath.PathPoints);

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

        private PathTypeEnum GetSelectedPathType()
        {
            if (SolidLineRadio.Checked) return PathTypeEnum.SolidLinePath;
            if (DottedLineRadio.Checked) return PathTypeEnum.DottedLinePath;
            if (DashedLineRadio.Checked) return PathTypeEnum.DashedLinePath;
            if (DashDotLineRadio.Checked) return PathTypeEnum.DashDotLinePath;
            if (DashDotDotLineRadio.Checked) return PathTypeEnum.DashDotDotLinePath;
            if (ChevronLineRadio.Checked) return PathTypeEnum.ChevronLinePath;
            if (LineAndDashesRadio.Checked) return PathTypeEnum.LineAndDashesPath;
            if (SmallDashesRadio.Checked) return PathTypeEnum.ShortIrregularDashPath;
            if (ThickLineRadio.Checked) return PathTypeEnum.ThickSolidLinePath;
            if (BlackBorderPathRadio.Checked) return PathTypeEnum.SolidBlackBorderPath;
            if (BorderedGradientRadio.Checked) return PathTypeEnum.BorderedGradientPath;
            if (BorderedLightSolidRadio.Checked) return PathTypeEnum.BorderedLightSolidPath;
            if (DoubleSolidBorderRadio.Checked) return PathTypeEnum.DoubleSolidBorderPath;
            if (BearTracksRadio.Checked) return PathTypeEnum.BearTracksPath;
            if (BirdTracksRadio.Checked) return PathTypeEnum.BirdTracksPath;
            if (FootPrintsRadio.Checked) return PathTypeEnum.FootprintsPath;
            if (RailroadTracksRadio.Checked) return PathTypeEnum.RailroadTracksPath;
            if (TexturePathRadio.Checked) return PathTypeEnum.TexturedPath;
            if (BorderTexturePathRadio.Checked) return PathTypeEnum.BorderAndTexturePath;

            return PathTypeEnum.SolidLinePath;
        }

        private void SetSelectedPathType(PathTypeEnum pathType)
        {
            if (pathType == PathTypeEnum.SolidLinePath) { SolidLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.DottedLinePath) { DottedLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.DashedLinePath) { DashedLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.DashDotLinePath) { DashDotLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.DashDotDotLinePath) { DashDotDotLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.ChevronLinePath) { ChevronLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.LineAndDashesPath) { LineAndDashesRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.ShortIrregularDashPath) { SmallDashesRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.ThickSolidLinePath) { ThickLineRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.SolidBlackBorderPath) { BlackBorderPathRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.BorderedGradientPath) { BorderedGradientRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.BorderedLightSolidPath) { BorderedLightSolidRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.DoubleSolidBorderPath) { DoubleSolidBorderRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.BearTracksPath) { BearTracksRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.BirdTracksPath) { BirdTracksRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.FootprintsPath) { FootPrintsRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.RailroadTracksPath) { RailroadTracksRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.TexturedPath) { TexturePathRadio.Checked = true; return; }
            if (pathType == PathTypeEnum.BorderAndTexturePath) { BorderTexturePathRadio.Checked = true; return; }
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
    }
}
