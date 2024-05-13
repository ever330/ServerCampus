using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;
using SqlKata.Execution;
using SuperSocket.SocketBase.Logging;

namespace OmokGameServer
{
    public class GameDBHandler
    {
        GameDB _gameDB;
        ILog _logger;
        Action<OmokBinaryRequestInfo> _sendToPP;

        public void Init(GameDB gameDB, ILog logger, Func<string, byte[], bool> sendFunc, Action<OmokBinaryRequestInfo> sendToPP)
        {
            _gameDB = gameDB;
            _logger = logger;
            _sendToPP = sendToPP;
        }

        public void RegistPacketHandler(Dictionary<short, Action<QueryFactory, DBRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PacketId.ReqUpdateResult, UpdateGameResult);
            packetHandlers.Add((short)PacketId.ReqUserData, GetUserData);
        }

        public void UpdateGameResult(QueryFactory queryFactory, DBRequestInfo req)
        {
            var gameResult = MemoryPackSerializer.Deserialize<ReqUpdateWinLose>(req.Body);

            var updateResult = _gameDB.UpdateGameResult(queryFactory, gameResult.UserId, gameResult.WinCount, gameResult.LoseCount);

            if (updateResult != ErrorCode.None)
            {
                _logger.Error($"{gameResult.UserId} 게임 결과 업데이트 에러 : {updateResult}");
            }
        }

        public void GetUserData(QueryFactory queryFactory, DBRequestInfo req)
        {
            var get = MemoryPackSerializer.Deserialize<ReqUserData>(req.Body);
            var userData = _gameDB.GetUserData(queryFactory, get.UserId);

            var res = new ResUserData();
            res.UserId = get.UserId;
            if (userData.Item1 != ErrorCode.None || userData.Item2 == null)
            {
                res.Result = false;
            }
            else
            {
                res.Result = true;
                res.WinCount = userData.Item2.WinCount;
                res.LoseCount = userData.Item2.LoseCount;
            }

            var resData = MemoryPackSerializer.Serialize(res);
            var reqInfo = new OmokBinaryRequestInfo((short)(resData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PacketId.ResUserData, resData);
            _sendToPP(reqInfo);
        }
    }
}
