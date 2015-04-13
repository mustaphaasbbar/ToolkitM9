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
using System.Windows.Media.Animation;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for UnlockBootloader.xaml
    /// </summary>
    public partial class UnlockBootloader : Window
    {
        public UnlockBootloader()
        {
            InitializeComponent();
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
                MessageBox.Show("A device was not detected... Please ensure that you have the correct drivers configured and that they are working!", "Device is not detected...", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void btnHDevSignUp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.htcdev.com/register/");
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnUnlock_Click(object sender, RoutedEventArgs e)
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
                tbToken.Text = filename;
            }

            Main m = new Main();

            IDDeviceState state = General.CheckDeviceState(ADB.Instance().DeviceID);
            if (state == IDDeviceState.DEVICE)
            {
                ADB.Instance().Reboot(IDBoot.BOOTLOADER);

                MessageBoxResult messageResult = MessageBox.Show("Unlocking the bootloader will wipe all DATA on your phone. Are you sure you want to continue?", "Erase DATA!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    m.Add(Fastboot.Instance().Flash(IDDevicePartition.UNLOCKTOKEN, tbToken.Text));
                }
                if (messageResult == MessageBoxResult.No)
                {
                    //No
                }
            }
            else if (state == IDDeviceState.FASTBOOT)
            {
                MessageBoxResult messageResult = MessageBox.Show("Unlocking the bootloader will wipe all DATA on your phone. Are you sure you want to continue?", "Erase DATA!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (messageResult == MessageBoxResult.Yes)
                {
                    m.Add(Fastboot.Instance().Flash(IDDevicePartition.UNLOCKTOKEN, tbToken.Text));
                }
                if (messageResult == MessageBoxResult.No)
                {
                    //No
                }
            }
            else
            {
                MessageBox.Show("A device was not detected... Please ensure that you have the correct drivers configured and that they are working!", "Device is not detected...", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
    }
}
