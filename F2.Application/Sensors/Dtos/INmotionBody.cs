using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Sensors.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class INmotionBody
    {
        /// <summary>
        /// 
        /// </summary>
        public string parkID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string deviceID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string battary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string parkingStatu { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string rssi { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string passTime { get; set; }

        /// <summary>
        /// 标记位
        /// </summary>
        public bool flag { get; set; }

        private string _sequence;
        /// <summary>
        /// 
        /// </summary>
        public string sequence
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sequence))
                {
                    flag = false;
                    return deviceID;
                }
                else
                {
                    flag = true;
                    return _sequence;
                }
            }
            set { _sequence = value; }
        }
    }
}
