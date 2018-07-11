using Api.Gateway.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

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
                context.Response.WriteAsync(string.Format("请求地址{0}无效", key), Encoding.UTF8);
            }
            else
            {
                var item = list.Find(a => a.Key.ToLower() == key.ToLower());

                //polling 轮循请求
                if (item.Schema.ToLower() == "polling")
                {
                    var rand = new Random();
                    var index = rand.Next(1, item.DownParam.Count);
                    var downparam = item.DownParam[index];

                    var info = GetReuslt(downparam, param, content);

                    if (info.status != 200 && item.DownParam.Count > 1)
                    {
                        var tempIndex = rand.Next(1, item.DownParam.Count);
                        while (index == tempIndex)
                        {
                            tempIndex = rand.Next(1, item.DownParam.Count);
                        }

                        downparam = item.DownParam[tempIndex];
                        info = GetReuslt(downparam, param, content);

                        context.Response.StatusCode = info.status;
                        context.Response.WriteAsync(info.msg, Encoding.UTF8);
                    }
                    else
                    {
                        context.Response.StatusCode = info.status;
                        context.Response.WriteAsync(info.msg, Encoding.UTF8);
                    }
                }


                //composite 合并请求
                if (item.Schema.ToLower() == "composite")
                {
                    var downDic = new List<Dictionary<string, object>>();
                    var result = new List<ReturnModel>();
                    var task = new List<Task>();

                    foreach (var downparam in item.DownParam)
                    {
                        task.Add(Task.Factory.StartNew(() =>
                        {
                            result.Add(GetReuslt(downparam, param, content));
                        }));
                    }

                    Task.WaitAll(task.ToArray());

                    foreach (var temp in result)
                    {
                        downDic.Add(BaseJson.JsonToDic(temp.msg));
                    }

                    context.Response.StatusCode = 200;
                    context.Response.WriteAsync(JsonConvert.SerializeObject(downDic).ToString(), Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="downparam">下游</param>
        /// <param name="param">请求参数</param>
        /// <param name="content">请求参数body</param>
        /// <returns></returns>
        private ReturnModel GetReuslt(DownParam downparam, string param, string content)
        {
            if (downparam.Method.ToLower() == "post")
            {
                if (downparam.IsBody)
                {
                    downparam.Param = content;
                    return BaseUrl.PostContent(downparam);
                }
                else
                {
                    downparam.Param = param;
                    return BaseUrl.PostUrl(downparam);
                }
            }
            else if (downparam.Method.ToLower() == "get")
            {
                downparam.Param = param;
                return BaseUrl.GetUrl(downparam);
            }
            else
                return new ReturnModel { status = 200, msg = "" };
        }
    }
}
