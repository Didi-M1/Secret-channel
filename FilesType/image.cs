
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

    public class Image : Files
    {

        #region main function
        /// <summary>
        /// encryptImage - main func to encrypt the msg.
        /// takes the filePath of the image. Byte[] the msg Bytes and the type of the msg.
        /// return bitMap with the msg data ot it with minor changes. 
        /// </summary>
        /// <param name="filePath">the image filePath</param>
        /// <param name="info">the msg in Byte array</param>
        /// <param name="type">the type of the msg</param>
        /// <returns>returns the image as a bitmap with the msg on it</returns>
        public Bitmap encryptImageFile(string filePath, Byte[] info, string type)
        {
            //takes the image and for every nedded pixsel change the lsb of the ARGB according to the Bytep[] info.

            int i = 0, j = 0;
            Bitmap bitmap = new Bitmap(filePath);

            //adds an alfha chanel to the image
            bitmap.MakeTransparent();
            ReversTransperntImg(ref bitmap);



            BitArray bitsInfo = new BitArray(info);
            BitArray lengthOfMessage = new BitArray(new int[] { info.Length }); // the Length of the message in bitArray
            BitArray typeBitArray = putTypeOFMsgInBit(type); // the type of the message in bitArray

            int length = bitsInfo.Length;


            ChangeImageFromInfo(ref i, ref j, ref bitmap, lengthOfMessage, LengthMsg);
            ChangeImageFromInfo(ref i, ref j, ref bitmap, typeBitArray, sizeOfType);
            ChangeImageFromInfo(ref i, ref j, ref bitmap, bitsInfo, length);

            return bitmap;
        }

        /// <summary>
        /// gets the filePath of the image and return the data that in it also the type of the data. 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>the data in Byte[] that was in the image also the type of the data</returns>
        public override Tuple<byte[], string> decryptInfoFromFile(string filePath)
        {
            //takes an image and exstret the length,type and the message and return it. 
            Bitmap img = new Bitmap(filePath);
            int i = 0, j = 0;
            int posionInImg = 0;



            BitArray LengthOfMsgInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, LengthMsg);
            int exsratLengthMsg = convertBitArrayLengthToInt(LengthOfMsgInBits);
            if (exsratLengthMsg <= 0)
                throw new ExceptionErrorInFileDycripting("length dycripting want wrong");

            BitArray typeInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, sizeOfType);
            bool[] boolArrType = convertBitArrayToBool(typeInBits);
            string type = getType(boolArrType);

            BitArray MsgInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, exsratLengthMsg);
            byte[] dataInByteArr = convertDataInBitArrayToByteArray(MsgInBits);


            return new Tuple<byte[], string>(dataInByteArr, type);

        }



        private BitArray getChangesBitFromImg(Bitmap img, ref int i, ref int j, ref int posionInImg, int lengthMsg)
        {
            BitArray helperToGetData = new BitArray(4);
            BitArray MsgInBits = new BitArray(lengthMsg);
            int Width = img.Width, Height = img.Height;
            int previusOfPosionInImg = posionInImg;

            //for every [ixsel in the Length takes the lsb and stored and return them into bitArray
            for (; i < Width; ++i)
            {
                for (; j < Height; ++j)
                {
                    if (previusOfPosionInImg + lengthMsg <= posionInImg)
                        break;
                    Color color = img.GetPixel(i, j);
                    getsBitsFromImg(ref helperToGetData, color);
                    putBitsInArr(ref MsgInBits, helperToGetData, j * 4 + (i * Height * 4) - previusOfPosionInImg);
                    posionInImg += 4;

                }
                if (previusOfPosionInImg + lengthMsg <= posionInImg)
                    break;
                else
                    j = 0;
            }
            return MsgInBits;
        }
        private void ChangeImageFromInfo(ref int iWidth, ref int jHeight, ref Bitmap bitmap, BitArray lengthOfMessage, int LengthOfData)
        {

            int posionInInfo = 0;
            BitArray HelpBitArrayToChange = new BitArray(4);
            int Width = bitmap.Width;
            int Height = bitmap.Height;

            //for every pixsel needed change the lsb of the ARGB and save it to the new bitmap
            for (; iWidth < Width; ++iWidth)
            {
                for (; jHeight < Height; ++jHeight)
                {
                    if (LengthOfData <= posionInInfo)
                        break;
                    getsBitsFromInfo(ref HelpBitArrayToChange, lengthOfMessage, posionInInfo);
                    Color color = bitmap.GetPixel(iWidth, jHeight);
                    color = ChangeArgbToInfo(color, HelpBitArrayToChange);
                    bitmap.SetPixel(iWidth, jHeight, color);
                    posionInInfo += 4;
                }
                if (LengthOfData <= posionInInfo)
                    break;
                else
                    jHeight = 0;
            }
        }
        #endregion

        #region sizes
        public override string maxMessageSize(string filename)
        {
            //return the max capacity that the image can save in string
            int max = maxMessageSizeNumber(filename);
            if (max > 1000000)//MB
                return (float.Parse(max.ToString()) / 1000000).ToString() + "MB";
            else if(max > 1000)
                return (float.Parse(max.ToString()) / 1000).ToString() + "kb";
            return max.ToString() + "B";
        }
        public override int maxMessageSizeNumber(string filename)
        {
            //return the max bytes capacity that the image can save in integer
            Bitmap img = new Bitmap(filename);

            int w = img.Width;
            int h = img.Height;
            return ((w * h - 10) /2); //for every pixsel we can store 4 bit. -10 beause the length and the type
        }
        #endregion

        #region converts and helpul function
        private Color ChangeArgbToInfo(Color color, BitArray bitsArrayInfo)
        {
            //changes the pixsel color according to the info(bitsArrayInfo)
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
        private void ChangeColor(ref int num, bool v)
        {
            //changes the lsb of the argb Byte to our info
            if (v)
            {
                if (num % 2 == 0)
                    num += 1;
            }
            else
                if (num % 2 == 1)
                num -= 1;
        }
        private bool getChangeColor(int num)
        {
            //get the modified lsb from the ARGB 
            if (num % 2 == 0)
                return false;
            else
                return true;
        }
        private void getsBitsFromImg(ref BitArray helperToGetData, Color color)
        {
            //gets the bits from the ARGB to bitarray
            helperToGetData[0] = getChangeColor(color.A);
            helperToGetData[1] = getChangeColor(color.R);
            helperToGetData[2] = getChangeColor(color.B);
            helperToGetData[3] = getChangeColor(color.G);
        }
        private void ReversTransperntImg(ref Bitmap bitmap)
        {
            //after we add the Alfha channel we need to undo the changes
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
        #endregion
    }
}
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FilesType
//{
//    /**
//     *
//     Work description:

//     *Step one - transfer the image to bitMap format.
//     *
//     *Step two - Make sure there is an alpha channel to the img with the makeTransparnt function.
//     *
//     *Step three - Change the lsb of the ARGB per pixel and save information 
//     *from our message in the following order - size (32b), file type (8b) and then the message.
//     *
//     *Step four - Do everything revers and decrypt the message from our bitMap. 
//     */

//    /// <summary>
//    /// Image class, inheritor to Files class. 
//    /// have 2 main function, encryptImageFile and decryptInfoFromFile. 
//    /// encryptImageFile - store data in an innocent Image file. 
//    /// decryptInfoFromFile - recover the data that have been encrypted. 
//    /// </summary>
//    public class Image : Files
//    {

//        #region main function
//        /// <summary>
//        /// encryptImage - main func to encrypt the msg.
//        /// takes the filePath of the image. Byte[] the msg Bytes and the type of the msg.
//        /// return bitMap with the msg data ot it with minor changes. 
//        /// </summary>
//        /// <param name="filePath">the image filePath</param>
//        /// <param name="info">the msg in Byte array</param>
//        /// <param name="type">the type of the msg</param>
//        /// <returns>returns the image as a bitmap with the msg on it</returns>
//        public Bitmap encryptImageFile(string filePath, Byte[] info,string type)
//        {
//            //takes the image and for every nedded pixsel change the lsb of the ARGB according to the Bytep[] info.

//            int i = 0 ,j = 0;
//            Bitmap bitmap = new Bitmap(filePath);

//            //adds an alfha chanel to the image
//            bitmap.MakeTransparent();
//            ReversTransperntImg(ref bitmap);



//            BitArray bitsInfo = new BitArray(info);
//            BitArray lengthOfMessage = new BitArray(new int[] { info.Length }); // the Length of the message in bitArray
//            BitArray typeBitArray = putTypeOFMsgInBit(type); // the type of the message in bitArray

//            int length = bitsInfo.Length;


//            ChangeImageFromInfo(ref i, ref j, ref bitmap,0, lengthOfMessage, LengthMsg);
//            ChangeImageFromInfo(ref i, ref j, ref bitmap,0, typeBitArray, sizeOfType);
//            ChangeImageFromInfo(ref i, ref j, ref bitmap,0, bitsInfo, length);

//            return bitmap;
//        }

//        /// <summary>
//        /// gets the filePath of the image and return the data that in it also the type of the data. 
//        /// </summary>
//        /// <param name="filePath"></param>
//        /// <returns>the data in Byte[] that was in the image also the type of the data</returns>
//        public override Tuple<byte[], string> decryptInfoFromFile(string filePath)
//        {
//            //takes an image and exstret the length,type and the message and return it. 
//            Bitmap img = new Bitmap(filePath);
//            int i = 0, j = 0;
//            int posionInImg = 0;



//            BitArray LengthOfMsgInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, LengthMsg);
//            int exsratLengthMsg = convertBitArrayLengthToInt(LengthOfMsgInBits);
//            if (exsratLengthMsg <= 0)
//                throw new ExceptionErrorInFileDycripting("length dycripting want wrong");

//            BitArray typeInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, sizeOfType);
//            bool[] boolArrType = convertBitArrayToBool(typeInBits);
//            string type = getType(boolArrType);

//            BitArray MsgInBits = getChangesBitFromImg(img, ref i, ref j, ref posionInImg, exsratLengthMsg);
//            byte[] dataInByteArr = convertDataInBitArrayToByteArray(MsgInBits);


//            return new Tuple<byte[], string>(dataInByteArr, type);

//        }



//        private BitArray getChangesBitFromImg(Bitmap img, ref int i, ref int j, ref int posionInImg, int lengthMsg)
//        {
//            BitArray helperToGetData = new BitArray(4);
//            BitArray MsgInBits = new BitArray(lengthMsg);
//            int Width = img.Width, Height = img.Height;
//            int previusOfPosionInImg = posionInImg;
//            bool a;
//            //for every [ixsel in the Length takes the lsb and stored and return them into bitArray
//            for (; i < Width; ++i)
//            {
//                for (; j < Height; ++j)
//                {
//                    if (posionInImg >= lengthMsg)
//                        a = true;
//                    if (previusOfPosionInImg + lengthMsg <= posionInImg)
//                        break;
//                    Color color = img.GetPixel(i, j);
//                    getsBitsFromImg(ref helperToGetData, color);
//                    putBitsInArr(ref MsgInBits, helperToGetData, j * 4 + (i * Height * 4) - previusOfPosionInImg);
//                    posionInImg += 4;

//                }
//                if (previusOfPosionInImg + lengthMsg <= posionInImg)
//                    break;
//                else
//                    j = 0;
//                if (i == 1070)
//                    a = true;
//            }
//            return MsgInBits;
//        }
//        private void ChangeImageFromInfo(ref int iWidth, ref int jHeight, ref Bitmap bitmap, int posionInInfo, BitArray lengthOfMessage, int LengthOfData)
//        {

//            BitArray HelpBitArrayToChange = new BitArray(4);
//            int Width = bitmap.Width;
//            int Height = bitmap.Height;

//            //for every pixsel needed change the lsb of the ARGB and save it to the new bitmap
//            for (; iWidth < Width; ++iWidth)
//            {
//                for (; jHeight < Height; ++jHeight)
//                {
//                    if (LengthOfData <= posionInInfo)
//                        break;
//                    getsBitsFromInfo(ref HelpBitArrayToChange, lengthOfMessage, posionInInfo);
//                    Color color = bitmap.GetPixel(iWidth, jHeight);
//                    color = ChangeArgbToInfo(color, HelpBitArrayToChange);
//                    bitmap.SetPixel(iWidth, jHeight, color);
//                    posionInInfo += 4;
//                }
//                if (LengthOfData <= posionInInfo)
//                    break;
//                else
//                    jHeight = 0;
//            }
//        }
//        #endregion

//        #region sizes
//        public override string maxMessageSize(string filename)
//        {
//            //return the max capacity that the image can save in string
//            int max = maxMessageSizeNumber(filename);
//            if (max > 1000000)//MB
//                return (float.Parse(max.ToString()) / 1000000).ToString() + "MB";
//            return max.ToString() + "B";
//        }
//        public override int maxMessageSizeNumber(string filename)
//        {
//            //return the max bytes capacity that the image can save in integer
//            Bitmap img = new Bitmap(filename);

//            int w = img.Width;
//            int h = img.Height;
//            return ((w * h - 10) * 4);
//        }
//        #endregion

//        #region converts and helpul function
//        private Color ChangeArgbToInfo(Color color, BitArray bitsArrayInfo)
//        {
//            //changes the pixsel color according to the info(bitsArrayInfo)
//            int alfa = color.A;
//            int red = color.R;
//            int blue = color.B;
//            int green = color.G;
//            ChangeColor(ref alfa, bitsArrayInfo[0]);
//            ChangeColor(ref red, bitsArrayInfo[1]);
//            ChangeColor(ref blue, bitsArrayInfo[2]);
//            ChangeColor(ref green, bitsArrayInfo[3]);

//            return Color.FromArgb(alfa, red, green, blue);
//        }
//        private void ChangeColor(ref int num, bool v)
//        {
//            //changes the lsb of the argb Byte to our info
//            if (v)
//            {
//                if (num % 2 == 0)
//                    num += 1;
//            }
//            else
//                if (num % 2 == 1)
//                num -= 1;
//        }
//        private bool getChangeColor(int num)
//        {
//            //get the modified lsb from the ARGB 
//            if (num % 2 == 0)
//                return false;
//            else
//                return true;
//        }
//        private void getsBitsFromImg(ref BitArray helperToGetData, Color color)
//        {
//            //gets the bits from the ARGB to bitarray
//            helperToGetData[0] = getChangeColor(color.A);
//            helperToGetData[1] = getChangeColor(color.R);
//            helperToGetData[2] = getChangeColor(color.B);
//            helperToGetData[3] = getChangeColor(color.G);
//        }
//        private void ReversTransperntImg(ref Bitmap bitmap)
//        {
//            //after we add the Alfha channel we need to undo the changes
//            int h = bitmap.Height, w = bitmap.Width;
//            for (int i = 0; i < h; i++)
//            {
//                for (int j = 0; j < w; j++)
//                {
//                    Color c = bitmap.GetPixel(j, i);
//                    if (c.A == 0)
//                        bitmap.SetPixel(j, i, Color.FromArgb(255, c));
//                }
//            }
//        }
//        #endregion
//    }
//}