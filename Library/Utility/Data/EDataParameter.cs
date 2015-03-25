using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Generic;
using Utility.Core;
using System.Reflection;

namespace Utility.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class EDataParameter
    {

        /// <summary>
        /// ParameterName的前缀
        /// </summary>
        internal static readonly char[] PARAMETER_NAME_PERFIX = { '@', ':' };

        /// <summary>
        /// ParameterName的前缀
        /// </summary>
        public static char[] ParameterNamePerfix
        {
            get { return PARAMETER_NAME_PERFIX; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="sourceColumn"></param>
        /// <param name="direction"></param>
        /// <returns>返回已经添加到集合里</returns>
        public static T Add<T>(this ICollection<T> collection, string parameterName, object value, string sourceColumn, ParameterDirection direction = ParameterDirection.Input) where T : IDataParameter, new()
        {
            T t = new T()
            {
                ParameterName = parameterName,
                Value = value,
                SourceColumn = sourceColumn,
                Direction = direction
            };
            collection.Add(t);
            return t;
        }

        /// <summary>
        /// 在SourceColumn获取比较运算符
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetComparer(this IDataParameter param)
        {
            int index = param.SourceColumn.IndexOf(Assist.WHITE_SPACE);
            if (index < 0)
                return String.Empty ;
            return param.SourceColumn.Substring(index);
        }

        /// <summary>
        /// 从 from（DbType, Direction, ParameterName, SourceColumn, Value）复制到 to
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to">到</param>
        /// <param name="from">从</param>
        /// <returns>to</returns> 
        public static T Copy<T>(T to, T from) where T : IDataParameter
        {
            to.DbType = from.DbType;
            to.Direction = from.Direction;
            to.ParameterName = from.ParameterName;
            to.SourceColumn = from.SourceColumn;
            to.Value = from.Value;
            return to;
        }

        /// <summary>
        /// 从 from（DbType, Direction, ParameterName, SourceColumn, Value）克隆到 to 。 并且可能改变值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to">到</param>
        /// <param name="from">从</param>
        /// <returns>to</returns> 
        internal static T Clone<T>(T to, T from) where T : IDataParameter
        {
            EDataParameter.Copy(to, from);
            if (to.SourceVersion == DataRowVersion.Original)
            {
                if (to.GetComparer().EndsWith(" like", StringComparison.OrdinalIgnoreCase))
                    to.Value = String.Format("%{0}%", to.Value.TryToString() ?? String.Empty);
                to.SourceVersion = DataRowVersion.Current;
            }
            return to;
        }
        
    }
}
