namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public  class BerthsecCheckDto
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
        public bool Checked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UseStatus { get; set; }

        /// <summary>
        /// 是否需获取车检器状态
        /// true获取，false不获取
        /// </summary>
        public virtual bool PushStatus { get; set; }
    }
}
