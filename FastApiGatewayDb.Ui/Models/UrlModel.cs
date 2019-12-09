using System.ComponentModel.DataAnnotations;
using FastApiGatewayDb.Ui.Mvc.Validation;

namespace FastApiGatewayDb.Ui.Mvc.Models
{
    public class UrlModel
    {
        [StringLength(16, ErrorMessage = "{0}最大长度16")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "键")]
        public string Key { get; set; }

        [StringLength(16, ErrorMessage = "{0}最大长度16")]
        [Schema(IsNull =true)]
        [Display(Name = "请求类型")]
        public string Schema { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0,1)]
        [Display(Name = "是否缓存")]
        public int IsCache { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 99)]
        [Display(Name = "缓存时间天")]
        public int CacheTimeOut { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否匿名访问")]
        public int IsAnonymous { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否获取token接口")]
        public int IsGetToken { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否数据日记")]
        public int IsDbLog { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否文本日记")]
        public int IsTxtLog { get; set; }

        [StringLength(128, ErrorMessage = "{0}最大长度128")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "名称")]
        public string Name { get; set; }
    }
}
