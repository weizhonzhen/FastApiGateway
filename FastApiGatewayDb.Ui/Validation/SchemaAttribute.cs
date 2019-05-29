using FastUntility.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace FastApiGatewayDb.Ui.Validation
{
    public class SchemaAttribute : ValidationAttribute
    {
        public bool IsNull { set; get; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsNull && string.IsNullOrEmpty(value.ToStr()))
                return ValidationResult.Success;

            if (value.ToStr().ToLower() == "composite")
                return ValidationResult.Success;
            else if (value.ToStr().ToLower() == "polling")
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = "{0}不正确");
        }
    }
}
