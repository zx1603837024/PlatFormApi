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
    public class EquipmentVersionDto
    {
        /// <summary>
        /// 商户代码
        /// </summary>
        public virtual int TenantId { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public virtual string EqupmentType { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// 是否整个型号自动升级（针对商户）
        /// </summary>
        public virtual bool IsUpgrade { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}
