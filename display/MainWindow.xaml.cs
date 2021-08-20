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
        bool toSave = false;
        Bitmap imgMsg;
        image file;
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
            choosFileBtn_Click(sender,e);
            Tuple<byte[], string> infoFromFile = file.decryptInfoFromFile(filename);
            saveFile(infoFromFile);
        }

        private void saveFile(Tuple<byte[], string> infoFromFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                System.IO.File.WriteAllBytes(saveFileDialog.FileName +"."+ infoFromFile.Item2, infoFromFile.Item1);
        }

        private void saveFile(Bitmap image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                image.Save(saveFileDialog.FileName + ".png", ImageFormat.Png);
        }

        private void encryptFileBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Be awar, this is the max size of the encrypt msg: " + file.maxMessageSize(filename));

            string Msgfilename = OpenFile();
            Byte[] msg = System.IO.File.ReadAllBytes(Msgfilename);
            if (msg.Length > file.maxMessageSizeNumber(filename))
            {
                MessageBox.Show("sorry. msg too big.");
                return;
            }
            string[] arr = Msgfilename.Split('.');

            imgMsg = file.encryptImage(filename, msg, arr[arr.Length-1]);
            imgMsg.Save(".\\temp");
            Bitmap bitmap = new Bitmap(filename);

            Image1.Source = ConvertToImageSorce(bitmap);
            Image2.Source = ConvertToImageSorce(imgMsg);
            Image1.Visibility = Visibility.Visible;
            Image2.Visibility = Visibility.Visible;
            TextBlockAfter.Visibility = Visibility.Visible;
            TextBlockBefor.Visibility = Visibility.Visible;
            TextBlockBefor.Text += " -size:" +new FileInfo(filename).Length.ToString()+"B";
            TextBlockAfter.Text += " -size:" + new FileInfo(".\\temp").Length.ToString() + "B";

            cencelButton.Visibility = Visibility.Visible;
            saveButton.Visibility = Visibility.Visible;
            encryptFileBtn.Visibility = Visibility.Hidden;
            decryptFileBtn.Visibility = Visibility.Hidden;
            FileNameTextBlock.Visibility = Visibility.Hidden;

        }
        public BitmapImage ConvertToImageSorce(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveFile(imgMsg);

            Image1.Visibility = Visibility.Hidden;
            Image2.Visibility = Visibility.Hidden;
            TextBlockAfter.Visibility = Visibility.Hidden;
            TextBlockBefor.Visibility = Visibility.Hidden;
            saveButton.Visibility = Visibility.Hidden;
            TextBlockBefor.Text ="Befor";
            TextBlockAfter.Text = "After";


            encryptFileBtn.Visibility = Visibility.Visible;
            decryptFileBtn.Visibility = Visibility.Visible;
            FileNameTextBlock.Visibility = Visibility.Visible;

        }

        private void cencelButton_Click(object sender, RoutedEventArgs e)
        {
            Image1.Visibility = Visibility.Hidden;
            Image2.Visibility = Visibility.Hidden;
            TextBlockAfter.Visibility = Visibility.Hidden;
            TextBlockBefor.Visibility = Visibility.Hidden;
            cencelButton.Visibility = Visibility.Hidden;
            saveButton.Visibility = Visibility.Hidden;
            TextBlockBefor.Text = "Befor";
            TextBlockAfter.Text = "After";


            encryptFileBtn.Visibility = Visibility.Visible;
            decryptFileBtn.Visibility = Visibility.Visible;
            FileNameTextBlock.Visibility = Visibility.Visible;

        }
    }
    
}
