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
using System.Diagnostics;

namespace RealmStudio
{
    internal sealed class ThemeManager
    {
        internal static MapTheme SaveCurentSettingsToTheme()
        {
            ArgumentNullException.ThrowIfNull(BackgroundManager.BackgroundMediator);
            ArgumentNullException.ThrowIfNull(VignetteManager.VignetteMediator);
            ArgumentNullException.ThrowIfNull(OceanManager.OceanMediator);
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);
            ArgumentNullException.ThrowIfNull(WaterFeatureManager.WaterFeatureMediator);
            ArgumentNullException.ThrowIfNull(PathManager.PathMediator);
            ArgumentNullException.ThrowIfNull(SymbolManager.SymbolMediator);
            ArgumentNullException.ThrowIfNull(LabelManager.LabelMediator);

            // TODO: implement theme mediator and manager

            MapTheme theme = new()
            {
                // label presets for the theme are serialized to a file at the time they are created
                // and loaded when the theme is loaded/selected; they are not stored with the theme

                // background
                BackgroundTexture = BackgroundManager.BackgroundMediator.BackgroundTextureList[BackgroundManager.BackgroundMediator.BackgroundTextureIndex],
                BackgroundTextureScale = BackgroundManager.BackgroundMediator.BackgroundTextureScale,
                MirrorBackgroundTexture = BackgroundManager.BackgroundMediator.MirrorBackgroundTexture,

                // vignette color and strength
                VignetteColor = VignetteManager.VignetteMediator.VignetteColor.ToArgb(),
                VignetteStrength = VignetteManager.VignetteMediator.VignetteStrength,
                VignetteShape = VignetteManager.VignetteMediator.VignetteShape,

                // ocean
                OceanTexture = OceanManager.OceanMediator.OceanTextureList[OceanManager.OceanMediator.OceanTextureIndex],

                OceanTextureOpacity = (int?)OceanManager.OceanMediator.OceanTextureOpacity,
                OceanTextureScale = OceanManager.OceanMediator.OceanTextureScale,
                MirrorOceanTexture = OceanManager.OceanMediator.MirrorOceanTexture,
                OceanColor = OceanManager.OceanMediator.OceanFillColor.ToArgb()
            };

            // save ocean custom colors
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor1.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor2.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor3.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor4.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor5.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor6.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor7.ToArgb());
            theme.OceanColorPalette?.Add(OceanManager.OceanMediator.CustomColor8.ToArgb());

            // landform
            theme.LandformOutlineColor = LandformManager.LandformMediator.LandOutlineColor.ToArgb();
            theme.LandformOutlineWidth = LandformManager.LandformMediator.LandOutlineWidth;

            theme.LandformBackgroundColor = LandformManager.LandformMediator.LandBackgroundColor.ToArgb();
            theme.FillLandformWithTexture = LandformManager.LandformMediator.UseTextureBackground;

            theme.LandformTexture = LandformManager.LandformMediator.LandTextureList[LandformManager.LandformMediator.LandformTextureIndex];

            theme.LandShorelineStyle = LandGradientDirection.None.ToString();   // light-to-dark shading vs. dark-to-light shading; not used now

            theme.LandformCoastlineColor = LandformManager.LandformMediator.CoastlineColor.ToArgb();
            theme.LandformCoastlineEffectDistance = LandformManager.LandformMediator.CoastlineEffectDistance;

            theme.LandformCoastlineStyle = LandformManager.LandformMediator.CoastlineStyle;


            // save land custom colors
            theme.LandformColorPalette?.Add(LandformManager.LandformMediator.CustomColor1.ToArgb());
            theme.LandformColorPalette?.Add(LandformManager.LandformMediator.CustomColor2.ToArgb());
            theme.LandformColorPalette?.Add(LandformManager.LandformMediator.CustomColor3.ToArgb());
            theme.LandformColorPalette?.Add(LandformManager.LandformMediator.CustomColor4.ToArgb());
            theme.LandformColorPalette?.Add(LandformManager.LandformMediator.CustomColor5.ToArgb());
            theme.LandformColorPalette?.Add(LandformManager.LandformMediator.CustomColor6.ToArgb());

            // freshwater
            theme.FreshwaterColor = WaterFeatureManager.WaterFeatureMediator.WaterColor.ToArgb();
            theme.FreshwaterShorelineColor = WaterFeatureManager.WaterFeatureMediator.ShorelineColor.ToArgb();

            theme.RiverWidth = WaterFeatureManager.WaterFeatureMediator.RiverWidth;
            theme.RiverSourceFadeIn = WaterFeatureManager.WaterFeatureMediator.RiverSourceFadeIn;

            // save freshwater custom colors
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor1.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor2.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor3.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor4.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor5.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor6.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor7.ToArgb());
            theme.FreshwaterColorPalette?.Add(WaterFeatureManager.WaterFeatureMediator.CustomColor8.ToArgb());

            // path
            theme.PathColor = PathManager.PathMediator.PathColor.ToArgb();
            theme.PathWidth = PathManager.PathMediator.PathWidth;
            theme.PathStyle = PathManager.PathMediator.PathType;

            // label
            FontConverter cvt = new();
            string? fontString = cvt.ConvertToString(LabelManager.LabelMediator.SelectedLabelFont);
            theme.LabelFont = fontString ?? string.Empty;
            theme.LabelColor = LabelManager.LabelMediator.LabelColor.ToArgb();
            theme.LabelOutlineColor = LabelManager.LabelMediator.OutlineColor.ToArgb();
            theme.LabelOutlineWidth = LabelManager.LabelMediator.OutlineWidth;
            theme.LabelGlowColor = LabelManager.LabelMediator.GlowColor.ToArgb();
            theme.LabelGlowStrength = (int?)LabelManager.LabelMediator.GlowStrength;

            // symbols
            if (theme.SymbolCustomColors != null)
            {
                theme.SymbolCustomColors[0] = SymbolManager.SymbolMediator.SymbolColor1.ToArgb();
                theme.SymbolCustomColors[1] = SymbolManager.SymbolMediator.SymbolColor2.ToArgb();
                theme.SymbolCustomColors[2] = SymbolManager.SymbolMediator.SymbolColor3.ToArgb();
            }

            return theme;
        }

        internal static void ApplyTheme(MapTheme theme, ThemeFilter themeFilter)
        {
            if (theme == null || themeFilter == null) return;

            ArgumentNullException.ThrowIfNull(BackgroundManager.BackgroundMediator);
            ArgumentNullException.ThrowIfNull(VignetteManager.VignetteMediator);
            ArgumentNullException.ThrowIfNull(OceanManager.OceanMediator);
            ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);
            ArgumentNullException.ThrowIfNull(WaterFeatureManager.WaterFeatureMediator);
            ArgumentNullException.ThrowIfNull(PathManager.PathMediator);
            ArgumentNullException.ThrowIfNull(SymbolManager.SymbolMediator);
            ArgumentNullException.ThrowIfNull(LabelManager.LabelMediator);


            try
            {
                if (themeFilter.ApplyBackgroundSettings)
                {
                    int backgroundTextureIndex = -1;

                    if (theme.BackgroundTexture != null)
                    {
                        MapLayer baseLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BASELAYER);

                        if (baseLayer.MapLayerComponents.Count == 0)
                        {
                            if (theme.BackgroundTexture != null)
                            {
                                for (int i = 0; i < BackgroundManager.BackgroundMediator.BackgroundTextureList.Count; i++)
                                {
                                    if (BackgroundManager.BackgroundMediator.BackgroundTextureList[i] != null
                                        && BackgroundManager.BackgroundMediator.BackgroundTextureList[i].TexturePath == theme.BackgroundTexture.TexturePath)
                                    {
                                        backgroundTextureIndex = i;
                                        break;
                                    }
                                }
                            }
                        }

                        if (backgroundTextureIndex >= 0)
                        {
                            // fix the theme background texture scale if needed, and serialize the theme
                            if (theme.BackgroundTextureScale < 0.0F)
                            {
                                theme.BackgroundTextureScale = 0.0F;
                                MapFileMethods.SerializeTheme(theme);
                            }

                            if (theme.BackgroundTextureScale > 1.0F)
                            {
                                theme.BackgroundTextureScale = theme.BackgroundTextureScale / 100.0F;
                                MapFileMethods.SerializeTheme(theme);
                            }

                            BackgroundManager.BackgroundMediator.Initialize(
                                backgroundTextureIndex,
                                (int)((theme.BackgroundTextureScale != null) ? theme.BackgroundTextureScale : 1.0F),
                                (bool)((theme.MirrorBackgroundTexture != null) ? theme.MirrorBackgroundTexture : false));

                            BackgroundManager.FillBackgroundTexture();
                        }
                    }

                    VignetteManager.VignetteMediator.Initialize(
                        (int)((theme.VignetteStrength != null) ? theme.VignetteStrength : 148),
                        Color.FromArgb(theme.VignetteColor ?? Color.FromArgb(201, 151, 123).ToArgb()),
                        (VignetteShapeType)((theme.VignetteShape == null) ? VignetteShapeType.Oval : theme.VignetteShape));

                }

                if (themeFilter.ApplyOceanSettings)
                {
                    MapLayer oceanTextureLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.OCEANTEXTURELAYER);

                    if (oceanTextureLayer.MapLayerComponents.Count == 0)
                    {
                        int oceanTextureIndex = -1;

                        if (theme.OceanTexture != null)
                        {
                            for (int i = 0; i < OceanManager.OceanMediator.OceanTextureList.Count; i++)
                            {
                                if (OceanManager.OceanMediator.OceanTextureList[i].TextureName == theme.OceanTexture.TextureName)
                                {
                                    oceanTextureIndex = i;
                                    break;
                                }
                            }
                        }

                        if (oceanTextureIndex >= 0)
                        {
                            // fix the theme ocean texture scale if needed, and serialize the theme
                            if (theme.OceanTextureScale < 0.0F)
                            {
                                theme.OceanTextureScale = 0.0F;
                                MapFileMethods.SerializeTheme(theme);
                            }

                            if (theme.OceanTextureScale > 1.0F)
                            {
                                theme.OceanTextureScale = theme.OceanTextureScale / 100.0F;
                                MapFileMethods.SerializeTheme(theme);
                            }

                            if (theme.OceanTextureOpacity < 0.0F)
                            {
                                theme.OceanTextureOpacity = 0.0F;
                                MapFileMethods.SerializeTheme(theme);
                            }

                            if (theme.OceanTextureOpacity > 1.0F)
                            {
                                theme.OceanTextureOpacity = theme.OceanTextureOpacity / 100.0F;
                                MapFileMethods.SerializeTheme(theme);
                            }

                            OceanManager.OceanMediator.Initialize(
                                oceanTextureIndex,
                                theme.OceanTexture?.TextureName == null ? string.Empty : theme.OceanTexture.TextureName,
                                (float)(theme.OceanTextureScale == null ? 1.0F : theme.OceanTextureScale),
                                (float)(theme.OceanTextureOpacity == null ? 1.0F : (float)theme.OceanTextureOpacity),
                                (bool)(theme.MirrorOceanTexture == null ? false : theme.MirrorOceanTexture));

                            OceanManager.ApplyOceanTexture();
                        }
                    }
                }

                if (themeFilter.ApplyOceanColorPaletteSettings)
                {
                    if (theme.OceanColorPalette?.Count > 0)
                    {
                        OceanManager.OceanMediator.CustomColor1 = Color.FromArgb(theme.OceanColorPalette[0] ?? Color.White.ToArgb());
                    }

                    if (theme.OceanColorPalette?.Count > 1)
                    {
                        OceanManager.OceanMediator.CustomColor2 = Color.FromArgb(theme.OceanColorPalette[1] ?? Color.White.ToArgb()); ;
                    }

                    if (theme.OceanColorPalette?.Count > 2)
                    {
                        OceanManager.OceanMediator.CustomColor3 = Color.FromArgb(theme.OceanColorPalette[2] ?? Color.White.ToArgb());
                    }

                    if (theme.OceanColorPalette?.Count > 3)
                    {
                        OceanManager.OceanMediator.CustomColor4 = Color.FromArgb(theme.OceanColorPalette[3] ?? Color.White.ToArgb());
                    }

                    if (theme.OceanColorPalette?.Count > 4)
                    {
                        OceanManager.OceanMediator.CustomColor5 = Color.FromArgb(theme.OceanColorPalette[4] ?? Color.White.ToArgb());
                    }

                    if (theme.OceanColorPalette?.Count > 5)
                    {
                        OceanManager.OceanMediator.CustomColor6 = Color.FromArgb(theme.OceanColorPalette[5] ?? Color.White.ToArgb());
                    }

                    if (theme.OceanColorPalette?.Count > 6)
                    {
                        OceanManager.OceanMediator.CustomColor7 = Color.FromArgb(theme.OceanColorPalette[6] ?? Color.White.ToArgb());
                    }

                    if (theme.OceanColorPalette?.Count > 7)
                    {
                        OceanManager.OceanMediator.CustomColor8 = Color.FromArgb(theme.OceanColorPalette[7] ?? Color.White.ToArgb());
                    }
                }

                if (themeFilter.ApplyLandSettings)
                {
                    LandformManager.LandformMediator.LandOutlineColor = Color.FromArgb(theme.LandformOutlineColor ?? Color.FromArgb(62, 55, 40).ToArgb());

                    if (theme.LandformTexture != null)
                    {
                        for (int i = 0; i < LandformManager.LandformMediator.LandTextureList.Count; i++)
                        {
                            if (LandformManager.LandformMediator.LandTextureList[i].TextureName == theme.LandformTexture.TextureName)
                            {
                                LandformManager.LandformMediator.LandformTextureIndex = i;
                                break;
                            }
                        }
                    }

                    LandformManager.LandformMediator.UseTextureBackground = (bool)((theme.FillLandformWithTexture != null) ? theme.FillLandformWithTexture : true);
                    LandformManager.LandformMediator.CoastlineColor = Color.FromArgb(theme.LandformCoastlineColor ?? Color.FromArgb(187, 156, 195, 183).ToArgb());
                    LandformManager.LandformMediator.CoastlineEffectDistance = theme.LandformCoastlineEffectDistance ?? 16;


                    if (theme.LandformCoastlineStyle != null)
                    {
                        LandformManager.LandformMediator.CoastlineStyle = theme.LandformCoastlineStyle;
                    }
                }

                if (themeFilter.ApplyLandformColorPaletteSettings)
                {
                    if (theme.LandformColorPalette?.Count > 0)
                    {
                        LandformManager.LandformMediator.CustomColor1 = Color.FromArgb(theme.LandformColorPalette[0] ?? Color.White.ToArgb());
                    }

                    if (theme.LandformColorPalette?.Count > 1)
                    {
                        LandformManager.LandformMediator.CustomColor2 = Color.FromArgb(theme.LandformColorPalette[1] ?? Color.White.ToArgb());
                    }

                    if (theme.LandformColorPalette?.Count > 2)
                    {
                        LandformManager.LandformMediator.CustomColor3 = Color.FromArgb(theme.LandformColorPalette[2] ?? Color.White.ToArgb());
                    }

                    if (theme.LandformColorPalette?.Count > 3)
                    {
                        LandformManager.LandformMediator.CustomColor4 = Color.FromArgb(theme.LandformColorPalette[3] ?? Color.White.ToArgb());
                    }

                    if (theme.LandformColorPalette?.Count > 4)
                    {
                        LandformManager.LandformMediator.CustomColor5 = Color.FromArgb(theme.LandformColorPalette[4] ?? Color.White.ToArgb());
                    }

                    if (theme.LandformColorPalette?.Count > 5)
                    {
                        LandformManager.LandformMediator.CustomColor6 = Color.FromArgb(theme.LandformColorPalette[5] ?? Color.White.ToArgb());
                    }
                }

                if (themeFilter.ApplyFreshwaterSettings)
                {
                    if (theme.FreshwaterColor != Color.Empty.ToArgb())
                    {
                        WaterFeatureManager.WaterFeatureMediator.WaterColor = Color.FromArgb(theme.FreshwaterColor ?? Color.FromArgb(101, 140, 191, 197).ToArgb());
                    }
                    else
                    {
                        WaterFeatureManager.WaterFeatureMediator.WaterColor = WaterFeatureManager.DEFAULT_WATER_COLOR;
                    }

                    if (theme.FreshwaterShorelineColor != Color.Empty.ToArgb())
                    {
                        WaterFeatureManager.WaterFeatureMediator.ShorelineColor = Color.FromArgb(theme.FreshwaterShorelineColor ?? Color.FromArgb(161, 144, 118).ToArgb());
                    }
                    else
                    {
                        WaterFeatureManager.WaterFeatureMediator.ShorelineColor = WaterFeatureManager.DEFAULT_WATER_OUTLINE_COLOR;
                    }

                    WaterFeatureManager.WaterFeatureMediator.RiverWidth = theme.RiverWidth ?? 4;
                    WaterFeatureManager.WaterFeatureMediator.RiverSourceFadeIn = theme.RiverSourceFadeIn ?? true;
                }
                else
                {
                    WaterFeatureManager.WaterFeatureMediator.ShorelineColor = WaterFeatureManager.DEFAULT_WATER_OUTLINE_COLOR;
                    WaterFeatureManager.WaterFeatureMediator.WaterColor = WaterFeatureManager.DEFAULT_WATER_COLOR;
                }

                if (themeFilter.ApplyFreshwaterColorPaletteSettings)
                {

                    if (theme.FreshwaterColorPalette?.Count > 0)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor1 = Color.FromArgb(theme.FreshwaterColorPalette[0] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 1)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor2 = Color.FromArgb(theme.FreshwaterColorPalette[1] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 2)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor3 = Color.FromArgb(theme.FreshwaterColorPalette[2] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 3)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor4 = Color.FromArgb(theme.FreshwaterColorPalette[3] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 4)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor5 = Color.FromArgb(theme.FreshwaterColorPalette[4] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 5)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor6 = Color.FromArgb(theme.FreshwaterColorPalette[5] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 6)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor7 = Color.FromArgb(theme.FreshwaterColorPalette[6] ?? Color.White.ToArgb());
                    }

                    if (theme.FreshwaterColorPalette?.Count > 7)
                    {
                        WaterFeatureManager.WaterFeatureMediator.CustomColor8 = Color.FromArgb(theme.FreshwaterColorPalette[7] ?? Color.White.ToArgb());
                    }
                }

                if (themeFilter.ApplyPathSetSettings)
                {
                    PathManager.PathMediator.PathColor = Color.FromArgb(theme.PathColor ?? Color.FromArgb(75, 49, 26).ToArgb());
                    PathManager.PathMediator.PathWidth = theme.PathWidth ?? 8;
                    PathManager.PathMediator.PathType = theme.PathStyle ?? PathType.SolidLinePath;
                }

                if (themeFilter.ApplySymbolSettings && theme.SymbolCustomColors != null)
                {
                    SymbolManager.SymbolMediator.SymbolColor1 = Color.FromArgb(theme.SymbolCustomColors[0] ?? Color.White.ToArgb());
                    SymbolManager.SymbolMediator.SymbolColor2 = Color.FromArgb(theme.SymbolCustomColors[1] ?? Color.White.ToArgb());
                    SymbolManager.SymbolMediator.SymbolColor3 = Color.FromArgb(theme.SymbolCustomColors[2] ?? Color.White.ToArgb());
                }

                if (themeFilter.ApplyLabelPresetSettings)
                {
                    LabelPresetManager.PresetMediator?.AddLabelPresets();
                }

                if (themeFilter.ApplyLabelSettings)
                {
                    if (!string.IsNullOrEmpty(theme.LabelFont))
                    {
                        Font? themeFont = new("Segoe UI", 12.0F, FontStyle.Regular, GraphicsUnit.Point, 0);

                        FontConverter cvt = new();
                        themeFont = (Font?)cvt.ConvertFromString(theme.LabelFont);

                        if (themeFont != null && !theme.LabelFont.Contains(themeFont.FontFamily.Name))
                        {
                            themeFont = null;
                        }

                        if (themeFont == null)
                        {
                            string[] fontParts = theme.LabelFont.Split(',');

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
                                    themeFont = new Font(family, fontsize, FontStyle.Regular, GraphicsUnit.Point);
                                }
                                catch
                                {
                                    // couldn't create the font, so try to find it in the list of embedded fonts
                                    for (int i = 0; i < LabelManager.EMBEDDED_FONTS.Families.Length; i++)
                                    {
                                        if (LabelManager.EMBEDDED_FONTS.Families[i].Name == ff)
                                        {
                                            themeFont = new Font(LabelManager.EMBEDDED_FONTS.Families[i], fontsize, FontStyle.Regular, GraphicsUnit.Point);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (themeFont != null)
                        {
                            FontPanelManager.FontPanelSelectedFont.SelectedFont = new Font(themeFont.FontFamily, 12);
                            FontPanelManager.FontPanelSelectedFont.FontSize = themeFont.SizeInPoints;

                            LabelManager.LabelMediator.SelectedLabelFont = themeFont;
                        }
                    }

                    LabelManager.LabelMediator.LabelColor = Color.FromArgb(theme.LabelColor ?? Color.FromArgb(61, 53, 30).ToArgb());
                    LabelManager.LabelMediator.OutlineColor = Color.FromArgb(theme.LabelOutlineColor ?? Color.FromArgb(161, 214, 202, 171).ToArgb());
                    LabelManager.LabelMediator.OutlineWidth = (int?)theme.LabelOutlineWidth / 10.0F ?? 0;
                    LabelManager.LabelMediator.GlowColor = Color.FromArgb(theme.LabelGlowColor ?? Color.White.ToArgb());
                    LabelManager.LabelMediator.GlowStrength = theme.LabelGlowStrength ?? 0;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying theme: {ex.Message}");
            }
        }
    }
}
