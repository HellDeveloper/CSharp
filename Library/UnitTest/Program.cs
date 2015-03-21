using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Generic;
using Utility.Core;
using Utility.Data;

namespace UnitTest
{
    class Program
    {

        public static void Main(string[] args)
        {
            List<string> list = new List<string>();
            list.Add(null);
            list.Add("A");
            list.Add(null);
            list.Add("A");
            list.Add(null);
            list.Add("A");
            Console.WriteLine(list.RemoveNull());
            Console.WriteLine(list.Count);

            object o = list;
            Console.WriteLine(o is IEnumerable<object>);

            Console.ReadKey();
        }


    }
}
