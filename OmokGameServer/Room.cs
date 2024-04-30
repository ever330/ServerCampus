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

    public class Room
    {
        int _roomNumber;
        List<User> _userList = new List<User>();

        int _roomUserMaxCount = 0;

        const int BOARD_SIZE = 19;
        STONE[,] _board = new STONE[BOARD_SIZE, BOARD_SIZE];

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
                    _board[x, y] = STONE.NONE;
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

        public void PutStone(STONE stone, int x, int y)
        {
            _board[x, y] = stone;
        }

        public PUT_RESULT CheckStoneCount(STONE stone, int row, int col)
        {
            int colCount = CheckLine(row, col, 0, 1, stone);
            int rowCount = CheckLine(row, col, 1, 0, stone);
            int diaCount1 = CheckLine(row, col, 1, 1, stone);
            int diaCount2 = CheckLine(row, col, 1, -1, stone);

            if (colCount == 5 || rowCount == 5 || diaCount1 == 5 || diaCount2 == 5)
            {
                return PUT_RESULT.WIN;
            }

            if (stone == STONE.BLACK)
            {
                if (colCount == 6 || rowCount == 6 || diaCount1 == 6 || diaCount2 == 6)
                {
                    return PUT_RESULT.ERROR;
                }
                else if ((colCount == 4 && rowCount == 4) || (colCount == 4 && diaCount1 == 4) ||
                        (colCount == 4 && diaCount2 == 4) || (rowCount == 4 && diaCount1 == 4) ||
                        (rowCount == 4 && diaCount2 == 4) || (diaCount1 == 4 && diaCount2 == 4))
                {
                    return PUT_RESULT.ERROR;
                }
                else if ((colCount == 3 && rowCount == 3) || (colCount == 3 && diaCount1 == 3) ||
                        (colCount == 3 && diaCount2 == 3) || (rowCount == 3 && diaCount1 == 3) ||
                        (rowCount == 3 && diaCount2 == 3) || (diaCount1 == 3 && diaCount2 == 3))
                {
                    return PUT_RESULT.ERROR;
                }
            }

            return PUT_RESULT.NONE;
        }

        int CheckLine(int row, int col, int dRow, int dCol, STONE stone)
        {
            int maxLinkCount = 0;
            int linkCount = 0;
            STONE prevStone = 0;

            for (int i = -6; i <= 6; i++)
            {
                int r = row + i * dRow;
                int c = col + i * dCol;

                if (r < 0 || r >= BOARD_SIZE || c < 0 || c >= BOARD_SIZE)
                {
                    continue;
                }

                if (i == 0 || (i == -6 && _board[r, c] == stone))
                {
                    linkCount++;
                }
                else if (_board[r, c] != STONE.NONE && _board[r, c] != stone)
                {
                    maxLinkCount = Math.Max(maxLinkCount, linkCount);
                    linkCount = 0;
                }
                else if (_board[r, c] == stone)
                {
                    linkCount++;
                }
                else if (prevStone == STONE.NONE && _board[r, c] == STONE.NONE)
                {
                    maxLinkCount = Math.Max(maxLinkCount, linkCount);
                    linkCount = 0;
                }

                prevStone = _board[r, c];
            }

            return maxLinkCount;
        }
    }
}
