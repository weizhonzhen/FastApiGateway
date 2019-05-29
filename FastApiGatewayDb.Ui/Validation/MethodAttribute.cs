using FastUntility.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FastApiGatewayDb.Ui.Validation
{
    public class MethodAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.ToStr().ToLower() == "get")
                return ValidationResult.Success;
            else if (value.ToStr().ToLower() == "post")
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = "{0}不正确");
        }
    }
}
