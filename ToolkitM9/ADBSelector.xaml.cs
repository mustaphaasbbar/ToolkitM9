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
using AndroidCtrl.Fastboot;
using AndroidCtrl.Tools;
using AndroidCtrlUI.Explorer;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for ADBSelector.xaml
    /// </summary>
    public partial class ADBSelector : Window
    {
        FileSystemViewModel _explorer;


        public ADBSelector()
        {
            InitializeComponent();

            AndroidCtrlUI.Tools.Language.AutoLoad();

            _explorer = new FileSystemViewModel();

            this.DataContext = _explorer;

            ADB.Start();
        }

        #region Nested type: Settings

        public static class Settings
        {
            public static string Selector = "null";
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Settings.Selector)
                {
                    case "Push":
                        {
                            this.Title = (this.Title + " - Push");
                            tbNotes.Text = "This will push a file from your computer to your device. Please use '/' for paths: eg. '/data/local/tmp/'.";
                            btnGo.Content = "Push";
                        }
                        break;

                    case "Pull":
                        {
                            this.Title = (this.Title + " - Pull");
                            tbNotes.Text = "This will push a pull from your device to your computer. Please use '/' for paths: eg. '/data/local/tmp/file.ext'.";
                            btnGo.Content = "Pull";
                        }
                        break;

                    case "Sideload":
                        {
                            this.Title = (this.Title + " - Sideload");
                            tbNotes.Text = "Select a zip to sideload to your device!";
                            this.Height = (175);
                            btnGo.Content = "Sideload";
                            tbRemotePath.IsEnabled = false;
                            ccAddressBarExt.IsEnabled = false;
                            ccTreeView.IsEnabled = false;
                            ccListView.IsEnabled = false;
                            ccListViewInfo.IsEnabled = false;
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

        private void btnBrowseLocal_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = "";
            dlg.Filter = "All Files (*.*)|*.*";
            dlg.Title = "Locate Files";

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

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (Settings.Selector)
                {
                    case "Push":
                        {
                            if (tbFilepath.Text == "")
                            {
                                MessageBox.Show("You must select a file to push to your device! Press Browse to select a file.", "Empty file location", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                ADB.Instance().Push(tbFilepath.Text, tbRemotePath.Text);
                            }
                        }
                        break;

                    case "Pull":
                        {
                            if (tbFilepath.Text == "")
                            {
                                MessageBox.Show("You must select a file to push to your device! Press Browse to select a file.", "Empty file location", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                ADB.Instance().Pull(tbRemotePath.Text, tbFilepath.Text);
                            }
                        }
                        break;

                    case "Sideload":
                        {
                            if (tbFilepath.Text == "")
                            {
                                MessageBox.Show("You must select a file to push to your device! Press Browse to select a file.", "Empty file location", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                ADB.Instance().Sideload(tbFilepath.Text);
                            }
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _explorer.ListView.Refresh();
        }
    }
}
