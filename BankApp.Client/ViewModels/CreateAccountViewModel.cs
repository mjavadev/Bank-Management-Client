using BankApp.Client.Dto;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Client.ViewModels
{
    public class CreateAccountViewModel
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Please select an account type")]
        public int AccountTypeID { get; set; }

        public List<AccountTypeDto> AvailableAccountTypes { get; set; } = new List<AccountTypeDto>();
    }
}
