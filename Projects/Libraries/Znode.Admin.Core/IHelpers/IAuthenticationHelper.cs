using System.Web.Mvc;

namespace Znode.Engine.Admin.Helpers
{
    public interface IAuthenticationHelper
    {

        //Set authentication cookied for the logged in user
        void SetAuthCookie(string userName, bool createPersistantCookie);

        //Redirect to login view in case user is not authenticate.
        void RedirectFromLoginPage(string userName, bool createPersistantCookie);

        //Overloaded method for Authorize attribute, user to authenticate & authorize the user for each action.
        void OnAuthorization(AuthorizationContext filterContext);

        //Method Used to Authenticate the user.
        void AuthenticateUser(AuthorizationContext filterContext);

    }
}
