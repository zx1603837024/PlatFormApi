using F2.Core.Extensions.DataExtend;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    public class SaveMonthlyRentRequest
    {
        /// <summary>
        /// 车场唯一编号
        /// </summary>
        public string park_id { get; set; }

        /// <summary>
        /// 车主姓名
        /// </summary>
        public string emp_name { get; set; }

        /// <summary>
        /// 车主手机
        /// </summary>
        public string emp_moblie { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string plate_number { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string family_address { get; set; }

        /// <summary>
        /// 车类ID
        /// </summary>
        public string cartypeid { get; set; }

        /// <summary>
        /// 车型ID
        /// </summary>
        public string carmodelid { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string begindate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string enddate { get; set; }

        /// <summary>
        /// 车位组
        /// </summary>
        public string pklot { get; set; }

        /// <summary>
        /// 车位数
        /// </summary>
        public string pklotnum { get; set; }

        /// <summary>
        /// 车主编号
        /// </summary>
        public string employeeNo { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string deptId { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public string gateid { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string appKey => ConfigurationManager.AppSettings["ZhiBoAppKey"].MD5Encrypt().ToUpper();

        /// <summary>
        /// 签名
        /// </summary>
        public string sign => $"park_id={park_id}&emp_name={emp_name}&plate_number={plate_number}&appKey={appKey}".MD5Encrypt().ToUpper();
    }
}
