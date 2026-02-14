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
namespace RealmStudioX
{
    public sealed class BackgroundUIMediator : UiMediatorBase, IUIMediatorObserver
    {

        private readonly List<MapTexture> _backgroundTextureList = [];
        private int _backgroundTextureIndex;

        private bool _showBackground = true;
        private float _backgroundTextureScale = 1.0F;
        private bool _mirrorBackgroundTexture;

        #region Property Setters/Getters

        internal bool ShowBackground
        {
            get { return _showBackground; }
            set { SetPropertyField(nameof(ShowBackground), ref _showBackground, value); }
        }

        internal List<MapTexture> BackgroundTextureList
        {
            get { return _backgroundTextureList; }
        }

        internal int BackgroundTextureIndex
        {
            get { return _backgroundTextureIndex; }
            set { SetPropertyField(nameof(BackgroundTextureIndex), ref _backgroundTextureIndex, value); }
        }

        internal float BackgroundTextureScale
        {
            get { return _backgroundTextureScale; }
            set { SetPropertyField(nameof(BackgroundTextureScale), ref _backgroundTextureScale, value); }
        }

        internal bool MirrorBackgroundTexture
        {
            get { return _mirrorBackgroundTexture; }
            set { SetPropertyField(nameof(MirrorBackgroundTexture), ref _mirrorBackgroundTexture, value); }
        }

        #endregion


        #region Property Change Handler Methods

        internal void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                RaiseChanged();
            }
        }

        #endregion


        #region Background UI methods

        /*
        internal void Initialize(int backgroundTextureIndex, float backgroundTextureScale, bool mirrorBackground)
        {
            _backgroundTextureIndex = backgroundTextureIndex;
            _backgroundTextureScale = backgroundTextureScale;
            _mirrorBackgroundTexture = mirrorBackground;

            MainForm.BackgroundTextureScaleTrack.Value = (int)(backgroundTextureScale * 100.0F);
            MainForm.MirrorBackgroundSwitch.Checked = mirrorBackground;

            UpdateBackgroundTexturePictureBox();
        }

        internal void Reset()
        {
            BackgroundTextureIndex = 0;
            ShowBackground = true;
            BackgroundTextureScale = 1.0F;
            MirrorBackgroundTexture = false;
        }

        private void UpdateBackgroundTexturePictureBox()
        {
            if (BackgroundTextureIndex < 0)
            {
                BackgroundTextureIndex = 0;
            }

            if (BackgroundTextureIndex > BackgroundTextureList.Count - 1)
            {
                BackgroundTextureIndex = BackgroundTextureList.Count - 1;
            }

            if (BackgroundTextureList[BackgroundTextureIndex].TextureBitmap == null)
            {
                BackgroundTextureList[BackgroundTextureIndex].TextureBitmap = (Bitmap?)Bitmap.FromFile(BackgroundTextureList[BackgroundTextureIndex].TexturePath);
            }

            MainForm.BackgroundTextureBox.Image = BackgroundTextureList[BackgroundTextureIndex].TextureBitmap;
            MainForm.BackgroundTextureNameLabel.Text = BackgroundTextureList[BackgroundTextureIndex].TextureName;
        }
        */

        #endregion
    }
}
