namespace APIServer.Models
{
    public class ReqVerifyAuthToken
    {
        public string Email { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
    }
}
