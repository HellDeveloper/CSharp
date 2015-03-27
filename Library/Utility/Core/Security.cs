using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class Security
    {
        #region 不可逆
        private static string hash_encrypt<T>(string source) where T : System.Security.Cryptography.HashAlgorithm, new()
        {
            using (T t = new T())
            {
                byte[] data = t.ComputeHash(Encoding.UTF8.GetBytes(source));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    builder.Append(data[i].ToString("x2"));
                return builder.ToString();
            }
        }

        /// <summary>
        /// 不可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string source)
        {
            return hash_encrypt<System.Security.Cryptography.MD5CryptoServiceProvider>(source);
        }

        /// <summary>
        /// 不可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string source)
        {
            return hash_encrypt<System.Security.Cryptography.SHA1CryptoServiceProvider>(source);
        }

        /// <summary>
        /// 不可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string source)
        {
            return hash_encrypt<System.Security.Cryptography.SHA256CryptoServiceProvider>(source);
        }

        /// <summary>
        /// 不可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA384Encrypt(string source)
        {
            return hash_encrypt<System.Security.Cryptography.SHA384CryptoServiceProvider>(source);
        }

        /// <summary>
        /// 不可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA512Encrypt(string source)
        {
            return hash_encrypt<System.Security.Cryptography.SHA512CryptoServiceProvider>(source);
        }
        #endregion

        #region SymmetricAlgorithm
        private static byte[] symmetric_encrypt<T>(string plainText, byte[] key, byte[] iv) where T : System.Security.Cryptography.SymmetricAlgorithm, new()
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (System.Security.Cryptography.SymmetricAlgorithm sa = new T())
            {
                sa.Key = key;
                sa.IV = iv;
                System.Security.Cryptography.ICryptoTransform encryptor = sa.CreateEncryptor(sa.Key, sa.IV);
                using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
                {
                    using (System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        using (System.IO.StreamWriter swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        private static string symmetric_decrypt<T>(byte[] cipherText, byte[] key, byte[] iv) where T : System.Security.Cryptography.SymmetricAlgorithm, new()
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("IV");
            string plaintext = null;
            using (System.Security.Cryptography.SymmetricAlgorithm sa = new T())
            {
                sa.Key = key;
                sa.IV = iv;
                System.Security.Cryptography.ICryptoTransform decryptor = sa.CreateDecryptor(sa.Key, sa.IV);
                using (System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(cipherText))
                {
                    using (System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                    {
                        using (System.IO.StreamReader srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;

        }
        #endregion

        #region Aes 对称
        /// <summary>
        /// 对称
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AesEncrypt(string plainText, byte[] key, byte[] iv)
        {
            return Security.symmetric_encrypt<System.Security.Cryptography.AesCryptoServiceProvider>(plainText, key, iv);
        }

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AesDecrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            return Security.symmetric_decrypt<System.Security.Cryptography.AesCryptoServiceProvider>(cipherText, key, iv);
        }
        #endregion

        #region Des 对称
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] DesEncrypt(string plainText, byte[] key, byte[] iv)
        {
            return symmetric_encrypt<System.Security.Cryptography.TripleDESCryptoServiceProvider>(plainText, key, iv);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesDecrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            return symmetric_decrypt<System.Security.Cryptography.TripleDESCryptoServiceProvider>(cipherText, key, iv);
        }
        #endregion

        #region RSA 非对称
        /// <summary>
        /// RSA 的密钥产生，产生私钥和公钥
        /// </summary>
        /// <param name="xmlPrivateKey">私钥（解密）</param>
        /// <param name="xmlPublicKey">公钥（加密）</param>
        public static void RSAKey(out string xmlPrivateKey, out string xmlPublicKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
            xmlPrivateKey = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }

        /// <summary>
        /// RSA加密算法比较耗时
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="m_strEncryptString"></param>
        /// <returns></returns>
        public static byte[] RSAEncrypt(string xmlPublicKey, string m_strEncryptString)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            return rsa.Encrypt(new UnicodeEncoding().GetBytes(m_strEncryptString), false);
        }

        /// <summary>
        /// RSA加密算法比较耗时
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="DecryptString"></param>
        /// <returns></returns>
        public static string RSADecrypt(string xmlPrivateKey, byte[] DecryptString)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            return new UnicodeEncoding().GetString(rsa.Decrypt(DecryptString, false));
        }
        #endregion
    }
}
