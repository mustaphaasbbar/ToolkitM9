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
using System.Xml.Linq;
using System.Reflection;

using AndroidCtrl;
using AndroidCtrl.ADB;
using AndroidCtrl.AAPT;
using AndroidCtrl.Tools;
using AndroidCtrl.Signer;
using AndroidCtrl.Fastboot;

using wyUpdate;
using System.Text.RegularExpressions;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            #region adbFilter List

            string[] adblist =
                this.FindResource("ADBCmdList") as string[];

            ICollectionView ADBview =
                CollectionViewSource.GetDefaultView(adblist);

            new TextSearchFilter(ADBview, this.adbSearch);

            string[] fbtlist =
                this.FindResource("FBTCmdList") as string[];

            ICollectionView FBTview =
                CollectionViewSource.GetDefaultView(fbtlist);

            new TextSearchFilter(FBTview, this.fbtSearch);

            #endregion



            #region Recovery buttons

            //Only use for special device exceptions (eg. sprint only has twrp avaliable)
            try
            {
                switch (Properties.Settings.Default["Device"].ToString())
                {
                    case "Sprint":
                        {
                            //Getting values local xml
                            XDocument docL = XDocument.Load("./Data/Recoveries/rVersion1.xml");
                            var nameL = docL.Root.Element("Name").Value;

                            //TWRP
                            btnFlashRecovery1.Content = nameL.ToString();
                            btnFlashRecovery1.IsEnabled = true;
                            //CWM
                            btnFlashRecovery2.Content = "Not Avaliable";
                            btnFlashRecovery2.IsEnabled = false;
                        }
                        break;

                    case "GSM":
                        {
                            //Getting values local xml
                            XDocument docL = XDocument.Load("./Data/Recoveries/rVersion1.xml");
                            var nameL = docL.Root.Element("Name").Value;

                            //TWRP
                            btnFlashRecovery1.Content = nameL.ToString();
                            btnFlashRecovery1.IsEnabled = true;
                            //CWM
                            btnFlashRecovery2.Content = "Not Avaliable";
                            btnFlashRecovery2.IsEnabled = false;
                        }
                        break;

                    case "AT&T":
                        {
                            //Getting values local xml
                            XDocument docL = XDocument.Load("./Data/Recoveries/rVersion1.xml");
                            var nameL = docL.Root.Element("Name").Value;

                            //TWRP
                            btnFlashRecovery1.Content = nameL.ToString();
                            btnFlashRecovery1.IsEnabled = true;
                            //CWM
                            btnFlashRecovery2.Content = "Not Avaliable";
                            btnFlashRecovery2.IsEnabled = false;
                        }
                        break;

                    case "T-Mobile":
                        {
                            //Getting values local xml
                            XDocument docL = XDocument.Load("./Data/Recoveries/rVersion1.xml");
                            var nameL = docL.Root.Element("Name").Value;

                            //TWRP
                            btnFlashRecovery1.Content = nameL.ToString();
                            btnFlashRecovery1.IsEnabled = true;
                            //CWM
                            btnFlashRecovery2.Content = "Not Avaliable";
                            btnFlashRecovery2.IsEnabled = false;
                        }
                        break;

                    case "Verizon":
                        {
                            //Getting values local xml
                            XDocument docL = XDocument.Load("./Data/Recoveries/rVersion1.xml");
                            var nameL = docL.Root.Element("Name").Value;

                            //TWRP
                            btnFlashRecovery1.Content = nameL.ToString();
                            btnFlashRecovery1.IsEnabled = true;
                            //CWM
                            btnFlashRecovery2.Content = "Not Avaliable";
                            btnFlashRecovery2.IsEnabled = false;
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
            #endregion

            #region ADB Start + Updates
            //Set menu item for update checking
            autoUpdate.MenuItem = muiUpdates;

            // Check if ADB is running
            if (ADB.IsStarted())
            {
                // Force Stop ADB
                ADB.Stop(true);

                //Start ADB
                ADB.Start();
            }
            else
            {
                // Start ADB
                ADB.Start();
            }
            #endregion

            #region StartDevice Detection Service (DDS)
            DeviceDetectionService();
            #endregion
        }

        #region Add

        public void Add(List<string> msg)
        {
            foreach (string tmp in msg)
            {
                Output.Document.Blocks.Add(new Paragraph(new Run(tmp.Replace("(bootloader) ", ""))));
            }
            Output.ScrollToEnd();
        }
        #endregion

        #region Clear

        public void Clear()
        {
            Output.Document.Blocks.Clear();
        }
        #endregion

        public class TextSearchFilter
        {
            public TextSearchFilter(ICollectionView filteredview, TextBox textbox)
            {
                string filterText = "";
                filteredview.Filter = delegate(object obj)
                {
                    if (string.IsNullOrEmpty(filterText))
                        return true;

                    string str = obj as string;
                    if (string.IsNullOrEmpty(str))
                        return false;

                    int index = str.IndexOf(filterText, 0, StringComparison.InvariantCultureIgnoreCase);

                    return index > -1;
                };

                textbox.TextChanged += delegate
                {
                    filterText = textbox.Text;
                    filteredview.Refresh();
                };

            }
        }

        //BGw on Load declared
        private readonly BackgroundWorker wLoad = new BackgroundWorker();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tBSelectedDevice.Text = Properties.Settings.Default["Device"].ToString();
            tbVersion.Text = " Version: " + Assembly.GetEntryAssembly().GetName().Version;

            //Remote Version
            XDocument doc = XDocument.Load("https://s.basketbuild.com/dl/devs?dl=squabbi/m9/toolkit/News.xml");
            var text = doc.Root.Element("Text").Value;

            muiTbNews.Text = text.ToString();
        }

        private static string GetStringBetween(string source, string start, string end)
        {
            int startIndex = source.IndexOf(start, StringComparison.InvariantCulture) + start.Length;
            int endIndex = source.IndexOf(end, startIndex, StringComparison.InvariantCulture);
            int length = endIndex - startIndex;
            return source.Substring(startIndex, length);
        }

        public void CheckDeviceState(List<DataModelDevicesItem> devices)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                // Here we refresh our combobox
                SetDeviceList();
            });
        }

        private void SetDeviceList()
        {
            string active = String.Empty;
            if (deviceselector.Items.Count != 0)
            {
                active = ((DataModelDevicesItem)deviceselector.SelectedItem).Serial;
            }

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                // Here we refresh our combobox
                deviceselector.Items.Clear();
            });

            // This will get the currently connected ADB devices
            List<DataModelDevicesItem> adbDevices = ADB.Devices();

            // This will get the currently connected Fastboot devices
            List<DataModelDevicesItem> fastbootDevices = Fastboot.Devices();

            foreach (DataModelDevicesItem device in adbDevices)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    // here goes the add command ;)
                    deviceselector.Items.Add(device);
                });
            }
            foreach (DataModelDevicesItem device in fastbootDevices)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    deviceselector.Items.Add(device);
                });
            }
            if (deviceselector.Items.Count != 0)
            {
                int i = 0;
                bool empty = true;
                foreach (DataModelDevicesItem device in deviceselector.Items)
                {
                    if (device.Serial == active)
                    {
                        empty = false;
                        deviceselector.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                if (empty)
                {

                    // This calls will select the BASE class if we have no connected devices
                    ADB.SelectDevice();
                    Fastboot.SelectDevice();


                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        deviceselector.SelectedIndex = 0;
                    });


                }
            }
        }

        private void DeviceDetectionService()
        {
            ADB.Start();

            // Here we initiate the BASE Fastboot instance
            Fastboot.Instance();

            //This will starte a thread which checks every 10 sec for connected devices and call the given callback
            if (Fastboot.ConnectionMonitor.Start())
            {
                //Here we define our callback function which will be raised if a device connects or disconnects
                Fastboot.ConnectionMonitor.Callback += ConnectionMonitorCallback;

                // Here we check if ADB is running and initiate the BASE ADB instance (IsStarted() will everytime check if the BASE ADB class exists, if not it will create it)
                if (ADB.IsStarted())
                {
                    //Here we check for connected devices
                    SetDeviceList();

                    //This will starte a thread which checks every 10 sec for connected devices and call the given callback
                    if (ADB.ConnectionMonitor.Start())
                    {
                        //Here we define our callback function which will be raised if a device connects or disconnects
                        ADB.ConnectionMonitor.Callback += ConnectionMonitorCallback;
                    }
                }
            }
        }

        public void ConnectionMonitorCallback(object sender, ConnectionMonitorArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                // Do what u want with the "List<DataModelDevicesItem> e.Devices"
                // The "sender" is a "string" and returns "adb" or "fastboot"
                SetDeviceList();
                
            });
        }

        private void SelectDeviceInstance(object sender, SelectionChangedEventArgs e)
        {
            if (deviceselector.Items.Count != 0)
            {
                DataModelDevicesItem device = (DataModelDevicesItem)deviceselector.SelectedItem;

                // This will select the given device in the Fastboot and ADB class
                Fastboot.SelectDevice(device.Serial);
                ADB.SelectDevice(device.Serial);
            }
        }

        private void muiExit_Click(object sender, RoutedEventArgs e)
        {
            ADB.Stop(true);

            //Closes the form
            this.Close();
        }

        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
                using (BinaryReader bR = new BinaryReader(s))
                    using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
                        using (BinaryWriter bW = new BinaryWriter(fs))
                            bW.Write(bR.ReadBytes((int)s.Length));
        }

        private void btnRoot_Click(object sender, RoutedEventArgs e)
        {          
            if (!File.Exists("SuperSU.zip"))
            {
                tBRootStatus.Text = "Extracting SuperSU.zip";

                Extract("ToolkitM9", "./Data", "Resources", "SuperSU.zip");
            }

            this.Add(ADB.Instance().Push("./Data/SuperSU.zip", "/sdcard/SuperSU.zip"));
            tBRootStatus.Text = "Rebooting into Recovery...";
            this.Add(ADB.Instance().Reboot(IDBoot.RECOVERY));
            MessageBox.Show("From your recovery, you will now flash the SuperSU.zip that has been placed in the ROOT of your INTERNAL STORAGE!", "Recovery time!", MessageBoxButton.OK, MessageBoxImage.Information);
            tBRootStatus.Text = "Idle...";
        }

        private void btnInstalledPrograms_Click(object sender, RoutedEventArgs e)
        {
            var cplPath = System.IO.Path.Combine(Environment.SystemDirectory, "control.exe");
            System.Diagnostics.Process.Start(cplPath, "/name Microsoft.ProgramsAndFeatures");
        }

        private void btnDeviceMan_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("devmgmt.msc");
        }

        private void muiHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("ToolkitM9.chm");
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

        private void muiChange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please restart the application after it has finished downloading your selected device's recoveries to see effects.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
            var selector = new DeviceSelector();
            selector.ShowDialog();
        }

        private void btnFlashRecovery1_Click(object sender, RoutedEventArgs e)
        {
            ToolkitM9.RVersion.Settings.Recovery = "1";

            RVersion rec = new RVersion();
            rec.ShowDialog();
        }

        private void btnFlashRecovery2_Click(object sender, RoutedEventArgs e)
        {
            ToolkitM9.RVersion.Settings.Recovery = "2";

            RVersion rec = new RVersion();
            rec.ShowDialog();       
        }

        private void muiDrivers_Click(object sender, RoutedEventArgs e)
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
                MessageBoxResult messageResult = MessageBox.Show("You already have CWM's Universal ADB Drivers installed. Would you like to download and install now?", "ADB Drivers", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    var download = new Downloader();
                    ToolkitM9.Downloader.Settings.Selector = "ADB";
                    download.ShowDialog();
                }
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
                MessageBoxResult messageResult = MessageBox.Show("You already have HTC Drivers installed! Would you like to download and install now?", "HTC Drivers", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    var download = new Downloader();
                    ToolkitM9.Downloader.Settings.Selector = "HTC";
                    download.ShowDialog();
                }
            }
            #endregion
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ADB.Stop();
            ADB.Dispose(true);
            Fastboot.Dispose();  
        }

        private void muiClearLogs_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete all log files?", "Confirm selection...", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.Yes)
            {
                foreach (FileInfo f in new DirectoryInfo("./Data/Logs/").GetFiles("*.txt"))
                {
                    f.Delete();
                }
            }
            if (messageResult == MessageBoxResult.No)
            {
                //No delete
            }
        }

        private void btnFBTGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (lbFastboot.SelectedValue.ToString())
                {
                    case "Flash":
                        {
                            var fbtsel = new FastbootSelector();
                            ToolkitM9.FastbootSelector.Settings.Selector = "Flash";
                            fbtsel.ShowDialog();
                        }
                        break;

                    case "Erase":
                        {
                            var fbtsel = new FastbootSelector();
                            ToolkitM9.FastbootSelector.Settings.Selector = "Erase";
                            fbtsel.ShowDialog();
                        }
                        break;

                    case "Boot":
                        {
                            var fbtsel = new FastbootSelector();
                            ToolkitM9.FastbootSelector.Settings.Selector = "Boot";
                            fbtsel.ShowDialog();
                        }
                        break;

                    case "Reboot":
                        {
                            this.Add(Fastboot.Instance().Reboot(IDBoot.REBOOT));
                        }
                        break;

                    case "Reboot Bootloader":
                        {
                            this.Add(Fastboot.Instance().Reboot(IDBoot.BOOTLOADER));
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

        private void btnADBGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (lbADB.SelectedValue.ToString())
                {
                    case "Reboot":
                        {
                            this.Add(ADB.Instance().Reboot(IDBoot.REBOOT));
                        }
                        break;

                    case "Reboot Bootloader":
                        {
                            this.Add(ADB.Instance().Reboot(IDBoot.BOOTLOADER));
                        }
                        break;

                    case "Reboot Recovery":
                        {
                            this.Add(ADB.Instance().Reboot(IDBoot.RECOVERY));
                        }
                        break;

                    case "Push":
                        {
                            ToolkitM9.ADBSelector.Settings.Selector = "Push";
                            var adbsel = new ADBSelector();
                            adbsel.ShowDialog();
                        }
                        break;

                    case "Sideload":
                        {
                            ToolkitM9.ADBSelector.Settings.Selector = "Sideload";
                            var adbsel = new ADBSelector();
                            adbsel.ShowDialog();
                        }
                        break;

                    case "Pull":
                        {
                            ToolkitM9.ADBSelector.Settings.Selector = "Pull";
                            var adbsel = new ADBSelector();
                            adbsel.ShowDialog();
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

        private void outClear_Click(object sender, RoutedEventArgs e)
        {
            this.Clear();
        }

        private void btnUnlockBL_Click(object sender, RoutedEventArgs e)
        {
            UnlockBootloader ulbl = new UnlockBootloader();
            ulbl.ShowDialog();
        }

    }
}
