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
using System.ComponentModel;

namespace RealmStudio
{
    internal class VignetteUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;

        private int _vignetteStrength = 148;
        private Color _vignetteColor = Color.FromArgb(201, 151, 123);
        private VignetteShapeType _vignetteShape = VignetteShapeType.Oval;

        public VignetteUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += VignetteUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters

        internal int VignetteStrength
        {
            get { return _vignetteStrength; }
            set { SetPropertyField(nameof(VignetteStrength), ref _vignetteStrength, value); }
        }

        internal Color VignetteColor
        {
            get { return _vignetteColor; }
            set { SetPropertyField(nameof(VignetteColor), ref _vignetteColor, value); }
        }

        internal VignetteShapeType VignetteShape
        {
            get { return _vignetteShape; }
            set { SetPropertyField(nameof(VignetteShape), ref _vignetteShape, value); }
        }


        #endregion

        #region Property Change Handler Methods
        internal void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyUpdate(string? changedPropertyName)
        {
            UpdateVignetteUI(changedPropertyName);
            VignetteManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateVignetteUI(string? changedPropertyName)
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.VignetteColorSelectionButton.BackColor = VignetteColor;

                MainForm.RectangleVignetteRadio.Checked = VignetteShape == VignetteShapeType.Rectangle;
                MainForm.OvalVignetteRadio.Checked = VignetteShape == VignetteShapeType.Oval;

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    if (changedPropertyName == "VignetteStrength")
                    {
                        MainForm.VignetteStrengthTrack.Value = VignetteStrength;
                    }
                }
            }));
        }

        #endregion

        #region Event Handlers
        private void VignetteUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

        #endregion
    }
}
