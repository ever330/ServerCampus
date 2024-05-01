using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public enum STONE { NONE, BLACK, WHITE };
    public enum USER_STATE { NONE, READY, PLAYING }

    public class User
    {
        int _userIndex;
        public string SessionId { get; private set; }
        public string UserId { get; private set; }
        public int RoomNumber { get; private set; }
        public USER_STATE State { get; set; }
        public STONE Stone { get; set; }
        public DateTime HeartBeatTime { get; set; }
        public UserGameData GameData { get; set; }
        public int TimeOutCount { get; set; }

        public void Set(int userIndex, string sessionId, string userId, UserGameData gameData)
        {
            _userIndex = userIndex;
            SessionId = sessionId;
            UserId = userId;
            State = USER_STATE.NONE;
            Stone = STONE.NONE;
            HeartBeatTime = DateTime.Now;
            GameData = gameData;
            TimeOutCount = 0;
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
