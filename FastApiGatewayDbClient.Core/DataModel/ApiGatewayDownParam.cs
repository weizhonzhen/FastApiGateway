namespace FastApiGatewayDbClient.Core.DataModel
{
    /// <summary>
    /// 下游请求表
    /// </summary>
    public class ApiGatewayDownParam
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 下游url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 下游名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 请求方法method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// is body action
        /// </summary>
        public int IsBody { get; set; }

        /// <summary>
        /// 无响应等待下次请求时间(小时单位)
        /// </summary>
        public string WaitHour { get; set; }

        /// <summary>
        /// 协议soap,http
        /// </summary>
        public string Protocol { get; set; } = "http";

        /// <summary>
        /// soap method
        /// </summary>
        public string SoapMethod { get; set; }

        /// <summary>
        /// soap param name,| 隔开 
        /// </summary>
        public string SoapParamName { get; set; }

        /// <summary>
        /// 是否解码 1- 解码 0-不处理
        /// </summary>  
        public int IsDecode { get; set; }

        /// <summary>
        /// 是否结果 1-是 0-否
        /// </summary>
        public int IsResult { get; set; }

        /// <summary>
        /// 参数来源 1=url,2=上个请求结果
        /// </summary>
        public int SourceParam { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// soap Namespace
        /// </summary>
        public string SoapNamespace { get; set; }

        /// <summary>
        /// Queue Name
        /// </summary>
        public string QueueName { get; set; }
    }
}
