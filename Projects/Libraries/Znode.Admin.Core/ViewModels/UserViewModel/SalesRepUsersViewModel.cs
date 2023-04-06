using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{

    public class SalesRepUsersViewModel : BaseViewModel
    {
        public int UserId { get; set; }
        public string AspNetUserId { get; set; }
        public string AspNetZnodeUserId { get; set; }
        public string Email { get; set; }
        public string StoreCode { get; set; }
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int? PortalId { get; set; }
    }
}

