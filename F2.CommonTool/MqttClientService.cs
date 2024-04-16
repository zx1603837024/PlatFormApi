using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Common
{
    public class MqttClientService
    {
        public static IMqttClient _mqttClient;
        /*public void MqttClientStart()
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer("121.37.71.125", 1883) // 要访问的mqtt服务端的 ip 和 端口号
                .WithCredentials("admin", "12345678") // 要访问的mqtt服务端的用户名和密码
                .WithClientId("testclient01") // 设置客户端id
                .WithCleanSession()
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false  // 是否使用 tls加密
                });

            var clientOptions = optionsBuilder.Build();
            _mqttClient = new MqttFactory().CreateMqttClient();

            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync; // 客户端连接成功事件
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync; // 客户端连接关闭事件
            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync; // 收到消息事件

            _mqttClient.ConnectAsync(clientOptions);


        }*/
        public void MqttClientStart()
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 10086) // 要访问的mqtt服务端的 ip 和 端口号
                .WithCredentials("admin", "123456") // 要访问的mqtt服务端的用户名和密码
                .WithClientId("testclient02") // 设置客户端id
                .WithCleanSession()
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false  // 是否使用 tls加密
                });

            var clientOptions = optionsBuilder.Build();
            _mqttClient = new MqttFactory().CreateMqttClient();

            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync; // 客户端连接成功事件
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync; // 客户端连接关闭事件
            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync; // 收到消息事件

            _mqttClient.ConnectAsync(clientOptions);


        }

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine($"客户端已断开与服务端的连接……");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 客户端连接成功事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine($"客户端已连接服务端……");

            _mqttClient.SubscribeAsync("topic2", MqttQualityOfServiceLevel.AtLeastOnce);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 收到消息事件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.WriteLine($"ApplicationMessageReceivedAsync：客户端ID=【{arg.ClientId}】接收到消息。 Topic主题=【{arg.ApplicationMessage.Topic}】 消息=【{Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)}】 qos等级=【{arg.ApplicationMessage.QualityOfServiceLevel}】");
            return Task.CompletedTask;
        }

        public void Publish(string data)
        {
            var message = new MqttApplicationMessage
            {
                Topic = "topic1",
                Payload = Encoding.Default.GetBytes(data),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                Retain = true  // 服务端是否保留消息。true为保留，如果有新的订阅者连接，就会立马收到该消息。
            };
            _mqttClient.PublishAsync(message);
        }
    }
}
