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
    /// Interaction logic for ImageDE.xaml
    /// </summary>
    public partial class ImageDE : Window
    {
        Bitmap imgMsg;
        FilesType.Image file;
        string filename;
        bool firstTime = true;

        public ImageDE()
        {
            file = new FilesType.Image();
            InitializeComponent();
        }

        private void saveFile(Tuple<byte[], string> infoFromFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                System.IO.File.WriteAllBytes(saveFileDialog.FileName + "." + infoFromFile.Item2, infoFromFile.Item1);
        }

        private void saveFile(Bitmap image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                image.Save(saveFileDialog.FileName + ".png", ImageFormat.Png);
        }

        private string OpenFile(string type = null, string Default = null)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            if (type != null)
                dlg.Filter = type;
            if (Default != null)
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

        #region btns
        private void choosFileBtn_Click(object sender, RoutedEventArgs e)
        {
            filename = OpenFile("PNG Files (*.png)|*.png", ".png");


            if (filename != null)
            {
                // Open document 
                choosFileBtn.Visibility = Visibility.Hidden;
                encryptFileBtn.Visibility = Visibility.Visible;
                decryptFileBtn.Visibility = Visibility.Visible;
                FileNameTextBlock.Visibility = Visibility.Visible;
                changeFileBtn.Visibility = Visibility.Visible;
                FileNameTextBlock.Text = filename;
                choosFileBtn.Visibility = Visibility.Hidden;
            }
        }

        private void decryptFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!firstTime)
                choosFileBtn_Click(sender, e);
            firstTime = false;
            try
            { 
               
            Tuple<byte[], string> infoFromFile = file.decryptInfoFromFile(filename);
            MessageBox.Show("success!");
            saveFile(infoFromFile);
            }
            catch (ExceptionErrorInFileDycripting)
            {
                MessageBox.Show("Something went wrong, are you sure the file is really encrypted and not passed any compression(like sended in Whatsapp etc')?", "Error");
            }
            catch(ExceptionErrorInTypeDycripting ex)
            {
                if(ex.Message== "File Type Not Suported yet")
                    MessageBox.Show("Something went wrong, Probably the encrypted file is not supported yet. ", "Error");
                else
                    MessageBox.Show("Something went wrong, are you sure the file is really encrypted and not passed any compression(like sended in Whatsapp etc')?", "Error");
            }
        }

        private void encryptFileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!firstTime)
                choosFileBtn_Click(sender, e);
            firstTime = false;

            MessageBox.Show("Be awar, this is the max size of the encrypt message   " + file.maxMessageSize(filename));

            string Msgfilename = OpenFile();
            Byte[] msg = System.IO.File.ReadAllBytes(Msgfilename);
            if (msg.Length > file.maxMessageSizeNumber(filename))
            {
                MessageBox.Show("sorry. message size too big.");
                return;
            }


            string[] arr = Msgfilename.Split('.');
            try
            {
                imgMsg = file.encryptImageFile(filename, msg, arr[arr.Length - 1]);


                HideAndVisibleAfterEncrypt();
            }
            catch(Exception )
            {
                MessageBox.Show("Image file corrupt, Please try another.", "Error");
            }

        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveFile(imgMsg);
            hideSaveAndCeneclBtn();
        }

        private void cencelButton_Click(object sender, RoutedEventArgs e)
        {
            hideSaveAndCeneclBtn();
        }

        private void changeFileBtn_Click(object sender, RoutedEventArgs e)
        {
            choosFileBtn_Click(sender, e);
            firstTime = true;
        }

        #endregion

        #region converts and helpul function

        private void hideSaveAndCeneclBtn()
        {

            Image1.Visibility = Visibility.Hidden;
            Image2.Visibility = Visibility.Hidden;
            TextBlockAfter1.Visibility = Visibility.Hidden;
            TextBlockBefor.Visibility = Visibility.Hidden;
            cencelButton.Visibility = Visibility.Hidden;
            saveButton.Visibility = Visibility.Hidden;
            TextBlockBefor.Text = "Befor:";
            TextBlockAfter1.Text = "After:";

            encryptFileBtn.Visibility = Visibility.Visible;
            decryptFileBtn.Visibility = Visibility.Visible;
            FileNameTextBlock.Visibility = Visibility.Visible;
            changeFileBtn.Visibility = Visibility.Visible;


        }

        private void HideAndVisibleAfterEncrypt()
        {
            imgMsg.Save(System.IO.Directory.GetCurrentDirectory() + "\\temp.png");
            Bitmap bitmap = new Bitmap(filename);
            Image1.Source = ConvertToImageSorce(bitmap);
            Image2.Source = ConvertToImageSorce(imgMsg);
            Image1.Visibility = Visibility.Visible;
            Image2.Visibility = Visibility.Visible;
            TextBlockAfter1.Visibility = Visibility.Visible;
            TextBlockBefor.Visibility = Visibility.Visible;

            cencelButton.Visibility = Visibility.Visible;
            saveButton.Visibility = Visibility.Visible;
            encryptFileBtn.Visibility = Visibility.Hidden;
            decryptFileBtn.Visibility = Visibility.Hidden;
            FileNameTextBlock.Visibility = Visibility.Hidden;
            changeFileBtn.Visibility = Visibility.Hidden;

            float beforeSize = new FileInfo(filename).Length;
            float afterSize = new FileInfo(System.IO.Directory.GetCurrentDirectory() + "\\temp.png").Length;
            if (beforeSize > 1000000)
                TextBlockBefor.Text += " -size:" + (beforeSize / 1000000).ToString() + "MB";
            else
                TextBlockBefor.Text += " -size:" + beforeSize.ToString() + "B";
            if (afterSize > 1000000)
                TextBlockAfter1.Text += " -size:" + (afterSize / 1000000).ToString() + "MB";
            else
                TextBlockAfter1.Text += " -size:" + afterSize.ToString() + "B";
        }

        private BitmapImage ConvertToImageSorce(Bitmap src)
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
        #endregion

    }
}

