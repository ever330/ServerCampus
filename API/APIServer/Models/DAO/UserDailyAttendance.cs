using System.ComponentModel.DataAnnotations.Schema;

namespace APIServer.Models.DAO
{
    [Table("user_daily_attendance")]
    public class UserDailyAttendance
    {
        [Column("attendanceDate")]
        public DateTime AttendanceDate { get; set; }
        [Column("consecutiveAttendance")]
        public int ConsecutiveAttendance { get; set; }
    }
}
