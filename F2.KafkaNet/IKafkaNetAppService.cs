using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.KafkaNet
{
    /// <summary>
    /// 
    /// </summary>
   public  interface IKafkaNetAppService
    {
        /// <summary>
        /// 创造方法
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="topic"></param>
        void Produce(string msg);

        /// <summary>
        /// 消费方法
        /// </summary>
        void Consumer();
    }
}