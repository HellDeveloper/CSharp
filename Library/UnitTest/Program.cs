using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Generic;
using Utility.Core;
using Utility.Data;
using System.Data.SqlClient;
using System.Data;

namespace UnitTest
{
    class Program
    {

        public static void Main(string[] args)
        {
            string str = "01234567";
            Assist.Replace(str, 'A', 0, 2, 4, 6, 8, 10);
            Console.WriteLine(str);
            Console.ReadKey();
        }


    }
}
