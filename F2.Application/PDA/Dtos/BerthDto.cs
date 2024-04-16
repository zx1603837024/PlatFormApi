using System;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class BerthDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 泊位号
        /// </summary>
        public string BerthNumber { get; set; }

        /// <summary>
        /// 车检器编号
        /// </summary>
        public string SensorNumber { get; set; }

        /// <summary>
        /// 泊位段名称
        /// </summary>
        public string BerthsecName { get; set; }
        /// <summary>
        /// 1再停
        /// 2未停
        /// </summary>
        public string BerthStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid guid { get; set; }
        /// <summary>
        /// 车检器guid
        /// </summary>
        public Guid? SensorGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// 分公司id
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 泊位段ID
        /// </summary>
        public int BerthsecId { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string RelateNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 进场时间
        /// </summary>

        public DateTime? InCarTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InCarTimeStr
        {
            get
            {
                if (InCarTime != null)
                {
                    return InCarTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return null;
            }
        }

        /// <summary> 
        /// 出场时间
        /// </summary>
        public DateTime? OutCarTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OutCarTimeStr
        {
            get
            {
                if (OutCarTime != null)
                {
                    return OutCarTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return null;
            }
        }


        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Prepaid { get; set; }

        /// <summary>
        /// 车类型
        /// 1.大型车
        /// 2.小型车
        /// 3.摩托车
        /// </summary>
        public virtual Int16 CarType { get; set; }

        /// <summary>
        /// 停车类型
        /// </summary>
        public Int16 StopType { get; set; }

        /// <summary>
        /// 车检器入场时间
        /// </summary>
        public virtual DateTime? SensorsInCarTime { get; set; }

        /// <summary>
        /// 车检器出场时间
        /// </summary>
        public virtual DateTime? SensorsOutCarTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int RateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int RateId1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int RateId2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string JsonRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string JsonRate1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string JsonRate2 { get; set; }
    }
}
