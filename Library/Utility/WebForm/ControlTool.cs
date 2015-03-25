using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utility.Core;

namespace Utility.WebForm
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControlTool
    {
        /// <summary>
        /// data-fieldname
        /// </summary>
        public const string DATA_FIELDNAME = "data-fieldname";

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
            string fieldname = ControlTool.GetDataFieldName(control);
            if (fieldname == null)
                return null;
            object value = GetValue(control);
            if (String.IsNullOrWhiteSpace(control.ID))
                return CreateParameter<T>(null, value, fieldname);
            return CreateParameter<T>("@" + control.ID, value, fieldname);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private static object GetValue(Control control)
        {
            if (control is ITextControl)
                return (control as ITextControl).Text;
            else if (control is HtmlInputControl)
                return (control as HtmlInputControl).Value;
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="maxLevel">control递归的次数</param>
        /// <returns></returns>
        public static List<T> CreateParameters<T>(this Control control, int maxLevel = 2) where T : class, IDataParameter, new()
        {
            List<T> list = new List<T>();
            if (0 > maxLevel)
                return list;
            create_parameters<T>(control, list, 1, maxLevel);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="list"></param>
        /// <param name="currentLevel"></param>
        /// <param name="maxLevel">control递归的次数</param>
        private static void create_parameters<T>(Control control, List<T> list, int currentLevel, int maxLevel) where T : class, IDataParameter, new()
        {
            T t = CreateParameter<T>(control);
            if (t != null)
                list.Add(t);
            if (currentLevel < maxLevel)
                foreach (Control ctrl in control.Controls)
                    create_parameters(ctrl, list, currentLevel + 1, maxLevel);
        }

        /// <summary>
        /// (control as IAttributeAccessor).GetAttribute(DATA_FIELDNAME)
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static string GetDataFieldName(this Control control)
        {
            if (!(control is IAttributeAccessor))
                return null;
            return (control as IAttributeAccessor).GetAttribute(DATA_FIELDNAME);
        }

        private static string get_field_name(Control control)
        {
            string fieldname = GetDataFieldName(control);
            if (String.IsNullOrWhiteSpace(fieldname))
                return null;
            int index = fieldname.IndexOf(Assist.WHITE_SPACE);
            if (index <= 0)
                return fieldname;
            return fieldname.Substring(0, index);
        }

        /// <summary>
        /// attribute.SetAttribute(DATA_FIELDNAME, value);
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool SetDataFieldName(this Control control, string value)
        {
            IAttributeAccessor attribute = control as IAttributeAccessor;
            if (attribute == null)
                return false;
            attribute.SetAttribute(DATA_FIELDNAME, value);
            return true;
        }

        /// <summary>
        /// 在control子控件填充数据
        /// </summary>
        /// <param name="control"></param>
        /// <param name="row"></param>
        public static void FillData(this Control control, DataRow row, Action<Control, string, object> after_action = null, int maxLevel = 2)
        {
            if (row.Table == null)
                return;
            fill_data(control, row, after_action, 1, maxLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="row"></param>
        /// <param name="currentLevel"></param>
        /// <param name="maxLevel"></param>
        private static void fill_data(Control control, DataRow row, Action<Control, string, object> after_action, int currentLevel, int maxLevel)
        {
            string fieldname = get_field_name(control);
            if (!String.IsNullOrWhiteSpace(fieldname))
            {
                int index = row.Table.Columns.IndexOf(fieldname);
                if (index >= 0)
                {
                    fill_data(control, fieldname, row[index]);
                    if (after_action != null)
                        after_action.Invoke(control, fieldname, row[index]);
                }
            }
            if (currentLevel < maxLevel)
                foreach (Control ctrl in control.Controls)
                    fill_data(ctrl, row, after_action, currentLevel + 1, maxLevel);
        }

        private static void fill_data(Control control, string name, object value)
        {
            value = value ?? String.Empty;
            if (control is System.Web.UI.ITextControl)
                (control as System.Web.UI.ITextControl).Text = value.ToString();
            else if (control is System.Web.UI.HtmlControls.HtmlInputControl)
                (control as System.Web.UI.HtmlControls.HtmlInputControl).Value = value.ToString();
            else if (control is System.Web.UI.IAttributeAccessor)
                (control as System.Web.UI.IAttributeAccessor).SetAttribute("value", value.ToString());

        }



    }
}
