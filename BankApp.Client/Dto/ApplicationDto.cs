namespace BankApp.Client.Dto
{
    public class ApplicationDto
    {
        public int ApplicationID { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string MobileNumber { get; set; }
        public string AadharNumber { get; set; }
        public string PAN { get; set; }
        public string CustomerImageURL { get; set; }
        public int AccountTypeID { get; set; }
        public int Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string RejectionReason { get; set; }
    }

}

