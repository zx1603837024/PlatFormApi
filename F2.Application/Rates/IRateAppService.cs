using F2.Application.Rates.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRateAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="berthsecID"></param>
        /// <param name="inCarTime"></param>
        /// <param name="outCarTime"></param>
        /// <param name="carType"></param>
        /// <param name="RateId"></param>
        /// <param name="parkId"></param>
        /// <param name="plateNumber"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        RateCalculateModel RateCalculate(int berthsecID, DateTime inCarTime, DateTime outCarTime, int carType, int RateId, int parkId, string plateNumber, int companyId);
    }
}
