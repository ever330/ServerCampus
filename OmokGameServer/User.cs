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
        public string SessionId { get; private set; }
        public string UserId { get; private set; }
        public int RoomNumber { get; private set; }

        public void Set(int userIndex, string sessionId, string userId)
        {
            _userIndex = userIndex;
            SessionId = sessionId;
            UserId = userId;
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
