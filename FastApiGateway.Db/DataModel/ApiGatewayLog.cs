using FastData.Core.Property;
using System;

namespace FastApiGatewayDb.DataModel
{
    [Table(Comments = "下游响应等待处理")]
    public class ApiGatewayLog
    {
        /// <summary>
        /// key
        /// </summary>
        [Column(Comments = "接口key", DataType = "varchar2", Length = 16, IsNull = false)]
        public string Key { get; set; }

        /// <summary>
        /// Success 1=成功 0=失败
        /// </summary>
        [Column(Comments = "Success 1=成功 0=失败", DataType = "number(1,0)")]
        public int Success { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        [Column(Comments = "Url", DataType = "varchar2", Length = 255, IsNull = false)]
        public string Url { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        [Column(Comments = "端口", DataType = "varchar2", Length = 6, IsNull = false)]
        public string Protocol { get; set; }

        /// <summary>
        /// 响应时间 
        /// </summary>
        [Column(Comments = "响应时间", DataType = "varchar2", Length = 12, IsNull = false)]
        public string Milliseconds { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        [Column(Comments = "请求时间", DataType = "date", IsNull = false)]
        public DateTime ActionTime { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [Column(Comments = "请求参数", DataType = "clob", IsNull = true)]
        public string ActionParam { get; set; }

        /// <summary>
        /// 返回结果
        /// </summary>
        [Column(Comments = "返回结果", DataType = "clob", IsNull = true)]
        public string Result { get; set; }
    }
}
