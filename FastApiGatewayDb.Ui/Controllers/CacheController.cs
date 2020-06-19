using System.Collections.Generic;
using FastApiGatewayDb.Ui.Models;
using FastData.Core.Context;
using FastData.Core.Repository;
using FastUntility.Core.Page;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class CacheController : Controller
    {
        private readonly IFastRepository IFast;
        public CacheController(IFastRepository _IFast)
        {
            IFast = _IFast;
        }

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
            using (var db = new DataContext(App.DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key });
                var info = IFast.QueryPage(page, "Api.Cache", param.ToArray(), db);

                return PartialView("List", info);
            }
        }
        #endregion
    }
}
