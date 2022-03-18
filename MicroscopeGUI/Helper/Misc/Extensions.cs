using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using Size = System.Windows.Size;
using System.Drawing.Drawing2D;
using Image = System.Drawing.Image;
using Matrix = System.Windows.Media.Matrix;

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

        // Stolen from https://stackoverflow.com/a/16362489/9241163
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        public static Bitmap KernellDllCopyBitmap(this Bitmap bmp, bool CopyPalette = true)
        {
            Bitmap bmpDest = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);

            if (!KernellDllCopyBitmap(bmp, bmpDest, CopyPalette))
                bmpDest = null;

            return bmpDest;
        }

        static bool KernellDllCopyBitmap(Bitmap bmpSrc, Bitmap bmpDest, bool CopyPalette = false)
        {
            bool copyOk = CheckCompatibility(bmpSrc, bmpDest);
            if (copyOk)
            {
                BitmapData bmpDataSrc;
                BitmapData bmpDataDest;

                //Lock Bitmap to get BitmapData
                bmpDataSrc = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), ImageLockMode.ReadOnly, bmpSrc.PixelFormat);
                bmpDataDest = bmpDest.LockBits(new Rectangle(0, 0, bmpDest.Width, bmpDest.Height), ImageLockMode.WriteOnly, bmpDest.PixelFormat);
                int lenght = bmpDataSrc.Stride * bmpDataSrc.Height;

                CopyMemory(bmpDataDest.Scan0, bmpDataSrc.Scan0, (uint)lenght);

                bmpSrc.UnlockBits(bmpDataSrc);
                bmpDest.UnlockBits(bmpDataDest);

                if (CopyPalette && bmpSrc.Palette.Entries.Length > 0)
                    bmpDest.Palette = bmpSrc.Palette;
            }
            return copyOk;
        }

        static bool CheckCompatibility(Bitmap bmp1, Bitmap bmp2)
            => ((bmp1.Width == bmp2.Width) && (bmp1.Height == bmp2.Height) && (bmp1.PixelFormat == bmp2.PixelFormat));


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

        // This is from https://www.codeproject.com/Articles/191424/Resizing-an-Image-On-The-Fly-using-NET
        public static Image Resize(this Image image, int Width, int Height)
        {
            int newWidth;
            int newHeight;
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            float percentWidth = Width / (float)originalWidth;
            float percentHeight = Height / (float)originalHeight;
            float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
            newWidth = (int)(originalWidth * percent);
            newHeight = (int)(originalHeight * percent);
            System.Drawing.Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
    }
}
