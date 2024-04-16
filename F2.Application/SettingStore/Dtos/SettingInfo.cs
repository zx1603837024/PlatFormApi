namespace F2.Application.SettingStore.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public  class SettingInfo
    {
        /// <summary>
        /// 商户id，如果商户为空，为管理员设置
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// 用户名id
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 名称唯一key
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设置值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
    }
}
