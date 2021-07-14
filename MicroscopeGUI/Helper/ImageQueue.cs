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

namespace MicroscopeGUI
{
    static class ImageQueue
    {
        // Is set to true at the end of the program, so the thread won't be running in the background
        public static volatile bool StopRunning = false;
        // Always holds the newest frame of the camera
        static byte[] CurrentFrameData = new byte[1280 * 1024 * 3];
        // Fires if the frame has changed
        public static event EventHandler OnFrameChange;
        // Contains all of the values of the histogram of the current image
        // 3 * 256, since there is a value for every 8 bit value of one channel
        public static uint[] Histogram = new uint[3 * 256];

        public static void Run()
        {
            while (!StopRunning)
            {
                // Waits for the next image and returns the memory ID if a new image was sent by the cam
                Status StatusRet = UI.Cam.Memory.Sequence.WaitForNextImage(100, out int MemID, out _);
                if (StatusRet == Status.Success)
                {
                    // Getting the values of the histogram
                    UI.Cam.Image.GetHistogram(MemID, ColorMode.BGR8Packed, out Histogram);

                    // Conversion to a bitmap
                    UI.Cam.Memory.Lock(MemID);

                    UI.Cam.Memory.ToBitmap(MemID, out Bitmap bmp);

                    //Setting the current image on the picture panel
                    UI.CurrentDispatcher.Invoke(() => UI.CurrentFrame.Source = Convert(bmp));

                    // Unlocking the image buffer
                    UI.Cam.Memory.Unlock(MemID);

                    OnFrameChange(null, null);
                }
            }
        }

        static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }
    }
}
