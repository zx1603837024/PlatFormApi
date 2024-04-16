using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA.Dtos
{
   public  class BusinessDetailDto
    {
        /// <summary>
        /// 
        /// </summary>
       public long Id { get; set; } 
       /// <summary>
     /// 进场批次号
     /// </summary>
        public string InBatchNo { get; set; }
        /// <summary>
        /// 出厂批次号
        /// </summary>
        public string OutBatchNo { get; set; }
        /// <summary>
        /// 追缴批次号
        /// </summary>
        public string PaymentBatchNo { get; set; }

        /// <summary>
        /// 泊位号
        /// </summary>
        public virtual string BerthNumber { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public virtual string PlateNumber { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public virtual Int16 CarType { get; set; }

        /// <summary>
        /// 预付费
        /// </summary>
        public virtual decimal Prepaid { get; set; }
        /// <summary>
        /// 预付支付类型
        /// 1.现金
        /// 2.刷卡
        /// 3.在线支付
        /// 4.MOne卡
        /// </summary>
        public virtual Int16 PrepaidPayStatus { get; set; }
        /// <summary>
        /// 进场预付费卡号   如果不是刷卡 预付费的卡号是0
        /// </summary>
        public virtual string PrepaidCarNo { get; set; }



        /// <summary>
        /// 出场应收=实际应收-预付
        /// </summary>
        public virtual decimal Receivable { get; set; }

        /// <summary>
        /// 总应收
        /// </summary>
        public virtual decimal? Money { get; set; }

        /// <summary>
        /// 总实收（包含预付）
        /// </summary>
        public virtual decimal FactReceive { get; set; }

        /// <summary>
        /// 欠费金额
        /// </summary>
        public virtual decimal Arrearage { get; set; }

        /// <summary>
        /// 打折前应收 
        /// </summary>
        public virtual decimal? BeforeDiscount { get; set; }
        /// <summary>
        /// 折扣金额= 打折前应收-总应收
        /// </summary>
        public virtual decimal? DiscountMoney { get; set; }
        /// <summary>
        /// 折扣率 9.5 九五折 8 8折
        /// </summary>
        public virtual double? DiscountRate { get; set; }


        /// <summary>
        /// 车辆入场时间
        /// </summary>
        public virtual DateTime? CarInTime { get; set; }

        /// <summary>
        /// 车辆出场时间
        /// </summary>
        public virtual DateTime? CarOutTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public virtual DateTime? CarPayTime { get; set; }

        /// <summary>
        /// 入场收费员
        /// </summary>
        public virtual long InOperaId { get; set; }

       

        /// <summary>
        /// 设备编号
        /// </summary>
        public virtual string InDeviceCode { get; set; }

        /// <summary>
        /// 出场收费员Id
        /// </summary>
        public virtual long? OutOperaId { get; set; }

        

        /// <summary>
        /// 设备编号
        /// </summary>
        public virtual string OutDeviceCode { get; set; }

        /// <summary>
        /// 收费操作员Id
        /// </summary>
        public virtual long? ChargeOperaId { get; set; }

        /// <summary>
        /// 收费设备编号
        /// 线上支付用什么好
        /// </summary>
        public virtual string ChargeDeviceCode { get; set; }




        /// <summary>
        /// 记录唯一key
        /// </summary>
        public virtual Guid guid { get; set; }

        #region 车检器栏位
        /// <summary>
        /// 车检器入场时间
        /// </summary>
        public virtual DateTime? SensorsInCarTime { get; set; }

        /// <summary>
        /// 车检器出场时间
        /// </summary>
        public virtual DateTime? SensorsOutCarTime { get; set; }

        /// <summary>
        /// 车检器停车时长
        /// </summary>
        public virtual int? SensorsStopTime { get; set; }

        /// <summary>
        /// 车检器应收
        /// </summary>
        public decimal? SensorsReceivable { get; set; }
        #endregion

        #region escape field

        /// <summary>
        /// 补缴金额（目前只是支持补缴一次，既补缴金额等于欠费金额）
        /// </summary>
        public decimal? Repayment { get; set; }

        /// <summary>
        /// 补缴时间
        /// </summary>
        public virtual DateTime? CarRepaymentTime { get; set; }

        /// <summary>
        /// 追缴类型
        /// 1.前端追缴
        /// 2.后台追缴
        /// 3.微信追缴
        /// </summary>

        public virtual Int16 PaymentType { get; set; }

        /// <summary>
        /// 逃逸时间
        /// </summary>
        public virtual DateTime? EscapeTime { get; set; }

        /// <summary>
        /// 支付类型（逃逸）  
        /// case 1:"现金";
        /// case 2:"刷卡支付";
        /// case 3:"微信支付";
        /// case 4:"账号支付";
        /// case 5:"未付";
        /// case 6:"支付宝支付";
        /// case 7:"其他";
        /// default:"未知";
        /// </summary>
        public virtual Int16 EscapePayStatus { get; set; }


        /// <summary>
        /// 是否支付（逃逸）
        /// </summary>
        public virtual bool IsEscapePay { get; set; }

        /// <summary>
        /// 逃逸追缴收费员
        /// </summary>
        public virtual long? EscapeOperaId { get; set; }


        /// <summary>
        /// 后台追缴，用户保存用户Id
        /// 当PaymentType为2时
        /// </summary>
        public virtual long? EscapeUserId { get; set; }


        /// <summary>
        /// 逃逸追缴设备
        /// </summary>
        public virtual string EscapeDeviceCode { get; set; }

        /// <summary>
        /// 追缴商户Id
        /// </summary>
        public virtual int? EscapeTenantId { get; set; }

        /// <summary>
        /// 追缴分公司
        /// </summary>
        public virtual int? EscapeCompanyId { get; set; }
        #endregion

        /// <summary>
        /// 出场支付类型（出场应收）
        /// 1.现金
        /// 2.刷卡
        /// 3.在线支付
        /// 4.MOne卡
        /// </summary>
        public virtual Int16 PayStatus { get; set; }
        /// <summary>
        /// 出场应收的卡号，如果出场是现金的话 出场应收卡号为0
        /// </summary>
        public virtual string ReceivableCarNo { get; set; }


        /// <summary>
        /// 支付账号关联id
        /// 现在支付无关联账号
        /// </summary>
        public virtual int? OtherAccountId { get; set; }

        /// <summary>
        /// 是否支付
        /// true:已支付
        /// false:未支付
        /// </summary>
        public virtual bool IsPay { get; set; }

        /// <summary>
        /// 停车类型
        /// 1.普通停车
        /// 2.包月停车
        /// 3.VIP停车
        /// 4.特殊停车
        /// 5.刷卡临时停车
        /// 6.新能源停车
        /// 7.白名单
        /// </summary>
        public virtual Int16 StopType { get; set; }

        /// <summary>
        /// 费用类型
        /// 1.为正常收费
        /// 2.为追缴收费
        /// </summary>
        public virtual Int16 FeeType { get; set; }

        /// <summary>
        /// 停车时长
        /// </summary>
        public virtual Int32? StopTime { get; set; }

        #region  基础数据
        /// <summary>
        /// 商户Id
        /// </summary>
        public virtual int TenantId { get; set; }


        public virtual int CompanyId { get; set; }

        public virtual int RegionId { get; set; }


        public virtual int ParkId { get; set; }

        public virtual int BerthsecId { get; set; }

        /// <summary>
        /// 记录状态
        /// 1.不完整数据
        /// 2.正常收费已缴费
        /// 3.逃逸未收费
        /// 4.逃逸已收费
        /// 5.余额不足（欠费）
        /// </summary>
        public virtual Int16 Status { get; set; }
        #endregion

        /// <summary>
        /// app账号关联id
        /// 现场支付无关联账号
        /// </summary>
        public virtual long? AppAccountId { get; set; }

        /// <summary>
        /// 是否锁定
        /// true为锁定，false为解锁
        /// </summary>
        public virtual bool IsLock { get; set; }

        /// <summary>
        /// 记录锁定人
        /// </summary>
        public virtual long? EmployeeId { get; set; }

        /// <summary>
        /// 出场支付账号关联id
        /// 现场支付无关联账号
        /// </summary>
        public virtual long? ReceivableOtherAccountId { get; set; }

        /// <summary>
        /// 追缴的卡号，如果追缴是现金的话 追缴卡号为0
        /// </summary>
        public virtual string EscapeCardNo { get; set; }

        /// <summary>
        /// 追缴支付账号关联id
        /// 现在支付无关联账号
        /// </summary>
        public virtual long? EscapeOtherAccountId { get; set; }
        /// <summary>
        /// 电子订单ID
        /// </summary>
        public virtual long? ElectronicOrderid { get; set; }
    }
}
