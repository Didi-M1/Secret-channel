using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using FilesType;
using Microsoft.Win32;

namespace display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MessageBox.Show("This software designed to \"Hide\" message file in a larger file. for now the software only supports images file(only .png).", "Administrative notice", MessageBoxButton.OK);
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ImageDE().Show();
            this.Close();
        }
    }

}
