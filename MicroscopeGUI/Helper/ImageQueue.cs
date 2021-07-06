using System;
using uEye.Defines;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace MicroscopeGUI
{
    static class ImageQueue
    {
        // Is set to true at the end of the program, so the thread won't be running in the background
        public static volatile bool StopRunning = false;
        // Always holds the newest frame of the camera
        public static Bitmap CurrentBitmap;
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
                Status StatusRet = GUI.Camera.Memory.Sequence.WaitForNextImage(100, out int MemID, out _);
                if (StatusRet == Status.Success)
                {
                    // Getting the values of the histogram
                    GUI.Camera.Image.GetHistogram(MemID, ColorMode.BGR8Packed, out Histogram);

                    // Conversion to a bitmap
                    GUI.Camera.Memory.ToIntPtr(MemID, out _);

                    GUI.Camera.Memory.Lock(MemID);
                    GUI.Camera.Memory.GetSize(MemID, out _, out _);

                    GUI.Camera.Memory.ToBitmap(MemID, out CurrentBitmap);

                    // Unlocking the image buffer
                    GUI.Camera.Memory.Unlock(MemID);

                    // Setting the current image on the picture panel
                    GUI.Display.Image = CurrentBitmap;

                    OnFrameChange(null, null);
                }
            }
        }
    }
}
