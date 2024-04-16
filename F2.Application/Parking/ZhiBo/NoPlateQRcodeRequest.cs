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
    /// 无牌车扫码进出
    /// </summary>
    public class NoPlateQRcodeRequest
    {
        /// <summary>
        /// 车场唯一编号
        /// </summary>
        public string park_id { get; set; }

        /// <summary>
        /// 通道编码
        /// </summary>
        public string lane_no { get; set; }

        /// <summary>
        /// 无牌车传OpenID
        /// 无牌车传微信OpenID,有牌车传车牌号
        /// </summary>
        public string open_id { get; set; } 

        /// <summary>
        /// appKey
        /// </summary>
        public string appKey => ConfigurationManager.AppSettings["ZhiBoAppKey"].MD5Encrypt().ToUpper();

        /// <summary>
        /// 签名
        /// </summary>
        public string sign
        {
            get
            {
                return $"park_id={park_id}&lane_no={lane_no}&appKey={appKey}".MD5Encrypt().ToUpper();
            }
        }
    }
}
