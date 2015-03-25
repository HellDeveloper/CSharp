using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Utility.Core;

namespace Utility.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class TDbConnection
    {

        /// <summary>
        /// 创建Command对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">连接对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="args">参数</param>
        /// <param name="notinput"></param>
        /// <returns></returns>
        private static System.Data.IDbCommand create_command<T>(this T conn, string sql, IEnumerable<object> args, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput) where T : System.Data.IDbConnection
        {
            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            TDbConnection.add_parameter(cmd, args, notinput, 0);
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
        private static T add_parameter<T>(T cmd, IEnumerable<object> args, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput, int increment) where T : System.Data.IDbCommand
        {
            foreach (var item in args)
            {
                if (item == null)
                    continue;
                else if (item is IEnumerable<object>)
                    TDbConnection.add_parameter(cmd, item as IEnumerable<object>, notinput, increment);
                else if (item is IDataParameter)
                    TDbConnection.add_parameter(cmd, item as IDataParameter, notinput);
                else
                    TDbConnection.add_parameter(cmd, item, increment++);
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
        private static void add_parameter<T>(T cmd, System.Data.IDataParameter param, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput) where T : System.Data.IDbCommand
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

        private static void add_parameter<T>(T cmd, object o, int increment) where T : System.Data.IDbCommand
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = "@" + increment;
            param.Value = o;
            cmd.Parameters.Add(param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notinput"></param>
        private static void clear_not_input(Dictionary<IDataParameter, IDataParameter> notinput)
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
        /// <param name="get_result"></param>
        /// <returns></returns>
        private static Result run<T, Data, Result>(T conn, string sql, IEnumerable<object> args, Func<T, string, IEnumerable<object>, Dictionary<IDataParameter, IDataParameter>, IDbCommand> create_cmd, Func<System.Data.IDbCommand, Data, Result> get_result, Data data) where T : System.Data.IDbConnection
        {
            Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput = new Dictionary<IDataParameter, IDataParameter>();
            bool need_close = conn.State == ConnectionState.Closed;
            System.Data.IDbCommand cmd = create_cmd(conn, sql, args, notinput);
            TDbConnection.OpenConnection(conn);
            Result temp = get_result.Invoke(cmd, data);
            if (need_close && !(temp is IDataReader))
                TDbConnection.CloseConnection(conn);
            TDbConnection.clear_not_input(notinput);
            cmd.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// System.Data.IDbCommand执行ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static int non_query<T>(T cmd, string str) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// System.Data.IDbCommand执行ExecuteScalar
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static object scalar<T>(T cmd, string str) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteScalar();
        }

        private static IDataReader reader<T>(T cmd, CommandBehavior behavior) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteReader(behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool try_add_column(DataTable table, string name)
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

    }


    /// <summary>
    /// 
    /// </summary>
    public static partial class TDbConnection
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
                if (TDbConnection.try_add_column(table, reader.GetName(i)))
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

    }

    /// <summary>
    /// 
    /// </summary>
    public static partial class TDbConnection
    {
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args) where T : System.Data.IDbConnection
        {
            return TDbConnection.run(conn, sql, args, TDbConnection.create_command, TDbConnection.non_query, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(this T conn, string sql, params System.Data.IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return TDbConnection.run(conn, sql, args, TDbConnection.create_command, TDbConnection.non_query, String.Empty);
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
            return TDbConnection.run(conn, sql, args, TDbConnection.create_command, TDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object ExecuteScalar<T>(this T conn, string sql, params System.Data.IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return TDbConnection.run(conn, sql, args, TDbConnection.create_command, TDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args, CommandBehavior behavior = CommandBehavior.CloseConnection) where T : System.Data.IDbConnection
        {
            return TDbConnection.run(conn, sql, args, TDbConnection.create_command, TDbConnection.reader, behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="behavior"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader<T>(this T conn, string sql, CommandBehavior behavior = CommandBehavior.CloseConnection, params System.Data.IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return TDbConnection.run(conn, sql, args, TDbConnection.create_command, TDbConnection.reader, behavior);
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
            IDataReader reader = TDbConnection.ExecuteReader(conn, sql, args, behavior);
            Result temp = func(reader);
            if (!reader.IsClosed)
                reader.Close();
            return temp; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Result"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Result ExecuteReader<T, Result>(this T conn, string sql, Func<IDataReader, Result> func, params System.Data.IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return TDbConnection.ExecuteReader(conn, sql, args, func);
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable ExecuteGetDataTable<T>(this T conn, string sql, IEnumerable<System.Data.IDataParameter> args) where T : System.Data.IDbConnection
        {
            return TDbConnection.ExecuteReader(conn, sql, args, TDbConnection.IDataReaderToDataTable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable ExecuteGetDataTable<T>(this T conn, string sql, params System.Data.IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return TDbConnection.ExecuteReader(conn, sql, args, TDbConnection.IDataReaderToDataTable);
        }

    }

}