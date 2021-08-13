using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesType
{
    public abstract class Files
    {
        /// <summary>
        /// encryptInfoInFile function - taks the file as Byte array, also take the message as Byte array
        /// and encrypt the message into copy file and return the copy after the change.
        /// </summary>
        /// <param name="arr">Main file</param>
        /// <param name="message">our message as Byte array</param>
        /// <param name="type">the message type</param>
        /// <returns>return the file after the message in it</returns>
        public abstract Byte[] encryptInfoInFile(Byte[] arr, Byte[] message,string type);

        /// <summary>
        /// encryptInfoInFile function - taks the file as Byte array, also take the message as stirng
        /// and encrypt the message into copy file and return the copy after the change.
        /// </summary>
        /// <param name="arr">Main file</param>
        /// <param name="message">our message as string</param>
        /// <returns>return the file after the message in it</returns>
        public abstract Byte[] encryptInfoInFile(Byte[] arr, string message);


        /// <summary>
        /// decryptInfoFromFile function - taks the file as Byte array, and decrypt the info out of him and 
        /// return the message as Byte Array
        /// </summary>
        /// <param name="arr">our File</param>
        /// <returns>the message as Byte array</returns>
        public abstract Tuple<Byte[],string> decryptInfoFromFile(Byte[] arr);

        public abstract string maxMessageSize(Byte[] arr);
        public abstract string getStringFromData(byte[] Data, int Length);
        public Byte changeByte(Byte b, bool bit)
        {
            int bInt = int.Parse(b.ToString());
            if (bit)
            {
                if (bInt % 2 == 0)
                    bInt += 1;
            }
            else
            {
                if (bInt % 2 == 1)
                    bInt -= 1;
            }
            return (Byte)bInt;
        }

        public bool[] findFileType(string type)
        {
            bool[] arr = new bool[] { false, false, false, false }; 
            if (type.ToLower() == "string")
                arr =  new bool[] { true, true, true, true };
            else if(type.ToLower() == "png")
                arr = new bool[] { true, true, true, false };
            else if(type.ToLower() == "txt")
                arr = new bool[] { true, true, false, true };
            return arr;
        }

        public bool GetChangedBit(byte b)
        {
            int bInt = int.Parse(b.ToString());
            if (bInt % 2 == 0)
                return false;
            return true;
        }
        public string getTypeData(bool[] typeData)
        {
            string str = "";
            if (Enumerable.SequenceEqual(typeData, new bool[] { true, true, true, true }))
                str = "string";
            else if (Enumerable.SequenceEqual(typeData, new bool[] { true, true, true, false }))
                str = "png";
            else if (Enumerable.SequenceEqual(typeData, new bool[] { true, true, false, true }))
                str = "txt";
            else if ((Enumerable.SequenceEqual(typeData, new bool[] { false, false, false, false })))
                throw new Exception("File Type Not Suported");
            
            return str;
        }
    }
}
