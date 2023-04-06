using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SavedCartLineItemModel : BaseModel
    {
        public int OmsSavedCartLineItemId { get; set; }
        public int ParentOmsSavedCartLineItemId { get; set; }
        public int OmsSavedCartId { get; set; }
        public string SKU { get; set; }
        public decimal Quantity { get; set; }
        public decimal AddOnQuantity { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public string CustomText { get; set; }
        public string CartAddOnDetails { get; set; }
        public string AddonProducts { get; set; }
        public string BundleProducts { get; set; }
        public string SimpleProducts { get; set; }
        public string ConfigurableProducts { get; set; }
        public string GroupProducts { get; set; }
        public string PersonaliseValuesList { get; set; }

        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }
        public int? OmsOrderId { get; set; }
        public string AutoAddon { get; set; }
        public string GroupId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? CustomUnitPrice { get; set; }
    }
}
