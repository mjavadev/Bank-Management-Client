using System;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Client.Dto
{
    public class EditCustomerDto
    {
        [Required]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Occupation is required.")]
        public string Occupation { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [Phone(ErrorMessage = "Mobile Number must be valid.")]
        [MaxLength(15)]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Aadhar Number is required.")]
        [MinLength(12)]
        [MaxLength(12)]
        public string AadharNumber { get; set; }

        [Required(ErrorMessage = "PAN is required.")]
        [MinLength(10)]
        [MaxLength(10)]
        public string PAN { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string ApplicationUserID { get; set; }
    }
}
