using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core;

namespace Utility.Data
{
    /// <summary>
    /// 数据库
    /// </summary>
    public static partial class EDbConnection
    {
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool OpenConnection<T>(this T conn) where T : System.Data.IDbConnection
        {
            if (conn.State == ConnectionState.Broken)
                conn.Close();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            return conn.State == ConnectionState.Open;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool CloseConnection<T>(this T conn) where T : System.Data.IDbConnection
        {
            if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Open)
                conn.Close();
            return conn.State == ConnectionState.Closed;
        }

        /// <summary>
        /// 创建Command对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static System.Data.IDbCommand CreateCommand<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args) where T : System.Data.IDbConnection
        {
            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            foreach (var item in args)
            {
                if (item == null || String.IsNullOrWhiteSpace(item.ParameterName))
                    continue;
                if (item.Value == null)
                    item.Value = DBNull.Value;
                if (cmd.CommandType != CommandType.StoredProcedure && item.Direction != ParameterDirection.Input)
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(item);
            }

            if (cmd.CommandType != CommandType.StoredProcedure && sql.IndexOf(Utility.Core.Assist.WHITE_SPACE, 0) == -1)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Result"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static Result Execute<T, Result>(T conn, string sql, IEnumerable<System.Data.IDataParameter> args, Func<System.Data.IDbCommand, Result> func) where T : System.Data.IDbConnection
        {
            bool need_close = conn.State == ConnectionState.Closed;
            System.Data.IDbCommand cmd = EDbConnection.CreateCommand(conn, sql, args);
            EDbConnection.OpenConnection(conn);
            Result temp = func.Invoke(cmd);
            if (need_close)
                EDbConnection.CloseConnection(conn);
            cmd.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// System.Data.IDbCommand执行ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal static int ExecuteNonQuery<T>(T cmd) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// System.Data.IDbCommand执行ExecuteScalar
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal static object ExecuteScalar<T>(T cmd) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool TryAddColumn(DataTable table, string name)
        {
            try
            {
                table.Columns.Add(name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.Execute(conn, sql, args, EDbConnection.ExecuteNonQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object ExecuteScalar<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.Execute(conn, sql, args, EDbConnection.ExecuteScalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Result"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result ExecuteReader<T, Result>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            CommandBehavior behavior = conn.State == ConnectionState.Closed ? CommandBehavior.CloseConnection : CommandBehavior.Default;
            IDataReader reader = EDbConnection.GetDataReader(conn, sql, args, behavior);
            Result temp = func(reader);
            if (!reader.IsClosed)
                reader.Close();
            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static IDataReader GetDataReader<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args, CommandBehavior behavior = CommandBehavior.CloseConnection) where T : System.Data.IDbConnection
        {
            System.Data.IDbCommand cmd = EDbConnection.CreateCommand(conn, sql.Trim(), args);
            EDbConnection.OpenConnection(conn);
            var temp = cmd.ExecuteReader(behavior);
            cmd.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// IDataReader 转 DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable IDataReaderToDataTable(IDataReader reader)
        {
            DataTable table = new DataTable();
            List<int> list = new List<int>();
            for (int i = 0; i < reader.FieldCount; i++)
                if (EDbConnection.TryAddColumn(table, reader.GetName(i)))
                    list.Add(i);
            while (reader.Read())
            {
                object[] array = new object[list.Count];
                int index = 0;
                foreach (var item in list)
                    array[index++] = reader.GetValue(item);
                table.Rows.Add(array);
            }
            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable GetDataTable<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.ExecuteReader(conn, sql, args, EDbConnection.IDataReaderToDataTable);
        }
    }


    /// <summary>
    /// 数据库
    /// </summary>
    public static partial class EDbConnection
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="str">没有分号，从app.config里查找</param>
        public static void SetConnectionString<T>(this T conn, string str) where T : System.Data.IDbConnection
        {
            if (String.IsNullOrWhiteSpace(str))
                return;
            if (str.IndexOf(Assist.SEMICOLON) >= 0)
            {
                conn.ConnectionString = str;
                return;
            }
            ConnectionStringSettings conn_str_set = ConfigurationManager.ConnectionStrings[str];
            if (conn_str_set != null)
                conn.ConnectionString = conn_str_set.ConnectionString;
        }

    }
}