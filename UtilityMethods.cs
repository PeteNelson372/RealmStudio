namespace RealmStudio
{
    internal class UtilityMethods
    {
        public static Color SelectColorFromDialog(Form owner, Color initialColor)
        {
            // color selector
            using ColorSelector cs = new()
            {
                Owner = owner,
                SelectedColor = initialColor
            };

            if (cs.ShowDialog(owner) == DialogResult.OK)
            {
                return cs.SelectedColor;
            }
            else
            {
                return Color.Empty;
            }
        }
    }
}
