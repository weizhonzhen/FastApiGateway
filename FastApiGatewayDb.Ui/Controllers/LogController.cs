using System;
using System.Collections.Generic;
using FastApiGatewayDb.Ui.Models;
using FastData.Core.Context;
using FastUntility.Core.Page;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using FastUntility.Core.Base;
using FastData.Core.Repository;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class LogController : Controller
    {
        private readonly IFastRepository IFast;
        public LogController(IFastRepository _IFast)
        {
            IFast = _IFast;
        }

        #region 加载日志
        /// <summary>
        /// 加载日志
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var model= new QueryPageUrlModel();
            model.Day = DateTime.Now.ToDate("yyyy-MM-dd");

            ViewData.Model = model;
            return View();
        }
        #endregion

        #region 日志分页
        /// <summary>
        /// 日志分页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List(QueryPageUrlModel item)
        {
            using (var db = new DataContext(App.DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key });
                param.Add(new OracleParameter { ParameterName = "Ip", Value = item.Ip });
                param.Add(new OracleParameter { ParameterName = "Day", Value = item.Day.ToDate("yyyy-MM-dd").ToDate() });
                param.Add(new OracleParameter { ParameterName = "Success", Value = item.Success });

                var info = IFast.QueryPage(page, "Api.Log", param.ToArray(), db);

                return PartialView("List", info);
            }
        }
        #endregion
    }
}
