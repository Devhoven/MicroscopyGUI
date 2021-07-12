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

namespace MicroscopeGUI
{
    static class ImageQueue
    {
        // Is set to true at the end of the program, so the thread won't be running in the background
        public static volatile bool StopRunning = false;
        // Always holds the newest frame of the camera
        public static Bitmap CurrentBitmap = new Bitmap("C:/Users/Vincent/Desktop/SEMImages/CPUBruch.png");
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
                //Status StatusRet = GUI.Cam.Memory.Sequence.WaitForNextImage(100, out int MemID, out _);
                //if (StatusRet == Status.Success)
                //{
                //    // Getting the values of the histogram
                //    GUI.Cam.Image.GetHistogram(MemID, ColorMode.BGR8Packed, out Histogram);

                //    // Conversion to a bitmap
                //    GUI.Cam.Memory.ToIntPtr(MemID, out _);

                //    GUI.Cam.Memory.Lock(MemID);
                //    GUI.Cam.Memory.GetSize(MemID, out _, out _);

                //    GUI.Cam.Memory.ToBitmap(MemID, out CurrentBitmap);

                //    // Unlocking the image buffer
                //    GUI.Cam.Memory.Unlock(MemID);

                //    //Setting the current image on the picture panel
                //    //GUI.Display.Image = CurrentBitmap;

                //    OnFrameChange(null, null);
                //}
            }

        }
    }
}
