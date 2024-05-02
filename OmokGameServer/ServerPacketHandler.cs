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
            _userManager.SetNewUser(packet.SessionId);
        }

        public void NtfSessionDisconnected(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 접속 종료");

            var user = _userManager.GetUser(packet.SessionId);
            if (user != null )
            {
                var result =  _userManager.RemoveUser(packet.SessionId);

                if (result != ERROR_CODE.NONE)
                {
                    _logger.Error($"{packet.SessionId} 접속 종료 에러");
                }

                _logger.Info($"현재 유저 수 : {_userManager.GetUserCount()}");
            }
        }
    }
}
