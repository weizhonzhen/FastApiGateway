using FastData.Core.Property;

namespace FastApiGatewayDb.DataModel.MySql
{
    /// <summary>
    /// 接口
    /// </summary>
    [Table(Comments = "接口")]
    public class ApiGatewayUrl
    {
        /// <summary>
        /// key
        /// </summary>
        [Column(Comments = "接口key", DataType = "Char", Length = 16, IsNull = false)]
        public string Key { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        [Column(Comments = "接口key", DataType = "Char", Length = 128, IsNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// Composite(合并请求),Polling(轮循请求)
        /// </summary>
        [Column(Comments = "Composite(合并请求),Polling(轮循请求)", DataType = "Char", Length = 16, IsNull = true)]
        public string Schema { get; set; }

        /// <summary>
        /// 是否缓存 1=是 0=否
        /// </summary>
        [Column(Comments = "是否缓存 1=是 0=否", DataType = "int",IsNull =true)]
        public int IsCache { get; set; }

        /// <summary>
        /// 缓存过期时间（天）
        /// </summary>
        [Column(Comments = "缓存过期时间（天）", DataType = "int",IsNull =true)]
        public int CacheTimeOut { get; set; }
        
        /// <summary>
        /// 是否匿名访问  1=是 0=否
        /// </summary>
        [Column(Comments = "是否匿名访问 1=是 0=否", DataType = "int")]
        public int IsAnonymous { get; set; }

        /// <summary>
        /// 是否获取token
        /// </summary>
        [Column(Comments = "是否获取token 1=是 0=否", DataType = "int")]
        public int IsGetToken { get; set; }

        /// <summary>
        /// 是否数据日记
        /// </summary>
        [Column(Comments = "是否数据日记 1=是 0=否", DataType = "int")]
        public int IsDbLog { get; set; }

        /// <summary>
        /// 是否文本日记
        /// </summary>
        [Column(Comments = "是否文本日记 1=是 0=否", DataType = "int")]
        public int IsTxtLog { get; set; }
    }
}
