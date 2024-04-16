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
    public class RemoteGuidDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 主键guid
        /// </summary>
        public Guid BusinessDetailGuid { get; set; }

        /// <summary>
        /// 同步状态
        /// true已同步
        /// false为同步
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 泊位段id
        /// </summary>
        public int BerthsecId { get; set; }
    }
}
