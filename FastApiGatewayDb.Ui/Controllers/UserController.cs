using System.Collections.Generic;
using FastApiGatewayDb.DataModel;
using FastApiGatewayDb.Ui.Models;
using FastData.Core.Context;
using FastUntility.Core.Page;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using FastData.Core.Repository;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class UserController : Controller
    {
        private readonly IFastRepository IFast;
        public UserController(IFastRepository _IFast)
        {
            IFast = _IFast;
        }

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
                var info = IFast.QueryPage(page, "Api.User", param.ToArray(),db);

                return PartialView("List", info);
            }
        }
        #endregion

        #region 加载用户操作
        /// <summary>
        /// 加载用户操作
        /// </summary>
        /// <returns></returns>
        public IActionResult Option(string key)
        {
            var list = new List<ApiGatewayUrl>();
            var model = new UserModel();
            model.AppKey = key;

            using (var db = new DataContext(App.DbKey.Api))
            {
                list = IFast.Query<ApiGatewayUrl>(a => a.Key != "", a => new { a.Key, a.Name }).ToList<ApiGatewayUrl>(db);

                if (!string.IsNullOrEmpty(key))
                {
                    var info = IFast.Query<ApiGatewayUser>(a => a.AppKey.ToLower() == key.ToLower()).ToItem<ApiGatewayUser>();
                    model.Ip = info.Ip;
                    model.Power = info.Power;
                    if (!string.IsNullOrEmpty(model.Power))
                        ViewBag.Power = model.Power.Split(",");
                    else
                        ViewBag.Power = new string[0];
                }
                else
                    ViewBag.Power = new string[0];
            }

            ViewBag.List = list;
            ViewData.Model = model;
            return View();
        }
        #endregion

        #region 用户操作
        /// <summary>
        /// 用户操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UserOption(UserModel item)
        {
            var isSuccess = false;
            var model = new ApiGatewayUser();

            model.AppKey = item.AppKey;
            model.AppSecret = Guid.NewGuid().ToString().Substring(0, 31);
            model.Power = item.Power;
            model.Ip = item.Ip;

            using (var db = new DataContext(App.DbKey.Api))
            {
                if (IFast.Query<ApiGatewayUser>(a => a.AppKey == model.AppKey).ToCount(db) > 0)
                    isSuccess = db.Update<ApiGatewayUser>(model, a => a.AppKey.ToLower() == model.AppKey.ToLower(), a => new { a.AppKey, a.Ip, a.Power }).writeReturn.IsSuccess;
                else
                    isSuccess = db.Add(model).writeReturn.IsSuccess;
            }

            if (isSuccess)
                return Json(new { success = isSuccess, msg = "操作成功" });
            else
                return Json(new { success = isSuccess, msg = "操作失败" });
        }
        #endregion
    }
}
