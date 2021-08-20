using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesType
{

    /*
     * 
     * jpg / JPEG / JFIFH -- file format
    BYTE SOI[2];          /* 00h  Start of Image Marker     
    BYTE APP0[2];         /* 02h  Application Use Marker    
    BYTE Length[2];       /* 04h  Length of APP0 Field      
    BYTE Identifier[5];   /* 06h  "JFIF" (zero terminated) Id String 
    BYTE Version[2];      /* 07h  JFIF Format Revision      
    BYTE Units;           /* 09h  Units used for Resolution 
    BYTE Xdensity[2];     /* 0Ah  Horizontal Resolution     
    BYTE Ydensity[2];     /* 0Ch  Vertical Resolution       
    BYTE XThumbnail;      /* 0Eh  Horizontal Pixel Count    
    BYTE YThumbnail;      /* 0Fh  Vertical Pixel Count      
     */
    public class image : Files
    {
        const int startFileByte = 150;
        public override Tuple<Byte[], string> decryptInfoFromFile(byte[] fileByteArray)
        {
            int fileLociton = startFileByte; //after all the Haders


            int Length = decryptLengthFromFile(fileByteArray, ref fileLociton);

            string type = decryptTypeDataFromFile(fileByteArray, ref fileLociton);


            byte[] Data;
            if (type =="string")
            {

                Data = decryptstringFromFile(fileByteArray, Length, ref fileLociton);
                string str = getStringFromData(Data,Length);  
            }
            else
                Data = decryptDataFromFile(fileByteArray, Length, ref fileLociton);

            return new Tuple<byte[], string>(Data, type);
        }

        private byte[] decryptDataFromFile(byte[] fileByteArray, int length, ref int fileLociton)
        {
            BitArray Data = new BitArray(8);
            byte[] DataByte = new byte[length];
            for (int i = 0; i < length*8; i++, fileLociton++)
            {
                if (i % 8 == 0 && i != 0)
                    DataByte[(i / 8) - 1] = ConvertToByte(Data);
                Data[i % 8] = GetChangedBit(fileByteArray[fileLociton]);

            }

            return DataByte;
        }

        
        public override string getStringFromData(byte[] Data, int Length)
        {
            string str = "";
            BitArray[] DataIn32Bits = new BitArray[Length / 32];
            byte[] arr = new byte[4];
            for (int i = 0, j = 0; i < (Length / 8); i += 4, j++)
            {
                arr[3] = Data[i + 3];
                arr[2] = Data[i + 2];
                arr[1] = Data[i + 1];
                arr[0] = Data[i];
                DataIn32Bits[j] = new BitArray(arr);
            }

            int[] convertHelper = new int[1];

            foreach (var item in DataIn32Bits)
            {
                item.CopyTo(convertHelper, 0);
                str += (char)convertHelper[0];
            }
            str += "\n";
            return str;
        }

        private byte[] decryptstringFromFile(byte[] fileByteArray, int length, ref int fileLociton)
        {

            BitArray Data = new BitArray(8);
            byte[] DataByte = new byte[length/8];
            for (int i=0; i < length; i++,fileLociton++)
            {
                if (i % 8 == 0 && i!=0)
                    DataByte[(i / 8)-1] = ConvertToByte(Data);
                Data[i%8] = GetChangedBit(fileByteArray[fileLociton]); 
                
            }

            return DataByte;
        }

        byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            for (int i = 0; i < 4; ++i)
            {
                bool temp = bits[i];
                bits[i] = bits[7 - i];
                bits[7 - i] = temp;
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }


        private string decryptTypeDataFromFile(byte[] fileByteArray, ref int fileLociton)
        {
            bool[] typeData = { true, true, true, true };
            for (int i = 0; i < 4; ++i, fileLociton++)
            {
                typeData[i] = GetChangedBit(fileByteArray[fileLociton]);
            }

            return getTypeData(typeData);

        }

        private int decryptLengthFromFile(byte[] fileByteArray,ref int fileLociton)
        {
            BitArray LengthOfDataInBits = new BitArray(24);
            //get the Length of the data
            for (int i = 0; fileLociton < startFileByte + 24; ++fileLociton, ++i)
            {
                LengthOfDataInBits[i] = GetChangedBit(fileByteArray[fileLociton]);
            }
            int[] convertHelper = new int[1];
            LengthOfDataInBits.CopyTo(convertHelper, 0);
            return convertHelper[0];
        }

        public override byte[] encryptInfoInFile(byte[] fileByteArray, string message)
        {

            //The function will change the 8-32 first byts to store the Length of the message
            //Then 4 bit to detect that it is a string message 
            //and then put all the information bits into the file.



            //Length of file in the encrypt file

            // messge can be till 16777216 bits
            // 2097152 Byte, 2048 mb ~ 2Gb

            BitArray lengthOfMessage = new BitArray(new int[] { message.Length*32 }); // the Length of the message in bitArray
         //   if (lengthOfMessage.Length > 24)
           //     throw new Exception("Message too big");

            int fileLociton = startFileByte; //after all the Haders

            for (int j = 0; fileLociton < startFileByte+24; ++fileLociton, j++)
                fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], lengthOfMessage[j]);


            bool[] fileType = findFileType("string");

            for (int i = 0; i < 4; ++i, ++fileLociton)
                fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], fileType[i]);


            foreach (char c in message)
            {
                BitArray char1 = new BitArray(new int[] { System.Convert.ToInt32(c) });
                
                foreach (var a in char1)
                {
                    fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], (bool)a);
                    fileLociton++;

                }

            }
            return fileByteArray;
        }

        public override byte[] encryptInfoInFile(byte[] fileByteArray, byte[] message, string type)
        {
            //The function will change the 8-32 first byts to store the Length of the message
            //Then 4 bit to detect that it is a string message 
            //and then put all the information bits into the file.



            //Length of file in the encrypt file

            // messge can be till 16777216 bits
            // 2097152 Byte, 2048 mb ~ 2Gb
            BitArray lengthOfMessage = new BitArray(new int[] { message.Length }); // the Length of the message in bitArray
            //if (lengthOfMessage.Length > 24)
              //  throw new Exception("Message too big");

            int fileLociton = startFileByte; //after all the Haders

            for (int j=0; fileLociton < startFileByte+24; ++fileLociton,j++)
                fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], lengthOfMessage[j]);
            

            bool[] fileType = findFileType(type);

            for(int i =0;i<4;++i, ++fileLociton)
                fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], fileType[i]);


            foreach (byte b in message)
            {
                BitArray Byte = ByteToBitFormat8(b);
                for (int i =0;i<8;++i)
                {

                    fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], Byte[i]);
                    fileLociton++;
                }
            }
            return fileByteArray;
        }

        private BitArray ByteToBitFormat8(byte b)
        {
            BitArray Byte = new BitArray(8);
            int num = int.Parse(b.ToString());
            int counter = 128;
            
            for (int i = 0; i < 8 ; i++)
            {
                if (num >= counter)
                {
                    Byte[i] = true;
                    num -= counter;
                }
                else
                    Byte[i] = false;

                counter /= 2;

            }
            return Byte;
        }

        public override string maxMessageSize(byte[] arr)
        {
            int numBits = arr.Length / 8;
            numBits -=(startFileByte + 24 + 4);
            return (numBits ).ToString()+" Byte";
        }
        public override string ToString()
        {
            return "file.jpg/jpag";
        }
    }
}
