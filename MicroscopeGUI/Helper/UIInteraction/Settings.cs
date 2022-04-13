using System.Windows.Media;

namespace MicroscopeGUI
{
    static class Settings
    {
        public static int LineThickness = 3;

        static SolidColorBrush _LineColor;
        public static SolidColorBrush LineColor
        {
            get => _LineColor;
            set
            {
                _LineColor = value;
                RegistryManager.SetValue("LineColor", _LineColor.Color.ToHexString());
            }
        }

        public static SolidColorBrush _LineTextColor = Brushes.Black;
        public static SolidColorBrush LineTextColor
        {
            get => _LineTextColor;
            set
            {
                _LineTextColor = value;
                RegistryManager.SetValue("LineTextColor", _LineTextColor.Color.ToHexString());
            }
        }

        public static string ImgGalleryPath = "";

        // Converts a color to a hex string
        static string ToHexString(this Color c) 
            => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        static Settings()
        {
            try
            {
                LineColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RegistryManager.GetStrVal("LineColor")));
                LineTextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RegistryManager.GetStrVal("LineTextColor")));
            }
            // They will fail at the first start
            catch 
            {
                LineColor = Brushes.Red;
                LineTextColor = Brushes.Black;
            }
        }
    }
}
