using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;


namespace BitsArray
{

    class Program
    {
        static public Byte[] AfterChangefile(Byte[] arr, string messge)
        {
            /*
            for (int p = 60; p < 80; p++)
            {
                Console.WriteLine(arr[p]);
            }
            Console.WriteLine("\n\n\n\n\n");
            */
            int i = 0;
            BitArray ch = new BitArray(messge.Length * 32);



            foreach (char c in messge)
            {
                BitArray char1 = new BitArray(new int[] { System.Convert.ToInt32(c) });
                foreach (var a in char1)
                {
                    ch[i] = (bool)a;
                    i++;
                }
            }

            i = 50;
            BitArray char2 = new BitArray(new int[] { messge.Length }); // messge can be till 1024 chars
            for (int j = 0; i < 60; ++i, ++j)
            {
                arr[i] = changeByte(arr[i], char2[j]);

            }


            foreach (var a in ch)
            {
                arr[i] = changeByte(arr[i], (bool)a);
                i++;
            }
            /*
            for (int p = 60; p < 80; p++)
            {
                Console.WriteLine(arr[p]);
            }
            Console.WriteLine("\n\n\n\n\n");
            */
            return arr;
        }

        static public void DycripteTheMessge(Byte[] arr)
        {
            /*for (int p = 60; p < 80; p++)
            {
                Console.WriteLine(arr[p]);
            }
            Console.WriteLine("\n\n\n\n\n");
            */

            BitArray char2 = new BitArray(10);
            int i = 50;
            int j = 0;

            for (; i < 60; ++i, ++j)
            {
                char2[j] = GetchangeBit(arr[i]);
            }

            int[] length = new int[1];
            char2.CopyTo(length, 0);
            int Length = 32 * length[0];
            BitArray[] Data = new BitArray[length[0]];
            int k = 0;
            for (i = 60; k < length[0]; i += 32, k++)
            {
                Data[k] = new BitArray(32);
                for (j = 0; j < 32; j++)
                {
                    Data[k][j] = GetchangeBit(arr[i + j]);
                }
            }
            string str = "";


            foreach (var item in Data)
            {
                item.CopyTo(length, 0);
                str += (char)length[0];
            }
            str += "\n";
            Console.WriteLine(str);

        }

        private static bool GetchangeBit(byte b)
        {
            int bInt = int.Parse(b.ToString());
            if (bInt % 2 == 0)
                return false;
            return true;
        }

        static public Byte changeByte(Byte b, bool bit)
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

        static void Main(string[] args)
        { 
        //{
            Byte[] info = System.IO.File.ReadAllBytes("C:\\Users\\Didi\\Desktop\\DD.jpg");
            DycripteTheMessge(info);
            
            if (info.GetType() == typeof(int))
            /*
            string FilePath = "C:\\Users\\Didi\\Desktop\\123.jpg";
            //            FileStream file = File.Open(FilePath, FileMode.Open);
            Byte[] info = System.IO.File.ReadAllBytes(FilePath);

            for (int i =0;i<100;i++)
            {
                Console.WriteLine(info[i]);
            }
            Console.WriteLine("\n\n\n\n\n");


            info = AfterChangefile(info, "Hello World im the knog of the world. DDDDDDD");

            DycripteTheMessge(info);

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(info[i]);
            }
            Console.WriteLine("\n\n\n\n\n");

            File.WriteAllBytes("C:\\Users\\Didi\\Desktop\\DD.jpg", info);
            */
            Console.ReadLine();
        }
    }
}
