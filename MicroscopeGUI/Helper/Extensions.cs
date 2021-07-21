using System;
using uEye.Defines;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ColorMode = uEye.Defines.ColorMode;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;


namespace MicroscopeGUI.Helper
{
    static class Extensions
    {
        public static BitmapSource ConvertToBitmapSource(this Bitmap BMP)
        {
            BitmapData BMPData = BMP.LockBits(
                new Rectangle(0, 0, BMP.Width, BMP.Height),
                ImageLockMode.ReadOnly, BMP.PixelFormat);

            BitmapSource BMPSource = BitmapSource.Create(
                BMPData.Width, BMPData.Height,
                BMP.HorizontalResolution, BMP.VerticalResolution,
                PixelFormats.Bgr24, null,
                BMPData.Scan0, BMPData.Stride * BMPData.Height, BMPData.Stride);

            BMP.UnlockBits(BMPData);

            BMP.Dispose();

            return BMPSource;
        }
    }
}
