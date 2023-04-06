using System;
using System.Configuration;
using Znode.Engine.Admin.Helpers;

namespace Znode.Engine.Admin
{
    public class AjaxHeadersModel
    {
        public string Authorization { get; set; } 
        public int ZnodeAccountId { get; set; } = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
        public string DomainName { get; set; } = ConfigurationManager.AppSettings["ZnodeAdminUri"];
        public string ApiUrl { get; set; } = ConfigurationManager.AppSettings["ZnodeApiRootUri"];
        public string Token { get; set; }
    }
}