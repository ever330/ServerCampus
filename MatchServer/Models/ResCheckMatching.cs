namespace MatchServer.Models
{
    public class ResCheckMatching
    {
        public ErrorCode MatchResult { get; set; } = ErrorCode.None;
        public string ServerAddress { get; set; }
        public int Port { get; set; }
        public int RoomNumber { get; set; }
    }
}
