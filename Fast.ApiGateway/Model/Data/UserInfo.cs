using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fast.ApiGateway.Model.Data
{
    public class UserInfo
    {
        /// <summary>
        /// mongo id
        /// </summary>
        public ObjectId _id { get; set; }

        /// <summary>
        /// key
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// token过期时间 
        /// </summary>
        public DateTime AccessExpires { get; set; }
        
        /// <summary>
        /// token ip 
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// power
        /// </summary>
        public List<Power> Power { get; set; }
    }

    /// <summary>
    /// 地址权限
    /// </summary>
    public class Power
    { 
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }
    }
}
