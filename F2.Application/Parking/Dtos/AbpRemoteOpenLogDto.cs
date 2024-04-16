using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 远程开门
    /// </summary>
    public class AbpRemoteOpenLogDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 封闭停车场ID（智泊云停平台ID）
        /// </summary>
        public string ParkingId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 车辆是否进入
        /// </summary>
        public bool IsCarIn { get; set; }

        /// <summary>
        /// 是否已出场
        /// </summary>
        public bool IsCarOut { get; set; }

        /// <summary>
        /// 停车场平台订单ID
        /// </summary>
        public string RecordId { get; set; }
    
        /// <summary>
        /// 账单ID
        /// </summary>
        public string RecordBillNo { get; set; }
    }
}
