using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Generic;
using Utility.Core;
using Utility.Data;

namespace UnitTest
{
    class Factory
    {
        public const string LETTER_TABLE = "Letter";

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection().SetConnectionString("UnitTest");
        }

        public static SqlParameter CreateParameter(string parameterName, object value, string sourceColumn, DbType dbtype = DbType.String, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(parameterName, value)
            {
                SourceColumn = sourceColumn,
                DbType = dbtype,
                Direction = direction
            };
        }

        public static List<SqlParameter> CreateParameters()
        {
            return new List<SqlParameter>();
        }

    }
}
