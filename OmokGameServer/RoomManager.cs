using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RoomManager
    {
        List<Room> _roomList = new List<Room>();
        protected Func<string, byte[], bool> _sendFunc;

        int _roomMaxCount = 0;
        int _roomEnterIndex = 0;

        public void Init(int roomMaxCount, int roomUserMax, Func<string, byte[], bool> sendFunc)
        {
            _roomMaxCount = roomMaxCount;
            for (int i = 0; i < _roomMaxCount; i++)
            {
                Room room = new Room(i, roomUserMax);
                _roomList.Add(room);
            }
            _sendFunc = sendFunc;
        }

        public bool EnterRoom(User user)
        {
            if (_roomList[_roomEnterIndex].EnterRoom(user) != ERROR_CODE.NONE)
            {
                _roomEnterIndex++;
                _roomList[_roomEnterIndex].EnterRoom(user);
            }

            ResEnterRoomPacket res = new ResEnterRoomPacket();
            res.RoomNumber = _roomEnterIndex;

            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_ENTER_ROOM, data);
            return _sendFunc(user.SessionId, sendData);
        }

        public bool LeaveRoom(User user, int roomNumber)
        {
            var result = _roomList[roomNumber].LeaveRoom(user.SessionId);

            var res = new ResLeaveRoomPacket();

            if (result != ERROR_CODE.NONE)
            {
                res.Result = false;
                return false;
            }
            res.Result = true;

            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LEAVE_ROOM, data);
            _sendFunc(user.SessionId, sendData);

            var ntfLeave = new NtfLeaveRoomPacket();
            ntfLeave.RoomNumber = roomNumber;
            ntfLeave.Id = user.UserId;
            var ntf = MemoryPackSerializer.Serialize(ntfLeave);
            var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_LEAVE_USER, ntf);
            _sendFunc(user.SessionId, ntfData);

            return true;
        }

        public void BroadCastChat(int roomNumber, string userId, string chat)
        {
            Room tempRoom = _roomList[roomNumber];
            List<User> tempUserList = tempRoom.GetUserList();

            NtfChatPacket chatPacket = new NtfChatPacket();
            chatPacket.Id = userId;
            chatPacket.Chat = chat;

            var data = MemoryPackSerializer.Serialize(chatPacket);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_ROOM_CHAT, data);

            for (int i = 0; i < tempUserList.Count; i++)
            {
                _sendFunc(tempUserList[i].SessionId, sendData);
            }
        }
    }
}
