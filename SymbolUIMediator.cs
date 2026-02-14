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
    internal sealed class SymbolUIMediator : UiMediatorBase, IUIMediatorObserver
    {
        // symbol UI values
        private bool _enabled = true;
        private bool _symbolScaleLocked;
        private float _symbolScale = 100;
        private bool _randomizeSymbolColors;
        private Color _symbolColor1 = Color.FromArgb(85, 44, 36);
        private Color _symbolColor2 = Color.FromArgb(53, 45, 32);
        private Color _symbolColor3 = Color.FromArgb(161, 214, 202, 171);
        private bool _useAreaBrush;
        private int _areaBrushSize = 64;
        private bool _mirrorSymbol;
        private float _symbolRotation;
        private float _symbolPlacementRate = 1.0F;
        private float _symbolPlacementDensity = 1.0F;

        // symbol UI constants
        private const int _pictureBoxWidth = 120;
        private const int _pictureBoxHeight = 45;

        #region Property Setter/Getters

        // UI value setters/getters
        internal bool Enabled
        {
            get { return _enabled; }
            set { SetPropertyField(nameof(Enabled), ref _enabled, value); }
        }

        internal bool SymbolScaleLocked
        {
            get { return _symbolScaleLocked; }
            set
            {
                SetPropertyField(nameof(SymbolScaleLocked), ref _symbolScaleLocked, value);
            }
        }

        internal float SymbolScale
        {
            get { return _symbolScale; }
            set
            {
                if (!SymbolScaleLocked)
                {
                    SetPropertyField(nameof(SymbolScale), ref _symbolScale, value);
                }
            }
        }

        internal bool RandomizeSymbolColors
        {
            get { return _randomizeSymbolColors; }
            set
            {
                SetPropertyField(nameof(RandomizeSymbolColors), ref _randomizeSymbolColors, value);
            }
        }

        internal Color SymbolColor1
        {
            get { return _symbolColor1; }
            set
            {
                SetPropertyField(nameof(SymbolColor1), ref _symbolColor1, value);
            }
        }

        internal Color SymbolColor2
        {
            get { return _symbolColor2; }
            set
            {
                SetPropertyField(nameof(SymbolColor2), ref _symbolColor2, value);
            }
        }

        internal Color SymbolColor3
        {
            get { return _symbolColor3; }
            set
            {
                SetPropertyField(nameof(SymbolColor3), ref _symbolColor3, value);
            }
        }

        internal bool UseAreaBrush
        {
            get { return _useAreaBrush; }
            set
            {
                SetPropertyField(nameof(UseAreaBrush), ref _useAreaBrush, value);
            }
        }

        internal int AreaBrushSize
        {
            get { return _areaBrushSize; }
            set
            {
                SetPropertyField(nameof(AreaBrushSize), ref _areaBrushSize, value);
            }
        }

        internal bool MirrorSymbol
        {
            get { return _mirrorSymbol; }
            set
            {
                SetPropertyField(nameof(MirrorSymbol), ref _mirrorSymbol, value);
            }
        }

        internal float SymbolRotation
        {
            get { return _symbolRotation; }
            set
            {
                SetPropertyField(nameof(SymbolRotation), ref _symbolRotation, value);
            }
        }

        internal float SymbolPlacementRate
        {
            get { return _symbolPlacementRate; }
            set
            {
                SetPropertyField(nameof(SymbolPlacementRate), ref _symbolPlacementRate, value);
            }
        }

        internal float SymbolPlacementDensity
        {
            get { return _symbolPlacementDensity; }
            set
            {
                SetPropertyField(nameof(SymbolPlacementDensity), ref _symbolPlacementDensity, value);
            }
        }

        internal static int SymbolPictureBoxWidth
        {
            get { return _pictureBoxWidth; }
        }

        internal static int SymbolPictureBoxHeight
        {
            get { return _pictureBoxHeight; }
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

        /*
        private void UpdateSymbolUI(string? changedPropertyName)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            // this methods updates the Main Form Symbol Tab UI
            // based on the property that has changed


            MapLayer symbollLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.SYMBOLLAYER);
            symbollLayer.ShowLayer = Enabled;

            if (string.IsNullOrEmpty(changedPropertyName))
            {
                SymbolScaleLockChange();
                SymbolScaleChange();
                SymbolColorButtonsChanged();
                SymbolRotationChanged();
                UseAreaBrushChanged();
            }
            else
            {
                switch (changedPropertyName)
                {
                    case "SymbolScaleLocked":
                        { SymbolScaleLockChange(); break; }
                    case "SymbolScale":
                        { SymbolScaleChange(); break; }
                    case "SymboColor1":
                    case "SymbolColor2":
                    case "SymbolColor3":
                        { SymbolColorButtonsChanged(); break; }
                    case "SymbolRotation":
                        { SymbolRotationChanged(); break; }
                    case "UseAreaBrush":
                        {
                            UseAreaBrushChanged(); break;
                        }
                    default:
                        {
                            SymbolScaleLockChange();
                            SymbolScaleChange();
                            SymbolColorButtonsChanged();
                            SymbolRotationChanged();
                            UseAreaBrushChanged();
                            break;
                        }
                }
            }
        }

        private void SymbolScaleLockChange()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                if (SymbolScaleLocked)
                {
                    MainForm.LockSymbolScaleButton.IconChar = FontAwesome.Sharp.IconChar.Lock;
                    MainForm.SymbolScaleTrack.Enabled = false;
                    MainForm.SymbolScaleUpDown.Enabled = false;
                }
                else
                {
                    MainForm.LockSymbolScaleButton.IconChar = FontAwesome.Sharp.IconChar.LockOpen;
                    MainForm.SymbolScaleTrack.Enabled = true;
                    MainForm.SymbolScaleUpDown.Enabled = true;
                }

                MainForm.LockSymbolScaleButton.Refresh();
                MainForm.SymbolScaleTrack.Refresh();
                MainForm.SymbolScaleUpDown.Refresh();
            }));
        }

        private void SymbolScaleChange()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                if (!SymbolScaleLocked)
                {
                    MainForm.SymbolScaleTrack.Value = (int)SymbolScale;
                    MainForm.SymbolScaleUpDown.Value = (int)SymbolScale;
                }

                MainForm.SymbolScaleTrack.Refresh();
                MainForm.SymbolScaleUpDown.Refresh();
            }));
        }

        private void SymbolColorButtonsChanged()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.SymbolColor1Button.BackColor = SymbolColor1;
                MainForm.SymbolColor2Button.BackColor = SymbolColor2;
                MainForm.SymbolColor3Button.BackColor = SymbolColor3;
            }));
        }

        private void SymbolRotationChanged()
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.SymbolRotationTrack.Value = (int)SymbolRotation;
                MainForm.SymbolRotationUpDown.Value = (decimal)SymbolRotation;

                MainForm.SymbolRotationUpDown.Refresh();
            }));
        }

        private void UseAreaBrushChanged()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                if (UseAreaBrush)
                {
                    AreaBrushSize = MainForm.AreaBrushSizeTrack.Value;
                    MapStateMediator.MainUIMediator.SelectedBrushSize = AreaBrushSize;
                }
                else
                {
                    MapStateMediator.MainUIMediator.SelectedBrushSize = 0;
                }
            }));
        }
        */
        #endregion

        #region Event Handlers
        /*


        // MainForm UI Event handlers //
        private void SymbolPictureBox_Paint(object? sender, PaintEventArgs e)
        {
            PictureBox? pb = (PictureBox?)sender;
            if (pb != null)
            {
                ControlPaint.DrawBorder(e.Graphics, pb.ClientRectangle, Color.LightGray, ButtonBorderStyle.Dashed);
            }
        }

        private void SymbolPictureBox_MouseHover(object? sender, EventArgs e)
        {
            PictureBox? pb = (PictureBox?)sender;

            if (pb != null && pb.Tag is MapSymbol s)
            {
                RealmStudioMainForm.TOOLTIP.Show(s.SymbolName, pb);
            }
        }

        private void SymbolPictureBox_MouseClick(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            PictureBox? pb = (PictureBox?)sender;
            if (pb == null) return;

            MapSymbol? clickedSymbol = pb.Tag as MapSymbol;

            if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Left)
            {
                // can't select more than one marker symbol
                if (RealmStudioMainForm.ModifierKeys == Keys.Shift && clickedSymbol?.SymbolType != MapSymbolType.Marker)
                {
                    // secondary symbol selection - for additional symbols to be used when painting symbols to the map (forests, etc.)
                    if (MapStateMediator.MainUIMediator.CurrentDrawingMode == MapDrawingMode.SymbolPlace)
                    {
                        if (pb.BackColor == Color.AliceBlue)
                        {
                            pb.BackColor = SystemColors.Control;
                            pb.Refresh();

                            if (pb.Tag is MapSymbol s)
                            {
                                SymbolManager.SecondarySelectedSymbols.Remove(s);
                            }
                        }
                        else
                        {
                            pb.BackColor = Color.AliceBlue;
                            pb.Refresh();

                            if (pb.Tag is MapSymbol s)
                            {
                                SymbolManager.SecondarySelectedSymbols.Add(s);
                            }
                        }
                    }
                }
                else if (RealmStudioMainForm.ModifierKeys == Keys.None)
                {
                    MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolPlace, 0);

                    // primary symbol selection                    
                    if (pb.Tag is MapSymbol)
                    {
                        SelectPrimarySymbolInSymbolTable(pb);
                    }
                }
            }
            else if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Right)
            {
                if (pb.Tag is MapSymbol s)
                {
                    SymbolInfo si = new(s);
                    si.ShowDialog();
                }
            }
        }
        */
        #endregion

        #region Map Symbol UI Methods
        /*
        internal void Reset()
        {
            // reset symbol UI values to default values
            Enabled = true;
            SymbolScaleLocked = false;
            SymbolScale = 100;
            SymbolColor1 = Color.FromArgb(85, 44, 36);
            SymbolColor2 = Color.FromArgb(53, 45, 32);
            SymbolColor3 = Color.FromArgb(161, 214, 202, 171);
            UseAreaBrush = false;
            AreaBrushSize = 0;
            MirrorSymbol = false;
            SymbolRotation = 0.0F;
            SymbolPlacementRate = 1.0F;
            SymbolPlacementDensity = 1.0F;
            ResetSymbolColorButtons();
        }

        internal void ColorButtonMouseUp(object sender, MouseEventArgs e)
        {
            var mainForm = UtilityMethods.GetMainForm();
            if (mainForm == null)
            {
                // Handle the case where the main form is null to avoid passing a null reference
                return;
            }

            if (RealmStudioMainForm.ModifierKeys == Keys.None)
            {
                IconButton colorButton = (IconButton)sender;

                Color c = UtilityMethods.SelectColor(mainForm, e, colorButton.BackColor);

                if (colorButton == MainForm.SymbolColor1Button)
                {
                    SymbolColor1 = c;
                }
                else if (colorButton == MainForm.SymbolColor2Button)
                {
                    SymbolColor2 = c;
                }
                else if (colorButton == MainForm.SymbolColor3Button)
                {
                    SymbolColor3 = c;
                }
                else
                {
                    SymbolColor1 = c;
                }
            }
            else if (RealmStudioMainForm.ModifierKeys == Keys.Control && MapStateMediator.SelectedMapSymbol != null)
            {
                if (MapStateMediator.SelectedMapSymbol.IsGrayscale || MapStateMediator.SelectedMapSymbol.UseCustomColors)
                {
                    // if a symbol has been selected and is grayscale or custom colored, then color it with the
                    // selected custom colors

                    Color paintColor = ((Button)sender).BackColor;

                    //Cmd_PaintSymbol cmd = new(MapStateMediator.SelectedMapSymbol,
                    //    paintColor.ToSKColor(), SymbolColor1.ToSKColor(), SymbolColor2.ToSKColor(), SymbolColor3.ToSKColor());

                    //CommandManager.AddCommand(cmd);
                    //cmd.DoOperation();
                }
            }
        }

        internal void ResetSymbolColorButtons()
        {
            if (AssetManager.CURRENT_THEME != null && AssetManager.CURRENT_THEME.SymbolCustomColors != null)
            {
                SymbolColor1 = Color.FromArgb(AssetManager.CURRENT_THEME.SymbolCustomColors[0] ?? Color.FromArgb(85, 44, 36).ToArgb());
                SymbolColor2 = Color.FromArgb(AssetManager.CURRENT_THEME.SymbolCustomColors[1] ?? Color.FromArgb(255, 53, 45, 32).ToArgb());
                SymbolColor3 = Color.FromArgb(AssetManager.CURRENT_THEME.SymbolCustomColors[2] ?? Color.FromArgb(161, 214, 202, 171).ToArgb());
            }
            else
            {
                SymbolColor1 = Color.FromArgb(255, 85, 44, 36);
                SymbolColor2 = Color.FromArgb(255, 53, 45, 32);
                SymbolColor3 = Color.FromArgb(161, 214, 202, 171);
            }
        }

        internal void AddSymbolsToSymbolTable(List<MapSymbol> symbols)
        {
            const int _pictureBoxBaseHeight = 680;

            MainForm.SymbolTable.AutoScroll = false;
            SymbolTable.VerticalScroll.Enabled = true;
            SymbolTable.Hide();
            SymbolToolPanel.Refresh();
            SymbolTable.Controls.Clear();
            SymbolTable.RowCount = 0;
            SymbolTable.Refresh();

            for (int i = 0; i < symbols.Count; i++)
            {
                MapSymbol symbol = symbols[i];
                Bitmap? pbm = SymbolManager.GetSymbolPictureBoxBitmap(symbol);

                if (pbm != null)
                {
                    PictureBox pb = new()
                    {
                        Width = SymbolPictureBoxWidth,
                        Height = SymbolPictureBoxHeight,
                        Tag = symbol,
                        SizeMode = PictureBoxSizeMode.CenterImage,
                        Image = (Image)pbm.Clone(),
                        Margin = new Padding(0, 0, 0, 0),
                        Padding = new Padding(0, 4, 0, 4),
                        BorderStyle = BorderStyle.None,
                    };

                    pb.MouseHover += SymbolPictureBox_MouseHover;
                    pb.MouseClick += SymbolPictureBox_MouseClick;
                    pb.Paint += SymbolPictureBox_Paint;

                    SymbolTable.Controls.Add(pb);
                    SymbolTable.RowStyles.Add(new RowStyle(SizeType.Absolute, SymbolPictureBoxHeight));
                }
            }

            SymbolTable.RowCount = symbols.Count;
            SymbolTable.Width = 130;

            SymbolTable.Height = Math.Min(_pictureBoxBaseHeight, (symbols.Count * SymbolPictureBoxHeight));
            SymbolTable.VerticalScroll.Maximum = (symbols.Count * SymbolPictureBoxHeight) + (symbols.Count * 2);
            SymbolTable.HorizontalScroll.Maximum = 0;
            SymbolTable.HorizontalScroll.Enabled = false;
            SymbolTable.HorizontalScroll.Visible = false;
            SymbolTable.AutoScroll = true;

            SymbolTable.Show();

            SymbolTable.Refresh();
            SymbolToolPanel.Refresh();
        }

        internal List<MapSymbol> GetFilteredMapSymbols()
        {
            List<string> selectedCollections = [.. MainForm.SymbolCollectionsListBox.CheckedItems.Cast<string>()];
            List<string> selectedTags = [.. MainForm.SymbolTagsListBox.CheckedItems.Cast<string>()];
            List<MapSymbol> filteredSymbols = SymbolManager.GetFilteredSymbolList(SymbolManager.SelectedSymbolType, selectedCollections, selectedTags);

            return filteredSymbols;
        }

        internal void SymbolCollectionsListItemCheck(ItemCheckEventArgs e)
        {
            List<string> checkedCollections = [];
            foreach (string item in MainForm.SymbolCollectionsListBox.CheckedItems)
            {
                checkedCollections.Add(item.ToString());
            }

            string? collectionitem = MainForm.SymbolCollectionsListBox.Items[e.Index].ToString();

            if (!string.IsNullOrEmpty(collectionitem))
            {
                collectionitem = collectionitem.Trim();
                if (e.NewValue == CheckState.Checked)
                {
                    checkedCollections.Add(collectionitem);
                }
                else
                {
                    checkedCollections.Remove(collectionitem);
                }
            }

            List<string> selectedTags = [.. MainForm.SymbolTagsListBox.CheckedItems.Cast<string>()];
            List<MapSymbol> filteredSymbols = SymbolManager.GetFilteredSymbolList(SymbolManager.SelectedSymbolType, checkedCollections, selectedTags);
            AddSymbolsToSymbolTable(filteredSymbols);
        }

        internal void SymbolTagsListItemCheck(ItemCheckEventArgs e)
        {

            List<string> checkedTags = [];
            foreach (string item in MainForm.SymbolTagsListBox.CheckedItems)
            {
                checkedTags.Add(item.ToString());
            }

            string? tagItem = MainForm.SymbolTagsListBox.Items[e.Index].ToString();
            if (!string.IsNullOrEmpty(tagItem))
            {
                if (e.NewValue == CheckState.Checked)
                {
                    checkedTags.Add(tagItem);
                }
                else
                {
                    checkedTags.Remove(tagItem);
                }
            }


            List<string> selectedCollections = [.. MainForm.SymbolCollectionsListBox.CheckedItems.Cast<string>()];
            List<MapSymbol> filteredSymbols = SymbolManager.GetFilteredSymbolList(SymbolManager.SelectedSymbolType, selectedCollections, checkedTags);
            AddSymbolsToSymbolTable(filteredSymbols);
        }

        internal void ColorSymbols()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            if (MapStateMediator.SelectedMapSymbol != null && MapStateMediator.SelectedMapSymbol.IsSelected)
            {
                SymbolManager.ColorSelectedSymbol(MapStateMediator.SelectedMapSymbol);
            }
            else
            {
                if (UseAreaBrush)
                {
                    AreaBrushSize = MapStateMediator.MainUIMediator.SelectedBrushSize;
                    MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolColor, AreaBrushSize);
                }
                else
                {
                    MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolColor, 0);
                    AreaBrushSize = 0;
                }
            }
        }

        internal void SelectPrimarySymbolInSymbolTable(PictureBox pb)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            if (pb.Tag is MapSymbol s)
            {
                if (SymbolManager.SelectedSymbolTableMapSymbol == null ||
                    s.SymbolGuid.ToString() != SymbolManager.SelectedSymbolTableMapSymbol.SymbolGuid.ToString())
                {
                    foreach (Control control in SymbolTable.Controls)
                    {
                        if (control != pb)
                        {
                            control.BackColor = SystemColors.Control;
                            control.Refresh();
                        }
                    }

                    SymbolManager.SecondarySelectedSymbols.Clear();
                    Color pbBackColor = pb.BackColor;

                    if (pbBackColor == SystemColors.Control)
                    {
                        // clicked symbol is not selected, so select it
                        pb.BackColor = Color.LightSkyBlue;
                        pb.Refresh();

                        SymbolManager.SelectedSymbolTableMapSymbol = s;
                    }
                    else
                    {
                        // clicked symbol is already selected, so deselect it
                        pb.BackColor = SystemColors.Control;
                        pb.Refresh();

                        SymbolManager.SelectedSymbolTableMapSymbol = null;
                        MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.None, 0);
                    }
                }
            }
        }

        internal void SelectSymbolsOfType(MapSymbolType symbolType)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolPlace, 0);

            if (SymbolManager.SelectedSymbolType != symbolType)
            {
                SymbolManager.SelectedSymbolType = symbolType;
                List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();

                AddSymbolsToSymbolTable(selectedSymbols);
                MainForm.AreaBrushSwitch.Checked = false;
                MainForm.AreaBrushSwitch.Enabled = false;
            }

            if (SymbolManager.SelectedSymbolType == MapSymbolType.Vegetation || SymbolManager.SelectedSymbolType == MapSymbolType.Terrain)
            {
                MainForm.AreaBrushSwitch.Enabled = true;
            }

            if (SymbolTable.Controls.Count > 0)
            {
                if (SymbolManager.SelectedSymbolTableMapSymbol == null || SymbolManager.SelectedSymbolTableMapSymbol.SymbolType != symbolType)
                {
                    PictureBox pb = (PictureBox)SymbolTable.Controls[0];
                    SelectPrimarySymbolInSymbolTable(pb);
                }
            }
        }

        internal void SearchSymbols(string searchText)
        {
            // filter symbol list based on text entered by the user

            if (searchText.Length > 2)
            {
                List<string> selectedCollections = [.. MainForm.SymbolCollectionsListBox.CheckedItems.Cast<string>()];
                List<string> selectedTags = [.. MainForm.SymbolTagsListBox.CheckedItems.Cast<string>()];
                List<MapSymbol> filteredSymbols = SymbolManager.GetFilteredSymbolList(SymbolManager.SelectedSymbolType,
                    selectedCollections, selectedTags, searchText);

                AddSymbolsToSymbolTable(filteredSymbols);
            }
            else if (searchText.Length == 0)
            {
                List<string> selectedCollections = [.. MainForm.SymbolCollectionsListBox.CheckedItems.Cast<string>()];
                List<string> selectedTags = [.. MainForm.SymbolTagsListBox.CheckedItems.Cast<string>()];
                List<MapSymbol> filteredSymbols = SymbolManager.GetFilteredSymbolList(SymbolManager.SelectedSymbolType, selectedCollections, selectedTags);

                AddSymbolsToSymbolTable(filteredSymbols);
            }
        }

        #endregion

        #region static Map Symbol UI methods
        internal static MapSymbol? SelectMapSymbolAtPoint(RealmStudioMap map, PointF mapClickPoint)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.CurrentMap);

            MapSymbol? selectedSymbol = null;

            
            List<MapComponent> mapSymbolComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.SYMBOLLAYER).MapLayerComponents;

            for (int i = 0; i < mapSymbolComponents.Count; i++)
            {
                if (mapSymbolComponents[i] is MapSymbol mapSymbol)
                {
                    RectangleF symbolRect = new(mapSymbol.X, mapSymbol.Y, mapSymbol.Width, mapSymbol.Height);

                    if (symbolRect.Contains(mapClickPoint))
                    {
                        selectedSymbol = mapSymbol;
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedSymbol);

            

            return selectedSymbol;
        }

        */

        #endregion
    }
}
