using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class UserManager
    {
        int _maxUserCount;
        UInt64 UserSequenceNumber = 0;

        Dictionary<string, User> UserMap = new Dictionary<string, User>();

        public void Init(int maxUserCount)
        {
            _maxUserCount = maxUserCount;
        }

        public ERROR_CODE AddUser(string userId, string sessionId)
        {
            if (IsFullUserCount())
            {
                return ERROR_CODE.LOGIN_FULL_USER_COUNT;
            }

            if (UserMap.ContainsKey(sessionId))
            {
                return ERROR_CODE.ADD_USER_DUPLICATION;
            }

            var user = new User();
            user.Set(++UserSequenceNumber, sessionId, userId);
            UserMap.Add(sessionId, user);

            return ERROR_CODE.NONE;
        }

        public ERROR_CODE RemoveUser(string sessionId)
        {
            if (UserMap.Remove(sessionId) == false)
            {
                return ERROR_CODE.REMOVE_USER_SEARCH_FAILURE_USER_ID;
            }

            return ERROR_CODE.NONE;
        }

        public User GetUser(string sessionID)
        {
            User user = null;
            UserMap.TryGetValue(sessionID, out user);
            return user;
        }

        bool IsFullUserCount()
        {
            return _maxUserCount <= UserMap.Count();
        }
    }

    public class User
    {
        UInt64 _sequenceNumber = 0;
        string _sessionId;

        public int RoomNumber { get; private set; } = -1;
        string _userId;

        public void Set(UInt64 sequence, string sessionId, string userId)
        {
            _sequenceNumber = sequence;
            _sessionId = sessionId;
            _userId = userId;
        }

        public bool IsConfirm(string netSessionId)
        {
            return _sessionId == netSessionId;
        }

        public string ID()
        {
            return _userId;
        }

        public void EnteredRoom(int roomNumber)
        {
            RoomNumber = roomNumber;
        }

        public void LeaveRoom()
        {
            RoomNumber = -1;
        }

        public bool IsStateLogin() { return _sequenceNumber != 0; }

        public bool IsStateRoom() { return RoomNumber != -1; }
    }
}
