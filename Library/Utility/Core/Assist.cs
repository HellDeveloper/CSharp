using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.Core
{
    /// <summary>
    /// 助手
    /// </summary>
    public static class Assist
    {
        /// <summary>
        /// 
        /// </summary>
        static Assist()
        {
            _mongo_id = new MongoDBObjectID();
            Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// 空格
        /// </summary>
        public const char WHITE_SPACE = ' ';

        /// <summary> 分号
        /// </summary>
        public const char SEMICOLON = ';';

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string ISO_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary> 1970-01-01 00:00:00 U
        /// </summary>
        public static DateTime Epoch { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private static readonly MongoDBObjectID _mongo_id;

        /// <summary> MongoDB 生成 唯一 的 ID
        /// </summary>
        public static string MongoID
        {
            get
            {
                return _mongo_id.Generate();
            }
        }

        /// <summary> 返回参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T ReturnParameter<T>(T t)
        {
            return t;
        }

        /// <summary> 创建数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T[] Array<T>(params T[] args)
        {
            return args;
        }

        /// <summary>
        /// 获取第一个空格前面的字符
        /// <param name="str"></param>
        /// <returns></returns>
        /// </summary>
        public static string GetBeforeFirstWhiteSpace(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return null;
            string[] array = str.Split(Assist.WHITE_SPACE);
            return array.Length > 0 ? array[0] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <param name="indexes"></param>
        public static unsafe void Replace(string str, char c, params int[] indexes)
        {
            fixed (char* ptr = str)
            {
                foreach (var index in indexes)
                    if (index < str.Length)
                        *(ptr + index) = c;
            }
        }

#if Hex
        static readonly char[] digits = 
        {  
        '0' , '1' , '2' , '3' , '4' , '5' ,  
        '6' , '7' , '8' , '9' , 'a' , 'b' ,  
        'c' , 'd' , 'e' , 'f' , 'g' , 'h' ,  
        'i' , 'j' , 'k' , 'l' , 'm' , 'n' ,  
        'o' , 'p' , 'q' , 'r' , 's' , 't' ,  
        'u' , 'v' , 'w' , 'x' , 'y' , 'z'  
        };

        public static String ToOctonary(long i, int shift)
        {
            char[] buf = new char[32];
            int charPos = 32;
            int radix = 1 << shift;
            int mask = radix - 1;
            do
            {
                buf[--charPos] = digits[i & mask]; //&二进制按位与  
                i >>= shift;  //无符号右移1位，相当于手算的时候把i除以shift，结果再赋予i  
            } while (i != 0);
            return new String(buf, charPos, (32 - charPos));
        }
#endif
    }
}
