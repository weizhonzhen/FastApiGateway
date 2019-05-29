using System.ComponentModel.DataAnnotations;
using FastApiGatewayDb.Ui.Validation;

namespace FastApiGatewayDb.Ui.Models
{
    public class UrlModel
    {
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "键")]
        public string Key { get; set; }

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

        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "名称")]
        public string Name { get; set; }
    }
}
