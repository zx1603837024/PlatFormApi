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
    public class BerthsecDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BerthsecName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 泊位总数
        /// </summary>
        public string BerthCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CheckOutName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CheckInName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BeginNumeber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EndNumeber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomNumeber { get; set; }


        /// <summary>
        /// 早班费率
        /// </summary>
        public int RateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RateName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FeeModel { get; set; }

        /// <summary>
        /// 早班费率
        /// </summary>
        public int? RateId1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RateName1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FeeModel1 { get; set; }


        /// <summary>
        /// 早班费率
        /// </summary>
        public int? RateId2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RateName2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FeeModel2 { get; set; }
    }
}
