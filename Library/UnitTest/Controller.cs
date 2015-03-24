using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility.Core;
using Utility.Data;
using Utility.Generic;
using Utility.WebForm;

namespace UnitTest
{
    /// <summary>
    /// SQL 的摘要说明
    /// </summary>
    [TestClass]
    public class Controller
    {
        public const string CONNECTION_STRING = "UnitTest";

        public const string TABLE_NAME = "Profile";

        public Controller()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) 
        {
            testContext.AddResultFile("TestResult.txt");
        }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ExectueInsertSql()
        {
            using (var conn = Factory.CreateConnection())
            {
                Panel pnl = new Panel();
                pnl.Controls.Add(CreateTextBox("FromName", "Tom", "FromName"));
                pnl.Controls.Add(CreateTextBox("ToName", "Mary", "ToName"));
                pnl.Controls.Add(CreateTextBox("Title", "Hi", "Title"));
                pnl.Controls.Add(CreateTextBox("Contents", "最近过得怎么样？！", "Contents"));
                pnl.Controls.Add(CreateTextBox("SendTime", DateTime.Now.ToString(), "SendTime"));
                pnl.Controls.Add(CreateTextBox(null, "NULL", "ReadTime")); // 拼接SQL
                pnl.Controls.Add(CreateTextBox("@Category", "问候", "Category"));
                var args = pnl.CreateParameters<SqlParameter>();
                string build_sql = args.BuildInsertSql(TABLE_NAME);
                string get_sql = args.GetInsertSql(TABLE_NAME);
            }
            
        }

        [TestMethod]
        public void ExectueDeleteSql()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                Panel pnl = new Panel();
                pnl.Controls.Add(CreateTextBox("FromName", "Tom", "FromName LIKE"));
                pnl.Controls.Add(CreateTextBox("ToName", "Mary", "ToName ="));
                pnl.Controls.Add(CreateTextBox("Title", "", "Title LIKE"));
                pnl.Controls.Add(CreateTextBox("Contents", "", "Contents LIKE"));
                pnl.Controls.Add(CreateTextBox("SendTime", "", "SendTime LIKE"));
                pnl.Controls.Add(CreateTextBox(null, "ReadTime IS NULL", " J")); // 拼接SQL
                pnl.Controls.Add(CreateTextBox("Category", "", "Category Like"));
                var args = pnl.CreateParameters<SqlParameter>();
                string build_sql = args.BuildDeleteSql(TABLE_NAME);
                string get_sql = args.GetDeleteSql(TABLE_NAME);
            }
        }

        public TextBox CreateTextBox(string id, string text, string fieldname)
        {
            TextBox txt = new TextBox()
            {
                ID = id,
                Text = text,
            };
            txt.Attributes["data-fieldname"] = fieldname;
            return txt;
        } 

    }
}
