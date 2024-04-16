using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.BerthLock.Dtos
{
    public class BerthLockRequest
    {
        /// <summary>
        /// 设备Id,用于区分设备
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long happenTime { get; set; }
    }
}
