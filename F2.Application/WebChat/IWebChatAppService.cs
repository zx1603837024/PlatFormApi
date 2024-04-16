using F2.Core.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.WebChat
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWebChatAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <param name="CarInTime"></param>
        void SendCarInMsg(string PlateNumber, string CarInTime, int BerthsecId, string Berthnumber, AbpUserLoginToken loginToken, string parkName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TenantId"></param>
        /// <param name="BerthsecId"></param>
        /// <param name="PlateNumber"></param>
        /// <param name="Berthnumber"></param>
        /// <param name="StopTime"></param>
        /// <param name="Money"></param>
        /// <param name="FactReceive"></param>
        /// <param name="PayStatus"></param>
        /// <param name="CarOutTime"></param>
        void SendCarOutMsg(int TenantId, int BerthsecId, string PlateNumber, string Berthnumber, double StopTime, decimal Money, decimal FactReceive, string PayStatus, string CarOutTime,string ParkName,string CarInTime);
    }
}
