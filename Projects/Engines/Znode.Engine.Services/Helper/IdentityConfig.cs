using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        //Create new user manager per request.
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            //Configuration for validating UserNames of User accounts.
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };            
            //Configuration for validating password of User accounts.
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireDigit = true,                
            };

            //Configuration for Maximum attempt of Invalid password set from web config.
            manager.MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(ZnodeApiSettings.MaxInvalidPasswordAttempts);
            manager.UserLockoutEnabledByDefault = true;
            //Default TimeSpan for account to be locked out.
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(36135);
            var dataProtectionProvider = options.DataProtectionProvider;
            if (!Equals(dataProtectionProvider, null))
            {
                //Create new Token provider per request.
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Default TimeSpan for password token to be expired.
                    //To Do time span
                    TokenLifespan = (string.IsNullOrEmpty(ZnodeApiSettings.ResetPasswordLinkExpirationDuration)) ? TimeSpan.FromDays(1) : TimeSpan.FromHours(Convert.ToInt32(ZnodeApiSettings.ResetPasswordLinkExpirationDuration))
                };
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user) => user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);

        //Create new signIn manager per request.
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context) => new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);

    }

    // Configure the application role manager which is used in this application.
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store)
            : base(store)
        {
        }

        //Create new Role manager per request.
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>());
            return new ApplicationRoleManager(roleStore);
        }
    }
}
