using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Xml.Linq;

using AndroidCtrl;
using AndroidCtrl.ADB;
using AndroidCtrl.AAPT;
using AndroidCtrl.Tools;
using AndroidCtrl.Signer;
using AndroidCtrl.Fastboot;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Load : Window
    {
        public Load()
        {
            InitializeComponent();
        }

        private void CheckFileSystem()
        {
            try
            {
                string[] neededDirectories = new string[] { "Data/", "Data/Installers", "Data/Logs", "Data/Recoveries", "Data/Config" };

                foreach (string dir in neededDirectories)
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                if (!File.Exists("./Data/Config/Device.cfg"))
                {
                    File.Create("./Data/Config/Device.cfg");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error has occured! A log file has been placed in the Logs folder. Please report this error, with the log file, in the toolkit thread on XDA. Links in the 'File' menu!",
                    "Critical Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                string fileDateTime = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmss");
                var file = new StreamWriter("./Data/Logs/" + fileDateTime + ".txt");
                file.WriteLine(ex);
                file.Close();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            #region FileStrucCheck

            CheckFileSystem();

            #endregion

            #region Select Device

            try
            {
                if (Properties.Settings.Default["Device"].ToString() == "None")
                {
                    MessageBox.Show("Please select your device on the next screen.", "No device selected", MessageBoxButton.OK, MessageBoxImage.Information);
                    var select = new DeviceSelector();
                    select.ShowDialog();
                }
                else
                {
                    //Continue
                }

                #region Restarting ADB server
                //Stopping of ADB server
                try
                {
                    foreach (Process proc in Process.GetProcessesByName("adb"))
                    {
                        proc.Kill();
                    }

                    foreach (Process proc in Process.GetProcessesByName("fastboot"))
                    {
                        proc.Kill();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Thread.Sleep(2000);

                //Deployment of ADB
                Deploy.ADB();

                //After deployment of adb and fastboot files we load the main form.

                Main main = new Main();
                main.Show();

                //After loading the main form, we close this one.

                this.Close();
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                "An error has occured! A log file has been placed in the Logs folder. Please report this error, with the log file, in the toolkit thread on XDA. Links in the 'File' menu!",
                "Critical Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                string fileDateTime = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmss");
                var file = new StreamWriter("./Data/Logs/" + fileDateTime + ".txt");
                file.WriteLine(ex);
                file.Close();
            }

            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Driver Check
            //Check for CWM Drivers
            if (!Directory.Exists(@"C:\Program Files (x86)\ClockworkMod\Universal Adb Driver\") &&
                !Directory.Exists(@"C:\Program Files\ClockworkMod\Universal Adb Driver"))
            {
                    MessageBoxResult messageResult = MessageBox.Show("You are missing some ADB Drivers! They are required for your device to connect with your computer. Would you like to install them now?", "ADB Drivers", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (messageResult == MessageBoxResult.Yes)
                    {
                        var download = new Downloader();
                        ToolkitM9.Downloader.Settings.Selector = "ADB";
                        download.ShowDialog();
                    }
            }
            else
            {
                //Continue
            }
            #endregion

            #region HTCDriver Check
            if (!Directory.Exists(@"C:\Program Files (x86)\HTC\HTC Driver\Driver Files") &&
                !Directory.Exists(@"C:\Program Files\HTC\HTC Driver\Driver Files"))
            {
                MessageBoxResult messageResult = MessageBox.Show("You are missing some HTC Drivers! They are required for your device to connect with your computer. Would you like to install them now?", "HTC Drivers", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    var download = new Downloader();
                    ToolkitM9.Downloader.Settings.Selector = "HTC";
                    download.ShowDialog();
                }
            }
            else
            {
                //Continue
            }
            #endregion
        }
    }
}
