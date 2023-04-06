using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class OrderPaymentCreateModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderStateIdRequired)]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed)]
        public int OmsOrderStateId { get; set; }        

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderIdRequired)]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed)]
        public int OmsOrderId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderDetailsIdRequired)]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed)]
        public int OmsOrderDetailsId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderStateRequired)]
        public string OrderState { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorTotalRequired)]
        public decimal Total { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderHistoryRequired)]
        public Dictionary<string, string> OrderHistory { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderLineItemHistoryRequired)]
        public Dictionary<string, OrderLineItemHistoryModel> OrderLineItemHistory { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOverDueAmountRequired)]
        public decimal OverDueAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingCostRequired)]
        public decimal ShippingCost { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingDifferenceRequired)]
        public decimal ShippingDifference { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorReturnItemListRequired)]
        public ReturnOrderLineItemListModel ReturnItemList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorOrderLineItemsRequired)]
        public List<OrderLineItemModel> OrderLineItems { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShoppingCartOverDueAmountRequired)]
        public decimal ShoppingCartOverDueAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorBillingAddressIdRequired)]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed)]
        public int BillingAddressId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShippingAddressIdRequired)]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed)]
        public int ShippingAddressId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorShoppingCartItemsCountRequired)]
        public int? ShoppingCartItemsCount { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorIsAllowedTerritoriesRequired)]
        public bool IsAllowedTerritories { get; set; }
    }
}
