using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Models
{
    public class ResGetMail
    {
        public ErrorCode Result { get; set; }
        public List<Mail>? Mails { get; set; }
    }
}
