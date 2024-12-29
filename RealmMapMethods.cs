using SkiaSharp;

namespace RealmStudio
{
    internal class RealmMapMethods
    {
        internal static void CreateDetailMap(RealmStudioMap currentMap, SKRect selectedArea)
        {
            if (selectedArea.IsEmpty)
            {
                MessageBox.Show("Please use the Area button on the Land tab to select an area for the detail map.", "Select a Map Area");
            }
            else
            {
                DetailMapForm detailMapForm = new(currentMap, selectedArea);
                DialogResult result = detailMapForm.ShowDialog();
            }


        }
    }
}
