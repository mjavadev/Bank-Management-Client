namespace BankApp.Client.Dto
{
    public class CustomerDto
    {
        public int CustomerID { get; set; }
        public string ApplicationUserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string MobileNumber { get; set; }
        public string ApprovedByUserID { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string AadharNumber { get; set; }
        public string PAN { get; set; }
        public string CustomerImageURL { get; set; }
        public bool IsActive { get; set; }
    }

}
