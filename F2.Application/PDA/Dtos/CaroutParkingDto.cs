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
    public class CaroutParkingDto : EntityDto
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal Receivable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal FactReceive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CarOutTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CarPayTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string guid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string SensorsOutCarTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SensorsStopTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SensorsReceivable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PayStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IsPay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StopTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Arrearage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BerthsecID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OutBatchNo { get; set; }

        /// <summary>
        /// 泊位段名称
        /// </summary>
        public string ParkName { get; set; }
    }
}
