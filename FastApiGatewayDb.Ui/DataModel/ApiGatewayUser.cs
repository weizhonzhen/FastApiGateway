using FastData.Core.Property;
using System;

namespace FastApiGatewayDb.DataModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Table(Comments = "用户信息")]
    public class ApiGatewayUser
    {
        /// <summary>
        /// key
        /// </summary>
        [Column(Comments = "key", DataType = "varchar2", Length = 32, IsKey =true)]
        public string AppKey { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        [Column(Comments = "密钥", DataType = "varchar2", Length = 32, IsNull =false)]
        public string AppSecret { get; set; }

        /// <summary>
        /// token
        /// </summary>
        [Column(Comments = "token", DataType = "varchar2", Length = 32, IsNull = true)]
        public string AccessToken { get; set; }

        /// <summary>
        /// token过期时间 
        /// </summary>
        [Column(Comments = "token过期时间", DataType = "date", IsNull = true)]
        public DateTime AccessExpires { get; set; }

        /// <summary>
        /// token ip 
        /// </summary>
        [Column(Comments = "token ip", DataType = "varchar2",Length =32, IsNull = true)]
        public string Ip { get; set; }

        /// <summary>
        /// 访问key权限逗号隔开
        /// </summary>
        [Column(Comments = "访问key权限逗号隔开", DataType = "varchar2", Length = 1024, IsNull = true)]
        public string Power { get; set; }
    }
}
