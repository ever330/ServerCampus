using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Models
{
    public class ReqLoginToAPI
    {
        public string Email { get; set; }
        public string AuthToken { get; set; }
    }
}
