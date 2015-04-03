using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core
{
    /// <summary>
    /// MongoDB 生成 唯一 的 ID
    /// </summary>
    internal class MongoID
    {

        private readonly object _inclock = new object();
        private int _inc;
        private byte[] _machineHash;
        private byte[] _procId;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MongoID" /> class.
        /// </summary>
        public MongoID()
        {
            GenerateConstants();
        }

        /// <summary>
        ///   Generates this instance.
        /// </summary>
        /// <returns></returns>
        private byte[] generate()
        {
            //FIXME Endian issues with this code.  
            //.Net runs in native endian mode which is usually little endian.  
            //Big endian machines don't need the reversing (Linux+PPC, XNA on XBox)
            var oid = new byte[12];
            var copyidx = 0;

            var time = BitConverter.GetBytes(GenerateTime());
            Array.Reverse(time);
            Array.Copy(time, 0, oid, copyidx, 4);
            copyidx += 4;

            Array.Copy(_machineHash, 0, oid, copyidx, 3);
            copyidx += 3;

            Array.Copy(_procId, 2, oid, copyidx, 2);
            copyidx += 2;

            var inc = BitConverter.GetBytes(GenerateInc());
            Array.Reverse(inc);
            Array.Copy(inc, 1, oid, copyidx, 3);

            return oid;
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            var _bytes = new byte[12];
            Array.Copy(generate(), _bytes, 12);
            return BitConverter.ToString(_bytes).Replace("-", "").ToUpper();
        }

        /// <summary>
        ///   Generates the time.
        /// </summary>
        /// <returns></returns>
        private uint GenerateTime()
        {
            var now = DateTime.UtcNow;
            //DateTime nowtime = new DateTime(epoch.Year, epoch.Month, epoch.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
            var diff = now - Assist.Epoch;
            return (uint)Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        ///   Generates the inc.
        /// </summary>
        /// <returns></returns>
        private int GenerateInc()
        {
            lock (_inclock)
            {
                return ++_inc;
            }
        }

        /// <summary>
        ///   Generates the constants.
        /// </summary>
        private void GenerateConstants()
        {
            _machineHash = GenerateHostHash();
            _procId = BitConverter.GetBytes(GenerateProcId());
            Array.Reverse(_procId);
        }

        /// <summary>
        ///   Generates the host hash.
        /// </summary>
        /// <returns></returns>
        private byte[] GenerateHostHash()
        {
            var md5 = MD5.Create();
            var host = Dns.GetHostName();
            return md5.ComputeHash(Encoding.Default.GetBytes(host));
        }

        /// <summary>
        ///   Generates the proc id.
        /// </summary>
        /// <returns></returns>
        private int GenerateProcId()
        {
            var proc = Process.GetCurrentProcess();
            return proc.Id;
        }
    }
}
