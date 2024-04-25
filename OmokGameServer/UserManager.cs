using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class UserManager
    {
        Dictionary<string, User> _userDict = new Dictionary<string, User>();
        int _maxUserCount = 0;
        int _userIndex = 0;

        public void Init(int maxUserCount)
        {
            _maxUserCount = maxUserCount;
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
            //User user = _userDict.Find(x => x.SessionId == sessionId);
            //if (user == null)
            //{
            //    return ERROR_CODE.USER_NOT_EXIST;
            //}
            //_userDict.Remove(user);

            return ERROR_CODE.NONE;
        }
    }
}
