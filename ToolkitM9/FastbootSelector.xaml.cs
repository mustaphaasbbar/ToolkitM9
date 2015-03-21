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

using AndroidCtrl;
using AndroidCtrl.ADB;
using AndroidCtrl.AAPT;
using AndroidCtrl.Tools;
using AndroidCtrl.Signer;
using AndroidCtrl.Fastboot;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for Flash.xaml
    /// </summary>
    public partial class FastbootSelector : Window
    {
        public FastbootSelector()
        {
            InitializeComponent();
        }

        #region Nested type: Settings

        public static class Settings
        {
            public static string Selector = "null";
        }

        #endregion Nested type: Settings

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Settings.Selector)
                {
                    case "Flash":
                        {
                            this.Title = (this.Title + " - Flash");
                            btnBrwsFile.IsEnabled = true;
                            btnGoFlash.Content = "Flash";
                        }
                        break;

                    case "Erase":
                        {
                            this.Title = (this.Title + " - Erase");
                            btnGoFlash.Content = "Erase";
                        }
                        break;
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

        private void btnGoFlash_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Settings.Selector == "Flash")
                {
                    switch (cbPartition.Text)
                    {
                        case "Boot":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.BOOT, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Cache":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.CACHE, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Data":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.DATA, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Hboot":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.HBOOT, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Kernel":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.KERNEL, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Misc":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.MISC, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Radio":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.RADIO, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Ramdisk":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.RAMDISK, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Recovery":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "System":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.SYSTEM, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Unlock Token":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.UNLOCKTOKEN, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;

                        case "Zip":
                            {
                                Fastboot.Instance().Flash(IDDevicePartition.ZIP, Quote.Text + tbFilepath.Text + Quote.Text);
                            }
                            break;
                    }
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
    }
}
