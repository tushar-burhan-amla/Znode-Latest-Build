using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.ViewModels
{
    public class SendInvoiceViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public string ReceiptHtml { get; set; }
        public string ReceiverEmail { get; set; }
        public string OrderNumber { get; set; }
    }
}
