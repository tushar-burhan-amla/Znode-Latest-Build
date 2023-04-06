using Znode.Engine.Api.Models.V2;

namespace Znode.Engine.Services
{
    public interface IUserServiceV2 : IUserService
    {
        /// <summary>
        /// Create the user account
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="model">CreateUserModelV2</param>
        /// <returns>CreateUserModelV2</returns>
        CreateUserModelV2 CreateCustomerV2(CreateUserModelV2 model);

        /// <summary>
        /// Update user account details.
        /// </summary>
        /// <param name="accountModel">User model.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateUserDataV2(UpdateUserModelV2 userModel);

        /// <summary>
        /// Creates the guest user at the time of checkout as a guest.
        /// </summary>
        /// <param name="model">GuestUserModelV2</param>
        /// <returns>GuestUserModelV2</returns>
        GuestUserModelV2 CreateGuestUserV2(GuestUserModelV2 model);
    }
}
