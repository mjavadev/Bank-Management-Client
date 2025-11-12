namespace BankApp.Client.HttpClients
{
    public static class ApiConstant
    {
        #region Account
        public const string Authenticate = "Account/authenticate";
        public const string ChangePassword = "Account/change-password";
        #endregion

        #region Application
        public const string CreateApplication = "Application";
        public const string GetAllApplications = "Application";
        public const string GetPendingApplications = "Application/pending";
        public const string GetApplicationById = "Application/{0}";
        public const string ApproveApplication = "Application/approve/{0}";
        public const string RejectApplication = "Application/reject/{0}";
        public const string UpdateApplication = "Application/{0}";
        #endregion

        #region Customer
        public const string GetAllCustomers = "Customer";
        public const string GetCustomerById = "Customer/{0}";
        public const string GetCustomerByUserId = "Customer/by-user/{0}";
        public const string UpdateCustomer = "Customer/{0}";
        public const string DeleteCustomer = "Customer/{0}";
        #endregion

        #region Employee
        public const string GetAllEmployees = "Employee";
        public const string GetEmployeeById = "Employee/{0}";
        public const string CreateEmployee = "Employee";
        public const string UpdateEmployee = "Employee/{0}";
        public const string DeleteEmployee = "Employee/{0}";
        #endregion

        #region BankAccount
        public const string GetAccountsByCustomerId = "BankAccount/customer/{0}";
        public const string GetAccountById = "BankAccount/{0}";
        public const string CreateAccount = "BankAccount";
        public const string DeactivateAccount = "BankAccount/deactivate/{0}";
        #endregion

        #region Transaction
        public const string GetAllTransactions = "Transaction";
        public const string GetTransactionsByAccountId = "Transaction/account/{0}";
        public const string GetPendingTransactions = "Transaction/pending";
        public const string GetTransactionById = "Transaction/{0}";
        public const string CreateTransaction = "Transaction";
        public const string ApproveTransaction = "Transaction/approve/{0}";
        public const string RejectTransaction = "Transaction/reject/{0}";
        #endregion
    }

}