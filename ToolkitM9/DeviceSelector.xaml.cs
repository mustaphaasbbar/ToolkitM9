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
    /// Interaction logic for DeviceSelector.xaml
    /// </summary>
    public partial class DeviceSelector : Window
    {
        public DeviceSelector()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Selector.Text.ToString())
                {
                    case "AT&T, International GSM":
                        {
                            ToolkitM9.Downloader.Settings.Device = "GSM";
                            ToolkitM9.Downloader.Settings.TwoRecoveries = true;

                            Properties.Settings.Default["Device"] = "GSM";
                            Properties.Settings.Default.Save();
                        }
                        break;

                    case "Sprint":
                        {
                            ToolkitM9.Downloader.Settings.Device = "Sprint";
                            ToolkitM9.Downloader.Settings.TwoRecoveries = true;

                            Properties.Settings.Default["Device"] = Selector.Text.ToString();
                            Properties.Settings.Default.Save();
                        }
                        break;

                    case "T-Mobile":
                        {
                            ToolkitM9.Downloader.Settings.Device = "T-Mobile";
                            ToolkitM9.Downloader.Settings.TwoRecoveries = true;

                            Properties.Settings.Default["Device"] = Selector.Text.ToString();
                            Properties.Settings.Default.Save();
                        }
                        break;

                    case "Verizon":
                        {
                            ToolkitM9.Downloader.Settings.Device = "Verizon";
                            ToolkitM9.Downloader.Settings.TwoRecoveries = true;

                            Properties.Settings.Default["Device"] = Selector.Text.ToString();
                            Properties.Settings.Default.Save();
                        }
                        break;
                }
                var download = new Downloader();
                download.ShowDialog();

                this.Close();
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
