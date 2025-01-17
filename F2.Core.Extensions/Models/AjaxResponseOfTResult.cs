﻿using System;
using System.Runtime.Serialization;

namespace F2.Core.Extensions.Models
{
    /// <summary>
    /// This class is used to create standard responses for ajax requests.
    /// </summary>
    [Serializable]
    public class AjaxResponse<TResult>
    {
        /// <summary>
        /// Indicates success status of the result.
        /// Set <see cref="Error"/> if this value is false.
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// The actual result object of ajax request.
        /// It is set if <see cref="Success"/> is true.
        /// </summary>
        [DataMember]
        public TResult Result { get; set; }

        /// <summary>
        /// Error details (Must and only set if <see cref="Success"/> is false).
        /// </summary>
        [DataMember]
        public ErrorInfo Error { get; set; }

        /// <summary>
        /// 用户无访问权限
        /// </summary>
        [DataMember]
        public bool UnAuthorizedRequest { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public object rows { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long total { get; set; }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="Result"/> specified.
        /// <see cref="Success"/> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of ajax request</param>
        public AjaxResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object.
        /// <see cref="Success"/> is set as true.
        /// </summary>
        public AjaxResponse()
        {
            Success = true;
        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="Success"/> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public AjaxResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Creates an <see cref="AjaxResponse"/> object with <see cref="Error"/> specified.
        /// <see cref="Success"/> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unAuthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public AjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
        {
            Error = error;
            UnAuthorizedRequest = unAuthorizedRequest;
            Success = false;
        }
    }
}