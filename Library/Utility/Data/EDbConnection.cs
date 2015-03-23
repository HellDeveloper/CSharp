using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        private static System.Data.IDbCommand CreateCommand<T>(this T conn, string sql, IEnumerable<object> args, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput) where T : System.Data.IDbConnection
        {
            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            EDbConnection.AddParameter(cmd, args, notinput);
            if (cmd.CommandType != CommandType.StoredProcedure && sql.IndexOf(Utility.Core.Assist.WHITE_SPACE, 0) == -1)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <param name="notinput"></param>
        /// <returns></returns>
        private static T AddParameter<T>(T cmd, IEnumerable<object> args, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput) where T : System.Data.IDbCommand
        {
            foreach (var item in args)
            {
                if (item == null)
                    continue;
                else if (item is IEnumerable<object>)
                    EDbConnection.AddParameter(cmd, item as IEnumerable<object>, notinput);
                else if (item is IDataParameter)
                    EDbConnection.AddParameter(cmd, item as IDataParameter, notinput);
            }
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="param"></param>
        /// <param name="notinput"></param>
        private static void AddParameter<T>(T cmd, System.Data.IDataParameter param, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput) where T : System.Data.IDbCommand
        {
            if (String.IsNullOrWhiteSpace(param.ParameterName))
                return;
            param.Value = param.Value ?? DBNull.Value;
            IDataParameter arg = EDataParameter.Clone<System.Data.IDataParameter>(cmd.CreateParameter(), param);
            if (arg.Direction != ParameterDirection.Input)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                notinput.Add(param, arg);
            }
            cmd.Parameters.Add(arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notinput"></param>
        private static void ClearNotInput(Dictionary<IDataParameter, IDataParameter> notinput)
        {
            foreach (var item in notinput)
                item.Key.Value = item.Value.Value;
            notinput.Clear();
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
        private static Result Execute<T, Result>(T conn, string sql, IEnumerable<object> args, Func<System.Data.IDbCommand, Result> func) where T : System.Data.IDbConnection
        {
            Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput = new Dictionary<IDataParameter, IDataParameter>();
            bool need_close = conn.State == ConnectionState.Closed;
            System.Data.IDbCommand cmd = EDbConnection.CreateCommand(conn, sql, args, notinput);
            EDbConnection.OpenConnection(conn);
            Result temp = func.Invoke(cmd);
            if (need_close)
                EDbConnection.CloseConnection(conn);
            EDbConnection.ClearNotInput(notinput);
            cmd.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// System.Data.IDbCommand执行ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static int ExecuteNonQuery<T>(T cmd) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// System.Data.IDbCommand执行ExecuteScalar
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static object ExecuteScalar<T>(T cmd) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool TryAddColumn(DataTable table, string name)
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
            Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput = new Dictionary<IDataParameter, IDataParameter>();
            System.Data.IDbCommand cmd = EDbConnection.CreateCommand(conn, sql.Trim(), args, notinput);
            EDbConnection.OpenConnection(conn);
            IDataReader temp = cmd.ExecuteReader(behavior);
            EDbConnection.ClearNotInput(notinput);
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
        /// 设置连接字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="str">没有分号，从app.config里查找</param>
        /// <returns>返回参数连接对象</returns>
        public static T SetConnectionString<T>(this T conn, string str) where T : System.Data.IDbConnection
        {
            if (String.IsNullOrWhiteSpace(str))
                return conn;
            if (str.IndexOf(Assist.SEMICOLON) >= 0)
            {
                conn.ConnectionString = str;
                return conn;
            }
            ConnectionStringSettings conn_str_set = ConfigurationManager.ConnectionStrings[str];
            //if (conn_str_set != null)
                conn.ConnectionString = conn_str_set.ConnectionString;
            
            return conn;
        }

    }
}