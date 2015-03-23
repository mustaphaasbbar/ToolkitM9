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
                            tbNote.Text = "Select an file and partition to flash to.";
                        }
                        break;

                    case "Erase":
                        {
                            this.Title = (this.Title + " - Erase");
                            btnGoFlash.Content = "Erase";
                            tbNote.Text = "Select a partition to erase.";
                        }
                        break;

                    case "Boot":
                        {
                            this.Title = (this.Title + " - Boot");
                            btnGoFlash.Content = "Boot";
                            btnBrwsFile.IsEnabled = true;
                            cbPartition.IsEnabled = false;
                            tbNote.Text = "Select an image to boot from.";
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
                #region Flash
                if (Settings.Selector == "Flash")
                    {
                        switch (cbPartition.Text)
                        {
                            case "Boot":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.BOOT, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Cache":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.CACHE, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Data":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.DATA, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Hboot":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.HBOOT, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Kernel":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.KERNEL, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Misc":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.MISC, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Radio":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.RADIO, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Ramdisk":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.RAMDISK, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Recovery":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "System":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.SYSTEM, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Unlock Token":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.UNLOCKTOKEN, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;

                            case "Zip":
                                {
                                    if (tbFilepath.Text == "")
                                    {
                                        MessageBox.Show("You must browse for a file to flash! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        Fastboot.Instance().Flash(IDDevicePartition.ZIP, Quote.Text + tbFilepath.Text + Quote.Text);
                                    }
                                }
                                break;
                        }
                #endregion
                    }
                #region Erase
                else if (Settings.Selector == "Erase")
                    {
                        switch (cbPartition.Text)
                        {
                            case "Boot":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.BOOT);
                                }
                                break;

                            case "Cache":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.CACHE);
                                }
                                break;

                            case "Data":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.DATA);
                                }
                                break;

                            case "Hboot":
                                {
                                    MessageBox.Show("You cannot erase the " + cbPartition.Text + " partition!", "Protected Partition!", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                break;

                            case "Kernel":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.KERNEL);
                                }
                                break;

                            case "Misc":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.MISC);
                                }
                                break;

                            case "Radio":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.RADIO);
                                }
                                break;

                            case "Ramdisk":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.RAMDISK);
                                }
                                break;

                            case "Recovery":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.RECOVERY);
                                }
                                break;

                            case "System":
                                {
                                    Fastboot.Instance().Erase(IDDevicePartition.SYSTEM);
                                }
                                break;

                            case "Unlock Token":
                                {
                                    MessageBox.Show("You cannot erase the " + cbPartition.Text + " partition!", "Protected Partition!", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                break;

                            case "Zip":
                                {
                                    MessageBox.Show("You cannot erase the " + cbPartition.Text + " partition!", "Protected Partition!", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                break;
                        }
                #endregion
                    }
                #region boot
                else if (Settings.Selector == "Boot")
                {
                    if (tbFilepath.Text == "")
                    {
                        MessageBox.Show("You must browse for a file to boot from! Please select a file.", "Missing file!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Fastboot.Instance().Boot(Quote.Text + tbFilepath.Text + Quote.Text);
                    }
                #endregion
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

        private void btnBrwsFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "*.*";
            dlg.Filter = "All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                tbFilepath.Text = filename;
            }
        }
    }
}
