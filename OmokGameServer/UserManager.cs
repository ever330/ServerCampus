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
        protected Func<string, byte[], bool> _sendFunc;
        DBManager _dbManager;

        public void Init(DBManager dbManager, int maxUserCount, Func<string, byte[], bool> sendFunc)
        {
            _dbManager = dbManager;
            _maxUserCount = maxUserCount;
            _sendFunc = sendFunc;
        }

        public async Task<ERROR_CODE> AddUser(string userId, string sessionId)
        {
            if (_userDict.Count >= _maxUserCount)
            {
                return ERROR_CODE.USER_COUNT_MAX;
            }

            var userData = _dbManager.GetUserData(userId);

            if (userData.Item1 != ERROR_CODE.NONE)
            {
                return userData.Item1;
            }

            User user = new User();
            user.Set(_userIndex, sessionId, userId, userData.Item2);

            _userDict.Add(sessionId, user);

            _userIndex++; 

            var res = new ResLoginPacket();
            res.Result = true;
            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LOGIN, data);
            bool sendResult = _sendFunc(sessionId, sendData);

            if (!sendResult)
            {
                return ERROR_CODE.NONE;
            }

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

        public ERROR_CODE UserLogin(string sessionId, string userId, string authToken)
        {
            var user = GetUser(sessionId);
            var result = ERROR_CODE.NONE;

            if (user != null)
            {
                result = ERROR_CODE.USER_ALREADY_EXIST;
                return result;
            }
            else
            {
                // 웹 API서버와 연동 후에는 토큰 세팅을 옮겨야함.
                var setResult = _dbManager.SetAuthToken(userId, authToken);
                var checkResult = _dbManager.CheckAuthToken(userId, authToken);

                if (setResult == ERROR_CODE.NONE && checkResult == ERROR_CODE.NONE)
                {
                    result = AddUser(userId, sessionId).Result;
                    return result;
                }
                else if (setResult != ERROR_CODE.NONE)
                {
                    return setResult;
                }
                else
                {
                    return checkResult;
                }
            }
        }

        public Dictionary<string, User> GetUsers()
        {
            return _userDict;
        }
    }
}
