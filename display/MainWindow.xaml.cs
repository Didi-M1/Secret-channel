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
using FilesType;
using Microsoft.Win32;

namespace display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Files file;
        string filename;
        public MainWindow()
        {
            file = new image();
            InitializeComponent();
        }

        private string OpenFile(string type =null, string Default = null)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            if (type != null)
                dlg.Filter = type;
            if(Default!= null)
                dlg.DefaultExt = Default;


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                return dlg.FileName;
            }
            return null;
        }

            private void choosFileBtn_Click(object sender, RoutedEventArgs e)
        {
            filename = OpenFile("JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif", ".jpg");


                if (filename != null)
            { 
                // Open document 
                choosFileBtn.Visibility = Visibility.Hidden;
                encryptFileBtn.Visibility = Visibility.Visible;
                decryptFileBtn.Visibility = Visibility.Visible;
                FileNameTextBlock.Visibility = Visibility.Visible;
                FileNameTextBlock.Text = filename;
                choosFileBtn.Visibility = Visibility.Hidden;
            }
        }
        

        private void decryptFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Byte[] info = System.IO.File.ReadAllBytes(filename);

            Tuple<byte[], string> infoFromFile = file.decryptInfoFromFile(info);
            if (infoFromFile.Item2 == "string")
            {
                msgTextBlock.Text = file.getStringFromData(infoFromFile.Item1, infoFromFile.Item1.Length * 8);
                msgTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                saveFile(infoFromFile);
            }
        }

        private void saveFile(Tuple<byte[], string> infoFromFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                System.IO.File.WriteAllBytes(saveFileDialog.FileName +"."+ infoFromFile.Item2, infoFromFile.Item1);
        }
    

        private void encryptFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Byte[] info = System.IO.File.ReadAllBytes(filename);
            MessageBox.Show("Be awar, this is the mac size of the encrypt msg: " + file.maxMessageSize(info));

            string Msgfilename = OpenFile();
            Byte[] msg = System.IO.File.ReadAllBytes(Msgfilename);
            string[] arr = Msgfilename.Split('.');
            bool Flag = true;
            Byte[] infoAfterChange = file.encryptInfoInFile(info, msg, arr[arr.Length-1]);
            //new Window1(info, infoAfterChange,Flag).Show();
            //    if(Flag)
            System.IO.File.WriteAllBytes(filename, infoAfterChange);

        }
    }
    
}
