using Api.Gateway.Model;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Api.Gateway
{
    public class ApiResponse : IResponse 
    {
        public void Content(HttpContext context)
        {
            context.Response.ContentType = "application/Json";
            var list = BaseConfig.GetListValue<UrlModel>("ApiGateway");
            var key = context.Request.Path.Value.Replace("/", "");
            var content = new StreamReader(context.Request.Body).ReadToEnd();
            var param = context.Request.QueryString.Value;

            if (!list.Exists(a => a.Key.ToLower() == key.ToLower()))
            {
                context.Response.StatusCode = 404;
                context.Response.WriteAsync(string.Format("请求地址无效", key), System.Text.Encoding.UTF8);
            }
            else
            {
                var item = list.Find(a => a.Key.ToLower() == key.ToLower());
                item.Param = param;

                if (item.Method.ToLower() == "post")
                {
                    if (item.IsContent)
                    {
                        item.Param = content;
                        var info = BaseUrl.PostContent(item);
                        context.Response.StatusCode = info.status;
                        context.Response.WriteAsync(info.msg, System.Text.Encoding.UTF8);
                    }
                    else
                    {
                        var info = BaseUrl.PostUrl(item);
                        context.Response.StatusCode = info.status;
                        context.Response.WriteAsync(info.msg, System.Text.Encoding.UTF8);
                    }
                }
                else if (item.Method.ToLower() == "get")
                {

                    var info = BaseUrl.GetUrl(item);
                    context.Response.StatusCode = info.status;
                    context.Response.WriteAsync(info.msg, System.Text.Encoding.UTF8);
                }
            }
        }       
    }
}
