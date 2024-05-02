using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public enum ROOM_STATE
    {
        NONE,
        PLAYING
    }

    public enum PUT_RESULT
    {
        NONE,
        WIN,
        ERROR
    }

    public enum LINE_STATE
    {
        OPEN,
        CLOSE,
        HALF
    }

    public class Room
    {
        int _roomNumber;
        List<User> _userList = new List<User>();

        int _roomUserMaxCount = 0;

        OmokData _omokData = new OmokData();

        public ROOM_STATE RoomState { get; set; }

        public Room(int roomNumber, int roomUserMaxCount)
        {
            _roomNumber = roomNumber;
            _roomUserMaxCount = roomUserMaxCount;
            RoomState = ROOM_STATE.NONE;
        }

        public void BoardClear()
        {
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    _omokData.Board[x, y] = STONE.NONE;
                }
            }
        }

        public ERROR_CODE EnterRoom(User user)
        {
            if (_userList.Count >= _roomUserMaxCount) 
            {
                return ERROR_CODE.ROOM_USER_MAX;
            }

            user.EnterRoom(_roomNumber);

            _userList.Add(user);

            return ERROR_CODE.NONE;
        }

        public ERROR_CODE LeaveRoom(string sessionId)
        {
            var user = _userList.Find(x => x.SessionId  == sessionId);

            if (user != null)
            {
                _userList.Remove(user);
                user.LeaveRoom();

                return ERROR_CODE.NONE;
            }

            return ERROR_CODE.ROOM_LEAVE_ERROR;
        }

        public List<User> GetUserList()
        {
            return _userList;
        }

        public void CheckRoomState()
        {
            bool isAllReady = true;

            for (int i = 0; i < _userList.Count; i++)
            {
                if (_userList[i].State != USER_STATE.READY)
                {
                    isAllReady = false;
                }
            }

            if (isAllReady)
            {
                RoomState = ROOM_STATE.PLAYING;
            }
        }

        public string GameStart()
        {
            _omokData.Init();
            _userList[0].Stone = STONE.BLACK;
            _userList[0].State = USER_STATE.PLAYING;
            _userList[1].Stone = STONE.WHITE;
            _userList[1].State = USER_STATE.PLAYING;

            return _userList[0].UserId;
        }

        public void PutStone(STONE stone, int x, int y)
        {
            _omokData.Board[x, y] = stone;
        }

        public PUT_RESULT CheckStoneCount(STONE stone, int row, int col)
        {
            var colCount = CheckLine(row, col, 0, 1, stone);
            var rowCount = CheckLine(row, col, 1, 0, stone);
            var diaCount1 = CheckLine(row, col, 1, 1, stone);
            var diaCount2 = CheckLine(row, col, 1, -1, stone);

            if (stone == STONE.WHITE && (colCount >= 5 || rowCount >= 5 || diaCount1 >= 5 || diaCount2 >= 5))
            {
                return PUT_RESULT.WIN;
            }

            if (stone == STONE.BLACK)
            {
                if (colCount == 6 || rowCount == 6 || diaCount1 == 6 || diaCount2 == 6)
                {
                    return PUT_RESULT.ERROR;
                }
                else if (colCount == 5 || rowCount == 5 || diaCount1 == 5 || diaCount2 == 5)
                {
                    return PUT_RESULT.WIN;
                }

                var colCheck = BlackStoneCheck(row, col, 0, 1, stone);
                var rowCheck = BlackStoneCheck(row, col, 1, 0, stone);
                var diaCheck1 = BlackStoneCheck(row, col, 1, 1, stone);
                var diaCheck2 = BlackStoneCheck(row, col, 1, -1, stone);

                if ((colCheck.Item1 == LINE_STATE.OPEN && rowCheck.Item1 == LINE_STATE.OPEN 
                    && colCheck.Item2 == rowCheck.Item2 && (rowCheck.Item2 == 3 || rowCheck.Item2 == 4))
                    || (colCheck.Item1 == LINE_STATE.OPEN && diaCheck1.Item1 == LINE_STATE.OPEN
                    && colCheck.Item2 == diaCheck1.Item2 && (diaCheck1.Item2 == 3 || diaCheck1.Item2 == 4))
                    || (colCheck.Item1 == LINE_STATE.OPEN && diaCheck2.Item1 == LINE_STATE.OPEN
                    && colCheck.Item2 == diaCheck2.Item2 && (diaCheck2.Item2 == 3 || diaCheck2.Item2 == 4))
                    || (rowCheck.Item1 == LINE_STATE.OPEN && diaCheck1.Item1 == LINE_STATE.OPEN
                    && rowCheck.Item2 == diaCheck1.Item2 && (diaCheck1.Item2 == 3 || diaCheck1.Item2 == 4))
                    || (rowCheck.Item1 == LINE_STATE.OPEN && diaCheck2.Item1 == LINE_STATE.OPEN
                    && rowCheck.Item2 == diaCheck2.Item2 && (diaCheck2.Item2 == 3 || diaCheck2.Item2 == 4))
                    || (diaCheck1.Item1 == LINE_STATE.OPEN && diaCheck2.Item1 == LINE_STATE.OPEN
                    && diaCheck1.Item2 == diaCheck2.Item2 && (diaCheck2.Item2 == 3 || diaCheck2.Item2 == 4)))
                {
                    return PUT_RESULT.ERROR;
                }
            }

            return PUT_RESULT.NONE;
        }

        (LINE_STATE, int) BlackStoneCheck(int row, int col, int dRow, int dCol, STONE stone)
        {
            int myStoneCount = 1;
            int emptyCount = 0;

            LINE_STATE lineState1 = LINE_STATE.CLOSE;
            LINE_STATE lineState2 = LINE_STATE.CLOSE;

            STONE prevStone = stone;
            
            // check 33
            for (int i = 1; i <= 6; i++)
            {
                int r = row + i * dRow;
                int c = col + i * dCol;
                if (r < 0 || r >= _omokData.BoardSize() || c < 0 || c >= _omokData.BoardSize())
                {
                    continue;
                }
                if (_omokData.Board[r, c] == stone)
                {
                    myStoneCount++;
                }
                else if (_omokData.Board[r, c] == STONE.NONE)
                {
                    emptyCount++;
                    if (emptyCount >= 2)
                    {
                        emptyCount = 0;
                        lineState1 = LINE_STATE.OPEN;
                        prevStone = STONE.NONE;
                        break;
                    }
                }
                else
                {
                    if (prevStone == STONE.NONE)
                    {
                        lineState1 = LINE_STATE.HALF;
                    }
                    else if (prevStone == stone)
                    {
                        lineState1 = LINE_STATE.CLOSE;
                    }
                    emptyCount = 0;
                    prevStone = STONE.NONE;
                    break;
                }

                prevStone = _omokData.Board[r, c];
            }
            
            for (int i = -1; i >= -6; i--)
            {
                int r = row + i * dRow;
                int c = col + i * dCol;
                if (r < 0 || r >= _omokData.BoardSize() || c < 0 || c >= _omokData.BoardSize())
                {
                    continue;
                }
                if (_omokData.Board[r, c] == stone)
                {
                    myStoneCount++;
                }
                else if (_omokData.Board[r, c] == STONE.NONE)
                {
                    emptyCount++;
                    if (emptyCount >= 2)
                    {
                        emptyCount = 0;
                        lineState2 = LINE_STATE.OPEN;
                        break;
                    }
                }
                else
                {
                    if (prevStone == STONE.NONE)
                    {
                        lineState2 = LINE_STATE.HALF;
                    }
                    else if (prevStone == stone)
                    {
                        lineState2 = LINE_STATE.CLOSE;
                    }
                    emptyCount = 0;
                    break;
                }

                prevStone = _omokData.Board[r, c];
            }

            if (myStoneCount == 3)
            {
                if (lineState1 == LINE_STATE.HALF && lineState2 == LINE_STATE.HALF)
                {
                    return (LINE_STATE.CLOSE, 3);
                }
                else if (lineState1 == LINE_STATE.CLOSE || lineState2 == LINE_STATE.CLOSE)
                {
                    return (LINE_STATE.CLOSE, 3);
                }
                else
                {
                    return (LINE_STATE.OPEN, 3);
                }
            }
            else if (myStoneCount == 4)
            {
                // check 44
                for (int i = 0; i <= 6; i++)
                {
                    int r = row + i * dRow;
                    int c = col + i * dCol;
                    if (r < 0 || r >= _omokData.BoardSize() || c < 0 || c >= _omokData.BoardSize())
                    {
                        continue;
                    }
                    if (_omokData.Board[r, c] == stone)
                    {
                        myStoneCount++;
                    }
                    else if (_omokData.Board[r, c] == STONE.NONE)
                    {
                        emptyCount++;
                        if (emptyCount >= 2)
                        {
                            emptyCount = 0;
                            lineState1 = LINE_STATE.OPEN;
                            prevStone = STONE.NONE;
                            break;
                        }
                    }
                    else
                    {
                        if (prevStone == STONE.NONE)
                        {
                            lineState1 = LINE_STATE.HALF;
                        }
                        else if (prevStone == stone && emptyCount == 1)
                        {
                            lineState1 = LINE_STATE.HALF;
                        }
                        else if (prevStone == stone && emptyCount == 0)
                        {
                            lineState1 = LINE_STATE.CLOSE;
                        }
                        emptyCount = 0;
                        prevStone = STONE.NONE;
                        break;
                    }

                    prevStone = _omokData.Board[r, c];
                }

                for (int i = -1; i >= -6; i--)
                {
                    int r = row + i * dRow;
                    int c = col + i * dCol;
                    if (r < 0 || r >= _omokData.BoardSize() || c < 0 || c >= _omokData.BoardSize())
                    {
                        continue;
                    }
                    if (_omokData.Board[r, c] == stone)
                    {
                        myStoneCount++;
                    }
                    else if (_omokData.Board[r, c] == STONE.NONE)
                    {
                        emptyCount++;
                        if (emptyCount >= 2)
                        {
                            emptyCount = 0;
                            lineState2 = LINE_STATE.OPEN;
                            break;
                        }
                    }
                    else
                    {
                        if (prevStone == STONE.NONE)
                        {
                            lineState2 = LINE_STATE.HALF;
                        }
                        else if (prevStone == stone && emptyCount == 1)
                        {
                            lineState1 = LINE_STATE.HALF;
                        }
                        else if (prevStone == stone && emptyCount == 0)
                        {
                            lineState1 = LINE_STATE.CLOSE;
                        }
                        emptyCount = 0;
                        break;
                    }

                    prevStone = _omokData.Board[r, c];
                }

                if (lineState1 == LINE_STATE.CLOSE && lineState2 == LINE_STATE.CLOSE)
                {
                    return (LINE_STATE.CLOSE, 4);
                }
                else
                {
                    return (LINE_STATE.OPEN, 4);
                }
            }
            
            return (LINE_STATE.OPEN, myStoneCount);
        }

        int CheckLine(int row, int col, int dRow, int dCol, STONE stone)
        {
            int maxLinkCount = 0;
            int linkCount = 0;

            for (int i = -5; i <= 5; i++)
            {
                int r = row + i * dRow;
                int c = col + i * dCol;

                if (r < 0 || r >= _omokData.BoardSize() || c < 0 || c >= _omokData.BoardSize())
                {
                    continue;
                }

                if (i == 0 || (i == -5 && _omokData.Board[r, c] == stone))
                {
                    linkCount++;
                }
                else if (_omokData.Board[r, c] != stone)
                {
                    maxLinkCount = Math.Max(maxLinkCount, linkCount);
                    linkCount = 0;
                }
                else if (_omokData.Board[r, c] == stone)
                {
                    linkCount++;
                }
            }
            return maxLinkCount;
        }
    }
}
