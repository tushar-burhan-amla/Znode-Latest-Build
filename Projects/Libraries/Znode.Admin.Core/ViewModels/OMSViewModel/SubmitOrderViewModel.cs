using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class SubmitOrderViewModel : BaseViewModel
    {
        public int OrderId { get; set; }
        [AllowHtml]
        public string ReceiptHtml { get; set; }
        public bool IsEmailSend { get; set; }
    }
}