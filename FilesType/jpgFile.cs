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
    public class jpgFile : Files
    {
        const int startFileByte = 8;
        public override Tuple<Byte[], string> decryptInfoFromFile(byte[] fileByteArray)
        {
            int fileLociton = startFileByte; //after all the Haders


            int Length = decryptLengthFromFile(fileByteArray, ref fileLociton);

            string type = decryptTypeDataFromFile(fileByteArray, ref fileLociton);

            byte[] Data = decryptDataFromFile(fileByteArray, Length, ref fileLociton);

            if(type =="string")
            {
                string str = "";
                BitArray[] DataIn32Bits = new BitArray[Length/32];
                byte[] arr = new byte[4];
                for(int i =0,j=0;i<(Length/8);i+=4,j++)
                {
                    arr[3] = Data[i + 3];
                    arr[2] = Data[i + 2];
                    arr[1] = Data[i + 1];
                    arr[0] = Data[i];
                    DataIn32Bits[j] = new BitArray(arr);
                    foreach(var b in DataIn32Bits[j])
                    {
                        Console.WriteLine(b);
                    }
                    Console.WriteLine("\n\n\n\n\n\n\n");
                }

                int[] convertHelper = new int[1];

                foreach (var item in DataIn32Bits)
                {
                    item.CopyTo(convertHelper, 0);
                    str += (char)convertHelper[0];
                }
                str += "\n";
                Console.WriteLine(str);
            }

            return new Tuple<byte[], string>(Data, type);
        }

        private byte[] decryptDataFromFile(byte[] fileByteArray, int length, ref int fileLociton)
        {

            BitArray Data = new BitArray(8);
            byte[] DataByte = new byte[length/8];
            for (int i=0; i < length; i++,fileLociton++)
            {
                if (i % 8 == 0 && i!=0)
                    DataByte[i / 8] = ConvertToByte(Data);
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
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
        private string decryptTypeDataFromFile(byte[] fileByteArray, ref int fileLociton)
        {
            bool[] typeData = { true, true, true, true };
            for (int i = 0; i < 4; ++i)
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

                    Console.WriteLine(a);
                }
                Console.WriteLine("\n\n\n\n\n\n\n");

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
            int MessageLengthInByte = message.Length * 32;//every char is 32 bits
            BitArray lengthOfMessage = new BitArray(new int[] { message.Length }); // the Length of the message in bitArray
            if (lengthOfMessage.Length > 24)
                throw new Exception("Message too big");

            int fileLociton = startFileByte; //after all the Haders

            for (int j=0; fileLociton < startFileByte+24; ++fileLociton,j++)
                fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], lengthOfMessage[j]);
            

            bool[] fileType = findFileType(type);

            for(int i =0;i<4;++i, ++fileLociton)
                fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], fileType[i]);


            foreach (byte b in message)
            {
                    fileByteArray[fileLociton] = changeByte(fileByteArray[fileLociton], bool.Parse(b.ToString()));
                    fileLociton++;
            }
            return fileByteArray;
        }

        
    }
}
