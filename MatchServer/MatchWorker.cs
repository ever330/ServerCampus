using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using ZLogger;

public interface IMatchWorker : IDisposable
{
    public void AddUser(string id);
    public CompleteMatchData GetCompleteMatchData(string id);
    public void CancelMatching(string id);
}

public class MatchWorker : IMatchWorker
{
    ConcurrentQueue<string> _reqUserQueue = new ConcurrentQueue<string>();
    ConcurrentDictionary<string, CompleteMatchData> _completeDic = new ConcurrentDictionary<string, CompleteMatchData>();
    List<string> _cancelUsers = new List<string>();
    object _lock = new object();

    Thread _reqThread;
    Thread _checkThread;

    bool _isThreadRunning;

    string _redisAddress;
    string _reqListKey;
    string _resListKey;

    readonly ILogger<MatchWorker> _logger;

    CloudStructures.RedisConnection _connection;

    public MatchWorker(ILogger<MatchWorker> logger, IOptions<MatchServerConfig> matchServerConfig)
    {
        _logger = logger;

        _redisAddress = matchServerConfig.Value.RedisAddress;
        _reqListKey = matchServerConfig.Value.ReqListKey;
        _resListKey = matchServerConfig.Value.ResListKey;

        var conf = new CloudStructures.RedisConfig("Match", _redisAddress);
        _connection = new CloudStructures.RedisConnection(conf);

        _logger.ZLogInformation($"MatchWorker생성");

        _isThreadRunning = true;

        _reqThread = new Thread(RunRequestMatching);
        _reqThread.Start();

        _checkThread = new Thread(RunCheckMatching);
        _checkThread.Start();
    }

    public void Dispose()
    {
        _isThreadRunning = false;
        _connection.GetConnection().Close();
    }

    public void AddUser(string id)
    {
        if (_completeDic.TryAdd(id, null))
        {
            _reqUserQueue.Enqueue(id);
        }
    }

    public CompleteMatchData GetCompleteMatchData(string id)
    {
        if (_completeDic.ContainsKey(id))
        {
            _completeDic.TryRemove(id, out CompleteMatchData comp);
            return comp;
        }
        return null;
    }

    public void CancelMatching(string id)
    {
        lock (_lock)
        {
            if (!_cancelUsers.Contains(id))
            {
                _cancelUsers.Add(id);
            }
        }
    }


    void RunRequestMatching()
    {
        while (_isThreadRunning)
        {
            if (_reqUserQueue.Count < 2)
            {
                Thread.Sleep(1);
                continue;
            }

            try
            {
                _logger.ZLogInformation($"Redis 매칭 요청");

                var req = new RequestMatchData();
                req.UserA = GetMatchUser();
                req.UserB = GetMatchUser();
                _logger.ZLogInformation($"요청유저 {req.UserA}, {req.UserB}");

                var defaultExpiry = TimeSpan.FromDays(1);
                var redis = new RedisList<RequestMatchData>(_connection, _reqListKey, defaultExpiry);

                redis.RightPushAsync(req);
            }
            catch
            {
                _logger.ZLogError($"Redis 매칭 요청 List Push 문제 발생");
            }
        }
    }

    string GetMatchUser()
    {
        string result;
        _reqUserQueue.TryDequeue(out result);
        while (_cancelUsers.Contains(result))
        {
            lock (_lock)
            {
                _cancelUsers.Remove(result);
            }
            _reqUserQueue.TryDequeue(out result);
        }

        return result;
    }

    void RunCheckMatching()
    {
        while (_isThreadRunning)
        {
            try
            {
                var res = new ResponseMatchData();

                var defaultExpiry = TimeSpan.FromDays(1);
                var redis = new RedisList<ResponseMatchData>(_connection, _resListKey, defaultExpiry);

                var result = redis.LeftPopAsync().Result;
                if (result.HasValue)
                {
                    _logger.ZLogInformation($"Redis 매칭 결과");

                    res = result.Value;

                    var comp = new CompleteMatchData();
                    comp.ServerAddress = res.ServerAddress;
                    comp.Port = res.Port;
                    comp.RoomNumber = res.RoomNumber;

                    _completeDic.AddOrUpdate(res.UserA, comp, (key, oldValue) => comp);
                    _completeDic.AddOrUpdate(res.UserB, comp, (key, oldValue) => comp);
                    _logger.ZLogInformation($"매칭완료 딕셔너리 카운트 {_completeDic.Count}");
                }
            }
            catch
            {
                _logger.ZLogError($"Redis 매칭 결과 List Pop 문제 발생");
            }
        }
    }
}
