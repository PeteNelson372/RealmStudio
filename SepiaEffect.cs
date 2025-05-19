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
using System.IO;
using System.Windows;
using System.Windows.Media.Effects;

namespace RealmStudio
{
    // this class and other effect classes derived from ShaderEffect
    // are based on the example from https://bursjootech.blogspot.com/2008/06/grayscale-effect-pixel-shader-effect-in.html
    //
    // Some HLSL shaders are here: https://developer.download.nvidia.com/shaderlibrary/webpages/hlsl_shaders.html
    //
    internal class SepiaEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new()
        {
            UriSource = new Uri(AppContext.BaseDirectory + Path.DirectorySeparatorChar + "Shaders" + Path.DirectorySeparatorChar + "SepiaEffect.ps")
        };

        public SepiaEffect()
        {
            PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(GrayscaleFactorProperty);
        }

        public static readonly DependencyProperty InputProperty = SepiaEffect.RegisterPixelShaderSamplerProperty("Input", typeof(SepiaEffect), 0);
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty GrayscaleFactorProperty = DependencyProperty.Register("DesaturationFactor", typeof(float), typeof(SepiaEffect), new UIPropertyMetadata(0.5f, PixelShaderConstantCallback(0), CoerceGrayscaleFactor));
        public float GrayscaleFactor
        {
            get { return (float)GetValue(GrayscaleFactorProperty); }
            set { SetValue(GrayscaleFactorProperty, value); }
        }

        private static object CoerceGrayscaleFactor(DependencyObject d, object value)
        {
            SepiaEffect effect = (SepiaEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0 || newFactor > 1.0)
            {
                return effect.GrayscaleFactor;
            }

            return newFactor;
        }
    }
}

