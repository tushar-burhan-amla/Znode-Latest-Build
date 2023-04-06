using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class UserServiceV2 : UserService, IUserServiceV2
    {
        #region Private Variables
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationRoleManager _roleManager;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodeUserProfile> _accountProfileRepository;
        private readonly IZnodeRepository<ZnodeUserPortal> _userPortalRepository;
        private readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodeUserAddress> _userAddressRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        #endregion

        #region Public Constructors
        public UserServiceV2() : base()
        {
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _accountProfileRepository = new ZnodeRepository<ZnodeUserProfile>();
            _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _userAddressRepository = new ZnodeRepository<ZnodeUserAddress>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
        }

        public UserServiceV2(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        #region Public Properties
        //Get value of ApplicationSignInManager which is used for the application.
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //Get value of ApplicationUserManager which is used for the application.
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //Get value of ApplicationRoleManager which is used for the application.
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        public virtual bool UpdateUserDataV2(UpdateUserModelV2 userModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNull(userModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            ZnodeLogging.LogMessage("Input parameter UserId of userModel: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { UserId = userModel?.UserId });

            UserModel model = UserV2Map.ToUserModelUpdate(userModel);

            if (model.UserId > 0)
            {
                ZnodeUser usr = _userRepository.GetById(model.UserId);
                model.AspNetUserId = usr?.AspNetUserId;

                //Update roles in AspNetUserRoles table.
                List<string> roles = UserManager.GetRoles(model.AspNetUserId).ToList();
                ZnodeLogging.LogMessage("Roles list count: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roles?.Count);

                bool updatedAccount = UpdateUser(model, true);
                ZnodeLogging.LogMessage(updatedAccount ? Admin_Resources.SuccessUpdateData : Admin_Resources.ErrorUpdateData, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                //Get user details by UserId.
                ApplicationUser user = UserManager.FindById(model.AspNetUserId);

                //Update email in AspNetUsers table.
                if (!Equals(user?.Email, model.Email))
                    user.Email = model.Email;

                if (updatedAccount)
                {
                    IdentityResult applicationUserManager = UserManager.Update(user);
                    ZnodeLogging.LogMessage(applicationUserManager.Succeeded ? Admin_Resources.SuccessUpdateAccount : Admin_Resources.ErrorUpdateAccount, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return true;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }

        public virtual CreateUserModelV2 CreateCustomerV2(CreateUserModelV2 model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.UserModelNotNull);

            ZnodeLogging.LogMessage("PortalId, UserName, AccountId, ProfileId properties of CreateUserModelV2: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.PortalId, model?.UserName, model?.AccountId, model?.ProfileId });

            ZnodePortal portalInfo = _portalRepository.GetById(model.PortalId);
            if (IsNull(portalInfo))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidPortalId);

            UserModel userModel = UserV2Map.ToUserModel(model);
            userModel = CreateCustomer(model.PortalId, userModel);
            userModel.PortalId = model.PortalId;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return UserV2Map.ToCreateUserModel(userModel);
        }

        public virtual GuestUserModelV2 CreateGuestUserV2(GuestUserModelV2 model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter ProfileId of GuestUserModelV2: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { ProfileId = model?.ProfileId});

            ZnodeProfile profileInfo = _profileRepository.GetById(model.ProfileId);
            if (IsNull(profileInfo))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidProfileId);

            //Save account details.
            ZnodeUser account = _userRepository.Insert(model?.ToEntity<ZnodeUser>());
            if (account?.UserId > 0)
            {
                model.UserId = account.UserId;
                ZnodeLogging.LogMessage(account.UserId > 0 ? Admin_Resources.SuccessUserDataSave : Admin_Resources.ErrorUserDataSave, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                //Insert portal in UserPortal table.
                _userPortalRepository.Insert(new ZnodeUserPortal { UserId = account.UserId, PortalId = model.PortalId });

                //Insert profile in AccountProfile table.
                _accountProfileRepository.Insert(new ZnodeUserProfile { UserId = account.UserId, ProfileId = model.ProfileId, IsDefault = true });

                //Add the addresses in the Znode Address table.
                ZnodeAddress billingAddress = model?.BillingAddress?.ToEntity<ZnodeAddress>();
                if(IsNotNull(billingAddress))
                    billingAddress.IsDefaultBilling = true;
                ZnodeAddress billAddress = _addressRepository.Insert(billingAddress);

                ZnodeAddress shippingAddress = model?.ShippingAddress?.ToEntity<ZnodeAddress>();
                if (IsNotNull(shippingAddress))
                    shippingAddress.IsDefaultShipping = true;
                ZnodeAddress shipAddress = _addressRepository.Insert(shippingAddress);

                //Add the entry in the ZnodeUserAddress table for the above userids
                ZnodeUserAddress userBillingAddress = new ZnodeUserAddress();
                userBillingAddress.AddressId = billAddress.AddressId;
                userBillingAddress.UserId = account.UserId;

                ZnodeUserAddress userShippingAddress = new ZnodeUserAddress();
                userShippingAddress.AddressId = shipAddress.AddressId;
                userShippingAddress.UserId = account.UserId;

                _userAddressRepository.Insert(userBillingAddress);
                _userAddressRepository.Insert(userShippingAddress);

                ZnodeLogging.LogMessage("billAddressAddressId, shipAddressAddressId and account.UserId: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { billAddress?.AddressId, shipAddress?.AddressId, account?.UserId });
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return UserV2Map.ToGuestUserModel(billAddress.AddressId, shipAddress.AddressId, account);

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }
    }
}
