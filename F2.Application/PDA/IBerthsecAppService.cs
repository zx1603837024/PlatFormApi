using F2.Application.PDA.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBerthsecAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        GetAllBerthsecListOutput GetBerthsecList(long employeeId, int tenantId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BerthsecId"></param>
        /// <returns></returns>
        BerthsecDto GetBerthsecInfo(int BerthsecId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        List<StopOrderDto> SearchStopOrder(string jsonstr);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        List<FreeOrderDetailDto> SearchFreepOrder(string jsonstr);
        
    }
}
