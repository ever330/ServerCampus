using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class User
    {
        int _userIndex;
        string _sessionId;
        string _userId;

        public int RoomNumber { get; private set; }

        public void Set(int userIndex, string sessionId, string userId)
        {
            _userIndex = userIndex;
            _sessionId = sessionId;
            _userId = userId;
        }

        public string GetId()
        {
            return _userId;
        }

        public void EnterRoom(int roomNumber)
        {
            RoomNumber = roomNumber;
        }

        public void LeaveRoom()
        {
            RoomNumber = -1;
        }
    }
}
