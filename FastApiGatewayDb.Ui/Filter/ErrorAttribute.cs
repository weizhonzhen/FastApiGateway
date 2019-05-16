using FastUntility.Core.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace FastApiGatewayDb.Ui.Filter
{ /// <summary>
  /// 出错处理过滤器
  /// </summary>
    public class ErrorAttribute : ExceptionFilterAttribute
    {
        #region 出错处理
        /// <summary>
        /// 出错处理
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            filterContext.ExceptionHandled = true;

            filterContext.Result = new ContentResult { Content = filterContext.Exception.StackTrace };

            var ip = filterContext.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
                ip = filterContext.HttpContext.Connection.RemoteIpAddress.ToString();

            BaseLog.SaveLog(string.Format("ip:{0}，错误内容:{1}", ip, filterContext.Exception.StackTrace), "web_error");
            return;
        }
        #endregion
    }
}