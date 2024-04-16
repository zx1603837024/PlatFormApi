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
    /// 获取临停缴费接口
    /// </summary>
    public class CalculatingTempCostNoIdsRequest
    {
        /// <summary>
        /// 停车场ID
        /// </summary>
        public string parking_id { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string plate_num { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string appKey=>ConfigurationManager.AppSettings["ZhiBoAppKey"].MD5Encrypt().ToUpper();

        /// <summary>
        /// 签名
        /// </summary>
        public string sign
        {
            get
            {
                var stringsigntemp = $"parking_id={parking_id}&plate_num={plate_num}&appKey={appKey}";
                return stringsigntemp.MD5Encrypt().ToUpper();
            }
        }
    }
}
