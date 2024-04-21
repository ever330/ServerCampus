namespace APIServer.Models
{
    public class ResGetMail
    {
        public ErrorCode Result { get; set; }
        public List<Mail>? Mails { get; set; }
    }

    public class Mail
    {
        public string MailName { get; set; } = null!;
        public string MailContent { get; set; } = null!;
        public int Reward { get; set; }
    }
}
