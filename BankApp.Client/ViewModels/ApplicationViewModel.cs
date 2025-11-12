using System.ComponentModel.DataAnnotations;

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
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Aadhar number is required")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhar must be 12 digits")]
        public string AadharNumber { get; set; }

        [Required(ErrorMessage = "PAN is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be 10 characters")]
        public string PAN { get; set; }

        [Required(ErrorMessage = "Account type is required")]
        public int AccountTypeID { get; set; }

        public IFormFile ImageFile { get; set; }
    }

}