namespace FastApiGatewayDbClient.Core.DataModel
{
    /// <summary>
    /// 接口
    /// </summary>
    public class ApiGatewayUrl
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Composite(合并请求),Polling(轮循请求)
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 是否缓存 1=是 0=否
        /// </summary>
        public int IsCache { get; set; }

        /// <summary>
        /// 缓存过期时间（天）
        /// </summary>
        public int CacheTimeOut { get; set; }

        /// <summary>
        /// 是否匿名访问  1=是 0=否
        /// </summary>
        public int IsAnonymous { get; set; } = 1;

        /// <summary>
        /// 是否获取token
        /// </summary>
        public int IsGetToken { get; set; }

        /// <summary>
        /// 是否数据日记  1=是 0=否
        /// </summary>
        public int IsDbLog { get; set; }

        /// <summary>
        /// 是否文本日记  1=是 0=否
        /// </summary>
        public int IsTxtLog { get; set; }
    }
}
