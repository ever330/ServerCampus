using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Models
{
    public class ReqGetMail
    {
        public string Email { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
    }
}
