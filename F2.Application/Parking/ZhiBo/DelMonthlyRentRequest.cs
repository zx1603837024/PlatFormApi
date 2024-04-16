using F2.Core.Extensions.DataExtend;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    /// <summary>
    /// 删除月租
    /// </summary>
    public class DelMonthlyRentRequest
    {
        /// <summary>
        /// 停车场ID
        /// </summary>
        public string park_id { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string plate_number { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string appKey => ConfigurationManager.AppSettings["ZhiBoAppKey"].MD5Encrypt().ToUpper();

        /// <summary>
        /// 签名
        /// </summary>
        public string sign => $"park_id={park_id}&plate_number={plate_number}&appKey={appKey}".MD5Encrypt().ToUpper();
    }
}
