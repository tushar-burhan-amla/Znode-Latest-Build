using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderInfoViewModel
    {
        public int OmsOrderId { get; set; }
        public int OmsOrderDetailId { get; set; }
        public int ShippingTypeId { get; set; }
        public int PaymentTypeId { get; set; }
        public string CreatedByName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelOrderNumber, ResourceType = typeof(Admin_Resources))]
        public string OrderNumber { get; set; }
        public string StoreName { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string CreditCardNumber { get; set; }
        public string TransactionId { get; set; }
        public string TrackingNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDisplayName { get; set; }
        public string ShippingType { get; set; }
        public int userId { get; set; }
        public int PortalId { get; set; }
        public string ShippingTypeDescription { get; set; }
        public string OrderDateWithTime { get; set; }
        public string ShippingTrackingUrl { get; set; }
        public string TaxTransactionNumber { get; set; }
        public string ShippingCode { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string AccountNumber { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingMethod, ResourceType = typeof(Admin_Resources))]
        public string ShippingMethod { get; set; }
        public string ShippingTypeClassName { get; set; }

        public string ExternalId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelJobName, ResourceType = typeof(Admin_Resources))]
        public string JobName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingConstraintsCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingConstraintCode { get; set; }
        public IList<ShippingConstraintsViewModel> ShippingConstraints { get; set; }

        public int ShippingId { get; set; }
        public string RoutingNumberAccountNumber { get; set; }
    }
}
