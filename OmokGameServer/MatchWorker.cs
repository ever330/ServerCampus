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

        string _redisDBConnectionString;
        string _reqListKey;
        string _resListKey;
        int _port;

        public void Init(ILog mainLogger, Action<OmokBinaryRequestInfo> sendToPP, Func<int> getEmptyRoomIndex, string redisDBConStr, string reqListKey, string resListKey, int port)
        {
            _mainLogger = mainLogger;
            _isThreadRunning = true;
            _redisDBConnectionString = redisDBConStr;
            _sendToPP = sendToPP;
            _getEmptyRoomIndex = getEmptyRoomIndex;
            _reqListKey = reqListKey;
            _resListKey = resListKey;
            _port= port;

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

            while (_isThreadRunning)
            {
                var roomNum = _getEmptyRoomIndex();

                if (roomNum == -1)
                {
                    Thread.Sleep(100);
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
                        req = result.Value;
                        RequestUserSetting(req.UserA, req.UserB, roomNum);

                        var res = new ResponseMatchData();
                        res.UserA = req.UserA;
                        res.UserB = res.UserB;

                        var host = Dns.GetHostEntry(Dns.GetHostName());
                        foreach (var ip in host.AddressList)
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork)
                            {
                                res.ServerAddress = ip.ToString();
                            }
                        }
                        res.Port = _port;
                        res.RoomNumber = roomNum;

                        var redisRes = new RedisList<ResponseMatchData>(connection, _resListKey, defaultExpiry);

                        redisRes.RightPushAsync(res);
                    }
                }
                catch (Exception ex)
                {
                    _mainLogger.Error($"MatchWorker Error : {ex.ToString()}");
                }
            }
        }

        void RequestUserSetting(string userA, string userB, int roomNum)
        {
            var pac = new ReqMatchUsersEnterPacket();
            pac.UserA = userA;
            pac.UserB = userB;
            pac.RoomNumber = roomNum;
            var pacData = MemoryPackSerializer.Serialize(pac);
            var binaryData = new OmokBinaryRequestInfo((short)(pacData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PacketId.ReqMatchUsersEnter, pacData);
            _sendToPP(binaryData);
        }
    }
}