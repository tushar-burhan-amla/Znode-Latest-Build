using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using System.Text;
namespace Znode.Engine.WebStore
{
    public class AjaxHeadersModel
    {
         public string Authorization { get; set; }
        public int ZnodeAccountId { get; set; } = (SessionHelper.GetDataFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId).GetValueOrDefault();

        public string DomainName { get; set; } = HttpContext.Current.Request.Url.Authority.Trim();

        public string ApiUrl { get; set; } = ZnodeWebstoreSettings.ZnodeApiRootUri;

        public string Token { get; set; }
    }
}