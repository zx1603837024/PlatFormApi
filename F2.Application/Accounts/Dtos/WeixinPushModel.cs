using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Accounts.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class WeixinPushModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// 推送类型
        /// </summary>
        public string PushType { get; set; }

        /// <summary>
        /// 推送内容
        /// </summary>
        public string PushContent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 商户代码
        /// </summary>
        public int TenantId { get; set; }
    }


    public class WeixinCarInModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string carNumber { get; set; }
        public string carInTime { get; set; }
        public string parkName { get; set; }
    }

    public class WeixinCarOutModel
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string carNumber { get; set; }
        public string carInTime { get; set; }
        public string parkName { get; set; }
        public string carOutTime { get; set; }
        
    }
}
