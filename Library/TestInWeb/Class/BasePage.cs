using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Utility.Core;
using Utility.Data;
using Utility.WebForm;

namespace TestInWeb
{
    public class BasePage : System.Web.UI.Page
    {

        private SqlConnection _connection;

        public SqlConnection Connection
        {
            get 
            {
                if (this._connection == null)
                    this._connection = new SqlConnection().SetConnectionString("default");
                return this._connection; 
            }
        }

        protected override void InitializeCulture()
        {
            
        }

    }
}