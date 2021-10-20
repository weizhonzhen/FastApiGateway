using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using FastUntility.Core.Base;
using FastData.Core.Context;
using FastApiGatewayDb.Model;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using FastApiGatewayDb.DataModel.Oracle;
using FastData.Core.Repository;
using FastApiGatewayDb.Aop;
using FastRabbitMQ.Core;
using FastRabbitMQ.Core.Model;

namespace FastApiGatewayDb
{
    public class FastApiGatewayDb : IFastApiGatewayDb
    {
        public static string DbApi = "";

        public Task ContentAsync(HttpContext context, IHttpClientFactory client, IFastRepository IFast, ConfigOption option)
        {
            DbApi = option.dbKey;
            var urlParam = GetUrlParam(context);
            var urlParamDecode = HttpUtility.UrlDecode(urlParam);

            using (var db = new DataContext(DbApi))
            {
                context.Response.ContentType = "application/Json";
                var key = context.Request.Path.Value.ToStr().Replace("/", "");

                if (IFast.Query<ApiGatewayUrl>(a => a.Key.ToUpper() == key.ToUpper()).ToCount(db) <= 0 || string.IsNullOrEmpty(key))
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("success", false);
                    dic.Add("result", string.Format("请求地址{0}无效", key));
                    context.Response.StatusCode = 404;
                    return context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
                }
                else
                {
                    var item = IFast.Query<ApiGatewayUrl>(a => a.Key.ToUpper() == key.ToUpper()).ToItem<ApiGatewayUrl>(db) ?? new ApiGatewayUrl();
                    var downParam = IFast.Query<ApiGatewayDownParam>(a => a.Key.ToUpper() == key.ToUpper()).OrderBy<ApiGatewayDownParam>(a => new { a.OrderBy }, false).ToList<ApiGatewayDownParam>(db);

                    //获取token
                    if (item.IsGetToken == 1)
                        return Token(context, db,IFast, urlParam);
                    else
                    {
                        //是否匿名访问
                        if (item.IsAnonymous == 0)
                            if (!CheckToken(item, context, db,IFast, urlParam))
                                return Task.CompletedTask;

                        //结果是否缓存
                        if (item.IsCache == 1)
                        {
                            var resultInfo = IFast.Query<ApiGatewayCache>(a => a.Key.ToUpper() == key).ToItem<ApiGatewayCache>(db);
                            if (DateTime.Compare(resultInfo.TimeOut, DateTime.Now) > 0)
                            {
                                context.Response.StatusCode = 200;
                                return context.Response.WriteAsync(resultInfo.result, Encoding.UTF8);
                            }
                            else
                            {
                                if (item.Schema.ToStr().ToLower() == "polling") //polling 轮循请求
                                    return Polling(item, context, db,IFast,downParam, urlParamDecode, urlParam,client);
                                else if (item.Schema.ToStr().ToLower() == "composite") //composite 合并请求
                                    return Composite(item, context, db,IFast, downParam, urlParamDecode, urlParam,client);
                                else
                                    return Normal(item, context, db,IFast, downParam, urlParamDecode, urlParam,client);
                            }
                        }
                        else
                        {
                            if (item.Schema.ToStr().ToLower() == "polling") //polling 轮循请求
                                return Polling(item, context, db,IFast, downParam, urlParamDecode, urlParam,client);
                            else if (item.Schema.ToStr().ToLower() == "composite") //composite 合并请求
                                return Composite(item, context, db,IFast, downParam, urlParamDecode, urlParam,client);
                            else
                                return Normal(item, context, db,IFast, downParam, urlParamDecode, urlParam,client);
                        }
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
        private ReturnModel GetReuslt(ApiGatewayDownParam downparam, string param, string key, int isTextLog, int isDbLog, DataContext db, IFastRepository IFast, HttpContext context, string ActionId, int OrderBy, IHttpClientFactory client)
        {
            var info = IFast.Query<ApiGatewayWait>(a => a.Key.ToLower() == key.ToLower() && a.Url.ToLower() == downparam.Url.ToLower()).ToItem<ApiGatewayWait>(db) ?? new ApiGatewayWait();
            if (info.Key.ToStr().ToLower() == key.ToLower() && DateTime.Compare(info.NextAction, DateTime.Now) > 0)
            {
                //return time out
                var dic = new Dictionary<string, object>();
                dic.Add("success", false);
                dic.Add("result", "等待恢复");
                return new ReturnModel { msg = BaseJson.ModelToJson(dic), status = 408 };
            }
            else
            {
                var result = new ReturnModel();
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                BaseAop.Before(downparam.Url,key, ref param, downparam.Protocol);

                if (downparam.Protocol.ToLower() == "soap")
                    result = BaseUrl.SoapUrl(downparam.Url, downparam.SoapParamName, downparam.SoapMethod, param, client, key, downparam.SoapNamespace);
                else if (downparam.Protocol.ToLower() == "http")
                {
                    //http
                    if (downparam.Method.ToStr().ToLower() == "post")
                    {
                        if (downparam.IsBody == 1)
                            result = BaseUrl.PostContent(downparam.Url, param, key, client);
                        else
                            result = BaseUrl.PostUrl(downparam.Url, param, key, client);
                    }
                    else if (downparam.Method.ToStr().ToLower() == "get")
                        result = BaseUrl.GetUrl(downparam.Url, param, key, client);
                }
                else if (downparam.Protocol.ToLower() == "rabbitmq")
                {
                    result.status = 200;
                    result.msg = "成功";

                    var mqConfig = new ConfigModel();
                    mqConfig.QueueName = downparam.QueueName;
                    FastRabbit.Send(mqConfig, GetUrlParam(param));
                }
                //else if (downparam.Protocol.ToLower() == "rpc")
                //    result = BaseUrl.RpcUrl(downparam.QueueName, param);
                else
                    result.status = 408;

                BaseAop.After(downparam.Url,key, ref param, downparam.Protocol, result);

                if (result.status == 408)
                {
                    //time out
                    var wait = new ApiGatewayWait();
                    wait.Key = key;
                    wait.Url = downparam.Url;
                    wait.WaitHour = downparam.WaitHour.ToInt(0);
                    wait.ErrorMsg = result.msg;
                    wait.NextAction = DateTime.Now.AddHours(wait.WaitHour.ToStr().ToDouble(0));

                    IFast.Add(wait, db);
                }
                else if (info.Key.ToStr().ToLower() == key.ToLower())
                    IFast.Delete<ApiGatewayWait>(a => a.Key.ToLower() == key.ToLower(), db);

                stopwatch.Stop();

                var logInfo = new ApiGatewayLog();
                logInfo.ActionId = ActionId;
                logInfo.OrderBy = OrderBy;
                logInfo.Key = key;
                logInfo.ActionTime = DateTime.Now;
                logInfo.Url = downparam.Url;
                logInfo.Protocol = downparam.Protocol;
                logInfo.Success = result.status == 200 ? 1 : 0;
                logInfo.Result = result.msg;
                logInfo.Milliseconds = stopwatch.Elapsed.TotalMilliseconds;
                logInfo.ActionIp = GetClientIp(context);
                logInfo.ActionParam = param;

                if (isDbLog == 1)
                    IFast.Add(logInfo, null, DbApi);

                if (isTextLog == 1)
                    BaseLog.SaveLog(BaseJson.ModelToJson(logInfo), logInfo.Key);

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
        private bool CheckToken(ApiGatewayUrl item, HttpContext context, DataContext db, IFastRepository IFast, string urlParam)
        {
            var dic = new Dictionary<string, object>();
            var token = GetUrlParamKey(urlParam, "token");

            if (IFast.Query<ApiGatewayUser>(a => a.AccessToken.ToUpper() == token.ToUpper()).ToCount(db) == 0)
            {
                context.Response.StatusCode = 200;
                dic.Add("success", false);
                dic.Add("result", "token不存在");
                context.Response.WriteAsync(BaseJson.ModelToJson(dic), Encoding.UTF8);
                return false;
            }
            else
            {
                var tokenInfo = IFast.Query<ApiGatewayUser>(a => a.AccessToken.ToUpper() == token.ToUpper()).ToItem<ApiGatewayUser>(db);
                if (DateTime.Compare(tokenInfo.AccessExpires, DateTime.Now) < 0)
                {
                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", "token过期");
                    context.Response.WriteAsync(BaseJson.ModelToJson(dic), Encoding.UTF8);
                    return false;
                }

                if (tokenInfo.Ip != GetClientIp(context))
                {
                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", "token授权ip地址异常");
                    context.Response.WriteAsync(BaseJson.ModelToJson(dic), Encoding.UTF8);
                    return false;
                }

                if (tokenInfo.Power.IndexOf(',') > 0)
                {
                    foreach (var temp in tokenInfo.Power.Split(','))
                    {
                        if (temp.ToLower() == item.Key.ToLower())
                            return true;
                    }

                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", string.Format("{0}没有权限访问", item.Key));
                    context.Response.WriteAsync(BaseJson.ModelToJson(dic), Encoding.UTF8);
                    return false;
                }
                else
                {
                    context.Response.StatusCode = 200;
                    dic.Add("success", false);
                    dic.Add("result", string.Format("{0}没有权限访问", item.Key));
                    context.Response.WriteAsync(BaseJson.ModelToJson(dic), Encoding.UTF8);
                    return false;
                }
            }
        }
        #endregion

        #region 普通请求
        /// <summary>
        /// 普通请求
        /// </summary>
        private Task Normal(ApiGatewayUrl item, HttpContext context, DataContext db, IFastRepository IFast, List<ApiGatewayDownParam> list, string urlParamDecode, string urlParam, IHttpClientFactory client)
        {
            var actionId = Guid.NewGuid().ToStr();
            var downparam = list.FirstOrDefault() ?? new ApiGatewayDownParam();
            var param = downparam.IsDecode == 1 ? urlParamDecode : urlParam;
            var info = GetReuslt(downparam, param, item.Key, item.IsTxtLog, item.IsDbLog, db,IFast, context, actionId, 1,client);

            //缓存结果
            if (item.IsCache == 1)
                CacheResult(item, db,IFast, info);

            context.Response.StatusCode = info.status;
            return context.Response.WriteAsync(info.msg, Encoding.UTF8);
        }
        #endregion

        #region 轮循请求
        /// <summary>
        /// 轮循请求
        /// </summary>
        private Task Polling(ApiGatewayUrl item, HttpContext context, DataContext db, IFastRepository IFast, List<ApiGatewayDownParam> list, string urlParamDecode, string urlParam, IHttpClientFactory client)
        {
            var orderBy = 1;
            var actionId = Guid.NewGuid().ToStr();
            var rand = new Random();
            var index = rand.Next(1, list.Count);
            var downparam = list[index];
            var param = downparam.IsDecode == 1 ? urlParamDecode : urlParam;
            var info = GetReuslt(downparam, param, item.Key, item.IsTxtLog, item.IsDbLog, db,IFast, context, actionId, orderBy,client);

            if (info.status != 200 && list.Count > 1)
            {
                orderBy++;
                var tempIndex = rand.Next(1, list.Count);
                while (index == tempIndex)
                {
                    tempIndex = rand.Next(1, list.Count);
                }

                downparam = list[tempIndex];
                info = GetReuslt(downparam, param, item.Key, item.IsTxtLog, item.IsDbLog, db,IFast, context, actionId, orderBy,client);


                context.Response.StatusCode = info.status;
                return context.Response.WriteAsync(info.msg, Encoding.UTF8);
            }
            else
            {
                //缓存结果
                if (item.IsCache == 1)
                    CacheResult(item, db,IFast, info);

                context.Response.StatusCode = info.status;
                return context.Response.WriteAsync(info.msg, Encoding.UTF8);
            }
        }
        #endregion

        #region 合并请求
        /// <summary>
        /// 合并请求
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        private Task Composite(ApiGatewayUrl item, HttpContext context, DataContext db, IFastRepository IFast, List<ApiGatewayDownParam> list, string urlParamDecode, string urlParam, IHttpClientFactory client)
        {
            var actionId = Guid.NewGuid().ToStr();
            var orderBy = 1;
            var result = new ReturnModel();
            var lastResult = new ReturnModel();

            foreach (var downparam in list)
            {
                var param = downparam.IsDecode == 1 ? urlParamDecode : urlParam;

                if (downparam.SourceParam == 2)
                    param = lastResult.msg;

                lastResult = GetReuslt(downparam, param, item.Key, item.IsTxtLog, item.IsDbLog, db,IFast, context, actionId, orderBy,client);

                if (string.IsNullOrEmpty(lastResult.msg) || lastResult.status != 200)
                {
                    result.msg = "";
                    break;
                }
                if (downparam.IsResult == 1)
                {
                    if (string.IsNullOrEmpty(result.msg))
                        result.msg = lastResult.msg;
                    else
                        result.msg = string.Format("{0}||{1}", result.msg, lastResult.msg);
                }
                orderBy++;
            }


            //缓存结果
            if (item.IsCache == 1)
                CacheResult(item, db,IFast, result, null);

            context.Response.StatusCode = 200;
            return context.Response.WriteAsync(result.msg, Encoding.UTF8);
        }
        #endregion

        #region 获取token
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        private Task Token(HttpContext context, DataContext db,IFastRepository IFast, string urlParam)
        {
            var dic = new Dictionary<string, object>();
            var AppKey = GetUrlParamKey(urlParam, "AppKey");
            var AppSecret = GetUrlParamKey(urlParam, "AppSecret");

            if (IFast.Query<ApiGatewayUser>(a => a.AppKey.ToLower() == AppKey.ToLower() && a.AppSecret.ToLower() == AppSecret.ToLower()).ToCount(db) <= 0)
            {
                context.Response.StatusCode = 200;
                dic.Add("success", false);
                dic.Add("result", "AppKey和AppSecret参数不存在");
                return context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
            }
            else
            {
                var info = IFast.Query<ApiGatewayUser>(a => a.AppKey.ToLower() == AppKey.ToLower() && a.AppSecret.ToLower() == AppSecret.ToLower()).ToItem<ApiGatewayUser>(db);
                info.Ip = GetClientIp(context);
                info.AccessExpires = DateTime.Now.AddHours(24).AddHours(8);
                info.AccessToken = BaseSymmetric.Generate(string.Format("{0}_{1}_{2}", info.AppKey, info.AppSecret, info.AccessExpires)).ToLower();

                //修改信息
                IFast.Update<ApiGatewayUser>(info,
                    a => a.AppKey.ToLower() == AppKey.ToLower() && a.AppSecret.ToLower() == AppSecret.ToLower()
                    , a => new { a.AccessExpires, a.Ip, a.AccessToken }, db);

                dic.Add("success", true);
                dic.Add("AccessToken", info.AccessToken);
                dic.Add("AccessExpires", info.AccessExpires);

                context.Response.StatusCode = 200;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8);
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
        private void CacheResult(ApiGatewayUrl item, DataContext db,IFastRepository IFast, ReturnModel info = null, Dictionary<string, object> dic = null)
        {
            var model = new ApiGatewayCache();
            model.Key = item.Key.ToLower();
            model.TimeOut = DateTime.Now.AddDays(item.CacheTimeOut.ToStr().ToDouble(0)).AddHours(8);

            if (info != null)
                model.result = BaseJson.ModelToJson(info.msg);

            if (dic != null)
                model.result = BaseJson.ModelToJson(dic);

            if (IFast.Query<ApiGatewayCache>(a => a.Key.ToUpper() == model.Key.ToUpper()).ToCount(db) <= 0)
                IFast.Add(model);
            else
                IFast.Update<ApiGatewayCache>(model, a => a.Key.ToUpper() == model.Key.ToUpper(), a => new { a.result, a.TimeOut }, db);
        }
        #endregion

        #region 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetUrlParam(HttpContext context)
        {
            using (var content = new StreamReader(context.Request.Body))
            {
                var param = context.Request.QueryString.Value;

                if (string.IsNullOrEmpty(param))
                    param = content.ReadToEnd();

                if (!string.IsNullOrEmpty(param) && param.Substring(0, 1) == "?")
                    param = param.Substring(1, param.Length - 1);

                return param;
            }
        }
        #endregion

        #region 解析参数
        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetUrlParamKey(string param, string key)
        {
            var dic = new Dictionary<string, object>();
            if (param.IndexOf('&') > 0)
            {
                foreach (var temp in param.Split('&'))
                {
                    if (temp.IndexOf('=') > 0)
                        dic.Add(temp.Split('=')[0], temp.Split('=')[1]);
                }
            }
            else
            {
                if (param.IndexOf('=') > 0)
                    dic.Add(param.Split('=')[0], param.Split('=')[1]);
            }

            return dic.GetValue(key).ToStr();
        }
        #endregion

        #region 解析参数
        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetUrlParam(string param)
        {
            var dic = new Dictionary<string, object>();
            if (param.IndexOf('&') > 0)
            {
                foreach (var temp in param.Split('&'))
                {
                    if (temp.IndexOf('=') > 0)
                        dic.Add(temp.Split('=')[0], temp.Split('=')[1]);
                }
            }
            else
            {
                if (param.IndexOf('=') > 0)
                    dic.Add(param.Split('=')[0], param.Split('=')[1]);
            }

            return dic;
        }
        #endregion
    }
}
