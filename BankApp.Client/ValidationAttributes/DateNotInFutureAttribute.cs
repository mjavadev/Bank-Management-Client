using System;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Client.ValidationAttributes
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is DateTime dateValue)
            {
                if (dateValue.Date > DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future");
                }
            }
            return ValidationResult.Success;
        }
    }
}
