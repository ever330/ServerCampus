using System.ComponentModel.DataAnnotations.Schema;

namespace APIServer.Models.DAO
{
    [Table("user_daily_attendance")]
    public class UserDailyAttendance
    {
        [Column("attendance_date")]
        public DateTime AttendanceDate { get; set; }
        [Column("consecutive_attendance")]
        public int ConsecutiveAttendance { get; set; }
    }
}
