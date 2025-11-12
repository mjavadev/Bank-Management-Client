using BankApp.Client.Dto;

namespace BankApp.Client.ViewModels
{
    public class DashboardViewModel
    {
        public CustomerDto Customer { get; set; }
        public List<AccountDto> Accounts { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; }
        public decimal TotalBalance { get; set; }
        public int PendingTransactionsCount { get; set; }
    }

}
