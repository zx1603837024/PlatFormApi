using F2.Core.Extensions.Models;
using System;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 追缴
    /// </summary>
    public class FeeBackDto : EntityDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ids { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Repayment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CarRepaymentTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEscapePay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public short EscapePayStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public short PaymentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PaymentBatchNo { get; set; }
    }
}
