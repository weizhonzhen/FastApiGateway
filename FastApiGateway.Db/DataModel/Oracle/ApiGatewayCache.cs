using FastData.Core.Property;
using System;

namespace FastApiGatewayDb.DataModel.Oracle
{
    /// <summary>
    /// 缓存
    /// </summary>
    [Table(Comments = "缓存")]
    public class ApiGatewayCache
    {
        /// <summary>
        /// 接口key
        /// </summary>
        [Column(Comments = "接口key", DataType = "varchar2", Length = 32, IsNull = false)]
        public string Key { get; set; }

        /// <summary>
        /// 缓存结果 
        /// </summary>
        [Column(Comments = "缓存结果", DataType = "clob", IsNull =false)]
        public string result { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Column(Comments = "过期时间", DataType = "date", IsNull =false)]
        public DateTime TimeOut { get; set; }
    }
}
