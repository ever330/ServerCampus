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

        public int EnterRoom(User user)
        {
            if (_roomList[_roomEnterIndex].EnterRoom(user) != ERROR_CODE.NONE)
            {
                _roomEnterIndex++;
                _roomList[_roomEnterIndex].EnterRoom(user);
            }
            return _roomEnterIndex;
        }

        public void BroadCast(int roomNumber, string userId, string chat)
        {
            Room tempRoom = _roomList[_roomEnterIndex];
            List<User> tempUserList = tempRoom.GetUserList();

            NtfChatPacket chatPacket = new NtfChatPacket();
            chatPacket.Id = userId;
            chatPacket.Chat = chat;

            byte[] data = MemoryPackSerializer.Serialize(chatPacket);
            short packetId = (short)PACKET_ID.NTF_ROOM_CHAT;
            short packetSize = (short)(PacketDefine.PACKET_HEADER + data.Length);

            byte[] sendData = new byte[packetSize];
            Array.Copy(BitConverter.GetBytes(packetSize), 0, sendData, 0, 2);
            Array.Copy(BitConverter.GetBytes(packetId), 0, sendData, 2, 2);
            Array.Copy(data, 0, sendData, 4, data.Length);

            for (int i = 0; i < tempUserList.Count; i++)
            {
                _sendFunc(tempUserList[i].SessionId, sendData);
            }
        }
    }
}
