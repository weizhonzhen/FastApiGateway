using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using FastMongoDb.Core;
using FastApiGateway.Model.Return;
using FastApiGateway.Model.Data;
using FastMongoDb.Core.Base;
using FastApiGateway.Model.Cache;

namespace FastApiGateway
{
    public class FastApiGateway : IFastApiGateway
    {
        public void Content(HttpContext context)
        {
            context.Response.ContentType = "application/Json";
            var key = context.Request.Path.Value.ToStr().Replace("/", "").ToLower();

            if (MongoDbInfo.GetCount<UrlModel>(a => a.Key.ToLower() == key) <= 0 || string.IsNullOrEmpty(key))
            {
                var dic = new Dictionary<string, object>();
                dic.Add("success", false);
                dic.Add("result", string.Format("请求地址{0}无效", key));
                context.Response.StatusCode = 404;
                context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
            }
            else
            {
                var item = MongoDbInfo.GetModel<UrlModel>(a => a.Key.ToLower() == key) ?? new UrlModel();
                item.DownParam = item.DownParam ?? new List<DownParam>();

                //获取token
                if (item.IsGetToken)
                    Token(context);
                else
                {
                    //是否匿名访问
                    if (!item.IsAnonymous)
                        if (!CheckToken(item, context))
                            return;

                    //结果是否缓存
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
                            if (item.Schema.ToStr().ToLower() == "polling") //polling 轮循请求
                                Polling(item, context);
                            else if (item.Schema.ToStr().ToLower() == "composite") //composite 合并请求
                                Composite(item, context);
                            else
                                Normal(item, context);
                        }
                    }
                    else
                    {
                        if (item.Schema.ToStr().ToLower() == "polling") //polling 轮循请求
                            Polling(item, context);
                        else if (item.Schema.ToStr().ToLower() == "composite") //composite 合并请求
                            Composite(item, context);
                        else
                            Normal(item, context);
                    }
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
        private ReturnModel GetReuslt(DownParam downparam, string param, string content, string key,bool isLog,int timeOut)
        {
            var info = MongoDbInfo.GetModel<WaitModel>(a => a.Key.ToLower() == key.ToLower() && a.Url.ToLower() == downparam.Url.ToLower()) ?? new WaitModel();
            if (info.Key.ToStr().ToLower() == key.ToLower() && DateTime.Compare(info.NextAction, DateTime.Now) > 0)
            {
                //return time out
                var dic = new Dictionary<string, object>();
                dic.Add("success", false);
                dic.Add("result", "等待恢复");
                return new ReturnModel { msg = JsonConvert.SerializeObject(dic).ToString(), status = 408 };
            }
            else
            {
                var result = new ReturnModel();
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                if (downparam.Protocol.ToLower() == "soap")
                {  
                    //soap
                    result = BaseUrl.SoapUrl(downparam.Url, downparam.SoapParamName, param, downparam.SoapMethod, timeOut);
                }
                else if (downparam.Protocol.ToLower() == "http")
                {
                    //http
                    if (downparam.Method.ToStr().ToLower() == "post")
                    {
                        if (downparam.IsBody)
                            result = BaseUrl.PostContent(downparam.Url, content,key, timeOut);
                        else
                            result = BaseUrl.PostUrl(downparam.Url, param,key, timeOut);
                    }
                    else if (downparam.Method.ToStr().ToLower() == "get")
                        result = BaseUrl.GetUrl(downparam.Url, param,key, timeOut);
                }
                else if (downparam.Protocol.ToLower() == "mq")
                    //mq
                    result = BaseUrl.RabbitUrl(downparam.QueueName, param);
                else
                    result.status = 408;

                if (result.status == 408)
                    //time out
                    Task.Factory.StartNew(() =>
                    {
                        var wait = new WaitModel();
                        wait.Key = key;
                        wait.Url = downparam.Url;
                        wait.WaitHour = downparam.WaitHour;
                        wait.ErrorMsg = result.msg;
                        wait.NextAction = DateTime.Now.AddHours(wait.WaitHour).AddHours(8);

                        MongoDbInfo.Add<WaitModel>(wait);
                    });
                else if (info.Key.ToStr().ToLower() == key.ToLower())
                    //log
                    Task.Factory.StartNew(() =>
                    {
                        MongoDbInfo.Delete<WaitModel>(a => a.Key.ToLower() == key.ToLower());
                    });

                stopwatch.Stop();

                if (isLog) //log
                    Task.Factory.StartNew(() =>
                    {
                        var logInfo = new LogModel();
                        logInfo.Key = key;
                        logInfo.ActionTime = DateTime.Now;
                        logInfo.Url = downparam.Url;
                        logInfo.Protocol = downparam.Protocol;
                        logInfo.Success = result.status == 200 ? true : false;
                        logInfo.Milliseconds = stopwatch.Elapsed.TotalMilliseconds;
                        MongoDbInfo.Add(logInfo);
                    });

                return result;
            }
        }
        #endregion

        #region 验证token
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        private bool CheckToken(UrlModel item, HttpContext context)
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
                return false;
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
                    return false;
                }

                if (tokenInfo.Ip != GetClientIp(context))
                {
                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", "token授权ip地址异常");
                    context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
                    return false;
                }
                                
                if(!tokenInfo.Power.Exists(a=>a.Key.ToLower()==item.Key.ToLower()))
                {
                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", string.Format("{0}没有权限访问", item.Key));
                    context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
                    return false;
                }
            }

            return true;
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

            var downparam = item.DownParam.FirstOrDefault() ?? new DownParam();
            var info = GetReuslt(downparam, param, content,item.Key,item.IsLog,item.RequestTimeOut);

            //缓存结果
            if (item.IsCache)
                CacheResult(item, info);

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
            
            var info = GetReuslt(downparam, param, content,item.Key, item.IsLog,item.RequestTimeOut);

            if (info.status != 200 && item.DownParam.Count > 1)
            {
                var tempIndex = rand.Next(1, item.DownParam.Count);
                while (index == tempIndex)
                {
                    tempIndex = rand.Next(1, item.DownParam.Count);
                }

                downparam = item.DownParam[tempIndex];
                info = GetReuslt(downparam, param, content,item.Key, item.IsLog,item.RequestTimeOut);


                context.Response.StatusCode = info.status;
                context.Response.WriteAsync(info.msg, Encoding.UTF8);
            }
            else
            {
                //缓存结果
                if (item.IsCache)
                    CacheResult(item, info);

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
                    result.Add(GetReuslt(downparam, param, content,item.Key, item.IsLog,item.RequestTimeOut));
                }));
            }

            Task.WaitAll(task.ToArray());

            var count = 0;
            foreach (var temp in result)
            {
                downDic.Add(string.Format("result{0}", count), BaseJson.JsonToDic(temp.msg));
                count++;
            }

            //缓存结果
            if (item.IsCache)
                CacheResult(item, null, downDic);

            context.Response.StatusCode = 200;
            context.Response.WriteAsync(JsonConvert.SerializeObject(downDic).ToString(), Encoding.UTF8);
        }
        #endregion

        #region 获取token
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        private void Token(HttpContext context)
        {
            var dic = new Dictionary<string, object>();
            var AppKey = "";
            var AppSecret = "";

            if (context.Request.Method.ToLower() == "get")
            {
                AppKey = context.Request.Query["AppKey"].ToStr().ToLower();
                AppSecret = context.Request.Query["AppSecret"].ToStr().ToLower();
            }
            else if (context.Request.Method.ToLower() == "post")
            {
                AppKey = context.Request.Form["AppKey"].ToStr().ToLower();
                AppSecret = context.Request.Form["AppSecret"].ToStr().ToLower();
            }

            if (MongoDbInfo.GetCount<UserInfo>(a => a.AppKey.ToLower() == AppKey && a.AppSecret.ToLower() == AppSecret) <= 0)
            {
                context.Response.StatusCode = 200;
                dic.Add("success", false);
                dic.Add("result", "AppKey和AppSecret参数不存在");
                context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
            }
            else
            {
                var info = MongoDbInfo.GetModel<UserInfo>(a => a.AppKey.ToLower() == AppKey && a.AppSecret.ToLower() == AppSecret);
                info.Ip = GetClientIp(context);
                info.AccessExpires = DateTime.Now.AddHours(24).AddHours(8);
                info.AccessToken = BaseSymmetric.Generate(string.Format("{0}_{1}_{2}", info.AppKey, info.AppSecret, info.AccessExpires)).ToLower();

                //修改信息
                MongoDbInfo.Update<UserInfo>(
                    a => a.AppKey.ToLower() == AppKey && a.AppSecret.ToLower() == AppSecret
                    , info, a => new { a.AccessExpires, a.Ip, a.AccessToken }
                );

                dic.Add("success", true);
                dic.Add("AccessToken", info.AccessToken);
                dic.Add("AccessExpires", info.AccessExpires);

                context.Response.StatusCode = 200;
                context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
            }
        }
        #endregion

        #region 获取客户Ip
        /// <summary>
        /// 获取客户Ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetClientIp(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        #endregion

        #region 缓存结果
        /// <summary>
        /// 缓存结果
        /// </summary>
        /// <param name="info"></param>
        /// <param name="item"></param>
        private void CacheResult(UrlModel item, ReturnModel info = null, Dictionary<string, object> dic = null)
        {
            var model = new CacheModel();
            model.Key = item.Key.ToLower();
            model.TimeOut = DateTime.Now.AddDays(item.CacheTimeOut).AddHours(8);

            if (info != null)
                model.result = BaseJson.JsonToDic(info.msg);

            if (dic != null)
                model.result = dic;

            if (MongoDbInfo.GetCount<CacheModel>(a => a.Key.ToLower() == model.Key) <= 0)
                MongoDbInfo.Add(model);
            else
            {
                MongoDbInfo.Update<CacheModel>(a => a.Key.ToLower() == model.Key, model, a => new { a.result, a.TimeOut });
            }
        }
        #endregion
    }
}
