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
using SkiaSharp;
using System.ComponentModel;
using System.Reflection;

namespace RealmStudio
{
    internal sealed class LabelUIMediator : IUIMediatorObserver, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly RealmStudioMainForm MainForm;
        private MapStateMediator? _mapState;

        bool _enabled = true;
        bool _creatingLabel;
        float _labelRotation;
        float _glowStrength;
        Color _glowColor = Color.White;
        Color _outlineColor = Color.FromArgb(161, 214, 202, 171);
        float _outlineWidth;
        Color _labelColor = Color.FromArgb(61, 53, 30);
        Font _selectedLabelFont = MapStateMediator.DefaultLabelFont;


        public LabelUIMediator(RealmStudioMainForm mainForm)
        {
            MainForm = mainForm;
            PropertyChanged += LabelUIMediator_PropertyChanged;
        }

        #region Property Setters/Getters
        public MapStateMediator? MapState
        {
            get { return _mapState; }
            set { _mapState = value; }
        }

        internal bool Enabled
        {
            get { return _enabled; }
            set { SetPropertyField(nameof(Enabled), ref _enabled, value); }
        }
        internal float LabelRotation
        {
            get { return _labelRotation; }
            set { SetPropertyField(nameof(LabelRotation), ref _labelRotation, value); }
        }

        internal float GlowStrength
        {
            get { return _glowStrength; }
            set { SetPropertyField(nameof(GlowStrength), ref _glowStrength, value); }
        }

        internal Color GlowColor
        {
            get { return _glowColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(GlowColor), ref _glowColor, value);
                }
            }
        }

        internal Font SelectedLabelFont
        {
            get { return _selectedLabelFont; }
            set { SetPropertyField(nameof(SelectedLabelFont), ref _selectedLabelFont, value); }
        }

        internal Color OutlineColor
        {
            get { return _outlineColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(OutlineColor), ref _outlineColor, value);
                }
            }
        }

        internal float OutlineWidth
        {
            get { return _outlineWidth; }
            set { SetPropertyField(nameof(OutlineWidth), ref _outlineWidth, value); }
        }

        internal Color LabelColor
        {
            get { return _labelColor; }
            set
            {
                if (value.ToArgb() != Color.Empty.ToArgb())
                {
                    SetPropertyField(nameof(LabelColor), ref _labelColor, value);
                }
            }
        }

        internal bool CreatingLabel
        {
            get { return _creatingLabel; }
            set { _creatingLabel = value; }
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
            UpdateFrameUI(changedPropertyName);
            FrameManager.Update();
            MainForm.SKGLRenderControl.Invalidate();
        }

        private void UpdateFrameUI(string? changedPropertyName)
        {
            MainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate ()
            {
                MainForm.GlowColorSelectButton.BackColor = GlowColor;
                MainForm.GlowColorSelectButton.Refresh();

                MainForm.OutlineColorSelectButton.BackColor = OutlineColor;
                MainForm.OutlineColorSelectButton.Refresh();

                MainForm.FontColorSelectButton.BackColor = LabelColor;
                MainForm.FontColorSelectButton.Refresh();

                MainForm.SelectLabelFontButton.Font = new Font(SelectedLabelFont.FontFamily, 14);
                MainForm.SelectLabelFontButton.Refresh();

                if (!string.IsNullOrEmpty(changedPropertyName))
                {
                    switch (changedPropertyName)
                    {
                        case "LabelRotation":
                            {
                                MainForm.LabelRotationUpDown.Value = (decimal)LabelRotation;
                                MainForm.LabelRotationTrack.Value = (int)LabelRotation;

                                MainForm.LabelRotationTrack.Refresh();
                                MainForm.LabelRotationUpDown.Refresh();
                            }
                            break;
                        case "Enabled":
                            {
                                EnableDisabledLabelAndBoxLayer();
                            }
                            break;
                        case "OutlineWidth":
                            {
                                MainForm.OutlineWidthTrack.Value = (int)OutlineWidth * 10;
                                MainForm.OutlineWidthTrack.Refresh();
                            }
                            break;
                    }
                }

            }));
        }

        #endregion

        #region Event Handlers
        private void LabelUIMediator_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // this event handler is called whenever a property is set
            // using the SetPropertyField method

            // *** Properties that are not set using the SetPropertyField method will not trigger a PropertyChanged event *** //

            NotifyUpdate(e.PropertyName);
        }


        internal void LabelTextBox_KeyPress(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            if (sender != null)
            {
                MapLayer labelLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LABELLAYER);

                TextBox tb = (TextBox)sender;

                Font labelFont = tb.Font;
                Color labelColor = LabelColor;
                Color outlineColor = OutlineColor;
                float outlineWidth = OutlineWidth;

                Color glowColor = GlowColor;
                int glowStrength = (int)GlowStrength;

                int labelRotationDegrees = (int)LabelRotation;

                if (((KeyPressEventArgs)e).KeyChar == (char)Keys.Escape)
                {
                    ((KeyPressEventArgs)e).Handled = false; // pass the event up

                    CreatingLabel = false;

                    // dispose of the text box, as it isn't needed once the label text has been entered
                    MainForm.SKGLRenderControl.Controls.Remove(tb);
                    tb.Dispose();
                }
                else if (((KeyPressEventArgs)e).KeyChar == (char)Keys.Return)
                {
                    ((KeyPressEventArgs)e).Handled = true;
                    CreatingLabel = false;

                    if (MapStateMediator.SelectedMapLabel != null)
                    {
                        labelFont = MapStateMediator.SelectedMapLabel.LabelFont;
                        labelColor = MapStateMediator.SelectedMapLabel.LabelColor;
                        outlineColor = MapStateMediator.SelectedMapLabel.LabelOutlineColor;
                        outlineWidth = MapStateMediator.SelectedMapLabel.LabelOutlineWidth;
                        glowColor = MapStateMediator.SelectedMapLabel.LabelGlowColor;
                        glowStrength = MapStateMediator.SelectedMapLabel.LabelGlowStrength;
                        labelRotationDegrees = (int)MapStateMediator.SelectedMapLabel.LabelRotationDegrees;
                    }

                    if (!string.IsNullOrEmpty(tb.Text))
                    {
                        // create a new MapLabel object and render it
                        MapLabel label = new()
                        {
                            LabelText = tb.Text,
                            LabelFont = labelFont,
                            IsSelected = true,
                            LabelColor = labelColor,
                            LabelOutlineColor = outlineColor,
                            LabelOutlineWidth = outlineWidth,
                            LabelGlowColor = glowColor,
                            LabelGlowStrength = glowStrength,
                            LabelRotationDegrees = labelRotationDegrees,
                        };

                        SKFont skLabelFont = LabelManager.GetSkLabelFont(labelFont);
                        SKPaint paint = LabelManager.CreateLabelPaint(labelColor);

                        label.LabelPaint = paint;
                        label.LabelSKFont.Dispose();
                        label.LabelSKFont = skLabelFont;

                        label.LabelSKFont.MeasureText(label.LabelText, out SKRect bounds, label.LabelPaint);

                        float descent = labelFont.FontFamily.GetCellDescent(labelFont.Style);
                        float descentPixel =
                            labelFont.Size * descent / labelFont.FontFamily.GetEmHeight(FontStyle.Regular);

                        // TODO: drawing zoom has to be taken into account?
                        float xDiff = (tb.Width - bounds.Width) / 2;
                        float yDiff = ((tb.Height - bounds.Height) / 2) + descentPixel / 2;

                        SKPoint zoomedScrolledPoint = new(((tb.Left + xDiff) / MapStateMediator.MainUIMediator.DrawingZoom) + MapStateMediator.DrawingPoint.X,
                            ((tb.Top + yDiff) / MapStateMediator.MainUIMediator.DrawingZoom) + MapStateMediator.DrawingPoint.Y);

                        label.X = (int)zoomedScrolledPoint.X;
                        label.Y = (int)zoomedScrolledPoint.Y;

                        if (MapStateMediator.SelectedMapLabel != null)
                        {
                            label.X = MapStateMediator.SelectedMapLabel.X;
                            label.Y = MapStateMediator.SelectedMapLabel.Y;
                        }

                        label.Width = (int)bounds.Width;
                        label.Height = (int)bounds.Height;

                        if (tb.Tag != null && tb.Tag is SKPath path)
                        {
                            label.LabelPath = path;
                        }
                        else if (LabelManager.CurrentMapLabelPath?.PointCount > 0)
                        {
                            label.LabelPath = new(LabelManager.CurrentMapLabelPath);

                            LabelManager.CurrentMapLabelPath.Dispose();
                            LabelManager.CurrentMapLabelPath = new();

                            MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.WORKLAYER).LayerSurface?.Canvas.Clear(SKColors.Transparent);
                        }

                        Cmd_AddLabel cmd = new(MapStateMediator.CurrentMap, label);
                        CommandManager.AddCommand(cmd);
                        cmd.DoOperation();

                        RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, label);

                        MapStateMediator.SelectedMapLabel = (MapLabel?)labelLayer.MapLayerComponents.Last();

                        MapStateMediator.CurrentMap.IsSaved = false;
                    }

                    MapStateMediator.MainUIMediator.SetDrawingMode(MapDrawingMode.LabelSelect, 0);

                    // dispose of the text box, as it isn't needed once the label text has been entered
                    MainForm.SKGLRenderControl.Controls.Remove(tb);
                    tb.Dispose();

                    MainForm.SKGLRenderControl.Refresh();
                }
                else
                {
                    if (tb.Text.StartsWith("...Label..."))
                    {
                        tb.Text = tb.Text["...Label...".Length..];
                    }

                    SKFontStyle fs = SKFontStyle.Normal;

                    if (labelFont.Bold && labelFont.Italic)
                    {
                        fs = SKFontStyle.BoldItalic;
                    }
                    else if (labelFont.Bold)
                    {
                        fs = SKFontStyle.Bold;
                    }
                    else if (labelFont.Italic)
                    {
                        fs = SKFontStyle.Italic;
                    }

                    List<string> resourceNames = [.. Assembly.GetExecutingAssembly().GetManifestResourceNames()];

                    SKTypeface? fontTypeface = null;

                    foreach (string resourceName in resourceNames)
                    {
                        if (resourceName.Contains(labelFont.FontFamily.Name))
                        {
                            fontTypeface = SKTypeface.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
                            break;
                        }
                    }

                    fontTypeface ??= SKTypeface.FromFamilyName(labelFont.FontFamily.Name, fs);

                    SKFont paintFont = new(fontTypeface, labelFont.SizeInPoints, 1, 0);
                    SKPaint labelPaint = LabelManager.CreateLabelPaint(labelColor);

                    float lblWidth = paintFont.MeasureText(tb.Text, labelPaint);
                    int tbWidth = (int)Math.Max(lblWidth, tb.Width);
                    tb.Width = tbWidth;

                    MainForm.SKGLRenderControl.Refresh();
                }
            }
        }
        #endregion

        #region Label UI Methods
        private void EnableDisabledLabelAndBoxLayer()
        {
            MapLayer labellLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.LABELLAYER);
            labellLayer.ShowLayer = Enabled;

            MapLayer boxLayer = MapBuilder.GetMapLayerByIndex(MapStateMediator.CurrentMap, MapBuilder.BOXLAYER);
            boxLayer.ShowLayer = Enabled;
        }

        internal static MapLabel? SelectLabelAtPoint(RealmStudioMap map, SKPoint zoomedScrolledPoint)
        {
            MapLabel? selectedLabel = null;

            List<MapComponent> mapLabelComponents = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LABELLAYER).MapLayerComponents;

            for (int i = 0; i < mapLabelComponents.Count; i++)
            {
                if (mapLabelComponents[i] is MapLabel mapLabel)
                {
                    SKRect labelRect = new(mapLabel.X, mapLabel.Y, mapLabel.X + mapLabel.Width, mapLabel.Y + mapLabel.Height);

                    if (labelRect.Contains(zoomedScrolledPoint))
                    {
                        selectedLabel = mapLabel;
                    }
                }
            }

            RealmMapMethods.DeselectAllMapComponents(MapStateMediator.CurrentMap, selectedLabel);

            return selectedLabel;
        }

        internal void SelectLabelOrBox()
        {
            MapLabel? selectedLabel = SelectLabelAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);

            if (selectedLabel != null)
            {
                bool isSelected = selectedLabel.IsSelected;

                selectedLabel.IsSelected = !isSelected;

                if (selectedLabel.IsSelected)
                {
                    MapStateMediator.SelectedMapLabel = selectedLabel;
                    LabelRotation = (int)MapStateMediator.SelectedMapLabel.LabelRotationDegrees;
                }
                else
                {
                    MapStateMediator.SelectedMapLabel = null;
                }

                MainForm.SKGLRenderControl.Invalidate();
            }
            else
            {
                MapStateMediator.SelectedMapLabel = null;

                BoxManager.SelectMapBoxAtPoint(MapStateMediator.CurrentMap, MapStateMediator.CurrentCursorPoint);
                MainForm.SKGLRenderControl.Invalidate();
            }
        }

        internal void RemoveTextBox()
        {
            if (LabelManager.LabelTextBox != null)
            {
                MainForm.SKGLRenderControl.Controls.Remove(LabelManager.LabelTextBox);
                LabelManager.LabelTextBox.Dispose();
            }
        }

        #endregion
    }
}
