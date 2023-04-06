namespace Znode.Engine.Api.Models
{
    public class SMTPModel : BaseModel
    {
        public int SmtpId { get; set; }
        public int PortalId { get; set; }
        public int? SmtpPort { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromDisplayName { get; set; }
        public string FromEmailAddress { get; set; }
        public string BccEmailAddress { get; set; }
        public string PortalName { get; set; }
        public bool EnableSslForSmtp { get; set; }
        public bool DisableAllEmailsForSmtp { get; set; }
    }
}
