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
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace RealmStudio
{
    public partial class LandformInfo : Form
    {
        private static readonly ToolTip TOOLTIP = new();

        private readonly RealmStudioMap Map;
        private readonly Landform Landform;
        private readonly SKGLControl RenderControl;
        private readonly MapTheme? CurrentTheme;
        private bool NameLocked;

        private int SelectedLandTextureIndex;

        public LandformInfo(RealmStudioMap map, Landform mapLandform, MapTheme? currentTheme, SKGLControl renderControl)
        {
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            InitializeComponent();

            Map = map;
            Landform = mapLandform;
            RenderControl = renderControl;
            CurrentTheme = currentTheme;

            GuidLabel.Text = Landform.LandformGuid.ToString();
            NameTextbox.Text = Landform.LandformName;

            LandformOutlineColorSelectButton.BackColor = Landform.LandformOutlineColor;
            LandformOutlineWidthTrack.Value = Landform.LandformOutlineWidth;
            LandformBackgroundColorSelectButton.BackColor = Landform.LandformBackgroundColor;
            UseTextureForBackgroundCheck.Checked = Landform.FillWithTexture;

            if (Landform.LandformTexture != null)
            {
                if (Landform.LandformTexture.TextureBitmap == null)
                {
                    Landform.LandformTexture.TextureBitmap = (Bitmap?)Bitmap.FromFile(LandformManager.LandformMediator.LandTextureList.First().TexturePath);
                }

                LandformTexturePreviewPicture.Image = Landform.LandformTexture.TextureBitmap;
                LandTextureNameLabel.Text = Landform.LandformTexture.TextureName;

                for (int i = 0; i < LandformManager.LandformMediator.LandTextureList.Count; i++)
                {
                    if (LandformManager.LandformMediator.LandTextureList[i].TexturePath == Landform.LandformTexture.TexturePath)
                    {
                        SelectedLandTextureIndex = i;
                        break;
                    }
                }
            }

            CoastlineEffectDistanceTrack.Value = Landform.CoastlineEffectDistance;
            CoastlineColorSelectionButton.BackColor = Landform.CoastlineColor;

            for (int i = 0; i < CoastlineStyleList.Items.Count; i++)
            {
                if (CoastlineStyleList.Items[i].ToString() == Landform.CoastlineStyleName)
                {
                    CoastlineStyleList.SetSelected(i, true);
                    break;
                }
            }

            Refresh();
        }

        private void PreviousTextureButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            if (SelectedLandTextureIndex > 0)
            {
                SelectedLandTextureIndex--;
            }

            if (LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureBitmap == null)
            {
                LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TexturePath);
            }

            LandformTexturePreviewPicture.Image = LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureBitmap;
            LandTextureNameLabel.Text = LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureName;
        }

        private void NextTextureButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            if (SelectedLandTextureIndex < LandformManager.LandformMediator.LandTextureList.Count - 1)
            {
                SelectedLandTextureIndex++;
            }

            if (LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureBitmap == null)
            {
                LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TexturePath);
            }

            LandformTexturePreviewPicture.Image = LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureBitmap;
            LandTextureNameLabel.Text = LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex].TextureName;
        }

        private void CloseLandformDataButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            Landform.LandformName = NameTextbox.Text;
            Landform.LandformOutlineColor = LandformOutlineColorSelectButton.BackColor;
            Landform.LandformFillColor = Color.FromArgb(Landform.LandformOutlineColor.A / 4, Landform.LandformOutlineColor);
            Landform.LandformTexture = LandformManager.LandformMediator.LandTextureList[SelectedLandTextureIndex];
            Landform.LandformOutlineWidth = LandformOutlineWidthTrack.Value;
            Landform.FillWithTexture = UseTextureForBackgroundCheck.Checked;
            Landform.LandformBackgroundColor = LandformBackgroundColorSelectButton.BackColor;

            if (!Landform.FillWithTexture || Landform.LandformTexture == null || Landform.LandformTexture.TextureBitmap == null)
            {
                // fill with background color
                Landform.LandformFillPaint.Shader?.Dispose();
                Landform.LandformFillPaint.Shader = null;

                SKShader flpShader = SKShader.CreateColor(Landform.LandformBackgroundColor.ToSKColor());
                Landform.LandformFillPaint.Shader = flpShader;
            }
            else
            {
                // fill with texture
                Bitmap resizedBitmap = new(Landform.LandformTexture.TextureBitmap, Map.MapWidth, Map.MapHeight);

                // create and set a shader from the texture
                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                Landform.LandformFillPaint.Shader = flpShader;
            }


            MapTexture? dashTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Watercolor Dashes");

            if (dashTexture != null)
            {
                dashTexture.TextureBitmap ??= new Bitmap(dashTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(dashTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                Landform.DashShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            MapTexture? lineHatchTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Line Hatch");

            if (lineHatchTexture != null)
            {
                lineHatchTexture.TextureBitmap ??= new Bitmap(lineHatchTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(lineHatchTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKSamplingOptions.Default);

                Landform.LineHatchBitmapShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }


            Landform.CoastlineEffectDistance = CoastlineEffectDistanceTrack.Value;
            Landform.CoastlineColor = CoastlineColorSelectionButton.BackColor;

            string? coastlineStyleName = CoastlineStyleList.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(coastlineStyleName))
            {
                Landform.CoastlineStyleName = coastlineStyleName;
            }

            TOOLTIP.Show("Landform data changes applied", this, new Point(StatusMessageLabel.Left, StatusMessageLabel.Top), 3000);

            Landform.IsModified = true;

            RenderControl.Invalidate();
        }

        private void LandformOutlineColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, LandformOutlineColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                LandformOutlineColorSelectButton.BackColor = selectedColor;
                LandformOutlineColorSelectButton.Refresh();
            }
        }

        private void LandformBackgroundColorSelectButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, LandformOutlineColorSelectButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                LandformBackgroundColorSelectButton.BackColor = selectedColor;
                LandformBackgroundColorSelectButton.Refresh();
            }
        }

        private void CoastlineColorSelectionButton_Click(object sender, EventArgs e)
        {
            Color selectedColor = UtilityMethods.SelectColorFromDialog(this, CoastlineColorSelectionButton.BackColor);

            if (selectedColor != Color.Empty)
            {
                CoastlineColorSelectionButton.BackColor = selectedColor;
                CoastlineColorSelectionButton.Refresh();
            }
        }

        private void CoastlineEffectDistanceTrack_Scroll(object sender, EventArgs e)
        {
            TOOLTIP.Show(CoastlineEffectDistanceTrack.Value.ToString(), CoastlineGroup, new Point(CoastlineEffectDistanceTrack.Right - 30, CoastlineEffectDistanceTrack.Top - 20), 2000);
        }

        private void ApplyThemeSettingsButton_Click(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            if (CurrentTheme != null)
            {
                LandformOutlineColorSelectButton.BackColor = (CurrentTheme.LandformOutlineColor != null) ?
                    Color.FromArgb((int)CurrentTheme.LandformOutlineColor) : Landform.LandformOutlineColor;

                LandformOutlineWidthTrack.Value = (int)((CurrentTheme.LandformOutlineWidth != null) ?
                    CurrentTheme.LandformOutlineWidth : Landform.LandformOutlineWidth);

                LandformBackgroundColorSelectButton.BackColor = (CurrentTheme.LandformBackgroundColor != null) ?
                    Color.FromArgb((int)CurrentTheme.LandformBackgroundColor) : Landform.LandformBackgroundColor;

                UseTextureForBackgroundCheck.Checked = (bool)((CurrentTheme.FillLandformWithTexture != null) ?
                    CurrentTheme.FillLandformWithTexture : Landform.FillWithTexture);

                if (CurrentTheme.LandformTexture != null)
                {
                    if (CurrentTheme.LandformTexture.TextureBitmap == null)
                    {
                        CurrentTheme.LandformTexture.TextureBitmap = (Bitmap?)Bitmap.FromFile(CurrentTheme.LandformTexture.TexturePath);
                    }

                    LandformTexturePreviewPicture.Image = CurrentTheme.LandformTexture.TextureBitmap;
                    LandTextureNameLabel.Text = CurrentTheme.LandformTexture.TextureName;

                    for (int i = 0; i < LandformManager.LandformMediator.LandTextureList.Count; i++)
                    {
                        if (LandformManager.LandformMediator.LandTextureList[i].TexturePath == CurrentTheme.LandformTexture.TexturePath)
                        {
                            SelectedLandTextureIndex = i;
                            break;
                        }
                    }
                }

                CoastlineEffectDistanceTrack.Value = (CurrentTheme.LandformCoastlineEffectDistance != null) ?
                    CurrentTheme.LandformCoastlineEffectDistance.Value : Landform.CoastlineEffectDistance;

                CoastlineColorSelectionButton.BackColor = (CurrentTheme.LandformCoastlineColor != null) ?
                    Color.FromArgb((int)CurrentTheme.LandformCoastlineColor) : Landform.CoastlineColor;

                if (!string.IsNullOrEmpty(CurrentTheme.LandformCoastlineStyle))
                {
                    Landform.CoastlineStyleName = CurrentTheme.LandformCoastlineStyle;
                }

                for (int i = 0; i < CoastlineStyleList.Items.Count; i++)
                {
                    if (CoastlineStyleList.Items[i].ToString() == Landform.CoastlineStyleName)
                    {
                        CoastlineStyleList.SetSelected(i, true);
                        break;
                    }
                }

                Landform.IsModified = true;
            }
        }

        private void LandformDescriptionButton_Click(object sender, EventArgs e)
        {
            DescriptionEditor descriptionEditor = new(Landform, Landform.LandformDescription);
            descriptionEditor.DescriptionEditorOverlay.Text = "Landform Description Editor";

            DialogResult r = descriptionEditor.ShowDialog(this);

            if (r == DialogResult.OK)
            {
                Landform.LandformDescription = descriptionEditor.DescriptionText;
            }
        }

        private void LandformDescriptionButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Edit Landform Description", this, new Point(LandformDescriptionButton.Left, LandformDescriptionButton.Top - 20), 3000);
        }

        private void GenerateLandformNameButton_Click(object sender, EventArgs e)
        {
            if (NameLocked)
            {
                return; // Do not generate a new name if the name is locked
            }

            List<INameGenerator> generators = RealmStudioMainForm.NAME_GENERATOR_CONFIG.GetSelectedNameGenerators();
            string generatedName = MapToolMethods.GenerateRandomPlaceName(generators);
            NameTextbox.Text = generatedName;
            Landform.LandformName = generatedName;
        }

        private void GenerateLandformNameButton_MouseHover(object sender, EventArgs e)
        {
            TOOLTIP.Show("Generate Landform Name", this, new Point(GenerateLandformNameButton.Left, GenerateLandformNameButton.Top - 20), 3000);
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
    }
}
