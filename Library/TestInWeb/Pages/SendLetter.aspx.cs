using System;
using System.Collections.Generic;
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
    public partial class SendLetter : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string snid = this.Request.QueryString["SNID"];
            var args = this.form1.CreateParameters<SqlParameter>();
            if (String.IsNullOrWhiteSpace(snid)) // 插入
            {
                snid = System.DateTime.Now.Ticks.ToString();
                args.Add("@Category", "I Don't Know", "Category");
                args.Add("", "'" + snid + "'", "SNID"); // 拼接SQL
                string sql = this.Connection.BuildInsertSql("Letter", args);
                this.Connection.ExecuteNonQuery(sql, args);
                this.btnSend.Text = "更新";
                this.form1.Action = "./SendLetter.aspx?SNID=" + snid;
            }
            else // 更新
            {
                List<SqlParameter> where = new List<SqlParameter>();
                where.Add("@SNID", snid, "SNID =");
                string sql = this.Connection.BuildUpdateSql("Letter", args, where);
                this.Connection.ExecuteNonQuery(sql, args.Concat(where));
            }
        }

    }
}