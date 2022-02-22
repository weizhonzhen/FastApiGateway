using FastUntility.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Validation
{
    public class MethodAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.Compare( value.ToStr(),"get", true) ==0)
                return ValidationResult.Success;
            else if (string.Compare( value.ToStr(), "post", true) ==0)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = "{0}不正确");
        }
    }
}
