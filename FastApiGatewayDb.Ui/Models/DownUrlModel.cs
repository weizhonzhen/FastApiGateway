using FastApiGatewayDb.Ui.Mvc.Validation;
using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Mvc.Models
{
    public class DownUrlModel
    {
        [StringLength(16, ErrorMessage = "{0}最大长度16")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "接口地址")]
        public string Key { get; set; }

        [StringLength(255, ErrorMessage = "{0}最大长度255")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "下游url")]
        public string Url { get; set; }

        [StringLength(128, ErrorMessage = "{0}最大长度128")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "下游名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "请求方法")]
        [Method]
        public string Method { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否流")]
        public int IsBody { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "协议")]
        [Protocol]
        public string Protocol { get; set; }

        [StringLength(32, ErrorMessage = "{0}最大长度32")]
        [Display(Name = "soap方法")]
        public string SoapMethod { get; set; }

        [StringLength(255, ErrorMessage = "{0}最大长度255")]
        [Display(Name = "soap参数")]
        public string SoapParamName { get; set; }
        
        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否解码")]
        public int IsDecode { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(0, 1)]
        [Display(Name = "是否结果")]
        public int IsResult { get; set; }

        [Required(ErrorMessage = "{0}不能为空")]
        [Range(1, 2)]
        [Display(Name = "参数来源")]
        public int SourceParam { get; set; }
        
        [Required(ErrorMessage = "{0}不能为空")]
        [Range(1, 9)]
        [Display(Name = "排序")]
        public int OrderBy { get; set; }

        [StringLength(16, ErrorMessage = "{0}最大长度16")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "无响应等待下次请求时间")]
        public string WaitHour { get; set; }

        [StringLength(16, ErrorMessage = "{0}最大长度16")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Display(Name = "无响应等待下次请求时间")]
        public string NameSpance { get; set; }


        /// <summary>
        /// Soap Namespace
        /// </summary>
        [StringLength(128, ErrorMessage = "{0}最大长度128")]
        [Display(Name = "Soap Namespace")]
        public string SoapNamespace { get; set; }
    }
}
