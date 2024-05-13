using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class UserManager
    {
        List<User> _userList = new List<User>();
        int _maxUserCount = 0;
        int _userIndex = 0;
        Func<string, byte[], bool> _sendFunc;
        Action<string> _disconnect;
        Action<DBRequestInfo> _reqToGameDB;
        Action<DBRequestInfo> _reqToRedisDB;
        Queue<string> _removeUserQueue = new Queue<string>();
        int _userConnectCount = 0;

        const int HeartBeatLimitSecond = 5;
        const int SessionConnectLimitMinite = 5;

        public void Init(int maxUserCount, Func<string, byte[], bool> sendFunc, Action<string> disconnect, Action<DBRequestInfo> reqToGameDB, Action<DBRequestInfo> reqToRedisDB)
        {
            _maxUserCount = maxUserCount;
            _sendFunc = sendFunc;
            _disconnect = disconnect;
            _reqToGameDB = reqToGameDB;
            _reqToRedisDB = reqToRedisDB;

            for (int i = 0; i < maxUserCount; i++)
            {
                User user = new User(_userIndex);
                _userIndex++;
                _userList.Add(user);
            }
        }

        public ErrorCode UserLogin(string sessionId, string id, string authToken)
        {
            var user = _userList.Find(x => x.SessionId == sessionId);

            var result = ErrorCode.None;

            if (LoginDuplicationCheck(id))
            {
                result = ErrorCode.UserAlreadyExist;
            }
            else
            {
                user.UserId = id;

                var checkToken = new ReqCheckAuthToken();
                checkToken.UserId = id;
                checkToken.AuthToken = authToken;

                _reqToRedisDB(DBRequest.MakeRequest((short)PacketId.ReqCheckAuthToken, MemoryPackSerializer.Serialize(checkToken)));

                var getUserData = new ReqUserData();
                getUserData.UserId = id;

                _reqToGameDB(DBRequest.MakeRequest((short)PacketId.ReqUserData, MemoryPackSerializer.Serialize(getUserData)));
            }
            return result;
        }

        public ErrorCode SetNewUser(string sessionId)
        {
            if (_userConnectCount >= _maxUserCount)
            {
                return ErrorCode.UserCountMax;
            }

            for (int i = 0; i < _maxUserCount; i++)
            {
                if (_userList[i].SessionId == "")
                {
                    _userList[i].Set(sessionId);
                    break;
                }
            }
            _userConnectCount++;
            return ErrorCode.None;
        }

        public ErrorCode SetUserData(string sessionId, int winCount, int loseCount)
        {
            User? user = _userList.Find(x => x.SessionId == sessionId);
            if (user == null)
            {
                return ErrorCode.UserNotExist;
            }

            user.SetData(winCount, loseCount);

            var res = new ResLoginPacket();
            res.Result = true;
            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PacketId.ResLogin, data);
            _sendFunc(sessionId, sendData);

            return ErrorCode.None;
        }

        public User? GetUserBySessionId(string sessionId)
        {
            User? user = _userList.Find(x => x.SessionId == sessionId);
            return user;
        }

        public User? GetUserByUserId(string userId)
        {
            User? user = _userList.Find(x => x.UserId == userId);
            return user;
        }

        public ErrorCode RemoveUser(string sessionId)
        {
            User? user = _userList.Find(x => x.SessionId == sessionId);
            if (user == null)
            {
                return ErrorCode.UserNotExist;
            }

            user.ResetData();
            _userConnectCount--;

            return ErrorCode.None;
        }

        public int GetUserCount()
        {
            return _userConnectCount;
        }

        bool LoginDuplicationCheck(string userId)
        {
            foreach (var user in _userList)
            {
                if (user.UserId == userId)
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckHeartBeat(int heartBeatIndex)
        {
            for (int i = (heartBeatIndex * _maxUserCount) / 4; i < ((heartBeatIndex + 1) * _maxUserCount) / 4; i++)
            {
                if (_userList[i].UserId == "")
                {
                    continue;
                }

                var userHeartBeat = _userList[i].HeartBeatTime;
                var dif = DateTime.Now - userHeartBeat;
                if (dif.TotalSeconds >= HeartBeatLimitSecond)
                {
                    _disconnect(_userList[i].SessionId);
                    RemoveUser(_userList[i].SessionId);
                }
            }
        }

        public void CheckSession(int checkSessionIndex)
        {
            for (int i = (checkSessionIndex * _maxUserCount) / 4; i < ((checkSessionIndex + 1) * _maxUserCount) / 4; i++)
            {
                if (_userList[i].UserId == "" && _userList[i].SessionId != "")
                {
                    var dif = DateTime.Now - _userList[i].ConnectTime;
                    if (dif.TotalMinutes >= SessionConnectLimitMinite)
                    {
                        var packet = new NtfSessionTimeLimitPacket();
                        var ntf = MemoryPackSerializer.Serialize(packet);
                        var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfSessionTimeLimit, ntf);
                        _sendFunc(_userList[i].SessionId, ntfData);
                        _disconnect(_userList[i].SessionId);
                        RemoveUser(_userList[i].SessionId);
                    }
                }
            }
        }
    }
}
