﻿using FastData.Core.Property;
using System;

namespace FastApiGatewayDb.DataModel.MySql
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
        [Column(Comments = "接口key", DataType = "Char", Length = 32, IsNull = false)]
        public string Key { get; set; }

        /// <summary>
        /// 缓存结果 
        /// </summary>
        [Column(Comments = "缓存结果", DataType = "text", IsNull =false)]
        public string result { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Column(Comments = "过期时间", DataType = "datetime", IsNull =false)]
        public DateTime TimeOut { get; set; }
    }
}
