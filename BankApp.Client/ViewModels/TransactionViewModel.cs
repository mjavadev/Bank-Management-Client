using System.ComponentModel.DataAnnotations;

namespace BankApp.Client.ViewModels
{
    public class TransactionViewModel
    {
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        public int TransactionType { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string? RecipientAccountNumber { get; set; }

        public decimal CurrentBalance { get; set; }
    }

}