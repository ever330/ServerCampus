namespace APIServer.Models
{
    public class ReqVerifyAuthToken
    {
        public string Id { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
    }
}
