using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesType
{
    /**
     *
     Work description:
     
     *Step one - transfer the image to bitMap format.
     *
     *Step two - Make sure there is an alpha channel to the img with the makeTransparnt function.
     *
     *Step three - Change the lsb of the ARGB per pixel and save information 
     *from our message in the following order - size (32b), file type (8b) and then the message.
     *
     *Step four - Do everything revers and decrypt the message from our bitMap. 
     */

    public class image
    {
        int LengthMsg = 32;
        int sizeOfType = 8;
        public Bitmap encryptImage(string filename, Byte[] info,string type)
        {
            Bitmap bitmap = new Bitmap(filename);
            bitmap.MakeTransparent();
            ReversTransperntImg(ref bitmap);
            int i = 0;
            int j = 0;


            BitArray bitsInfo = new BitArray(info);

            BitArray lengthOfMessage = new BitArray(new int[] { info.Length }); // the Length of the message in bitArray

            BitArray typeBitArray = putTypeOFMsgInBit(type); // the type of the message in bitArray
             



            int length = bitsInfo.Length;


            ChangeImageFromInfo(ref i, ref j, 0, ref bitmap, lengthOfMessage, LengthMsg);

            ChangeImageFromInfo(ref i, ref j, 0, ref bitmap, typeBitArray, sizeOfType);

            ChangeImageFromInfo(ref i, ref j, 0, ref bitmap, bitsInfo, length);

            return bitmap;
        }

        private BitArray putTypeOFMsgInBit(string type)
        {
            bool[] arr;
            if (type == "png")
                arr = new bool[] { true, true, true, true, true, true, true, true };
            else if (type == "txt")
                arr = new bool[] { true, true, true, true, true, true, true, false };
            else
                arr = new bool[] { false, false, false, false, false, false, false, false };
            return convertBoolToBitArray(arr);
        }

        public Tuple<byte[], string> decryptInfoFromFile(string filePath)
        {
            Bitmap img = new Bitmap(filePath);

            int i = 0, j = 0;
            int posionInImg = 0;

            BitArray LengthOfMsgInBits = getChangesBitFromImg(img, ref i, ref j,ref posionInImg, LengthMsg);
            int[] convertHelper = new int[1];
            LengthOfMsgInBits.CopyTo(convertHelper, 0);
            int exsratLengthMsg = convertHelper[0] * 8;


            BitArray typeInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, sizeOfType);
            bool[] boolArrType = convertBitArrayToBool(typeInBits);
            string type = getType(boolArrType);



            BitArray MsgInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, exsratLengthMsg);
            byte[] dataInByteArr = getDataFromBitArray(MsgInBits);



            return new Tuple<byte[], string>(dataInByteArr, type);

        }

        public string maxMessageSize(string filename)
        {
            return maxMessageSizeNumber(filename).ToString() + "B";
        }

        public int maxMessageSizeNumber(string filename)
        {
            Bitmap img = new Bitmap(filename);

            int w = img.Width;
            int h = img.Height;
             return ((w * h - 10) * 4);
        }

        private bool[] convertBitArrayToBool(BitArray typeInBits)
        {
            bool[] arr = new bool[8];
            for (int i = 0; i < 8; ++i)
                arr[i] = typeInBits[i];
            return arr;
        }
        private BitArray convertBoolToBitArray(bool[] typeInbool)
        {
            BitArray typeInBits = new BitArray(sizeOfType);
            for (int i = 0; i < 8; ++i)
                typeInBits[i] = typeInbool[i];
            return typeInBits;
        }

        private string getType(bool[] typeInBits)
        {

            string str = "";
            if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, true, true, true }))
                str = "png";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, true, true, false }))
                str = "txt";
            else if ((Enumerable.SequenceEqual(typeInBits, new bool[] { false, false, false, false, false, false, false, false })))
                throw new Exception("File Type Not Suported");
            
            return str;        }

        private BitArray getChangesBitFromImg(Bitmap img,ref int i,ref int j, ref int posionInImg, int lengthMsg)
        {
            BitArray helperToGetData = new BitArray(4);
            BitArray MsgInBits = new BitArray(lengthMsg);
            int Width = img.Width, Height = img.Height;
            int previusOfPosionInImg = posionInImg;
            for (; i < Width; ++i)
            {
                for (; j < Height; ++j)
                {
                    if (previusOfPosionInImg+ lengthMsg <= posionInImg)
                        break;
                    Color color = img.GetPixel(i, j);
                    getsBitsFromImg(ref helperToGetData, color);
                    putBitsInArr(ref MsgInBits, helperToGetData, j*4 + (i * Height*4)- previusOfPosionInImg);
                    posionInImg += 4;

                }
                if (previusOfPosionInImg + lengthMsg <= posionInImg)
                    break;
                else
                    j = 0;
            }
            return MsgInBits;
        }

        private static void ConvertTo8Bit(BitArray msgInBits, ref BitArray convertHelper, int i)
        {
            for(int j =0;j<8;++j)
                convertHelper[j] = msgInBits[(i * 8) + j];
        }
        private static void putBitsInArr(ref BitArray lengthOfMsgInBits, BitArray helperToGetData, int i)
        {
            for(int j=0;j<4;++j)
                lengthOfMsgInBits[(i)+j] = helperToGetData[j];
        }
        private static void getsBitsFromINfo(ref BitArray helpBitArrayToChange, BitArray bitsInfo, int posionInInfo)
        {
            for (int j = 0; j < 4; ++j)
            {
                helpBitArrayToChange[j] = bitsInfo[posionInInfo + j];
            }
        }


        private static void ChangeImageFromInfo(ref int i, ref int j, int posionInInfo, ref Bitmap bitmap, BitArray lengthOfMessage, int LengthOfData)
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
                else
                    j = 0;
            }
        }


        private static Color ChangeArgbToInfo(Color color, BitArray bitsArrayInfo)
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
        private static void getsBitsFromImg(ref BitArray helperToGetData, Color color)
        {
            helperToGetData[0] = getChangeColor(color.A);
            helperToGetData[1] = getChangeColor(color.R);
            helperToGetData[2] = getChangeColor(color.B);
            helperToGetData[3] = getChangeColor(color.G);
        }
        private byte[] getDataFromBitArray(BitArray msgInBits)
        {
            int lengthInByte = msgInBits.Length / 8;
            byte[] data = new byte[lengthInByte];
            BitArray convertHelper = new BitArray(8);
            for (int i = 0; i < lengthInByte; ++i)
            {
                ConvertTo8Bit(msgInBits, ref convertHelper, i);
                data[i] = ConvertToByte(convertHelper);
            }
            return data;
        }


        private static void ReversTransperntImg(ref Bitmap bitmap)
        {
            int h = bitmap.Height, w = bitmap.Width;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Color c = bitmap.GetPixel(j, i);
                    if (c.A == 0)
                        bitmap.SetPixel(j, i, Color.FromArgb(255, c));
                }
            }
        }
        public byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

    }
}