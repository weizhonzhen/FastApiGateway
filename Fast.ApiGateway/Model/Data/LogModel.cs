using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fast.ApiGateway.Model.Data
{
    public class LogModel
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
        /// Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Milliseconds 
        /// </summary>
        public double Milliseconds { get; set; }

        /// <summary>
        /// action time
        /// </summary>
        public DateTime ActionTime { get; set; }
    }
}
