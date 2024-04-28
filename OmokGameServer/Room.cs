using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class Room
    {
        int _roomNumber;
        List<User> _userList = new List<User>();

        int _roomUserMaxCount = 0;


        public Room(int roomNumber, int roomUserMaxCount)
        {
            _roomNumber = roomNumber;
            _roomUserMaxCount = roomUserMaxCount;
        }

        public ERROR_CODE EnterRoom(User user)
        {
            if (_userList.Count >= _roomUserMaxCount) 
            {
                return ERROR_CODE.ROOM_USER_MAX;
            }

            user.EnterRoom(_roomNumber);
            _userList.Add(user);

            return ERROR_CODE.NONE;
        }

        public void LeaveRoom(string sessionId)
        {
            var user = _userList.Find(x => x.SessionId  == sessionId);

            if (user != null)
            {
                _userList.Remove(user);
                user.LeaveRoom();
            }
        }

        public List<User> GetUserList()
        {
            return _userList;
        }
    }
}
