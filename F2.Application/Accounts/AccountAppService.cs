using F2.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Accounts
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountAppService : IAccountAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <returns></returns>
        public bool CheckExistsWeixinTuser(string PlateNumber)
        {
            string sql = "select 1 from WeixinTuser where CarNumber1 = '" + PlateNumber + "' or CarNumber2 = '" + PlateNumber + "' or CarNumber3 = '" + PlateNumber + "'";
            object obj = SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            if (obj != null)
                return true;
            return false;
        }
    }
}
