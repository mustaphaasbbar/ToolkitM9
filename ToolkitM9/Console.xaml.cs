using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for CidDialog.xaml
    /// </summary>
    public partial class Console : Window
    {
        #region Instance

        private static Console _instance = null;

        public static Console Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                else
                {
                    _instance = new Console();
                    return _instance;
                }
            }
        }

        #endregion

        public Console()
        {
            InitializeComponent();
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.Closing += WindowClosing;
        }

        #region WindowClosing

        ///<summary>
        /// Clean exit
        ///</summary>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }
        #endregion

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
    }
}
