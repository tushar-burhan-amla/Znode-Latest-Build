using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services
{
    public interface IUserLoginHelper
    {
        /// <summary>
        /// Check whether the user already exists or not.
        /// </summary>
        /// <param name="username">username for the user.</param>
        /// <param name="portalId">portal Id for the user.</param>
        /// <returns>Return true or false.</returns>
        bool IsUserExists(string username, int portalId);

        /// <summary>
        /// Insert new user details.
        /// </summary>
        /// <param name="model">Model having the user related details.</param>
        /// <returns>Returns the newly created user details.</returns>
        AspNetZnodeUser CreateUser(UserModel model);

        /// <summary>
        /// Get the Znode user details.
        /// </summary>
        /// <param name="model">Model having user requested details.</param>
        /// <param name="portalId">portal Id for the user.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetUser(UserModel model, int? portalId = 0);

        /// <summary>
        /// Get the Znode user details by userId.
        /// </summary>
        /// <param name="userId">userId for the user.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetUserById(string userId);

        /// <summary>
        /// Get the Znode user details.
        /// </summary>
        /// <param name="username">username for the user.</param>
        /// <param name="portalId">portal Id for the user.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetUserInfo(string username, int? portalId);

        /// <summary>
        /// Check whether the user belongs to an account or not.
        /// </summary>
        /// <param name="model">Model having requested user details.</param>
        /// <returns>Return true or false.</returns>
        bool IsAccountCustomer(UserModel model);

        /// <summary>
        /// Get the Znode user details.
        /// </summary>
        /// <param name="AspNetUserId">AspNetUserId for the user.</param>
        /// <param name="portalId">portal Id for the user.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetUserInfoByAspNetUserId(string AspNetUserId, int? portalId);

        /// <summary>
        /// Get the Znode user details.
        /// </summary>
        /// <param name="model">Model having requested user details.</param>
        /// <param name="portalId">portal Id for the user.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetUserInfoByAspNetUserId(UserModel model, int? portalId = 0);

        /// <summary>
        /// Get user if the user exists by AspNetUserId.
        /// </summary>
        /// <param name="model">Model having requested user details.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetIsExistsByAspNetUserId(UserModel userModel);

        /// <summary>
        /// Get user list for same username associated with different portals.
        /// </summary>
        /// <param name="username">username to get user list.</param>
        /// <returns>Returns the requested user list.</returns>
        List<AspNetZnodeUser> GetUserListByUsername(string username);

        /// <summary>
        /// Get the Znode user details.
        /// </summary>
        /// <param name="userModel">userModel for the user.</param>
        /// <param name="portalId">portal Id for the user.</param>
        /// <returns>Returns the requested user details.</returns>
        AspNetZnodeUser GetUserDetails(UserModel userModel, int? portalId);
    }
}
