using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastApiGateway.Model.Cache
{
    internal class CacheModel
    {
        /// <summary>
        /// mongo id
        /// </summary>
        public ObjectId _id { get; set; }

        /// <summary>
        /// url key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// result
        /// </summary>
        public Dictionary<string,object> result { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime TimeOut { get; set; }
    }
}
