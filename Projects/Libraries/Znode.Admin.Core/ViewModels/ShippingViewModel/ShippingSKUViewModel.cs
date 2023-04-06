using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingSKUViewModel : BaseViewModel
    {
        public int ShippingSKUId { get; set; }
        public int? ShippingRuleId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.SKURequiredMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SKU { get; set; }
        public int? ShippingTypeId { get; set; }
        public string ClassName { get; set; }
        /// <summary>
        /// this propert use for comma separated SKU names.
        /// </summary>
        public string SKUs { get; set; }
        public string ProductName { get; set; }
        public int ShippingId { get; set; }
    }
}