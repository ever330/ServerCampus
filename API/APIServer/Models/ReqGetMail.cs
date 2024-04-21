namespace APIServer.Models
{
    public class ReqGetMail
    {
        public string Email { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
    }
}
