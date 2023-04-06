using Microsoft.AspNet.Identity.Owin;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class SocialLoginModel
    {
        public ExternalLoginInfo LoginInfo { get; set; }
        public bool IsPersistent { get; set; }
        public int PortalId { get; set; }
        public string UserName { get; set; }
        public UserVerificationTypeEnum UserVerificationTypeCode { get; set; }
    }
}
