using System.ComponentModel.DataAnnotations.Schema;

namespace HiveServer.Models.DAO
{
    [Table("hive_users")]
    public class AccountInfo
    {
        [Column("email")]
        public string Email { get; set; } = null!;
        [Column("password")]
        public string Password { get; set; } = null!;
        [Column("salt")]
        public string Salt { get; set; } = null!;
    }
}
