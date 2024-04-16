using System;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class StopOrderDto
    {

        /// <summary>
        /// 
        /// </summary>
        public int parkID { get; set; }
        /// <summary>
        /// 停车场Id
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        public string parkCode { get; set; }
        /// <summary>
        /// 停车场编号
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        public long berthID { get; set; }
        /// <summary>
        /// 泊位ID
        /// </summary>
         
        
        /// <summary>
        /// 泊位号
        /// </summary>
        public string berthCode { get; set; }

        /// <summary>
        /// 泊位编号
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string batchNo { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string seriaNo { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>

        public string carNumber { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public int carType { get; set; }
        /// <summary>
        /// 车辆类型
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public int stopType { get; set; }
        /// <summary>
        /// 停车类型
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        public int regionID { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public int berthsecID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        


        /// <summary>
        /// 
        /// </summary>
        public DateTime carInTime { get; set; }
        /// <summary>
        /// 进场时间
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public long inOperaId { get; set; }
        /// <summary>
        /// 进场收费员Id
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string inOperaCode { get; set; }
        /// <summary>
        /// 进场收费员编号
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public decimal prepay { get; set; }
        /// <summary>
        /// 预付金额
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string prePayType { get; set; }
        /// <summary>
        /// 预付类型
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string preCardNo { get; set; }
        /// <summary>
        /// 预付卡号
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string preCardType { get; set; }
        /// <summary>
        /// 预付卡类型
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string berthStatus { get; set; }
        /// <summary>
        /// 泊位停车状态
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public int sensorStatus { get; set; }
        /// <summary>
        /// 车检器停车状态
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public Guid berthGuid { get; set; }
        /// <summary>
        /// 泊位guid
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public Guid sensorGuid { get; set; }
        /// <summary>
        /// 车检器guid
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public DateTime sensorBeatTime { get; set; }
        /// <summary>
        /// 车检器心跳时间
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        public DateTime sensorCarInTime { get; set; }
        /// <summary>
        /// 车检器进场时间
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public string hasOwing { get; set; }
        /// <summary>
        /// 数据来源自视频设备
        /// </summary>
        public bool? IsSourceVideo { get; set; }



        /// <summary>
        /// 泊位是否异常
        /// </summary>
        public int? IsFaultFlag { get; set; }





















    }
}
