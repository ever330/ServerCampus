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
            packetHandlers.Add((short)PacketId.SessionConnect, NtfSessionConnected);
            packetHandlers.Add((short)PacketId.SessionDisconnect, NtfSessionDisconnected);
        }

        public void NtfSessionConnected(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 접속");
            _userManager.SetNewUser(packet.SessionId);
        }

        public void NtfSessionDisconnected(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 접속 종료");

            var user = _userManager.GetUserBySessionId(packet.SessionId);
            if (user != null )
            {
                var result =  _userManager.RemoveUser(packet.SessionId);

                if (result != ErrorCode.None)
                {
                    _logger.Error($"{packet.SessionId} 접속 종료 에러");
                }

                _logger.Info($"현재 유저 수 : {_userManager.GetUserCount()}");
            }
        }
    }
}
