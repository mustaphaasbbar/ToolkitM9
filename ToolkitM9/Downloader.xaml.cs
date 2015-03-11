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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.ComponentModel;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for Downloader.xaml
    /// </summary>
    public partial class Downloader : Window
    {
        public Downloader()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new WebClient();
                var client2 = new WebClient();
                switch (Settings.Selector)
                {
                    case "ADB":
                        {
                            tBStatus.Text = "Downloading Universal ADB Driver...";
                            client.DownloadProgressChanged += (client_DownloadProgressChanged);
                            client.DownloadFileCompleted += (client_DownloadFileCompleted);
                            client.DownloadFileAsync(
                                new Uri("https://s.basketbuild.com/dl/devs?dl=squabbi/ADBDriver.msi"),
                                "./Data/Installers/ADBDriver.msi");
                        }
                        break;

                    case "HTC":
                        {
                            tBStatus.Text = "Downloading HTC Driver...";
                            client.DownloadProgressChanged += (client_DownloadProgressChanged);
                            client.DownloadFileCompleted += (client_DownloadFileCompleted);
                            client.DownloadFileAsync(
                                new Uri("https://s.basketbuild.com/dl/devs?dl=squabbi/HTCDriver.msi"),
                                "./Data/Installers/HTCDriver.msi");
                        }
                        break;

                    case "Recovery":
                        {
                            if (Settings.TwoRecoveries == true)
                            {
                                tBStatus.Text = "Downloading recoveries for " + Settings.Device.ToString() + "...";
                                client.DownloadFileAsync(
                                    new Uri("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/" + Settings.Device + "/recovery1.img"),
                                    "./Data/Recoveries/Recovery1.img");
                                client2.DownloadProgressChanged += (client_DownloadProgressChanged);
                                client2.DownloadFileCompleted += (client_DownloadFileCompleted);
                                client2.DownloadFileAsync(
                                    new Uri("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/" + Settings.Device + "/recovery2.img"),
                                    "./Data/Recoveries/Recovery2.img");
                            }
                            else
                            {
                                client.DownloadProgressChanged += (client_DownloadProgressChanged);
                                client.DownloadFileCompleted += (client_DownloadFileCompleted);
                                client.DownloadFileAsync(
                                    new Uri("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/" + Settings.Device + "/recovery1.img"),
                                    "./Data/Recoveries/Recovery1.img");
                            }
                        }
                        break;
                }
                Settings.Device = null;
                Settings.TwoRecoveries = false;
            }
            catch (Exception ex)
            {
                string fileDateTime = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmss");
                var file = new StreamWriter("./Data/Logs/" + fileDateTime + ".txt");
                file.WriteLine(ex);
                file.Close();
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                double bytesIn = e.BytesReceived;
                double totalBytes = e.TotalBytesToReceive;
                double percentage = bytesIn / totalBytes * 100;
                pBDlStatus.Value = int.Parse(Math.Truncate(percentage).ToString());
                pBDlStatus.Value = (int)Math.Truncate(percentage);
            }
            catch (Exception ex)
            {
                string fileDateTime = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmss");
                var file = new StreamWriter("./Data/Logs/" + fileDateTime + ".txt");
                file.WriteLine(ex);
                file.Close();
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(
                        @"An error occured while attempting to download the necessary files! Please restart the toolkit, check your internet connection and try again in a few minutes.",
                        @"Download Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (File.Exists("./Data/Recoveries/Recovery1.img"))
                    {
                        File.Delete("./Data/Recoveries/Recovery1.img");
                    }
                    if (File.Exists("./Data/Recoveries/Recovery2.img"))
                    {
                        File.Delete("./Data/Recoveries/Recovery2.img");
                    }
                    Settings.Selector = "Recovery";
                    Close();
                }
                else if (Settings.Selector == "ADB")
                {
                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "/Data/Installers/ADBDriver.msi");
                    Settings.Selector = "Recovery";
                    Close();
                }
                else if (Settings.Selector == "HTC")
                {
                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "/Data/Installers/HTCDriver.msi");
                    Settings.Selector = "Recovery";
                    Close();
                }
                else
                {
                    Settings.Selector = "Recovery";
                    Close();
                }
            }
            catch (Exception ex)
            {
                string fileDateTime = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmss");
                var file = new StreamWriter("./Data/Logs/" + fileDateTime + ".txt");
                file.WriteLine(ex);
                file.Close();
            }
        }

        #region Nested type: Settings

        public static class Settings
        {
            public static string Selector = "Recovery";
            public static string Device;
            public static bool TwoRecoveries;
        }

        #endregion Nested type: Settings
    }
}