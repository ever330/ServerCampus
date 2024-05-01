using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIServer.Models.DAO
{
    [Table("user_mailbox")]
    public class UserMailbox
    {
        [Key]
        [Column("uid")]
        public int Uid { get; set; }
        [Column("mailName")]
        public string MailName { get; set; } = null!;
        [Column("mailContent")]
        public string MailContent { get; set; } = null!;
        [Column("reward")]
        public int Reward { get; set; }
    }
}
