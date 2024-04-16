using System;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class FreeOrderDetailDto
    {

        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Id
        /// </summary>


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
        /// 泊位Id
        /// </summary>
        public long berthID { get; set; }

     

        /// <summary>
        /// 泊位编号
        /// </summary>
        public string berthCode { get; set; }
       
        
        

        /// <summary>
        ///  批次号
        /// </summary>
        public string batchNo { get; set; }
      
        
        /// <summary>
        /// 流水号
        /// </summary>
        public string seriaNo { get; set; }
        

        /// <summary>
        /// 车牌号
        /// </summary>
        public string carNumber { get; set; }
       
        
        /// <summary>
        /// 车辆类型
        /// </summary>
        public int carType { get; set; }

        /// <summary>
        /// 停车类型
        /// </summary>
        public int stopType { get; set; }
       
        
        /// <summary>
        /// 
        /// </summary>
        public int regionID { get; set; }
        /// <summary>
        /// 
        /// </summary>



        /// <summary>
        /// 泊位段Id
        /// </summary>
        public int berthsecID { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 进场时间
        /// </summary>
        public DateTime carInTime { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        ///  预付金额
        /// </summary>
        public decimal prepay { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 预付类型
        /// </summary>
        public string prePayType { get; set; }
        /// <summary>
        /// 
        /// </summary>


        /// <summary>
        /// 预付卡号
        /// </summary>
        public string preCardNo { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 预付卡类型
        /// </summary>
        public string preCardType { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 泊位状态
        /// </summary>
        public string berthStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 泊位guid
        /// </summary>
        public Guid guid { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 出场时间
        /// </summary>
        public DateTime carOutTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 


        /// <summary>
        /// 金额
        /// </summary>
        public decimal money { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        public Guid sensorGuid { get; set; }
        /// <summary>
        /// 车检器guid
        /// </summary>

        /// <summary>
        /// 订单状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        public decimal factReceive { get; set; }
        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime carPayTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        
        
        /// <summary>
        /// 欠费金额
        /// </summary>
        public decimal arrearage { get; set; }
        /// <summary>
        /// 
        /// </summary> 


        /// <summary>
        /// 出场支付类型
        /// </summary>
        public int payStatus { get; set; }
        /// <summary>
        /// 
        /// </summary> 

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime creationTime { get; set; }
        /// <summary>
        /// 
        /// </summary> 


        /// <summary>
        /// 是否支付
        /// </summary>
        public bool isPay { get; set; }
        /// <summary>
        /// 
        /// </summary> 
        /// 


        /// <summary>
        /// 费用类型
        /// </summary>
        public int feeType { get; set; }
        /// <summary>
        /// 
        /// </summary> 

























    }
}
