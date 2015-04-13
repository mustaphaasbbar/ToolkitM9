using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Globalization;
using System.Net;
using System.Xml;
using System.Reflection;

using AndroidCtrl;
using AndroidCtrl.ADB;
using AndroidCtrl.AAPT;
using AndroidCtrl.Tools;
using AndroidCtrl.Signer;
using AndroidCtrl.Fastboot;

using System.Xml.Linq;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for Version.xaml
    /// </summary>
    public partial class RVersion : Window
    {
        public RVersion()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Settings.Recovery.ToString() == "1")
                {
                        //Remote Version
                        XDocument doc = XDocument.Load("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/recoveries/CTB_TWRP/Version.xml");
                        var name = doc.Root.Element("Name").Value;
                        var version = doc.Root.Element("Version").Value;
                        var notes = doc.Root.Element("Notes").Value;
                        var site = doc.Root.Element("Site").Value;
                        var dl = doc.Root.Element("DL").Value;

                        sRecName.Text = name;
                        sRecVer.Text = version;
                        sRecNotes.Text = notes;

                        //Local Version
                        XDocument docL = XDocument.Load("./Data/Recoveries/rVersion1.xml");
                        var nameL = docL.Root.Element("Name").Value;
                        var versionL = docL.Root.Element("Version").Value;
                        //var notesL = docL.Root.Element("Notes").Value;
                        //var siteL = docL.Root.Element("openSite").Value;
                        //var linkL = docL.Root.Element("Link").Value;

                        lRecName.Text = nameL;
                        lRecVer.Text = versionL;
                        //sRecNotes.Text = notesL;

                        if (version.Contains(versionL))
                        {
                            //MessageBox.Show("You have the latest version..");
                            this.Title = "You have the latest recovery (" + name + " " + version + ")";
                        }
                        else
                        {
                            this.Title = "There is a newer version avaliable on the server!";
                            MessageBox.Show("You have an alternate version to the server. Please compare the versions listed in the boxes...");
                            btnUpdate.IsEnabled = true;
                        }           
                }
                else if (Settings.Recovery.ToString() == "2")
                {
                    if (ToolkitM9.Properties.Settings.Default["Device"].ToString() == "AT&T")
                    {
                        //Remote Version
                        XDocument doc = XDocument.Load("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/recoveries/ATT/Version2.xml");
                        var name = doc.Root.Element("Name").Value;
                        var version = doc.Root.Element("Version").Value;
                        var notes = doc.Root.Element("Notes").Value;
                        var site = doc.Root.Element("openSite").Value;
                        var link = doc.Root.Element("Link").Value;

                        sRecName.Text = name;
                        sRecVer.Text = version;
                        sRecNotes.Text = notes;

                        //Local Version
                        XDocument docL = XDocument.Load("./Data/Recoveries/rVersion2.xml");
                        var nameL = docL.Root.Element("Name").Value;
                        var versionL = docL.Root.Element("Version").Value;
                        //var notesL = docL.Root.Element("Notes").Value;
                        //var siteL = docL.Root.Element("openSite").Value;
                        //var linkL = docL.Root.Element("Link").Value;

                        lRecName.Text = nameL;
                        lRecVer.Text = versionL;
                        //sRecNotes.Text = notesL;

                        if (version.Contains(versionL))
                        {
                            //MessageBox.Show("You have the latest version..");
                            this.Title = "You have the latest recovery (" + name + " " + version + ")";
                        }
                        else
                        {
                            this.Title = "There is a newer version avaliable on the server!";
                            MessageBox.Show("You have an alternate version to the server. Please compare the versions listed in the boxes...");
                            btnUpdate.IsEnabled = true;
                        }

                        //Important notes open
                        if (site.ToString() == "true")
                        {
                            MessageBox.Show("There are important notes to read about your recovery before you continue, press OK to view them.", "Important Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            Process.Start(link);
                        }
                        else
                        {
                            //Nothing.
                        }
                    }
                    else
                    {
                        //Remote Version
                        XDocument doc = XDocument.Load("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/recoveries/" + ToolkitM9.Properties.Settings.Default["Device"].ToString() + "/Version2.xml");
                        var name = doc.Root.Element("Name").Value;
                        var version = doc.Root.Element("Version").Value;
                        var notes = doc.Root.Element("Notes").Value;
                        var site = doc.Root.Element("openSite").Value;
                        var link = doc.Root.Element("Link").Value;

                        sRecName.Text = name;
                        sRecVer.Text = version;
                        sRecNotes.Text = notes;

                        //Local Version
                        XDocument docL = XDocument.Load("./Data/Recoveries/rVersion2.xml");
                        var nameL = docL.Root.Element("Name").Value;
                        var versionL = docL.Root.Element("Version").Value;
                        //var notesL = docL.Root.Element("Notes").Value;
                        //var siteL = docL.Root.Element("openSite").Value;
                        //var linkL = docL.Root.Element("Link").Value;

                        lRecName.Text = nameL;
                        lRecVer.Text = versionL;
                        //sRecNotes.Text = notesL;

                        if (version.Contains(versionL))
                        {
                            //MessageBox.Show("You have the latest version..");
                            this.Title = "You have the latest recovery (" + name + " " + version + ")";
                        }
                        else
                        {
                            this.Title = "There is a newer version avaliable on the server!";
                            MessageBox.Show("You have an alternate version to the server. Please compare the versions listed in the boxes...");
                            btnUpdate.IsEnabled = true;
                        }

                        //Important notes open
                        if (site.ToString() == "true")
                        {
                            MessageBox.Show("There are important notes to read about your recovery before you continue, press OK to view them.", "Important Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            Process.Start(link);
                        }
                        else
                        {
                            //Nothing.
                        }
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

        #region Nested type: Settings

        public static class Settings
        {
            public static string Device;
            public static string Recovery;
        }

        #endregion Nested type: Settings

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Recovery.ToString() == "1")
            {
                if (ADB.Instance().GetState() == IDDeviceState.DEVICE)
                {
                    MessageBoxResult messageResult = MessageBox.Show("Your phone needs to be in 'fastboot USB' mode. Would you like to reboot into it now?", "Reboot to bootloader required!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (messageResult == MessageBoxResult.Yes)
                    {
                        tBRecoveryStatus.Text = "Rebooting into Bootloader...";
                        ADB.Instance().Reboot(IDBoot.BOOTLOADER);
                        tBRecoveryStatus.Text = "Waiting for device...";

                        if (cbFlashDownload.IsChecked == true)
                        {
                            Task.Delay(10000).ContinueWith(_ =>
                            {
                                tBRecoveryStatus.Text = "Rebooting into Download Mode...";
                                Fastboot.Instance().OEM.RebootRUU();
                            });
                        }

                        Task.Delay(10000).ContinueWith(_ =>
                        {
                            tBRecoveryStatus.Text = "Flashing recovery...";
                            Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery1.img");

                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                tBRecoveryStatus.Text = "Rebooting...";
                                Fastboot.Instance().Reboot(IDBoot.REBOOT);
                            });
                        }
                        );
                    }
                }
                else if (ADB.Instance().GetState() == IDDeviceState.FASTBOOT)
                {
                    tBRecoveryStatus.Text = "Flashing recovery...";
                    Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery1.img");

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        tBRecoveryStatus.Text = "Rebooting...";
                        Fastboot.Instance().Reboot(IDBoot.REBOOT);
                    });
                }
            }
            else if (Settings.Recovery.ToString() == "2")
            {
                if (ADB.Instance().GetState() == IDDeviceState.DEVICE)
                {
                    MessageBoxResult messageResult = MessageBox.Show("Your phone needs to be in 'fastboot USB' mode. Would you like to reboot into it now?", "Reboot to bootloader required!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (messageResult == MessageBoxResult.Yes)
                    {
                        //tBRecoveryStatus.Text = "Rebooting into Bootloader...";
                        ADB.Instance().Reboot(IDBoot.BOOTLOADER);
                        tBRecoveryStatus.Text = "Waiting for device...";

                        //Rebooting into RUU mode if cheked.
                        if (cbFlashDownload.IsChecked == true)
                        {
                            Task.Delay(10000).ContinueWith(_ =>
                            {
                                tBRecoveryStatus.Text = "Rebooting into Download Mode...";
                                Fastboot.Instance().OEM.RebootRUU();
                            });

                        }

                        Task.Delay(10000).ContinueWith(_ =>
                        {
                            //tBRecoveryStatus.Text = "Flashing recovery...";
                            Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery2.img");

                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                //tBRecoveryStatus.Text = "Rebooting...";
                                Fastboot.Instance().Reboot(IDBoot.REBOOT);
                            });
                        }
                        );
                    }
                }
                else if (ADB.Instance().GetState() == IDDeviceState.FASTBOOT)
                {
                    //tBRecoveryStatus.Text = "Flashing recovery...";
                    Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery2.img");

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        //tBRecoveryStatus.Text = "Rebooting...";
                        Fastboot.Instance().Reboot(IDBoot.REBOOT);
                    });
                }
            } 
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Properties.Settings.Default["Device"].ToString())
                {
                    case "AT&T":
                        {
                            ToolkitM9.Downloader.Settings.Device = "ATT";

                            //Downloader Window Init
                            Downloader dl = new Downloader();
                            dl.ShowDialog();

                            //Close this form
                            Close();

                            RVersion recver = new RVersion();
                            recver.ShowDialog();
                        }
                        break;

                    case "GSM":
                        {
                            ToolkitM9.Downloader.Settings.Device = "GSM";

                            //Downloader Window Init
                            Downloader dl = new Downloader();
                            dl.ShowDialog();

                            //Close this form
                            Close();

                            RVersion recver = new RVersion();
                            recver.ShowDialog();
                        }
                        break;

                    case "Sprint":
                        {
                            ToolkitM9.Downloader.Settings.Device = "Sprint";

                            //Downloader Window Init
                            Downloader dl = new Downloader();
                            dl.ShowDialog();

                            //Close this form
                            Close();

                            RVersion recver = new RVersion();
                            recver.ShowDialog();
                        }
                        break;

                    case "T-Mobile":
                        {
                            ToolkitM9.Downloader.Settings.Device = "T-Mobile";

                            //Downloader Window Init
                            Downloader dl = new Downloader();
                            dl.ShowDialog();

                            //Close this form
                            Close();

                            RVersion recver = new RVersion();
                            recver.ShowDialog();
                        }
                        break;

                    case "Verizon":
                        {
                            ToolkitM9.Downloader.Settings.Device = "Verizon";

                            //Downloader Window Init
                            Downloader dl = new Downloader();
                            dl.ShowDialog();

                            //Close this form
                            Close();

                            RVersion recver = new RVersion();
                            recver.ShowDialog();
                        }
                        break;
                }
            }
            catch
            {

            }
        }

        private void btnNotes_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default["Device"].ToString() == "AT&T")
            {
                //Remote Version
                XDocument doc = XDocument.Load("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/recoveries/ATT/Version.xml");
                var link = doc.Root.Element("Link").Value;
                Process.Start(link);
            }
            else
            {
                //Remote Version
                XDocument doc = XDocument.Load("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/recoveries/" + Properties.Settings.Default["Device"] +"/Version.xml");
                var link = doc.Root.Element("Link").Value;
                Process.Start(link);
            }
        }
    }
}
