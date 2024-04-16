using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA.Dtos
{
   public  class WhiteListsDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CompanyName { get; set; }

        public string VehicleType { get; set; }


        public string RelateNumber { get; set; }


        public string Remarks { get; set; }


        public decimal Rebate { get; set; }


        public bool IsActive { get; set; }
    }
}
