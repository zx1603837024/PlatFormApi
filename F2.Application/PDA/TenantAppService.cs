using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions;

namespace F2.Application.PDA
{
    /// <summary>
    /// 
    /// </summary>
    public class TenantAppService : ITenantAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public TenantDto GetTenantInfo(string tenantName)
        {
            string sql = "select Id, IsActive, Name from AbpTenants where DomainName = @DomainName and IsDeleted = 0";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@DomainName", tenantName)
            };
            var list = DataProcessHelper.GetEntityFromTable<TenantDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, param));
            if (list.Count > 0)
                return list[0];
            return null;
        }
    }
}
