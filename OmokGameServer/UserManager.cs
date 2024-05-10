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
        //Dictionary<string, User> _userDict = new Dictionary<string, User>();
        List<User> _userList = new List<User>();
        int _maxUserCount = 0;
        int _userIndex = 0;
        Func<string, byte[], bool> _sendFunc;
        Action<string> _disconnect;
        DBManager _dbManager;
        Queue<string> _removeUserQueue = new Queue<string>();
        int _userConnectCount = 0;

        const int HeartBeatLimitSecond = 5;
        const int SessionConnectLimitMinite = 5;

        public void Init(DBManager dbManager, int maxUserCount, Func<string, byte[], bool> sendFunc, Action<string> disconnect)
        {
            _dbManager = dbManager;
            _maxUserCount = maxUserCount;
            _sendFunc = sendFunc;
            _disconnect = disconnect;

            for (int i = 0; i < maxUserCount; i++)
            {
                User user = new User(_userIndex);
                _userIndex++;
                _userList.Add(user);
            }
        }

        public ERROR_CODE SetNewUser(string sessionId)
        {
            if (_userConnectCount >= _maxUserCount)
            {
                return ERROR_CODE.USER_COUNT_MAX;
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
            return ERROR_CODE.NONE;
        }

        public ERROR_CODE SetUserData(string userId, string sessionId, int winCount, int loseCount)
        {
            User? user = _userList.Find(x => x.SessionId == sessionId);
            if (user == null)
            {
                return ERROR_CODE.USER_NOT_EXIST;
            }

            user.SetData(userId, winCount, loseCount);

            var res = new ResLoginPacket();
            res.Result = true;
            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LOGIN, data);
            _sendFunc(sessionId, sendData);

            return ERROR_CODE.NONE;
        }

        public User? GetUser(string sessionId)
        {
            User? user = _userList.Find(x => x.SessionId == sessionId);
            return user;
        }

        public ERROR_CODE RemoveUser(string sessionId)
        {
            User? user = _userList.Find(x => x.SessionId == sessionId);
            if (user == null)
            {
                return ERROR_CODE.USER_NOT_EXIST;
            }

            user.ResetData();
            _userConnectCount--;

            return ERROR_CODE.NONE;
        }

        public int GetUserCount()
        {
            return _userConnectCount;
        }

        public ERROR_CODE UserLogin(string sessionId, string userId, int winCount, int loseCount)
        {
            var result = ERROR_CODE.NONE;

            if (LoginDuplicationCheck(userId))
            {
                result = ERROR_CODE.USER_ALREADY_EXIST;
            }
            else
            {
                result = SetUserData(userId, sessionId, winCount, loseCount);
            }
            return result;
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
            for (int i = heartBeatIndex * (_maxUserCount / 4); i < (heartBeatIndex + 1) * (_maxUserCount / 4); i++)
            {
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
            for (int i = checkSessionIndex * (_maxUserCount / 4); i < (checkSessionIndex + 1) * (_maxUserCount / 4); i++)
            {
                if (_userList[i].UserId == "")
                {
                    var dif = DateTime.Now - _userList[i].ConnectTime;
                    if (dif.TotalMinutes >= SessionConnectLimitMinite)
                    {
                        var packet = new NtfSessionTimeLimitPacket();
                        var ntf = MemoryPackSerializer.Serialize(packet);
                        var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_SESSION_TIME_LIMIT, ntf);
                        _sendFunc(_userList[i].SessionId, ntfData);
                        _disconnect(_userList[i].SessionId);
                        RemoveUser(_userList[i].SessionId);
                    }
                }
            }
        }
    }
}
