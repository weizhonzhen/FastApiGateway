using FastData.Core.Property;
using System;

namespace FastApiGatewayDb.DataModel.SqlServer
{
    [Table(Comments = "下游响应等待处理")]
    public class ApiGatewayLog
    {
        /// <summary>
        /// key
        /// </summary>
        [Column(Comments = "接口key", DataType = "char", Length = 32, IsNull = false)]
        public string Key { get; set; }

        /// <summary>
        /// Success 1=成功 0=失败
        /// </summary>
        [Column(Comments = "Success 1=成功 0=失败", DataType = "int")]
        public int Success { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        [Column(Comments = "Url", DataType = "char", Length = 255, IsNull = false)]
        public string Url { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        [Column(Comments = "端口", DataType = "char", Length = 6, IsNull = false)]
        public string Protocol { get; set; }

        /// <summary>
        /// 响应时间 
        /// </summary>
        [Column(Comments = "响应时间", DataType = "decimal(16,6)", Length = 12, IsNull = false)]
        public double Milliseconds { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        [Column(Comments = "请求时间", DataType = "datetime", IsNull = false)]
        public DateTime ActionTime { get; set; }
        
        /// <summary>
        /// ip
        /// </summary>
        [Column(Comments = "ip", DataType = "char", Length = 16, IsNull = true)]
        public string ActionIp{ get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [Column(Comments = "请求参数", DataType = "text", IsNull = true)]
        public string ActionParam { get; set; }
        
        /// <summary>
        /// 返回结果
        /// </summary>
        [Column(Comments = "返回结果", DataType = "text", IsNull = true)]
        public string Result { get; set; }

        /// <summary>
        /// 请求id
        /// </summary>
        [Column(Comments = "请求id", DataType = "char", Length = 36, IsNull = true)]
        public string ActionId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column(Comments = "排序", DataType = "int", IsNull = true)]
        public int OrderBy { get; set; }
    }
}
