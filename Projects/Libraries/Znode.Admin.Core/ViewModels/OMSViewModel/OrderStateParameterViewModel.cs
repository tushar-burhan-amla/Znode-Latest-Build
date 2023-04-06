using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderStateParameterViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelOrderId, ResourceType = typeof(Admin_Resources))]
        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public int OmsOrderStateId { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string TrackingNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelOrderState, ResourceType = typeof(Admin_Resources))]
        public string OrderState { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPaymentStatus, ResourceType = typeof(Admin_Resources))]
        public string PaymentStatus { get; set; }
        public decimal Total { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectListItem> OrderStatusList { get; set; }

        public string AdditionalNotes { get; set; }
        public string ShippingTypeName { get; set; }
        public string UserName { get; set; }
        public string UpdatePageType { get; set; }
        public string CurrencyCode { get; set; }
        public string OrderNumber { get; set; }
        public string ExternalId { get; set; }
    }
}