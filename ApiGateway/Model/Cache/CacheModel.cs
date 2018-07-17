using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Gateway.Model.Cache
{
    internal class CacheModel
    {
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
