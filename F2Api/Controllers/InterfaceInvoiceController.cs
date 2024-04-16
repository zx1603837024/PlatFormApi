using CommonTool;
using F2.Application.Invoice;
using F2.Application.Invoice.Dtos;
using F2.Application.Parkade.Dtos;
using F2.Common;
using F2.Core.Extensions;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.UI.WebControls;
using System.Windows.Documents;
using static CommonTool.ApiEnum;
using static CommonTool.Commons;

namespace F2Api.Controllers
{
    /// <summary>
    /// 视频设备接口
    /// </summary>
    public class InterfaceInvoiceController : ApiController
    {
        #region Define
        private readonly IInvoiceService _InvoiceService;
        #endregion

        /// <summary>
        /// construct
        /// </summary>
        public InterfaceInvoiceController()
        {
            _InvoiceService = new InvoiceService();
        }
        /// <summary>
        /// 发票回调接口
        /// </summary>
        /// <param name="dto"></param>
        public InvoiceResponse InvoiceCallBack(int timestamp, object payload)
        {
            
            return _InvoiceService.InvoiceCallBack(timestamp, payload);
        }
        
    }
}