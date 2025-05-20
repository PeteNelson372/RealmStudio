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
    internal sealed class BloomEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new()
        {
            UriSource = new Uri(AppContext.BaseDirectory + Path.DirectorySeparatorChar + "Shaders" + Path.DirectorySeparatorChar + "BloomEffect.ps")
        };

        public BloomEffect()
        {
            PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(SceneIntensityProperty);
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BloomEffect), 0);
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }


        public static readonly DependencyProperty SceneIntensityProperty = DependencyProperty.Register("SceneIntensity", typeof(float), typeof(BloomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceSceneIntensity));
        public float SceneIntensity
        {
            get { return (float)GetValue(SceneIntensityProperty); }
            set { SetValue(SceneIntensityProperty, value); }
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1859 // Use concrete types when possible for improved performance

        private static object CoerceSceneIntensity(DependencyObject d, object value)
        {
            BloomEffect effect = (BloomEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 2.0f)
            {
                return effect.SceneIntensity;
            }

            return newFactor;
        }

        //
        public static readonly DependencyProperty GlowIntensityProperty = DependencyProperty.Register("GlowIntensity", typeof(float), typeof(BloomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceGlowIntensity));
        public float GlowIntensity
        {
            get { return (float)GetValue(GlowIntensityProperty); }
            set { SetValue(GlowIntensityProperty, value); }
        }

        private static object CoerceGlowIntensity(DependencyObject d, object value)
        {
            BloomEffect effect = (BloomEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 2.0f)
            {
                return effect.GlowIntensity;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty HighlightThresholdProperty = DependencyProperty.Register("HighlightThreshold", typeof(float), typeof(BloomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceHighlightThreshold));
        public float HighlightThreshold
        {
            get { return (float)GetValue(HighlightThresholdProperty); }
            set { SetValue(HighlightThresholdProperty, value); }
        }

        private static object CoerceHighlightThreshold(DependencyObject d, object value)
        {
            BloomEffect effect = (BloomEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 1.0f)
            {
                return effect.HighlightThreshold;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty HighlightIntensityProperty = DependencyProperty.Register("HighlightIntensity", typeof(float), typeof(BloomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceHighlightIntensity));
        public float HighlightIntensity
        {
            get { return (float)GetValue(HighlightIntensityProperty); }
            set { SetValue(HighlightIntensityProperty, value); }
        }

        private static object CoerceHighlightIntensity(DependencyObject d, object value)
        {
            BloomEffect effect = (BloomEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 10.0f)
            {
                return effect.HighlightIntensity;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty BlurWidthProperty = DependencyProperty.Register("BlurWidth", typeof(float), typeof(BloomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceBlurWidth));
        public float BlurWidth
        {
            get { return (float)GetValue(BlurWidthProperty); }
            set { SetValue(BlurWidthProperty, value); }
        }
        private static object CoerceBlurWidth(DependencyObject d, object value)
        {
            BloomEffect effect = (BloomEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 10.0f)
            {
                return effect.BlurWidth;
            }

            return newFactor;
        }

#pragma warning restore CA1859 // Use concrete types when possible for improved performance
#pragma warning restore IDE0079 // Remove unnecessary suppression
    }
}
