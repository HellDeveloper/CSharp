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
            var args = this.form1.CreateParameters<SqlParameter>();
            args.Add("@Category", "I Don't Know", "Category");
            args.Add("", "'" + DateTime.Now.Ticks + "'", "SNID"); // 拼接SQL
            string sql = args.BuildInsertSql("Letter");
            this.Connection.ExecuteNonQuery(sql, args);
        }

    }
}