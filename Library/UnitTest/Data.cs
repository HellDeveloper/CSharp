using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Utility.Generic;
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
            conn.SetConnectionString("UnitTest11");
        }

        [TestMethod]
        public void ExectueInsertSql()
        {
            using (var conn = Factory.CreateConnection())
            {
                var args = Factory.CreateParameters();
                args.Add("@FromName", "Tom", "FromName");
                args.Add("@ToName", "Mary", "ToName");
                args.Add("@Title", "Hi", "Title");
                args.Add("@Contents", "最近过得怎么样？！", "Contents");
                args.Add("@SendTime", DateTime.Now, "SendTime");
                args.Add(null, "NULL", "ReadTime"); // 拼接SQL
                args.Add("@Category", "问候", "Category");
                string insert_sql = args.BuildInsertSql(Factory.LETTER_TABLE);
                int result = conn.ExecuteNonQuery(insert_sql, args);
                this.TestContext.WriteLine("ExectueInsertSql");
                this.TestContext.WriteLine("Get SQL:{0}", args.GetInsertSql(TABLE_NAME));
                this.TestContext.WriteLine("Build SQL:{0}", insert_sql);
                this.TestContext.WriteLine("result: {0}", result);
                this.TestContext.WriteLine("");
            }
            
        }

        [TestMethod]
        public void ExectueDeleteSql()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                
            }
        }


    }
}
