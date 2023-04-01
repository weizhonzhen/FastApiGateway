using FastData.Core.Property;
using System;

namespace FastApiGatewayDb.DataModel.MySql
{
    /// <summary>
    /// 下游响应等待处理
    /// </summary>
    [Table(Comments = "下游响应等待处理")]
    public class ApiGatewayWait
    {
        /// <summary>
        /// key
        /// </summary>
        [Column(Comments = "接口key", DataType = "Char", Length = 32, IsNull = false)]
        public string Key { get; set; }

        /// <summary>
        /// 无响应等待下次请求时间(小时单位)
        [Column(Comments = "无响应等待下次请求时间(小时单位)", DataType = "int")]
        /// </summary>
        public int WaitHour { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [Column(Comments = "url", DataType = "Char", Length = 255, IsNull = false)]
        public string Url { get; set; }

        /// <summary>
        /// 无响应等待下次请求时间
        /// </summary>
        [Column(Comments = "无响应等待下次请求时间", DataType = "Datetime", IsNull = false)]
        public DateTime NextAction { get; set; }

        /// <summary>
        /// 出错信息
        /// </summary>
        [Column(Comments = "出错信息", DataType = "text", IsNull = true)]
        public string ErrorMsg { get; set; }
    }
}
