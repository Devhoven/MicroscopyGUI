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
using Size = System.Windows.Size;
using System.Windows.Controls;

namespace MicroscopeGUI
{
    public static class Extensions
    {
        static WriteableBitmap FrameSource = new WriteableBitmap(1280, 1028, 100, 100, PixelFormats.Bgr24, BitmapPalettes.WebPalette);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        // Copies the memory from one location another
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        public static WriteableBitmap GetWriteableBitmap(this Bitmap BMP)
        {
            // So the program will still run with every cam / resolution
            if (BMP.Width != FrameSource.PixelWidth || BMP.Height != FrameSource.PixelHeight)
                FrameSource = new WriteableBitmap(BMP.Width, BMP.Height, 100, 100, PixelFormats.Bgr24, BitmapPalettes.WebPalette);

            BitmapData BMPData = BMP.LockBits(
                new Rectangle(0, 0, BMP.Width, BMP.Height),
                ImageLockMode.ReadOnly, BMP.PixelFormat);

            FrameSource.Lock();
            
            CopyMemory(FrameSource.BackBuffer, BMPData.Scan0, BMPData.Stride * BMPData.Height);

            FrameSource.AddDirtyRect(new Int32Rect(0, 0, BMP.Width, BMP.Height));
            FrameSource.Unlock();

            BMP.UnlockBits(BMPData);
            BMP.Dispose();

            return FrameSource;
        }

        // Returns the size in pixels of a given UI Element
        // Stolen from: https://stackoverflow.com/a/3450426/9241163
        public static Size GetElementPixelSize(this UIElement element)
        {
            Matrix transformToDevice;
            var source = PresentationSource.FromVisual(element);
            if (source != null)
                transformToDevice = source.CompositionTarget.TransformToDevice;
            else
                using (var src = new HwndSource(new HwndSourceParameters()))
                    transformToDevice = src.CompositionTarget.TransformToDevice;

            if (element.DesiredSize == new Size())
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return (Size)transformToDevice.Transform((Vector)element.DesiredSize);
        }
    }
}
