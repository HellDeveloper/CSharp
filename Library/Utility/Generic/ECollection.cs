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
        /// 
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

    }
}
