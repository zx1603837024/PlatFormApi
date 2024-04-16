using F2.Application.SettingStore.Dtos;
using F2.Core.Extensions;
using System.Collections.Generic;

namespace F2.Application
{
    /// <summary>
    /// 获取配置信息
    /// </summary>
    public static class SettingStoreAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name)
        {
            string sqlwhere = " where Name = '" + name + "'";
            if (tenantId.HasValue)
            {
                sqlwhere += " and tenantId = " + tenantId.Value;
            }
            if (userId.HasValue)
            {
                sqlwhere += " and userId = " + userId.Value;
            }
            string sql = "select * from AbpSettings " + sqlwhere;

            var list = DataProcessHelper.GetEntityFromTable<SettingInfo>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, null));
            if (list.Count > 0)
                return list[0];
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static List<SettingInfo> GetSettingList(int tenantId)
        {
            string sql = "select * from AbpSettings where tenantId = " + tenantId;
            return DataProcessHelper.GetEntityFromTable<SettingInfo>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, null));
        }
    }
}
