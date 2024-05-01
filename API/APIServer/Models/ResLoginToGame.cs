using APIServer.Models.DAO;

namespace APIServer.Models
{
    public class ResLoginToGame
    {
        public ERROR_CODE Result { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public int Money { get; set; }
    }
}
