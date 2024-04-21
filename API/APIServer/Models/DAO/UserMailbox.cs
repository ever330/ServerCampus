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
        [Column("mail_name")]
        public string MailName { get; set; } = null!;
        [Column("mail_content")]
        public string MailContent { get; set; } = null!;
        [Column("reward")]
        public int Reward { get; set; }
    }
}
