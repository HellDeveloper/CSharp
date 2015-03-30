using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Generic
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ECollection
    {
        /// <summary>
        /// 移除null元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int RemoveNull<T>(this ICollection<T> args) where T : class
        {
            int count = 0;
            while (args.Remove(null))
                count++;
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int RemoveBy<T, D>(this ICollection<T> collection, Func<T, D, bool> func, D data)
        {
            if (collection == null || collection.Count == 0)
                return 0;
            List<T> list = new List<T>();
            foreach (var item in collection)
                if (func(item, data))
                    list.Add(item);
            foreach (var item in list)
                collection.Remove(item);
            return list.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="collection"></param>
        /// <param name="func"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int RemoveBy<T, D0, D1>(this ICollection<T> collection, Func<T, D0, D1, bool> func, D0 data0, D1 data1)
        {
            if (collection == null || collection.Count == 0)
                return 0;
            List<T> list = new List<T>();
            foreach (var item in collection)
                if (func(item, data0, data1))
                    list.Add(item);
            foreach (var item in list)
                collection.Remove(item);
            return list.Count;
        }

    }
}
