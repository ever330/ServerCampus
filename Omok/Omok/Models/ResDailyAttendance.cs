using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Models
{
    public class ResDailyAttendance
    {
        public ErrorCode Result { get; set; }
        public int ConsecutiveAttendance { get; set; }
    }
}
