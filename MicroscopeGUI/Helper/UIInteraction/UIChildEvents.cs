﻿using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using MicroscopeGUI.UIElements.Steps;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace MicroscopeGUI
{
    // Class for all of the ui element events
    public partial class UI : Window
    {
        // Used for key bindings
        private void UIKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
                ReloadCamera();
            else if (e.Key == System.Windows.Input.Key.F1)
                SettingsClick(null, null);
            else if (e.Key == System.Windows.Input.Key.Escape)
                Close();
            else if (e.Key == System.Windows.Input.Key.Left)
                ConfigBtnClick(null, null);
            else if (e.Key == System.Windows.Input.Key.Right)
                AnalysisBtnClick(null, null);
            // If Ctrl is pressed
            else if (e.KeyboardDevice.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control))
            {
                // Ctrl+S
                if (e.Key == System.Windows.Input.Key.S)
                {
                    // Ctrl+Shift+S
                    if (e.KeyboardDevice.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift))
                        ConfigSaveClick(null, null);
                    else
                        SaveClick(null, null);
                }
                // Ctrl+O
                else if (e.Key == System.Windows.Input.Key.O)
                    ConfigLoadClick(null, null);
                // Ctrl+L
                else if (e.Key == System.Windows.Input.Key.L)
                    LiveFeedClick(null, null);
                // Ctrl+F
                else if (e.Key == System.Windows.Input.Key.F)
                    FreezeCamClick(null, null);
            }
        }

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
            ReloadCamera();
        }

        // Freezes the cam
        void FreezeCamClick(object sender, RoutedEventArgs e)
        {
            Cam.Acquisition.Freeze();
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Frozen;
            LiveFeedBtn.Background = Brushes.Transparent;
            FreezeCamBtn.Background = Brushes.LightSkyBlue;

            UserInfo.SetInfo("Freezed the camera");
        }

        // Starts the camera live feed again
        void LiveFeedClick(object sender, RoutedEventArgs e)
        {
            Cam.Acquisition.Capture();
            ImageQueue.Mode = ImageQueue.ImgQueueMode.Live;
            LiveFeedBtn.Background = Brushes.LightSkyBlue;
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
                Sender.Background = Brushes.LightSkyBlue;
                RegistryManager.SetValue(ValName, true);
            }
        }
    }
}
