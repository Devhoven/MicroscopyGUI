using uEye;
using System;
using System.IO;
using System.Text;
using uEye.Defines;
using System.Windows;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using MicroscopeGUI.UIElements.Steps;
using Image = System.Windows.Controls.Image;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using Button = System.Windows.Controls.Button;
using Application = System.Windows.Application;

namespace MicroscopeGUI
{
    // Class for all of the ui element events
    public partial class UI : Window
    {
        // Changes the directory of the image gallery
        void ChangeDirClick(object sender, RoutedEventArgs e) =>
            ImgGallery.UpdatePath();

        // Makes the config step visible
        void ConfigBtnClick(object sender, RoutedEventArgs e)
        {
            SetVisibillity(ConfigCon, ConfigConBtn);
            RegistryManager.SetValue("CurrentConfigStep", 0);
        }

        // Makes the analysis step visible
        void AnalysisBtnClick(object sender, RoutedEventArgs e)
        {
            SetVisibillity(AnalysisCon, AnalysisConBtn);
            RegistryManager.SetValue("CurrentConfigStep", 1);
        }

        // Sets the visibillity of the containers and changes the color of the buttons
        void SetVisibillity(StepCon Con, Button Btn)
        {
            ConfigCon.Visibility = Visibility.Hidden;
            AnalysisCon.Visibility = Visibility.Hidden;

            ConfigConBtn.Background = Brushes.Transparent;
            AnalysisConBtn.Background = Brushes.Transparent;

            Con.Visibility = Visibility.Visible;
            Btn.Background = Brushes.LightSkyBlue;
        }

        // Opens the settings
        void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow Settings = new SettingsWindow();
            Settings.Owner = this;
            Settings.Show();
        }

        // Saves the current frame
        void SaveClick(object sender, RoutedEventArgs e)
        {
            MetadataPopup = new MetaDataWindow();
            MetadataPopup.Owner = this;
            MetadataPopup.Show();
        }

        // Exits the application
        private void ExitClick(object sender, RoutedEventArgs e) =>
            Close();

        // Saves a config
        void ConfigSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.Title = "Save file";
            SaveDialog.Filter = "Configfile|*.xml";

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(SaveDialog.FileName, Control.GetXMLString(), Encoding.UTF8);
                UserInfo.SetInfo("Saved the current config to " + SaveDialog.FileName);
            }
            else
                UserInfo.SetInfo("Action aborted");
        }

        // Loads a config
        void ConfigLoadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.Title = "Choose the config file";
            OpenDialog.Filter = "Configfile|*.xml";

            if (OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Control.LoadXML(File.ReadAllText(OpenDialog.FileName, Encoding.UTF8));
                UserInfo.SetInfo("Loaded the config from " + OpenDialog.FileName);
            }
            else
                UserInfo.SetInfo("Action aborted");
        }

        // Closes all the stuff the camera set up (Except the ring buffer) and initializes it again
        void ReloadCamClick(object sender, RoutedEventArgs e)
        {
            UserInfo.SetInfo("Reloading the cam...");

            // Starts a new thread with the camera restarting logic inthere
            Thread RestartThread = new Thread(RestartCam);
            RestartThread.Start();
            // Tries to join the thread together with the normal one again
            // When this is done, the Join method waits for 10 seconds till the thread is finished
            // If it hasn't finished until then, the restarting is aborted
            if (!RestartThread.Join(TimeSpan.FromSeconds(10)))
            {
                RestartThread.Interrupt();
                UserInfo.SetInfo("Aborted reloading the cam, since it took too long ( > 10 seconds), check if everything is plugged in correctly");
            }
            else
            {
                // Reloading all of the control elements
                Control.RemoveAllControls();
                ToolCon.Children.Remove(ConfigCon);
                ToolCon.Children.Remove(AnalysisCon);
                ConfigCon = new ConfigStepCon(ToolCon);
                AnalysisCon = new AnalysisStepCon(ToolCon);
                SetVisibillity(ConfigCon, ConfigConBtn);

                UserInfo.SetInfo("Reloaded the cam");

                if (!(OldXMLConfig is null))
                {
                    MessageBoxResult Result = MessageBox.Show("There is a saved config from before the camera crashed\nDo you want to load it?", "Question", MessageBoxButton.YesNo);
                    if (Result == MessageBoxResult.Yes)
                    {
                        Control.LoadXML(OldXMLConfig);
                        UserInfo.SetInfo("Loaded the config");
                    }
                }
            }

            // Capsuled in a method, so I can put it in a seperate thread
            void RestartCam()
            {
                // Closes the thread and joins it to the current
                ImageQueue.Mode = ImageQueue.ImgQueueMode.Frozen;
                ImageQueue.StopRunning = true;
                WorkerThread.Join();

                // Closes the camera
                Cam.Exit();

                // Initializes a new thread and a new camera
                InitializeCam(false);

                ImageQueue.Mode = ImageQueue.ImgQueueMode.Live;

                ImageQueue.StopRunning = false;

                StartCapture();
            }
        }

        // Freezes the cam
        void FreezeCamClick(object sender, RoutedEventArgs e)
        {
            Cam.Acquisition.Freeze();
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Frozen;
            LiveFeedBtn.Background = Brushes.Transparent;
            FreezeCamBtn.Background = Brushes.LightBlue;

            UserInfo.SetInfo("Freezed the camera");
        }

        // Starts the camera live feed again
        void LiveFeedClick(object sender, RoutedEventArgs e)
        {
            Cam.Acquisition.Capture();
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Live;
            LiveFeedBtn.Background = Brushes.LightBlue;
            FreezeCamBtn.Background = Brushes.Transparent;

            UserInfo.SetInfo("Started the live feed");
        }

        void ConfigConToggle(object sender, RoutedEventArgs e) =>
            ToggleItemVisibillity((MenuItem)sender, ToolCon, "ConfigConActivated");

        void ImgGalleryToggle(object sender, RoutedEventArgs e) =>
            ToggleItemVisibillity((MenuItem)sender, ImgGalleryCon, "ImgGalleryActivated");

        // Toggles the visibillity of a UI Elements and with it the color of the specified MenuItem
        void ToggleItemVisibillity(MenuItem Sender, UIElement Element, string ValName)
        {
            if (Element.Visibility == Visibility.Visible)
            {
                Element.Visibility = Visibility.Collapsed;
                Sender.Background = Brushes.Transparent;
                RegistryManager.SetValue(ValName, false);
            }
            else
            {
                Element.Visibility = Visibility.Visible;
                Sender.Background = Brushes.LightBlue;
                RegistryManager.SetValue(ValName, true);
            }
        }
    }
}
