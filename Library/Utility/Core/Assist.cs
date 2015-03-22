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


    }
}
