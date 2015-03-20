﻿using System;
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

            #region Recovery exception buttons

            //Only use for special device exceptions (eg. sprint only has twrp avaliable)
            try
            {
                switch (Properties.Settings.Default["Device"].ToString())
                {
                    case "Sprint":
                        {
                            //TWRP
                            btnFlashRecovery1.Content = "Flash TWRP";
                            btnFlashRecovery1.IsEnabled = true;
                            //CWM
                            btnFlashRecovery2.Content = "Not Avaliable (CWM)";
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tBSelectedDevice.Text = Properties.Settings.Default["Device"].ToString();
            this.Title = this.Title + " Version: " + Assembly.GetEntryAssembly().GetName().Version;
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

        private void btnAutoRoot_Click(object sender, RoutedEventArgs e)
        {
            
            if (Properties.Settings.Default["Device"].ToString() == "AT-T GSM")
            {
                #region GSM Root
                MessageBoxResult messageResult = MessageBox.Show("You have chosen International GSM / AT&T. This will flash TWRP and push the SU.zip to your device. Do you want to continue?", "Confirm Selection", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    tBRootStatus.Text = "Rebooting to bootloader...";

                    Task.Delay(2000).ContinueWith(_ =>
                    {
                        ADB.Start();
                        ADB.Instance().Reboot(IDBoot.BOOTLOADER);

                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            tBRootStatus.Text = "Waiting for device...";
                        });

                        Task.Delay(10000).ContinueWith(a_ =>
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                tBRootStatus.Text = "Flashing recovery...";
                                Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery1.img");
                                Fastboot.Instance().Reboot(IDBoot.REBOOT);
                                tBRootStatus.Text = "Waiting for deivce...";
                            });

                            Task.Delay(40000).ContinueWith(b_ =>
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    ADB.Instance().Push("./Data/SU.zip", "/sdcard/su.zip");
                                    tBRootStatus.Text = "Rebooting Recovery...";
                                    ADB.Instance().Reboot(IDBoot.RECOVERY);
                                });
                            }
                            );
                        }
                        );
                    }
                    );
                }
                else if (messageResult == MessageBoxResult.No)
                {

                }
                #endregion
            }
            else if (Properties.Settings.Default["Device"].ToString() == "Sprint")
            {
                //Selected Spr
            }
            else if (Properties.Settings.Default["Device"].ToString() == "T-Mobile")
            {
                //Selected TM
            }
            else if (Properties.Settings.Default["Device"].ToString() == "Verizon")
            {
                //Selected Ver
            }
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

        private void btnUnlockCode_Click(object sender, RoutedEventArgs e)
        {
            IDDeviceState state = General.CheckDeviceState(ADB.Instance().DeviceID);
            if (state == IDDeviceState.DEVICE)
            {
                tBUnlockStatus.Text = "Rebooting into the bootloader...";
                ADB.Instance().Reboot(IDBoot.BOOTLOADER);

                using (StreamWriter sw = File.CreateText("./Data/token.txt"))
                {
                    List<string> _token = new List<string>();
                    foreach (string line in Fastboot.Instance().OEM.GetIdentifierToken())
                    {
                        GroupCollection groups = Regex.Match(line, @"^\(bootloader\)\s{1,}(?<PART>.*?)$").Groups;
                        string part = groups["PART"].Value;
                        if (String.IsNullOrEmpty(part) == false && Regex.IsMatch(part, @"^<{1,}.*?>{1,}$") == false)
                        {
                            _token.Add(part);
                        }
                    }

                    tBUnlockStatus.Text = "Collecting token...";

                    //the final string which u can write to an file
                    string token = String.Join("\n", _token.ToArray());
                    sw.WriteLine(token.ToString());

                    sw.WriteLine(" ");
                    sw.WriteLine("Please copy everything above this line!");
                    sw.WriteLine(" ");
                    sw.WriteLine("Next, sign into your HTC Dev account on the webpage that just opened.");
                    sw.WriteLine("If you do not have an account, create and activate an account with your email, then come back to this link.");
                    sw.WriteLine("http://www.htcdev.com/bootloader/unlock-instructions/page-3");
                    sw.WriteLine("Then, paste the Token ID you just copied at the bottom of the webpage.");
                    sw.WriteLine("Hit submit, and wait for the email with the unlock file.");
                    sw.WriteLine(" ");
                    sw.WriteLine("Once you have received the unlock file, download it and continue on to the next step, unlocking your bootloader.");
                    sw.WriteLine("This file is saved as token.txt in the Data folder if you need it in the future.");
                    sw.Close();
                }

                MessageBox.Show("The token is saved as token.txt in the Data folder. Further instructions are there. Please press OK to dismiss...");
                //Process.Start("./Data/token.txt");

                MessageBoxResult messageResult = MessageBox.Show("The package has been secured! Your unlock code is located '/Data/token.txt'. Would you like to reboot now?", "Token Obtained!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    Fastboot.Instance().Reboot(IDBoot.REBOOT);
                    Process.Start("http://www.htcdev.com/bootloader/unlock-instructions/page-3");
                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "/Data/token.txt");
                    MessageBox.Show("Next Step!", "Once you have recieved the unlock file from HTC, you can move on to the next step, unlocking your bootloader!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (messageResult == MessageBoxResult.No)
                {
                    Process.Start("http://www.htcdev.com/bootloader/unlock-instructions/page-3");
                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "/Data/token.txt");
                    MessageBox.Show("Next Step!", "Once you have recieved the unlock file from HTC, you can move on to the next step, unlocking your bootloader! More information is also avaliable in the /Data/token.txt file.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (state == IDDeviceState.FASTBOOT)
            {
                using (StreamWriter sw = File.CreateText("./Data/token.txt"))
                {
                    List<string> _token = new List<string>();
                    foreach (string line in Fastboot.Instance().OEM.GetIdentifierToken())
                    {
                        GroupCollection groups = Regex.Match(line, @"^\(bootloader\)\s{1,}(?<PART>.*?)$").Groups;
                        string part = groups["PART"].Value;
                        if (String.IsNullOrEmpty(part) == false && Regex.IsMatch(part, @"^<{1,}.*?>{1,}$") == false)
                        {
                            _token.Add(part);
                        }
                    }

                    tBUnlockStatus.Text = "Collecting token...";

                    //the final string which u can write to an file
                    string token = String.Join("\n", _token.ToArray());
                    sw.WriteLine(token.ToString());

                    sw.WriteLine(" ");
                    sw.WriteLine("Please copy everything above this line!");
                    sw.WriteLine(" ");
                    sw.WriteLine("Next, sign into your HTC Dev account on the webpage that just opened.");
                    sw.WriteLine("If you do not have an account, create and activate an account with your email, then come back to this link.");
                    sw.WriteLine("http://www.htcdev.com/bootloader/unlock-instructions/page-3");
                    sw.WriteLine("Then, paste the Token ID you just copied at the bottom of the webpage.");
                    sw.WriteLine("Hit submit, and wait for the email with the unlock file.");
                    sw.WriteLine(" ");
                    sw.WriteLine("Once you have received the unlock file, download it and continue on to the next step, unlocking your bootloader.");
                    sw.WriteLine("This file is saved as token.txt in the Data folder if you need it in the future.");
                    sw.Close();
                }

                MessageBox.Show("The token is saved as token.txt in the Data folder. Further instructions are there. Please press OK to dismiss...");
                //Process.Start("./Data/token.txt");

                MessageBoxResult messageResult = MessageBox.Show("The package has been secured! Your unlock code is located '/Data/token.txt'. Would you like to reboot now?", "Token Obtained!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    Fastboot.Instance().Reboot(IDBoot.REBOOT);
                    Process.Start("http://www.htcdev.com/bootloader/unlock-instructions/page-3");
                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "/Data/token.txt");
                    MessageBox.Show("Next Step!", "Once you have recieved the unlock file from HTC, you can move on to the next step, unlocking your bootloader!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (messageResult == MessageBoxResult.No)
                {
                    Process.Start("http://www.htcdev.com/bootloader/unlock-instructions/page-3");
                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "/Data/token.txt");
                    MessageBox.Show("Next Step!", "Once you have recieved the unlock file from HTC, you can move on to the next step, unlocking your bootloader! More information is also avaliable in the /Data/token.txt file.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("A device was not detected... Please ensure that you have the correct drivers configured and that they are working!");
            }

            //Add this later if needed...

            //Console con = Console.Instance;
            //con.Add(Fastboot.Instance().OEM.GetIdentifierToken());
            //con.Show();

        }

        private void btnHDevSignUp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.htcdev.com/register/");
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
            if (ADB.Instance().GetState() == IDDeviceState.DEVICE)
            {
                MessageBoxResult messageResult = MessageBox.Show("Your phone needs to be in 'fastboot USB' mode. Would you like to reboot into it now?", "Reboot to bootloader required!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    tBRecoveryStatus.Text = "Rebooting into Bootloader...";
                    ADB.Instance().Reboot(IDBoot.BOOTLOADER);
                    tBRootStatus.Text = "Waiting for device...";

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

        private void btnFlashRecovery2_Click(object sender, RoutedEventArgs e)
        {
            if (ADB.Instance().GetState() == IDDeviceState.DEVICE)
            {
                MessageBoxResult messageResult = MessageBox.Show("Your phone needs to be in 'fastboot USB' mode. Would you like to reboot into it now?", "Reboot to bootloader required!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    tBRecoveryStatus.Text = "Rebooting into Bootloader...";
                    ADB.Instance().Reboot(IDBoot.BOOTLOADER);
                    tBRootStatus.Text = "Waiting for device...";

                    Task.Delay(10000).ContinueWith(_ =>
                    {
                        tBRecoveryStatus.Text = "Flashing recovery...";
                        Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery2.img");

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
                Fastboot.Instance().Flash(IDDevicePartition.RECOVERY, "./Data/Recoveries/Recovery2.img");

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    tBRecoveryStatus.Text = "Rebooting...";
                    Fastboot.Instance().Reboot(IDBoot.REBOOT);
                });
            }
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
    }
}
