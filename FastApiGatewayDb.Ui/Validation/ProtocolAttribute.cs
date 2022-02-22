using FastUntility.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Validation
{
    public class ProtocolAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.Compare( value.ToStr(), "http", true) ==0)
                return ValidationResult.Success;
            else if (string.Compare( value.ToStr(), "soap", true) ==0)
                return ValidationResult.Success;
            else if (string.Compare( value.ToStr(),"rabbitmq", true) ==0)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = string.Format("{0}不正确",validationContext.DisplayName));
        }
    }
}
