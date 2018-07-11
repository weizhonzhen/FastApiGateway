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

                    if (downparam.Method.ToLower() == "post")
                    {
                        if (downparam.IsBody)
                        {
                            var info = BaseUrl.PostContent(downparam);
                            context.Response.StatusCode = info.status;
                            context.Response.WriteAsync(info.msg, Encoding.UTF8);
                        }
                        else
                        {
                            var info = BaseUrl.PostUrl(downparam);
                            context.Response.StatusCode = info.status;
                            context.Response.WriteAsync(info.msg, Encoding.UTF8);
                        }
                    }
                    else if (downparam.Method.ToLower() == "get")
                    {
                        var info = BaseUrl.GetUrl(downparam);
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
                        if (downparam.Method.ToLower() == "post")
                        {
                            task.Add(Task.Factory.StartNew(() =>
                            {
                                if (downparam.IsBody)
                                {
                                    downparam.Param = content;
                                    result.Add(BaseUrl.PostContent(downparam));
                                }
                                else
                                    result.Add(BaseUrl.PostUrl(downparam));
                            }));
                        }
                        else if (downparam.Method.ToLower() == "get")
                        {
                            task.Add(Task.Factory.StartNew(() =>
                            {
                                result.Add(BaseUrl.GetUrl(downparam));
                            }));
                        }
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
    }
}
