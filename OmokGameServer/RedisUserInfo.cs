using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RedisUserInfo
    {
        public string Id { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
    }
}
