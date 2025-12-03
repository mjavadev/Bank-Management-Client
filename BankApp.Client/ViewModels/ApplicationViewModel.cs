using BankApp.Client.Dto;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BankApp.Client.ViewModels
{
    public class ApplicationViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Occupation is required")]
        public string Occupation { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Aadhar Number is required")]
        [Display(Name = "Aadhar Number")]
        [AadharValidation(ErrorMessage = "Aadhar must be 12 digits")]
        public string AadharNumber { get; set; }

        [Required(ErrorMessage = "PAN is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be 10 characters")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "PAN format is invalid (e.g., ABCDE1234F)")]
        public string PAN { get; set; }

        [Required(ErrorMessage = "Account type is required")]
        public int AccountTypeID { get; set; }

        public IFormFile ImageFile { get; set; }

        public List<AccountTypeDto> AvailableAccountTypes { get; set; } = new List<AccountTypeDto>();

        public class AadharValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                    return ValidationResult.Success;

                string aadhar = value.ToString().Replace(" ", "").Replace("-", "");

                if (aadhar.Length == 12 && aadhar.All(char.IsDigit))
                    return ValidationResult.Success;

                return new ValidationResult("Aadhar must be exactly 12 digits");
            }
        }
    }
}
