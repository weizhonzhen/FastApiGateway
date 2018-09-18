using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastApiGateway.Model.Data
{
    /// <summary>
    /// 下游响应等待处理
    /// </summary>
    public class WaitModel
    {
        /// <summary>
        /// mongo id
        /// </summary>
        public ObjectId _id { get; set; }

        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 无响应等待下次请求时间(小时单位)
        /// </summary>
        public int WaitHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 无响应等待下次请求时间
        /// </summary>
        public DateTime NextAction { get; set; }

        /// <summary>
        /// error msg
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
