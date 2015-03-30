using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Generic;
using Utility.Core;
using Utility.Data;
using Utility.WebForm;
using System.Web.UI.WebControls;

namespace UnitTest
{
    [TestClass]
    class WebForm
    {

        public WebForm()
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

        public string[] Array = { "1", "2", "3", "4" };

        public Dictionary<string, string> Dictionary = new Dictionary<string, string>()
        {
            {"A", "1"},
            {"B", "2"},
            {"C", "3"},
            {"D", "4"},
            {"E", "5"}
        };

        [TestMethod]
        public void AddListItem()
        {
            DropDownList ddl_array = new DropDownList();
            DropDownList ddl_dictionary = new DropDownList();
            ddl_array.Items.AddRange(Array);
            ddl_dictionary.Items.AddRange(Dictionary);
        }

    }
}
