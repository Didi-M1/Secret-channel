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

namespace display
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        public Window1(byte[] info, byte[] infoAfterChange, bool flag)
        {
            Info = info;
            InfoAfterChange = infoAfterChange;
            Flag = flag;
            
            Image1.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + "Debugtemp1.jpg"));
            Image2.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + "Debugtemp2.jpg"));

        }

        public byte[] Info { get; }
        public byte[] InfoAfterChange { get; }
        public bool Flag { get; }
    }
}
