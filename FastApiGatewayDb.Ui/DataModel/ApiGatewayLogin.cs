using FastData.Core.Property;

namespace FastApiGatewayDb.DataModel
{
    /// <summary>
    /// 登陆用户
    /// </summary>
    [Table(Comments = "登陆用户")]
    public class ApiGatewayLogin
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Column(Comments = "用户名", DataType = "varchar2", Length = 32, IsKey = true)]
        public string UserName { get; set; }
        
        /// <summary>
        /// 密码
        /// </summary>
        [Column(Comments = "密码", DataType = "varchar2", Length = 32, IsNull =false)]
        public string UserPwd { get; set; }
    }
}
