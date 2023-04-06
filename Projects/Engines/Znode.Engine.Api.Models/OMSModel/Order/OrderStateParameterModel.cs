using System;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class OrderStateParameterModel : BaseModel
    {
        public int OmsOrderId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public int OmsOrderStateId { get; set; }
        public string TrackingNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelOrderState, ResourceType = typeof(Admin_Resources))]
        public string OrderState { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPaymentStatus, ResourceType = typeof(Admin_Resources))]
        public string PaymentStatus { get; set; }
        public decimal Total { get; set; }
        public Nullable<decimal> ShippingHandlingCharges { get; set; }
        public Nullable<decimal> ReturnCharges { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdditionalNotes { get; set; }
    }
}
