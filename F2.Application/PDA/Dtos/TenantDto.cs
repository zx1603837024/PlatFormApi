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
    public class TenantDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TenancyName
        { get; set; }

        public string Name
        { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive
        { get; set; }

        public DateTime LastModificationTime
        { get; set; }

        public long? LastModifierUserId
        { get; set; }

        public DateTime CreationTime
        { get; set; }

        public long? CreatorUserId
        { get; set; }

        public string DomainName
        { get; set; }

        public string HQ
        { get; set; }

        public string Password
        { get; set; }

        public string Address
        { get; set; }
        public string X
        { get; set; }
        public string Y
        { get; set; }

        public string Telephone

        { get; set; }

        public string Contacts
        { get; set; }


        /// <summary>
        /// 版本名称
        /// </summary>
        public string EditionName
        { get; set; }
    }
}
