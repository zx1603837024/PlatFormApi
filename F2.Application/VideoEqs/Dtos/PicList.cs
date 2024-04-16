using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.VideoEqs.Dtos
{

    public class PicList
    {
        public string data { get; set; }
        public int size     { get; set; }
        /// <summary>
        /// 照片类型涉及：1-车辆大图；2-车牌小图
        /// </summary>
        public int type { set; get; }
    }
}
