namespace F2.Application.Sensors.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class UmengModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string appkey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string device_tokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string alias_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object filter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string file_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PayloadModel payload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string production_mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string mipush { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string mi_activity { get; set; }
    }
}