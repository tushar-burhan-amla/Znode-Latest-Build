using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceListViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public int PriceListId { get; set; }
        public int PriceListUserId { get; set; }
        public int PriceListAccountId { get; set; }
        public List<PriceViewModel> PriceList { get; set; }
        public int ProfileId { get; set; }
        public string Profile { get; set; }
        public string PortalName { get; set; }

        [Display(Name = Znode.Libraries.Resources.ZnodeAdmin_Resources.LabelSelectProfile, ResourceType = typeof(Znode.Libraries.Resources.Admin_Resources))]
        public List<SelectListItem> Profiles { get; set; }
        public GridModel GridModel { get; set; }
        public bool HasParentAccounts { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
    }
}