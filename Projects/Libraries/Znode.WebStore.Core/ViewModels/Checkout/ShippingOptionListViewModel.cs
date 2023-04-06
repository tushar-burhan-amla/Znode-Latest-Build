using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ShippingOptionListViewModel : BaseViewModel
    {
        public List<ShippingOptionViewModel> ShippingOptions { get; set; }
        public bool IsB2BUser { get; set; }
        public int OmsQuoteId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string AccountNumber { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingMethod, ResourceType = typeof(Admin_Resources))]
        public string ShippingMethod { get; set; }
        public string CustomerServicePhone { get; set; }
        public ShippingOptionListViewModel()
        {
            ShippingOptions = new List<ShippingOptionViewModel>();
        }
    }
}