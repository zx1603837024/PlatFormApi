using F2.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking
{
    internal  class SQLFactory
    {
        public static T ExecuteScalar<T>(string sql,SqlParameter[] commandParameters=null)
        {
           var obj= SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, sql, commandParameters);
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static void ExecuteNonQuery(string sql, SqlParameter[] commandParameters = null)
        {
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql, commandParameters);
        }

        public static List<T> GetEntityFromSQL<T>(string sql, SqlParameter[] commandParameters = null)
        {
            var dataTable = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, commandParameters);
            return DataProcessHelper.GetEntityFromTable<T>(dataTable);
        }

    }
}
