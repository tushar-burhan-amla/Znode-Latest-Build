using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ShippingListViewModel : BaseViewModel
    {
        public List<ShippingViewModel> ShippingList { get; set; }
        public GridModel GridModel { get; set; }
        public int SelectedShippingCode { get; set; }
        public bool IsQuote { get; set; }
        public int OrderID { get; set; }
        public int ShippingId { get ; set; }
        public int ProfileId { get; set; }
        public int PortalId { get; set; }
        public string ProfileName { get; set; }
        public string PortalName { get; set; }
        public string ShippingIds { get; set; }
        public int PromotionId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelAccountNumber, ResourceType = typeof(Admin_Resources))]
        public string AccountNumber { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingMethod, ResourceType = typeof(Admin_Resources))]
        public string ShippingMethod { get; set; }
        public decimal? CustomShippingCost { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelInHandsDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? InHandDate { get; set; }
        public IList<ShippingConstraintsViewModel> ShippingConstraints { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelShippingConstraintsCode, ResourceType = typeof(Admin_Resources))]
        public string ShippingConstraintCode { get; set; }
    }
}