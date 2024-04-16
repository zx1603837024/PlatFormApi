namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 黑名单
    /// </summary>
    public class BlackListDto
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string VehicleType { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string RelateNumber { get; set; }


        /// <summary>
        /// 告知手机号码
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 车主名称
        /// </summary>
        public string CarOwner { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 分公司id
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }
    }
}
