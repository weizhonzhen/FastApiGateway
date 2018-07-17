using Api.Gateway.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using Api.Gateway.Model.Data;
using Api.Gateway.Model.Return;
using Api.Gateway.Model.Cache;
using Untility.Core.Base;
using MongoDb.Core;

namespace Api.Gateway
{
    public class ApiResponse : IResponse
    {
        public void Content(HttpContext context)
        {
            context.Response.ContentType = "application/Json";
            var key = context.Request.Path.Value.Replace("/", "").ToLower();

            if (MongoDbInfo.GetCount<UrlModel>(a => a.Key.ToLower() == key)<0)
            {
                context.Response.StatusCode = 404;
                context.Response.WriteAsync(string.Format("请求地址{0}无效", key), Encoding.UTF8);
            }
            else
            {
                var item = MongoDbInfo.GetModel<UrlModel>(a => a.Key.ToLower() == key);

                if (!item.IsGetToken || !item.IsAnonymous)
                    CheckToken(item, context);

                if (item.IsCache)
                {
                    var resultInfo = MongoDbInfo.GetModel<CacheModel>(a => a.Key.ToLower() == key);
                    if (DateTime.Compare(resultInfo.TimeOut, DateTime.Now) > 0)
                    {
                        context.Response.StatusCode = 200;
                        context.Response.WriteAsync(JsonConvert.SerializeObject(resultInfo.result).ToString(), Encoding.UTF8);
                    }
                    else
                    {
                        if (item.Schema.ToLower() == "polling") //polling 轮循请求
                            Polling(item, context);
                        else if (item.Schema.ToLower() == "composite") //composite 合并请求
                            Composite(item, context);
                        else
                            Normal(item, context);
                    }
                }
                else
                {
                    if (item.Schema.ToLower() == "polling") //polling 轮循请求
                        Polling(item, context);
                    else if (item.Schema.ToLower() == "composite") //composite 合并请求
                        Composite(item, context);
                    else
                        Normal(item, context);
                }
            }
        }

        #region 处理请求
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
        #endregion

        #region 验证token
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        private void CheckToken(UrlModel item, HttpContext context)
        {
            var dic = new Dictionary<string, object>();
            var token = "";
            if (context.Request.Method.ToLower() == "get")
                token = context.Request.Query["token"].ToStr().ToLower();
            else if (context.Request.Method.ToLower() == "post")
                token = context.Request.Form["token"].ToStr().ToLower();

            if (MongoDbInfo.GetCount<UserInfo>(a => a.AccessToken.ToLower() == token) == 0)
            {
                context.Response.StatusCode = 200;
                dic.Add("success", false);
                dic.Add("result", "token不存在");
                context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
            }
            else
            {
                var tokenInfo = MongoDbInfo.GetModel<UserInfo>(a => a.AccessToken.ToLower() == token);
                if (DateTime.Compare(tokenInfo.AccessExpires, DateTime.Now) < 0)
                {
                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", "token过期");
                    context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
                }
            }
        }
        #endregion

        #region 普通请求
        /// <summary>
        /// 普通请求
        /// </summary>
        private void Normal(UrlModel item, HttpContext context)
        {
            var content = new StreamReader(context.Request.Body).ReadToEnd();
            var param = context.Request.QueryString.Value;

            var downparam = item.DownParam.First();
            var info = GetReuslt(downparam, param, content);

            context.Response.StatusCode = info.status;
            context.Response.WriteAsync(info.msg, Encoding.UTF8);
        }
        #endregion

        #region 轮循请求
        /// <summary>
        /// 轮循请求
        /// </summary>
        private void Polling(UrlModel item, HttpContext context)
        {
            var content = new StreamReader(context.Request.Body).ReadToEnd();
            var param = context.Request.QueryString.Value;

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
        #endregion

        #region 合并请求
        /// <summary>
        /// 合并请求
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        private void Composite(UrlModel item, HttpContext context)
        {
            var downDic = new Dictionary<string, object>();
            var result = new List<ReturnModel>();
            var task = new List<Task>();
            var content = new StreamReader(context.Request.Body).ReadToEnd();
            var param = context.Request.QueryString.Value;

            foreach (var downparam in item.DownParam)
            {
                task.Add(Task.Factory.StartNew(() =>
                {
                    result.Add(GetReuslt(downparam, param, content));
                }));
            }

            Task.WaitAll(task.ToArray());

            var count = 0;
            foreach (var temp in result)
            {
                downDic.Add(string.Format("result{0}", count), BaseJson.JsonToDic(temp.msg));
                count++;
            }

            context.Response.StatusCode = 200;
            context.Response.WriteAsync(JsonConvert.SerializeObject(downDic).ToString(), Encoding.UTF8);
        }
        #endregion
    }
}
