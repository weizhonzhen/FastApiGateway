using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using FastUntility.Core.Base;
using System.IO.Compression;

namespace FastApiGatewayDb.Ui.Filter
{
    /// <summary>
    /// 权限过滤器
    /// </summary>
    public class PowerAttribute : ActionFilterAttribute
    {
        #region 请求过滤器
        /// <summary>
        /// 请求过滤器
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var controller = filterContext.Controller as Controller;

            #region model验证处理
            if (!controller.ModelState.IsValid)
            {
                var item = controller.ViewData.ModelState.Values.ToList().Find(a => a.Errors.Count > 0);
                var error = item.Errors.Where(a => !string.IsNullOrEmpty(a.ErrorMessage)).Take(1).SingleOrDefault().ErrorMessage;
                filterContext.Result = new JsonResult(new { success = false, msg = error });
                return;
            }
            #endregion
        }
        #endregion

        #region 结果过滤器
        /// <summary>
        /// 结果过滤器
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //压缩
            if (filterContext.HttpContext.Response.StatusCode == 200)
            {
                var acceptEncoding = filterContext.HttpContext.Request.Headers["Accept-Encoding"].ToString().ToLower();
                if (!string.IsNullOrEmpty(acceptEncoding))
                {
                    if (filterContext.HttpContext.Response.Headers["Content-encoding"].ToArray().Length > 0)
                        filterContext.HttpContext.Response.Headers.Remove("Content-encoding");

                    if (acceptEncoding.Contains("gzip"))
                        filterContext.HttpContext.Response.Body = new GZipStream(filterContext.HttpContext.Response.Body, CompressionMode.Compress);
                    else if (acceptEncoding.Contains("deflate"))
                        filterContext.HttpContext.Response.Body = new DeflateStream(filterContext.HttpContext.Response.Body, CompressionMode.Compress);
                }
            }
        }
        #endregion
    }
}