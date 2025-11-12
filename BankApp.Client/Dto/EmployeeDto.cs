namespace BankApp.Client.Dto
{
    public class EmployeeDto
    {
        public int EmployeeID { get; set; }
        public string ApplicationUserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string StaffCode { get; set; }
        public string JobTitle { get; set; }
        public DateTime DateHired { get; set; }
    }

}
