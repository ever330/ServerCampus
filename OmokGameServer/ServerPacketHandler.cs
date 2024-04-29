using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace OmokGameServer
{
    public class ServerPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<OmokBinaryRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.SESSION_CONNECT, NtfSessionConnected);
            packetHandlers.Add((short)PACKET_ID.SESSION_DISCONNECT, NtfSessionDisconnected);
        }

        public void NtfSessionConnected(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 접속");
        }

        public void NtfSessionDisconnected(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 접속 종료");
        }
    }
}
