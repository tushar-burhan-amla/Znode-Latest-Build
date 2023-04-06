namespace Znode.Engine.Api.Models
{
    public class StockNoticeSettingsModel : BaseModel
    {
        public string DeleteAlreadySentEmails { get; set; }
        public string DeletePendingEmails { get; set; }
    }
}
