namespace RealmStudio
{
    internal class LandformMethods
    {
        public static Color LandformOutlineColor { get; set; } = ColorTranslator.FromHtml("#3D3728");
        public static int LandformOutlineWidth { get; set; } = 2;
        public static GradientDirectionEnum ShorelineStyle { get; set; } = GradientDirectionEnum.None;
        public static Color CoastlineColor { get; set; } = ColorTranslator.FromHtml("#BB9CC3B7");
        public static int CoastlineEffectDistance { get; set; } = 16;
        public static string CoastlineStyleName { get; set; } = "Dash Pattern";
        public static string? CoastlineHatchPattern { get; set; } = string.Empty;
        public static int CoastlineHatchOpacity { get; set; } = 0;
        public static int CoastlineHatchScale { get; set; } = 0;
        public static string? CoastlineHatchBlendMode { get; set; } = string.Empty;
    }
}
