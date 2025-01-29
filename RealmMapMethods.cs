using SkiaSharp;

namespace RealmStudio
{
    internal class RealmMapMethods
    {
        internal static RealmStudioMap? CreateDetailMap(RealmStudioMainForm mainForm, RealmStudioMap currentMap, SKRect selectedArea)
        {
            if (selectedArea.IsEmpty)
            {
                MessageBox.Show("Please use the Area button on the Land tab to select an area for the detail map.", "Select a Map Area");
            }
            else
            {
                DetailMapForm detailMapForm = new(mainForm, currentMap, selectedArea);
                DialogResult result = detailMapForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return detailMapForm.detailMap;
                }

                return null;
            }

            return null;

        }
    }
}
