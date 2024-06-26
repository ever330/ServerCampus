﻿using CloudStructures;
using CloudStructures.Structures;
using MemoryPack;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmokGameServer
{
    public class MatchWorker
    {
        bool _isThreadRunning = false;
        Thread _matchThread;
        ILog _mainLogger;

        Action<OmokBinaryRequestInfo> _sendToPP;
        Func<int> _getEmptyRoomIndex;
        Func<bool> _checkEmptyRoom;

        string _redisDBConnectionString;
        string _reqListKey;
        string _resListKey;
        int _port;

        string _address;

        public void Init(ILog mainLogger, Action<OmokBinaryRequestInfo> sendToPP, Func<int> getEmptyRoomIndex, Func<bool> checkEmptyRoom, string redisDBConStr, string reqListKey, string resListKey, int port)
        {
            _mainLogger = mainLogger;
            _isThreadRunning = true;
            _redisDBConnectionString = redisDBConStr;
            _sendToPP = sendToPP;
            _getEmptyRoomIndex = getEmptyRoomIndex;
            _checkEmptyRoom = checkEmptyRoom;
            _reqListKey = reqListKey;
            _resListKey = resListKey;
            _port= port;

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    _address = ip.ToString();
                }
            }

            _matchThread = new Thread(Process);
            _matchThread.Start();

        }

        public void Destroy()
        {
            _isThreadRunning = false;
        }

        void Process()
        {
            var conf = new RedisConfig("Match", _redisDBConnectionString);
            var connection = new RedisConnection(conf);
            _mainLogger.Info("매칭 스레드 시작");

            while (_isThreadRunning)
            {
                var checkResult = _checkEmptyRoom();

                if (!checkResult)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                try
                {
                    if (!connection.GetConnection().IsConnected)
                    {
                        connection = new RedisConnection(conf);
                        _mainLogger.Info("RedisConnection 연결 종료로 인한 재생성");
                    }

                    var req = new RequestMatchData();

                    var defaultExpiry = TimeSpan.FromDays(1);
                    var redisReq = new RedisList<RequestMatchData>(connection, _reqListKey, defaultExpiry);

                    var result = redisReq.LeftPopAsync().Result;
                    if (result.HasValue)
                    {
                        _mainLogger.Info("매칭요청 데이터 가져옴");
                        req = result.Value;

                        var res = new ResponseMatchData();
                        res.UserA = req.UserA;
                        res.UserB = req.UserB;
                        _mainLogger.Info($"매칭유저 {req.UserA}, {req.UserB}");

                        res.ServerAddress = _address;
                        res.Port = _port;
                        res.RoomNumber = _getEmptyRoomIndex();

                        var redisRes = new RedisList<ResponseMatchData>(connection, _resListKey, defaultExpiry);

                        redisRes.RightPushAsync(res);
                        _mainLogger.Info("매칭결과 푸시");
                    }
                }
                catch (Exception ex)
                {
                    _mainLogger.Error($"MatchWorker Error : {ex.ToString()}");
                }
            }
        }
    }
}
