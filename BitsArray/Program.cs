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
        static int LengthMsg = 32;
        static void Main(string[] args)
        {
            string FilePath = "C:\\Users\\Didi\\Desktop\\HelloWorldpng.png";
            Bitmap bitmap = new Bitmap(FilePath);

            bitmap.MakeTransparent();
            ReversTransperntImg(ref bitmap);
            //Color c123 = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);
            //bitmap.SetPixel(bitmap.Width / 2, bitmap.Height / 2, Color.FromArgb(123, 132, 152, 111));
            //c123 = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);


            string infoString = "Hello there";

            Byte[] info = new byte[4 * infoString.Length];
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

            Bitmap img = ChangeBitMap(bitmap, info);

            img.Save("C:\\Users\\Didi\\Desktop\\file.png", ImageFormat.Png); // ImageFormat.Jpeg, etc

            Byte[] arr1 = decryptDataFromBitmap(img);
            
            Console.ReadLine();
        }

        private static void ReversTransperntImg(ref Bitmap bitmap)
        {
            int h = bitmap.Height, w = bitmap.Width;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Color c = bitmap.GetPixel(j, i);
                    if(c.A==0)
                        bitmap.SetPixel(j, i, Color.FromArgb(255, c));
                }
            }
        }

        private static byte[] decryptDataFromBitmap(Bitmap img)
        {
            BitArray helperToGetData = new BitArray(4);
            BitArray LengthOfMsgInBits = new BitArray(32);

            int i = 0, j = 0;
            int Width = img.Width, Height = img.Height;
            int posionInImg = 0;
            for (; i < Width; ++i)
            {
                for (; j < Height; ++j)
                {
                    if (LengthMsg <= posionInImg)
                        break;
                    Color color = img.GetPixel(i, j);
                    getsBitsFromImg(ref helperToGetData, color);
                    putBitsInArr(ref LengthOfMsgInBits, helperToGetData, j+(i*32));
                    posionInImg += 4;

                }
                if (LengthMsg <= posionInImg)
                    break;
            }


            int[] convertHelper = new int[1];
            LengthOfMsgInBits.CopyTo(convertHelper, 0);
            int exsratLengthMsg = convertHelper[0]*8;


            

            BitArray MsgInBits = new BitArray(exsratLengthMsg);

            for (; i < Width; ++i)
            {
                for (; j < Height; ++j)
                {
                    if (exsratLengthMsg + LengthMsg <= posionInImg)
                        break;
                    Color color = img.GetPixel(i, j);
                    getsBitsFromImg(ref helperToGetData, color);
                    putBitsInArr(ref MsgInBits, helperToGetData, j+ (i * Height)-8);
                    posionInImg += 4;

                }
                if (exsratLengthMsg + LengthMsg <= posionInImg)
                    break;
            }


            return getDataFromBitArray(MsgInBits);
        }

        private static byte[] getDataFromBitArray(BitArray msgInBits)
        {
            int lengthInByte = msgInBits.Length / 8;
            byte[] data = new byte[lengthInByte];
            BitArray convertHelper = new BitArray(8);
            for(int i=0;i< lengthInByte;++i)
            {
                ConvertTo8Bit(msgInBits, ref convertHelper, i);
                data[i] = ConvertToByte(convertHelper);
            }
            return data;
        }

        private static void ConvertTo8Bit(BitArray msgInBits, ref BitArray convertHelper, int i)
        {
            convertHelper[0] = msgInBits[i * 8];
            convertHelper[1] = msgInBits[i * 8+1];
            convertHelper[2] = msgInBits[i * 8+2];
            convertHelper[3] = msgInBits[i * 8+3];
            convertHelper[4] = msgInBits[i * 8+4];
            convertHelper[5] = msgInBits[i * 8+5];
            convertHelper[6] = msgInBits[i * 8+6];
            convertHelper[7] = msgInBits[i * 8+7];
        }

        static byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }


        private static void putBitsInArr(ref BitArray lengthOfMsgInBits, BitArray helperToGetData, int i)
        {
            lengthOfMsgInBits[(i*4)] = helperToGetData[0];
            lengthOfMsgInBits[(i * 4) + 1] = helperToGetData[1];
            lengthOfMsgInBits[(i * 4) + 2] = helperToGetData[2];
            lengthOfMsgInBits[(i * 4) + 3] = helperToGetData[3];
        }

        private static void getsBitsFromImg(ref BitArray helperToGetData, Color color)
        {
            helperToGetData[0]= getChangeColor(color.A);
            helperToGetData[1]= getChangeColor(color.R);
            helperToGetData[2]= getChangeColor(color.B);
            helperToGetData[3]= getChangeColor(color.G);
        }
        public bool IsAlphaBitmap(ref System.Drawing.Imaging.BitmapData BmpData)
        {
            byte[] Bytes = new byte[BmpData.Height * BmpData.Stride];
            Marshal.Copy(BmpData.Scan0, Bytes, 0, Bytes.Length);
            for (int p = 3; p < Bytes.Length; p += 4)
            {
                if (Bytes[p] != 255) return true;
            }
            return false;
        }
        private static Bitmap ChangeBitMap(Bitmap bitmap, Byte[] info)
        {

            int i = 0;
            int j = 0;
            BitArray bitsInfo = new BitArray(info);

            int length = bitsInfo.Length;
            int posionInInfo = 0;

            BitArray lengthOfMessage = new BitArray(new int[] { info.Length }); // the Length of the message in bitArray
            int posionInLength = 0;


            ChangeImageFromInfo(ref i, ref j, posionInLength, ref bitmap, lengthOfMessage, LengthMsg);

            ChangeImageFromInfo(ref i, ref j, posionInInfo, ref bitmap, bitsInfo, length);

            return bitmap;
        }


        private static void ChangeImageFromInfo(ref int i, ref int j, int posionInInfo,ref Bitmap bitmap, BitArray lengthOfMessage, int LengthOfData)
        {
            BitArray HelpBitArrayToChange = new BitArray(4);
            int Width = bitmap.Width;
            int Height = bitmap.Height;

            for (; i < Width; ++i)
            {
                for (; j < Height; ++j)
                {
                    if (LengthOfData <= posionInInfo)
                        break;
                    getsBitsFromINfo(ref HelpBitArrayToChange, lengthOfMessage, posionInInfo);

                    Color color = bitmap.GetPixel(i, j);
                    color = ChangeArgbToInfo(color, HelpBitArrayToChange);
                    bitmap.SetPixel(i, j, color);


                    posionInInfo += 4;
                }
                if (LengthOfData <= posionInInfo)
                    break;
            }
        }

        private static void getsBitsFromINfo(ref BitArray helpBitArrayToChange, BitArray bitsInfo, int posionInInfo)
        {
            helpBitArrayToChange[0] = bitsInfo[posionInInfo];
            helpBitArrayToChange[1] = bitsInfo[posionInInfo+1];
            helpBitArrayToChange[2] = bitsInfo[posionInInfo+2];
            helpBitArrayToChange[3] = bitsInfo[posionInInfo+3];
        }

        private static Color ChangeArgbToInfo(Color color, BitArray bitsArrayInfo )
        {
            int alfa = color.A;
            int red = color.R;
            int blue = color.B;
            int green = color.G;
            ChangeColor(ref alfa, bitsArrayInfo[0]);
            ChangeColor(ref red, bitsArrayInfo[1]);
            ChangeColor(ref blue, bitsArrayInfo[2]);
            ChangeColor(ref green, bitsArrayInfo[3]);

            return Color.FromArgb(alfa, red, green, blue);
        }

        private static void ChangeColor(ref int num, bool v)
        {
            if (v)
            {
                if (num % 2 == 0)
                    num += 1;
            }
            else
                if (num % 2 == 1)
                num -= 1;
        }
        private static bool getChangeColor(int num)
        {
            if (num % 2 == 0)
                return false;
            else
                return true;
        }
    }
}
