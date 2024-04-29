using Microsoft.Extensions.Logging;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmokGameServer
{
    public class PacketProcessor
    {
        bool _isThreadRunning = false;
        Thread _processThread;
        ILog _mainLogger;

        BufferBlock<OmokBinaryRequestInfo> _packetBuffer = new BufferBlock<OmokBinaryRequestInfo>();
        Func<string, byte[], bool> _sendFunc;
        Dictionary<short, Action<OmokBinaryRequestInfo>> _handlerDict = new Dictionary<short, Action<OmokBinaryRequestInfo>>();

        ServerPacketHandler _serverPacketHandler = new ServerPacketHandler();
        CommonPacketHandler _lobbyPacketHandler = new CommonPacketHandler();
        RoomPacketHandler _roomPacketHandler = new RoomPacketHandler();

        UserManager _userManager;
        RoomManager _roomManager;

        public void Init(ILog mainLogger, UserManager userManager, RoomManager roomManager, Func<string, byte[], bool> sendDataFunc)
        {
            _mainLogger = mainLogger;
            _userManager = userManager;
            _roomManager = roomManager;
            _serverPacketHandler.Init(userManager, roomManager, mainLogger, sendDataFunc);
            _lobbyPacketHandler.Init(userManager, roomManager, mainLogger, sendDataFunc);
            _roomPacketHandler.Init(userManager, roomManager, mainLogger, sendDataFunc);
            _isThreadRunning = true;
            _processThread= new Thread(Process);
            _processThread.Start();
        }

        public void Destroy()
        {
            _isThreadRunning = false;
            _packetBuffer.Complete();
        }

        public void RegistHandlers()
        {
            _serverPacketHandler.RegistPacketHandler(_handlerDict);
            _lobbyPacketHandler.RegistPacketHandler(_handlerDict);
            _roomPacketHandler.RegistPacketHandler(_handlerDict);
        }

        public void InsertPacket(OmokBinaryRequestInfo req)
        {
            _packetBuffer.Post(req);
        }

        void Process()
        {
            while (_isThreadRunning)
            {
                try
                {
                    var packet = _packetBuffer.Receive();

                    if (_handlerDict.ContainsKey(packet.PacketId))
                    {
                        _handlerDict[packet.PacketId](packet);
                    }
                    else
                    {
                        _mainLogger.Info($"PacketProcessor Error : 없는 패킷 ID {packet.PacketId}");
                    }
                }
                catch (Exception ex)
                {
                    _mainLogger.Error($"PacketProcessor Error : {ex.ToString()}");
                }
            }
        }
    }
}
