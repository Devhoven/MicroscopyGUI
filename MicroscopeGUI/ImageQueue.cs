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
        public static volatile bool StopRunning = false;
        public static Bitmap CurrentBitmap;

        public static void Run()
        {
            uint Timeout = 100;
            while (!StopRunning)
            {
                Status StatusRet = GUI.Camera.Memory.Sequence.WaitForNextImage(Timeout, out int MemID, out _);
                if (StatusRet == Status.Success)
                {
                    GUI.Camera.Memory.ToIntPtr(MemID, out _);

                    GUI.Camera.Memory.Lock(MemID);
                    GUI.Camera.Memory.GetSize(MemID, out int s32Width, out int s32Height);

                    GUI.Camera.Memory.ToBitmap(MemID, out CurrentBitmap);

                    // unlock image buffer
                    GUI.Camera.Memory.Unlock(MemID);

                    CurrentBitmap.SetPixel(640, 512, Color.Black);

                    GUI.Display.Image = CurrentBitmap;
                }
            }
        }
    }
}
