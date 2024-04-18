namespace APIServer.Models
{
    public class UserGameData
    {
        public string Email { get; set; } = null!;
        public int Level { get; set; }
        public int Exp { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
    }
}
