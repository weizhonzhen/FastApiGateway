using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Gateway.Model
{
    public class UrlModel
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        //down url
        public string Url { get; set; }

        /// <summary>
        /// method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// is body action
        /// </summary>
        public bool IsContent { get; set; }

        /// <summary>
        /// param
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// time out
        /// </summary>
        public int Seconds { get; set; }
    }
}
