using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastApiGatewayDb.Ui.Models;
using FastData.Core;
using FastData.Core.Context;
using FastUntility.Core.Page;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class CacheController : Controller
    {
        #region 加载缓存
        /// <summary>
        /// 加载缓存
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData.Model = new QueryPageUrlModel();
            return View();
        }
        #endregion

        #region 缓存分页
        /// <summary>
        /// 缓存分页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List(QueryPageUrlModel item)
        {
            using (var db = new DataContext(DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key });
                var info = FastMap.QueryPage(page, "Api.Cache", param.ToArray(), null, DbKey.Api);

                return PartialView("List", info);
            }
        }
        #endregion
    }
}
