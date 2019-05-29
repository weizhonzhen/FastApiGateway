using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastApiGatewayDb.Ui.Models;
using FastData.Core;
using FastData.Core.Context;
using FastUntility.Core.Base;
using FastUntility.Core.Page;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class UserController : Controller
    {
        #region 加载用户
        /// <summary>
        /// 加载用户
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData.Model = new QueryPageUrlModel();
            return View();
        }
        #endregion

        #region 用户分页
        /// <summary>
        /// 用户分页
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
                var info = FastMap.QueryPage(page, "Api.User", param.ToArray(),db);

                return PartialView("List", info);
            }
        }
        #endregion

        #region 用户操作
        /// <summary>
        /// 用户操作
        /// </summary>
        /// <returns></returns>
        public IActionResult Option(QueryPageUrlModel item)
        {
            if (string.IsNullOrEmpty(item.Key))
                return Content("key不能为空");
            else
            {

                return View();
            }
        }
        #endregion
    }
}
