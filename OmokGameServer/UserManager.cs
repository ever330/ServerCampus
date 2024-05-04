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
        Dictionary<string, User> _userDict = new Dictionary<string, User>();
        int _maxUserCount = 0;
        int _userIndex = 0;
        Func<string, byte[], bool> _sendFunc;
        Action<string> _disconnect;
        DBManager _dbManager;
        Queue<string> _removeUserQueue = new Queue<string>();

        const int HeartBeatLimitSecond = 5;
        const int SessionConnectLimitMinite = 5;

        public void Init(DBManager dbManager, int maxUserCount, Func<string, byte[], bool> sendFunc, Action<string> disconnect)
        {
            _dbManager = dbManager;
            _maxUserCount = maxUserCount;
            _sendFunc = sendFunc;
            _disconnect = disconnect;
        }

        public ERROR_CODE SetNewUser(string sessionId)
        {
            if (_userDict.Count >= _maxUserCount)
            {
                return ERROR_CODE.USER_COUNT_MAX;
            }

            User user = new User();
            user.Set(_userIndex, sessionId);
            _userIndex++;
            _userDict.Add(sessionId, user);

            return ERROR_CODE.NONE;
        }

        public ERROR_CODE SetUserData(string userId, string sessionId, int winCount, int loseCount)
        {
            if (!_userDict.ContainsKey(sessionId))
            {
                return ERROR_CODE.USER_NOT_EXIST;
            }

            _userDict[sessionId].SetData(userId, winCount, loseCount);

            var res = new ResLoginPacket();
            res.Result = true;
            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LOGIN, data);
            _sendFunc(sessionId, sendData);

            return ERROR_CODE.NONE;
        }

        public User GetUser(string sessionId)
        {
            User user = null;
            _userDict.TryGetValue(sessionId, out user);
            return user;
        }

        public ERROR_CODE RemoveUser(string sessionId)
        {
            if (!_userDict.ContainsKey(sessionId))
            {
                return ERROR_CODE.USER_NOT_EXIST;
            }

            _userDict.Remove(sessionId);

            return ERROR_CODE.NONE;
        }

        public int GetUserCount()
        {
            return _userDict.Count;
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
            foreach (var user in _userDict.Values)
            {
                if (user.UserId == userId)
                {
                    return true;
                }
            }
            return false;
        }

        public Dictionary<string, User> GetUsers()
        {
            return _userDict;
        }

        public void CheckHeartBeat(int heartBeatIndex)
        {
            int counter = 0;
            foreach (var user in _userDict)
            {
                if (counter >= heartBeatIndex * ((float)_maxUserCount / 4) && counter < (heartBeatIndex + 1) * ((float)_maxUserCount / 4))
                {
                    var userHeartBeat = user.Value.HeartBeatTime;
                    var dif = DateTime.Now - userHeartBeat;
                    if (dif.TotalSeconds >= HeartBeatLimitSecond)
                    {
                        _removeUserQueue.Enqueue(user.Key);
                        continue;
                    }
                    var packet = new ReqHeartBeatPacket();
                    var req = MemoryPackSerializer.Serialize(packet);
                    var reqData = ClientPacket.MakeClientPacket(PACKET_ID.REQ_HEART_BEAT, req);
                    _sendFunc(user.Key, reqData);
                }
                counter++;
            }
        }

        public void CheckSession(int checkSessionIndex)
        {
            int counter = 0;
            foreach (var user in _userDict)
            {
                if (user.Value.UserId != "" && counter >= checkSessionIndex * ((float)_maxUserCount / 4) && counter < (checkSessionIndex + 1) * ((float)_maxUserCount / 4))
                {
                    var dif = DateTime.Now - user.Value.ConnectTime;
                    if (dif.TotalMinutes >= SessionConnectLimitMinite)
                    {
                        var packet = new NtfSessionTimeLimitPacket();
                        var ntf = MemoryPackSerializer.Serialize(packet);
                        var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_SESSION_TIME_LIMIT, ntf);
                        _sendFunc(user.Key, ntfData);
                        RemoveUser(user.Key);
                        _disconnect(user.Key);
                    }
                }
                counter++;
            }
        }
    }
}
