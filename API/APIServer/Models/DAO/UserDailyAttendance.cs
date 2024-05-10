using System.ComponentModel.DataAnnotations.Schema;

namespace APIServer.Models.DAO
{
    [Table("userDailyAttendance")]
    public class UserDailyAttendance
    {
        [Column("attendanceDate")]
        public DateTime AttendanceDate { get; set; }
        [Column("consecutiveAttendance")]
        public int ConsecutiveAttendance { get; set; }
    }
}
