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
            packetHandlers.Add((short)PACKET_ID.REQ_USER_DATA, GetUserData);
        }

        public void UpdateGameResult(DBRequestInfo req)
        {
            var gameResult = MemoryPackSerializer.Deserialize<ReqUpdateWinLose>(req.Body);

            var user = _userManager.GetUser(req.SessionId);
            if (gameResult.Result)
            {
                user.WinCount++;
            }
            else
            {
                user.LoseCount++;
            }
            var updateResult = _dbManager.UpdateGameResult(gameResult.UserId, user.WinCount, user.LoseCount);

            if (updateResult != ERROR_CODE.NONE)
            {
                _logger.Error($"{gameResult.UserId} 게임 결과 업데이트 에러 : {updateResult}");
            }
        }

        public void GetUserData(DBRequestInfo req)
        {
            var get = MemoryPackSerializer.Deserialize<ReqUserData>(req.Body);
            var userData = _dbManager.GetUserData(get.UserId);

            var res = new ResUserData();
            res.UserId = get.UserId;
            if (userData.Item1 == ERROR_CODE.NONE)
            {
                res.Result = true;
                res.WinCount = userData.Item2.WinCount;
                res.LoseCount = userData.Item2.LoseCount;
            }
            else
            {
                res.Result = false;
            }

            var resData = MemoryPackSerializer.Serialize(res);
            var reqInfo = new OmokBinaryRequestInfo((short)(resData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PACKET_ID.RES_USER_DATA, resData);
            reqInfo.SessionId = req.SessionId;
            _sendToPP(reqInfo);
        }
    }
}
