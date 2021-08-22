using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesType
{
    public abstract class Files
    {
        protected int LengthMsg = 32;
        protected int sizeOfType = 8;

        #region main functions

        /*        /// <summary>
                /// encryptInfoInFile function - taks the file as Byte array, also take the message as Byte array
                /// and encrypt the message into copy file and return the copy after the change.
                /// </summary>
                /// <param name="arr">Main file</param>
                /// <param name="message">our message as Byte array</param>
                /// <param name="type">the message type</param>
                /// <returns>return the file after the message in it</returns>
        //        public abstract Byte[] encryptFile(string filePath, Byte[] message,string type);
        */

        /// <summary>
        /// decryptInfoFromFile function - taks the file as Byte array, and decrypt the info out of him and 
        /// return the message as Byte Array
        /// </summary>
        /// <param name="arr">our File</param>
        /// <returns>the message as Byte array</returns>
        /// <exception cref="ExceptionErrorInFileDycripting"></exception>
        /// <exception cref="ExceptionErrorInTypeDycripting"></exception>
        public abstract Tuple<Byte[],string> decryptInfoFromFile(string FilePath);

        #endregion

        #region sizes

        /// <summary>
        /// gets the fileName of the image -
        /// return the max size of the msg that can be in the image as a string + B at the end.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>the max size of the msg that can be in the image as a string</returns>
        public abstract string maxMessageSize(string FilePath);

        /// <summary>
        /// gets the fileName of the image -
        /// return the max size of the msg that can be in the image as a integer.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>the max size of the msg that can be in the image as a integer</returns>
        public abstract int maxMessageSizeNumber(string filename);
        #endregion

        #region types
        protected BitArray putTypeOFMsgInBit(string type)
        {
            //from the string(type) defines the bool array
            bool[] arr;
            switch (type)
            {
                case "png":
                    arr = new bool[] { true, true, true, true, true, true, true, true };
                    break;
                case "txt":
                    arr = new bool[] { true, true, true, true, true, true, true, false };
                    break;
                case "mp3":
                    arr = new bool[] { true, true, true, true, true, true, false, true };
                    break;
                case "wav":
                    arr = new bool[] { true, true, true, true, true, true, false, false };
                    break;
                case "7z":
                    arr = new bool[] { true, true, true, true, true, false, true, true };
                    break;
                case "zip":
                    arr = new bool[] { true, true, true, true, true, false, true, false };
                    break;
                case "rar":
                    arr = new bool[] { true, true, true, true, true, false, false, true };
                    break;
                case "bin":
                    arr = new bool[] { true, true, true, true, true, false, false, false };
                    break;
                case "xml":
                    arr = new bool[] { true, true, true, true, false, true, true, true };
                    break;
                case "gif":
                    arr = new bool[] { true, true, true, true, false, true, true, false };
                    break;
                case "jpeg":
                    arr = new bool[] { true, true, true, true, false, true, false, true };
                    break;
                case "ppt":
                    arr = new bool[] { true, true, true, true, false, true, false, false };
                    break;
                case "pptx":
                    arr = new bool[] { true, true, true, true, false, false, true, true };
                    break;
                case "mkv":
                    arr = new bool[] { true, true, true, true, false, false, true, false };
                    break;
                case "mp4":
                    arr = new bool[] { true, true, true, true, false, false, false, true };
                    break;
                case "doc":
                    arr = new bool[] { true, true, true, true, false, false, false, false };
                    break;
                case "docx":
                    arr = new bool[] { true, true, true, false, true, true, true, true };
                    break;
                case "pdf":
                    arr = new bool[] { true, true, true, false, true, true, true, false };
                    break;
                default:
                    arr = new bool[] { false, false, false, false, false, false, false, false };
                    break;
            }
            return convertBoolToBitArray(arr);
        }

        protected string getType(bool[] typeInBits)
        {
            //from the bool array defines the type

            string str;
            if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, true, true, true }))
                str = "png";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, true, true, false }))
                str = "txt";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, true, false, true }))
                str = "mp3";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, true, false, false }))
                str = "wav";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, false, true, true }))
                str = "7z";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, false, true, false }))
                str = "zip";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, false, false, true }))
                str = "rar";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, true, false, false, false }))
                str = "bin";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, true, true, true }))
                str = "xml";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, true, true, false }))
                str = "gif";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, true, false, true }))
                str = "jpeg";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, true, false, false }))
                str = "ppt";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, false, true, true }))
                str = "pptx";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, false, true, false }))
                str = "mkv";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, false, false, true }))
                str = "mp4";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, true, false, false, false, false }))
                str = "doc";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, false, true, true, true, true }))
                str = "docx";
            else if (Enumerable.SequenceEqual(typeInBits, new bool[] { true, true, true, false, true, true, true, false }))
                str = "pdf";
            else if ((Enumerable.SequenceEqual(typeInBits, new bool[] { false, false, false, false, false, false, false, false })))
                throw new ExceptionErrorInTypeDycripting("File Type Not Suported yet");
            else
                throw new ExceptionErrorInTypeDycripting("Something went wrong. are you shure this is an encryptesFile?");


            return str;
        }
        #endregion

        #region converts and helpul function

        protected byte[] convertDataInBitArrayToByteArray(BitArray msgInBits)
        {
            //takes the long data bitArray and convert it to Byte[]
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
        protected void putBitsInArr(ref BitArray lengthOfMsgInBits, BitArray helperToGetData, int i)
        {
            //takes the bits frim helperToGetData and put them in lengthOfMsgInBits
            for (int j = 0; j < 4; ++j)
                lengthOfMsgInBits[(i) + j] = helperToGetData[j];
        }
        protected void getsBitsFromInfo(ref BitArray helpBitArrayToChange, BitArray bitsInfo, int posionInInfo)
        {
            //gets the modified bits from the info
            for (int j = 0; j < 4; ++j)
            {
                helpBitArrayToChange[j] = bitsInfo[posionInInfo + j];
            }
        }
        protected byte ConvertToByte(BitArray bits)
        {
            //convert BitArray to byte
            if (bits.Count != 8)
            {
                throw new ArgumentException("BitArray size isnt 8");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
        protected void ConvertTo8Bit(BitArray msgInBits, ref BitArray convertHelper, int i)
        {
            
            for (int j = 0; j < 8; ++j)
                convertHelper[j] = msgInBits[(i * 8) + j];
        }
        protected bool[] convertBitArrayToBool(BitArray typeInBits)
        {
            bool[] arr = new bool[8];
            for (int i = 0; i < 8; ++i)
                arr[i] = typeInBits[i];
            return arr;
        }
        protected BitArray convertBoolToBitArray(bool[] typeInbool)
        {
            BitArray typeInBits = new BitArray(sizeOfType);
            for (int i = 0; i < 8; ++i)
                typeInBits[i] = typeInbool[i];
            return typeInBits;
        }
        protected int convertBitArrayLengthToInt(BitArray lengthOfMsgInBits)
        {
            int[] convertHelper = new int[1];
            lengthOfMsgInBits.CopyTo(convertHelper, 0);
            return (convertHelper[0] * 8);
        }
        #endregion


    }
}
