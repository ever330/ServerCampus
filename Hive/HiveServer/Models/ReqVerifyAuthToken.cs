namespace HiveServer.Models
{
    public class ReqVerifyAuthToken
    {
        public string Email { get; set; }
        public string AuthToken { get; set; }
    }
}
