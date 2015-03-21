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
using System.Windows.Shapes;

namespace ToolkitM9
{
    /// <summary>
    /// Interaction logic for Flash.xaml
    /// </summary>
    public partial class Flash : Window
    {
        public Flash()
        {
            InitializeComponent();
        }

        #region Nested type: Settings

        public static class Settings
        {
            public static string Selector = "null";
            public static bool File;
        }

        #endregion Nested type: Settings

    }
}
