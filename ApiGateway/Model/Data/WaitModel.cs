using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Gateway.Model.Data
{
    /// <summary>
    /// 下游响应等待处理
    /// </summary>
    public class WaitModel
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 无响应等待下次请求时间(小时单位)
        /// </summary>
        public int WaitHour { get; set; }

        /// <summary>
        /// 无响应等待下次请求时间
        /// </summary>
        public DateTime NextAction { get; set; }
    }
}
