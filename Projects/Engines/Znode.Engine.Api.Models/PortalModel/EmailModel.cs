namespace Znode.Engine.Api.Models
{
    public  class EmailModel : BaseModel
    {    
        public int PortalId { get; set; }
        public string ToEmailAddress { get; set; }
        public string CcEmailAddress { get; set; }
        public string BccEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
    }
}
