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

        public void Init(int maxUserCount, Func<string, byte[], bool> sendFunc)
        {
            _maxUserCount = maxUserCount;
            _sendFunc = sendFunc;
        }

        public ERROR_CODE AddUser(string userId, string sessionId)
        {
            if (_userDict.Count >= _maxUserCount)
            {
                return ERROR_CODE.USER_COUNT_MAX;
            }

            User user = new User();
            user.Set(_userIndex, sessionId, userId);

            _userDict.Add(sessionId, user);

            _userIndex++; 

            ResLoginPacket res = new ResLoginPacket();
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
    }
}
