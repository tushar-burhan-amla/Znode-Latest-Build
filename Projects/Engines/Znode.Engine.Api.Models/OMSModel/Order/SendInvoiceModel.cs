namespace Znode.Engine.Api.Models
{
    public class SendInvoiceModel : BaseModel
    {
        public string ReceiptHtml { get; set; }
        public int PortalId { get; set; }
        public string ReceiverEmail { get; set; }
        public string OrderNumber { get; set; }
    }
}