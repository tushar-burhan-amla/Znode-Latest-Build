using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net.Http;
using System.Net;
using System.Web.Security;
using Znode.Engine.WebStore.Agents;
using System.Linq;

namespace Znode.Engine.WebStore
{
    public interface IAuthenticationHelper
    {
        //Set Authorization cookie for the logged in user
         void SetAuthCookie(string userName, bool createPersistantCookie);

        //Redirect to login view in case user is not authenticate.
         void RedirectFromLoginPage(string userName, bool createPersistantCookie);


        //Overloaded method for Authorize attribute, user to authenticate & authorize the user for each action.
        void OnAuthorization(AuthorizationContext filterContext);

        //Method Used to Authenticate the user.
        void AuthenticateUser(AuthorizationContext filterContext);

        //Handle the Unauthorized Request.
        // This method is used to check whether authentication is mandatory or not for the current portal(Login Required Flag set from store setting).
        string IsAuthorizationMandatory();

        //Check action is annotate with AuthorizeAttribute
        bool IsAuthorizeAttribute(AuthorizationContext filterContext);
    }
}
