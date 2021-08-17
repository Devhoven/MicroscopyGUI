using System;
using uEye.Defines;
using System.Drawing;
using ColorMode = uEye.Defines.ColorMode;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace MicroscopeGUI
{
    static class ImageQueue
    {
        // Is set to true at the end of the program, so the thread won't be running in the background
        public static volatile bool StopRunning = false;
        // Holds the current status of the cam, which we get from WaitForNextImage
        public static volatile Status CurrentCamStatus;
        // Fires if the frame has changed
        public static event EventHandler OnFrameChange = delegate { };
        // Contains all of the values of the histogram of the current image
        // 3 * 256, since there is a value for every 8 bit value of one channel
        public static uint[] Histogram = new uint[3 * 256];
        // Holds the current frame as a bitmap
        public static Bitmap CurrentFrameBitmap;
        // Is there to prevent issues with the camera
        public static ImgQueueMode Mode = ImgQueueMode.Live;
        // Are holding the current width and height of the image
        public static int Width = 1280, Height = 1028;
        // Is used for the histogram, so it won't be updated every frame, which would result in flickering 
        static Stopwatch HistogramTimer;
        // Counts how many images failed continuously to reach the computer
        // Implemented for error tolerance, since sometimes sent frames would get lost, which would cause the software to fail
        static int FailCount = 0;

        public static void Run()
        {
            HistogramTimer = new Stopwatch();
            HistogramTimer.Start();
            while (!StopRunning)
            {
                // Skips the loop if the image is frozen or the camera is shut
                if (Mode == ImgQueueMode.Frozen || !UI.Cam.Memory.IsOpened)
                    continue;
                // Waits for the next image and returns the memory ID if a new image was sent by the cam
                CurrentCamStatus = UI.Cam.Memory.Sequence.WaitForNextImage(500, out int MemID, out _);
                if (CurrentCamStatus == Status.Success)
                {
                    FailCount = 0;

                    // Getting the values of the histogram, every 30 ms
                    if (HistogramTimer.ElapsedMilliseconds >= 30)
                    {
                        UI.Cam.Image.GetHistogram(MemID, ColorMode.BGR8Packed, out Histogram);
                        HistogramTimer.Restart();

                        UI.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            UI.HistogramControl.UpdateHistogram();
                        }));
                    }

                    UI.Cam.Memory.GetSize(MemID, out Width, out Height);

                    // Converting it to a bitmap, since you can dispose it
                    // When you do it directly with an array, you get a lot of gc calls
                    UI.Cam.Memory.ToBitmap(MemID, out Bitmap CFB);
                    CurrentFrameBitmap = CFB;

                    // Checking if the main Thread is still running
                    if (!StopRunning && !UI.CurrentDispatcher.HasShutdownStarted)
                        SetCurrentFrame(CFB);

                    // Unlocking the image buffer
                    UI.Cam.Memory.Unlock(MemID);
                }
                else
                {
                    // The program gets here if the user froze the cam and the cam was waiting for another image
                    // If that happened, it skips to the next iteration of the loop and does not break out of it
                    if (Mode != ImgQueueMode.Live)
                        continue;

                    // Counts one up if something went wrong
                    FailCount++;
                    if (FailCount >= 69)
                        UI.CurrentDispatcher.Invoke(() => UserInfo.SetErrorInfo(FailCount + " frames were lost"));
                    else if (FailCount > 20)
                        UI.CurrentDispatcher.Invoke(() => UserInfo.SetInfo(FailCount + " frames were lost"));

                    // If 10 frames fail continuously the ImageQueue won't continue
                    if (FailCount < 100)
                        continue;

                    // Enabling error reports from the uEye Library
                    // Doing it after the initialization, because if you don't have a camera you will get a lot of dialog boxes
                    UI.Cam.Information.SetEnableErrorReport(true);

                    // Displaying the NoCam Image if you can't receive an image anymore
                    UI.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                            new Action(() =>
                                {
                                    UI.OldXMLConfig = Control.GetXMLString();
                                    UserInfo.SetErrorInfo("Camera disconnected (" + Enum.GetName(typeof(Status), CurrentCamStatus) + ")");
                                    UI.CurrentFrame.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/NoCamConnected.png"));
                                }
                           ));
                    break;
                }
            }
        }

        public static void SetCurrentFrame(Bitmap Bmp)
        {
            // Setting the current image on the picture panel
            // Calling the frame change event
            UI.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                new Action(() =>
                {
                    if (Mode == ImgQueueMode.Live)
                    {
                        UI.CurrentFrame.Source = Bmp.GetWriteableBitmap();

                        OnFrameChange(null, null);
                    }
                }));
        }

        public enum ImgQueueMode
        {
            Live,
            Frozen,
            ViewingAnotherImage
        }
    }
}
