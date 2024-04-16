using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Sensors.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class INmotionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string cmd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public INmotionBody body { get; set; }
    }
}
