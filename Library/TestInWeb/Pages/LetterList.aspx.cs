using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility.Core;
using Utility.Data;
using Utility.WebForm;

namespace TestInWeb.Pages
{
    public partial class LetterList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!this.IsPostBack)
            {
                BindingGridView();
            }
        }

        protected void BindingGridView(int pageIndex = 0)
        {
            var args = this.pnlSearch.CreateParameters<SqlParameter>();
            args.RemoveByParameterName("@A");
            string condition = args.BuildConditionSql();
            string sql = String.Empty;
            if (String.IsNullOrWhiteSpace(condition))
                sql = "SELECT * FROM Letter";
            else
                sql = String.Format("SELECT * FROM Letter WHERE {0}", condition);
            sql = this.Connection.Limit(sql, 0, 10); // 拿前10条
            DataTable dt = this.Connection.ExecuteDataTable(sql, args);
            this.gv.DataSource = dt;
            this.gv.DataBind();
        }

        protected void gv_RowDataBound(Control control, string fieldname, object value)
        {
            if (fieldname == "SendTime" && control is ITextControl)
            {
                DateTime? dt = value.TryToString().TryToDateTime();
                if (dt.HasValue)
                {
                    (control as ITextControl).Text = dt.Value.ToString("yyyy-MM-dd");
                }
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex >= 0)
            {
                DataTable dt = this.gv.DataSource as DataTable;
                e.Row.FillData(dt.Rows[e.Row.RowIndex], gv_RowDataBound);
            }
        }

        protected void gv_RowCreated(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gv_DataBound(object sender, EventArgs e)
        {

        }

        protected void gv_DataBinding(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindingGridView();
        }
    }
}