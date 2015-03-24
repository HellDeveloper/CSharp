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
            IDataParameter param = new SqlParameter();
            Console.WriteLine(param.SourceVersion);
            Console.ReadKey();
        }


    }
}
