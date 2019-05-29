using FastUntility.Core.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Validation
{
    public class ProtocolAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.ToStr().ToLower() == "http")
                return ValidationResult.Success;
            else if (value.ToStr().ToLower() == "soap")
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = "{0}不正确");
        }
    }
}
