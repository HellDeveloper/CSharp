using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Utility.Collections;
using Utility.Core;
using Utility.Data;

namespace UnitTest
{
    /// <summary>
    /// SQL 的摘要说明
    /// </summary>
    [TestClass]
    public class Data
    {
        public const string CONNECTION_STRING = "UnitTest";

        public const string TABLE_NAME = "Profile";

        public Data()
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
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
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
        public void SetConnectionString()
        {
            SqlConnection conn = new SqlConnection();
            conn.SetConnectionString("UnitTest11");
        }

        [TestMethod]
        public void ExectueInsertSql()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                
                conn.SetConnectionString("UnitTest");
                conn.OpenConnection();

                List<SqlParameter> build = new List<SqlParameter>();
                build.Add("@Name", "build", "Name =");
                build.Add("@Sex", false, "");
                build.Add("@BornDate", "1990-10-10", "BornDate =");
                conn.ExecuteNonQuery(build.BuildInsertSql(TABLE_NAME), build);

                List<SqlParameter> get = new List<SqlParameter>();
                get.Add("@Name", "get", "Name =");
                get.Add("@Sex", false, "Sex =");
                get.Add("@BornDate", DateTime.Now, "BornDate");
                conn.ExecuteNonQuery(get.GetInsertSql(TABLE_NAME), get);

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add("", "'sql'", "Name =");
                sql.Add("", 1, "Sex = ");
                sql.Add("", "'1990-10-10'", "BornDate =");
                conn.ExecuteNonQuery(sql.BuildInsertSql(TABLE_NAME), sql);
            }
            
        }

        [TestMethod]
        public void ExectueDeleteSql()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.SetConnectionString("UnitTest");
                conn.OpenConnection();
                List<SqlParameter> build = new List<SqlParameter>();
                build.Add("@Name", "build", "Name =");
                build.Add("@Sex", "1", "Sex =");
                conn.ExecuteNonQuery(build.BuildDeleteSql(TABLE_NAME), build);

                List<SqlParameter> get = new List<SqlParameter>();
                get.Add("@Name", "g'e't", "Name =");
                get.Add("@Sex", false, "Sex =");
                conn.ExecuteNonQuery(get.GetDeleteSql(TABLE_NAME), get);

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add("", "Name = 'sql'", "");
                sql.Add("", "Sex = 0", "");
                conn.ExecuteNonQuery(sql.GetDeleteSql(TABLE_NAME), sql);
            }
        }
    }
}
