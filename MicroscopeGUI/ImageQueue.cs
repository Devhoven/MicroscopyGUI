using System;
using uEye.Defines;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroscopeGUI
{
    static class ImageQueue
    {
        public static volatile bool StopRunning;

        public static void Run()
        {
            uint u32Timeout = 100;
            while (!StopRunning)
            {
                Status StatusRet = GUI.Camera.Memory.Sequence.WaitForNextImage(u32Timeout, out int s32MemID, out _);
                if (StatusRet == Status.Success)
                {
                    GUI.Camera.Memory.ToIntPtr(s32MemID, out _);
                    GUI.Camera.Display.Render(s32MemID, GUI.DisplayHandle, DisplayRenderMode.FitToWindow);
                    GUI.Camera.Memory.Sequence.Unlock(s32MemID);
                }
                else
                {
                    Console.WriteLine("but whyu? :(");
                    int i = (int)StatusRet;
                }
            }
        }
    }
}
