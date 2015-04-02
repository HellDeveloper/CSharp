using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Utility.Core;

namespace Utility.Data
{
    #region 核心
    /// <summary>
    /// 
    /// </summary>
    public static partial class EDbConnection
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
        private static IDbCommand execute_create_command<T>(this T conn, string sql, IEnumerable<object> args, Dictionary<IDataParameter, IDataParameter> notinput) where T : IDbConnection
        {
            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            EDbConnection.add_parameter(cmd, args, notinput);
            if (cmd.CommandType != CommandType.StoredProcedure && sql.IndexOf(Utility.Core.Assist.WHITE_SPACE, 0) == -1)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="notinput"></param>
        /// <returns></returns>
        private static IDbCommand number_create_command<T>(this T conn, string sql, IEnumerable<object> args, Dictionary<IDataParameter, IDataParameter> notinput) where T : IDbConnection
        {
            System.Data.IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            int increment = 0;
            foreach (var item in args)
            { 
                IDataParameter param = cmd.CreateParameter();
                param.ParameterName = CommonSql.ParameterNamePerfix + (increment++).ToString();
                param.Value = item;
                cmd.Parameters.Add(param);
            }
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
        private static T add_parameter<T>(T cmd, IEnumerable<object> args, Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput) where T : System.Data.IDbCommand
        {
            foreach (var item in args)
            {
                if (item == null)
                    continue;
                else if (item is IEnumerable<object>)
                    EDbConnection.add_parameter(cmd, item as IEnumerable<object>, notinput);
                else if (item is IDataParameter)
                    EDbConnection.add_parameter(cmd, item as IDataParameter, notinput);
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
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="create_command"></param>
        /// <param name="func"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Result run<Result, T, Data>(T conn, string sql, IEnumerable<object> args, Func<T, string, IEnumerable<object>, Dictionary<IDataParameter, IDataParameter>, IDbCommand> create_command, Func<System.Data.IDbCommand, Data, Result> func, Data data) where T : System.Data.IDbConnection
        {
            Dictionary<System.Data.IDataParameter, System.Data.IDataParameter> notinput = new Dictionary<IDataParameter, IDataParameter>();
            bool need_close = conn.State == ConnectionState.Closed;
            System.Data.IDbCommand cmd = create_command(conn, sql, args, notinput);
            EDbConnection.OpenConnection(conn);
            Result temp = func.Invoke(cmd, data);
            if (need_close && !(temp is IDataReader))
                EDbConnection.CloseConnection(conn);
            EDbConnection.clear_not_input(notinput);
            cmd.Parameters.Clear();
            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Result"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="create_command"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static Result run<Result, T>(T conn, string sql, IEnumerable<object> args, Func<T, string, IEnumerable<object>, Dictionary<IDataParameter, IDataParameter>, IDbCommand> create_command, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            CommandBehavior behavior = conn.State == ConnectionState.Closed ? CommandBehavior.CloseConnection : CommandBehavior.Default;
            IDataReader reader = EDbConnection.run(conn, sql, args, create_command, EDbConnection.reader, behavior);
            Result temp = func(reader);
            if (!reader.IsClosed)
                reader.Close();
            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Data"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Result execute<Result, T, Data>(T conn, string sql, IEnumerable<object> args, Func<System.Data.IDbCommand, Data, Result> func, Data data) where T : System.Data.IDbConnection
        {
            return EDbConnection.run(conn, sql, args, EDbConnection.execute_create_command, func, data);
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
        private static Result execute<Result, T>(T conn, string sql, IEnumerable<object> args, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            return EDbConnection.run(conn, sql, args, EDbConnection.execute_create_command, func);
        }

        private static Result number<Result, T, Data>(T conn, string sql, IEnumerable<object> args, Func<System.Data.IDbCommand, Data, Result> func, Data data) where T : System.Data.IDbConnection
        {
            return EDbConnection.run(conn, sql, args, EDbConnection.number_create_command, func, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static Result number<Result, T>(T conn, string sql, IEnumerable<object> args, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            return EDbConnection.run(conn, sql, args, EDbConnection.number_create_command, func);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int non_query<T>(T cmd, string str) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static object scalar<T>(T cmd, string str) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        private static IDataReader reader<T>(T cmd, CommandBehavior behavior) where T : System.Data.IDbCommand
        {
            return cmd.ExecuteReader(behavior);
        }
    }
    #endregion

    #region 辅助
    /// <summary>
    /// 
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

        /// <summary> MongoDB 生成 唯一 的 ID </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string MongoID<T>(this T conn) where T : System.Data.IDbConnection
        {
            return Assist.MongoID;
        }

        /// <summary> IDataReader 转 DataTable </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable IDataReaderToDataTable(IDataReader reader)
        {
            DataTable table = new DataTable();
            List<int> list = new List<int>();
            for (int i = 0; i < reader.FieldCount; i++)
                if (EDbConnection.try_add_column(table, reader.GetName(i)))
                    list.Add(i);
            while (reader.Read())
            {
                object[] array = new object[list.Count];
                int index = 0;
                foreach (var item in list)
                    array[index++] = reader.GetValue(item);
                table.Rows.Add(array);
            }
            if (!reader.IsClosed)
                reader.Close();
            return table;
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
    #endregion

    #region execute 和 number   调用核心
    /// <summary>
    /// 
    /// </summary>
    public static partial class EDbConnection
    {
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(this T conn, string sql, IEnumerable<IDataParameter> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.non_query, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(this T conn, string sql, IEnumerable<IEnumerable<IDataParameter>> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.non_query, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(this T conn, string sql, params IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.non_query, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object ExecuteScalar<T>(this T conn, string sql, IEnumerable<IDataParameter> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object ExecuteScalar<T>(this T conn, string sql, IEnumerable<IEnumerable<IDataParameter>> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object ExecuteScalar<T>(this T conn, string sql, params IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader<T>(this T conn, string sql, IEnumerable<IDataParameter> args, CommandBehavior behavior = CommandBehavior.CloseConnection) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.reader, behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader<T>(this T conn, string sql, IEnumerable<IEnumerable<IDataParameter>> args, CommandBehavior behavior = CommandBehavior.CloseConnection) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.reader, behavior);
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
        public static IDataReader ExecuteReader<T>(this T conn, string sql, CommandBehavior behavior = CommandBehavior.CloseConnection, params IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.reader, behavior);
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
        public static Result ExecuteReader<Result, T>(this T conn, string sql, IEnumerable<IDataParameter> args, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, func);
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
        public static Result ExecuteReader<Result, T>(this T conn, string sql, IEnumerable<IEnumerable<IDataParameter>> args, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, func);
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
        public static Result ExecuteReader<Result, T>(this T conn, string sql, Func<IDataReader, Result> func, params IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, func);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable<T>(this T conn, string sql, IEnumerable<IDataParameter> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.IDataReaderToDataTable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable<T>(this T conn, string sql, IEnumerable<IEnumerable<IDataParameter>> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.IDataReaderToDataTable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable<T>(this T conn, string sql, params IDataParameter[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.execute(conn, sql, args, EDbConnection.IDataReaderToDataTable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int NumberNonQuery<T>(this T conn, string sql, IEnumerable<object> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.non_query, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int NumberNonQuery<T>(this T conn, string sql, params object[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.non_query, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object NumberScalar<T>(this T conn, string sql, IEnumerable<object> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object NumberScalar<T>(this T conn, string sql, params object[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.scalar, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static IDataReader NumberReader<T>(this T conn, string sql, IEnumerable<object> args, CommandBehavior behavior = CommandBehavior.CloseConnection) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.reader, behavior);
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
        public static IDataReader NumberReader<T>(this T conn, string sql, CommandBehavior behavior = CommandBehavior.CloseConnection, params object[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.reader, behavior);
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
        public static Result NumberReader<Result, T>(this T conn, string sql, IEnumerable<object> args, Func<IDataReader, Result> func) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, func);
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
        public static Result NumberReader<Result, T>(this T conn, string sql, Func<IDataReader, Result> func, params object[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, func);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable NumberDataTable<T>(this T conn, string sql, IEnumerable<object> args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.IDataReaderToDataTable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable NumberDataTable<T>(this T conn, string sql, params object[] args) where T : System.Data.IDbConnection
        {
            return EDbConnection.number(conn, sql, args, EDbConnection.IDataReaderToDataTable);
        }
    }
    #endregion

    #region SQL
    public static partial class EDbConnection
    {
        #region Get SQL
        /// <summary>
        /// 条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetConditionSql<T>(this T conn, IDataParameter param) where T : IDbConnection
        {
            return CommonSql.ConditionSql(param, CommonSql.FormatSqlValue);
        }

        /// <summary>
        /// 条件AND
        /// </summary>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string GetConditionSql<T>(this T conn, IEnumerable<IDataParameter> args) where T : IDbConnection
        {
            return CommonSql.ConditionSql(args, CommonSql.GetConditionSql);
        }

        /// <summary>
        /// 条件OR
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetConditionSql<T>(this T conn, IEnumerable<IEnumerable<IDataParameter>> args) where T : IDbConnection
        {
            return CommonSql.ConditionSql(args, CommonSql.GetConditionSql);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string GetInsertSql<T>(this T conn, string table_name, IEnumerable<IDataParameter> args) where T : IDbConnection
        {
            return CommonSql.InsertSql(args, table_name, CommonSql.GetParameterValue);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="sets">设置</param>
        /// <param name="where">条件</param>
        /// <param name="table_name">表名</param>
        /// <returns></returns>
        public static string GetUpdateSql<T>(this T conn, string table_name, IEnumerable<IDataParameter> sets, IEnumerable<IDataParameter> where) where T : IDbConnection
        {
            return CommonSql.UpdateSql(sets, where, table_name, CommonSql.GetSetSql, CommonSql.GetConditionSql);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="where"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string GetDeleteSql<T>(this T conn, string table_name, IEnumerable<IDataParameter> where) where T : IDbConnection
        {
            return CommonSql.DeleteSql(where, table_name, CommonSql.GetConditionSql);
        }

        /// <summary>
        /// 构建查询SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args">where 条件</param>
        /// <param name="table_naem">表名</param>
        /// <param name="fieldname">查询的字段名(如果是String.Empty || null 就是 *)</param>
        /// <returns>SELECT语句</returns>
        public static string GetSelectSql<T>(this T conn, string table_naem, IEnumerable<IDataParameter> args, string fieldname = null) where T : IDbConnection
        {
            return CommonSql.SelectSql(args, table_naem, CommonSql.GetConditionSql, fieldname);
        }
        #endregion

        #region Build SQL
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string BuildConditionSql<T>(this T conn, IDataParameter param) where T : IDbConnection
        {
            return CommonSql.ConditionSql(param, CommonSql.GetParameterName);
        }

        /// <summary>
        /// AND
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string BuildConditionSql<T>(this T conn, IEnumerable<IDataParameter> args) where T : IDbConnection
        {
            return CommonSql.ConditionSql(args, CommonSql.BuildConditionSql);
        }

        /// <summary>
        /// OR
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string BuildConditionSql<T>(this T conn, IEnumerable<IEnumerable<IDataParameter>> args) where T : IDbConnection
        {
            return CommonSql.ConditionSql(args, CommonSql.BuildConditionSql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string BuildInsertSql<T>(this T conn, string table_name, IEnumerable<IDataParameter> args) where T : IDbConnection
        {
            return CommonSql.InsertSql(args, table_name, CommonSql.BuildParameterValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string BuildUpdateSql<T>(this T conn, string table_name, IEnumerable<IDataParameter> args, IEnumerable<IDataParameter> where) where T : IDbConnection
        {
            return CommonSql.UpdateSql(args, where, table_name, CommonSql.BuildSetSql, CommonSql.BuildConditionSql);
        }

        /// <summary>
        /// 构建删除的SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args"></param>
        /// <param name="table_name">表名</param>
        /// <returns>SQL语句</returns>
        public static string BuildDeleteSql<T>(this T conn, string table_name, IEnumerable<IDataParameter> args) where T : IDbConnection
        {
            return CommonSql.DeleteSql(args, table_name, CommonSql.BuildConditionSql);
        }

        /// <summary>
        /// 构建查询SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="args">where 条件</param>
        /// <param name="table_naem">表名</param>
        /// <param name="fieldname">查询的字段名(如果是String.Empty || null 就是 *)</param>
        /// <returns>SELECT语句</returns>
        public static string BuildSelectSql<T>(this T conn, string table_naem, IEnumerable<IDataParameter> args, string fieldname = null) where T : IDbConnection
        {
            return CommonSql.SelectSql(args, table_naem, CommonSql.BuildConditionSql, fieldname);
        }
        #endregion
    }
    #endregion
}