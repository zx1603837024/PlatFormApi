using System.Collections.Generic;
using F2.Application.Inspectors.Dtos;
using F2.Core.Extensions;
using System.Data;
using System.Data.SqlClient;

namespace F2.Application.Inspectors
{
    /// <summary>
    /// 
    /// </summary>
    public class InspectorAppService : IInspectorAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="berthId"></param>
        /// <returns></returns>
        public List<BerthDto> GetBerthList(int berthsecId)
        {
            string sql = "select AbpBerths.Id, AbpBerths.CarType, Prepaid, BerthNumber, AbpBerths.RelateNumber, SensorNumber, BerthsecName, BerthStatus, guid, SensorGuid, ParkName, InCarTime, OutCarTime, case when AbpMonthlyCars.PlateNumber is null then 1 else 2 end as StopType from AbpBerths with(nolock) left join AbpBerthsecs on AbpBerths.BerthsecId = AbpBerthsecs.Id left join AbpParks on AbpParks.Id = AbpBerths.ParkId left join AbpMonthlyCars on AbpMonthlyCars.CompanyId = AbpBerths.CompanyId and AbpMonthlyCars.PlateNumber = AbpBerths.RelateNumber and AbpMonthlyCars.IsDeleted = 0 and EndTime > GETDATE() and BeginTime < GETDATE() and (',' + AbpMonthlyCars.ParkIds + ',' like ','+ CONVERT(varchar(10), AbpBerths.ParkId) + ',' or AbpMonthlyCars.ParkIds = '0')  where AbpBerths.BerthsecId = @BerthsecId  and AbpBerths.IsActive = 1 order by BerthNumber";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BerthsecId", berthsecId)
            };
            return DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, param));
        }
    }
}
