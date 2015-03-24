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
        /// 空格
        /// </summary>
        public const char WHITE_SPACE = ' ';

        /// <summary>
        /// 分号
        /// </summary>
        public const char SEMICOLON = ';';

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string ISO_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 返回参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T ReturnParameter<T>(T t)
        {
            return t;
        }

        /// <summary>
        /// 创建数组
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
            fixed(char* ptr = str)
            {
                foreach (var index in indexes)
                    if (index < str.Length)
                       *(ptr + index) = c;
            }
        }
    }
}
