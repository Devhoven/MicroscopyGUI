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
using System.Windows.Data;
using System.Globalization;

namespace MicroscopeGUI.Helper
{
    static class Extensions
    {
        static WriteableBitmap ImgSource = new WriteableBitmap(1280, 1028, 100, 100, PixelFormats.Bgr24, BitmapPalettes.WebPalette);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        public static WriteableBitmap GetWriteableBitmap(this Bitmap BMP)
        {
            // So the program will still run with every cam / resolution
            if (BMP.Width != ImgSource.PixelWidth || BMP.Height != ImgSource.PixelHeight)
                ImgSource = new WriteableBitmap(BMP.Width, BMP.Height, 100, 100, PixelFormats.Bgr24, BitmapPalettes.WebPalette);

            BitmapData BMPData = BMP.LockBits(
                new Rectangle(0, 0, BMP.Width, BMP.Height),
                ImageLockMode.ReadOnly, BMP.PixelFormat);

            ImgSource.Lock();

            CopyMemory(ImgSource.BackBuffer, BMPData.Scan0, BMPData.Stride * BMPData.Height);

            ImgSource.AddDirtyRect(new Int32Rect(0, 0, BMP.Width, BMP.Height));
            ImgSource.Unlock();

            BMP.UnlockBits(BMPData);
            BMP.Dispose();

            return ImgSource;
        }
    }
}
