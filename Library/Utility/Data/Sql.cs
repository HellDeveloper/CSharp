﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Generic;
using Utility.Core;

namespace Utility.Data
{
    /// <summary>
    /// SQL语句
    /// </summary>
    public static class Sql
    {
        #region Help
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetFieldName<T>(T param) where T : IDataParameter
        {
            if (String.IsNullOrWhiteSpace(param.SourceColumn) && !String.IsNullOrWhiteSpace(param.ParameterName))
                return param.ParameterName.TrimStart(EDataParameter.PARAMETER_NAME_PERFIX);
            return Assist.GetBeforeFirstWhiteSpace(param.SourceColumn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string FormatSqlValue<T>(this T param) where T : IDataParameter
        {
            if (param.Value == null || DBNull.Value.Equals(param.Value))
                return "NULL";
            else if (param.Value is String)
                if (param.SourceVersion == DataRowVersion.Original && param.GetComparer().EndsWith(" like", StringComparison.OrdinalIgnoreCase))
                    return String.Format("'%{0}%'", ((String)param.Value).Replace("'", "''"));
                else
                    return String.Format("'{0}'", ((String)param.Value).Replace("'", "''"));
            else if (param is DateTime)
                return String.Format("'{0}'", ((DateTime)param.Value).ToString(Assist.ISO_DATETIME_FORMAT));
            else if (param.Value is bool)
                return ((bool)param.Value) == false ? "0" : "1";
            else
                return param.ToString();
        }

        /// <summary>
        /// 返回ParameterName
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object GetParameterName<T>(T param) where T : IDataParameter
        {
            return param.ParameterName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object GetParameterValue(IDataParameter param)
        {
            if (String.IsNullOrWhiteSpace(param.ParameterName))
                return param.Value == null ? null : param.Value.ToString();
            return FormatSqlValue(param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object BuildParameterValue(IDataParameter param)
        {
            if (String.IsNullOrWhiteSpace(param.ParameterName))
                return param.Value == null ? null : param.Value.ToString();
            return param.ParameterName;
        }

        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetSetSql<T>(T param) where T : IDataParameter
        {
            return String.Format("{0} = {1}", Sql.GetFieldName(param), Sql.GetParameterValue(param));
        }

        /// <param name="param"></param>
        /// <returns></returns>
        public static string BuildSetSql<T>(T param) where T : IDataParameter
        {
            return String.Format("{0} = {1}", Sql.GetFieldName(param), Sql.BuildParameterValue(param));
        }
        #endregion

        #region Core
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string ConditionSql(IDataParameter param, Func<IDataParameter, object> func)
        {
            if (String.IsNullOrWhiteSpace(param.ParameterName))
                return param.Value == null ? null : param.Value.ToString();
            if (param.SourceVersion == DataRowVersion.Original)
                if (String.IsNullOrWhiteSpace(param.Value.TryToString()))
                    return null;
            return String.Format("{0}{1}{2}", param.SourceColumn, Assist.WHITE_SPACE, func(param));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string ConditionSql(IEnumerable<IDataParameter> args, Func<IDataParameter, object> func)
        {
            var temp = EEnumerable.ToStringBuilder(args, func, " AND ");
            return temp == null ? String.Empty : temp.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string ConditionSql(IEnumerable<IEnumerable<IDataParameter>> args, Func<IEnumerable<IDataParameter>, object> func)
        {
            var temp = EEnumerable.ToStringBuilder(args, func, " OR ");
            return temp == null ? String.Empty : temp.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string InsertSql(IEnumerable<IDataParameter> args, string table_name, Func<IDataParameter, object> func) 
        {
            var tuple = EEnumerable.ToStringBuilder(args, Sql.GetFieldName, func, ", ");
            return String.Format("INSERT INTO {0} ({1}) VALUES ({2})", table_name, tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="where"></param>
        /// <param name="table_name"></param>
        /// <param name="func_args"></param>
        /// <param name="func_where"></param>
        /// <returns></returns>
        public static string UpdateSql(IEnumerable<IDataParameter> args, IEnumerable<IDataParameter> where, string table_name, Func<IDataParameter, object> func_args, Func<IEnumerable<IDataParameter>, string> func_where)
        {
            var sets = EEnumerable.ToStringBuilder(args, func_args);
            if (sets == null)
                return String.Empty;
            var wheres = func_where(where);
            if (String.IsNullOrWhiteSpace(wheres))
                return String.Format("UPDATE {0} SET {1} ", table_name, sets);
            return String.Format("UPDATE {0} SET {1} WHERE {2}", table_name, sets, wheres);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string DeleteSql(IEnumerable<IDataParameter> args, string table_name, Func<IEnumerable<IDataParameter>, string> func)
        {
            string where = func(args);
            if (String.IsNullOrWhiteSpace(where))
                return "DELETE FROM " + table_name;
            return String.Format("DELETE FROM {0} WHERE {1}", table_name, where);
        }
        #endregion

        #region Get SQL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetConditionSql(this IDataParameter param)
        {
            return Sql.ConditionSql(param, Sql.FormatSqlValue);
        }

        /// <summary>
        /// AND
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetConditionSql(this IEnumerable<IDataParameter> args)
        {
            return Sql.ConditionSql(args, Sql.GetConditionSql);
        }

        /// <summary>
        /// OR
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetConditionSql(this IEnumerable<IEnumerable<IDataParameter>> args)
        {
            return Sql.ConditionSql(args, Sql.GetConditionSql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string GetInsertSql(this IEnumerable<IDataParameter> args, string table_name)
        {
            return Sql.InsertSql(args, table_name, Sql.GetParameterValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="where"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string GetUpdateSql(IEnumerable<IDataParameter> args, IEnumerable<IDataParameter> where, string table_name)
        {
            return Sql.UpdateSql(args, where, table_name, Sql.GetSetSql, Sql.GetConditionSql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string GetDeleteSql(this IEnumerable<IDataParameter> args, string table_name)
        {
            return Sql.DeleteSql(args, table_name, Sql.GetConditionSql);
        }
        #endregion

        #region Build SQL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string BuildConditionSql(this IDataParameter param)
        {
            return Sql.ConditionSql(param, Sql.GetParameterName);
        }

        /// <summary>
        /// AND
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string BuildConditionSql(this IEnumerable<IDataParameter> args)
        {
            return Sql.ConditionSql(args, Sql.BuildConditionSql);
        }

        /// <summary>
        /// OR
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string BuildConditionSql(this IEnumerable<IEnumerable<IDataParameter>> args)
        {
            return Sql.ConditionSql(args, Sql.BuildConditionSql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string BuildInsertSql(this IEnumerable<IDataParameter> args, string table_name)
        {
            return Sql.InsertSql(args as IEnumerable<IDataParameter>, table_name, Sql.BuildParameterValue);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string BuildUpdateSql(this IEnumerable<IDataParameter> args, string table_name, IEnumerable<IDataParameter> where)
        {
            return Sql.UpdateSql(args, where, table_name, Sql.BuildSetSql, Sql.BuildConditionSql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public static string BuildDeleteSql(this IEnumerable<IDataParameter> args, string table_name)
        {
            return Sql.DeleteSql(args, table_name, Sql.BuildConditionSql);
        }
        #endregion
    }

}
