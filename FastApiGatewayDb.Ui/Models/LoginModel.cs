using FastApiGatewayDb.Ui.Validation;
using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Models
{
    public class LoginModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "用户名")]
        [User]
        public string Code { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "密码")]
        public string Pwd { get; set; }
    }
}
