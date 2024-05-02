using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class OmokData
    {
        const int BOARD_SIZE = 19;
        public STONE[,] Board;

        public void Init()
        {
            Board = new STONE[BOARD_SIZE, BOARD_SIZE];
        }

        public int BoardSize()
        {
            return BOARD_SIZE;
        }
    }
}
