using FontAwesome.Sharp;
using SkiaSharp.Views.Desktop;
using System.ComponentModel;

namespace RealmStudio
{
    internal class MapSymbolUIMediator : IUIMediatorObserver
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;
        private TableLayoutPanel _symbolTable;
        private readonly Panel _symbolToolPanel;

        // symbol UI values
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

        internal MapSymbolUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            _symbolTable = mainForm.SymbolTable;
            _symbolToolPanel = mainForm.SymbolToolPanel;
            PropertyChanged += MapSymbolUIMediator_PropertyChanged;
        }

        #region Property Setter/Getters

        internal MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        internal TableLayoutPanel SymbolTable
        {
            get { return _symbolTable; }
            set { _symbolTable = value; }
        }

        internal Panel SymbolToolPanel
        {
            get { return _symbolToolPanel; }
        }

        // UI value setters/getters
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

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void SetPropertyField<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyUpdate(string? changedPropertyName)
        {
            if (MapStateMediator.CurrentMap != null)
            {
                UpdateSymbolUI(changedPropertyName);
                SymbolManager.Update(MapStateMediator.CurrentMap, MapState, this);
            }
        }

        private void UpdateSymbolUI(string? changedPropertyName)
        {
            // this methods updates the Main Form Symbol Tab UI
            // based on the property that has changed
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
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                if (UseAreaBrush)
                {
                    MapStateMediator.MainUIMediator.SelectedBrushSize = AreaBrushSize;
                }
                else
                {
                    AreaBrushSize = 0;
                    MapStateMediator.MainUIMediator.SelectedBrushSize = 0;
                }
            }));
        }

        #endregion

        #region EventHandlers

        private void MapSymbolUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }

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

        #endregion

        #region Map Symbol UI Methods

        internal void ColorButtonMouseUp(object sender, MouseEventArgs e)
        {
            IconButton colorButton = (IconButton)sender;

            if (e.Button == MouseButtons.Left)
            {
                Color c = UtilityMethods.SelectColorFromDialog(MainForm, colorButton.BackColor);
                SymbolColor1 = c;

                List<MapSymbol> selectedSymbols = GetFilteredMapSymbols();
                AddSymbolsToSymbolTable(selectedSymbols);
            }
            else if (e.Button == MouseButtons.Right && MapStateMediator.SelectedMapSymbol != null)
            {
                if (MapStateMediator.SelectedMapSymbol.IsGrayscale || MapStateMediator.SelectedMapSymbol.UseCustomColors)
                {
                    // if a symbol has been selected and is grayscale or custom colored, then color it with the
                    // selected custom colors
                    Cmd_PaintSymbol cmd = new(MapStateMediator.SelectedMapSymbol,
                        SymbolColor1.ToSKColor(), SymbolColor1.ToSKColor(), SymbolColor2.ToSKColor(), SymbolColor2.ToSKColor());

                    CommandManager.AddCommand(cmd);
                    cmd.DoOperation();
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
            if (MapStateMediator.SelectedMapSymbol != null && MapStateMediator.SelectedMapSymbol.IsSelected)
            {
                SymbolManager.ColorSelectedSymbol(MapStateMediator.SelectedMapSymbol);
            }
            else
            {
                if (UseAreaBrush)
                {
                    MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.SymbolColor, MainForm.AreaBrushSizeTrack.Value);
                    AreaBrushSize = MapStateMediator.MainUIMediator.SelectedBrushSize;
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

        #endregion
    }
}
