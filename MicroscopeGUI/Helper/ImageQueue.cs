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
        // Fires if the frame has changed
        public static event EventHandler OnFrameChange = delegate { };
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
                    
                    // Checking if the main Thread is still running
                    if (!StopRunning && !UI.CurrentDispatcher.HasShutdownStarted)
                    {
                        // Setting the current image on the picture panel
                        // Calling the frame change event
                        UI.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                            new Action(() => {
                                UI.CurrentFrame.Source = Convert(bmp);
                                OnFrameChange(null, null);
                            }));
                    }

                    // Unlocking the image buffer
                    UI.Cam.Memory.Unlock(MemID);
                }
            }
        }

        static BitmapSource Convert(Bitmap BMP)
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

            return BMPSource;
        }
    }
}
