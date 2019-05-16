using Microsoft.AspNetCore.Mvc;
using FastData.Core;
using FastData.Core.Context;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using FastUntility.Core.Page;
using FastApiGatewayDb.Ui.Models;
using FastApiGatewayDb.DataModel;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class HomeController : Controller
    {
        #region 加载接口
        /// <summary>
        /// 加载接口
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData.Model = new QueryPageUrlModel();
            return View();
        }
        #endregion

        #region 下游分页
        /// <summary>
        /// 下游分页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownList(QueryPageUrlModel item)
        {
            using (var db = new DataContext(DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;
                var info = new PageResult();

                if (!string.IsNullOrEmpty(item.Key))
                {
                    var param = new List<OracleParameter>();
                    param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key });
                    info = FastMap.QueryPage(page, "Api.DownUrl", param.ToArray(), null, DbKey.Api);
                }
                else
                    info.list = new List<Dictionary<string, object>>();

                return PartialView("DownList", info);
            }
        }
        #endregion

        #region url分页
        /// <summary>
        /// url分页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UrlList(QueryPageUrlModel item)
        {
            using (var db = new DataContext(DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key });
                var info = FastMap.QueryPage(page, "Api.Url", param.ToArray(), null, DbKey.Api);

                return PartialView("UrlList", info);
            }
        }
        #endregion
        
        #region 接口操作
        /// <summary>
        /// 接口操作
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
