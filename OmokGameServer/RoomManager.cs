using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RoomManager
    {
        List<Room> _roomList = new List<Room>();

        int _roomMaxCount = 0;

        public void Init(int RoomMaxCount)
        {
            _roomMaxCount = RoomMaxCount;
        }
    }
}
