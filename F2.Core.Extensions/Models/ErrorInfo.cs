using System;

namespace F2.Core.Extensions.Models
{
    /// <summary>
    /// 错误信息承载类
    /// </summary>
    [Serializable]
    public class ErrorInfo
    {
        /// <summary>
        /// 错误代号
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 错误详细
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// 是否存在校验错误，如果存在将存储校验信息
        /// </summary>
        //public ValidationErrorInfo[] ValidationErrors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ErrorInfo()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误消息</param>
        public ErrorInfo(string message)
        {
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">错误代码</param>
        public ErrorInfo(int code)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="message">Error message</param>
        public ErrorInfo(int code, string message)
            : this(message)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="details">错误详细</param>
        public ErrorInfo(string message, string details)
            : this(message)
        {
            Details = details;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="details"></param>
        public ErrorInfo(int code, string message, string details)
            : this(message, details)
        {
            Code = code;
        }
    }
}