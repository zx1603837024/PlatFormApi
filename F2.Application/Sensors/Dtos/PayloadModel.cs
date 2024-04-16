using System.Collections.Generic;

namespace F2.Application.Sensors.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class PayloadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string display_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BodyModel body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> extra { get; set; }
    }
}