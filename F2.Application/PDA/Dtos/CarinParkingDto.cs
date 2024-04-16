using F2.Core.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class CarinParkingDto : EntityDto
    {
        /// <summary>
        /// 泊位号
        /// </summary>
        public string BerthNumber { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNumber { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 预缴金额
        /// </summary>
        public string Prepaid { get; set; }

        /// <summary>
        /// 进场时间
        /// </summary>
        public string CarInTime { get; set; }

        /// <summary>
        /// 唯一值
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// 地磁进场时间
        /// </summary>
        public string SensorsInCarTime { get; set; }

        /// <summary>
        /// 停车类型
        /// </summary>
        public string StopType { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public int RegionId { get; set; }
        
        /// <summary>
        /// 停车场
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 泊位段
        /// </summary>
        public int BerthsecId { get; set; }

        /// <summary>
        /// 进场批次号
        /// </summary>
        public string InBatchNo { get; set; }

        /// <summary>
        /// 泊位段名称
        /// </summary>
        public string ParkName { get; set; }
    }
}
