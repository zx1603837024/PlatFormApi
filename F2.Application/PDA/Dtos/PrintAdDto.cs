using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintAdDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 广告名称
        /// </summary>
        public virtual string AdName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual string Context { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public virtual string QrCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime BeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string BeginTimeStr
        {
            get { return BeginTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string EndTimeStr
        {
            get { return EndTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}
