namespace BankApp.Client.Dto
{
    public class AccountDto
    {
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }

}

