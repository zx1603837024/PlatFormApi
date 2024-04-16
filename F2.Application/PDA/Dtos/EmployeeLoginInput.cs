using System;
using System.Collections.Generic;
using System.Text;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public  class EmployeeLoginInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string userNameOrEmailAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string plainPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string tenancyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceType { get; set; }
    }
}
