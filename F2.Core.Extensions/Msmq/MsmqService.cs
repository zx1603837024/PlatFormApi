/********************************************************************************
** auth： 黎通
** date： 2015/12/21 17:38:29
** desc： 消息队列
** Ver.:  V1.0.0
*********************************************************************************/
using System;
using System.Configuration;
using System.Messaging;

namespace F2.Core.Extensions.Msmq
{
    /// <summary>
    /// 消息队列
    /// </summary>
    public static class MsmqService
    {
        #region Var
        private static MessageQueue _operaMessageQueue = null;
        private static string P4OLMsmq = ConfigurationManager.AppSettings["P4OLMsmq"];

        private static MessageQueue _dataMessageQueue = null;
        private static string P4DLMsmq = ConfigurationManager.AppSettings["P4DLMsmq"];

        private static MessageQueue _auditMessageQueue = null;
        private static string P4AuditMsmq = ConfigurationManager.AppSettings["P4AuditMsmq"];

        /// <summary>
        /// 车检器日志
        /// </summary>
        private static MessageQueue _sensorMessageQueue = null;
        private static string P4SensorMsmq = ConfigurationManager.AppSettings["P4SensorMsmq"];
        #endregion
        /// <summary>
        /// 消息队列对象
        /// </summary>
        public static MessageQueue OperaMessQueue
        {
            get
            {
                if (_operaMessageQueue == null)
                {
                    if (MessageQueue.Exists(P4OLMsmq))
                    {
                        _operaMessageQueue = new MessageQueue(P4OLMsmq);
                    }
                    else
                    {
                        _operaMessageQueue = MessageQueue.Create(P4OLMsmq);
                        _operaMessageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                    }
                }
                return _operaMessageQueue;
            }
        }
        /// <summary>
        /// 数据操作日志消息队列对象
        /// </summary>
        public static MessageQueue DataMessQueue
        {
            get
            {
                if (_dataMessageQueue == null)
                {
                    if (MessageQueue.Exists(P4DLMsmq))
                    {
                        _dataMessageQueue = new MessageQueue(P4DLMsmq);
                    }
                    else
                    {
                        _dataMessageQueue = MessageQueue.Create(P4DLMsmq);
                        _dataMessageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                    }
                }
                return _dataMessageQueue;
            }
        }

        /// <summary>
        /// 车检器消息队列
        /// </summary>
        public static MessageQueue SensorMessQueue
        {
            get
            {
                if (_sensorMessageQueue == null)
                {
                    if (MessageQueue.Exists(P4SensorMsmq))
                    {
                        _sensorMessageQueue = new MessageQueue(P4SensorMsmq);
                    }
                    else
                    {
                        _sensorMessageQueue = MessageQueue.Create(P4SensorMsmq);
                        _sensorMessageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                    }
                }
                return _sensorMessageQueue;
            }
        }

        /// <summary>
        /// 审计日志消息队列
        /// </summary>
        public static MessageQueue AuditMessQueue
        {
            get
            {
                if (_auditMessageQueue == null)
                {
                    if (MessageQueue.Exists(P4AuditMsmq))
                    {
                        _auditMessageQueue = new MessageQueue(P4AuditMsmq);
                    }
                    else
                    {
                        _auditMessageQueue = MessageQueue.Create(P4AuditMsmq);
                        _auditMessageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                    }
                }
                return _auditMessageQueue;
            }
        }
        #region Public Method

        /// <summary>
        /// 一个将对象发送到队列的方法,这里发送的是对象
        /// </summary>
        public static void SendOperaMsmq(object arr)
        {
            System.Messaging.Message message = new System.Messaging.Message();
            message.Label = "操作日志";
            message.Body = arr;
            OperaMessQueue.Send(message);
        }

        /// <summary>
        /// 数据操作日志---一个将对象发送到队列的方法,这里发送的是对象
        /// </summary>
        public static void SendDataMsmq(object arr)
        {
            System.Messaging.Message message = new System.Messaging.Message();
            message.Label = "数据日志";
            message.Body = arr;
            DataMessQueue.Send(message);
        }

        /// <summary>
        ///审计日志
        /// </summary>
        /// <param name="entity"></param>
        public static void SendAuditMsmq(object entity)
        { 
            System.Messaging.Message message = new System.Messaging.Message();
            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(Auditing.AuditLog) });
            message.Label = "审计日志";
            message.Body = entity;
            AuditMessQueue.Send(message);
        }

        /// <summary>
        ///车检器日志
        /// </summary>
        /// <param name="entity"></param>
        public static void SendSensorMsmq(object entity)
        {
            System.Messaging.Message message = new System.Messaging.Message();
            message.Label = "车检器日志";
            message.Body = entity;
            SensorMessQueue.Send(message);
        }

        #endregion
    }
}