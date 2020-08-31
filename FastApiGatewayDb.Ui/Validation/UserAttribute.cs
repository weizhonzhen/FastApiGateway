using FastUntility.Core.Base;
using FastApiGatewayDb.DataModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using FastData.Core.Repository;

namespace FastApiGatewayDb.Ui.Validation
{
    public class UserAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var IFast = validationContext.GetService<IFastRepository>();
            var count = IFast.Query<ApiGatewayLogin>(a => a.UserName.ToLower() == value.ToStr().ToLower(), null, App.DbKey.Api).ToCount();

            if (count > 0)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = "用户名不正确");
        }
    }
}
