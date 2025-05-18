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
    internal class ScratchedFilmEffect : ShaderEffect
    {
        private static PixelShader _pixelShader = new PixelShader()
        {
            UriSource = new Uri(AppContext.BaseDirectory + Path.DirectorySeparatorChar + "Shaders" + Path.DirectorySeparatorChar + "ScratchedFilmEffect.ps")
        };

        public ScratchedFilmEffect()
        {
            PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(TimerProperty);
            UpdateShaderValue(Speed1Property);
            UpdateShaderValue(Speed2Property);
            UpdateShaderValue(ScratchIntensityProperty);
            UpdateShaderValue(ISProperty);
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ScratchedFilmEffect), 0);
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        //
        public static readonly DependencyProperty TimerProperty = DependencyProperty.Register("Timer", typeof(float), typeof(ScratchedFilmEffect), new UIPropertyMetadata(0.03f, PixelShaderConstantCallback(0), CoerceTimer));
        public float Timer
        {
            get { return (float)GetValue(TimerProperty); }
            set { SetValue(TimerProperty, value); }
        }

        private static object CoerceTimer(DependencyObject d, object value)
        {
            ScratchedFilmEffect effect = (ScratchedFilmEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 1.0f)
            {
                return effect.Timer;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty Speed1Property = DependencyProperty.Register("Speed1", typeof(float), typeof(ScratchedFilmEffect), new UIPropertyMetadata(0.03f, PixelShaderConstantCallback(0), CoerceSpeed1));
        public float Speed1
        {
            get { return (float)GetValue(Speed1Property); }
            set { SetValue(Speed1Property, value); }
        }

        private static object CoerceSpeed1(DependencyObject d, object value)
        {
            ScratchedFilmEffect effect = (ScratchedFilmEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 0.2f)
            {
                return effect.Speed1;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty Speed2Property = DependencyProperty.Register("Speed2", typeof(float), typeof(ScratchedFilmEffect), new UIPropertyMetadata(0.01f, PixelShaderConstantCallback(0), CoerceSpeed2));
        public float Speed2
        {
            get { return (float)GetValue(Speed2Property); }
            set { SetValue(Speed2Property, value); }
        }

        private static object CoerceSpeed2(DependencyObject d, object value)
        {
            ScratchedFilmEffect effect = (ScratchedFilmEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 0.01f)
            {
                return effect.Speed2;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty ScratchIntensityProperty = DependencyProperty.Register("ScratchIntensity", typeof(float), typeof(ScratchedFilmEffect), new UIPropertyMetadata(0.65f, PixelShaderConstantCallback(0), CoerceScratchIntensity));
        public float ScratchIntensity
        {
            get { return (float)GetValue(ScratchIntensityProperty); }
            set { SetValue(ScratchIntensityProperty, value); }
        }

        private static object CoerceScratchIntensity(DependencyObject d, object value)
        {
            ScratchedFilmEffect effect = (ScratchedFilmEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 1.0f)
            {
                return effect.ScratchIntensity;
            }

            return newFactor;
        }

        //

        public static readonly DependencyProperty ISProperty = DependencyProperty.Register("IS", typeof(float), typeof(ScratchedFilmEffect), new UIPropertyMetadata(0.01f, PixelShaderConstantCallback(0), CoerceIS));
        public float IS
        {
            get { return (float)GetValue(ISProperty); }
            set { SetValue(ISProperty, value); }
        }

        private static object CoerceIS(DependencyObject d, object value)
        {
            ScratchedFilmEffect effect = (ScratchedFilmEffect)d;
            float newFactor = (float)value;

            if (newFactor < 0.0f || newFactor > 0.1f)
            {
                return effect.IS;
            }

            return newFactor;
        }
    }
}
