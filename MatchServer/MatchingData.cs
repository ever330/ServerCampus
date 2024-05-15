
public class MatchServerConfig
{
    public string RedisAddress { get; set; }
    public string ReqListKey { get; set; }
    public string ResListKey { get; set; }
}

public class RequestMatchData
{
    public string UserA { get; set; }
    public string UserB { get; set; }
}

public class ResponseMatchData
{
    public string UserA { get; set; }
    public string UserB { get; set; }
    public string ServerAddress { get; set; }
    public int Port { get; set; }
    public int RoomNumber { get; set; }
}

public class CompleteMatchData
{
    public string ServerAddress { get; set; }
    public int Port { get; set; }
    public int RoomNumber { get; set; }
    public string OtherUserId { get; set; }
}