using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA.Dtos
{
    public class PicMongoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string BusinessDetailGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BusinessDetailId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PicType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long CreatorUserId { get; set; }

        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string FileSavePath { get; set; }
    }
}
