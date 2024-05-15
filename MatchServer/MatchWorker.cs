﻿using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using ZLogger;

public interface IMatchWorker : IDisposable
{
    public void AddUser(string id);
    public CompleteMatchData GetCompleteMatchData(string id);
}

public class MatchWorker : IMatchWorker
{
    ConcurrentQueue<string> _reqUserQueue = new ConcurrentQueue<string>();
    ConcurrentDictionary<string, CompleteMatchData> _completeDic = new ConcurrentDictionary<string, CompleteMatchData>();

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

        _isThreadRunning = true;

        _reqThread = new Thread(RunRequestMatching);
        _reqThread.Start();

        _checkThread = new Thread(RunCheckMatching);
        _checkThread.Start();

        var conf = new CloudStructures.RedisConfig("Match", _redisAddress);
        _connection = new CloudStructures.RedisConnection(conf);

        _logger.ZLogInformation($"MatchWorker생성");
    }

    public void Dispose()
    {
        _isThreadRunning = false;
        _connection.GetConnection().Close();
    }

    public void AddUser(string id)
    {
        _reqUserQueue.Enqueue(id);
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
                var req = new RequestMatchData();
                _reqUserQueue.TryDequeue(out string result1);
                req.UserA = result1;
                _reqUserQueue.TryDequeue(out string result2);
                req.UserB = result2;

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
                    res = result.Value;

                    var compA = new CompleteMatchData();
                    compA.ServerAddress = res.ServerAddress;
                    compA.Port = res.Port;
                    compA.RoomNumber = res.RoomNumber;
                    compA.OtherUserId = res.UserB;

                    var compB = new CompleteMatchData();
                    compB.ServerAddress = res.ServerAddress;
                    compB.Port = res.Port;
                    compB.RoomNumber = res.RoomNumber;
                    compB.OtherUserId = res.UserA;

                    _completeDic.TryAdd(res.UserA, compA);
                    _completeDic.TryAdd(res.UserB, compB);
                }
            }
            catch
            {
                _logger.ZLogError($"Redis 매칭 결과 List Pop 문제 발생");
            }
        }
    }
}