using System;
using System.Collections.Generic;

namespace Api.Gateway.Model.Data
{
    public class UrlModel
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Composite(合并请求),Polling(轮循请求)
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// down param
        /// </summary>
        public List<DownParam> DownParam { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 过期时间天单位
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// 是否匿名访问
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// 是否获取token
        /// </summary>
        public bool IsGetToken { get; set; }
    }

    public class DownParam
    {
        //down url
        public string Url { get; set; }

        /// <summary>
        /// method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// is body action
        /// </summary>
        public bool IsBody { get; set; }

        /// <summary>
        /// param
        /// </summary>
        public string Param { get; set; }
    }
}
