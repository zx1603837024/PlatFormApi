using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdKafka;
using System.Configuration;

namespace F2.KafkaNet
{
    /// <summary>
    /// 
    /// </summary>
    public class KafkaNetAppService : IKafkaNetAppService
    {
        private readonly static string broker = ConfigurationManager.AppSettings["broker"].ToString();
        private readonly static string topic = ConfigurationManager.AppSettings["topic"].ToString();

        /// <summary>
        /// 消费者
        /// </summary>
        public void Consumer()
        {
            var config = new Config() { GroupId = "simple-csharp-consumer" };
            using (var consumer = new EventConsumer(config, broker))
            {
                consumer.OnMessage += (obj, msg) =>
                {
                    string text = Encoding.UTF8.GetString(msg.Payload, 0, msg.Payload.Length);
                    //Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {text}");
                };

                consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topic, 0, 5) });
                consumer.Start();
            }
        }

        /// <summary>
        /// kafka生产者
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="topic"></param>
        public void Produce(string msg)
        {
            string brokerList = "26.2.4.171:9092,26.2.4.172:9092,26.2.4.173:9092";
            string topicName = "test_topic";
            var topicConfig = new TopicConfig
            {

            };
            using (Producer producer = new Producer(brokerList))
            using (Topic temp = producer.Topic(topicName, topicConfig))
            {
                byte[] data = Encoding.UTF8.GetBytes(msg); Task<DeliveryReport> deliveryReport = temp.Produce(data);
                var unused = deliveryReport.ContinueWith(task =>
                {

                });
            }
        }
    }
}
