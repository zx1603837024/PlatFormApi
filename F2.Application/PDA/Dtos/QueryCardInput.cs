using F2.Core.Extensions.Models;
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
    public class QueryCardInput : EntityDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string plateNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string phone { get; set; }
    }
}
