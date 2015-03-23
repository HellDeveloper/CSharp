using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Utility.Core;

namespace Utility.WebForm
{
    public static class EListItemCollection
    {
        /// <summary>
        /// 添加ListItem ----- new ListItem() { Text = text, Value = value }
        /// </summary>
        /// <param name="lic"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        public static void Add(this ListItemCollection lic, string text, string value)
        {
            lic.Add(new ListItem() { Text = text, Value = value });
        }

        /// <summary>
        /// item是Text也是Value
        /// </summary>
        /// <param name="lic"></param>
        /// <param name="args"></param>
        public static void AddRange<T>(this ListItemCollection lic, IEnumerable<T> args) 
        {
            foreach (var item in args)
            {
                string str = item.TryToString() ?? String.Empty;
                EListItemCollection.Add(lic, str, str);
            }
        }

        /// <summary>
        /// Key=Text, Value=Value
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <typeparam name="Value"></typeparam>
        /// <param name="lic"></param>
        /// <param name="dic"></param>
        public static void AddRange<Key, Value>(this ListItemCollection lic, Dictionary<Key, Value> dic)
        {
            foreach (var item in dic)
                EListItemCollection.Add(lic, item.Key.TryToString() ?? String.Empty, item.Value.TryToString() ?? String.Empty); 
        }

    }
}
