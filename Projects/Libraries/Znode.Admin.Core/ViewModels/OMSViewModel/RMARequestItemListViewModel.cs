using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMARequestItemListViewModel : BaseViewModel
    {
        public int RMARequestId { get; set; }
        public string RequestNumber { get; set; }
        public string RequestDate { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string flag { get; set; }
        public string Comments { get; set; }
        public List<RMARequestItemViewModel> RMARequestItemList { get; set; }
        public List<SelectListItem> ReasonForReturnItems { get; set; }
        public IssuedGiftCardListViewModel GiftCardsIssued { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string CustomerName { get; set; }
        public string StoreName { get; set; }
        public string SalesPhoneNumber { get; set; }
        public string CurrencyCode { get; set; }
        public MvcHtmlString ViewHtml { get; set; }
        public string RequestCode { get; set; }
        public int PortalId { get; set; }
        public int OMSOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CultureCode { get; set; }
    }
}