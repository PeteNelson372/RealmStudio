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

        private int SelectedLandTextureIndex = 0;

        public LandformInfo(RealmStudioMap map, Landform mapLandform, SKGLControl renderControl)
        {
            InitializeComponent();

            Map = map;
            Landform = mapLandform;
            RenderControl = renderControl;

            GuidLabel.Text = Landform.LandformGuid.ToString();
            NameTextbox.Text = Landform.LandformName;

            LandformOutlineColorSelectButton.BackColor = Landform.LandformOutlineColor;

            if (Landform.LandformTexture != null)
            {
                if (Landform.LandformTexture.TextureBitmap == null)
                {
                    Landform.LandformTexture.TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST.First().TexturePath);
                }

                LandformTexturePreviewPicture.Image = Landform.LandformTexture.TextureBitmap;
                LandTextureNameLabel.Text = Landform.LandformTexture.TextureName;

                for (int i = 0; i < AssetManager.LAND_TEXTURE_LIST.Count; i++)
                {
                    if (AssetManager.LAND_TEXTURE_LIST[i].TexturePath == Landform.LandformTexture.TexturePath)
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
        }

        private void PreviousTextureButton_Click(object sender, EventArgs e)
        {
            if (SelectedLandTextureIndex > 0)
            {
                SelectedLandTextureIndex--;
            }

            if (AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TexturePath);
            }

            LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureBitmap;
            LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureName;
        }

        private void NextTextureButton_Click(object sender, EventArgs e)
        {
            if (SelectedLandTextureIndex < AssetManager.LAND_TEXTURE_LIST.Count - 1)
            {
                SelectedLandTextureIndex++;
            }

            if (AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureBitmap == null)
            {
                AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TexturePath);
            }

            LandformTexturePreviewPicture.Image = AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureBitmap;
            LandTextureNameLabel.Text = AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex].TextureName;
        }

        private void CloseLandformDataButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            Landform.LandformName = NameTextbox.Text;
            Landform.LandformOutlineColor = LandformOutlineColorSelectButton.BackColor;
            Landform.LandformFillColor = Color.FromArgb(Landform.LandformOutlineColor.A / 4, Landform.LandformOutlineColor);
            Landform.LandformTexture = AssetManager.LAND_TEXTURE_LIST[SelectedLandTextureIndex];

            if (Landform.LandformTexture.TextureBitmap != null)
            {
                Bitmap resizedBitmap = new(Landform.LandformTexture.TextureBitmap, Map.MapWidth, Map.MapHeight);

                // create and set a shader from the selected texture
                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                Landform.LandformFillPaint.Shader = flpShader;
            }

            MapTexture? dashTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Watercolor Dashes");

            if (dashTexture != null)
            {
                dashTexture.TextureBitmap ??= new Bitmap(dashTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(dashTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKFilterQuality.High);

                Landform.DashShader = SKShader.CreateBitmap(resizedSKBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
            }

            MapTexture? lineHatchTexture = AssetManager.HATCH_TEXTURE_LIST.Find(x => x.TextureName == "Line Hatch");

            if (lineHatchTexture != null)
            {
                lineHatchTexture.TextureBitmap ??= new Bitmap(lineHatchTexture.TexturePath);

                SKBitmap resizedSKBitmap = new(100, 100);

                Extensions.ToSKBitmap(lineHatchTexture.TextureBitmap).ScalePixels(resizedSKBitmap, SKFilterQuality.High);

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

            MapBuilder.SetLayerModified(Landform.ParentMap, MapBuilder.LANDCOASTLINELAYER, true);
            MapBuilder.SetLayerModified(Landform.ParentMap, MapBuilder.LANDFORMLAYER, true);

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
            TOOLTIP.Show(CoastlineEffectDistanceTrack.Value.ToString(), CoastlineEffectDistanceTrack, new Point(CoastlineEffectDistanceTrack.Right - 42, CoastlineEffectDistanceTrack.Top - 62), 2000);
        }
    }
}
