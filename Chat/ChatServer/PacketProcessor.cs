using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ChatServer
{
    public class PacketProcessor
    {
        bool _isThreadRunning = false;
        Thread ProcessThread = null;

        BufferBlock<ServerPacketData> _messageBuffer = new BufferBlock<ServerPacketData>();

        UserManager _userManager = new UserManager();

        Tuple<int, int> _roomNumberRange = new Tuple<int, int>(-1, -1);
        List<Room> _roomList = new List<Room>();

        Dictionary<int, Action<ServerPacketData>> _packetHandlerMap = new Dictionary<int, Action<ServerPacketData>>();
        CommomPacketHandler _commonPacketHandler = new CommomPacketHandler();
        RoomPacketHandler _roomPacketHandler = new RoomPacketHandler();


        public void CreateAndStart(List<Room> roomList, MainServer mainServer)
        {
            var maxUserCount = MainServer.ServerOption.RoomMaxCount * MainServer.ServerOption.RoomMaxUserCount;
            _userManager.Init(maxUserCount);

            _roomList = roomList;
            var minRoomNum = _roomList[0].Number;
            var maxRoomNum = _roomList[0].Number + _roomList.Count() - 1;
            _roomNumberRange = new Tuple<int, int>(minRoomNum, maxRoomNum);

            RegistPacketHandler(mainServer);

            _isThreadRunning = true;
            ProcessThread = new System.Threading.Thread(this.Process);
            ProcessThread.Start();
        }

        public void Destroy()
        {
            _isThreadRunning = false;
            _messageBuffer.Complete();
        }

        public void InsertPacket(ServerPacketData data)
        {
            _messageBuffer.Post(data);
        }


        void RegistPacketHandler(MainServer serverNetwork)
        {
            _commonPacketHandler.Init(serverNetwork, _userManager);
            _commonPacketHandler.RegistPacketHandler(_packetHandlerMap);

            _roomPacketHandler.Init(serverNetwork, _userManager);
            _roomPacketHandler.SetRooomList(_roomList);
            _roomPacketHandler.RegistPacketHandler(_packetHandlerMap);
        }

        void Process()
        {
            while (_isThreadRunning)
            {
                //System.Threading.Thread.Sleep(64); //테스트 용
                try
                {
                    var packet = _messageBuffer.Receive();

                    if (_packetHandlerMap.ContainsKey(packet.PacketId))
                    {
                        _packetHandlerMap[packet.PacketId](packet);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("세션 번호 {0}, PacketId {1}, 받은 데이터 크기: {2}", packet.SessionId, packet.PacketId, packet.Body.Length);
                    }
                }
                catch (Exception ex)
                {
                    if (_isThreadRunning)
                    {
                        MainServer.MainLogger.Error(ex.ToString());
                    }
                }
            }
        }
    }
}
