namespace HiveServer.Models
{
    public class AccountInfo
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Salt { get; set; } = null!;
    }
}
