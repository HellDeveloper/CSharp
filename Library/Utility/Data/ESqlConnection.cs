using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Data
{
    /// <summary>
    /// Limit
    /// </summary>
    public static class ESqlConnection
    {
        /// <summary>
        /// 限制查询 必须是SQL Server 2005 以上
        /// </summary>
        /// <param name="select_sql">查询的SQL语句</param>
        /// <param name="startIndex">开始的索引（从零开始）</param>
        /// <param name="count">拿多少条</param>
        /// <param name="orderBy">排序SQL语句（不需要写 order by 关键字）</param>
        /// <returns>新的查询语句</returns>
        public static string LimitRowNumber(string select_sql, int startIndex, int count = 10, string orderBy = "ID ASC")
        {
            var i = select_sql.IndexOf(Utility.Core.Assist.WHITE_SPACE);
            //if (i < 0 || String.IsNullOrWhiteSpace(orderBy))
            //    return null;
            string format = "SELECT * FROM (SELECT ROW_NUMBER () OVER (ORDER BY {1}) AS ___RowID, {0}) as ___temp WHERE ___RowID >= {2} AND ___RowID < {3}";
            return String.Format(format, select_sql.Substring(i, select_sql.Length - i), orderBy, startIndex + 1, startIndex + count + 1);
        }

        /// <summary>
        /// 限制查询 (TOP NOT IN)最老版本
        /// </summary>
        /// <param name="select_sql">查询的SQL语句</param>
        /// <param name="startIndex">开始的索引（从零开始）</param>
        /// <param name="count">拿多少条</param>
        /// <param name="select_field">not in 的 字段</param>
        /// <returns>新的查询语句</returns>
        public static string LimitTopNotInSql(string select_sql, int startIndex, string select_field, int count = 10)
        {
            var i = select_sql.IndexOf(Utility.Core.Assist.WHITE_SPACE);
            //if (i < 0 || String.IsNullOrWhiteSpace(orderBy))
            //    return null;
            string format = "SELECT TOP {3} * FROM ({0}) as ___tempo WHERE {1} NOT IN (SELECT TOP {2} {1} FROM ({0}) AS ___tempi)";
            return String.Format(format, select_sql, select_field, startIndex, count);
        }

        /// <summary>
        /// 限制查询 必须是SQL Server 2005 以上 (ROW_NUMBER)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="select_sql">查询的SQL语句</param>
        /// <param name="startIndex">开始的索引（从零开始）</param>
        /// <param name="count">拿多少条</param>
        /// <param name="orderBy">排序SQL语句（不需要写 order by 关键字）</param>
        /// <returns>新的查询语句</returns>
        public static string LimitSql(this System.Data.SqlClient.SqlConnection conn, string select_sql, int startIndex, int count, string orderBy = "ID ASC")
        {
            return ESqlConnection.LimitRowNumber(select_sql, startIndex, count, orderBy);
        }

        /// <summary>
        /// 限制查询 (TOP NOT IN)最老版本
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="select_sql">查询的SQL语句</param>
        /// <param name="startIndex">开始的索引（从零开始）</param>
        /// <param name="count">拿多少条</param>
        /// <param name="select_field">not in 的 字段</param>
        /// <returns>新的查询语句</returns>
        public static string LimitSql(this System.Data.SqlClient.SqlConnection conn, string select_sql, int startIndex, string select_field = "ID", int count = 10)
        {
            return ESqlConnection.LimitTopNotInSql(select_sql, startIndex, select_field, count);
        }
    }
}
