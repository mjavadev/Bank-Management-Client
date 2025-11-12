namespace BankApp.Client.Dto
{
    public class TransactionDto
    {
        public int TransactionID { get; set; }
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public int? RecipientAccountID { get; set; }
        public string RecipientAccountNumber { get; set; }
        public int Status { get; set; }
        public string ProcessedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }

}
