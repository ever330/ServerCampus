using MemoryPack;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class MySqlHandler : DBHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<DBRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.REQ_UPDATE_RESULT, UpdateGameResult);
        }

        public void UpdateGameResult(DBRequestInfo req)
        {
            var gameResult = MemoryPackSerializer.Deserialize<ReqUpdateWinLose>(req.Body);

            var user = _userManager.GetUser(req.SessionId);
            if (gameResult.Result)
            {
                user.GameData.WinCount++;
            }
            else
            {
                user.GameData.LoseCount++;
            }
            var updateResult = _dbManager.UpdateGameResult(gameResult.UserId, user.GameData.WinCount, user.GameData.LoseCount);

            if (updateResult != ERROR_CODE.NONE)
            {
                _logger.Error($"{gameResult.UserId} 게임 결과 업데이트 에러 : {updateResult}");
            }
        }
    }
}
