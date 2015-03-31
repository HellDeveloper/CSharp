using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Utility.Generic;
using Utility.Core;
using Utility.Data;
using System.Web;
using System.Security.Cryptography;

namespace UnitTest
{
    [TestClass]
    public class Core
    {

        public const string str = "123456";

        public Core()
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
        public void Replace()
        {
            string str = "01234567";
            Assist.Replace(str, 'A', 0, 2, 4, 6, 8, 10);
            // result:
            // str = "A1A3A5A7"
        }

        [TestMethod]
        public void TryToInt()
        {
            string s = "0x890"; // 
            int try_to_int = s.TryToInt() ?? int.MinValue;
            // int convert_int = Convert.ToInt32(s);
            // 两个都转换失败，TryToInt 不抛异常，只返回null
        }

        [TestMethod]
        public void Aes()
        {
            string original = "Here is some data to encrypt";
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                byte[] encrypted = Security.AesEncrypt(original, aes.Key, aes.IV);
                string roundtrip = Security.AesDecrypt(encrypted, aes.Key, aes.IV);
            }
        }

        [TestMethod]
        public void Des()
        {
            string original = "Here is some data to encrypt";
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                byte[] encrypted = Security.DesEncrypt(original, des.Key, des.IV);
                string roundtrip = Security.DesDecrypt(encrypted, des.Key, des.IV);
            }
        }

        [TestMethod]
        public void SHA1()
        {
            var result = Security.SHA1Encrypt(str);
        }

        [TestMethod]
        public void RSA()
        {
            string public_key, private_key;
            Security.RSAKey(out private_key, out public_key);
            var bytes = Security.RSAEncrypt(public_key, str);
            var result = Security.RSADecrypt(private_key, bytes);
        }

        [TestMethod]
        public void MongoID()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < 100000; i++)
            {
                list.Add(Assist.MongoID);
            }

        }

    }
}
