using System.ComponentModel.DataAnnotations;

namespace BankApp.Client.ViewModels
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Staff code is required")]
        public string StaffCode { get; set; }

        [Required(ErrorMessage = "Job title is required")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Date hired is required")]
        [DataType(DataType.Date)]
        public DateTime DateHired { get; set; }
    }

}
