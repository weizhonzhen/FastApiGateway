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

            if (string.Compare( value.ToStr(),"composite",false)==0)
                return ValidationResult.Success;
            else if (string.Compare( value.ToStr(), "polling",false)==0)
                return ValidationResult.Success;
            else
                return new ValidationResult(ErrorMessage = "{0}不正确");
        }
    }
}
