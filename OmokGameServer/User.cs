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
        public string UserId { get; set; }
        public int RoomNumber { get; private set; }
        public DateTime ConnectTime { get; set; }
        public DateTime HeartBeatTime { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }

        public User(int userIndex)
        {
            _userIndex = userIndex;
            SessionId = "";
            UserId = "";
            RoomNumber = -1;
        }

        public void Set(string sessionId)
        {
            SessionId = sessionId;
            ConnectTime = DateTime.Now;
            HeartBeatTime = DateTime.Now;
        }

        public void SetData(int level, int exp, int winCount, int loseCount)
        {
            Level = level;
            Exp = exp;
            WinCount = winCount;
            LoseCount = loseCount;
        }

        public void ResetData()
        {
            SessionId = "";
            UserId = "";
            RoomNumber = -1;
            Level = 0;
            Exp = 0;
            WinCount = 0;
            LoseCount = 0;
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
