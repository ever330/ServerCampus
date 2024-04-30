using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok
{
    public class UserInfo
    {
        public string Email { get; set; }
        public string AuthToken { get; set; }
        public string Id { get; set; }

        public int Level { get; set; }
        public int Exp { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public int Money { get; set; }
    }
}
