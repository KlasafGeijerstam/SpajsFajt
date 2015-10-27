using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.IO;

namespace CT
{
    class Program
    {
        static string s, p = "hej";
        static void Main(string[] args)
        {
            Console.WriteLine(s);
            Console.WriteLine(p);
            Console.ReadKey();
        }
        static string RetModLine(string dat)
        {
            string n = "";
            for (int i = 0; i < dat.Length; i++)
            {
                if (dat[i] != ' ')
                    n += dat[i];
                else if (i + 2 > dat.Length)
                    n += "\r\n";
                else if (dat[i + 1] != ' ')
                    n += "\r\n";
            }
            return n;
        }
    }
}
