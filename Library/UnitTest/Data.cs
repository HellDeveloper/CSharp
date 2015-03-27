using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Utility.Generic;
using Utility.Core;
using Utility.Data;
using System.Data;

namespace UnitTest
{
    /// <summary>
    /// SQL 的摘要说明
    /// </summary>
    [TestClass]
    public class Data
    {
        public const string CONNECTION_STRING = "UnitTest";

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
        public void SetConnectionString()
        {
            SqlConnection conn = new SqlConnection();
            conn.SetConnectionString("UnitTest");
        }

        [TestMethod]
        public void ExectueInsertSql()
        {
            using (var conn = Factory.CreateConnection())
            {
                var args = Factory.CreateParameters();
                args.Add("@FromName", "Jack", "FromName =");
                args.Add("@ToName", "Mary", "ToName =");
                args.Add("@Title", "Hi", "Title =");
                args.Add("@Contents", "最近过得怎么样？！", "Contents =");
                args.Add("@SendTime", DateTime.Now, "SendTime =");
                args.Add(null, "NULL", "ReadTime"); // 拼接SQL
                args.Add("@Category", "问候", "Category =");
                string insert_sql = args.BuildInsertSql(Factory.LETTER_TABLE);
                /*
                 INSERT INTO Letter 
                 (FromName, ToName, Title, Contents, SendTime, ReadTime, Category) 
                 VALUES (@FromName, @ToName, @Title, @Contents, @SendTime, NULL, @Category)
                 */
                int result = conn.ExecuteNonQuery(insert_sql, args);
            }   
        }

        [TestMethod]
        public void ExecuteUpdateSql()
        {
            using (var conn = Factory.CreateConnection())
            {
                var args = Factory.CreateParameters();
                args.Add("@ReadTime", DateTime.Now, "ReadTime =");
                args.Add("", "'Kate'", "FromName ="); // 拼接SQL, ParameterName is null or whitespace
                var where = Factory.CreateParameters();
                where.Add("@FromName", "Jack", "FromName =");
                where.Add("", "ReadTime IS NULL", "");// 拼接SQL, ParameterName is null or whitespace
                string update_sql = args.BuildUpdateSql(Factory.LETTER_TABLE, where);
                args.AddRange(where);
                conn.ExecuteNonQuery(update_sql, args);
            }
        }

        [TestMethod]
        public void ExectueDeleteSql()
        {
            using (SqlConnection conn = Factory.CreateConnection())
            {
                var args = Factory.CreateParameters();
                args.Add(new SqlParameter("@Title", "Hi") { SourceColumn = "Title =" });
                args.Add(new SqlParameter("@Content", "%？！") { SourceColumn = "Contents LIKE" });
                args.Add(new SqlParameter("", "Contents IS NULL")); // 拼接SQL
                string conditions = args.GetConditionSql();
                string delete_sql = String.Format("DELETE FROM {0} WHERE {1}", Factory.LETTER_TABLE, conditions);
                conn.ExecuteNonQuery(delete_sql);
            }
        }

        [TestMethod]
        public void ExecuteSelectSql()
        {
            using (SqlConnection conn = Factory.CreateConnection())
            {
                List<List<SqlParameter>> list = new List<List<SqlParameter>>() 
                {
                    new List<SqlParameter>(),
                    new List<SqlParameter>()
                };
                list[0].Add(new SqlParameter("@Title", "Hi") { SourceColumn = "Title =" });
                list[0].Add(new SqlParameter("@Content1", "%？！") { SourceColumn = "Contents LIKE" });
                list[0].Add(new SqlParameter("", "Contents IS NULL")); // 拼接SQL
                list[1].Add(new SqlParameter("@Title2", "Hi") { SourceColumn = "Title =" });
                list[1].Add(new SqlParameter("@Content2", "%？！") { SourceColumn = "Contents LIKE" });
                list[1].Add(new SqlParameter("", "Contents IS NULL")); // 拼接SQL
                string conditions = list.GetConditionSql();
                string sql = String.Empty;
                if (String.IsNullOrWhiteSpace(conditions))
                    sql = String.Format("SELECT * FROM {0}", Factory.LETTER_TABLE);
                else
                    sql = String.Format("SELECT * FROM {0} WHERE {1}", Factory.LETTER_TABLE, conditions);
                DataTable dt = conn.ExecuteDataTable(sql, list);
            }
        }

    }
}
