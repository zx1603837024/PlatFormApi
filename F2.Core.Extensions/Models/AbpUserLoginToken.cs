namespace F2.Core.Extensions.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpUserLoginToken
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 商户编号
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public string RegionIds { get; set; }

        /// <summary>
        /// 停车场id
        /// </summary>
        public string ParkIds { get; set; }

        /// <summary>
        /// 泊位段id
        /// </summary>
        public string BerthsecIds { get; set; }

        /// <summary>
        /// 收费员编号
        /// </summary>
        public long EmployeeId { get; set; }

        /// <summary>
        /// 巡查员编号
        /// </summary>
        public long InspectorsId { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
    }
}
