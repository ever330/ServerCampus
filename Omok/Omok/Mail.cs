using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Omok
{
    public class Mail
    {
        public string MailName { get; set; } = null!;
        public string MailContent { get; set; } = null!;
        public int Reward { get; set; }


        public override string ToString()
        {
            return MailName;
        }
    }
}
