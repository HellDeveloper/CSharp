using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Utility.WebForm
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControlTool
    {
        #region CreateParameter
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="sourceColumn"></param>
        /// <returns></returns>
        public static T CreateParameter<T>(string parameterName, object value, string sourceColumn) where T : class, IDataParameter, new()
        {
            T t = new T();
            t.ParameterName = parameterName;
            t.SourceVersion = DataRowVersion.Original;
            t.SourceColumn = sourceColumn.Trim();
            t.Value = value;
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <returns></returns>
        public static T CreateParameter<T>(this Control control) where T : class, IDataParameter, new()
        {
            if (!(control is IAttributeAccessor))
               return null;
            string fieldname = (control as IAttributeAccessor).GetAttribute("data-fieldname");
            if (fieldname == null)
                return null;
            string value = String.Empty;
            if (control is ITextControl)
                value = (control as ITextControl).Text;
            if (String.IsNullOrWhiteSpace(control.ID))
                return CreateParameter<T>(null, value, fieldname);
            return CreateParameter<T>("@" + control.ID, value, fieldname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="maxLevel">递归的次数</param>
        /// <returns></returns>
        public static List<T> CreateParameters<T>(this Control control, int maxLevel = 2) where T : class, IDataParameter, new()
        {
            List<T> list = new List<T>();
            if (0 > maxLevel)
                return list;
            T t = CreateParameter<T>(control);
            if (t != null)
                list.Add(t);
            CreateParameters<T>(control, list, 0, maxLevel);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="list"></param>
        /// <param name="currentLevel"></param>
        /// <param name="maxLevel"></param>
        private static void CreateParameters<T>(this Control control, List<T> list, int currentLevel, int maxLevel) where T : class, IDataParameter, new()
        {
            foreach (Control ctrl in control.Controls)
            {
                T t = CreateParameter<T>(ctrl);
                if (t != null)
                    list.Add(t);
                if (currentLevel < maxLevel)
                    CreateParameters(ctrl, list, currentLevel + 1, maxLevel);
            }
        }
        #endregion

        public static string GetDataFieldName(this Control control)
        {
            if (!(control is IAttributeAccessor))
                return null;
            return (control as IAttributeAccessor).GetAttribute("data-fieldname");
        }

    }
}
