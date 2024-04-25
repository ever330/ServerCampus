using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace ChatServer
{
    public class Room
    {
        public int Index { get; private set; }
        public int Number { get; private set; }

        int MaxUserCount = 0;

        List<RoomUser> UserList = new List<RoomUser>();

        public static Func<string, byte[], bool> NetSendFunc;

        public void Init(int index, int number, int maxUserCount)
        {
            Index = index;
            Number = number;
            MaxUserCount = maxUserCount;
        }

        public RoomUser GetUserId(string userId)
        {
            return UserList.Find(x => x.UserId == userId);
        }

        public bool AddUser(string userId, string netSessionId)
        {
            if (GetUserId(userId) != null)
            {
                return false;
            }

            var newUser = new RoomUser();
            newUser.Set(userId, netSessionId);
            UserList.Add(newUser);

            return true;
        }

        public void RemoveUser(string netSessionId)
        {
            var index = UserList.FindIndex(x => x.NetSessionId == netSessionId);
            UserList.RemoveAt(index);
        }

        public bool RemoveUser(RoomUser user)
        {
            return UserList.Remove(user);
        }

        public RoomUser GetUserByNetSessionId(string netSessionId)
        {
            return UserList.Find(x => x.NetSessionId == netSessionId);
        }

        public int CurrentRoomUsers()
        {
            return UserList.Count;
        }

        public void NotifyRoomUsers(string netSessionId)
        {
            var packet = new NtfRoomUserListPacket();
            foreach (var user in UserList)
            {
                packet.UserIdList.Add(user.UserId);
            }

            var body = MemoryPackSerializer.Serialize(packet);
            var sendPacket = PacketSerializer.Serialize(PACKET_ID.NTF_ROOM_USER_LIST, body);

            NetSendFunc(netSessionId, sendPacket);
        }

        public void NofifyNewUser(string newNetSessionId, string newUserId)
        {
            var packet = new NtfRoomNewUserPacket();
            packet.UserId = newUserId;

            var body = MemoryPackSerializer.Serialize(packet);
            var sendPacket = PacketSerializer.Serialize(PACKET_ID.NTF_ROOM_NEW_USER, body);

            Broadcast(newNetSessionId, sendPacket);
        }

        public void NotifyLeaveUser(string userId)
        {
            if (CurrentRoomUsers() == 0)
            {
                return;
            }

            var packet = new NtfRoomLeaveUserPacket();
            packet.UserId = userId;

            var body = MemoryPackSerializer.Serialize(packet);
            var sendPacket = PacketSerializer.Serialize(PACKET_ID.NTF_ROOM_LEAVE_USER, body);

            Broadcast("", sendPacket);
        }

        public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
        {
            foreach (var user in UserList)
            {
                if (user.NetSessionId == excludeNetSessionID)
                {
                    continue;
                }

                NetSendFunc(user.NetSessionId, sendPacket);
            }
        }
    }

    public class RoomUser
    {
        public string UserId { get; private set; }
        public string NetSessionId { get; private set; }

        public void Set(string userId, string netSessionId)
        {
            UserId = userId;
            NetSessionId = netSessionId;
        }
    }
}
