using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Utility.Core;
using Utility.Generic;

namespace Aes_Example
{
    class AesExample
    {
        public static void Main()
        {
            int uone = -1;
            int zero = 0;
            int one = 1;
            
            Console.WriteLine(uone / (float)zero);
            Console.WriteLine(uone / (double)zero);
            Console.WriteLine(zero / (float)zero);
            Console.WriteLine(zero / (double)zero);
            Console.WriteLine(one / (float)zero);
            Console.WriteLine(one / (double)zero);
            
            Console.ReadKey(true);
        }
    }
}