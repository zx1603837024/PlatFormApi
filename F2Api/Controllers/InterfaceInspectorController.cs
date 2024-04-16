using F2.Application.Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace F2Api.Controllers
{
    /// <summary>
    /// 巡查端接口
    /// </summary>
    public class InterfaceInspectorController : ApiController
    {
        #region Var
        private readonly IInspectorAppService _inspectorAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public InterfaceInspectorController()
        {
            _inspectorAppService = new InspectorAppService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BerthsecId"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetBerthList([FromUri]int BerthsecId)
        {
            return Json(_inspectorAppService.GetBerthList(BerthsecId));
        }
    }
}
