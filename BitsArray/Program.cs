using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using FilesType;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BitsArray
{

    class Program
    {
        static void Main(string[] args)
        {
            image image = new image();
           // string FilePath = "C:\\Users\\Didi\\Desktop\\HelloWorldpng.png";
            //Bitmap bitmap = new Bitmap(FilePath);

            //Byte[] msg = File.ReadAllBytes("C:\\Users\\Didi\\Desktop\\Help.txt");
           // Bitmap ChangedImage = image.encryptImage(bitmap, msg, "txt");

            //ChangedImage.Save("C:\\Users\\Didi\\Desktop\\file.png", ImageFormat.Png);
            Tuple<byte[], string> info = image.decryptInfoFromFile("C:\\Users\\Didi\\Desktop\\file.png");

            File.WriteAllBytes("C:\\Users\\Didi\\Desktop\\Help123." + info.Item2, info.Item1);
            //Color c123 = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);
            //bitmap.SetPixel(bitmap.Width / 2, bitmap.Height / 2, Color.FromArgb(123, 132, 152, 111));
            //c123 = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);



            //Byte[] info = new byte[4 * infoString.Length];
            /*
            int j = 0;
            foreach (char c in infoString)
            {
                BitArray char1 = new BitArray(new int[] { System.Convert.ToInt32(c) });
                for(int i=0;i<4;++i)
                {
                    BitArray bitArray = new BitArray(8);
                    for (int k = 0; k < 8; ++k)
                        bitArray[k] = char1[i + k];
                    info[j + i] = ConvertToByte(bitArray);
                }
                j += 4;
            }
            */

            //Bitmap img = ChangeBitMap(bitmap, info);

            //img.Save("C:\\Users\\Didi\\Desktop\\file.png", ImageFormat.Png); // ImageFormat.Jpeg, etc

            //Byte[] arr1 = decryptDataFromBitmap(img);

            Console.ReadLine();
        }

        

    }
}
