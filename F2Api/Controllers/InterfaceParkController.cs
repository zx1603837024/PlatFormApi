using F2.Application.PDA;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions.Models;
using F2.Core.Extensions.WebMvc;
using System;
using System.Web.Http;

namespace F2Api.Controllers
{
    /// <summary>
    /// 封闭停车场接口
    /// </summary>
    public class InterfaceParkController : ApiController
    {
        #region Var
        private readonly IPDAAppService _pdaAppService;
        #endregion

        /// <summary>
        /// 封闭停车场接口构造函数
        /// </summary>
        public InterfaceParkController()
        {
            _pdaAppService = new PDAAppService();
        }

        /// <summary>
        /// 获取车辆欠费记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetEscapeDetailsList([FromUri]EscapeDetailInput input)
        {
            return Json(new AjaxResponse(_pdaAppService.GetEscapeDetailList(input)));
        }

        /// <summary>
        /// 获取微信支付二维码
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetWeixinPay(string guid)
        {
            throw new Exception("1131334");
            
            //return null;
        }


        /// <summary>
        /// 获取支付宝二维码
        /// </summary>
        /// <param name="total_amount"></param>
        /// <param name="subject"></param>
        /// <param name="out_trade_no"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetAliPayCode(string total_amount,string subject,string out_trade_no)
        {
            return Json(new AjaxResponse(_pdaAppService.GetAliPayCode(total_amount,subject, out_trade_no)));
         
            //return null;
        }

        /// <summary>
        /// 统一收单交易创建接口
        /// </summary>
        /// <param name="total_amount"></param>
        /// <param name="subject"></param>
        /// <param name="out_trade_no"></param>
        /// <param name="buyer_id"></param>
        /// <returns></returns>
        [HttpGet]
        public object CreatDeal(string total_amount, string subject,string out_trade_no,string buyer_id)
        {
            return Json(new AjaxResponse(_pdaAppService.CreatDeal(total_amount, subject, out_trade_no, buyer_id)));

            //return null;
        }

        /// <summary>
        /// 统一收单线下交易查询
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <returns></returns>
        [HttpGet]
        public object QueryAliPay(string out_trade_no)
        {
            return Json(new AjaxResponse(_pdaAppService.QueryAliPay(out_trade_no)));

            //return null;
        }
    }
}
