using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    public class CommonOutput
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public EPushParingRecordOutputStatus status { get; set; }

        /// <summary>
        /// 响应描述
        /// </summary>
        public object message { get; set; }
    }
}
