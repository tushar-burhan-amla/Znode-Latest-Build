using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IUserCache
    {
        /// <summary>
        /// Get User account data.
        /// </summary>
        /// <param name="userId">user Id to get account details.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <param name="portalId">portalId.</param>
        /// <returns>Returns account.</returns>
        string GetUser(int userId, string routeUri, string routeTemplate, int portalId = 0);

        /// <summary>
        /// This method will get the account details by user name
        /// </summary>
        /// <param name="username">string User Name</param>
        /// <param name="portalId">Portal id.</param>
        /// <param name="routeUri">string Route URI</param>
        /// <param name="routeTemplate">string Route Template</param>
        /// <returns>Returns the account details</returns>
        string GetUserByUsername(string username, int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get user account list.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <param name="columnList">List of column to display on grid</param>
        /// <returns>Returns user account list.</returns>
        string GetUserList(int loggedUserAccountId, string routeUri, string routeTemplate, string columnList ="");

        /// <summary>
        /// Get user account list.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="userName">userName</param>   
        /// <param name="roleName">roleName</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <param name="columnList">List of column to display on grid</param>
        /// <returns>Returns user account list.</returns>
        string GetUserListForAdmin(int loggedUserAccountId, string routeUri, string routeTemplate, string userName, string roleName, string columnList ="" );


        /// <summary>
        /// Gets the assigned portals to user.
        /// </summary>
        /// <param name="aspNetUserId">User id.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns assigned portals to user.</returns>
        string GetPortalIds(string aspNetUserId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the List of Sales Rep
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
       string GetSalesRepForAssociation(string routeUri, string routeTemplate);

        /// <summary>
        /// This method will useful for the login purpose.
        /// </summary>
        /// <param name="portalId">int Portal Id</param>
        /// <param name="model">UserModel model</param>
        /// <param name="errorCode">out error code</param>
        /// <returns></returns>
        UserModel Login(int portalId, UserModel model, out int? errorCode);

        /// <summary>
        /// Get UnAssociated Customer(s).
        /// </summary>
        /// <param name="portalId">int Portal Id</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>        
        /// <returns>Returns customer list</returns>
        string GetUnAssociatedCustomerList(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the List of Sales Rep
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetSalesRepForAccount(string routeUri, string routeTemplate);

        /// <summary>
        /// Get user account details.
        /// </summary>
        /// <param name="userId">customer userId</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns user account details.</returns>
        string GetCustomerAccountdetails(int userId, string routeUri, string routeTemplate);

        /// <summary>
        /// Remove all registration user attempt details.
        /// </summary>
        /// <returns> Return boolean status </returns>
        bool RemoveUserRegistrationAttemptDetail();

        /// <summary>
        /// Get User Detail By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        string GetUserDetailById(int userId, string routeUri, string routeTemplate, int portalId);
    }
}
