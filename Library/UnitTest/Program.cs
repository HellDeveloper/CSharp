using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core;

namespace UnitTest
{
    class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToShortDateString());
            Console.WriteLine(DateTime.Now.ToLongDateString());
            Console.ReadKey();
        }


    }
}
