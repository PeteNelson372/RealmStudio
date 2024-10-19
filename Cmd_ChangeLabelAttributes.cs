using SkiaSharp;

namespace RealmStudio
{
    internal class Cmd_ChangeLabelAttributes : IMapOperation
    {
        private readonly RealmStudioMap Map;
        private readonly MapLabel Label;
        private readonly Color LabelColor;
        private readonly Color OutlineColor;
        private readonly float OutlineWidth;
        private readonly Color GlowColor;
        private readonly int GlowStrength;
        private readonly Font SelectedFont;

        private readonly Color StoredLabelColor;
        private readonly Color StoredOutlineColor;
        private readonly float StoredOutlineWidth;
        private readonly Color StoredGlowColor;
        private readonly int StoredGlowStrength;
        private readonly Font StoredSelectedFont;


        public Cmd_ChangeLabelAttributes(RealmStudioMap map, MapLabel label, Color labelColor, Color outlineColor, float outlineWidth, Color glowColor, int glowStrength, Font selectedFont)
        {
            Map = map;
            Label = label;
            LabelColor = labelColor;
            OutlineColor = outlineColor;
            OutlineWidth = outlineWidth;
            GlowColor = glowColor;
            GlowStrength = glowStrength;
            SelectedFont = selectedFont;

            StoredLabelColor = Label.LabelColor;
            StoredOutlineColor = Label.LabelOutlineColor;
            StoredOutlineWidth = Label.LabelOutlineWidth;
            StoredGlowColor = Label.LabelGlowColor;
            StoredGlowStrength = Label.LabelGlowStrength;
            StoredSelectedFont = Label.LabelFont;
        }

        public void DoOperation()
        {
            Label.LabelColor = LabelColor;
            Label.LabelOutlineColor = OutlineColor;
            Label.LabelOutlineWidth = OutlineWidth;
            Label.LabelGlowColor = GlowColor;
            Label.LabelGlowStrength = GlowStrength;
            Label.LabelFont = SelectedFont;

            SKPaint paint = MapLabelMethods.CreateLabelPaint(Label.LabelFont, LabelColor, LabelTextAlignEnum.AlignLeft);

            Label.LabelPaint = paint;
            SKRect bounds = new();
            Label.Width = (int)paint.MeasureText(Label.LabelText, ref bounds);
            Label.Height = (int)bounds.Height;

            MapBuilder.SetLayerModified(Map, MapBuilder.LABELLAYER, true);
        }

        public void UndoOperation()
        {
            Label.LabelColor = StoredLabelColor;
            Label.LabelOutlineColor = StoredOutlineColor;
            Label.LabelOutlineWidth = StoredOutlineWidth;
            Label.LabelGlowColor = StoredGlowColor;
            Label.LabelGlowStrength = StoredGlowStrength;
            Label.LabelFont = StoredSelectedFont;

            SKPaint paint = MapLabelMethods.CreateLabelPaint(Label.LabelFont, LabelColor, LabelTextAlignEnum.AlignLeft);

            Label.LabelPaint = paint;
            SKRect bounds = new();
            Label.Width = (int)paint.MeasureText(Label.LabelText, ref bounds);
            Label.Height = (int)bounds.Height;

            MapBuilder.SetLayerModified(Map, MapBuilder.LABELLAYER, true);
        }
    }
}
