namespace BankApp.Client.Dto
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public bool MustChangePassword { get; set; }
        public string TemporaryPassword { get; set; }
        public string Token { get; set; }
    }
}
