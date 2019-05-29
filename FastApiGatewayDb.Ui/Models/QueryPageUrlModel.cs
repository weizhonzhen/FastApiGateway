using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Models
{
    public class QueryPageUrlModel
    {
        // <summary>
        /// 页码
        /// </summary>
        [Display(Name = "页码")]
        public int PageId { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [Display(Name = "每页条数")]
        public int PageSize { get; set; }

        /// <summary>
        /// Key
        /// </summary>                              
        [Display(Name = "Key")]
        public string Key { get; set; }

        /// <summary>
        /// Ip
        /// </summary>                              
        [Display(Name = "Ip")]
        public string Ip { get; set; }
        
        /// <summary>
        /// 日期
        /// </summary>                              
        [Display(Name = "日期")]
        public string Day { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>                              
        [Display(Name = "状态")]
        public string Success { get; set; }
    }
}
