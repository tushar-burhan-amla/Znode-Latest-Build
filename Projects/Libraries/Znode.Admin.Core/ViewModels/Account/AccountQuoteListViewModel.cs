﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AccountQuoteListViewModel : BaseViewModel
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HeaderOrderStatus, ResourceType = typeof(Admin_Resources))]
        public int OmsOrderStateId { get; set; }

        public List<AccountQuoteViewModel> AccountQuotes { get; set; }
        public List<SelectListItem> OrderStatusList { get; set; }
        public GridModel GridModel { get; set; }

        public bool HasParentAccounts { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string UpdatePageType { get; set; }
    }
}