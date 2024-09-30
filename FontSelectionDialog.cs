using System.Data;
using System.Drawing.Text;

namespace RealmStudio
{
    public partial class FontSelectionDialog : Form
    {
        public event EventHandler? FontSelected;

        private bool isBold = false;
        private bool isItalic = false;

        public Font? SelectedFont = null;

        private readonly Form? FormParentForm;
        private readonly Font? InitialFont;

        public FontSelectionDialog(Form? parent, Font? initialFont)
        {
            InitializeComponent();

            FormParentForm = parent;

            if (FormParentForm != null)
            {
                Location = new Point(FormParentForm.Location.X + 274, FormParentForm.Location.Y + 114);
            }

            InitialFont = initialFont;

            PopulateUI();
        }

        protected virtual void OnFontSelected(EventArgs e)
        {
            FontSelected?.Invoke(this, e);
        }

        private void PopulateUI()
        {
            InstalledFontCollection installedFontCollection = new();

            FontFamilyCombo.DrawItem += FontFamilyCombo_DrawItem;
            FontFamilyCombo.DisplayMember = "Name";

            // Get the array of FontFamily objects.
            foreach (var t in installedFontCollection.Families.Where(t => t.IsStyleAvailable(FontStyle.Regular)))
            {
                FontFamilyCombo.Items.Add(new Font(t, 12));
            }

            int fontIndex = 0;

            if (InitialFont != null)
            {
                fontIndex = FontFamilyCombo.Items.IndexOf(InitialFont);

                if (FontFamilyCombo.Items != null && fontIndex < 0 && InitialFont != null)
                {
                    // find by name
                    for (int i = 0; i < FontFamilyCombo.Items?.Count; i++)
                    {
                        Font? f = FontFamilyCombo.Items[i] as Font;

                        string? fontName = f?.Name;

                        if (!string.IsNullOrEmpty(fontName) && fontName == InitialFont.Name)
                        {
                            fontIndex = i;
                            break;
                        }
                    }
                }
            }

            FontFamilyCombo.SelectedIndex = (fontIndex >= 0) ? fontIndex : 0;

            FontSizeCombo.SelectedIndex = 7; // 12 points

            SelectedFont = Font;

            SetFont();
            SetExampleText();
        }

        private void FontFamilyCombo_DrawItem(object? sender, DrawItemEventArgs e)
        {
            ComboBox? comboBox = (ComboBox?)sender;

            if (comboBox != null)
            {
                Font? font = (Font?)comboBox.Items[e.Index];

                if (font != null)
                {
                    e.DrawBackground();
                    e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
                }
            }
        }

        private void FontFamilyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FontFamilyCombo.SelectedIndex >= 0)
            {
                Font? selectedFont = (Font?)FontFamilyCombo.Items[FontFamilyCombo.SelectedIndex];

                if (selectedFont != null)
                {
                    FontFamilyCombo.Text = selectedFont.Name;

                    SetFont();
                    SetExampleText();
                }
            }
        }

        private void SetExampleText()
        {
            ExampleTextLabel.Font = SelectedFont;
            ExampleTextLabel.Text = "The quick brown fox";
        }

        private void SetFont()
        {
            Font? selectedFont = (Font?)FontFamilyCombo.Items[FontFamilyCombo.SelectedIndex];

            if (selectedFont != null)
            {
                FontFamily ff = selectedFont.FontFamily;

                if (FontSizeCombo.SelectedIndex >= 0)
                {
                    string? selectedFontSize = (string?)FontSizeCombo.Items[FontSizeCombo.SelectedIndex];

                    if (float.TryParse(selectedFontSize, out float fontSize))
                    {
                        fontSize *= 1.33F;

                        FontStyle fs = FontStyle.Regular;

                        if (isBold)
                        {
                            fs = fs | FontStyle.Bold;
                        }

                        if (isItalic)
                        {
                            fs = fs | FontStyle.Italic;
                        }

                        try
                        {
                            SelectedFont = new Font(ff, fontSize, fs, GraphicsUnit.Point);
                        }
                        catch { }
                    }
                }
            }

            OnFontSelected(EventArgs.Empty);
        }

        private void BoldFontButton_Click(object sender, EventArgs e)
        {
            isBold = !isBold;

            if (isBold)
            {
                BoldFontButton.BackColor = ColorTranslator.FromHtml("#D2F1C1");
            }
            else
            {
                BoldFontButton.BackColor = Color.White;
            }

            SetFont();
            SetExampleText();
        }

        private void ItalicFontButton_Click(object sender, EventArgs e)
        {
            isItalic = !isItalic;

            if (isItalic)
            {
                ItalicFontButton.BackColor = ColorTranslator.FromHtml("#D2F1C1");
            }
            else
            {
                ItalicFontButton.BackColor = Color.White;
            }

            SetFont();
            SetExampleText();
        }

        private void FontSizeCombo_TextChanged(object sender, EventArgs e)
        {
            SetFont();
            SetExampleText();
        }

        private void BigFontButton_Click(object sender, EventArgs e)
        {
            int sizeIndex = FontSizeCombo.SelectedIndex;

            if (sizeIndex < FontSizeCombo.Items.Count - 1)
            {
                sizeIndex++;
                FontSizeCombo.SelectedIndex = sizeIndex;
            }
        }

        private void SmallFontButton_Click(object sender, EventArgs e)
        {
            int sizeIndex = FontSizeCombo.SelectedIndex;

            if (sizeIndex > 0)
            {
                sizeIndex--;
                FontSizeCombo.SelectedIndex = sizeIndex;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            // set selected font
            OnFontSelected(EventArgs.Empty);
        }

        private void CloseFormButton_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
