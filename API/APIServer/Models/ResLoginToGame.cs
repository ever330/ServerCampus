namespace APIServer.Models
{
    public class ResLoginToGame
    {
        public ErrorCode Result { get; set; }
        public UserGameData? GameData { get; set; }
    }
}
